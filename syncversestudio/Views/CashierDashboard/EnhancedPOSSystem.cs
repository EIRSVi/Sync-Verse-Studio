using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SyncVerseStudio.Data;
using SyncVerseStudio.Models;
using SyncVerseStudio.Services;
using FontAwesome.Sharp;
using QRCoder;

namespace SyncVerseStudio.Views.CashierDashboard
{
    public partial class EnhancedPOSSystem : Form
    {
        private readonly AuthenticationService _authService;
        private readonly ApplicationDbContext _context;
        private List<POSCartItem> cartItems = new List<POSCartItem>();
        
        // UI Controls
        private ComboBox cmbCategory;
        private ComboBox cmbSortBy;
        private TextBox txtSearch;
        private FlowLayoutPanel productsPanel;
        private DataGridView cartGrid;
        private Label lblSubtotal, lblTax, lblTotal;
        private NumericUpDown numTaxRate;
        private RadioButton rbCash, rbCard, rbMobile;
        private TextBox txtCashAmount;
        private Label lblChange;
        private Button btnCompleteSale;
        
        // Tax settings
        private decimal taxRate = 10m; // Default 10%
        
        public EnhancedPOSSystem(AuthenticationService authService)
        {
            _authService = authService;
            _context = new ApplicationDbContext();
            InitializeComponent();
            LoadCategories();
            LoadProducts();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(248, 250, 252);
            this.Size = new Size(1600, 900);
            this.Dock = DockStyle.Fill;

            CreatePOSLayout();
            this.ResumeLayout(false);
        }

        private void CreatePOSLayout()
        {
            // Main container
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = Color.FromArgb(248, 250, 252)
            };

            // Header
            var header = CreateHeader();
            header.Location = new Point(0, 0);
            mainPanel.Controls.Add(header);

            // Products Panel (Left)
            var productsContainer = CreateProductsContainer();
            productsContainer.Location = new Point(0, 90);
            productsContainer.Size = new Size(950, 790);
            mainPanel.Controls.Add(productsContainer);

            // Cart Panel (Right)
            var cartContainer = CreateCartContainer();
            cartContainer.Location = new Point(970, 90);
            cartContainer.Size = new Size(610, 790);
            mainPanel.Controls.Add(cartContainer);

            this.Controls.Add(mainPanel);
        }

