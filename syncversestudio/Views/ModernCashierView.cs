using System;
using System.Collections.Generic;
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
using System.Threading.Tasks;

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
                Height = 90,
                BackColor = Color.FromArgb(30, 41, 59)
            };

            // Add gradient background
            _headerPanel.Paint += (s, e) =>
            {
                using (var brush = new LinearGradientBrush(
                    new Rectangle(0, 0, _headerPanel.Width, _headerPanel.Height),
                    Color.FromArgb(30, 41, 59),
                    Color.FromArgb(51, 65, 85),
                    LinearGradientMode.Horizontal))
                {
                    e.Graphics.FillRectangle(brush, _headerPanel.ClientRectangle);
                }
            };

            // Logo section
            var logoPanel = new Panel
            {
                Location = new Point(25, 15),
                Size = new Size(250, 60),
                BackColor = Color.Transparent
            };

            var logoIcon = new IconPictureBox
            {
                IconChar = IconChar.CashRegister,
                IconColor = Color.FromArgb(34, 197, 94),
                IconSize = 36,
                Location = new Point(0, 12),
                Size = new Size(36, 36),
                BackColor = Color.Transparent
            };
            logoPanel.Controls.Add(logoIcon);

            var logoLabel = new Label
            {
                Text = "SyncVerse POS",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(45, 8),
                Size = new Size(180, 30),
                BackColor = Color.Transparent
            };
            logoPanel.Controls.Add(logoLabel);

            var taglineLabel = new Label
            {
                Text = "Professional Point of Sale System",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(45, 35),
                Size = new Size(200, 20),
                BackColor = Color.Transparent
            };
            logoPanel.Controls.Add(taglineLabel);

            _headerPanel.Controls.Add(logoPanel);

            // Status indicators
            CreateStatusIndicators();

            // User info and controls
            CreateUserControls();

            this.Controls.Add(_headerPanel);
        }

        private void CreateStatusIndicators()
        {
            var statusPanel = new Panel
            {
                Location = new Point(400, 20),
                Size = new Size(400, 50),
                BackColor = Color.Transparent
            };

            // Sales today indicator
            var salesIcon = new IconPictureBox
            {
                IconChar = IconChar.ChartLine,
                IconColor = Color.FromArgb(34, 197, 94),
                IconSize = 20,
                Location = new Point(0, 15),
                Size = new Size(20, 20),
                BackColor = Color.Transparent
            };
            statusPanel.Controls.Add(salesIcon);

            _statusLabel = new Label
            {
                Text = "Today: $0.00 | 0 Sales",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(34, 197, 94),
                Location = new Point(25, 10),
                Size = new Size(200, 25),
                BackColor = Color.Transparent
            };
            statusPanel.Controls.Add(_statusLabel);

            // Connection status
            var connectionIcon = new IconPictureBox
            {
                IconChar = IconChar.Wifi,
                IconColor = Color.FromArgb(34, 197, 94),
                IconSize = 16,
                Location = new Point(250, 17),
                Size = new Size(16, 16),
                BackColor = Color.Transparent
            };
            statusPanel.Controls.Add(connectionIcon);

            var connectionLabel = new Label
            {
                Text = "Online",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(34, 197, 94),
                Location = new Point(270, 15),
                Size = new Size(50, 20),
                BackColor = Color.Transparent
            };
            statusPanel.Controls.Add(connectionLabel);

            _headerPanel.Controls.Add(statusPanel);
        }

        private void CreateUserControls()
        {
            var currentUser = _authService.CurrentUser;
            
            // User info panel
            var userPanel = new Panel
            {
                Location = new Point(this.Width - 350, 15),
                Size = new Size(320, 60),
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            var userIcon = new IconPictureBox
            {
                IconChar = IconChar.UserCircle,
                IconColor = Color.FromArgb(148, 163, 184),
                IconSize = 32,
                Location = new Point(0, 14),
                Size = new Size(32, 32),
                BackColor = Color.Transparent
            };
            userPanel.Controls.Add(userIcon);

            var userLabel = new Label
            {
                Text = $"{currentUser?.FirstName} {currentUser?.LastName}",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(40, 10),
                Size = new Size(200, 25),
                BackColor = Color.Transparent
            };
            userPanel.Controls.Add(userLabel);

            var roleLabel = new Label
            {
                Text = "Cashier Terminal",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(40, 30),
                Size = new Size(100, 20),
                BackColor = Color.Transparent
            };
            userPanel.Controls.Add(roleLabel);

            var timeLabel = new Label
            {
                Text = DateTime.Now.ToString("MMM dd, yyyy - HH:mm"),
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(150, 30),
                Size = new Size(150, 20),
                TextAlign = ContentAlignment.MiddleRight,
                BackColor = Color.Transparent
            };
            userPanel.Controls.Add(timeLabel);

            // Logout button
            var logoutBtn = new IconButton
            {
                IconChar = IconChar.SignOutAlt,
                IconColor = Color.FromArgb(239, 68, 68),
                IconSize = 18,
                Text = "",
                Location = new Point(280, 20),
                Size = new Size(35, 35),
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            logoutBtn.FlatAppearance.BorderSize = 0;
            logoutBtn.Click += LogoutBtn_Click;
            userPanel.Controls.Add(logoutBtn);

            // Update time every minute
            var timer = new System.Windows.Forms.Timer { Interval = 60000 };
            timer.Tick += (s, e) => timeLabel.Text = DateTime.Now.ToString("MMM dd, yyyy - HH:mm");
            timer.Start();

            _headerPanel.Controls.Add(userPanel);
        }

        private void LogoutBtn_Click(object? sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to logout?\n\nAny unsaved transactions will be lost.",
                "Confirm Logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Close();
            }
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
            // Cart header with item count
            var cartHeaderPanel = new Panel
            {
                Location = new Point(20, 20),
                Size = new Size(410, 35),
                BackColor = Color.Transparent
            };

            var cartHeader = new Label
            {
                Text = "Shopping Cart",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(0, 5),
                Size = new Size(150, 30),
                BackColor = Color.Transparent
            };
            cartHeaderPanel.Controls.Add(cartHeader);

            var itemCountLabel = new Label
            {
                Text = "(0 items)",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(155, 10),
                Size = new Size(80, 20),
                BackColor = Color.Transparent
            };
            cartHeaderPanel.Controls.Add(itemCountLabel);

            // Quick actions for cart
            var clearAllBtn = new Button
            {
                Text = "Clear All",
                Font = new Font("Segoe UI", 8F),
                ForeColor = Color.FromArgb(239, 68, 68),
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(320, 5),
                Size = new Size(80, 25),
                Cursor = Cursors.Hand
            };
            clearAllBtn.FlatAppearance.BorderColor = Color.FromArgb(239, 68, 68);
            clearAllBtn.FlatAppearance.BorderSize = 1;
            clearAllBtn.Click += ClearButton_Click;
            cartHeaderPanel.Controls.Add(clearAllBtn);

            _rightPanel.Controls.Add(cartHeaderPanel);

            _cartGrid = new DataGridView
            {
                Location = new Point(20, 65),
                Size = new Size(410, 195),
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
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ScrollBars = ScrollBars.Vertical
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
            var customerPanel = new Panel
            {
                Location = new Point(20, 275),
                Size = new Size(410, 65),
                BackColor = Color.Transparent
            };

            var customerLabel = new Label
            {
                Text = "Customer Information",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(0, 0),
                Size = new Size(200, 25),
                BackColor = Color.Transparent
            };
            customerPanel.Controls.Add(customerLabel);

            _customerNameBox = new TextBox
            {
                Location = new Point(0, 30),
                Size = new Size(300, 30),
                Font = new Font("Segoe UI", 10F),
                PlaceholderText = "Customer name (optional)"
            };
            customerPanel.Controls.Add(_customerNameBox);

            _customerSearchButton = new Button
            {
                Text = "🔍",
                Location = new Point(310, 30),
                Size = new Size(35, 30),
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12F),
                Cursor = Cursors.Hand
            };
            _customerSearchButton.FlatAppearance.BorderSize = 0;
            _customerSearchButton.Click += CustomerSearchButton_Click;
            customerPanel.Controls.Add(_customerSearchButton);

            var newCustomerBtn = new Button
            {
                Text = "New",
                Location = new Point(355, 30),
                Size = new Size(55, 30),
                BackColor = Color.FromArgb(34, 197, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F),
                Cursor = Cursors.Hand
            };
            newCustomerBtn.FlatAppearance.BorderSize = 0;
            newCustomerBtn.Click += NewCustomerButton_Click;
            customerPanel.Controls.Add(newCustomerBtn);

            _rightPanel.Controls.Add(customerPanel);
        }

        private void CreatePaymentSection()
        {
            _paymentPanel = new Panel
            {
                Location = new Point(20, 350),
                Size = new Size(410, 160),
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
                Text = "0.00",
                TextAlign = HorizontalAlignment.Right
            };
            _paymentPanel.Controls.Add(_cashAmountBox);

            // Quick cash amount buttons
            var quickCashPanel = new Panel
            {
                Location = new Point(230, 70),
                Size = new Size(170, 35),
                BackColor = Color.Transparent
            };

            var amounts = new[] { "10", "20", "50", "100" };
            for (int i = 0; i < amounts.Length; i++)
            {
                var quickBtn = new Button
                {
                    Text = "$" + amounts[i],
                    Location = new Point(i * 40, 0),
                    Size = new Size(35, 25),
                    BackColor = Color.FromArgb(226, 232, 240),
                    ForeColor = Color.FromArgb(51, 65, 85),
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 8F),
                    Cursor = Cursors.Hand,
                    Tag = amounts[i]
                };
                quickBtn.FlatAppearance.BorderSize = 0;
                quickBtn.Click += QuickCashButton_Click;
                quickCashPanel.Controls.Add(quickBtn);
            }

            _paymentPanel.Controls.Add(quickCashPanel);

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
                Location = new Point(20, 530),
                Size = new Size(410, 60),
                BackColor = Color.Transparent
            };

            _completeButton = new Button
            {
                Text = "💰 COMPLETE SALE (F12)",
                Location = new Point(0, 0),
                Size = new Size(200, 50),
                BackColor = Color.FromArgb(34, 197, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            _completeButton.FlatAppearance.BorderSize = 0;
            _completeButton.Click += CompleteButton_Click;
            buttonPanel.Controls.Add(_completeButton);

            _clearButton = new Button
            {
                Text = "🗑️ CLEAR (F9)",
                Location = new Point(210, 0),
                Size = new Size(95, 50),
                BackColor = Color.FromArgb(239, 68, 68),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            _clearButton.FlatAppearance.BorderSize = 0;
            _clearButton.Click += ClearButton_Click;
            buttonPanel.Controls.Add(_clearButton);

            _holdButton = new Button
            {
                Text = "⏸️ HOLD (F10)",
                Location = new Point(315, 0),
                Size = new Size(95, 50),
                BackColor = Color.FromArgb(249, 115, 22),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
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
            
            // Keyboard shortcuts
            this.KeyPreview = true;
            this.KeyDown += ModernCashierView_KeyDown;
            
            // Focus events for better UX
            _searchBox.Enter += (s, e) => _searchBox.SelectAll();
            _cashAmountBox.Enter += (s, e) => _cashAmountBox.SelectAll();
            _customerNameBox.Enter += (s, e) => _customerNameBox.SelectAll();
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
                
                // Start status update timer
                _statusTimer = new System.Windows.Forms.Timer { Interval = 30000 }; // Update every 30 seconds
                _statusTimer.Tick += async (s, e) => await UpdateDailySalesStatus();
                _statusTimer.Start();
                
                // Initial status update
                await UpdateDailySalesStatus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tax settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async System.Threading.Tasks.Task UpdateDailySalesStatus()
        {
            try
            {
                var today = DateTime.Today;
                var todaySales = await _context.Sales
                    .Where(s => s.SaleDate.Date == today && s.Status == SaleStatus.Completed)
                    .SumAsync(s => s.TotalAmount);
                
                var salesCount = await _context.Sales
                    .CountAsync(s => s.SaleDate.Date == today && s.Status == SaleStatus.Completed);

                if (_statusLabel != null)
                {
                    _statusLabel.Text = $"Today: ${todaySales:N2} | {salesCount} Sales";
                }
            }
            catch
            {
                // Ignore errors in status update
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
                Size = new Size(185, 240),
                Margin = new Padding(8),
                BackColor = Color.White,
                Cursor = Cursors.Hand,
                Tag = product
            };

            card.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                
                // Draw shadow
                using (var shadowBrush = new SolidBrush(Color.FromArgb(20, 0, 0, 0)))
                {
                    e.Graphics.FillRoundedRectangle(shadowBrush, new Rectangle(2, 2, card.Width - 2, card.Height - 2), 12);
                }
                
                // Draw main card
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, card.Width - 1, card.Height - 1), 12))
                using (var brush = new SolidBrush(Color.White))
                using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1))
                {
                    e.Graphics.FillPath(brush, path);
                    e.Graphics.DrawPath(pen, path);
                }
            };

            // Stock status indicator
            var stockIndicator = new Panel
            {
                Location = new Point(10, 10),
                Size = new Size(8, 8),
                BackColor = product.Quantity > product.MinQuantity ? Color.FromArgb(34, 197, 94) :
                           product.Quantity > 0 ? Color.FromArgb(249, 115, 22) : Color.FromArgb(239, 68, 68)
            };
            stockIndicator.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.FillEllipse(new SolidBrush(stockIndicator.BackColor), 0, 0, 8, 8);
            };
            card.Controls.Add(stockIndicator);

            // Product ID badge
            var idBadge = new Label
            {
                Text = $"#{product.Id}",
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(59, 130, 246),
                BackColor = Color.FromArgb(239, 246, 255),
                Location = new Point(140, 8),
                Size = new Size(35, 18),
                TextAlign = ContentAlignment.MiddleCenter
            };
            idBadge.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, idBadge.Width - 1, idBadge.Height - 1), 9))
                using (var brush = new SolidBrush(Color.FromArgb(239, 246, 255)))
                {
                    e.Graphics.FillPath(brush, path);
                }
            };
            card.Controls.Add(idBadge);

            // Product image
            var imageBox = new PictureBox
            {
                Location = new Point(15, 30),
                Size = new Size(155, 110),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(248, 250, 252)
            };

            imageBox.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, imageBox.Width - 1, imageBox.Height - 1), 8))
                using (var brush = new SolidBrush(Color.FromArgb(248, 250, 252)))
                {
                    e.Graphics.FillPath(brush, path);
                }
            };

            try
            {
                var primaryImage = ProductImageHelper.GetPrimaryImage(product);
                if (primaryImage != null)
                {
                    imageBox.Image = ProductImageHelper.ResizeImage(primaryImage, 155, 110);
                }
                else
                {
                    // Use default brand image
                    var defaultImage = ProductImageHelper.GetDefaultBrandImage();
                    if (defaultImage != null)
                    {
                        imageBox.Image = ProductImageHelper.ResizeImage(defaultImage, 155, 110);
                    }
                    else
                    {
                        // Fallback icon with better styling
                        var defaultIcon = new IconPictureBox
                        {
                            IconChar = IconChar.Box,
                            IconColor = Color.FromArgb(156, 163, 175),
                            IconSize = 48,
                            Location = new Point(54, 31),
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
                    imageBox.Image = ProductImageHelper.ResizeImage(defaultImage, 155, 110);
                }
                else
                {
                    // Default icon on error with better styling
                    var defaultIcon = new IconPictureBox
                    {
                        IconChar = IconChar.Box,
                        IconColor = Color.FromArgb(156, 163, 175),
                        IconSize = 48,
                        Location = new Point(54, 31),
                        Size = new Size(48, 48),
                        BackColor = Color.Transparent,
                        Parent = imageBox
                    };
                }
            }

            card.Controls.Add(imageBox);

            // Category badge
            if (product.Category != null)
            {
                var categoryBadge = new Label
                {
                    Text = product.Category.Name,
                    Font = new Font("Segoe UI", 7F),
                    ForeColor = Color.FromArgb(100, 116, 139),
                    BackColor = Color.FromArgb(241, 245, 249),
                    Location = new Point(15, 145),
                    Size = new Size(155, 16),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                categoryBadge.Paint += (s, e) =>
                {
                    using (var path = GetRoundedRectPath(new Rectangle(0, 0, categoryBadge.Width - 1, categoryBadge.Height - 1), 8))
                    using (var brush = new SolidBrush(Color.FromArgb(241, 245, 249)))
                    {
                        e.Graphics.FillPath(brush, path);
                    }
                };
                card.Controls.Add(categoryBadge);
            }

            // Product name
            var nameLabel = new Label
            {
                Text = product.Name.Length > 25 ? product.Name.Substring(0, 22) + "..." : product.Name,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(10, 165),
                Size = new Size(165, 35),
                TextAlign = ContentAlignment.TopCenter,
                BackColor = Color.Transparent
            };
            card.Controls.Add(nameLabel);

            // Price with better styling
            var pricePanel = new Panel
            {
                Location = new Point(15, 200),
                Size = new Size(155, 30),
                BackColor = Color.FromArgb(34, 197, 94)
            };
            pricePanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, pricePanel.Width - 1, pricePanel.Height - 1), 6))
                using (var brush = new SolidBrush(Color.FromArgb(34, 197, 94)))
                {
                    e.Graphics.FillPath(brush, path);
                }
            };

            var priceLabel = new Label
            {
                Text = $"${product.SellingPrice:N2}",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(0, 0),
                Size = new Size(155, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent,
                Parent = pricePanel
            };

            card.Controls.Add(pricePanel);

            // Stock indicator with icon
            var stockPanel = new Panel
            {
                Location = new Point(140, 175),
                Size = new Size(35, 20),
                BackColor = Color.Transparent
            };

            var stockColor = product.Quantity > product.MinQuantity ? Color.FromArgb(34, 197, 94) :
                            product.Quantity > 0 ? Color.FromArgb(249, 115, 22) : Color.FromArgb(239, 68, 68);

            var stockIcon = new IconPictureBox
            {
                IconChar = product.Quantity > 0 ? IconChar.Check : IconChar.ExclamationTriangle,
                IconColor = stockColor,
                IconSize = 12,
                Location = new Point(0, 4),
                Size = new Size(12, 12),
                BackColor = Color.Transparent
            };
            stockPanel.Controls.Add(stockIcon);

            var stockLabel = new Label
            {
                Text = product.Quantity.ToString(),
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = stockColor,
                Location = new Point(15, 2),
                Size = new Size(20, 16),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            stockPanel.Controls.Add(stockLabel);

            card.Controls.Add(stockPanel);

            // Add to cart button overlay (appears on hover)
            var addButton = new Button
            {
                Text = "ADD TO CART",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(59, 130, 246),
                Location = new Point(25, 85),
                Size = new Size(135, 35),
                FlatStyle = FlatStyle.Flat,
                Visible = false,
                Cursor = Cursors.Hand
            };
            addButton.FlatAppearance.BorderSize = 0;
            addButton.Click += (s, e) => AddToCart(product);
            card.Controls.Add(addButton);

            // Click events
            card.Click += (s, e) => AddToCart(product);
            imageBox.Click += (s, e) => AddToCart(product);
            nameLabel.Click += (s, e) => AddToCart(product);
            pricePanel.Click += (s, e) => AddToCart(product);

            // Enhanced hover effects
            card.MouseEnter += (s, e) => 
            {
                card.BackColor = Color.FromArgb(248, 250, 252);
                addButton.Visible = true;
                imageBox.BackColor = Color.FromArgb(239, 246, 255);
            };
            card.MouseLeave += (s, e) => 
            {
                card.BackColor = Color.White;
                addButton.Visible = false;
                imageBox.BackColor = Color.FromArgb(248, 250, 252);
            };

            // Disable card if out of stock
            if (product.Quantity <= 0)
            {
                card.Enabled = false;
                card.BackColor = Color.FromArgb(250, 250, 250);
                nameLabel.ForeColor = Color.FromArgb(156, 163, 175);
                pricePanel.BackColor = Color.FromArgb(156, 163, 175);
                addButton.Text = "OUT OF STOCK";
                addButton.BackColor = Color.FromArgb(239, 68, 68);
                addButton.Visible = true;
                addButton.Enabled = false;
            }

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
            
            // Update cart item count in header
            var itemCount = _cartGrid.Rows.Count;
            var totalItems = 0;
            foreach (DataGridViewRow row in _cartGrid.Rows)
            {
                if (row.Cells["Qty"].Value != null)
                {
                    totalItems += Convert.ToInt32(row.Cells["Qty"].Value);
                }
            }
            
            // Find and update the item count label
            foreach (Control control in _rightPanel.Controls)
            {
                if (control is Panel panel && panel.Location.Y == 20)
                {
                    foreach (Control subControl in panel.Controls)
                    {
                        if (subControl is Label label && label.Text.Contains("items"))
                        {
                            label.Text = $"({totalItems} items)";
                            break;
                        }
                    }
                    break;
                }
            }
        }

        // New Event Handlers
        private void ModernCashierView_KeyDown(object? sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F9:
                    if (_clearButton.Enabled)
                        ClearButton_Click(sender, e);
                    break;
                case Keys.F10:
                    if (_holdButton.Enabled)
                        HoldButton_Click(sender, e);
                    break;
                case Keys.F12:
                    if (_completeButton.Enabled)
                        CompleteButton_Click(sender, e);
                    break;
                case Keys.F1:
                    _searchBox.Focus();
                    break;
                case Keys.F2:
                    ScanButton_Click(sender, e);
                    break;
                case Keys.Escape:
                    _searchBox.Clear();
                    _searchBox.Focus();
                    break;
            }
        }

        private void CustomerSearchButton_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("Customer Search - Coming Soon!\n\nThis will allow you to search and select existing customers from the database.",
                "Customer Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void NewCustomerButton_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("New Customer - Coming Soon!\n\nThis will allow you to quickly add a new customer to the database.",
                "New Customer", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void QuickCashButton_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is string amount)
            {
                _cashAmountBox.Text = amount;
                UpdateTotals();
            }
        }

        // Event Handlers
        private void SearchBox_TextChanged(object? sender, EventArgs e)
        {
            LoadProducts();
        }

        private void CategoryFilter_SelectedIndexChanged(object? sender, EventArgs e)
        {
            LoadProducts();
        }

        private void PaymentMethod_CheckedChanged(object? sender, EventArgs e)
        {
            _cashAmountBox.Visible = _cashRadio.Checked;
            _changeLabel.Visible = _cashRadio.Checked;
            UpdateTotals();
        }

        private void CashAmount_TextChanged(object? sender, EventArgs e)
        {
            UpdateTotals();
        }

        private async void CartGrid_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
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

        private void CartGrid_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == _cartGrid.Columns["Remove"].Index)
            {
                _cartGrid.Rows.RemoveAt(e.RowIndex);
                UpdateTotals();
            }
        }

        private void ScanButton_Click(object? sender, EventArgs e)
        {
            // Simulate barcode scanner with simple input form
            using (var inputForm = new Form())
            {
                inputForm.Text = "Barcode Scanner";
                inputForm.Size = new Size(350, 150);
                inputForm.StartPosition = FormStartPosition.CenterParent;
                inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                inputForm.MaximizeBox = false;
                inputForm.MinimizeBox = false;

                var label = new Label
                {
                    Text = "Enter barcode or product code:",
                    Location = new Point(20, 20),
                    Size = new Size(300, 20)
                };
                inputForm.Controls.Add(label);

                var textBox = new TextBox
                {
                    Location = new Point(20, 45),
                    Size = new Size(300, 25),
                    Font = new Font("Segoe UI", 10F)
                };
                inputForm.Controls.Add(textBox);

                var okButton = new Button
                {
                    Text = "Scan",
                    Location = new Point(165, 80),
                    Size = new Size(75, 30),
                    DialogResult = DialogResult.OK,
                    BackColor = Color.FromArgb(34, 197, 94),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                okButton.FlatAppearance.BorderSize = 0;
                inputForm.Controls.Add(okButton);

                var cancelButton = new Button
                {
                    Text = "Cancel",
                    Location = new Point(245, 80),
                    Size = new Size(75, 30),
                    DialogResult = DialogResult.Cancel,
                    BackColor = Color.FromArgb(156, 163, 175),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                cancelButton.FlatAppearance.BorderSize = 0;
                inputForm.Controls.Add(cancelButton);

                inputForm.AcceptButton = okButton;
                inputForm.CancelButton = cancelButton;
                textBox.Focus();

                if (inputForm.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(textBox.Text))
                {
                    SearchProductByCode(textBox.Text.Trim());
                }
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

        private async void CompleteButton_Click(object? sender, EventArgs e)
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
            
            // Update daily sales status
            _ = UpdateDailySalesStatus();
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

        private void ClearButton_Click(object? sender, EventArgs e)
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

        private void HoldButton_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("Hold functionality - Coming Soon!\n\nThis will allow you to save the current cart and resume later.",
                "Hold Sale", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void TaxSettingsButton_Click(object? sender, EventArgs e)
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