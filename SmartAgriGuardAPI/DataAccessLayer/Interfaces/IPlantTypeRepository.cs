using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IPlantTypeRepository
    {
        Task AddAsync(PlantType plantType);          
        Task<PlantType?> GetByIdAsync(Guid id);                
        Task<List<PlantType>> GetAllAsync();                   
        Task UpdateAsync(PlantType plantType);                
        Task DeleteAsync(Guid id);                             
    }
}
