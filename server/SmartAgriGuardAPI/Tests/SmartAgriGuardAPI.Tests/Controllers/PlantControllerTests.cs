using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using WebAPILayer.Controllers;
using ApplicationLayer.Interfaces;
using ApplicationLayer.DTOs;
using Microsoft.AspNetCore.Http;
using WebAPILayer.RequestDTO;
using System.Security.Claims;

namespace SmartAgriGuardAPI.Tests.Controllers
{
    public class PlantControllerTests
    {
        private readonly Mock<IFileStorageService> _mockFileStorageService;
        private readonly Mock<IPlantService> _mockPlantService;
        private readonly Mock<IPlantScheduleService> _mockPlantScheduleService;
        private readonly PlantController _controller;

        public PlantControllerTests()
        {
            _mockFileStorageService = new Mock<IFileStorageService>();
            _mockPlantService = new Mock<IPlantService>();
            _mockPlantScheduleService = new Mock<IPlantScheduleService>();
            _controller = new PlantController(_mockFileStorageService.Object, _mockPlantService.Object, _mockPlantScheduleService.Object);
        }

        [Fact]
        public async Task GetPlantById_ReturnsOk_WhenPlantExists()
        {
            // Arrange
            var plantId = Guid.NewGuid();
            var plantDto = new PlantDTO { Id = plantId, PlantName = "Test Plant" };
            _mockPlantService.Setup(s => s.GetPlantById(plantId)).ReturnsAsync(plantDto);

            // Act
            var result = await _controller.GetPlantById(plantId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(plantDto, okResult.Value);
        }

        [Fact]
        public async Task GetPlantById_ReturnsNotFound_WhenPlantDoesNotExist()
        {
            // Arrange
            var plantId = Guid.NewGuid();
            _mockPlantService.Setup(s => s.GetPlantById(plantId)).Returns(Task.FromResult<PlantDTO>(null!));

            // Act
            var result = await _controller.GetPlantById(plantId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeletePlant_ReturnsOk_WhenDeletionSuccessful()
        {
            // Arrange
            var plantId = Guid.NewGuid();
            _mockPlantService.Setup(s => s.isPlnatAssignmentExists(plantId)).ReturnsAsync(false);
            _mockPlantService.Setup(s => s.DeletePlantAsync(plantId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeletePlant(plantId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("The plant has successfully deleted", okResult.Value);
        }

        [Fact]
        public async Task DeletePlant_ReturnsBadRequest_WhenAssignmentExists()
        {
            // Arrange
            var plantId = Guid.NewGuid();
            _mockPlantService.Setup(s => s.isPlnatAssignmentExists(plantId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeletePlant(plantId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("The plant has farmer assigned to it , remove the assignment first", badRequestResult.Value);
        }

        [Fact]
        public async Task GetAllGreenhousePlantsWithMetrics_ReturnsOk_WhenPlantsExist()
        {
             // Arrange
            var greenhouseId = Guid.NewGuid();
            var userTimeZone = "UTC";
            var plants = new List<PlantWithMetricsDTO> { new PlantWithMetricsDTO { Id = Guid.NewGuid(), PlantName = "Test Plant" } };
            
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
            
            _mockPlantService.Setup(s => s.GetAllGreenhousePlantsWithMetrics(greenhouseId, userTimeZone)).ReturnsAsync(plants);

            // Act
            var result = await _controller.GetAllGreenhousePlantsWithMetrics(greenhouseId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(plants, okResult.Value);
        }
    }
}
