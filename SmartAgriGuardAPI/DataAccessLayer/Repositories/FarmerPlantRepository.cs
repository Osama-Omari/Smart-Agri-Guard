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
                throw new Exception($"Error Happend While Adding a farmer plant: {ex.Message}");
            }

        }

        public async Task AddAsync(List<FarmerPlant> assignments)
        {
            try
            {
                await _context.FarmerPlants.AddRangeAsync(assignments);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error Happend While Adding a farmer plant: {ex.Message}");
            }
        }


    }
}
