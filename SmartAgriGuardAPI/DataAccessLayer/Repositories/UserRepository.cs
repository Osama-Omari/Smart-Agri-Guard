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
    public class UserRepository : IUserRepository
    {
        private readonly SmartAgriGuardDbContext _context;
        public UserRepository(SmartAgriGuardDbContext context)
        {
            _context = context;
        }

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetUserByUserName(string username)
        {
            try
            {
                return await _context.Users.FirstOrDefaultAsync(x => x.username == username);
            }
            catch (Exception ex)
            {
                throw new Exception($"an error happened while returning user: {ex.Message}");
            }

        }


    }
    
}
