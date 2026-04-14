namespace CourseManager.Models;

    public class Subject
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Credits { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation property
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
