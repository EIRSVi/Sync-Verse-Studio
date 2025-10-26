using Microsoft.EntityFrameworkCore;
using SyncVerseStudio.Models;
using SyncVerseStudio.Helpers;

namespace SyncVerseStudio.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }
        public DbSet<InventoryMovement> InventoryMovements { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = DatabaseConnectionManager.GetConnectionString();
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured. Please configure it in the application.");
            }
            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Username).IsUnique();
                entity.Property(e => e.Role)
                    .HasConversion<string>()
                    .HasMaxLength(20);
            });

            // Configure Product entity
            //skfskjfksj 
            //jhjkhjkhkjhkh hhkhkjhkjhkh
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(e => e.Barcode).IsUnique();
                entity.HasIndex(e => e.SKU).IsUnique();
                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.SupplierId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure Sale entity
            modelBuilder.Entity<Sale>(entity =>
            {
                entity.HasIndex(e => e.InvoiceNumber).IsUnique();
                entity.Property(e => e.PaymentMethod)
                    .HasConversion<string>()
                    .HasMaxLength(20);
                entity.Property(e => e.Status)
                    .HasConversion<string>()
                    .HasMaxLength(20);
                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Sales)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(d => d.Cashier)
                    .WithMany(p => p.Sales)
                    .HasForeignKey(d => d.CashierId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure SaleItem entity
            modelBuilder.Entity<SaleItem>(entity =>
            {
                entity.HasOne(d => d.Sale)
                    .WithMany(p => p.SaleItems)
                    .HasForeignKey(d => d.SaleId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(d => d.Product)
                    .WithMany(p => p.SaleItems)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure InventoryMovement entity
            modelBuilder.Entity<InventoryMovement>(entity =>
            {
                entity.Property(e => e.MovementType)
                    .HasConversion<string>()
                    .HasMaxLength(20);
                entity.HasOne(d => d.Product)
                    .WithMany(p => p.InventoryMovements)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(d => d.User)
                    .WithMany(p => p.InventoryMovements)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure ProductImage entity
            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductImages)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => new { e.ProductId, e.IsPrimary });
            });

            // Configure AuditLog entity
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithMany(p => p.AuditLogs)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // NO DEFAULT SEED DATA - Admin can use seeder functions
        }
    }
}
