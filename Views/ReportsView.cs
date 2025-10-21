using SyncVerseStudio.Services;
using SyncVerseStudio.Models;
using SyncVerseStudio.Data;
using Microsoft.EntityFrameworkCore;

namespace SyncVerseStudio.Views
{
    public partial class ReportsView : Form
    {
        private readonly AuthenticationService _authService;
        private ApplicationDbContext? _context;
        private Panel reportTypesPanel;
        private Panel filtersPanel;
        private Panel contentPanel;
        private ComboBox reportTypeComboBox;
        private DateTimePicker fromDatePicker;
        private DateTimePicker toDatePicker;
        private DataGridView reportGridView;
        private Panel summaryPanel;
        private Button generateReportButton;
        private Button exportButton;

        public ReportsView(AuthenticationService authService)
        {
            _authService = authService;
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.AutoScaleDimensions = new SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(248, 250, 252);
            this.ClientSize = new Size(1200, 800);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Name = "ReportsView";
            this.Text = "Reports";

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
                Text = "Reports & Analytics",
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(20, 20),
                Size = new Size(400, 40),
                TextAlign = ContentAlignment.MiddleLeft
            };
            headerPanel.Controls.Add(titleLabel);

            this.Controls.Add(headerPanel);

            // Report Types Panel
            CreateReportTypesPanel();

            // Filters Panel
            CreateFiltersPanel();

            // Summary Panel
            CreateSummaryPanel();

            // Content Panel
            CreateContentPanel();
        }

        private void CreateReportTypesPanel()
        {
            reportTypesPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = Color.White,
                Padding = new Padding(20, 15, 20, 15)
            };

            var reportTypeLabel = new Label
            {
                Text = "Report Type:",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(0, 0),
                Size = new Size(100, 25)
            };

