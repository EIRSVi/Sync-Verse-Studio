using System.Drawing;
using SyncVerseStudio.Services;
using SyncVerseStudio.Data;
using SyncVerseStudio.Models;
using Microsoft.EntityFrameworkCore;

namespace SyncVerseStudio.Views
{
    public partial class SalesView : Form
    {
        private readonly AuthenticationService _authService;
        private readonly ApplicationDbContext _context;
        private DataGridView salesGrid;
        private Panel topPanel;
        private Button refreshButton, viewDetailsButton, refundButton, printReceiptButton;
        private TextBox searchBox;
        private ComboBox statusFilter, cashierFilter, paymentMethodFilter;
        private DateTimePicker fromDatePicker, toDatePicker;
        private Label totalSalesLabel, transactionCountLabel;

        public SalesView(AuthenticationService authService)
        {
            _authService = authService;
            _context = new ApplicationDbContext();
            InitializeComponent();
            LoadSales();
            LoadFilters();
        }

        private void InitializeComponent()
        {
            this.AutoScaleDimensions = new SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(250, 250, 250);
            this.ClientSize = new Size(1200, 700);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Name = "SalesView";
            this.Text = "Sales Management";

            CreateTopPanel();
            CreateSalesGrid();
        }

        private void CreateTopPanel()
        {
            topPanel = new Panel
            {
                BackColor = System.Drawing.Color.White,
                Dock = DockStyle.Top,
                Height = 140,
                Padding = new Padding(20, 10, 20, 10)
            };

            var titleLabel = new Label
            {
                Text = "?? Sales Management",
                Font = new System.Drawing.Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(33, 33, 33),
                Location = new Point(20, 15),
                Size = new Size(300, 30)
            };

            // First row - search and filters
            searchBox = new TextBox
            {
                PlaceholderText = "?? Search by invoice, customer...",
                Font = new System.Drawing.Font("Segoe UI", 10F),
                Location = new Point(20, 50),
                Size = new Size(200, 25)
            };
            searchBox.TextChanged += SearchBox_TextChanged;

            statusFilter = new ComboBox
            {
                Font = new System.Drawing.Font("Segoe UI", 10F),
                Location = new Point(240, 50),
                Size = new Size(120, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            statusFilter.Items.AddRange(new object[] { "All Status", "Completed", "Pending", "Cancelled", "Returned" });
            statusFilter.SelectedIndex = 0;
            statusFilter.SelectedIndexChanged += StatusFilter_SelectedIndexChanged;

            paymentMethodFilter = new ComboBox
            {
                Font = new System.Drawing.Font("Segoe UI", 10F),
                Location = new Point(380, 50),
                Size = new Size(120, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            paymentMethodFilter.Items.AddRange(new object[] { "All Methods", "Cash", "Card", "Mobile", "Mixed" });
            paymentMethodFilter.SelectedIndex = 0;
            paymentMethodFilter.SelectedIndexChanged += PaymentMethodFilter_SelectedIndexChanged;

            // Second row - date filters
            var dateLabel = new Label
            {
                Text = "?? Date Range:",
                Font = new System.Drawing.Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(64, 64, 64),
                Location = new Point(20, 85),
                Size = new Size(80, 25)
            };

            fromDatePicker = new DateTimePicker
            {
                Font = new System.Drawing.Font("Segoe UI", 10F),
                Location = new Point(105, 85),
                Size = new Size(120, 25),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today.AddDays(-7)
            };
            fromDatePicker.ValueChanged += DateFilter_Changed;

            var toLabel = new Label
            {
                Text = "to",
                Font = new System.Drawing.Font("Segoe UI", 10F),
                Location = new Point(235, 85),
                Size = new Size(20, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };

            toDatePicker = new DateTimePicker
            {
                Font = new System.Drawing.Font("Segoe UI", 10F),
                Location = new Point(260, 85),
                Size = new Size(120, 25),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today
            };
            toDatePicker.ValueChanged += DateFilter_Changed;

            // Summary labels
            totalSalesLabel = new Label
            {
                Text = "?? Total Sales: $0.00",
                Font = new System.Drawing.Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(24, 119, 18),
                Location = new Point(400, 85),
                Size = new Size(150, 25)
            };

            transactionCountLabel = new Label
            {
                Text = "?? Transactions: 0",
                Font = new System.Drawing.Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(37, 99, 102),
                Location = new Point(570, 85),
                Size = new Size(130, 25)
            };

            // Buttons
            int buttonX = 750;
            viewDetailsButton = CreateButton("??? View Details", System.Drawing.Color.FromArgb(37, 99, 102), buttonX, 50);
            viewDetailsButton.Click += ViewDetailsButton_Click;
            buttonX += 110;

            printReceiptButton = CreateButton("??? Print", System.Drawing.Color.FromArgb(24, 119, 18), buttonX, 50);
            printReceiptButton.Click += PrintReceiptButton_Click;
            buttonX += 90;

            refundButton = CreateButton("?? Refund", System.Drawing.Color.FromArgb(255, 152, 0), buttonX, 50);
            refundButton.Click += RefundButton_Click;
            buttonX += 90;

            refreshButton = CreateButton("?? Refresh", System.Drawing.Color.FromArgb(158, 158, 158), buttonX, 50);
            refreshButton.Click += RefreshButton_Click;

            topPanel.Controls.AddRange(new Control[] {
                titleLabel, searchBox, statusFilter, paymentMethodFilter,
                dateLabel, fromDatePicker, toLabel, toDatePicker,
                totalSalesLabel, transactionCountLabel,
                viewDetailsButton, printReceiptButton, refundButton, refreshButton
            });

            this.Controls.Add(topPanel);
        }

        private Button CreateButton(string text, System.Drawing.Color backgroundColor, int x, int y)
        {
            return new Button
            {
                Text = text,
                Font = new System.Drawing.Font("Segoe UI", 9F, FontStyle.Bold),
                BackColor = backgroundColor,
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Location = new Point(x, y),
                Size = new Size(text.Contains("View Details") ? 100 : 80, 30),
                Cursor = Cursors.Hand
            };
        }

        private void CreateSalesGrid()
        {
            // Create a wrapper panel with padding
            var gridPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.FromArgb(250, 250, 250),
                Padding = new Padding(10, 200, 10, 10) // Left, Top, Right, Bottom - 200px padding from top
            };

            salesGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = System.Drawing.Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new System.Drawing.Font("Segoe UI", 9F),
                RowHeadersVisible = false,
                GridColor = System.Drawing.Color.FromArgb(230, 230, 230),
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = System.Drawing.Color.White,
                    ForeColor = System.Drawing.Color.FromArgb(33, 33, 33),
                    SelectionBackColor = System.Drawing.Color.FromArgb(24, 119, 18),
                    SelectionForeColor = System.Drawing.Color.White,
                    Padding = new Padding(5)
                },
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = System.Drawing.Color.FromArgb(245, 245, 245),
                    ForeColor = System.Drawing.Color.FromArgb(33, 33, 33),
                    Font = new System.Drawing.Font("Segoe UI", 9F, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(5)
                },
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 35
            };

            // Configure columns
            salesGrid.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID", Width = 50, Visible = false },
                new DataGridViewTextBoxColumn { Name = "InvoiceNumber", HeaderText = "?? Invoice", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "Customer", HeaderText = "?? Customer", Width = 150 },
                new DataGridViewTextBoxColumn { Name = "Cashier", HeaderText = "?? Cashier", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "TotalAmount", HeaderText = "?? Total", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "PaymentMethod", HeaderText = "?? Payment", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "ItemCount", HeaderText = "?? Items", Width = 80 },
                new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "?? Status", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "SaleDate", HeaderText = "?? Date & Time", Width = 150 }
            });