        private Panel CreateHeader()
        {
            var panel = new Panel
            {
                Size = new Size(1560, 80),
                BackColor = Color.White
            };

            panel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, panel.Width - 1, panel.Height - 1), 12))
                using (var brush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillPath(brush, path);
                }
            };

            var logoIcon = new IconPictureBox
            {
                IconChar = IconChar.CashRegister,
                IconColor = Color.FromArgb(20, 184, 166),
                IconSize = 32,
                Location = new Point(25, 24),
                Size = new Size(32, 32),
                BackColor = Color.Transparent
            };

            var titleLabel = new Label
            {
                Text = "Point of Sale System",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(65, 22),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            var dateLabel = new Label
            {
                Text = DateTime.Now.ToString("MMMM dd, yyyy - HH:mm"),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(1350, 30),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            panel.Controls.AddRange(new Control[] { logoIcon, titleLabel, dateLabel });
            return panel;
        }

        private Panel CreateProductsContainer()
        {
            var panel = new Panel
            {
                BackColor = Color.White
            };

            panel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, panel.Width - 1, panel.Height - 1), 12))
                using (var brush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillPath(brush, path);
                }
            };

            var titleLabel = new Label
            {
                Text = "Products",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(25, 20),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            // Search box
            txtSearch = new TextBox
            {
                Location = new Point(25, 60),
                Size = new Size(300, 35),
                Font = new Font("Segoe UI", 11),
                PlaceholderText = "Search products..."
            };
            txtSearch.TextChanged += (s, e) => FilterProducts();

            // Category filter
            var lblCategory = new Label
            {
                Text = "Category:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(345, 65),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            cmbCategory = new ComboBox
            {
                Location = new Point(425, 60),
                Size = new Size(200, 35),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbCategory.SelectedIndexChanged += (s, e) => FilterProducts();

            // Sort by
            var lblSort = new Label
            {
                Text = "Sort by:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(645, 65),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            cmbSortBy = new ComboBox
            {
                Location = new Point(710, 60),
                Size = new Size(210, 35),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbSortBy.Items.AddRange(new string[] { 
                "Name (A-Z)", 
                "Name (Z-A)", 
                "Price (Low to High)", 
                "Price (High to Low)",
                "Most Popular",
                "Recently Added"
            });
            cmbSortBy.SelectedIndex = 0;
            cmbSortBy.SelectedIndexChanged += (s, e) => FilterProducts();

            // Products panel
            productsPanel = new FlowLayoutPanel
            {
                Location = new Point(25, 110),
                Size = new Size(900, 660),
                AutoScroll = true,
                BackColor = Color.Transparent
            };

            panel.Controls.AddRange(new Control[] { 
                titleLabel, txtSearch, lblCategory, cmbCategory, 
                lblSort, cmbSortBy, productsPanel 
            });

            return panel;
        }

        private Panel CreateCartContainer()
        {
            var panel = new Panel
            {
                BackColor = Color.FromArgb(59, 130, 246)
            };

            panel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, panel.Width - 1, panel.Height - 1), 12))
                using (var brush = new SolidBrush(Color.FromArgb(59, 130, 246)))
                {
                    e.Graphics.FillPath(brush, path);
                }
            };

            var titleLabel = new Label
            {
                Text = "Shopping Cart",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(25, 20),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            // Cart Grid
            cartGrid = new DataGridView
            {
                Location = new Point(25, 60),
                Size = new Size(560, 300),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Font = new Font("Segoe UI", 10),
                ColumnHeadersHeight = 40,
                RowTemplate = { Height = 45 },
                GridColor = Color.FromArgb(226, 232, 240),
                EnableHeadersVisualStyles = false,
                ReadOnly = true
            };

            cartGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(241, 245, 249);
            cartGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(51, 65, 85);
            cartGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            cartGrid.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);

            cartGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            cartGrid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 64, 175);

            // Define columns
            cartGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Product",
                HeaderText = "Product",
                Width = 200,
                DataPropertyName = "ProductName"
            });

            cartGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Qty",
                HeaderText = "Qty",
                Width = 70,
                DataPropertyName = "Quantity"
            });

            cartGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Price",
                HeaderText = "Price",
                Width = 100,
                DataPropertyName = "UnitPrice"
            });

            cartGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Total",
                HeaderText = "Total",
                Width = 120,
                DataPropertyName = "Total"
            });

            var btnRemove = new DataGridViewButtonColumn
            {
                Name = "Remove",
                HeaderText = "",
                Text = "✕",
                UseColumnTextForButtonValue = true,
                Width = 50
            };
            cartGrid.Columns.Add(btnRemove);

            cartGrid.CellContentClick += CartGrid_CellContentClick;

            // Summary Panel
            var summaryPanel = CreateSummaryPanel();
            summaryPanel.Location = new Point(25, 380);

            // Payment Panel
            var paymentPanel = CreatePaymentPanel();
            paymentPanel.Location = new Point(25, 540);

            // Action Buttons
            var actionsPanel = CreateActionButtons();
            actionsPanel.Location = new Point(25, 710);

            panel.Controls.AddRange(new Control[] { 
                titleLabel, cartGrid, summaryPanel, paymentPanel, actionsPanel 
            });

            return panel;
        }

        private Panel CreateSummaryPanel()
        {
            var panel = new Panel
            {
                Size = new Size(560, 140),
                BackColor = Color.White
            };

            panel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, panel.Width - 1, panel.Height - 1), 8))
                using (var brush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillPath(brush, path);
                }
            };

            int yPos = 15;

            // Subtotal
            var lblSubtotalText = new Label
            {
                Text = "Subtotal:",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(20, yPos),
                AutoSize = true
            };

            lblSubtotal = new Label
            {
                Text = "$0.00",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(450, yPos),
                AutoSize = true
            };

            yPos += 35;

            // Tax Rate Control
            var lblTaxText = new Label
            {
                Text = "Tax Rate (%):",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(20, yPos),
                AutoSize = true
            };

            numTaxRate = new NumericUpDown
            {
                Location = new Point(140, yPos - 3),
                Size = new Size(80, 25),
                Font = new Font("Segoe UI", 10),
                Minimum = 0,
                Maximum = 100,
                DecimalPlaces = 2,
                Value = 10m
            };
            numTaxRate.ValueChanged += (s, e) => { taxRate = numTaxRate.Value; UpdateCartTotals(); };

            lblTax = new Label
            {
                Text = "$0.00",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(450, yPos),
                AutoSize = true
            };

            yPos += 40;

            // Total
            var lblTotalText = new Label
            {
                Text = "TOTAL:",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(20, yPos),
                AutoSize = true
            };

            lblTotal = new Label
            {
                Text = "$0.00",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(34, 197, 94),
                Location = new Point(400, yPos),
                AutoSize = true
            };

            panel.Controls.AddRange(new Control[] { 
                lblSubtotalText, lblSubtotal, 
                lblTaxText, numTaxRate, lblTax, 
                lblTotalText, lblTotal 
            });

            return panel;
        }

        private Panel CreatePaymentPanel()
        {
            var panel = new Panel
            {
                Size = new Size(560, 150),
                BackColor = Color.White
            };

            panel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, panel.Width - 1, panel.Height - 1), 8))
                using (var brush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillPath(brush, path);
                }
            };

            var lblPaymentMethod = new Label
            {
                Text = "Payment Method:",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(20, 15),
                AutoSize = true
            };

            // Payment method radio buttons
            rbCash = new RadioButton
            {
                Text = "💵 Cash",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(20, 50),
                AutoSize = true,
                Checked = true
            };
            rbCash.CheckedChanged += PaymentMethod_Changed;

            rbCard = new RadioButton
            {
                Text = "💳 Card",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(150, 50),
                AutoSize = true
            };
            rbCard.CheckedChanged += PaymentMethod_Changed;

            rbMobile = new RadioButton
            {
                Text = "📱 Mobile/QR",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(280, 50),
                AutoSize = true
            };
            rbMobile.CheckedChanged += PaymentMethod_Changed;

            // Cash amount and change
            var lblCashAmount = new Label
            {
                Text = "Cash Amount:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(20, 90),
                AutoSize = true
            };

            txtCashAmount = new TextBox
            {
                Location = new Point(140, 87),
                Size = new Size(120, 25),
                Font = new Font("Segoe UI", 11),
                Text = "0.00"
            };
            txtCashAmount.TextChanged += (s, e) => CalculateChange();

            var lblChangeText = new Label
            {
                Text = "Change:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(300, 90),
                AutoSize = true
            };

            lblChange = new Label
            {
                Text = "$0.00",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(34, 197, 94),
                Location = new Point(380, 88),
                AutoSize = true
            };

            panel.Controls.AddRange(new Control[] { 
                lblPaymentMethod, rbCash, rbCard, rbMobile, 
                lblCashAmount, txtCashAmount, lblChangeText, lblChange 
            });

            return panel;
        }

        private Panel CreateActionButtons()
        {
            var panel = new Panel
            {
                Size = new Size(560, 60),
                BackColor = Color.Transparent
            };

            btnCompleteSale = new Button
            {
                Text = "✓ COMPLETE SALE",
                Location = new Point(0, 0),
                Size = new Size(270, 55),
                BackColor = Color.FromArgb(34, 197, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCompleteSale.FlatAppearance.BorderSize = 0;
            btnCompleteSale.Click += BtnCompleteSale_Click;

            var btnClearCart = new Button
            {
                Text = "🗑 CLEAR CART",
                Location = new Point(290, 0),
                Size = new Size(130, 55),
                BackColor = Color.FromArgb(239, 68, 68),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnClearCart.FlatAppearance.BorderSize = 0;
            btnClearCart.Click += (s, e) => ClearCart();

            var btnHold = new Button
            {
                Text = "⏸ HOLD",
                Location = new Point(440, 0),
                Size = new Size(120, 55),
                BackColor = Color.FromArgb(249, 115, 22),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnHold.FlatAppearance.BorderSize = 0;
            btnHold.Click += BtnHold_Click;

            panel.Controls.AddRange(new Control[] { btnCompleteSale, btnClearCart, btnHold });
            return panel;
        }

        // Load Methods
        private async void LoadCategories()
        {
            try
            {
                var categories = await _context.Categories
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.Name)
                    .ToListAsync();

                cmbCategory.Items.Add("All Categories");
                foreach (var category in categories)
                {
                    cmbCategory.Items.Add(category);
                }
                cmbCategory.DisplayMember = "Name";
                cmbCategory.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void LoadProducts()
        {
            try
            {
                var products = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.ProductImages)
                    .Where(p => p.IsActive && p.Quantity > 0)
                    .ToListAsync();

                DisplayProducts(products);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void FilterProducts()
        {
            try
            {
                var query = _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.ProductImages)
                    .Where(p => p.IsActive && p.Quantity > 0);

                // Category filter
                if (cmbCategory.SelectedIndex > 0 && cmbCategory.SelectedItem is Category category)
                {
                    query = query.Where(p => p.CategoryId == category.Id);
                }

                // Search filter
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    var searchTerm = txtSearch.Text.ToLower();
                    query = query.Where(p => p.Name.ToLower().Contains(searchTerm) || 
                                           p.Barcode.ToLower().Contains(searchTerm));
                }

                var products = await query.ToListAsync();

                // Sort
                if (cmbSortBy.SelectedIndex >= 0)
                {
                    products = cmbSortBy.SelectedIndex switch
                    {
                        0 => products.OrderBy(p => p.Name).ToList(),
                        1 => products.OrderByDescending(p => p.Name).ToList(),
                        2 => products.OrderBy(p => p.SellingPrice).ToList(),
                        3 => products.OrderByDescending(p => p.SellingPrice).ToList(),
                        4 => products.OrderByDescending(p => GetProductPopularity(p.Id)).ToList(),
                        5 => products.OrderByDescending(p => p.CreatedAt).ToList(),
                        _ => products
                    };
                }

                DisplayProducts(products);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error filtering products: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int GetProductPopularity(int productId)
        {
            try
            {
                return _context.SaleItems
                    .Where(si => si.ProductId == productId)
                    .Sum(si => si.Quantity);
            }
            catch
            {
                return 0;
            }
        }

        private void DisplayProducts(List<Product> products)
        {
            productsPanel.Controls.Clear();

            foreach (var product in products)
            {
                var productCard = CreateProductCard(product);
                productsPanel.Controls.Add(productCard);
            }
        }

        private Panel CreateProductCard(Product product)
        {
            var card = new Panel
            {
                Size = new Size(200, 240),
                Margin = new Padding(10),
                BackColor = Color.White,
                Cursor = Cursors.Hand
            };

            card.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, card.Width - 1, card.Height - 1), 10))
                using (var brush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillPath(brush, path);
                }
                
                // Shadow
                using (var shadowPath = GetRoundedRectPath(new Rectangle(2, 2, card.Width - 3, card.Height - 3), 10))
                using (var shadowBrush = new SolidBrush(Color.FromArgb(15, 0, 0, 0)))
                {
                    e.Graphics.FillPath(shadowBrush, shadowPath);
                }
            };

            // Product Image
            var imgBox = new PictureBox
            {
                Location = new Point(10, 10),
                Size = new Size(180, 120),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(248, 250, 252)
            };

            // Load product image
            var primaryImage = product.ProductImages?.FirstOrDefault(pi => pi.IsPrimary && pi.IsActive);
            if (primaryImage != null && !string.IsNullOrEmpty(primaryImage.ImagePath))
            {
                try
                {
                    string imagePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, primaryImage.ImagePath);
                    if (System.IO.File.Exists(imagePath))
                    {
                        imgBox.Image = Image.FromFile(imagePath);
                    }
                }
                catch { }
            }

            // Product Name
            var nameLabel = new Label
            {
                Text = product.Name,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(10, 140),
                Size = new Size(180, 40),
                TextAlign = ContentAlignment.TopCenter,
                BackColor = Color.Transparent
            };

            // Price
            var priceLabel = new Label
            {
                Text = $"${product.SellingPrice:N2}",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.FromArgb(34, 197, 94),
                Location = new Point(10, 185),
                Size = new Size(180, 25),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            // Stock
            var stockLabel = new Label
            {
                Text = $"Stock: {product.Quantity}",
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(10, 215),
                Size = new Size(180, 20),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            card.Click += (s, e) => AddToCart(product);
            imgBox.Click += (s, e) => AddToCart(product);
            nameLabel.Click += (s, e) => AddToCart(product);
            priceLabel.Click += (s, e) => AddToCart(product);

            card.MouseEnter += (s, e) => card.BackColor = Color.FromArgb(249, 250, 251);
            card.MouseLeave += (s, e) => card.BackColor = Color.White;

            card.Controls.AddRange(new Control[] { imgBox, nameLabel, priceLabel, stockLabel });
            return card;
        }

        // Cart Operations
        private void AddToCart(Product product)
        {
            var existingItem = cartItems.FirstOrDefault(i => i.ProductId == product.Id);
            
            if (existingItem != null)
            {
                if (existingItem.Quantity < product.Quantity)
                {
                    existingItem.Quantity++;
                }
                else
                {
                    MessageBox.Show("Insufficient stock!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else
            {
                cartItems.Add(new POSCartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Quantity = 1,
                    UnitPrice = product.SellingPrice
                });
            }

            UpdateCartDisplay();
        }

        private void UpdateCartDisplay()
        {
            cartGrid.DataSource = null;
            cartGrid.DataSource = cartItems.ToList();
            
            // Format currency columns
            if (cartGrid.Columns["Price"] != null)
            {
                cartGrid.Columns["Price"].DefaultCellStyle.Format = "C2";
            }
            if (cartGrid.Columns["Total"] != null)
            {
                cartGrid.Columns["Total"].DefaultCellStyle.Format = "C2";
            }

            UpdateCartTotals();
        }

        private void UpdateCartTotals()
        {
            decimal subtotal = cartItems.Sum(i => i.Total);
            decimal tax = subtotal * (taxRate / 100);
            decimal total = subtotal + tax;

            lblSubtotal.Text = $"${subtotal:N2}";
            lblTax.Text = $"${tax:N2}";
            lblTotal.Text = $"${total:N2}";

            CalculateChange();
        }

        private void CalculateChange()
        {
            if (decimal.TryParse(txtCashAmount.Text, out decimal cashAmount))
            {
                decimal total = decimal.Parse(lblTotal.Text.Replace("$", "").Replace(",", ""));
                decimal change = cashAmount - total;
                lblChange.Text = $"${Math.Max(0, change):N2}";
                lblChange.ForeColor = change >= 0 ? Color.FromArgb(34, 197, 94) : Color.FromArgb(239, 68, 68);
            }
        }

        private void CartGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == cartGrid.Columns["Remove"].Index)
            {
                var item = cartItems[e.RowIndex];
                cartItems.RemoveAt(e.RowIndex);
                UpdateCartDisplay();
            }
        }

        private void ClearCart()
        {
            if (cartItems.Count > 0)
            {
                var result = MessageBox.Show("Clear all items from cart?", "Confirm", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    cartItems.Clear();
                    UpdateCartDisplay();
                    txtCashAmount.Text = "0.00";
                }
            }
        }

        private void PaymentMethod_Changed(object sender, EventArgs e)
        {
            txtCashAmount.Enabled = rbCash.Checked;
            if (!rbCash.Checked)
            {
                txtCashAmount.Text = lblTotal.Text.Replace("$", "").Replace(",", "");
                CalculateChange();
            }
        }

        private async void BtnCompleteSale_Click(object sender, EventArgs e)
        {
            if (cartItems.Count == 0)
            {
                MessageBox.Show("Cart is empty!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate payment
            if (rbCash.Checked)
            {
                decimal cashAmount = decimal.Parse(txtCashAmount.Text);
                decimal total = decimal.Parse(lblTotal.Text.Replace("$", "").Replace(",", ""));
                
                if (cashAmount < total)
                {
                    MessageBox.Show("Insufficient cash amount!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            // Show payment popup for card/mobile
            if (rbCard.Checked)
            {
                ShowCardPaymentDialog();
                return;
            }
            else if (rbMobile.Checked)
            {
                ShowMobilePaymentDialog();
                return;
            }

            // Process sale
            await ProcessSale();
        }

        private void ShowCardPaymentDialog()
        {
            var dialog = new Form
            {
                Text = "Card Payment",
                Size = new Size(400, 300),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.White
            };

            var iconBox = new IconPictureBox
            {
                IconChar = IconChar.CreditCard,
                IconColor = Color.FromArgb(59, 130, 246),
                IconSize = 64,
                Location = new Point(168, 30),
                Size = new Size(64, 64),
                BackColor = Color.Transparent
            };

            var lblInstruction = new Label
            {
                Text = "Please insert or tap card",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(50, 110),
                Size = new Size(300, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var lblAmount = new Label
            {
                Text = $"Amount: {lblTotal.Text}",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(34, 197, 94),
                Location = new Point(50, 150),
                Size = new Size(300, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var btnConfirm = new Button
            {
                Text = "✓ Confirm Payment",
                Location = new Point(100, 200),
                Size = new Size(200, 40),
                BackColor = Color.FromArgb(34, 197, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnConfirm.FlatAppearance.BorderSize = 0;
            btnConfirm.Click += async (s, e) => 
            {
                dialog.DialogResult = DialogResult.OK;
                dialog.Close();
                await ProcessSale();
            };

            dialog.Controls.AddRange(new Control[] { iconBox, lblInstruction, lblAmount, btnConfirm });
            dialog.ShowDialog();
        }

        private void ShowMobilePaymentDialog()
        {
            var dialog = new Form
            {
                Text = "Mobile Payment - QR Code",
                Size = new Size(450, 550),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.White
            };

            var lblTitle = new Label
            {
                Text = "Scan QR Code to Pay",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(50, 20),
                Size = new Size(350, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var lblAmount = new Label
            {
                Text = lblTotal.Text,
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(34, 197, 94),
                Location = new Point(50, 60),
                Size = new Size(350, 40),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Generate QR Code
            var qrGenerator = new QRCodeGenerator();
            string paymentData = $"SYNCVERSE_PAY:{lblTotal.Text}:INV{DateTime.Now:yyyyMMddHHmmss}";
            var qrCodeData = qrGenerator.CreateQrCode(paymentData, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrCodeData);
            var qrBitmap = qrCode.GetGraphic(20);

            var qrPictureBox = new PictureBox
            {
                Image = qrBitmap,
                Location = new Point(75, 120),
                Size = new Size(300, 300),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White
            };

            var lblInstruction = new Label
            {
                Text = "Scan with your mobile payment app",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(50, 430),
                Size = new Size(350, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var btnConfirm = new Button
            {
                Text = "✓ Payment Received",
                Location = new Point(125, 460),
                Size = new Size(200, 40),
                BackColor = Color.FromArgb(34, 197, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnConfirm.FlatAppearance.BorderSize = 0;
            btnConfirm.Click += async (s, e) => 
            {
                dialog.DialogResult = DialogResult.OK;
                dialog.Close();
                await ProcessSale();
            };

            dialog.Controls.AddRange(new Control[] { 
                lblTitle, lblAmount, qrPictureBox, lblInstruction, btnConfirm 
            });
            dialog.ShowDialog();
        }

        private async System.Threading.Tasks.Task ProcessSale()
        {
            try
            {
                // Prompt for customer information
                var customerDialog = new CustomerCaptureDialog();
                var customerResult = customerDialog.ShowDialog();
                
                int? customerId = null;
                string customerName = null;

                if (customerResult == DialogResult.OK)
                {
                    customerId = customerDialog.SelectedCustomerId;
                    customerName = customerDialog.CustomerName;
                }

                // Calculate totals
                decimal subtotal = cartItems.Sum(i => i.Total);
                decimal taxAmount = subtotal * (taxRate / 100);
                decimal total = subtotal + taxAmount;

                // Create Sale
                var sale = new Sale
                {
                    InvoiceNumber = GenerateInvoiceNumber(),
                    CustomerId = customerId,
                    CashierId = _authService.CurrentUser.Id,
                    TotalAmount = total,
                    TaxAmount = taxAmount,
                    PaymentMethod = GetPaymentMethodEnum(),
                    SaleDate = DateTime.Now,
                    Status = SaleStatus.Completed
                };

                _context.Sales.Add(sale);
                await _context.SaveChangesAsync();

                // Create Sale Items
                foreach (var item in cartItems)
                {
                    var saleItem = new SaleItem
                    {
                        SaleId = sale.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        TotalPrice = item.Total
                    };
                    _context.SaleItems.Add(saleItem);

                    // Update product quantity
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        product.Quantity -= item.Quantity;
                        product.UpdatedAt = DateTime.Now;
                    }
                }

                // Create Invoice
                var invoice = new Invoice
                {
                    InvoiceNumber = GenerateInvoiceNumber(),
                    CustomerId = customerId,
                    CustomerName = customerName,
                    CreatedByUserId = _authService.CurrentUser.Id,
                    SubTotal = subtotal,
                    TaxAmount = taxAmount,
                    TotalAmount = total,
                    PaidAmount = total,
                    BalanceAmount = 0,
                    Status = InvoiceStatus.Paid,
                    InvoiceDate = DateTime.Now,
                    SaleId = sale.Id,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync();

                // Create Invoice Items
                foreach (var item in cartItems)
                {
                    var invoiceItem = new InvoiceItem
                    {
                        InvoiceId = invoice.Id,
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        TotalPrice = item.Total
                    };
                    _context.InvoiceItems.Add(invoiceItem);
                }

                await _context.SaveChangesAsync();

                // Print Invoice
                PrintInvoice(invoice, customerName);

                // Show success message
                MessageBox.Show($"Sale completed successfully!\n\nInvoice: {invoice.InvoiceNumber}\nTotal: ${total:N2}\nChange: {lblChange.Text}", 
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Clear cart
                cartItems.Clear();
                UpdateCartDisplay();
                txtCashAmount.Text = "0.00";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing sale: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetPaymentMethod()
        {
            if (rbCash.Checked) return "Cash";
            if (rbCard.Checked) return "Card";
            if (rbMobile.Checked) return "Mobile";
            return "Cash";
        }

        private PaymentMethod GetPaymentMethodEnum()
        {
            if (rbCash.Checked) return PaymentMethod.Cash;
            if (rbCard.Checked) return PaymentMethod.Card;
            if (rbMobile.Checked) return PaymentMethod.Mobile;
            return PaymentMethod.Cash;
        }

        private string GenerateInvoiceNumber()
        {
            return $"INV-{DateTime.Now:yyyyMMdd}-{DateTime.Now:HHmmss}";
        }

        private void PrintInvoice(Invoice invoice, string customerName)
        {
            try
            {
                PrintDocument printDoc = new PrintDocument();
                printDoc.PrintPage += (sender, e) => PrintInvoicePage(sender, e, invoice, customerName);
                
                // Show print preview
                PrintPreviewDialog previewDialog = new PrintPreviewDialog
                {
                    Document = printDoc,
                    Width = 800,
                    Height = 600
                };
                previewDialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error printing invoice: {ex.Message}", "Print Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintInvoicePage(object sender, PrintPageEventArgs e, Invoice invoice, string customerName)
        {
            Graphics g = e.Graphics;
            Font titleFont = new Font("Segoe UI", 20, FontStyle.Bold);
            Font headerFont = new Font("Segoe UI", 12, FontStyle.Bold);
            Font normalFont = new Font("Segoe UI", 10);
            Font smallFont = new Font("Segoe UI", 8);
            
            Brush blackBrush = Brushes.Black;
            Brush grayBrush = Brushes.Gray;
            
            int yPos = 50;
            int leftMargin = 50;
            int rightMargin = e.PageBounds.Width - 50;

            // Company Logo and Name
            g.DrawString("SYNCVERSE STUDIO", titleFont, new SolidBrush(Color.FromArgb(20, 184, 166)), leftMargin, yPos);
            yPos += 35;
            
            g.DrawString("Point of Sale System", normalFont, grayBrush, leftMargin, yPos);
            yPos += 20;
            g.DrawString("123 Business Street, City, Country", smallFont, grayBrush, leftMargin, yPos);
            yPos += 15;
            g.DrawString("Phone: +1 234 567 8900 | Email: sales@syncverse.com", smallFont, grayBrush, leftMargin, yPos);
            yPos += 40;

            // Invoice Title
            g.DrawString("INVOICE", new Font("Segoe UI", 18, FontStyle.Bold), blackBrush, leftMargin, yPos);
            yPos += 40;

            // Invoice Details
            g.DrawString($"Invoice Number:", headerFont, blackBrush, leftMargin, yPos);
            g.DrawString(invoice.InvoiceNumber, normalFont, blackBrush, leftMargin + 200, yPos);
            yPos += 25;

            g.DrawString($"Date:", headerFont, blackBrush, leftMargin, yPos);
            g.DrawString(invoice.InvoiceDate.ToString("MMMM dd, yyyy HH:mm"), normalFont, blackBrush, leftMargin + 200, yPos);
            yPos += 25;

            g.DrawString($"Cashier:", headerFont, blackBrush, leftMargin, yPos);
            g.DrawString(_authService.CurrentUser.Username, normalFont, blackBrush, leftMargin + 200, yPos);
            yPos += 25;

            if (!string.IsNullOrEmpty(customerName))
            {
                g.DrawString($"Customer:", headerFont, blackBrush, leftMargin, yPos);
                g.DrawString(customerName, normalFont, blackBrush, leftMargin + 200, yPos);
                yPos += 25;
            }

            yPos += 20;

            // Line separator
            g.DrawLine(new Pen(Color.Black, 2), leftMargin, yPos, rightMargin, yPos);
            yPos += 20;

            // Table Headers
            g.DrawString("Item", headerFont, blackBrush, leftMargin, yPos);
            g.DrawString("Qty", headerFont, blackBrush, leftMargin + 300, yPos);
            g.DrawString("Price", headerFont, blackBrush, leftMargin + 380, yPos);
            g.DrawString("Total", headerFont, blackBrush, leftMargin + 480, yPos);
            yPos += 30;

            g.DrawLine(Pens.Gray, leftMargin, yPos, rightMargin, yPos);
            yPos += 15;

            // Invoice Items
            var invoiceItems = _context.InvoiceItems
                .Where(ii => ii.InvoiceId == invoice.Id)
                .ToList();

            foreach (var item in invoiceItems)
            {
                g.DrawString(item.ProductName, normalFont, blackBrush, leftMargin, yPos);
                g.DrawString(item.Quantity.ToString(), normalFont, blackBrush, leftMargin + 300, yPos);
                g.DrawString($"${item.UnitPrice:N2}", normalFont, blackBrush, leftMargin + 380, yPos);
                g.DrawString($"${item.TotalPrice:N2}", normalFont, blackBrush, leftMargin + 480, yPos);
                yPos += 25;
            }

            yPos += 10;
            g.DrawLine(Pens.Gray, leftMargin, yPos, rightMargin, yPos);
            yPos += 20;

            // Totals
            g.DrawString("Subtotal:", headerFont, blackBrush, leftMargin + 350, yPos);
            g.DrawString($"${invoice.SubTotal:N2}", normalFont, blackBrush, leftMargin + 480, yPos);
            yPos += 25;

            g.DrawString($"Tax ({taxRate}%):", headerFont, blackBrush, leftMargin + 350, yPos);
            g.DrawString($"${invoice.TaxAmount:N2}", normalFont, blackBrush, leftMargin + 480, yPos);
            yPos += 30;

            g.DrawLine(new Pen(Color.Black, 2), leftMargin + 350, yPos, rightMargin, yPos);
            yPos += 15;

            g.DrawString("TOTAL:", new Font("Segoe UI", 14, FontStyle.Bold), blackBrush, leftMargin + 350, yPos);
            g.DrawString($"${invoice.TotalAmount:N2}", new Font("Segoe UI", 14, FontStyle.Bold), 
                new SolidBrush(Color.FromArgb(34, 197, 94)), leftMargin + 480, yPos);
            yPos += 40;

            // Payment Info
            if (rbCash.Checked)
            {
                decimal cashAmount = decimal.Parse(txtCashAmount.Text);
                decimal change = cashAmount - invoice.TotalAmount;
                
                g.DrawString("Cash Tendered:", normalFont, blackBrush, leftMargin + 350, yPos);
                g.DrawString($"${cashAmount:N2}", normalFont, blackBrush, leftMargin + 480, yPos);
                yPos += 25;

                g.DrawString("Change:", normalFont, blackBrush, leftMargin + 350, yPos);
                g.DrawString($"${change:N2}", normalFont, blackBrush, leftMargin + 480, yPos);
                yPos += 30;
            }
            else
            {
                g.DrawString($"Payment Method: {GetPaymentMethod()}", normalFont, blackBrush, leftMargin + 350, yPos);
                yPos += 30;
            }

            // Footer
            yPos += 40;
            g.DrawString("Thank you for your business!", new Font("Segoe UI", 12, FontStyle.Bold), 
                blackBrush, leftMargin + 150, yPos);
            yPos += 25;
            g.DrawString("Please keep this invoice for your records", smallFont, grayBrush, leftMargin + 170, yPos);
        }

        private async void BtnHold_Click(object sender, EventArgs e)
        {
            if (cartItems.Count == 0)
            {
                MessageBox.Show("Cart is empty!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                decimal subtotal = cartItems.Sum(i => i.Total);
                decimal tax = subtotal * (taxRate / 100);
                decimal total = subtotal + tax;

                var heldTransaction = new HeldTransaction
                {
                    TransactionCode = $"HOLD-{DateTime.Now:yyyyMMddHHmmss}",
                    HeldByUserId = _authService.CurrentUser.Id,
                    SubTotal = subtotal,
                    TaxAmount = tax,
                    TotalAmount = total,
                    CartItemsJson = System.Text.Json.JsonSerializer.Serialize(cartItems),
                    HeldAt = DateTime.Now
                };

                _context.HeldTransactions.Add(heldTransaction);
                await _context.SaveChangesAsync();

                MessageBox.Show("Transaction held successfully!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                cartItems.Clear();
                UpdateCartDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error holding transaction: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
