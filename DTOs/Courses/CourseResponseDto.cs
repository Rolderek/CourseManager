using CourseManager.Models.Enums;

namespace CourseManager.DTOs.Courses
{
    public class CourseResponseDto
    {
        public int Id { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public int MaxStudents { get; set; }
        public int CurrentStudents { get; set; }
        public CourseType Type { get; set; }
        public CourseForm Form { get; set; }
        public ScheduleType ScheduleType { get; set; }
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public string SubjectCode { get; set; } = string.Empty;
        public List<InstructorDto> Instructors { get; set; } = new List<InstructorDto>();
    }

    public class InstructorDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}