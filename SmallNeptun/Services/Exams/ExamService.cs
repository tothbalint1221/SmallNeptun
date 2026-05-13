using Microsoft.EntityFrameworkCore;
using SmallNeptun.Dtos.Exams;
using SmallNeptun.Entities;
using SmallNeptun.Enums.UserEnums;
using SmallNeptun.Repository;

namespace SmallNeptun.Services.Exams
{
    public class ExamService : IExamService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExamService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<(int statusCode, string errorMessage, ExamViewDto? exam)> CreateAsync(CreateExamDto dto)
        {
            var subject = await _unitOfWork.Subjects.GetByIdAsync(dto.SubjectId);
            if (subject is null)
            {
                return (1, $"Subject with id {dto.SubjectId} was not found.", null);
            }

            if (!subject.IsActive)
            {
                return (2, "This subject is inactive.", null);
            }

            if (!await _unitOfWork.Semesters.Query().AnyAsync(s => s.Id == dto.SemesterId))
            {
                return (1, $"Semester with id {dto.SemesterId} was not found.", null);
            }

            if (!await _unitOfWork.Courses.Query().AnyAsync(c => c.SubjectId == dto.SubjectId && c.SemesterId == dto.SemesterId))
            {
                return (3, "Subject is not announced in this semester.", null);
            }

            if (await _unitOfWork.Exams.Query().AnyAsync(e =>
                e.SubjectId == dto.SubjectId &&
                e.SemesterId == dto.SemesterId &&
                e.ExamTime == dto.ExamTime))
            {
                return (2, "An exam with the same subject, semester and time already exists.", null);
            }

            var exam = new Exam
            {
                SubjectId = dto.SubjectId,
                SemesterId = dto.SemesterId,
                ExamTime = dto.ExamTime
            };

            await _unitOfWork.Exams.AddAsync(exam);
            await _unitOfWork.SaveAsync();

            var createdExam = await GetExamWithDetails(exam.Id);
            return (0, "", MapToViewDto(createdExam!));
        }

        public async Task<IEnumerable<ExamViewDto>> GetAllAsync()
        {
            var exams = await _unitOfWork.Exams.Query()
                .Include(e => e.Subject)
                .Include(e => e.Semester)
                .Include(e => e.ExamRegistrations)
                .OrderBy(e => e.ExamTime)
                .ToListAsync();

            return exams.Select(MapToViewDto);
        }

        public async Task<(int statusCode, string errorMessage, IEnumerable<ExamViewDto>? exams)> GetSubjectExamsAsync(int subjectId, SubjectExamsQueryDto query)
        {
            if (!await _unitOfWork.Subjects.Query().AnyAsync(s => s.Id == subjectId))
            {
                return (1, $"Subject with id {subjectId} was not found.", null);
            }

            var semester = await _unitOfWork.Semesters.GetByIdAsync(query.SemesterId);
            if (semester is null)
            {
                return (1, $"Semester with id {query.SemesterId} was not found.", null);
            }

            var exams = await _unitOfWork.Exams.Query()
                .Include(e => e.Subject)
                .Include(e => e.Semester)
                .Include(e => e.ExamRegistrations)
                .Where(e => e.SubjectId == subjectId && e.SemesterId == query.SemesterId)
                .OrderBy(e => e.ExamTime)
                .ToListAsync();

            return (0, "", exams.Select(MapToViewDto));
        }

        public async Task<(int statusCode, string errorMessage)> DeleteAsync(int examId)
        {
            var exam = await _unitOfWork.Exams.GetByIdAsync(examId);
            if (exam is null)
            {
                return (1, $"Exam with id {examId} was not found.");
            }

            _unitOfWork.Exams.Delete(exam);
            await _unitOfWork.SaveAsync();
            return (0, "");
        }

