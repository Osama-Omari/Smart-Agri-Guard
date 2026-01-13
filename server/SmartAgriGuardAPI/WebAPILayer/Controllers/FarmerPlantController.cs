using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPILayer.Controllers
{
    /// <summary>
    /// Manages the relationship and assignments between Farmers and Plants.
    /// Provides functionality for managers to assign work and farmers to view their tasks.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class FarmerPlantController : ControllerBase
    {
        private readonly IFarmerPlantService _farmerPlantService;

        public FarmerPlantController(IFarmerPlantService farmerPlantService)
        {
            _farmerPlantService = farmerPlantService;
        }

        /// <summary>
        /// Retrieves all plants assigned to the currently authenticated Farmer.
        /// </summary>
        /// <remarks>
        /// This method extracts the Farmer's ID and TimeZone from the JWT claims to ensure 
        /// the plant data is localized to the user's specific region.
        /// </remarks>
        /// <returns>A list of plants assigned to the farmer.</returns>
        /// <response code="200">Returns the list of assigned plants.</response>
        /// <response code="401">If the user ID cannot be found in the token.</response>
        /// <response code="404">If no plants are found for the specific farmer.</response>
        [HttpGet("Get-Assigned-Plants")]
        [Authorize(Roles = "Farmer")]
        public async Task<IActionResult> GetAssignedPlantsForFarmer()
        {
            try
            {
                // Extract User ID from the JWT NameIdentifier claim
                var FarmerIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Guid.TryParse(FarmerIdString, out Guid farmerId);

                if (farmerId == Guid.Empty)
                {
                    return Unauthorized("Invalid user ID.");
                }

                // Retrieve the custom 'timezone' claim to handle localized scheduling/reporting
                var userTimeZoneId = User.Claims.FirstOrDefault(c => c.Type == "timezone")?.Value!;

                var assignedPlants = await _farmerPlantService.GetAssignedPlantsForFarmer(farmerId, userTimeZoneId);
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

        /// <summary>
        /// Removes a farmer's assignment from a specific plant.
        /// </summary>
        /// <param name="plantId">The unique ID of the plant.</param>
        /// <param name="farmerId">The unique ID of the farmer to be unassigned.</param>
        /// <returns>A success message confirming the removal.</returns>
        /// <response code="200">The unassignment was successful.</response>
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

        /// <summary>
        /// Assigns one or more farmers to a specific plant.
        /// </summary>
        /// <param name="plantId">The ID of the plant to receive assignments.</param>
        /// <param name="farmers">A DTO containing the list of Farmer IDs to assign.</param>
        /// <returns>A success message.</returns>
        /// <response code="200">The farmers were successfully assigned to the plant.</response>
        [HttpPost("Assign-Farmer/{plantId}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> AssignFarmer([FromRoute] Guid plantId, [FromBody] AssignFarmerDTO farmers)
        {
            try
            {
                // Logic assumes the service handles validation of plant existence and farmer roles
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