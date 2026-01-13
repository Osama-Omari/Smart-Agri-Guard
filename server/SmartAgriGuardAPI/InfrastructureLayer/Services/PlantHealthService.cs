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
        private readonly INotificationService _notificationService;
        private readonly IPlantNotificationsRepository _plantNotificationsRepository;
        public PlantHealthService(IPlantHealthModel plantHealthModel,
            IPredictionRepository predictionRepository,
            IPlantRepository plantRepository,
            INotificationService notificationService,
            IPlantNotificationsRepository plantNotificationsRepository)
        {
            _plantHealthModel = plantHealthModel;
            _predictionRepository = predictionRepository;
            _plantRepository = plantRepository;
            _notificationService = notificationService;
            _plantNotificationsRepository = plantNotificationsRepository;
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


        public async Task EvaluateAndAlertPlantHealth(Plant plant, SensorData latestData)
        {
            var thresholds = GetThresholdsForPlantType(plant);
            var alerts = new List<string>();
            
            if (latestData.Temperature < thresholds.TempMin || latestData.Temperature > thresholds.TempMax)
            {
                alerts.Add($"Temperature out of range: {latestData.Temperature}°C");
            }
            if (latestData.Humidity < thresholds.HumidityMin)
            {
                alerts.Add($"Humidity too low: {latestData.Humidity}%");
            }
            if (latestData.SoilMoisture < thresholds.SoilMoistureLow)
            {
                alerts.Add($"Soil moisture too low: {latestData.SoilMoisture}%");
            }
            if (latestData.Nitrogen < thresholds.NLow)
            {
                alerts.Add($"Nitrogen level too low: {latestData.Nitrogen} mg/kg");
            }
            if (latestData.Phosphorus < thresholds.PLow)
            {
                alerts.Add($"Phosphorus level too low: {latestData.Phosphorus} mg/kg");
            }
            if (latestData.Potassium < thresholds.KLow)
            {
                alerts.Add($"Potassium level too low: {latestData.Potassium} mg/kg");
            }
            if (latestData.Ph < thresholds.PhMin || latestData.Ph > thresholds.PhMax)
            {
                alerts.Add($"pH out of range: {latestData.Ph}");
            }
            if (alerts.Any())
            {
                var plantNotification = new PlantNotifications
                {
                    PlantId = plant.Id,
                    NotificationDate = DateTimeOffset.UtcNow,
                    Message = string.Join("; ", alerts),
                    IsRead = false,
                    TriggerType = "CriticalCondition"
                };
                await _plantNotificationsRepository.AddAsync(plantNotification);

                var message = $"Critical conditions detected for plant {plant.Name}:\n" + string.Join("\n", alerts);
                await _notificationService.SendPlantAlertAsync(plant.Id, message);
            }

        }


        private PlantThresholdProfile GetThresholdsForPlantType(Plant plant)
        {
            var typeName = plant.PlantType?.Name?.ToLower() ?? "";

            return typeName switch
            {
                "tomato" => GetTomatoThresholds(),
                // "cucumber" => GetCucumberThresholds(),
                // "pepper" => GetPepperThresholds(),
                _ => GetDefaultThresholds()
            };
        }

        private PlantThresholdProfile GetDefaultThresholds()
        {
            return new PlantThresholdProfile
            {
                TempMin = 15,
                TempMax = 32,
                HumidityMin = 50,
                SoilMoistureLow = 40,
                NLow = 25,
                PLow = 15,
                KLow = 35,
                PhMin = 5.5,
                PhMax = 7.5
            };
        }
        private PlantThresholdProfile GetTomatoThresholds()
        {
            return new PlantThresholdProfile
            {
                // Temperature (°C)
                TempMin = 18,
                TempMax = 30,

                // Humidity (%)
                HumidityMin = 60,

                // Soil moisture (%)
                SoilMoistureLow = 45, 

                // NPK thresholds in mg/kg
                NLow = 30,     // mg/kg
                PLow = 20,     // mg/kg
                KLow = 150,    // mg/kg

                // pH
                PhMin = 5.5,
                PhMax = 6.8
            };
        }
    }
}
