using System.Drawing;
using System.Drawing.Drawing2D;
using SyncVerseStudio.Services;
using SyncVerseStudio.Data;
using SyncVerseStudio.Helpers;
using Microsoft.EntityFrameworkCore;
using FontAwesome.Sharp;

namespace SyncVerseStudio.Views
{
    public partial class ReportsView : Form
    {
        private readonly AuthenticationService _authService;
        private readonly ApplicationDbContext _context;
        private Dictionary<string, Label> _metricLabels = new Dictionary<string, Label>();
        private Dictionary<string, Label> _trendLabels = new Dictionary<string, Label>();
        private DateTimePicker _fromDatePicker;
        private DateTimePicker _toDatePicker;
        private ComboBox _periodComboBox;

        public ReportsView(AuthenticationService authService)
        {
            _authService = authService;
            _context = new ApplicationDbContext();
            InitializeComponent();
            LoadSummaryData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            this.Name = "ReportsView";
            this.Text = "Reports & Analytics";
            this.WindowState = FormWindowState.Normal;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.ClientSize = new Size(1200, 800);
            this.Padding = new Padding(0);
            
            CreateHeaderPanel();
            CreateContentPanel();
            
            this.ResumeLayout(false);
        }

        private void CreateHeaderPanel()
        {
            var headerPanel = new Panel
            {
                BackColor = Color.White,
                Dock = DockStyle.Top,
                Height = 140,
                Padding = new Padding(30, 20, 30, 20)
            };

            // Icon and Title
            var iconBox = new IconPictureBox
            {
                IconChar = IconChar.ChartLine,
                IconColor = Color.FromArgb(59, 130, 246),
                IconSize = 36,
                Location = new Point(30, 22),
                Size = new Size(36, 36),
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(iconBox);

            var titleLabel = new Label
            {
                Text = "Reports & Analytics",
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(75, 22),
                Size = new Size(400, 35),
                AutoSize = false,
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(titleLabel);

            var subtitleLabel = new Label
            {
                Text = "Comprehensive Reporting Suite • Real-time Business Intelligence",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(120, 120, 120),
                Location = new Point(75, 52),
                Size = new Size(500, 20),
                AutoSize = false,
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(subtitleLabel);

            // Date Range Controls
            int controlY = 85;
            
            var fromLabel = new Label
            {
                Text = "From:",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(80, 80, 80),
                Location = new Point(30, controlY + 5),
                Size = new Size(40, 20),
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(fromLabel);

            _fromDatePicker = new DateTimePicker
            {
                Location = new Point(75, controlY),
                Size = new Size(130, 30),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today.AddMonths(-1),
                Font = new Font("Segoe UI", 9F)
            };
            _fromDatePicker.ValueChanged += (s, e) => LoadSummaryData();
            headerPanel.Controls.Add(_fromDatePicker);

            var toLabel = new Label
            {
                Text = "To:",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(80, 80, 80),
                Location = new Point(220, controlY + 5),
                Size = new Size(25, 20),
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(toLabel);

            _toDatePicker = new DateTimePicker
            {
                Location = new Point(250, controlY),
                Size = new Size(130, 30),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today,
                Font = new Font("Segoe UI", 9F)
            };
            _toDatePicker.ValueChanged += (s, e) => LoadSummaryData();
            headerPanel.Controls.Add(_toDatePicker);

            _periodComboBox = new ComboBox
            {
                Location = new Point(395, controlY),
                Size = new Size(120, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F),
                FlatStyle = FlatStyle.Flat
            };
            _periodComboBox.Items.AddRange(new object[] { "This Month", "Last Month", "This Quarter", "This Year", "Custom" });
            _periodComboBox.SelectedIndex = 0;
            _periodComboBox.SelectedIndexChanged += PeriodComboBox_SelectedIndexChanged;
            headerPanel.Controls.Add(_periodComboBox);

            // Export Buttons
            int btnX = 670;
            int btnSpacing = 5;

            var refreshBtn = CreateIconButton(IconChar.Sync, "", Color.FromArgb(59, 130, 246), btnX, controlY, 40);
            refreshBtn.Click += (s, e) => LoadSummaryData();
            headerPanel.Controls.Add(refreshBtn);
            btnX += 40 + btnSpacing;

            var pdfBtn = CreateIconButton(IconChar.FilePdf, "PDF", Color.FromArgb(239, 68, 68), btnX, controlY, 70);
            pdfBtn.Click += (s, e) => MessageBox.Show("Export to PDF - Coming Soon!", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
            headerPanel.Controls.Add(pdfBtn);
            btnX += 70 + btnSpacing;

            var excelBtn = CreateIconButton(IconChar.FileExcel, "Excel", Color.FromArgb(34, 197, 94), btnX, controlY, 75);
            excelBtn.Click += (s, e) => MessageBox.Show("Export to Excel - Coming Soon!", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
            headerPanel.Controls.Add(excelBtn);
            btnX += 75 + btnSpacing;

            var csvBtn = CreateIconButton(IconChar.FileCsv, "CSV", Color.FromArgb(168, 85, 247), btnX, controlY, 70);
            csvBtn.Click += (s, e) => MessageBox.Show("Export to CSV - Coming Soon!", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
            headerPanel.Controls.Add(csvBtn);

            this.Controls.Add(headerPanel);
        }

        private void PeriodComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var today = DateTime.Today;
            switch (_periodComboBox.SelectedIndex)
            {
                case 0: // This Month
                    _fromDatePicker.Value = new DateTime(today.Year, today.Month, 1);
                    _toDatePicker.Value = today;
                    break;
                case 1: // Last Month
                    var lastMonth = today.AddMonths(-1);
                    _fromDatePicker.Value = new DateTime(lastMonth.Year, lastMonth.Month, 1);
                    _toDatePicker.Value = new DateTime(lastMonth.Year, lastMonth.Month, DateTime.DaysInMonth(lastMonth.Year, lastMonth.Month));
                    break;
                case 2: // This Quarter
                    var quarter = (today.Month - 1) / 3;
                    _fromDatePicker.Value = new DateTime(today.Year, quarter * 3 + 1, 1);
                    _toDatePicker.Value = today;
                    break;
                case 3: // This Year
                    _fromDatePicker.Value = new DateTime(today.Year, 1, 1);
                    _toDatePicker.Value = today;
                    break;
            }
            LoadSummaryData();
        }

        private IconButton CreateIconButton(IconChar icon, string text, Color backgroundColor, int x, int y, int width)
        {
            var btn = new IconButton
            {
                Text = text,
                IconChar = icon,
                IconColor = Color.White,
                IconSize = 16,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                BackColor = backgroundColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(x, y),
                Size = new Size(width, 30),
                Cursor = Cursors.Hand,
                Padding = new Padding(string.IsNullOrEmpty(text) ? 0 : 5, 0, 5, 0)
            };
            btn.FlatAppearance.BorderSize = 0;
            if (string.IsNullOrEmpty(text))
            {
                btn.TextAlign = ContentAlignment.MiddleCenter;
                btn.ImageAlign = ContentAlignment.MiddleCenter;
            }
            return btn;
        }

        private void CreateContentPanel()
        {
            var contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(245, 247, 250),
                Padding = new Padding(30, 20, 30, 20),
                AutoScroll = true
            };

            // Metrics Cards Row
            var metricsFlow = new FlowLayoutPanel
            {
                Location = new Point(0, 0),
                Size = new Size(1140, 110),
                BackColor = Color.Transparent,
                WrapContents = true
            };

            CreateMetricCard(metricsFlow, IconChar.DollarSign, "Total Sales", "$22.60", "+2.28%", Color.FromArgb(59, 130, 246), "sales", true);
            CreateMetricCard(metricsFlow, IconChar.ChartLine, "Revenue", "$22.60", "+2.28%", Color.FromArgb(34, 197, 94), "revenue", true);
            CreateMetricCard(metricsFlow, IconChar.MoneyBillTrendUp, "Profit", "$8.25", "+0.25%", Color.FromArgb(168, 85, 247), "profit", true);
            CreateMetricCard(metricsFlow, IconChar.Receipt, "Transactions", "3", "+2.28%", Color.FromArgb(249, 115, 22), "transactions", true);
            CreateMetricCard(metricsFlow, IconChar.ChartBar, "Avg Transaction", "$7.53", "+0.28%", Color.FromArgb(236, 72, 153), "avgtrans", true);
            CreateMetricCard(metricsFlow, IconChar.Star, "Top Product", "Coca Cola ...", "+0.28%", Color.FromArgb(14, 165, 233), "topproduct", false);

            contentPanel.Controls.Add(metricsFlow);

            // Charts Section
            int chartY = 130;
            
            var salesChartPanel = CreateChartPanel("Sales Analytics Trends", IconChar.ChartArea, chartY, 0, 560);
            contentPanel.Controls.Add(salesChartPanel);

            var productsChartPanel = CreateChartPanel("Top Products & Categories", IconChar.ChartPie, chartY, 580, 560);
            contentPanel.Controls.Add(productsChartPanel);

            // Available Reports Section
            var reportsLabel = new Label
            {
                Text = "Available Reports",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(0, 520),
                Size = new Size(300, 35),
                BackColor = Color.Transparent
            };
            contentPanel.Controls.Add(reportsLabel);

            var reportsPanel = new Panel
            {
                Location = new Point(0, 565),
                Size = new Size(1140, 200),
                BackColor = Color.White,
                AutoScroll = true
            };

            reportsPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, reportsPanel.Width - 1, reportsPanel.Height - 1), 12))
                using (var brush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillPath(brush, path);
                }
            };

            CreateReportItem(reportsPanel, IconChar.CalendarDays, "Daily, Weekly, Monthly Reports", "Generate comprehensive time-based reports", 20, 20);
            CreateReportItem(reportsPanel, IconChar.ShieldHalved, "Loss Prevention Analytics", "Track and analyze inventory shrinkage", 20, 80);
            CreateReportItem(reportsPanel, IconChar.ArrowsLeftRight, "Comparative Period Analysis", "Compare performance across time periods", 400, 20);
            CreateReportItem(reportsPanel, IconChar.FileExport, "Export to PDF, Excel, CSV", "Download reports in multiple formats", 400, 80);

            contentPanel.Controls.Add(reportsPanel);
            this.Controls.Add(contentPanel);
        }

        private Panel CreateChartPanel(string title, IconChar icon, int y, int x, int width)
        {
            var panel = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(width, 370),
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
                Text = title,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(20, 20),
                Size = new Size(400, 30),
                BackColor = Color.Transparent
            };
            panel.Controls.Add(titleLabel);

            var iconBox = new IconPictureBox
            {
                IconChar = icon,
                IconColor = Color.FromArgb(59, 130, 246),
                IconSize = 24,
                Location = new Point(width - 50, 20),
                Size = new Size(24, 24),
                BackColor = Color.Transparent
            };
            panel.Controls.Add(iconBox);

            var placeholderPanel = new Panel
            {
                Location = new Point(20, 70),
                Size = new Size(width - 40, 280),
                BackColor = Color.FromArgb(248, 250, 252)
            };

            placeholderPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, placeholderPanel.Width - 1, placeholderPanel.Height - 1), 8))
                using (var brush = new SolidBrush(Color.FromArgb(248, 250, 252)))
                {
                    e.Graphics.FillPath(brush, path);
                }

