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
    public class PlantRepository : IPlantRepository
    {
        private readonly SmartAgriGuardDbContext _context;
        public PlantRepository(SmartAgriGuardDbContext context)
        {
            _context = context;
        }
         public async Task AddAsync(Plant plant)
        {
            try
            {
                await _context.Plants.AddAsync(plant);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error While adding a plant: {ex.Message}");
            }
        }
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
        public async Task<List<Plant>> GetAllGreenhousePlantsAsync(Guid greenhouseId)
        {
            try
            {
                return await _context.Plants
                    .Include(p => p.Greenhouse)
                    .Include(p => p.PlantType)
                    .Include(p => p.FarmerPlants)
                    .Where(p=> p.GreenhouseId == greenhouseId)
                    .ToListAsync();
            }
            catch (Exception ex) { throw new Exception($"Error while getting the plant: {ex.Message}"); }
        }

        public async Task<List<Plant>> GetAllGreenhousePlantsWithMetrics(Guid greenhouseId)
        {
            try
            {
                return await _context.Plants
                    .Include(p => p.Greenhouse)
                    .Include(p => p.PlantType)
                    .Include(p => p.FarmerPlants)
                    .Include(p => p.SensorData)
                    .Where(p => p.GreenhouseId == greenhouseId)
                    .ToListAsync();
            }
            catch (Exception ex) { throw new Exception($"Error while getting the plant with metrics: {ex.Message}"); }
        }
        public async Task UpdateAsync(Plant plant)
        {
            try
            {
                _context.Plants.Update(plant);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception($"Error while updating a plant: {ex.Message}"); }
        }
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
            catch (Exception ex) { throw new Exception($"Error while deleting a plant: {ex.Message}"); }

        }

        public async Task<Plant?> GetPlantWithFarmerPlant(Guid plantId)
        {
            try
            {
                return await _context.Plants.Include(fp=>fp.FarmerPlants)
                    .ThenInclude(fp=>fp.Farmer)
                    .Include(p=>p.Greenhouse)
                    .FirstOrDefaultAsync(x=>x.Id == plantId);
            }
            catch(Exception ex) { throw new Exception(ex.Message); }    
        }

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
            catch (Exception ex) { throw new Exception($"Error while getting assigned plants for the farmer: {ex.Message}"); }
        }

        public async Task<List<Plant>> GetPlantsWithAssignedFarmers(Guid GreenhouseId)
        {
            try
            {
                return await _context.Plants
                    .Include(p=> p.FarmerPlants)
                    .ThenInclude(fp=> fp.Farmer)
                    .ToListAsync();
                     

            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message); 
            }
        }

    }
}
