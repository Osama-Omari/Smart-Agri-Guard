using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using InfrastructureLayer.AI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureLayer.Services
{
    public class PlantHealthService : IPlantHealthService
    {
        private readonly IPlantHealthModel _plantHealthModel;
        private readonly IPredictionRepository _predictionRepository;
        private readonly IPlantRepository _plantRepository;
        public PlantHealthService(IPlantHealthModel plantHealthModel,IPredictionRepository predictionRepository,IPlantRepository plantRepository)
        {
            _plantHealthModel = plantHealthModel;
            _predictionRepository = predictionRepository;
            _plantRepository = plantRepository;
        }

        public async Task GeneratePlantHealth(Guid PlantId, TomatoHealthInput input)
        {
            var plant  = await _plantRepository.GetPlantById(PlantId);
            if(plant == null)
                throw new Exception("Plant not found");
            var healthStatus = await _plantHealthModel.PredictAsync(input);
            var prediction = new Prediction
            {
                healthStatus = healthStatus.ToString(),
                PlantId = PlantId,
                PredictionDate = DateTimeOffset.UtcNow
            };
            await _predictionRepository.AddAsync(prediction);
        }
    }
}
