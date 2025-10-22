using SyncVerseStudio.Services;
using SyncVerseStudio.Models;
using SyncVerseStudio.Data;
using Microsoft.EntityFrameworkCore;

namespace SyncVerseStudio.Views
{
    public partial class AnalyticsView : Form
    {


//    Hello from shcool
//KONGA
        private readonly AuthenticationService _authService;
        private ApplicationDbContext? _context;
        private Panel dashboardPanel;
        private Panel kpiPanel;
        private Panel chartsPanel;
        private Panel trendsPanel;
        private System.Windows.Forms.Timer refreshTimer;

        public AnalyticsView(AuthenticationService authService)
        {
            _authService = authService;
            InitializeComponent();
            LoadData();
            
            // Auto-refresh every 30 seconds
            refreshTimer = new System.Windows.Forms.Timer
            {
                Interval = 30000
            };
            refreshTimer.Tick += async (s, e) => await RefreshData();
            refreshTimer.Start();
        }

        private void InitializeComponent()
        {
            this.AutoScaleDimensions = new SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(248, 250, 252);
            this.ClientSize = new Size(1200, 800);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Name = "AnalyticsView";
            this.Text = "Analytics";

            CreateLayout();
        }

        private void CreateLayout()
        {
            // Header
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.White,
                Padding = new Padding(20, 15, 20, 15)
            };

            var titleLabel = new Label
            {
                Text = "Business Analytics",
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(20, 20),
                Size = new Size(400, 40),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var lastUpdatedLabel = new Label
            {
                Text = $"Last Updated: {DateTime.Now:HH:mm:ss}",
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(450, 30),
                Size = new Size(200, 20),
                TextAlign = ContentAlignment.MiddleLeft,
                Name = "LastUpdatedLabel"
            };

            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(lastUpdatedLabel);
            this.Controls.Add(headerPanel);

            // Main container with scroll
            var mainContainer = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.Transparent,
                Padding = new Padding(20)
            };

            // KPI Panel
            CreateKPIPanel(mainContainer);

            // Charts Panel
            CreateChartsPanel(mainContainer);

            // Trends Panel
            CreateTrendsPanel(mainContainer);

            this.Controls.Add(mainContainer);
        }

        private void CreateKPIPanel(Panel parent)
        {
            kpiPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(parent.Width - 40, 150),
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            var kpiTitle = new Label
            {
                Text = "Key Performance Indicators",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(0, 0),
                Size = new Size(400, 30)
            };
            kpiPanel.Controls.Add(kpiTitle);

            var kpiCardsPanel = new FlowLayoutPanel
            {
                Location = new Point(0, 40),
                Size = new Size(kpiPanel.Width, 100),
                BackColor = Color.Transparent,
                WrapContents = true,
                AutoSize = false
            };

            // Create KPI cards
            var kpiCards = new[]
            {
                new { Title = "Today's Revenue", Value = "$0.00", Color = Color.FromArgb(34, 197, 94) },
                new { Title = "Daily Transactions", Value = "0", Color = Color.FromArgb(59, 130, 246) },
                new { Title = "Average Order", Value = "$0.00", Color = Color.FromArgb(168, 85, 247) },
                new { Title = "Top Product Sales", Value = "0", Color = Color.FromArgb(245, 158, 11) },
                new { Title = "Customer Growth", Value = "0%", Color = Color.FromArgb(239, 68, 68) }
            };

            foreach (var kpi in kpiCards)
            {
                var card = CreateKPICard(kpi.Title, kpi.Value, kpi.Color);
                kpiCardsPanel.Controls.Add(card);
            }

            kpiPanel.Controls.Add(kpiCardsPanel);
            parent.Controls.Add(kpiPanel);
        }

