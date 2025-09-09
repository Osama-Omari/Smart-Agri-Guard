using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("{farmerId}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetAssignedPlantsForFarmer([FromRoute] Guid farmerId)
        {
            try
            {
                var assignedPlants = await _farmerPlantService.GetAssignedPlantsForFarmer(farmerId);
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

        [HttpPut("Update/{farmerId}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdateFarmerPlantAssignment([FromRoute] Guid farmerId, [FromBody] FarmerPlantDTO farmerPlantDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                await _farmerPlantService.UpdateFarmerPlantAssignment(farmerId, farmerPlantDTO);
                return Ok("Farmer plant assignments updated successfully.");
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
    }
}
