using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces;
using AutoMapper;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureLayer.Services
{
    /// <summary>
    /// Service responsible for managing scheduled care tasks for plants.
    /// Orchestrates both database persistence and Hangfire background job scheduling.
    /// </summary>
    public class PlantScheduleService : IPlantScheduleService
    {
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IPlantRepository _plantRepository;
        private readonly IPlantScheduleRepository _plantScheduleRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes the service with required repositories and the Hangfire job manager.
        /// </summary>
        public PlantScheduleService(IRecurringJobManager recurringJobManager, 
            IPlantRepository plantRepository, IPlantScheduleRepository plantScheduleRepository,IMapper mapper)
        {
            _recurringJobManager = recurringJobManager;
            _plantRepository = plantRepository;
            _plantScheduleRepository = plantScheduleRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates a new plant care schedule and registers it as a recurring background job.
        /// </summary>
        /// <param name="PlantId">The unique identifier of the plant.</param>
        /// <param name="dto">Schedule details including frequency (Daily/Weekly), time, and task type.</param>
        public async Task AddPlantScheduleAsync(Guid PlantId, CreateScheduleDTO dto)
        {
            var plant = await _plantRepository.GetPlantById(PlantId);
            if (plant == null)
                throw new KeyNotFoundException("The plant was not found.");

            string cronExp = GenerateCronExpression(dto);

            var schedule = new PlantSchedule
            {
                PlantId = PlantId,
                TaskType = dto.TaskType,
                Frequency = dto.Frequency,
                DaysOfWeek = dto.Days != null ? string.Join(",", dto.Days) : null,
                Hour = dto.Hour,
                Minute = dto.Minute,
                CronExpression = cronExp,
                IsActive = true
            };

            // Save to DB first to ensure we have a persistent record
            await _plantScheduleRepository.AddPlantScheduleAsync(schedule);

            // Register the task in Hangfire with a unique ID based on Task and Plant
            string jobId = GetJobId(schedule.TaskType, PlantId);
            _recurringJobManager.AddOrUpdate<IPlantNotificationJob>(
                jobId,
                job => job.ExecuteNotification(PlantId, schedule.TaskType),
                cronExp
            );
        }

        /// <summary>
        /// Updates an existing schedule and refreshes the associated Hangfire background job.
        /// </summary>
        public async Task UpdatePlantScheduleAsync(Guid scheduleId, CreateScheduleDTO dto)
        {
            var existingSchedule = await _plantScheduleRepository.GetPlantScheduleByIdAsync(scheduleId);
            if (existingSchedule == null)
                throw new KeyNotFoundException("Schedule not found");

            string newCronExp = GenerateCronExpression(dto);

            // Sync Database record
            existingSchedule.TaskType = dto.TaskType;
            existingSchedule.Frequency = dto.Frequency;
            existingSchedule.Hour = dto.Hour;
            existingSchedule.Minute = dto.Minute;
            existingSchedule.DaysOfWeek = dto.Days != null ? string.Join(",", dto.Days) : null;
            existingSchedule.CronExpression = newCronExp;
            await _plantScheduleRepository.UpdatePlantScheduleAsync(existingSchedule);

            // Sync Hangfire (AddOrUpdate will overwrite the existing job with this ID)
            string jobId = GetJobId(existingSchedule.TaskType, existingSchedule.PlantId);
            _recurringJobManager.AddOrUpdate<IPlantNotificationJob>(
                jobId,
                job => job.ExecuteNotification(existingSchedule.PlantId, existingSchedule.TaskType),
                newCronExp
            );
        }

        /// <summary>
        /// Toggles the active state of a schedule.
        /// When deactivated, the job is removed from the Hangfire scheduler but kept in the DB.
        /// </summary>
        public async Task TogglePlantScheduleAsync(Guid scheduleId)
        {
            var schedule = await _plantScheduleRepository.GetPlantScheduleByIdAsync(scheduleId);
            if (schedule == null)
                throw new KeyNotFoundException("Schedule not found.");

            schedule.IsActive = !schedule.IsActive;
            await _plantScheduleRepository.UpdatePlantScheduleAsync(schedule);

            string jobId = GetJobId(schedule.TaskType, schedule.PlantId);

            if (schedule.IsActive)
            {
                _recurringJobManager.AddOrUpdate<IPlantNotificationJob>(
                    jobId,
                    job => job.ExecuteNotification(schedule.PlantId, schedule.TaskType),
                    schedule.CronExpression
                );
            }
            else
            {
                _recurringJobManager.RemoveIfExists(jobId);
            }
        }

        /// <summary>
        /// Permanently removes a schedule from both the database and the Hangfire scheduler.
        /// </summary>
        public async Task DeletePlantScheduleAsync(Guid scheduleId)
        {
            var schedule = await _plantScheduleRepository.GetPlantScheduleByIdAsync(scheduleId);
            if (schedule == null)
                throw new KeyNotFoundException("Schedule not found.");

            _recurringJobManager.RemoveIfExists(GetJobId(schedule.TaskType, schedule.PlantId));
            await _plantScheduleRepository.DeletePlantScheduleAsync(scheduleId);
        }

        // --- Helper Methods ---

        private string GetJobId(string taskType, Guid plantId) => $"{taskType}-Plant-{plantId}";

        private string GenerateCronExpression(CreateScheduleDTO dto)
        {
            if (dto.Frequency == "Daily")
                return Cron.Daily(dto.Hour, dto.Minute);

            if (dto.Frequency == "Weekly" && dto.Days != null && dto.Days.Any())
                return $"{dto.Minute} {dto.Hour} * * {string.Join(",", dto.Days.Select(GetCronDay))}";

            return string.Empty;
        }

        private string GetCronDay(string day) => day.ToUpperInvariant() switch
        {
            "SUNDAY" => "0",
            "MONDAY" => "1",
            "TUESDAY" => "2",
            "WEDNESDAY" => "3",
            "THURSDAY" => "4",
            "FRIDAY" => "5",
            "SATURDAY" => "6",
            _ => throw new ArgumentException($"Invalid day name: {day}")
        };

        public async Task<List<PlantScheduleDTO>?> GetPlantSchedulesAsync(Guid plantId)
        {
            var schedules = await _plantScheduleRepository.GetSchedulesByPlantIdAsync(plantId);
            
            return _mapper.Map<List<PlantScheduleDTO>>(schedules);

        }
    }
}