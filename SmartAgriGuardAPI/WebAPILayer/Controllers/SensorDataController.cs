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
        private readonly ISensorDataArchiveService _sensorDataArchiveService;
        public SensorDataController(ISensorDataService sensorDataService, ISensorDataArchiveService sensorDataArchiveService)
        {
            _sensorDataService = sensorDataService;
            _sensorDataArchiveService = sensorDataArchiveService;
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
                var userTimeZoneId = User.Claims.FirstOrDefault(c => c.Type == "timezone")?.Value!;
                var sensorData = await _sensorDataService.GetLatestSensorData(plantId, userTimeZoneId);
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

        [HttpGet("Trend")]
        [Authorize(Roles = "Manager,Farmer")]
        public async Task<IActionResult> GetTrends([FromBody] SensorTrendRequestDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var userTimeZoneId = User.Claims.FirstOrDefault(c => c.Type == "timezone")?.Value!;
                var trends = await _sensorDataService.GetSensorTrendsAsync(dto, userTimeZoneId);
                if (trends == null)
                    return NotFound("No sensor trend data found for the specified criteria.");
                return Ok(trends);
            }
            catch (KeyNotFoundException Ex)
            {
                return NotFound(Ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("Archive-Trend")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetArchiveTrends([FromBody] SensorTrendArchiveRequestDTO dto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var userTimeZoneId =  User.Claims.FirstOrDefault(c => c.Type == "timezone")?.Value!;

                var trends = await _sensorDataArchiveService.GetSensorArchiveTrendsAsync(dto,userTimeZoneId);
                if (trends == null)
                    return NotFound("No sensor trend data found for the specified criteria.");
                return Ok(trends);
            }
            catch (KeyNotFoundException Ex)
            {
                return NotFound(Ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }

        }

    }
}
