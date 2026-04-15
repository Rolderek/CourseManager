using CourseManager.Data;
using CourseManager.DTOs.Notifications;
using Microsoft.EntityFrameworkCore;

namespace CourseManager.Services
{
    public class NotificationService
    {
        private readonly AppDbContext _context;

        public NotificationService(AppDbContext context)
        {
            _context = context;
        }

        // GET ALL NOTIFICATIONS (with optional filters)

        public async Task<List<NotificationResponseDto>> GetAllAsync(int? userId, int? courseId)
        {
            var query = _context.Notifications
                .Include(n => n.User)
                .Include(n => n.Course)
                .AsQueryable();

            if (userId.HasValue)
                query = query.Where(n => n.UserId == userId.Value);

            if (courseId.HasValue)
                query = query.Where(n => n.CourseId == courseId.Value);

            var notifications = await query
                .OrderByDescending(n => n.GeneratedAt)
                .ToListAsync();

            return notifications.Select(n => new NotificationResponseDto
            {
                Id = n.Id,
                Message = n.Message,
                GeneratedAt = n.GeneratedAt,
                UserId = n.UserId,
                Username = n.User.Username,
                CourseId = n.CourseId,
                CourseCode = n.Course.CourseCode
            }).ToList();
        }
    }
}