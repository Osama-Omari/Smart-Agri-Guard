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
    /// Repository responsible for managing system-level reports and facility notifications.
    /// Handles the persistence of greenhouse status logs and administrative alerts.
    /// </summary>
    public class SystemReportsRepository : ISystemReportsRepository
    {
        private readonly SmartAgriGuardDbContext _context;

        public SystemReportsRepository(SmartAgriGuardDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Persists a new system report entry to the database.
        /// </summary>
        /// <param name="systemReports">The report entity containing status details and greenhouse reference.</param>
        public async Task AddAsync(SystemReports systemReports)
        {
            try
            {
                await _context.SystemReports.AddAsync(systemReports);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the system report.", ex);
            }
        }

        /// <summary>
        /// Permanently removes a system report from the record.
        /// </summary>
        /// <param name="reportId">The unique GUID of the report to delete.</param>
        public async Task DeleteAsync(Guid reportId)
        {
            try
            {
                var report = await _context.SystemReports.FindAsync(reportId);
                if (report != null)
                {
                    _context.SystemReports.Remove(report);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the system report.", ex);
            }
        }

        /// <summary>
        /// Retrieves all system reports in the database, ordered by the most recent date.
        /// Eagerly loads the associated Greenhouse entity for context.
        /// </summary>
        /// <returns>A list of system reports including facility metadata.</returns>
        public async Task<List<SystemReports>> GetAllAsync()
        {
            try
            {
                return await _context.SystemReports
                    .OrderByDescending(x => x.ReportDate)
                    .Include(g => g.Greenhouse)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving system reports.", ex);
            }
        }

        /// <summary>
        /// Filters system reports for a specific greenhouse, sorted chronologically.
        /// </summary>
        /// <param name="greenhouseId">The GUID of the target greenhouse.</param>
        public async Task<List<SystemReports>> GetByGreenhouseIdAsync(Guid greenhouseId)
        {
            try
            {
                return await _context.SystemReports
                    .Where(r => r.GreenhouseId == greenhouseId)
                    .OrderByDescending(x => x.ReportDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving system reports.", ex);
            }
        }

        /// <summary>
        /// Retrieves a specific set of reports based on a list of IDs.
        /// Useful for administrative bulk updates.
        /// </summary>
        public async Task<List<SystemReports>> GetSystemReportsAsyncByIds(List<Guid> ids)
        {
            try
            {
                return await _context.SystemReports
                    .Where(r => ids.Contains(r.Id))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving system reports by IDs.", ex);
            }
        }

        /// <summary>
        /// Updates a specific report to 'Read' status.
        /// </summary>
        public async Task MarkAsReadAsync(Guid reportId)
        {
            try
            {
                var report = await _context.SystemReports.FindAsync(reportId);
                if (report != null)
                {
                    report.IsRead = true;
                    _context.SystemReports.Update(report);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while marking the system report as read.", ex);
            }
        }

        /// <summary>
        /// Updates the attributes of an existing system report.
        /// </summary>
        public async Task UpdateAsync(SystemReports systemReports)
        {
            try
            {
                _context.SystemReports.Update(systemReports);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the system report.", ex);
            }
        }
    }
}