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

        
        public async Task<IEnumerable<SensorData>> GetAllAsync()
        {
            try
            {
                return await _context.SensorData
                    .Include(s => s.Plant)
                    .ToListAsync();
            }
            catch (Exception ex) { throw new Exception("Error while getting all sensors"); }
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

       
        public async Task<SensorData?> GetLatestSensorDataByPlantIdAsync(Guid plantId)
        {
            try
            {
                return await _context.SensorData
                    .Where(s => s.PlantId == plantId)
                    .OrderByDescending(s => s.Timestamp)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex) { throw new Exception("Error while getting the last sensor"); }
        }

        public async Task<SensorData> UpdateAsync(SensorData sensorData) 
        {
            try
            {
                var existing = await _context.SensorData.FindAsync(sensorData.Id);

                if (existing == null)
                    throw new KeyNotFoundException("Sensor data not found");

                existing.Temperature = sensorData.Temperature;
                existing.Humidity = sensorData.Humidity;
                existing.SoilMoisture = sensorData.SoilMoisture;
                existing.Nitrogen = sensorData.Nitrogen;
                existing.Phosphorus = sensorData.Phosphorus;
                existing.Potassium = sensorData.Potassium;
                existing.Ph = sensorData.Ph;
                existing.Timestamp = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return existing;
            }
            catch (Exception ex) { throw new Exception("Error while updating sensor"); }
        }

        
        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var sensorData = await _context.SensorData.FindAsync(id);

                if (sensorData == null)
                    return false;

                _context.SensorData.Remove(sensorData);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) { throw new Exception("Error while Deleting a Sensor"); }
        }

        public void RemoveRange(IEnumerable<SensorData> sensorData)
        {
             _context.SensorData.RemoveRange(sensorData);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
    