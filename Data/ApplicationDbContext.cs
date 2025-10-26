using Microsoft.EntityFrameworkCore;
using SyncVerseStudio.Models;

namespace SyncVerseStudio.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }
        public DbSet<InventoryMovement> InventoryMovements { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentLink> PaymentLinks { get; set; }
        public DbSet<HeldTransaction> HeldTransactions { get; set; }
        public DbSet<OnlineStoreIntegration> OnlineStoreIntegrations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=DESKTOP-6RCREN5\MSSQLSERVER01;Initial Catalog=POSDB;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=True;Application Name="SQL Server Management Studio";Command Timeout=0");
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

            // Configure Product entity \ TEST FROM MIDNANA
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

            // Configure Sale entity \ KAJSHKJDKSAHKDHKSH
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
        // GOLD OR BIT
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

            // Configure AuditLog entity
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithMany(p => p.AuditLogs)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Invoice entity
            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.HasIndex(e => e.InvoiceNumber).IsUnique();
                entity.Property(e => e.Status)
                    .HasConversion<string>()
                    .HasMaxLength(20);
                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.CreatedInvoices)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(d => d.VoidedByUser)
                    .WithMany(p => p.VoidedInvoices)
                    .HasForeignKey(d => d.VoidedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(d => d.Sale)
                    .WithOne(p => p.Invoice)
                    .HasForeignKey<Invoice>(d => d.SaleId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure InvoiceItem entity
            modelBuilder.Entity<InvoiceItem>(entity =>
            {
                entity.HasOne(d => d.Invoice)
                    .WithMany(p => p.InvoiceItems)
                    .HasForeignKey(d => d.InvoiceId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(d => d.Product)
                    .WithMany(p => p.InvoiceItems)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Payment entity
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasIndex(e => e.PaymentReference).IsUnique();
                entity.Property(e => e.PaymentMethod)
                    .HasConversion<string>()
                    .HasMaxLength(20);
                entity.Property(e => e.Status)
                    .HasConversion<string>()
                    .HasMaxLength(20);
                entity.HasOne(d => d.Invoice)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.InvoiceId)
                    .OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(d => d.Sale)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.SaleId)
                    .OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(d => d.ProcessedByUser)
                    .WithMany(p => p.ProcessedPayments)
                    .HasForeignKey(d => d.ProcessedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure PaymentLink entity
            modelBuilder.Entity<PaymentLink>(entity =>
            {
                entity.HasIndex(e => e.LinkCode).IsUnique();
                entity.Property(e => e.Status)
                    .HasConversion<string>()
                    .HasMaxLength(20);
                entity.HasOne(d => d.Invoice)
                    .WithMany()
                    .HasForeignKey(d => d.InvoiceId)
                    .OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.PaymentLinks)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(d => d.Payment)
                    .WithMany()
                    .HasForeignKey(d => d.PaymentId)
                    .OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.CreatedPaymentLinks)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure HeldTransaction entity
            modelBuilder.Entity<HeldTransaction>(entity =>
            {
                entity.HasIndex(e => e.TransactionCode).IsUnique();
                entity.HasOne(d => d.Customer)
                    .WithMany()
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(d => d.HeldByUser)
                    .WithMany(p => p.HeldTransactions)
                    .HasForeignKey(d => d.HeldByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure OnlineStoreIntegration entity
            modelBuilder.Entity<OnlineStoreIntegration>(entity =>
            {
                entity.Property(e => e.Platform)
                    .HasConversion<string>()
                    .HasMaxLength(50);
                entity.Property(e => e.LastSyncStatus)
                    .HasConversion<string>()
                    .HasMaxLength(20);
            });

            // Seed data
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "vi",
                    Password = "$2a$11$8K1p/a0dL8B9WvjqRJlqaOK9vF8JXw8tJ1K1K1K1K1K1K1K1K1K1K1",
                    Email = "vi@syncverse.com",
                    FirstName = "Vi",
                    LastName = "Admin",
                    Role = UserRole.Administrator,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
            );

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics", Description = "Electronic devices and accessories" },
                new Category { Id = 2, Name = "Beverages", Description = "Drinks and beverages" },
                new Category { Id = 3, Name = "Snacks", Description = "Snacks and confectionery" },
                new Category { Id = 4, Name = "Stationery", Description = "Office and school supplies" }
            );

            modelBuilder.Entity<Supplier>().HasData(
                new Supplier { Id = 1, Name = "Tech Solutions Ltd", ContactPerson = "John Smith", Phone = "+855123456789", Email = "contact@techsolutions.com" },
                new Supplier { Id = 2, Name = "Fresh Beverages Co", ContactPerson = "Mary Johnson", Phone = "+855987654321", Email = "orders@freshbev.com" }
            );
        }
    }
}