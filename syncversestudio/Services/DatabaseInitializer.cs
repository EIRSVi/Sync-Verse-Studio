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
                
                // Check if admin user exists
                var adminUser = await context.Users.FirstOrDefaultAsync(u => u.Username == "vi");
                
                if (adminUser == null)
                {
                    // Create admin user with hashed password
                    adminUser = new User
                    {
                        Username = "vi",
                        Password = BCrypt.Net.BCrypt.HashPassword("vi"),
                        Email = "vi@syncverse.com",
                        FirstName = "Vi",
                        LastName = "Admin",
                        Role = UserRole.Administrator,
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    
                    context.Users.Add(adminUser);
                }
                else
                {
                    // Update password to ensure it's properly hashed
                    adminUser.Password = BCrypt.Net.BCrypt.HashPassword("vi");
                    context.Users.Update(adminUser);
                }
                
                // Initialize categories if they don't exist
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
                }
                
                // Initialize suppliers if they don't exist
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
                }
                
                // Initialize sample products if they don't exist
                if (!await context.Products.AnyAsync())
                {
                    await context.SaveChangesAsync(); // Save categories and suppliers first
                    
                    var electronicsCategory = await context.Categories.FirstAsync(c => c.Name == "Electronics");
                    var beveragesCategory = await context.Categories.FirstAsync(c => c.Name == "Beverages");
                    var snacksCategory = await context.Categories.FirstAsync(c => c.Name == "Snacks");
                    var stationeryCategory = await context.Categories.FirstAsync(c => c.Name == "Stationery");
                    
                    var techSupplier = await context.Suppliers.FirstAsync(s => s.Name == "Tech Solutions Ltd");
                    var beverageSupplier = await context.Suppliers.FirstAsync(s => s.Name == "Fresh Beverages Co");
                    
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
                        },
                        new Product
                        {
                            Name = "Notebook A4",
                            Description = "Ruled notebook 200 pages",
                            Barcode = "3456789012345",
                            SKU = "NOTE-A4-200",
                            CategoryId = stationeryCategory.Id,
                            SupplierId = techSupplier.Id,
                            CostPrice = 1.00m,
                            SellingPrice = 2.50m,
                            Quantity = 75,
                            MinQuantity = 15,
                            IsActive = true
                        },
                        new Product
                        {
                            Name = "Potato Chips",
                            Description = "Original flavor potato chips",
                            Barcode = "4567890123456",
                            SKU = "CHIPS-001",
                            CategoryId = snacksCategory.Id,
                            SupplierId = beverageSupplier.Id,
                            CostPrice = 0.75m,
                            SellingPrice = 1.50m,
                            Quantity = 80,
                            MinQuantity = 20,
                            IsActive = true
                        }
                    };
                    
                    context.Products.AddRange(products);
                }
                
                // Initialize guest customer if doesn't exist
                if (!await context.Customers.AnyAsync())
                {
                    var guestCustomer = new Customer
                    {
                        FirstName = "Guest",
                        LastName = "Customer",
                        Phone = "",
                        Email = "",
                        LoyaltyPoints = 0
                    };
                    
                    context.Customers.Add(guestCustomer);
                }
                
                await context.SaveChangesAsync();
                Console.WriteLine("Database initialized successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing database: {ex.Message}");
                throw;
            }
        }
    }
}
