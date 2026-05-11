using Microsoft.AspNetCore.Mvc;
using SmallNeptun.Dtos.Courses;
using SmallNeptun.Dtos.Enrollments;
using SmallNeptun.Dtos.Schedules;
using SmallNeptun.Services.Courses;
using SmallNeptun.Services.Enrollments;
using SmallNeptun.Services.Schedules;

namespace SmallNeptun.Controllers
{
    [ApiController]
    [Route("api/courses")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly IEnrollmentService _enrollmentService;
        private readonly IScheduleService _scheduleService;

        public CoursesController(
            ICourseService courseService,
            IEnrollmentService enrollmentService,
            IScheduleService scheduleService)
        {
            _courseService = courseService;
            _enrollmentService = enrollmentService;
            _scheduleService = scheduleService;
        }

        /// <summary>
        /// Uj kurzus hirdetese.
        /// </summary>
        /// <remarks>
        /// Enum ertekeket pontosan igy kell megadni:
        /// CourseType: Lecture, Practice, Lab
        /// CourseForm: FullTime, PartTime, Both
        /// HourType: Weekly, Semester
        ///
        /// Pelda:
        /// {
        ///   "courseCode": "PTI001-E",
        ///   "subjectCode": "PTI001",
        ///   "semesterId": 1,
        ///   "maxStudents": 80,
        ///   "courseType": "Lecture",
        ///   "courseForm": "Both",
        ///   "hourType": "Weekly",
        ///   "hourCount": 2,
        ///   "teacherIds": [3, 4]
        /// }
        /// </remarks>
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

        /// <summary>
        /// Kurzus adatainak modositasa.
        /// </summary>
        /// <remarks>
        /// Enum ertekeket pontosan igy kell megadni:
        /// CourseType: Lecture, Practice, Lab
        /// CourseForm: FullTime, PartTime, Both
        /// HourType: Weekly, Semester
        ///
        /// Pelda:
        /// {
        ///   "courseCode": "PTI001-GY1",
        ///   "semesterId": 1,
        ///   "maxStudents": 30,
        ///   "courseType": "Practice",
        ///   "courseForm": "FullTime",
        ///   "hourType": "Weekly",
        ///   "hourCount": 2
        /// }
        /// </remarks>
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

        /// <summary>
        /// Orarendi idopontok megadasa.
        /// </summary>
        /// <remarks>
        /// DayOfWeek ertekek: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday.
        ///
        /// Pelda heti orara:
        /// {
        ///   "schedules": [
        ///     {
        ///       "isWeekly": true,
        ///       "dayOfWeek": "Monday",
        ///       "startTime": "09:00:00",
        ///       "endTime": "10:30:00",
        ///       "specificDate": null
        ///     }
        ///   ]
        /// }
        ///
        /// Pelda tombositett orara:
        /// {
        ///   "schedules": [
        ///     {
        ///       "isWeekly": false,
        ///       "dayOfWeek": null,
        ///       "startTime": "09:00:00",
        ///       "endTime": "12:00:00",
        ///       "specificDate": "2026-05-20"
        ///     }
        ///   ]
        /// }
        /// </remarks>
        [HttpPost("{courseId}/schedule")]
        public async Task<IActionResult> CreateSchedule(int courseId, CreateScheduleDto dto)
        {
            var result = await _scheduleService.CreateAsync(courseId, dto);

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

            return Ok(result.schedules);
        }

        /// <summary>
        /// Orarendi idopontok modositasa.
        /// </summary>
        /// <remarks>
        /// A modositas lecsereli a kurzus teljes orarendjet az itt megadott listara.
        /// DayOfWeek ertekek: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday.
        ///
        /// Pelda:
        /// {
        ///   "schedules": [
        ///     {
        ///       "isWeekly": true,
        ///       "dayOfWeek": "Tuesday",
        ///       "startTime": "13:00:00",
        ///       "endTime": "14:30:00",
        ///       "specificDate": null
        ///     }
        ///   ]
        /// }
        /// </remarks>
        [HttpPost("{courseId}/schedule/modify")]
        public async Task<IActionResult> ModifySchedule(int courseId, ModifyScheduleDto dto)
        {
            var result = await _scheduleService.ModifyAsync(courseId, dto);

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

            return Ok(result.schedules);
        }
    }
}
