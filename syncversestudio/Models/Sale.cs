using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyncVerseStudio.Models
{
    public class Sale
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string InvoiceNumber { get; set; } = string.Empty;

        public int? CustomerId { get; set; }

        public int CashierId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; } = 0;

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        public DateTime SaleDate { get; set; } = DateTime.Now;

        public SaleStatus Status { get; set; } = SaleStatus.Completed;

        // Navigation properties
        public virtual Customer? Customer { get; set; }
        public virtual User Cashier { get; set; } = null!;
        public virtual ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();

        [NotMapped]
        public decimal SubTotal => SaleItems?.Sum(si => si.TotalPrice) ?? 0;

        [NotMapped]
        public decimal GrandTotal => SubTotal + TaxAmount - DiscountAmount;

        [NotMapped]
        public int ItemCount => SaleItems?.Sum(si => si.Quantity) ?? 0;
    }

    public enum PaymentMethod
    {
        Cash,
        Card,
        Mobile,
        Mixed
    }

    public enum SaleStatus
    {
        Pending,
        Completed,
        Cancelled,
        Returned
    }
}
