using SmallNeptun.Enums.CourseEnums;
using System.Runtime.Serialization;

namespace SmallNeptun.Entities
{
    public class Course
    {
        public int Id { get; set; }
        public string CourseCode { get; set; }
        public int MaxStudents { get; set; }
        public CourseType CourseType { get; set; }
        public CourseForm CourseForm { get; set; }
        public HourType HourType { get; set; }
        public int HourCount { get; set; }

        public int SemesterId { get; set; }
        public Semester Semester { get; set; }
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<CourseInstructor> CourseInstructors { get; set; } = new List<CourseInstructor>();
        public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    }
}
