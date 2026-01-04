using DataAccessLayer.Data;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    /// <summary>
    /// Repository for managing Plant entities. 
    /// Handles complex queries involving greenhouse associations, farmer assignments, and historical sensor metrics.
    /// </summary>
    public class PlantRepository : IPlantRepository
    {
        private readonly SmartAgriGuardDbContext _context;

        public PlantRepository(SmartAgriGuardDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Persists a new plant record to the database.
        /// </summary>
        public async Task AddAsync(Plant plant)
        {
            try
            {
                await _context.Plants.AddAsync(plant);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while adding a plant: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves a plant by its unique identifier with basic navigation properties.
        /// </summary>
        public async Task<Plant?> GetPlantById(Guid plantId)
        {
            try
            {
                return await _context.Plants
                    .Include(p => p.Greenhouse)
                    .Include(p => p.PlantType)
                    .Include(p => p.FarmerPlants)
                    .FirstOrDefaultAsync(p => p.Id == plantId);
            }
            catch (Exception ex) { throw new Exception($"Error while getting the plant: {ex.Message}"); }
        }

        /// <summary>
        /// Retrieves all plants located within a specific greenhouse.
        /// </summary>
        public async Task<List<Plant>> GetAllGreenhousePlantsAsync(Guid greenhouseId)
        {
            try
            {
                return await _context.Plants
                    .Include(p => p.Greenhouse)
                    .Include(p => p.PlantType)
                    .Include(p => p.FarmerPlants)
                    .Where(p => p.GreenhouseId == greenhouseId)
                    .ToListAsync();
            }
            catch (Exception ex) { throw new Exception($"Error while getting greenhouse plants: {ex.Message}"); }
        }

        /// <summary>
        /// Retrieves all plants in a greenhouse along with their full sensor data history.
        /// </summary>
        /// <remarks>
        /// Use with caution for greenhouses with large histories to avoid memory overhead.
        /// </remarks>
        public async Task<List<Plant>> GetAllGreenhousePlantsWithMetrics(Guid greenhouseId)
        {
            try
            {
                return await _context.Plants
                    .Include(p => p.Greenhouse)
                    .Include(p => p.PlantType)
                    .Include(p => p.FarmerPlants)
                    .Include(p => p.SensorData) // Loads historical telemetry
                    .Where(p => p.GreenhouseId == greenhouseId)
                    .ToListAsync();
            }
            catch (Exception ex) { throw new Exception($"Error while getting plants with metrics: {ex.Message}"); }
        }

        /// <summary>
        /// Updates an existing plant's metadata.
        /// </summary>
        public async Task UpdateAsync(Plant plant)
        {
            try
            {
                _context.Plants.Update(plant);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception($"Error while updating plant: {ex.Message}"); }
        }

        /// <summary>
        /// Permanently removes a plant record from the database.
        /// </summary>
        public async Task DeleteAsync(Guid plantId)
        {
            try
            {
                var plant = await _context.Plants.FindAsync(plantId);
                if (plant != null)
                {
                    _context.Plants.Remove(plant);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex) { throw new Exception($"Error while deleting plant: {ex.Message}"); }
        }

        /// <summary>
        /// Retrieves a plant and specifically drills down into the farmers assigned to it.
        /// </summary>
        public async Task<Plant?> GetPlantWithFarmerPlant(Guid plantId)
        {
            try
            {
                return await _context.Plants
                    .Include(fp => fp.FarmerPlants)
                        .ThenInclude(fp => fp.Farmer)
                    .Include(p => p.Greenhouse)
                    .FirstOrDefaultAsync(x => x.Id == plantId);
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        /// <summary>
        /// Filters the global plant list to only those assigned to a specific farmer.
        /// Useful for the "My Dashboard" view in the mobile app.
        /// </summary>
        public async Task<List<Plant>> GetAssignedPlantsByFarmerIdAsync(Guid farmerId)
        {
            try
            {
                return await _context.Plants
                    .Include(p => p.PlantType)
                    .Include(p => p.Greenhouse)
                    .Include(p => p.FarmerPlants)
                    .Include(p => p.SensorData)
                    .Where(p => p.FarmerPlants.Any(fp => fp.FarmerId == farmerId))
                    .ToListAsync();
            }
            catch (Exception ex) { throw new Exception($"Error while getting farmer's assigned plants: {ex.Message}"); }
        }

        /// <summary>
        /// Retrieves all plants and projects the identity of assigned farmers.
        /// </summary>
        public async Task<List<Plant>> GetPlantsWithAssignedFarmers(Guid GreenhouseId)
        {
            try
            {
                return await _context.Plants
                    .Include(p => p.FarmerPlants)
                        .ThenInclude(fp => fp.Farmer)
                    .ToListAsync();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}