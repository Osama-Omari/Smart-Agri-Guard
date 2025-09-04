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
            catch (Exception ex) { throw new Exception("Error while adding GreenHouse"); }
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
            catch (Exception ex) { throw new Exception("Error while deleting greenhouse"); }
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
            catch (Exception ex) { throw new NotImplementedException(); }
        }

        public async Task<Greenhouse?> GetGreenhouseById(Guid id)
        {
            try
            {
                return await _context.Greenhouses
                    .Include(x=>x.Plants)
                    .Include(x=> x.Farmers)
                   . FirstOrDefaultAsync(x => x.Id == id)
                    ;
            }
            catch(Exception ex) {
                throw new Exception($"An error happend while retrieving the greenhouse: {ex.Message}");
            }
        }

        public async Task UpdateAsync(Greenhouse greenhouse)
        {
            try
            {
                _context.Greenhouses.Update(greenhouse);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception("Error while Updating the GreenHouse"); }
        }
    }
}
