using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPILayer.Filters;

namespace WebAPILayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SensorDataController : ControllerBase
    {

        private readonly ISensorDataService _sensorDataService;
        public SensorDataController(ISensorDataService sensorDataService)
        {
            _sensorDataService = sensorDataService;
        }

        [HttpPost("Add/{plantId}")]
        [ApiKeyAuth]
        public async Task<IActionResult> AddSensorData(Guid plantId, [FromBody] SensorDataRegisterDTO dto)
        {
            try
            {
                await _sensorDataService.AddSensorData(plantId, dto);
                return Ok("Sensor data added successfully.");
            }
            catch (KeyNotFoundException knfEx)
            {
                return NotFound(knfEx.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("Latest/{plantId}")]
        [Authorize]
        public async Task<IActionResult> GetLatestSensorData(Guid plantId)
        {
            try
            {
                var sensorData = await _sensorDataService.GetLatestSensorData(plantId);
                if (sensorData == null)
                    return NotFound("No sensor data found for the specified plant.");
                return Ok(sensorData);
            }
            catch (KeyNotFoundException knfEx)
            {
                return NotFound(knfEx.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

    }
}
