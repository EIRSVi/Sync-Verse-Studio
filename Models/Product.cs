using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyncVerseStudio.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Description { get; set; }

        [StringLength(50)]
        public string? Barcode { get; set; }

        [StringLength(50)]
        public string? SKU { get; set; }

        public int? CategoryId { get; set; }

        public int? SupplierId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CostPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SellingPrice { get; set; }

        public int Quantity { get; set; }

        public int MinQuantity { get; set; } = 10;

        [StringLength(255)]
        public string? ImagePath { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsSyncedToOnlineStore { get; set; } = false;

        public DateTime? LastSyncedAt { get; set; }

        [StringLength(100)]
        public string? OnlineStoreProductId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual Category? Category { get; set; }
        public virtual Supplier? Supplier { get; set; }
        public virtual ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
        public virtual ICollection<InventoryMovement> InventoryMovements { get; set; } = new List<InventoryMovement>();
        public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();

        [NotMapped]
        public bool IsLowStock => Quantity <= MinQuantity;

        [NotMapped]
        public decimal Profit => SellingPrice - CostPrice;

        [NotMapped]
        public decimal ProfitPercentage => CostPrice > 0 ? (Profit / CostPrice) * 100 : 0;
    }
}