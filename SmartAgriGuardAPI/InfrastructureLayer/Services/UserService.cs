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
    /// <summary>
    /// Implements business logic for user management, including authentication, 
    /// role-based registration, and administrative account actions.
    /// </summary>
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

        public UserService(IUserRepository userRepository, IMapper mapper, IPasswordHasherService passwordHasherService, IUserRoleRepository userRoleRepository, IGreenhouseRepository greenhouseRepository,
            IPlantRepository plantRepository, IFarmerPlantRepository farmerPlantRepository, IDeviceTokenRepository deviceTokenRepository)
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

        /// <summary>
        /// Validates user credentials, manages mobile device tokens, and updates user timezone.
        /// </summary>
        /// <param name="loginDTO">Login credentials and optional device telemetry.</param>
        /// <returns>A UserDTO if successful; otherwise, null.</returns>
        public async Task<UserDTO?> Authenticate(UserLoginDTO loginDTO)
        {
            var user = await _userRepository.GetUserByUserName(loginDTO.UserName);
            if (user == null)
                return null;

            // Verify password using the injected hashing service
            if (_passwordHasherService.VerifyPasswordHash(loginDTO.Password, loginDTO.UserName, user.PasswordHash) == false)
                return null;

            // Manage Device Tokens for Push Notifications
            if (!string.IsNullOrEmpty(loginDTO.DeviceToken))
            {
                var existingToken = user.DeviceTokens.FirstOrDefault(t => t.Token == loginDTO.DeviceToken);
                if (existingToken == null)
                {
                    user.DeviceTokens.Add(new DeviceToken
                    {
                        Token = loginDTO.DeviceToken,
                        UserId = user.Id,
                        CreatedAt = DateTime.UtcNow,
                        DeviceType = loginDTO.DeviceType,
                        DeviceModel = loginDTO.DeviceModel,
                        IsActive = true,
                        LastUpdated = DateTime.UtcNow
                    });
                }
                else
                {
                    existingToken.IsActive = true;
                    existingToken.LastUpdated = DateTime.UtcNow;
                }
                await _userRepository.UpdateUserAsync(user);
            }

            // Update user's preferred TimeZone if it has changed
            if (!string.IsNullOrWhiteSpace(loginDTO.TimeZoneId) &&
                !string.Equals(loginDTO.TimeZoneId, user.TimeZoneId, StringComparison.OrdinalIgnoreCase))
            {
                user.TimeZoneId = loginDTO.TimeZoneId;
                await _userRepository.UpdateUserAsync(user);
            }

            return _mapper.Map<UserDTO>(user);
        }

        /// <summary>
        /// Updates a user's password after verifying the current password.
        /// </summary>
        /// <exception cref="KeyNotFoundException">Thrown if user does not exist.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown if current password check fails.</exception>
        public async Task ChangePasswordAsync(ChangePasswordDTO dto, Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            if (!_passwordHasherService.VerifyPasswordHash(dto.CurrentPassword, user.username, user.PasswordHash))
                throw new UnauthorizedAccessException("Current password is incorrect");

            _passwordHasherService.CreatePasswordHash(dto.NewPassword, user.username, out byte[] newHash);
            user.PasswordHash = newHash;
            await _userRepository.UpdateUserAsync(user);
        }

        /// <summary>
        /// Deletes a farmer account. Ensures the requesting manager owns the farmer's greenhouse.
        /// </summary>
        /// <exception cref="Exception">Thrown for unauthorized access or if farmer has active plant assignments.</exception>
        public async Task DeleteFarmerAsync(Guid farmerId, Guid managerId)
        {
            var manager = await _userRepository.GetManagerById(managerId);
            if (manager == null)
                throw new Exception("Manager not found");

            var farmer = await _userRepository.GetFarmerWithPlants(farmerId);
            if (farmer == null)
                throw new Exception("Farmer not found");

            // Authorization Check: Does the manager own the greenhouse where this farmer works?
            if (farmer.GreenhouseId != manager.ManagedGreenhouses?.FirstOrDefault()?.Id)
                throw new Exception("Manager not authorized to delete this farmer");

            // Integrity Check: Prevent deletion if the farmer is currently responsible for plants
            if (farmer.FarmerPlants.Any())
                throw new Exception("Cannot delete farmer with assigned plants");

            await _userRepository.DeleteUserAsync(farmerId);
        }

        /// <summary>
        /// Deletes a manager. Prevented if the manager is still assigned to a greenhouse.
        /// </summary>
        public async Task DeleteManagerAsync(Guid managerId)
        {
            var manager = await _userRepository.GetManagerById(managerId);
            if (manager == null)
                throw new Exception("Manager not found");

            if (manager.ManagedGreenhouses != null && manager.ManagedGreenhouses.Any())
                throw new Exception("Cannot delete manager with assigned greenhouses");

            await _userRepository.DeleteUserAsync(managerId);
        }

        /// <summary>
        /// Fetches all users with the 'Manager' role.
        /// </summary>
        public async Task<List<ManagerDTO>> GetAllManagersAsync()
        {
            var managers = await _userRepository.GetAllManagersAsync();
            return _mapper.Map<List<ManagerDTO>>(managers);
        }

        /// <summary>
        /// Checks if a username is already taken in the system.
        /// </summary>
        public async Task<bool> isUserNameExists(string userName)
        {
            var user = await _userRepository.GetUserByUserName(userName);
            return user != null;
        }

        /// <summary>
        /// Deactivates a specific device token during logout to stop push notifications.
        /// </summary>
        public async Task LogoutAsync(LogoutRequestDTO logoutRequestDTO)
        {
            var deviceToken = await _deviceTokenRepository.GetTokenByValueAsync(logoutRequestDTO.DeviceToken);
            if (deviceToken == null)
                throw new Exception("Device token not found");

            await _deviceTokenRepository.DeactivateTokenAsync(deviceToken.Id);
        }

        /// <summary>
        /// Creates a new Admin user with a generated password hash.
        /// </summary>
        public async Task RegisterAdmin(AdminRegisterDTO adminRegisterDTO)
        {
            var user = _mapper.Map<User>(adminRegisterDTO);
            _passwordHasherService.CreatePasswordHash(adminRegisterDTO.password, user.username, out byte[] hash);
            user.PasswordHash = hash;

            var adminRole = await _roleRepository.GetUserRoleByName("Admin");
            if (adminRole == null)
                throw new Exception("Admin role not found");

            user.UserRoleId = adminRole.Id;
            await _userRepository.AddUserAsync(user);
        }

        /// <summary>
        /// Registers a farmer, assigns them to a greenhouse, and links initial plant assignments.
        /// </summary>
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

            // Link specified plants to the new farmer
            if (dto.AssignedPlants != null && dto.AssignedPlants.Count > 0)
            {
                foreach (var p in dto.AssignedPlants)
                {
                    var plant = await _plantRepository.GetPlantById(p);
                    if (plant == null) continue;

                    await _farmerPlantRepository.AddAsync(new FarmerPlant
                    {
                        FarmerId = user.Id,
                        PlantId = plant.Id,
                        AssignedAt = DateTime.UtcNow
                    });
                }
            }

            return _mapper.Map<UserDTO>(user);
        }

        /// <summary>
        /// Registers a manager and optionally assigns them ownership of specified greenhouses.
        /// </summary>
        /// <exception cref="Exception">Thrown if a chosen greenhouse already has a manager.</exception>
        public async Task<UserDTO> RegisterManager(ManagerRegisterDTO managerRegisterDTO)
        {
            var user = _mapper.Map<User>(managerRegisterDTO);
            _passwordHasherService.CreatePasswordHash(managerRegisterDTO.Password, user.username, out byte[] passwordHash);
            user.PasswordHash = passwordHash;

            var ManagerRole = await _roleRepository.GetUserRoleByName("Manager");
            if (ManagerRole == null) throw new Exception("Manager Role not found");

            user.UserRoleId = ManagerRole.Id;
            await _userRepository.AddUserAsync(user);

            // Assign greenhouse ownership
            if (managerRegisterDTO.GreenhousesIds != null && managerRegisterDTO.GreenhousesIds.Count > 0)
            {
                foreach (var x in managerRegisterDTO.GreenhousesIds)
                {
                    var greenhouse = await _greenhouseRepository.GetGreenhouseById(x);
                    if (greenhouse == null) continue;

                    if (greenhouse.ManagerId != null)
                        throw new Exception($"Greenhouse {greenhouse.Name} already has a manager.");

                    greenhouse.ManagerId = user.Id;
                    await _greenhouseRepository.UpdateAsync(greenhouse);
                }
            }
            return _mapper.Map<UserDTO>(user);
        }

        /// <summary>
        /// Updates basic user profile information (Full Name).
        /// </summary>
        public async Task<UserDTO> UpdateUserAsync(UpdateUserDTO dto, Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            user.FullName = dto.FullName;
            await _userRepository.UpdateUserAsync(user);

            return _mapper.Map<UserDTO>(user);
        }
    }
}