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
        Task<GreenhouseDTO> GetGreenhouseById(Guid id);
        Task<GreenhouseDTO> AddGreenhouse(GreenhouseRegisterDTO dto);
        
        Task AssignManagerAsync(Guid managerId,Guid GreenhouseId);

        Task UnAssignManagerAsync(Guid id);

        Task<List<GreenhouseDTO>> GetAllGreenhouses();

        Task DeleteGreenhouseAsync(Guid id);

        Task UpdateGreenhouseAsync(Guid id, GreenhouseUpdateDTO dto);

        Task<List<GreenhouseDTO>?> GetGreenhousesByManagerIdAsync(Guid managerId);

        Task<List<FarmerDTO>?> GetFarmersByGreenhouseIdAsync(Guid greenhouseId);

        Task<UserDTO> GetManagerByGreenhouseIdAsync(Guid greenhouseId);

        Task<List<GreenhouseDTO>?> GetGreenhousesWithoutManagerAsync();

        Task<List<SystemReportDTO>?> GetGreenhouseNotifications(Guid greenhouseId);

        Task<List<SystemReportDTO>?> GetAllGreenhousesNotifications();

        Task MarkGreenhouseNotificationAsRead (List<Guid> ids);
    }
}
