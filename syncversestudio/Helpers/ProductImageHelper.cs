using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using SyncVerseStudio.Models;

namespace SyncVerseStudio.Helpers
{
    /// <summary>
    /// Helper class for handling product images in the POS system
    /// </summary>
    public static class ProductImageHelper
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        
        /// <summary>
        /// Get the primary image for a product
        /// </summary>
        public static Image GetPrimaryImage(Product product)
        {
            try
            {
                if (product?.ProductImages != null && product.ProductImages.Count > 0)
                {
                    var primaryImage = product.ProductImages.FirstOrDefault(img => img.IsPrimary) 
                                     ?? product.ProductImages.First();
                    
                    if (!string.IsNullOrEmpty(primaryImage.ImagePath))
                    {
                        if (File.Exists(primaryImage.ImagePath))
                        {
                            return Image.FromFile(primaryImage.ImagePath);
                        }
                        else if (Uri.IsWellFormedUriString(primaryImage.ImagePath, UriKind.Absolute))
                        {
                            // Try to load from URL (async operation, return null for now)
                            return null;
                        }
                    }
                }
            }
            catch
            {
                // Return null on any error
            }
            
            return null;
        }

        /// <summary>
        /// Get the default brand image
        /// </summary>
        public static Image GetDefaultBrandImage()
        {
            try
            {
                // Try to load the brand logo from local assets
                var logoPath = Path.Combine(Application.StartupPath, "assets", "brand", "noBgColor.png");
                if (File.Exists(logoPath))
                {
                    return Image.FromFile(logoPath);
                }

                // Try alternative paths
                var altPaths = new[]
                {
                    Path.Combine(Application.StartupPath, "assets", "logo.png"),
                    Path.Combine(Application.StartupPath, "logo.png"),
                    Path.Combine(Directory.GetCurrentDirectory(), "assets", "brand", "noBgColor.png")
                };

                foreach (var path in altPaths)
                {
                    if (File.Exists(path))
                    {
                        return Image.FromFile(path);
                    }
                }
            }
            catch
            {
                // Ignore errors and return null
            }

            return null;
        }

        /// <summary>
        /// Resize an image to fit within the specified dimensions while maintaining aspect ratio
        /// </summary>
        public static Image ResizeImage(Image originalImage, int maxWidth, int maxHeight)
        {
            if (originalImage == null)
                return null;

            try
            {
                // Calculate the scaling factor
                float scaleX = (float)maxWidth / originalImage.Width;
                float scaleY = (float)maxHeight / originalImage.Height;
                float scale = Math.Min(scaleX, scaleY);

                // Calculate new dimensions
                int newWidth = (int)(originalImage.Width * scale);
                int newHeight = (int)(originalImage.Height * scale);

                // Create new bitmap with high quality settings
                var resizedImage = new Bitmap(newWidth, newHeight);
                using (var graphics = Graphics.FromImage(resizedImage))
                {
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    graphics.DrawImage(originalImage, 0, 0, newWidth, newHeight);
                }

                return resizedImage;
            }
            catch
            {
                return originalImage; // Return original on error
            }
        }

        /// <summary>
        /// Create a placeholder image with the product name
        /// </summary>
        public static Image CreatePlaceholderImage(string productName, int width, int height)
        {
            try
            {
                var bitmap = new Bitmap(width, height);
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                    // Fill background with light gray
                    using (var brush = new SolidBrush(Color.FromArgb(248, 250, 252)))
                    {
                        graphics.FillRectangle(brush, 0, 0, width, height);
                    }

                    // Draw border
                    using (var pen = new Pen(Color.FromArgb(226, 232, 240), 2))
                    {
                        graphics.DrawRectangle(pen, 1, 1, width - 2, height - 2);
                    }

                    // Draw text
                    var displayText = string.IsNullOrEmpty(productName) ? "No Image" : 
                                    productName.Length > 15 ? productName.Substring(0, 12) + "..." : productName;

                    using (var font = new Font("Segoe UI", 10F, FontStyle.Bold))
                    using (var textBrush = new SolidBrush(Color.FromArgb(100, 116, 139)))
                    {
                        var textSize = graphics.MeasureString(displayText, font);
                        var x = (width - textSize.Width) / 2;
                        var y = (height - textSize.Height) / 2;
                        graphics.DrawString(displayText, font, textBrush, x, y);
                    }
                }

                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Download image from URL asynchronously
        /// </summary>
        public static async Task<Image> DownloadImageAsync(string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl) || !Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
                    return null;

                var response = await _httpClient.GetAsync(imageUrl);
                if (response.IsSuccessStatusCode)
                {
                    var imageBytes = await response.Content.ReadAsByteArrayAsync();
                    using (var stream = new MemoryStream(imageBytes))
                    {
                        return Image.FromStream(stream);
                    }
                }
            }
            catch
            {
                // Return null on any error
            }

