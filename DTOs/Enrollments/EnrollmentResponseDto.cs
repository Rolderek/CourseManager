namespace CourseManager.DTOs.Enrollments
{
    public class EnrollmentResponseDto
    {
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public string? Grade { get; set; }
    }
}