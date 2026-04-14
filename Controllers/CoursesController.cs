using CourseManager.DTOs.Courses;
using CourseManager.Services;
using Microsoft.AspNetCore.Mvc;
using CourseManager.DTOs.Enrollments;

namespace CourseManager.Controllers
{
    [ApiController]
    [Route("api/courses")]
    public class CoursesController : ControllerBase
    {
        private readonly CourseService _courseService;

        public CoursesController(CourseService courseService)
        {
            _courseService = courseService;
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
    }
}