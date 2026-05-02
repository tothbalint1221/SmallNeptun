namespace SmallNeptun.Entities
{
    public class Schedule
    {
        public int Id { get; set; }
        public bool IsWeekly { get; set; }
        public DayOfWeek? DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DateOnly? SpecificDate { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

    }
}
