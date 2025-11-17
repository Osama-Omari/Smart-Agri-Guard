using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces;
using DocumentFormat.OpenXml.ExtendedProperties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPILayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJWTService _jwtService;
        private readonly IUserService _userService;

        public AuthController(IJWTService jwtService, IUserService userService)
        {
            _jwtService = jwtService;
            _userService = userService;
        }

        [HttpPost("Register-Manager")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterManager([FromBody] ManagerRegisterDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (await _userService.isUserNameExists(dto.UserName))
                {
                    return BadRequest("UserName Already Exist");
                }

                var user = await _userService.RegisterManager(dto);
                return Ok("The manager registered successfully");


            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }

        }

        [HttpPost("Register-Farmer/{GreehouseId}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> RegisterFarmer([FromBody] FarmerRegisterDTO dto, [FromRoute] Guid GreehouseId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (await _userService.isUserNameExists(dto.UserName))
                {
                    return BadRequest("UserName Already Exist");
                }

                var user = await _userService.RegisterFarmer(dto, GreehouseId);
                return Ok("The farmer registered successfully");


            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }

        }

        [HttpPost("Register-Admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] AdminRegisterDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                if (await _userService.isUserNameExists(dto.userName))
                {
                    return BadRequest("UserName Already Exist");
                }
                await _userService.RegisterAdmin(dto);
                return Ok("the admin registered successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");

            }
        }



        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _userService.Authenticate(dto);
                if (user == null)
                {
                    return Unauthorized("Invalid username or password");
                }
                var token = _jwtService.GenerateToken(user);
                return Ok(new { Token = token, User = user });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }


        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDTO dto)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {

                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Guid.TryParse(userIdString, out Guid userId);
                if (userId == Guid.Empty)
                {
                    return Unauthorized("Invalid user ID.");
                }
                await _userService.LogoutAsync(dto);
                return Ok("Logged out successfully.");

            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }
    }
}

