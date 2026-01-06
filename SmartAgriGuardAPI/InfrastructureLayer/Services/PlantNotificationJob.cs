using ApplicationLayer.Interfaces;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureLayer.Services
{
    public class PlantNotificationJob : IPlantNotificationJob
    {
        private readonly INotificationService _notificationService;
        private readonly IPlantNotificationsRepository _plantNotificationsRepository;
        private readonly IPlantRepository _plantRepository;
        public PlantNotificationJob(INotificationService notificationService, IPlantNotificationsRepository plantNotificationsRepository, IPlantRepository plantRepository)
        {
            _notificationService = notificationService;
            _plantRepository = plantRepository;
            _plantNotificationsRepository = plantNotificationsRepository;
        }

        public async Task ExecuteNotification(Guid plantId, string TaskType)
        {
            var plant = await _plantRepository.GetPlantWithFarmerPlant(plantId);
            if (plant == null || plant.FarmerPlants == null) return;

            string message = $"Attention! It's time for {TaskType} on plant: {plant.Name}";

            var notification = new PlantNotifications
            {
                PlantId = plantId,
                NotificationDate = DateTimeOffset.UtcNow,
                TriggerType = TaskType,
                Message = message,
                IsRead = false,


            };
            await _plantNotificationsRepository.AddAsync(notification);

            foreach (var farmerPlant in plant.FarmerPlants)
            {
                if (farmerPlant.Farmer != null)
                {
                    await _notificationService.SendToUserAsync(farmerPlant.Farmer.Id, "Plant Care Reminder", message);
                }
            }


        }
    }
}
