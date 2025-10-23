using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SyncVerseStudio.Helpers;

namespace SyncVerseStudio.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string? FirstName { get; set; }

        [StringLength(50)]
        public string? LastName { get; set; }

        [StringLength(500)]
        public string? Phone { get; set; } // Encrypted - increased length

        [StringLength(500)]
        public string? Email { get; set; } // Encrypted - increased length

        [StringLength(255)]
        public string? Address { get; set; }

        public int LoyaltyPoints { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();

        public string FullName => $"{FirstName} {LastName}".Trim();
        
        // Decrypted properties for display
        [NotMapped]
        public string? DecryptedPhone => !string.IsNullOrEmpty(Phone) ? EncryptionHelper.Decrypt(Phone) : null;
        
        [NotMapped]
        public string? DecryptedEmail => !string.IsNullOrEmpty(Email) ? EncryptionHelper.Decrypt(Email) : null;
        
        [NotMapped]
        public string? MaskedPhone => !string.IsNullOrEmpty(Phone) ? EncryptionHelper.MaskPhone(DecryptedPhone ?? "") : null;
        
        [NotMapped]
        public string? MaskedEmail => !string.IsNullOrEmpty(Email) ? EncryptionHelper.MaskEmail(DecryptedEmail ?? "") : null;
    }
}
