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
        private readonly IPlantRepository _plantRepository;
        private readonly IFarmerPlantRepository _farmerPlantRepository;


        public UserService(IUserRepository userRepository,IMapper mapper, IPasswordHasherService passwordHasherService , IUserRoleRepository userRoleRepository, IGreenhouseRepository greenhouseRepository,
            IPlantRepository plantRepository, IFarmerPlantRepository farmerPlantRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _passwordHasherService = passwordHasherService;
            _roleRepository = userRoleRepository;
            _greenhouseRepository = greenhouseRepository;
            _plantRepository = plantRepository;
            _farmerPlantRepository = farmerPlantRepository;
        }
        public async Task<UserDTO?> Authenticate(UserLoginDTO loginDTO)
        {
            var user = await _userRepository.GetUserByUserName(loginDTO.UserName);
            if (user == null)
                return null;
            if (_passwordHasherService.VerifyPasswordHash(loginDTO.Password, loginDTO.UserName, user.PasswordHash) == false)
                return null;

            return _mapper.Map<UserDTO>(user);
        }

        public async Task DeleteFarmerAsync(Guid farmerId)
        {
            var farmer = await _userRepository.GetFarmerWithPlants(farmerId);
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

        public async Task<FarmerDTO> GetFarmer(Guid farmerId)
        {
            var farmer = await _userRepository.GetFarmerWithPlants(farmerId);
            if (farmer == null)
                throw new Exception("Farmer not found");
            return _mapper.Map<FarmerDTO>(farmer);
        }

        public async Task<ManagerDTO> GetManager(Guid managerId)
        {
            var manager = await _userRepository.GetManagerById(managerId);
            if (manager == null)
                throw new Exception("Manager not found");
            return _mapper.Map<ManagerDTO>(manager);
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
    }
}
