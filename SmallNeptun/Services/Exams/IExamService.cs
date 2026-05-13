using SmallNeptun.Dtos.Exams;

namespace SmallNeptun.Services.Exams
{
    public interface IExamService
    {
        Task<(int statusCode, string errorMessage, ExamViewDto? exam)> CreateAsync(CreateExamDto dto);
        Task<IEnumerable<ExamViewDto>> GetAllAsync();
        Task<(int statusCode, string errorMessage, IEnumerable<ExamViewDto>? exams)> GetSubjectExamsAsync(int subjectId, SubjectExamsQueryDto query);
        Task<(int statusCode, string errorMessage)> DeleteAsync(int examId);
        Task<(int statusCode, string errorMessage)> RegisterAsync(int examId, ExamRegistrationDto dto);
        Task<(int statusCode, string errorMessage)> AddGradeAsync(int examId, int userId, ExamGradeDto dto);
        Task<(int statusCode, string errorMessage)> UpdateGradeAsync(int examId, int userId, ExamGradeDto dto);
        Task<(int statusCode, string errorMessage)> AddAllGradesAsync(int examId, BulkExamGradeDto dto);
        Task<(int statusCode, string errorMessage, IEnumerable<UserGradeResultDto>? grades)> GetUserGradesAsync(int userId, UserGradesQueryDto query);
    }
}
