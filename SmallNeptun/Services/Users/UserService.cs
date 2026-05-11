using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmallNeptun.Dtos.Users;
using SmallNeptun.Entities;
using SmallNeptun.Enums.UserEnums;
using SmallNeptun.Repository;

namespace SmallNeptun.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<(int statusCode, string errorMessage, UserViewDto? user)> RegisterAsync(RegisterUserDto dto)
        {
            if (await _unitOfWork.Users.Query().AnyAsync(u => u.Email == dto.Email))
            {
                return (2, "Email address is already used.", null);
            }

            if (dto.UserType == UserType.Student && dto.StudyForm == StudyForm.None)
            {
                return (3, "Student must have FullTime or PartTime study form.", null);
            }

            if (dto.UserType != UserType.Student && dto.StudyForm != StudyForm.None)
            {
                return (3, "Only students can have a study form.", null);
            }

            var user = _mapper.Map<User>(dto);
            user.IsActive = true;

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveAsync();

            return (0, "", _mapper.Map<UserViewDto>(user));
        }

        public async Task<(int statusCode, string errorMessage, UserViewDto? user)> GetByIdAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user is null)
            {
                return (1, $"User with id {userId} was not found.", null);
            }

            return (0, "", _mapper.Map<UserViewDto>(user));
        }

        public async Task<IEnumerable<UserViewDto>> GetStudentsAsync()
        {
            var users = await _unitOfWork.Users.Query()
                .Where(u => u.UserType == UserType.Student)
                .ToListAsync();

            return _mapper.Map<IEnumerable<UserViewDto>>(users);
        }

        public async Task<IEnumerable<UserViewDto>> GetTeachersAsync()
        {
            var users = await _unitOfWork.Users.Query()
                .Where(u => u.UserType == UserType.Teacher)
                .ToListAsync();

            return _mapper.Map<IEnumerable<UserViewDto>>(users);
        }

        public async Task<IEnumerable<UserViewDto>> GetAgentsAsync()
        {
            var users = await _unitOfWork.Users.Query()
                .Where(u => u.UserType == UserType.Agent)
                .ToListAsync();

            return _mapper.Map<IEnumerable<UserViewDto>>(users);
        }

        public async Task<(int statusCode, string errorMessage, UserViewDto? user)> UpdateAsync(int userId, UpdateUserDto dto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user is null)
            {
                return (1, $"User with id {userId} was not found.", null);
            }

            if (await _unitOfWork.Users.Query().AnyAsync(u => u.Email == dto.Email && u.Id != userId))
            {
                return (2, "Email address is already used.", null);
            }

            if (user.UserType == UserType.Student && dto.StudyForm == StudyForm.None)
            {
                return (3, "Student must have FullTime or PartTime study form.", null);
            }

            if (user.UserType != UserType.Student && dto.StudyForm != StudyForm.None)
            {
                return (3, "Only students can have a study form.", null);
            }

            user.Name = dto.Name;
            user.Email = dto.Email;
            user.StudyForm = dto.StudyForm;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveAsync();

            return (0, "", _mapper.Map<UserViewDto>(user));
        }

        public async Task<(int statusCode, string errorMessage)> ChangePasswordAsync(int userId, ChangePasswordDto dto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user is null)
            {
                return (1, $"User with id {userId} was not found.");
            }

            user.Password = dto.NewPassword;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveAsync();

            return (0, "");
        }

        public async Task<(int statusCode, string errorMessage)> DeactivateAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user is null)
            {
                return (1, $"User with id {userId} was not found.");
            }

            user.IsActive = false;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveAsync();

            return (0, "");
        }

        public async Task<(int statusCode, string errorMessage)> ReactivateAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user is null)
            {
                return (1, $"User with id {userId} was not found.");
            }

            user.IsActive = true;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveAsync();

            return (0, "");
        }
    }
}
