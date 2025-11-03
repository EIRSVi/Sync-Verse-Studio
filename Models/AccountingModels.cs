using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyncVerseStudio.Models
{
    // General Ledger Entry
    public class GeneralLedgerEntry
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string EntryNumber { get; set; } = string.Empty;

        public DateTime EntryDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(100)]
        public string AccountName { get; set; } = string.Empty;

        public AccountType AccountType { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DebitAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CreditAmount { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(50)]
        public string? ReferenceNumber { get; set; }

        public BookOfEntry BookOfEntry { get; set; }

        public int? RelatedSaleId { get; set; }
        public int? RelatedPurchaseId { get; set; }
        public int? RelatedPaymentId { get; set; }

        public int CreatedByUserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual User CreatedByUser { get; set; } = null!;
        public virtual Sale? RelatedSale { get; set; }
        public virtual Payment? RelatedPayment { get; set; }
    }

    // Purchase Order for tracking purchases
    public class Purchase
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string PurchaseNumber { get; set; } = string.Empty;

        public int SupplierId { get; set; }

        public DateTime PurchaseDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PaidAmount { get; set; }

        public PurchaseStatus Status { get; set; } = PurchaseStatus.Pending;

        [StringLength(500)]
        public string? Notes { get; set; }

        public int CreatedByUserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual Supplier Supplier { get; set; } = null!;
        public virtual User CreatedByUser { get; set; } = null!;
        public virtual ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
    }

    public class PurchaseItem
    {
        public int Id { get; set; }
        public int PurchaseId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitCost { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalCost { get; set; }

        // Navigation properties
        public virtual Purchase Purchase { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }

    // Financial Statement Accounts
    public class FinancialAccount
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string AccountCode { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string AccountName { get; set; } = string.Empty;

        public AccountType AccountType { get; set; }

        public FinancialStatementCategory Category { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CurrentBalance { get; set; }

        public bool IsActive { get; set; } = true;

        [StringLength(500)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    // Enums
    public enum AccountType
    {
        Asset,
        Liability,
        Equity,
        Revenue,
        Expense
    }

    public enum FinancialStatementCategory
    {
        // Assets
        CurrentAssets,
        FixedAssets,
        IntangibleAssets,
        
        // Liabilities
        CurrentLiabilities,
        LongTermLiabilities,
        
        // Equity
        OwnersEquity,
        RetainedEarnings,
        
        // Income
        SalesRevenue,
        OtherIncome,
        
        // Expenses
        CostOfGoodsSold,
        OperatingExpenses,
        OtherExpenses
    }

    public enum BookOfEntry
    {
        SalesDayBook,
        PurchasesDayBook,
        CashBook,
        GeneralJournal
    }

    public enum PurchaseStatus
    {
        Pending,
        Completed,
        Cancelled
    }
}
