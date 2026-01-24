using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using WebAPILayer.Controllers;
using ApplicationLayer.Interfaces;
using ApplicationLayer.DTOs;
using WebAPILayer.RequestDTO;
using Microsoft.AspNetCore.Http;
using DataAccessLayer.Models;

namespace SmartAgriGuardAPI.Tests.Controllers
{
    public class GreenhouseControllerTests
    {
        private readonly Mock<IFileStorageService> _mockFileStorageService;
        private readonly Mock<IGreenhouseService> _mockGreenhouseService;
        private readonly GreenhouseController _controller;

        public GreenhouseControllerTests()
        {
            _mockFileStorageService = new Mock<IFileStorageService>();
            _mockGreenhouseService = new Mock<IGreenhouseService>();
            _controller = new GreenhouseController(_mockFileStorageService.Object, _mockGreenhouseService.Object);
        }

        [Fact]
        public async Task CreateGreenhouse_ReturnsOk_WhenCreationSuccessful()
        {
            // Arrange
            var dto = new CreateGreenhouseRequestDTO { Name = "Greenhouse 1", Location = "Location 1" };
            var greenhouseDto = new GreenhouseDTO { Name = "Greenhouse 1", Location = "Location 1" };
            _mockGreenhouseService.Setup(s => s.AddGreenhouse(It.IsAny<GreenhouseRegisterDTO>())).ReturnsAsync(greenhouseDto);

            // Act
            var result = await _controller.CreateGreenhouse(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("The greenhhouse registered successfully", okResult.Value);
        }

        [Fact]
        public async Task GetGreenhouseById_ReturnsOk_WhenGreenhouseExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var greenhouse = new GreenhouseDTO { Id = id, Name = "Greenhouse 1" };
            _mockGreenhouseService.Setup(s => s.GetGreenhouseById(id)).ReturnsAsync(greenhouse);

            // Act
            var result = await _controller.GetGreenhouseById(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(greenhouse, okResult.Value);
        }

         [Fact]
        public async Task GetGreenhouseById_ReturnsNotFound_WhenGreenhouseDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockGreenhouseService.Setup(s => s.GetGreenhouseById(id)).Returns(Task.FromResult<GreenhouseDTO>(null!));

            // Act
            var result = await _controller.GetGreenhouseById(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AssignManagerToGreenhouse_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var managerId = Guid.NewGuid();
            var greenhouseId = Guid.NewGuid();
            _mockGreenhouseService.Setup(s => s.AssignManagerAsync(managerId, greenhouseId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AssignManagerToGreenhouse(managerId, greenhouseId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Manager assigned to greenhouse successfully.", okResult.Value);
        }
    }
}
