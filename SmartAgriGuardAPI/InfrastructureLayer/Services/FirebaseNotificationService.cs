using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Repositories;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureLayer.Services
{
    public class FirebaseNotificationService : INotificationService
    {
        private readonly IDeviceTokenRepository _deviceTokenRepository;
        private readonly IPlantRepository _plantRepository;

        public FirebaseNotificationService(IDeviceTokenRepository deviceTokenRepository, IPlantRepository plantRepository, IConfiguration configuration, IWebHostEnvironment env)
        {
            _deviceTokenRepository = deviceTokenRepository;
            _plantRepository = plantRepository;

            InitializeFirebase(configuration, env);

        }

        public async Task SendToUserAsync(Guid userId, string title, string message)
        {
            var deviceToken = await _deviceTokenRepository.GetTokenByUserIdAsync(userId);
            if (deviceToken == null)
            {
                return;
            }

            var messaging = FirebaseAdmin.Messaging.FirebaseMessaging.DefaultInstance;
            var multicastMessage = new MulticastMessage
            {
                Tokens = new List<string> { deviceToken.Token }, // FIX: wrap token in a list
                Notification = new Notification
                {
                    Title = title,
                    Body = message
                }
            };
            await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(multicastMessage);
        }

        public async Task SendToUsersAsync(IEnumerable<Guid> userIds, string title, string message)
        {
            var allTokens = new List<string>();
            foreach (var userId in userIds)
            {
                var deviceToken = await _deviceTokenRepository.GetTokenByIdAsync(userId);
                if (deviceToken != null)
                {
                    allTokens.Add(deviceToken.Token);
                }
            }

            if (!allTokens.Any())
            {
                return;
            }

            var multicast = new MulticastMessage
            {
                Tokens = allTokens,
                Notification = new Notification
                {
                    Title = title,
                    Body = message
                }
            };

            await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(multicast);
        }

        public async Task NotifyPlantNeedsWatering(Guid plantId)
        {
            var plant = await _plantRepository.GetPlantWithFarmerPlant(plantId);
            if (plant == null) return;

            var farmerIds = plant.FarmerPlants.Select(fp => fp.FarmerId).Distinct();
            await SendToUsersAsync(farmerIds, "Plant Needs Watering", $"The plant {plant.Name} needs watering.");
        }

        public async Task NotifyPlantNeedsNutrients(Guid plantId)
        {
            var plant = await _plantRepository.GetPlantWithFarmerPlant(plantId);
            if (plant == null) return;
            var farmerIds = plant.FarmerPlants.Select(fp => fp.FarmerId).Distinct();
            await SendToUsersAsync(farmerIds, "Plant Needs Nutrients", $"The plant {plant.Name} needs nutrients.");
        }

        //test sending notification for admin
        public async Task NotifyAdminTest(Guid adminId)
        {
            await SendToUserAsync(adminId, "Test Notification", "This is a test notification for admin.");
        }


        private void InitializeFirebase(IConfiguration configuration, IWebHostEnvironment env)
        {
            if (FirebaseApp.DefaultInstance != null)
                return;

            var relativePath = configuration["Firebase:ServiceAccountPath"];
            if (string.IsNullOrWhiteSpace(relativePath))
                throw new InvalidOperationException("Firebase ServiceAccountPath is not configured.");

            var fullPath = Path.Combine(env.ContentRootPath, relativePath);

            if (!File.Exists(fullPath))
                throw new FileNotFoundException("Firebase service account file not found.", fullPath);

            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile(fullPath)
            });
        }


    }
}
