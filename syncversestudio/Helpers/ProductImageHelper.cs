using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SyncVerseStudio.Models;

namespace SyncVerseStudio.Helpers
{
    public static class ProductImageHelper
    {
        private static readonly string BaseImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "product");
        private static readonly string[] SupportedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
        private static readonly string DefaultBrandImageUrl = "https://raw.githubusercontent.com/EIRSVi/eirsvi/refs/heads/docs/assets/brand/logo.png";
        private static Image? _cachedDefaultImage;

        static ProductImageHelper()
        {
            // Ensure the directory exists
            if (!Directory.Exists(BaseImagePath))
            {
                Directory.CreateDirectory(BaseImagePath);
            }
        }

        public static string GetImageFullPath(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return string.Empty;

            // If it's a URL, return as is
            if (imagePath.StartsWith("http://") || imagePath.StartsWith("https://"))
                return imagePath;

            // If it's already a full path, return as is
            if (Path.IsPathRooted(imagePath))
                return imagePath;

            // Otherwise, combine with base path
            return Path.Combine(BaseImagePath, imagePath);
        }

        public static Image? LoadImage(string imagePath)
        {
            try
            {
                if (string.IsNullOrEmpty(imagePath))
                    return null;

                // Handle URL images
                if (imagePath.StartsWith("http://") || imagePath.StartsWith("https://"))
                {
                    return LoadImageFromUrl(imagePath);
                }

                // Handle local images
                string fullPath = GetImageFullPath(imagePath);
                if (File.Exists(fullPath))
                {
                    using (var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                    {
                        return Image.FromStream(stream);
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public static Image? LoadImageFromUrl(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(10);
                    var response = client.GetAsync(url).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        using (var stream = response.Content.ReadAsStreamAsync().Result)
                        {
                            return Image.FromStream(stream);
                        }
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public static string SaveUploadedImage(Image image, string productName, int productId)
        {
            try
            {
                // Generate unique filename
                string sanitizedName = SanitizeFileName(productName);
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string fileName = $"{sanitizedName}_{productId}_{timestamp}.png";
                string fullPath = Path.Combine(BaseImagePath, fileName);

                // Save image
                image.Save(fullPath, ImageFormat.Png);

                // Return relative path
                return fileName;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save image: {ex.Message}");
            }
        }

        public static string CopyImageToAssets(string sourcePath, string productName, int productId)
        {
            try
            {
                if (!File.Exists(sourcePath))
                    throw new FileNotFoundException("Source image not found");

                string extension = Path.GetExtension(sourcePath).ToLower();
                if (!SupportedExtensions.Contains(extension))
                    throw new NotSupportedException($"Image format {extension} is not supported");

                // Generate unique filename
                string sanitizedName = SanitizeFileName(productName);
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string fileName = $"{sanitizedName}_{productId}_{timestamp}{extension}";
                string destPath = Path.Combine(BaseImagePath, fileName);

                // Copy file
                File.Copy(sourcePath, destPath, true);

                // Return relative path
                return fileName;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to copy image: {ex.Message}");
            }
        }

        public static ProductImage CreateProductImage(int productId, string imagePath, string imageType, bool isPrimary = false)
        {
            var productImage = new ProductImage
            {
                ProductId = productId,
                ImagePath = imagePath,
                ImageType = imageType,
                IsPrimary = isPrimary,
                ImageName = Path.GetFileName(imagePath),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                IsActive = true
            };

            // Get file info if it's a local file
            if (imageType == "Local" || imageType == "Upload")
            {
                string fullPath = GetImageFullPath(imagePath);
                if (File.Exists(fullPath))
                {
                    var fileInfo = new FileInfo(fullPath);
                    productImage.FileSize = fileInfo.Length;
                    productImage.FileExtension = fileInfo.Extension;
                }
            }

            return productImage;
        }

        public static Image? GetPrimaryImage(Product product)
        {
            if (product.ProductImages == null || !product.ProductImages.Any())
                return GetDefaultBrandImage();

            var primaryImage = product.ProductImages
                .Where(img => img.IsActive && img.IsPrimary)
                .OrderBy(img => img.DisplayOrder)
                .FirstOrDefault();

            if (primaryImage == null)
            {
                primaryImage = product.ProductImages
                    .Where(img => img.IsActive)
                    .OrderBy(img => img.DisplayOrder)
                    .FirstOrDefault();
            }

            var image = primaryImage != null ? LoadImage(primaryImage.ImagePath) : null;
            return image ?? GetDefaultBrandImage();
        }

        public static Image? GetDefaultBrandImage()
        {
            try
            {
                if (_cachedDefaultImage != null)
                    return new Bitmap(_cachedDefaultImage);

                // Try to load from URL
                _cachedDefaultImage = LoadImageFromUrl(DefaultBrandImageUrl);
                
                if (_cachedDefaultImage != null)
                    return new Bitmap(_cachedDefaultImage);

                // Fallback: Create a simple default image with brand colors
                return CreateFallbackImage();
            }
            catch
            {
                return CreateFallbackImage();
            }
        }

        private static Image CreateFallbackImage()
        {
            var bitmap = new Bitmap(200, 200);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                // Fill with brand color background
                graphics.Clear(Color.FromArgb(59, 130, 246));
                
                // Draw "No Image" text
                using (var font = new Font("Segoe UI", 14, FontStyle.Bold))
                using (var brush = new SolidBrush(Color.White))
                {
                    var text = "SyncVerse\nPOS";
                    var format = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    graphics.DrawString(text, font, brush, new RectangleF(0, 0, 200, 200), format);
                }
            }
            return bitmap;
        }

        public static Image ResizeImage(Image image, int maxWidth, int maxHeight)
        {
            int width = image.Width;
            int height = image.Height;

            // Calculate new dimensions while maintaining aspect ratio
            if (width > maxWidth || height > maxHeight)
            {
                double ratioX = (double)maxWidth / width;
                double ratioY = (double)maxHeight / height;
                double ratio = Math.Min(ratioX, ratioY);

                width = (int)(width * ratio);
                height = (int)(height * ratio);
            }

            var resized = new Bitmap(width, height);
            using (var graphics = Graphics.FromImage(resized))
            {
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(image, 0, 0, width, height);
            }

            return resized;
        }

        public static bool DeleteImage(string imagePath)
        {
            try
            {
                if (string.IsNullOrEmpty(imagePath))
                    return false;

                // Don't delete URL images
                if (imagePath.StartsWith("http://") || imagePath.StartsWith("https://"))
                    return true;

                string fullPath = GetImageFullPath(imagePath);
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidImageUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult))
                return false;

            if (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps)
                return false;

            // Check if URL ends with image extension
            string extension = Path.GetExtension(uriResult.AbsolutePath).ToLower();
            return SupportedExtensions.Contains(extension);
        }

        private static string SanitizeFileName(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitized = string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
            return sanitized.Length > 50 ? sanitized.Substring(0, 50) : sanitized;
        }

        public static string[] GetSupportedExtensions()
        {
            return SupportedExtensions;
        }

        public static string GetSupportedExtensionsFilter()
        {
            return "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp;*.webp|All Files|*.*";
        }
    }
}
