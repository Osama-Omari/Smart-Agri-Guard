using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces;
using AutoMapper;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
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

        public SensorDataService(ISensorDataRepository sensorDataRepository, IPlantRepository plantRepository, IMapper mapper)
        {
            _sensorDataRepository = sensorDataRepository;
            _plantRepository = plantRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Persists a new reading from a physical sensor to the database.
        /// </summary>
        /// <remarks>
        /// This method performs null-checks on each metric to allow partial data uploads 
        /// from sensors that might only support specific readings (e.g., only temperature).
        /// </remarks>
        /// <param name="plantId">The GUID of the plant being monitored.</param>
        /// <param name="dto">The telemetry packet containing optional environmental values.</param>
        /// <exception cref="KeyNotFoundException">Thrown if the specified plant does not exist.</exception>
        public async Task AddSensorData(Guid plantId, SensorDataRegisterDTO dto)
        {
            var plant = await _plantRepository.GetPlantById(plantId);
            if (plant == null)
                throw new KeyNotFoundException("Plant not found.");

            var sensordata = new SensorData();

            // Conditional mapping to handle sensors that might not send all metrics simultaneously
            if (dto.Temperature.HasValue) sensordata.Temperature = dto.Temperature.Value;
            if (dto.Humidity.HasValue) sensordata.Humidity = dto.Humidity.Value;
            if (dto.SoilMoisture.HasValue) sensordata.SoilMoisture = dto.SoilMoisture.Value;
            if (dto.PH.HasValue) sensordata.Ph = dto.PH.Value;
            if (dto.Phosphorus.HasValue) sensordata.Phosphorus = dto.Phosphorus.Value;
            if (dto.Potassium.HasValue) sensordata.Potassium = dto.Potassium.Value;
            if (dto.Nitrogen.HasValue) sensordata.Nitrogen = dto.Nitrogen.Value;

            sensordata.PlantId = plantId;
            sensordata.Timestamp = dto.Timestamp;

            await _sensorDataRepository.AddAsync(sensordata);
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