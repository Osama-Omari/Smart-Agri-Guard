using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces;
using DataAccessLayer.Models;
using InfrastructureLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebAPILayer.RequestDTO;

namespace WebAPILayer.Controllers
{
    /// <summary>
    /// Controller for managing greenhouse entities, including file uploads for images, 
    /// administrator-level CRUD operations, and manager assignments.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class GreenhouseController : ControllerBase
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly IGreenhouseService _greenhouseService;

        public GreenhouseController(IFileStorageService fileStorageService, IGreenhouseService greenhouseService)
        {
            _fileStorageService = fileStorageService;
            _greenhouseService = greenhouseService;
        }

        /// <summary>
        /// Retrieves a specific greenhouse by its unique identifier.
        /// </summary>
        /// <param name="Id">The GUID of the greenhouse.</param>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetGreenhouseById(Guid Id)
        {
            try
            {
                var greenhouse = await _greenhouseService.GetGreenhouseById(Id);
                if (greenhouse == null)
                    return NotFound();
                return Ok(greenhouse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Returns a list of all greenhouses in the system.
        /// </summary>
        [HttpGet("All")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllGreenhouses()
        {
            try
            {
                var greenhouses = await _greenhouseService.GetAllGreenhouses();
                if (greenhouses == null)
                    return NotFound();
                return Ok(greenhouses);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates a new greenhouse with an optional image upload.
        /// </summary>
        /// <param name="dto">The multipart/form-data containing Name, Location, and Image file.</param>
        /// <response code="200">Greenhouse created successfully.</response>
        /// <response code="400">Model validation failed.</response>
        [HttpPost("Add")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateGreenhouse([FromForm] CreateGreenhouseRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                string? imagePath = null;
                // Handle file upload if an image is provided
                if (dto.Image is { Length: > 0 })
                {
                    await using var stream = dto.Image.OpenReadStream();
                    var filedata = new FileDataDTO
                    {
                        Content = stream,
                        FileName = dto.Image.FileName
                    };
                    // Saves the file to the 'greenhouses' directory
                    imagePath = await _fileStorageService.SaveFileAsync(filedata, "greenhouses");
                }

                var greenhouseRegisterDTO = new GreenhouseRegisterDTO
                {
                    Location = dto.Location,
                    Name = dto.Name,
                    ImagePath = imagePath
                };

                await _greenhouseService.AddGreenhouse(greenhouseRegisterDTO);
                return Ok("The greenhhouse registered successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves greenhouses that currently do not have an assigned manager.
        /// </summary>
        [HttpGet("Get-Without-Manager")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetGreenhousesWithoutManager()
        {
            try
            {
                var greenhouses = await _greenhouseService.GetGreenhousesWithoutManagerAsync();
                if (greenhouses == null)
                    return NotFound("There is no greenhouses without managers");
                return Ok(greenhouses);
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
        /// Assigns a manager to a specific greenhouse.
        /// </summary>
        /// <param name="ManagerId">The GUID of the user with Manager role.</param>
        /// <param name="GreenhouseId">The GUID of the target greenhouse.</param>
        [HttpPatch("Assign-Manager/{ManagerId}/{GreenhouseId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignManagerToGreenhouse(Guid ManagerId, Guid GreenhouseId)
        {
            try
            {
                await _greenhouseService.AssignManagerAsync(ManagerId, GreenhouseId);
                return Ok("Manager assigned to greenhouse successfully.");
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
        /// Removes the manager assignment from a greenhouse.
        /// </summary>
        [HttpPatch("UnAssign-Manager/{GreenhouseId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UnAssignManager([FromRoute] Guid GreenhouseId)
        {
            try
            {
                await _greenhouseService.UnAssignManagerAsync(GreenhouseId);
                return Ok("The unassignment process succeed");
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
        /// Permanently deletes a greenhouse from the system.
        /// </summary>
        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteGreenhouse(Guid id)
        {
            try
            {
                await _greenhouseService.DeleteGreenhouseAsync(id);
                return Ok("Greenhouse deleted successfully.");
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
        /// Updates greenhouse details and/or replaces the existing image.
        /// </summary>
        /// <param name="id">The GUID of the greenhouse to update.</param>
        /// <param name="dto">The updated data and optional new image file.</param>
        [HttpPut("Update/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateGreenhouse([FromRoute] Guid id, [FromForm] UpdateGreenhouseRequestDTO dto)
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
                    imagePath = await _fileStorageService.SaveFileAsync(filedata, "greenhouses");
                }

                var greenhouseUpdateDTO = new GreenhouseUpdateDTO
                {
                    Location = dto.Location,
                    Name = dto.Name,
                    ImagePath = imagePath
                };
                await _greenhouseService.UpdateGreenhouseAsync(id, greenhouseUpdateDTO);
                return Ok("Greenhouse updated successfully.");
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
        /// Gets all greenhouses assigned to the currently logged-in Manager.
        /// </summary>
        [HttpGet("Assigned-Greenhouses")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetAssignedGreenhouses()
        {
            try
            {
                var UserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Guid.TryParse(UserIdString, out Guid managerId);
                if (managerId == Guid.Empty)
                    return Unauthorized("Invalid manager ID.");

                var greenhouses = await _greenhouseService.GetGreenhousesByManagerIdAsync(managerId);
                if (greenhouses == null)
                    return NotFound("There is no greenhouses assigned to this manager");
                return Ok(greenhouses);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Returns all farmers associated with a specific greenhouse.
        /// </summary>
        [HttpGet("Farmers/{Id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetGreenhouseFarmers(Guid Id)
        {
            try
            {
                var farmers = await _greenhouseService.GetFarmersByGreenhouseIdAsync(Id);
                if (farmers == null)
                    return NotFound("No farmers found for this greenhouse.");
                return Ok(farmers);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves the manager details for a specific greenhouse.
        /// </summary>
        [HttpGet("Greenhouse-Manager/{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetGreenhouseManager(Guid Id)
        {
            try
            {
                var manager = await _greenhouseService.GetManagerByGreenhouseIdAsync(Id);
                if (manager == null)
                    return NotFound("No manager assigned to this greenhouse.");
                return Ok(manager);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }
    }
}