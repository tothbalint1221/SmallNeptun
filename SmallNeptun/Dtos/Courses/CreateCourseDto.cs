using SmallNeptun.Enums.CourseEnums;

namespace SmallNeptun.Dtos.Courses
{
    public class CreateCourseDto
    {
        public string CourseCode { get; set; }
        public string SubjectCode { get; set; }
        public int SemesterId { get; set; }
        public int MaxStudents { get; set; }
        public CourseType CourseType { get; set; }
        public CourseForm CourseForm { get; set; }
        public HourType HourType { get; set; }
        public int HourCount { get; set; }
        public List<int> TeacherIds { get; set; } = new List<int>();
    }
}
