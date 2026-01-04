using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPILayer.Filters;

namespace WebAPILayer.Controllers
{
    /// <summary>
    /// Manages the ingestion and retrieval of environmental sensor data (Temperature, Humidity, etc.).
    /// Handles both real-time data and historical archives for trend analysis.
    /// </summary>
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

        /// <summary>
        /// Receives and stores new sensor readings for a specific plant.
        /// </summary>
        /// <remarks>
        /// This endpoint uses **ApiKeyAuth** instead of JWT, typically for IoT devices 
        /// that don't support complex authentication flows.
        /// </remarks>
        /// <param name="plantId">The GUID of the plant the sensor is attached to.</param>
        /// <param name="dto">The telemetry data (e.g., soil moisture, light levels).</param>
        /// <response code="200">Data ingested successfully.</response>
        /// <response code="401">Invalid or missing API Key.</response>
        /// <response code="404">Plant ID not found.</response>
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

        /// <summary>
        /// Gets the most recent sensor reading for a specific plant.
        /// </summary>
        /// <param name="plantId">The GUID of the plant.</param>
        /// <returns>The latest telemetry point localized to the user's timezone.</returns>
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

        /// <summary>
        /// Retrieves statistical trends from active/recent sensor data.
        /// Accessible by Managers and Farmers to monitor plant health over short periods.
        /// </summary>
        /// <param name="dto">Filters including StartDate, EndDate, and PlantId.</param>
        [HttpGet("Trend")]
        [Authorize(Roles = "Manager,Farmer")]
        public async Task<IActionResult> GetTrends([FromBody] SensorTrendRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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

        /// <summary>
        /// Retrieves statistical trends from archived (long-term) sensor data.
        /// Optimized for high-volume historical analysis, restricted to Managers.
        /// </summary>
        /// <param name="dto">Archive query parameters.</param>
        [HttpGet("Archive-Trend")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetArchiveTrends([FromBody] SensorTrendArchiveRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userTimeZoneId = User.Claims.FirstOrDefault(c => c.Type == "timezone")?.Value!;

                var trends = await _sensorDataArchiveService.GetSensorArchiveTrendsAsync(dto, userTimeZoneId);

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