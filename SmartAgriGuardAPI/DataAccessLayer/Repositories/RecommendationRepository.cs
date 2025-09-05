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
    public class RecommendationRepository : IRecommendationRepository
    {
        private readonly SmartAgriGuardDbContext _context;

        public RecommendationRepository(SmartAgriGuardDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Recommendation recommendation)
        {
            try
            {
                await _context.Recommendations.AddAsync(recommendation);
                await _context.SaveChangesAsync();
            }
            catch { throw new Exception("Error whie adding recommendation "); }
        }

        public async Task DeleteAsync(Guid id)
        {
          var reco= await _context.Recommendations.FindAsync(id);
            if (reco != null)
            {
                _context.Recommendations.Remove(reco);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Recommendation>> GetAllAsync()
        {
           return await _context.Recommendations
                .Include(r=>r.Plant)
                .ToListAsync();
        }

        public async Task<Recommendation?> GetByIdAsync(Guid id)
        {
            return await _context.Recommendations
                .Include(r => r.Plant)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<List<Recommendation>> GetByPlantIdAsync(Guid plantId)
        {
            return await _context.Recommendations
                .Include(r => r.Plant)
                .Where(r => r.PlantId == plantId)
                .ToListAsync();
        }

        public async Task UpdateAsync(Recommendation recommendation)
        {
            _context.Recommendations.Update(recommendation);
            await _context.SaveChangesAsync();
        }
    }
}
