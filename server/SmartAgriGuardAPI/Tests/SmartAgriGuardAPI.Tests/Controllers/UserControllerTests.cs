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
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _controller = new UserController(_mockUserService.Object);
        }

        [Fact]
        public async Task GetAllManagers_ReturnsOk_WhenManagersExist()
        {
            // Arrange
            var managers = new List<ManagerDTO> { new ManagerDTO { Id = Guid.NewGuid(), Username = "Manager1" } };
            _mockUserService.Setup(s => s.GetAllManagersAsync()).ReturnsAsync(managers);

            // Act
            var result = await _controller.GetAllManagers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(managers, okResult.Value);
        }

        [Fact]
        public async Task DeleteManager_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var managerId = Guid.NewGuid();
            _mockUserService.Setup(s => s.DeleteManagerAsync(managerId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteManager(managerId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Manager deleted successfully.", okResult.Value);
        }

        [Fact]
        public async Task ChangeUserInfo_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var dto = new UpdateUserDTO { FullName = "New Name" };
            var userDto = new UserDTO { Id = userId, FullName = "New Name" };

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                    }))
                }
            };

            _mockUserService.Setup(s => s.UpdateUserAsync(dto, userId)).ReturnsAsync(userDto);

            // Act
            var result = await _controller.ChangeUserInfo(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(userDto, okResult.Value);
        }

        [Fact]
        public async Task ChangeUserInfo_ReturnsUnauthorized_WhenUserIdInvalid()
        {
             // Arrange
            var dto = new UpdateUserDTO();
             _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                     User = new ClaimsPrincipal(new ClaimsIdentity()) // Empty identity
                }
            };

            // Act
            var result = await _controller.ChangeUserInfo(dto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid user ID.", unauthorizedResult.Value);
        }
    }
}
