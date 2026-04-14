using CourseManager.Models;

namespace CourseManager.Models;

    public class Notification
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
    }

/*

Subject ──< Course >──< CourseInstructor >── User (Instructor)
                 │
                 └──< Enrollment >── User (Student)
                 │
                 └──< ScheduleEntry >── Notification
*/