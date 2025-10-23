using SyncVerseStudio.Services;
using SyncVerseStudio.Data;
using SyncVerseStudio.Models;
using Microsoft.EntityFrameworkCore;

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
            this.ClientSize = new Size(850, 650);
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
                Location = new Point(400, 80),
                Size = new Size(420, 500),
                BackColor = Color.FromArgb(248, 250, 252),
                BorderStyle = BorderStyle.FixedSingle
            };

            var headerLabel = new Label
            {
                Text = "Product Images",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(10, 10),
                Size = new Size(400, 25),
                BackColor = Color.Transparent
            };
            imagePreviewPanel.Controls.Add(headerLabel);

            imageCountLabel = new Label
            {
                Text = "No images",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(120, 120, 120),
                Location = new Point(10, 35),
                Size = new Size(400, 20),
                BackColor = Color.Transparent
            };
            imagePreviewPanel.Controls.Add(imageCountLabel);

            primaryImageBox = new PictureBox
            {
                Location = new Point(10, 60),
                Size = new Size(400, 400),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            imagePreviewPanel.Controls.Add(primaryImageBox);

            var placeholderLabel = new Label
            {
                Text = "No primary image\nClick 'Manage Images' to add",
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.FromArgb(150, 150, 150),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(50, 200),
                Size = new Size(300, 60),
                BackColor = Color.Transparent,
                Parent = primaryImageBox
            };

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
                using var context = new ApplicationDbContext();
                var images = await context.ProductImages
                    .Where(img => img.ProductId == _productId && img.IsActive)
                    .OrderByDescending(img => img.IsPrimary)
                    .ThenBy(img => img.DisplayOrder)
                    .ToListAsync();

                if (images.Any())
                {
                    imageCountLabel.Text = $"{images.Count} image(s)";

                    var primaryImage = images.FirstOrDefault(img => img.IsPrimary) ?? images.First();
                    var image = Helpers.ProductImageHelper.LoadImage(primaryImage.ImagePath);
                    
                    if (image != null)
                    {
                        primaryImageBox.Image = Helpers.ProductImageHelper.ResizeImage(image, 400, 400);
                        
                        // Remove placeholder
                        foreach (Control ctrl in primaryImageBox.Controls.OfType<Label>().ToList())
                        {
                            primaryImageBox.Controls.Remove(ctrl);
                        }
                    }
                    else
                    {
                        // Show default brand image
                        var defaultImage = Helpers.ProductImageHelper.GetDefaultBrandImage();
                        if (defaultImage != null)
                        {
                            primaryImageBox.Image = Helpers.ProductImageHelper.ResizeImage(defaultImage, 400, 400);
                        }
                    }
                }
                else
                {
                    imageCountLabel.Text = "No images";
                    // Show default brand image when no images
                    var defaultImage = Helpers.ProductImageHelper.GetDefaultBrandImage();
                    if (defaultImage != null)
                    {
                        primaryImageBox.Image = Helpers.ProductImageHelper.ResizeImage(defaultImage, 400, 400);
                        
                        // Remove placeholder
                        foreach (Control ctrl in primaryImageBox.Controls.OfType<Label>().ToList())
                        {
                            primaryImageBox.Controls.Remove(ctrl);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                imageCountLabel.Text = $"Error loading images: {ex.Message}";
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