        private Panel CreateKPICard(string title, string value, Color bgColor)
        {
            var card = new Panel
            {
                Size = new Size(200, 90),
                BackColor = bgColor,
                Margin = new Padding(10, 5, 10, 5),
                Tag = title
            };

            card.Paint += (s, e) =>
            {
                var rect = card.ClientRectangle;
                using (var shadowBrush = new SolidBrush(Color.FromArgb(20, 0, 0, 0)))
                {
                    e.Graphics.FillRectangle(shadowBrush, new Rectangle(3, 3, rect.Width, rect.Height));
                }
                using (var brush = new SolidBrush(bgColor))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(15, 15),
                Size = new Size(170, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(15, 40),
                Size = new Size(170, 30),
                TextAlign = ContentAlignment.MiddleLeft,
                Name = "ValueLabel"
            };

            card.Controls.Add(titleLabel);
            card.Controls.Add(valueLabel);

            return card;
        }

        private void CreateChartsPanel(Panel parent)
        {
            chartsPanel = new Panel
            {
                Location = new Point(0, 170),
                Size = new Size(parent.Width - 40, 300),
                BackColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            chartsPanel.Paint += (s, e) =>
            {
                var rect = chartsPanel.ClientRectangle;
                using (var shadowBrush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                {
                    e.Graphics.FillRectangle(shadowBrush, new Rectangle(2, 2, rect.Width, rect.Height));
                }
                using (var brush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }
            };

            var chartsTitle = new Label
            {
                Text = "Sales Performance Chart",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(20, 20),
                Size = new Size(300, 30)
            };
            chartsPanel.Controls.Add(chartsTitle);

            // Create a custom chart panel
            var chartArea = new Panel
            {
                Location = new Point(20, 60),
                Size = new Size(chartsPanel.Width - 40, 220),
                BackColor = Color.White,
                Name = "ChartArea"
            };

            chartArea.Paint += DrawSalesChart;
            chartsPanel.Controls.Add(chartArea);

            parent.Controls.Add(chartsPanel);
        }

        private void DrawSalesChart(object sender, PaintEventArgs e)
        {
            var panel = sender as Panel;
            if (panel == null) return;

            var graphics = e.Graphics;
            var rect = panel.ClientRectangle;

            // Draw background
            using (var brush = new SolidBrush(Color.White))
            {
                graphics.FillRectangle(brush, rect);
            }

            // Draw grid
            using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1))
            {
                // Horizontal lines
                for (int i = 1; i <= 5; i++)
                {
                    int y = rect.Height * i / 6;
                    graphics.DrawLine(pen, 40, y, rect.Width - 20, y);
                }

                // Vertical lines
                for (int i = 1; i <= 7; i++)
                {
                    int x = 40 + (rect.Width - 60) * i / 8;
                    graphics.DrawLine(pen, x, 20, x, rect.Height - 30);
                }
            }

            // Sample data for demonstration
            var salesData = new[] { 1200, 1800, 1500, 2200, 1900, 2400, 2100 };
            var days = new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };

            var maxValue = salesData.Max();
            var barWidth = (rect.Width - 100) / 7;

            // Draw bars
            for (int i = 0; i < salesData.Length; i++)
            {
                var barHeight = (int)((double)salesData[i] / maxValue * (rect.Height - 80));
                var x = 50 + i * barWidth + (barWidth - 40) / 2;
                var y = rect.Height - 40 - barHeight;

                var color = Color.FromArgb(59, 130, 246); // Blue
                using (var brush = new SolidBrush(color))
                {
                    graphics.FillRectangle(brush, x, y, 40, barHeight);
                }

                // Draw value on top of bar
                using (var textBrush = new SolidBrush(Color.FromArgb(15, 23, 42)))
                using (var font = new Font("Segoe UI", 8F))
                {
                    var text = $"${salesData[i]:N0}";
                    var textSize = graphics.MeasureString(text, font);
                    graphics.DrawString(text, font, textBrush, 
                        x + 20 - textSize.Width / 2, y - 20);
                }

                // Draw day labels
                using (var textBrush = new SolidBrush(Color.FromArgb(100, 116, 139)))
                using (var font = new Font("Segoe UI", 9F))
                {
                    var textSize = graphics.MeasureString(days[i], font);
                    graphics.DrawString(days[i], font, textBrush, 
                        x + 20 - textSize.Width / 2, rect.Height - 25);
                }
            }

            // Draw Y-axis labels
            using (var textBrush = new SolidBrush(Color.FromArgb(100, 116, 139)))
            using (var font = new Font("Segoe UI", 8F))
            {
                for (int i = 0; i <= 5; i++)
                {
                    var value = (int)(maxValue * i / 5);
                    var y = rect.Height - 40 - (rect.Height - 60) * i / 5;
                    graphics.DrawString($"${value:N0}", font, textBrush, 5, y - 6);
                }
            }

            // Chart title
            using (var titleBrush = new SolidBrush(Color.FromArgb(15, 23, 42)))
            using (var titleFont = new Font("Segoe UI", 10F, FontStyle.Bold))
            {
                graphics.DrawString("Weekly Sales Performance", titleFont, titleBrush, 50, 5);
            }
        }

