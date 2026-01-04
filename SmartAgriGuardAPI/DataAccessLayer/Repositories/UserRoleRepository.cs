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
    /// Repository managing the UserRole entities.
    /// Provides the data layer for the system's security and authorization levels.
    /// </summary>
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly SmartAgriGuardDbContext _context;

        public UserRoleRepository(SmartAgriGuardDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a role by its unique name (e.g., "Admin", "Manager").
        /// Essential for assigning roles during user registration.
        /// </summary>
        public async Task<UserRole?> GetUserRoleByName(string roleName)
        {
            try
            {
                return await _context.UserRoles
                    .Where(role => role.Name.Equals(roleName))
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the user role by name.", ex);
            }
        }

        /// <summary>
        /// Fetches a specific user role using its unique GUID.
        /// </summary>
        public async Task<UserRole?> GetUserRoleByIdAsync(Guid id)
        {
            try
            {
                return await _context.UserRoles.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while getting user role by its id", ex);
            }
        }

        /// <summary>
        /// Returns a list of all defined roles in the system.
        /// </summary>
        public async Task<List<UserRole>> GetAllUserRolesAsync()
        {
            try
            {
                return await _context.UserRoles.ToListAsync();
            }
            catch (Exception ex) { throw new Exception("Error while getting all user roles", ex); }
        }

        /// <summary>
        /// Adds a new role type to the system.
        /// </summary>
        public async Task AddUserRoleAsync(UserRole role)
        {
            try
            {
                await _context.UserRoles.AddAsync(role);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception("Error while adding a user role", ex); }
        }

        /// <summary>
        /// Updates the name of an existing user role.
        /// </summary>
        public async Task<UserRole> UpdateUserRoleAsync(UserRole role)
        {
            try
            {
                var existing = await _context.UserRoles.FindAsync(role.Id);
                if (existing == null)
                    throw new KeyNotFoundException("UserRole not found");

                existing.Name = role.Name;

                await _context.SaveChangesAsync();
                return existing;
            }
            catch (Exception ex) { throw new Exception("Error while updating user role", ex); }
        }

        /// <summary>
        /// Physically removes a role from the database.
        /// </summary>
        public async Task DeleteUserRoleAsync(Guid id)
        {
            try
            {
                var role = await _context.UserRoles.FindAsync(id);
                if (role != null)
                {
                    _context.UserRoles.Remove(role);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex) { throw new Exception("Error while deleting user role ", ex); }
        }
    }
}