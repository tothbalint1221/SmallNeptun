using SmallNeptun.Dtos.Users;

namespace SmallNeptun.Services.Users
{
    public interface IUserService
    {
        Task<(int statusCode, string errorMessage, UserViewDto? user)> RegisterAsync(RegisterUserDto dto);
        Task<(int statusCode, string errorMessage, UserViewDto? user)> GetByIdAsync(int userId);
        Task<IEnumerable<UserViewDto>> GetStudentsAsync();
        Task<IEnumerable<UserViewDto>> GetTeachersAsync();
        Task<IEnumerable<UserViewDto>> GetAgentsAsync();
        Task<(int statusCode, string errorMessage, UserViewDto? user)> UpdateAsync(int userId, UpdateUserDto dto);
        Task<(int statusCode, string errorMessage)> ChangePasswordAsync(int userId, ChangePasswordDto dto);
        Task<(int statusCode, string errorMessage)> DeactivateAsync(int userId);
        Task<(int statusCode, string errorMessage)> ReactivateAsync(int userId);
    }
}
