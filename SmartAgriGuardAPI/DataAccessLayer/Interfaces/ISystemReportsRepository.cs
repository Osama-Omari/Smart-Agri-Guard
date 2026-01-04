using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface ISystemReportsRepository
    {
        Task AddAsync(SystemReports systemReports);

        Task<List<SystemReports>> GetByGreenhouseIdAsync(Guid greenhouseId);

        Task MarkAsReadAsync(Guid reportId);

        Task DeleteAsync(Guid reportId);

        Task<List<SystemReports>> GetAllAsync();

        Task<List<SystemReports>> GetSystemReportsAsyncByIds(List<Guid> ids);

        Task UpdateAsync(SystemReports systemReports);

    }
}
