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

        Task<PlantDTO> GetPlantById(Guid PlantId);

        Task DeletePlantAsync(Guid PlantId);

        Task<bool> isPlnatAssignmentExists(Guid PlnatId);
    }
}
