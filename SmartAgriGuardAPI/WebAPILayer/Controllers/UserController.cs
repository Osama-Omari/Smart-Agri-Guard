using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPILayer.Controllers
{
    /// <summary>
    /// Provides endpoints for managing user profiles, including account deletion 
    /// for specific roles and updating personal information or passwords.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Deletes a farmer account. Only accessible by a Manager.
        /// </summary>
        /// <remarks>
        /// The system validates that the deleting manager has the authority 
        /// to remove the specified farmer.
        /// </remarks>
        /// <param name="farmerId">The unique ID of the farmer to be deleted.</param>
        /// <response code="200">Farmer deleted successfully.</response>
        /// <response code="401">Unauthorized if the manager ID is invalid or missing.</response>
        [HttpDelete("DeleteFarmer/{farmerId}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteFarmer(Guid farmerId)
        {
            try
            {
                // Extract Manager's ID from JWT claims for authorization check in the service layer
                var managerIdstring = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Guid.TryParse(managerIdstring, out Guid managerId);

                if (managerId == Guid.Empty)
                {
                    return Unauthorized("Invalid manager ID.");
                }

                await _userService.DeleteFarmerAsync(farmerId, managerId);
                return Ok("Farmer deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Retrieves a list of all managers in the system.
        /// Restricted to Admin users.
        /// </summary>
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

        /// <summary>
        /// Deletes a manager account. Only accessible by an Admin.
        /// </summary>
        /// <param name="managerId">The unique ID of the manager to be removed.</param>
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

        /// <summary>
        /// Updates personal information for the currently authenticated user.
        /// </summary>
        /// <param name="dto">The updated user details.</param>
        /// <returns>The updated user profile data.</returns>
        [HttpPut("ChangeUserInfo")]
        [Authorize]
        public async Task<IActionResult> ChangeFarmerInfo([FromBody] UpdateUserDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                // Extracts the user ID from the token to ensure users can only update their own profile
                var UserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Guid.TryParse(UserIdString, out Guid userId);

                if (userId == Guid.Empty)
                {
                    return Unauthorized("Invalid user ID.");
                }

                var user = await _userService.UpdateUserAsync(dto, userId);
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

        /// <summary>
        /// Updates the password for the currently authenticated user.
        /// </summary>
        /// <param name="dto">Contains old and new password details.</param>
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