using ApplicationLayer.Interfaces;
using DataAccessLayer.Interfaces;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
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
        private readonly IUserRepository _userRepository;

        public FirebaseNotificationService(IDeviceTokenRepository deviceTokenRepository, IPlantRepository plantRepository, IUserRepository userRepository)
        {
            _deviceTokenRepository = deviceTokenRepository;
            _plantRepository = plantRepository;
            _userRepository = userRepository;

            if(FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = Google.Apis.Auth.OAuth2.GoogleCredential.FromFile("Config/smartagriguard-ec6f7-firebase-adminsdk-fbsvc-9538d4a6e9.json"),
                });
            }

        }

        public async Task SendToUserAsync(Guid userId, string title, string message)
        {
            var deviceTokens = await _deviceTokenRepository.GetTokensByUserIdAsync(userId);
            if (deviceTokens == null || !deviceTokens.Any())
            {
                return;
            }
            var messaging = FirebaseAdmin.Messaging.FirebaseMessaging.DefaultInstance;
            var multicastMessage = new MulticastMessage
            {
                Tokens = deviceTokens.Select(dt => dt.Token).ToList(),
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
                var deviceTokens = await _deviceTokenRepository.GetTokensByUserIdAsync(userId);
                if (deviceTokens != null && deviceTokens.Any())
                {
                    allTokens.AddRange(deviceTokens.Select(dt => dt.Token));
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


    }
}
