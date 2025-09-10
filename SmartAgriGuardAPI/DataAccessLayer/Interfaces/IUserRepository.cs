using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataAccessLayer.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByUserName(string username);
        Task<User?> GetUserByIdAsync(Guid id);
        Task<List<User>> GetAllUsersAsync();
        Task AddUserAsync(User user);
        Task<User> UpdateUserAsync(User user);
        Task DeleteUserAsync(Guid id);

        Task<User?> GetFarmerWithPlants(Guid farmerId);

        Task<User?> GetManagerById(Guid managerId);


    }
}
