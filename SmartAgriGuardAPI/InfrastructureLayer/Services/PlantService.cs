using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces;
using AutoMapper;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeZoneConverter;

namespace InfrastructureLayer.Services
{
    /// <summary>
    /// Service managing the core lifecycle and operational data of individual plants.
    /// Orchestrates interactions between greenhouses, sensor data, file storage, and notifications.
    /// </summary>
    public class PlantService : IPlantService
    {
        private readonly IPlantRepository _plantRepository;
        private readonly IPlantTypeRepository _plantTypeRepository;
        private readonly IMapper _mapper;
        private readonly IGreenhouseRepository _greenhouseRepository;
        private readonly ISensorDataService _sensorDataService;
        private readonly ISensorDataArchiveService _sensorDataArchiveService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IPlantNotificationsRepository _plantNotificationsRepository;

        public PlantService(IPlantRepository plantRepository, IPlantTypeRepository plantTypeRepository, IMapper mapper, IGreenhouseRepository greenhouseRepository,
            ISensorDataService sensorDataService, ISensorDataArchiveService sensorDataArchiveService, IFileStorageService fileStorageService, IPlantNotificationsRepository plantNotificationsRepository)
        {
            _plantRepository = plantRepository;
            _plantTypeRepository = plantTypeRepository;
            _mapper = mapper;
            _greenhouseRepository = greenhouseRepository;
            _sensorDataService = sensorDataService;
            _sensorDataArchiveService = sensorDataArchiveService;
            _fileStorageService = fileStorageService;
            _plantNotificationsRepository = plantNotificationsRepository;
        }

        /// <summary>
        /// Registers a new plant within a specified greenhouse.
        /// </summary>
        /// <param name="GreenhouseId">The ID of the parent greenhouse.</param>
        /// <param name="dTO">Registration data including Name, Type, and optional Image/Location.</param>
        /// <exception cref="KeyNotFoundException">Thrown if the greenhouse or plant type does not exist.</exception>
        public async Task AddPlantToGreenhouse(Guid GreenhouseId, PlantRegisterDTO dTO)
        {
            var greenhoues = await _greenhouseRepository.GetGreenhouseById(GreenhouseId);
            if (greenhoues == null)
                throw new KeyNotFoundException("Greenhouse not found.");

            var plantType = await _plantTypeRepository.GetByIdAsync(dTO.PlantTypeId);
            if (plantType == null)
                throw new KeyNotFoundException("PlantType not found.");

            var plant = new Plant
            {
                GreenhouseId = GreenhouseId,
                PlantTypeId = dTO.PlantTypeId,
                Name = dTO.Name,
            };

            if (!string.IsNullOrEmpty(dTO.ImagePath))
                plant.ImageUrl = dTO.ImagePath;
            if (!string.IsNullOrEmpty(dTO.Location))
                plant.Location = dTO.Location;

            await _plantRepository.AddAsync(plant);
        }

        /// <summary>
        /// Verifies if any farmers are currently assigned to the specified plant.
        /// </summary>
        /// <returns>True if assignments exist; otherwise, false.</returns>
        public async Task<bool> isPlnatAssignmentExists(Guid PlantId)
        {
            var plant = await _plantRepository.GetPlantWithFarmerPlant(PlantId);
            if (plant == null)
                throw new KeyNotFoundException("plant not found");

            return plant.FarmerPlants.Any();
        }

        /// <summary>
        /// Performs a "Hard Delete" of a plant, ensuring all cascading data (Sensor data, Archive data, and Physical Images) are purged.
        /// </summary>
        /// <remarks>
        /// This ensures no orphaned data remains in the file system or time-series tables.
        /// </remarks>
        public async Task DeletePlantAsync(Guid PlantId)
        {
            var plant = await _plantRepository.GetPlantWithFarmerPlant(PlantId);
            if (plant == null)
                throw new KeyNotFoundException("the plant not found");

            // Purge telemetry data
            await _sensorDataService.DeleteAllByPlantIdAsync(plant.Id);
            await _sensorDataArchiveService.DeleteAllByPlantIdAsync(plant.Id);

            // Purge physical image file from storage
            await _fileStorageService.DeleteFileAsync(plant.ImageUrl);

            await _plantRepository.DeleteAsync(plant.Id);
        }

