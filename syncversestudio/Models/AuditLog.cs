using System.ComponentModel.DataAnnotations;

namespace SyncVerseStudio.Models
{
    public class AuditLog
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Action { get; set; } = string.Empty;

        [StringLength(50)]
        public string? TableName { get; set; }

        public int? RecordId { get; set; }

        public string? OldValues { get; set; }

        public string? NewValues { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

        [StringLength(45)]
        public string? IpAddress { get; set; }

        // Navigation properties
        public virtual User User { get; set; } = null!;
    }
}