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
    public class PlantTypeRepository : IPlantTypeRepository
    {
        private readonly SmartAgriGuardDbContext _context;

        public PlantTypeRepository(SmartAgriGuardDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PlantType plantType)
        {
            try
            {
                await _context.PlantTypes.AddAsync(plantType);
                await _context.SaveChangesAsync();
            }
            catch { throw new Exception("Error while adding a new plant type "); }
            
        }

        public async Task DeleteAsync(Guid id)
        {
            try
            {
                var plantType = await _context.PlantTypes.FindAsync(id);
                if (plantType != null)
                {
                    _context.PlantTypes.Remove(plantType);
                    await _context.SaveChangesAsync();
                }
            }
            catch
            {
                throw new Exception("Erreor while deleting the plant type");
            }
        }

        public async Task<List<PlantType>> GetAllAsync()
        {
            try
            {
                return await _context.PlantTypes
                    .Include(pt => pt.Plants)
                    .ToListAsync();
            }
            catch
            {
                throw new Exception("error whie getting all plant types");
            }
        }

        public async Task<PlantType?> GetByIdAsync(Guid id)
        {
            try
            {
                return await _context.PlantTypes
                    .Include(pt => pt.Plants)
                    .FirstOrDefaultAsync(pt => pt.Id == id);
            }
            catch
            {
                throw new Exception("error while getting plant type");
            }
        }


        public async Task UpdateAsync(PlantType plantType)
        {
            try
            {
                _context.PlantTypes.Update(plantType);
                await _context.SaveChangesAsync();
            }
            catch { throw new Exception("Error while updating the plantType"); }

        }
    }
}
