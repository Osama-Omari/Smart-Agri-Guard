using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationLayer.DTOs;
using DataAccessLayer.Interfaces;
using ApplicationLayer.Interfaces;
using AutoMapper;
using DataAccessLayer.Models;

namespace InfrastructureLayer.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly IUserRoleRepository _roleRepository;
        private readonly IGreenhouseRepository _greenhouseRepository;


        public UserService(IUserRepository userRepository,IMapper mapper, IPasswordHasherService passwordHasherService , IUserRoleRepository userRoleRepository, IGreenhouseRepository greenhouseRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _passwordHasherService = passwordHasherService;
            _roleRepository = userRoleRepository;
            _greenhouseRepository = greenhouseRepository;
        }
        public Task<UserDTO> Authenticate(UserLoginDTO loginDTO)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> isUserNameExists(string userName)
        {
            var user = await _userRepository.GetUserByUserName(userName);
            if (user == null)
            {
                return false;
            }
            return true;

        }

        public Task<UserDTO> RegisterFarmer(FarmerRegisterDTO dto)
        {
            throw new NotImplementedException();
        }

        public async Task<UserDTO> RegisterManager(ManagerRegisterDTO managerRegisterDTO)
        {
            var user = _mapper.Map<User>(managerRegisterDTO);
            _passwordHasherService.CreatePasswordHash(managerRegisterDTO.Password, user.username, out byte[] passwordHash);
            user.PasswordHash = passwordHash;
            var ManagerRole = await _roleRepository.GetUserRoleByName("Manager");
            if (ManagerRole == null)
            {
                throw new Exception("Manager Role not found");
            }
            user.UserRoleId = ManagerRole.Id;
            await _userRepository.AddUserAsync(user);

            if (managerRegisterDTO.GreenhousesIds != null && managerRegisterDTO.GreenhousesIds.Count > 0)
            {
                foreach(var x in managerRegisterDTO.GreenhousesIds)
                {
                    var greenhouse = await _greenhouseRepository.GetGreenhouseById(x);
                    if (greenhouse == null)
                        continue;
                    if (greenhouse.ManagerId != null)
                    {
                        throw new Exception("The selected greenhouse has a manager");
                        
                    }
                    greenhouse.ManagerId = user.Id;

                }
            }
            return _mapper.Map<UserDTO>(user);
        }
    }
}
