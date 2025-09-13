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

        public async Task<SensorTrendResponseDTO> GetSensorArchiveTrendsAsync(SensorTrendArchiveRequestDTO dto)
        {
            var data = await _sensorDataArchiveRepository.GetByPlantIdAndDateRangeAsync(dto.PlantId, dto.StartDate, dto.EndDate);

            if (data == null || !data.Any())
                throw new KeyNotFoundException("No sensor archive data found for the specified criteria.");
            return new SensorTrendResponseDTO
            {
                PlantId = dto.PlantId,
                Readings = _mapper.Map<List<SensorReadingMultiDto>>(data)
            };

        }
    }
}
