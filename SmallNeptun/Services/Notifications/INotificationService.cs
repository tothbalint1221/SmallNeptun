using SmallNeptun.Dtos.Notifications;

namespace SmallNeptun.Services.Notifications
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationViewDto>> GetAllAsync(NotificationQueryDto query);
    }
}
