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
using System.Threading.Tasks;

namespace SyncVerseStudio.Views
{
    /// <summary>
    /// Modern, clean Point of Sale interface
    /// Three-panel layout: Products | Shopping Cart | Recent Sales
    /// </summary>
    public partial class CashierPOSView : Form
    {
        private readonly AuthenticationService _authService;
        private readonly ApplicationDbContext _context;
        private decimal _taxRate = 0.10m;
        private System.Windows.Forms.Timer _statusTimer;

        // Main Panels
        private Panel _sidebarPanel, _productsPanel, _cartPanel, _recentSalesPanel;
        private FlowLayoutPanel _productCardsPanel;
        private ListView _cartListView, _recentSalesListView;
        
        // Controls
        private TextBox _searchBox, _cashAmountBox;
        private ComboBox _categoryFilter;
        private Label _subtotalLabel, _taxLabel, _totalLabel, _changeLabel;
        private Button _completeSaleButton, _clearCartButton, _holdButton;
        private RadioButton _cashRadio, _cardRadio, _mobileRadio;

        public CashierPOSView(AuthenticationService authService)
        {
            _authService = authService;
            _context = new ApplicationDbContext();
            
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Text = "SYNCVERSE STUDIO - Point of Sale";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.MinimumSize = new Size(1400, 800);
            
            CreateUI();
            this.ResumeLayout(false);
        }

        private void CreateUI()
        {
            // Sidebar (140px)
            CreateSidebar();
            
            // Products Section (Left - 40%)
            CreateProductsPanel();
            
            // Shopping Cart (Center - 35%)
            CreateShoppingCartPanel();
            
            // Recent Sales (Right - 25%)
            CreateRecentSalesPanel();
        }

        #region Sidebar
        private void CreateSidebar()
        {
            _sidebarPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 140,
                BackColor = Color.FromArgb(230, 240, 255),
                Padding = new Padding(0)
            };

            // Logo Section
            var logoPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(140, 100),
                BackColor = Color.White
            };

            var logoIcon = new IconPictureBox
            {
                IconChar = IconChar.Store,
                IconColor = Color.FromArgb(59, 130, 246),
                IconSize = 45,
                Location = new Point(48, 15),
                Size = new Size(45, 45)
            };
            logoPanel.Controls.Add(logoIcon);

            var logoText = new Label
            {
                Text = "SYNCVERSE",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(0, 65),
                Size = new Size(140, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };
            logoPanel.Controls.Add(logoText);

            _sidebarPanel.Controls.Add(logoPanel);

            // Navigation Buttons
            var dashboardBtn = CreateNavButton("Dashboard", IconChar.Home, 120, false);
            dashboardBtn.Click += (s, e) => MessageBox.Show("Dashboard - Coming Soon!", "Navigation", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            CreateNavButton("Point of Sale", IconChar.ShoppingCart, 180, true);
            
            var salesHistoryBtn = CreateNavButton("Sales History", IconChar.Receipt, 240, false);
            salesHistoryBtn.Click += (s, e) => MessageBox.Show("Sales History - Coming Soon!", "Navigation", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            var customersBtn = CreateNavButton("Customers", IconChar.Users, 300, false);
            customersBtn.Click += (s, e) => MessageBox.Show("Customers - Coming Soon!", "Navigation", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Logout Button (Bottom)
            var logoutBtn = CreateNavButton("Logout", IconChar.SignOutAlt, this.ClientSize.Height - 80, false);
            logoutBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            logoutBtn.Click += LogoutButton_Click;

            this.Controls.Add(_sidebarPanel);
        }

        private Button CreateNavButton(string text, IconChar icon, int yPos, bool isActive)
        {
            var btn = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = isActive ? Color.FromArgb(59, 130, 246) : Color.FromArgb(71, 85, 105),
                BackColor = isActive ? Color.White : Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(10, yPos),
                Size = new Size(120, 45),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(35, 0, 0, 0),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;

            var iconPic = new IconPictureBox
            {
                IconChar = icon,
                IconColor = isActive ? Color.FromArgb(59, 130, 246) : Color.FromArgb(71, 85, 105),
                IconSize = 20,
                Location = new Point(15, 12),
                Size = new Size(20, 20),
                BackColor = Color.Transparent
            };
            btn.Controls.Add(iconPic);

            _sidebarPanel.Controls.Add(btn);
            return btn;
        }
        #endregion

        #region Products Panel
        private void CreateProductsPanel()
        {
            _productsPanel = new Panel
            {
                Location = new Point(140, 0),
                Size = new Size((this.ClientSize.Width - 140) * 40 / 100, this.ClientSize.Height),
                BackColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left
            };

            // Header with green background
            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.FromArgb(76, 217, 100)
            };

            var headerLabel = new Label
            {
                Text = "Products",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 12),
                AutoSize = true
            };
            header.Controls.Add(headerLabel);
            _productsPanel.Controls.Add(header);

            // Search and Filter Panel
            var searchPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.White,
                Padding = new Padding(15, 15, 15, 10)
            };

            _searchBox = new TextBox
            {
                Location = new Point(15, 15),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 10F),
                PlaceholderText = "Search products..."
            };
            searchPanel.Controls.Add(_searchBox);

