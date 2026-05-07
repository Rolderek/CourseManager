using CourseManager.DTOs.Courses;
using CourseManager.DTOs.Enrollments;
using CourseManager.DTOs.Schedule;
using CourseManager.Services;
using Microsoft.AspNetCore.Mvc;

//ez látszik?
//THERE WAS TWO CONSTRUCTOR, THIS WAS REPAIRED :D
namespace CourseManager.Controllers
{
    [ApiController]
    [Route("api/courses")]
    public class CoursesController : ControllerBase
    {
        private readonly CourseService _courseService;
        private readonly EnrollmentService _enrollmentService;
        private readonly ScheduleService _scheduleService;

        public CoursesController(CourseService courseService, EnrollmentService enrollmentService, ScheduleService scheduleService)
        {
            _courseService = courseService;
            _enrollmentService = enrollmentService;
            _scheduleService = scheduleService;
        }

        /// <summary>Create a new course</summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCourseDto dto)
        {
            try
            {
                var result = await _courseService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { courseId = result.Id }, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>Get a course by ID</summary>
        [HttpGet("{courseId}")]
        public async Task<IActionResult> GetById(int courseId)
        {
            try
            {
                var result = await _courseService.GetByIdAsync(courseId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        /// <summary>Update a course</summary>
        [HttpPut("{courseId}")]
        public async Task<IActionResult> Update(int courseId, [FromBody] UpdateCourseDto dto)
        {
            try
            {
                var result = await _courseService.UpdateAsync(courseId, dto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>Delete a course (only if no students enrolled)</summary>
        [HttpDelete("{courseId}")]
        public async Task<IActionResult> Delete(int courseId)
        {
            try
            {
                await _courseService.DeleteAsync(courseId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>Change a student from one course to another</summary>
        [HttpPost("change")]
        public async Task<IActionResult> ChangeCourse([FromBody] ChangeCourseDto dto)
        {
            try
            {
                var result = await _enrollmentService.ChangeCourseAsync(dto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>List all students enrolled in a course</summary>
        [HttpGet("{courseId}/students")]
        public async Task<IActionResult> GetCourseStudents(int courseId)
        {
            try
            {
                var result = await _enrollmentService.GetCourseStudentsAsync(courseId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Add schedule entries to a course.
        /// For WEEKLY courses: provide exactly one entry (repeats every week for 14 weeks).
        /// For BLOCK courses: provide one entry per session.
        /// </summary>
        [HttpPost("{courseId}/schedule")]
        public async Task<IActionResult> AddSchedule(int courseId, [FromBody] AddScheduleDto dto)
        {
            try
            {
                var result = await _scheduleService.AddScheduleAsync(courseId, dto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Modify schedule entries for a course (full replacement).
        /// For WEEKLY courses: provide exactly one entry.
        /// For BLOCK courses: provide all sessions again.
        /// </summary>
        [HttpPost("{courseId}/schedule/modify")]
        public async Task<IActionResult> ModifySchedule(int courseId, [FromBody] ModifyScheduleDto dto)
        {
            try
            {
                var result = await _scheduleService.ModifyScheduleAsync(courseId, dto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>Get the schedule for a course</summary>
        [HttpGet("{courseId}/schedule")]
        public async Task<IActionResult> GetSchedule(int courseId)
        {
            try
            {
                var result = await _scheduleService.GetScheduleAsync(courseId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        /// <summary>Register a student directly to a course</summary>
        /// <remarks>
        /// Directly registers a student to a specific course.
        /// 
        /// **Rules checked:**
        /// - Student must be active
        /// - Student must not already be enrolled in this course
        /// - Course must not be full
        /// - Study mode must be compatible (full-time/part-time/mixed)
        ///
        /// **Example request:**
        /// ```json
        /// {
        ///   "studentId": 5
        /// }
        /// ```
        /// </remarks>
        /// <param name="courseId">The unique ID of the course</param>
        /// <param name="dto">Student ID to register</param>
        /// <response code="200">Student successfully registered</response>
        /// <response code="400">Validation error (e.g. course full, study mode mismatch)</response>
        /// <response code="404">Course or student not found</response>
        [HttpPost("{courseId}/register")]
        [ProducesResponseType(typeof(EnrollmentResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RegisterToCourse(int courseId, [FromBody] CourseRegistrationDto dto)
        {
            try
            {
                var result = await _enrollmentService.RegisterToCourseAsync(courseId, dto.StudentId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>Unregister a student from a course</summary>
        /// <remarks>
        /// Directly removes a student's enrollment from a specific course.
        ///
        /// **Example request:**
        /// ```json
        /// {
        ///   "studentId": 5
        /// }
        /// ```
        /// </remarks>
        /// <param name="courseId">The unique ID of the course</param>
        /// <param name="dto">Student ID to unregister</param>
        /// <response code="204">Student successfully unregistered</response>
        /// <response code="400">Student is not enrolled in this course</response>
        /// <response code="404">Course or student not found</response>
        [HttpPost("{courseId}/unregister")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UnregisterFromCourse(int courseId, [FromBody] CourseRegistrationDto dto)
        {
            try
            {
                await _enrollmentService.UnregisterFromCourseAsync(courseId, dto.StudentId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}