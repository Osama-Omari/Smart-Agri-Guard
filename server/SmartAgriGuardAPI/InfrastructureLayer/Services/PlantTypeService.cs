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
    /// <summary>
    /// Service for managing plant classifications (Species/Types).
    /// Handles the administrative rules for creating and maintaining the catalog of plant types.
    /// </summary>
    public class PlantTypeService : IPlantTypeService
    {
        private readonly IPlantTypeRepository _plantTypeRepository;
        private readonly IMapper _mapper;

        public PlantTypeService(IPlantTypeRepository plantTypeRepository, IMapper mapper)
        {
            _plantTypeRepository = plantTypeRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Registers a new plant type after ensuring the name is unique.
        /// </summary>
        /// <param name="dto">The registration data for the plant type.</param>
        /// <exception cref="Exception">Thrown if a plant type with the same name already exists.</exception>
        public async Task AddPlantType(PlantTypeRegisterDTO dto)
        {
            // Business Rule: Plant type names must be unique to avoid confusion in reports
            if (await _plantTypeRepository.IsNameExists(dto.Name))
            {
                throw new Exception("The name for the plant type already exist");
            }

            var plantType = new PlantType
            {
                Name = dto.Name,
            };

            if (!string.IsNullOrEmpty(dto.Description))
                plantType.Description = dto.Description;

            await _plantTypeRepository.AddAsync(plantType);
        }

        /// <summary>
        /// Deletes a plant type only if it is not currently associated with any specific plant instances.
        /// </summary>
        /// <param name="Id">The GUID of the plant type to remove.</param>
        /// <exception cref="Exception">Thrown if there are plants still linked to this type (Referential Integrity).</exception>
        /// <exception cref="KeyNotFoundException">Thrown if the ID does not correspond to an existing type.</exception>
        public async Task DeletePlantType(Guid Id)
        {
            var plantType = await _plantTypeRepository.GetByIdAsync(Id);

            // Safety Check: Prevent orphaned plant records by blocking deletion of used types
            if (plantType != null && (plantType.Plants == null || plantType.Plants.Count == 0))
            {
                await _plantTypeRepository.DeleteAsync(plantType.Id);
            }
            else if (plantType != null && plantType.Plants.Any())
            {
                throw new Exception("Cannot delete plant type with associated plants.");
            }
            else
            {
                throw new KeyNotFoundException("Plant type not found.");
            }
        }

        /// <summary>
        /// Retrieves all plant types from the database and maps them to DTOs.
        /// </summary>
        public async Task<List<PlantTypeDTO>> GetAllPlantTypes()
        {
            var plantTypes = await _plantTypeRepository.GetAllAsync();
            return _mapper.Map<List<PlantTypeDTO>>(plantTypes);
        }

        /// <summary>
        /// Retrieves a specific plant type by its identifier.
        /// </summary>
        /// <exception cref="KeyNotFoundException">Thrown if the plant type is not found.</exception>
        public async Task<PlantTypeDTO> GetPlantTypeById(Guid Id)
        {
            var plantType = await _plantTypeRepository.GetByIdAsync(Id);
            if (plantType == null)
                throw new KeyNotFoundException("Plant type not found.");
            return _mapper.Map<PlantTypeDTO>(plantType);
        }

        /// <summary>
        /// Updates the metadata of an existing plant type.
        /// </summary>
        /// <remarks>
        /// If the name is changed, it re-validates uniqueness against other existing records.
        /// </remarks>
        /// <param name="Id">The ID of the type to update.</param>
        /// <param name="dto">The updated values.</param>
        public async Task UpdatePlantType(Guid Id, PlantTypeUpdateDTO dto)
        {
            var plantType = await _plantTypeRepository.GetByIdAsync(Id);
            if (plantType == null)
                throw new KeyNotFoundException("Plant type not found.");

            // Update description or clear it if null/empty
            if (!string.IsNullOrEmpty(dto.Description))
                plantType.Description = dto.Description;
            else
            {
                plantType.Description = null;
            }

            // Handle name change with uniqueness validation
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