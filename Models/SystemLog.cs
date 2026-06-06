namespace MediAI.Models
{
    public class SystemLog
    {
        public int Id { get; set; }

        [Required]
        public string? UserId { get; set; }  // أو اسم المستخدم إذا لم تكن تستخدم Identity

        public User? User { get; set; }

        [Required]
        public string? Action { get; set; } // مثال: "AddDoctor", "DeleteDiagnosis"

        public string? Details { get; set; } // تفاصيل إضافية

        public string? IPAddress { get; set; } // عنوان المستخدم

        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
