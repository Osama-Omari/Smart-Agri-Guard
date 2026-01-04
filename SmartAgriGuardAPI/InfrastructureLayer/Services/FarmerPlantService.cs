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
using TimeZoneConverter;

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

        public async Task AssignFarmers(Guid plantId, AssignFarmerDTO dto)
        {
            if (dto == null || dto.farmersIds == null || !dto.farmersIds.Any())
                throw new ArgumentException("Farmers list cannot be empty.");

            var plant = await _plantRepository.GetPlantWithFarmerPlant(plantId);

            if (plant == null)
                throw new KeyNotFoundException("Plant not found.");

            var farmers = await _userRepository.GetFarmersByIdsAsync(dto.farmersIds);

            if (farmers.Count != dto.farmersIds.Count)
                throw new KeyNotFoundException("One or more farmers were not found.");

            var assignedFarmerIds = plant.FarmerPlants
                .Select(fp => fp.FarmerId)
                .ToHashSet();

            var newAssignments = farmers
                .Where(f => !assignedFarmerIds.Contains(f.Id))
                .Select(f => new FarmerPlant
                {
                    FarmerId = f.Id,
                    PlantId = plantId,
                    AssignedAt = DateTimeOffset.UtcNow
                })
                .ToList();

            if (!newAssignments.Any())
                return;

            await _farmerPlantRepository.AddAsync(newAssignments);

        }

        public async Task<List<PlantWithMetricsDTO>> GetAssignedPlantsForFarmer(Guid farmerId , string userTimeZoneId)
        {
            var plants = await _plantRepository.GetAssignedPlantsByFarmerIdAsync(farmerId);
            if (plants == null || !plants.Any())
                throw new KeyNotFoundException("No plants assigned to this farmer");
           

            var  dtos = _mapper.Map<List<PlantWithMetricsDTO>>(plants);

            TimeZoneInfo tz;
            try
            {
                tz = TZConvert.GetTimeZoneInfo(userTimeZoneId);
            }
            catch
            {
                tz = TimeZoneInfo.Utc;
            }

            foreach (var plant in dtos)
            {
                if (plant.LatestMetrics != null)
                {
                    plant.LatestMetrics.Timestamp = TimeZoneInfo.ConvertTime(plant.LatestMetrics.Timestamp, tz);
                }
            }

            return dtos;

        }

        public async Task UnAssignFarmerAsync(Guid plantId, Guid farmerId)
        {
            var farmer = await _userRepository.GetFarmerWithPlants(farmerId);
            if (farmer == null)
                throw new KeyNotFoundException("farmer not found");
            if (farmer.FarmerPlants == null || !farmer.FarmerPlants.Any())
                throw new InvalidOperationException("This farmer has no assigned plants.");
            var farmerPlant = farmer.FarmerPlants
            .FirstOrDefault(fp => fp.PlantId == plantId);

            if (farmerPlant == null)
                throw new KeyNotFoundException("This plant is not assigned to the farmer.");

            farmer.FarmerPlants.Remove(farmerPlant);

            await _userRepository.UpdateAsync(farmer);
        }

    }
}
