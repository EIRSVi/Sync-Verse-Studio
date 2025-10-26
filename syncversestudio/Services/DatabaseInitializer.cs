using SyncVerseStudio.Data;
using SyncVerseStudio.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace SyncVerseStudio.Services
{
    public class DatabaseInitializer
    {
        public static async Task InitializeAsync()
        {
            try
            {
                using var context = new ApplicationDbContext();
                
                // Ensure database is created
                await context.Database.EnsureCreatedAsync();
                
                Console.WriteLine("Database initialized successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing database: {ex.Message}");
                throw;
            }
        }

        // Manual seeder for categories - Admin can call this
        public static async Task SeedCategoriesAsync()
        {
            using var context = new ApplicationDbContext();
            
            if (!await context.Categories.AnyAsync())
            {
                var categories = new[]
                {
                    new Category { Name = "Electronics", Description = "Electronic devices and accessories", IsActive = true },
                    new Category { Name = "Beverages", Description = "Drinks and beverages", IsActive = true },
                    new Category { Name = "Snacks", Description = "Snacks and confectionery", IsActive = true },
                    new Category { Name = "Stationery", Description = "Office and school supplies", IsActive = true }
                };
                
                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();
            }
        }

        // Manual seeder for suppliers - Admin can call this
        public static async Task SeedSuppliersAsync()
        {
            using var context = new ApplicationDbContext();
            
            if (!await context.Suppliers.AnyAsync())
            {
                var suppliers = new[]
                {
                    new Supplier 
                    { 
                        Name = "Tech Solutions Ltd", 
                        ContactPerson = "John Smith", 
                        Phone = "+855123456789", 
                        Email = "contact@techsolutions.com",
                        IsActive = true
                    },
                    new Supplier 
                    { 
                        Name = "Fresh Beverages Co", 
                        ContactPerson = "Mary Johnson", 
                        Phone = "+855987654321", 
                        Email = "orders@freshbev.com",
                        IsActive = true
                    }
                };
                
                context.Suppliers.AddRange(suppliers);
                await context.SaveChangesAsync();
            }
        }

        // Manual seeder for sample products - Admin can call this
        public static async Task SeedProductsAsync()
        {
            using var context = new ApplicationDbContext();
            
            if (!await context.Products.AnyAsync())
            {
                var electronicsCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Electronics");
                var beveragesCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Beverages");
                var snacksCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Snacks");
                var stationeryCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Stationery");
                
                var techSupplier = await context.Suppliers.FirstOrDefaultAsync(s => s.Name == "Tech Solutions Ltd");
                var beverageSupplier = await context.Suppliers.FirstOrDefaultAsync(s => s.Name == "Fresh Beverages Co");
                
                if (electronicsCategory != null && beveragesCategory != null && techSupplier != null && beverageSupplier != null)
                {
                    var products = new[]
                    {
                        new Product
                        {
                            Name = "USB Cable Type-C",
                            Description = "1-meter USB-C charging cable",
                            Barcode = "1234567890123",
                            SKU = "USB-C-001",
                            CategoryId = electronicsCategory.Id,
                            SupplierId = techSupplier.Id,
                            CostPrice = 2.50m,
                            SellingPrice = 5.00m,
                            Quantity = 50,
                            MinQuantity = 10,
                            IsActive = true
                        },
                        new Product
                        {
                            Name = "Coca Cola 330ml",
                            Description = "Classic Coca Cola can",
                            Barcode = "2345678901234",
                            SKU = "COKE-330",
                            CategoryId = beveragesCategory.Id,
                            SupplierId = beverageSupplier.Id,
                            CostPrice = 0.50m,
                            SellingPrice = 1.00m,
                            Quantity = 100,
                            MinQuantity = 20,
                            IsActive = true
                        }
                    };
                    
                    context.Products.AddRange(products);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
