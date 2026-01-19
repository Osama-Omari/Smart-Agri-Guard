using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces;
using DocumentFormat.OpenXml.ExtendedProperties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPILayer.Controllers
{
    /// <summary>
    /// Handles user authentication, registration for different roles, and session management.
    /// </summary>
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

        /// <summary>
        /// Registers a new user with the 'Manager' role.
        /// </summary>
        /// <param name="dto">The manager registration details.</param>
        /// <returns>A success message or an error if the username exists.</returns>
        /// <response code="200">Manager created successfully.</response>
        /// <response code="400">Invalid input or username already exists.</response>
        /// <response code="500">Internal server error.</response>
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

        /// <summary>
        /// Registers a new user with the 'Farmer' role and assigns them to a specific greenhouse.
        /// </summary>
        /// <param name="dto">The farmer registration details.</param>
        /// <param name="GreehouseId">The unique identifier of the greenhouse assigned to the farmer.</param>
        /// <returns>A success message.</returns>
        /// <response code="200">Farmer created successfully.</response>
        /// <response code="401">Unauthorized if the requester is not a Manager.</response>
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

        /// <summary>
        /// Authenticates a user and generates a JWT Bearer token.
        /// </summary>
        /// <param name="dto">User login credentials (username and password).</param>
        /// <returns>A JSON object containing the JWT Token and User details.</returns>
        /// <response code="200">Returns the token and user data.</response>
        /// <response code="401">Invalid credentials provided.</response>
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
                return new JsonResult(new { Token = token, User = user });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Logs out the user and invalidates the session or token on the server side.
        /// </summary>
        /// <param name="dto">Details required to identify the session to close.</param>
        /// <response code="200">Logged out successfully.</response>
        /// <response code="401">User is not authenticated or user ID is invalid.</response>
        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Extracts the user ID from the claims in the JWT token
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Guid.TryParse(userIdString, out Guid userId);

                if (userId == Guid.Empty)
                {
                    return Unauthorized("Invalid user ID.");
                }

                await _userService.LogoutAsync(dto);
                return Ok("Logged out successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }
    }
}