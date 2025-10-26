using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyncVerseStudio.Models
{
    public class Payment
    {
        public int Id { get; set; }

        public int? InvoiceId { get; set; }

        public int? SaleId { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentReference { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public PaymentMethodType PaymentMethod { get; set; }

        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        [StringLength(100)]
        public string? TransactionId { get; set; } // Gateway transaction ID

        [StringLength(50)]
        public string? PaymentGateway { get; set; } // Stripe, PayPal, etc.

        [StringLength(500)]
        public string? Notes { get; set; }

        [StringLength(500)]
        public string? FailureReason { get; set; }

        public int ProcessedByUserId { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.Now;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual Invoice? Invoice { get; set; }
        public virtual Sale? Sale { get; set; }
        public virtual User ProcessedByUser { get; set; } = null!;
    }

    public enum PaymentMethodType
    {
        Cash,
        Card,
        Online,
        BankTransfer,
        Mobile
    }

    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed,
        Refunded,
        Cancelled
    }
}
