namespace CourseManager.DTOs.Enrollments
{
    public class ChangeCourseDto
    {
        public int StudentId { get; set; }
        public int FromCourseId { get; set; }
        public int ToCourseId { get; set; }
    }
}