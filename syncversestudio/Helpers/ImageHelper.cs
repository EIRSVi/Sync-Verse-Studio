using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using QRCoder;

namespace SyncVerseStudio.Helpers
{
    public static class ImageHelper
    {
        private static readonly string ImagesFolder = Path.Combine(Application.StartupPath, "Images");
        private static readonly string ProductImagesFolder = Path.Combine(ImagesFolder, "Products");
        private static readonly string UserImagesFolder = Path.Combine(ImagesFolder, "Users");
        private static readonly string QRCodesFolder = Path.Combine(ImagesFolder, "QRCodes");

        static ImageHelper()
        {
            // Ensure directories exist
            Directory.CreateDirectory(ProductImagesFolder);
            Directory.CreateDirectory(UserImagesFolder);
            Directory.CreateDirectory(QRCodesFolder);
        }

        public static string SelectImageFromPC()
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.webpb;*.gif;*.bmp|All Files|*.*";
                openFileDialog.Title = "Select an Image";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    return openFileDialog.FileName;
                }
            }
            return null;
        }

        public static async Task<string> DownloadImageFromUrl(string url, string targetFolder)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var imageBytes = await httpClient.GetByteArrayAsync(url);
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(url)}";
                    var filePath = Path.Combine(targetFolder, fileName);

                    await File.WriteAllBytesAsync(filePath, imageBytes);
                    return filePath;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error downloading image: {ex.Message}", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public static string SaveProductImage(string sourcePath)
        {
            try
            {
                var fileName = $"product_{Guid.NewGuid()}{Path.GetExtension(sourcePath)}";
                var destinationPath = Path.Combine(ProductImagesFolder, fileName);

                File.Copy(sourcePath, destinationPath, true);
                return destinationPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving product image: {ex.Message}", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public static string SaveUserImage(string sourcePath)
        {
            try
            {
                var fileName = $"user_{Guid.NewGuid()}{Path.GetExtension(sourcePath)}";
                var destinationPath = Path.Combine(UserImagesFolder, fileName);

                File.Copy(sourcePath, destinationPath, true);
                return destinationPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving user image: {ex.Message}", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public static string GenerateQRCode(string data, string prefix = "qr")
        {
            try
            {
                using (var qrGenerator = new QRCodeGenerator())
                {
                    var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
                    using (var qrCode = new QRCode(qrCodeData))
                    {
                        using (var qrCodeImage = qrCode.GetGraphic(20))
                        {
                            var fileName = $"{prefix}_{Guid.NewGuid()}.png";
                            var filePath = Path.Combine(QRCodesFolder, fileName);

                            qrCodeImage.Save(filePath, ImageFormat.Png);
                            return filePath;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating QR code: {ex.Message}", "QR Code Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public static Image LoadImage(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path) || !File.Exists(path))
                    return null;

                return Image.FromFile(path);
            }
            catch
            {
                return null;
            }
        }

        public static async Task<Image> LoadImageFromUrl(string url)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                    return null;

                using (var httpClient = new HttpClient())
                {
                    var imageBytes = await httpClient.GetByteArrayAsync(url);
                    using (var ms = new MemoryStream(imageBytes))
                    {
                        return Image.FromStream(ms);
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public static void DeleteImage(string path)
        {
            try
            {
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch
            {
                // Silently fail
            }
        }

        public static Image ResizeImage(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);
            using (var graphics = Graphics.FromImage(newImage))
            {
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return newImage;
        }

        public static string GetProductImagesFolder() => ProductImagesFolder;
        public static string GetUserImagesFolder() => UserImagesFolder;
        public static string GetQRCodesFolder() => QRCodesFolder;
    }
}
