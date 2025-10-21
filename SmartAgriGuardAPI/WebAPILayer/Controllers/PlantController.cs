using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPILayer.RequestDTO;

namespace WebAPILayer.Controllers
{
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

        [HttpPost("Add/{GreenhouseId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddPlant([FromRoute] Guid GreenhouseId, [FromForm] CreatePlantRequestDTO dto)
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
                var plantRegisterDto = new PlantRegisterDTO
                {
                    ImagePath = imagePath,
                    Location = dto.Location,
                    Name = dto.Name,
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

        [HttpGet("All-Greenhouse-Plants/{GreenhouseId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAllGreenhousePlants([FromRoute] Guid GreenhouseId)
        {
            try
            {
                var plants = await _plantService.GetAllGreenhousePlants(GreenhouseId);
                if(plants == null)
                    return NotFound();
                return Ok(plants);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");

            }
        }

        [HttpDelete("Delete/{plantId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePlant([FromRoute] Guid plantId)
        {
            try
            {
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

        [HttpPut("Update/{plantId}")]
        [Authorize (Roles = "Admin")]
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
                    Name = dto.Name,
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
