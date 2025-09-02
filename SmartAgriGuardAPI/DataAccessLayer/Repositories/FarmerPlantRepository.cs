using DataAccessLayer.Data;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
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
           await _context.AddAsync(obj);
           await _context.SaveChangesAsync();

        }
    }
}
