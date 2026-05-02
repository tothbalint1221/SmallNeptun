using System.ComponentModel.DataAnnotations;
using SmallNeptun.Enums.UserEnums;

namespace SmallNeptun.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public UserType UserType { get; set; }
        public StudyForm StudyForm { get; set; } = StudyForm.None;
        public bool IsActive { get; set; } = true;

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<CourseInstructor> CourseInstructors { get; set; } = new List<CourseInstructor>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
