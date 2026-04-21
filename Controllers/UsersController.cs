using CourseManager.DTOs.Users;
using CourseManager.Services;
using Microsoft.AspNetCore.Mvc;
using CourseManager.DTOs.Enrollments;


//need to check again, I'm not sure this is fine
//ezeket vállalatnál a SERVICES-ben szokták, ha végre ott fogok dolgozni meglátjuk :)
namespace CourseManager.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        /// <summary>Register a new user</summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            try
            {
                var result = await _userService.RegisterAsync(dto);
                return CreatedAtAction(nameof(GetById), new { userId = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>Get a user by ID</summary>
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetById(int userId)
        {
            try
            {
                var result = await _userService.GetByIdAsync(userId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        /// <summary>Update a user's username and email</summary>
        [HttpPut("{userId}")]
        public async Task<IActionResult> Update(int userId, [FromBody] UpdateUserDto dto)
        {
            try
            {
                var result = await _userService.UpdateAsync(userId, dto);
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

        /// <summary>Change a user's password</summary>
        [HttpPut("{userId}/password")]
        public async Task<IActionResult> ChangePassword(int userId, [FromBody] ChangePasswordDto dto)
        {
            try
            {
                await _userService.ChangePasswordAsync(userId, dto);
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

        /// <summary>Deactivate a user</summary>
        [HttpPost("{userId}/deactivate")]
        public async Task<IActionResult> Deactivate(int userId)
        {
            try
            {
                await _userService.DeactivateAsync(userId);
                return NoContent();
                //itt lehetne 200-al is visszatérni
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

        /// <summary>Reactivate a user</summary>
        [HttpPost("{userId}/reactivate")]
        public async Task<IActionResult> Reactivate(int userId)
        {
            try
            {
                await _userService.ReactivateAsync(userId);
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