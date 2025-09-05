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
        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            try
            {
                return await _context.Users.FindAsync(id);
            }
            catch (Exception ex) { throw new Exception("Error while getting user by it id "); }

        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            try
            {
                return await _context.Users.ToListAsync();
            }
            catch (Exception ex) { throw new Exception("Error while getting all users"); }

        }

        public async Task<User> UpdateUserAsync(User user)
        {
            try
            {
                var existing = await _context.Users.FindAsync(user.Id);
                if (existing == null)
                    throw new KeyNotFoundException("User not found");

                existing.FullName = user.FullName;
                existing.username = user.username;
                existing.UserRoleId = user.UserRoleId;
                existing.GreenhouseId = user.GreenhouseId;

                await _context.SaveChangesAsync();
                return existing;
            }
            catch (Exception ex) { throw new Exception("Error while updating user"); }
        }

        public async Task DeleteUserAsync(Guid id)
        {
            try
            {
                var existing = await _context.Users.FindAsync(id);
                if (existing == null)
                    throw new KeyNotFoundException("User not found");

                _context.Users.Remove(existing);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception("Error while deleting a user"); }
        }


    }
    
}