            reportTypeComboBox = new ComboBox
            {
                Font = new Font("Segoe UI", 11F),
                Location = new Point(0, 25),
                Size = new Size(250, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            var reportTypes = new[]
            {
                "Sales Report",
                "Product Performance",
                "Customer Analysis",
                "Inventory Report",
                "Daily Sales Summary",
                "Monthly Revenue",
                "Top Selling Products",
                "Payment Methods Analysis"
            };

            reportTypeComboBox.Items.AddRange(reportTypes);
            reportTypeComboBox.SelectedIndex = 0;

            reportTypesPanel.Controls.Add(reportTypeLabel);
            reportTypesPanel.Controls.Add(reportTypeComboBox);

            this.Controls.Add(reportTypesPanel);
        }

        private void CreateFiltersPanel()
        {
            filtersPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(243, 244, 246),
                Padding = new Padding(20, 15, 20, 15)
            };

            // Date Range
            var fromLabel = new Label
            {
                Text = "From Date:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(0, 0),
                Size = new Size(80, 20)
            };

            fromDatePicker = new DateTimePicker
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, 20),
                Size = new Size(150, 25),
                Value = DateTime.Today.AddDays(-30)
            };

            var toLabel = new Label
            {
                Text = "To Date:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(170, 0),
                Size = new Size(70, 20)
            };

            toDatePicker = new DateTimePicker
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(170, 20),
                Size = new Size(150, 25),
                Value = DateTime.Today
            };

            // Generate Report Button
            generateReportButton = new Button
            {
                Text = "Generate Report",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(59, 130, 246),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(340, 20),
                Size = new Size(130, 30),
                Cursor = Cursors.Hand
            };
            generateReportButton.FlatAppearance.BorderSize = 0;
            generateReportButton.Click += GenerateReportButton_Click;

            // Export Button
            exportButton = new Button
            {
                Text = "Export to CSV",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(34, 197, 94),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(480, 20),
                Size = new Size(120, 30),
                Cursor = Cursors.Hand
            };
            exportButton.FlatAppearance.BorderSize = 0;
            exportButton.Click += ExportButton_Click;

            filtersPanel.Controls.Add(fromLabel);
            filtersPanel.Controls.Add(fromDatePicker);
            filtersPanel.Controls.Add(toLabel);
            filtersPanel.Controls.Add(toDatePicker);
            filtersPanel.Controls.Add(generateReportButton);
            filtersPanel.Controls.Add(exportButton);

            this.Controls.Add(filtersPanel);
        }

        private void CreateSummaryPanel()
        {
            summaryPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = Color.Transparent,
                Padding = new Padding(20, 10, 20, 10)
            };

            this.Controls.Add(summaryPanel);
        }

        private void CreateSummaryCards(Dictionary<string, string> summaryData)
        {
            summaryPanel.Controls.Clear();

            var summaryCardsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                WrapContents = true,
                AutoSize = false
            };

            var colors = new[]
            {
                Color.FromArgb(59, 130, 246),   // blue
                Color.FromArgb(34, 197, 94),    // green
                Color.FromArgb(168, 85, 247),   // purple
                Color.FromArgb(245, 158, 11),   // yellow
                Color.FromArgb(239, 68, 68)     // red
            };

            int colorIndex = 0;
            foreach (var kvp in summaryData)
            {
                var card = CreateSummaryCard(kvp.Key, kvp.Value, colors[colorIndex % colors.Length]);
                summaryCardsPanel.Controls.Add(card);
                colorIndex++;
            }

            summaryPanel.Controls.Add(summaryCardsPanel);
        }

        private Panel CreateSummaryCard(string title, string value, Color bgColor)
        {
            var card = new Panel
            {
                Size = new Size(220, 90),
                BackColor = bgColor,
                Margin = new Padding(10),
                Padding = new Padding(15, 10, 15, 10)
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
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(0, 10),
                Size = new Size(200, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(0, 35),
                Size = new Size(200, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            card.Controls.Add(titleLabel);
            card.Controls.Add(valueLabel);

            return card;
        }

        private void CreateContentPanel()
        {
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(20)
            };

            reportGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoGenerateColumns = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 10F)
            };

            var noDataLabel = new Label
            {
                Text = "Select a report type and click 'Generate Report' to view data",
                Font = new Font("Segoe UI", 14F, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 116, 139),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Name = "NoDataLabel"
            };

            contentPanel.Controls.Add(noDataLabel);
            contentPanel.Controls.Add(reportGridView);

            this.Controls.Add(contentPanel);
        }

        private async void LoadData()
        {
            try
            {
                _context = new ApplicationDbContext();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void GenerateReportButton_Click(object sender, EventArgs e)
        {
            if (_context == null) return;

            try
            {
                var reportType = reportTypeComboBox.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(reportType)) return;

                // Hide no data label and show grid
                var noDataLabel = contentPanel.Controls.Find("NoDataLabel", false).FirstOrDefault();
                if (noDataLabel != null) noDataLabel.Visible = false;
                reportGridView.Visible = true;

                switch (reportType)
                {
                    case "Sales Report":
                        await GenerateSalesReport();
                        break;
                    case "Product Performance":
                        await GenerateProductPerformanceReport();
                        break;
                    case "Customer Analysis":
                        await GenerateCustomerAnalysisReport();
                        break;
                    case "Inventory Report":
                        await GenerateInventoryReport();
                        break;
                    case "Daily Sales Summary":
                        await GenerateDailySalesReport();
                        break;
                    case "Monthly Revenue":
                        await GenerateMonthlyRevenueReport();
                        break;
                    case "Top Selling Products":
                        await GenerateTopSellingProductsReport();
                        break;
                    case "Payment Methods Analysis":
                        await GeneratePaymentMethodsReport();
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task GenerateSalesReport()
        {
            if (_context == null) return;

            var fromDate = fromDatePicker.Value.Date;
            var toDate = toDatePicker.Value.Date.AddDays(1);

            var salesData = await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.Cashier)
                .Where(s => s.SaleDate >= fromDate && s.SaleDate <= toDate)
                .Select(s => new
                {
                    ReceiptNumber = s.ReceiptNumber,
                    Date = s.SaleDate,
                    Customer = s.Customer != null ? s.Customer.FullName : "Walk-in",
                    Cashier = s.Cashier.FullName,
                    Total = s.TotalAmount,
                    PaymentMethod = s.PaymentMethod,
                    Status = s.Status.ToString()
                })
                .OrderByDescending(s => s.Date)
                .ToListAsync();

            reportGridView.DataSource = salesData;

            // Create summary
            var totalSales = salesData.Where(s => s.Status == "Completed").Sum(s => s.Total);
            var transactionCount = salesData.Count(s => s.Status == "Completed");
            var averageSale = transactionCount > 0 ? totalSales / transactionCount : 0;

            var summaryData = new Dictionary<string, string>
            {
                { "Total Sales", $"${totalSales:N2}" },
                { "Transactions", transactionCount.ToString("N0") },
                { "Average Sale", $"${averageSale:N2}" },
                { "Date Range", $"{fromDate:MM/dd/yyyy} - {toDatePicker.Value:MM/dd/yyyy}" }
            };

            CreateSummaryCards(summaryData);
        }

        private async Task GenerateProductPerformanceReport()
        {
            if (_context == null) return;

            var fromDate = fromDatePicker.Value.Date;
            var toDate = toDatePicker.Value.Date.AddDays(1);

            var productData = await _context.SaleItems
                .Include(si => si.Product)
                .Include(si => si.Sale)
                .Where(si => si.Sale.SaleDate >= fromDate && 
                           si.Sale.SaleDate <= toDate && 
                           si.Sale.Status == SaleStatus.Completed)
                .GroupBy(si => new { si.ProductId, si.Product.Name })
                .Select(g => new
                {
                    ProductName = g.Key.Name,
                    QuantitySold = g.Sum(si => si.Quantity),
                    Revenue = g.Sum(si => si.Quantity * si.UnitPrice),
                    TimesOrdered = g.Count()
                })
                .OrderByDescending(p => p.Revenue)
                .ToListAsync();

            reportGridView.DataSource = productData;

            var totalRevenue = productData.Sum(p => p.Revenue);
            var totalQuantity = productData.Sum(p => p.QuantitySold);
            var productCount = productData.Count;

            var summaryData = new Dictionary<string, string>
            {
                { "Total Revenue", $"${totalRevenue:N2}" },
                { "Products Sold", totalQuantity.ToString("N0") },
                { "Active Products", productCount.ToString("N0") }
            };

            CreateSummaryCards(summaryData);
        }

        private async Task GenerateCustomerAnalysisReport()
        {
            if (_context == null) return;

            var fromDate = fromDatePicker.Value.Date;
            var toDate = toDatePicker.Value.Date.AddDays(1);

            var customerData = await _context.Customers
                .Select(c => new
                {
                    CustomerName = c.FirstName + " " + c.LastName,
                    Email = c.Email,
                    Phone = c.Phone,
                    TotalPurchases = _context.Sales
                        .Where(s => s.CustomerId == c.Id && 
                                  s.SaleDate >= fromDate && 
                                  s.SaleDate <= toDate &&
                                  s.Status == SaleStatus.Completed)
                        .Sum(s => (decimal?)s.TotalAmount) ?? 0,
                    OrderCount = _context.Sales
                        .Count(s => s.CustomerId == c.Id && 
                                  s.SaleDate >= fromDate && 
                                  s.SaleDate <= toDate &&
                                  s.Status == SaleStatus.Completed),
                    LastPurchase = _context.Sales
                        .Where(s => s.CustomerId == c.Id && s.Status == SaleStatus.Completed)
                        .Max(s => (DateTime?)s.SaleDate)
                })
                .Where(c => c.TotalPurchases > 0)
                .OrderByDescending(c => c.TotalPurchases)
                .ToListAsync();

            reportGridView.DataSource = customerData;

            var totalCustomers = customerData.Count;
            var totalRevenue = customerData.Sum(c => c.TotalPurchases);
            var averageOrderValue = customerData.Where(c => c.OrderCount > 0).Average(c => c.TotalPurchases / c.OrderCount);

            var summaryData = new Dictionary<string, string>
            {
                { "Active Customers", totalCustomers.ToString("N0") },
                { "Total Revenue", $"${totalRevenue:N2}" },
                { "Avg Order Value", $"${averageOrderValue:N2}" }
            };

            CreateSummaryCards(summaryData);
        }

        private async Task GenerateInventoryReport()
        {
            if (_context == null) return;

            var inventoryData = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive)
                .Select(p => new
                {
                    ProductName = p.Name,
                    Category = p.Category.Name,
                    CurrentStock = p.Quantity,
                    MinStock = p.MinQuantity,
                    StockValue = p.Quantity * p.CostPrice,
                    Status = p.Quantity <= p.MinQuantity ? "Low Stock" : "In Stock"
                })
                .OrderBy(p => p.ProductName)
                .ToListAsync();

            reportGridView.DataSource = inventoryData;

            var totalProducts = inventoryData.Count;
            var lowStockCount = inventoryData.Count(p => p.Status == "Low Stock");
            var totalStockValue = inventoryData.Sum(p => p.StockValue);

            var summaryData = new Dictionary<string, string>
            {
                { "Total Products", totalProducts.ToString("N0") },
                { "Low Stock Items", lowStockCount.ToString("N0") },
                { "Stock Value", $"${totalStockValue:N2}" }
            };

            CreateSummaryCards(summaryData);
        }

        private async Task GenerateDailySalesReport()
        {
            if (_context == null) return;

            var fromDate = fromDatePicker.Value.Date;
            var toDate = toDatePicker.Value.Date.AddDays(1);

            var dailyData = await _context.Sales
                .Where(s => s.SaleDate >= fromDate && 
                           s.SaleDate <= toDate && 
                           s.Status == SaleStatus.Completed)
                .GroupBy(s => s.SaleDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    TransactionCount = g.Count(),
                    TotalSales = g.Sum(s => s.TotalAmount),
                    AverageSale = g.Average(s => s.TotalAmount)
                })
                .OrderByDescending(d => d.Date)
                .ToListAsync();

            reportGridView.DataSource = dailyData;

            var totalSales = dailyData.Sum(d => d.TotalSales);
            var totalTransactions = dailyData.Sum(d => d.TransactionCount);
            var daysCount = dailyData.Count;
            var avgDailySales = daysCount > 0 ? totalSales / daysCount : 0;

            var summaryData = new Dictionary<string, string>
            {
                { "Total Sales", $"${totalSales:N2}" },
                { "Total Transactions", totalTransactions.ToString("N0") },
                { "Avg Daily Sales", $"${avgDailySales:N2}" },
                { "Days Analyzed", daysCount.ToString("N0") }
            };

            CreateSummaryCards(summaryData);
        }

        private async Task GenerateMonthlyRevenueReport()
        {
            if (_context == null) return;

            var monthlyData = await _context.Sales
                .Where(s => s.Status == SaleStatus.Completed)
                .GroupBy(s => new { s.SaleDate.Year, s.SaleDate.Month })
                .Select(g => new
                {
                    Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                    Revenue = g.Sum(s => s.TotalAmount),
                    TransactionCount = g.Count(),
                    AverageTransaction = g.Average(s => s.TotalAmount)
                })
                .OrderByDescending(m => m.Month)
                .Take(12)
                .ToListAsync();

            reportGridView.DataSource = monthlyData;

            var totalRevenue = monthlyData.Sum(m => m.Revenue);
            var totalTransactions = monthlyData.Sum(m => m.TransactionCount);
            var monthsCount = monthlyData.Count;

            var summaryData = new Dictionary<string, string>
            {
                { "Total Revenue", $"${totalRevenue:N2}" },
                { "Total Transactions", totalTransactions.ToString("N0") },
                { "Months Analyzed", monthsCount.ToString("N0") }
            };

            CreateSummaryCards(summaryData);
        }

        private async Task GenerateTopSellingProductsReport()
        {
            if (_context == null) return;

            var fromDate = fromDatePicker.Value.Date;
            var toDate = toDatePicker.Value.Date.AddDays(1);

            var topProducts = await _context.SaleItems
                .Include(si => si.Product)
                .Include(si => si.Sale)
                .Where(si => si.Sale.SaleDate >= fromDate && 
                           si.Sale.SaleDate <= toDate && 
                           si.Sale.Status == SaleStatus.Completed)
                .GroupBy(si => new { si.ProductId, si.Product.Name })
                .Select(g => new
                {
                    ProductName = g.Key.Name,
                    QuantitySold = g.Sum(si => si.Quantity),
                    Revenue = g.Sum(si => si.Quantity * si.UnitPrice),
                    TimesOrdered = g.Count(),
                    AverageOrderQuantity = g.Average(si => si.Quantity)
                })
                .OrderByDescending(p => p.QuantitySold)
                .Take(20)
                .ToListAsync();

            reportGridView.DataSource = topProducts;

            var totalRevenue = topProducts.Sum(p => p.Revenue);
            var totalQuantity = topProducts.Sum(p => p.QuantitySold);

            var summaryData = new Dictionary<string, string>
            {
                { "Top Products", topProducts.Count.ToString("N0") },
                { "Total Quantity", totalQuantity.ToString("N0") },
                { "Total Revenue", $"${totalRevenue:N2}" }
            };

            CreateSummaryCards(summaryData);
        }

        private async Task GeneratePaymentMethodsReport()
        {
            if (_context == null) return;

            var fromDate = fromDatePicker.Value.Date;
            var toDate = toDatePicker.Value.Date.AddDays(1);

            var paymentData = await _context.Sales
                .Where(s => s.SaleDate >= fromDate && 
                           s.SaleDate <= toDate && 
                           s.Status == SaleStatus.Completed)
                .GroupBy(s => s.PaymentMethod)
                .Select(g => new
                {
                    PaymentMethod = g.Key,
                    TransactionCount = g.Count(),
                    TotalAmount = g.Sum(s => s.TotalAmount),
                    AverageAmount = g.Average(s => s.TotalAmount),
                    Percentage = 0.0 // Will be calculated after loading
                })
                .OrderByDescending(p => p.TotalAmount)
                .ToListAsync();

            // Calculate percentages
            var grandTotal = paymentData.Sum(p => p.TotalAmount);
            var paymentDataWithPercentage = paymentData.Select(p => new
            {
                p.PaymentMethod,
                p.TransactionCount,
                p.TotalAmount,
                p.AverageAmount,
                Percentage = grandTotal > 0 ? (double)(p.TotalAmount / grandTotal * 100) : 0
            }).ToList();

            reportGridView.DataSource = paymentDataWithPercentage;

            var totalTransactions = paymentData.Sum(p => p.TransactionCount);

            var summaryData = new Dictionary<string, string>
            {
                { "Payment Methods", paymentData.Count.ToString("N0") },
                { "Total Transactions", totalTransactions.ToString("N0") },
                { "Total Amount", $"${grandTotal:N2}" }
            };

            CreateSummaryCards(summaryData);
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (reportGridView.DataSource == null)
                {
                    MessageBox.Show("No data to export. Please generate a report first.", 
                        "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv",
                    FileName = $"{reportTypeComboBox.SelectedItem}_Report_{DateTime.Now:yyyyMMdd}.csv"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportToCSV(saveFileDialog.FileName);
                    MessageBox.Show("Report exported successfully!", "Export Complete", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting report: {ex.Message}", "Export Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportToCSV(string fileName)
        {
            using (var writer = new StreamWriter(fileName))
            {
                // Write headers
                var headers = reportGridView.Columns.Cast<DataGridViewColumn>()
                    .Select(column => $"\"{column.HeaderText}\"");
                writer.WriteLine(string.Join(",", headers));

                // Write data
                foreach (DataGridViewRow row in reportGridView.Rows)
                {
                    if (row.IsNewRow) continue;
                    
                    var values = row.Cells.Cast<DataGridViewCell>()
                        .Select(cell => $"\"{cell.Value?.ToString() ?? ""}\"");
                    writer.WriteLine(string.Join(",", values));
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