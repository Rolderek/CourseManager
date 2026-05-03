namespace CourseManager.DTOs.Schedule
{
    /// <summary>Request body for modifying (replacing) schedule entries of a course</summary>
    public class ModifyScheduleDto
    {
        /// <summary>
        /// New list of schedule entries — completely replaces all existing ones.
        /// For WEEKLY courses provide exactly one entry.
        /// For BLOCK courses provide all sessions.
        /// </summary>
        /// <example>
        /// Block: [{ "startTime": "2025-02-15T08:00:00", "endTime": "2025-02-15T16:00:00", "location": "B203" }]
        /// </example>
        public List<ScheduleEntryDto> Entries { get; set; } = new List<ScheduleEntryDto>();
    }
}