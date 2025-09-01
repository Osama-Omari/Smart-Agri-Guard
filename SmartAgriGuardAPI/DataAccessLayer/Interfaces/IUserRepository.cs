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

        Task AddUserAsync(User user);
    }
}
