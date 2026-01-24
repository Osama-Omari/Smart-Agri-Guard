using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using WebAPILayer.Controllers;
using ApplicationLayer.Interfaces;
using ApplicationLayer.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace SmartAgriGuardAPI.Tests.Controllers
{
    public class NotificationControllerTests
    {
        private readonly Mock<INotificationService> _mockNotificationService;
        private readonly Mock<IPlantService> _mockPlantService;
        private readonly Mock<IGreenhouseService> _mockGreenhouseService;
        private readonly NotificationController _controller;

        public NotificationControllerTests()
        {
            _mockNotificationService = new Mock<INotificationService>();
            _mockPlantService = new Mock<IPlantService>();
            _mockGreenhouseService = new Mock<IGreenhouseService>();
            _controller = new NotificationController(_mockNotificationService.Object, _mockPlantService.Object, _mockGreenhouseService.Object);
        }

        [Fact]
        public async Task GetPlantNotifications_ReturnsOk_WhenNotificationsExist()
        {
             // Arrange
            var plantId = Guid.NewGuid();
            var userTimeZone = "UTC";
            var notifications = new List<PlantNotificationDTO> { new PlantNotificationDTO { Message = "Test" } };

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim("timezone", userTimeZone)
                    }))
                }
            };
            
            _mockPlantService.Setup(s => s.GetPlantNotificationDTOs(plantId, userTimeZone)).ReturnsAsync(notifications);

            // Act
            var result = await _controller.GetPlantNotifications(plantId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(notifications, okResult.Value);
        }

        [Fact]
        public async Task MarkNotificationsAsRead_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var notificationIds = new List<Guid> { Guid.NewGuid() };
            _mockPlantService.Setup(s => s.MarkPlantNotificationsAsRead(notificationIds)).Returns(Task.CompletedTask);

             // Act
            var result = await _controller.MarkNotificationsAsRead(notificationIds);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            // Dynamic check or value check
            Assert.NotNull(okResult.Value);
        }

         [Fact]
        public async Task MarkNotificationsAsRead_ReturnsBadRequest_WhenIdsAreNullOrEmpty()
        {
             // Act
            var result = await _controller.MarkNotificationsAsRead(new List<Guid>());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Notification IDs are required.", badRequestResult.Value);
        }
    }
}
