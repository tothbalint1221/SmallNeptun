using SmallNeptun.Entities;

namespace SmallNeptun.Repository
{
    public interface IUnitOfWork
    {
        IRepository<User> Users { get; }
        IRepository<Subject> Subjects { get; }
        IRepository<Semester> Semesters { get; }
        IRepository<Course> Courses { get; }
        IRepository<Enrollment> Enrollments { get; }
        IRepository<CourseInstructor> CourseInstructors { get; }
        IRepository<Schedule> Schedules { get; } 
        IRepository<Notification> Notifications { get; }
        IRepository<Grade> Grades { get; }
        IRepository<Signature> Signatures { get; }

        Task<int> SaveAsync();

    }
}
