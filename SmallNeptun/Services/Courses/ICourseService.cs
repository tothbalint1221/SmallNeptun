using SmallNeptun.Dtos.Courses;

namespace SmallNeptun.Services.Courses
{
    public interface ICourseService
    {
        Task<(int statusCode, string errorMessage, CourseViewDto? course)> CreateAsync(CreateCourseDto dto);
        Task<(int statusCode, string errorMessage, CourseViewDto? course)> GetByIdAsync(int courseId);
        Task<(int statusCode, string errorMessage, CourseViewDto? course)> UpdateAsync(int courseId, UpdateCourseDto dto);
        Task<(int statusCode, string errorMessage)> DeleteAsync(int courseId);
    }
}
