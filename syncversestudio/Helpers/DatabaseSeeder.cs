using SyncVerseStudio.Data;
using SyncVerseStudio.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace SyncVerseStudio.Helpers
{
    public static class DatabaseSeeder
    {
        public static async Task SeedSuppliersAsync(ApplicationDbContext context)
        {
            var suppliers = new List<string>
            {
                "Amru Rice (Cambodia) Co., Ltd",
                "Aprati Foods (Cambodia) Ltd",
                "Aprati Foods Asia",
                "BAYON BAKERY",
                "ELITE FOOD Co., Ltd.",
                "Fair Farms - Kampot Jewels",
                "GOGO FOOD",
                "Graindorge Pastry & Catering",
                "Indoguna Cambodia",
                "Jinkao Farm",
                "Khemara Kampot Pepper PGI",
                "Khmer Food Co., Ltd",
                "Khmer Foods Group",
                "La Plantation - Fair Spices Producer",
                "Lees Food Service",
                "MISOTA FOOD IMPORT EXPORT CO LTD",
                "Phnom Penh Water Supply Authority",
                "Specialized Cambodian Produces",
                "TH F&B Co., Ltd",
                "VISSOT Co., Ltd"
            };

            foreach (var supplierName in suppliers)
            {
                var exists = await context.Suppliers.AnyAsync(s => s.Name == supplierName);
                if (!exists)
                {
                    context.Suppliers.Add(new Supplier
                    {
                        Name = supplierName,
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    });
                }
            }

            await context.SaveChangesAsync();
        }

        public static async Task SeedCategoriesAsync(ApplicationDbContext context)
        {
            var categories = new List<string>
            {
                "Coca-Cola",
                "Khmer Beverages",
                "Vattanac Beverages",
                "Dairy Products",
                "Bakery & Pastry",
                "Rice & Grains",
                "Spices & Seasonings",
                "Fresh Produce",
                "Meat & Poultry",
                "Seafood",
                "Frozen Foods",
                "Canned Goods",
                "Snacks & Confectionery",
                "Water & Soft Drinks",
                "Alcoholic Beverages",
                "Condiments & Sauces",
                "Cooking Oil",
                "Household Items",
                "Personal Care",
                "Cleaning Supplies"
            };

            foreach (var categoryName in categories)
            {
                var exists = await context.Categories.AnyAsync(c => c.Name == categoryName);
                if (!exists)
                {
                    context.Categories.Add(new Category
                    {
                        Name = categoryName,
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    });
                }
            }

            await context.SaveChangesAsync();
        }

        public static async Task SeedCustomersAsync(ApplicationDbContext context)
        {
            // SECURITY NOTE: Customer emails and phones are encrypted before storage
            // The actual data is only visible when decrypted in the application interface
            // Plain text data is encrypted using EncryptionHelper before being added to database
            
            var customersPlainData = new List<(string Email, string Phone, string FirstName, string LastName, string? FacebookId)>
            {
                // Sample customer data - will be encrypted before storage
                ("customer1@example.com", "0123456789", "Phan", "Danuphon", "FB001"),
                ("customer2@example.com", "0987654321", "Samnang", "Dy", "FB002"),
                ("customer3@example.com", "0111222333", "Sokla", "Choeun", "FB003"),
                ("customer4@example.com", "0444555666", "Channa", "Phork", "FB004"),
                ("customer5@example.com", "0777888999", "Vinthyratthanak", "Pha", "FB005")
            };

            foreach (var (email, phone, firstName, lastName, facebookId) in customersPlainData)
            {
                // Encrypt sensitive data before checking and storing
                var encryptedEmail = EncryptionHelper.Encrypt(email);
                var encryptedPhone = EncryptionHelper.Encrypt(phone);
                
                // Check if customer already exists
                var allCustomers = await context.Customers.ToListAsync();
                var exists = allCustomers.Any(c => 
                    c.DecryptedEmail == email || c.DecryptedPhone == phone);
                
                if (!exists)
                {
                    context.Customers.Add(new Customer
                    {
                        Email = encryptedEmail,  // Stored encrypted
                        Phone = encryptedPhone,  // Stored encrypted
                        FirstName = firstName,
                        LastName = lastName,
                        Address = facebookId != null ? $"Facebook ID: {facebookId}" : null,
                        LoyaltyPoints = 0,
                        CreatedAt = DateTime.Now
                    });
                }
            }

            await context.SaveChangesAsync();
        }

        public static async Task SeedProductsAsync(ApplicationDbContext context)
        {
            // Get categories and suppliers
            var beverageCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Coca-Cola");
            var khmerBevCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Khmer Beverages");
            var snacksCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Snacks & Confectionery");
            var dairyCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Dairy Products");
            var riceCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Rice & Grains");
            
            var supplier1 = await context.Suppliers.FirstOrDefaultAsync(s => s.Name.Contains("GOGO FOOD"));
            var supplier2 = await context.Suppliers.FirstOrDefaultAsync(s => s.Name.Contains("Amru Rice"));
            var supplier3 = await context.Suppliers.FirstOrDefaultAsync(s => s.Name.Contains("BAYON BAKERY"));

            var products = new List<(string Name, string Description, string Barcode, decimal Cost, decimal Price, int Qty, int? CategoryId, int? SupplierId, string? ImageName)>
            {
                ("Coca-Cola 330ml", "Classic Coca-Cola Can 330ml", "8850999320019", 0.50m, 1.00m, 500, beverageCategory?.Id, supplier1?.Id, "coca-cola-330ml.jpg"),
                ("Pepsi 330ml", "Pepsi Cola Can 330ml", "8850999320026", 0.45m, 0.95m, 450, beverageCategory?.Id, supplier1?.Id, "pepsi-330ml.jpg"),
                ("Sprite 330ml", "Sprite Lemon-Lime Can 330ml", "8850999320033", 0.45m, 0.95m, 400, beverageCategory?.Id, supplier1?.Id, "sprite-330ml.jpg"),
                ("Fanta Orange 330ml", "Fanta Orange Can 330ml", "8850999320040", 0.45m, 0.95m, 380, beverageCategory?.Id, supplier1?.Id, "fanta-330ml.jpg"),
                ("Sting Energy Drink", "Sting Energy Drink 330ml", "8850999320057", 0.60m, 1.20m, 300, khmerBevCategory?.Id, supplier1?.Id, "sting-energy.jpg"),
                ("Number 1 Energy Drink", "Number 1 Energy Drink 250ml", "8850999320064", 0.55m, 1.10m, 280, khmerBevCategory?.Id, supplier1?.Id, "number1-energy.jpg"),
                ("Angkor Beer 330ml", "Angkor Beer Can 330ml", "8850999320071", 0.70m, 1.50m, 600, khmerBevCategory?.Id, supplier1?.Id, "angkor-beer.jpg"),
                ("Cambodia Beer 330ml", "Cambodia Beer Can 330ml", "8850999320088", 0.65m, 1.40m, 550, khmerBevCategory?.Id, supplier1?.Id, "cambodia-beer.jpg"),
                ("Lay's Potato Chips", "Lay's Classic Potato Chips 50g", "8850999320095", 0.80m, 1.50m, 200, snacksCategory?.Id, supplier3?.Id, "lays-chips.jpg"),
                ("Pringles Original", "Pringles Original 110g", "8850999320101", 2.00m, 3.50m, 150, snacksCategory?.Id, supplier3?.Id, "pringles.jpg"),
                ("Oreo Cookies", "Oreo Original Cookies 137g", "8850999320118", 1.20m, 2.00m, 180, snacksCategory?.Id, supplier3?.Id, "oreo-cookies.jpg"),
                ("KitKat Chocolate", "KitKat 4 Finger 41.5g", "8850999320125", 0.60m, 1.00m, 250, snacksCategory?.Id, supplier3?.Id, "kitkat.jpg"),
                ("Snickers Bar", "Snickers Chocolate Bar 50g", "8850999320132", 0.70m, 1.20m, 220, snacksCategory?.Id, supplier3?.Id, "snickers.jpg"),
                ("Dutch Mill Yogurt", "Dutch Mill Yogurt Drink 180ml", "8850999320149", 0.50m, 0.90m, 300, dairyCategory?.Id, supplier1?.Id, "dutchmill-yogurt.jpg"),
                ("Vitamilk Soy Milk", "Vitamilk Original Soy Milk 300ml", "8850999320156", 0.40m, 0.80m, 350, dairyCategory?.Id, supplier1?.Id, "vitamilk.jpg"),
                ("Jasmine Rice 5kg", "Premium Jasmine Rice 5kg", "8850999320163", 8.00m, 12.00m, 100, riceCategory?.Id, supplier2?.Id, "jasmine-rice-5kg.jpg"),
                ("Fragrant Rice 10kg", "Cambodian Fragrant Rice 10kg", "8850999320170", 15.00m, 22.00m, 80, riceCategory?.Id, supplier2?.Id, "fragrant-rice-10kg.jpg"),
                ("Instant Noodles", "Mama Instant Noodles Pack", "8850999320187", 0.30m, 0.60m, 500, snacksCategory?.Id, supplier1?.Id, "instant-noodles.jpg"),
                ("Mineral Water 1.5L", "Pure Mineral Water 1.5L", "8850999320194", 0.25m, 0.50m, 800, beverageCategory?.Id, supplier1?.Id, "mineral-water.jpg"),
                ("Green Tea 500ml", "Oishi Green Tea 500ml", "8850999320200", 0.60m, 1.00m, 250, khmerBevCategory?.Id, supplier1?.Id, "green-tea.jpg")
            };

            foreach (var (name, desc, barcode, cost, price, qty, catId, suppId, imageName) in products)
            {
                var exists = await context.Products.AnyAsync(p => p.Barcode == barcode);
                if (!exists)
                {
                    var imagePath = imageName != null ? Path.Combine("assets", "product", imageName) : null;
                    
                    context.Products.Add(new Product
                    {
                        Name = name,
                        Description = desc,
                        Barcode = barcode,
                        SKU = $"SKU-{barcode.Substring(barcode.Length - 6)}",
                        CategoryId = catId,
                        SupplierId = suppId,
                        CostPrice = cost,
                        SellingPrice = price,
                        Quantity = qty,
                        MinQuantity = 50,
                        ImagePath = imagePath,
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    });
                }
            }

            await context.SaveChangesAsync();
        }

        public static async Task SeedAllAsync(ApplicationDbContext context)
        {
            await SeedCategoriesAsync(context);
            await SeedSuppliersAsync(context);
            await SeedCustomersAsync(context);
            await SeedProductsAsync(context);
        }

        public static async Task RefreshAllDataAsync(ApplicationDbContext context)
        {
            try
            {
                // Clear existing data (preserve sales and users)
                // Order is important due to foreign key constraints
                await ClearProductDataAsync(context);
                await ClearCategoriesAsync(context);
                await ClearSuppliersAsync(context);
                await ClearCustomersAsync(context);

                // Re-seed with fresh data
                await SeedAllAsync(context);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error refreshing database: {ex.Message}", ex);
            }
        }

        public static async Task ClearAllDataAsync(ApplicationDbContext context)
        {
            try
            {
                // Clear all data except current admin user
                // Order is important due to foreign key constraints
                await ClearSalesDataAsync(context);
                await ClearProductDataAsync(context);
                await ClearCategoriesAsync(context);
                await ClearSuppliersAsync(context);
                await ClearCustomersAsync(context);
                // Note: We preserve the current admin user for security
            }
            catch (Exception ex)
            {
                throw new Exception($"Error clearing database: {ex.Message}", ex);
            }
        }

        public static async Task ResetDatabaseAsync(ApplicationDbContext context)
        {
            try
            {
                // This is the most destructive operation - recreates the entire database
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();
                
                // Create default admin user
                await CreateDefaultAdminAsync(context);
                
                // Seed with default data
                await SeedAllAsync(context);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error resetting database: {ex.Message}", ex);
            }
        }

        private static async Task ClearProductDataAsync(ApplicationDbContext context)
        {
            try
            {
                // Clear product images first (foreign key constraint)
                var productImages = await context.ProductImages.ToListAsync();
                if (productImages.Any())
                {
                    context.ProductImages.RemoveRange(productImages);
                    await context.SaveChangesAsync();
                }
                
                // Clear products
                var products = await context.Products.ToListAsync();
                if (products.Any())
                {
                    context.Products.RemoveRange(products);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error clearing product data: {ex.Message}", ex);
            }
        }

        private static async Task ClearCategoriesAsync(ApplicationDbContext context)
        {
            try
            {
                var categories = await context.Categories.ToListAsync();
                if (categories.Any())
                {
                    context.Categories.RemoveRange(categories);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error clearing categories: {ex.Message}", ex);
            }
        }

        private static async Task ClearSuppliersAsync(ApplicationDbContext context)
        {
            try
            {
                var suppliers = await context.Suppliers.ToListAsync();
                if (suppliers.Any())
                {
                    context.Suppliers.RemoveRange(suppliers);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error clearing suppliers: {ex.Message}", ex);
            }
        }

        private static async Task ClearCustomersAsync(ApplicationDbContext context)
        {
            try
            {
                var customers = await context.Customers.ToListAsync();
                if (customers.Any())
                {
                    context.Customers.RemoveRange(customers);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error clearing customers: {ex.Message}", ex);
            }
        }

        private static async Task ClearSalesDataAsync(ApplicationDbContext context)
        {
            try
            {
                // Clear sale items first (foreign key constraint)
                var saleItems = await context.SaleItems.ToListAsync();
                if (saleItems.Any())
                {
                    context.SaleItems.RemoveRange(saleItems);
                    await context.SaveChangesAsync();
                }
                
                // Clear sales
                var sales = await context.Sales.ToListAsync();
                if (sales.Any())
                {
                    context.Sales.RemoveRange(sales);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error clearing sales data: {ex.Message}", ex);
            }
        }

        public static async Task AddProductImagesToExistingProductsAsync(ApplicationDbContext context)
        {
            // Get all products that don't have images yet
            var productsWithoutImages = await context.Products
                .Where(p => !p.ProductImages.Any() && p.IsActive)
                .ToListAsync();

            foreach (var product in productsWithoutImages)
            {
                // Try to find matching image based on product name or existing ImagePath
                string? imagePath = null;
                string? imageName = null;

                // First check if product already has an ImagePath set
                if (!string.IsNullOrEmpty(product.ImagePath))
                {
                    imagePath = product.ImagePath;
                    imageName = Path.GetFileName(product.ImagePath);
                }
                else
                {
                    // Generate image name based on product name
                    imageName = GenerateImageNameFromProduct(product.Name);
                    imagePath = Path.Combine("assets", "product", imageName);
                }

                // Check if the image file actually exists
                var fullImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imagePath);
                bool imageExists = File.Exists(fullImagePath);

                // Create ProductImage entry
                var productImage = new ProductImage
                {
                    ProductId = product.Id,
                    ImagePath = imagePath,
                    ImageType = imageExists ? "Local" : "Default",
                    ImageName = imageName,
                    IsPrimary = true, // First image is primary
                    DisplayOrder = 1,
                    Description = $"Primary image for {product.Name}",
                    FileSize = imageExists ? new FileInfo(fullImagePath).Length : 0,
                    FileExtension = Path.GetExtension(imageName),
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    IsActive = true
                };

                context.ProductImages.Add(productImage);
            }

            await context.SaveChangesAsync();
        }

        public static async Task AddSpecificImageToProductAsync(ApplicationDbContext context, int productId, string imagePath, bool isPrimary = false)
        {
            var product = await context.Products.FindAsync(productId);
            if (product == null)
                throw new ArgumentException($"Product with ID {productId} not found");

            // Check if image already exists for this product
            var existingImage = await context.ProductImages
                .FirstOrDefaultAsync(pi => pi.ProductId == productId && pi.ImagePath == imagePath);

            if (existingImage != null)
                return; // Image already exists

            // If this is set as primary, update other images to not be primary
            if (isPrimary)
            {
                var existingPrimaryImages = await context.ProductImages
                    .Where(pi => pi.ProductId == productId && pi.IsPrimary)
                    .ToListAsync();

                foreach (var img in existingPrimaryImages)
                {
                    img.IsPrimary = false;
                    img.UpdatedAt = DateTime.Now;
                }
            }

            // Get the next display order
            var maxDisplayOrder = await context.ProductImages
                .Where(pi => pi.ProductId == productId)
                .MaxAsync(pi => (int?)pi.DisplayOrder) ?? 0;

            // Check if the image file exists
            var fullImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imagePath);
            bool imageExists = File.Exists(fullImagePath);

            var productImage = new ProductImage
            {
                ProductId = productId,
                ImagePath = imagePath,
                ImageType = imageExists ? "Local" : "URL",
                ImageName = Path.GetFileName(imagePath),
                IsPrimary = isPrimary,
                DisplayOrder = maxDisplayOrder + 1,
                Description = $"Image for {product.Name}",
                FileSize = imageExists ? new FileInfo(fullImagePath).Length : 0,
                FileExtension = Path.GetExtension(imagePath),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                IsActive = true
            };

            context.ProductImages.Add(productImage);
            await context.SaveChangesAsync();
        }

        public static async Task AddBulkImagesToProductsAsync(ApplicationDbContext context, Dictionary<string, string> productImageMappings)
        {
            foreach (var mapping in productImageMappings)
            {
                var productIdentifier = mapping.Key; // Can be product name, barcode, or SKU
                var imagePath = mapping.Value;

                // Find product by name, barcode, or SKU
                var product = await context.Products
                    .FirstOrDefaultAsync(p => 
                        p.Name.Contains(productIdentifier) || 
                        p.Barcode == productIdentifier || 
                        p.SKU == productIdentifier);

                if (product != null)
                {
                    await AddSpecificImageToProductAsync(context, product.Id, imagePath, false);
                }
            }
        }

        private static string GenerateImageNameFromProduct(string productName)
        {
            // Convert product name to a suitable image filename
            var imageName = productName.ToLower()
                .Replace(" ", "-")
                .Replace("&", "and")
                .Replace(".", "")
                .Replace(",", "")
                .Replace("(", "")
                .Replace(")", "")
                .Replace("/", "-");

            // Remove any remaining special characters
            imageName = System.Text.RegularExpressions.Regex.Replace(imageName, @"[^a-z0-9\-]", "");
            
            return $"{imageName}.jpg";
        }

        private static async Task CreateDefaultAdminAsync(ApplicationDbContext context)
        {
            // Create default admin user if none exists
            var adminExists = await context.Users.AnyAsync(u => u.Role == UserRole.Administrator);
            if (!adminExists)
            {
                var defaultAdmin = new User
                {
                    Username = "admin",
                    FirstName = "System",
                    LastName = "Administrator",
                    Email = "admin@syncverse.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("admin123"), // Default password
                    Role = UserRole.Administrator,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                context.Users.Add(defaultAdmin);
                await context.SaveChangesAsync();
            }
        }
    }
}
