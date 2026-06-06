namespace MediAI.Controllers
{
    public class SymptomsController : _BaseController
    {
        private readonly ApplicationDbContext _context;

        public SymptomsController(ApplicationDbContext context, ISystemLogService logService)
            : base(logService)
        {
            _context = context;
        }

        public async Task<IActionResult> GetSymptoms()
        {
            var result = await _context.Symptoms.ToListAsync();
            return Ok(result);
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Symptoms.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var symptom = await _context.Symptoms.FirstOrDefaultAsync(m => m.Id == id);
            if (symptom == null) return NotFound();

            return View(symptom);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Severity,Description")] Symptom symptom)
        {
            if (ModelState.IsValid)
            {
                _context.Add(symptom);
                await _context.SaveChangesAsync();
                await _logService.LogAsync(this.GetUserId(), "Symptom_Created", symptom.Id.ToString(), HttpContext.GetUserIP());
                return RedirectToAction(nameof(Index));
            }
            return View(symptom);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var symptom = await _context.Symptoms.FindAsync(id);
            if (symptom == null) return NotFound();

            return View(symptom);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Severity,Description")] Symptom symptom)
        {
            if (id != symptom.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(symptom);
                    await _context.SaveChangesAsync();
                    await _logService.LogAsync(this.GetUserId(), "Symptom_Updated", symptom.Id.ToString(), HttpContext.GetUserIP());
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SymptomExists(symptom.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(symptom);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var symptom = await _context.Symptoms.FirstOrDefaultAsync(m => m.Id == id);
            if (symptom == null) return NotFound();

            return View(symptom);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var symptom = await _context.Symptoms.FindAsync(id);
            if (symptom != null)
            {
                _context.Symptoms.Remove(symptom);
                await _logService.LogAsync(this.GetUserId(), "Symptom_Deleted", id.ToString(), HttpContext.GetUserIP());
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SymptomExists(int id)
        {
            return _context.Symptoms.Any(e => e.Id == id);
        }
    }
}