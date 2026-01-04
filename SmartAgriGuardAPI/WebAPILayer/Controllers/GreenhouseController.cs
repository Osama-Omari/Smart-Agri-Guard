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



        [HttpPost("Add")]
        [Authorize(Roles = "Admin")]

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
                return Ok("The greenhhouse registered successfully");

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }

        }

        [HttpGet("Get-Without-Manager")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetGreenhousesWithoutManager()
        {
            try
            {
                var greenhouses = await _greenhouseService.GetGreenhousesWithoutManagerAsync();
                if(greenhouses ==  null)
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
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


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

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> UpdateGreenhouse([FromRoute] Guid id, [FromForm] UpdateGreenhouseRequestDTO dto)
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

        [HttpGet("Assigned-Greenhouses")]
        [Authorize(Roles = "Manager")]

        public async Task<IActionResult> GetAssignedGreenhouses()
        {
            try
            {
                var UserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Guid.TryParse(UserIdString, out Guid managerId);
                if (managerId == Guid.Empty)
                {
                    return Unauthorized("Invalid manager ID.");
                }
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




        [HttpGet("Farmers/{Id}")]
        [Authorize(Roles = "Manager")]

        public async Task<IActionResult> GetGreenhouseFarmers(Guid Id)
        {
            try
            {
                var farmers = await _greenhouseService.GetFarmersByGreenhouseIdAsync(Id);
                if(farmers  == null)
                    return NotFound("No farmers found for this greenhouse.");
                return Ok(farmers);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }


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
    
