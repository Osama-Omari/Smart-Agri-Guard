using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces;
using AutoMapper;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeZoneConverter;

namespace InfrastructureLayer.Services
{
    /// <summary>
    /// Service responsible for processing sensor telemetry, calculating trends, 
    /// and managing time-series data for plants.
    /// </summary>
    public class SensorDataService : ISensorDataService
    {
        private readonly ISensorDataRepository _sensorDataRepository;
        private readonly IPlantRepository _plantRepository;
        private readonly IMapper _mapper;
        private readonly ISystemReportsRepository _systemReportRepository;
        private readonly INotificationService _notificationService;
        private readonly IPlantHealthService _plantHealthService;

        public SensorDataService(ISensorDataRepository sensorDataRepository, IPlantRepository plantRepository,
            IMapper mapper, ISystemReportsRepository systemReportRepository,
            INotificationService notificationService, IPlantHealthService plantHealthService)
        {
            _sensorDataRepository = sensorDataRepository;
            _plantRepository = plantRepository;
            _mapper = mapper;
            _systemReportRepository = systemReportRepository;
            _notificationService = notificationService;
            _plantHealthService = plantHealthService;
        }

        /// <summary>
        /// Orchestrates the ingestion of telemetry data, hardware health monitoring, and automated alerting.
        /// </summary>
        /// <remarks>
        /// This method processes raw sensor data by:
        /// 1. Validating the existence of the physical plant entity.
        /// 2. Mapping telemetry metrics (supporting partial nulls for modular sensor arrays).
        /// 3. Detecting hardware-specific failures (DHT22 or Modbus) based on explicit status flags or data absence.
        /// 4. Triggering a multi-channel notification workflow if hardware faults are identified.
        /// </remarks>
        /// <param name="plantId">The unique identifier for the targeted plant.</param>
        /// <param name="dto">The telemetry data transfer object containing environmental metrics and sensor status flags.</param>
        /// <exception cref="KeyNotFoundException">Thrown if the plantId does not correspond to an existing database record.</exception>
        public async Task AddSensorData(Guid plantId, SensorDataRegisterDTO dto)
        {
            var plant = await _plantRepository.GetPlantById(plantId);
            if (plant == null)
                throw new KeyNotFoundException("Plant not found.");

            // Map incoming telemetry to the persistence entity
            // Nullable types are used to preserve 'null' states for inactive or broken hardware modules
            var sensordata = new SensorData
            {
                PlantId = plantId,
                Timestamp = dto.Timestamp,
                Temperature = dto.Temperature,
                Humidity = dto.Humidity,
                SoilMoisture = dto.SoilMoisture,
                Ph = dto.PH,
                Phosphorus = dto.Phosphorus,
                Potassium = dto.Potassium,
                Nitrogen = dto.Nitrogen
            };
            
            // Evaluate atmospheric sensor (DHT22) health
            // Failure is defined by an explicit 'Faulty' status or the total absence of air-related metrics
            if (dto.AirSensorStatus == "Faulty" || (dto.Temperature == null && dto.Humidity == null))
            {
                await CreateSystemReportAndNotify(plant, $"{plant.Greenhouse.Name} : (DHT22) Failure Detected for Plant: {plant.Name}");
            }

            // Evaluate soil-subsystem (Modbus/RS485) health
            // We now check all metrics (Moisture, pH, N, P, K) to ensure the entire probe is functional
            if (dto.SoilSensorStatus == "Faulty" ||
               (dto.SoilMoisture == null && dto.PH == null && dto.Nitrogen == null && dto.Phosphorus == null && dto.Potassium == null))
            {
                await CreateSystemReportAndNotify(plant, $"{plant.Greenhouse.Name} : Soil Sensor Array (Modbus/NPK) Failure Detected for Plant: {plant.Name}");
            }

            // Persist validated telemetry to the historical archive
            await _sensorDataRepository.AddAsync(sensordata);

            //check if all necessary data is present to evaluate plant health
            if (dto.Temperature == null || dto.Humidity == null || dto.SoilMoisture == null ||
                dto.Nitrogen == null || dto.Phosphorus == null || dto.Potassium == null || dto.PH == null)
            {
                //insufficient data to evaluate plant health
                return;
            }

            //call the plant health service to generate the plant health status
            var input = new TomatoHealthInput
            {

                Temperature = (float)dto.Temperature,
                Humidity = (float)dto.Humidity,
                SoilMoisture = (float)dto.SoilMoisture,
                Nitrogen = (float)dto.Nitrogen,
                Phosphorus = (float)dto.Phosphorus,
                Potassium = (float)dto.Potassium,
                Ph = (float)dto.PH
            };

            await _plantHealthService.GeneratePlantHealth(plantId, input);


            //check if the sensor data indicates any abnormal conditions that require alerting
            await _plantHealthService.EvaluateAndAlertPlantHealth(plant, sensordata);
        }

