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
        private RadioButton rbCashUSD, rbCashKHR;
        private Label lblCashCurrency;
        private System.Media.SoundPlayer soundPlayer;
        
        // Tax settings
        private decimal taxRate = 10m; // Default 10%
        
        // Timer for live dashboard updates
        private System.Windows.Forms.Timer _liveUpdateTimer;
        
        public EnhancedPOSSystem(AuthenticationService authService)
        {
            _authService = authService;
            _context = new ApplicationDbContext();
            InitializeComponent();
            LoadCategories();
            LoadProducts();
            StartLiveUpdates();
        }
        
        private void StartLiveUpdates()
        {
            _liveUpdateTimer = new System.Windows.Forms.Timer();
            _liveUpdateTimer.Interval = 3000; // Update every 3 seconds
            _liveUpdateTimer.Tick += async (s, e) => await UpdateLiveDashboard();
            _liveUpdateTimer.Start();
        }
        
        private async System.Threading.Tasks.Task UpdateLiveDashboard()
        {
            // This will be called every 3 seconds to refresh live data
            // Can be used to update recent sales, statistics, etc.
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
                Value = 10m,
                // Role-based access: Only Admin and Inventory Clerk can edit
                Enabled = _authService.CurrentUser.Role == UserRole.Administrator || 
                         _authService.CurrentUser.Role == UserRole.InventoryClerk
            };
            numTaxRate.ValueChanged += (s, e) => { taxRate = numTaxRate.Value; UpdateCartTotals(); };
            
            // Add tooltip for restricted users
            if (!numTaxRate.Enabled)
            {
                var tooltip = new ToolTip();
                tooltip.SetToolTip(numTaxRate, "Tax rate editing restricted to Admin/Inventory Clerk roles");
            }

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

            // Cash currency selection
            lblCashCurrency = new Label
            {
                Text = "Currency:",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(420, 52),
                AutoSize = true,
                Visible = true
            };

            rbCashUSD = new RadioButton
            {
                Text = "USD ($)",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(420, 70),
                AutoSize = true,
                Checked = true,
                Visible = true
            };
            rbCashUSD.CheckedChanged += (s, e) => { if (rbCashUSD.Checked) CalculateChange(); };

            rbCashKHR = new RadioButton
            {
                Text = "Riel (៛)",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(420, 92),
                AutoSize = true,
                Visible = true
            };
            rbCashKHR.CheckedChanged += (s, e) => { if (rbCashKHR.Checked) CalculateChange(); };

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
                Location = new Point(20, 120),
                AutoSize = true
            };

            lblChange = new Label
            {
                Text = "$0.00",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(34, 197, 94),
                Location = new Point(140, 118),
                AutoSize = true
            };

            panel.Controls.AddRange(new Control[] { 
                lblPaymentMethod, rbCash, rbCard, rbMobile, 
                lblCashCurrency, rbCashUSD, rbCashKHR,
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
                using (var context = new ApplicationDbContext())
                {
                    var categories = await context.Categories
                        .AsNoTracking()
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
                using (var context = new ApplicationDbContext())
                {
                    var products = await context.Products
                        .AsNoTracking()
                        .Include(p => p.Category)
                        .Include(p => p.ProductImages)
                        .Where(p => p.IsActive && p.Quantity > 0)
                        .ToListAsync();

                    DisplayProducts(products);
                }
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
                using (var context = new ApplicationDbContext())
                {
                    var query = context.Products
                        .AsNoTracking()
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
                using (var context = new ApplicationDbContext())
                {
                    return context.SaleItems
                        .AsNoTracking()
                        .Where(si => si.ProductId == productId)
                        .Sum(si => (int?)si.Quantity) ?? 0;
                }
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

            // Glitch hover effect
            var glitchTimer = new System.Windows.Forms.Timer { Interval = 50 };
            var random = new Random();
            int glitchCount = 0;
            
            card.MouseEnter += (s, e) =>
            {
                glitchCount = 0;
                glitchTimer.Tick += (ts, te) =>
                {
                    if (glitchCount < 3)
                    {
                        // Glitch effect: random color shifts
                        int r = random.Next(240, 256);
                        int g = random.Next(240, 256);
                        int b = random.Next(240, 256);
                        card.BackColor = Color.FromArgb(r, g, b);
                        
                        // Slight position shift
                        int offsetX = random.Next(-2, 3);
                        int offsetY = random.Next(-2, 3);
                        card.Location = new Point(card.Location.X + offsetX, card.Location.Y + offsetY);
                        
                        glitchCount++;
                    }
                    else
                    {
                        glitchTimer.Stop();
                        card.BackColor = Color.FromArgb(219, 234, 254); // Light blue highlight
                        // Reset position
                        card.Location = new Point(
                            (card.Location.X / 220) * 220 + 10,
                            (card.Location.Y / 260) * 260 + 10
                        );
                    }
                };
                glitchTimer.Start();
            };
            
            card.MouseLeave += (s, e) =>
            {
                glitchTimer.Stop();
                card.BackColor = Color.White;
                glitchCount = 0;
            };

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

            // Auto-calculate cash amount
            if (rbCash.Checked && txtCashAmount != null)
            {
                // Auto-fill with exact amount, user can adjust if needed
                txtCashAmount.Text = total.ToString("F2");
            }

            CalculateChange();
        }

        private void CalculateChange()
        {
            if (decimal.TryParse(txtCashAmount.Text, out decimal cashAmount))
            {
                decimal total = decimal.Parse(lblTotal.Text.Replace("$", "").Replace(",", ""));
                
                // Determine payment currency
                var paidCurrency = rbCashUSD.Checked ? CurrencyService.Currency.USD : CurrencyService.Currency.KHR;
                
                try
                {
                    // Calculate change in the same currency as payment
                    var (changeAmount, changeCurrency) = CurrencyService.CalculateChange(
                        total, 
                        cashAmount, 
                        paidCurrency, 
                        paidCurrency // Return change in same currency
                    );
                    
                    // Format change display
                    lblChange.Text = CurrencyService.Format(changeAmount, changeCurrency);
                    lblChange.ForeColor = changeAmount >= 0 ? Color.FromArgb(34, 197, 94) : Color.FromArgb(239, 68, 68);
                }
                catch (InvalidOperationException)
                {
                    lblChange.Text = "Insufficient!";
                    lblChange.ForeColor = Color.FromArgb(239, 68, 68);
                }
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
            bool isCash = rbCash.Checked;
            txtCashAmount.Enabled = isCash;
            lblCashCurrency.Visible = isCash;
            rbCashUSD.Visible = isCash;
            rbCashKHR.Visible = isCash;
            
            if (!isCash)
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
                // Play QR/Mobile payment sound
                PlaySound(@"assets\audio\cash-register-kaching-376867.mp3");
                
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

                // Play invoice completion sound
                PlaySound(@"assets\audio\cash-register-kaching-sound-effect-125042.mp3");

                // Print Invoice
                PrintInvoice(invoice, customerName);

                // Prepare change message with currency
                string changeMessage = lblChange.Text;
                if (rbCash.Checked)
                {
                    var paidCurrency = rbCashUSD.Checked ? "USD" : "KHR";
                    changeMessage = $"{lblChange.Text} ({paidCurrency})";
                }

                // Show success message
                MessageBox.Show($"Sale completed successfully!\n\nInvoice: {invoice.InvoiceNumber}\nTotal: ${total:N2}\nChange: {changeMessage}", 
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
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            
            Font titleFont = new Font("Segoe UI", 24, FontStyle.Bold);
            Font companyFont = new Font("Segoe UI", 16, FontStyle.Bold);
            Font headerFont = new Font("Segoe UI", 11, FontStyle.Bold);
            Font normalFont = new Font("Segoe UI", 10);
            Font smallFont = new Font("Segoe UI", 8);
            Font invoiceFont = new Font("Segoe UI", 20, FontStyle.Bold);
            
            Brush blackBrush = Brushes.Black;
            Brush grayBrush = new SolidBrush(Color.FromArgb(100, 100, 100));
            Brush tealBrush = new SolidBrush(Color.FromArgb(20, 184, 166));
            
            int yPos = 40;
            int leftMargin = 60;
            int rightMargin = e.PageBounds.Width - 60;
            int centerX = e.PageBounds.Width / 2;

            // Header Background
            using (var headerBrush = new SolidBrush(Color.FromArgb(240, 253, 250)))
            {
                g.FillRectangle(headerBrush, 0, 0, e.PageBounds.Width, 180);
            }

            // Company Logo (Text-based)
            g.DrawString("SYNCVERSE", titleFont, tealBrush, leftMargin, yPos);
            yPos += 35;
            g.DrawString("STUDIO", new Font("Segoe UI", 18, FontStyle.Bold), tealBrush, leftMargin + 10, yPos);
            yPos += 40;
            
            // Company Details
            g.DrawString("Retail & Point of Sale Solutions", normalFont, grayBrush, leftMargin, yPos);
            yPos += 18;
            g.DrawString("📍 123 Business Street, City, State 12345", smallFont, grayBrush, leftMargin, yPos);
            yPos += 15;
            g.DrawString("📞 +1 (234) 567-8900  |  ✉ sales@syncverse.com", smallFont, grayBrush, leftMargin, yPos);
            yPos += 15;
            g.DrawString("🌐 www.syncverse.com  |  Tax ID: 12-3456789", smallFont, grayBrush, leftMargin, yPos);

            // Invoice Title (Right side)
            g.DrawString("INVOICE", invoiceFont, new SolidBrush(Color.FromArgb(239, 68, 68)), rightMargin - 180, 50);
            
            // Invoice Box (Right side)
            Rectangle invoiceBox = new Rectangle(rightMargin - 220, 90, 220, 80);
            using (var boxBrush = new SolidBrush(Color.White))
            {
                g.FillRectangle(boxBrush, invoiceBox);
                g.DrawRectangle(new Pen(Color.FromArgb(200, 200, 200), 1), invoiceBox);
            }
            
            g.DrawString("Invoice #:", new Font("Segoe UI", 9, FontStyle.Bold), blackBrush, rightMargin - 210, 100);
            g.DrawString(invoice.InvoiceNumber, new Font("Segoe UI", 9), blackBrush, rightMargin - 210, 118);
            g.DrawString("Date:", new Font("Segoe UI", 9, FontStyle.Bold), blackBrush, rightMargin - 210, 140);
            g.DrawString(invoice.InvoiceDate.ToString("MMM dd, yyyy HH:mm"), new Font("Segoe UI", 9), blackBrush, rightMargin - 210, 158);

            yPos = 200;

            // Bill To Section
            g.DrawString("BILL TO:", headerFont, blackBrush, leftMargin, yPos);
            yPos += 25;
            
            if (!string.IsNullOrEmpty(customerName))
            {
                g.DrawString(customerName, normalFont, blackBrush, leftMargin, yPos);
                yPos += 20;
            }
            else
            {
                g.DrawString("Walk-in Customer", normalFont, grayBrush, leftMargin, yPos);
                yPos += 20;
            }

            // Seller Info (Right side)
            g.DrawString("SOLD BY:", headerFont, blackBrush, rightMargin - 220, 200);
            g.DrawString($"Cashier: {_authService.CurrentUser.Username}", normalFont, blackBrush, rightMargin - 220, 225);
            g.DrawString($"Terminal: POS-01", normalFont, blackBrush, rightMargin - 220, 245);

            yPos += 20;

            // Items Table Header
            using (var tableBrush = new SolidBrush(Color.FromArgb(241, 245, 249)))
            {
                g.FillRectangle(tableBrush, leftMargin, yPos, rightMargin - leftMargin, 35);
            }
            g.DrawRectangle(new Pen(Color.FromArgb(200, 200, 200)), leftMargin, yPos, rightMargin - leftMargin, 35);
            
            yPos += 10;
            g.DrawString("ITEM DESCRIPTION", headerFont, blackBrush, leftMargin + 10, yPos);
            g.DrawString("QTY", headerFont, blackBrush, leftMargin + 350, yPos);
            g.DrawString("UNIT PRICE", headerFont, blackBrush, leftMargin + 420, yPos);
            g.DrawString("AMOUNT", headerFont, blackBrush, leftMargin + 550, yPos);
            yPos += 30;

            // Invoice Items
            var invoiceItems = _context.InvoiceItems
                .Where(ii => ii.InvoiceId == invoice.Id)
                .ToList();

            int itemNumber = 1;
            foreach (var item in invoiceItems)
            {
                // Alternating row colors
                if (itemNumber % 2 == 0)
                {
                    using (var rowBrush = new SolidBrush(Color.FromArgb(250, 250, 250)))
                    {
                        g.FillRectangle(rowBrush, leftMargin, yPos - 5, rightMargin - leftMargin, 25);
                    }
                }
                
                g.DrawString($"{itemNumber}. {item.ProductName}", normalFont, blackBrush, leftMargin + 10, yPos);
                g.DrawString(item.Quantity.ToString(), normalFont, blackBrush, leftMargin + 360, yPos);
                g.DrawString($"${item.UnitPrice:N2}", normalFont, blackBrush, leftMargin + 430, yPos);
                g.DrawString($"${item.TotalPrice:N2}", new Font("Segoe UI", 10, FontStyle.Bold), blackBrush, leftMargin + 560, yPos);
                yPos += 25;
                itemNumber++;
            }

            // Bottom border of items table
            g.DrawRectangle(new Pen(Color.FromArgb(200, 200, 200)), leftMargin, 295, rightMargin - leftMargin, yPos - 295);

            yPos += 15;

            // Totals Section with background
            Rectangle totalsBox = new Rectangle(leftMargin + 340, yPos, rightMargin - leftMargin - 340, 120);
            using (var totalsBrush = new SolidBrush(Color.FromArgb(249, 250, 251)))
            {
                g.FillRectangle(totalsBrush, totalsBox);
                g.DrawRectangle(new Pen(Color.FromArgb(200, 200, 200)), totalsBox);
            }

            yPos += 15;

            // Subtotal
            g.DrawString("Subtotal:", headerFont, blackBrush, leftMargin + 360, yPos);
            g.DrawString($"${invoice.SubTotal:N2}", normalFont, blackBrush, leftMargin + 560, yPos);
            yPos += 25;

            // Tax
            g.DrawString($"Tax ({taxRate}%):", headerFont, blackBrush, leftMargin + 360, yPos);
            g.DrawString($"${invoice.TaxAmount:N2}", normalFont, blackBrush, leftMargin + 560, yPos);
            yPos += 30;

            // Total line
            g.DrawLine(new Pen(Color.Black, 2), leftMargin + 360, yPos, rightMargin - 20, yPos);
            yPos += 15;

            // Grand Total
            g.DrawString("TOTAL:", new Font("Segoe UI", 14, FontStyle.Bold), blackBrush, leftMargin + 360, yPos);
            g.DrawString($"${invoice.TotalAmount:N2}", new Font("Segoe UI", 16, FontStyle.Bold), 
                new SolidBrush(Color.FromArgb(34, 197, 94)), leftMargin + 540, yPos - 2);
            
            yPos += 50;

            // Payment Information Box
            Rectangle paymentBox = new Rectangle(leftMargin, yPos, 300, 80);
            using (var paymentBrush = new SolidBrush(Color.FromArgb(240, 253, 250)))
            {
                g.FillRectangle(paymentBrush, paymentBox);
                g.DrawRectangle(new Pen(Color.FromArgb(20, 184, 166), 2), paymentBox);
            }

            yPos += 15;
            g.DrawString("PAYMENT DETAILS", new Font("Segoe UI", 10, FontStyle.Bold), tealBrush, leftMargin + 10, yPos);
            yPos += 25;

            if (rbCash.Checked)
            {
                decimal cashAmount = decimal.Parse(txtCashAmount.Text);
                decimal change = cashAmount - invoice.TotalAmount;
                
                g.DrawString($"Method: Cash", normalFont, blackBrush, leftMargin + 10, yPos);
                yPos += 20;
                g.DrawString($"Tendered: ${cashAmount:N2}", normalFont, blackBrush, leftMargin + 10, yPos);
                g.DrawString($"Change: ${change:N2}", new Font("Segoe UI", 10, FontStyle.Bold), 
                    new SolidBrush(Color.FromArgb(34, 197, 94)), leftMargin + 160, yPos);
            }
            else
            {
                g.DrawString($"Method: {GetPaymentMethod()}", normalFont, blackBrush, leftMargin + 10, yPos);
                yPos += 20;
                g.DrawString("Status: Paid", new Font("Segoe UI", 10, FontStyle.Bold), 
                    new SolidBrush(Color.FromArgb(34, 197, 94)), leftMargin + 10, yPos);
            }

            yPos += 50;

            // Footer Section
            g.DrawLine(new Pen(Color.FromArgb(200, 200, 200), 1), leftMargin, yPos, rightMargin, yPos);
            yPos += 20;
            
            g.DrawString("Thank you for your business!", new Font("Segoe UI", 13, FontStyle.Bold), 
                tealBrush, centerX - 120, yPos);
            yPos += 25;
            g.DrawString("For questions about this invoice, please contact us at sales@syncverse.com", 
                smallFont, grayBrush, centerX - 180, yPos);
            yPos += 15;
            g.DrawString("This is a computer-generated invoice and is valid without signature", 
                new Font("Segoe UI", 7, FontStyle.Italic), grayBrush, centerX - 150, yPos);
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

        private void PlaySound(string soundFile)
        {
            try
            {
                string soundPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, soundFile);
                if (System.IO.File.Exists(soundPath))
                {
                    soundPlayer = new System.Media.SoundPlayer(soundPath);
                    soundPlayer.Play();
                }
            }
            catch (Exception ex)
            {
                // Silently fail if sound cannot be played
                System.Diagnostics.Debug.WriteLine($"Sound error: {ex.Message}");
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
