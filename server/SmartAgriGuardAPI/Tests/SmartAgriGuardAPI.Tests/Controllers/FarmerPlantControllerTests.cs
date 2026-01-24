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
    public class FarmerPlantControllerTests
    {
        private readonly Mock<IFarmerPlantService> _mockFarmerPlantService;
        private readonly FarmerPlantController _controller;

        public FarmerPlantControllerTests()
        {
            _mockFarmerPlantService = new Mock<IFarmerPlantService>();
            _controller = new FarmerPlantController(_mockFarmerPlantService.Object);
        }

        [Fact]
        public async Task GetAssignedPlantsForFarmer_ReturnsOk_WhenPlantsExist()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var userTimeZone = "UTC";
            var plants = new List<PlantWithMetricsDTO> { new PlantWithMetricsDTO { Id = Guid.NewGuid(), PlantName = "Test Plant" } };

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, farmerId.ToString()),
                        new Claim("timezone", userTimeZone)
                    }))
                }
            };
            
            _mockFarmerPlantService.Setup(s => s.GetAssignedPlantsForFarmer(farmerId, userTimeZone)).ReturnsAsync(plants);

            // Act
            var result = await _controller.GetAssignedPlantsForFarmer();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(plants, okResult.Value);
        }
        
        [Fact]
        public async Task GetAssignedPlantsForFarmer_ReturnsUnauthorized_WhenUserIdInvalid()
        {
             // Arrange
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                     User = new ClaimsPrincipal(new ClaimsIdentity()) // Empty identity
                }
            };

            // Act
            var result = await _controller.GetAssignedPlantsForFarmer();

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid user ID.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task AssignFarmer_ReturnsOk_WhenAssignmentSuccessful()
        {
            // Arrange
            var plantId = Guid.NewGuid();
            var assignDto = new AssignFarmerDTO { farmersIds = new List<Guid> { Guid.NewGuid() } };
            _mockFarmerPlantService.Setup(s => s.AssignFarmers(plantId, assignDto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AssignFarmer(plantId, assignDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("the farmers have been assigned successfully", okResult.Value);
        }
    }
}
