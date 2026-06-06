namespace MediAI.DTOs
{
    // نماذج نقل البيانات (DTOs)
    public class LoginDto
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    public class RegisterPatientDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string? BloodType { get; set; }
    }

    public class CreateDoctorDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? PhoneNumber { get; set; }        
        public string? LicenseNumber { get; set; }
        public int? YearsOfExperience { get; set; }
    }

    public class CreateDiagnosisDto
    {
        public string? Description { get; set; }
        public List<IFormFile> Images { get; set; }
    }

    public class DiagnosisInputDto
    {
        public string? Condition { get; set; }
        public string? Description { get; set; }
        public decimal ConfidenceLevel { get; set; }
        public string? TreatmentPlan { get; set; }
        public string? Notes { get; set; }
    }

    public class GenerateReportDto
    {
        public int DiagnosisId { get; set; }
        public ReportFormat Format { get; set; }
        public string? Notes { get; set; }
    }

    public class DiagnosisDto
    {
        public int Id { get; set; }
        //public PatientDto Patient { get; set; }
        //public DoctorDto Doctor { get; set; }
        public DateTime RequestDate { get; set; }
        //public string Description { get; set; }
        public DiagnosisStatus Status { get; set; }
        public PriorityLevel Priority { get; set; }

        //public List<SymptomDto> Symptoms { get; set; }        
        //public DiagnosisDto Diagnosis { get; set; }
        public string? DoctorName { get; set; }
        public string? Result { get; set; }
    }

    public class NotificationDto
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public string? UserId { get; set; }
        //public User User { get; set; }

        [Required]
        public string? Title { get; set; }

        public string? Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool  IsRead { get; set; } = false;
        public NotificationType Type { get; set; }
        public string? RelatedEntityType { get; set; } // e.g., "Diagnosis"
        public int RelatedEntityId { get; set; } // e.g., 123
    }

    public class BroadcastNotificationDto
    {
        /*public int Id { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public User User { get; set; }

        [Required]*/
        public string? Title { get; set; }

        public string? Message { get; set; }
        /*public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsRead { get; set; } = false;
        public NotificationType Type { get; set; }
        public string RelatedEntityType { get; set; } // e.g., "Diagnosis"
        public int RelatedEntityId { get; set; } // e.g., 123*/
    }

    public class PendingCaseDto
    {
        public int RequestId { get; set; }
        public string? PatientName { get; set; }        
        public DateTime RequestDate { get; set; }                
        public string? Priority { get; set; } // "Low", "Medium", "High"
        public string? Status { get; set; } // "Pending", "InReview", "Completed"
        public int ImageCount { get; set; }
    }

    public class SymptomDto
    {
        public string? Name { get; set; }
        public string? Severity { get; set; } // "Mild", "Moderate", "Severe"
        public string? Description { get; set; }
    }

    public class ToggleStatusDto
    {
        [Required]
        public int Id { get; set; } // Could be UserId, RequestId, etc.

        [Required]
        public string EntityType { get; set; } // "User", "Diagnosis", etc.

        public bool NewStatus { get; set; } = true;// For boolean toggles
        public string NewStatusValue { get; set; } // For enum/string statuses

        // Optional reason for status change (especially for rejections)
        public string Reason { get; set; }
    }

    public class UploadImageDto
    {
        [Required]
        public int RequestId { get; set; }

        [Required]
        public IFormFile ImageFile { get; set; }

        public string? ImageType { get; set; } // "MRI", "XRay", "CTScan", etc.
        public string? Notes { get; set; }
    }

    // For multiple image uploads
    public class UploadMultipleImagesDto
    {
        [Required]
        public int RequestId { get; set; }

        [Required]
        public List<IFormFile> ImageFiles { get; set; } = new List<IFormFile>();

        public string? ImageType { get; set; }
    }

    // Response after upload
    public class UploadImageResponseDto
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? ImageUrl { get; set; }
        public int ImageId { get; set; }
    }

    public class UserDto
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public UserType UserType { get; set; } // "Patient", "Doctor", "Admin"
        public string? ProfileImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }

        // Patient specific
        public string? BloodType { get; set; }
        public double? Height { get; set; }
        public double? Weight { get; set; }

        // Doctor specific
        public string? Specialization { get; set; }
        public string? LicenseNumber { get; set; }
        public int? YearsOfExperience { get; set; }

        // Admin specific
        public string? Department { get; set; }
        public string? AccessLevel { get; set; }
    }

    // For creating/updating users
    public class CreateUserDto
    {
        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "كلمة المرور وتأكيدها غير متطابقين")]
        public string? ConfirmPassword { get; set; }

        public string? PhoneNumber { get; set; }
        public UserType? UserType { get; set; }

        // Additional fields based on user type
        public string? BloodType { get; set; }                
    }

    public class EditUserDto
    {
        [Required]
        public string? Id { get; set; }

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "بريد إلكتروني غير صالح")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "يجب تحديد نوع المستخدم")]
        public UserType UserType { get; set; } // Admin أو Doctor
    }

    // For user login
    /*public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }*/
}
