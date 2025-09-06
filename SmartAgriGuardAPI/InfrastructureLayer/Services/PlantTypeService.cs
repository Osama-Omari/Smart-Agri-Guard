using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureLayer.Services
{
    public class PlantTypeService : IPlantTypeService
    {
        private readonly IPlantTypeRepository _plantTypeRepository;
        public PlantTypeService(IPlantTypeRepository plantTypeRepository)
        {
            _plantTypeRepository = plantTypeRepository;
        }

        public async Task AddPlantType(PlantTypeRegisterDTO dto)
        {
            if (await _plantTypeRepository.IsNameExists(dto.Name))
            {
                throw new Exception("The name for the plant type already exist");
            }
            var plantType = new PlantType
            { 
                Name = dto.Name,
            };
            if(!string.IsNullOrEmpty(dto.Description))
                plantType.Description = dto.Description;
            await _plantTypeRepository.AddAsync(plantType);
        }
    }
}
