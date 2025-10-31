using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SyncVerseStudio.Data;
using SyncVerseStudio.Models;
using SyncVerseStudio.Services;
using FontAwesome.Sharp;

namespace SyncVerseStudio.Views.CashierDashboard
{
    public partial class RealTimeCashierDashboard : Form
    {
        private readonly AuthenticationService _authService;
        private readonly ApplicationDbContext _context;
        private System.Windows.Forms.Timer _refreshTimer;
        
        // Metric Labels for real-time updates
        private Label lblInvoiceCount;
        private Label lblTotalRevenue;
        private Label lblPendingAmount;
        private Label lblCompletedToday;
        
        // Chart panels
        private Panel statusChartPanel;
        
        // Time period filter
        private ComboBox timePeriodCombo;
        
        // Data grid
        private DataGridView invoiceGrid;
        
        // Account summary labels
        private Label lblActiveInvoices;
        private Label lblRepeatedInvoices;
        private Label lblPaymentLinks;
        private Label lblStockSales;
        private Label lblProducts;

        public RealTimeCashierDashboard(AuthenticationService authService)
        {
            _authService = authService;
            _context = new ApplicationDbContext();
            InitializeComponent();
            LoadDashboardData();
            StartAutoRefresh();
        }

        private void StartAutoRefresh()
        {
            _refreshTimer = new System.Windows.Forms.Timer();
            _refreshTimer.Interval = 5000; // Refresh every 5 seconds
            _refreshTimer.Tick += async (s, e) => await RefreshDashboardData();
            _refreshTimer.Start();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(248, 250, 252);
            this.Size = new Size(1400, 900);
            this.Dock = DockStyle.Fill;

            // Main Content Area (Full Width)
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(248, 250, 252),
                Padding = new Padding(30, 20, 30, 20)
            };

            int yPos = 0;

            // Header
            var header = CreateHeader();
            header.Location = new Point(0, yPos);
            mainPanel.Controls.Add(header);
            yPos += 70;

            // Metric Cards
            var metricsPanel = CreateMetricsPanel();
            metricsPanel.Location = new Point(0, yPos);
            mainPanel.Controls.Add(metricsPanel);
            yPos += 120;

            // Statistics Charts (Clean Modern Style)
            var statsPanel = CreateModernStatisticsPanel();
            statsPanel.Location = new Point(0, yPos);
            mainPanel.Controls.Add(statsPanel);
            yPos += 500;

            // Latest Invoices with Click-to-Print
            var bottomPanel = CreateInteractiveBottomPanel();
            bottomPanel.Location = new Point(0, yPos);
            mainPanel.Controls.Add(bottomPanel);

            this.Controls.Add(mainPanel);
            this.ResumeLayout(false);
        }





