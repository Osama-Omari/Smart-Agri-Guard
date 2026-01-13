using ApplicationLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface IPlantScheduleService
    {
        Task AddPlantScheduleAsync(Guid PlantId, CreateScheduleDTO dto);

        Task UpdatePlantScheduleAsync(Guid scheduleId, CreateScheduleDTO dto);

        Task TogglePlantScheduleAsync(Guid scheduleId);

        Task DeletePlantScheduleAsync(Guid scheduleId);

        Task<List<PlantScheduleDTO>?> GetPlantSchedulesAsync(Guid plantId);
    }
}
