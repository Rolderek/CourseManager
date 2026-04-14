using CourseManager.Models.Enums;

namespace CourseManager.Models;

    public class Course
    {
        public int Id { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;  // e.g. "2024-25-2"
        public int MaxStudents { get; set; }
        public CourseType Type { get; set; }
        public CourseForm Form { get; set; }
        public ScheduleType ScheduleType { get; set; }

        // Foreign key to Subject
        public int SubjectId { get; set; }
        public Subject Subject { get; set; } = null!;

        // Navigation properties
        public ICollection<CourseInstructor> CourseInstructors { get; set; } = new List<CourseInstructor>();
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<ScheduleEntry> ScheduleEntries { get; set; } = new List<ScheduleEntry>();
    }
