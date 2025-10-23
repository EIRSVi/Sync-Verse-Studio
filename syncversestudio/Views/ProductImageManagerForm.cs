using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using SyncVerseStudio.Data;
using SyncVerseStudio.Models;
using SyncVerseStudio.Helpers;
using FontAwesome.Sharp;

namespace SyncVerseStudio.Views
{
    public partial class ProductImageManagerForm : Form
    {
        private readonly ApplicationDbContext _context;
        private readonly int _productId;
        private Product? _product;
        private FlowLayoutPanel _imagesPanel;
        private Button _addLocalButton, _addUrlButton, _addUploadButton, _closeButton;
        private Label _titleLabel, _productNameLabel;

        public ProductImageManagerForm(int productId)
        {
            _context = new ApplicationDbContext();
            _productId = productId;
            InitializeComponent();
            LoadProduct();
            LoadImages();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Name = "ProductImageManagerForm";
            this.Text = "Manage Product Images";
            this.Size = new Size(900, 700);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(245, 247, 250);

            CreateHeader();
            CreateImagePanel();
            CreateButtons();

            this.ResumeLayout(false);
        }

        private void CreateHeader()
        {
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.White
            };

            var iconBox = new IconPictureBox
            {
                IconChar = IconChar.Images,
                IconColor = Color.FromArgb(59, 130, 246),
                IconSize = 32,
                Location = new Point(20, 24),
                Size = new Size(32, 32),
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(iconBox);

            _titleLabel = new Label
            {
                Text = "Product Images",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(60, 20),
                Size = new Size(400, 30),
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(_titleLabel);

            _productNameLabel = new Label
            {
                Text = "Loading...",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(120, 120, 120),
                Location = new Point(60, 50),
                Size = new Size(400, 20),
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(_productNameLabel);

            this.Controls.Add(headerPanel);
        }

        private void CreateImagePanel()
        {
            var containerPanel = new Panel
            {
                Location = new Point(20, 100),
                Size = new Size(840, 480),
                BackColor = Color.White,
                AutoScroll = true
            };

            containerPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, containerPanel.Width - 1, containerPanel.Height - 1), 12))
                using (var brush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillPath(brush, path);
                }
            };

            _imagesPanel = new FlowLayoutPanel
            {
                Location = new Point(10, 10),
                Size = new Size(810, 460),
                BackColor = Color.Transparent,
                AutoScroll = true,
                WrapContents = true
            };

