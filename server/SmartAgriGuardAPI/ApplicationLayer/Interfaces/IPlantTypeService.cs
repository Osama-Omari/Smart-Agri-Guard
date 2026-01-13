using ApplicationLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface IPlantTypeService
    {
        Task AddPlantType(PlantTypeRegisterDTO dto);

        Task<List<PlantTypeDTO>> GetAllPlantTypes();

        Task UpdatePlantType(Guid Id, PlantTypeUpdateDTO dto);

        Task<PlantTypeDTO> GetPlantTypeById(Guid Id);

        Task DeletePlantType(Guid Id);

    }
}
