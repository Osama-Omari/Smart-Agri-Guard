using ApplicationLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface IGreenhouseService
    {
        Task<GreenhouseDTO> AddGreenhouse(GreenhouseRegisterDTO dto);
        
        Task AssignManagerAsync(Guid managerId,Guid GreenhouseId);
    }
}
