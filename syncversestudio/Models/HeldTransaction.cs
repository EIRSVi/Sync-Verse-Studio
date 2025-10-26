using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyncVerseStudio.Models
{
    public class HeldTransaction
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string TransactionCode { get; set; } = string.Empty;

        public int? CustomerId { get; set; }

        [StringLength(100)]
        public string? CustomerName { get; set; }

        public int HeldByUserId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        public string CartItemsJson { get; set; } = string.Empty; // JSON serialized cart items

        [StringLength(500)]
        public string? Notes { get; set; }

        public DateTime HeldAt { get; set; } = DateTime.Now;

        public DateTime? ResumedAt { get; set; }

        public bool IsCompleted { get; set; } = false;

        // Navigation properties
        public virtual Customer? Customer { get; set; }
        public virtual User HeldByUser { get; set; } = null!;
    }
}
