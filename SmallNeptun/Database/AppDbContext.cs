using Microsoft.EntityFrameworkCore;
using SmallNeptun.Entities;

namespace SmallNeptun.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }
        public DbSet<User> Users => Set<User>();
        public DbSet<Subject> Subjects => Set<Subject>();
        public DbSet<Semester> Semesters => Set<Semester>();
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<CourseInstructor> CourseInstructors => Set<CourseInstructor>();
        public DbSet<Enrollment> Enrollments => Set<Enrollment>();
        public DbSet<Schedule> Schedules => Set<Schedule>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<Grade> Grades => Set<Grade>();
        public DbSet<Signature> Signatures => Set<Signature>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().Property(u => u.UserType).HasConversion<string>();
            modelBuilder.Entity<User>().Property(u => u.StudyForm).HasConversion<string>();
            
            modelBuilder.Entity<Course>().Property(c => c.CourseType).HasConversion<string>();
            modelBuilder.Entity<Course>().Property(c => c.CourseForm).HasConversion<string>();
           
            modelBuilder.Entity<Course>().Property(c => c.HourType).HasConversion<string>();
            modelBuilder.Entity<Signature>().Property(s => s.Value).HasConversion<string>();


            modelBuilder.Entity<User>().Property(u => u.Name).HasMaxLength(100);
            modelBuilder.Entity<User>().Property(u => u.Email).HasMaxLength(100);
            modelBuilder.Entity<User>().Property(u => u.Password).HasMaxLength(200);
           
            modelBuilder.Entity<Subject>().Property(s => s.Code).HasMaxLength(20);
            modelBuilder.Entity<Subject>().Property(s => s.Name).HasMaxLength(100);
            
            modelBuilder.Entity<Course>().Property(c => c.CourseCode).HasMaxLength(30);
            
            modelBuilder.Entity<Semester>().Property(s => s.AcademicYear).HasMaxLength(20);
            
            modelBuilder.Entity<Notification>().Property(n => n.Message).HasMaxLength(500);
            modelBuilder.Entity<Notification>().Property(n => n.Reason).HasMaxLength(200);

            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Subject>().HasIndex(s => s.Code).IsUnique();
            modelBuilder.Entity<Course>().HasIndex(c => c.CourseCode).IsUnique();

            modelBuilder.Entity<Semester>()
                .HasIndex(s => new { s.AcademicYear, s.SemesterNumber })
                .IsUnique();

            modelBuilder.Entity<Enrollment>()
                .HasIndex(e => new { e.StudentId, e.CourseId })
                .IsUnique();

            modelBuilder.Entity<CourseInstructor>()
                .HasIndex(ci => new { ci.TeacherId, ci.CourseId })
                .IsUnique();

            // Course -> Subject
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Subject)
                .WithMany(s => s.Courses)
                .HasForeignKey(c => c.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            // Course -> Semester
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Semester)
                .WithMany(s => s.Courses)
                .HasForeignKey(c => c.SemesterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Enrollment -> Student
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Enrollment -> Course
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // CourseInstructor -> Teacher
            modelBuilder.Entity<CourseInstructor>()
                .HasOne(ci => ci.Teacher)
                .WithMany(u => u.CourseInstructors)
                .HasForeignKey(ci => ci.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            // CourseInstructor -> Course
            modelBuilder.Entity<CourseInstructor>()
                .HasOne(ci => ci.Course)
                .WithMany(c => c.CourseInstructors)
                .HasForeignKey(ci => ci.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Schedule -> Course
            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.Course)
                .WithMany(c => c.Schedules)
                .HasForeignKey(s => s.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Notification -> User
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Notification -> Course
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Course)
                .WithMany(c => c.Notifications)
                .HasForeignKey(n => n.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Grade -> Student
            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Student)
                .WithMany(u => u.Grades)
                .HasForeignKey(g => g.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Grade -> Subject
            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Subject)
                .WithMany(s => s.Grades)
                .HasForeignKey(g => g.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            // Grade -> Semester
            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Semester)
                .WithMany()
                .HasForeignKey(g => g.SemesterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Signature -> Student
            modelBuilder.Entity<Signature>()
                .HasOne(s => s.Student)
                .WithMany(u => u.Signatures)
                .HasForeignKey(s => s.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Signature -> Subject
            modelBuilder.Entity<Signature>()
                .HasOne(s => s.Subject)
                .WithMany(sub => sub.Signatures)
                .HasForeignKey(s => s.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            // Signature -> Semester
            modelBuilder.Entity<Signature>()
                .HasOne(s => s.Semester)
                .WithMany()
                .HasForeignKey(s => s.SemesterId)
                .OnDelete(DeleteBehavior.Restrict);

        }

    }
}
