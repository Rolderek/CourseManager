using CourseManager.DTOs.Subjects;
using CourseManager.Services;
using Microsoft.AspNetCore.Mvc;
using CourseManager.DTOs.Enrollments;

namespace CourseManager.Controllers
{
    [ApiController]
    [Route("api/subjects")]
    public class SubjectsController : ControllerBase
    {
        private readonly SubjectService _subjectService;

        public SubjectsController(SubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        /// <summary>Get all subjects</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _subjectService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>Get a subject by ID</summary>
        [HttpGet("{subjectId}")]
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
        [HttpPost]
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
        [HttpPut("{subjectId}")]
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
        [HttpPost("{subjectId}/deactivate")]
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

        /*
         * amik még hiányoznak:
            • Tárgyra feliratkozás (POST /api/subjects/{subjectId}/register)
            o Input: { studentId, courseIds }
            o Validálás: A fent leírtak szerint
            • Tárgyról lejelentkezés (POST /api/subjects/{subjectId}/unregister)
            o Input: { studentId, semester }
        */



    }
}