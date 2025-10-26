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
        private Panel trendChartPanel;
        private Panel statusChartPanel;
        
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
            yPos += 80;

            // Metric Cards
            var metricsPanel = CreateMetricsPanel();
            metricsPanel.Location = new Point(0, yPos);
            mainPanel.Controls.Add(metricsPanel);
            yPos += 130;

            // Statistics Charts
            var statsPanel = CreateStatisticsPanel();
            statsPanel.Location = new Point(0, yPos);
            mainPanel.Controls.Add(statsPanel);
            yPos += 380;

            // Latest Invoices and Account Summary
            var bottomPanel = CreateBottomPanel();
            bottomPanel.Location = new Point(0, yPos);
            mainPanel.Controls.Add(bottomPanel);

            this.Controls.Add(mainPanel);
            this.ResumeLayout(false);
        }

        private Panel CreateHeader()
        {
            var panel = new Panel
            {
                Size = new Size(1340, 70),
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
                IconChar = IconChar.ChartLine,
                IconColor = Color.FromArgb(20, 184, 166),
                IconSize = 32,
                Location = new Point(25, 19),
                Size = new Size(32, 32),
                BackColor = Color.Transparent
            };

            var syncLabel = new Label
            {
                Text = "SYNCVERSE",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 184, 166),
                Location = new Point(65, 18),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            var urlLabel = new Label
            {
                Text = "https://syncverse.studio/sync",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(220, 25),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            var dateLabel = new Label
            {
                Text = DateTime.Now.ToString("MMM dd, yyyy, HH:mm"),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(1180, 25),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            // Auto-update time
            var timeTimer = new System.Windows.Forms.Timer { Interval = 60000 };
            timeTimer.Tick += (s, e) => dateLabel.Text = DateTime.Now.ToString("MMM dd, yyyy, HH:mm");
            timeTimer.Start();

            panel.Controls.AddRange(new Control[] { logoIcon, syncLabel, urlLabel, dateLabel });
            return panel;
        }

        private Panel CreateMetricsPanel()
        {
            var panel = new Panel
            {
                Size = new Size(1340, 120),
                BackColor = Color.Transparent
            };

            var invoiceCard = CreateMetricCard("Invoices count", "0 Invoice", IconChar.FileInvoice, 
                Color.FromArgb(59, 130, 246), out lblInvoiceCount);
            invoiceCard.Location = new Point(0, 0);

            var revenueCard = CreateMetricCard("Total paid invoices", "0 KHR", IconChar.MoneyBillWave, 
                Color.FromArgb(34, 197, 94), out lblTotalRevenue);
            revenueCard.Location = new Point(680, 0);

            panel.Controls.AddRange(new Control[] { invoiceCard, revenueCard });
            return panel;
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
                
                // Add subtle shadow
                using (var shadowPath = GetRoundedRectPath(new Rectangle(2, 2, card.Width - 3, card.Height - 3), 12))
                using (var shadowBrush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                {
                    e.Graphics.FillPath(shadowBrush, shadowPath);
                }
            };

            var iconPanel = new Panel
            {
                Size = new Size(60, 60),
                Location = new Point(25, 25),
                BackColor = Color.FromArgb(20, iconColor)
            };
            iconPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var brush = new SolidBrush(Color.FromArgb(20, iconColor)))
                {
                    e.Graphics.FillEllipse(brush, 0, 0, 60, 60);
                }
            };

            var iconBox = new IconPictureBox
            {
                IconChar = icon,
                IconColor = iconColor,
                IconSize = 32,
                Location = new Point(14, 14),
                Size = new Size(32, 32),
                BackColor = Color.Transparent,
                Parent = iconPanel
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(100, 28),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            valueLabel = new Label
            {
                Text = initialValue,
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(100, 52),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            // Hover effect
            card.MouseEnter += (s, e) => card.BackColor = Color.FromArgb(249, 250, 251);
            card.MouseLeave += (s, e) => card.BackColor = Color.White;

            card.Controls.AddRange(new Control[] { iconPanel, titleLabel, valueLabel });
            return card;
        }

        private Panel CreateStatisticsPanel()
        {
            var panel = new Panel
            {
                Size = new Size(1340, 360),
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
                Text = "Statistics",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(25, 20),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            var dateRangeCombo = new ComboBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(1170, 20),
                Size = new Size(145, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.White
            };
            dateRangeCombo.Items.AddRange(new[] { "Last 7 days", "Last 30 days", "Last 90 days", "This Year" });
            dateRangeCombo.SelectedIndex = 0;
            dateRangeCombo.SelectedIndexChanged += async (s, e) => await RefreshCharts();

            // Trend Chart Panel
            trendChartPanel = new Panel
            {
                Location = new Point(25, 70),
                Size = new Size(850, 270),
                BackColor = Color.FromArgb(249, 250, 251),
                BorderStyle = BorderStyle.None
            };
            trendChartPanel.Paint += TrendChartPanel_Paint;

            // Status Distribution Chart Panel
            statusChartPanel = new Panel
            {
                Location = new Point(895, 70),
                Size = new Size(420, 270),
                BackColor = Color.FromArgb(249, 250, 251),
                BorderStyle = BorderStyle.None
            };
            statusChartPanel.Paint += StatusChartPanel_Paint;

            panel.Controls.AddRange(new Control[] { titleLabel, dateRangeCombo, trendChartPanel, statusChartPanel });
            return panel;
        }

        private void TrendChartPanel_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw title
            using (var titleFont = new Font("Segoe UI", 12, FontStyle.Bold))
            using (var titleBrush = new SolidBrush(Color.FromArgb(51, 65, 85)))
            {
                g.DrawString("Invoice Trends Chart", titleFont, titleBrush, 15, 10);
            }

            // Get data from database
            var invoiceData = GetInvoiceTrendData();
            if (invoiceData.Count == 0)
            {
                using (var font = new Font("Segoe UI", 11, FontStyle.Italic))
                using (var brush = new SolidBrush(Color.FromArgb(148, 163, 184)))
                {
                    var text = "No data available";
                    var size = g.MeasureString(text, font);
                    g.DrawString(text, font, brush, 
                        (trendChartPanel.Width - size.Width) / 2, 
                        (trendChartPanel.Height - size.Height) / 2);
                }
                return;
            }

            DrawLineChart(g, invoiceData);
        }

        private void DrawLineChart(Graphics g, List<(DateTime Date, decimal Amount, int Count)> data)
        {
            int chartX = 50;
            int chartY = 50;
            int chartWidth = trendChartPanel.Width - 80;
            int chartHeight = trendChartPanel.Height - 80;

            // Draw axes
            using (var axisPen = new Pen(Color.FromArgb(203, 213, 225), 2))
            {
                g.DrawLine(axisPen, chartX, chartY + chartHeight, chartX + chartWidth, chartY + chartHeight); // X-axis
                g.DrawLine(axisPen, chartX, chartY, chartX, chartY + chartHeight); // Y-axis
            }

            if (data.Count < 2) return;

            // Calculate scales
            decimal maxAmount = data.Max(d => d.Amount);
            if (maxAmount == 0) maxAmount = 100;

            // Draw grid lines
            using (var gridPen = new Pen(Color.FromArgb(241, 245, 249), 1))
            {
                for (int i = 0; i <= 5; i++)
                {
                    int y = chartY + (chartHeight * i / 5);
                    g.DrawLine(gridPen, chartX, y, chartX + chartWidth, y);
                }
            }

            // Draw line
            var points = new List<PointF>();
            for (int i = 0; i < data.Count; i++)
            {
                float x = chartX + (chartWidth * i / (float)(data.Count - 1));
                float y = chartY + chartHeight - (float)(data[i].Amount / maxAmount * chartHeight);
                points.Add(new PointF(x, y));
            }

            // Draw gradient fill
            if (points.Count > 1)
            {
                var fillPoints = new List<PointF>(points);
                fillPoints.Add(new PointF(points[points.Count - 1].X, chartY + chartHeight));
                fillPoints.Add(new PointF(points[0].X, chartY + chartHeight));

                using (var fillBrush = new LinearGradientBrush(
                    new PointF(0, chartY),
                    new PointF(0, chartY + chartHeight),
                    Color.FromArgb(100, 59, 130, 246),
                    Color.FromArgb(10, 59, 130, 246)))
                {
                    g.FillPolygon(fillBrush, fillPoints.ToArray());
                }

                // Draw line
                using (var linePen = new Pen(Color.FromArgb(59, 130, 246), 3))
                {
                    g.DrawLines(linePen, points.ToArray());
                }

                // Draw points
                foreach (var point in points)
                {
                    using (var pointBrush = new SolidBrush(Color.FromArgb(59, 130, 246)))
                    {
                        g.FillEllipse(pointBrush, point.X - 4, point.Y - 4, 8, 8);
                    }
                }
            }

            // Draw labels
            using (var labelFont = new Font("Segoe UI", 8))
            using (var labelBrush = new SolidBrush(Color.FromArgb(100, 116, 139)))
            {
                // Y-axis labels
                for (int i = 0; i <= 5; i++)
                {
                    decimal value = maxAmount * (5 - i) / 5;
                    int y = chartY + (chartHeight * i / 5);
                    g.DrawString($"{value:N0}", labelFont, labelBrush, 5, y - 8);
                }

                // X-axis labels (dates)
                for (int i = 0; i < Math.Min(data.Count, 7); i++)
                {
                    int index = i * (data.Count - 1) / 6;
                    float x = chartX + (chartWidth * index / (float)(data.Count - 1));
                    string dateStr = data[index].Date.ToString("MMM dd");
                    var size = g.MeasureString(dateStr, labelFont);
                    g.DrawString(dateStr, labelFont, labelBrush, x - size.Width / 2, chartY + chartHeight + 5);
                }
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

            float total = data.Sum(d => d.Count);
            float startAngle = -90;

            // Draw donut segments
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

            // Draw center circle
            using (var centerBrush = new SolidBrush(Color.FromArgb(249, 250, 251)))
            {
                g.FillEllipse(centerBrush, centerX - innerRadius, centerY - innerRadius, innerRadius * 2, innerRadius * 2);
            }

            // Draw total in center
            using (var font = new Font("Segoe UI", 18, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.FromArgb(15, 23, 42)))
            {
                string totalText = ((int)total).ToString();
                var size = g.MeasureString(totalText, font);
                g.DrawString(totalText, font, brush, centerX - size.Width / 2, centerY - size.Height / 2);
            }

            // Draw legend
            int legendY = 50;
            int legendX = 20;
            using (var font = new Font("Segoe UI", 9))
            {
                foreach (var item in data)
                {
                    using (var brush = new SolidBrush(item.Color))
                    {
                        g.FillRectangle(brush, legendX, legendY, 12, 12);
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

        private Panel CreateBottomPanel()
        {
            var panel = new Panel
            {
                Size = new Size(1340, 300),
                BackColor = Color.Transparent
            };

            var invoicesPanel = CreateLatestInvoicesPanel();
            invoicesPanel.Location = new Point(0, 0);

            var summaryPanel = CreateAccountSummaryPanel();
            summaryPanel.Location = new Point(900, 0);

            panel.Controls.AddRange(new Control[] { invoicesPanel, summaryPanel });
            return panel;
        }

        private Panel CreateLatestInvoicesPanel()
        {
            var panel = new Panel
            {
                Size = new Size(880, 300),
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
                Text = "Latest invoices",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(25, 20),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            invoiceGrid = new DataGridView
            {
                Location = new Point(25, 65),
                Size = new Size(830, 215),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Font = new Font("Segoe UI", 10),
                ColumnHeadersHeight = 40,
                RowTemplate = { Height = 35 },
                GridColor = Color.FromArgb(226, 232, 240),
                EnableHeadersVisualStyles = false
            };

            invoiceGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            invoiceGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(71, 85, 105);
            invoiceGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            invoiceGrid.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);

            invoiceGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            invoiceGrid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 64, 175);

            invoiceGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "InvoiceNumber",
                HeaderText = "Invoice Number",
                Width = 150,
                DataPropertyName = "InvoiceNumber"
            });

            invoiceGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ClientName",
                HeaderText = "Client Name",
                Width = 200,
                DataPropertyName = "ClientName"
            });

            invoiceGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Status",
                HeaderText = "Status",
                Width = 120,
                DataPropertyName = "Status"
            });

            invoiceGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Amount",
                HeaderText = "Amount",
                Width = 150,
                DataPropertyName = "Amount"
            });

            invoiceGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Date",
                HeaderText = "Date",
                Width = 180,
                DataPropertyName = "Date"
            });

            panel.Controls.AddRange(new Control[] { titleLabel, invoiceGrid });
            return panel;
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
            CreateSummaryRow(panel, "Total active invoices", "0 KHR", yPos, out lblActiveInvoices);
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
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(320, yPos),
                AutoSize = true,
                BackColor = Color.Transparent
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
                // Update metrics
                var invoiceCount = await _context.Invoices.CountAsync();
                var totalRevenue = await _context.Invoices
                    .Where(i => i.Status == InvoiceStatus.Paid)
                    .SumAsync(i => (decimal?)i.TotalAmount) ?? 0;

                lblInvoiceCount.Text = $"{invoiceCount} Invoice{(invoiceCount != 1 ? "s" : "")}";
                lblTotalRevenue.Text = $"{totalRevenue:N0} KHR";

                // Update account summary
                var activeInvoicesTotal = await _context.Invoices
                    .Where(i => i.Status == InvoiceStatus.Active)
                    .SumAsync(i => (decimal?)i.TotalAmount) ?? 0;
                lblActiveInvoices.Text = $"{activeInvoicesTotal:N0} KHR";

                var repeatedCount = await _context.Invoices
                    .GroupBy(i => i.CustomerId)
                    .CountAsync(g => g.Count() > 1);
                lblRepeatedInvoices.Text = repeatedCount.ToString();

                var paymentLinksCount = await _context.PaymentLinks.CountAsync();
                lblPaymentLinks.Text = paymentLinksCount.ToString();

                var stockSalesCount = await _context.Sales.CountAsync();
                lblStockSales.Text = stockSalesCount.ToString();

                var productsCount = await _context.Products.CountAsync(p => p.IsActive);
                lblProducts.Text = productsCount.ToString();

                // Update latest invoices grid
                await LoadLatestInvoices();

                // Refresh charts
                trendChartPanel.Invalidate();
                statusChartPanel.Invalidate();
            }
            catch (Exception ex)
            {
                // Log error silently
                System.Diagnostics.Debug.WriteLine($"Error refreshing dashboard: {ex.Message}");
            }
        }

        private async System.Threading.Tasks.Task LoadLatestInvoices()
        {
            var invoices = await _context.Invoices
                .Include(i => i.Customer)
                .OrderByDescending(i => i.CreatedAt)
                .Take(5)
                .Select(i => new
                {
                    InvoiceNumber = i.InvoiceNumber,
                    ClientName = i.Customer != null ? i.Customer.FullName : (i.CustomerName ?? "Walk-in Customer"),
                    Status = i.Status.ToString(),
                    Amount = $"{i.TotalAmount:N0} KHR",
                    Date = i.CreatedAt.ToString("MMMM dd, yyyy")
                })
                .ToListAsync();

            invoiceGrid.DataSource = invoices;

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

        private async System.Threading.Tasks.Task RefreshCharts()
        {
            trendChartPanel.Invalidate();
            statusChartPanel.Invalidate();
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
