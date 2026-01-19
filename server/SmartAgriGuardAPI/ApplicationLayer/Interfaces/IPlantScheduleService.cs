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
        Task AddPlantScheduleAsync(Guid PlantId, CreateScheduleDTO dto,string? userTimeZoneId);

        Task UpdatePlantScheduleAsync(Guid scheduleId, CreateScheduleDTO dto,string ? userTimeZoneId);

        Task TogglePlantScheduleAsync(Guid scheduleId);

        Task DeletePlantScheduleAsync(Guid scheduleId);

        Task<List<PlantScheduleDTO>?> GetPlantSchedulesAsync(Guid plantId , string? userTimeZoneId);
    }
}
