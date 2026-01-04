using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces;
using AutoMapper;
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
        private readonly IMapper _mapper;
        public PlantTypeService(IPlantTypeRepository plantTypeRepository, IMapper mapper)
        {
            _plantTypeRepository = plantTypeRepository;
            _mapper = mapper;
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

        public async Task DeletePlantType(Guid Id)
        {
            var plantType = await _plantTypeRepository.GetByIdAsync(Id);
            if (plantType.Plants == null || plantType.Plants.Count == 0)
            {
                await _plantTypeRepository.DeleteAsync(plantType.Id);
            }
            else if (plantType.Plants.Any())
            {
                throw new Exception("Cannot delete plant type with associated plants.");
            }
            else
            {
                throw new KeyNotFoundException("Plant type not found.");
            }
        }

        public async Task<List<PlantTypeDTO>> GetAllPlantTypes()
        {
            var plantTypes = await _plantTypeRepository.GetAllAsync();
            return _mapper.Map<List<PlantTypeDTO>>(plantTypes);
        }

        public async Task<PlantTypeDTO> GetPlantTypeById(Guid Id)
        {
            var plantType = await _plantTypeRepository.GetByIdAsync(Id);
            if (plantType == null)
                throw new KeyNotFoundException("Plant type not found.");
            return _mapper.Map<PlantTypeDTO>(plantType);
        }

        public async Task UpdatePlantType(Guid Id, PlantTypeUpdateDTO dto)
        {
            var plantType = await _plantTypeRepository.GetByIdAsync(Id);
            if (plantType == null)
                throw new KeyNotFoundException("Plant type not found.");
            if (!string.IsNullOrEmpty(dto.Description))
                plantType.Description = dto.Description;
            else
            {
                plantType.Description = null;
            }

            if (!string.IsNullOrEmpty(dto.Name) && !plantType.Name.Equals(dto.Name.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                if (await _plantTypeRepository.IsNameExists(dto.Name))
                {
                    throw new Exception("The name for the plant type already exist");
                }
                plantType.Name = dto.Name;
            }
            await _plantTypeRepository.UpdateAsync(plantType);
        }
    }
}
