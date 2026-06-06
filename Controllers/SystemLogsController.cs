namespace MediAI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SystemLogsController : _BaseController
    {
        public SystemLogsController(ISystemLogService logService) : base(logService)
        {
          
        }

        public async Task<IActionResult> Index()
        {
            var logs = await _logService.GetAllLogsAsync();
            return View(logs);
        }
    }
}