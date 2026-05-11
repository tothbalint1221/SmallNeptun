using SmallNeptun.Enums.CourseEnums;

namespace SmallNeptun.Dtos.Courses
{
    public class UpdateCourseDto
    {
        public string CourseCode { get; set; }
        public int SemesterId { get; set; }
        public int MaxStudents { get; set; }
        public CourseType CourseType { get; set; }
        public CourseForm CourseForm { get; set; }
        public HourType HourType { get; set; }
        public int HourCount { get; set; }
    }
}
