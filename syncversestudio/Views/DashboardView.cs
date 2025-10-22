using System.Drawing;
using System.Drawing.Drawing2D;
using SyncVerseStudio.Services;
using SyncVerseStudio.Data;
using Microsoft.EntityFrameworkCore;
using FontAwesome.Sharp;
using SyncVerseStudio.Helpers;

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
            BackColor = BrandTheme.Background;
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
                BackColor = BrandTheme.Background,
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
                BackColor = BrandTheme.CoolWhite,
                Location = new Point(0, 0),
                Size = new Size(container.Width - 60, 100),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Padding = new Padding(30, 20, 30, 20)
            };

            // Logo
            var logoPic = new PictureBox
            {
                Size = new Size(60, 60),
                Location = new Point(30, 30),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };

            try
            {
                var logoPath = Path.Combine(Application.StartupPath, "..", "..", "..", BrandTheme.LogoPath);
                if (File.Exists(logoPath))
                {
                    logoPic.Image = Image.FromFile(logoPath);
                }
                else
                {
                    // Fallback icon
                    var fallbackIcon = new IconPictureBox
                    {
                        IconChar = IconChar.Store,
                        IconColor = BrandTheme.LimeGreen,
                        IconSize = 50,
                        Location = new Point(30, 30),
                        Size = new Size(60, 60),
                        BackColor = Color.Transparent
                    };
                    headerPanel.Controls.Add(fallbackIcon);
                }
            }
            catch
            {
                var fallbackIcon = new IconPictureBox
                {
                    IconChar = IconChar.Store,
                    IconColor = BrandTheme.LimeGreen,
                    IconSize = 50,
                    Location = new Point(30, 30),
                    Size = new Size(60, 60),
                    BackColor = Color.Transparent
                };
                headerPanel.Controls.Add(fallbackIcon);
            }

            if (logoPic.Image != null)
            {
                headerPanel.Controls.Add(logoPic);
            }

            // Welcome message
            var welcomeLabel = new Label
            {
                Text = $"Welcome, {user.FirstName}!",
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(110, 25),
                Size = new Size(600, 38),
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(welcomeLabel);

            var subtitleLabel = new Label
            {
                Text = "Administrator Dashboard - System",
                Font = new Font("Segoe UI", 11F),
                ForeColor = BrandTheme.CoolWhite,
                Location = new Point(110, 65),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(subtitleLabel);

            // Time status indicator with rounded corners
            var statusPanel = new Panel
            {
                Size = new Size(180, 50),
                Location = new Point(headerPanel.Width - 210, 35),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = BrandTheme.LimeGreen
            };

            statusPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                var rect = statusPanel.ClientRectangle;
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, rect.Width - 1, rect.Height - 1), 15))
                using (var brush = new SolidBrush(BrandTheme.LimeGreen))
                {
                    e.Graphics.FillPath(brush, path);
                }
            };

            var statusIcon = new IconPictureBox
            {
                IconChar = IconChar.Clock,
                IconColor = BrandTheme.Charcoal,
                IconSize = 24,
                Location = new Point(15, 13),
                Size = new Size(24, 24),
                BackColor = Color.Transparent
            };
            statusPanel.Controls.Add(statusIcon);

            var statusLabel = new Label
            {
                Text = DateTime.Now.ToString("HH:mm:ss"),
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = BrandTheme.Charcoal,
                Location = new Point(45, 13),
                Size = new Size(125, 24),
                TextAlign = ContentAlignment.MiddleLeft,
                Name = "LiveStatusLabel",
                BackColor = Color.Transparent
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
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.Black,
                Location = new Point(0, 120),
                AutoSize = true
            };
            container.Controls.Add(metricsTitle);

            var metricsSubtitle = new Label
            {
                Text = "Real-time data updates every 15 seconds",
                Font = new Font("Segoe UI", 9F),
                ForeColor = BrandTheme.Charcoal,
                Location = new Point(0, 145),
                AutoSize = true
            };
            container.Controls.Add(metricsSubtitle);

            _cardsContainer = new FlowLayoutPanel
            {
                Location = new Point(0, 175),
                Size = new Size(container.Width - 60, 600),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                BackColor = Color.Transparent,
                WrapContents = true,
                AutoSize = false,
                AutoScroll = false
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
                    CreateLiveCard("Products", "0", IconChar.Box, BrandTheme.OceanBlue, cardWidth, cardHeight, cardMargin);
                    CreateLiveCard("Categories", "0", IconChar.Tags, BrandTheme.OceanBlue, cardWidth, cardHeight, cardMargin);
                    CreateLiveCard("Customers", "0", IconChar.Users, BrandTheme.OceanBlue, cardWidth, cardHeight, cardMargin);
                    CreateLiveCard("Sales Today", "$0.00", IconChar.DollarSign, BrandTheme.OceanBlue, cardWidth, cardHeight, cardMargin);
                    CreateLiveCard("Suppliers", "0", IconChar.Truck, BrandTheme.OceanBlue, cardWidth, cardHeight, cardMargin);
                    CreateLiveCard("Users", "0", IconChar.UserTie, BrandTheme.OceanBlue, cardWidth, cardHeight, cardMargin);
                    break;

                case Models.UserRole.Cashier:
                    CreateLiveCard("Today's Sales", "$0.00", IconChar.DollarSign, BrandTheme.OceanBlue, cardWidth, cardHeight, cardMargin);
                    CreateLiveCard("Transactions", "0", IconChar.Receipt, BrandTheme.OceanBlue, cardWidth, cardHeight, cardMargin);
                    CreateLiveCard("Customers", "0", IconChar.Users, BrandTheme.OceanBlue, cardWidth, cardHeight, cardMargin);
                    CreateLiveCard("Average Sale", "$0.00", IconChar.ChartLine, BrandTheme.OceanBlue, cardWidth, cardHeight, cardMargin);
                    break;

                case Models.UserRole.InventoryClerk:
                    CreateLiveCard("Products", "0", IconChar.Box, BrandTheme.OceanBlue, cardWidth, cardHeight, cardMargin);
                    CreateLiveCard("Low Stock", "0", IconChar.ExclamationTriangle, BrandTheme.OceanBlue, cardWidth, cardHeight, cardMargin);
                    CreateLiveCard("Categories", "0", IconChar.Tags, BrandTheme.OceanBlue, cardWidth, cardHeight, cardMargin);
                    CreateLiveCard("Stock Value", "$0.00", IconChar.Gem, BrandTheme.OceanBlue, cardWidth, cardHeight, cardMargin);
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
                Cursor = Cursors.Hand,
                Tag = bgColor
            };

            // Rounded corners with shadow
            card.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                var rect = card.ClientRectangle;
                int radius = 20;

                // Shadow
                using (var shadowPath = GetRoundedRectPath(new Rectangle(6, 6, rect.Width - 2, rect.Height - 2), radius))
                using (var shadowBrush = new SolidBrush(Color.FromArgb(40, 0, 0, 0)))
                {
                    e.Graphics.FillPath(shadowBrush, shadowPath);
                }

                // Card background
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, rect.Width - 1, rect.Height - 1), radius))
                using (var brush = new SolidBrush(bgColor))
                {
                    e.Graphics.FillPath(brush, path);
                }
            };

            // Icon with circular background
            var iconContainer = new Panel
            {
                Size = new Size(70, 70),
                Location = new Point(20, 20),
                BackColor = Color.FromArgb(50, 255, 255, 255)
            };
            iconContainer.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var brush = new SolidBrush(Color.FromArgb(50, 255, 255, 255)))
                {
                    e.Graphics.FillEllipse(brush, 0, 0, 70, 70);
                }
            };

            var iconPic = new IconPictureBox
            {
                IconChar = icon,
                IconColor = Color.White,
                IconSize = 40,
                Location = new Point(15, 15),
                Size = new Size(40, 40),
                BackColor = Color.Transparent
            };
            iconContainer.Controls.Add(iconPic);
            card.Controls.Add(iconContainer);

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(250, 250, 250),
                Location = new Point(20, 100),
                Size = new Size(width - 40, 22),
                BackColor = Color.Transparent
            };
            card.Controls.Add(titleLabel);

            var valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 55),
                Size = new Size(width - 110, 40),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleRight
            };
            card.Controls.Add(valueLabel);
            _cardValueLabels[title] = valueLabel;

            // Hover animation
            card.MouseEnter += (s, e) =>
            {
                card.Size = new Size(width + 5, height + 5);
                card.BackColor = LightenColor(bgColor, 20);
            };
            card.MouseLeave += (s, e) =>
            {
                card.Size = new Size(width, height);
                card.BackColor = bgColor;
            };

            _cardsContainer.Controls.Add(card);
        }

        private System.Drawing.Drawing2D.GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
        }

        private Color LightenColor(Color color, int amount)
        {
            return Color.FromArgb(
                Math.Min(255, color.R + amount),
                Math.Min(255, color.G + amount),
                Math.Min(255, color.B + amount)
            );
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