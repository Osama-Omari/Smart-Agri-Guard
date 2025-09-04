using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IPlantRepository
    {
        Task AddAsync(Plant plant);

        Task<Plant?> GetPlantById(Guid plantId);
        Task<List<Plant>> GetAllPlantsAsync();

        Task UpdateAsync(Plant plant);

        Task DeleteAsync(Guid plantId);
    }
}
