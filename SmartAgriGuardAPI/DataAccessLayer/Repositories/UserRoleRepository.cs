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
    }
}
