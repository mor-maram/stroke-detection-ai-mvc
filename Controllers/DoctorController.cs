namespace MediAI.Controllers
{
    //[Authorize(Roles = "Doctor")]
    public class DoctorController : _BaseController
    {
        private readonly ApplicationDbContext _context;

        public DoctorController(ApplicationDbContext context,
           ISystemLogService logService) : base(logService)
        {
            _context = context;
        }

        public async Task<IActionResult> DoctorDashboard()
        {
            var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var diagnoses = await _context.Diagnosis
                .Include(d => d.Patient)
                .Where(d => d.Status == DiagnosisStatus.Pending ||
                            d.Status == DiagnosisStatus.NeedsReview ||
                            d.Status == DiagnosisStatus.InProgress)
                .OrderByDescending(d => d.RequestDate)
                .ToListAsync();

            foreach (var item in diagnoses)
            {
                if (!string.IsNullOrWhiteSpace(item.PatientId) && item.Patient == null)
                    item.Patient = _context.Users.FirstOrDefault(p => p.Id == item.PatientId)?.ToPatient();

                if (!string.IsNullOrWhiteSpace(item.DoctorId) && item.Doctor == null)
                    item.Doctor = _context.Doctors.FirstOrDefault(p => p.Id == item.DoctorId)?.ToDoctor();
            }

            var viewModel = new DoctorDashboardViewModel
            {
                Diagnoses = diagnoses
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Review(int id)
        {
            var diagnosis = await _context.Diagnosis
                .Include(d => d.Patient)                
                .FirstOrDefaultAsync(d => d.Id == id);

            if (diagnosis == null)
                return NotFound();

            if (!string.IsNullOrWhiteSpace(diagnosis.PatientId) && diagnosis.Patient == null)
                diagnosis.Patient = _context.Users.FirstOrDefault(p => p.Id == diagnosis.PatientId)?.ToPatient();

            if (!string.IsNullOrWhiteSpace(diagnosis.DoctorId) && diagnosis.Doctor == null)
                diagnosis.Doctor = _context.Doctors.FirstOrDefault(p => p.Id == diagnosis.DoctorId)?.ToDoctor();

            var viewModel = new ReviewCaseViewModel
            {
                DiagnosisId = diagnosis.Id,
                PatientName = diagnosis.PatientName,
                Condition = diagnosis.Condition,
                DiagnosisDescription = diagnosis.DiagnosisDescription,
                ConfidenceLevel = diagnosis.ConfidenceLevel,
                Notes = diagnosis.Notes,
                TreatmentPlan = diagnosis.TreatmentPlan,
                ImageUrl = diagnosis.ImageUrl
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Review(ReviewCaseViewModel model)
        {
            var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var diagnosis = await _context.Diagnosis.FindAsync(model.DiagnosisId);
            if (diagnosis == null)
                return NotFound();
            diagnosis.DoctorId = doctorId;
            diagnosis.Condition = model.Condition;
            diagnosis.DiagnosisDescription = model.DiagnosisDescription;
            diagnosis.Notes = model.Notes;
            diagnosis.TreatmentPlan = model.TreatmentPlan;
            diagnosis.Status = DiagnosisStatus.Completed;
            diagnosis.DiagnosisDate = DateTime.Now;

            _context.Update(diagnosis);
            await _context.SaveChangesAsync();

            await _logService.LogAsync(this.GetUserId(), "Diagnosis_Reviewed", diagnosis.Id, HttpContext.GetUserIP());

            return RedirectToAction("DoctorDashboard");
        }

        [HttpGet("PendingCases")]
        public async Task<IActionResult> GetPendingCases()
        {
            var cases = await _context.Diagnosis
                .Where(r => r.Status == DiagnosisStatus.Pending || r.Status == DiagnosisStatus.NeedsReview)
                .Include(r => r.Patient)                
                .OrderByDescending(r => r.RequestDate)
                .Select(r => new PendingCaseDto
                {
                    RequestId = r.Id,
                    PatientName = r.PatientName,
                    RequestDate = r.RequestDate,
                    Status = r.Status.ToString(),                    
                    Priority = r.Priority.ToString()
                })
                .ToListAsync();

            return Ok(cases);
        }

        [HttpPost("cases/{id}/accept")]
        public async Task<IActionResult> AcceptCase(int id)
        {
            var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var request = await _context.Diagnosis.FindAsync(id);

            if (request == null)
                return NotFound();

            request.DoctorId = doctorId;
            request.Status = DiagnosisStatus.InProgress;

            await _context.SaveChangesAsync();
            await _logService.LogAsync(this.GetUserId(), "Diagnosis_Accepted", id, HttpContext.GetUserIP());

            return Ok();
        }

        [HttpPost("cases/{id}/diagnosis")]
        public async Task<IActionResult> AddDiagnosis(int id, [FromBody] DiagnosisInputDto dto)
        {
            var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var diagnosis = await _context.Diagnosis.FindAsync(id);
            if (diagnosis == null)
                return NotFound();

            diagnosis.DoctorId = doctorId;
            diagnosis.Condition = dto.Condition;
            diagnosis.DiagnosisDescription = dto.Description;
            diagnosis.ConfidenceLevel = dto.ConfidenceLevel;
            diagnosis.TreatmentPlan = dto.TreatmentPlan;
            diagnosis.Notes = dto.Notes;
            diagnosis.Status = DiagnosisStatus.Completed;
            diagnosis.DiagnosisDate = DateTime.Now;

            _context.Update(diagnosis);
            await _context.SaveChangesAsync();

            var report = GenerateMedicalReport(diagnosis);
            _context.MedicalReports.Add(report);
            await _context.SaveChangesAsync();

            await _logService.LogAsync(this.GetUserId(),"Diagnosis_Completed", id, HttpContext.GetUserIP());

            return Ok(new { ReportId = report.Id });
        }

        private MedicalReport GenerateMedicalReport(Diagnosis diagnosis)
        {
            return new MedicalReport
            {
                DiagnosisId = diagnosis.Id,
                PatientId = diagnosis.PatientId,
                ReportDate = DateTime.Now,
                Format = ReportFormat.PDF,
                Notes = "تم إنشاء التقرير تلقائياً"
            };
        }
    }
}