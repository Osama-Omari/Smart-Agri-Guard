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
            sensordata.Timestamp = DateTime.UtcNow;
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

        public async Task<SensorDataDTO?> GetLatestSensorData(Guid plantId)
        {
            var sensorData = await _sensorDataRepository.GetLatestByPlantIdAsync(plantId);
            if (sensorData == null)
                return null;
            return _mapper.Map<SensorDataDTO>(sensorData);


        }
    }
}