        /// <summary>
        /// Retrieves all plants belonging to a specific greenhouse.
        /// </summary>
        public async Task<List<PlantDTO>> GetAllGreenhousePlants(Guid GreenhouseId)
        {
            var plants = await _plantRepository.GetAllGreenhousePlantsAsync(GreenhouseId);
            if (plants == null || plants.Count == 0)
                throw new KeyNotFoundException("No Plants found for this greenhouse");

            return _mapper.Map<List<PlantDTO>>(plants);
        }

        /// <summary>
        /// Fetches greenhouse plants along with their most recent sensor readings, localized to the user's timezone.
        /// </summary>
        /// <param name="userTimeZoneId">The IANA or Windows TimeZone ID (e.g., 'UTC' or 'Eastern Standard Time').</param>
        public async Task<List<PlantWithMetricsDTO>> GetAllGreenhousePlantsWithMetrics(Guid GreenhouseId, string userTimeZoneId)
        {
            var plants = await _plantRepository.GetAllGreenhousePlantsWithMetrics(GreenhouseId);
            if (plants == null || plants.Count == 0)
                throw new KeyNotFoundException("No Plants found for this greenhouse");

            var dtos = _mapper.Map<List<PlantWithMetricsDTO>>(plants);

            // Safely resolve TimeZone
            TimeZoneInfo tz;
            try { tz = TZConvert.GetTimeZoneInfo(userTimeZoneId); }
            catch { tz = TimeZoneInfo.Utc; }

            // Localize timestamps for each plant's telemetry
            foreach (var plant in dtos.Where(p => p.LatestMetrics != null))
            {
                plant.LatestMetrics.Timestamp = TimeZoneInfo.ConvertTime(plant.LatestMetrics.Timestamp, tz);
            }

            return dtos;
        }

        /// <summary>
        /// Updates a collection of plant notifications to 'Read' status.
        /// </summary>
        public async Task MarkPlantNotificationsAsRead(List<Guid> notificationsIds)
        {
            var notifications = await _plantNotificationsRepository.GetByIdsAsync(notificationsIds);
            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                await _plantNotificationsRepository.UpdateAsync(notification);
            }
        }

        /// <summary>
        /// Gets all historical notifications for a specific plant.
        /// </summary>
        public async Task<List<PlantNotificationDTO>> GetPlantNotificationDTOs(Guid PlantId)
        {
            var plant = await _plantRepository.GetPlantById(PlantId);
            if (plant == null)
                throw new KeyNotFoundException("plant not found");

            var notifications = await _plantNotificationsRepository.GetByPlantIdAsync(PlantId);
            return _mapper.Map<List<PlantNotificationDTO>>(notifications);
        }

        /// <summary>
        /// Fetches basic metadata for a specific plant.
        /// </summary>
        public async Task<PlantDTO> GetPlantById(Guid PlantId)
        {
            var plant = await _plantRepository.GetPlantById(PlantId);
            return plant == null ? null : _mapper.Map<PlantDTO>(plant);
        }

        /// <summary>
        /// Updates plant details. If a new image is provided, the old physical file is deleted to save space.
        /// </summary>
        public async Task<PlantDTO> UpdatePlantAsync(Guid PlantId, PlantUpdateDTO dTO)
        {
            var plant = await _plantRepository.GetPlantById(PlantId);
            if (plant == null)
                throw new KeyNotFoundException("the plant not found");

            if (!string.IsNullOrEmpty(dTO.Name))
                plant.Name = dTO.Name;

            if (!string.IsNullOrEmpty(dTO.Location))
                plant.Location = dTO.Location;

            // Handle Image Replacement logic
            if (!string.IsNullOrEmpty(dTO.ImagePath))
            {
                // Delete previous file if it exists to avoid storage bloat
                if (!string.IsNullOrEmpty(plant.ImageUrl))
                    await _fileStorageService.DeleteFileAsync(plant.ImageUrl);

                plant.ImageUrl = dTO.ImagePath;
            }

            await _plantRepository.UpdateAsync(plant);
            return _mapper.Map<PlantDTO>(plant);
        }

        /// <summary>
        /// Retrieves a list of plants and the farmers currently assigned to care for them.
        /// </summary>
        public async Task<List<PlantWithAssignedFarmersDTO>> getPlantsWithAssignedFarmers(Guid GreenhouseId)
        {
            var plants = await _plantRepository.GetPlantsWithAssignedFarmers(GreenhouseId);
            if (plants == null || plants.Count == 0)
                throw new KeyNotFoundException("There is no data");

            return _mapper.Map<List<PlantWithAssignedFarmersDTO>>(plants);
        }
    }
}