using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using SyncVerseStudio.Data;
using SyncVerseStudio.Models;
using SyncVerseStudio.Services;
using FontAwesome.Sharp;

namespace SyncVerseStudio.Views
{
    public partial class AnalyticsView : Form
    {
        private readonly AuthenticationService _authService;
        private readonly ApplicationDbContext _context;
        
        // Main panels
        private Panel headerPanel = null!;
        private Panel metricsPanel = null!;
        private Panel chartsPanel = null!;
        private Panel reportsPanel = null!;
        
        // Date filters
        private DateTimePicker fromDatePicker = null!;
        private DateTimePicker toDatePicker = null!;
        private ComboBox periodFilter = null!;
        
        // Metric cards
        private Label totalSalesLabel = null!;
        private Label totalRevenueLabel = null!;
        private Label totalProfitLabel = null!;
        private Label totalTransactionsLabel = null!;
        private Label avgTransactionLabel = null!;
        private Label topProductLabel = null!;
        
        // Buttons
        private IconButton refreshButton = null!;
        private IconButton exportPdfButton = null!;
        private IconButton exportExcelButton = null!;
        private IconButton exportCsvButton = null!;

        public AnalyticsView(AuthenticationService authService)
        {
            _authService = authService;
            _context = new ApplicationDbContext();
            InitializeComponent();
            LoadAnalytics();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form settings
            this.Text = "Analytics & Reports";
            this.Size = new Size(1400, 900);
            this.BackColor = Color.FromArgb(248, 250, 252);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;

            // Header Panel
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = Color.White,
                Padding = new Padding(30, 20, 30, 20)
            };

            var titleIcon = new IconPictureBox
            {
                IconChar = IconChar.ChartLine,
                IconColor = Color.FromArgb(255, 152, 0),
                IconSize = 40,
                Location = new Point(30, 25),
                Size = new Size(40, 40)
            };

            var titleLabel = new Label
            {
                Text = "Reports  Analytics",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(80, 20),
                AutoSize = true
            };

            var subtitleLabel = new Label
            {
                Text = "Comprehensive Reporting Suite - Real-time Business Intelligence",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(80, 55),
                AutoSize = true
            };

            // Date filters - Adjusted for 200px padding
            var filterPanel = new Panel
            {
                Location = new Point(230, 85),
                Size = new Size(940, 35),
                BackColor = Color.Transparent
            };

            var fromLabel = new Label
            {
                Text = "From:",
                Location = new Point(0, 8),
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(71, 85, 105)
            };

            fromDatePicker = new DateTimePicker
            {
                Location = new Point(50, 5),
                Width = 150,
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now.AddMonths(-1)
            };
            fromDatePicker.ValueChanged += DateFilter_Changed;

            var toLabel = new Label
            {
                Text = "To:",
                Location = new Point(220, 8),
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(71, 85, 105)
            };

            toDatePicker = new DateTimePicker
            {
                Location = new Point(250, 5),
                Width = 150,
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now
            };
            toDatePicker.ValueChanged += DateFilter_Changed;