            _categoryFilter = new ComboBox
            {
                Location = new Point(225, 15),
                Size = new Size(140, 30),
                Font = new Font("Segoe UI", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat
            };
            _categoryFilter.Items.Add("All Categories");
            _categoryFilter.SelectedIndex = 0;
            searchPanel.Controls.Add(_categoryFilter);

            _productsPanel.Controls.Add(searchPanel);

            // Product Cards Container
            _productCardsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                AutoScroll = true,
                Padding = new Padding(10, 5, 10, 10)
            };
            _productsPanel.Controls.Add(_productCardsPanel);

            this.Controls.Add(_productsPanel);
        }
        #endregion

        #region Shopping Cart Panel
        private void CreateShoppingCartPanel()
        {
            int leftWidth = (this.ClientSize.Width - 140) * 40 / 100;
            int cartWidth = (this.ClientSize.Width - 140) * 35 / 100;
            
            _cartPanel = new Panel
            {
                Location = new Point(140 + leftWidth, 0),
                Size = new Size(cartWidth, this.ClientSize.Height),
                BackColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            // Header with blue background
            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.FromArgb(59, 130, 246)
            };

            var headerLabel = new Label
            {
                Text = "Shopping Cart",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 12),
                AutoSize = true
            };
            header.Controls.Add(headerLabel);
            _cartPanel.Controls.Add(header);

            // Cart Items ListView
            var cartContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            _cartListView = new ListView
            {
                Dock = DockStyle.Top,
                Height = 250,
                View = View.Details,
                FullRowSelect = true,
                GridLines = false,
                HeaderStyle = ColumnHeaderStyle.Nonclickable,
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.None
            };

            _cartListView.Columns.Add("Product", 180);
            _cartListView.Columns.Add("Qty", 50, HorizontalAlignment.Center);
            _cartListView.Columns.Add("Price", 70, HorizontalAlignment.Right);
            _cartListView.Columns.Add("Total", 80, HorizontalAlignment.Right);

            cartContainer.Controls.Add(_cartListView);

            // Totals Section
            var totalsPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 120,
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            _subtotalLabel = CreateTotalLabel("Subtotal: $0.00", 10);
            _taxLabel = CreateTotalLabel("Tax (10%): $0.00", 35);
            _totalLabel = CreateTotalLabel("$0.00", 60, 18, true);
            _totalLabel.ForeColor = Color.FromArgb(34, 197, 94);

            totalsPanel.Controls.AddRange(new Control[] { _subtotalLabel, _taxLabel, _totalLabel });
            cartContainer.Controls.Add(totalsPanel);

            // Payment Method Section
            var paymentPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 120,
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            var paymentLabel = new Label
            {
                Text = "Payment Method:",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(15, 10),
                Size = new Size(150, 25)
            };
            paymentPanel.Controls.Add(paymentLabel);

            _cashRadio = new RadioButton
            {
                Text = "Cash",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(15, 40),
                Size = new Size(80, 25),
                Checked = true
            };
            paymentPanel.Controls.Add(_cashRadio);

            _cardRadio = new RadioButton
            {
                Text = "Card",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(110, 40),
                Size = new Size(80, 25)
            };
            paymentPanel.Controls.Add(_cardRadio);

            _mobileRadio = new RadioButton
            {
                Text = "Mobile",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(205, 40),
                Size = new Size(80, 25)
            };
            paymentPanel.Controls.Add(_mobileRadio);

            var cashLabel = new Label
            {
                Text = "Cash Amount:",
                Font = new Font("Segoe UI", 9F),
                Location = new Point(15, 75),
                Size = new Size(100, 20)
            };
            paymentPanel.Controls.Add(cashLabel);

            _cashAmountBox = new TextBox
            {
                Location = new Point(120, 73),
                Size = new Size(100, 25),
                Font = new Font("Segoe UI", 10F),
                Text = "0.00",
                TextAlign = HorizontalAlignment.Right
            };
            _cashAmountBox.TextChanged += (s, e) => CalculateChange();
            paymentPanel.Controls.Add(_cashAmountBox);

            _changeLabel = new Label
            {
                Text = "Change: $0.00",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(34, 197, 94),
                Location = new Point(230, 75),
                Size = new Size(150, 20)
            };
            paymentPanel.Controls.Add(_changeLabel);

            cartContainer.Controls.Add(paymentPanel);

            // Action Buttons
            var buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 120,
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            _completeSaleButton = new Button
            {
                Text = "COMPLETE SALE",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(59, 130, 246),
                Location = new Point(15, 15),
                Size = new Size(cartWidth - 50, 45),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _completeSaleButton.FlatAppearance.BorderSize = 0;
            _completeSaleButton.Click += CompleteSale_Click;
            buttonPanel.Controls.Add(_completeSaleButton);

            var bottomButtonsPanel = new Panel
            {
                Location = new Point(15, 65),
                Size = new Size(cartWidth - 50, 40),
                BackColor = Color.Transparent
            };

            _clearCartButton = new Button
            {
                Text = "CLEAR CART",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(52, 58, 64),
                Location = new Point(0, 0),
                Size = new Size((cartWidth - 60) / 2, 35),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _clearCartButton.FlatAppearance.BorderSize = 0;
            _clearCartButton.Click += ClearCart_Click;
            bottomButtonsPanel.Controls.Add(_clearCartButton);

            _holdButton = new Button
            {
                Text = "HOLD",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(76, 217, 100),
                Location = new Point((cartWidth - 60) / 2 + 10, 0),
                Size = new Size((cartWidth - 60) / 2, 35),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _holdButton.FlatAppearance.BorderSize = 0;
            bottomButtonsPanel.Controls.Add(_holdButton);

            buttonPanel.Controls.Add(bottomButtonsPanel);
            cartContainer.Controls.Add(buttonPanel);

            _cartPanel.Controls.Add(cartContainer);
            this.Controls.Add(_cartPanel);
        }

        private Label CreateTotalLabel(string text, int yPos, int fontSize = 11, bool isBold = false)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", fontSize, isBold ? FontStyle.Bold : FontStyle.Regular),
                Location = new Point(15, yPos),
                Size = new Size(350, 25),
                TextAlign = ContentAlignment.MiddleRight
            };
        }
        #endregion

        #region Recent Sales Panel
        private void CreateRecentSalesPanel()
        {
            int leftWidth = (this.ClientSize.Width - 140) * 40 / 100;
            int cartWidth = (this.ClientSize.Width - 140) * 35 / 100;
            int salesWidth = (this.ClientSize.Width - 140) * 25 / 100;
            
            _recentSalesPanel = new Panel
            {
                Location = new Point(140 + leftWidth + cartWidth, 0),
                Size = new Size(salesWidth, this.ClientSize.Height),
                BackColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right
            };

            // Header with blue background
            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.FromArgb(59, 130, 246)
            };

            var headerIcon = new IconPictureBox
            {
                IconChar = IconChar.Receipt,
                IconColor = Color.White,
                IconSize = 20,
                Location = new Point(15, 15),
                Size = new Size(20, 20)
            };
            header.Controls.Add(headerIcon);

            var headerLabel = new Label
            {
                Text = "Recent Sales",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(40, 12),
                AutoSize = true
            };
            header.Controls.Add(headerLabel);
            _recentSalesPanel.Controls.Add(header);

            // Recent Sales ListView
            var salesContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            _recentSalesListView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = false,
                HeaderStyle = ColumnHeaderStyle.Nonclickable,
                Font = new Font("Segoe UI", 9F),
                BorderStyle = BorderStyle.None
            };

            _recentSalesListView.Columns.Add("Invoice", 100);
            _recentSalesListView.Columns.Add("Time", 80);
            _recentSalesListView.Columns.Add("Amount", 80, HorizontalAlignment.Right);

            salesContainer.Controls.Add(_recentSalesListView);
            _recentSalesPanel.Controls.Add(salesContainer);

            this.Controls.Add(_recentSalesPanel);
        }
        #endregion

        #region Data Loading
        private async void LoadData()
        {
            await LoadCategories();
            await LoadProducts();
            await LoadRecentSales();
        }

        private async Task LoadCategories()
        {
            try
            {
                var categories = await _context.Categories
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.Name)
                    .ToListAsync();

                _categoryFilter.Items.Clear();
                _categoryFilter.Items.Add("All Categories");
                
                foreach (var category in categories)
                {
                    _categoryFilter.Items.Add(category);
                }
                
                _categoryFilter.DisplayMember = "Name";
                _categoryFilter.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadProducts()
        {
            try
            {
                _productCardsPanel.Controls.Clear();

                var query = _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.ProductImages)
                    .Where(p => p.IsActive);

                // Apply search filter
                if (!string.IsNullOrEmpty(_searchBox.Text))
                {
                    var searchTerm = _searchBox.Text.ToLower();
                    query = query.Where(p => p.Name.ToLower().Contains(searchTerm) ||
                                           p.Barcode.ToLower().Contains(searchTerm));
                }

                // Apply category filter
                if (_categoryFilter.SelectedIndex > 0 && _categoryFilter.SelectedItem is Category category)
                {
                    query = query.Where(p => p.CategoryId == category.Id);
                }

                var products = await query.OrderBy(p => p.Name).ToListAsync();

                foreach (var product in products)
                {
                    CreateProductCard(product);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadRecentSales()
        {
            try
            {
                _recentSalesListView.Items.Clear();

                var recentSales = await _context.Sales
                    .Where(s => s.Status == SaleStatus.Completed)
                    .OrderByDescending(s => s.SaleDate)
                    .Take(20)
                    .ToListAsync();

                foreach (var sale in recentSales)
                {
                    var item = new ListViewItem(sale.InvoiceNumber);
                    item.SubItems.Add(sale.SaleDate.ToString("HH:mm"));
                    item.SubItems.Add($"${sale.TotalAmount:N2}");
                    item.Tag = sale;
                    _recentSalesListView.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading recent sales: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Product Card Creation
        private void CreateProductCard(Product product)
        {
            var card = new Panel
            {
                Size = new Size(170, 220),
                Margin = new Padding(5),
                BackColor = Color.White,
                Cursor = Cursors.Hand,
                Tag = product,
                BorderStyle = BorderStyle.FixedSingle
            };

            card.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, card.Width - 1, card.Height - 1), 6))
                using (var brush = new SolidBrush(Color.White))
                using (var pen = new Pen(Color.FromArgb(220, 220, 220), 1))
                {
                    e.Graphics.FillPath(brush, path);
                    e.Graphics.DrawPath(pen, path);
                }
            };

            // Product Image
            var imageBox = new PictureBox
            {
                Location = new Point(5, 5),
                Size = new Size(160, 90),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(250, 250, 250)
            };

            try
            {
                var primaryImage = ProductImageHelper.GetPrimaryImage(product);
                if (primaryImage != null)
                {
                    imageBox.Image = ProductImageHelper.ResizeImage(primaryImage, 160, 90);
                }
                else
                {
                    var defaultImage = ProductImageHelper.GetDefaultBrandImage();
                    if (defaultImage != null)
                    {
                        imageBox.Image = ProductImageHelper.ResizeImage(defaultImage, 160, 90);
                    }
                }
            }
            catch { }

            card.Controls.Add(imageBox);

            // Product Name
            var nameLabel = new Label
            {
                Text = product.Name.Length > 22 ? product.Name.Substring(0, 19) + "..." : product.Name,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(5, 100),
                Size = new Size(160, 35),
                TextAlign = ContentAlignment.TopCenter
            };
            card.Controls.Add(nameLabel);

            // Price with green background
            var pricePanel = new Panel
            {
                Location = new Point(5, 140),
                Size = new Size(160, 30),
                BackColor = Color.FromArgb(76, 217, 100)
            };

            var priceLabel = new Label
            {
                Text = $"${product.SellingPrice:N2}",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            pricePanel.Controls.Add(priceLabel);
            card.Controls.Add(pricePanel);

            // Stock label
            var stockLabel = new Label
            {
                Text = $"Stock: {product.Quantity}",
                Font = new Font("Segoe UI", 8F),
                ForeColor = product.Quantity > 0 ? Color.FromArgb(100, 116, 139) : Color.FromArgb(239, 68, 68),
                Location = new Point(5, 175),
                Size = new Size(160, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };
            card.Controls.Add(stockLabel);

            // Add Button (green)
            var addButton = new Button
            {
                Text = "+ ADD",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(76, 217, 100),
                Location = new Point(5, 195),
                Size = new Size(160, 25),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            addButton.FlatAppearance.BorderSize = 0;
            addButton.Click += (s, e) => AddToCart(product);
            card.Controls.Add(addButton);

            // Hover effects
            card.MouseEnter += (s, e) =>
            {
                card.BackColor = Color.FromArgb(248, 250, 252);
            };
            card.MouseLeave += (s, e) =>
            {
                card.BackColor = Color.White;
            };

            // Click to add
            card.Click += (s, e) => AddToCart(product);
            imageBox.Click += (s, e) => AddToCart(product);
            nameLabel.Click += (s, e) => AddToCart(product);
            pricePanel.Click += (s, e) => AddToCart(product);

            // Disable if out of stock
            if (product.Quantity <= 0)
            {
                card.Enabled = false;
                card.BackColor = Color.FromArgb(245, 245, 245);
                addButton.Text = "OUT OF STOCK";
                addButton.BackColor = Color.FromArgb(200, 200, 200);
                addButton.Enabled = false;
                pricePanel.BackColor = Color.FromArgb(200, 200, 200);
            }

            _productCardsPanel.Controls.Add(card);
        }
        #endregion

        #region Cart Operations
        private void AddToCart(Product product)
        {
            if (product.Quantity <= 0)
            {
                MessageBox.Show("This product is out of stock!", "Out of Stock", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check if product already in cart
            foreach (ListViewItem item in _cartListView.Items)
            {
                if (item.Tag is Product p && p.Id == product.Id)
                {
                    int currentQty = int.Parse(item.SubItems[1].Text);
                    if (currentQty < product.Quantity)
                    {
                        currentQty++;
                        item.SubItems[1].Text = currentQty.ToString();
                        item.SubItems[3].Text = $"${(currentQty * product.SellingPrice):N2}";
                        UpdateTotals();
                    }
                    else
                    {
                        MessageBox.Show("Cannot add more items. Insufficient stock!", "Stock Limit", 
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    return;
                }
            }

            // Add new item
            var listItem = new ListViewItem(product.Name);
            listItem.SubItems.Add("1");
            listItem.SubItems.Add($"${product.SellingPrice:N2}");
            listItem.SubItems.Add($"${product.SellingPrice:N2}");
            listItem.Tag = product;
            _cartListView.Items.Add(listItem);

            UpdateTotals();
        }

        private void UpdateTotals()
        {
            decimal subtotal = 0;

            foreach (ListViewItem item in _cartListView.Items)
            {
                if (item.Tag is Product product)
                {
                    int qty = int.Parse(item.SubItems[1].Text);
                    subtotal += qty * product.SellingPrice;
                }
            }

            decimal tax = subtotal * _taxRate;
            decimal total = subtotal + tax;

            _subtotalLabel.Text = $"Subtotal: ${subtotal:N2}";
            _taxLabel.Text = $"Tax ({_taxRate:P0}): ${tax:N2}";
            _totalLabel.Text = $"${total:N2}";

            CalculateChange();
        }

        private void CalculateChange()
        {
            if (_cashRadio.Checked && decimal.TryParse(_cashAmountBox.Text, out decimal cashAmount))
            {
                decimal total = GetCartTotal();
                decimal change = cashAmount - total;
                _changeLabel.Text = $"Change: ${Math.Max(0, change):N2}";
                _changeLabel.ForeColor = change >= 0 ? Color.FromArgb(34, 197, 94) : Color.FromArgb(239, 68, 68);
            }
            else
            {
                _changeLabel.Text = "Change: $0.00";
            }
        }

        private decimal GetCartTotal()
        {
            decimal subtotal = 0;
            foreach (ListViewItem item in _cartListView.Items)
            {
                if (item.Tag is Product product)
                {
                    int qty = int.Parse(item.SubItems[1].Text);
                    subtotal += qty * product.SellingPrice;
                }
            }
            return subtotal + (subtotal * _taxRate);
        }

        private void ClearCart_Click(object sender, EventArgs e)
        {
            if (_cartListView.Items.Count == 0) return;

            var result = MessageBox.Show("Are you sure you want to clear the cart?", "Clear Cart",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                _cartListView.Items.Clear();
                _cashAmountBox.Text = "0.00";
                UpdateTotals();
            }
        }
        #endregion

        #region Complete Sale
        private async void CompleteSale_Click(object sender, EventArgs e)
        {
            if (_cartListView.Items.Count == 0)
            {
                MessageBox.Show("Cart is empty!", "Cannot Complete Sale", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate payment
            if (_cashRadio.Checked)
            {
                if (!decimal.TryParse(_cashAmountBox.Text, out decimal cashAmount))
                {
                    MessageBox.Show("Please enter a valid cash amount!", "Invalid Amount", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                decimal total = GetCartTotal();
                if (cashAmount < total)
                {
                    MessageBox.Show("Insufficient cash amount!", "Payment Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            try
            {
                await ProcessSale();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing sale: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task ProcessSale()
        {
            var currentUser = _authService.CurrentUser;
            decimal subtotal = 0;

            foreach (ListViewItem item in _cartListView.Items)
            {
                if (item.Tag is Product product)
                {
                    int qty = int.Parse(item.SubItems[1].Text);
                    subtotal += qty * product.SellingPrice;
                }
            }

            decimal tax = subtotal * _taxRate;
            decimal total = subtotal + tax;

            // Create sale
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

            // Add sale items and update stock
            foreach (ListViewItem item in _cartListView.Items)
            {
                if (item.Tag is Product product)
                {
                    int qty = int.Parse(item.SubItems[1].Text);

                    var saleItem = new SaleItem
                    {
                        SaleId = sale.Id,
                        ProductId = product.Id,
                        Quantity = qty,
                        UnitPrice = product.SellingPrice,
                        TotalPrice = qty * product.SellingPrice
                    };

                    _context.SaleItems.Add(saleItem);

                    // Update stock
                    product.Quantity -= qty;
                    product.UpdatedAt = DateTime.Now;
                }
            }

            await _context.SaveChangesAsync();

            // Show receipt
            ShowReceipt(sale);

            // Clear cart and refresh
            _cartListView.Items.Clear();
            _cashAmountBox.Text = "0.00";
            UpdateTotals();
            await LoadProducts();
            await LoadRecentSales();
        }

        private void ShowReceipt(Sale sale)
        {
            var receiptText = $@"
╔══════════════════════════════════╗
║      SYNCVERSE STUDIO POS        ║
╚══════════════════════════════════╝

Invoice: {sale.InvoiceNumber}
Date: {sale.SaleDate:yyyy-MM-dd HH:mm:ss}
Cashier: {_authService.CurrentUser?.FirstName} {_authService.CurrentUser?.LastName}

--------------------------------
ITEMS:
";

            foreach (ListViewItem item in _cartListView.Items)
            {
                if (item.Tag is Product product)
                {
                    int qty = int.Parse(item.SubItems[1].Text);
                    decimal itemTotal = qty * product.SellingPrice;
                    receiptText += $"{product.Name}\n  {qty} x ${product.SellingPrice:N2} = ${itemTotal:N2}\n\n";
                }
            }

            receiptText += $@"--------------------------------
Subtotal: ${(sale.TotalAmount - sale.TaxAmount):N2}
Tax ({_taxRate:P0}): ${sale.TaxAmount:N2}
TOTAL: ${sale.TotalAmount:N2}

Payment: {sale.PaymentMethod}
";

            if (_cashRadio.Checked && decimal.TryParse(_cashAmountBox.Text, out decimal cashAmount))
            {
                decimal change = cashAmount - sale.TotalAmount;
                receiptText += $"Cash: ${cashAmount:N2}\nChange: ${change:N2}\n";
            }

            receiptText += @"
Thank you for your purchase!
Visit us again soon!
";

            MessageBox.Show(receiptText, "Sale Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private string GenerateInvoiceNumber()
        {
            return $"INV-{DateTime.Now:yyyyMMdd}-{DateTime.Now:HHmmss}";
        }
        #endregion

        #region Event Handlers & Helpers
        private void SetupEventHandlers()
        {
            _searchBox.TextChanged += async (s, e) => await LoadProducts();
            _categoryFilter.SelectedIndexChanged += async (s, e) => await LoadProducts();
            _cashRadio.CheckedChanged += (s, e) => CalculateChange();
            _cardRadio.CheckedChanged += (s, e) => CalculateChange();
            _mobileRadio.CheckedChanged += (s, e) => CalculateChange();
            
            // Double-click to remove item from cart
            _cartListView.DoubleClick += (s, e) =>
            {
                if (_cartListView.SelectedItems.Count > 0)
                {
                    var result = MessageBox.Show("Remove this item from cart?", "Remove Item",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    
                    if (result == DialogResult.Yes)
                    {
                        _cartListView.Items.Remove(_cartListView.SelectedItems[0]);
                        UpdateTotals();
                    }
                }
            };

            // Double-click recent sale to view details
            _recentSalesListView.DoubleClick += async (s, e) =>
            {
                if (_recentSalesListView.SelectedItems.Count > 0)
                {
                    var sale = _recentSalesListView.SelectedItems[0].Tag as Sale;
                    if (sale != null)
                    {
                        await ShowSaleDetails(sale);
                    }
                }
            };

            // Keyboard shortcuts
            this.KeyPreview = true;
            this.KeyDown += (s, e) =>
            {
                if (e.Control && e.KeyCode == Keys.F) _searchBox.Focus();
                if (e.KeyCode == Keys.F12) CompleteSale_Click(s, e);
                if (e.KeyCode == Keys.Escape) ClearCart_Click(s, e);
            };
        }

        private async Task ShowSaleDetails(Sale sale)
        {
            var saleWithItems = await _context.Sales
                .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
                .FirstOrDefaultAsync(s => s.Id == sale.Id);

            if (saleWithItems == null) return;

            var details = $@"Invoice: {saleWithItems.InvoiceNumber}
Date: {saleWithItems.SaleDate:yyyy-MM-dd HH:mm:ss}
Total: ${saleWithItems.TotalAmount:N2}
Payment: {saleWithItems.PaymentMethod}

Items:
";

            foreach (var item in saleWithItems.SaleItems)
            {
                details += $"- {item.Product.Name} x{item.Quantity} = ${item.TotalPrice:N2}\n";
            }

            MessageBox.Show(details, "Sale Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void LoadTaxSettings()
        {
            try
            {
                _taxRate = 0.10m; // Default 10%
                
                // Start status update timer
                _statusTimer = new System.Windows.Forms.Timer { Interval = 30000 };
                _statusTimer.Tick += async (s, e) => await LoadRecentSales();
                _statusTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading settings: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void InitializeSounds()
        {
            try
            {
                // Initialize sound player if needed
            }
            catch
            {
                // Ignore sound errors
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

        private void LogoutButton_Click(object sender, EventArgs e)
        {
            // Logout and exit application directly
            _authService.Logout();
            
            // Close all forms
            foreach (Form form in Application.OpenForms.Cast<Form>().ToList())
            {
                form.Close();
            }
            
            // Exit application
            Application.Exit();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context?.Dispose();
                _statusTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }

    // Extension method for rounded rectangles
    public static class GraphicsExtensions
    {
        public static void FillRoundedRectangle(this Graphics graphics, Brush brush, Rectangle bounds, int radius)
        {
            using (var path = new GraphicsPath())
            {
                path.AddArc(bounds.X, bounds.Y, radius, radius, 180, 90);
                path.AddArc(bounds.Right - radius, bounds.Y, radius, radius, 270, 90);
                path.AddArc(bounds.Right - radius, bounds.Bottom - radius, radius, radius, 0, 90);
                path.AddArc(bounds.X, bounds.Bottom - radius, radius, radius, 90, 90);
                path.CloseFigure();
                graphics.FillPath(brush, path);
            }
        }
    }
}