                // Draw placeholder content
                using (var font = new Font("Segoe UI", 10F))
                using (var brush = new SolidBrush(Color.FromArgb(150, 150, 150)))
                {
                    var text = title.Contains("Sales") ? "Sales trend chart\nDaily, weekly, monthly patterns" : "Top products chart\nBest categories and items";
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    e.Graphics.DrawString(text, font, brush, new RectangleF(0, 0, placeholderPanel.Width, placeholderPanel.Height), sf);
                }
            };

            panel.Controls.Add(placeholderPanel);
            return panel;
        }

        private void CreateReportItem(Panel parent, IconChar icon, string title, string description, int x, int y)
        {
            var itemPanel = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(340, 50),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };

            var iconBox = new IconPictureBox
            {
                IconChar = icon,
                IconColor = Color.FromArgb(59, 130, 246),
                IconSize = 28,
                Location = new Point(5, 11),
                Size = new Size(28, 28),
                BackColor = Color.Transparent
            };
            itemPanel.Controls.Add(iconBox);

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(45, 5),
                Size = new Size(285, 22),
                BackColor = Color.Transparent
            };
            itemPanel.Controls.Add(titleLabel);

            var descLabel = new Label
            {
                Text = description,
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.FromArgb(120, 120, 120),
                Location = new Point(45, 27),
                Size = new Size(285, 18),
                BackColor = Color.Transparent
            };
            itemPanel.Controls.Add(descLabel);

            itemPanel.MouseEnter += (s, e) => itemPanel.BackColor = Color.FromArgb(248, 250, 252);
            itemPanel.MouseLeave += (s, e) => itemPanel.BackColor = Color.Transparent;
            itemPanel.Click += (s, e) => MessageBox.Show($"{title}\n\n{description}\n\nComing Soon!", "Report", MessageBoxButtons.OK, MessageBoxIcon.Information);

            parent.Controls.Add(itemPanel);
        }

        private void CreateMetricCard(FlowLayoutPanel parent, IconChar icon, string title, string value, string trend, Color color, string tag, bool showTrend)
        {
            var card = new Panel
            {
                Size = new Size(180, 90),
                Margin = new Padding(0, 0, 10, 10),
                BackColor = Color.White,
                Tag = tag
            };

            card.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var rect = card.ClientRectangle;
                
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, rect.Width - 1, rect.Height - 1), 10))
                using (var brush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillPath(brush, path);
                }
            };

            // Icon with background
            var iconPanel = new Panel
            {
                Size = new Size(40, 40),
                Location = new Point(15, 15),
                BackColor = Color.FromArgb(15, color)
            };
            iconPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var brush = new SolidBrush(Color.FromArgb(20, color)))
                {
                    e.Graphics.FillEllipse(brush, 0, 0, 40, 40);
                }
            };
            card.Controls.Add(iconPanel);

            var iconBox = new IconPictureBox
            {
                IconChar = icon,
                IconColor = color,
                IconSize = 20,
                Location = new Point(10, 10),
                Size = new Size(20, 20),
                BackColor = Color.Transparent,
                Parent = iconPanel
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.FromArgb(110, 110, 110),
                Location = new Point(15, 60),
                Size = new Size(150, 16),
                BackColor = Color.Transparent
            };
            card.Controls.Add(titleLabel);

            var valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(15, 12),
                Size = new Size(110, 24),
                BackColor = Color.Transparent,
                Tag = "value",
                TextAlign = ContentAlignment.MiddleRight
            };
            card.Controls.Add(valueLabel);
            _metricLabels[tag] = valueLabel;

            if (showTrend)
            {
                var trendLabel = new Label
                {
                    Text = trend,
                    Font = new Font("Segoe UI", 8F),
                    ForeColor = trend.StartsWith("+") ? Color.FromArgb(34, 197, 94) : Color.FromArgb(239, 68, 68),
                    Location = new Point(130, 15),
                    Size = new Size(45, 16),
                    BackColor = Color.Transparent,
                    Tag = "trend",
                    TextAlign = ContentAlignment.MiddleRight
                };
                card.Controls.Add(trendLabel);
                _trendLabels[tag] = trendLabel;
            }

            parent.Controls.Add(card);
        }



        private async void LoadSummaryData()
        {
            try
            {
                var fromDate = _fromDatePicker?.Value.Date ?? DateTime.Today.AddMonths(-1);
                var toDate = _toDatePicker?.Value.Date ?? DateTime.Today;

                // Calculate previous period for comparison
                var periodDays = (toDate - fromDate).Days;
                var prevFromDate = fromDate.AddDays(-periodDays);
                var prevToDate = fromDate.AddDays(-1);

                // Current period data
                var currentSales = await _context.Sales
                    .Where(s => s.SaleDate >= fromDate && s.SaleDate <= toDate && s.Status == Models.SaleStatus.Completed)
                    .SumAsync(s => (decimal?)s.TotalAmount) ?? 0;

                var currentCost = await _context.Sales
                    .Where(s => s.SaleDate >= fromDate && s.SaleDate <= toDate && s.Status == Models.SaleStatus.Completed)
                    .SelectMany(s => s.SaleItems)
                    .SumAsync(si => (decimal?)(si.Quantity * si.Product.CostPrice)) ?? 0;

                var currentProfit = currentSales - currentCost;

                var currentTransactions = await _context.Sales
                    .CountAsync(s => s.SaleDate >= fromDate && s.SaleDate <= toDate && s.Status == Models.SaleStatus.Completed);

                var avgTransaction = currentTransactions > 0 ? currentSales / currentTransactions : 0;

                // Previous period data for trends
                var prevSales = await _context.Sales
                    .Where(s => s.SaleDate >= prevFromDate && s.SaleDate <= prevToDate && s.Status == Models.SaleStatus.Completed)
                    .SumAsync(s => (decimal?)s.TotalAmount) ?? 0;

                var prevProfit = prevSales - (await _context.Sales
                    .Where(s => s.SaleDate >= prevFromDate && s.SaleDate <= prevToDate && s.Status == Models.SaleStatus.Completed)
                    .SelectMany(s => s.SaleItems)
                    .SumAsync(si => (decimal?)(si.Quantity * si.Product.CostPrice)) ?? 0);

                var prevTransactions = await _context.Sales
                    .CountAsync(s => s.SaleDate >= prevFromDate && s.SaleDate <= prevToDate && s.Status == Models.SaleStatus.Completed);

                var prevAvgTransaction = prevTransactions > 0 ? prevSales / prevTransactions : 0;

                // Top product
                var topProduct = await _context.Sales
                    .Where(s => s.SaleDate >= fromDate && s.SaleDate <= toDate && s.Status == Models.SaleStatus.Completed)
                    .SelectMany(s => s.SaleItems)
                    .GroupBy(si => si.Product.Name)
                    .Select(g => new { Name = g.Key, Total = g.Sum(si => si.Quantity * si.UnitPrice) })
                    .OrderByDescending(x => x.Total)
                    .FirstOrDefaultAsync();

                // Calculate trends
                var salesTrend = CalculateTrend(currentSales, prevSales);
                var profitTrend = CalculateTrend(currentProfit, prevProfit);
                var transTrend = CalculateTrend(currentTransactions, prevTransactions);
                var avgTrend = CalculateTrend(avgTransaction, prevAvgTransaction);

                // Update metric labels
                if (_metricLabels.ContainsKey("sales"))
                    _metricLabels["sales"].Text = $"${currentSales:N2}";
                if (_metricLabels.ContainsKey("revenue"))
                    _metricLabels["revenue"].Text = $"${currentSales:N2}";
                if (_metricLabels.ContainsKey("profit"))
                    _metricLabels["profit"].Text = $"${currentProfit:N2}";
                if (_metricLabels.ContainsKey("transactions"))
                    _metricLabels["transactions"].Text = currentTransactions.ToString();
                if (_metricLabels.ContainsKey("avgtrans"))
                    _metricLabels["avgtrans"].Text = $"${avgTransaction:N2}";
                if (_metricLabels.ContainsKey("topproduct"))
                    _metricLabels["topproduct"].Text = topProduct?.Name ?? "N/A";

                // Update trend labels
                if (_trendLabels.ContainsKey("sales"))
                {
                    _trendLabels["sales"].Text = salesTrend;
                    _trendLabels["sales"].ForeColor = salesTrend.StartsWith("+") ? Color.FromArgb(34, 197, 94) : Color.FromArgb(239, 68, 68);
                }
                if (_trendLabels.ContainsKey("revenue"))
                {
                    _trendLabels["revenue"].Text = salesTrend;
                    _trendLabels["revenue"].ForeColor = salesTrend.StartsWith("+") ? Color.FromArgb(34, 197, 94) : Color.FromArgb(239, 68, 68);
                }
                if (_trendLabels.ContainsKey("profit"))
                {
                    _trendLabels["profit"].Text = profitTrend;
                    _trendLabels["profit"].ForeColor = profitTrend.StartsWith("+") ? Color.FromArgb(34, 197, 94) : Color.FromArgb(239, 68, 68);
                }
                if (_trendLabels.ContainsKey("transactions"))
                {
                    _trendLabels["transactions"].Text = transTrend;
                    _trendLabels["transactions"].ForeColor = transTrend.StartsWith("+") ? Color.FromArgb(34, 197, 94) : Color.FromArgb(239, 68, 68);
                }
                if (_trendLabels.ContainsKey("avgtrans"))
                {
                    _trendLabels["avgtrans"].Text = avgTrend;
                    _trendLabels["avgtrans"].ForeColor = avgTrend.StartsWith("+") ? Color.FromArgb(34, 197, 94) : Color.FromArgb(239, 68, 68);
                }
            }
            catch { }
        }

        private string CalculateTrend(decimal current, decimal previous)
        {
            if (previous == 0) return current > 0 ? "+100%" : "0%";
            var change = ((current - previous) / previous) * 100;
            return $"{(change >= 0 ? "+" : "")}{change:N2}%";
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
