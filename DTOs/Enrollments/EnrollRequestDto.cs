namespace CourseManager.DTOs.Enrollments
{
    public class EnrollRequestDto
    {
        public int StudentId { get; set; }
        public List<int> CourseIds { get; set; } = new List<int>();
    }
}