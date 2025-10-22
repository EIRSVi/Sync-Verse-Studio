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
        private Button saveButton, cancelButton, generateBarcodeButton;
        private Label titleLabel;

        public ProductEditForm(AuthenticationService authService, int? productId = null)
        {
            _authService = authService;
            _context = new ApplicationDbContext();
            _productId = productId;
            
            InitializeComponent();
            LoadComboBoxData();
            
            if (_productId.HasValue)
            {
                LoadProduct();
            }
        }

        private void InitializeComponent()
        {
            this.AutoScaleDimensions = new SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.White;
            this.ClientSize = new Size(600, 650);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProductEditForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = _productId.HasValue ? "Edit Product" : "Add Product";

            CreateControls();
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
            cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(leftMargin + 150, yPos),
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
                Location = new Point(leftMargin + 240, yPos),
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

        private async void LoadComboBoxData()
        {
            try
            {
                // Load categories
                var categories = await _context.Categories
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
                var suppliers = await _context.Suppliers
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

        private async void LoadProduct()
        {
            try
            {
                _product = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Supplier)
                    .FirstOrDefaultAsync(p => p.Id == _productId);

                if (_product != null)
                {
                    nameTextBox.Text = _product.Name;
                    descriptionTextBox.Text = _product.Description ?? "";
                    barcodeTextBox.Text = _product.Barcode ?? "";
                    skuTextBox.Text = _product.SKU ?? "";
                    costPriceNumeric.Value = _product.CostPrice;
                    sellingPriceNumeric.Value = _product.SellingPrice;
                    quantityNumeric.Value = _product.Quantity;
                    minQuantityNumeric.Value = _product.MinQuantity;

                    // Set category
                    for (int i = 0; i < categoryCombo.Items.Count; i++)
                    {
                        var item = categoryCombo.Items[i] as dynamic;
                        if (item?.Id == _product.CategoryId)
                        {
                            categoryCombo.SelectedIndex = i;
                            break;
                        }
                    }

                    // Set supplier
                    for (int i = 0; i < supplierCombo.Items.Count; i++)
                    {
                        var item = supplierCombo.Items[i] as dynamic;
                        if (item?.Id == _product.SupplierId)
                        {
                            supplierCombo.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading product: {ex.Message}", "Error", 
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
