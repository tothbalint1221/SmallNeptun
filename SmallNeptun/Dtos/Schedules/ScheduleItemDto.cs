namespace SmallNeptun.Dtos.Schedules
{
    public class ScheduleItemDto
    {
        public bool IsWeekly { get; set; }
        /// <summary>Ertekek: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday. Tombositett oranal null.</summary>
        public DayOfWeek? DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DateOnly? SpecificDate { get; set; }
    }
}
