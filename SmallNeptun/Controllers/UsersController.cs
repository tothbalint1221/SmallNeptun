using Microsoft.AspNetCore.Mvc;
using SmallNeptun.Dtos.Users;
using SmallNeptun.Services.Users;

namespace SmallNeptun.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto dto)
        {
            var result = await _userService.RegisterAsync(dto);

            if (result.statusCode == 2)
            {
                return Conflict(result.errorMessage);
            }

            if (result.statusCode == 3)
            {
                return BadRequest(result.errorMessage);
            }

            return CreatedAtAction(nameof(GetById), new { userId = result.user!.Id }, result.user);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetById(int userId)
        {
            var result = await _userService.GetByIdAsync(userId);

            if (result.statusCode == 1)
            {
                return NotFound(result.errorMessage);
            }

            return Ok(result.user);
        }

        [HttpGet("students")]
        public async Task<IActionResult> GetStudents()
        {
            return Ok(await _userService.GetStudentsAsync());
        }

        [HttpGet("teachers")]
        public async Task<IActionResult> GetTeachers()
        {
            return Ok(await _userService.GetTeachersAsync());
        }

        [HttpGet("agents")]
        public async Task<IActionResult> GetAgents()
        {
            return Ok(await _userService.GetAgentsAsync());
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> Update(int userId, UpdateUserDto dto)
        {
            var result = await _userService.UpdateAsync(userId, dto);

            if (result.statusCode == 1)
            {
                return NotFound(result.errorMessage);
            }

            if (result.statusCode == 2)
            {
                return Conflict(result.errorMessage);
            }

            if (result.statusCode == 3)
            {
                return BadRequest(result.errorMessage);
            }

            return Ok(result.user);
        }

        [HttpPost("{userId}/password")]
        public async Task<IActionResult> ChangePassword(int userId, ChangePasswordDto dto)
        {
            var result = await _userService.ChangePasswordAsync(userId, dto);

            if (result.statusCode == 1)
            {
                return NotFound(result.errorMessage);
            }

            if (result.statusCode == 2)
            {
                return Conflict(result.errorMessage);
            }

            return NoContent();
        }

        [HttpPost("{userId}/deactivate")]
        public async Task<IActionResult> Deactivate(int userId)
        {
            var result = await _userService.DeactivateAsync(userId);

            if (result.statusCode == 1)
            {
                return NotFound(result.errorMessage);
            }

            return NoContent();
        }

        [HttpPost("{userId}/reactivate")]
        public async Task<IActionResult> Reactivate(int userId)
        {
            var result = await _userService.ReactivateAsync(userId);

            if (result.statusCode == 1)
            {
                return NotFound(result.errorMessage);
            }

            return NoContent();
        }
    }
}
