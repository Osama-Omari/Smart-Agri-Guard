using ApplicationLayer.Interfaces;
using DataAccessLayer.Interfaces;
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
        
        public SensorDataService(ISensorDataRepository sensorDataRepository)
        {
            _sensorDataRepository = sensorDataRepository;
        }

        public async Task DeleteAllByPlantIdAsync(Guid plantId)
        {
            var sensordata = await _sensorDataRepository.GetByPlantIdAsync(plantId);
            if (sensordata == null || !sensordata.Any())
                return;
             _sensorDataRepository.RemoveRange(sensordata);
            await _sensorDataRepository.SaveChangesAsync();

        }
    }
}
