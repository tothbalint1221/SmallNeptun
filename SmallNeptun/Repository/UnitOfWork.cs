using SmallNeptun.Database;
using SmallNeptun.Entities;

namespace SmallNeptun.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IRepository<User> Users => new Repository<User>(_context);
        public IRepository<Subject> Subjects => new Repository<Subject>(_context);
        public IRepository<Semester> Semesters => new Repository<Semester>(_context);
        public IRepository<Course> Courses => new Repository<Course>(_context);
        public IRepository<Enrollment> Enrollments => new Repository<Enrollment>(_context);
        public IRepository<CourseInstructor> CourseInstructors => new Repository<CourseInstructor>(_context);
        public IRepository<Schedule> Schedules => new Repository<Schedule>(_context);
        public IRepository<Notification> Notifications => new Repository<Notification>(_context);
        public IRepository<Grade> Grades => new Repository<Grade>(_context);
        public IRepository<Signature> Signatures => new Repository<Signature>(_context);

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
