namespace MediAI.Controllers
{
    [Authorize]
    public class NotificationsController : _BaseController
    {
        private readonly ApplicationDbContext _context;

        public NotificationsController(ApplicationDbContext context,
           ISystemLogService logService) : base(logService)
        {
            _context = context;
        }

        /// <summary>
        /// الحصول على إشعارات المستخدم
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUserNotifications()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(20)
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Message = n.Message,
                    CreatedAt = n.CreatedAt,
                    IsRead = n.IsRead,
                    Type = n.Type,
                    RelatedEntityType = n.RelatedEntityType,
                    RelatedEntityId = n.RelatedEntityId
                })
                .ToListAsync();

            return Ok(notifications);
        }

        /// <summary>
        /// تعليم إشعار كمقروء
        /// </summary>
        [HttpPatch("{id}/mark-as-read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (notification == null)
                return NotFound();

            notification.IsRead = true;
            await _context.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// إرسال إشعار لجميع المستخدمين (للمشرف فقط)
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("broadcast")]
        public async Task<IActionResult> BroadcastNotification([FromBody] BroadcastNotificationDto notificationDto)
        {
            var users = await _context.Users.ToListAsync();

            foreach (var user in users)
            {
                _context.Notifications.Add(new Notification
                {
                    UserId = user.Id,
                    Title = notificationDto.Title,
                    Message = notificationDto.Message,
                    Type = NotificationType.Info,
                    CreatedAt = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();
            return Ok(new { Message = "تم إرسال الإشعار لجميع المستخدمين" });
        }
    }
}