        private Panel CreateHeader()
        {
            var panel = new Panel
            {
                Size = new Size(1340, 60),
                BackColor = Color.White
            };

            panel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, panel.Width - 1, panel.Height - 1), 10))
                using (var brush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillPath(brush, path);
                }
            };

            var syncLabel = new Label
            {
                Text = "SYNCVERSE DASHBOARD",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 184, 166),
                Location = new Point(20, 18),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            // Real-time date and time
            var dateLabel = new Label
            {
                Text = GetFormattedDateTime(),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(1130, 22),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            var timeTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            timeTimer.Tick += (s, e) => dateLabel.Text = GetFormattedDateTime();
            timeTimer.Start();

            panel.Controls.AddRange(new Control[] { syncLabel, dateLabel });
            return panel;
        }

        private string GetFormattedDateTime()
        {
            return DateTime.Now.ToString("MMM dd, yyyy, HH:mm:ss");
        }

        private Panel CreateMetricsPanel()
        {
            var panel = new Panel
            {
                Size = new Size(1340, 110),
                BackColor = Color.Transparent
            };

            var invoiceCard = CreateModernMetricCard("Total Sales", "0 Sale", IconChar.ShoppingCart, 
                Color.FromArgb(59, 130, 246), out lblInvoiceCount);
            invoiceCard.Location = new Point(0, 0);

            var revenueCard = CreateModernMetricCard("Total Revenue", "$0.00", IconChar.MoneyBillWave, 
                Color.FromArgb(34, 197, 94), out lblTotalRevenue);
            revenueCard.Location = new Point(460, 0);

            var avgCard = CreateModernMetricCard("Avg Transaction", "$0.00", IconChar.ChartBar, 
                Color.FromArgb(168, 85, 247), out lblPendingAmount);
            avgCard.Location = new Point(920, 0);

            panel.Controls.AddRange(new Control[] { invoiceCard, revenueCard, avgCard });
            return panel;
        }

        private Panel CreateModernMetricCard(string title, string initialValue, IconChar icon, Color accentColor, out Label valueLabel)
        {
            var card = new Panel
            {
                Size = new Size(420, 100),
                BackColor = Color.White
            };

            card.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, card.Width - 1, card.Height - 1), 10))
                using (var brush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillPath(brush, path);
                }
                
                // Accent bar on left
                using (var accentBrush = new SolidBrush(accentColor))
                {
                    e.Graphics.FillRectangle(accentBrush, 0, 0, 4, card.Height);
                }
            };

            var iconBox = new IconPictureBox
            {
                IconChar = icon,
                IconColor = accentColor,
                IconSize = 32,
                Location = new Point(20, 34),
                Size = new Size(32, 32),
                BackColor = Color.Transparent
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(65, 25),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            valueLabel = new Label
            {
                Text = initialValue,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(65, 45),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            card.Controls.AddRange(new Control[] { iconBox, titleLabel, valueLabel });
            return card;
        }

        private Panel CreateMetricCard(string title, string initialValue, IconChar icon, Color iconColor, out Label valueLabel)
        {
            var card = new Panel
            {
                Size = new Size(650, 110),
                BackColor = Color.White,
                Cursor = Cursors.Hand
            };

            card.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, card.Width - 1, card.Height - 1), 12))
                using (var brush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillPath(brush, path);
                }
                
                using (var shadowPath = GetRoundedRectPath(new Rectangle(2, 2, card.Width - 3, card.Height - 3), 12))
                using (var shadowBrush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                {
                    e.Graphics.FillPath(shadowBrush, shadowPath);
                }
            };

            var iconBox = new IconPictureBox
            {
                IconChar = icon,
                IconColor = iconColor,
                IconSize = 32,
                Location = new Point(25, 39),
                Size = new Size(32, 32),
                BackColor = Color.Transparent
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(70, 28),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            valueLabel = new Label
            {
                Text = initialValue,
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(70, 52),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            card.MouseEnter += (s, e) => card.BackColor = Color.FromArgb(249, 250, 251);
            card.MouseLeave += (s, e) => card.BackColor = Color.White;

            card.Controls.AddRange(new Control[] { iconBox, titleLabel, valueLabel });
            return card;
        }

        private Panel CreateStatisticsPanel()
        {
            var panel = new Panel
            {
                Size = new Size(1340, 660),
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
                Text = "Analytics Dashboard",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(25, 20),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            timePeriodCombo = new ComboBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(1170, 20),
                Size = new Size(145, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.White
            };
            timePeriodCombo.Items.AddRange(new[] { "Today", "Last 7 days", "This Month", "This Year" });
            timePeriodCombo.SelectedIndex = 0;
            timePeriodCombo.SelectedIndexChanged += async (s, e) => await RefreshCharts();

            // Row 1: Sales Volume by Category and Transaction Frequency
            var salesByCategoryPanel = new Panel
            {
                Location = new Point(25, 70),
                Size = new Size(650, 270),
                BackColor = Color.FromArgb(249, 250, 251),
                BorderStyle = BorderStyle.None
            };
            salesByCategoryPanel.Paint += SalesByCategoryPanel_Paint;

            var transactionFrequencyPanel = new Panel
            {
                Location = new Point(690, 70),
                Size = new Size(625, 270),
                BackColor = Color.FromArgb(249, 250, 251),
                BorderStyle = BorderStyle.None
            };
            transactionFrequencyPanel.Paint += TransactionFrequencyPanel_Paint;

            // Row 2: Average Transaction Value and Status Distribution
            var avgTransactionPanel = new Panel
            {
                Location = new Point(25, 360),
                Size = new Size(650, 270),
                BackColor = Color.FromArgb(249, 250, 251),
                BorderStyle = BorderStyle.None
            };
            avgTransactionPanel.Paint += AvgTransactionPanel_Paint;

            // Status Distribution Chart Panel
            statusChartPanel = new Panel
            {
                Location = new Point(690, 360),
                Size = new Size(625, 270),
                BackColor = Color.FromArgb(249, 250, 251),
                BorderStyle = BorderStyle.None
            };
            statusChartPanel.Paint += StatusChartPanel_Paint;

            panel.Controls.AddRange(new Control[] { 
                titleLabel, timePeriodCombo, 
                salesByCategoryPanel, transactionFrequencyPanel,
                avgTransactionPanel, statusChartPanel
            });
            return panel;
        }

        private void SalesByCategoryPanel_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var titleFont = new Font("Segoe UI", 12, FontStyle.Bold))
            using (var titleBrush = new SolidBrush(Color.FromArgb(51, 65, 85)))
            {
                g.DrawString("Sales Volume by Category", titleFont, titleBrush, 15, 10);
            }

            var categoryData = GetSalesByCategory();
            if (categoryData.Count == 0)
            {
                DrawNoDataMessage(g, (Panel)sender);
                return;
            }

            DrawBarChart(g, categoryData, (Panel)sender);
        }

        private void TransactionFrequencyPanel_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var titleFont = new Font("Segoe UI", 12, FontStyle.Bold))
            using (var titleBrush = new SolidBrush(Color.FromArgb(51, 65, 85)))
            {
                g.DrawString("Transaction Frequency Over Time", titleFont, titleBrush, 15, 10);
            }

            var frequencyData = GetTransactionFrequency();
            if (frequencyData.Count == 0)
            {
                DrawNoDataMessage(g, (Panel)sender);
                return;
            }

            DrawAreaChart(g, frequencyData, (Panel)sender);
        }

        private void AvgTransactionPanel_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var titleFont = new Font("Segoe UI", 12, FontStyle.Bold))
            using (var titleBrush = new SolidBrush(Color.FromArgb(51, 65, 85)))
            {
                g.DrawString("Average Transaction Value Trends", titleFont, titleBrush, 15, 10);
            }

            var avgData = GetAverageTransactionValue();
            if (avgData.Count == 0)
            {
                DrawNoDataMessage(g, (Panel)sender);
                return;
            }

            DrawSmoothLineChart(g, avgData, (Panel)sender);
        }

        private void DrawNoDataMessage(Graphics g, Panel panel)
        {
            using (var font = new Font("Segoe UI", 11, FontStyle.Italic))
            using (var brush = new SolidBrush(Color.FromArgb(148, 163, 184)))
            {
                var text = "No data available";
                var size = g.MeasureString(text, font);
                g.DrawString(text, font, brush, 
                    (panel.Width - size.Width) / 2, 
                    (panel.Height - size.Height) / 2);
            }
        }

        private void DrawBarChart(Graphics g, List<(string Category, decimal Amount, int Count)> data, Panel panel)
        {
            int chartX = 50;
            int chartY = 50;
            int chartWidth = panel.Width - 80;
            int chartHeight = panel.Height - 80;

            decimal maxAmount = data.Max(d => d.Amount);
            if (maxAmount == 0) maxAmount = 100;

            int barWidth = chartWidth / (data.Count * 2);
            int spacing = barWidth / 2;
            int depth3D = 12; // 3D depth effect

            for (int i = 0; i < data.Count; i++)
            {
                int barHeight = (int)(data[i].Amount / maxAmount * chartHeight);
                int x = chartX + (i * (barWidth + spacing));
                int y = chartY + chartHeight - barHeight;

                // Draw 3D shadow/depth
                using (var shadowBrush = new SolidBrush(Color.FromArgb(80, 0, 0, 0)))
                {
                    Point[] shadowPoints = {
                        new Point(x + depth3D, y - depth3D),
                        new Point(x + barWidth + depth3D, y - depth3D),
                        new Point(x + barWidth + depth3D, y + barHeight - depth3D),
                        new Point(x + barWidth, y + barHeight),
                        new Point(x + barWidth, y),
                        new Point(x + depth3D, y - depth3D)
                    };
                    g.FillPolygon(shadowBrush, shadowPoints);
                }

                // Draw top 3D face
                using (var topBrush = new SolidBrush(Color.FromArgb(100, 150, 255)))
                {
                    Point[] topPoints = {
                        new Point(x, y),
                        new Point(x + depth3D, y - depth3D),
                        new Point(x + barWidth + depth3D, y - depth3D),
                        new Point(x + barWidth, y)
                    };
                    g.FillPolygon(topBrush, topPoints);
                }

                // Draw main bar with gradient
                using (var barBrush = new LinearGradientBrush(
                    new Rectangle(x, y, barWidth, barHeight),
                    Color.FromArgb(59, 130, 246),
                    Color.FromArgb(37, 99, 235),
                    LinearGradientMode.Vertical))
                {
                    g.FillRectangle(barBrush, x, y, barWidth, barHeight);
                }

                // Draw bar border for definition
                using (var borderPen = new Pen(Color.FromArgb(30, 64, 175), 1))
                {
                    g.DrawRectangle(borderPen, x, y, barWidth, barHeight);
                }

                using (var font = new Font("Segoe UI", 8))
                using (var brush = new SolidBrush(Color.FromArgb(100, 116, 139)))
                {
                    var categoryText = data[i].Category.Length > 10 ? data[i].Category.Substring(0, 10) + "..." : data[i].Category;
                    var size = g.MeasureString(categoryText, font);
                    g.DrawString(categoryText, font, brush, x + (barWidth - size.Width) / 2, chartY + chartHeight + 5);
                    
                    var amountText = FormatCurrency(data[i].Amount);
                    var amountSize = g.MeasureString(amountText, font);
                    g.DrawString(amountText, font, brush, x + (barWidth - amountSize.Width) / 2, y - 20);
                }
            }
        }

        private void DrawAreaChart(Graphics g, List<(DateTime Date, int Count)> data, Panel panel)
        {
            int chartX = 50;
            int chartY = 50;
            int chartWidth = panel.Width - 80;
            int chartHeight = panel.Height - 80;

            if (data.Count < 2) return;

            int maxCount = data.Max(d => d.Count);
            if (maxCount == 0) maxCount = 10;

            var points = new List<PointF>();
            for (int i = 0; i < data.Count; i++)
            {
                float x = chartX + (chartWidth * i / (float)(data.Count - 1));
                float y = chartY + chartHeight - (data[i].Count / (float)maxCount * chartHeight);
                points.Add(new PointF(x, y));
            }

            if (points.Count > 1)
            {
                // Draw 3D radar-style grid
                using (var gridPen = new Pen(Color.FromArgb(50, 200, 200, 200), 1))
                {
                    for (int i = 0; i <= 5; i++)
                    {
                        int gridY = chartY + (chartHeight * i / 5);
                        g.DrawLine(gridPen, chartX, gridY, chartX + chartWidth, gridY);
                    }
                }

                // Draw multiple gradient layers for 3D effect
                for (int layer = 3; layer >= 0; layer--)
                {
                    var layerPoints = new List<PointF>();
                    foreach (var point in points)
                    {
                        layerPoints.Add(new PointF(point.X, point.Y + (layer * 2)));
                    }
                    layerPoints.Add(new PointF(points[points.Count - 1].X, chartY + chartHeight));
                    layerPoints.Add(new PointF(points[0].X, chartY + chartHeight));

                    int alpha = 120 - (layer * 20);
                    using (var fillBrush = new LinearGradientBrush(
                        new PointF(0, chartY),
                        new PointF(0, chartY + chartHeight),
                        Color.FromArgb(alpha, 34, 197, 94),
                        Color.FromArgb(alpha / 4, 34, 197, 94)))
                    {
                        g.FillPolygon(fillBrush, layerPoints.ToArray());
                    }
                }

                // Draw main line with glow effect
                using (var glowPen = new Pen(Color.FromArgb(100, 34, 197, 94), 6))
                {
                    g.DrawLines(glowPen, points.ToArray());
                }
                using (var linePen = new Pen(Color.FromArgb(34, 197, 94), 3))
                {
                    g.DrawLines(linePen, points.ToArray());
                }

                // Draw 3D points
                foreach (var point in points)
                {
                    using (var outerBrush = new SolidBrush(Color.FromArgb(150, 34, 197, 94)))
                    using (var innerBrush = new SolidBrush(Color.FromArgb(34, 197, 94)))
                    {
                        g.FillEllipse(outerBrush, point.X - 6, point.Y - 6, 12, 12);
                        g.FillEllipse(innerBrush, point.X - 4, point.Y - 4, 8, 8);
                    }
                }
            }
        }

        private void DrawSmoothLineChart(Graphics g, List<(DateTime Date, decimal AvgAmount)> data, Panel panel)
        {
            int chartX = 50;
            int chartY = 50;
            int chartWidth = panel.Width - 80;
            int chartHeight = panel.Height - 80;

            if (data.Count < 2) return;

            decimal maxAmount = data.Max(d => d.AvgAmount);
            if (maxAmount == 0) maxAmount = 100;

            var points = new List<PointF>();
            for (int i = 0; i < data.Count; i++)
            {
                float x = chartX + (chartWidth * i / (float)(data.Count - 1));
                float y = chartY + chartHeight - (float)(data[i].AvgAmount / maxAmount * chartHeight);
                points.Add(new PointF(x, y));
            }

            if (points.Count > 1)
            {
                // Draw financial-style candlestick background
                using (var gridPen = new Pen(Color.FromArgb(30, 200, 200, 200), 1))
                {
                    for (int i = 0; i <= 5; i++)
                    {
                        int gridY = chartY + (chartHeight * i / 5);
                        g.DrawLine(gridPen, chartX, gridY, chartX + chartWidth, gridY);
                    }
                }

                // Draw shadow line for 3D depth
                using (var shadowPen = new Pen(Color.FromArgb(80, 168, 85, 247), 4))
                {
                    var shadowPoints = points.Select(p => new PointF(p.X + 2, p.Y + 2)).ToArray();
                    g.DrawLines(shadowPen, shadowPoints);
                }

                // Draw glow effect
                using (var glowPen = new Pen(Color.FromArgb(100, 168, 85, 247), 6))
                {
                    g.DrawLines(glowPen, points.ToArray());
                }

                // Draw main line with gradient
                using (var linePen = new Pen(Color.FromArgb(168, 85, 247), 3))
                {
                    linePen.DashStyle = DashStyle.Solid;
                    g.DrawLines(linePen, points.ToArray());
                }

                // Draw 3D financial-style markers
                for (int i = 0; i < points.Count; i++)
                {
                    var point = points[i];
                    
                    // Draw candlestick-style marker
                    using (var outerBrush = new SolidBrush(Color.White))
                    using (var innerBrush = new SolidBrush(Color.FromArgb(168, 85, 247)))
                    using (var borderPen = new Pen(Color.FromArgb(126, 34, 206), 2))
                    {
                        // Outer circle
                        g.FillEllipse(outerBrush, point.X - 7, point.Y - 7, 14, 14);
                        g.DrawEllipse(borderPen, point.X - 7, point.Y - 7, 14, 14);
                        
                        // Inner circle
                        g.FillEllipse(innerBrush, point.X - 4, point.Y - 4, 8, 8);
                    }

                    // Draw value label for key points
                    if (i % 3 == 0)
                    {
                        using (var font = new Font("Segoe UI", 7, FontStyle.Bold))
                        using (var brush = new SolidBrush(Color.FromArgb(100, 116, 139)))
                        {
                            var valueText = FormatCurrency(data[i].AvgAmount);
                            var size = g.MeasureString(valueText, font);
                            g.DrawString(valueText, font, brush, point.X - size.Width / 2, point.Y - 20);
                        }
                    }
                }
            }
        }

        private List<(string Category, decimal Amount, int Count)> GetSalesByCategory()
        {
            try
            {
                var (startDate, endDate) = GetDateRange();

                var data = _context.Sales
                    .Include(s => s.SaleItems)
                    .ThenInclude(si => si.Product)
                    .ThenInclude(p => p.Category)
                    .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate && s.Status == SaleStatus.Completed)
                    .SelectMany(s => s.SaleItems.Select(si => new
                    {
                        CategoryName = si.Product.Category != null ? si.Product.Category.Name : "Uncategorized",
                        Amount = si.TotalPrice,
                        SaleId = s.Id
                    }))
                    .GroupBy(x => x.CategoryName)
                    .Select(g => new
                    {
                        Category = g.Key,
                        Amount = g.Sum(x => x.Amount),
                        Count = g.Select(x => x.SaleId).Distinct().Count()
                    })
                    .OrderByDescending(x => x.Amount)
                    .Take(6)
                    .ToList();

                return data.Select(d => (d.Category, d.Amount, d.Count)).ToList();
            }
            catch
            {
                return new List<(string Category, decimal Amount, int Count)>();
            }
        }

        private List<(DateTime Date, int Count)> GetTransactionFrequency()
        {
            try
            {
                var (startDate, endDate) = GetDateRange();

                var data = _context.Sales
                    .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate && s.Status == SaleStatus.Completed)
                    .GroupBy(s => s.SaleDate.Date)
                    .Select(g => new { Date = g.Key, Count = g.Count() })
                    .OrderBy(x => x.Date)
                    .ToList();

                var result = new List<(DateTime Date, int Count)>();
                for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
                {
                    var dayData = data.FirstOrDefault(d => d.Date == date);
                    result.Add((date, dayData?.Count ?? 0));
                }

                return result;
            }
            catch
            {
                return new List<(DateTime Date, int Count)>();
            }
        }

        private List<(DateTime Date, decimal AvgAmount)> GetAverageTransactionValue()
        {
            try
            {
                var (startDate, endDate) = GetDateRange();

                var data = _context.Sales
                    .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate && s.Status == SaleStatus.Completed)
                    .GroupBy(s => s.SaleDate.Date)
                    .Select(g => new { Date = g.Key, AvgAmount = g.Average(s => s.TotalAmount) })
                    .OrderBy(x => x.Date)
                    .ToList();

                var result = new List<(DateTime Date, decimal AvgAmount)>();
                for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
                {
                    var dayData = data.FirstOrDefault(d => d.Date == date);
                    result.Add((date, dayData?.AvgAmount ?? 0));
                }

                return result;
            }
            catch
            {
                return new List<(DateTime Date, decimal AvgAmount)>();
            }
        }

        private string FormatCurrency(decimal amount)
        {
            return CurrencyService.FormatAuto(amount);
        }

        // Clean Chart Drawing Methods
        private void DrawSimpleBarChart(Graphics g, List<(string Category, decimal Amount, int Count)> data, Panel panel)
        {
            int chartX = 30;
            int chartY = 50;
            int chartWidth = panel.Width - 60;
            int chartHeight = panel.Height - 80;

            decimal maxAmount = data.Max(d => d.Amount);
            if (maxAmount == 0) maxAmount = 100;

            int barWidth = chartWidth / (data.Count * 2);
            int spacing = barWidth / 2;

            for (int i = 0; i < data.Count; i++)
            {
                int barHeight = (int)(data[i].Amount / maxAmount * chartHeight);
                int x = chartX + (i * (barWidth + spacing));
                int y = chartY + chartHeight - barHeight;

                using (var barBrush = new SolidBrush(Color.FromArgb(59, 130, 246)))
                {
                    g.FillRectangle(barBrush, x, y, barWidth, barHeight);
                }

                using (var font = new Font("Segoe UI", 7))
                using (var brush = new SolidBrush(Color.FromArgb(100, 116, 139)))
                {
                    var categoryText = data[i].Category.Length > 8 ? data[i].Category.Substring(0, 8) : data[i].Category;
                    var size = g.MeasureString(categoryText, font);
                    g.DrawString(categoryText, font, brush, x + (barWidth - size.Width) / 2, chartY + chartHeight + 5);
                }
            }
        }

        private void DrawSimpleLineChart(Graphics g, List<(DateTime Date, int Count)> data, Panel panel)
        {
            int chartX = 30;
            int chartY = 50;
            int chartWidth = panel.Width - 60;
            int chartHeight = panel.Height - 80;

            if (data.Count < 2) return;

            int maxCount = data.Max(d => d.Count);
            if (maxCount == 0) maxCount = 10;

            var points = new List<PointF>();
            for (int i = 0; i < data.Count; i++)
            {
                float x = chartX + (chartWidth * i / (float)(data.Count - 1));
                float y = chartY + chartHeight - (data[i].Count / (float)maxCount * chartHeight);
                points.Add(new PointF(x, y));
            }

            if (points.Count > 1)
            {
                using (var linePen = new Pen(Color.FromArgb(34, 197, 94), 2))
                {
                    g.DrawLines(linePen, points.ToArray());
                }

                foreach (var point in points)
                {
                    using (var pointBrush = new SolidBrush(Color.FromArgb(34, 197, 94)))
                    {
                        g.FillEllipse(pointBrush, point.X - 3, point.Y - 3, 6, 6);
                    }
                }
            }
        }

        private void DrawSimpleAreaChart(Graphics g, List<(DateTime Date, decimal AvgAmount)> data, Panel panel)
        {
            int chartX = 30;
            int chartY = 50;
            int chartWidth = panel.Width - 60;
            int chartHeight = panel.Height - 80;

            if (data.Count < 2) return;

            decimal maxAmount = data.Max(d => d.AvgAmount);
            if (maxAmount == 0) maxAmount = 100;

            var points = new List<PointF>();
            for (int i = 0; i < data.Count; i++)
            {
                float x = chartX + (chartWidth * i / (float)(data.Count - 1));
                float y = chartY + chartHeight - (float)(data[i].AvgAmount / maxAmount * chartHeight);
                points.Add(new PointF(x, y));
            }

            if (points.Count > 1)
            {
                var fillPoints = new List<PointF>(points);
                fillPoints.Add(new PointF(points[points.Count - 1].X, chartY + chartHeight));
                fillPoints.Add(new PointF(points[0].X, chartY + chartHeight));

                using (var fillBrush = new SolidBrush(Color.FromArgb(100, 168, 85, 247)))
                {
                    g.FillPolygon(fillBrush, fillPoints.ToArray());
                }

                using (var linePen = new Pen(Color.FromArgb(168, 85, 247), 2))
                {
                    g.DrawLines(linePen, points.ToArray());
                }
            }
        }

        private void DrawSimplePieChart(Graphics g, Panel panel)
        {
            int centerX = panel.Width / 2;
            int centerY = panel.Height / 2 + 10;
            int radius = 60;

            var colors = new[] {
                Color.FromArgb(59, 130, 246),
                Color.FromArgb(34, 197, 94),
                Color.FromArgb(249, 115, 22),
                Color.FromArgb(168, 85, 247)
            };

            float startAngle = 0;
            for (int i = 0; i < 4; i++)
            {
                float sweepAngle = 90;
                using (var brush = new SolidBrush(colors[i]))
                {
                    g.FillPie(brush, centerX - radius, centerY - radius, radius * 2, radius * 2, startAngle, sweepAngle);
                }
                startAngle += sweepAngle;
            }
        }

        private void DrawHourlySalesChart(Graphics g, Panel panel)
        {
            int chartX = 30;
            int chartY = 50;
            int chartWidth = panel.Width - 60;
            int chartHeight = panel.Height - 80;

            int hours = 12;
            int barWidth = chartWidth / (hours * 2);
            int spacing = barWidth / 2;

            var random = new Random();
            for (int i = 0; i < hours; i++)
            {
                int barHeight = random.Next(20, chartHeight);
                int x = chartX + (i * (barWidth + spacing));
                int y = chartY + chartHeight - barHeight;

                using (var barBrush = new SolidBrush(Color.FromArgb(249, 115, 22)))
                {
                    g.FillRectangle(barBrush, x, y, barWidth, barHeight);
                }
            }
        }

        private void DrawSimpleDonutChart(Graphics g, List<(string Status, int Count, Color Color)> data, Panel panel)
        {
            int centerX = panel.Width / 2;
            int centerY = panel.Height / 2 + 10;
            int outerRadius = 60;
            int innerRadius = 35;

            float total = data.Sum(d => d.Count);
            float startAngle = -90;

            foreach (var item in data)
            {
                float sweepAngle = (item.Count / total) * 360;

                using (var brush = new SolidBrush(item.Color))
                using (var path = new GraphicsPath())
                {
                    path.AddArc(centerX - outerRadius, centerY - outerRadius, outerRadius * 2, outerRadius * 2, startAngle, sweepAngle);
                    path.AddArc(centerX - innerRadius, centerY - innerRadius, innerRadius * 2, innerRadius * 2, startAngle + sweepAngle, -sweepAngle);
                    path.CloseFigure();
                    g.FillPath(brush, path);
                }

                startAngle += sweepAngle;
            }

            using (var centerBrush = new SolidBrush(Color.FromArgb(249, 250, 251)))
            {
                g.FillEllipse(centerBrush, centerX - innerRadius, centerY - innerRadius, innerRadius * 2, innerRadius * 2);
            }
        }

        private List<(string Status, int Count, Color Color)> GetPaymentMethodDistribution()
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var (startDate, endDate) = GetDateRange();

                    var paymentCounts = context.Sales
                        .AsNoTracking()
                        .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate && s.Status == SaleStatus.Completed)
                        .GroupBy(s => s.PaymentMethod)
                        .Select(g => new { Method = g.Key, Count = g.Count() })
                        .ToList();

                    var result = new List<(string Status, int Count, Color Color)>();

                    foreach (var item in paymentCounts)
                    {
                        Color color = item.Method switch
                        {
                            PaymentMethod.Cash => Color.FromArgb(34, 197, 94),
                            PaymentMethod.Card => Color.FromArgb(59, 130, 246),
                            PaymentMethod.Mobile => Color.FromArgb(168, 85, 247),
                            PaymentMethod.Mixed => Color.FromArgb(249, 115, 22),
                            _ => Color.FromArgb(100, 116, 139)
                        };

                        result.Add((item.Method.ToString(), item.Count, color));
                    }

                    return result;
                }
            }
            catch
            {
                return new List<(string Status, int Count, Color Color)>();
            }
        }



        private void StatusChartPanel_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw title
            using (var titleFont = new Font("Segoe UI", 12, FontStyle.Bold))
            using (var titleBrush = new SolidBrush(Color.FromArgb(51, 65, 85)))
            {
                g.DrawString("Status Distribution", titleFont, titleBrush, 15, 10);
            }

            // Get status data
            var statusData = GetStatusDistribution();
            if (statusData.Sum(d => d.Count) == 0)
            {
                using (var font = new Font("Segoe UI", 11, FontStyle.Italic))
                using (var brush = new SolidBrush(Color.FromArgb(148, 163, 184)))
                {
                    var text = "No data available";
                    var size = g.MeasureString(text, font);
                    g.DrawString(text, font, brush, 
                        (statusChartPanel.Width - size.Width) / 2, 
                        (statusChartPanel.Height - size.Height) / 2);
                }
                return;
            }

            DrawDonutChart(g, statusData);
        }

        private void DrawDonutChart(Graphics g, List<(string Status, int Count, Color Color)> data)
        {
            int centerX = statusChartPanel.Width / 2;
            int centerY = 140;
            int outerRadius = 80;
            int innerRadius = 50;
            int depth3D = 8; // 3D depth

            float total = data.Sum(d => d.Count);
            float startAngle = -90;

            // Draw 3D shadow layers
            for (int layer = depth3D; layer > 0; layer--)
            {
                float layerStartAngle = -90;
                foreach (var item in data)
                {
                    float sweepAngle = (item.Count / total) * 360;
                    
                    using (var shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
                    using (var path = new GraphicsPath())
                    {
                        int offsetY = centerY + layer;
                        path.AddArc(centerX - outerRadius, offsetY - outerRadius, outerRadius * 2, outerRadius * 2, layerStartAngle, sweepAngle);
                        path.AddArc(centerX - innerRadius, offsetY - innerRadius, innerRadius * 2, innerRadius * 2, layerStartAngle + sweepAngle, -sweepAngle);
                        path.CloseFigure();
                        g.FillPath(shadowBrush, path);
                    }
                    
                    layerStartAngle += sweepAngle;
                }
            }

            // Draw main donut segments with gradient
            startAngle = -90;
            foreach (var item in data)
            {
                float sweepAngle = (item.Count / total) * 360;
                
                // Create gradient brush for 3D effect
                using (var gradientBrush = new LinearGradientBrush(
                    new Rectangle(centerX - outerRadius, centerY - outerRadius, outerRadius * 2, outerRadius * 2),
                    item.Color,
                    Color.FromArgb(Math.Max(0, item.Color.R - 40), Math.Max(0, item.Color.G - 40), Math.Max(0, item.Color.B - 40)),
                    LinearGradientMode.Vertical))
                using (var path = new GraphicsPath())
                {
                    path.AddArc(centerX - outerRadius, centerY - outerRadius, outerRadius * 2, outerRadius * 2, startAngle, sweepAngle);
                    path.AddArc(centerX - innerRadius, centerY - innerRadius, innerRadius * 2, innerRadius * 2, startAngle + sweepAngle, -sweepAngle);
                    path.CloseFigure();
                    g.FillPath(gradientBrush, path);
                }

                // Draw segment border for definition
                using (var borderPen = new Pen(Color.White, 2))
                using (var path = new GraphicsPath())
                {
                    path.AddArc(centerX - outerRadius, centerY - outerRadius, outerRadius * 2, outerRadius * 2, startAngle, sweepAngle);
                    path.AddArc(centerX - innerRadius, centerY - innerRadius, innerRadius * 2, innerRadius * 2, startAngle + sweepAngle, -sweepAngle);
                    path.CloseFigure();
                    g.DrawPath(borderPen, path);
                }

                startAngle += sweepAngle;
            }

            // Draw 3D center circle with gradient
            using (var centerGradient = new LinearGradientBrush(
                new Rectangle(centerX - innerRadius, centerY - innerRadius, innerRadius * 2, innerRadius * 2),
                Color.FromArgb(255, 255, 255),
                Color.FromArgb(240, 245, 250),
                LinearGradientMode.Vertical))
            {
                g.FillEllipse(centerGradient, centerX - innerRadius, centerY - innerRadius, innerRadius * 2, innerRadius * 2);
            }

            // Draw center circle border
            using (var centerBorderPen = new Pen(Color.FromArgb(200, 200, 200), 2))
            {
                g.DrawEllipse(centerBorderPen, centerX - innerRadius, centerY - innerRadius, innerRadius * 2, innerRadius * 2);
            }

            // Draw total in center with shadow
            using (var shadowFont = new Font("Segoe UI", 18, FontStyle.Bold))
            using (var shadowBrush = new SolidBrush(Color.FromArgb(50, 0, 0, 0)))
            using (var font = new Font("Segoe UI", 18, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.FromArgb(15, 23, 42)))
            {
                string totalText = ((int)total).ToString();
                var size = g.MeasureString(totalText, font);
                // Shadow
                g.DrawString(totalText, shadowFont, shadowBrush, centerX - size.Width / 2 + 2, centerY - size.Height / 2 + 2);
                // Main text
                g.DrawString(totalText, font, brush, centerX - size.Width / 2, centerY - size.Height / 2);
            }

            // Draw 3D legend with rounded rectangles
            int legendY = 50;
            int legendX = 20;
            using (var font = new Font("Segoe UI", 9))
            {
                foreach (var item in data)
                {
                    // Draw 3D legend box
                    using (var brush = new SolidBrush(item.Color))
                    using (var shadowBrush = new SolidBrush(Color.FromArgb(50, 0, 0, 0)))
                    {
                        // Shadow
                        g.FillRectangle(shadowBrush, legendX + 2, legendY + 2, 12, 12);
                        // Main box
                        g.FillRectangle(brush, legendX, legendY, 12, 12);
                    }

                    using (var borderPen = new Pen(Color.FromArgb(150, 150, 150), 1))
                    {
                        g.DrawRectangle(borderPen, legendX, legendY, 12, 12);
                    }

                    using (var textBrush = new SolidBrush(Color.FromArgb(71, 85, 105)))
                    {
                        string text = $"{item.Status}: {item.Count} ({(item.Count / total * 100):F1}%)";
                        g.DrawString(text, font, textBrush, legendX + 18, legendY - 2);
                    }

                    legendY += 25;
                }
            }
        }

        private Panel CreateModernStatisticsPanel()
        {
            var panel = new Panel
            {
                Size = new Size(1340, 480),
                BackColor = Color.White
            };

            panel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, panel.Width - 1, panel.Height - 1), 10))
                using (var brush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillPath(brush, path);
                }
            };

            var titleLabel = new Label
            {
                Text = "Analytics Overview",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(20, 15),
                AutoSize = true
            };

            timePeriodCombo = new ComboBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(1180, 15),
                Size = new Size(140, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.White
            };
            timePeriodCombo.Items.AddRange(new[] { "Today", "Last 7 days", "This Month", "This Year" });
            timePeriodCombo.SelectedIndex = 0;
            timePeriodCombo.SelectedIndexChanged += async (s, e) => await RefreshCharts();

            // Clean Modern Charts
            var salesChart = CreateCleanBarChart("Sales by Category", 20, 60, 360, 180);
            var frequencyChart = CreateCleanLineChart("Transaction Frequency", 400, 60, 360, 180);
            var revenueChart = CreateCleanAreaChart("Revenue Trend", 780, 60, 360, 180);
            
            var performanceChart = CreateCleanPieChart("Top Products", 20, 260, 360, 200);
            var hourlyChart = CreateCleanColumnChart("Hourly Sales", 400, 260, 360, 200);
            statusChartPanel = CreateCleanDonutChart("Payment Methods", 780, 260, 360, 200);

            panel.Controls.AddRange(new Control[] { 
                titleLabel, timePeriodCombo,
                salesChart, frequencyChart, revenueChart,
                performanceChart, hourlyChart, statusChartPanel
            });

            return panel;
        }

        private Panel CreateCleanBarChart(string title, int x, int y, int width, int height)
        {
            var chart = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = Color.FromArgb(249, 250, 251)
            };

            chart.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                using (var titleFont = new Font("Segoe UI", 10, FontStyle.Bold))
                using (var titleBrush = new SolidBrush(Color.FromArgb(51, 65, 85)))
                {
                    g.DrawString(title, titleFont, titleBrush, 10, 10);
                }

                var data = GetSalesByCategory();
                if (data.Count == 0)
                {
                    DrawNoDataMessage(g, chart);
                    return;
                }

                DrawSimpleBarChart(g, data, chart);
            };

            return chart;
        }

        private Panel CreateCleanLineChart(string title, int x, int y, int width, int height)
        {
            var chart = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = Color.FromArgb(249, 250, 251)
            };

            chart.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                using (var titleFont = new Font("Segoe UI", 10, FontStyle.Bold))
                using (var titleBrush = new SolidBrush(Color.FromArgb(51, 65, 85)))
                {
                    g.DrawString(title, titleFont, titleBrush, 10, 10);
                }

                var data = GetTransactionFrequency();
                if (data.Count == 0)
                {
                    DrawNoDataMessage(g, chart);
                    return;
                }

                DrawSimpleLineChart(g, data, chart);
            };

            return chart;
        }

        private Panel CreateCleanAreaChart(string title, int x, int y, int width, int height)
        {
            var chart = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = Color.FromArgb(249, 250, 251)
            };

            chart.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                using (var titleFont = new Font("Segoe UI", 10, FontStyle.Bold))
                using (var titleBrush = new SolidBrush(Color.FromArgb(51, 65, 85)))
                {
                    g.DrawString(title, titleFont, titleBrush, 10, 10);
                }

                var data = GetAverageTransactionValue();
                if (data.Count == 0)
                {
                    DrawNoDataMessage(g, chart);
                    return;
                }

                DrawSimpleAreaChart(g, data, chart);
            };

            return chart;
        }

        private Panel CreateCleanPieChart(string title, int x, int y, int width, int height)
        {
            var chart = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = Color.FromArgb(249, 250, 251)
            };

            chart.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                using (var titleFont = new Font("Segoe UI", 10, FontStyle.Bold))
                using (var titleBrush = new SolidBrush(Color.FromArgb(51, 65, 85)))
                {
                    g.DrawString(title, titleFont, titleBrush, 10, 10);
                }

                // Draw simple pie chart for top products
                DrawSimplePieChart(g, chart);
            };

            return chart;
        }

        private Panel CreateCleanColumnChart(string title, int x, int y, int width, int height)
        {
            var chart = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = Color.FromArgb(249, 250, 251)
            };

            chart.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                using (var titleFont = new Font("Segoe UI", 10, FontStyle.Bold))
                using (var titleBrush = new SolidBrush(Color.FromArgb(51, 65, 85)))
                {
                    g.DrawString(title, titleFont, titleBrush, 10, 10);
                }

                // Draw hourly sales column chart
                DrawHourlySalesChart(g, chart);
            };

            return chart;
        }

        private Panel CreateCleanDonutChart(string title, int x, int y, int width, int height)
        {
            var chart = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = Color.FromArgb(249, 250, 251)
            };

            chart.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                using (var titleFont = new Font("Segoe UI", 10, FontStyle.Bold))
                using (var titleBrush = new SolidBrush(Color.FromArgb(51, 65, 85)))
                {
                    g.DrawString(title, titleFont, titleBrush, 10, 10);
                }

                var statusData = GetPaymentMethodDistribution();
                if (statusData.Sum(d => d.Count) == 0)
                {
                    DrawNoDataMessage(g, chart);
                    return;
                }

                DrawSimpleDonutChart(g, statusData, chart);
            };

            return chart;
        }

        private Panel CreateInteractiveBottomPanel()
        {
            var panel = new Panel
            {
                Size = new Size(1340, 320),
                BackColor = Color.Transparent
            };

            var invoicesPanel = CreateInteractiveInvoicesPanel();
            invoicesPanel.Location = new Point(0, 0);

            var summaryPanel = CreateCompactSummaryPanel();
            summaryPanel.Location = new Point(940, 0);

            panel.Controls.AddRange(new Control[] { invoicesPanel, summaryPanel });
            return panel;
        }

        private Panel CreateInteractiveInvoicesPanel()
        {
            var panel = new Panel
            {
                Size = new Size(920, 310),
                BackColor = Color.White
            };

            panel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, panel.Width - 1, panel.Height - 1), 10))
                using (var brush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillPath(brush, path);
                }
            };

            var titleLabel = new Label
            {
                Text = "Recent Transactions",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(20, 15),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            var subtitleLabel = new Label
            {
                Text = "Click any row to print invoice",
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(20, 40),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            invoiceGrid = new DataGridView
            {
                Location = new Point(20, 70),
                Size = new Size(880, 225),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Font = new Font("Segoe UI", 9),
                ColumnHeadersHeight = 35,
                RowTemplate = { Height = 32 },
                GridColor = Color.FromArgb(226, 232, 240),
                EnableHeadersVisualStyles = false,
                Cursor = Cursors.Hand
            };

            invoiceGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            invoiceGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(71, 85, 105);
            invoiceGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            invoiceGrid.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);

            invoiceGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            invoiceGrid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 64, 175);

            // Add print icon column
            var printColumn = new DataGridViewImageColumn
            {
                Name = "Print",
                HeaderText = "",
                Width = 40,
                Image = null // Will add icon
            };
            invoiceGrid.Columns.Add(printColumn);

            invoiceGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "InvoiceNumber",
                HeaderText = "Invoice #",
                Width = 120,
                DataPropertyName = "InvoiceNumber"
            });

            invoiceGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ClientName",
                HeaderText = "Customer",
                Width = 180,
                DataPropertyName = "ClientName"
            });

            invoiceGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Status",
                HeaderText = "Status",
                Width = 100,
                DataPropertyName = "Status"
            });

            invoiceGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Amount",
                HeaderText = "Amount",
                Width = 120,
                DataPropertyName = "Amount"
            });

            invoiceGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Date",
                HeaderText = "Date",
                Width = 140,
                DataPropertyName = "Date"
            });

            // Click to print functionality
            invoiceGrid.CellClick += InvoiceGrid_CellClick;
            invoiceGrid.CellMouseEnter += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    invoiceGrid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(240, 249, 255);
                }
            };
            invoiceGrid.CellMouseLeave += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    invoiceGrid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                }
            };

            panel.Controls.AddRange(new Control[] { titleLabel, subtitleLabel, invoiceGrid });
            return panel;
        }

        private async void InvoiceGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            try
            {
                var invoiceNumber = invoiceGrid.Rows[e.RowIndex].Cells["InvoiceNumber"].Value?.ToString();
                if (string.IsNullOrEmpty(invoiceNumber)) return;

                // Find the sale/invoice
                using (var context = new ApplicationDbContext())
                {
                    var sale = await context.Sales
                        .Include(s => s.SaleItems)
                        .ThenInclude(si => si.Product)
                        .Include(s => s.Customer)
                        .Include(s => s.Cashier)
                        .FirstOrDefaultAsync(s => s.InvoiceNumber == invoiceNumber);

                    if (sale != null)
                    {
                        // Show print dialog
                        var result = MessageBox.Show(
                            $"Print invoice {invoiceNumber}?\n\nCustomer: {sale.Customer?.FullName ?? "Walk-in"}\nAmount: {CurrencyService.FormatAuto(sale.TotalAmount)}",
                            "Print Invoice",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            // Print receipt
                            var receipt = new ReceiptPrintView(sale, thermalPrint: true);
                            receipt.ShowDialog();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invoice not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error printing invoice: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Panel CreateCompactSummaryPanel()
        {
            var panel = new Panel
            {
                Size = new Size(380, 310),
                BackColor = Color.White
            };

            panel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, panel.Width - 1, panel.Height - 1), 10))
                using (var brush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillPath(brush, path);
                }
            };

            var titleLabel = new Label
            {
                Text = "Quick Summary",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(20, 15),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            int yPos = 60;
            CreateCompactSummaryRow(panel, "Active Invoices", "$0.00", yPos, out lblActiveInvoices);
            yPos += 45;
            CreateCompactSummaryRow(panel, "Total Products", "0", yPos, out lblProducts);
            yPos += 45;
            CreateCompactSummaryRow(panel, "Total Sales", "0", yPos, out lblStockSales);
            yPos += 45;
            CreateCompactSummaryRow(panel, "Repeat Customers", "0", yPos, out lblRepeatedInvoices);
            yPos += 45;
            CreateCompactSummaryRow(panel, "Payment Links", "0", yPos, out lblPaymentLinks);

            panel.Controls.Add(titleLabel);
            return panel;
        }

        private void CreateCompactSummaryRow(Panel parent, string label, string initialValue, int yPos, out Label valueLabel)
        {
            var labelControl = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(20, yPos),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            valueLabel = new Label
            {
                Text = initialValue,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(220, yPos - 2),
                Size = new Size(100, 25),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleRight
            };

            parent.Controls.AddRange(new Control[] { labelControl, valueLabel });
        }

        private Panel CreateAccountSummaryPanel()
        {
            var panel = new Panel
            {
                Size = new Size(440, 300),
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
                Text = "Account Summary",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(25, 20),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            int yPos = 70;
            CreateSummaryRow(panel, "Total active invoices", "$0.00", yPos, out lblActiveInvoices);
            yPos += 40;
            CreateSummaryRow(panel, "Repeated invoices", "0", yPos, out lblRepeatedInvoices);
            yPos += 40;
            CreateSummaryRow(panel, "Payment links", "0", yPos, out lblPaymentLinks);
            yPos += 40;
            CreateSummaryRow(panel, "Stock sales", "0", yPos, out lblStockSales);
            yPos += 40;
            CreateSummaryRow(panel, "Products", "0", yPos, out lblProducts);

            panel.Controls.Add(titleLabel);
            return panel;
        }

        private void CreateSummaryRow(Panel parent, string label, string initialValue, int yPos, out Label valueLabel)
        {
            var labelControl = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(25, yPos),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            valueLabel = new Label
            {
                Text = initialValue,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(220, yPos),
                Size = new Size(200, 25),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleRight
            };

            parent.Controls.AddRange(new Control[] { labelControl, valueLabel });
        }

        // Data Loading Methods
        private async void LoadDashboardData()
        {
            await RefreshDashboardData();
        }

        private async System.Threading.Tasks.Task RefreshDashboardData()
        {
            try
            {
                // Use a new context instance to avoid DataReader conflicts
                using (var context = new ApplicationDbContext())
                {
                    // Get date range based on selected period
                    var (startDate, endDate) = GetDateRange();

                    // Update metrics with real sales data
                    var salesInPeriod = await context.Sales
                        .AsNoTracking()
                        .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate && s.Status == SaleStatus.Completed)
                        .ToListAsync();

                    var invoiceCount = salesInPeriod.Count;
                    var totalRevenue = salesInPeriod.Sum(s => s.TotalAmount);

                    lblInvoiceCount.Text = $"{invoiceCount} Sale{(invoiceCount != 1 ? "s" : "")}";
                    lblTotalRevenue.Text = FormatCurrency(totalRevenue);

                    // Update account summary
                    var activeInvoicesTotal = await context.Invoices
                        .AsNoTracking()
                        .Where(i => i.Status == InvoiceStatus.Active)
                        .SumAsync(i => (decimal?)i.TotalAmount) ?? 0;
                    lblActiveInvoices.Text = FormatCurrency(activeInvoicesTotal);

                    var repeatedCount = await context.Invoices
                        .AsNoTracking()
                        .GroupBy(i => i.CustomerId)
                        .CountAsync(g => g.Count() > 1);
                    lblRepeatedInvoices.Text = repeatedCount.ToString();

                    var paymentLinksCount = await context.PaymentLinks.AsNoTracking().CountAsync();
                    lblPaymentLinks.Text = paymentLinksCount.ToString();

                    var stockSalesCount = await context.Sales.AsNoTracking().CountAsync();
                    lblStockSales.Text = stockSalesCount.ToString();

                    var productsCount = await context.Products.AsNoTracking().CountAsync(p => p.IsActive);
                    lblProducts.Text = productsCount.ToString();

                    // Update latest invoices grid
                    await LoadLatestInvoices();
                }

                // Refresh all charts
                statusChartPanel?.Invalidate();
            }
            catch (Exception ex)
            {
                // Log error silently
                System.Diagnostics.Debug.WriteLine($"Error refreshing dashboard: {ex.Message}");
            }
        }

        private (DateTime startDate, DateTime endDate) GetDateRange()
        {
            var endDate = DateTime.Now;
            DateTime startDate;

            if (timePeriodCombo == null || timePeriodCombo.SelectedIndex == -1)
            {
                // Default to today
                startDate = DateTime.Today;
            }
            else
            {
                switch (timePeriodCombo.SelectedIndex)
                {
                    case 0: // Today
                        startDate = DateTime.Today;
                        break;
                    case 1: // Last 7 days
                        startDate = DateTime.Today.AddDays(-7);
                        break;
                    case 2: // This Month
                        startDate = new DateTime(endDate.Year, endDate.Month, 1);
                        break;
                    case 3: // This Year
                        startDate = new DateTime(endDate.Year, 1, 1);
                        break;
                    default:
                        startDate = DateTime.Today;
                        break;
                }
            }

            return (startDate, endDate);
        }

        private async System.Threading.Tasks.Task LoadLatestInvoices()
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var invoices = await context.Invoices
                        .AsNoTracking()
                        .Include(i => i.Customer)
                        .OrderByDescending(i => i.CreatedAt)
                        .Take(5)
                        .ToListAsync();

                    var invoiceData = invoices.Select(i => new
                    {
                        InvoiceNumber = i.InvoiceNumber,
                        ClientName = i.Customer != null ? i.Customer.FullName : (i.CustomerName ?? "Walk-in Customer"),
                        Status = i.Status.ToString(),
                        Amount = FormatCurrency(i.TotalAmount),
                        Date = i.CreatedAt.ToString("MMMM dd, yyyy")
                    }).ToList();

                    invoiceGrid.DataSource = invoiceData;

                    // Color code status column
                    foreach (DataGridViewRow row in invoiceGrid.Rows)
                    {
                        var status = row.Cells["Status"].Value?.ToString();
                        if (status == "Paid")
                        {
                            row.Cells["Status"].Style.ForeColor = Color.FromArgb(34, 197, 94);
                            row.Cells["Status"].Style.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                        }
                        else if (status == "Active")
                        {
                            row.Cells["Status"].Style.ForeColor = Color.FromArgb(249, 115, 22);
                            row.Cells["Status"].Style.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                        }
                        else if (status == "Overdue")
                        {
                            row.Cells["Status"].Style.ForeColor = Color.FromArgb(239, 68, 68);
                            row.Cells["Status"].Style.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading invoices: {ex.Message}");
            }
        }

        private async System.Threading.Tasks.Task RefreshCharts()
        {
            statusChartPanel?.Invalidate();
            await RefreshDashboardData();
        }

        private List<(DateTime Date, decimal Amount, int Count)> GetInvoiceTrendData()
        {
            try
            {
                var endDate = DateTime.Today;
                var startDate = endDate.AddDays(-30);

                var data = _context.Invoices
                    .Where(i => i.CreatedAt >= startDate && i.CreatedAt <= endDate)
                    .GroupBy(i => i.CreatedAt.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        Amount = g.Sum(i => i.TotalAmount),
                        Count = g.Count()
                    })
                    .OrderBy(x => x.Date)
                    .ToList();

                // Fill in missing dates with zero values
                var result = new List<(DateTime Date, decimal Amount, int Count)>();
                for (var date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    var dayData = data.FirstOrDefault(d => d.Date == date);
                    if (dayData != null)
                    {
                        result.Add((date, dayData.Amount, dayData.Count));
                    }
                    else
                    {
                        result.Add((date, 0, 0));
                    }
                }

                return result;
            }
            catch
            {
                return new List<(DateTime Date, decimal Amount, int Count)>();
            }
        }

        private List<(string Status, int Count, Color Color)> GetStatusDistribution()
        {
            try
            {
                var statusCounts = _context.Invoices
                    .GroupBy(i => i.Status)
                    .Select(g => new { Status = g.Key, Count = g.Count() })
                    .ToList();

                var result = new List<(string Status, int Count, Color Color)>();

                foreach (var item in statusCounts)
                {
                    Color color = item.Status switch
                    {
                        InvoiceStatus.Paid => Color.FromArgb(34, 197, 94),
                        InvoiceStatus.Active => Color.FromArgb(59, 130, 246),
                        InvoiceStatus.Overdue => Color.FromArgb(239, 68, 68),
                        InvoiceStatus.Void => Color.FromArgb(148, 163, 184),
                        _ => Color.FromArgb(100, 116, 139)
                    };

                    result.Add((item.Status.ToString(), item.Count, color));
                }

                return result;
            }
            catch
            {
                return new List<(string Status, int Count, Color Color)>();
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
                _refreshTimer?.Stop();
                _refreshTimer?.Dispose();
                _context?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
