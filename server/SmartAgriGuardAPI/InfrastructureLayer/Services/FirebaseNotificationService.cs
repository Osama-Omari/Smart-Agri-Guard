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
        private readonly IUserRepository _userRepository;

        public FirebaseNotificationService(IDeviceTokenRepository deviceTokenRepository, IPlantRepository plantRepository,IUserRepository userRepository)
        {
            _deviceTokenRepository = deviceTokenRepository;
            _plantRepository = plantRepository;
            _userRepository = userRepository;
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
        public async Task SendToUsersAsync(
            IEnumerable<Guid> userIds,
            string title,
            string message
            )
        {
            // 1️⃣ Fetch tokens in parallel (much faster)
            var tokenTasks = userIds.Select(async userId =>
            {
                var deviceToken = await _deviceTokenRepository.GetTokenByUserIdAsync(userId);
                return deviceToken?.IsActive == true ? deviceToken.Token : null;
            });

            var tokens = (await Task.WhenAll(tokenTasks))
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Distinct()
                .ToList();

            // 2️⃣ No tokens → nothing to send
            if (!tokens.Any())
                return;

            var multicast = new MulticastMessage
            {
                Tokens = tokens,
                Notification = new Notification
                {
                    Title = title,
                    Body = message
                }
            };

            // 3️⃣ Send notification
            var response = await FirebaseMessaging.DefaultInstance
                .SendEachForMulticastAsync(multicast);

            // 4️⃣ Deactivate invalid tokens safely
            if (response.FailureCount > 0)
            {
                var deactivateTasks = new List<Task>();

                for (int i = 0; i < response.Responses.Count; i++)
                {
                    var result = response.Responses[i];

                    if (!result.IsSuccess &&
                        result.Exception is FirebaseMessagingException ex &&
                        (ex.MessagingErrorCode == MessagingErrorCode.Unregistered ||
                         ex.MessagingErrorCode == MessagingErrorCode.InvalidArgument))
                    {
                        deactivateTasks.Add(
                            _deviceTokenRepository.DeactivateTokenAsync(tokens[i])
                        );
                    }
                }

                await Task.WhenAll(deactivateTasks);
            }
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

        public async Task SendToAdmin(string title, string message)
        {
            var admins = await _userRepository.GetAdmins();
            var adminIds = admins.Select(a => a.Id);
            await SendToUsersAsync(adminIds, title, message);


        }

        public async Task SendPlantAlertAsync(Guid plantId, string message)
        {
            var plant =  await _plantRepository.GetPlantWithFarmerPlant(plantId);
            var farmerIds = plant.FarmerPlants?
            .Select(fp => fp.FarmerId)
            .Distinct()
            .ToList();

            if (farmerIds == null || !farmerIds.Any())
                return;
            await SendToUsersAsync(farmerIds, $"Alert for Plant: {plant.Name}", message);

        }
    }
}