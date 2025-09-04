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
    public class FarmerPlantRepository : IFarmerPlantRepository
    {
        private readonly SmartAgriGuardDbContext _context;

        public FarmerPlantRepository(SmartAgriGuardDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(FarmerPlant obj)
        {
            try
            {
                await _context.AddAsync(obj);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error Happend While Adding a farmer plant");
            }

        }
        public async Task<List<FarmerPlant>> GetPlantsByFarmerIdAsync(Guid farmerId)
        {
            return await _context.FarmerPlants
                .Include(fp => fp.Plant)
                .Where(fp => fp.FarmerId == farmerId)
                .ToListAsync();
        }

        public async Task RemoveAsync(Guid farmerId, Guid plantId)
        {
            var entry = await _context.FarmerPlants
                .FirstOrDefaultAsync(fp => fp.FarmerId == farmerId && fp.PlantId == plantId);

            if (entry != null)
            {
                _context.FarmerPlants.Remove(entry);
                await _context.SaveChangesAsync();
            }
        }
    }
}
