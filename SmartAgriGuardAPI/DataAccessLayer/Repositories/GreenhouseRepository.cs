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
    public class GreenhouseRepository : IGreenhouseRepository
    {
        private readonly SmartAgriGuardDbContext _context;
        public GreenhouseRepository(SmartAgriGuardDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Greenhouse greenhouse)
        {
            try
            {
                await _context.Greenhouses.AddAsync(greenhouse);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception($"Error while adding GreenHouse: {ex.Message}"); }
        }

        

        public async Task DeleteAsync(Guid id)
        {
            try
            {
                var greenhouse = await _context.Greenhouses.FindAsync(id);
                if (greenhouse != null)
                {
                    _context.Greenhouses.Remove(greenhouse);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex) { throw new Exception($"Error while deleting greenhouse: {ex.Message}"); }
        }

        public async Task<List<Greenhouse>> GetAllAsync()
        {
            try
            {
                return await _context.Greenhouses
                     .Include(G => G.Plants)
                     .Include(G => G.Farmers)
                     .ToListAsync();
            }
            catch (Exception ex) { throw new Exception($"Error while retriving greenhouses: {ex.Message}"); }
        }

        public async Task<Greenhouse?> GetGreenhouseById(Guid id)
        {
            try
            {
                return await _context.Greenhouses
                    .Include(x=> x.Farmers)
                    .Include(x=> x.Manager)
                    .ThenInclude(x => x.FarmerPlants)
                    .ThenInclude(x=> x.Plant)
                   . FirstOrDefaultAsync(x => x.Id == id)
                    ;
            }
            catch(Exception ex) {
                throw new Exception($"An error happend while retrieving the greenhouse: {ex.Message}");
            }
        }

        public async Task<List<Greenhouse>?> GetGreenhousesByManagerIdAsync(Guid managerId)
        {
            try
            {
                return  await _context.Greenhouses
                    .Where(g => g.ManagerId == managerId)
                    .Include(g => g.Plants)
                    .Include(g => g.Farmers)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while retriving greenhouse by manager id: {ex.Message}");

            }
        }

        public async Task<List<Greenhouse>?> GetGreenhousesWithoutManagerAsync()
        {
            try
            {
                return await _context.Greenhouses
                    .Where(g => g.ManagerId == null)
                    .Include(g => g.Plants)
                    .Include(g => g.Farmers)
                    .ToListAsync();

            }
            catch (Exception ex)
            {
                throw new Exception($"Error while retriving greenhouses without manager: {ex.Message}");
            }
        }

        public async Task UpdateAsync(Greenhouse greenhouse)
        {
            try
            {
                _context.Greenhouses.Update(greenhouse);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception($"Error while Updating the GreenHouse : {ex.Message}"); }
        }
    }
}