        public async Task<(int statusCode, string errorMessage)> RegisterAsync(int examId, ExamRegistrationDto dto)
        {
            var exam = await _unitOfWork.Exams.Query()
                .Include(e => e.Subject)
                .FirstOrDefaultAsync(e => e.Id == examId);
            if (exam is null)
            {
                return (1, $"Exam with id {examId} was not found.");
            }

            var user = await _unitOfWork.Users.GetByIdAsync(dto.UserId);
            if (user is null)
            {
                return (1, $"User with id {dto.UserId} was not found.");
            }

            if (user.UserType != UserType.Student || !user.IsActive)
            {
                return (3, "User must be an active student.");
            }

            if (await _unitOfWork.ExamRegistrations.Query().AnyAsync(er => er.UserId == dto.UserId && er.Grade == null))
            {
                return (2, "Student already has a pending exam registration.");
            }

            if (await _unitOfWork.ExamRegistrations.Query().AnyAsync(er => er.ExamId == examId && er.UserId == dto.UserId))
            {
                return (2, "Student is already registered for this exam.");
            }

            if (!await StudentHasSubjectInSemester(dto.UserId, exam.SubjectId, exam.SemesterId))
            {
                return (3, "Student did not take this subject in this semester.");
            }

            await _unitOfWork.ExamRegistrations.AddAsync(new ExamRegistration
            {
                ExamId = examId,
                UserId = dto.UserId
            });
            await _unitOfWork.SaveAsync();

            return (0, "");
        }

        public async Task<(int statusCode, string errorMessage)> AddGradeAsync(int examId, int userId, ExamGradeDto dto)
        {
            if (!IsValidGrade(dto.Grade))
            {
                return (3, "Grade must be between 1 and 5.");
            }

            var registration = await GetRegistration(examId, userId);
            if (registration is null)
            {
                return (1, "Exam registration was not found.");
            }

            if (registration.Grade is not null)
            {
                return (2, "Exam grade is already written.");
            }

            registration.Grade = dto.Grade;
            registration.GradeDate = DateTime.Now;

            _unitOfWork.ExamRegistrations.Update(registration);
            await _unitOfWork.SaveAsync();

            return (0, "");
        }

        public async Task<(int statusCode, string errorMessage)> UpdateGradeAsync(int examId, int userId, ExamGradeDto dto)
        {
            if (!IsValidGrade(dto.Grade))
            {
                return (3, "Grade must be between 1 and 5.");
            }

            var registration = await GetRegistration(examId, userId);
            if (registration is null)
            {
                return (1, "Exam registration was not found.");
            }

            if (registration.Grade is null)
            {
                return (1, "Exam grade was not found.");
            }

            registration.Grade = dto.Grade;
            registration.GradeDate = DateTime.Now;

            _unitOfWork.ExamRegistrations.Update(registration);
            await _unitOfWork.SaveAsync();

            return (0, "");
        }

        public async Task<(int statusCode, string errorMessage)> AddAllGradesAsync(int examId, BulkExamGradeDto dto)
        {
            if (!await _unitOfWork.Exams.Query().AnyAsync(e => e.Id == examId))
            {
                return (1, $"Exam with id {examId} was not found.");
            }

            if (dto.Grades.Count == 0)
            {
                return (3, "At least one grade must be given.");
            }

            if (dto.Grades.Select(g => g.UserId).Distinct().Count() != dto.Grades.Count)
            {
                return (3, "Grade list contains duplicate users.");
            }

            foreach (var gradeItem in dto.Grades)
            {
                if (!IsValidGrade(gradeItem.Grade))
                {
                    return (3, "Every grade must be between 1 and 5.");
                }

                var registration = await GetRegistration(examId, gradeItem.UserId);
                if (registration is null)
                {
                    return (1, $"Exam registration was not found for user {gradeItem.UserId}.");
                }

                if (registration.Grade is not null)
                {
                    return (2, $"Exam grade is already written for user {gradeItem.UserId}.");
                }

                registration.Grade = gradeItem.Grade;
                registration.GradeDate = DateTime.Now;
                _unitOfWork.ExamRegistrations.Update(registration);
            }

            await _unitOfWork.SaveAsync();
            return (0, "");
        }

