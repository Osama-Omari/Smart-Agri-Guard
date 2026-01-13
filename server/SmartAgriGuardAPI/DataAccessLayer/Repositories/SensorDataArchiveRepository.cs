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
    /// Repository managing historical sensor telemetry (Archived Data).
    /// Used for long-term data analysis, reporting, and auditing without slowing down real-time operations.
    /// </summary>
    public class SensorDataArchiveRepository : ISensorDataArchiveRepository
    {
        private readonly SmartAgriGuardDbContext _context;

        public SensorDataArchiveRepository(SmartAgriGuardDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Persists a new archive record to the database.
        /// </summary>
        public async Task AddAsync(SensorDataArchive archive)
        {
            try
            {
                await _context.SensorDataArchives.AddAsync(archive);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception("Error while adding Sensor Archive"); }
        }

        /// <summary>
        /// Retrieves a specific archive entry by its ID, including the associated Plant metadata.
        /// </summary>
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

        /// <summary>
        /// Retrieves all archived sensor data entries. 
        /// Use with caution as this table is expected to be very large.
        /// </summary>
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

        /// <summary>
        /// Retrieves the complete sensor history for a specific plant from the archive.
        /// </summary>
        public async Task<List<SensorDataArchive>> GetByPlantIdAsync(Guid plantId)
        {
            try
            {
                return await _context.SensorDataArchives
                    .Where(a => a.PlantId == plantId)
                    .Include(a => a.Plant)
                    .ToListAsync();
            }
            catch (Exception ex) { throw new Exception("Error while getting Sensor Archive by plant id "); }
        }

        /// <summary>
        /// Updates an existing archive record.
        /// </summary>
        public async Task UpdateAsync(SensorDataArchive archive)
        {
            try
            {
                _context.SensorDataArchives.Update(archive);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception("Error while updating archive"); }
        }

        /// <summary>
        /// Physically removes a specific archive record by ID.
        /// </summary>
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
            catch (Exception ex) { throw new Exception("Error while deleting archive"); }
        }

        /// <summary>
        /// Prepares a collection of archive records for deletion. 
        /// Note: Must call SaveChangesAsync() to commit the removal.
        /// </summary>
        public async Task  RemoveRange(IEnumerable<SensorDataArchive> archives)
        {
            try
            {
                _context.SensorDataArchives.RemoveRange(archives);
                _context.SaveChanges();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        /// <summary>
        /// Commits all pending changes (Add/Update/Delete) to the archive table.
        /// </summary>
        public async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        /// <summary>
        /// Core query method for historical reporting. Retrieves sensor data for a plant
        /// within a specific time window, ordered chronologically.
        /// </summary>
        /// <param name="plantId">Target plant GUID.</param>
        /// <param name="startDate">Beginning of the history window.</param>
        /// <param name="endDate">End of the history window.</param>
        public Task<List<SensorDataArchive>> GetByPlantIdAndDateRangeAsync(Guid plantId, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            try
            {
                return _context.SensorDataArchives
                    .Where(sd => sd.PlantId == plantId && sd.Timestamp >= startDate && sd.Timestamp <= endDate)
                    .OrderBy(sd => sd.Timestamp)
                    .ToListAsync();
            }
            catch (Exception ex) { throw new Exception($"Error while getting archive by date range: {ex.Message}"); }
        }

        

        public async Task AddRange(IEnumerable<SensorDataArchive> archives)
        {
            try
            {
                await _context.SensorDataArchives.AddRangeAsync(archives);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw new Exception($"Error while adding Sensor Archive range: {ex.Message}");
            }
        }

        public async Task<List<SensorDataArchive>> GetSensorDataArchivesOlderThan(DateTimeOffset cutoffDate)
        {
            try
            {
                return await _context.SensorDataArchives
                    .Where(sd => sd.Timestamp < cutoffDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while getting sensor data archives older than cutoff date: {ex.Message}");
            }
        }
    }
}