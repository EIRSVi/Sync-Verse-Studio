using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using SyncVerseStudio.Data;
using SyncVerseStudio.Models;
using SyncVerseStudio.Services;
using SyncVerseStudio.Helpers;
using FontAwesome.Sharp;
using System.Media;

namespace SyncVerseStudio.Views
{
    public partial class ModernCashierView : Form
    {
        private readonly AuthenticationService _authService;
        private readonly ApplicationDbContext _context;
        private Sale _currentSale;
        private decimal _taxRate = 0.10m; // 10% default tax
        private SoundPlayer _cashRegisterSound;

        // UI Components
        private Panel _leftPanel, _rightPanel, _headerPanel, _footerPanel;
        private FlowLayoutPanel _productsPanel;
        private DataGridView _cartGrid;
        private TextBox _searchBox, _customerNameBox, _cashAmountBox;
        private ComboBox _categoryFilter;
        private Label _subtotalLabel, _taxLabel, _totalLabel, _changeLabel, _statusLabel;
        private Button _completeButton, _clearButton, _holdButton, _scanButton, _customerSearchButton;
        private RadioButton _cashRadio, _cardRadio, _mobileRadio;
        private Panel _paymentPanel;
        private System.Windows.Forms.Timer _statusTimer;

        public ModernCashierView(AuthenticationService authService)
        {
            _authService = authService;
            _context = new ApplicationDbContext();
            _currentSale = new Sale();
            
            InitializeComponent();
            LoadTaxSettings();
            LoadProducts();
            InitializeSounds();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Name = "ModernCashierView";
            this.Text = "SyncVerse POS - Cashier Terminal";
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(240, 242, 247);

            CreateLayout();
            SetupEventHandlers();

            this.ResumeLayout(false);
        }

        private void CreateLayout()
        {
            // Header Panel
            CreateHeaderPanel();
            
            // Main Content
            CreateMainPanels();
            
            // Footer Panel
            CreateFooterPanel();
        }

        private void CreateHeaderPanel()
        {
            _headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(59, 130, 246)
            };

            var logoPanel = new Panel
            {
                Location = new Point(20, 15),
                Size = new Size(200, 50),
                BackColor = Color.Transparent
            };

            var logoIcon = new IconPictureBox
            {
                IconChar = IconChar.CashRegister,
                IconColor = Color.White,
                IconSize = 32,
                Location = new Point(0, 9),
                Size = new Size(32, 32),
                BackColor = Color.Transparent
            };
            logoPanel.Controls.Add(logoIcon);

            var logoLabel = new Label
            {
                Text = "SyncVerse POS",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(40, 10),
                Size = new Size(150, 30),
                BackColor = Color.Transparent
            };
            logoPanel.Controls.Add(logoLabel);

            _headerPanel.Controls.Add(logoPanel);

            // User info
            var currentUser = _authService.CurrentUser;
            var userLabel = new Label
            {
                Text = $"Cashier: {currentUser?.FirstName} {currentUser?.LastName}",
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.White,
                Location = new Point(this.Width - 300, 20),
                Size = new Size(250, 25),
                TextAlign = ContentAlignment.MiddleRight,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            _headerPanel.Controls.Add(userLabel);

            var timeLabel = new Label
            {
                Text = DateTime.Now.ToString("MMM dd, yyyy - HH:mm"),
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(200, 220, 255),
                Location = new Point(this.Width - 300, 45),
                Size = new Size(250, 20),
                TextAlign = ContentAlignment.MiddleRight,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            _headerPanel.Controls.Add(timeLabel);

            // Update time every minute
            var timer = new System.Windows.Forms.Timer { Interval = 60000 };
            timer.Tick += (s, e) => timeLabel.Text = DateTime.Now.ToString("MMM dd, yyyy - HH:mm");
            timer.Start();

            this.Controls.Add(_headerPanel);
        }

        private void CreateMainPanels()
        {
            var mainContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(20, 20, 20, 100)
            };

            // Left Panel - Products
            CreateLeftPanel();
            mainContainer.Controls.Add(_leftPanel);

            // Right Panel - Cart & Payment
            CreateRightPanel();
            mainContainer.Controls.Add(_rightPanel);

            this.Controls.Add(mainContainer);
        }

