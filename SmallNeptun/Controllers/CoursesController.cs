using Microsoft.AspNetCore.Mvc;
using SmallNeptun.Dtos.Courses;
using SmallNeptun.Dtos.Enrollments;
using SmallNeptun.Services.Courses;
using SmallNeptun.Services.Enrollments;

namespace SmallNeptun.Controllers
{
    [ApiController]
    [Route("api/courses")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly IEnrollmentService _enrollmentService;

        public CoursesController(ICourseService courseService, IEnrollmentService enrollmentService)
        {
            _courseService = courseService;
            _enrollmentService = enrollmentService;
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

        [HttpPost("change")]
        public async Task<IActionResult> ChangeCourse(ChangeCourseDto dto)
        {
            var result = await _enrollmentService.ChangeCourseAsync(dto);

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

        [HttpGet("{courseId}/students")]
        public async Task<IActionResult> GetCourseStudents(int courseId)
        {
            var result = await _enrollmentService.GetCourseStudentsAsync(courseId);

            if (result.statusCode == 1)
            {
                return NotFound(result.errorMessage);
            }

            return Ok(result.students);
        }
    }
}
