using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmallNeptun.Dtos.Enrollments;
using SmallNeptun.Dtos.Users;
using SmallNeptun.Entities;
using SmallNeptun.Enums.CourseEnums;
using SmallNeptun.Enums.UserEnums;
using SmallNeptun.Repository;

namespace SmallNeptun.Services.Enrollments
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EnrollmentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<(int statusCode, string errorMessage)> RegisterToSubjectAsync(int subjectId, RegisterSubjectDto dto)
        {
            var student = await _unitOfWork.Users.GetByIdAsync(dto.StudentId);
            if (student is null)
            {
                return (1, $"Student with id {dto.StudentId} was not found.");
            }

            if (student.UserType != UserType.Student || !student.IsActive)
            {
                return (3, "User must be an active student.");
            }

            var subject = await _unitOfWork.Subjects.GetByIdAsync(subjectId);
            if (subject is null)
            {
                return (1, $"Subject with id {subjectId} was not found.");
            }

            if (!subject.IsActive)
            {
                return (2, "This subject is inactive.");
            }

            if (dto.CourseIds.Count == 0)
            {
                return (3, "At least one course must be selected.");
            }

            var selectedCourseIds = dto.CourseIds.Distinct().ToList();
            if (selectedCourseIds.Count != dto.CourseIds.Count)
            {
                return (3, "Course list contains duplicates.");
            }

            var courses = await _unitOfWork.Courses.Query()
                .Where(c => selectedCourseIds.Contains(c.Id))
                .Include(c => c.Enrollments)
                .ToListAsync();

            if (courses.Count != selectedCourseIds.Count)
            {
                return (1, "One or more selected courses were not found.");
            }

            var validation = await ValidateSubjectRegistration(subjectId, student, courses);
            if (validation.statusCode != 0)
            {
                return validation;
            }

            foreach (var course in courses)
            {
                await _unitOfWork.Enrollments.AddAsync(new Enrollment
                {
                    StudentId = student.Id,
                    CourseId = course.Id
                });
            }

            await _unitOfWork.SaveAsync();
            return (0, "");
        }

        public async Task<(int statusCode, string errorMessage)> UnregisterFromSubjectAsync(int subjectId, UnregisterSubjectDto dto)
        {
            var student = await _unitOfWork.Users.GetByIdAsync(dto.StudentId);
            if (student is null)
            {
                return (1, $"Student with id {dto.StudentId} was not found.");
            }

            if (student.UserType != UserType.Student || !student.IsActive)
            {
                return (3, "User must be an active student.");
            }

            var subject = await _unitOfWork.Subjects.GetByIdAsync(subjectId);
            if (subject is null)
            {
                return (1, $"Subject with id {subjectId} was not found.");
            }

            if (!subject.IsActive)
            {
                return (2, "This subject is inactive.");
            }

            var enrollments = await _unitOfWork.Enrollments.Query()
                .Include(e => e.Course)
                .Where(e => e.StudentId == dto.StudentId &&
                            e.Course.SubjectId == subjectId &&
                            e.Course.SemesterId == dto.SemesterId)
                .ToListAsync();

            if (enrollments.Count == 0)
            {
                return (1, "No enrollment was found for this subject and semester.");
            }

            foreach (var enrollment in enrollments)
            {
                _unitOfWork.Enrollments.Delete(enrollment);
            }

            await _unitOfWork.SaveAsync();
            return (0, "");
        }

        public async Task<(int statusCode, string errorMessage)> ChangeCourseAsync(ChangeCourseDto dto)
        {
            var fromEnrollment = await _unitOfWork.Enrollments.Query()
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.StudentId == dto.StudentId && e.CourseId == dto.FromCourseId);

            if (fromEnrollment is null)
            {
                return (1, "The student is not enrolled on the source course.");
            }

            var toCourse = await _unitOfWork.Courses.Query()
                .Include(c => c.Enrollments)
                .FirstOrDefaultAsync(c => c.Id == dto.ToCourseId);

            if (toCourse is null)
            {
                return (1, $"Target course with id {dto.ToCourseId} was not found.");
            }

            if (fromEnrollment.Course.SubjectId != toCourse.SubjectId)
            {
                return (3, "Course change is only allowed inside the same subject.");
            }

            if (fromEnrollment.Course.CourseType != toCourse.CourseType)
            {
                return (3, "Course change is only allowed to the same course type.");
            }

            if (fromEnrollment.Course.SemesterId != toCourse.SemesterId)
            {
                return (3, "Course change is only allowed inside the same semester.");
            }

            var student = await _unitOfWork.Users.GetByIdAsync(dto.StudentId);
            if (student is null)
            {
                return (1, $"Student with id {dto.StudentId} was not found.");
            }

            if (student.UserType != UserType.Student || !student.IsActive)
            {
                return (3, "User must be an active student.");
            }

            var subject = await _unitOfWork.Subjects.GetByIdAsync(fromEnrollment.Course.SubjectId);
            if (subject is null)
            {
                return (1, $"Subject with id {fromEnrollment.Course.SubjectId} was not found.");
            }

            if (!subject.IsActive)
            {
                return (2, "This subject is inactive.");
            }

            if (!CourseFormMatchesStudent(toCourse.CourseForm, student.StudyForm))
            {
                return (3, "Target course form does not match the student's study form.");
            }

            if (toCourse.Enrollments.Count >= toCourse.MaxStudents)
            {
                return (2, "Target course is full.");
            }

            if (await _unitOfWork.Enrollments.Query().AnyAsync(e => e.StudentId == dto.StudentId && e.CourseId == dto.ToCourseId))
            {
                return (2, "Student is already enrolled on the target course.");
            }

            fromEnrollment.CourseId = toCourse.Id;
            _unitOfWork.Enrollments.Update(fromEnrollment);
            await _unitOfWork.SaveAsync();

            return (0, "");
        }

        public async Task<(int statusCode, string errorMessage, IEnumerable<UserViewDto>? students)> GetCourseStudentsAsync(int courseId)
        {
            if (!await _unitOfWork.Courses.Query().AnyAsync(c => c.Id == courseId))
            {
                return (1, $"Course with id {courseId} was not found.", null);
            }

            var students = await _unitOfWork.Enrollments.Query()
                .Where(e => e.CourseId == courseId)
                .Include(e => e.Student)
                .Select(e => e.Student)
                .ToListAsync();

            return (0, "", _mapper.Map<IEnumerable<UserViewDto>>(students));
        }

        public async Task<(int statusCode, string errorMessage, IEnumerable<UserViewDto>? students)> GetSubjectStudentsAsync(int subjectId, SubjectStudentsQueryDto query)
        {
            if (!await _unitOfWork.Subjects.Query().AnyAsync(s => s.Id == subjectId))
            {
                return (1, $"Subject with id {subjectId} was not found.", null);
            }

            if (!await _unitOfWork.Semesters.Query().AnyAsync(s => s.Id == query.SemesterId))
            {
                return (1, $"Semester with id {query.SemesterId} was not found.", null);
            }

            var students = await _unitOfWork.Enrollments.Query()
                .Include(e => e.Course)
                .Include(e => e.Student)
                .Where(e => e.Course.SubjectId == subjectId && e.Course.SemesterId == query.SemesterId)
                .Select(e => e.Student)
                .Distinct()
                .ToListAsync();

            return (0, "", _mapper.Map<IEnumerable<UserViewDto>>(students));
        }

        private async Task<(int statusCode, string errorMessage)> ValidateSubjectRegistration(int subjectId, User student, List<Course> selectedCourses)
        {
            if (selectedCourses.Any(c => c.SubjectId != subjectId))
            {
                return (3, "Every selected course must belong to the requested subject.");
            }

            var semesterIds = selectedCourses.Select(c => c.SemesterId).Distinct().ToList();
            if (semesterIds.Count != 1)
            {
                return (3, "Every selected course must belong to the same semester.");
            }

            if (selectedCourses.Any(c => !CourseFormMatchesStudent(c.CourseForm, student.StudyForm)))
            {
                return (3, "Every selected course must match the student's study form.");
            }

            if (selectedCourses.Any(c => c.Enrollments.Count >= c.MaxStudents))
            {
                return (2, "One or more selected courses are full.");
            }

            var selectedTypes = selectedCourses.Select(c => c.CourseType).ToList();
            if (selectedTypes.Count != selectedTypes.Distinct().Count())
            {
                return (3, "Only one course can be selected from each course type.");
            }

            var semesterId = semesterIds[0];
            var availableTypes = await _unitOfWork.Courses.Query()
                .Where(c => c.SubjectId == subjectId &&
                            c.SemesterId == semesterId &&
                            (c.CourseForm == CourseForm.Both ||
                             (c.CourseForm == CourseForm.FullTime && student.StudyForm == StudyForm.FullTime) ||
                             (c.CourseForm == CourseForm.PartTime && student.StudyForm == StudyForm.PartTime)))
                .Select(c => c.CourseType)
                .Distinct()
                .ToListAsync();

            if (!availableTypes.OrderBy(t => t).SequenceEqual(selectedTypes.Distinct().OrderBy(t => t)))
            {
                return (3, "Exactly one course must be selected from every available course type.");
            }

            var selectedCourseIds = selectedCourses.Select(c => c.Id).ToList();
            if (await _unitOfWork.Enrollments.Query().AnyAsync(e => e.StudentId == student.Id && selectedCourseIds.Contains(e.CourseId)))
            {
                return (2, "Student is already enrolled on one or more selected courses.");
            }

            return (0, "");
        }

        private static bool CourseFormMatchesStudent(CourseForm courseForm, StudyForm studyForm)
        {
            return courseForm == CourseForm.Both ||
                   courseForm == CourseForm.FullTime && studyForm == StudyForm.FullTime ||
                   courseForm == CourseForm.PartTime && studyForm == StudyForm.PartTime;
        }
    }
}