            periodFilter = new ComboBox
            {
                Location = new Point(420, 5),
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            periodFilter.Items.AddRange(new object[] { "Today", "This Week", "This Month", "This Year", "Custom" });
            periodFilter.SelectedIndex = 2;
            periodFilter.SelectedIndexChanged += PeriodFilter_Changed;

            refreshButton = new IconButton
            {
                Text = " Refresh",
                IconChar = IconChar.Sync,
                IconColor = Color.White,
                Location = new Point(700, 0),
                Size = new Size(110, 35),
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            refreshButton.FlatAppearance.BorderSize = 0;
            refreshButton.Click += RefreshButton_Click;

            exportPdfButton = new IconButton
            {
                IconChar = IconChar.FilePdf,
                IconColor = Color.White,
                Location = new Point(820, 0),
                Size = new Size(35, 35),
                BackColor = Color.FromArgb(220, 38, 38),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            exportPdfButton.FlatAppearance.BorderSize = 0;
            exportPdfButton.Click += ExportPdf_Click;

            exportExcelButton = new IconButton
            {
                IconChar = IconChar.FileExcel,
                IconColor = Color.White,
                Location = new Point(865, 0),
                Size = new Size(35, 35),
                BackColor = Color.FromArgb(34, 197, 94),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            exportExcelButton.FlatAppearance.BorderSize = 0;
            exportExcelButton.Click += ExportExcel_Click;

            exportCsvButton = new IconButton
            {
                IconChar = IconChar.FileCsv,
                IconColor = Color.White,
                Location = new Point(910, 0),
                Size = new Size(35, 35),
                BackColor = Color.FromArgb(168, 85, 247),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            exportCsvButton.FlatAppearance.BorderSize = 0;
            exportCsvButton.Click += ExportCsv_Click;

            filterPanel.Controls.AddRange(new Control[] { 
                fromLabel, fromDatePicker, toLabel, toDatePicker, periodFilter,
                refreshButton, exportPdfButton, exportExcelButton, exportCsvButton
            });

            headerPanel.Controls.AddRange(new Control[] { titleIcon, titleLabel, subtitleLabel, filterPanel });

            // Metrics Panel (KPI Cards) - Added 200px padding
            metricsPanel = new Panel
            {
                Location = new Point(230, 140),
                Size = new Size(940, 150),
                BackColor = Color.Transparent
            };

            CreateMetricCards();

            // Charts Panel - Added 200px padding
            chartsPanel = new Panel
            {
                Location = new Point(230, 310),
                Size = new Size(940, 280),
                BackColor = Color.Transparent
            };

            CreateChartsSection();

            // Reports Panel - Added 200px padding
            reportsPanel = new Panel
            {
                Location = new Point(230, 610),
                Size = new Size(940, 250),
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            CreateReportsSection();

            // Add all to form
            this.Controls.AddRange(new Control[] { headerPanel, metricsPanel, chartsPanel, reportsPanel });

            this.ResumeLayout(false);
        }

        private void CreateMetricCards()
        {
            int cardWidth = 145;
            int cardHeight = 140;
            int spacing = 10;

            // Total Sales Card
            var salesCard = CreateMetricCard("Total Sales", "$0.00", IconChar.DollarSign, 
                Color.FromArgb(59, 130, 246), 0 * (cardWidth + spacing), out totalSalesLabel);
            
            // Total Revenue Card
            var revenueCard = CreateMetricCard("Revenue", "$0.00", IconChar.ChartLine, 
                Color.FromArgb(34, 197, 94), 1 * (cardWidth + spacing), out totalRevenueLabel);
            
            // Total Profit Card
            var profitCard = CreateMetricCard("Profit", "$0.00", IconChar.MoneyBillWave, 
                Color.FromArgb(168, 85, 247), 2 * (cardWidth + spacing), out totalProfitLabel);
            
            // Transactions Card
            var transactionsCard = CreateMetricCard("Transactions", "0", IconChar.Receipt, 
                Color.FromArgb(249, 115, 22), 3 * (cardWidth + spacing), out totalTransactionsLabel);
            
            // Avg Transaction Card
            var avgCard = CreateMetricCard("Avg Transaction", "$0.00", IconChar.ChartBar, 
                Color.FromArgb(236, 72, 153), 4 * (cardWidth + spacing), out avgTransactionLabel);
            
            // Top Product Card
            var topProductCard = CreateMetricCard("Top Product", "N/A", IconChar.Star, 
                Color.FromArgb(14, 165, 233), 5 * (cardWidth + spacing), out topProductLabel);

            metricsPanel.Controls.AddRange(new Control[] { 
                salesCard, revenueCard, profitCard, transactionsCard, avgCard, topProductCard 
            });
        }

        private Panel CreateMetricCard(string title, string value, IconChar icon, Color color, int xPos, out Label valueLabel)
        {
            var card = new Panel
            {
                Location = new Point(xPos, 0),
                Size = new Size(145, 140),
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            var iconBox = new IconPictureBox
            {
                IconChar = icon,
                IconColor = color,
                IconSize = 28,
                Location = new Point(10, 10),
                Size = new Size(28, 28)
            };

            var titleLbl = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(10, 45),
                Size = new Size(125, 30),
                AutoEllipsis = true
            };

            valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(10, 70),
                Size = new Size(125, 25),
                AutoEllipsis = true
            };

            var trendIcon = new IconPictureBox
            {
                IconChar = IconChar.ArrowUp,
                IconColor = Color.FromArgb(34, 197, 94),
                IconSize = 12,
                Location = new Point(10, 105),
                Size = new Size(12, 12)
            };

            var trendLabel = new Label
            {
                Text = "+12.5%",
                Font = new Font("Segoe UI", 7),
                ForeColor = Color.FromArgb(34, 197, 94),
                Location = new Point(25, 103),
                AutoSize = true
            };

            card.Controls.AddRange(new Control[] { iconBox, titleLbl, valueLabel, trendIcon, trendLabel });
            
            // Add shadow effect
            card.Paint += (s, e) =>
            {
                var rect = card.ClientRectangle;
                using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, rect.Width - 1, rect.Height - 1);
                }
            };

            return card;
        }

        private void CreateChartsSection()
        {
            // Sales Trend Chart
            var salesChartPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(460, 280),
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            var salesChartTitle = new Label
            {
                Text = "Sales Analytics Trends",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(20, 15),
                AutoSize = true
            };

            var salesChartIcon = new IconPictureBox
            {
                IconChar = IconChar.ChartArea,
                IconColor = Color.FromArgb(59, 130, 246),
                IconSize = 20,
                Location = new Point(420, 15),
                Size = new Size(20, 20)
            };

            var salesChartPlaceholder = new Label
            {
                Text = "📊 Sales trend chart\n\nDaily, weekly, monthly patterns",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(20, 80),
                Size = new Size(420, 180),
                TextAlign = ContentAlignment.MiddleCenter
            };

            salesChartPanel.Controls.AddRange(new Control[] { salesChartTitle, salesChartIcon, salesChartPlaceholder });

            // Top Products Chart
            var productsChartPanel = new Panel
            {
                Location = new Point(480, 0),
                Size = new Size(460, 280),
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            var productsChartTitle = new Label
            {
                Text = "Top Products & Categories",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(20, 15),
                AutoSize = true
            };

            var productsChartIcon = new IconPictureBox
            {
                IconChar = IconChar.ChartPie,
                IconColor = Color.FromArgb(168, 85, 247),
                IconSize = 20,
                Location = new Point(420, 15),
                Size = new Size(20, 20)
            };

            var productsChartPlaceholder = new Label
            {
                Text = "📈 Top products chart\n\nBest categories and items",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(20, 80),
                Size = new Size(420, 180),
                TextAlign = ContentAlignment.MiddleCenter
            };

            productsChartPanel.Controls.AddRange(new Control[] { productsChartTitle, productsChartIcon, productsChartPlaceholder });

            chartsPanel.Controls.AddRange(new Control[] { salesChartPanel, productsChartPanel });
        }

        private void CreateReportsSection()
        {
            var reportsTitle = new Label
            {
                Text = "Available Reports",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(0, 0),
                AutoSize = true
            };

            var reportsGrid = new FlowLayoutPanel
            {
                Location = new Point(0, 40),
                Size = new Size(900, 190),
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true
            };

            // Report items
            var reports = new[]
            {
                ("Sales Analytics Trends", IconChar.ChartLine, Color.FromArgb(59, 130, 246)),
                ("Revenue & Profit Reports", IconChar.MoneyBillTrendUp, Color.FromArgb(34, 197, 94)),
                ("Inventory Performance Analysis", IconChar.BoxesStacked, Color.FromArgb(249, 115, 22)),
                ("Customer Behavior Insights", IconChar.Users, Color.FromArgb(168, 85, 247)),
                ("Staff Performance Metrics", IconChar.UserTie, Color.FromArgb(236, 72, 153)),
                ("Business Intelligence Dashboard", IconChar.ChartPie, Color.FromArgb(14, 165, 233)),
                ("Daily, Weekly, Monthly Reports", IconChar.CalendarDays, Color.FromArgb(139, 92, 246)),
                ("Loss Prevention Analytics", IconChar.ShieldHalved, Color.FromArgb(239, 68, 68)),
                ("Comparative Period Analysis", IconChar.ArrowsLeftRight, Color.FromArgb(6, 182, 212)),
                ("Export to PDF, Excel, CSV", IconChar.FileExport, Color.FromArgb(132, 204, 22))
            };

            foreach (var (title, icon, color) in reports)
            {
                var reportCard = CreateReportCard(title, icon, color);
                reportsGrid.Controls.Add(reportCard);
            }

            reportsPanel.Controls.AddRange(new Control[] { reportsTitle, reportsGrid });
        }

        private Panel CreateReportCard(string title, IconChar icon, Color color)
        {
            var card = new Panel
            {
                Size = new Size(250, 80),
                BackColor = Color.FromArgb(248, 250, 252),
                Margin = new Padding(5),
                Cursor = Cursors.Hand
            };

            var iconBox = new IconPictureBox
            {
                IconChar = icon,
                IconColor = color,
                IconSize = 28,
                Location = new Point(15, 26),
                Size = new Size(28, 28)
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(55, 20),
                Size = new Size(180, 40),
                AutoEllipsis = true
            };

            var statusLabel = new Label
            {
                Text = "Coming Soon",
                Font = new Font("Segoe UI", 7),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(55, 50),
                AutoSize = true
            };

            card.Controls.AddRange(new Control[] { iconBox, titleLabel, statusLabel });

            card.MouseEnter += (s, e) => card.BackColor = Color.FromArgb(241, 245, 249);
            card.MouseLeave += (s, e) => card.BackColor = Color.FromArgb(248, 250, 252);

            return card;
        }

        private void LoadAnalytics()
        {
            try
            {
                var fromDate = fromDatePicker.Value.Date;
                var toDate = toDatePicker.Value.Date.AddDays(1).AddSeconds(-1);

                // Get sales data
                var sales = _context.Sales
                    .Include(s => s.SaleItems)
                    .Where(s => s.SaleDate >= fromDate && s.SaleDate <= toDate && s.Status == SaleStatus.Completed)
                    .ToList();

                // Calculate metrics
                var totalSales = sales.Sum(s => s.TotalAmount);
                var totalTransactions = sales.Count;
                var avgTransaction = totalTransactions > 0 ? totalSales / totalTransactions : 0;

                // Calculate profit
                var totalProfit = 0m;
                foreach (var sale in sales)
                {
                    foreach (var item in sale.SaleItems)
                    {
                        var product = _context.Products.Find(item.ProductId);
                        if (product != null)
                        {
                            totalProfit += (item.UnitPrice - product.CostPrice) * item.Quantity;
                        }
                    }
                }

                // Get top product
                var topProduct = _context.SaleItems
                    .Include(si => si.Product)
                    .Where(si => si.Sale.SaleDate >= fromDate && si.Sale.SaleDate <= toDate)
                    .GroupBy(si => si.ProductId)
                    .Select(g => new { ProductId = g.Key, TotalQty = g.Sum(si => si.Quantity) })
                    .OrderByDescending(x => x.TotalQty)
                    .FirstOrDefault();

                string topProductName = "N/A";
                if (topProduct != null)
                {
                    var product = _context.Products.Find(topProduct.ProductId);
                    topProductName = product?.Name ?? "N/A";
                }

                // Update UI
                totalSalesLabel.Text = $"${totalSales:N2}";
                totalRevenueLabel.Text = $"${totalSales:N2}";
                totalProfitLabel.Text = $"${totalProfit:N2}";
                totalTransactionsLabel.Text = totalTransactions.ToString();
                avgTransactionLabel.Text = $"${avgTransaction:N2}";
                topProductLabel.Text = topProductName.Length > 15 ? topProductName.Substring(0, 15) + "..." : topProductName;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading analytics: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DateFilter_Changed(object? sender, EventArgs e)
        {
            periodFilter.SelectedIndex = 4; // Set to Custom
            LoadAnalytics();
        }

        private void PeriodFilter_Changed(object? sender, EventArgs e)
        {
            switch (periodFilter.SelectedIndex)
            {
                case 0: // Today
                    fromDatePicker.Value = DateTime.Today;
                    toDatePicker.Value = DateTime.Today;
                    break;
                case 1: // This Week
                    fromDatePicker.Value = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                    toDatePicker.Value = DateTime.Today;
                    break;
                case 2: // This Month
                    fromDatePicker.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    toDatePicker.Value = DateTime.Today;
                    break;
                case 3: // This Year
                    fromDatePicker.Value = new DateTime(DateTime.Today.Year, 1, 1);
                    toDatePicker.Value = DateTime.Today;
                    break;
                case 4: // Custom
                    return;
            }
            LoadAnalytics();
        }

        private void RefreshButton_Click(object? sender, EventArgs e)
        {
            LoadAnalytics();
        }

        private void ExportPdf_Click(object? sender, EventArgs e)
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "PDF Files (*.pdf)|*.pdf",
                    FileName = $"Analytics_Report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf",
                    Title = "Export Analytics Report to PDF"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportToPdf(saveDialog.FileName);
                    MessageBox.Show($"Report exported successfully to:\n{saveDialog.FileName}", "Export Successful", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting to PDF: {ex.Message}", "Export Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportExcel_Click(object? sender, EventArgs e)
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "Excel Files (*.xlsx)|*.xlsx",
                    FileName = $"Analytics_Report_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                    Title = "Export Analytics Report to Excel"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportToExcel(saveDialog.FileName);
                    MessageBox.Show($"Report exported successfully to:\n{saveDialog.FileName}", "Export Successful", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting to Excel: {ex.Message}", "Export Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportCsv_Click(object? sender, EventArgs e)
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "CSV Files (*.csv)|*.csv",
                    FileName = $"Analytics_Report_{DateTime.Now:yyyyMMdd_HHmmss}.csv",
                    Title = "Export Analytics Report to CSV"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportToCsv(saveDialog.FileName);
                    MessageBox.Show($"Report exported successfully to:\n{saveDialog.FileName}", "Export Successful", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting to CSV: {ex.Message}", "Export Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportToPdf(string filePath)
        {
            using (var writer = new System.IO.StreamWriter(filePath))
            {
                writer.WriteLine("SYNCVERSE STUDIO - ANALYTICS REPORT");
                writer.WriteLine("=" + new string('=', 60));
                writer.WriteLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                writer.WriteLine($"Period: {fromDatePicker.Value:yyyy-MM-dd} to {toDatePicker.Value:yyyy-MM-dd}");
                writer.WriteLine();
                writer.WriteLine("KEY PERFORMANCE INDICATORS");
                writer.WriteLine("-" + new string('-', 60));
                writer.WriteLine($"Total Sales:        {totalSalesLabel.Text}");
                writer.WriteLine($"Revenue:            {totalRevenueLabel.Text}");
                writer.WriteLine($"Profit:             {totalProfitLabel.Text}");
                writer.WriteLine($"Transactions:       {totalTransactionsLabel.Text}");
                writer.WriteLine($"Avg Transaction:    {avgTransactionLabel.Text}");
                writer.WriteLine($"Top Product:        {topProductLabel.Text}");
                writer.WriteLine();
                
                // Get detailed sales data
                var fromDate = fromDatePicker.Value.Date;
                var toDate = toDatePicker.Value.Date.AddDays(1).AddSeconds(-1);
                var sales = _context.Sales
                    .Include(s => s.SaleItems)
                    .ThenInclude(si => si.Product)
                    .Include(s => s.Customer)
                    .Include(s => s.Cashier)
                    .Where(s => s.SaleDate >= fromDate && s.SaleDate <= toDate && s.Status == SaleStatus.Completed)
                    .OrderByDescending(s => s.SaleDate)
                    .ToList();

                writer.WriteLine("DETAILED TRANSACTIONS");
                writer.WriteLine("-" + new string('-', 60));
                foreach (var sale in sales)
                {
                    writer.WriteLine($"Invoice: {sale.InvoiceNumber} | Date: {sale.SaleDate:yyyy-MM-dd HH:mm}");
                    writer.WriteLine($"Cashier: {sale.Cashier.FullName} | Customer: {sale.Customer?.FullName ?? "Walk-in"}");
                    writer.WriteLine($"Amount: ${sale.TotalAmount:N2} | Payment: {sale.PaymentMethod}");
                    writer.WriteLine();
                }
            }
        }

        private void ExportToExcel(string filePath)
        {
            var fromDate = fromDatePicker.Value.Date;
            var toDate = toDatePicker.Value.Date.AddDays(1).AddSeconds(-1);
            var sales = _context.Sales
                .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
                .Include(s => s.Customer)
                .Include(s => s.Cashier)
                .Where(s => s.SaleDate >= fromDate && s.SaleDate <= toDate && s.Status == SaleStatus.Completed)
                .OrderByDescending(s => s.SaleDate)
                .ToList();

            using (var writer = new System.IO.StreamWriter(filePath, false, System.Text.Encoding.UTF8))
            {
                // Write header
                writer.WriteLine("SYNCVERSE STUDIO - ANALYTICS REPORT");
                writer.WriteLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                writer.WriteLine($"Period: {fromDatePicker.Value:yyyy-MM-dd} to {toDatePicker.Value:yyyy-MM-dd}");
                writer.WriteLine();
                
                // KPIs
                writer.WriteLine("KEY METRICS");
                writer.WriteLine($"Total Sales\t{totalSalesLabel.Text}");
                writer.WriteLine($"Revenue\t{totalRevenueLabel.Text}");
                writer.WriteLine($"Profit\t{totalProfitLabel.Text}");
                writer.WriteLine($"Transactions\t{totalTransactionsLabel.Text}");
                writer.WriteLine($"Avg Transaction\t{avgTransactionLabel.Text}");
                writer.WriteLine($"Top Product\t{topProductLabel.Text}");
                writer.WriteLine();
                
                // Detailed data
                writer.WriteLine("TRANSACTION DETAILS");
                writer.WriteLine("Invoice Number\tDate\tCashier\tCustomer\tTotal Amount\tPayment Method\tStatus");
                foreach (var sale in sales)
                {
                    writer.WriteLine($"{sale.InvoiceNumber}\t{sale.SaleDate:yyyy-MM-dd HH:mm}\t{sale.Cashier.FullName}\t" +
                        $"{sale.Customer?.FullName ?? "Walk-in"}\t${sale.TotalAmount:N2}\t{sale.PaymentMethod}\t{sale.Status}");
                }
            }
        }

        private void ExportToCsv(string filePath)
        {
            var fromDate = fromDatePicker.Value.Date;
            var toDate = toDatePicker.Value.Date.AddDays(1).AddSeconds(-1);
            var sales = _context.Sales
                .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
                .Include(s => s.Customer)
                .Include(s => s.Cashier)
                .Where(s => s.SaleDate >= fromDate && s.SaleDate <= toDate && s.Status == SaleStatus.Completed)
                .OrderByDescending(s => s.SaleDate)
                .ToList();

            using (var writer = new System.IO.StreamWriter(filePath, false, System.Text.Encoding.UTF8))
            {
                // Write CSV header
                writer.WriteLine("Invoice Number,Date,Time,Cashier,Customer,Subtotal,Tax,Discount,Total Amount,Payment Method,Status");
                
                // Write data rows
                foreach (var sale in sales)
                {
                    var subtotal = sale.TotalAmount - sale.TaxAmount + sale.DiscountAmount;
                    writer.WriteLine($"\"{sale.InvoiceNumber}\",\"{sale.SaleDate:yyyy-MM-dd}\",\"{sale.SaleDate:HH:mm:ss}\"," +
                        $"\"{sale.Cashier.FullName}\",\"{sale.Customer?.FullName ?? "Walk-in"}\"," +
                        $"{subtotal:F2},{sale.TaxAmount:F2},{sale.DiscountAmount:F2},{sale.TotalAmount:F2}," +
                        $"\"{sale.PaymentMethod}\",\"{sale.Status}\"");
                }
            }
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
