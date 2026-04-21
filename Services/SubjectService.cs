using CourseManager.Data;
using CourseManager.DTOs.Subjects;
using CourseManager.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseManager.Services
{
    public class SubjectService
    {
        private readonly AppDbContext _context;

        public SubjectService(AppDbContext context)
        {
            _context = context;
        }

        // GET ALL

        public async Task<List<SubjectResponseDto>> GetAllAsync()
        {
            var subjects = await _context.Subjects.ToListAsync();
            return subjects.Select(MapToResponse).ToList();
        }

        // GET BY ID

        public async Task<SubjectResponseDto> GetByIdAsync(int subjectId)
        {
            var subject = await _context.Subjects.FindAsync(subjectId);

            if (subject == null)
                throw new KeyNotFoundException($"Subject with ID {subjectId} not found.");

            return MapToResponse(subject);
        }

        // CREATE

        public async Task<SubjectResponseDto> CreateAsync(CreateSubjectDto dto)
        {
            // Validate: code must be unique
            if (await _context.Subjects.AnyAsync(s => s.Code == dto.Code))
                throw new InvalidOperationException($"A subject with code '{dto.Code}' already exists.");

            // Validate: credits must be positive
            if (dto.Credits <= 0)
                throw new InvalidOperationException("Credits must be a positive number.");

            var subject = new Subject
            {
                Code = dto.Code,
                Name = dto.Name,
                Credits = dto.Credits,
                IsActive = true
            };

            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();

            return MapToResponse(subject);
        }

        // UPDATE

        public async Task<SubjectResponseDto> UpdateAsync(int subjectId, UpdateSubjectDto dto)
        {
            var subject = await _context.Subjects.FindAsync(subjectId);

            if (subject == null)
                throw new KeyNotFoundException($"Subject with ID {subjectId} not found.");

            if (!subject.IsActive)
                throw new InvalidOperationException("Cannot update an inactive subject.");

            //létre tudok hozni nulla kredittel, de módosítani már nem tudom, átírjuk?
            if (dto.Credits <= 0)
                throw new InvalidOperationException("Credits must be a positive number.");

            subject.Name = dto.Name;
            subject.Credits = dto.Credits;

            await _context.SaveChangesAsync();

            return MapToResponse(subject);
        }

        // DEACTIVATE

        public async Task DeactivateAsync(int subjectId)
        {
            var subject = await _context.Subjects.FindAsync(subjectId);

            if (subject == null)
                throw new KeyNotFoundException($"Subject with ID {subjectId} not found.");

            if (!subject.IsActive)
                throw new InvalidOperationException("Subject is already inactive.");

            subject.IsActive = false;
            await _context.SaveChangesAsync();
        }

        // REACTIVATE

        public async Task ReactivateAsync(int subjectId)
        {
            var subject = await _context.Subjects.FindAsync(subjectId);

            if (subject == null)
                throw new KeyNotFoundException($"Subject with ID {subjectId} not found.");

            if (subject.IsActive)
                throw new InvalidOperationException("Subject is already active.");

            subject.IsActive = true;
            await _context.SaveChangesAsync();
        }

        // HELPER

        private SubjectResponseDto MapToResponse(Subject subject)
        {
            return new SubjectResponseDto
            {
                Id = subject.Id,
                Code = subject.Code,
                Name = subject.Name,
                Credits = subject.Credits,
                IsActive = subject.IsActive
            };
        }
    }
}