using CourseManager.Data;
using CourseManager.DTOs.Schedule;
using CourseManager.Models;
using CourseManager.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace CourseManager.Services
{
    public class ScheduleService
    {
        private readonly AppDbContext _context;

        public ScheduleService(AppDbContext context)
        {
            _context = context;
        }

        // ADD SCHEDULE ENTRIES

        public async Task<ScheduleResponseDto> AddScheduleAsync(int courseId, AddScheduleDto dto)
        {
            var course = await _context.Courses
                .Include(c => c.ScheduleEntries)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
                throw new KeyNotFoundException($"Course with ID {courseId} not found.");

            // Check if schedule already exists
            if (course.ScheduleEntries.Any())
                throw new InvalidOperationException(
                    "Schedule already exists for this course. Use the modify endpoint to update it.");

            if (dto.Entries == null || dto.Entries.Count == 0)
                throw new InvalidOperationException("At least one schedule entry must be provided.");

            // Weekly courses should only have one entry
            if (course.ScheduleType == ScheduleType.Weekly && dto.Entries.Count > 1)
                throw new InvalidOperationException(
                    "Weekly courses can only have one schedule entry (it repeats every week for 14 weeks).");

            // Validate each entry
            foreach (var entry in dto.Entries)
            {
                if (entry.EndTime <= entry.StartTime)
                    throw new InvalidOperationException(
                        $"EndTime must be after StartTime for entry starting at {entry.StartTime}.");
            }

            var entries = dto.Entries.Select(e => new ScheduleEntry
            {
                CourseId = courseId,
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                Location = e.Location
            }).ToList();

            _context.ScheduleEntries.AddRange(entries);
            await _context.SaveChangesAsync();

            return MapToResponse(course, entries);
        }

        // MODIFY SCHEDULE ENTRIES

        public async Task<ScheduleResponseDto> ModifyScheduleAsync(int courseId, ModifyScheduleDto dto)
        {
            //a másikkal összevonni, fölösleges duplikálás
            var course = await _context.Courses
                .Include(c => c.ScheduleEntries)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
                throw new KeyNotFoundException($"Course with ID {courseId} not found.");

            if (dto.Entries == null || dto.Entries.Count == 0)
                throw new InvalidOperationException("At least one schedule entry must be provided.");

            // Weekly courses should only have one entry
            if (course.ScheduleType == ScheduleType.Weekly && dto.Entries.Count > 1)
                throw new InvalidOperationException(
                    "Weekly courses can only have one schedule entry (it repeats every week for 14 weeks).");

            // Validate each entry
            foreach (var entry in dto.Entries)
            {
                if (entry.EndTime <= entry.StartTime)
                    throw new InvalidOperationException(
                        $"EndTime must be after StartTime for entry starting at {entry.StartTime}.");
            }

            // Remove all existing entries
            _context.ScheduleEntries.RemoveRange(course.ScheduleEntries);

            // Add the new ones
            var entries = dto.Entries.Select(e => new ScheduleEntry
            {
                CourseId = courseId,
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                Location = e.Location
            }).ToList();

            _context.ScheduleEntries.AddRange(entries);
            await _context.SaveChangesAsync();

            return MapToResponse(course, entries);
        }

        // GET SCHEDULE

        public async Task<ScheduleResponseDto> GetScheduleAsync(int courseId)
        {
            var course = await _context.Courses
                .Include(c => c.ScheduleEntries)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
                throw new KeyNotFoundException($"Course with ID {courseId} not found.");

            return MapToResponse(course, course.ScheduleEntries.ToList());
        }

        // HELPER

        private ScheduleResponseDto MapToResponse(Course course, List<ScheduleEntry> entries)
        {
            return new ScheduleResponseDto
            {
                CourseId = course.Id,
                CourseCode = course.CourseCode,
                ScheduleType = course.ScheduleType,
                Entries = entries.Select(e => new ScheduleEntryResponseDto
                {
                    Id = e.Id,
                    StartTime = e.StartTime,
                    EndTime = e.EndTime,
                    Location = e.Location
                }).ToList()
            };
        }
    }
}