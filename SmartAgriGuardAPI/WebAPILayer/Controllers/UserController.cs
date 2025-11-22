using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace WebAPILayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpDelete("DleteFarmer/{farmerId}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteFarmer(Guid farmerId)
        {
            try
            {
                var managerIdstring = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Guid.TryParse(managerIdstring, out Guid managerId);
                if (managerId == Guid.Empty)
                {
                    return Unauthorized("Invalid manager ID.");
                }
                var manager = await _userService.GetManager(managerId);
                var farmer = await _userService.GetFarmer(farmerId);
                if(manager.GreenhousesIds.Contains(farmer.GreenhouseId) == false)
                {
                    return Forbid("You are not authorized to delete this farmer.");
                }
                await _userService.DeleteFarmerAsync(farmerId);
                return Ok("Farmer deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("AllManagers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllManagers()
        {
            try
            {
                var managers = await _userService.GetAllManagersAsync();
                return Ok(managers);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("DeleteManager/{managerId}")]
        [Authorize(Roles = "Admin")]    
        public async Task<IActionResult> DeleteManager(Guid managerId)
        {
            try
            {
                await _userService.DeleteManagerAsync(managerId);
                return Ok("Manager deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

        [HttpPut("ChangeUserInfo")]
        [Authorize]
        public async Task<IActionResult> ChangeFarmerInfo([FromBody] UpdateUserDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var UserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Guid.TryParse(UserIdString, out Guid userId);
                if (userId == Guid.Empty)
                {
                    return Unauthorized("Invalid user ID.");
                }

                var user = await _userService.UpdateUserAsync(dto,userId);
                return Ok(user);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("Change-Password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var UserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Guid.TryParse(UserIdString, out Guid userId);
                if (userId == Guid.Empty)
                {
                    return Unauthorized("Invalid user ID.");
                }
                await _userService.ChangePasswordAsync(dto, userId);
                return Ok("Password changed successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
