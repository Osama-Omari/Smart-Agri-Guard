using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureLayer.Services
{
    public static class DbInitializer
    {
        public static async Task SeedAdmins(IUserService userService,string fullname, string username, string password)
        {
            if(!await userService.isUserNameExists("osama25"))
            {
                var user = new AdminRegisterDTO
                {
                    FullName = fullname,
                    userName = username,
                    password = password
                };
                await userService.RegisterAdmin(user);
            }
        }

    }
}
