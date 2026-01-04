using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface ISensorDataArchiveRepository
    {
        Task AddAsync(SensorDataArchive archive);

        Task<SensorDataArchive?> GetByIdAsync(Guid id);
        Task<List<SensorDataArchive>> GetAllAsync();
        Task<List<SensorDataArchive>> GetByPlantIdAsync(Guid plantId);
        Task UpdateAsync(SensorDataArchive archive);

        Task DeleteAsync(Guid id);

        void RemoveRange(IEnumerable<SensorDataArchive> archives);

        Task SaveChangesAsync();

        Task<List<SensorDataArchive>> GetByPlantIdAndDateRangeAsync(Guid plantId, DateTimeOffset startDate, DateTimeOffset endDate);
    }
}
