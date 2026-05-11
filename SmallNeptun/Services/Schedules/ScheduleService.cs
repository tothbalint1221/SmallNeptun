using Microsoft.EntityFrameworkCore;
using SmallNeptun.Dtos.Schedules;
using SmallNeptun.Entities;
using SmallNeptun.Repository;

namespace SmallNeptun.Services.Schedules
{
    public class ScheduleService : IScheduleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ScheduleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<(int statusCode, string errorMessage, IEnumerable<ScheduleViewDto>? schedules)> CreateAsync(int courseId, CreateScheduleDto dto)
        {
            var validation = await ValidateCourseAndSchedules(courseId, dto.Schedules);
            if (validation.statusCode != 0)
            {
                return (validation.statusCode, validation.errorMessage, null);
            }

            foreach (var item in dto.Schedules)
            {
                await _unitOfWork.Schedules.AddAsync(MapToEntity(courseId, item));
            }

            await _unitOfWork.SaveAsync();

            var schedules = await GetCourseSchedules(courseId);
            return (0, "", schedules);
        }

        public async Task<(int statusCode, string errorMessage, IEnumerable<ScheduleViewDto>? schedules)> ModifyAsync(int courseId, ModifyScheduleDto dto)
        {
            var validation = await ValidateCourseAndSchedules(courseId, dto.Schedules);
            if (validation.statusCode != 0)
            {
                return (validation.statusCode, validation.errorMessage, null);
            }

            var oldSchedules = await _unitOfWork.Schedules.Query()
                .Where(s => s.CourseId == courseId)
                .ToListAsync();

            foreach (var schedule in oldSchedules)
            {
                _unitOfWork.Schedules.Delete(schedule);
            }

            foreach (var item in dto.Schedules)
            {
                await _unitOfWork.Schedules.AddAsync(MapToEntity(courseId, item));
            }

            await _unitOfWork.SaveAsync();

            var schedules = await GetCourseSchedules(courseId);
            return (0, "", schedules);
        }

        private async Task<(int statusCode, string errorMessage)> ValidateCourseAndSchedules(int courseId, List<ScheduleItemDto> schedules)
        {
            var course = await _unitOfWork.Courses.Query()
                .Include(c => c.Subject)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course is null)
            {
                return (1, $"Course with id {courseId} was not found.");
            }

            if (!course.Subject.IsActive)
            {
                return (2, "This subject is inactive.");
            }

            if (schedules.Count == 0)
            {
                return (3, "At least one schedule item is required.");
            }

            foreach (var schedule in schedules)
            {
                if (schedule.StartTime >= schedule.EndTime)
                {
                    return (3, "StartTime must be earlier than EndTime.");
                }

                if (schedule.IsWeekly && schedule.DayOfWeek is null)
                {
                    return (3, "Weekly schedule requires DayOfWeek.");
                }

                if (schedule.IsWeekly && schedule.SpecificDate is not null)
                {
                    return (3, "Weekly schedule cannot have SpecificDate.");
                }

                if (!schedule.IsWeekly && schedule.SpecificDate is null)
                {
                    return (3, "Specific schedule requires SpecificDate.");
                }

                if (!schedule.IsWeekly && schedule.DayOfWeek is not null)
                {
                    return (3, "Specific schedule cannot have DayOfWeek.");
                }
            }

            return (0, "");
        }

        private static Schedule MapToEntity(int courseId, ScheduleItemDto item)
        {
            return new Schedule
            {
                CourseId = courseId,
                IsWeekly = item.IsWeekly,
                DayOfWeek = item.DayOfWeek,
                StartTime = item.StartTime,
                EndTime = item.EndTime,
                SpecificDate = item.SpecificDate
            };
        }

        private async Task<IEnumerable<ScheduleViewDto>> GetCourseSchedules(int courseId)
        {
            var schedules = await _unitOfWork.Schedules.Query()
                .Where(s => s.CourseId == courseId)
                .OrderBy(s => s.SpecificDate)
                .ThenBy(s => s.DayOfWeek)
                .ThenBy(s => s.StartTime)
                .ToListAsync();

            return schedules.Select(s => new ScheduleViewDto
            {
                Id = s.Id,
                CourseId = s.CourseId,
                IsWeekly = s.IsWeekly,
                DayOfWeek = s.DayOfWeek,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                SpecificDate = s.SpecificDate
            });
        }
    }
}
