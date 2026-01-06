using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureLayer.Services
{
    /// <summary>
    /// Service for sending push notifications using Firebase Cloud Messaging (FCM).
    /// Handles single-user and multicast notifications, as well as specific plant-care alerts.
    /// </summary>
    public class FirebaseNotificationService : INotificationService
    {
        private readonly IDeviceTokenRepository _deviceTokenRepository;
        private readonly IPlantRepository _plantRepository;

        public FirebaseNotificationService(IDeviceTokenRepository deviceTokenRepository, IPlantRepository plantRepository, IConfiguration configuration, IWebHostEnvironment env)
        {
            _deviceTokenRepository = deviceTokenRepository;
            _plantRepository = plantRepository;

            // Ensures Firebase is initialized once at the start of the service lifecycle
            InitializeFirebase(configuration, env);
        }

        /// <summary>
        /// Sends a push notification to a specific user based on their registered device token.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="title">The notification title.</param>
        /// <param name="message">The notification body text.</param>
        public async Task SendToUserAsync(Guid userId, string title, string message)
        {
            var deviceToken = await _deviceTokenRepository.GetTokenByUserIdAsync(userId);
            if (deviceToken == null) return;

            var msg = new Message()
            {
                Token = deviceToken.Token,
                Notification = new Notification
                {
                    Title = title,
                    Body = message
                }
            };

            try
            {
                // Capture the response string
                string response = await FirebaseMessaging.DefaultInstance.SendAsync(msg);
                Console.WriteLine("Successfully sent message: " + response);
            }
            catch (FirebaseMessagingException ex)
            {
                // This will tell you if the token is invalid, expired, or permissions are wrong
                Console.WriteLine("Error sending FCM message: " + ex.MessagingErrorCode);
                Console.WriteLine("Details: " + ex.Message);
            }
        }

        /// <summary>
        /// Sends a notification to multiple users simultaneously (Multicast).
        /// </summary>
        /// <param name="userIds">A collection of user IDs to receive the alert.</param>
        /// <param name="title">The notification title.</param>
        /// <param name="message">The notification body text.</param>
        public async Task SendToUsersAsync(IEnumerable<Guid> userIds, string title, string message)
        {
            var allTokens = new List<string>();
            foreach (var userId in userIds)
            {
                
                var deviceToken = await _deviceTokenRepository.GetTokenByUserIdAsync(userId);
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

        /// <summary>
        /// Notifies all farmers assigned to a plant that it requires watering.
        /// </summary>
        /// <param name="plantId">The ID of the plant requiring attention.</param>
        public async Task NotifyPlantNeedsWatering(Guid plantId)
        {
            var plant = await _plantRepository.GetPlantWithFarmerPlant(plantId);
            if (plant == null) return;

            // Extract unique farmer IDs assigned to this specific plant
            var farmerIds = plant.FarmerPlants.Select(fp => fp.FarmerId).Distinct();
            await SendToUsersAsync(farmerIds, "Plant Needs Watering", $"The plant {plant.Name} needs watering.");
        }

        /// <summary>
        /// Notifies all farmers assigned to a plant that it requires nutritional supplements.
        /// </summary>
        /// <param name="plantId">The ID of the plant requiring attention.</param>
        public async Task NotifyPlantNeedsNutrients(Guid plantId)
        {
            var plant = await _plantRepository.GetPlantWithFarmerPlant(plantId);
            if (plant == null) return;

            var farmerIds = plant.FarmerPlants.Select(fp => fp.FarmerId).Distinct();
            await SendToUsersAsync(farmerIds, "Plant Needs Nutrients", $"The plant {plant.Name} needs nutrients.");
        }

        /// <summary>
        /// Utility method to test notification delivery for administrators.
        /// </summary>
        public async Task NotifyAdminTest(Guid adminId)
        {
            await SendToUserAsync(adminId, "Test Notification", "This is a test notification for admin.");
        }

        /// <summary>
        /// Configures the Firebase Admin SDK using a service account JSON file.
        /// </summary>
        /// <param name="configuration">Application configuration to read the file path.</param>
        /// <param name="env">Hosting environment to resolve the physical path.</param>
        /// <exception cref="InvalidOperationException">Thrown if configuration path is missing.</exception>
        /// <exception cref="FileNotFoundException">Thrown if the service account file does not exist.</exception>
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