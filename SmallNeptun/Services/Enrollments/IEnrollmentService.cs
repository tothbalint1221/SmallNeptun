using SmallNeptun.Dtos.Enrollments;
using SmallNeptun.Dtos.Users;

namespace SmallNeptun.Services.Enrollments
{
    public interface IEnrollmentService
    {
        Task<(int statusCode, string errorMessage)> RegisterToSubjectAsync(int subjectId, RegisterSubjectDto dto);
        Task<(int statusCode, string errorMessage)> UnregisterFromSubjectAsync(int subjectId, UnregisterSubjectDto dto);
        Task<(int statusCode, string errorMessage)> ChangeCourseAsync(ChangeCourseDto dto);
        Task<(int statusCode, string errorMessage, IEnumerable<UserViewDto>? students)> GetCourseStudentsAsync(int courseId);
        Task<(int statusCode, string errorMessage, IEnumerable<UserViewDto>? students)> GetSubjectStudentsAsync(int subjectId, SubjectStudentsQueryDto query);
    }
}
