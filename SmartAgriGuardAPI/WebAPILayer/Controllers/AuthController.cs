using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
                if(await _userService.isUserNameExists(dto.UserName))
                {
                    return BadRequest("UserName Already Exist");
                }

                var user = await _userService.RegisterManager(dto);
                return Ok(user);


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
                return Ok(user);


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
    }



}

