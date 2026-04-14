using CourseManager.Models.Enums;


//this is waht the prog send back to the caller
namespace CourseManager.DTOs.Users
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserType UserType { get; set; }
        public StudyMode? StudyMode { get; set; }
        public bool IsActive { get; set; }
    }
}