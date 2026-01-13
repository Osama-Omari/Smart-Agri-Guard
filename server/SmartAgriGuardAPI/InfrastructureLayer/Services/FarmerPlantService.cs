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
    /// <summary>
    /// Service managing the assignments between Farmers and Plants.
    /// Handles the operational mapping of responsibility within a greenhouse.
    /// </summary>
    public class FarmerPlantService : IFarmerPlantService
    {
        private readonly IFarmerPlantRepository _farmerPlantRepository;
        private readonly IPlantRepository _plantRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public FarmerPlantService(IFarmerPlantRepository farmerPlantRepository, IPlantRepository plantRepository, IUserRepository userRepository, IMapper mapper)
        {
            _farmerPlantRepository = farmerPlantRepository;
            _plantRepository = plantRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Assigns multiple farmers to a specific plant.
        /// </summary>
        /// <remarks>
        /// This method performs delta-checking: it identifies which farmers in the list are 
        /// not already assigned to the plant and only creates new records for them.
        /// </remarks>
        /// <param name="plantId">The GUID of the plant.</param>
        /// <param name="dto">A DTO containing a list of Farmer GUIDs.</param>
        /// <exception cref="ArgumentException">Thrown if the farmer list is empty.</exception>
        /// <exception cref="KeyNotFoundException">Thrown if the plant or one of the farmers is not found.</exception>
        public async Task AssignFarmers(Guid plantId, AssignFarmerDTO dto)
        {
            if (dto == null || dto.farmersIds == null || !dto.farmersIds.Any())
                throw new ArgumentException("Farmers list cannot be empty.");

            var plant = await _plantRepository.GetPlantWithFarmerPlant(plantId);

            if (plant == null)
                throw new KeyNotFoundException("Plant not found.");

            // Verify all provided farmer IDs exist in the database
            var farmers = await _userRepository.GetFarmersByIdsAsync(dto.farmersIds);

            if (farmers.Count != dto.farmersIds.Count)
                throw new KeyNotFoundException("One or more farmers were not found.");

            // Get IDs of farmers already assigned to this plant to avoid duplicates
            var assignedFarmerIds = plant.FarmerPlants
                .Select(fp => fp.FarmerId)
                .ToHashSet();

            // Filter for new assignments only
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

        /// <summary>
        /// Retrieves all plants assigned to a specific farmer, including localized latest metrics.
        /// </summary>
        /// <param name="farmerId">The GUID of the farmer.</param>
        /// <param name="userTimeZoneId">The timezone identifier used for timestamp conversion.</param>
        /// <returns>A list of plants with their associated sensor metrics.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if no plants are assigned to the farmer.</exception>
        public async Task<List<PlantWithMetricsDTO>> GetAssignedPlantsForFarmer(Guid farmerId, string userTimeZoneId)
        {
            var plants = await _plantRepository.GetAssignedPlantsByFarmerIdAsync(farmerId);
            if (plants == null || !plants.Any())
                throw new KeyNotFoundException("No plants assigned to this farmer");

            var dtos = _mapper.Map<List<PlantWithMetricsDTO>>(plants);

            // Resolve TimeZone safely
            TimeZoneInfo tz;
            try
            {
                tz = TZConvert.GetTimeZoneInfo(userTimeZoneId);
            }
            catch
            {
                tz = TimeZoneInfo.Utc;
            }

            // Localize timestamps for the 'LatestMetrics' of each plant
            foreach (var plant in dtos.Where(p => p.LatestMetrics != null))
            {
                plant.LatestMetrics.Timestamp = TimeZoneInfo.ConvertTime(plant.LatestMetrics.Timestamp, tz);
            }

            return dtos;
        }

        /// <summary>
        /// Removes a farmer's assignment from a specific plant.
        /// </summary>
        /// <param name="plantId">The GUID of the plant.</param>
        /// <param name="farmerId">The GUID of the farmer.</param>
        /// <exception cref="KeyNotFoundException">Thrown if the farmer or the specific assignment is not found.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the farmer has no assignments at all.</exception>
        public async Task UnAssignFarmerAsync(Guid plantId, Guid farmerId)
        {
            var farmer = await _userRepository.GetFarmerWithPlants(farmerId);
            if (farmer == null)
                throw new KeyNotFoundException("farmer not found");

            if (farmer.FarmerPlants == null || !farmer.FarmerPlants.Any())
                throw new InvalidOperationException("This farmer has no assigned plants.");

            // Find the specific join record
            var farmerPlant = farmer.FarmerPlants
                .FirstOrDefault(fp => fp.PlantId == plantId);

            if (farmerPlant == null)
                throw new KeyNotFoundException("This plant is not assigned to the farmer.");

            // Remove the link (many-to-many relationship)
            farmer.FarmerPlants.Remove(farmerPlant);

            await _userRepository.UpdateAsync(farmer);
        }
    }
}