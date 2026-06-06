namespace MediAI.Services
{
    public interface ISystemLogService
    {
        Task LogAsync(string userId, string action, string details, string ipAddress);
        Task LogAsync(string userId, string action, int details, string ipAddress);

        Task<IEnumerable<SystemLog>> GetAllLogsAsync();
    }

    public class SystemLogService : ISystemLogService
    {
        private readonly ApplicationDbContext _context;

        public SystemLogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SystemLog>> GetAllLogsAsync()
        {
            return await _context.SystemLogs
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync();
        }

        public async Task LogAsync(string userId, string action, string details, string ipAddress)
        {
            var log = new SystemLog
            {
                UserId = userId,
                Action = action,
                Details = details,
                IPAddress = ipAddress,
                Timestamp = DateTime.Now
            };

            _context.SystemLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task LogAsync(string userId, string action, int details, string ipAddress)
        {
            await LogAsync(userId, action, details.ToString(), ipAddress);
        }
    }
}
