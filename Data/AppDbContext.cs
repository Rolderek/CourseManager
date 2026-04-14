using CourseManager.Models;
using CourseManager.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace CourseManager.Data;

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // tables representation:
    public DbSet<User> Users { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseInstructor> CourseInstructors { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<ScheduleEntry> ScheduleEntries { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // USER
            modelBuilder.Entity<User>(entity =>
            {
                // Email must be unique
                entity.HasIndex(u => u.Email).IsUnique();

                // Store enums as strings (readable in the DB)
                entity.Property(u => u.UserType)
                      .HasConversion<string>();

                entity.Property(u => u.StudyMode)
                      .HasConversion<string>();
            });

            // SUBJECT
            modelBuilder.Entity<Subject>(entity =>
            {
                // Subject code must be unique
                entity.HasIndex(s => s.Code).IsUnique();
            });

            // COURSE
            modelBuilder.Entity<Course>(entity =>
            {
                // Store enums as strings
                entity.Property(c => c.Type)
                      .HasConversion<string>();

                entity.Property(c => c.Form)
                      .HasConversion<string>();

                entity.Property(c => c.ScheduleType)
                      .HasConversion<string>();

                // Course belongs to a Subject
                entity.HasOne(c => c.Subject)
                      .WithMany(s => s.Courses)
                      .HasForeignKey(c => c.SubjectId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // COURSE INSTRUCTOR (junction table)
            modelBuilder.Entity<CourseInstructor>(entity =>
            {
                // The primary key is the COMBINATION of both IDs
                entity.HasKey(ci => new { ci.CourseId, ci.InstructorId });

                entity.HasOne(ci => ci.Course)
                      .WithMany(c => c.CourseInstructors)
                      .HasForeignKey(ci => ci.CourseId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ci => ci.Instructor)
                      .WithMany(u => u.CourseInstructors)
                      .HasForeignKey(ci => ci.InstructorId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ENROLLMENT
            modelBuilder.Entity<Enrollment>(entity =>
            {
                // A student can only be enrolled once per course
                entity.HasIndex(e => new { e.StudentId, e.CourseId }).IsUnique();

                entity.HasOne(e => e.Student)
                      .WithMany(u => u.Enrollments)
                      .HasForeignKey(e => e.StudentId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Course)
                      .WithMany(c => c.Enrollments)
                      .HasForeignKey(e => e.CourseId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // SCHEDULE ENTRY
            modelBuilder.Entity<ScheduleEntry>(entity =>
            {
                entity.HasOne(se => se.Course)
                      .WithMany(c => c.ScheduleEntries)
                      .HasForeignKey(se => se.CourseId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // NOTIFICATION
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasOne(n => n.User)
                      .WithMany(u => u.Notifications)
                      .HasForeignKey(n => n.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(n => n.Course)
                      .WithMany()
                      .HasForeignKey(n => n.CourseId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
