namespace CourseManager.DTOs.Schedule
{
    /// <summary>
    /// This need to reractor? Check later.
    /// 
    /// Used to add schedule entries to a course.
    /// 
    /// For WEEKLY courses: provide exactly one entry.
    /// The system will display it as repeating every week for 14 weeks.
    /// Example:
    /// {
    ///   "entries": [
    ///     { "startTime": "2025-02-03T08:00:00", "endTime": "2025-02-03T10:00:00", "location": "A101" }
    ///   ]
    /// }
    /// 
    /// For BLOCK courses: provide one entry per session.
    /// Example:
    /// {
    ///   "entries": [
    ///     { "startTime": "2025-02-15T08:00:00", "endTime": "2025-02-15T16:00:00", "location": "B203" },
    ///     { "startTime": "2025-03-15T08:00:00", "endTime": "2025-03-15T16:00:00", "location": "B203" }
    ///   ]
    /// }
    /// </summary>
    public class AddScheduleDto
    {
        public List<ScheduleEntryDto> Entries { get; set; } = new List<ScheduleEntryDto>();
    }

    public class ScheduleEntryDto
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Location { get; set; }
    }
}