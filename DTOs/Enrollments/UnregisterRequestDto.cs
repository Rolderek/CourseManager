namespace CourseManager.DTOs.Enrollments
{
    public class UnregisterRequestDto
    {
        public int StudentId { get; set; }
        public string Semester { get; set; } = string.Empty;
    }
}