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
    public class SensorDataService : ISensorDataService
    {
        private readonly ISensorDataRepository _sensorDataRepository;
        private readonly IPlantRepository _plantRepository;
        private readonly IMapper _mapper;

        public SensorDataService(ISensorDataRepository sensorDataRepository, IPlantRepository plantRepository,IMapper mapper)
        {
            _sensorDataRepository = sensorDataRepository;
            _plantRepository = plantRepository;
            _mapper = mapper;
        }

        public async Task AddSensorData(Guid plantId, SensorDataRegisterDTO dto)
        {
            var plant = await _plantRepository.GetPlantById(plantId);
            if (plant == null)
                throw new KeyNotFoundException("Plant not found.");
            var sensordata = new SensorData();
            if(dto.Temperature.HasValue)
                sensordata.Temperature = dto.Temperature.Value;
            if(dto.Humidity.HasValue)
                sensordata.Humidity = dto.Humidity.Value;
            if(dto.SoilMoisture.HasValue)
                sensordata.SoilMoisture = dto.SoilMoisture.Value;
            if(dto.PH.HasValue)
                sensordata.Ph = dto.PH.Value;
            if(dto.Phosphorus.HasValue)
                sensordata.Phosphorus = dto.Phosphorus.Value;
            if(dto.Potassium.HasValue)
                sensordata.Potassium = dto.Potassium.Value;
            if(dto.Nitrogen.HasValue)
                sensordata.Nitrogen = dto.Nitrogen.Value;
            sensordata.PlantId = plantId;
            sensordata.Timestamp = dto.Timestamp;
            await _sensorDataRepository.AddAsync(sensordata);

        }

        public async Task DeleteAllByPlantIdAsync(Guid plantId)
        {
            var sensordata = await _sensorDataRepository.GetByPlantIdAsync(plantId);
            if (sensordata == null || !sensordata.Any())
                return;
             _sensorDataRepository.RemoveRange(sensordata);
            await _sensorDataRepository.SaveChangesAsync();

        }

        public async Task<SensorDataDTO?> GetLatestSensorData(Guid plantId, string userTimeZoneId)
        {
            var sensorData = await _sensorDataRepository.GetLatestByPlantIdAsync(plantId);
            if (sensorData == null)
                return null;
            var tz = TimeZoneInfo.FindSystemTimeZoneById(userTimeZoneId);
            sensorData.Timestamp = TimeZoneInfo.ConvertTimeFromUtc(
                        sensorData.Timestamp.UtcDateTime, tz);
            return _mapper.Map<SensorDataDTO>(sensorData);
        }

        public async Task<SensorTrendResponseDTO> GetSensorTrendsAsync(SensorTrendRequestDTO dto, string userTimeZoneId)
        {
            var data = await _sensorDataRepository
                .GetByPlantIdAndDateRangeAsync(dto.PlantId, dto.StartDate, dto.EndDate);

            if (data == null || !data.Any())
                throw new KeyNotFoundException("No sensor data found for the specified plant in the given date range.");

            var result = new List<Dictionary<string, object>>();

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
