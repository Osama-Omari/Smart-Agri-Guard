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
        private readonly ISensorDataService _sensorDataService;
        private readonly ISensorDataArchiveService _sensorDataArchiveService;
        
        public PlantService(IPlantRepository plantRepository, IPlantTypeRepository plantTypeRepository,IMapper mapper,IGreenhouseRepository greenhouseRepository,
            ISensorDataService sensorDataService,ISensorDataArchiveService sensorDataArchiveService)
        {
            _plantRepository = plantRepository;
            _plantTypeRepository = plantTypeRepository;
            _mapper = mapper;
            _greenhouseRepository = greenhouseRepository;
            _sensorDataService = sensorDataService;
            _sensorDataArchiveService = sensorDataArchiveService;
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

        public async Task<bool> isPlnatAssignmentExists(Guid PlantId)
        {
            var plant = await _plantRepository.GetPlantWithFarmerPlant(PlantId);
            if (plant == null)
                throw new KeyNotFoundException("plant not found");
            if(plant.FarmerPlants.Any())
                return true;
            return false;
        }

        public async Task DeletePlantAsync(Guid PlantId)
        {
            var plant = await _plantRepository.GetPlantWithFarmerPlant(PlantId);
            if (plant == null)
                throw new KeyNotFoundException("the plant not found");
            await _sensorDataService.DeleteAllByPlantIdAsync(plant.Id);
            await _sensorDataArchiveService.DeleteAllByPlantIdAsync(plant.Id);
            await _plantRepository.DeleteAsync(plant.Id);
        }

        public async Task<List<PlantDTO>> GetAllGreenhousePlants(Guid GreenhouseId)
        {
            var plants = await _plantRepository.GetAllGreenhousePlantsAsync(GreenhouseId);
            if (plants == null || plants.Count == 0)
                return null;
            return _mapper.Map<List<PlantDTO>>(plants);
        }

        public async Task<PlantDTO> GetPlantById(Guid PlantId)
        {
            var plant = await _plantRepository.GetPlantById(PlantId);
            if (plant == null)
                return null;
            return _mapper.Map<PlantDTO>(plant);
        }
    }
}
