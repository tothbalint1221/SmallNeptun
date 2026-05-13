using Microsoft.AspNetCore.Mvc;
using SmallNeptun.Dtos.Exams;
using SmallNeptun.Services.Exams;

namespace SmallNeptun.Controllers
{
    [ApiController]
    [Route("api/exams")]
    public class ExamsController : ControllerBase
    {
        private readonly IExamService _examService;

        public ExamsController(IExamService examService)
        {
            _examService = examService;
        }

        /// <summary>
        /// Uj vizsga letrehozasa.
        /// </summary>
        /// <remarks>
        /// Pelda:
        /// {
        ///   "subjectId": 1,
        ///   "semesterId": 1,
        ///   "examTime": "2026-06-10T10:00:00"
        /// }
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> Create(CreateExamDto dto)
        {
            var result = await _examService.CreateAsync(dto);

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

            return CreatedAtAction(nameof(GetAll), new { examId = result.exam!.Id }, result.exam);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _examService.GetAllAsync());
        }

        [HttpDelete("{examId}")]
        public async Task<IActionResult> Delete(int examId)
        {
            var result = await _examService.DeleteAsync(examId);
            return SimpleResult(result);
        }

        /// <summary>
        /// Hallgato jelentkezese vizsgara.
        /// </summary>
        /// <remarks>
        /// Pelda:
        /// {
        ///   "userId": 12
        /// }
        /// </remarks>
        [HttpPost("{examId}/registration")]
        public async Task<IActionResult> Register(int examId, ExamRegistrationDto dto)
        {
            var result = await _examService.RegisterAsync(examId, dto);
            return SimpleResult(result);
        }

        /// <summary>
        /// Vizsgajegy beirasa.
        /// </summary>
        /// <remarks>
        /// Grade erteke csak 1, 2, 3, 4 vagy 5 lehet.
        ///
        /// Pelda:
        /// {
        ///   "grade": 5
        /// }
        /// </remarks>
        [HttpPost("{examId}/grades/{userId}")]
        public async Task<IActionResult> AddGrade(int examId, int userId, ExamGradeDto dto)
        {
            var result = await _examService.AddGradeAsync(examId, userId, dto);
            return SimpleResult(result);
        }

        /// <summary>
        /// Vizsgajegy modositasa.
        /// </summary>
        /// <remarks>
        /// Grade erteke csak 1, 2, 3, 4 vagy 5 lehet.
        ///
        /// Pelda:
        /// {
        ///   "grade": 4
        /// }
        /// </remarks>
        [HttpPut("{examId}/grades/{userId}")]
        public async Task<IActionResult> UpdateGrade(int examId, int userId, ExamGradeDto dto)
        {
            var result = await _examService.UpdateGradeAsync(examId, userId, dto);
            return SimpleResult(result);
        }

        /// <summary>
        /// Vizsgajegyek tombositett beirasa.
        /// </summary>
        /// <remarks>
        /// Pelda:
        /// {
        ///   "grades": [
        ///     { "userId": 12, "grade": 5 },
        ///     { "userId": 13, "grade": 3 }
        ///   ]
        /// }
        /// </remarks>
        [HttpPost("{examId}/allgrades")]
        public async Task<IActionResult> AddAllGrades(int examId, BulkExamGradeDto dto)
        {
            var result = await _examService.AddAllGradesAsync(examId, dto);
            return SimpleResult(result);
        }

        private IActionResult SimpleResult((int statusCode, string errorMessage) result)
        {
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

            return NoContent();
        }
    }
}
