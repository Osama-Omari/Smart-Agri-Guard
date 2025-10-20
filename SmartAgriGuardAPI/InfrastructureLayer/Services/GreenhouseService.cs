using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces;
using AutoMapper;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureLayer.Services
{
    public class GreenhouseService : IGreenhouseService
    {
        private readonly IGreenhouseRepository _greenhouseRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IFileStorageService _fileStorageService;
        public GreenhouseService(IGreenhouseRepository greenhouseRepository, IMapper mapper, IUserRepository userRepository,IFileStorageService fileStorageService)
        {
            _greenhouseRepository = greenhouseRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _fileStorageService = fileStorageService;
        }

        public async Task<GreenhouseDTO> AddGreenhouse(GreenhouseRegisterDTO dto)
        {
            var greenhouse = new Greenhouse();
            greenhouse.Name = dto.Name;
            greenhouse.Location = dto.Location;
            if (!string.IsNullOrEmpty(dto.ImagePath))
            {
                greenhouse.ImageUrl = dto.ImagePath;
            }
            await _greenhouseRepository.AddAsync(greenhouse);
            return _mapper.Map<GreenhouseDTO>(greenhouse);
        }

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
                throw new Exception("The greenhouse has a manager");

            greenhouse.ManagerId = managerId;

            await _greenhouseRepository.UpdateAsync(greenhouse);
        }

        public async Task DeleteGreenhouseAsync(Guid id)
        {
            var greenhouse = await _greenhouseRepository.GetGreenhouseById(id);
            if (greenhouse == null)
                throw new KeyNotFoundException("greenhouse not found");
            if(greenhouse.Plants != null && greenhouse.Plants.Any())
                throw new Exception("Cannot delete greenhouse with assigned plants");
            if(greenhouse.Farmers != null && greenhouse.Farmers.Any())
                throw new Exception("Cannot delete greenhouse with assigned farmers");

            await _fileStorageService.DeleteFileAsync(greenhouse.ImageUrl);
            await _greenhouseRepository.DeleteAsync(id);
        }

        public async Task<List<GreenhouseDTO>> GetAllGreenhouses()
        {
            var greennhouses = await _greenhouseRepository.GetAllAsync();
            if (greennhouses == null || greennhouses.Count == 0)
                return null;
            return _mapper.Map<List<GreenhouseDTO>>(greennhouses);
            
        }

        public async Task<GreenhouseDTO> GetGreenhouseById(Guid id)
        {
            var greenhouse = await _greenhouseRepository.GetGreenhouseById(id);
            if (greenhouse == null)
                throw new KeyNotFoundException("greenhouse not found");
            return _mapper.Map<GreenhouseDTO>(greenhouse);
        }

        public async Task<List<GreenhouseDTO>> GetGreenhousesByManagerIdAsync(Guid managerId)
        {
            var greenhouses = await _greenhouseRepository.GetGreenhousesByManagerIdAsync(managerId);
            if (greenhouses == null || !greenhouses.Any())
                return null;
            return _mapper.Map<List<GreenhouseDTO>>(greenhouses);

        }

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

        public async Task UpdateGreenhouseAsync(Guid id, GreenhouseUpdateDTO dto)
        {
            var greenhouse = await _greenhouseRepository.GetGreenhouseById(id);
            if (greenhouse == null)
                throw new KeyNotFoundException("greenhouse not found");
            if(!string.IsNullOrEmpty(dto.Name))
                greenhouse.Name = dto.Name;
            if(!string.IsNullOrEmpty(dto.Location))
                greenhouse.Location = dto.Location;
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
    }
}
