using System.ComponentModel.DataAnnotations;

namespace SyncVerseStudio.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string? FirstName { get; set; }

        [StringLength(50)]
        public string? LastName { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

        public int LoyaltyPoints { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();

        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}