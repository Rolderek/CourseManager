using CourseManager.Data;
using CourseManager.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseManager.BackgroundServices
{
    public class NotificationBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<NotificationBackgroundService> _logger;

        // Check every minute ,maybe change later the time period if its needed
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

        public NotificationBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<NotificationBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Notification Background Service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAndGenerateNotificationsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while generating notifications.");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Notification Background Service stopped.");
        }

        private async Task CheckAndGenerateNotificationsAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var now = DateTime.Now;
            var targetTime = now.AddMinutes(30);

            // Find all schedule entries that start within the next 30-31 minutes
            // The 1 minute window matches the check interval (if the time period will change need to change this too)
            var upcomingEntries = await context.ScheduleEntries
                .Include(se => se.Course)
                    .ThenInclude(c => c.Enrollments)
                        .ThenInclude(e => e.Student)
                .Include(se => se.Course)
                    .ThenInclude(c => c.CourseInstructors)
                        .ThenInclude(ci => ci.Instructor)
                .Where(se => se.StartTime >= targetTime
                          && se.StartTime < targetTime.AddMinutes(1))
                .ToListAsync();

            if (!upcomingEntries.Any()) return;

            _logger.LogInformation(
                "Found {Count} upcoming schedule entries. Generating notifications...",
                upcomingEntries.Count);

            var notifications = new List<Notification>();

            foreach (var entry in upcomingEntries)
            {
                var course = entry.Course;

                // Notify all enrolled students
                foreach (var enrollment in course.Enrollments)
                {
                    // Skip if notification already sent for this user + entry
                    var alreadyNotified = await context.Notifications.AnyAsync(n =>
                        n.UserId == enrollment.StudentId &&
                        n.CourseId == course.Id &&
                        n.Message.Contains(entry.StartTime.ToString("yyyy-MM-dd HH:mm")));

                    if (alreadyNotified) continue;

                    notifications.Add(new Notification
                    {
                        UserId = enrollment.StudentId,
                        CourseId = course.Id,
                        Message = $"Reminder: Your course '{course.CourseCode}' starts at " +
                                  $"{entry.StartTime:yyyy-MM-dd HH:mm} in {entry.Location ?? "TBD"}. " +
                                  $"This notification was generated 30 minutes before the class.",
                        GeneratedAt = now
                    });
                }

                // Notify all instructors
                foreach (var courseInstructor in course.CourseInstructors)
                {
                    var alreadyNotified = await context.Notifications.AnyAsync(n =>
                        n.UserId == courseInstructor.InstructorId &&
                        n.CourseId == course.Id &&
                        n.Message.Contains(entry.StartTime.ToString("yyyy-MM-dd HH:mm")));

                    if (alreadyNotified) continue;

                    notifications.Add(new Notification
                    {
                        UserId = courseInstructor.InstructorId,
                        CourseId = course.Id,
                        Message = $"Reminder: You are teaching '{course.CourseCode}' at " +
                                  $"{entry.StartTime:yyyy-MM-dd HH:mm} in {entry.Location ?? "TBD"}. " +
                                  $"This notification was generated 30 minutes before the class.",
                        GeneratedAt = now
                    });
                }
            }

            if (notifications.Any())
            {
                context.Notifications.AddRange(notifications);
                await context.SaveChangesAsync();

                _logger.LogInformation(
                    "Generated {Count} notifications.", notifications.Count);
            }
        }
    }
}