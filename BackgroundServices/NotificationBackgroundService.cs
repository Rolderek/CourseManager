using CourseManager.Data;
using CourseManager.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseManager.BackgroundServices
{
    public class NotificationBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<NotificationBackgroundService> _logger;

        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

        public NotificationBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<NotificationBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }


        //orchestrates, nothing else
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

        //fetches entries, builds notifications, saves ???
        /*
         //old one:
        private async Task CheckAndGenerateNotificationsAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var upcomingEntries = await GetUpcomingScheduleEntriesAsync(context);

            if (!upcomingEntries.Any()) return;

            _logger.LogInformation(
                "Found {Count} upcoming schedule entries. Generating notifications...",
                upcomingEntries.Count);

            var notifications = new List<Notification>();

            foreach (var entry in upcomingEntries)
            {
                var studentNotifications = await BuildStudentNotificationsAsync(context, entry);
                var instructorNotifications = await BuildInstructorNotificationsAsync(context, entry);
                //ne várják meg egymást, átalakítani!
                //await task when all-al, 
                notifications.AddRange(studentNotifications);
                notifications.AddRange(instructorNotifications);
            }

            await SaveNotificationsAsync(context, notifications);
        }
        */

        private async Task CheckAndGenerateNotificationsAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var upcomingEntries = await GetUpcomingScheduleEntriesAsync(context);

            if (!upcomingEntries.Any()) return;

            _logger.LogInformation(
                "Found {Count} upcoming schedule entries. Generating notifications...",
                upcomingEntries.Count);

            // Fire all student AND instructor notification tasks at the same time
            var tasks = upcomingEntries.Select(entry => BuildNotificationsForEntryAsync(context, entry));

            // New version on this part
            var results = await Task.WhenAll(tasks);
            var notifications = results.SelectMany(n => n).ToList();

            await SaveNotificationsAsync(context, notifications);
        }

        // New helper: builds both student and instructor notifications in parallel
        private async Task<List<Notification>> BuildNotificationsForEntryAsync(
            AppDbContext context, ScheduleEntry entry)
        {
            var studentTask = BuildStudentNotificationsAsync(context, entry);
            var instructorTask = BuildInstructorNotificationsAsync(context, entry);

            var results = await Task.WhenAll(studentTask, instructorTask);

            return results.SelectMany(n => n).ToList();
        }

        //gets schedule entries starting in ~30 minutes
        private async Task<List<ScheduleEntry>> GetUpcomingScheduleEntriesAsync(AppDbContext context)
        {
            var targetTime = DateTime.Now.AddMinutes(30);

            return await context.ScheduleEntries
                .Include(se => se.Course)
                    .ThenInclude(c => c.Enrollments)
                        .ThenInclude(e => e.Student)
                .Include(se => se.Course)
                    .ThenInclude(c => c.CourseInstructors)
                        .ThenInclude(ci => ci.Instructor)
                .Where(se => se.StartTime >= targetTime
                          && se.StartTime < targetTime.AddMinutes(1))
                .ToListAsync();
        }

        //creates notification records for students
        private async Task<List<Notification>> BuildStudentNotificationsAsync(
            AppDbContext context, ScheduleEntry entry)
        {
            var notifications = new List<Notification>();

            foreach (var enrollment in entry.Course.Enrollments)
            {
                if (await IsAlreadyNotifiedAsync(context, enrollment.StudentId, entry))
                    continue;

                notifications.Add(CreateNotification(
                    userId: enrollment.StudentId,
                    courseId: entry.Course.Id,
                    message: $"Reminder: Your course '{entry.Course.CourseCode}' starts at " +
                             $"{entry.StartTime:yyyy-MM-dd HH:mm} in {entry.Location ?? "TBD"}. " +
                             $"This notification was generated 30 minutes before the class."
                ));
            }

            return notifications;
        }

        //creates notification records for instructors

        private async Task<List<Notification>> BuildInstructorNotificationsAsync(
            AppDbContext context, ScheduleEntry entry)
        {
            var notifications = new List<Notification>();

            foreach (var courseInstructor in entry.Course.CourseInstructors)
            {
                if (await IsAlreadyNotifiedAsync(context, courseInstructor.InstructorId, entry))
                    continue;

                notifications.Add(CreateNotification(
                    userId: courseInstructor.InstructorId,
                    courseId: entry.Course.Id,
                    message: $"Reminder: You are teaching '{entry.Course.CourseCode}' at " +
                             $"{entry.StartTime:yyyy-MM-dd HH:mm} in {entry.Location ?? "TBD"}. " +
                             $"This notification was generated 30 minutes before the class."
                ));
            }

            return notifications;
        }

        // returns true if notification already exists
        private async Task<bool> IsAlreadyNotifiedAsync(
            AppDbContext context, int userId, ScheduleEntry entry)
        {
            return await context.Notifications.AnyAsync(n =>
                n.UserId == userId &&
                n.CourseId == entry.Course.Id &&
                n.Message.Contains(entry.StartTime.ToString("yyyy-MM-dd HH:mm")));
        }

        // single Notification object
        private Notification CreateNotification(int userId, int courseId, string message)
        {
            return new Notification
            {
                UserId = userId,
                CourseId = courseId,
                Message = message,
                GeneratedAt = DateTime.Now
            };
        }

        //persists notifications and logs the result
        private async Task SaveNotificationsAsync(
            AppDbContext context, List<Notification> notifications)
        {
            if (!notifications.Any()) return;

            context.Notifications.AddRange(notifications);
            await context.SaveChangesAsync();

            _logger.LogInformation("Generated {Count} notifications.", notifications.Count);
        }
    }
}