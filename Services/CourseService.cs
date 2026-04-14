using CourseManager.Data;
using CourseManager.DTOs.Courses;
using CourseManager.Models;
using CourseManager.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace CourseManager.Services
{
    public class CourseService
    {
        private readonly AppDbContext _context;

        public CourseService(AppDbContext context)
        {
            _context = context;
        }

        // CREATE

        public async Task<CourseResponseDto> CreateAsync(CreateCourseDto dto)
        {
            // Find the subject by code
            var subject = await _context.Subjects
                .FirstOrDefaultAsync(s => s.Code == dto.SubjectCode);

            if (subject == null)
                throw new KeyNotFoundException($"Subject with code '{dto.SubjectCode}' not found.");

            if (!subject.IsActive)
                throw new InvalidOperationException("Cannot create a course for an inactive subject.");

            // Validate: course code must be unique
            if (await _context.Courses.AnyAsync(c => c.CourseCode == dto.CourseCode))
                throw new InvalidOperationException($"A course with code '{dto.CourseCode}' already exists.");

            // Validate: max students must be positive
            if (dto.MaxStudents <= 0)
                throw new InvalidOperationException("MaxStudents must be a positive number.");

            // Validate instructors
            if (dto.InstructorIds == null || dto.InstructorIds.Count == 0)
                throw new InvalidOperationException("At least one instructor must be assigned.");

            var instructors = await _context.Users
                .Where(u => dto.InstructorIds.Contains(u.Id))
                .ToListAsync();

            // Check all provided IDs exist and are instructors
            foreach (var id in dto.InstructorIds)
            {
                var instructor = instructors.FirstOrDefault(u => u.Id == id);
                if (instructor == null)
                    throw new KeyNotFoundException($"User with ID {id} not found.");
                if (instructor.UserType != UserType.Instructor)
                    throw new InvalidOperationException($"User with ID {id} is not an instructor.");
                if (!instructor.IsActive)
                    throw new InvalidOperationException($"Instructor with ID {id} is inactive.");
            }

            var course = new Course
            {
                CourseCode = dto.CourseCode,
                SubjectId = subject.Id,
                Semester = dto.Semester,
                MaxStudents = dto.MaxStudents,
                Type = dto.Type,
                Form = dto.Form,
                ScheduleType = dto.ScheduleType
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            // Assign instructors
            var courseInstructors = instructors.Select(i => new CourseInstructor
            {
                CourseId = course.Id,
                InstructorId = i.Id
            }).ToList();

            _context.CourseInstructors.AddRange(courseInstructors);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(course.Id);
        }

        // GET BY ID

        public async Task<CourseResponseDto> GetByIdAsync(int courseId)
        {
            var course = await _context.Courses
                .Include(c => c.Subject)
                .Include(c => c.CourseInstructors)
                    .ThenInclude(ci => ci.Instructor)
                .Include(c => c.Enrollments)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
                throw new KeyNotFoundException($"Course with ID {courseId} not found.");

            return MapToResponse(course);
        }

        // UPDATE

        public async Task<CourseResponseDto> UpdateAsync(int courseId, UpdateCourseDto dto)
        {
            var course = await _context.Courses
                .Include(c => c.Enrollments)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
                throw new KeyNotFoundException($"Course with ID {courseId} not found.");

            // Validate: max students can't be less than current enrollment
            if (dto.MaxStudents < course.Enrollments.Count)
                throw new InvalidOperationException(
                    $"MaxStudents ({dto.MaxStudents}) cannot be less than current enrollment ({course.Enrollments.Count}).");

            course.Semester = dto.Semester;
            course.MaxStudents = dto.MaxStudents;
            course.Type = dto.Type;
            course.Form = dto.Form;
            course.ScheduleType = dto.ScheduleType;

            await _context.SaveChangesAsync();

            return await GetByIdAsync(course.Id);
        }

        // DELETE

        public async Task DeleteAsync(int courseId)
        {
            var course = await _context.Courses
                .Include(c => c.Enrollments)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
                throw new KeyNotFoundException($"Course with ID {courseId} not found.");

            // Validate: can only delete if no students enrolled
            if (course.Enrollments.Any())
                throw new InvalidOperationException("Cannot delete a course that has enrolled students.");

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
        }

        // HELPER

        private CourseResponseDto MapToResponse(Course course)
        {
            return new CourseResponseDto
            {
                Id = course.Id,
                CourseCode = course.CourseCode,
                Semester = course.Semester,
                MaxStudents = course.MaxStudents,
                CurrentStudents = course.Enrollments?.Count ?? 0,
                Type = course.Type,
                Form = course.Form,
                ScheduleType = course.ScheduleType,
                SubjectId = course.SubjectId,
                SubjectName = course.Subject?.Name ?? string.Empty,
                SubjectCode = course.Subject?.Code ?? string.Empty,
                Instructors = course.CourseInstructors?.Select(ci => new InstructorDto
                {
                    Id = ci.Instructor.Id,
                    Username = ci.Instructor.Username,
                    Email = ci.Instructor.Email
                }).ToList() ?? new List<InstructorDto>()
            };
        }
    }
}