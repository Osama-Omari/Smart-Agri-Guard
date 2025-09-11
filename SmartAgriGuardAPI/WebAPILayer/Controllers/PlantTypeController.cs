using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPILayer.Controllers
{
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
        [HttpPost("Add")]
        public async Task<IActionResult> AddPlantType(PlantTypeRegisterDTO dto)
        {
            if(!ModelState.IsValid)
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

    }
}
