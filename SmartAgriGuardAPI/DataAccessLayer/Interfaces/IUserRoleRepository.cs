using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IUserRoleRepository
    {
        Task<UserRole?> GetUserRoleByName(string roleName);

        // Get a role by its ID
        Task<UserRole?> GetUserRoleByIdAsync(Guid id);

        // Get all roles
        Task<List<UserRole>> GetAllUserRolesAsync();

        // Add a new role
        Task AddUserRoleAsync(UserRole role);

        // Update an existing role
        Task<UserRole> UpdateUserRoleAsync(UserRole role);

        // Delete a role
        Task DeleteUserRoleAsync(Guid id);
    }
}
