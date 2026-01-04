using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPILayer.RequestDTO;

namespace WebAPILayer.Controllers
{
    /// <summary>
    /// Manages plant-specific operations including registration, retrieval of plant metrics, 
    /// and greenhouse plant management.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PlantController : ControllerBase
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly IPlantService _plantService;

        public PlantController(IFileStorageService fileStorageService, IPlantService plantService)
        {
            _fileStorageService = fileStorageService;
            _plantService = plantService;
        }

        /// <summary>
        /// Registers a new plant and assigns it to a specific greenhouse.
        /// </summary>
        /// <param name="GreenhouseId">The unique ID of the target greenhouse.</param>
        /// <param name="dto">The plant details including an optional image file.</param>
        /// <returns>A success message or error details.</returns>
        /// <response code="200">Plant added successfully.</response>
        /// <response code="404">Greenhouse ID not found.</response>
        [HttpPost("Add/{GreenhouseId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddPlant([FromRoute] Guid GreenhouseId, [FromForm] CreatePlantRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                string? imagePath = null;
                // Process image upload to storage service if provided
                if (dto.Image is { Length: > 0 })
                {
                    await using var stream = dto.Image.OpenReadStream();
                    var filedata = new FileDataDTO
                    {
                        Content = stream,
                        FileName = dto.Image.FileName
                    };
                    imagePath = await _fileStorageService.SaveFileAsync(filedata, "plants");
                }

                var plantRegisterDto = new PlantRegisterDTO
                {
                    ImagePath = imagePath,
                    Location = dto.Location,
                    Name = dto.PlantName,
                    PlantTypeId = dto.PlantTypeId,
                };
                await _plantService.AddPlantToGreenhouse(GreenhouseId, plantRegisterDto);
                return Ok("The plant has been added successfully");
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
        /// Retrieves detailed information for a single plant.
        /// </summary>
        [HttpGet("{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPlantById([FromRoute] Guid id)
        {
            try
            {
                var plant = await _plantService.GetPlantById(id);
                if (plant == null)
                    return NotFound();
                return Ok(plant);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Lists all plants belonging to a specific greenhouse.
        /// </summary>
        [HttpGet("All-Greenhouse-Plants/{GreenhouseId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAllGreenhousePlants([FromRoute] Guid GreenhouseId)
        {
            try
            {
                var plants = await _plantService.GetAllGreenhousePlants(GreenhouseId);
                if (plants == null)
                    return NotFound();
                return Ok(plants);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves plants along with the farmers assigned to them within a greenhouse.
        /// </summary>
        [HttpGet("Plants-With-Assigned-Farmers/{GreenhouseId}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetPlantsWithAssignedFarmers(Guid GreenhouseId)
        {
            try
            {
                var plants = await _plantService.getPlantsWithAssignedFarmers(GreenhouseId);
                return Ok(plants);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves all plants in a greenhouse along with their health/growth metrics.
        /// </summary>
        /// <remarks>
        /// Uses the 'timezone' claim from the user's token to localize metric timestamps.
        /// </remarks>
        [HttpGet("All-Greenhouse-Plants-With-Metrics/{GreenhouseId}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetAllGreenhousePlantsWithMetrics([FromRoute] Guid GreenhouseId)
        {
            try
            {
                var userTimeZoneId = User.Claims.FirstOrDefault(c => c.Type == "timezone")?.Value!;
                var plants = await _plantService.GetAllGreenhousePlantsWithMetrics(GreenhouseId, userTimeZoneId);
                if (plants == null)
                    return NotFound();
                return Ok(plants);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a plant from the system. 
        /// Prevents deletion if a farmer is still assigned to the plant.
        /// </summary>
        /// <response code="400">If the plant still has active farmer assignments.</response>
        [HttpDelete("Delete/{plantId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePlant([FromRoute] Guid plantId)
        {
            try
            {
                // Safety check: Cannot delete a plant if work is still assigned
                if (await _plantService.isPlnatAssignmentExists(plantId))
                    return BadRequest("The plant has farmer assigned to it , remove the assignment first");

                await _plantService.DeletePlantAsync(plantId);
                return Ok("The plant has successfully deleted");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the metadata and/or the image of an existing plant.
        /// </summary>
        [HttpPut("Update/{plantId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePlant([FromRoute] Guid plantId, [FromForm] UpdatePlantRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                string? imagePath = null;
                if (dto.Image is { Length: > 0 })
                {
                    await using var stream = dto.Image.OpenReadStream();
                    var filedata = new FileDataDTO
                    {
                        Content = stream,
                        FileName = dto.Image.FileName
                    };
                    imagePath = await _fileStorageService.SaveFileAsync(filedata, "plants");
                }

                var plantUpdateDto = new PlantUpdateDTO
                {
                    ImagePath = imagePath,
                    Location = dto.Location,
                    Name = dto.PlantName,
                };
                await _plantService.UpdatePlantAsync(plantId, plantUpdateDto);
                return Ok("The plant has been updated successfully");
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