        private void CreateLeftPanel()
        {
            _leftPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(800, 600),
                BackColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            _leftPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, _leftPanel.Width - 1, _leftPanel.Height - 1), 12))
                using (var brush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillPath(brush, path);
                }
            };

            // Search and Filter Section
            CreateSearchSection();

            // Products Grid
            CreateProductsSection();
        }

        private void CreateSearchSection()
        {
            var searchPanel = new Panel
            {
                Location = new Point(20, 20),
                Size = new Size(760, 60),
                BackColor = Color.Transparent
            };

            // Search box
            _searchBox = new TextBox
            {
                Location = new Point(0, 15),
                Size = new Size(300, 30),
                Font = new Font("Segoe UI", 11F),
                PlaceholderText = "Search products..."
            };
            searchPanel.Controls.Add(_searchBox);

            // Category filter
            _categoryFilter = new ComboBox
            {
                Location = new Point(320, 15),
                Size = new Size(150, 30),
                Font = new Font("Segoe UI", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            searchPanel.Controls.Add(_categoryFilter);

            // Scan button
            _scanButton = new Button
            {
                Text = "📷 Scan QR",
                Location = new Point(490, 10),
                Size = new Size(120, 40),
                BackColor = Color.FromArgb(34, 197, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            _scanButton.FlatAppearance.BorderSize = 0;
            _scanButton.Click += ScanButton_Click;
            searchPanel.Controls.Add(_scanButton);

            // Clear search button
            var clearSearchBtn = new Button
            {
                Text = "Clear",
                Location = new Point(620, 15),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(156, 163, 175),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F),
                Cursor = Cursors.Hand
            };
            clearSearchBtn.FlatAppearance.BorderSize = 0;
            clearSearchBtn.Click += (s, e) => { _searchBox.Clear(); LoadProducts(); };
            searchPanel.Controls.Add(clearSearchBtn);

            _leftPanel.Controls.Add(searchPanel);
        }

        private void CreateProductsSection()
        {
            _productsPanel = new FlowLayoutPanel
            {
                Location = new Point(20, 90),
                Size = new Size(760, 490),
                BackColor = Color.Transparent,
                AutoScroll = true,
                WrapContents = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            _leftPanel.Controls.Add(_productsPanel);
        }

        private void CreateRightPanel()
        {
            _rightPanel = new Panel
            {
                Location = new Point(820, 0),
                Size = new Size(450, 600),
                BackColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right
            };

            _rightPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, _rightPanel.Width - 1, _rightPanel.Height - 1), 12))
                using (var brush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillPath(brush, path);
                }
            };

            // Cart Section
            CreateCartSection();

            // Customer Section
            CreateCustomerSection();

            // Payment Section
            CreatePaymentSection();

            // Action Buttons
            CreateActionButtons();
        }
        private void CreateCartSection()
        {
            var cartHeader = new Label
            {
                Text = "Shopping Cart",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(20, 20),
                Size = new Size(200, 30),
                BackColor = Color.Transparent
            };
            _rightPanel.Controls.Add(cartHeader);

            _cartGrid = new DataGridView
            {
                Location = new Point(20, 60),
                Size = new Size(410, 200),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 35,
                EnableHeadersVisualStyles = false,
                GridColor = Color.FromArgb(230, 230, 230),
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                ReadOnly = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            // Style the grid
            _cartGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            _cartGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(60, 60, 60);
            _cartGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _cartGrid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(248, 250, 252);

            _cartGrid.DefaultCellStyle.BackColor = Color.White;
            _cartGrid.DefaultCellStyle.ForeColor = Color.FromArgb(60, 60, 60);
            _cartGrid.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            _cartGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(239, 246, 255);
            _cartGrid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 30, 30);

            _cartGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
            _cartGrid.RowTemplate.Height = 35;

            // Add columns
            _cartGrid.Columns.Add("Product", "Product");
            _cartGrid.Columns.Add("Qty", "Qty");
            _cartGrid.Columns.Add("Price", "Price");
            _cartGrid.Columns.Add("Total", "Total");
            _cartGrid.Columns.Add("Remove", "");

            _cartGrid.Columns["Qty"].Width = 60;
            _cartGrid.Columns["Price"].Width = 80;
            _cartGrid.Columns["Total"].Width = 80;
            _cartGrid.Columns["Remove"].Width = 40;

            // Make quantity column editable
            _cartGrid.Columns["Qty"].ReadOnly = false;

            _rightPanel.Controls.Add(_cartGrid);
        }

        private void CreateCustomerSection()
        {
            var customerLabel = new Label
            {
                Text = "Customer Information",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(20, 280),
                Size = new Size(200, 25),
                BackColor = Color.Transparent
            };
            _rightPanel.Controls.Add(customerLabel);

            _customerNameBox = new TextBox
            {
                Location = new Point(20, 310),
                Size = new Size(410, 30),
                Font = new Font("Segoe UI", 10F),
                PlaceholderText = "Customer name (optional)"
            };
            _rightPanel.Controls.Add(_customerNameBox);
        }

        private void CreatePaymentSection()
        {
            _paymentPanel = new Panel
            {
                Location = new Point(20, 350),
                Size = new Size(410, 150),
                BackColor = Color.FromArgb(248, 250, 252)
            };

            _paymentPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, _paymentPanel.Width - 1, _paymentPanel.Height - 1), 8))
                using (var brush = new SolidBrush(Color.FromArgb(248, 250, 252)))
                {
                    e.Graphics.FillPath(brush, path);
                }
            };

            // Payment method selection
            var paymentLabel = new Label
            {
                Text = "Payment Method",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(15, 10),
                Size = new Size(150, 25),
                BackColor = Color.Transparent
            };
            _paymentPanel.Controls.Add(paymentLabel);

            _cashRadio = new RadioButton
            {
                Text = "💵 Cash",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(15, 40),
                Size = new Size(80, 25),
                Checked = true,
                BackColor = Color.Transparent
            };
            _paymentPanel.Controls.Add(_cashRadio);

            _cardRadio = new RadioButton
            {
                Text = "💳 Card",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(110, 40),
                Size = new Size(80, 25),
                BackColor = Color.Transparent
            };
            _paymentPanel.Controls.Add(_cardRadio);

            _mobileRadio = new RadioButton
            {
                Text = "📱 KHQR",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(205, 40),
                Size = new Size(90, 25),
                BackColor = Color.Transparent
            };
            _paymentPanel.Controls.Add(_mobileRadio);

            // Cash amount input (only visible for cash payments)
            var cashLabel = new Label
            {
                Text = "Cash Amount:",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(15, 75),
                Size = new Size(100, 25),
                BackColor = Color.Transparent
            };
            _paymentPanel.Controls.Add(cashLabel);

            _cashAmountBox = new TextBox
            {
                Location = new Point(120, 75),
                Size = new Size(100, 25),
                Font = new Font("Segoe UI", 10F),
                Text = "0.00"
            };
            _paymentPanel.Controls.Add(_cashAmountBox);

            // Totals section
            CreateTotalsSection();

            _rightPanel.Controls.Add(_paymentPanel);
        }

        private void CreateTotalsSection()
        {
            int yPos = 105;

            _subtotalLabel = new Label
            {
                Text = "Subtotal: $0.00",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(250, yPos),
                Size = new Size(150, 20),
                TextAlign = ContentAlignment.MiddleRight,
                BackColor = Color.Transparent
            };
            _paymentPanel.Controls.Add(_subtotalLabel);

            _taxLabel = new Label
            {
                Text = "Tax (10%): $0.00",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(250, yPos + 20),
                Size = new Size(150, 20),
                TextAlign = ContentAlignment.MiddleRight,
                BackColor = Color.Transparent
            };
            _paymentPanel.Controls.Add(_taxLabel);

            _totalLabel = new Label
            {
                Text = "Total: $0.00",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(59, 130, 246),
                Location = new Point(250, yPos + 40),
                Size = new Size(150, 25),
                TextAlign = ContentAlignment.MiddleRight,
                BackColor = Color.Transparent
            };
            _paymentPanel.Controls.Add(_totalLabel);

            _changeLabel = new Label
            {
                Text = "Change: $0.00",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(34, 197, 94),
                Location = new Point(15, 105),
                Size = new Size(150, 20),
                BackColor = Color.Transparent,
                Visible = false
            };
            _paymentPanel.Controls.Add(_changeLabel);
        }

        private void CreateActionButtons()
        {
            var buttonPanel = new Panel
            {
                Location = new Point(20, 520),
                Size = new Size(410, 60),
                BackColor = Color.Transparent
            };

            _completeButton = new Button
            {
                Text = "💰 COMPLETE SALE",
                Location = new Point(0, 0),
                Size = new Size(200, 50),
                BackColor = Color.FromArgb(34, 197, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            _completeButton.FlatAppearance.BorderSize = 0;
            _completeButton.Click += CompleteButton_Click;
            buttonPanel.Controls.Add(_completeButton);

            _clearButton = new Button
            {
                Text = "🗑️ CLEAR",
                Location = new Point(210, 0),
                Size = new Size(95, 50),
                BackColor = Color.FromArgb(239, 68, 68),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            _clearButton.FlatAppearance.BorderSize = 0;
            _clearButton.Click += ClearButton_Click;
            buttonPanel.Controls.Add(_clearButton);

            _holdButton = new Button
            {
                Text = "⏸️ HOLD",
                Location = new Point(315, 0),
                Size = new Size(95, 50),
                BackColor = Color.FromArgb(249, 115, 22),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            _holdButton.FlatAppearance.BorderSize = 0;
            _holdButton.Click += HoldButton_Click;
            buttonPanel.Controls.Add(_holdButton);

            _rightPanel.Controls.Add(buttonPanel);
        }

        private void CreateFooterPanel()
        {
            _footerPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 80,
                BackColor = Color.FromArgb(30, 30, 30)
            };

            var footerLabel = new Label
            {
                Text = "SyncVerse Studio - Professional POS System",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.White,
                Location = new Point(20, 30),
                Size = new Size(400, 20),
                BackColor = Color.Transparent
            };
            _footerPanel.Controls.Add(footerLabel);

            // Tax settings button (Admin only)
            var currentUser = _authService.CurrentUser;
            if (currentUser?.Role == UserRole.Administrator)
            {
                var taxSettingsBtn = new Button
                {
                    Text = "⚙️ Tax Settings",
                    Location = new Point(this.Width - 150, 25),
                    Size = new Size(120, 30),
                    BackColor = Color.FromArgb(59, 130, 246),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9F),
                    Cursor = Cursors.Hand,
                    Anchor = AnchorStyles.Bottom | AnchorStyles.Right
                };
                taxSettingsBtn.FlatAppearance.BorderSize = 0;
                taxSettingsBtn.Click += TaxSettingsButton_Click;
                _footerPanel.Controls.Add(taxSettingsBtn);
            }

            this.Controls.Add(_footerPanel);
        }
        private void SetupEventHandlers()
        {
            _searchBox.TextChanged += SearchBox_TextChanged;
            _categoryFilter.SelectedIndexChanged += CategoryFilter_SelectedIndexChanged;
            _cashRadio.CheckedChanged += PaymentMethod_CheckedChanged;
            _cardRadio.CheckedChanged += PaymentMethod_CheckedChanged;
            _mobileRadio.CheckedChanged += PaymentMethod_CheckedChanged;
            _cashAmountBox.TextChanged += CashAmount_TextChanged;
            _cartGrid.CellValueChanged += CartGrid_CellValueChanged;
            _cartGrid.CellClick += CartGrid_CellClick;
        }

        private void InitializeSounds()
        {
            try
            {
                // You can add a cash register sound file to resources
                // _cashRegisterSound = new SoundPlayer(Properties.Resources.CashRegisterSound);
            }
            catch
            {
                // Fallback to system sound
                _cashRegisterSound = null;
            }
        }

        private async void LoadTaxSettings()
        {
            try
            {
                // Load tax settings from database or config
                // For now, using default 10%
                _taxRate = 0.10m;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tax settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void LoadProducts()
        {
            try
            {
                _productsPanel.Controls.Clear();

                var query = _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.ProductImages)
                    .Where(p => p.IsActive);

                // Apply search filter
                if (!string.IsNullOrEmpty(_searchBox.Text))
                {
                    var searchTerm = _searchBox.Text.ToLower();
                    query = query.Where(p => p.Name.ToLower().Contains(searchTerm) ||
                                           p.Barcode.ToLower().Contains(searchTerm) ||
                                           p.SKU.ToLower().Contains(searchTerm));
                }

                // Apply category filter
                if (_categoryFilter.SelectedItem != null && _categoryFilter.SelectedIndex > 0)
                {
                    var selectedItem = _categoryFilter.SelectedItem;
                    var categoryIdProperty = selectedItem.GetType().GetProperty("Id");
                    if (categoryIdProperty != null)
                    {
                        var categoryId = (int)categoryIdProperty.GetValue(selectedItem);
                        query = query.Where(p => p.CategoryId == categoryId);
                    }
                }

                var products = await query.OrderBy(p => p.Name).ToListAsync();

                foreach (var product in products)
                {
                    CreateProductCard(product);
                }

                // Load categories for filter
                if (_categoryFilter.Items.Count == 0)
                {
                    await LoadCategories();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async System.Threading.Tasks.Task LoadCategories()
        {
            try
            {
                var categories = await _context.Categories
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.Name)
                    .ToListAsync();

                _categoryFilter.Items.Clear();
                _categoryFilter.Items.Add(new { Id = 0, Name = "All Categories" });
                
                foreach (var category in categories)
                {
                    _categoryFilter.Items.Add(new { Id = category.Id, Name = category.Name });
                }

                _categoryFilter.DisplayMember = "Name";
                _categoryFilter.ValueMember = "Id";
                _categoryFilter.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateProductCard(Product product)
        {
            var card = new Panel
            {
                Size = new Size(180, 220),
                Margin = new Padding(5),
                BackColor = Color.White,
                Cursor = Cursors.Hand,
                Tag = product
            };

            card.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, card.Width - 1, card.Height - 1), 8))
                using (var brush = new SolidBrush(Color.White))
                using (var pen = new Pen(Color.FromArgb(230, 230, 230), 1))
                {
                    e.Graphics.FillPath(brush, path);
                    e.Graphics.DrawPath(pen, path);
                }
            };

            // Product image
            var imageBox = new PictureBox
            {
                Location = new Point(10, 10),
                Size = new Size(160, 120),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(248, 250, 252)
            };

            try
            {
                var primaryImage = ProductImageHelper.GetPrimaryImage(product);
                if (primaryImage != null)
                {
                    imageBox.Image = ProductImageHelper.ResizeImage(primaryImage, 160, 120);
                }
                else
                {
                    // Use default brand image
                    var defaultImage = ProductImageHelper.GetDefaultBrandImage();
                    if (defaultImage != null)
                    {
                        imageBox.Image = ProductImageHelper.ResizeImage(defaultImage, 160, 120);
                    }
                    else
                    {
                        // Fallback icon
                        var defaultIcon = new IconPictureBox
                        {
                            IconChar = IconChar.Box,
                            IconColor = Color.FromArgb(156, 163, 175),
                            IconSize = 48,
                            Location = new Point(56, 36),
                            Size = new Size(48, 48),
                            BackColor = Color.Transparent,
                            Parent = imageBox
                        };
                    }
                }
            }
            catch
            {
                // Use default brand image on error
                var defaultImage = ProductImageHelper.GetDefaultBrandImage();
                if (defaultImage != null)
                {
                    imageBox.Image = ProductImageHelper.ResizeImage(defaultImage, 160, 120);
                }
                else
                {
                    // Default icon on error
                    var defaultIcon = new IconPictureBox
                    {
                        IconChar = IconChar.Box,
                        IconColor = Color.FromArgb(156, 163, 175),
                        IconSize = 48,
                        Location = new Point(56, 36),
                        Size = new Size(48, 48),
                        BackColor = Color.Transparent,
                        Parent = imageBox
                    };
                }
            }

            card.Controls.Add(imageBox);

            // Product name
            var nameLabel = new Label
            {
                Text = product.Name,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(10, 135),
                Size = new Size(160, 40),
                TextAlign = ContentAlignment.TopCenter,
                BackColor = Color.Transparent
            };
            card.Controls.Add(nameLabel);

            // Price
            var priceLabel = new Label
            {
                Text = $"${product.SellingPrice:N2}",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(34, 197, 94),
                Location = new Point(10, 175),
                Size = new Size(160, 25),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            card.Controls.Add(priceLabel);

            // Stock indicator
            var stockColor = product.Quantity > product.MinQuantity ? Color.FromArgb(34, 197, 94) :
                            product.Quantity > 0 ? Color.FromArgb(249, 115, 22) : Color.FromArgb(239, 68, 68);

            var stockLabel = new Label
            {
                Text = $"Stock: {product.Quantity}",
                Font = new Font("Segoe UI", 8F),
                ForeColor = stockColor,
                Location = new Point(10, 200),
                Size = new Size(160, 15),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            card.Controls.Add(stockLabel);

            // Click events
            card.Click += (s, e) => AddToCart(product);
            imageBox.Click += (s, e) => AddToCart(product);
            nameLabel.Click += (s, e) => AddToCart(product);
            priceLabel.Click += (s, e) => AddToCart(product);

            // Hover effects
            card.MouseEnter += (s, e) => card.BackColor = Color.FromArgb(248, 250, 252);
            card.MouseLeave += (s, e) => card.BackColor = Color.White;

            _productsPanel.Controls.Add(card);
        }

        private void AddToCart(Product product)
        {
            if (product.Quantity <= 0)
            {
                MessageBox.Show("This product is out of stock!", "Out of Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check if product already in cart
            foreach (DataGridViewRow row in _cartGrid.Rows)
            {
                if (row.Cells["Product"].Tag?.ToString() == product.Id.ToString())
                {
                    // Increase quantity
                    var currentQty = Convert.ToInt32(row.Cells["Qty"].Value);
                    if (currentQty < product.Quantity)
                    {
                        row.Cells["Qty"].Value = currentQty + 1;
                        UpdateCartRowTotal(row, product.SellingPrice);
                        UpdateTotals();
                    }
                    else
                    {
                        MessageBox.Show("Cannot add more items. Insufficient stock!", "Stock Limit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    return;
                }
            }

            // Add new item to cart
            var rowIndex = _cartGrid.Rows.Add();
            var newRow = _cartGrid.Rows[rowIndex];
            
            newRow.Cells["Product"].Value = product.Name;
            newRow.Cells["Product"].Tag = product.Id.ToString();
            newRow.Cells["Qty"].Value = 1;
            newRow.Cells["Price"].Value = $"${product.SellingPrice:N2}";
            newRow.Cells["Total"].Value = $"${product.SellingPrice:N2}";
            newRow.Cells["Remove"].Value = "❌";

            UpdateTotals();
        }

        private void UpdateCartRowTotal(DataGridViewRow row, decimal unitPrice)
        {
            var qty = Convert.ToInt32(row.Cells["Qty"].Value);
            var total = qty * unitPrice;
            row.Cells["Total"].Value = $"${total:N2}";
        }

        private void UpdateTotals()
        {
            decimal subtotal = 0;

            foreach (DataGridViewRow row in _cartGrid.Rows)
            {
                if (row.Cells["Total"].Value != null)
                {
                    var totalStr = row.Cells["Total"].Value.ToString().Replace("$", "");
                    if (decimal.TryParse(totalStr, out decimal rowTotal))
                    {
                        subtotal += rowTotal;
                    }
                }
            }

            var tax = subtotal * _taxRate;
            var total = subtotal + tax;

            _subtotalLabel.Text = $"Subtotal: ${subtotal:N2}";
            _taxLabel.Text = $"Tax ({_taxRate:P0}): ${tax:N2}";
            _totalLabel.Text = $"Total: ${total:N2}";

            // Calculate change for cash payments
            if (_cashRadio.Checked && decimal.TryParse(_cashAmountBox.Text, out decimal cashAmount))
            {
                var change = cashAmount - total;
                _changeLabel.Text = $"Change: ${Math.Max(0, change):N2}";
                _changeLabel.Visible = true;
                _changeLabel.ForeColor = change >= 0 ? Color.FromArgb(34, 197, 94) : Color.FromArgb(239, 68, 68);
            }
            else
            {
                _changeLabel.Visible = false;
            }

            _completeButton.Enabled = _cartGrid.Rows.Count > 0;
        }     
   // Event Handlers
        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            LoadProducts();
        }

        private void CategoryFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadProducts();
        }

        private void PaymentMethod_CheckedChanged(object sender, EventArgs e)
        {
            _cashAmountBox.Visible = _cashRadio.Checked;
            _changeLabel.Visible = _cashRadio.Checked;
            UpdateTotals();
        }

        private void CashAmount_TextChanged(object sender, EventArgs e)
        {
            UpdateTotals();
        }

        private async void CartGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == _cartGrid.Columns["Qty"].Index)
            {
                var row = _cartGrid.Rows[e.RowIndex];
                var productId = Convert.ToInt32(row.Cells["Product"].Tag);
                var product = await _context.Products.FindAsync(productId);
                
                if (product != null)
                {
                    var newQty = Convert.ToInt32(row.Cells["Qty"].Value);
                    if (newQty > product.Quantity)
                    {
                        MessageBox.Show($"Cannot set quantity to {newQty}. Only {product.Quantity} items available.", 
                            "Stock Limit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        row.Cells["Qty"].Value = product.Quantity;
                        newQty = product.Quantity;
                    }
                    else if (newQty <= 0)
                    {
                        _cartGrid.Rows.RemoveAt(e.RowIndex);
                        UpdateTotals();
                        return;
                    }

                    UpdateCartRowTotal(row, product.SellingPrice);
                    UpdateTotals();
                }
            }
        }

        private void CartGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == _cartGrid.Columns["Remove"].Index)
            {
                _cartGrid.Rows.RemoveAt(e.RowIndex);
                UpdateTotals();
            }
        }

        private void ScanButton_Click(object sender, EventArgs e)
        {
            // QR Code scanner implementation
            var scanForm = new QRScannerForm();
            if (scanForm.ShowDialog() == DialogResult.OK)
            {
                var scannedCode = scanForm.ScannedCode;
                // Find product by barcode or QR code
                SearchProductByCode(scannedCode);
            }
        }

        private async void SearchProductByCode(string code)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.ProductImages)
                    .FirstOrDefaultAsync(p => p.Barcode == code || p.SKU == code);

                if (product != null)
                {
                    AddToCart(product);
                }
                else
                {
                    MessageBox.Show("Product not found!", "Scan Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching product: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void CompleteButton_Click(object sender, EventArgs e)
        {
            if (_cartGrid.Rows.Count == 0)
            {
                MessageBox.Show("Cart is empty!", "Cannot Complete Sale", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate payment
            if (_cashRadio.Checked)
            {
                if (!decimal.TryParse(_cashAmountBox.Text, out decimal cashAmount))
                {
                    MessageBox.Show("Please enter a valid cash amount!", "Invalid Amount", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var total = GetCartTotal();
                if (cashAmount < total)
                {
                    MessageBox.Show("Insufficient cash amount!", "Payment Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            try
            {
                await ProcessSale();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing sale: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async System.Threading.Tasks.Task ProcessSale()
        {
            var currentUser = _authService.CurrentUser;
            var subtotal = GetCartSubtotal();
            var tax = subtotal * _taxRate;
            var total = subtotal + tax;

            // Create sale record
            var sale = new Sale
            {
                InvoiceNumber = GenerateInvoiceNumber(),
                CashierId = currentUser.Id,
                TaxAmount = tax,
                TotalAmount = total,
                PaymentMethod = _cashRadio.Checked ? PaymentMethod.Cash : 
                              _cardRadio.Checked ? PaymentMethod.Card : PaymentMethod.Mobile,
                Status = SaleStatus.Completed,
                SaleDate = DateTime.Now
            };

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();

            // Add sale items
            foreach (DataGridViewRow row in _cartGrid.Rows)
            {
                var productId = Convert.ToInt32(row.Cells["Product"].Tag);
                var quantity = Convert.ToInt32(row.Cells["Qty"].Value);
                var product = await _context.Products.FindAsync(productId);

                if (product != null)
                {
                    var saleItem = new SaleItem
                    {
                        SaleId = sale.Id,
                        ProductId = productId,
                        Quantity = quantity,
                        UnitPrice = product.SellingPrice,
                        TotalPrice = quantity * product.SellingPrice
                    };

                    _context.SaleItems.Add(saleItem);

                    // Update product stock
                    product.Quantity -= quantity;
                    product.UpdatedAt = DateTime.Now;
                }
            }

            await _context.SaveChangesAsync();

            // Handle payment method specific actions
            if (_mobileRadio.Checked)
            {
                ShowKHQRPayment(total);
            }
            else
            {
                CompleteSale(sale);
            }
        }

        private void ShowKHQRPayment(decimal amount)
        {
            var khqrForm = new KHQRPaymentForm(amount);
            khqrForm.PaymentCompleted += (s, e) =>
            {
                PlayCashRegisterSound();
                CompleteSale(null);
            };
            khqrForm.ShowDialog();
        }

        private void CompleteSale(Sale sale)
        {
            PlayCashRegisterSound();

            // Show receipt option
            var result = MessageBox.Show(
                "Sale completed successfully!\n\nWould you like to print the receipt?",
                "Sale Complete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes && sale != null)
            {
                PrintReceipt(sale);
            }

            // Clear cart
            ClearCart();
        }

        private void PrintReceipt(Sale sale)
        {
            var receiptForm = new ReceiptPreviewForm(sale);
            receiptForm.ShowDialog();
        }

        private void PlayCashRegisterSound()
        {
            try
            {
                if (_cashRegisterSound != null)
                {
                    _cashRegisterSound.Play();
                }
                else
                {
                    SystemSounds.Asterisk.Play();
                }
            }
            catch
            {
                // Ignore sound errors
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to clear the cart?",
                "Clear Cart",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                ClearCart();
            }
        }

        private void ClearCart()
        {
            _cartGrid.Rows.Clear();
            _customerNameBox.Clear();
            _cashAmountBox.Text = "0.00";
            UpdateTotals();
        }

        private void HoldButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Hold functionality - Coming Soon!\n\nThis will allow you to save the current cart and resume later.",
                "Hold Sale", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void TaxSettingsButton_Click(object sender, EventArgs e)
        {
            var taxForm = new TaxSettingsForm(_taxRate);
            if (taxForm.ShowDialog() == DialogResult.OK)
            {
                _taxRate = taxForm.TaxRate;
                UpdateTotals();
            }
        }

        // Helper methods
        private decimal GetCartSubtotal()
        {
            decimal subtotal = 0;
            foreach (DataGridViewRow row in _cartGrid.Rows)
            {
                if (row.Cells["Total"].Value != null)
                {
                    var totalStr = row.Cells["Total"].Value.ToString().Replace("$", "");
                    if (decimal.TryParse(totalStr, out decimal rowTotal))
                    {
                        subtotal += rowTotal;
                    }
                }
            }
            return subtotal;
        }

        private decimal GetCartTotal()
        {
            var subtotal = GetCartSubtotal();
            return subtotal + (subtotal * _taxRate);
        }

        private string GenerateInvoiceNumber()
        {
            return $"INV-{DateTime.Now:yyyyMMdd}-{DateTime.Now:HHmmss}";
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
                _cashRegisterSound?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}