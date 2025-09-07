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
    public class FarmerPlantService : IFarmerPlantService
    {
        private readonly IFarmerPlantRepository _farmerPlantRepository;
        private readonly IPlantRepository _plantRepository;
        private readonly IUserRepository _userRepository;

        public FarmerPlantService(IFarmerPlantRepository farmerPlantRepository, IPlantRepository plantRepository,IUserRepository userRepository)
        {
            _farmerPlantRepository = farmerPlantRepository;
            _plantRepository = plantRepository;
            _userRepository = userRepository;
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
