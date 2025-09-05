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
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly SmartAgriGuardDbContext _context;

        public UserRoleRepository(SmartAgriGuardDbContext context)
        {
            _context = context;
        }

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
                throw new Exception("An error occurred while retrieving the user role.", ex);
            }
        }

        public async Task<UserRole?> GetUserRoleByIdAsync(Guid id)
        {
            try
            {
                return await _context.UserRoles.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while getting user role by its id");
            }
        }

        public async Task<List<UserRole>> GetAllUserRolesAsync()
        {
            try
            {
                return await _context.UserRoles.ToListAsync();
            }
            catch (Exception ex) { throw new Exception("Error while getting all users"); }
        }

        public async Task AddUserRoleAsync(UserRole role)
        {
            try
            {
                await _context.UserRoles.AddAsync(role);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { throw new Exception("Error while adding a user role"); }
        }

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
            catch (Exception ex) { throw new Exception("Error while updating user role"); }
        }

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
            catch (Exception ex) { throw new Exception("Error while deleting user role "); }
        }
    }
}
