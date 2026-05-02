namespace SmallNeptun.Entities
{
    public class Semester
    {
        public int Id { get; set; }
        public string AcademicYear { get; set; } 
        public int SemesterNumber { get; set; }
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
