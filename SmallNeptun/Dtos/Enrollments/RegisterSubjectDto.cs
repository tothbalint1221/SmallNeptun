namespace SmallNeptun.Dtos.Enrollments
{
    public class RegisterSubjectDto
    {
        public int StudentId { get; set; }
        public List<int> CourseIds { get; set; } = new List<int>();
    }
}
