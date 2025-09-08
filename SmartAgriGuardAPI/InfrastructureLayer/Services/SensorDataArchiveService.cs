using ApplicationLayer.Interfaces;
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
        public SensorDataArchiveService(ISensorDataArchiveRepository sensorDataArchiveRepository)
        {
            _sensorDataArchiveRepository = sensorDataArchiveRepository;
        }
        public async Task DeleteAllByPlantIdAsync(Guid PlantId)
        {
            var sensorDataArchives = await _sensorDataArchiveRepository.GetByPlantIdAsync(PlantId);
            if(sensorDataArchives == null || !sensorDataArchives.Any())
                return;
            _sensorDataArchiveRepository.RemoveRange(sensorDataArchives);
            await _sensorDataArchiveRepository.SaveChangesAsync();
        }
    }
}
