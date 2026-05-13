using Microsoft.AspNetCore.Mvc;
using SmallNeptun.Dtos.Exams;
using SmallNeptun.Dtos.Users;
using SmallNeptun.Services.Exams;
using SmallNeptun.Services.Users;

namespace SmallNeptun.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IExamService _examService;

        public UsersController(IUserService userService, IExamService examService)
        {
            _userService = userService;
            _examService = examService;
        }

        /// <summary>
        /// Uj felhasznalo letrehozasa.
        /// </summary>
        /// <remarks>
        /// Enum ertekeket pontosan igy kell megadni:
        /// UserType: Student, Teacher, Agent
        /// StudyForm: None, FullTime, PartTime
        ///
        /// Student eseten StudyForm csak FullTime vagy PartTime lehet.
        /// Teacher/Agent eseten StudyForm legyen None.
        ///
        /// Pelda student:
        /// {
        ///   "name": "Teszt Elek",
        ///   "email": "teszt.elek@smallneptun.hu",
        ///   "password": "Pass1234",
        ///   "userType": "Student",
        ///   "studyForm": "FullTime"
        /// }
        ///
        /// Pelda teacher:
        /// {
        ///   "name": "Dr. Uj Oktato",
        ///   "email": "uj.oktato@smallneptun.hu",
        ///   "password": "Pass1234",
        ///   "userType": "Teacher",
        ///   "studyForm": "None"
        /// }
        /// </remarks>
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

        /// <summary>
        /// Felhasznalo adatainak modositasa.
        /// </summary>
        /// <remarks>
        /// Csak a nev, email es StudyForm modosul.
        /// UserType nem modosithato, jelszo kulon endpointon van.
        ///
        /// StudyForm ertekek: None, FullTime, PartTime.
        /// Pelda:
        /// {
        ///   "name": "Uj Nev",
        ///   "email": "uj.email@smallneptun.hu",
        ///   "studyForm": "FullTime"
        /// }
        /// </remarks>
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

        /// <summary>
        /// Felhasznalo jelszavanak modositasa.
        /// </summary>
        /// <remarks>
        /// Pelda:
        /// {
        ///   "newPassword": "UjJelszo123"
        /// }
        /// </remarks>
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

        [HttpGet("{userId}/grades")]
        public async Task<IActionResult> GetGrades(int userId, [FromQuery] UserGradesQueryDto query)
        {
            var result = await _examService.GetUserGradesAsync(userId, query);

            if (result.statusCode == 1)
            {
                return NotFound(result.errorMessage);
            }

            if (result.statusCode == 3)
            {
                return BadRequest(result.errorMessage);
            }

            return Ok(result.grades);
        }
    }
}
