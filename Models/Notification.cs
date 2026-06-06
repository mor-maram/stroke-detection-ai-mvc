namespace MediAI.Models
{
    public class Notification
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public string? UserId { get; set; }
        public User? User { get; set; }

        [Required]
        public string? Title { get; set; }

        public string? Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsRead { get; set; } = false;
        public NotificationType Type { get; set; }
        public string? RelatedEntityType { get; set; } // e.g., "Diagnosis"
        public int RelatedEntityId { get; set; } // e.g., 123
    }

    public enum NotificationType
    {
        Info,
        Warning,
        Alert,
        Success,
        Error
    }
}
