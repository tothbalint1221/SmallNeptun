using Microsoft.AspNetCore.Mvc;
using SmallNeptun.Dtos.Courses;
using SmallNeptun.Services.Courses;

namespace SmallNeptun.Controllers
{
    [ApiController]
    [Route("api/courses")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCourseDto dto)
        {
            var result = await _courseService.CreateAsync(dto);

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

            return CreatedAtAction(nameof(GetById), new { courseId = result.course!.Id }, result.course);
        }

        [HttpGet("{courseId}")]
        public async Task<IActionResult> GetById(int courseId)
        {
            var result = await _courseService.GetByIdAsync(courseId);

            if (result.statusCode == 1)
            {
                return NotFound(result.errorMessage);
            }

            return Ok(result.course);
        }

        [HttpPut("{courseId}")]
        public async Task<IActionResult> Update(int courseId, UpdateCourseDto dto)
        {
            var result = await _courseService.UpdateAsync(courseId, dto);

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

            return Ok(result.course);
        }

        [HttpDelete("{courseId}")]
        public async Task<IActionResult> Delete(int courseId)
        {
            var result = await _courseService.DeleteAsync(courseId);

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
    }
}
