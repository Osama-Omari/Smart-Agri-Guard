using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces;
using DataAccessLayer.Models;
using InfrastructureLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPILayer.RequestDTO;

namespace WebAPILayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GreenhouseController : ControllerBase
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly IGreenhouseService _greenhouseService;
        private readonly IPlantService _plantService;
        private readonly IUserService _userService;
        private readonly IFarmerPlantService _farmerPlantService;
        public GreenhouseController(IFileStorageService fileStorageService, IGreenhouseService greenhouseService, IPlantService plantService, IUserService userService, IFarmerPlantService farmerPlantService)
        {
            _fileStorageService = fileStorageService;
            _greenhouseService = greenhouseService;
            _plantService = plantService;
            _userService = userService;
            _farmerPlantService = farmerPlantService;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager")]
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

        [HttpGet("All")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllGreenhouses()
        {
            try
            {
                var greenhouses = await _greenhouseService.GetAllGreenhouses();
                if(greenhouses == null)
                    return NotFound();
                return Ok(greenhouses);
             
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");

            }
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("Add")]
        public async Task<IActionResult> CreateGreenhouse([FromForm] CreateGreenhouseRequestDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
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
                var greenhouseRegisterDTO = new GreenhouseRegisterDTO
                {
                    Location = dto.Location,
                    Name = dto.Name,
                    ImagePath = imagePath
                };
                var result = await _greenhouseService.AddGreenhouse(greenhouseRegisterDTO);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }

        }

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

        [HttpPatch("UnAssign-Manager/{GreenhouseId}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> UnAssignManager([FromRoute] Guid GreenhouseId)
        {
            try
            {
                await _greenhouseService.UnAssignManagerAsync(GreenhouseId);
                return Ok("The unassignment process succeed");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        

        [HttpPut("Update-FarmerPlant-Assignment/{farmerId}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdateFarmerAssignment([FromRoute] Guid farmerId,[FromBody] FarmerPlantDTO dto)
        {
            if(!ModelState.IsValid) 
                return BadRequest(ModelState);
            try
            {
                await _farmerPlantService.UpdateFarmerPlantAssignment(farmerId,dto);
                return Ok("The assignment process happend successfully");
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
    
