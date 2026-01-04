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
using System.Security.AccessControl;

namespace InfrastructureLayer.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly IUserRoleRepository _roleRepository;
        private readonly IGreenhouseRepository _greenhouseRepository;
        private readonly IPlantRepository _plantRepository;
        private readonly IFarmerPlantRepository _farmerPlantRepository;
        private readonly IDeviceTokenRepository _deviceTokenRepository;


        public UserService(IUserRepository userRepository,IMapper mapper, IPasswordHasherService passwordHasherService , IUserRoleRepository userRoleRepository, IGreenhouseRepository greenhouseRepository,
            IPlantRepository plantRepository, IFarmerPlantRepository farmerPlantRepository,IDeviceTokenRepository deviceTokenRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _passwordHasherService = passwordHasherService;
            _roleRepository = userRoleRepository;
            _greenhouseRepository = greenhouseRepository;
            _plantRepository = plantRepository;
            _farmerPlantRepository = farmerPlantRepository;
            _deviceTokenRepository = deviceTokenRepository;
        }
        public async Task<UserDTO?> Authenticate(UserLoginDTO loginDTO)
        {
            var user = await _userRepository.GetUserByUserName(loginDTO.UserName);
            if (user == null)
                return null;
            if (_passwordHasherService.VerifyPasswordHash(loginDTO.Password, loginDTO.UserName, user.PasswordHash) == false)
                return null;

            if(!string.IsNullOrEmpty(loginDTO.DeviceToken))
            {
               var existingToken = user.DeviceTokens.FirstOrDefault(t => t.Token == loginDTO.DeviceToken);
                if (existingToken == null)
                {
                    user.DeviceTokens.Add(new DeviceToken { Token = loginDTO.DeviceToken, UserId = user.Id, CreatedAt = DateTime.UtcNow,
                    DeviceType = loginDTO.DeviceType,DeviceModel = loginDTO.DeviceModel,IsActive = true, LastUpdated = DateTime.UtcNow });
                    await _userRepository.UpdateUserAsync(user);
                }
                else
                {
                    existingToken.IsActive = true;
                    existingToken.LastUpdated = DateTime.UtcNow;
                    await _userRepository.UpdateUserAsync(user);
                }
            }

            if (!string.IsNullOrWhiteSpace(loginDTO.TimeZoneId) &&
            !string.Equals(loginDTO.TimeZoneId, user.TimeZoneId, StringComparison.OrdinalIgnoreCase))
            {
                user.TimeZoneId = loginDTO.TimeZoneId;
                await _userRepository.UpdateUserAsync(user);
            }


            return _mapper.Map<UserDTO>(user);
        }

        public async Task ChangePasswordAsync(ChangePasswordDTO dto, Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            if(!_passwordHasherService.VerifyPasswordHash(dto.CurrentPassword, user.username, user.PasswordHash))
                throw new UnauthorizedAccessException("Current password is incorrect");

            _passwordHasherService.CreatePasswordHash(dto.NewPassword, user.username, out byte[] newHash);
            user.PasswordHash = newHash;
            await _userRepository.UpdateUserAsync(user);
        }

        public async Task DeleteFarmerAsync(Guid farmerId, Guid managerId)
        {
            var manager = await _userRepository.GetManagerById(managerId);
            if (manager == null)
                throw new Exception("Manager not found");
            var farmer = await _userRepository.GetFarmerWithPlants(farmerId);
            if(farmer == null)
                throw new Exception("Farmer not found");

            if(farmer.GreenhouseId != manager.ManagedGreenhouses?.FirstOrDefault()?.Id)
                throw new Exception("Manager not authorized to delete this farmer");
            if(farmer == null)
                throw new Exception("Farmer not found");
            if(farmer.FarmerPlants.Any())
                throw new Exception("Cannot delete farmer with assigned plants");
            await _userRepository.DeleteUserAsync(farmerId);
        }

        public async Task DeleteManagerAsync(Guid managerId)
        {
            var manager = await _userRepository.GetManagerById(managerId);
            if (manager == null)
                throw new Exception("Manager not found");
            if(manager.ManagedGreenhouses != null && manager.ManagedGreenhouses.Any())
                throw new Exception("Cannot delete manager with assigned greenhouses");
            await _userRepository.DeleteUserAsync(managerId);
        }

        public async Task<List<ManagerDTO>> GetAllManagersAsync()
        {
            var managers = await _userRepository.GetAllManagersAsync();
            return _mapper.Map<List<ManagerDTO>>(managers);
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

        public async Task LogoutAsync(LogoutRequestDTO logoutRequestDTO)
        {
            var deviceToken = await _deviceTokenRepository.GetTokenByValueAsync(logoutRequestDTO.DeviceToken);
            if(deviceToken == null)
                throw new Exception("Device token not found");
            await _deviceTokenRepository.DeactivateTokenAsync(deviceToken.Id);
        }

        public async Task RegisterAdmin(AdminRegisterDTO adminRegisterDTO)
        {
            var user = _mapper.Map<User>(adminRegisterDTO);
            _passwordHasherService.CreatePasswordHash(adminRegisterDTO.password,user.username,out byte[] hash); 
            user.PasswordHash = hash;
            var adminRole = await _roleRepository.GetUserRoleByName("Admin");
            if (adminRole == null)
                throw new Exception("Admin role not found");
            user.UserRoleId = adminRole.Id;
            await _userRepository.AddUserAsync(user);
            
        }

        public async Task<UserDTO> RegisterFarmer(FarmerRegisterDTO dto, Guid greenhouseId)
        {
            var user = _mapper.Map<User>(dto);
            _passwordHasherService.CreatePasswordHash(dto.Password, user.username, out byte[] passwordHash);
            user.PasswordHash = passwordHash;
            var farmerRole = await _roleRepository.GetUserRoleByName("Farmer");
            if (farmerRole == null)
                throw new Exception("Farmer role not found");
            user.UserRoleId = farmerRole.Id;
            user.GreenhouseId = greenhouseId;
            await _userRepository.AddUserAsync(user);
            if (dto.AssignedPlants != null && dto.AssignedPlants.Count > 0)
            {

                foreach(var p in dto.AssignedPlants)
                {
                    var plant = await _plantRepository.GetPlantById(p);
                    if(plant == null)
                        continue;
                    FarmerPlant farmerPlant = new FarmerPlant();
                    farmerPlant.FarmerId = user.Id;
                    farmerPlant.PlantId = plant.Id;
                    farmerPlant.AssignedAt = DateTime.UtcNow;
                    await _farmerPlantRepository.AddAsync(farmerPlant);
                }
            }

            return _mapper.Map<UserDTO>(user);
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
                    await _greenhouseRepository.UpdateAsync(greenhouse);
                }
            }
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO> UpdateUserAsync(UpdateUserDTO dto, Guid userId)
        {
            var user = await  _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");
            user.FullName = dto.FullName;
            await _userRepository.UpdateUserAsync(user);

            return _mapper.Map<UserDTO>(user);


        }
    }
}
