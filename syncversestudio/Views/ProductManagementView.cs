using System.Drawing;
using SyncVerseStudio.Services;
using SyncVerseStudio.Data;
using SyncVerseStudio.Models;
using SyncVerseStudio.Helpers;
using Microsoft.EntityFrameworkCore;

namespace SyncVerseStudio.Views
{
    public partial class ProductManagementView : Form
    {
        private readonly AuthenticationService _authService;
        private readonly ApplicationDbContext _context;
        private DataGridView productsGrid;
        private Panel topPanel;
        private Button addButton, editButton, deleteButton, refreshButton;
        private TextBox searchBox;
        private ComboBox categoryFilter;

        public ProductManagementView(AuthenticationService authService)
        {
            _authService = authService;
            _context = new ApplicationDbContext();
            InitializeComponent();
            LoadProducts();
            LoadCategories();
        }

        private void InitializeComponent()
        {
            this.AutoScaleDimensions = new SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(250, 250, 250);
            this.ClientSize = new Size(1000, 700);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Name = "ProductManagementView";
            this.Text = "Product Management";

            CreateTopPanel();
            CreateProductsGrid();
        }
        //Padding = new Padding(10, 200, 10, 10)
        private void CreateTopPanel()
        {
            topPanel = new Panel
            {
                BackColor = System.Drawing.Color.White,
                Dock = DockStyle.Top,
                Height = 80,
                Padding = new Padding(20, 10, 20, 10)
            };

            var titleLabel = new Label
            {
                Text = "PRODUCT MANAGEMENT",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                Location = new Point(20, 18),
                Size = new Size(400, 35)
            };

            searchBox = new TextBox
            {
                PlaceholderText = "Search products...",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(20, 45),
                Size = new Size(200, 25)
            };
            searchBox.TextChanged += SearchBox_TextChanged;

            categoryFilter = new ComboBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(240, 45),
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            categoryFilter.SelectedIndexChanged += CategoryFilter_SelectedIndexChanged;

            // Buttons
            int buttonX = 600;
            addButton = CreateButton("ADD PRODUCT", Color.FromArgb(48, 148, 255), buttonX, 45, 130);
            addButton.Click += AddButton_Click;
            buttonX += 140;

            editButton = CreateButton("EDIT", Color.FromArgb(48, 148, 255), buttonX, 45, 80);
            editButton.Click += EditButton_Click;
            buttonX += 90;

            deleteButton = CreateButton("DELETE", Color.FromArgb(255, 0, 80), buttonX, 45, 90);
            deleteButton.Click += DeleteButton_Click;
            buttonX += 100;

            refreshButton = CreateButton("REFRESH", Color.FromArgb(117, 117, 117), buttonX, 45, 100);
            refreshButton.Click += RefreshButton_Click;

            topPanel.Controls.AddRange(new Control[] {
                titleLabel, searchBox, categoryFilter, 
                addButton, editButton, deleteButton, refreshButton
            });

            this.Controls.Add(topPanel);
        }

        private Button CreateButton(string text, Color backgroundColor, int x, int y, int width)
        {
            return new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = backgroundColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Location = new Point(x, y),
                Size = new Size(width, 35),
                Cursor = Cursors.Hand
            };
        }

        private void CreateProductsGrid()
        {
            var gridPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(10, 200, 10, 10)
            };
            
            // Add rounded border to panel with larger radius
            gridPanel.Paint += (s, e) =>
            {
                var rect = new Rectangle(10, 200, gridPanel.Width - 20, gridPanel.Height - 210);
                using (var pen = new Pen(BrandTheme.TableBorder, 3))
                {
                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    var path = new System.Drawing.Drawing2D.GraphicsPath();
                    int radius = 20;
                    path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
                    path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
                    path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
                    path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
                    path.CloseFigure();
                    e.Graphics.DrawPath(pen, path);
                }
            };

            productsGrid = new DataGridView
            {
                Dock = DockStyle.Fill
            };
            
            // Apply standard BrandTheme styling - same as AuditLogView
            BrandTheme.StyleDataGridView(productsGrid);