        public async Task<(int statusCode, string errorMessage, IEnumerable<UserGradeResultDto>? grades)> GetUserGradesAsync(int userId, UserGradesQueryDto query)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user is null)
            {
                return (1, $"User with id {userId} was not found.", null);
            }

            if (user.UserType != UserType.Student)
            {
                return (3, "User must be a student.", null);
            }

            var semester = await _unitOfWork.Semesters.GetByIdAsync(query.SemesterId);
            if (semester is null)
            {
                return (1, $"Semester with id {query.SemesterId} was not found.", null);
            }

            var enrolledSubjects = await _unitOfWork.Enrollments.Query()
                .Include(e => e.Course)
                    .ThenInclude(c => c.Subject)
                .Where(e => e.StudentId == userId && e.Course.SemesterId == query.SemesterId)
                .Select(e => e.Course.Subject)
                .Distinct()
                .ToListAsync();

            var results = new List<UserGradeResultDto>();
            foreach (var subject in enrolledSubjects)
            {
                var registrations = await _unitOfWork.ExamRegistrations.Query()
                    .Include(er => er.Exam)
                    .Where(er => er.UserId == userId &&
                                 er.Exam.SubjectId == subject.Id &&
                                 er.Exam.SemesterId == query.SemesterId)
                    .ToListAsync();

                if (registrations.Count == 0)
                {
                    results.Add(CreateUserGradeResult(subject, semester, "Nincs jegy.", null, null));
                    continue;
                }

                if (registrations.Any(r => r.Grade is null))
                {
                    results.Add(CreateUserGradeResult(subject, semester, "Folyamatban.", null, null));
                    continue;
                }

                var latestGrade = registrations
                    .Where(r => r.Grade is not null)
                    .OrderByDescending(r => r.GradeDate)
                    .First();

                results.Add(CreateUserGradeResult(subject, semester, latestGrade.Grade!.Value.ToString(), latestGrade.Grade, latestGrade.GradeDate));
            }

            return (0, "", results);
        }

        private async Task<Exam?> GetExamWithDetails(int examId)
        {
            return await _unitOfWork.Exams.Query()
                .Include(e => e.Subject)
                .Include(e => e.Semester)
                .Include(e => e.ExamRegistrations)
                .FirstOrDefaultAsync(e => e.Id == examId);
        }

        private async Task<ExamRegistration?> GetRegistration(int examId, int userId)
        {
            return await _unitOfWork.ExamRegistrations.Query()
                .FirstOrDefaultAsync(er => er.ExamId == examId && er.UserId == userId);
        }

        private async Task<bool> StudentHasSubjectInSemester(int userId, int subjectId, int semesterId)
        {
            return await _unitOfWork.Enrollments.Query()
                .Include(e => e.Course)
                .AnyAsync(e => e.StudentId == userId &&
                               e.Course.SubjectId == subjectId &&
                               e.Course.SemesterId == semesterId);
        }

        private static bool IsValidGrade(int grade)
        {
            return grade >= 1 && grade <= 5;
        }

        private static ExamViewDto MapToViewDto(Exam exam)
        {
            return new ExamViewDto
            {
                Id = exam.Id,
                SubjectId = exam.SubjectId,
                SubjectCode = exam.Subject.Code,
                SubjectName = exam.Subject.Name,
                SemesterId = exam.SemesterId,
                Semester = $"{exam.Semester.AcademicYear}/{exam.Semester.SemesterNumber}",
                ExamTime = exam.ExamTime,
                RegistrationCount = exam.ExamRegistrations.Count
            };
        }

        private static UserGradeResultDto CreateUserGradeResult(Subject subject, Semester semester, string result, int? grade, DateTime? gradeDate)
        {
            return new UserGradeResultDto
            {
                SubjectId = subject.Id,
                SubjectCode = subject.Code,
                SubjectName = subject.Name,
                SemesterId = semester.Id,
                Semester = $"{semester.AcademicYear}/{semester.SemesterNumber}",
                Result = result,
                Grade = grade,
                GradeDate = gradeDate
            };
        }
    }
}
