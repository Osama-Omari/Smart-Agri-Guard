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
        private readonly IPlantScheduleService _plantScheduleService;
        public PlantController(IFileStorageService fileStorageService, IPlantService plantService,IPlantScheduleService plantScheduleService)
        {
            _fileStorageService = fileStorageService;
            _plantService = plantService;
            _plantScheduleService = plantScheduleService;

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

        /// <summary>
        /// Creates a new recurring care schedule for a plant and registers it in Hangfire.
        /// </summary>
        /// <param name="PlantId">The unique identifier of the plant.</param>
        /// <param name="dto">The schedule details (Frequency, Time, Task Type).</param>
        /// <returns>A confirmation message if the schedule was created and job registered.</returns>
        /// <response code="200">Successfully created in DB and scheduled in Hangfire.</response>
        /// <response code="404">Plant not found.</response>
        [HttpPost("Add-Plant-Schedule/{PlantId}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> AddPlantSchedule([FromRoute] Guid PlantId, [FromBody] CreateScheduleDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var userTimeZoneId = User.Claims.FirstOrDefault(c => c.Type == "timezone")?.Value!;
                await _plantScheduleService.AddPlantScheduleAsync(PlantId, dto, userTimeZoneId);
                return Ok("The schedule has been added successfully");
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
        /// Updates an existing plant schedule and synchronizes the change with the Hangfire background worker.
        /// </summary>
        /// <param name="ScheduleId">The unique identifier of the schedule to modify.</param>
        /// <param name="dto">The updated schedule data.</param>
        /// <returns>A confirmation that the database and the background job were updated.</returns>
        /// <response code="200">Database updated and Hangfire job rescheduled.</response>
        /// <response code="404">Schedule not found.</response>
        [HttpPut("Update-Plant-Schedule/{ScheduleId}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdatePlantSchedule([FromRoute] Guid ScheduleId, [FromBody] CreateScheduleDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var userTimeZoneId = User.Claims.FirstOrDefault(c => c.Type == "timezone")?.Value!;
                await _plantScheduleService.UpdatePlantScheduleAsync(ScheduleId, dto, userTimeZoneId);
                return Ok("The schedule has been updated successfully");
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
        /// Toggles the active status of a schedule. 
        /// Disabling it removes the job from Hangfire while keeping the configuration in the database.
        /// </summary>
        /// <param name="ScheduleId">The unique identifier of the schedule.</param>
        /// <returns>The new toggle state (Active/Inactive).</returns>
        /// <response code="200">Status flipped and background job added/removed accordingly.</response>
        [HttpPatch("Toggle-Plant-Schedule/{ScheduleId}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> TogglePlantSchedule([FromRoute] Guid ScheduleId)
        {
            try
            {
                await _plantScheduleService.TogglePlantScheduleAsync(ScheduleId);
                return Ok("The schedule status has been toggled successfully");
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
        /// Permanently deletes a plant schedule from the database and stops all associated background tasks.
        /// </summary>
        /// <param name="ScheduleId">The unique identifier of the schedule to be removed.</param>
        /// <returns>A success message indicating the schedule is fully purged.</returns>
        /// <response code="200">Schedule record deleted and Hangfire job permanently removed.</response>
        [HttpDelete("Delete-Plant-Schedule/{ScheduleId}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeletePlantSchedule([FromRoute] Guid ScheduleId)
        {
            try
            {
                await _plantScheduleService.DeletePlantScheduleAsync(ScheduleId);
                return Ok("The schedule has been deleted successfully");
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

        [HttpGet("Get-Plant-Schedules/{PlantId}")]
        public async Task<IActionResult> GetPlantSchedules([FromRoute] Guid PlantId)
        {
            try
            {
                var userTimeZoneId = User.Claims.FirstOrDefault(c => c.Type == "timezone")?.Value!;
                var schedules = await _plantScheduleService.GetPlantSchedulesAsync(PlantId,userTimeZoneId);
                if(schedules == null || !schedules.Any())
                    return NotFound("No schedules found for the specified plant.");
                return Ok(schedules);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}