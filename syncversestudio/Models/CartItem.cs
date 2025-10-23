namespace SyncVerseStudio.Models
{
    /// <summary>
    /// Represents an item in the shopping cart for POS operations
    /// </summary>
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int MaxStock { get; set; }
        public decimal Total => (UnitPrice > 0 ? UnitPrice : Price) * Quantity;
    }
}
