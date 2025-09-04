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
    public class PredictionRepository : IPredictionRepository
    {
        private readonly SmartAgriGuardDbContext _context;

        public PredictionRepository(SmartAgriGuardDbContext context)
        {
            _context = context;
        }

       
        public async Task AddAsync(Prediction prediction)
        {
            await _context.Predictions.AddAsync(prediction);
            await _context.SaveChangesAsync();
        }

       
        public async Task<Prediction?> GetByIdAsync(Guid id)
        {
            return await _context.Predictions
                .Include(p => p.Plant)  
                .FirstOrDefaultAsync(p => p.Id == id);
        }

      
        public async Task<List<Prediction>> GetAllAsync()
        {
            return await _context.Predictions
                .Include(p => p.Plant)
                .ToListAsync();
        }

     
        public async Task UpdateAsync(Prediction prediction)
        {
            _context.Predictions.Update(prediction);
            await _context.SaveChangesAsync();
        }

       
        public async Task DeleteAsync(Guid id)
        {
            var prediction = await _context.Predictions.FindAsync(id);
            if (prediction != null)
            {
                _context.Predictions.Remove(prediction);
                await _context.SaveChangesAsync();
            }
        }

     
       
    }
}
