namespace CourseManager.Models;

    public class ScheduleEntry
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Location { get; set; }  // e.g. "B301"

        // Foreign key to Course
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
    }
