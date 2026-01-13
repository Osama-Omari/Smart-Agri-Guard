using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IPlantNotificationsRepository
    {
         Task AddAsync(PlantNotifications plantNotification);

         Task<List<PlantNotifications>> GetByPlantIdAsync(Guid plantId);

         Task MarkAsReadAsync(Guid notificationId);

         Task DeleteAsync(Guid notificationId);


        Task<List<PlantNotifications>> GetByIdsAsync(List<Guid> notificationIds);

        Task UpdateAsync(PlantNotifications plantNotifications);

        Task<List<PlantNotifications>> GetReadPlantsNotifications();

        Task DeleteRangeAsync(List<PlantNotifications> notificationsToDelete);
    }
}
