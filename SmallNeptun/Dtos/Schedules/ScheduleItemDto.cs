namespace SmallNeptun.Dtos.Schedules
{
    public class ScheduleItemDto
    {
        public bool IsWeekly { get; set; }
        public DayOfWeek? DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DateOnly? SpecificDate { get; set; }
    }
}
