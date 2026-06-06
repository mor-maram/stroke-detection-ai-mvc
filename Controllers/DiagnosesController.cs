namespace MediAI.Controllers
{
    public class DiagnosesController : _BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IStrokePredictionService _strokePredictionService;

        public DiagnosesController(ApplicationDbContext context,
            IStrokePredictionService strokePredictionService,
            ISystemLogService logService) : base(logService)
        {
            _context = context;
            _strokePredictionService = strokePredictionService;
        }

        [HttpPost]
        public async Task<IActionResult> Predict(Diagnosis diagnosis)
        {
            IFormFile imageFile = null;
            if (Request.Form.Files.Count > 0)
                imageFile = Request.Form.Files[0];

            if (imageFile == null || imageFile.Length == 0)
                return BadRequest("No file selected.");

            if (!imageFile.ContentType.StartsWith("image/"))
                return BadRequest("Invalid file type.");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
                await imageFile.CopyToAsync(stream);

            var relativePath = "/uploads/" + uniqueFileName;
            var prediction = await _strokePredictionService.PredictAsync(filePath);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            diagnosis.PatientId = userId;
            diagnosis.ImageUrl = relativePath;
            diagnosis.Status = DiagnosisStatus.NeedsReview;
            diagnosis.Condition = prediction.Condition;
            diagnosis.ResultIndex = prediction.ResultIndex;
            diagnosis.ConfidenceLevel = prediction.ResultRate;

            try
            {
                _context.Add(diagnosis);
                await _context.SaveChangesAsync();
                await _logService.LogAsync(this.GetUserId(), "Diagnosis_Predicted_AI", diagnosis.Id, HttpContext.GetUserIP());
            }
            catch (Exception ex)
            {
                return BadRequest("Error saving diagnosis: " + ex.Message);
            }

            return Ok(diagnosis.Deserialize<Diagnosis>());
        }

        public async Task<IActionResult> Index()
        {
            var diagnoses = await _context.Diagnosis.Include(d => d.Doctor).Include(d => d.Patient).ToListAsync();
            return View(diagnoses);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var diagnosis = await _context.Diagnosis
                .Include(d => d.Doctor)
                .Include(d => d.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (diagnosis == null) return NotFound();

            return View(diagnosis);
        }

        public IActionResult Create()
        {
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "Id", "Name");
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Diagnosis diagnosis)
        {
            if (ModelState.IsValid)
            {
                _context.Add(diagnosis);
                await _context.SaveChangesAsync();
                await _logService.LogAsync(this.GetUserId(),"Diagnosis_Created", diagnosis.Id, HttpContext.GetUserIP());
                return RedirectToAction(nameof(Index));
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "Id", "Name", diagnosis.DoctorId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "Name", diagnosis.PatientId);
            return View(diagnosis);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var diagnosis = await _context.Diagnosis.FindAsync(id);
            if (diagnosis == null) return NotFound();

            ViewData["DoctorId"] = new SelectList(_context.Doctors, "Id", "Name", diagnosis.DoctorId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "Name", diagnosis.PatientId);
            return View(diagnosis);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Diagnosis diagnosis)
        {
            if (id != diagnosis.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(diagnosis);
                    await _context.SaveChangesAsync();
                    await _logService.LogAsync(this.GetUserId(),"Diagnosis_Updated", diagnosis.Id, HttpContext.GetUserIP());
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiagnosisExists(diagnosis.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "Id", "Name", diagnosis.DoctorId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "Name", diagnosis.PatientId);
            return View(diagnosis);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var diagnosis = await _context.Diagnosis
                .Include(d => d.Doctor)
                .Include(d => d.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (diagnosis == null) return NotFound();

            return View(diagnosis);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var diagnosis = await _context.Diagnosis.FindAsync(id);
            if (diagnosis != null)
            {
                _context.Diagnosis.Remove(diagnosis);
                await _context.SaveChangesAsync();
                await _logService.LogAsync(this.GetUserId(),"Diagnosis_Deleted", id, HttpContext.GetUserIP());
            }
            return RedirectToAction(nameof(Index));
        }

        private bool DiagnosisExists(int id)
        {
            return _context.Diagnosis.Any(e => e.Id == id);
        }
    }
}
