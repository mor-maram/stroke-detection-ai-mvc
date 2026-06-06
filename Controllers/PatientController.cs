namespace MediAI.Controllers
{
    //[Authorize(Roles = "Patient")]
    public class PatientController : _BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public PatientController(ApplicationDbContext context, IWebHostEnvironment env,
           ISystemLogService logService) : base(logService)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> PatientDashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var diagnoses = await _context.Diagnosis
                .Where(d => d.PatientId == userId)
                .OrderByDescending(d => d.RequestDate)
                .Take(10)
                .ToListAsync();

            return View(diagnoses);
        }

        /// <summary>
        /// الحصول على جميع طلبات التشخيص للمريض
        /// </summary>      
        public async Task<IActionResult> GetDiagnosiss()
        {
            var patientId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var diagnosiss = await _context.Diagnosis
                .Where(r => r.PatientId == patientId)                
                .OrderByDescending(r => r.RequestDate)
                .Select(r => new DiagnosisDto
                {
                    Id = r.Id,
                    RequestDate = r.RequestDate,
                    Status = r.Status,
                    DoctorName = r.Doctor != null ? $"{r.Doctor.FirstName} {r.Doctor.LastName}" : null,
                    //Result = r.Diagnosis != null ? r.Diagnosis.Condition : null
                })
                .ToListAsync();

            return Ok(diagnosiss);
        }

        /// <summary>
        /// الحصول على تفاصيل طلب تشخيص معين
        /// </summary>        
        public async Task<IActionResult> GetDiagnosisDetails(int id)
        {
            var patientId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var request = await _context.Diagnosis                
                //.Include(r => r.Symptoms)
                //.Include(r => r.Diagnosis)
                //.ThenInclude(dr => dr.MedicalReport)
                .FirstOrDefaultAsync(r => r.Id == id && r.PatientId == patientId);

            if (request == null)
                return NotFound();

            return Ok(request);
        }

        private async Task<string> SaveImage(IFormFile image)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return $"/uploads/{uniqueFileName}";
        }

        public async Task<IActionResult> MyDiagnoses()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var diagnoses = await _context.Diagnosis
                .Include(d => d.Doctor)
                .Include(d => d.Patient)
                .Where(d=>d.PatientId== userId)
                .ToListAsync();
            return View(diagnoses);
        }

        public async Task<IActionResult> MyDiagnosisDetails(int? id)
        {
            if (id == null) return NotFound();

            var diagnosis = await _context.Diagnosis
                .Include(d => d.Doctor)
                .Include(d => d.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (diagnosis == null) return NotFound();

            return View(diagnosis);
        }
    }
}