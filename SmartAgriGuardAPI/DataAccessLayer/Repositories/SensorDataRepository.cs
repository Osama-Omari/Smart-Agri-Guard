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
    public class SensorDataRepository : ISensorDataRepository
    {
        private readonly SmartAgriGuardDbContext _context;

        public SensorDataRepository(SmartAgriGuardDbContext context)
        {
            _context = context;
        }
        
        public async Task<SensorData> AddAsync(SensorData sensorData)
        {
            try
            {
                sensorData.Id = Guid.NewGuid();
                sensorData.Timestamp = DateTime.UtcNow;

                await _context.SensorData.AddAsync(sensorData);
                await _context.SaveChangesAsync();
                return sensorData;
            }
            catch (Exception ex) { throw new Exception("Error while adding sensor"); }
        }

       
        public async Task<SensorData?> GetByIdAsync(Guid id)
        {
            try
            {
                return await _context.SensorData
                    .Include(s => s.Plant)
                    .FirstOrDefaultAsync(s => s.Id == id);
            }
            catch (Exception ex) { throw new Exception("error while getting sensor"); }

        }

        
        
        public async Task<IEnumerable<SensorData>> GetByPlantIdAsync(Guid plantId)
        {
            try
            {
                return await _context.SensorData
                    .Include(s => s.Plant)
                    .Where(s => s.PlantId == plantId)
                    .OrderByDescending(s => s.Timestamp)
                    .ToListAsync();
            }
            catch (Exception ex) { throw new Exception("Error while gettind a sensor"); }
        }


       
        public void RemoveRange(IEnumerable<SensorData> sensorData)
        {
             _context.SensorData.RemoveRange(sensorData);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public Task<SensorData?> GetLatestByPlantIdAsync(Guid plantId)
        {
            try
            {
                return _context.SensorData
                    .Where(s => s.PlantId == plantId)
                    .OrderByDescending(s => s.Timestamp)
                    .FirstOrDefaultAsync();
            }
            catch(Exception ex)
            {
                throw new Exception($"Error while getting the last sensors data: {ex.Message}");
            }
        }

        public async Task<List<SensorData>> GetByPlantIdAndDateRangeAsync(Guid plantId, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            try
            {
                return await _context.SensorData
                    .Where(sd => sd.PlantId == plantId && sd.Timestamp >= startDate && sd.Timestamp <= endDate)
                    .OrderBy(sd => sd.Timestamp)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while getting sensors data in range: {ex.Message}");
            }

        }
    }
}
    