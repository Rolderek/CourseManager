using Swashbuckle.AspNetCore.Annotations;

namespace CourseManager.DTOs.Schedule
{
    /// <summary>Request body for adding schedule entries to a course</summary>
    public class AddScheduleDto
    {
        /// <summary>
        /// List of schedule entries.
        /// For WEEKLY courses provide exactly one entry — it repeats every week for 14 weeks.
        /// For BLOCK courses provide one entry per session.
        /// </summary>
        /// <example>
        /// Weekly: [{ "startTime": "2025-02-03T08:00:00", "endTime": "2025-02-03T10:00:00", "location": "A101" }]
        /// </example>
        public List<ScheduleEntryDto> Entries { get; set; } = new List<ScheduleEntryDto>();
    }

    public class ScheduleEntryDto
    {
        /// <summary>Start date and time of the session</summary>
        /// <example>2025-02-03T08:00:00</example>
        public DateTime StartTime { get; set; }

        /// <summary>End date and time of the session</summary>
        /// <example>2025-02-03T10:00:00</example>
        public DateTime EndTime { get; set; }

        /// <summary>Room or location (optional)</summary>
        /// <example>A101</example>
        public string? Location { get; set; }
    }
}