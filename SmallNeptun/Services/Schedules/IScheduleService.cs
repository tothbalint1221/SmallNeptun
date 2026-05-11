using SmallNeptun.Dtos.Schedules;

namespace SmallNeptun.Services.Schedules
{
    public interface IScheduleService
    {
        Task<(int statusCode, string errorMessage, IEnumerable<ScheduleViewDto>? schedules)> CreateAsync(int courseId, CreateScheduleDto dto);
        Task<(int statusCode, string errorMessage, IEnumerable<ScheduleViewDto>? schedules)> ModifyAsync(int courseId, ModifyScheduleDto dto);
    }
}
