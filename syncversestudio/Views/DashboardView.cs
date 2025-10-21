using System.Drawing;
using SyncVerseStudio.Services;
using SyncVerseStudio.Data;
using Microsoft.EntityFrameworkCore;
using FontAwesome.Sharp;

namespace SyncVerseStudio.Views
{
    public partial class DashboardView : Form
    {
        private readonly AuthenticationService _authService;
        private ApplicationDbContext? _context;
        private readonly Dictionary<string, Label> _cardValueLabels = new Dictionary<string, Label>();
        private bool _isLoading = false;
        private System.Windows.Forms.Timer _refreshTimer;
        private FlowLayoutPanel _cardsContainer;

        public DashboardView(AuthenticationService authService)
        {
            _authService = authService;
            InitializeComponent();
            this.Load += DashboardView_Load;

            _refreshTimer = new System.Windows.Forms.Timer
            {
                Interval = 15000
            };
            _refreshTimer.Tick += async (s, e) => await RefreshLiveDataAsync();
        }

        private async void DashboardView_Load(object sender, EventArgs e)
        {
            try
            {
                _context = new ApplicationDbContext();
                await CreateModernDashboardLayout();
                await LoadDashboardDataAsync();
                _refreshTimer.Start();
            }
            catch (Exception ex)
            {
                ShowError($"Error initializing dashboard: {ex.Message}");
            }
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(248, 250, 252);
            ClientSize = new Size(1400, 900);
            FormBorderStyle = FormBorderStyle.None;
            Name = "DashboardView";
            Text = "Dashboard";
            ResumeLayout(false);
        }

        private async Task CreateModernDashboardLayout()
        {
            this.Controls.Clear();

            var mainContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 250, 252),
                AutoScroll = true,
                Padding = new Padding(30, 25, 30, 30)
            };

            await CreateDashboardHeader(mainContainer);
            CreateLiveMetricsSection(mainContainer);