            return null;
        }

        /// <summary>
        /// Load image from file path or URL
        /// </summary>
        public static Image LoadImage(string imagePath)
        {
            try
            {
                if (string.IsNullOrEmpty(imagePath))
                    return null;

                if (File.Exists(imagePath))
                {
                    return Image.FromFile(imagePath);
                }
                else if (Uri.IsWellFormedUriString(imagePath, UriKind.Absolute))
                {
                    // For URLs, return null for now (could implement async loading)
                    return null;
                }
            }
            catch
            {
                // Return null on error
            }

            return null;
        }

        /// <summary>
        /// Get the full path for an image
        /// </summary>
        public static string GetImageFullPath(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return string.Empty;

            if (Path.IsPathRooted(imagePath))
                return imagePath;

            return Path.Combine(Application.StartupPath, "assets", "images", imagePath);
        }

        /// <summary>
        /// Get supported image extensions filter for file dialogs
        /// </summary>
        public static string GetSupportedExtensionsFilter()
        {
            return "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.tiff;*.webp;*.ico|" +
                   "JPEG Files|*.jpg;*.jpeg|" +
                   "PNG Files|*.png|" +
                   "BMP Files|*.bmp|" +
                   "GIF Files|*.gif|" +
                   "TIFF Files|*.tiff|" +
                   "WebP Files|*.webp|" +
                   "Icon Files|*.ico|" +
                   "All Files|*.*";
        }

        /// <summary>
        /// Check if a file is a supported image type
        /// </summary>
        public static bool IsSupportedImageType(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            var extension = Path.GetExtension(filePath).ToLower();
            var supportedExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tiff", ".webp", ".ico" };
            
            return supportedExtensions.Contains(extension);
        }

        /// <summary>
        /// Create a product image record
        /// </summary>
        public static ProductImage CreateProductImage(int productId, string imagePath, bool isPrimary = false)
        {
            return new ProductImage
            {
                ProductId = productId,
                ImagePath = imagePath,
                IsPrimary = isPrimary,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
        }

        /// <summary>
        /// Check if a URL is a valid image URL
        /// </summary>
        public static bool IsValidImageUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                return false;

            var uri = new Uri(url);
            var extension = Path.GetExtension(uri.LocalPath).ToLower();
            var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tiff" };

            return validExtensions.Contains(extension);
        }

        /// <summary>
        /// Copy image to assets folder
        /// </summary>
        public static string CopyImageToAssets(string sourcePath)
        {
            try
            {
                if (!File.Exists(sourcePath))
                    return string.Empty;

                var assetsDir = Path.Combine(Application.StartupPath, "assets", "images");
                Directory.CreateDirectory(assetsDir);

                var fileName = Path.GetFileName(sourcePath);
                var destinationPath = Path.Combine(assetsDir, fileName);

                // If file exists, create unique name
                int counter = 1;
                while (File.Exists(destinationPath))
                {
                    var nameWithoutExt = Path.GetFileNameWithoutExtension(sourcePath);
                    var extension = Path.GetExtension(sourcePath);
                    fileName = $"{nameWithoutExt}_{counter}{extension}";
                    destinationPath = Path.Combine(assetsDir, fileName);
                    counter++;
                }

                File.Copy(sourcePath, destinationPath);
                return Path.Combine("assets", "images", fileName);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Delete an image file
        /// </summary>
        public static bool DeleteImage(string imagePath)
        {
            try
            {
                var fullPath = GetImageFullPath(imagePath);
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return true;
                }
            }
            catch
            {
                // Ignore errors
            }

            return false;
        }

        /// <summary>
        /// Dispose of the HTTP client when the application shuts down
        /// </summary>
        public static void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}