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
    /// Repository for managing User accounts and Role-based data retrieval.
    /// Handles Farmer and Manager specific data relationships across the system.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly SmartAgriGuardDbContext _context;
        public UserRepository(SmartAgriGuardDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Registers a new user account in the system.
        /// </summary>
        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves a user by their unique username for login and authentication.
        /// Includes UserRole for authorization and DeviceTokens for push notifications.
        /// </summary>
        public async Task<User?> GetUserByUserName(string username)
        {
            try
            {
                return await _context.Users
                    .Include(x => x.UserRole)
                    .Include(u => u.DeviceTokens)
                    .FirstOrDefaultAsync(x => x.username == username);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error happened while returning user: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves a batch of farmer accounts based on a list of IDs.
        /// </summary>
        public async Task<List<User>> GetFarmersByIdsAsync(List<Guid> farmerIds)
        {
            try
            {
                return await _context.Users
                    .Where(u => farmerIds.Contains(u.Id))
                    .ToListAsync();
            }
            catch (Exception ex) { throw new Exception($"Error while getting farmers: {ex.Message}"); }
        }

        /// <summary>
        /// Updates a user's profile information in the database.
        /// </summary>
        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Fetches a user by their GUID, including their assigned role.
        /// </summary>
        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            try
            {
                return await _context.Users
                    .Include(x => x.UserRole)
                    .FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex) { throw new Exception($"Error while getting user by id: {ex.Message}"); }
        }

        /// <summary>
        /// Saves changes to an existing user entity.
        /// </summary>
        public async Task UpdateUserAsync(User user)
        {
            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception($"Error while updating user: {ex.Message}"); }
        }

        /// <summary>
        /// Permanently removes a user from the system.
        /// </summary>
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

        /// <summary>
        /// Retrieves a user specifically as a Farmer, including their assigned plants.
        /// Validates that the role matches "Farmer" to prevent incorrect data mapping.
        /// </summary>
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

        /// <summary>
        /// Retrieves a user specifically as a Manager, including the greenhouses they oversee.
        /// </summary>
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

        /// <summary>
        /// Retrieves a complete list of all users with the "Manager" role.
        /// </summary>
        public async Task<List<User>> GetAllManagersAsync()
        {
            try
            {
                return await _context.Users
                    .Include(u => u.UserRole)
                    .Include(u => u.ManagedGreenhouses)
                    .Where(u => u.UserRole.Name == "Manager")
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while getting all managers : {ex.Message}");
            }
        }
    }
}