        /// <summary>
        /// Internal workflow to log hardware incidents and dispatch real-time alerts to responsible personnel.
        /// </summary>
        /// <param name="plant">The plant entity where the failure occurred.</param>
        /// <param name="errorMessage">Detailed diagnostic message describing the failure type.</param>
        private async Task CreateSystemReportAndNotify(Plant plant, string errorMessage)
        {
            // Log the event in SystemReports for administrative auditing and dashboard visualization
            var report = new SystemReports
            {
                GreenhouseId = plant.GreenhouseId,
                ErrorType = "HardwareFailure",
                Message = $"Plant '{plant.Name}': {errorMessage}",
                ReportDate = DateTime.UtcNow,
                IsRead = false
            };

            await _systemReportRepository.AddAsync(report);

            // 1. Dispatch push notification to the specific Greenhouse Manager via FCM
            if (plant.Greenhouse?.ManagerId != null)
            {
                await _notificationService.SendToUserAsync(
                    plant.Greenhouse.ManagerId.Value,
                    "Hardware Alert",
                    errorMessage
                );
            }

            // 2. Dispatch a global broadcast notification to all Administrators
            await _notificationService.SendToAdmin(
                "Hardware Alert",
                errorMessage
            );
        }


        /// <summary>
        /// Purges all historical sensor data associated with a specific plant.
        /// Typically used when a plant is being removed or reset.
        /// </summary>
        public async Task DeleteAllByPlantIdAsync(Guid plantId)
        {
            var sensordata = await _sensorDataRepository.GetByPlantIdAsync(plantId);
            if (sensordata == null || !sensordata.Any())
                return;

            await _sensorDataRepository.RemoveRange(sensordata);
        }

        /// <summary>
        /// Fetches the most recent sensor reading and converts its timestamp to the user's local time.
        /// </summary>
        /// <param name="plantId">The target plant GUID.</param>
        /// <param name="userTimeZoneId">The IANA or Windows TimeZone identifier.</param>
        public async Task<SensorDataDTO?> GetLatestSensorData(Guid plantId, string userTimeZoneId)
        {
            var sensorData = await _sensorDataRepository.GetLatestByPlantIdAsync(plantId);
            if (sensorData == null)
                return null;

            // Convert UTC timestamp to User's localized time for display
            var tz = TimeZoneInfo.FindSystemTimeZoneById(userTimeZoneId);
            sensorData.Timestamp = TimeZoneInfo.ConvertTimeFromUtc(sensorData.Timestamp.UtcDateTime, tz);

            return _mapper.Map<SensorDataDTO>(sensorData);
        }

        /// <summary>
        /// Generates a trend report by filtering and transforming sensor data over a date range.
        /// </summary>
        /// <remarks>
        /// This method dynamically builds a response dictionary containing only the metrics 
        /// requested in the <paramref name="dto"/>. This reduces the JSON payload size.
        /// </remarks>
        /// <param name="dto">Contains PlantId, StartDate, EndDate, and a list of requested Metrics.</param>
        /// <param name="userTimeZoneId">The timezone used to format the result timestamps.</param>
        /// <returns>A localized trend response with dynamic readings.</returns>
        public async Task<SensorTrendResponseDTO> GetSensorTrendsAsync(SensorTrendRequestDTO dto, string userTimeZoneId)
        {
            var data = await _sensorDataRepository
                .GetByPlantIdAndDateRangeAsync(dto.PlantId, dto.StartDate, dto.EndDate);

            if (data == null || !data.Any())
                throw new KeyNotFoundException("No sensor data found for the specified plant in the given date range.");

            var result = new List<Dictionary<string, object>>();

            // Safely resolve the TimeZoneInfo using the cross-platform TimeZoneConverter
            TimeZoneInfo tz;
            try
            {
                tz = TZConvert.GetTimeZoneInfo(userTimeZoneId);
            }
            catch
            {
                tz = TimeZoneInfo.Utc;
            }

            foreach (var row in data)
            {
                var record = new Dictionary<string, object>();
                var localTimestamp = TimeZoneInfo.ConvertTime(row.Timestamp, tz);
                record["timestamp"] = localTimestamp;

                // Dynamically include only metrics specified in the request
                foreach (var sensor in dto.Metrics)
                {
                    var key = sensor.Trim().ToLower();
                    switch (key)
                    {
                        case "temperature": record["temperature"] = row.Temperature; break;
                        case "humidity": record["humidity"] = row.Humidity; break;
                        case "soilmoisture": record["soilMoisture"] = row.SoilMoisture; break;
                        case "nitrogen": record["nitrogen"] = row.Nitrogen; break;
                        case "phosphorus": record["phosphorus"] = row.Phosphorus; break;
                        case "potassium": record["potassium"] = row.Potassium; break;
                        case "ph": record["ph"] = row.Ph; break;
                    }
                }
                result.Add(record);
            }

            return new SensorTrendResponseDTO
            {
                PlantId = dto.PlantId,
                Readings = result
            };
        }
    }
}