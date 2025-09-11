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
                return await _context.Users.Include(x=>x.UserRole).FirstOrDefaultAsync(x => x.username == username);
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
                return await _context.Users.Include(x=>x.UserRole).FirstOrDefaultAsync(x=> x.Id == id);
            }
            catch (Exception ex) { throw new Exception($"Error while getting user by it id: {ex.Message}"); }

        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            try
            {
                return await _context.Users.ToListAsync();
            }
            catch (Exception ex) { throw new Exception($"Error while getting all users: {ex.Message}"); }

        }

        public async Task UpdateUserAsync(User user)
        {
            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception($"Error while updating user: {ex.Message}"); }
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
            catch (Exception ex) { throw new Exception($"Error while deleting a user: {ex.Message}"); }
        }

        public async Task<User?> GetFarmerWithPlants(Guid farmerId)
        {
            try
            {
                return await _context.Users
                .Include(u => u.FarmerPlants)
                .Include(u => u.UserRole)
                .Where(u => u.UserRole.Name == "Farmer" && u.Id == farmerId)
                .FirstOrDefaultAsync();

            }
            catch (Exception ex)
            {
                throw new Exception($"Error while retrieving farmer with assigned plants: {ex.Message}");
            }
        }

        public Task<User?> GetManagerById(Guid managerId)
        {
            try
            {
                return _context.Users
                .Include(u => u.UserRole)
                .Include(u => u.ManagedGreenhouses)
                .Where(u => u.UserRole.Name == "Manager" && u.Id == managerId)
                .FirstOrDefaultAsync() ?? throw new Exception("Manager not found");

            }
            catch (Exception ex)
            {
                throw new Exception($"Error while getting manager by id: {ex.Message}");
            }
        }
    }
    
}
