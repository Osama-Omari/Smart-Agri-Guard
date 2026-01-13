using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IDeviceTokenRepository
    {
        Task AddTokenAsync(DeviceToken deviceToken);
        Task<DeviceToken?> GetTokenByIdAsync(Guid id);
        Task<DeviceToken> GetTokenByUserIdAsync(Guid userId);

        Task<DeviceToken?> GetTokenByValueAsync(string token);
        Task DeactivateTokenAsync(Guid id);
        Task DeleteTokenAsync(Guid id);

        Task DeactivateTokenAsync(string token);

        Task<List<Guid>> GetOldTokenIdsAsync(DateTime cutoffDate);

        Task DeleteTokensAsync(Guid[] ids);

        Task<List<DeviceToken>> GetInactiveDeviceTokens();

        Task DeleteRangeAsync(List<DeviceToken> deviceTokens);


    }
}
