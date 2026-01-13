using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface IDataCleanupJob
    {
        Task ClearReadNotificationsAndReports();

        Task RemoveInactiveDeviceTokens();

        Task ProcessSensorDataLifecycle();
    }
}
