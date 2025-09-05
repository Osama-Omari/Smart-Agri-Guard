using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IRecommendationRepository
    {
        Task AddAsync(Recommendation recommendation);
        Task<Recommendation?> GetByIdAsync(Guid id);
        Task<List<Recommendation>> GetAllAsync();
        Task<List<Recommendation>> GetByPlantIdAsync(Guid plantId);
        Task UpdateAsync(Recommendation recommendation);
        Task DeleteAsync(Guid id);
    }
}
