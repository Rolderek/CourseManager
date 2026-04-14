namespace CourseManager.Models;

    public class CourseInstructor
    {
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        public int InstructorId { get; set; }
        public User Instructor { get; set; } = null!;
    }
