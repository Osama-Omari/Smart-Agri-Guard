using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPILayer.Controllers
{
    /// <summary>
    /// Provides administrative endpoints to manage plant types (categories).
    /// Access is restricted to users with the 'Admin' role.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class PlantTypeController : ControllerBase
    {
        private readonly IPlantTypeService _plantTypeService;

        public PlantTypeController(IPlantTypeService plantTypeService)
        {
            _plantTypeService = plantTypeService;
        }

        /// <summary>
        /// Registers a new plant type in the system.
        /// </summary>
        /// <param name="dto">The plant type registration details (e.g., Species name, care requirements).</param>
        /// <returns>A confirmation message.</returns>
        /// <response code="200">Plant type created successfully.</response>
        /// <response code="400">If the data provided is invalid.</response>
        [HttpPost("Add")]
        public async Task<IActionResult> AddPlantType(PlantTypeRegisterDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                await _plantTypeService.AddPlantType(dto);
                return Ok("The plantType has been registered successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves all plant types available in the database.
        /// </summary>
        /// <returns>A list of plant types.</returns>
        /// <response code="200">Returns the list of plant types.</response>
        /// <response code="404">If no plant types exist.</response>
        [HttpGet("All")]
        public async Task<IActionResult> GetAllPlantTypes()
        {
            try
            {
                var plantTypes = await _plantTypeService.GetAllPlantTypes();
                if (plantTypes == null || !plantTypes.Any())
                    return NotFound("No plant types found.");
                return Ok(plantTypes);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the details of an existing plant type.
        /// </summary>
        /// <param name="Id">The unique identifier of the plant type.</param>
        /// <param name="dto">The updated information.</param>
        /// <response code="200">Update successful.</response>
        /// <response code="404">If the specified ID does not exist.</response>
        [HttpPut("Update/{Id}")]
        public async Task<IActionResult> UpdatePlantType([FromRoute] Guid Id, [FromBody] PlantTypeUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                await _plantTypeService.UpdatePlantType(Id, dto);
                return Ok("The plant type has been updated successfully");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves details for a specific plant type by ID.
        /// </summary>
        /// <param name="Id">The GUID of the plant type.</param>
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetPlantTypeById([FromRoute] Guid Id)
        {
            try
            {
                var plantType = await _plantTypeService.GetPlantTypeById(Id);
                if (plantType == null)
                    return NotFound("Plant type not found.");
                return Ok(plantType);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a plant type from the system.
        /// </summary>
        /// <param name="Id">The GUID of the plant type to remove.</param>
        /// <response code="200">Deletion successful.</response>
        /// <response code="404">If the plant type was not found.</response>
        [HttpDelete("Delete/{Id}")]
        public async Task<IActionResult> DeletePlantType([FromRoute] Guid Id)
        {
            try
            {
                await _plantTypeService.DeletePlantType(Id);
                return Ok("The plant type has been deleted successfully");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}