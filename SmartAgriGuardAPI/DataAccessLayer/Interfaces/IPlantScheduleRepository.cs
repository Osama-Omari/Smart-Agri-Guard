using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IPlantScheduleRepository
    {
        Task<IEnumerable<PlantSchedule>> GetAllPlantSchedulesAsync();
        Task<PlantSchedule?> GetPlantScheduleByIdAsync(Guid id);

        Task<IEnumerable<PlantSchedule>> GetPlantSchedulesByPlantIdAsync(Guid plantId);
        Task AddPlantScheduleAsync(PlantSchedule plantSchedule);
        Task UpdatePlantScheduleAsync(PlantSchedule plantSchedule);
        Task DeletePlantScheduleAsync(Guid id);
    }
}
