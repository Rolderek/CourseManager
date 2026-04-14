using CourseManager.Models.Enums;

namespace CourseManager.DTOs.Users
{
    public class RegisterUserDto
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserType UserType { get; set; }

        // Only required if UserType == Student
        public StudyMode? StudyMode { get; set; }
    }
}