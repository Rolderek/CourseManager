namespace CourseManager.DTOs.Schedule
{
    /// <summary>
    /// Used to modify existing schedule entries for a course.
    /// All existing entries for the course will be replaced with the new ones.
    ///
    /// For WEEKLY courses: provide exactly one entry.
    /// Example:
    /// {
    ///   "entries": [
    ///     { "startTime": "2025-02-03T10:00:00", "endTime": "2025-02-03T12:00:00", "location": "C301" }
    ///   ]
    /// }
    ///
    /// For BLOCK courses: provide all sessions again (full replacement).
    /// Example:
    /// {
    ///   "entries": [
    ///     { "startTime": "2025-02-20T08:00:00", "endTime": "2025-02-20T16:00:00", "location": "A101" },
    ///     { "startTime": "2025-03-20T08:00:00", "endTime": "2025-03-20T16:00:00", "location": "A101" }
    ///   ]
    /// }
    /// </summary>
    public class ModifyScheduleDto
    {
        public List<ScheduleEntryDto> Entries { get; set; } = new List<ScheduleEntryDto>();
    }
}