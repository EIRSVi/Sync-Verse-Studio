using System.ComponentModel.DataAnnotations;

namespace SyncVerseStudio.Models
{
    public class InventoryMovement
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        [Required]
        public MovementType MovementType { get; set; }

        public int Quantity { get; set; }

        [StringLength(100)]
        public string? Reference { get; set; }

        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual Product Product { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }

    public enum MovementType
    {
        Sale,
        Purchase,
        Adjustment,
        Transfer,
        Return
    }
}
