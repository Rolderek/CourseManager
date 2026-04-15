namespace CourseManager.DTOs.Notifications
{
    public class NotificationResponseDto
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public string CourseCode { get; set; } = string.Empty;
    }
}