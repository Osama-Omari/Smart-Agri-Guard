using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces;

namespace InfrastructureLayer.Services
{
    public class UserService : IUserService
    {
        public Task<UserDTO> Authenticate(UserLoginDTO loginDTO)
        {
            throw new NotImplementedException();
        }

        public Task<bool> isUserNameExists(string userName)
        {
            throw new NotImplementedException();
        }

        public Task<UserDTO> RegisterFarmer(FarmerRegisterDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<UserDTO> RegisterManager(ManagerRegisterDTO managerRegisterDTO)
        {
            throw new NotImplementedException();
        }
    }
}
