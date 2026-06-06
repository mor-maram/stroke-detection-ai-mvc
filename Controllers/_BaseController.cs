namespace MediAI.Controllers
{
    public class _BaseController : Controller
    {
        protected readonly ISystemLogService _logService;

        public _BaseController(ISystemLogService logService)
        {
            _logService = logService;
        }

        protected async Task LogAsync(string action, string details)
        {
            var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Anonymous";
            var ip = HttpContext.Connection?.RemoteIpAddress?.ToString();

            await _logService.LogAsync(userId, action, details, ip);
        }

        protected string GetUserId()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                return userId;
            }
            return null;
        }
    }
}