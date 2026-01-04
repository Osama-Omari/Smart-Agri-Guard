using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces;
using AutoMapper;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureLayer.Services
{
    /// <summary>
    /// Service for managing Greenhouse infrastructure, including manager assignments, 
    /// facility updates, and system-level notifications for greenhouses.
    /// </summary>
    public class GreenhouseService : IGreenhouseService
    {
        private readonly IGreenhouseRepository _greenhouseRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly ISystemReportsRepository _systemReportsRepository;

        public GreenhouseService(IGreenhouseRepository greenhouseRepository, IMapper mapper, IUserRepository userRepository, IFileStorageService fileStorageService, ISystemReportsRepository systemReportsRepository)
        {
            _greenhouseRepository = greenhouseRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _fileStorageService = fileStorageService;
            _systemReportsRepository = systemReportsRepository;
        }

        /// <summary>
        /// Registers a new greenhouse facility in the system.
        /// </summary>
        /// <param name="dto">The registration data including name, location, and optional image path.</param>
        public async Task<GreenhouseDTO> AddGreenhouse(GreenhouseRegisterDTO dto)
        {
            var greenhouse = new Greenhouse
            {
                Name = dto.Name,
                Location = dto.Location,
                ImageUrl = !string.IsNullOrEmpty(dto.ImagePath) ? dto.ImagePath : null
            };

            await _greenhouseRepository.AddAsync(greenhouse);
            return _mapper.Map<GreenhouseDTO>(greenhouse);
        }

        /// <summary>
        /// Assigns a specific user to a greenhouse as its manager.
        /// </summary>
        /// <remarks>
        /// Validates that the user exists, holds the 'Manager' role, and the greenhouse does not already have a manager.
        /// </remarks>
        /// <exception cref="KeyNotFoundException">Thrown if User or Greenhouse ID is invalid.</exception>
        /// <exception cref="Exception">Thrown if user is not a manager or greenhouse is already occupied.</exception>
        public async Task AssignManagerAsync(Guid managerId, Guid GreenhouseId)
        {
            var manager = await _userRepository.GetUserByIdAsync(managerId);
            if (manager == null)
                throw new KeyNotFoundException("Manager not found.");

            if (manager.UserRole.Name != "Manager")
                throw new Exception("The user is not a Manager");

            var greenhouse = await _greenhouseRepository.GetGreenhouseById(GreenhouseId);
            if (greenhouse == null)
                throw new KeyNotFoundException("Greenhouse not found.");

            if (greenhouse.ManagerId != null)
                throw new Exception("The greenhouse already has a manager.");

            greenhouse.ManagerId = managerId;
            await _greenhouseRepository.UpdateAsync(greenhouse);
        }

        /// <summary>
        /// Deletes a greenhouse facility and its associated image file.
        /// </summary>
        /// <remarks>
        /// Business Rule: Prevents deletion if there are active plants or farmers linked to the facility.
        /// </remarks>
        public async Task DeleteGreenhouseAsync(Guid id)
        {
            var greenhouse = await _greenhouseRepository.GetGreenhouseById(id);
            if (greenhouse == null)
                throw new KeyNotFoundException("Greenhouse not found");

            // Integrity Check: Prevent accidental deletion of active infrastructure
            if (greenhouse.Plants != null && greenhouse.Plants.Any())
                throw new Exception("Cannot delete greenhouse with assigned plants");

            if (greenhouse.Farmers != null && greenhouse.Farmers.Any())
                throw new Exception("Cannot delete greenhouse with assigned farmers");

            // Clean up physical assets
            await _fileStorageService.DeleteFileAsync(greenhouse.ImageUrl);
            await _greenhouseRepository.DeleteAsync(id);
        }

        /// <summary>
        /// Retrieves all system reports/notifications for all greenhouses (Admin View).
        /// </summary>
        public async Task<List<SystemReportDTO>?> GetAllGreenhousesNotifications()
        {
            var reports = await _systemReportsRepository.GetAllAsync();
            return (reports == null || !reports.Any()) ? null : _mapper.Map<List<SystemReportDTO>>(reports);
        }

        /// <summary>
        /// Retrieves a complete list of all greenhouses.
        /// </summary>
        public async Task<List<GreenhouseDTO>> GetAllGreenhouses()
        {
            var greennhouses = await _greenhouseRepository.GetAllAsync();
            return (greennhouses == null || greennhouses.Count == 0) ? null : _mapper.Map<List<GreenhouseDTO>>(greennhouses);
        }

        /// <summary>
        /// Lists all farmers employed within a specific greenhouse.
        /// </summary>
        public async Task<List<FarmerDTO>?> GetFarmersByGreenhouseIdAsync(Guid greenhouseId)
        {
            var greenhouse = await _greenhouseRepository.GetGreenhouseById(greenhouseId);
            if (greenhouse == null)
                throw new KeyNotFoundException("Greenhouse not found");

            return (greenhouse.Farmers == null || !greenhouse.Farmers.Any()) ? null : _mapper.Map<List<FarmerDTO>>(greenhouse.Farmers);
        }

        /// <summary>
        /// Fetches basic details for a specific greenhouse facility.
        /// </summary>
        public async Task<GreenhouseDTO> GetGreenhouseById(Guid id)
        {
            var greenhouse = await _greenhouseRepository.GetGreenhouseById(id);
            if (greenhouse == null)
                throw new KeyNotFoundException("Greenhouse not found");

            return _mapper.Map<GreenhouseDTO>(greenhouse);
        }

        /// <summary>
        /// Retrieves specific system notifications for a single greenhouse.
        /// </summary>
        public async Task<List<SystemReportDTO>?> GetGreenhouseNotifications(Guid greenhouseId)
        {
            var greenhouse = await _greenhouseRepository.GetGreenhouseById(greenhouseId);
            if (greenhouse == null)
                throw new KeyNotFoundException("Greenhouse not found");

            var reports = await _systemReportsRepository.GetByGreenhouseIdAsync(greenhouseId);
            return (reports == null || !reports.Any()) ? null : _mapper.Map<List<SystemReportDTO>>(reports);
        }

        /// <summary>
        /// Retrieves all greenhouses managed by a specific user.
        /// </summary>
        public async Task<List<GreenhouseDTO>?> GetGreenhousesByManagerIdAsync(Guid managerId)
        {
            var greenhouses = await _greenhouseRepository.GetGreenhousesByManagerIdAsync(managerId);
            return (greenhouses == null || !greenhouses.Any()) ? null : _mapper.Map<List<GreenhouseDTO>>(greenhouses);
        }

        /// <summary>
        /// Filters for greenhouses that currently have no manager assigned (Admin Utility).
        /// </summary>
        public async Task<List<GreenhouseDTO>?> GetGreenhousesWithoutManagerAsync()
        {
            var greenhouses = await _greenhouseRepository.GetGreenhousesWithoutManagerAsync();
            return (greenhouses == null || !greenhouses.Any()) ? null : _mapper.Map<List<GreenhouseDTO>>(greenhouses);
        }

        /// <summary>
        /// Retrieves the manager details for a specific greenhouse.
        /// </summary>
        public async Task<UserDTO> GetManagerByGreenhouseIdAsync(Guid greenhouseId)
        {
            var greenhouse = await _greenhouseRepository.GetGreenhouseById(greenhouseId);
            if (greenhouse == null)
                throw new KeyNotFoundException("Greenhouse not found");

            return _mapper.Map<UserDTO>(greenhouse.Manager);
        }

        /// <summary>
        /// Removes the current manager from a greenhouse, leaving it vacant.
        /// </summary>
        public async Task UnAssignManagerAsync(Guid GreenhouseId)
        {
            var greenhouse = await _greenhouseRepository.GetGreenhouseById(GreenhouseId);
            if (greenhouse == null)
                throw new KeyNotFoundException("Greenhouse not found");

            if (greenhouse.ManagerId == null)
                throw new Exception("The greenhouse does not have a manager");

            greenhouse.ManagerId = null;
            await _greenhouseRepository.UpdateAsync(greenhouse);
        }

        /// <summary>
        /// Updates greenhouse name, location, and replaces/deletes the associated image.
        /// </summary>
        public async Task UpdateGreenhouseAsync(Guid id, GreenhouseUpdateDTO dto)
        {
            var greenhouse = await _greenhouseRepository.GetGreenhouseById(id);
            if (greenhouse == null)
                throw new KeyNotFoundException("Greenhouse not found");

            if (!string.IsNullOrEmpty(dto.Name)) greenhouse.Name = dto.Name;
            if (!string.IsNullOrEmpty(dto.Location)) greenhouse.Location = dto.Location;

            // Handle image replacement logic
            if (!string.IsNullOrEmpty(dto.ImagePath))
            {
                if (!string.IsNullOrEmpty(greenhouse.ImageUrl))
                {
                    await _fileStorageService.DeleteFileAsync(greenhouse.ImageUrl);
                }
                greenhouse.ImageUrl = dto.ImagePath;
            }

            await _greenhouseRepository.UpdateAsync(greenhouse);
        }

        /// <summary>
        /// Marks a set of greenhouse-level notifications as read.
        /// </summary>
        public async Task MarkGreenhouseNotificationAsRead(List<Guid> ids)
        {
            var reports = await _systemReportsRepository.GetSystemReportsAsyncByIds(ids);
            foreach (var report in reports)
            {
                report.IsRead = true;
                await _systemReportsRepository.UpdateAsync(report);
            }
        }
    }
}