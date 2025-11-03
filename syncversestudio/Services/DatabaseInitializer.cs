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
                    new Category { Name = "Soft Drinks", Description = "Carbonated and non-carbonated beverages", IsActive = true },
                    new Category { Name = "Beer & Alcohol", Description = "Alcoholic beverages and beer", IsActive = true },
                    new Category { Name = "Water", Description = "Bottled water and mineral water", IsActive = true },
                    new Category { Name = "Energy Drinks", Description = "Energy and sports drinks", IsActive = true },
                    new Category { Name = "Cosmetics", Description = "Beauty and personal care products", IsActive = true }
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
                        Name = "KRUD Khmer Beverages", 
                        ContactPerson = "Sok Pisey", 
                        Phone = "+855 23 720 123", 
                        Email = "sales@krudbeverage.com.kh",
                        Address = "Phnom Penh, Cambodia",
                        IsActive = true
                    },
                    new Supplier 
                    { 
                        Name = "Hanuman Trading Co", 
                        ContactPerson = "Chea Sophea", 
                        Phone = "+855 12 345 678", 
                        Email = "orders@hanuman.com.kh",
                        Address = "Siem Reap, Cambodia",
                        IsActive = true
                    },
                    new Supplier 
                    { 
                        Name = "Cambodia Cosmetics Supply", 
                        ContactPerson = "Lim Dara", 
                        Phone = "+855 92 888 999", 
                        Email = "contact@cambodiacosmetics.com",
                        Address = "Phnom Penh, Cambodia",
                        IsActive = true
                    },
                    new Supplier 
                    { 
                        Name = "Vital Water Cambodia", 
                        ContactPerson = "Pov Samnang", 
                        Phone = "+855 77 123 456", 
                        Email = "info@vitalwater.com.kh",
                        Address = "Battambang, Cambodia",
                        IsActive = true
                    },
                    new Supplier 
                    { 
                        Name = "Angkor Beer Distributor", 
                        ContactPerson = "Meas Chanthy", 
                        Phone = "+855 89 456 789", 
                        Email = "sales@angkorbeer.com.kh",
                        Address = "Kampong Cham, Cambodia",
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
                var softDrinksCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Soft Drinks");
                var beerCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Beer & Alcohol");
                var waterCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Water");
                var energyCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Energy Drinks");
                var cosmeticsCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Cosmetics");
                
                var krudBeverage = await context.Suppliers.FirstOrDefaultAsync(s => s.Name == "KRUD Khmer Beverages");
                var hanumanTrading = await context.Suppliers.FirstOrDefaultAsync(s => s.Name == "Hanuman Trading Co");
                var cambodiaCosmetics = await context.Suppliers.FirstOrDefaultAsync(s => s.Name == "Cambodia Cosmetics Supply");
                var vitalWater = await context.Suppliers.FirstOrDefaultAsync(s => s.Name == "Vital Water Cambodia");
                var angkorBeer = await context.Suppliers.FirstOrDefaultAsync(s => s.Name == "Angkor Beer Distributor");
                
                if (softDrinksCategory != null && beerCategory != null && krudBeverage != null && angkorBeer != null)
                {
                    var products = new[]
                    {
                        new Product
                        {
                            Name = "KRUD Soda 330ml",
                            Description = "Cambodian local soda drink",
                            Barcode = "8850999320101",
                            SKU = "KRUD-330",
                            CategoryId = softDrinksCategory.Id,
                            SupplierId = krudBeverage.Id,
                            CostPrice = 0.35m,
                            SellingPrice = 0.70m,
                            Quantity = 200,
                            MinQuantity = 50,
                            IsActive = true
                        },
                        new Product
                        {
                            Name = "Angkor Beer 330ml",
                            Description = "Cambodia's premium lager beer",
                            Barcode = "8850123456789",
                            SKU = "ANGKOR-330",
                            CategoryId = beerCategory.Id,
                            SupplierId = angkorBeer.Id,
                            CostPrice = 0.60m,
                            SellingPrice = 1.00m,
                            Quantity = 150,
                            MinQuantity = 40,
                            IsActive = true
                        },
                        new Product
                        {
                            Name = "Vital Water 500ml",
                            Description = "Pure drinking water",
                            Barcode = "8850234567890",
                            SKU = "VITAL-500",
                            CategoryId = waterCategory?.Id ?? softDrinksCategory.Id,
                            SupplierId = vitalWater?.Id ?? krudBeverage.Id,
                            CostPrice = 0.15m,
                            SellingPrice = 0.30m,
                            Quantity = 300,
                            MinQuantity = 100,
                            IsActive = true
                        },
                        new Product
                        {
                            Name = "Hanuman Energy Drink 250ml",
                            Description = "Local energy drink",
                            Barcode = "8850345678901",
                            SKU = "HANUMAN-250",
                            CategoryId = energyCategory?.Id ?? softDrinksCategory.Id,
                            SupplierId = hanumanTrading?.Id ?? krudBeverage.Id,
                            CostPrice = 0.70m,
                            SellingPrice = 1.20m,
                            Quantity = 120,
                            MinQuantity = 30,
                            IsActive = true
                        },
                        new Product
                        {
                            Name = "Khmer Beauty Cream 50g",
                            Description = "Natural beauty cream",
                            Barcode = "8850456789012",
                            SKU = "COSMETIC-001",
                            CategoryId = cosmeticsCategory?.Id ?? softDrinksCategory.Id,
                            SupplierId = cambodiaCosmetics?.Id ?? krudBeverage.Id,
                            CostPrice = 2.50m,
                            SellingPrice = 5.00m,
                            Quantity = 80,
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
