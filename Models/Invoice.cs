using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyncVerseStudio.Models
{
    public class Invoice
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string InvoiceNumber { get; set; } = string.Empty; // Format: #YYYYNNN

        public int? CustomerId { get; set; }

        [StringLength(100)]
        public string? CustomerName { get; set; } // For walk-in customers

        public int CreatedByUserId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PaidAmount { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal BalanceAmount { get; set; }

        public InvoiceStatus Status { get; set; } = InvoiceStatus.Active;

        public DateTime InvoiceDate { get; set; } = DateTime.Now;

        public DateTime? DueDate { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [StringLength(500)]
        public string? VoidReason { get; set; }

        public DateTime? VoidedAt { get; set; }

        public int? VoidedByUserId { get; set; }

        public int? SaleId { get; set; } // Link to Sale if created from POS

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual Customer? Customer { get; set; }
        public virtual User CreatedByUser { get; set; } = null!;
        public virtual User? VoidedByUser { get; set; }
        public virtual Sale? Sale { get; set; }
        public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

        [NotMapped]
        public bool IsFullyPaid => BalanceAmount <= 0;

        [NotMapped]
        public bool IsOverdue => DueDate.HasValue && DueDate.Value < DateTime.Now && Status == InvoiceStatus.Active;
    }

    public enum InvoiceStatus
    {
        Active,
        Paid,
        Void,
        Overdue
    }
}
