using CourseManager.Data;
using CourseManager.DTOs.Enrollments;
using CourseManager.Models;
using CourseManager.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace CourseManager.Services
{
    public class EnrollmentService
    {
        private readonly AppDbContext _context;

        public EnrollmentService(AppDbContext context)
        {
            _context = context;
        }

        // ENROLL IN SUBJECT

        public async Task<List<EnrollmentResponseDto>> EnrollAsync(int subjectId, EnrollRequestDto dto)
        {
            // Validate student
            var student = await _context.Users.FindAsync(dto.StudentId);
            if (student == null)
                throw new KeyNotFoundException($"User with ID {dto.StudentId} not found.");
            if (student.UserType != UserType.Student)
                throw new InvalidOperationException("Only students can enroll in courses.");
            if (!student.IsActive)
                throw new InvalidOperationException("Inactive students cannot enroll.");

            // Validate subject
            var subject = await _context.Subjects.FindAsync(subjectId);
            if (subject == null)
                throw new KeyNotFoundException($"Subject with ID {subjectId} not found.");
            if (!subject.IsActive)
                throw new InvalidOperationException("Cannot enroll in an inactive subject.");

            // Load the courses the student wants to enroll in
            var courses = await _context.Courses
                .Include(c => c.Enrollments)
                .Include(c => c.Subject)
                .Where(c => dto.CourseIds.Contains(c.Id))
                .ToListAsync();

            // Check all course IDs were found
            foreach (var id in dto.CourseIds)
            {
                if (!courses.Any(c => c.Id == id))
                    throw new KeyNotFoundException($"Course with ID {id} not found.");
            }

            // All courses must belong to the requested subject
            if (courses.Any(c => c.SubjectId != subjectId))
                throw new InvalidOperationException("All courses must belong to the specified subject.");

            // All courses must be in the same semester
            var semesters = courses.Select(c => c.Semester).Distinct().ToList();
            if (semesters.Count > 1)
                throw new InvalidOperationException("All courses must be in the same semester.");

            var semester = semesters.First();

            // Check student is not already enrolled in this subject this semester
            var alreadyEnrolled = await _context.Enrollments
                .Include(e => e.Course)
                .AnyAsync(e => e.StudentId == dto.StudentId
                            && e.Course.SubjectId == subjectId
                            && e.Course.Semester == semester);

            if (alreadyEnrolled)
                throw new InvalidOperationException("Student is already enrolled in this subject this semester.");

            // Get all course types available for this subject in this semester
            var availableCourses = await _context.Courses
                .Where(c => c.SubjectId == subjectId && c.Semester == semester)
                .ToListAsync();

            // Filter by study mode compatibility
            var compatibleCourses = availableCourses.Where(c =>
                c.Form == CourseForm.Mixed ||
                (student.StudyMode == StudyMode.FullTime && c.Form == CourseForm.FullTime) ||
                (student.StudyMode == StudyMode.PartTime && c.Form == CourseForm.PartTime)
            ).ToList();

            // Get distinct course types available for this student
            var requiredTypes = compatibleCourses.Select(c => c.Type).Distinct().ToList();

            // Student must pick exactly one course per available type
            foreach (var type in requiredTypes)
            {
                var selectedOfType = courses.Where(c => c.Type == type).ToList();
                if (selectedOfType.Count == 0)
                    throw new InvalidOperationException(
                        $"You must select exactly one course of type '{type}' for this subject.");
                if (selectedOfType.Count > 1)
                    throw new InvalidOperationException(
                        $"You can only select one course of type '{type}', but {selectedOfType.Count} were provided.");
            }

            // Check no extra course types were selected
            foreach (var course in courses)
            {
                if (!requiredTypes.Contains(course.Type))
                    throw new InvalidOperationException(
                        $"Course '{course.CourseCode}' has type '{course.Type}' which is not available for this subject/semester.");
            }

            // Validate study mode compatibility for each selected course
            foreach (var course in courses)
            {
                if (course.Form != CourseForm.Mixed)
                {
                    if (student.StudyMode == StudyMode.FullTime && course.Form != CourseForm.FullTime)
                        throw new InvalidOperationException(
                            $"Full-time student cannot enroll in part-time course '{course.CourseCode}'.");
                    if (student.StudyMode == StudyMode.PartTime && course.Form != CourseForm.PartTime)
                        throw new InvalidOperationException(
                            $"Part-time student cannot enroll in full-time course '{course.CourseCode}'.");
                }
            }

            // Check capacity for each course
            foreach (var course in courses)
            {
                if (course.Enrollments.Count >= course.MaxStudents)
                    throw new InvalidOperationException(
                        $"Course '{course.CourseCode}' is full ({course.MaxStudents}/{course.MaxStudents}).");
            }

            // All validations passed — create enrollments
            var enrollments = courses.Select(c => new Enrollment
            {
                StudentId = dto.StudentId,
                CourseId = c.Id
            }).ToList();

            _context.Enrollments.AddRange(enrollments);
            await _context.SaveChangesAsync();

            return enrollments.Select(e => MapToResponse(e, courses.First(c => c.Id == e.CourseId), student)).ToList();
        }

        // UNREGISTER FROM SUBJECT

        public async Task UnregisterAsync(int subjectId, UnregisterRequestDto dto)
        {
            var student = await _context.Users.FindAsync(dto.StudentId);
            if (student == null)
                throw new KeyNotFoundException($"User with ID {dto.StudentId} not found.");

            // Find all enrollments for this student, subject and semester
            var enrollments = await _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.StudentId == dto.StudentId
                         && e.Course.SubjectId == subjectId
                         && e.Course.Semester == dto.Semester)
                .ToListAsync();

            if (!enrollments.Any())
                throw new InvalidOperationException(
                    "Student is not enrolled in this subject for the given semester.");

            _context.Enrollments.RemoveRange(enrollments);
            await _context.SaveChangesAsync();
        }

        // CHANGE COURSE

        public async Task<EnrollmentResponseDto> ChangeCourseAsync(ChangeCourseDto dto)
        {
            var student = await _context.Users.FindAsync(dto.StudentId);
            if (student == null)
                throw new KeyNotFoundException($"User with ID {dto.StudentId} not found.");
            if (!student.IsActive)
                throw new InvalidOperationException("Inactive students cannot change courses.");

            // Load both courses
            var fromCourse = await _context.Courses
                .Include(c => c.Enrollments)
                .Include(c => c.Subject)
                .FirstOrDefaultAsync(c => c.Id == dto.FromCourseId);

            var toCourse = await _context.Courses
                .Include(c => c.Enrollments)
                .Include(c => c.Subject)
                .FirstOrDefaultAsync(c => c.Id == dto.ToCourseId);

            if (fromCourse == null)
                throw new KeyNotFoundException($"Course with ID {dto.FromCourseId} not found.");
            if (toCourse == null)
                throw new KeyNotFoundException($"Course with ID {dto.ToCourseId} not found.");

            // Must be same subject
            if (fromCourse.SubjectId != toCourse.SubjectId)
                throw new InvalidOperationException("Can only change courses within the same subject.");

            // Must be same type
            if (fromCourse.Type != toCourse.Type)
                throw new InvalidOperationException("Can only change to a course of the same type.");

            // Must be same semester
            if (fromCourse.Semester != toCourse.Semester)
                throw new InvalidOperationException("Can only change to a course in the same semester.");

            // Check student is enrolled in the from course
            var existingEnrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == dto.StudentId && e.CourseId == dto.FromCourseId);

            if (existingEnrollment == null)
                throw new InvalidOperationException("Student is not enrolled in the source course.");

            // Check study mode compatibility with the new course
            if (toCourse.Form != CourseForm.Mixed)
            {
                if (student.StudyMode == StudyMode.FullTime && toCourse.Form != CourseForm.FullTime)
                    throw new InvalidOperationException("Full-time student cannot switch to a part-time course.");
                if (student.StudyMode == StudyMode.PartTime && toCourse.Form != CourseForm.PartTime)
                    throw new InvalidOperationException("Part-time student cannot switch to a full-time course.");
            }

            // Check capacity on the new course
            if (toCourse.Enrollments.Count >= toCourse.MaxStudents)
                throw new InvalidOperationException($"Course '{toCourse.CourseCode}' is full.");

            // Switch the enrollment
            existingEnrollment.CourseId = toCourse.Id;
            await _context.SaveChangesAsync();

            return MapToResponse(existingEnrollment, toCourse, student);
        }

        // LIST STUDENTS IN COURSE

        public async Task<List<EnrollmentResponseDto>> GetCourseStudentsAsync(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
                throw new KeyNotFoundException($"Course with ID {courseId} not found.");

            var enrollments = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                    .ThenInclude(c => c.Subject)
                .Where(e => e.CourseId == courseId)
                .ToListAsync();

            return enrollments.Select(e => MapToResponse(e, e.Course, e.Student)).ToList();
        }

        // LIST STUDENTS IN SUBJECT FOR SEMESTER

        public async Task<List<EnrollmentResponseDto>> GetSubjectStudentsAsync(int subjectId, string semester)
        {
            var subject = await _context.Subjects.FindAsync(subjectId);
            if (subject == null)
                throw new KeyNotFoundException($"Subject with ID {subjectId} not found.");

            var enrollments = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                    .ThenInclude(c => c.Subject)
                .Where(e => e.Course.SubjectId == subjectId && e.Course.Semester == semester)
                .ToListAsync();

            return enrollments.Select(e => MapToResponse(e, e.Course, e.Student)).ToList();
        }

        // HELPER

        private EnrollmentResponseDto MapToResponse(Enrollment enrollment, Course course, User student)
        {
            return new EnrollmentResponseDto
            {
                EnrollmentId = enrollment.Id,
                StudentId = student.Id,
                StudentName = student.Username,
                CourseId = course.Id,
                CourseCode = course.CourseCode,
                SubjectName = course.Subject?.Name ?? string.Empty,
                Semester = course.Semester,
                Grade = enrollment.Grade
            };
        }

        //This is the new missed feature:

        // -------------------------------------------------------
        // REGISTER DIRECTLY TO A COURSE
        // -------------------------------------------------------
        public async Task<EnrollmentResponseDto> RegisterToCourseAsync(int courseId, int studentId)
        {
            var student = await _context.Users.FindAsync(studentId);
            if (student == null)
                throw new KeyNotFoundException($"User with ID {studentId} not found.");
            if (student.UserType != UserType.Student)
                throw new InvalidOperationException("Only students can enroll in courses.");
            if (!student.IsActive)
                throw new InvalidOperationException("Inactive students cannot enroll.");

            var course = await _context.Courses
                .Include(c => c.Enrollments)
                .Include(c => c.Subject)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
                throw new KeyNotFoundException($"Course with ID {courseId} not found.");

            // Check study mode compatibility
            if (course.Form != CourseForm.Mixed)
            {
                if (student.StudyMode == StudyMode.FullTime && course.Form != CourseForm.FullTime)
                    throw new InvalidOperationException(
                        "Full-time student cannot enroll in a part-time course.");
                if (student.StudyMode == StudyMode.PartTime && course.Form != CourseForm.PartTime)
                    throw new InvalidOperationException(
                        "Part-time student cannot enroll in a full-time course.");
            }

            // Check capacity
            if (course.Enrollments.Count >= course.MaxStudents)
                throw new InvalidOperationException(
                    $"Course '{course.CourseCode}' is full ({course.MaxStudents}/{course.MaxStudents}).");

            // Check not already enrolled
            var alreadyEnrolled = await _context.Enrollments
                .AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId);
            if (alreadyEnrolled)
                throw new InvalidOperationException("Student is already enrolled in this course.");

            var enrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            // Reload with navigation properties for mapping
            var reloaded = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                    .ThenInclude(c => c.Subject)
                .FirstOrDefaultAsync(e => e.Id == enrollment.Id);

            return MapToResponse(reloaded!, reloaded!.Course, reloaded.Student);
        }

        // -------------------------------------------------------
        // UNREGISTER DIRECTLY FROM A COURSE
        // -------------------------------------------------------
        public async Task UnregisterFromCourseAsync(int courseId, int studentId)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

            if (enrollment == null)
                throw new InvalidOperationException("Student is not enrolled in this course.");

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
        }

    }
}