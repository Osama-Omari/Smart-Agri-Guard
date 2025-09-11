using ApplicationLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO?> Authenticate(UserLoginDTO loginDTO);

        Task<UserDTO> RegisterManager(ManagerRegisterDTO managerRegisterDTO);

        Task<UserDTO> RegisterFarmer(FarmerRegisterDTO dto,Guid Id);

        Task<bool> isUserNameExists(string userName);

        Task RegisterAdmin(AdminRegisterDTO adminRegisterDTO);

        Task DeleteFarmerAsync(Guid farmerId);

        Task DeleteManagerAsync(Guid managerId);

        Task<ManagerDTO> GetManager(Guid managerId);

        Task<FarmerDTO> GetFarmer(Guid farmerId);

        Task<UserDTO> UpdateUserAsync(UpdateUserDTO dto, Guid userId);

        Task ChangePasswordAsync(ChangePasswordDTO dto, Guid userId);


    }
}
