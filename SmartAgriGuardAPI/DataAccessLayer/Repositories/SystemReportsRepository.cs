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
    public class SystemReportsRepository : ISystemReportsRepository
    {
        private readonly SmartAgriGuardDbContext _context;

        public SystemReportsRepository(SmartAgriGuardDbContext context)
        {
            _context = context;
        }

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

        public async Task<List<SystemReports>> GetAllAsync()
        {
            try
            {
                return await _context.SystemReports.OrderByDescending(x=> x.ReportDate).Include(g=>g.Greenhouse).ToListAsync();

            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving system reports.", ex);
            }
        }

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

        public async Task<List<SystemReports>> GetSystemReportsAsyncByIds(List<Guid> ids)
        {
            try
            {
                return await _context.SystemReports
                    .Where(r => ids.Contains(r.Id))
                    .ToListAsync();

            }
            catch (Exception ex) {
                throw new Exception("An error occurred while retrieving system reports by IDs.", ex);
            }
        }

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
