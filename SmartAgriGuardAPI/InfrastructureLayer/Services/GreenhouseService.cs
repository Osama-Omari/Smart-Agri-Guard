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
        public GreenhouseService(IGreenhouseRepository greenhouseRepository, IMapper mapper, IUserRepository userRepository)
        {
            _greenhouseRepository = greenhouseRepository;
            _mapper = mapper;
            _userRepository = userRepository;
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
    }
}