            // Configure columns
            productsGrid.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID", Width = 50, Visible = false },
                new DataGridViewTextBoxColumn { Name = "Name", HeaderText = "Product Name", Width = 200 },
                new DataGridViewTextBoxColumn { Name = "SKU", HeaderText = "SKU", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "Category", HeaderText = "Category", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "Supplier", HeaderText = "Supplier", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "CostPrice", HeaderText = "Cost Price", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "SellingPrice", HeaderText = "Selling Price", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "Quantity", HeaderText = "Stock", Width = 80 },
                new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Status", Width = 100 }
            });

            // Format currency columns
            productsGrid.Columns["CostPrice"].DefaultCellStyle.Format = "C2";
            productsGrid.Columns["SellingPrice"].DefaultCellStyle.Format = "C2";

            gridPanel.Controls.Add(productsGrid);
            this.Controls.Add(gridPanel);
        }

        private async void LoadProducts()
        {
            try
            {
                var products = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Supplier)
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.Name)
                    .ToListAsync();

                productsGrid.Rows.Clear();

                foreach (var product in products)
                {
                    var status = product.IsLowStock ? "Low Stock" : "In Stock";
                    var rowIndex = productsGrid.Rows.Add(
                        product.Id,
                        product.Name,
                        product.SKU,
                        product.Category?.Name ?? "No Category",
                        product.Supplier?.Name ?? "No Supplier",
                        product.CostPrice,
                        product.SellingPrice,
                        product.Quantity,
                        status
                    );

                    // Keep white background for all rows
                    productsGrid.Rows[rowIndex].DefaultCellStyle.BackColor = Color.White;
                    productsGrid.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(30, 30, 30);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void LoadCategories()
        {
            try
            {
                var categories = await _context.Categories
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.Name)
                    .ToListAsync();

                categoryFilter.Items.Clear();
                categoryFilter.Items.Add("All Categories");
                
                foreach (var category in categories)
                {
                    categoryFilter.Items.Add(category.Name);
                }
                
                categoryFilter.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading categories: {ex.Message}");
            }
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            FilterProducts();
        }

        private void CategoryFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterProducts();
        }

        private async void FilterProducts()
        {
            try
            {
                var searchTerm = searchBox.Text.ToLower();
                var selectedCategory = categoryFilter.SelectedItem?.ToString();

                var query = _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Supplier)
                    .Where(p => p.IsActive);

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(p => 
                        p.Name.ToLower().Contains(searchTerm) ||
                        (p.SKU != null && p.SKU.ToLower().Contains(searchTerm)) ||
                        (p.Barcode != null && p.Barcode.ToLower().Contains(searchTerm)));
                }

                if (!string.IsNullOrEmpty(selectedCategory) && selectedCategory != "All Categories")
                {
                    query = query.Where(p => p.Category != null && p.Category.Name == selectedCategory);
                }

                var products = await query.OrderBy(p => p.Name).ToListAsync();

                productsGrid.Rows.Clear();

                foreach (var product in products)
                {
                    var status = product.IsLowStock ? "Low Stock" : "In Stock";
                    var rowIndex = productsGrid.Rows.Add(
                        product.Id,
                        product.Name,
                        product.SKU,
                        product.Category?.Name ?? "No Category",
                        product.Supplier?.Name ?? "No Supplier",
                        product.CostPrice,
                        product.SellingPrice,
                        product.Quantity,
                        status
                    );

                    // Keep white background for all rows
                    productsGrid.Rows[rowIndex].DefaultCellStyle.BackColor = Color.White;
                    productsGrid.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(30, 30, 30);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error filtering products: {ex.Message}");
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (!_authService.HasPermission("ADD_PRODUCT"))
            {
                MessageBox.Show("You don't have permission to add products.", "Access Denied", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var addForm = new ProductEditForm(_authService);
            if (addForm.ShowDialog() == DialogResult.OK)
            {
                LoadProducts();
            }
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            if (!_authService.HasPermission("EDIT_PRODUCT"))
            {
                MessageBox.Show("You don't have permission to edit products.", "Access Denied", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (productsGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a product to edit.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var productId = (int)productsGrid.SelectedRows[0].Cells["Id"].Value;
            var editForm = new ProductEditForm(_authService, productId);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadProducts();
            }
        }

        private async void DeleteButton_Click(object sender, EventArgs e)
        {
            if (!_authService.HasPermission("DELETE_PRODUCT"))
            {
                MessageBox.Show("You don't have permission to delete products.", "Access Denied", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (productsGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a product to delete.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var productName = productsGrid.SelectedRows[0].Cells["Name"].Value?.ToString();
            var result = MessageBox.Show($"Are you sure you want to delete the product '{productName}'?", 
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    var productId = (int)productsGrid.SelectedRows[0].Cells["Id"].Value;
                    var product = await _context.Products.FindAsync(productId);
                    
                    if (product != null)
                    {
                        product.IsActive = false;
                        await _context.SaveChangesAsync();
                        LoadProducts();
                        
                        MessageBox.Show("Product deleted successfully.", "Success", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting product: {ex.Message}", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            LoadProducts();
            LoadCategories();
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
