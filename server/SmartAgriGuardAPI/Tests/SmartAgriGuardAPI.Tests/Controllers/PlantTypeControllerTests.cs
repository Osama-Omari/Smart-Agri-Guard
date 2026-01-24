using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using WebAPILayer.Controllers;
using ApplicationLayer.Interfaces;
using ApplicationLayer.DTOs;
using DataAccessLayer.Models;

namespace SmartAgriGuardAPI.Tests.Controllers
{
    public class PlantTypeControllerTests
    {
        private readonly Mock<IPlantTypeService> _mockPlantTypeService;
        private readonly PlantTypeController _controller;

        public PlantTypeControllerTests()
        {
            _mockPlantTypeService = new Mock<IPlantTypeService>();
            _controller = new PlantTypeController(_mockPlantTypeService.Object);
        }

        [Fact]
        public async Task GetAllPlantTypes_ReturnsOk_WhenTypesExist()
        {
            // Arrange
            var types = new List<PlantTypeDTO> { new PlantTypeDTO { Id = Guid.NewGuid(), Name = "Rose" } };
            _mockPlantTypeService.Setup(s => s.GetAllPlantTypes()).ReturnsAsync(types);

            // Act
            var result = await _controller.GetAllPlantTypes();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(types, okResult.Value);
        }

        [Fact]
        public async Task GetAllPlantTypes_ReturnsNotFound_WhenNoTypes()
        {
            // Arrange
            _mockPlantTypeService.Setup(s => s.GetAllPlantTypes()).ReturnsAsync(new List<PlantTypeDTO>());

            // Act
            var result = await _controller.GetAllPlantTypes();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No plant types found.", notFoundResult.Value);
        }

        [Fact]
        public async Task AddPlantType_ReturnsOk_WhenSuccessful()
        {
             // Arrange
            var dto = new PlantTypeRegisterDTO { Name = "Tulip" };
            _mockPlantTypeService.Setup(s => s.AddPlantType(dto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AddPlantType(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("The plantType has been registered successfully", okResult.Value);
        }
    }
}
