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
        public Task AddAsync(PlantNotifications plantNotification);

        public Task<List<PlantNotifications>> GetByPlantIdAsync(Guid plantId);

        public Task MarkAsReadAsync(Guid notificationId);

        public Task DeleteAsync(Guid notificationId);
    }
}
