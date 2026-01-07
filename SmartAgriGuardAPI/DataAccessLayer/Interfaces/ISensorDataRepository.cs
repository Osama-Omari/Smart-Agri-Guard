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
        Task<IEnumerable<SensorData>> GetByPlantIdAsync(Guid plantId);

        Task RemoveRange(IEnumerable<SensorData> sensorData);

        Task SaveChangesAsync();

        Task<SensorData?> GetLatestByPlantIdAsync(Guid plantId);

        Task<List<SensorData>> GetByPlantIdAndDateRangeAsync(Guid plantId, DateTimeOffset startDate, DateTimeOffset endDate);

        Task<List<SensorData>> GetSensorDataOlderThan(DateTimeOffset cutoffDate);
    }
}
