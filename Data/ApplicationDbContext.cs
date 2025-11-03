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
        public DbSet<GeneralLedgerEntry> GeneralLedgerEntries { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseItem> PurchaseItems { get; set; }
        public DbSet<FinancialAccount> FinancialAccounts { get; set; }

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

            // Configure GeneralLedgerEntry entity
            modelBuilder.Entity<GeneralLedgerEntry>(entity =>
            {
                entity.HasIndex(e => e.EntryNumber).IsUnique();
                entity.Property(e => e.AccountType)
                    .HasConversion<string>()
                    .HasMaxLength(20);
                entity.Property(e => e.BookOfEntry)
                    .HasConversion<string>()
                    .HasMaxLength(30);
                entity.HasOne(d => d.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(d => d.RelatedSale)
                    .WithMany()
                    .HasForeignKey(d => d.RelatedSaleId)
                    .OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(d => d.RelatedPayment)
                    .WithMany()
                    .HasForeignKey(d => d.RelatedPaymentId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure Purchase entity
            modelBuilder.Entity<Purchase>(entity =>
            {
                entity.HasIndex(e => e.PurchaseNumber).IsUnique();
                entity.Property(e => e.Status)
                    .HasConversion<string>()
                    .HasMaxLength(20);
                entity.HasOne(d => d.Supplier)
                    .WithMany()
                    .HasForeignKey(d => d.SupplierId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(d => d.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(d => d.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure PurchaseItem entity
            modelBuilder.Entity<PurchaseItem>(entity =>
            {
                entity.HasOne(d => d.Purchase)
                    .WithMany(p => p.PurchaseItems)
                    .HasForeignKey(d => d.PurchaseId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(d => d.Product)
                    .WithMany()
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure FinancialAccount entity
            modelBuilder.Entity<FinancialAccount>(entity =>
            {
                entity.HasIndex(e => e.AccountCode).IsUnique();
                entity.Property(e => e.AccountType)
                    .HasConversion<string>()
                    .HasMaxLength(20);
                entity.Property(e => e.Category)
                    .HasConversion<string>()
                    .HasMaxLength(50);
            });

            // Seed data - Cambodia-based categories, suppliers, and products
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Soft Drinks", Description = "Carbonated and non-carbonated beverages", IsActive = true, CreatedAt = DateTime.Now },
                new Category { Id = 2, Name = "Beer & Alcohol", Description = "Alcoholic beverages and beer", IsActive = true, CreatedAt = DateTime.Now },
                new Category { Id = 3, Name = "Water", Description = "Bottled water and mineral water", IsActive = true, CreatedAt = DateTime.Now },
                new Category { Id = 4, Name = "Energy Drinks", Description = "Energy and sports drinks", IsActive = true, CreatedAt = DateTime.Now },
                new Category { Id = 5, Name = "Cosmetics", Description = "Beauty and personal care products", IsActive = true, CreatedAt = DateTime.Now }
            );

            modelBuilder.Entity<Supplier>().HasData(
                new Supplier 
                { 
                    Id = 1, 
                    Name = "KRUD Khmer Beverages", 
                    ContactPerson = "Sok Pisey", 
                    Phone = "+855 23 720 123", 
                    Email = "sales@krudbeverage.com.kh",
                    Address = "Phnom Penh, Cambodia",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new Supplier 
                { 
                    Id = 2, 
                    Name = "Hanuman Trading Co", 
                    ContactPerson = "Chea Sophea", 
                    Phone = "+855 12 345 678", 
                    Email = "orders@hanuman.com.kh",
                    Address = "Siem Reap, Cambodia",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new Supplier 
                { 
                    Id = 3, 
                    Name = "Cambodia Cosmetics Supply", 
                    ContactPerson = "Lim Dara", 
                    Phone = "+855 92 888 999", 
                    Email = "contact@cambodiacosmetics.com",
                    Address = "Phnom Penh, Cambodia",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new Supplier 
                { 
                    Id = 4, 
                    Name = "Vital Water Cambodia", 
                    ContactPerson = "Pov Samnang", 
                    Phone = "+855 77 123 456", 
                    Email = "info@vitalwater.com.kh",
                    Address = "Battambang, Cambodia",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new Supplier 
                { 
                    Id = 5, 
                    Name = "KRUD Beer Distributor", 
                    ContactPerson = "Meas Chanthy", 
                    Phone = "+855 89 456 789", 
                    Email = "sales@KRUDbeer.com.kh",
                    Address = "Kampong Cham, Cambodia",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "KRUD Soda 330ml",
                    Description = "Cambodian local soda drink",
                    Barcode = "8850999320101",
                    SKU = "KRUD-330",
                    CategoryId = 1,
                    SupplierId = 1,
                    CostPrice = 0.35m,
                    SellingPrice = 0.70m,
                    Quantity = 200,
                    MinQuantity = 50,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 2,
                    Name = "Angkor Beer 330ml",
                    Description = "Cambodia's premium lager beer",
                    Barcode = "8850123456789",
                    SKU = "ANGKOR-330",
                    CategoryId = 2,
                    SupplierId = 5,
                    CostPrice = 0.60m,
                    SellingPrice = 1.00m,
                    Quantity = 150,
                    MinQuantity = 40,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 3,
                    Name = "Vital Water 500ml",
                    Description = "Pure drinking water",
                    Barcode = "8850234567890",
                    SKU = "VITAL-500",
                    CategoryId = 3,
                    SupplierId = 4,
                    CostPrice = 0.15m,
                    SellingPrice = 0.30m,
                    Quantity = 300,
                    MinQuantity = 100,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 4,
                    Name = "Hanuman Energy Drink 250ml",
                    Description = "Local energy drink",
                    Barcode = "8850345678901",
                    SKU = "HANUMAN-250",
                    CategoryId = 4,
                    SupplierId = 2,
                    CostPrice = 0.70m,
                    SellingPrice = 1.20m,
                    Quantity = 120,
                    MinQuantity = 30,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new Product
                {
                    Id = 5,
                    Name = "Khmer Beauty Cream 50g",
                    Description = "Natural beauty cream",
                    Barcode = "8850456789012",
                    SKU = "COSMETIC-001",
                    CategoryId = 5,
                    SupplierId = 3,
                    CostPrice = 2.50m,
                    SellingPrice = 5.00m,
                    Quantity = 80,
                    MinQuantity = 20,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
            );

            modelBuilder.Entity<Customer>().HasData(
                new Customer
                {
                    Id = 1,
                    FirstName = "Guest",
                    LastName = "Customer",
                    Phone = "",
                    Email = "",
                    Address = "",
                    LoyaltyPoints = 0,
                    CreatedAt = DateTime.Now
                }
            );
        }
    }
}