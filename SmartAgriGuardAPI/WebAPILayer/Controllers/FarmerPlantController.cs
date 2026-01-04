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
    public class FarmerPlantController : ControllerBase
    {
        private readonly IFarmerPlantService _farmerPlantService;
        public FarmerPlantController(IFarmerPlantService farmerPlantService)
        {
            _farmerPlantService = farmerPlantService;
        }

        [HttpGet("Get-Assigned-Plants")]
        [Authorize(Roles = "Farmer")]
        public async Task<IActionResult> GetAssignedPlantsForFarmer()
        {
            try
            {
                var FarmerIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Guid.TryParse(FarmerIdString, out Guid farmerId);
                if (farmerId == Guid.Empty)
                {
                    return Unauthorized("Invalid user ID.");
                }
                var userTimeZoneId = User.Claims.FirstOrDefault(c => c.Type == "timezone")?.Value!;
                var assignedPlants = await _farmerPlantService.GetAssignedPlantsForFarmer(farmerId,userTimeZoneId);
                return Ok(assignedPlants);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("UnAssign-Farmer/{plantId}/{farmerId}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UnAssignFarmer(Guid plantId, Guid farmerId)
        {
            try
            {
                await _farmerPlantService.UnAssignFarmerAsync(plantId, farmerId);
                return Ok("the farmer has been unassigned successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("Assign-Farmer/{plantId}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> AssignFarmer([FromRoute]Guid plantId , [FromBody] AssignFarmerDTO farmers)
        {
            try
            {
                await _farmerPlantService.AssignFarmers(plantId, farmers);
                return Ok("the farmers have been assigned successfully");

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

    }
}
