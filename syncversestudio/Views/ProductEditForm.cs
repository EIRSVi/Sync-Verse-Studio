using SyncVerseStudio.Services;
using SyncVerseStudio.Data;
using SyncVerseStudio.Models;
using SyncVerseStudio.Helpers;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace SyncVerseStudio.Views
{
    public partial class ProductEditForm : Form
    {
        private readonly AuthenticationService _authService;
        private readonly ApplicationDbContext _context;
        private readonly int? _productId;
        private Product? _product;

        private TextBox nameTextBox, descriptionTextBox, barcodeTextBox, skuTextBox;
        private NumericUpDown costPriceNumeric, sellingPriceNumeric, quantityNumeric, minQuantityNumeric;
        private ComboBox categoryCombo, supplierCombo;
        private Button saveButton, cancelButton, generateBarcodeButton, manageImagesButton;
        private Label titleLabel;
        private Panel imagePreviewPanel;
        private PictureBox primaryImageBox;
        private Label imageCountLabel;
        private FlowLayoutPanel thumbnailsPanel;
        private List<ProductImage> tempProductImages = new List<ProductImage>();

        public ProductEditForm(AuthenticationService authService, int? productId = null)
        {
            _authService = authService;
            _context = new ApplicationDbContext();
            _productId = productId;
            
            InitializeComponent();
            
            // Load data after form is shown
            this.Load += async (s, e) => 
            {
                await LoadComboBoxDataAsync();
                if (_productId.HasValue)
                {
                    await LoadProductAsync();
                }
            };
        }

        private void InitializeComponent()
        {
            this.AutoScaleDimensions = new SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.White;
            this.ClientSize = new Size(1000, 700);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProductEditForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = _productId.HasValue ? "Edit Product" : "Add Product";

            CreateControls();
            CreateImagePreviewPanel();
        }

        private void CreateControls()
        {
            int yPos = 20;
            int leftMargin = 30;
            int controlWidth = 300;
            int labelHeight = 25;
            int controlHeight = 30;
            int spacing = 15;

            // Title
            titleLabel = new Label
            {
                Text = _productId.HasValue ? "Edit Product" : "Add New Product",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                Location = new Point(leftMargin, yPos),
                Size = new Size(400, 30)
            };
            this.Controls.Add(titleLabel);
            yPos += 50;

            // Product Name
            AddLabel("Product Name *", leftMargin, yPos);
            yPos += labelHeight + 5;
            nameTextBox = new TextBox
            {
                Location = new Point(leftMargin, yPos),
                Size = new Size(controlWidth, controlHeight),
                Font = new Font("Segoe UI", 10F)
            };
            this.Controls.Add(nameTextBox);
            yPos += controlHeight + spacing;

            // Description
            AddLabel("Description", leftMargin, yPos);
            yPos += labelHeight + 5;
            descriptionTextBox = new TextBox
            {
                Location = new Point(leftMargin, yPos),
                Size = new Size(controlWidth, controlHeight * 2),
                Font = new Font("Segoe UI", 10F),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };
            this.Controls.Add(descriptionTextBox);
            yPos += (controlHeight * 2) + spacing;

            // SKU and Barcode row
            AddLabel("SKU", leftMargin, yPos);
            AddLabel("Barcode", leftMargin + 200, yPos);
            yPos += labelHeight + 5;

            skuTextBox = new TextBox
            {
                Location = new Point(leftMargin, yPos),
                Size = new Size(150, controlHeight),
                Font = new Font("Segoe UI", 10F)
            };
            this.Controls.Add(skuTextBox);

            barcodeTextBox = new TextBox
            {
                Location = new Point(leftMargin + 200, yPos),
                Size = new Size(120, controlHeight),
                Font = new Font("Segoe UI", 10F)
            };
            this.Controls.Add(barcodeTextBox);

            generateBarcodeButton = new Button
            {
                Text = "Generate",
                Location = new Point(leftMargin + 330, yPos),
                Size = new Size(70, controlHeight),
                BackColor = Color.FromArgb(37, 99, 102),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8F)
            };
            generateBarcodeButton.FlatAppearance.BorderSize = 0;
            generateBarcodeButton.Click += GenerateBarcodeButton_Click;
            this.Controls.Add(generateBarcodeButton);
            yPos += controlHeight + spacing;

            // Category and Supplier row
            AddLabel("Category", leftMargin, yPos);
            AddLabel("Supplier", leftMargin + 200, yPos);
            yPos += labelHeight + 5;

            categoryCombo = new ComboBox
            {
                Location = new Point(leftMargin, yPos),
                Size = new Size(150, controlHeight),
                Font = new Font("Segoe UI", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            this.Controls.Add(categoryCombo);

            supplierCombo = new ComboBox
            {
                Location = new Point(leftMargin + 200, yPos),
                Size = new Size(150, controlHeight),
                Font = new Font("Segoe UI", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            this.Controls.Add(supplierCombo);
            yPos += controlHeight + spacing;

            // Price row
            AddLabel("Cost Price *", leftMargin, yPos);
            AddLabel("Selling Price *", leftMargin + 200, yPos);
            yPos += labelHeight + 5;

            costPriceNumeric = new NumericUpDown
            {
                Location = new Point(leftMargin, yPos),
                Size = new Size(150, controlHeight),
                Font = new Font("Segoe UI", 10F),
                DecimalPlaces = 2,
                Maximum = 999999,
                Minimum = 0
            };
            this.Controls.Add(costPriceNumeric);

            sellingPriceNumeric = new NumericUpDown
            {
                Location = new Point(leftMargin + 200, yPos),
                Size = new Size(150, controlHeight),
                Font = new Font("Segoe UI", 10F),
                DecimalPlaces = 2,
                Maximum = 999999,
                Minimum = 0
            };
            this.Controls.Add(sellingPriceNumeric);
            yPos += controlHeight + spacing;

            // Quantity row
            AddLabel("Current Stock", leftMargin, yPos);
            AddLabel("Minimum Stock", leftMargin + 200, yPos);
            yPos += labelHeight + 5;

            quantityNumeric = new NumericUpDown
            {
                Location = new Point(leftMargin, yPos),
                Size = new Size(150, controlHeight),
                Font = new Font("Segoe UI", 10F),
                Maximum = 999999,
                Minimum = 0
            };
            this.Controls.Add(quantityNumeric);

            minQuantityNumeric = new NumericUpDown
            {
                Location = new Point(leftMargin + 200, yPos),
                Size = new Size(150, controlHeight),
                Font = new Font("Segoe UI", 10F),
                Maximum = 999999,
                Minimum = 1,
                Value = 10
            };
            this.Controls.Add(minQuantityNumeric);
            yPos += controlHeight + 40;

            // Buttons
            // Manage Images button (only for Inventory Clerk role)
            var currentUser = _authService.CurrentUser;
            bool canManageImages = currentUser?.Role == UserRole.InventoryClerk || currentUser?.Role == UserRole.Administrator;
            
            if (canManageImages)
            {
                manageImagesButton = new Button
                {
                    Text = "📷 Manage Images",
                    Location = new Point(leftMargin, yPos),
                    Size = new Size(140, 35),
                    BackColor = Color.FromArgb(59, 130, 246),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    Enabled = _productId.HasValue // Enable only after product is saved
                };
                manageImagesButton.FlatAppearance.BorderSize = 0;
                manageImagesButton.Click += ManageImagesButton_Click;
                this.Controls.Add(manageImagesButton);

                if (!_productId.HasValue)
                {
                    var noteLabel = new Label
                    {
                        Text = "Save product first to add images",
                        Font = new Font("Segoe UI", 8F, FontStyle.Italic),
                        ForeColor = Color.FromArgb(150, 150, 150),
                        Location = new Point(leftMargin, yPos + 40),
                        Size = new Size(200, 15),
                        BackColor = Color.Transparent
                    };
                    this.Controls.Add(noteLabel);
                }
            }

            cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(leftMargin + 160, yPos),
                Size = new Size(80, 35),
                BackColor = Color.FromArgb(158, 158, 158),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                DialogResult = DialogResult.Cancel
            };
            cancelButton.FlatAppearance.BorderSize = 0;
            this.Controls.Add(cancelButton);

            saveButton = new Button
            {
                Text = "Save",
                Location = new Point(leftMargin + 250, yPos),
                Size = new Size(80, 35),
                BackColor = Color.FromArgb(24, 119, 18),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            saveButton.FlatAppearance.BorderSize = 0;
            saveButton.Click += SaveButton_Click;
            this.Controls.Add(saveButton);

            this.CancelButton = cancelButton;
            this.AcceptButton = saveButton;
        }

        private void AddLabel(string text, int x, int y)
        {
            var label = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = Color.FromArgb(64, 64, 64),
                Location = new Point(x, y),
                Size = new Size(150, 25)
            };
            this.Controls.Add(label);
        }

        private async System.Threading.Tasks.Task LoadComboBoxDataAsync()
        {
            try
            {
                using var context = new ApplicationDbContext();
                
                // Load categories
                var categories = await context.Categories
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.Name)
                    .ToListAsync();

                categoryCombo.Items.Clear();
                categoryCombo.Items.Add(new { Id = (int?)null, Name = "No Category" });
                foreach (var category in categories)
                {
                    categoryCombo.Items.Add(new { Id = (int?)category.Id, Name = category.Name });
                }
                categoryCombo.DisplayMember = "Name";
                categoryCombo.ValueMember = "Id";
                categoryCombo.SelectedIndex = 0;

                // Load suppliers
                var suppliers = await context.Suppliers
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.Name)
                    .ToListAsync();

                supplierCombo.Items.Clear();
                supplierCombo.Items.Add(new { Id = (int?)null, Name = "No Supplier" });
                foreach (var supplier in suppliers)
                {
                    supplierCombo.Items.Add(new { Id = (int?)supplier.Id, Name = supplier.Name });
                }
                supplierCombo.DisplayMember = "Name";
                supplierCombo.ValueMember = "Id";
                supplierCombo.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async System.Threading.Tasks.Task LoadProductAsync()
        {
            try
            {
                using var context = new ApplicationDbContext();
                _product = await context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Supplier)
                    .Include(p => p.ProductImages)
                    .FirstOrDefaultAsync(p => p.Id == _productId);

                if (_product != null)
                {
                    // Debug: Show what product was loaded
                    this.Text = $"Edit Product - {_product.Name}";
                    
                    // Populate form fields
                    nameTextBox.Text = _product.Name ?? "";
                    descriptionTextBox.Text = _product.Description ?? "";
                    barcodeTextBox.Text = _product.Barcode ?? "";
                    skuTextBox.Text = _product.SKU ?? "";
                    costPriceNumeric.Value = _product.CostPrice;
                    sellingPriceNumeric.Value = _product.SellingPrice;
                    quantityNumeric.Value = _product.Quantity;
                    minQuantityNumeric.Value = _product.MinQuantity;

                    // Set category
                    if (_product.CategoryId.HasValue)
                    {
                        for (int i = 0; i < categoryCombo.Items.Count; i++)
                        {
                            var item = categoryCombo.Items[i];
                            var categoryIdProperty = item.GetType().GetProperty("Id");
                            if (categoryIdProperty != null)
                            {
                                var categoryId = categoryIdProperty.GetValue(item);
                                if (categoryId != null && categoryId.Equals(_product.CategoryId.Value))
                                {
                                    categoryCombo.SelectedIndex = i;
                                    break;
                                }
                            }
                        }
                    }

                    // Set supplier
                    if (_product.SupplierId.HasValue)
                    {
                        for (int i = 0; i < supplierCombo.Items.Count; i++)
                        {
                            var item = supplierCombo.Items[i];
                            var supplierIdProperty = item.GetType().GetProperty("Id");
                            if (supplierIdProperty != null)
                            {
                                var supplierId = supplierIdProperty.GetValue(item);
                                if (supplierId != null && supplierId.Equals(_product.SupplierId.Value))
                                {
                                    supplierCombo.SelectedIndex = i;
                                    break;
                                }
                            }
                        }
                    }

                    // Load product images
                    LoadProductImages();
                }
                else
                {
                    MessageBox.Show($"Product with ID {_productId} not found.", "Product Not Found", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading product: {ex.Message}\n\nStack Trace: {ex.StackTrace}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateBarcodeButton_Click(object sender, EventArgs e)
        {
            // Generate a random barcode
            var random = new Random();
            var barcode = "";
            for (int i = 0; i < 13; i++)
            {
                barcode += random.Next(0, 10).ToString();
            }
            barcodeTextBox.Text = barcode;
        }

        private async void SaveButton_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                saveButton.Enabled = false;
                saveButton.Text = "Saving...";

                var categoryItem = categoryCombo.SelectedItem as dynamic;
                var supplierItem = supplierCombo.SelectedItem as dynamic;

                if (_product == null)
                {
                    // Create new product
                    _product = new Product
                    {
                        Name = nameTextBox.Text.Trim(),
                        Description = string.IsNullOrEmpty(descriptionTextBox.Text.Trim()) ? null : descriptionTextBox.Text.Trim(),
                        Barcode = string.IsNullOrEmpty(barcodeTextBox.Text.Trim()) ? null : barcodeTextBox.Text.Trim(),
                        SKU = string.IsNullOrEmpty(skuTextBox.Text.Trim()) ? null : skuTextBox.Text.Trim(),
                        CategoryId = categoryItem?.Id,
                        SupplierId = supplierItem?.Id,
                        CostPrice = costPriceNumeric.Value,
                        SellingPrice = sellingPriceNumeric.Value,
                        Quantity = (int)quantityNumeric.Value,
                        MinQuantity = (int)minQuantityNumeric.Value,
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    _context.Products.Add(_product);
                }
                else
                {
                    // Update existing product
                    _product.Name = nameTextBox.Text.Trim();
                    _product.Description = string.IsNullOrEmpty(descriptionTextBox.Text.Trim()) ? null : descriptionTextBox.Text.Trim();
                    _product.Barcode = string.IsNullOrEmpty(barcodeTextBox.Text.Trim()) ? null : barcodeTextBox.Text.Trim();
                    _product.SKU = string.IsNullOrEmpty(skuTextBox.Text.Trim()) ? null : skuTextBox.Text.Trim();
                    _product.CategoryId = categoryItem?.Id;
                    _product.SupplierId = supplierItem?.Id;
                    _product.CostPrice = costPriceNumeric.Value;
                    _product.SellingPrice = sellingPriceNumeric.Value;
                    _product.Quantity = (int)quantityNumeric.Value;
                    _product.MinQuantity = (int)minQuantityNumeric.Value;
                    _product.UpdatedAt = DateTime.Now;
                }

                await _context.SaveChangesAsync();

                // Save temporary images if any
                if (tempProductImages.Any())
                {
                    foreach (var tempImage in tempProductImages)
                    {
                        tempImage.ProductId = _product.Id;
                        _context.ProductImages.Add(tempImage);
                    }
                    
                    await _context.SaveChangesAsync();
                    tempProductImages.Clear();
                }

                MessageBox.Show("Product saved successfully!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving product: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                saveButton.Enabled = true;
                saveButton.Text = "Save";
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(nameTextBox.Text))
            {
                MessageBox.Show("Product name is required.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nameTextBox.Focus();
                return false;
            }

            if (costPriceNumeric.Value <= 0)
            {
                MessageBox.Show("Cost price must be greater than 0.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                costPriceNumeric.Focus();
                return false;
            }

            if (sellingPriceNumeric.Value <= 0)
            {
                MessageBox.Show("Selling price must be greater than 0.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                sellingPriceNumeric.Focus();
                return false;
            }

            return true;
        }

        private void CreateImagePreviewPanel()
        {
            // Image preview panel on the right side
            imagePreviewPanel = new Panel
            {
                Location = new Point(450, 70),
                Size = new Size(520, 580),
                BackColor = Color.FromArgb(248, 250, 252),
                BorderStyle = BorderStyle.FixedSingle
            };

            var headerLabel = new Label
            {
                Text = "Product Images",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(10, 10),
                Size = new Size(150, 25),
                BackColor = Color.Transparent
            };
            imagePreviewPanel.Controls.Add(headerLabel);

            imageCountLabel = new Label
            {
                Text = "No images",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(120, 120, 120),
                Location = new Point(10, 35),
                Size = new Size(100, 20),
                BackColor = Color.Transparent
            };
            imagePreviewPanel.Controls.Add(imageCountLabel);

            // Primary image display
            primaryImageBox = new PictureBox
            {
                Location = new Point(25, 70),
                Size = new Size(470, 300),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Cursor = Cursors.Hand
            };
            primaryImageBox.Click += PrimaryImageBox_Click;
            imagePreviewPanel.Controls.Add(primaryImageBox);

            var placeholderLabel = new Label
            {
                Text = "No primary image\nClick 'Add Images' to add",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(150, 150, 150),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(25, 180),
                Size = new Size(470, 50),
                BackColor = Color.Transparent,
                Parent = primaryImageBox
            };

            // Image management buttons
            var addImageBtn = new Button
            {
                Text = "📷 Add Images",
                Location = new Point(25, 385),
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(34, 197, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            addImageBtn.FlatAppearance.BorderSize = 0;
            addImageBtn.Click += AddImageButton_Click;
            imagePreviewPanel.Controls.Add(addImageBtn);

            var addUrlBtn = new Button
            {
                Text = "🌐 Add URL",
                Location = new Point(155, 385),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            addUrlBtn.FlatAppearance.BorderSize = 0;
            addUrlBtn.Click += AddUrlImageButton_Click;
            imagePreviewPanel.Controls.Add(addUrlBtn);

            var clearImagesBtn = new Button
            {
                Text = "🗑️ Clear",
                Location = new Point(265, 385),
                Size = new Size(80, 35),
                BackColor = Color.FromArgb(239, 68, 68),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            clearImagesBtn.FlatAppearance.BorderSize = 0;
            clearImagesBtn.Click += ClearImagesButton_Click;
            imagePreviewPanel.Controls.Add(clearImagesBtn);

            var setPrimaryBtn = new Button
            {
                Text = "⭐ Set Primary",
                Location = new Point(355, 385),
                Size = new Size(110, 35),
                BackColor = Color.FromArgb(249, 115, 22),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            setPrimaryBtn.FlatAppearance.BorderSize = 0;
            setPrimaryBtn.Click += SetPrimaryImageButton_Click;
            imagePreviewPanel.Controls.Add(setPrimaryBtn);

            // Image thumbnails panel
            thumbnailsPanel = new FlowLayoutPanel
            {
                Location = new Point(25, 430),
                Size = new Size(470, 120),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true
            };
            imagePreviewPanel.Controls.Add(thumbnailsPanel);

            this.Controls.Add(imagePreviewPanel);

            // Load images if editing existing product
            if (_productId.HasValue)
            {
                LoadProductImages();
            }
        }

        private async void LoadProductImages()
        {
            try
            {
                if (_productId.HasValue)
                {
                    using var context = new ApplicationDbContext();
                    var images = await context.ProductImages
                        .Where(img => img.ProductId == _productId && img.IsActive)
                        .OrderByDescending(img => img.IsPrimary)
                        .ThenBy(img => img.DisplayOrder)
                        .ToListAsync();

                    // Load existing images into temp collection for editing
                    tempProductImages.Clear();
                    tempProductImages.AddRange(images);
                }

                RefreshImageDisplay();
            }
            catch (Exception ex)
            {
                imageCountLabel.Text = $"Error loading images: {ex.Message}";
            }
        }

        private void AddImageButton_Click(object? sender, EventArgs e)
        {
            using var dialog = new OpenFileDialog
            {
                Title = "Select Product Images",
                Filter = ProductImageHelper.GetSupportedExtensionsFilter(),
                Multiselect = true
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var addedCount = 0;
                var errorCount = 0;

                foreach (var filePath in dialog.FileNames)
                {
                    try
                    {
                        // Validate image type
                        if (!ProductImageHelper.IsSupportedImageType(filePath))
                        {
                            MessageBox.Show($"Unsupported image type: {Path.GetFileName(filePath)}\n\nSupported formats: JPG, PNG, BMP, GIF, TIFF, WebP, ICO", 
                                "Unsupported Format", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            errorCount++;
                            continue;
                        }

                        // Copy image to assets folder
                        string relativePath = ProductImageHelper.CopyImageToAssets(filePath);
                        
                        if (!string.IsNullOrEmpty(relativePath))
                        {
                            var productImage = new ProductImage
                            {
                                ProductId = _productId ?? 0, // Will be set when product is saved
                                ImagePath = relativePath,
                                IsPrimary = tempProductImages.Count == 0, // First image is primary
                                DisplayOrder = tempProductImages.Count + 1,
                                IsActive = true,
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now
                            };

                            tempProductImages.Add(productImage);
                            addedCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error adding image {Path.GetFileName(filePath)}: {ex.Message}", 
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        errorCount++;
                    }
                }

                RefreshImageDisplay();

                // Show summary message
                if (addedCount > 0)
                {
                    var message = $"Successfully added {addedCount} image(s)";
                    if (errorCount > 0)
                    {
                        message += $"\n{errorCount} image(s) failed to add";
                    }
                    MessageBox.Show(message, "Images Added", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void AddUrlImageButton_Click(object? sender, EventArgs e)
        {
            using var urlForm = new Form
            {
                Text = "Add Image from URL",
                Size = new Size(450, 150),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var urlLabel = new Label
            {
                Text = "Image URL:",
                Location = new Point(20, 20),
                Size = new Size(80, 25)
            };
            urlForm.Controls.Add(urlLabel);

            var urlTextBox = new TextBox
            {
                Location = new Point(20, 45),
                Size = new Size(400, 25),
                PlaceholderText = "https://example.com/image.jpg"
            };
            urlForm.Controls.Add(urlTextBox);

            var okButton = new Button
            {
                Text = "Add",
                Location = new Point(265, 80),
                Size = new Size(75, 30),
                DialogResult = DialogResult.OK,
                BackColor = Color.FromArgb(34, 197, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            okButton.FlatAppearance.BorderSize = 0;
            urlForm.Controls.Add(okButton);

            var cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(345, 80),
                Size = new Size(75, 30),
                DialogResult = DialogResult.Cancel,
                BackColor = Color.FromArgb(156, 163, 175),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            cancelButton.FlatAppearance.BorderSize = 0;
            urlForm.Controls.Add(cancelButton);

            urlForm.AcceptButton = okButton;
            urlForm.CancelButton = cancelButton;

            if (urlForm.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(urlTextBox.Text))
            {
                var url = urlTextBox.Text.Trim();
                
                if (ProductImageHelper.IsValidImageUrl(url))
                {
                    var productImage = new ProductImage
                    {
                        ProductId = _productId ?? 0,
                        ImagePath = url,
                        IsPrimary = tempProductImages.Count == 0,
                        DisplayOrder = tempProductImages.Count + 1,
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    tempProductImages.Add(productImage);
                    RefreshImageDisplay();
                }
                else
                {
                    MessageBox.Show("Please enter a valid image URL", "Invalid URL", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void ClearImagesButton_Click(object? sender, EventArgs e)
        {
            if (tempProductImages.Any())
            {
                var result = MessageBox.Show("Are you sure you want to clear all images?", 
                    "Clear Images", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    tempProductImages.Clear();
                    RefreshImageDisplay();
                }
            }
        }

        private void SetPrimaryImageButton_Click(object? sender, EventArgs e)
        {
            if (tempProductImages.Any())
            {
                // Show selection dialog for primary image
                using var selectForm = new Form
                {
                    Text = "Select Primary Image",
                    Size = new Size(400, 300),
                    StartPosition = FormStartPosition.CenterParent,
                    FormBorderStyle = FormBorderStyle.FixedDialog
                };

                var listBox = new ListBox
                {
                    Location = new Point(20, 20),
                    Size = new Size(350, 200),
                    DisplayMember = "DisplayName"
                };

                for (int i = 0; i < tempProductImages.Count; i++)
                {
                    var img = tempProductImages[i];
                    listBox.Items.Add(new { 
                        Index = i, 
                        DisplayName = $"Image {i + 1} - {Path.GetFileName(img.ImagePath)}" 
                    });
                }

                selectForm.Controls.Add(listBox);

                var okBtn = new Button
                {
                    Text = "Set Primary",
                    Location = new Point(220, 240),
                    Size = new Size(80, 30),
                    DialogResult = DialogResult.OK
                };
                selectForm.Controls.Add(okBtn);

                var cancelBtn = new Button
                {
                    Text = "Cancel",
                    Location = new Point(310, 240),
                    Size = new Size(60, 30),
                    DialogResult = DialogResult.Cancel
                };
                selectForm.Controls.Add(cancelBtn);

                if (selectForm.ShowDialog() == DialogResult.OK && listBox.SelectedItem != null)
                {
                    var selectedIndex = ((dynamic)listBox.SelectedItem).Index;
                    
                    // Reset all to non-primary
                    foreach (var img in tempProductImages)
                    {
                        img.IsPrimary = false;
                    }
                    
                    // Set selected as primary
                    tempProductImages[selectedIndex].IsPrimary = true;
                    RefreshImageDisplay();
                }
            }
        }

        private void RefreshImageDisplay()
        {
            // Update count
            imageCountLabel.Text = tempProductImages.Any() ? $"{tempProductImages.Count} image(s)" : "No images";

            // Clear thumbnails
            thumbnailsPanel.Controls.Clear();

            // Update primary image
            var primaryImage = tempProductImages.FirstOrDefault(img => img.IsPrimary) ?? tempProductImages.FirstOrDefault();
            
            if (primaryImage != null)
            {
                try
                {
                    var image = ProductImageHelper.LoadImage(primaryImage.ImagePath);
                    if (image != null)
                    {
                        primaryImageBox.Image = ProductImageHelper.ResizeImage(image, 470, 300);
                        
                        // Remove placeholder
                        foreach (Control ctrl in primaryImageBox.Controls.OfType<Label>().ToList())
                        {
                            primaryImageBox.Controls.Remove(ctrl);
                        }
                    }
                }
                catch
                {
                    // Show placeholder on error
                    ShowImagePlaceholder();
                }

                // Add thumbnails
                for (int i = 0; i < tempProductImages.Count; i++)
                {
                    var img = tempProductImages[i];
                    var thumbnail = CreateThumbnail(img, i);
                    thumbnailsPanel.Controls.Add(thumbnail);
                }
            }
            else
            {
                ShowImagePlaceholder();
            }
        }

        private void ShowImagePlaceholder()
        {
            primaryImageBox.Image = null;
            
            // Remove existing placeholder
            foreach (Control ctrl in primaryImageBox.Controls.OfType<Label>().ToList())
            {
                primaryImageBox.Controls.Remove(ctrl);
            }

            // Add new placeholder
            var placeholderLabel = new Label
            {
                Text = "No primary image\nClick 'Add Images' to add",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(150, 150, 150),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(25, 120),
                Size = new Size(420, 60),
                BackColor = Color.Transparent
            };
            primaryImageBox.Controls.Add(placeholderLabel);
        }

        private Panel CreateThumbnail(ProductImage productImage, int index)
        {
            var thumbnailPanel = new Panel
            {
                Size = new Size(80, 90),
                Margin = new Padding(5),
                BackColor = productImage.IsPrimary ? Color.FromArgb(34, 197, 94) : Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Cursor = Cursors.Hand
            };

            var pictureBox = new PictureBox
            {
                Location = new Point(5, 5),
                Size = new Size(68, 60),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White
            };

            try
            {
                var image = ProductImageHelper.LoadImage(productImage.ImagePath);
                if (image != null)
                {
                    pictureBox.Image = ProductImageHelper.ResizeImage(image, 68, 60);
                }
            }
            catch
            {
                // Show error icon
                pictureBox.BackColor = Color.FromArgb(248, 250, 252);
            }

            thumbnailPanel.Controls.Add(pictureBox);

            var indexLabel = new Label
            {
                Text = (index + 1).ToString(),
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = productImage.IsPrimary ? Color.White : Color.Black,
                Location = new Point(5, 70),
                Size = new Size(20, 15),
                BackColor = Color.Transparent
            };
            thumbnailPanel.Controls.Add(indexLabel);

            var primaryLabel = new Label
            {
                Text = productImage.IsPrimary ? "★" : "",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(55, 70),
                Size = new Size(20, 15),
                BackColor = Color.Transparent
            };
            thumbnailPanel.Controls.Add(primaryLabel);

            // Click to set as primary
            thumbnailPanel.Click += (s, e) =>
            {
                foreach (var img in tempProductImages)
                {
                    img.IsPrimary = false;
                }
                productImage.IsPrimary = true;
                RefreshImageDisplay();
            };

            // Right-click to delete
            thumbnailPanel.MouseClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    var result = MessageBox.Show($"Delete this image?\n\n{Path.GetFileName(productImage.ImagePath)}", 
                        "Delete Image", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    
                    if (result == DialogResult.Yes)
                    {
                        tempProductImages.Remove(productImage);
                        
                        // If this was the primary image, set the first remaining image as primary
                        if (productImage.IsPrimary && tempProductImages.Any())
                        {
                            tempProductImages.First().IsPrimary = true;
                        }
                        
                        RefreshImageDisplay();
                    }
                }
            };

            return thumbnailPanel;
        }

        private void PrimaryImageBox_Click(object? sender, EventArgs e)
        {
            var primaryImage = tempProductImages.FirstOrDefault(img => img.IsPrimary) ?? tempProductImages.FirstOrDefault();
            
            if (primaryImage != null && primaryImageBox.Image != null)
            {
                // Show full-size image preview
                using var previewForm = new Form
                {
                    Text = "Image Preview",
                    Size = new Size(800, 600),
                    StartPosition = FormStartPosition.CenterParent,
                    FormBorderStyle = FormBorderStyle.Sizable,
                    BackColor = Color.Black
                };

                var previewPictureBox = new PictureBox
                {
                    Dock = DockStyle.Fill,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Image = primaryImageBox.Image,
                    BackColor = Color.Black
                };

                previewForm.Controls.Add(previewPictureBox);
                previewForm.ShowDialog();
            }
        }

        private void ManageImagesButton_Click(object sender, EventArgs e)
        {
            if (_productId.HasValue)
            {
                var imageManagerForm = new ProductImageManagerForm(_productId.Value);
                imageManagerForm.FormClosed += (s, args) => LoadProductImages(); // Refresh images when form closes
                imageManagerForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please save the product first before adding images.", 
                    "Save Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
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