            // Format currency and date columns
            salesGrid.Columns["TotalAmount"].DefaultCellStyle.Format = "C2";
            salesGrid.Columns["SaleDate"].DefaultCellStyle.Format = "MM/dd/yyyy HH:mm";

            gridPanel.Controls.Add(salesGrid);
            this.Controls.Add(gridPanel);
        }

        private async void LoadSales()
        {
            try
            {
                var fromDate = fromDatePicker.Value.Date;
                var toDate = toDatePicker.Value.Date.AddDays(1).AddSeconds(-1);

                var sales = await _context.Sales
                    .Include(s => s.Customer)
                    .Include(s => s.Cashier)
                    .Include(s => s.SaleItems)
                    .Where(s => s.SaleDate >= fromDate && s.SaleDate <= toDate)
                    .OrderByDescending(s => s.SaleDate)
                    .ToListAsync();

                // Apply role-based filtering
                if (_authService.CurrentUser?.Role == UserRole.Cashier)
                {
                    sales = sales.Where(s => s.CashierId == _authService.CurrentUser.Id).ToList();
                }

                salesGrid.Rows.Clear();

                decimal totalSales = 0;
                int transactionCount = 0;

                foreach (var sale in sales)
                {
                    var customerName = sale.Customer != null ? 
                        $"{sale.Customer.FirstName} {sale.Customer.LastName}" : 
                        "Walk-in Customer";

                    var cashierName = sale.Cashier?.FullName ?? "Unknown";

                    var statusIcon = sale.Status switch
                    {
                        SaleStatus.Completed => "?",
                        SaleStatus.Pending => "?",
                        SaleStatus.Cancelled => "?",
                        SaleStatus.Returned => "??",
                        _ => "??"
                    };

                    var paymentIcon = sale.PaymentMethod switch
                    {
                        PaymentMethod.Cash => "??",
                        PaymentMethod.Card => "??",
                        PaymentMethod.Mobile => "??",
                        PaymentMethod.Mixed => "??",
                        _ => "??"
                    };

                    var itemCount = sale.SaleItems?.Count ?? 0;

                    var rowIndex = salesGrid.Rows.Add(
                        sale.Id,
                        sale.InvoiceNumber,
                        customerName,
                        cashierName,
                        sale.TotalAmount,
                        $"{paymentIcon} {sale.PaymentMethod}",
                        $"?? {itemCount}",
                        $"{statusIcon} {sale.Status}",
                        sale.SaleDate
                    );

                    // Color code different status types
                    var row = salesGrid.Rows[rowIndex];
                    switch (sale.Status)
                    {
                        case SaleStatus.Completed:
                            row.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(235, 255, 235);
                            break;
                        case SaleStatus.Pending:
                            row.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(255, 248, 235);
                            break;
                        case SaleStatus.Cancelled:
                            row.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(255, 235, 235);
                            break;
                        case SaleStatus.Returned:
                            row.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(248, 235, 255);
                            break;
                    }

                    // Calculate totals
                    if (sale.Status == SaleStatus.Completed)
                    {
                        totalSales += sale.TotalAmount;
                        transactionCount++;
                    }
                }

                // Update summary labels
                totalSalesLabel.Text = $"?? Total Sales: {totalSales:C2}";
                transactionCountLabel.Text = $"?? Transactions: {transactionCount}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading sales: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void LoadFilters()
        {
            try
            {
                // This method can load dynamic filter options if needed
                // For now, static options are sufficient
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading filters: {ex.Message}");
            }
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            FilterSales();
        }

        private void StatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterSales();
        }

        private void PaymentMethodFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterSales();
        }

