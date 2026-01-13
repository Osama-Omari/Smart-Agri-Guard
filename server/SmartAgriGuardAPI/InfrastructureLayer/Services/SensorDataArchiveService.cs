using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces;
using AutoMapper;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeZoneConverter;

namespace InfrastructureLayer.Services
{
    /// <summary>
    /// Service responsible for handling historical (archived) sensor data.
    /// This service allows for long-term trend analysis without impacting the performance 
    /// of real-time sensor data tracking.
    /// </summary>
    public class SensorDataArchiveService : ISensorDataArchiveService
    {
        private readonly ISensorDataArchiveRepository _sensorDataArchiveRepository;
        private readonly IMapper _mapper;

        public SensorDataArchiveService(ISensorDataArchiveRepository sensorDataArchiveRepository, IMapper mapper)
        {
            _sensorDataArchiveRepository = sensorDataArchiveRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Permanently deletes all archived sensor records for a specific plant.
        /// </summary>
        /// <param name="PlantId">The unique identifier of the plant.</param>
        public async Task DeleteAllByPlantIdAsync(Guid PlantId)
        {
            var sensorDataArchives = await _sensorDataArchiveRepository.GetByPlantIdAsync(PlantId);
            if (sensorDataArchives == null || !sensorDataArchives.Any())
                return;

            await _sensorDataArchiveRepository.RemoveRange(sensorDataArchives);
            
        }

        /// <summary>
        /// Retrieves and localizes historical sensor trends from the archive.
        /// </summary>
        /// <remarks>
        /// Similar to the real-time sensor service, this method allows dynamic metric selection.
        /// It is optimized for larger date ranges typical of archived data.
        /// </remarks>
        /// <param name="dto">The request criteria including the plant ID, date range, and metrics to include.</param>
        /// <param name="userTimeZoneId">The user's preferred timezone ID for timestamp conversion.</param>
        /// <returns>A trend response containing localized timestamps and the requested historical metrics.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if no archived data matches the specified criteria.</exception>
        public async Task<SensorTrendResponseDTO> GetSensorArchiveTrendsAsync(SensorTrendArchiveRequestDTO dto, string userTimeZoneId)
        {
            // Fetch historical data within the specified range from the archive repository
            var data = await _sensorDataArchiveRepository
                .GetByPlantIdAndDateRangeAsync(dto.PlantId, dto.StartDate, dto.EndDate);

            if (data == null || !data.Any())
                throw new KeyNotFoundException("No sensor archive data found for the specified criteria.");

            // Resolve TimeZone info, defaulting to UTC if the provided ID is invalid
            TimeZoneInfo tz;
            try
            {
                tz = TZConvert.GetTimeZoneInfo(userTimeZoneId);
            }
            catch
            {
                tz = TimeZoneInfo.Utc;
            }

            var result = new List<Dictionary<string, object>>();

            foreach (var row in data)
            {
                var record = new Dictionary<string, object>();

                // Convert stored UTC timestamp to the user's local time
                var localTimestamp = TimeZoneInfo.ConvertTimeFromUtc(
                    row.Timestamp.UtcDateTime, tz);

                record["timestamp"] = localTimestamp;

                // Dynamically populate the dictionary based on the Metrics requested in the DTO
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