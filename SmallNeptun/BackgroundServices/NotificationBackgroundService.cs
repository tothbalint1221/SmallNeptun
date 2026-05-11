using Microsoft.EntityFrameworkCore;
using SmallNeptun.Database;
using SmallNeptun.Entities;

namespace SmallNeptun.BackgroundServices
{
    public class NotificationBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public NotificationBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await CreateUpcomingScheduleNotifications(stoppingToken);
            }
        }

        private async Task CreateUpcomingScheduleNotifications(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var localNow = DateTime.Now;
            var limit = localNow.AddMinutes(30);

            var schedules = await context.Schedules
                .Include(s => s.Course)
                    .ThenInclude(c => c.Subject)
                .Include(s => s.Course)
                    .ThenInclude(c => c.Enrollments)
                        .ThenInclude(e => e.Student)
                .Include(s => s.Course)
                    .ThenInclude(c => c.CourseInstructors)
                        .ThenInclude(ci => ci.Teacher)
                .Where(s => s.Course.Subject.IsActive)
                .ToListAsync(cancellationToken);

            foreach (var schedule in schedules)
            {
                var lessonStart = GetNextLessonStart(schedule, localNow);

                if (lessonStart is null || lessonStart < localNow || lessonStart > limit)
                {
                    continue;
                }

                var reason = $"ScheduleReminder:{schedule.Id}:{lessonStart:yyyyMMddHHmm}";
                var userIds = GetNotificationUserIds(schedule);

                foreach (var userId in userIds)
                {
                    var alreadyExists = await context.Notifications.AnyAsync(n =>
                        n.UserId == userId &&
                        n.CourseId == schedule.CourseId &&
                        n.Reason == reason,
                        cancellationToken);

                    if (alreadyExists)
                    {
                        continue;
                    }

                    context.Notifications.Add(new Notification
                    {
                        UserId = userId,
                        CourseId = schedule.CourseId,
                        Reason = reason,
                        Message = $"Course {schedule.Course.CourseCode} starts at {lessonStart:yyyy-MM-dd HH:mm}."
                    });
                }
            }

            await context.SaveChangesAsync(cancellationToken);
        }

        private static DateTime? GetNextLessonStart(Schedule schedule, DateTime localNow)
        {
            if (schedule.IsWeekly)
            {
                if (schedule.DayOfWeek is null)
                {
                    return null;
                }

                var daysUntilLesson = ((int)schedule.DayOfWeek.Value - (int)localNow.DayOfWeek + 7) % 7;
                var date = DateOnly.FromDateTime(localNow.AddDays(daysUntilLesson));
                var lessonStart = date.ToDateTime(schedule.StartTime);

                if (lessonStart < localNow)
                {
                    lessonStart = lessonStart.AddDays(7);
                }

                return lessonStart;
            }

            if (schedule.SpecificDate is null)
            {
                return null;
            }

            return schedule.SpecificDate.Value.ToDateTime(schedule.StartTime);
        }

        private static List<int> GetNotificationUserIds(Schedule schedule)
        {
            var studentIds = schedule.Course.Enrollments
                .Where(e => e.Student.IsActive)
                .Select(e => e.StudentId);

            var teacherIds = schedule.Course.CourseInstructors
                .Where(ci => ci.Teacher.IsActive)
                .Select(ci => ci.TeacherId);

            return studentIds
                .Concat(teacherIds)
                .Distinct()
                .ToList();
        }
    }
}
