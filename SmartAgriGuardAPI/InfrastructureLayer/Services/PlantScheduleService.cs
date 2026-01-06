using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces;
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
    public class PlantScheduleService : IPlantScheduleService
    {
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IPlantRepository _plantRepository;
        private readonly IPlantScheduleRepository _plantScheduleRepository;


        public PlantScheduleService(IRecurringJobManager recurringJobManager,IPlantRepository plantRepository, IPlantScheduleRepository plantScheduleRepository)
        {
            _recurringJobManager = recurringJobManager;
            _plantRepository = plantRepository;
            _plantScheduleRepository = plantScheduleRepository;

        }


        public async Task AddPlantScheduleAsync(Guid PlantId, CreateScheduleDTO dto)
        {
            var plant = await _plantRepository.GetPlantById(PlantId);
            if (plant == null)
                throw new KeyNotFoundException("the plant not found");

            string cronExp = "";

            if (dto.Frequency == "Daily")
            {
                cronExp = Cron.Daily(dto.Hour, dto.Minute);
            }
            else if (dto.Frequency == "Weekly" && dto.Days != null && dto.Days.Count > 0)
            {
                var days = dto.Days.Select(d => Enum.Parse<DayOfWeek>(d)).ToArray();
                cronExp = $"{dto.Minute} {dto.Hour} * * {string.Join(",", dto.Days.Select(GetCronDay))}";
            }

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

            await _plantScheduleRepository.AddPlantScheduleAsync(schedule);

            // Schedule the recurring job in Hangfire

            string jobId = $"{schedule.TaskType}-Plant-{PlantId}";
            _recurringJobManager.AddOrUpdate<IPlantNotificationJob>(
                jobId,
                job => job.ExecuteNotification(PlantId, schedule.TaskType),
                cronExp
                );
        }

        private string GetCronDay(string day) => day switch
        {
            "Sunday" => "0",
            "Monday" => "1",
            "Tuesday" => "2",
            "Wednesday" => "3",
            "Thursday" => "4",
            "Friday" => "5",
            "Saturday" => "6",
            _ => "*"
        };
    }
}
