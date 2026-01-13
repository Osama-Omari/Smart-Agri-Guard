using DataAccessLayer.Data;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    /// <summary>
    /// Repository responsible for managing mobile device tokens used for push notifications.
    /// Handles registration, deactivation, and maintenance of token data.
    /// </summary>
    public class DeviceTokenRepository : IDeviceTokenRepository
    {
        private readonly SmartAgriGuardDbContext _context;

        public DeviceTokenRepository(SmartAgriGuardDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Persists a new device token to the database.
        /// </summary>
        /// <param name="deviceToken">The token entity to be added.</param>
        public async Task AddTokenAsync(DeviceToken deviceToken)
        {
            try
            {
                await _context.DeviceTokens.AddAsync(deviceToken);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding device token", ex);
            }
            }

        /// <summary>
        /// Retrieves a specific token by its unique identifier, including associated User data.
        /// </summary>
        /// <param name="id">The GUID of the token record.</param>
        /// <returns>The DeviceToken if found; otherwise, null.</returns>
        public async Task<DeviceToken?> GetTokenByIdAsync(Guid id)
        {
            try { 
            return await _context.DeviceTokens
                .Include(dt => dt.User)
                .FirstOrDefaultAsync(dt => dt.Id == id);
            }
            catch (Exception ex) {
                throw new Exception("Error retrieving token by ID", ex);
            }
        }

        /// <summary>
        /// Retrieves the most recently updated active token for a specific user.
        /// </summary>
        /// <param name="userId">The GUID of the user.</param>
        /// <returns>The active DeviceToken record.</returns>
        /// <exception cref="Exception">Thrown if no active token is found for the user.</exception>
        public async Task<DeviceToken> GetTokenByUserIdAsync(Guid userId)
        {
            try
            {
                var token = await _context.DeviceTokens
                    .OrderByDescending(dt => dt.LastUpdated)
                    .Where(dt => dt.UserId == userId && dt.IsActive)
                    .FirstOrDefaultAsync();

                if (token == null)
                {
                    throw new Exception("Token not found for the specified user ID");
                }
                return token;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving token by user ID", ex);
            }
        }

        /// <summary>
        /// Performs a soft-delete by deactivating a token instead of removing it.
        /// Useful for maintaining history or handling temporary logouts.
        /// </summary>
        /// <param name="id">The GUID of the token to deactivate.</param>
        public async Task DeactivateTokenAsync(Guid id)
        {
            var token = await _context.DeviceTokens.FindAsync(id);
            if (token != null)
            {
                token.IsActive = false;
                token.LastUpdated = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Physically deletes a token record from the database.
        /// </summary>
        /// <param name="id">The GUID of the token to delete.</param>
        public async Task DeleteTokenAsync(Guid id)
        {
            var token = await _context.DeviceTokens.FindAsync(id);
            if (token != null)
            {
                _context.DeviceTokens.Remove(token);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Retrieves a token record using the raw string value (the actual push token).
        /// </summary>
        /// <param name="token">The unique string token provided by the mobile device.</param>
        public async Task<DeviceToken?> GetTokenByValueAsync(string token)
        {
            try
            {
                return await _context.DeviceTokens
                    .Include(dt => dt.User)
                    .FirstOrDefaultAsync(dt => dt.Token == token);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving token by value", ex);
            }
        }

        /// <summary>
        /// Bulk deletes multiple token records.
        /// </summary>
        /// <param name="ids">An array of token GUIDs to remove.</param>
        public async Task DeleteTokensAsync(Guid[] ids)
        {
            try
            {
                var tokens = await _context.DeviceTokens
                    .Where(dt => ids.Contains(dt.Id))
                    .ToListAsync();

                if (tokens.Any())
                {
                    _context.DeviceTokens.RemoveRange(tokens);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting tokens {ex.Message}");
            }
        }

        /// <summary>
        /// Identifies tokens that haven't been updated since a specific date.
        /// Primarily used for background cleanup tasks to remove stale device data.
        /// </summary>
        /// <param name="cutoffDate">The date threshold for staleness.</param>
        /// <returns>A list of GUIDs for tokens older than the cutoff.</returns>
        public async Task<List<Guid>> GetOldTokenIdsAsync(DateTime cutoffDate)
        {
            try
            {
                return await _context.DeviceTokens
                    .Where(dt => dt.LastUpdated < cutoffDate)
                    .Select(dt => dt.Id)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving old tokens: {ex.Message} ");
            }
        }

        public async Task<List<DeviceToken>> GetInactiveDeviceTokens()
        {
            try
            {
                return await _context.DeviceTokens
                    .Where(dt => !dt.IsActive)
                    .ToListAsync();

            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving inactive device tokens", ex);
            }
        }

        public async Task DeleteRangeAsync(List<DeviceToken> deviceTokens)
        {
            try
            {
                _context.DeviceTokens.RemoveRange(deviceTokens);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting range of device tokens", ex);
            }
        }

        public async Task DeactivateTokenAsync(string token)
        {
            var deviceToken = await _context.DeviceTokens
                .FirstOrDefaultAsync(dt => dt.Token == token);
            if (deviceToken != null)
            {
                deviceToken.IsActive = false;
                deviceToken.LastUpdated = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

        }
    }
}