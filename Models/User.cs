using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SyncVerseStudio.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; }

        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

        // Navigation properties
        public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
        public virtual ICollection<InventoryMovement> InventoryMovements { get; set; } = new List<InventoryMovement>();
        public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
        public virtual ICollection<Invoice> CreatedInvoices { get; set; } = new List<Invoice>();
        public virtual ICollection<Invoice> VoidedInvoices { get; set; } = new List<Invoice>();
        public virtual ICollection<Payment> ProcessedPayments { get; set; } = new List<Payment>();
        public virtual ICollection<PaymentLink> CreatedPaymentLinks { get; set; } = new List<PaymentLink>();
        public virtual ICollection<HeldTransaction> HeldTransactions { get; set; } = new List<HeldTransaction>();
    }

    public enum UserRole
    {
        Administrator,
        Cashier,
        InventoryClerk
    }
}