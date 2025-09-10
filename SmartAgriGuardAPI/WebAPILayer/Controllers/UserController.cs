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
    }
}
