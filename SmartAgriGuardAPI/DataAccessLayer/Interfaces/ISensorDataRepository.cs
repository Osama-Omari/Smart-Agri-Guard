using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface ISensorDataRepository
    {
        Task<SensorData> AddAsync(SensorData sensorData);

        
        Task<SensorData?> GetByIdAsync(Guid id);
        Task<IEnumerable<SensorData>> GetAllAsync();
        Task<IEnumerable<SensorData>> GetByPlantIdAsync(Guid plantId);

        Task<SensorData?> GetLatestSensorDataByPlantIdAsync(Guid plantId);

        Task<SensorData> UpdateAsync(SensorData sensorData);

        Task<bool> DeleteAsync(Guid id);
    }
}
