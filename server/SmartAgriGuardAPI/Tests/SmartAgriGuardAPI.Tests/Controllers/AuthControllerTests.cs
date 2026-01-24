using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using WebAPILayer.Controllers;
using ApplicationLayer.Interfaces;
using ApplicationLayer.DTOs;
using System.Threading.Tasks;

namespace SmartAgriGuardAPI.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IJWTService> _mockJwtService;
        private readonly Mock<IUserService> _mockUserService;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockJwtService = new Mock<IJWTService>();
            _mockUserService = new Mock<IUserService>();
            _controller = new AuthController(_mockJwtService.Object, _mockUserService.Object);
        }

        [Fact]
        public async Task Login_ReturnsOk_WhenCredentialsAreValid()
        {
            // Arrange
            var loginDto = new UserLoginDTO { UserName = "testuser", Password = "password" };
            var userDto = new UserDTO { Id = Guid.NewGuid(), Username = "testuser", RoleName = "Manager" };
            var token = "generated_token";

            _mockUserService.Setup(s => s.Authenticate(loginDto)).ReturnsAsync(userDto);
            _mockJwtService.Setup(s => s.GenerateToken(userDto)).Returns(token);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var value = jsonResult.Value;
            Assert.NotNull(value);
            // Quick check via reflection or dynamic if simple anonymous type
            var tokenProperty = value.GetType().GetProperty("Token")?.GetValue(value, null);
            Assert.Equal(token, tokenProperty);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange
            var loginDto = new UserLoginDTO { UserName = "testuser", Password = "wrongpassword" };
            _mockUserService.Setup(s => s.Authenticate(loginDto)).ReturnsAsync((UserDTO?)null);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid username or password", unauthorizedResult.Value);
        }

        [Fact]
        public async Task RegisterManager_ReturnsOk_WhenRegistrationSuccessful()
        {
             // Arrange
            var dto = new ManagerRegisterDTO { UserName = "newmanager", Password = "password" };
            _mockUserService.Setup(s => s.isUserNameExists(dto.UserName)).ReturnsAsync(false);
            _mockUserService.Setup(s => s.RegisterManager(dto)).ReturnsAsync(new UserDTO());

            // Act
            var result = await _controller.RegisterManager(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("The manager registered successfully", okResult.Value);
        }

         [Fact]
        public async Task RegisterManager_ReturnsBadRequest_WhenUserNameExists()
        {
             // Arrange
            var dto = new ManagerRegisterDTO { UserName = "existinguser", Password = "password" };
            _mockUserService.Setup(s => s.isUserNameExists(dto.UserName)).ReturnsAsync(true);

            // Act
            var result = await _controller.RegisterManager(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("UserName Already Exist", badRequestResult.Value);
        }
    }
}
