using ApplicationLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface IPlantService
    {
        Task AddPlantToGreenhouse(Guid GreenhouseId,PlantRegisterDTO dTO);

        Task<List<PlantDTO>> GetAllGreenhousePlants(Guid GreenhouseId);

        Task<List<PlantWithMetricsDTO>> GetAllGreenhousePlantsWithMetrics(Guid GreenhouseId, string userTimeZoneId);

        Task<PlantDTO> GetPlantById(Guid PlantId);

        Task DeletePlantAsync(Guid PlantId);

        Task<bool> isPlnatAssignmentExists(Guid PlnatId);

        Task<List<PlantWithAssignedFarmersDTO>> getPlantsWithAssignedFarmers(Guid GreenhouseId);

        Task<PlantDTO> UpdatePlantAsync(Guid PlantId, PlantUpdateDTO dTO);

        Task MarkPlantNotificationsAsRead(List<Guid> notificationsIds);

        Task<List<PlantNotificationDTO>> GetPlantNotificationDTOs(Guid PlantId , string? userTimeZoneId);
    }
}
