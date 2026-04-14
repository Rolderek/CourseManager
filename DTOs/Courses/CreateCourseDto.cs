using CourseManager.Models.Enums;

namespace CourseManager.DTOs.Courses
{
    public class CreateCourseDto
    {
        public string CourseCode { get; set; } = string.Empty;
        public string SubjectCode { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public int MaxStudents { get; set; }
        public CourseType Type { get; set; }
        public CourseForm Form { get; set; }
        public ScheduleType ScheduleType { get; set; }

        // List of instructor user IDs to assign
        public List<int> InstructorIds { get; set; } = new List<int>();
    }
}