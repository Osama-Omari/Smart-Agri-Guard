using ApplicationLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface INotificationService
    {
        Task SendToUserAsync(Guid userId, string title, string message);
        Task SendToUsersAsync(IEnumerable<Guid> userIds, string title, string message);

        Task NotifyPlantNeedsWatering(Guid plantId);
        Task NotifyPlantNeedsNutrients(Guid plantId);

        Task NotifyAdminTest(Guid adminId);


    }
}
