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
    public class SensorDataArchiveService : ISensorDataArchiveService
    {
        private readonly ISensorDataArchiveRepository _sensorDataArchiveRepository;
        private readonly IMapper _mapper;
        public SensorDataArchiveService(ISensorDataArchiveRepository sensorDataArchiveRepository,IMapper mapper)
        {
            _sensorDataArchiveRepository = sensorDataArchiveRepository;
            _mapper = mapper;
        }
        public async Task DeleteAllByPlantIdAsync(Guid PlantId)
        {
            var sensorDataArchives = await _sensorDataArchiveRepository.GetByPlantIdAsync(PlantId);
            if(sensorDataArchives == null || !sensorDataArchives.Any())
                return;
            _sensorDataArchiveRepository.RemoveRange(sensorDataArchives);
            await _sensorDataArchiveRepository.SaveChangesAsync();
        }

        public async Task<SensorTrendResponseDTO> GetSensorArchiveTrendsAsync(SensorTrendArchiveRequestDTO dto, string userTimeZoneId)
        {
            var data = await _sensorDataArchiveRepository
                .GetByPlantIdAndDateRangeAsync(dto.PlantId, dto.StartDate, dto.EndDate);

            if (data == null || !data.Any())
                throw new KeyNotFoundException("No sensor archive data found for the specified criteria.");

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
                var localTimestamp = TimeZoneInfo.ConvertTimeFromUtc(
                    row.Timestamp.UtcDateTime, tz);

                record["timestamp"] = localTimestamp;

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
