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
    public class DeviceTokenRepository : IDeviceTokenRepository
    {
        private readonly SmartAgriGuardDbContext _context;

        public DeviceTokenRepository(SmartAgriGuardDbContext context)
        {
            _context = context;
        }

        
        public async Task AddTokenAsync(DeviceToken deviceToken)
        {
            await _context.DeviceTokens.AddAsync(deviceToken);
            await _context.SaveChangesAsync();
        }

        public async Task<DeviceToken?> GetTokenByIdAsync(Guid id)
        {
            return await _context.DeviceTokens
                .Include(dt => dt.User)
                .FirstOrDefaultAsync(dt => dt.Id == id);
        }

        
        public async Task<List<DeviceToken>> GetTokensByUserIdAsync(Guid userId)//all tokens for user
        {
            return await _context.DeviceTokens
                .Where(dt => dt.UserId == userId && dt.IsActive)
                .ToListAsync();
        }

        public async Task DeactivateTokenAsync(Guid id) //sofrt delete
        {
            var token = await _context.DeviceTokens.FindAsync(id);
            if (token != null)
            {
                token.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }

        // Optional: Remove token completely
        public async Task DeleteTokenAsync(Guid id)
        {
            var token = await _context.DeviceTokens.FindAsync(id);
            if (token != null)
            {
                _context.DeviceTokens.Remove(token);
                await _context.SaveChangesAsync();
            }
        }
    }
}
