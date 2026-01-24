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
    public class SensorDataControllerTests
    {
        private readonly Mock<ISensorDataService> _mockSensorDataService;
        private readonly Mock<ISensorDataArchiveService> _mockArchiveService;
        private readonly SensorDataController _controller;

        public SensorDataControllerTests()
        {
            _mockSensorDataService = new Mock<ISensorDataService>();
            _mockArchiveService = new Mock<ISensorDataArchiveService>();
            _controller = new SensorDataController(_mockSensorDataService.Object, _mockArchiveService.Object);
        }

        [Fact]
        public async Task AddSensorData_ReturnsOk_WhenDataIsValid()
        {
            // Arrange
            var plantId = Guid.NewGuid();
            var dto = new SensorDataRegisterDTO { Temperature = 25, Humidity = 60 };
            _mockSensorDataService.Setup(s => s.AddSensorData(plantId, dto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AddSensorData(plantId, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Sensor data added successfully.", okResult.Value);
        }

        [Fact]
        public async Task AddSensorData_ReturnsNotFound_WhenPlantIdNotFound()
        {
            // Arrange
            var plantId = Guid.NewGuid();
            var dto = new SensorDataRegisterDTO { Temperature = 25, Humidity = 60 };
            _mockSensorDataService.Setup(s => s.AddSensorData(plantId, dto)).ThrowsAsync(new KeyNotFoundException("Plant not found"));

            // Act
            var result = await _controller.AddSensorData(plantId, dto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Plant not found", notFoundResult.Value);
        }

        [Fact]
        public async Task GetLatestSensorData_ReturnsOk_WhenDataExists()
        {
            // Arrange
            var plantId = Guid.NewGuid();
            var userTimeZone = "UTC";
            var expectedData = new SensorDataDTO { Temperature = 25, Humidity = 60 };

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

            _mockSensorDataService.Setup(s => s.GetLatestSensorData(plantId, userTimeZone)).ReturnsAsync(expectedData);

            // Act
            var result = await _controller.GetLatestSensorData(plantId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedData, okResult.Value);
        }

        [Fact]
        public async Task GetLatestSensorData_ReturnsNotFound_WhenDataDoesNotExist()
        {
             // Arrange
            var plantId = Guid.NewGuid();
            var userTimeZone = "UTC";

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

            _mockSensorDataService.Setup(s => s.GetLatestSensorData(plantId, userTimeZone)).ReturnsAsync((SensorDataDTO?)null);

            // Act
            var result = await _controller.GetLatestSensorData(plantId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("No sensor data found", notFoundResult.Value?.ToString());
        }
    }
}
