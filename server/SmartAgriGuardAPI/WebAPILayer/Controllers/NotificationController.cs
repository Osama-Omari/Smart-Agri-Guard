using ApplicationLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPILayer.Controllers
{
    /// <summary>
    /// Handles system and manual notifications for both individual plants and entire greenhouses.
    /// Provides endpoints for triggering alerts, retrieving notification history, and marking alerts as read.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IPlantService _plantService;
        private readonly IGreenhouseService _greenhouseService;

        public NotificationController(INotificationService notificationService, IPlantService plantService, IGreenhouseService greenhouseService)
        {
            _notificationService = notificationService;
            _plantService = plantService;
            _greenhouseService = greenhouseService;
        }

        /// <summary>
        /// Manually triggers a watering notification for a specific plant.
        /// </summary>
        /// <param name="plantId">The unique ID of the plant needing water.</param>
        /// <response code="200">Notification sent successfully.</response>
        /// <response code="403">Forbidden if the user is not an Admin.</response>
        [Authorize(Roles = "Admin")]
        [HttpPost("plant/{plantId}/watering")]
        public async Task<IActionResult> NotifyPlantNeedsWatering(Guid plantId)
        {
            try
            {
                await _notificationService.NotifyPlantNeedsWatering(plantId);
                return Ok(new { Message = "Notification sent for plant needing watering." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while sending the notification.", Details = ex.Message });
            }
        }

        /// <summary>
        /// Manually triggers a nutrient/fertilizer notification for a specific plant.
        /// </summary>
        /// <param name="plantId">The unique ID of the plant needing nutrients.</param>
        [Authorize(Roles = "Admin")]
        [HttpPost("plant/{plantId}/nutrients")]
        public async Task<IActionResult> NotifyPlantNeedsNutrients(Guid plantId)
        {
            try
            {
                await _notificationService.NotifyPlantNeedsNutrients(plantId);
                return Ok(new { Message = "Notification sent for plant needing nutrients." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while sending the notification.", Details = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves all notifications associated with a specific plant.
        /// Accessible by any authenticated user (Admin, Manager, or Farmer).
        /// </summary>
        [Authorize]
        [HttpGet("Plant/{plantId}/notifications")]
        public async Task<IActionResult> GetPlantNotifications(Guid plantId)
        {
            try
            {
                var userTimeZoneId = User.Claims.FirstOrDefault(c => c.Type == "timezone")?.Value!;
                var notifications = await _plantService.GetPlantNotificationDTOs(plantId, userTimeZoneId);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving notifications.", Details = ex.Message });
            }
        }

        /// <summary>
        /// Updates a list of plant-specific notifications to 'Read' status.
        /// </summary>
        /// <param name="notificationIds">A list of GUIDs for the notifications to be updated.</param>
        [Authorize]
        [HttpPatch("plants/notifications/read")]
        public async Task<IActionResult> MarkNotificationsAsRead([FromBody] List<Guid> notificationIds)
        {
            if (notificationIds == null || !notificationIds.Any())
                return BadRequest("Notification IDs are required.");

            await _plantService.MarkPlantNotificationsAsRead(notificationIds);

            return Ok(new { Message = "Notifications marked as read." });
        }

        /// <summary>
        /// Retrieves all notifications related to a specific greenhouse.
        /// Restricted to Admins and the Managers overseeing the greenhouse.
        /// </summary>
        [Authorize(Roles = "Admin,Manager")]
        [HttpGet("Greenhouse/{greenhouseId}/notifications")]
        public async Task<IActionResult> GetGreenhouseNotifications(Guid greenhouseId)
        {
            try
            {
                var userTimeZoneId = User.Claims.FirstOrDefault(c => c.Type == "timezone")?.Value!;
                var notifications = await _greenhouseService.GetGreenhouseNotifications(greenhouseId,userTimeZoneId);
                if (notifications == null)
                    return BadRequest("There is no notifications for this greenhouse");
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving notifications.", Details = ex.Message });
            }
        }

        /// <summary>
        /// Returns a global list of all notifications across all greenhouses.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("Greenhouses/notifications")]
        public async Task<IActionResult> GetAllGreenhousesNotifications()
        {
            try
            {
                var notifications = await _greenhouseService.GetAllGreenhousesNotifications();
                if (notifications == null)
                    return BadRequest("There is no notifications for greenhouses");
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving notifications.", Details = ex.Message });
            }
        }

        /// <summary>
        /// Marks greenhouse-level notifications as read.
        /// </summary>
        /// <param name="notificationIds">List of notification GUIDs.</param>
        [Authorize(Roles = "Admin,Manager")]
        [HttpPatch("greenhouse/notifications/read")]
        public async Task<IActionResult> MarkGreenhouseNotificationAsRead([FromBody] List<Guid> notificationIds)
        {
            try
            {
                await _greenhouseService.MarkGreenhouseNotificationAsRead(notificationIds);
                return Ok(new { Message = "Notification for the greenhouse marked as read." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while marking notification as read.", Details = ex.Message });
            }
        }
    }
}