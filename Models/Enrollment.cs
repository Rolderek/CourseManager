namespace CourseManager.Models;

    public class Enrollment
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public User Student { get; set; } = null!;

        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        public string? Grade { get; set; }  // null = no grade yet
    }
