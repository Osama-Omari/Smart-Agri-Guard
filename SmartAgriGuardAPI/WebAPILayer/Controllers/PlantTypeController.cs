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
    }
}
