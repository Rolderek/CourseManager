using CourseManager.DTOs.Enrollments;
using CourseManager.DTOs.Subjects;
using CourseManager.Services;
using Microsoft.AspNetCore.Mvc;

namespace CourseManager.Controllers
{
    /// <summary>
    /// Manages university subjects and student enrollments.
    /// Subjects are never deleted — only deactivated.
    /// </summary>
    [ApiController]
    [Route("api/subjects")]
    [Produces("application/json")]
    public class SubjectsController : ControllerBase
    {
        private readonly SubjectService _subjectService;
        private readonly EnrollmentService _enrollmentService;

        public SubjectsController(SubjectService subjectService, EnrollmentService enrollmentService)
        {
            _subjectService = subjectService;
            _enrollmentService = enrollmentService;
        }

        /// <summary>Get all subjects</summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<SubjectResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _subjectService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>Get a subject by ID</summary>
        [HttpGet("{subjectId}")]
        [ProducesResponseType(typeof(SubjectResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int subjectId)
        {
            try
            {
                var result = await _subjectService.GetByIdAsync(subjectId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        /// <summary>Create a new subject</summary>
        /// <remarks>
        /// Example request:
        /// ```json
        /// {
        ///   "code": "PROG301",
        ///   "name": "Software Engineering",
        ///   "credits": 4
        /// }
        /// ```
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(SubjectResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateSubjectDto dto)
        {
            try
            {
                var result = await _subjectService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { subjectId = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>Update a subject's name and credits</summary>
        /// <remarks>
        /// Note: subject code cannot be changed after creation.
        /// </remarks>
        [HttpPut("{subjectId}")]
        [ProducesResponseType(typeof(SubjectResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int subjectId, [FromBody] UpdateSubjectDto dto)
        {
            try
            {
                var result = await _subjectService.UpdateAsync(subjectId, dto);
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

        /// <summary>Deactivate a subject</summary>
        /// <remarks>
        /// Deactivates a subject without deleting it.
        /// Inactive subjects cannot have new courses created for them.
        /// </remarks>
        [HttpPost("{subjectId}/deactivate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Deactivate(int subjectId)
        {
            try
            {
                await _subjectService.DeactivateAsync(subjectId);
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

        /// <summary>Reactivate a subject</summary>
        [HttpPost("{subjectId}/reactivate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Reactivate(int subjectId)
        {
            try
            {
                await _subjectService.ReactivateAsync(subjectId);
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

        /// <summary>Enroll a student in a subject</summary>
        /// <remarks>
        /// Enrolls a student in a subject by selecting one course per available course type.
        ///
        /// **Rules:**
        /// - Must select exactly one course per type (Lecture, Practice, Lab)
        /// - Full-time students can only enroll in FullTime or Mixed courses
        /// - Part-time students can only enroll in PartTime or Mixed courses
        /// - All selected courses must be from the same semester
        /// - Courses must not be full
        ///
        /// **Example request:**
        /// ```json
        /// {
        ///   "studentId": 5,
        ///   "courseIds": [1, 4]
        /// }
        /// ```
        /// </remarks>
        [HttpPost("{subjectId}/register")]
        [ProducesResponseType(typeof(List<EnrollmentResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Enroll(int subjectId, [FromBody] EnrollRequestDto dto)
        {
            try
            {
                var result = await _enrollmentService.EnrollAsync(subjectId, dto);
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

        /// <summary>Unregister a student from a subject</summary>
        /// <remarks>
        /// Removes all of a student's enrollments for a subject in a given semester.
        ///
        /// **Example request:**
        /// ```json
        /// {
        ///   "studentId": 5,
        ///   "semester": "2024-25-2"
        /// }
        /// ```
        /// </remarks>
        [HttpPost("{subjectId}/unregister")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Unregister(int subjectId, [FromBody] UnregisterRequestDto dto)
        {
            try
            {
                await _enrollmentService.UnregisterAsync(subjectId, dto);
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

        /// <summary>List all students enrolled in a subject for a given semester</summary>
        /// <remarks>
        /// Example: GET /api/subjects/1/students?semester=2024-25-2
        /// </remarks>
        [HttpGet("{subjectId}/students")]
        [ProducesResponseType(typeof(List<EnrollmentResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSubjectStudents(int subjectId, [FromQuery] string semester)
        {
            try
            {
                var result = await _enrollmentService.GetSubjectStudentsAsync(subjectId, semester);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }
    }
}