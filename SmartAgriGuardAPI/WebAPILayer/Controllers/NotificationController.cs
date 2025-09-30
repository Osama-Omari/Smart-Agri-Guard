using ApplicationLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPILayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class NotificationController : ControllerBase
    {

        private readonly INotificationService _notificationService;
        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

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

    }
}
