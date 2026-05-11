using SmallNeptun.Enums.CourseEnums;

namespace SmallNeptun.Dtos.Courses
{
    public class UpdateCourseDto
    {
        public string CourseCode { get; set; }
        public int SemesterId { get; set; }
        public int MaxStudents { get; set; }
        /// <summary>Ertekek: Lecture, Practice, Lab.</summary>
        public CourseType CourseType { get; set; }
        /// <summary>Ertekek: FullTime, PartTime, Both.</summary>
        public CourseForm CourseForm { get; set; }
        /// <summary>Ertekek: Weekly, Semester.</summary>
        public HourType HourType { get; set; }
        public int HourCount { get; set; }
    }
}
