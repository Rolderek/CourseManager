namespace CourseManager.DTOs.Subjects
{
    public class SubjectResponseDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Credits { get; set; }
        public bool IsActive { get; set; }
    }
}