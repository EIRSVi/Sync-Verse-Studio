using System.Drawing;
using System.Drawing.Drawing2D;
using SyncVerseStudio.Services;
using SyncVerseStudio.Data;
using Microsoft.EntityFrameworkCore;
using FontAwesome.Sharp;
using SyncVerseStudio.Helpers;
using System.Globalization;

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
        private List<CategoryData> _categoryData = new List<CategoryData>();
        
        private class CategoryData
        {
            public string Name { get; set; } = "";
            public int Count { get; set; }
            public Color Color { get; set; }
        }

        public DashboardView(AuthenticationService authService)
        {
            _authService = authService;
            InitializeComponent();
            this.Load += DashboardView_Load;

            _refreshTimer = new System.Windows.Forms.Timer
            {
                Interval = 5000 // Update every 5 seconds - reduced frequency for stability
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
            BackColor = Color.White; // Clean white background
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
                BackColor = Color.White, // Clean white background
                AutoScroll = true,
                Padding = new Padding(40, 30, 40, 30) // Proper padding to show all content
            };

            await CreateDashboardHeader(mainContainer);
            CreateLiveMetricsSection(mainContainer);

            this.Controls.Add(mainContainer);
        }

        private async Task CreateDashboardHeader(Panel container)
        {
            var user = _authService.CurrentUser;
            if (user == null) return;

            // Clean header panel - no background, minimal design
            var headerPanel = new Panel
            {
                BackColor = Color.Transparent,
                Location = new Point(40, 10),
                Size = new Size(container.Width - 120, 90),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Padding = new Padding(0)
            };

            // Page title - modern clean design with Khmer support
            var titleLabel = new Label
            {
                Text = "Dashboard",
                Font = GetKhmerFont(28F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
               Location = new Point(0, 0),  // 20 pixels from top
                Size = new Size(800, 80),
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(titleLabel);

            // Subtitle with last update time and Khmer support
            var subtitleLabel = new Label
            {
                // Text = $"Welcome back, {user.FirstName}! Here's what's happening today.",
                Font = GetKhmerFont(11F, FontStyle.Regular),
                ForeColor = Color.FromArgb(200, 200, 100),
                Location = new Point(0, 45),
                Size = new Size(800, 25),
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(subtitleLabel);

            container.Controls.Add(headerPanel);
        }

        private void CreateLiveMetricsSection(Panel container)
        {
            // Center container for all content
            var centerContainer = new Panel
            {
                Size = new Size(1320, 1000),
                Location = new Point((container.Width - 1320) / 2, 110),
                Anchor = AnchorStyles.Top,
                BackColor = Color.Transparent
            };

            // Metrics cards container - two column layout
            _cardsContainer = new FlowLayoutPanel
            {
                Location = new Point(0, 0),
                Size = new Size(700, 400),
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                BackColor = Color.Transparent,
                WrapContents = true,
                AutoSize = false,
                AutoScroll = false
            };

            CreateLiveMetricCards();
            centerContainer.Controls.Add(_cardsContainer);

            // Pie chart panel on the right
            var chartPanel = new Panel
            {
                Location = new Point(720, 0),
                Size = new Size(600, 380),
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                BackColor = Color.White,
                Name = "ChartPanel"
            };

            chartPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var rect = chartPanel.ClientRectangle;
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, rect.Width - 1, rect.Height - 1), 20))
                using (var brush = new SolidBrush(Color.White))
                using (var pen = new Pen(Color.FromArgb(230, 230, 230), 2))
                {
                    e.Graphics.FillPath(brush, path);
                    e.Graphics.DrawPath(pen, path);
                }
            };

            // Chart title with Khmer support
            var chartTitle = new Label
            {
                Text = "Product Distribution",
                Font = GetKhmerFont(18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(30, 25),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            chartPanel.Controls.Add(chartTitle);

            var chartSubtitle = new Label
            {
                Text = "Live inventory breakdown by category",
                Font = GetKhmerFont(10F, FontStyle.Regular),
                ForeColor = Color.FromArgb(120, 120, 120),
                Location = new Point(30, 55),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            chartPanel.Controls.Add(chartSubtitle);

            // Pie chart drawing area - no hover effects
            var pieChartPanel = new Panel
            {
                Location = new Point(30, 90),
                Size = new Size(540, 270),
                BackColor = Color.Transparent,
                Name = "PieChartDrawing"
            };
            pieChartPanel.Paint += DrawPieChart;
            chartPanel.Controls.Add(pieChartPanel);

            centerContainer.Controls.Add(chartPanel);
            container.Controls.Add(centerContainer);
        }

        private void CreateLiveMetricCards()
        {
            var user = _authService.CurrentUser;
            if (user == null) return;

            int cardWidth = 320;
            int cardHeight = 130;
            int cardMargin = 15;

            _cardsContainer.Controls.Clear();
            _cardValueLabels.Clear();

            switch (user.Role)
            {
                case Models.UserRole.Administrator:
                    CreateLiveCard("Total Products", "0", IconChar.Box, Color.FromArgb(99, 102, 241), cardWidth, cardHeight, cardMargin); // Indigo
                    CreateLiveCard("Categories", "0", IconChar.Tags, Color.FromArgb(236, 72, 153), cardWidth, cardHeight, cardMargin); // Pink
                    CreateLiveCard("Customers", "0", IconChar.Users, Color.FromArgb(34, 197, 94), cardWidth, cardHeight, cardMargin); // Green
                    CreateLiveCard("Sales Today", "$0.00", IconChar.DollarSign, Color.FromArgb(249, 115, 22), cardWidth, cardHeight, cardMargin); // Orange
                    CreateLiveCard("Suppliers", "0", IconChar.Truck, Color.FromArgb(168, 85, 247), cardWidth, cardHeight, cardMargin); // Purple
                    CreateLiveCard("Active Users", "0", IconChar.UserTie, Color.FromArgb(14, 165, 233), cardWidth, cardHeight, cardMargin); // Sky blue
                    break;

                case Models.UserRole.Cashier:
                    CreateLiveCard("Today's Sales", "$0.00", IconChar.DollarSign, Color.FromArgb(34, 197, 94), cardWidth, cardHeight, cardMargin);
                    CreateLiveCard("Transactions", "0", IconChar.Receipt, Color.FromArgb(99, 102, 241), cardWidth, cardHeight, cardMargin);
                    CreateLiveCard("Customers", "0", IconChar.Users, Color.FromArgb(236, 72, 153), cardWidth, cardHeight, cardMargin);
                    CreateLiveCard("Average Sale", "$0.00", IconChar.ChartLine, Color.FromArgb(249, 115, 22), cardWidth, cardHeight, cardMargin);
                    break;

                case Models.UserRole.InventoryClerk:
                    CreateLiveCard("Products", "0", IconChar.Box, Color.FromArgb(99, 102, 241), cardWidth, cardHeight, cardMargin);
                    CreateLiveCard("Low Stock", "0", IconChar.ExclamationTriangle, Color.FromArgb(239, 68, 68), cardWidth, cardHeight, cardMargin);
                    CreateLiveCard("Categories", "0", IconChar.Tags, Color.FromArgb(236, 72, 153), cardWidth, cardHeight, cardMargin);
                    CreateLiveCard("Stock Value", "$0.00", IconChar.Gem, Color.FromArgb(34, 197, 94), cardWidth, cardHeight, cardMargin);
                    break;
            }
        }

        private void CreateLiveCard(string title, string value, IconChar icon, Color accentColor, int width, int height, int margin)
        {
            var card = new Panel
            {
                Size = new Size(width, height),
                Margin = new Padding(margin / 2),
                BackColor = accentColor,
                Cursor = Cursors.Default,
                Tag = accentColor
            };

            // Clean gradient card - no glitch effects
            card.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var rect = card.ClientRectangle;
                int radius = 16;

                // Subtle shadow
                using (var shadowPath = GetRoundedRectPath(new Rectangle(3, 3, rect.Width - 2, rect.Height - 2), radius))
                using (var shadowBrush = new SolidBrush(Color.FromArgb(25, 0, 0, 0)))
                {
                    e.Graphics.FillPath(shadowBrush, shadowPath);
                }

                // Clean gradient background
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, rect.Width - 1, rect.Height - 1), radius))
                {
                    // Create lighter version of accent color for gradient
                    Color lightColor = Color.FromArgb(
                        Math.Min(255, accentColor.R + 40),
                        Math.Min(255, accentColor.G + 40),
                        Math.Min(255, accentColor.B + 40)
                    );
                    
                    using (var gradientBrush = new LinearGradientBrush(rect, accentColor, lightColor, 135f))
                    {
                        e.Graphics.FillPath(gradientBrush, path);
                    }
                }
            };

            // Icon with white background circle
            var iconBg = new Panel
            {
                Size = new Size(60, 60),
                Location = new Point(20, 20),
                BackColor = Color.Transparent
            };
            iconBg.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var brush = new SolidBrush(Color.FromArgb(255, 255, 255, 255)))
                {
                    e.Graphics.FillEllipse(brush, 0, 0, 60, 60);
                }
            };
            card.Controls.Add(iconBg);

            var iconPic = new IconPictureBox
            {
                IconChar = icon,
                IconColor = accentColor,
                IconSize = 32,
                Location = new Point(14, 14),
                Size = new Size(32, 32),
                BackColor = Color.Transparent
            };
            iconBg.Controls.Add(iconPic);

            // Title - with Khmer font support
            var titleLabel = new Label
            {
                Text = title,
                Font = GetKhmerFont(10F, FontStyle.Regular),
                ForeColor = Color.FromArgb(255, 255, 255, 255),
                Location = new Point(95, 25),
                Size = new Size(width - 110, 20),
                BackColor = Color.Transparent
            };
            card.Controls.Add(titleLabel);

            // Value - large and bold with Khmer font support
            var valueLabel = new Label
            {
                Text = value,
                Font = GetKhmerFont(28F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(95, 45),
                Size = new Size(width - 110, 50),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft
            };
            card.Controls.Add(valueLabel);
            _cardValueLabels[title] = valueLabel;

            // No hover effects - clean and simple
            _cardsContainer.Controls.Add(card);
        }

        private Font GetKhmerFont(float size, FontStyle style)
        {
            // Use Segoe UI which supports both English and Khmer well
            try
            {
                return new Font("Segoe UI", size, style);
            }
            catch
            {
                return new Font(FontFamily.GenericSansSerif, size, style);
            }
        }

        private string GetEnglishDateTime()
        {
            var now = DateTime.Now;
            return $"{now:dddd, MMMM dd, yyyy - HH:mm:ss}";
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
                await LoadPieChartData();
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
                            UpdateCardValue("Total Products", products.ToString("N0"));
                            UpdateCardValue("Categories", categories.ToString("N0"));
                            UpdateCardValue("Customers", customers.ToString("N0"));
                            UpdateCardValue("Sales Today", $"${todaySales:N2}");
                            UpdateCardValue("Suppliers", suppliers.ToString("N0"));
                            UpdateCardValue("Active Users", users.ToString("N0"));
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
                if (label.Text != value)
                {
                    // Animate value change with color flash
                    var originalColor = label.ForeColor;
                    label.ForeColor = Color.FromArgb(255, 255, 100); // Flash yellow
                    label.Text = value;
                    
                    // Reset color after brief delay
                    var flashTimer = new System.Windows.Forms.Timer { Interval = 300 };
                    flashTimer.Tick += (s, e) =>
                    {
                        if (!label.IsDisposed)
                            label.ForeColor = originalColor;
                        flashTimer.Stop();
                        flashTimer.Dispose();
                    };
                    flashTimer.Start();
                }
            }
        }

        private void UpdateLiveStatus()
        {
            // Time section removed - no longer needed
        }

        private async Task LoadPieChartData()
        {
            if (_context == null) return;
            
            try
            {
                var categoryColors = new Dictionary<string, Color>
                {
                    { "Electronics", Color.FromArgb(99, 102, 241) },
                    { "Clothing", Color.FromArgb(236, 72, 153) },
                    { "Food", Color.FromArgb(34, 197, 94) },
                    { "Books", Color.FromArgb(249, 115, 22) },
                    { "Others", Color.FromArgb(168, 85, 247) }
                };
                
                var categoryData = await _context.Products
                    .Where(p => p.IsActive)
                    .GroupBy(p => p.Category.Name)
                    .Select(g => new { Name = g.Key, Count = g.Count() })
                    .ToListAsync();
                
                // Only update if data has changed
                bool dataChanged = false;
                if (_categoryData.Count != categoryData.Count)
                {
                    dataChanged = true;
                }
                else
                {
                    for (int i = 0; i < categoryData.Count; i++)
                    {
                        var existing = _categoryData.FirstOrDefault(c => c.Name == categoryData[i].Name);
                        if (existing == null || existing.Count != categoryData[i].Count)
                        {
                            dataChanged = true;
                            break;
                        }
                    }
                }
                
                if (dataChanged)
                {
                    _categoryData.Clear();
                    foreach (var cat in categoryData)
                    {
                        _categoryData.Add(new CategoryData
                        {
                            Name = cat.Name,
                            Count = cat.Count,
                            Color = categoryColors.ContainsKey(cat.Name) ? categoryColors[cat.Name] : Color.FromArgb(168, 85, 247)
                        });
                    }
                    
                    // Only refresh pie chart if data actually changed
                    var piePanel = this.Controls.Find("PieChartDrawing", true).FirstOrDefault();
                    piePanel?.Invalidate();
                }
            }
            catch { }
        }
        


        private void DrawPieChart(object sender, PaintEventArgs e)
        {
            if (_context == null) return;

            try
            {
                // High quality rendering - no animation
                e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                
                // Use real data from database
                if (_categoryData.Count == 0) return;

                int total = _categoryData.Sum(c => c.Count);
                if (total == 0) return;

                // Draw pie chart - stable positioning
                int centerX = 130;
                int centerY = 135;
                int radius = 100;

                float startAngle = 0;
                int legendY = 20;

                foreach (var category in _categoryData)
                {
                    float sweepAngle = (category.Count / (float)total) * 360;
                    
                    // Static pie chart - no animations or effects
                    Rectangle pieRect = new Rectangle(centerX - radius, centerY - radius, radius * 2, radius * 2);
                        
                    using (var brush = new SolidBrush(category.Color))
                    {
                        e.Graphics.FillPie(brush, pieRect, startAngle, sweepAngle);
                    }

                    // Draw white border between slices for clarity
                    using (var pen = new Pen(Color.White, 3))
                    {
                        e.Graphics.DrawPie(pen, pieRect, startAngle, sweepAngle);
                    }

                    // Draw legend
                    int legendX = 280;
                    
                    // Color box
                    using (var brush = new SolidBrush(category.Color))
                    {
                        e.Graphics.FillRectangle(brush, legendX, legendY, 16, 16);
                    }

                    // Category name
                    using (var font = GetKhmerFont(9.5F, FontStyle.Regular))
                    using (var brush = new SolidBrush(Color.FromArgb(60, 60, 60)))
                    {
                        e.Graphics.DrawString(category.Name, font, brush, legendX + 22, legendY - 2);
                    }

                    // Count and percentage
                    float percentage = (category.Count / (float)total) * 100;
                    using (var font = GetKhmerFont(9F, FontStyle.Bold))
                    using (var brush = new SolidBrush(Color.FromArgb(30, 30, 30)))
                    {
                        string text = $"{category.Count} ({percentage:F1}%)";
                        e.Graphics.DrawString(text, font, brush, legendX + 22, legendY + 14);
                    }

                    legendY += 50;
                    startAngle += sweepAngle;
                }

                // Draw center circle for donut effect
                int innerRadius = 40;
                Rectangle innerRect = new Rectangle(centerX - innerRadius, centerY - innerRadius, innerRadius * 2, innerRadius * 2);
                using (var brush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillEllipse(brush, innerRect);
                }

                // Draw total in center
                using (var font = GetKhmerFont(14F, FontStyle.Bold))
                using (var brush = new SolidBrush(Color.FromArgb(30, 30, 30)))
                {
                    string totalText = total.ToString();
                    SizeF textSize = e.Graphics.MeasureString(totalText, font);
                    e.Graphics.DrawString(totalText, font, brush, 
                        centerX - textSize.Width / 2, centerY - textSize.Height / 2 - 8);
                }

                using (var font = GetKhmerFont(8.5F, FontStyle.Regular))
                using (var brush = new SolidBrush(Color.FromArgb(120, 120, 120)))
                {
                    string label = "Products";
                    SizeF textSize = e.Graphics.MeasureString(label, font);
                    e.Graphics.DrawString(label, font, brush, 
                        centerX - textSize.Width / 2, centerY + 8);
                }
            }
            catch (Exception ex)
            {
                // Silently handle drawing errors
                Console.WriteLine($"Pie chart drawing error: {ex.Message}");
            }
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
