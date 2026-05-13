using Microsoft.AspNetCore.Mvc;
using SmallNeptun.Dtos.Enrollments;
using SmallNeptun.Dtos.Exams;
using SmallNeptun.Dtos.Subjects;
using SmallNeptun.Services.Enrollments;
using SmallNeptun.Services.Exams;
using SmallNeptun.Services.Subjects;

namespace SmallNeptun.Controllers
{
    [ApiController]
    [Route("api/subjects")]
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectService _subjectService;
        private readonly IEnrollmentService _enrollmentService;
        private readonly IExamService _examService;

        public SubjectsController(
            ISubjectService subjectService,
            IEnrollmentService enrollmentService,
            IExamService examService)
        {
            _subjectService = subjectService;
            _enrollmentService = enrollmentService;
            _examService = examService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] SubjectQueryDto query)
        {
            return Ok(await _subjectService.GetAllAsync(query));
        }

        [HttpGet("{subjectId}")]
        public async Task<IActionResult> GetById(int subjectId)
        {
            var result = await _subjectService.GetByIdAsync(subjectId);

            if (result.statusCode == 1)
            {
                return NotFound(result.errorMessage);
            }

            return Ok(result.subject);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSubjectDto dto)
        {
            var result = await _subjectService.CreateAsync(dto);

            if (result.statusCode == 2)
            {
                return Conflict(result.errorMessage);
            }

            if (result.statusCode == 3)
            {
                return BadRequest(result.errorMessage);
            }

            return CreatedAtAction(nameof(GetById), new { subjectId = result.subject!.Id }, result.subject);
        }

        [HttpPut("{subjectId}")]
        public async Task<IActionResult> Update(int subjectId, UpdateSubjectDto dto)
        {
            var result = await _subjectService.UpdateAsync(subjectId, dto);

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

            return Ok(result.subject);
        }

        [HttpPost("{subjectId}/deactivate")]
        public async Task<IActionResult> Deactivate(int subjectId)
        {
            var result = await _subjectService.DeactivateAsync(subjectId);

            if (result.statusCode == 1)
            {
                return NotFound(result.errorMessage);
            }

            return NoContent();
        }

        [HttpPost("{subjectId}/reactivate")]
        public async Task<IActionResult> Reactivate(int subjectId)
        {
            var result = await _subjectService.ReactivateAsync(subjectId);

            if (result.statusCode == 1)
            {
                return NotFound(result.errorMessage);
            }

            return NoContent();
        }

        [HttpPost("{subjectId}/register")]
        public async Task<IActionResult> RegisterToSubject(int subjectId, RegisterSubjectDto dto)
        {
            var result = await _enrollmentService.RegisterToSubjectAsync(subjectId, dto);
            return EnrollmentResult(result);
        }

        [HttpPost("{subjectId}/unregister")]
        public async Task<IActionResult> UnregisterFromSubject(int subjectId, UnregisterSubjectDto dto)
        {
            var result = await _enrollmentService
                .UnregisterFromSubjectAsync(subjectId, dto);
            return EnrollmentResult(result);
        }

        [HttpGet("{subjectId}/students")]
        public async Task<IActionResult> GetSubjectStudents(int subjectId, [FromQuery] SubjectStudentsQueryDto query)
        {
            var result = await _enrollmentService
                .GetSubjectStudentsAsync(subjectId, query);

            if (result.statusCode == 1)
            {
                return NotFound(result.errorMessage);
            }

            return Ok(result.students);
        }

        [HttpGet("{subjectId}/exams")]
        public async Task<IActionResult> GetSubjectExams(int subjectId, [FromQuery] SubjectExamsQueryDto query)
        {
            var result = await _examService.GetSubjectExamsAsync(subjectId, query);

            if (result.statusCode == 1)
            {
                return NotFound(result.errorMessage);
            }

            return Ok(result.exams);
        }

        private IActionResult EnrollmentResult((int statusCode, string errorMessage) result)
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
