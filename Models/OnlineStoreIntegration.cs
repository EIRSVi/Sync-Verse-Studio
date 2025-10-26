using System.ComponentModel.DataAnnotations;

namespace SyncVerseStudio.Models
{
    public class OnlineStoreIntegration
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string StoreName { get; set; } = string.Empty;

        public StorePlatform Platform { get; set; }

        [StringLength(500)]
        public string? ApiKey { get; set; }

        [StringLength(500)]
        public string? ApiSecret { get; set; }

        [StringLength(500)]
        public string? StoreUrl { get; set; }

        [StringLength(500)]
        public string? WebhookUrl { get; set; }

        public bool IsEnabled { get; set; } = true;

        public DateTime? LastSyncDate { get; set; }

        public SyncStatus LastSyncStatus { get; set; } = SyncStatus.Never;

        [StringLength(500)]
        public string? LastSyncMessage { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    public enum StorePlatform
    {
        Custom,
        Shopify,
        WooCommerce,
        Magento,
        Other
    }

    public enum SyncStatus
    {
        Never,
        Success,
        Failed,
        InProgress
    }
}
