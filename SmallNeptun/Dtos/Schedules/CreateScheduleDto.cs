namespace SmallNeptun.Dtos.Schedules
{
    public class CreateScheduleDto
    {
        public List<ScheduleItemDto> Schedules { get; set; } = new List<ScheduleItemDto>();
    }
}