            this.Controls.Add(mainContainer);
        }

        private async Task CreateDashboardHeader(Panel container)
        {
            var user = _authService.CurrentUser;
            if (user == null) return;

            var headerPanel = new Panel
            {
                BackColor = Color.White,
                Location = new Point(0, 0),
                Size = new Size(container.Width - 60, 120),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Padding = new Padding(30, 20, 30, 20)
            };

            headerPanel.Paint += (s, e) =>
            {
                var rect = headerPanel.ClientRectangle;
                using (var shadowBrush = new SolidBrush(Color.FromArgb(15, 0, 0, 0)))
                {
                    e.Graphics.FillRectangle(shadowBrush, new Rectangle(2, 2, rect.Width, rect.Height));
                }
                using (var brush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }
            };

            // Welcome message
            var welcomeLabel = new Label
            {
                Text = $"Welcome back, {user.FirstName}!",
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(30, 30),
                Size = new Size(600, 38)
            };
            headerPanel.Controls.Add(welcomeLabel);

            // Time status indicator
            var statusPanel = new Panel
            {
                Size = new Size(200, 32),
                Location = new Point(headerPanel.Width - 230, 35),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Color.FromArgb(34, 197, 94)
            };

            var statusLabel = new Label
            {
                Text = DateTime.Now.ToString("HH:mm:ss"),
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(15, 6),
                Size = new Size(170, 20),
                TextAlign = ContentAlignment.MiddleLeft,
                Name = "LiveStatusLabel"
            };
            statusPanel.Controls.Add(statusLabel);
            headerPanel.Controls.Add(statusPanel);

            container.Controls.Add(headerPanel);
        }

        private void CreateLiveMetricsSection(Panel container)
        {
            var metricsTitle = new Label
            {
                Text = "Live System Metrics",
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(0, 140),
                AutoSize = true
            };
            container.Controls.Add(metricsTitle);

            _cardsContainer = new FlowLayoutPanel
            {
                Location = new Point(0, 180),
                Size = new Size(container.Width - 60, 400),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.Transparent,
                WrapContents = true,
                AutoSize = false
            };

            CreateLiveMetricCards();
            container.Controls.Add(_cardsContainer);
        }

        private void CreateLiveMetricCards()
        {
            var user = _authService.CurrentUser;
            if (user == null) return;

            int cardWidth = 280;
            int cardHeight = 140;
            int cardMargin = 15;

            _cardsContainer.Controls.Clear();
            _cardValueLabels.Clear();

            switch (user.Role)
            {
                case Models.UserRole.Administrator:
                    CreateLiveCard("Products", "0", IconChar.Box, Color.FromArgb(59, 130, 246), cardWidth, cardHeight, cardMargin);
                    CreateLiveCard("Categories", "0", IconChar.Tags, Color.FromArgb(34, 197, 94), cardWidth, cardHeight, cardMargin);
                    CreateLiveCard("Customers", "0", IconChar.Users, Color.FromArgb(168, 85, 247), cardWidth, cardHeight, cardMargin);
                    CreateLiveCard("Sales Today", "$0.00", IconChar.DollarSign, Color.FromArgb(239, 68, 68), cardWidth, cardHeight, cardMargin);
                    CreateLiveCard("Suppliers", "0", IconChar.Truck, Color.FromArgb(245, 158, 11), cardWidth, cardHeight, cardMargin);
                    CreateLiveCard("Users", "0", IconChar.UserTie, Color.FromArgb(156, 163, 175), cardWidth, cardHeight, cardMargin);
                    break;

                case Models.UserRole.Cashier:
                    CreateLiveCard("Today's Sales", "$0.00", IconChar.DollarSign, Color.FromArgb(34, 197, 94), cardWidth, cardHeight, cardMargin);
                    CreateLiveCard("Transactions", "0", IconChar.Receipt, Color.FromArgb(59, 130, 246), cardWidth, cardHeight, cardMargin);
                    CreateLiveCard("Customers", "0", IconChar.Users, Color.FromArgb(168, 85, 247), cardWidth, cardHeight, cardMargin);
                    CreateLiveCard("Average Sale", "$0.00", IconChar.ChartLine, Color.FromArgb(245, 158, 11), cardWidth, cardHeight, cardMargin);
                    break;

                case Models.UserRole.InventoryClerk:
                    CreateLiveCard("Products", "0", IconChar.Box, Color.FromArgb(59, 130, 246), cardWidth, cardHeight, cardMargin);
                    CreateLiveCard("Low Stock", "0", IconChar.ExclamationTriangle, Color.FromArgb(239, 68, 68), cardWidth, cardHeight, cardMargin);
                    CreateLiveCard("Categories", "0", IconChar.Tags, Color.FromArgb(168, 85, 247), cardWidth, cardHeight, cardMargin);
                    CreateLiveCard("Stock Value", "$0.00", IconChar.Gem, Color.FromArgb(34, 197, 94), cardWidth, cardHeight, cardMargin);
                    break;
            }
        }

        private void CreateLiveCard(string title, string value, IconChar icon, Color bgColor, int width, int height, int margin)
        {
            var card = new Panel
            {
                Size = new Size(width, height),
                Margin = new Padding(margin / 2),
                BackColor = bgColor,
                Cursor = Cursors.Hand
            };

            card.Paint += (s, e) =>
            {
                var rect = card.ClientRectangle;
                using (var shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
                {
                    e.Graphics.FillRectangle(shadowBrush, new Rectangle(4, 4, rect.Width, rect.Height));
                }
                using (var brush = new SolidBrush(bgColor))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }
            };

            var iconPic = new IconPictureBox
            {
                IconChar = icon,
                IconColor = Color.White,
                IconSize = 48,
                Location = new Point(25, 25),
                Size = new Size(60, 60),
                BackColor = Color.Transparent
            };
            card.Controls.Add(iconPic);

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(100, 30),
                Size = new Size(width - 120, 25)
            };
            card.Controls.Add(titleLabel);

            var valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(100, 55),
                Size = new Size(width - 120, 35)
            };
            card.Controls.Add(valueLabel);
            _cardValueLabels[title] = valueLabel;

            _cardsContainer.Controls.Add(card);
        }

        private async Task LoadDashboardDataAsync()
        {
            if (_isLoading || _context == null) return;

            try
            {
                _isLoading = true;
                var user = _authService.CurrentUser;
                if (user == null) return;

                await LoadLiveMetrics(user.Role);
                UpdateLiveStatus();
            }
            catch (Exception ex)
            {
                ShowError($"Error loading dashboard data: {ex.Message}");
            }
            finally
            {
                _isLoading = false;
            }
        }

        private async Task RefreshLiveDataAsync()
        {
            await LoadDashboardDataAsync();
        }

        private async Task LoadLiveMetrics(Models.UserRole role)
        {
            if (_context == null) return;

            try
            {
                switch (role)
                {
                    case Models.UserRole.Administrator:
                        var products = await _context.Products.CountAsync(p => p.IsActive);
                        var categories = await _context.Categories.CountAsync(c => c.IsActive);
                        var customers = await _context.Customers.CountAsync();
                        var todaySales = await _context.Sales
                            .Where(s => s.SaleDate.Date == DateTime.Today && s.Status == Models.SaleStatus.Completed)
                            .SumAsync(s => (decimal?)s.TotalAmount) ?? 0;
                        var suppliers = await _context.Suppliers.CountAsync(s => s.IsActive);
                        var users = await _context.Users.CountAsync(u => u.IsActive);

                        this.Invoke(new Action(() =>
                        {
                            UpdateCardValue("Products", products.ToString("N0"));
                            UpdateCardValue("Categories", categories.ToString("N0"));
                            UpdateCardValue("Customers", customers.ToString("N0"));
                            UpdateCardValue("Sales Today", $"${todaySales:N2}");
                            UpdateCardValue("Suppliers", suppliers.ToString("N0"));
                            UpdateCardValue("Users", users.ToString("N0"));
                        }));
                        break;

                    case Models.UserRole.Cashier:
                        var cashierSales = await _context.Sales
                            .Where(s => s.SaleDate.Date == DateTime.Today &&
                                      s.CashierId == _authService.CurrentUser!.Id &&
                                      s.Status == Models.SaleStatus.Completed)
                            .SumAsync(s => (decimal?)s.TotalAmount) ?? 0;

                        var transactions = await _context.Sales
                            .CountAsync(s => s.SaleDate.Date == DateTime.Today &&
                                       s.CashierId == _authService.CurrentUser!.Id &&
                                       s.Status == Models.SaleStatus.Completed);

                        var avgSale = transactions > 0 ? cashierSales / transactions : 0;

                        this.Invoke(new Action(() =>
                        {
                            UpdateCardValue("Today's Sales", $"${cashierSales:N2}");
                            UpdateCardValue("Transactions", transactions.ToString("N0"));
                            UpdateCardValue("Average Sale", $"${avgSale:N2}");
                        }));
                        break;

                    case Models.UserRole.InventoryClerk:
                        var totalProducts = await _context.Products.CountAsync(p => p.IsActive);
                        var lowStock = await _context.Products
                            .CountAsync(p => p.IsActive && p.Quantity <= p.MinQuantity);
                        var stockValue = await _context.Products
                            .Where(p => p.IsActive)
                            .SumAsync(p => p.CostPrice * p.Quantity);

                        this.Invoke(new Action(() =>
                        {
                            UpdateCardValue("Products", totalProducts.ToString("N0"));
                            UpdateCardValue("Low Stock", lowStock.ToString("N0"));
                            UpdateCardValue("Stock Value", $"${stockValue:N2}");
                        }));
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error loading metrics: {ex.Message}");
            }
        }

        private void UpdateCardValue(string title, string value)
        {
            if (_cardValueLabels.TryGetValue(title, out var label))
            {
                label.Text = value;
            }
        }

        private void UpdateLiveStatus()
        {
            this.Invoke(new Action(() =>
            {
                var statusLabel = this.Controls.Find("LiveStatusLabel", true).FirstOrDefault() as Label;
                if (statusLabel != null)
                {
                    statusLabel.Text = DateTime.Now.ToString("HH:mm:ss");
                }
            }));
        }

        private void ShowError(string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => ShowError(message)));
                return;
            }
            MessageBox.Show(message, "Dashboard Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _refreshTimer?.Stop();
                _refreshTimer?.Dispose();
                _context?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}