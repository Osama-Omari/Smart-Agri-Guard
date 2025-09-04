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
        public GreenhouseService(IGreenhouseRepository greenhouseRepository, IMapper mapper)
        {
            _greenhouseRepository = greenhouseRepository;
            _mapper = mapper;
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
    }
}
