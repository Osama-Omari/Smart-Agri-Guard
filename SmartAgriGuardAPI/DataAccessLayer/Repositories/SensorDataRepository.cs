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
    /// <summary>
    /// Repository for managing real-time sensor telemetry.
    /// Optimized for high-frequency inserts and time-sensitive retrieval of environmental metrics.
    /// </summary>
    public class SensorDataRepository : ISensorDataRepository
    {
        private readonly SmartAgriGuardDbContext _context;

        public SensorDataRepository(SmartAgriGuardDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Records new telemetry data.
        /// to ensure data consistency across different sensor hardware.
        /// </summary>
        /// <param name="sensorData">The telemetry object containing soil, temperature, and nutrient metrics.</param>
        public async Task<SensorData> AddAsync(SensorData sensorData)
        {
            try
            {
                await _context.SensorData.AddAsync(sensorData);
                await _context.SaveChangesAsync();
                return sensorData;
            }
            catch (Exception ex) { throw new Exception($"Error while adding sensor data: {ex.Message}"); }
        }

        /// <summary>
        /// Retrieves a specific sensor reading by its ID, including the related Plant profile.
        /// </summary>
        public async Task<SensorData?> GetByIdAsync(Guid id)
        {
            try
            {
                return await _context.SensorData
                    .Include(s => s.Plant)
                    .FirstOrDefaultAsync(s => s.Id == id);
            }
            catch (Exception ex) { throw new Exception($"Error while retrieving specific sensor record: {ex.Message}"); }
        }

        /// <summary>
        /// Retrieves all active telemetry for a specific plant, ordered from newest to oldest.
        /// </summary>
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
            catch (Exception ex) { throw new Exception($"Error while retrieving plant telemetry history: {ex.Message}"); }
        }

        /// <summary>
        /// Prepares a collection of sensor records for removal (e.g., during a data archiving process).
        /// </summary>
        public async Task RemoveRange(IEnumerable<SensorData> sensorData)
        {
            try
            {
                _context.SensorData.RemoveRange(sensorData);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception(ex.Message);
            }


        }

        /// <summary>
        /// Commits all pending telemetry changes to the database.
        /// </summary>
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Fetches the single most recent reading for a plant. 
        /// This is the primary query for live monitoring dashboards.
        /// </summary>
        public Task<SensorData?> GetLatestByPlantIdAsync(Guid plantId)
        {
            try
            {
                return _context.SensorData
                    .Where(s => s.PlantId == plantId)
                    .OrderByDescending(s => s.Timestamp)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while getting the latest sensor reading: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves a time-series window of telemetry data. 
        /// Essential for rendering trend charts for the last 24-48 hours.
        /// </summary>
        /// <param name="plantId">The target plant GUID.</param>
        /// <param name="startDate">Start of the window (UTC).</param>
        /// <param name="endDate">End of the window (UTC).</param>
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
                throw new Exception($"Error while retrieving telemetry range: {ex.Message}");
            }
        }

        public async Task<List<SensorData>> GetSensorDataOlderThan(DateTimeOffset cutoffDate)
        {
            try
            {
                return await _context.SensorData
                    .Where(sd => sd.Timestamp < cutoffDate)
                    .ToListAsync();

            }
            catch (Exception ex)
            {
                throw new Exception("Error while retrieving old sensor data: " + ex.Message);
            }
        }
    }
}