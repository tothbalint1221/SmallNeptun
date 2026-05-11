using Microsoft.EntityFrameworkCore;
using SmallNeptun.Dtos.Notifications;
using SmallNeptun.Repository;

namespace SmallNeptun.Services.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<NotificationViewDto>> GetAllAsync(NotificationQueryDto query)
        {
            var notificationsQuery = _unitOfWork.Notifications.Query()
                .Include(n => n.User)
                .Include(n => n.Course)
                .AsQueryable();

            if (query.UserId is not null)
            {
                notificationsQuery = notificationsQuery.Where(n => n.UserId == query.UserId);
            }

            if (query.CourseId is not null)
            {
                notificationsQuery = notificationsQuery.Where(n => n.CourseId == query.CourseId);
            }

            var notifications = await notificationsQuery
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return notifications.Select(n => new NotificationViewDto
            {
                Id = n.Id,
                Message = n.Message,
                Reason = n.Reason,
                CreatedAt = n.CreatedAt,
                UserId = n.UserId,
                UserName = n.User.Name,
                CourseId = n.CourseId,
                CourseCode = n.Course.CourseCode
            });
        }
    }
}
