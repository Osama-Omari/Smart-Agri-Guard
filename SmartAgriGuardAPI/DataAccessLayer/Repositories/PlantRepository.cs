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
                throw new Exception("Error While adding a plant");
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
            catch (Exception ex) { throw new Exception("Error while getting the plant"); }
        }
        public async Task<List<Plant>> GetAllGreenhousePlantsAsync(Guid greenhouseId)
        {
            try
            {
                return await _context.Plants
                    .Include(p => p.Greenhouse)
                    .Include(p => p.PlantType)
                    .Include(p => p.FarmerPlants)
                    .Where(p=> p.Id == greenhouseId)
                    .ToListAsync();
            }
            catch (Exception ex) { throw new Exception("Error while getting the plant"); }
        }
        public async Task UpdateAsync(Plant plant)
        {
            try
            {
                _context.Plants.Update(plant);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception("Error while updating a plant "); }
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
            catch (Exception ex) { throw new Exception("Error while deleting a plant "); }

        }

        public async Task<Plant> GetPlantWithFarmerPlant(Guid plantId)
        {
            try
            {
                return await _context.Plants.Include(fp=>fp.FarmerPlants).FirstOrDefaultAsync(x=>x.Id == plantId);

            }
            catch(Exception ex) { throw new Exception(ex.Message); }    
        }
    }
}