            containerPanel.Controls.Add(_imagesPanel);
            this.Controls.Add(containerPanel);
        }

        private void CreateButtons()
        {
            var buttonPanel = new Panel
            {
                Location = new Point(20, 600),
                Size = new Size(840, 50),
                BackColor = Color.Transparent
            };

            _addLocalButton = CreateButton("Add from Assets", IconChar.FolderOpen, Color.FromArgb(59, 130, 246), 0);
            _addLocalButton.Click += AddLocalImage_Click;
            buttonPanel.Controls.Add(_addLocalButton);

            _addUrlButton = CreateButton("Add from URL", IconChar.Link, Color.FromArgb(34, 197, 94), 180);
            _addUrlButton.Click += AddUrlImage_Click;
            buttonPanel.Controls.Add(_addUrlButton);

            _addUploadButton = CreateButton("Upload Image", IconChar.Upload, Color.FromArgb(168, 85, 247), 360);
            _addUploadButton.Click += UploadImage_Click;
            buttonPanel.Controls.Add(_addUploadButton);

            _closeButton = CreateButton("Close", IconChar.Times, Color.FromArgb(100, 100, 100), 660);
            _closeButton.Click += (s, e) => this.Close();
            buttonPanel.Controls.Add(_closeButton);

            this.Controls.Add(buttonPanel);
        }

        private Button CreateButton(string text, IconChar icon, Color color, int x)
        {
            var button = new Button
            {
                Text = $"  {text}",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = color,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(x, 0),
                Size = new Size(170, 45),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(35, 0, 0, 0)
            };
            button.FlatAppearance.BorderSize = 0;

            var iconBox = new IconPictureBox
            {
                IconChar = icon,
                IconColor = Color.White,
                IconSize = 20,
                Location = new Point(10, 12),
                Size = new Size(20, 20),
                BackColor = Color.Transparent,
                Parent = button
            };

            return button;
        }

        private async void LoadProduct()
        {
            try
            {
                _product = await _context.Products
                    .Include(p => p.ProductImages)
                    .FirstOrDefaultAsync(p => p.Id == _productId);

                if (_product != null)
                {
                    _productNameLabel.Text = $"Product: {_product.Name}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading product: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void LoadImages()
        {
            try
            {
                _imagesPanel.Controls.Clear();

                var images = await _context.ProductImages
                    .Where(img => img.ProductId == _productId && img.IsActive)
                    .OrderByDescending(img => img.IsPrimary)
                    .ThenBy(img => img.DisplayOrder)
                    .ToListAsync();

                if (!images.Any())
                {
                    var noImagesLabel = new Label
                    {
                        Text = "No images added yet.\nClick the buttons below to add images.",
                        Font = new Font("Segoe UI", 12F),
                        ForeColor = Color.FromArgb(150, 150, 150),
                        TextAlign = ContentAlignment.MiddleCenter,
                        Size = new Size(800, 100),
                        Location = new Point(0, 180)
                    };
                    _imagesPanel.Controls.Add(noImagesLabel);
                    return;
                }

                foreach (var productImage in images)
                {
                    CreateImageCard(productImage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading images: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateImageCard(ProductImage productImage)
        {
            var card = new Panel
            {
                Size = new Size(180, 240),
                Margin = new Padding(10),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Tag = productImage
            };

            // Image display
            var pictureBox = new PictureBox
            {
                Location = new Point(10, 10),
                Size = new Size(160, 160),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(245, 247, 250)
            };

            try
            {
                var image = ProductImageHelper.LoadImage(productImage.ImagePath);
                if (image != null)
                {
                    pictureBox.Image = ProductImageHelper.ResizeImage(image, 160, 160);
                }
                else
                {
                    pictureBox.BackColor = Color.FromArgb(220, 220, 220);
                }
            }
            catch
            {
                pictureBox.BackColor = Color.FromArgb(220, 220, 220);
            }

            card.Controls.Add(pictureBox);

            // Primary badge
            if (productImage.IsPrimary)
            {
                var primaryLabel = new Label
                {
                    Text = "PRIMARY",
                    Font = new Font("Segoe UI", 7F, FontStyle.Bold),
                    ForeColor = Color.White,
                    BackColor = Color.FromArgb(34, 197, 94),
                    Location = new Point(10, 10),
                    Size = new Size(60, 18),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Parent = pictureBox
                };
                pictureBox.Controls.Add(primaryLabel);
            }

            // Image type label
            var typeLabel = new Label
            {
                Text = productImage.ImageType,
                Font = new Font("Segoe UI", 8F),
                ForeColor = Color.FromArgb(120, 120, 120),
                Location = new Point(10, 175),
                Size = new Size(160, 16),
                TextAlign = ContentAlignment.MiddleCenter
            };
            card.Controls.Add(typeLabel);

            // Action buttons
            var buttonPanel = new FlowLayoutPanel
            {
                Location = new Point(10, 195),
                Size = new Size(160, 35),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false
            };

            var setPrimaryBtn = new Button
            {
                Text = "★",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Size = new Size(35, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = productImage.IsPrimary ? Color.FromArgb(34, 197, 94) : Color.FromArgb(200, 200, 200),
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Tag = productImage
            };
            setPrimaryBtn.FlatAppearance.BorderSize = 0;
            setPrimaryBtn.Click += async (s, e) => await SetPrimaryImage(productImage);
            buttonPanel.Controls.Add(setPrimaryBtn);

            var viewBtn = new Button
            {
                Text = "👁",
                Font = new Font("Segoe UI", 10F),
                Size = new Size(35, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Tag = productImage
            };
            viewBtn.FlatAppearance.BorderSize = 0;
            viewBtn.Click += (s, e) => ViewFullImage(productImage);
            buttonPanel.Controls.Add(viewBtn);

            var editBtn = new Button
            {
                Text = "✎",
                Font = new Font("Segoe UI", 10F),
                Size = new Size(35, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(249, 115, 22),
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Tag = productImage
            };
            editBtn.FlatAppearance.BorderSize = 0;
            editBtn.Click += (s, e) => EditImageDescription(productImage);
            buttonPanel.Controls.Add(editBtn);

            var deleteBtn = new Button
            {
                Text = "🗑",
                Font = new Font("Segoe UI", 10F),
                Size = new Size(35, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(239, 68, 68),
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Tag = productImage
            };
            deleteBtn.FlatAppearance.BorderSize = 0;
            deleteBtn.Click += async (s, e) => await DeleteImage(productImage);
            buttonPanel.Controls.Add(deleteBtn);

            card.Controls.Add(buttonPanel);
            _imagesPanel.Controls.Add(card);
        }

        private async void AddLocalImage_Click(object sender, EventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Title = "Select Image from Assets",
                    InitialDirectory = ProductImageHelper.GetImageFullPath(""),
                    Filter = ProductImageHelper.GetSupportedExtensionsFilter(),
                    Multiselect = true
                };

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (var filePath in dialog.FileNames)
                    {
                        // Get relative path from assets folder
                        string assetsPath = ProductImageHelper.GetImageFullPath("");
                        string relativePath = filePath.Replace(assetsPath + "\\", "");

                        var productImage = ProductImageHelper.CreateProductImage(_productId, relativePath, "Local");
                        _context.ProductImages.Add(productImage);
                    }

                    await _context.SaveChangesAsync();
                    LoadImages();
                    MessageBox.Show("Images added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding images: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void AddUrlImage_Click(object sender, EventArgs e)
        {
            var urlForm = new Form
            {
                Text = "Add Image from URL",
                Size = new Size(500, 200),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var label = new Label
            {
                Text = "Enter Image URL:",
                Location = new Point(20, 20),
                Size = new Size(450, 20),
                Font = new Font("Segoe UI", 10F)
            };
            urlForm.Controls.Add(label);

            var urlTextBox = new TextBox
            {
                Location = new Point(20, 50),
                Size = new Size(440, 30),
                Font = new Font("Segoe UI", 10F)
            };
            urlForm.Controls.Add(urlTextBox);

            var addButton = new Button
            {
                Text = "Add Image",
                Location = new Point(260, 100),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            addButton.FlatAppearance.BorderSize = 0;
            addButton.Click += async (s, ev) =>
            {
                try
                {
                    string url = urlTextBox.Text.Trim();
                    if (string.IsNullOrEmpty(url))
                    {
                        MessageBox.Show("Please enter a URL", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (!ProductImageHelper.IsValidImageUrl(url))
                    {
                        MessageBox.Show("Please enter a valid image URL", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    var productImage = ProductImageHelper.CreateProductImage(_productId, url, "URL");
                    _context.ProductImages.Add(productImage);
                    await _context.SaveChangesAsync();

                    urlForm.Close();
                    LoadImages();
                    MessageBox.Show("Image added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            urlForm.Controls.Add(addButton);

            var cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(370, 100),
                Size = new Size(90, 35),
                BackColor = Color.FromArgb(150, 150, 150),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F)
            };
            cancelButton.FlatAppearance.BorderSize = 0;
            cancelButton.Click += (s, ev) => urlForm.Close();
            urlForm.Controls.Add(cancelButton);

            urlForm.ShowDialog();
        }

        private async void UploadImage_Click(object sender, EventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Title = "Upload Image",
                    Filter = ProductImageHelper.GetSupportedExtensionsFilter(),
                    Multiselect = true
                };

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (var filePath in dialog.FileNames)
                    {
                        // Copy image to assets folder
                        string relativePath = ProductImageHelper.CopyImageToAssets(filePath, _product?.Name ?? "Product", _productId);

                        var productImage = ProductImageHelper.CreateProductImage(_productId, relativePath, "Upload");
                        _context.ProductImages.Add(productImage);
                    }

                    await _context.SaveChangesAsync();
                    LoadImages();
                    MessageBox.Show("Images uploaded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error uploading images: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async System.Threading.Tasks.Task SetPrimaryImage(ProductImage productImage)
        {
            try
            {
                // Remove primary from all other images
                var allImages = await _context.ProductImages
                    .Where(img => img.ProductId == _productId)
                    .ToListAsync();

                foreach (var img in allImages)
                {
                    img.IsPrimary = img.Id == productImage.Id;
                    img.UpdatedAt = DateTime.Now;
                }

                await _context.SaveChangesAsync();
                LoadImages();
                MessageBox.Show("Primary image updated!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting primary image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ViewFullImage(ProductImage productImage)
        {
            try
            {
                var viewForm = new Form
                {
                    Text = $"View Image - {productImage.ImageName}",
                    Size = new Size(800, 600),
                    StartPosition = FormStartPosition.CenterParent,
                    BackColor = Color.Black
                };

                var pictureBox = new PictureBox
                {
                    Dock = DockStyle.Fill,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BackColor = Color.Black
                };

                var image = ProductImageHelper.LoadImage(productImage.ImagePath);
                if (image != null)
                {
                    pictureBox.Image = image;
                }

                viewForm.Controls.Add(pictureBox);
                viewForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error viewing image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void EditImageDescription(ProductImage productImage)
        {
            var editForm = new Form
            {
                Text = "Edit Image Description",
                Size = new Size(400, 200),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var label = new Label
            {
                Text = "Description:",
                Location = new Point(20, 20),
                Size = new Size(350, 20),
                Font = new Font("Segoe UI", 10F)
            };
            editForm.Controls.Add(label);

            var descTextBox = new TextBox
            {
                Text = productImage.Description ?? "",
                Location = new Point(20, 50),
                Size = new Size(340, 60),
                Multiline = true,
                Font = new Font("Segoe UI", 10F)
            };
            editForm.Controls.Add(descTextBox);

            var saveButton = new Button
            {
                Text = "Save",
                Location = new Point(170, 120),
                Size = new Size(90, 35),
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            saveButton.FlatAppearance.BorderSize = 0;
            saveButton.Click += async (s, ev) =>
            {
                try
                {
                    productImage.Description = descTextBox.Text;
                    productImage.UpdatedAt = DateTime.Now;
                    await _context.SaveChangesAsync();

                    editForm.Close();
                    LoadImages();
                    MessageBox.Show("Description updated!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating description: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            editForm.Controls.Add(saveButton);

            var cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(270, 120),
                Size = new Size(90, 35),
                BackColor = Color.FromArgb(150, 150, 150),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F)
            };
            cancelButton.FlatAppearance.BorderSize = 0;
            cancelButton.Click += (s, ev) => editForm.Close();
            editForm.Controls.Add(cancelButton);

            editForm.ShowDialog();
        }

        private async System.Threading.Tasks.Task DeleteImage(ProductImage productImage)
        {
            try
            {
                var result = MessageBox.Show(
                    "Are you sure you want to delete this image?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    // Delete from database
                    _context.ProductImages.Remove(productImage);
                    await _context.SaveChangesAsync();

                    // Delete physical file if it's a local/upload image
                    if (productImage.ImageType == "Local" || productImage.ImageType == "Upload")
                    {
                        ProductImageHelper.DeleteImage(productImage.ImagePath);
                    }

                    LoadImages();
                    MessageBox.Show("Image deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
