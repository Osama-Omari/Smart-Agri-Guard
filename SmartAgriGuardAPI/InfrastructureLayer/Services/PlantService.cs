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
    public class PlantService : IPlantService
    {
        private readonly IPlantRepository _plantRepository;
        private readonly IPlantTypeRepository _plantTypeRepository;
        private readonly IMapper _mapper;
        private readonly IGreenhouseRepository _greenhouseRepository;
        public PlantService(IPlantRepository plantRepository, IPlantTypeRepository plantTypeRepository,IMapper mapper,IGreenhouseRepository greenhouseRepository)
        {
            _plantRepository = plantRepository;
            _plantTypeRepository = plantTypeRepository;
            _mapper = mapper;
            _greenhouseRepository = greenhouseRepository;
        }

        public async Task AddPlantToGreenhouse(Guid GreenhouseId, PlantRegisterDTO dTO)
        {
            var greenhoues = await _greenhouseRepository.GetGreenhouseById(GreenhouseId);
            if(greenhoues == null)
                throw new KeyNotFoundException("Greenhouse not found.");

            var plantType = await _plantTypeRepository.GetByIdAsync(dTO.PlantTypeId);
            if (plantType == null)
                throw new KeyNotFoundException("PlantType not found.");

            var plant = new Plant
            {
                GreenhouseId = GreenhouseId,
                PlantTypeId = dTO.PlantTypeId,
                Name = dTO.Name,
            };
            if(!string.IsNullOrEmpty(dTO.ImagePath))
                plant.ImageUrl = dTO.ImagePath;
            if(!string.IsNullOrEmpty(dTO.Location))
                plant.Location = dTO.Location;
            await _plantRepository.AddAsync(plant);
        }
    }
}
