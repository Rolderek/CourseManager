using CourseManager.Models.Enums;

namespace CourseManager.DTOs.Courses
{
    public class UpdateCourseDto
    {
        public string Semester { get; set; } = string.Empty;
        public int MaxStudents { get; set; }
        public CourseType Type { get; set; }
        public CourseForm Form { get; set; }
        public ScheduleType ScheduleType { get; set; }
    }
}