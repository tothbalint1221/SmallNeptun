using SmallNeptun.Enums.CourseEnums;

namespace SmallNeptun.Dtos.Courses
{
    public class CourseViewDto
    {
        public int Id { get; set; }
        public string CourseCode { get; set; }
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public string Semester { get; set; }
        public int MaxStudents { get; set; }
        public CourseType CourseType { get; set; }
        public CourseForm CourseForm { get; set; }
        public HourType HourType { get; set; }
        public int HourCount { get; set; }
        public List<string> Teachers { get; set; } = new List<string>();
    }
}
