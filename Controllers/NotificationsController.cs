using CourseManager.Services;
using Microsoft.AspNetCore.Mvc;

namespace CourseManager.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly NotificationService _notificationService;

        public NotificationsController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // Get all generated notifications.
        // Optionally filter by userId and/or courseId.

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? userId,
            [FromQuery] int? courseId)
        {
            var result = await _notificationService.GetAllAsync(userId, courseId);
            return Ok(result);
        }
    }
}