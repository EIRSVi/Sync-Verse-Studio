using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyncVerseStudio.Models
{
    public class PaymentLink
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string LinkCode { get; set; } = string.Empty; // Unique code for URL

        public int? InvoiceId { get; set; }

        public int? CustomerId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [StringLength(200)]
        public string? Description { get; set; }

        public PaymentLinkStatus Status { get; set; } = PaymentLinkStatus.Active;

        public DateTime ExpiryDate { get; set; }

        public DateTime? PaidAt { get; set; }

        public int? PaymentId { get; set; }

        public int CreatedByUserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual Invoice? Invoice { get; set; }
        public virtual Customer? Customer { get; set; }
        public virtual Payment? Payment { get; set; }
        public virtual User CreatedByUser { get; set; } = null!;

        [NotMapped]
        public bool IsExpired => DateTime.Now > ExpiryDate;

        [NotMapped]
        public string FullUrl => $"https://syncverse.com/pay/{LinkCode}";
    }

    public enum PaymentLinkStatus
    {
        Active,
        Paid,
        Expired,
        Cancelled
    }
}
