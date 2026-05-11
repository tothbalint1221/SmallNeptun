using Microsoft.AspNetCore.Mvc;
using SmallNeptun.Dtos.Notifications;
using SmallNeptun.Services.Notifications;

namespace SmallNeptun.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] NotificationQueryDto query)
        {
            var notifications = await _notificationService.GetAllAsync(query);
            return Ok(notifications);
        }
    }
}
