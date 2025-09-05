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
    public class SensorDataArchiveRepository : ISensorDataArchiveRepository
    {
        private readonly SmartAgriGuardDbContext _context;

        public SensorDataArchiveRepository(SmartAgriGuardDbContext context)
        {
            _context = context;
        }

      public async Task AddAsync(SensorDataArchive archive)
        {
            try
            {
                await _context.SensorDataArchives.AddAsync(archive);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception("Erroe while adding Sensor Archive"); }
        }

        public async Task<SensorDataArchive?> GetByIdAsync(Guid id)
        {
            try
            {
                return await _context.SensorDataArchives
                    .Include(a => a.Plant)
                    .FirstOrDefaultAsync(a => a.Id == id);
            }
            catch (Exception ex) { throw new Exception("Error while getting Sensor Archive by id"); }
        }

        public async Task<List<SensorDataArchive>> GetAllAsync()
        {
            try
            {
                return await _context.SensorDataArchives
                    .Include(a => a.Plant)
                    .ToListAsync();
            }
            catch (Exception ex) { throw new Exception("Error while getting all Sensor Archive "); }
        }

        public async Task<List<SensorDataArchive>> GetByPlantIdAsync(Guid plantId)
        {
            try
            {
                return await _context.SensorDataArchives
                    .Where(a => a.PlantId == plantId)
                    .Include(a => a.Plant)
                    .ToListAsync();
            }
            catch (Exception ex) { throw new Exception("Error while getting Sensor Archive by plan id "); }
        }

        public async Task<List<SensorDataArchive>> GetByDateRangeAsync(Guid plantId, DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _context.SensorDataArchives
                    .Where(a => a.PlantId == plantId && a.Timestamp >= startDate && a.Timestamp <= endDate)
                    .Include(a => a.Plant)
                    .ToListAsync();
            }
            catch (Exception ex) { throw new Exception("Error while getting archieve by it date "); }
        }

        public async Task UpdateAsync(SensorDataArchive archive)
        {
            try
            {
                _context.SensorDataArchives.Update(archive);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception("Error while updatig arhieve"); }
        }

        public async Task DeleteAsync(Guid id)
        {
            try
            {
                var archive = await _context.SensorDataArchives.FindAsync(id);
                if (archive != null)
                {
                    _context.SensorDataArchives.Remove(archive);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex) { throw new Exception("Error while deleting archieve"); }
        }
    }
}