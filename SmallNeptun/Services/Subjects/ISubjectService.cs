using SmallNeptun.Dtos.Subjects;

namespace SmallNeptun.Services.Subjects
{
    public interface ISubjectService
    {
        Task<IEnumerable<SubjectViewDto>> GetAllAsync(SubjectQueryDto query);
        Task<(int statusCode, string errorMessage, SubjectViewDto? subject)> GetByIdAsync(int subjectId);
        Task<(int statusCode, string errorMessage, SubjectViewDto? subject)> CreateAsync(CreateSubjectDto dto);
        Task<(int statusCode, string errorMessage, SubjectViewDto? subject)> UpdateAsync(int subjectId, UpdateSubjectDto dto);
        Task<(int statusCode, string errorMessage)> DeactivateAsync(int subjectId);
        Task<(int statusCode, string errorMessage)> ReactivateAsync(int subjectId);
    }
}
