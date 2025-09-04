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
        Task<Greenhouse?> GetGreenhouseById(Guid id);            
        Task<List<Greenhouse>> GetAllAsync();                    
        Task AddAsync(Greenhouse greenhouse);                   
        Task UpdateAsync(Greenhouse greenhouse);                
        Task DeleteAsync(Guid id);                               
    }
}
