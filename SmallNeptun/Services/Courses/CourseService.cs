using Microsoft.EntityFrameworkCore;
using SmallNeptun.Dtos.Courses;
using SmallNeptun.Entities;
using SmallNeptun.Enums.UserEnums;
using SmallNeptun.Repository;

namespace SmallNeptun.Services.Courses
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CourseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<(int statusCode, string errorMessage, CourseViewDto? course)> CreateAsync(CreateCourseDto dto)
        {
            if (await _unitOfWork.Courses.Query().AnyAsync(c => c.CourseCode == dto.CourseCode))
            {
                return (2, "Course code is already used.", null);
            }

            var subject = await _unitOfWork.Subjects.Query()
                .FirstOrDefaultAsync(s => s.Code == dto.SubjectCode);
            if (subject is null)
            {
                return (1, $"Subject with code {dto.SubjectCode} was not found.", null);
            }

            if (!subject.IsActive)
            {
                return (2, "Cannot create course for inactive subject.", null);
            }

            var semester = await _unitOfWork.Semesters.GetByIdAsync(dto.SemesterId);
            if (semester is null)
            {
                return (1, $"Semester with id {dto.SemesterId} was not found.", null);
            }

            if (dto.MaxStudents <= 0)
            {
                return (3, "Max students must be greater than zero.", null);
            }

            if (dto.HourCount <= 0)
            {
                return (3, "Hour count must be greater than zero.", null);
            }

            if (dto.TeacherIds.Count == 0)
            {
                return (3, "At least one teacher must be assigned to the course.", null);
            }

            var distinctTeacherIds = dto.TeacherIds.Distinct().ToList();
            var teachers = await _unitOfWork.Users.Query()
                .Where(u => distinctTeacherIds.Contains(u.Id))
                .ToListAsync();

            if (teachers.Count != distinctTeacherIds.Count)
            {
                return (1, "One or more teachers were not found.", null);
            }

            if (teachers.Any(t => t.UserType != UserType.Teacher || !t.IsActive))
            {
                return (3, "Every assigned user must be an active teacher.", null);
            }

            var course = new Course
            {
                CourseCode = dto.CourseCode,
                SubjectId = subject.Id,
                SemesterId = semester.Id,
                MaxStudents = dto.MaxStudents,
                CourseType = dto.CourseType,
                CourseForm = dto.CourseForm,
                HourType = dto.HourType,
                HourCount = dto.HourCount
            };

            await _unitOfWork.Courses.AddAsync(course);
            await _unitOfWork.SaveAsync();

            foreach (var teacher in teachers)
            {
                await _unitOfWork.CourseInstructors.AddAsync(new CourseInstructor
                {
                    CourseId = course.Id,
                    TeacherId = teacher.Id
                });
            }

            await _unitOfWork.SaveAsync();

            var createdCourse = await GetCourseWithDetails(course.Id);
            return (0, "", MapToViewDto(createdCourse!));
        }

        public async Task<(int statusCode, string errorMessage, CourseViewDto? course)> GetByIdAsync(int courseId)
        {
            var course = await GetCourseWithDetails(courseId);
            if (course is null)
            {
                return (1, $"Course with id {courseId} was not found.", null);
            }

            return (0, "", MapToViewDto(course));
        }

        public async Task<(int statusCode, string errorMessage, CourseViewDto? course)> UpdateAsync(int courseId, UpdateCourseDto dto)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(courseId);
            if (course is null)
            {
                return (1, $"Course with id {courseId} was not found.", null);
            }

            if (await _unitOfWork.Courses.Query().AnyAsync(c => c.CourseCode == dto.CourseCode && c.Id != courseId))
            {
                return (2, "Course code is already used.", null);
            }

            if (!await _unitOfWork.Semesters.Query().AnyAsync(s => s.Id == dto.SemesterId))
            {
                return (1, $"Semester with id {dto.SemesterId} was not found.", null);
            }

            if (dto.MaxStudents <= 0)
            {
                return (3, "Max students must be greater than zero.", null);
            }

            if (dto.HourCount <= 0)
            {
                return (3, "Hour count must be greater than zero.", null);
            }

            course.CourseCode = dto.CourseCode;
            course.SemesterId = dto.SemesterId;
            course.MaxStudents = dto.MaxStudents;
            course.CourseType = dto.CourseType;
            course.CourseForm = dto.CourseForm;
            course.HourType = dto.HourType;
            course.HourCount = dto.HourCount;

            _unitOfWork.Courses.Update(course);
            await _unitOfWork.SaveAsync();

            var updatedCourse = await GetCourseWithDetails(courseId);
            return (0, "", MapToViewDto(updatedCourse!));
        }

        public async Task<(int statusCode, string errorMessage)> DeleteAsync(int courseId)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(courseId);
            if (course is null)
            {
                return (1, $"Course with id {courseId} was not found.");
            }

            if (await _unitOfWork.Enrollments.Query().AnyAsync(e => e.CourseId == courseId))
            {
                return (2, "Course cannot be deleted because students are enrolled.");
            }

            _unitOfWork.Courses.Delete(course);
            await _unitOfWork.SaveAsync();

            return (0, "");
        }

        private async Task<Course?> GetCourseWithDetails(int courseId)
        {
            return await _unitOfWork.Courses.Query()
                .Include(c => c.Subject)
                .Include(c => c.Semester)
                .Include(c => c.CourseInstructors)
                    .ThenInclude(ci => ci.Teacher)
                .FirstOrDefaultAsync(c => c.Id == courseId);
        }

        private static CourseViewDto MapToViewDto(Course course)
        {
            return new CourseViewDto
            {
                Id = course.Id,
                CourseCode = course.CourseCode,
                SubjectCode = course.Subject.Code,
                SubjectName = course.Subject.Name,
                Semester = $"{course.Semester.AcademicYear}/{course.Semester.SemesterNumber}",
                MaxStudents = course.MaxStudents,
                CourseType = course.CourseType,
                CourseForm = course.CourseForm,
                HourType = course.HourType,
                HourCount = course.HourCount,
                Teachers = course.CourseInstructors.Select(ci => ci.Teacher.Name).ToList()
            };
        }
    }
}