        private void DateFilter_Changed(object sender, EventArgs e)
        {
            LoadSales();
        }

        private async void FilterSales()
        {
            // Implementation for filtering sales based on criteria
            LoadSales(); // For now, reload all sales
        }

        private void ViewDetailsButton_Click(object sender, EventArgs e)
        {
            if (salesGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a sale to view details.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var saleId = (int)salesGrid.SelectedRows[0].Cells["Id"].Value;
            MessageBox.Show($"??? Sale Details View - Coming Soon!\n\nThis will show:\n\n• Complete transaction details\n• Item breakdown\n• Customer information\n• Payment details\n• Timestamps\n\nSale ID: {saleId}", 
                "Feature In Development", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void PrintReceiptButton_Click(object sender, EventArgs e)
        {
            if (salesGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a sale to print receipt.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            MessageBox.Show("??? Receipt Printing - Coming Soon!\n\nThis feature will:\n\n• Generate professional receipts\n• Support thermal printer formats\n• Include company branding\n• Show detailed item breakdown\n• Display payment information", 
                "Feature In Development", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void RefundButton_Click(object sender, EventArgs e)
        {
            if (salesGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a sale to process refund.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (_authService.CurrentUser?.Role == UserRole.Cashier)
            {
                MessageBox.Show("Refund processing requires supervisor approval.", "Access Restricted", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show("?? Refund Processing - Coming Soon!\n\nThis feature will:\n\n• Process full/partial refunds\n• Update inventory levels\n• Generate refund receipts\n• Track refund history\n• Require authorization", 
                "Feature In Development", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            LoadSales();
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