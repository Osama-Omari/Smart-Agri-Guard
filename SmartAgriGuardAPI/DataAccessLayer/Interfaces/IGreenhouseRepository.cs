using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IGreenhouseRepository
    {
        Task<Greenhouse?> GetGreenhouseById(Guid id);            // Read one greenhouse
        Task<List<Greenhouse>> GetAllAsync();                    // Read all greenhouses
        Task AddAsync(Greenhouse greenhouse);                   // Create a new greenhouse
        Task UpdateAsync(Greenhouse greenhouse);                // Update an existing greenhouse
        Task DeleteAsync(Guid id);                               // Delete a greenhouse    }
    }
}
