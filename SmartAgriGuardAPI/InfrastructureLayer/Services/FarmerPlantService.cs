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
    public class FarmerPlantService : IFarmerPlantService
    {
        private readonly IFarmerPlantRepository _farmerPlantRepository;
        private readonly IPlantRepository _plantRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public FarmerPlantService(IFarmerPlantRepository farmerPlantRepository, IPlantRepository plantRepository,IUserRepository userRepository,IMapper mapper)
        {
            _farmerPlantRepository = farmerPlantRepository;
            _plantRepository = plantRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<List<PlantDTO>> GetAssignedPlantsForFarmer(Guid farmerId)
        {
            var plants = await _plantRepository.GetAssignedPlantsByFarmerIdAsync(farmerId);
            if (plants == null || !plants.Any())
                throw new KeyNotFoundException("No plants assigned to this farmer");
            return _mapper.Map<List<PlantDTO>>(plants);

        }

        public async Task UpdateFarmerPlantAssignment(Guid farmerId, FarmerPlantDTO farmerPlantDTO)
        {
            var farmer = await _userRepository.GetFarmerWithPlants(farmerId);
            if (farmer == null)
                throw new KeyNotFoundException("farmer not found");

            var requestedPlantIds = farmerPlantDTO.assignedPlants ?? new List<Guid>();

            var toRemove = farmer.FarmerPlants
                                 .Where(fp => !requestedPlantIds.Contains(fp.PlantId))
                                 .ToList();

            foreach (var fp in toRemove)
            {
                farmer.FarmerPlants.Remove(fp);
            }

            foreach (var plantId in farmerPlantDTO.assignedPlants)
            {
                if (farmer.FarmerPlants.Any(fp => fp.PlantId == plantId))
                    continue;

                var farmerPlant = new FarmerPlant
                {
                    FarmerId = farmerId,
                    PlantId = plantId,
                    AssignedAt = DateTime.UtcNow,
                };
                farmer.FarmerPlants.Add(farmerPlant);
                
            }

            await _userRepository.UpdateUserAsync(farmer);      
        }
    }
}