        private void CreateTrendsPanel(Panel parent)
        {
            trendsPanel = new Panel
            {
                Location = new Point(0, 490),
                Size = new Size(parent.Width - 40, 250),
                BackColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            trendsPanel.Paint += (s, e) =>
            {
                var rect = trendsPanel.ClientRectangle;
                using (var shadowBrush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                {
                    e.Graphics.FillRectangle(shadowBrush, new Rectangle(2, 2, rect.Width, rect.Height));
                }
                using (var brush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }
            };

            var trendsTitle = new Label
            {
                Text = "Business Insights & Trends",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(20, 20),
                Size = new Size(300, 30)
            };
            trendsPanel.Controls.Add(trendsTitle);

            // Create insights cards
            var insightsPanel = new FlowLayoutPanel
            {
                Location = new Point(20, 60),
                Size = new Size(trendsPanel.Width - 40, 170),
                BackColor = Color.Transparent,
                WrapContents = true,
                AutoSize = false
            };

            var insights = new[]
            {
                new { Title = "Best Selling Day", Value = "Friday", Trend = "+15% vs last week", Color = Color.FromArgb(34, 197, 94) },
                new { Title = "Peak Hours", Value = "2-4 PM", Trend = "Most sales activity", Color = Color.FromArgb(59, 130, 246) },
                new { Title = "Top Category", Value = "Electronics", Trend = "40% of total sales", Color = Color.FromArgb(168, 85, 247) },
                new { Title = "Customer Retention", Value = "68%", Trend = "+5% this month", Color = Color.FromArgb(245, 158, 11) }
            };

            foreach (var insight in insights)
            {
                var card = CreateInsightCard(insight.Title, insight.Value, insight.Trend, insight.Color);
                insightsPanel.Controls.Add(card);
            }

            trendsPanel.Controls.Add(insightsPanel);
            parent.Controls.Add(trendsPanel);
        }

        private Panel CreateInsightCard(string title, string value, string trend, Color bgColor)
        {
            var card = new Panel
            {
                Size = new Size(270, 80),
                BackColor = Color.FromArgb(248, 250, 252),
                Margin = new Padding(10),
                Padding = new Padding(15)
            };

            card.Paint += (s, e) =>
            {
                var rect = card.ClientRectangle;
                using (var pen = new Pen(bgColor, 3))
                {
                    e.Graphics.DrawLine(pen, 0, 0, 0, rect.Height);
                }
                using (var brush = new SolidBrush(Color.FromArgb(248, 250, 252)))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(0, 5),
                Size = new Size(240, 20)
            };

            var valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = bgColor,
                Location = new Point(0, 25),
                Size = new Size(240, 25)
            };

            var trendLabel = new Label
            {
                Text = trend,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(0, 50),
                Size = new Size(240, 20)
            };

            card.Controls.Add(titleLabel);
            card.Controls.Add(valueLabel);
            card.Controls.Add(trendLabel);

            return card;
        }

        private async void LoadData()
        {
            try
            {
                _context = new ApplicationDbContext();
                await RefreshData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading analytics data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task RefreshData()
        {
            if (_context == null) return;

            try
            {
                await UpdateKPICards();
                UpdateLastUpdatedLabel();
                
                // Refresh chart
                var chartArea = this.Controls.Find("ChartArea", true).FirstOrDefault();
                chartArea?.Invalidate();
            }
            catch (Exception ex)
            {
                // Log error but don't show message box to avoid interrupting user
                Console.WriteLine($"Error refreshing analytics data: {ex.Message}");
            }
        }

        private async Task UpdateKPICards()
        {
            if (_context == null) return;

            try
            {
                var today = DateTime.Today;
                var yesterday = today.AddDays(-1);
                var lastMonth = today.AddMonths(-1);

                // Today's revenue
                var todayRevenue = await _context.Sales
                    .Where(s => s.SaleDate.Date == today && s.Status == SaleStatus.Completed)
                    .SumAsync(s => (decimal?)s.TotalAmount) ?? 0;

                // Daily transactions
                var dailyTransactions = await _context.Sales
                    .CountAsync(s => s.SaleDate.Date == today && s.Status == SaleStatus.Completed);

                // Average order
                var avgOrder = dailyTransactions > 0 ? todayRevenue / dailyTransactions : 0;

                // Top product sales (today)
                var topProductSales = await _context.SaleItems
                    .Include(si => si.Sale)
                    .Where(si => si.Sale.SaleDate.Date == today && si.Sale.Status == SaleStatus.Completed)
                    .GroupBy(si => si.ProductId)
                    .Select(g => g.Sum(si => si.Quantity))
                    .OrderByDescending(q => q)
                    .FirstOrDefaultAsync();

                // Customer growth (this month vs last month)
                var thisMonthCustomers = await _context.Customers
                    .CountAsync(c => c.CreatedDate.Month == today.Month && c.CreatedDate.Year == today.Year);
                
                var lastMonthCustomers = await _context.Customers
                    .CountAsync(c => c.CreatedDate.Month == lastMonth.Month && c.CreatedDate.Year == lastMonth.Year);

                var customerGrowth = lastMonthCustomers > 0 
                    ? ((double)(thisMonthCustomers - lastMonthCustomers) / lastMonthCustomers * 100) 
                    : 0;

                // Update KPI cards
                this.Invoke(new Action(() =>
                {
                    UpdateKPICard("Today's Revenue", $"${todayRevenue:N2}");
                    UpdateKPICard("Daily Transactions", dailyTransactions.ToString("N0"));
                    UpdateKPICard("Average Order", $"${avgOrder:N2}");
                    UpdateKPICard("Top Product Sales", topProductSales.ToString("N0"));
                    UpdateKPICard("Customer Growth", $"{customerGrowth:+0.0;-0.0;0}%");
                }));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating KPI cards: {ex.Message}");
            }
        }

        private void UpdateKPICard(string title, string value)
        {
            var kpiCards = kpiPanel.Controls.OfType<FlowLayoutPanel>().FirstOrDefault()?.Controls.OfType<Panel>();
            if (kpiCards == null) return;

            var card = kpiCards.FirstOrDefault(c => c.Tag?.ToString() == title);
            if (card == null) return;

            var valueLabel = card.Controls.OfType<Label>().FirstOrDefault(l => l.Name == "ValueLabel");
            if (valueLabel != null)
            {
                valueLabel.Text = value;
            }
        }

        private void UpdateLastUpdatedLabel()
        {
            this.Invoke(new Action(() =>
            {
                var lastUpdatedLabel = this.Controls.Find("LastUpdatedLabel", true).FirstOrDefault() as Label;
                if (lastUpdatedLabel != null)
                {
                    lastUpdatedLabel.Text = $"Last Updated: {DateTime.Now:HH:mm:ss}";
                }
            }));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                refreshTimer?.Stop();
                refreshTimer?.Dispose();
                _context?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}