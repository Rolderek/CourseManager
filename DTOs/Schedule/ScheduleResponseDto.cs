using CourseManager.Models.Enums;

namespace CourseManager.DTOs.Schedule
{
    public class ScheduleResponseDto
    {
        public int CourseId { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        public ScheduleType ScheduleType { get; set; }
        public List<ScheduleEntryResponseDto> Entries { get; set; } = new List<ScheduleEntryResponseDto>();
    }

    public class ScheduleEntryResponseDto
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Location { get; set; }
    }
}