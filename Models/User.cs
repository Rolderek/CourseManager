using CourseManager.Models.Enums;

namespace CourseManager.Models;

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserType UserType { get; set; }
        public bool IsActive { get; set; } = true;

        // Only relevant if UserType == Student
        public StudyMode? StudyMode { get; set; }

        // Navigation properties (for EF Core relationships)
        public ICollection<CourseInstructor> CourseInstructors { get; set; } = new List<CourseInstructor>();
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
