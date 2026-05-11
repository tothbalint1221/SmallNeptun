using Microsoft.AspNetCore.Mvc;
using SmallNeptun.Dtos.Subjects;
using SmallNeptun.Services.Subjects;

namespace SmallNeptun.Controllers
{
    [ApiController]
    [Route("api/subjects")]
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectsController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
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
    }
}
