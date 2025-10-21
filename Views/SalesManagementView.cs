using SyncVerseStudio.Services;
using SyncVerseStudio.Models;
using SyncVerseStudio.Data;
using Microsoft.EntityFrameworkCore;

namespace SyncVerseStudio.Views
{
    public partial class SalesManagementView : Form
    {
        private readonly AuthenticationService _authService;
        private ApplicationDbContext? _context;
        private DataGridView salesGridView;
        private Panel filterPanel;
        private DateTimePicker fromDatePicker;
        private DateTimePicker toDatePicker;
        private ComboBox statusComboBox;
        private TextBox searchTextBox;
        private Panel summaryPanel;
        private Label totalSalesLabel;
        private Label transactionCountLabel;
        private Label averageSaleLabel;

        public SalesManagementView(AuthenticationService authService)
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
            this.Name = "SalesManagementView";
            this.Text = "Sales Management";

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
                Text = "Sales Management",
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(20, 20),
                Size = new Size(400, 40),
                TextAlign = ContentAlignment.MiddleLeft
            };
            headerPanel.Controls.Add(titleLabel);

            this.Controls.Add(headerPanel);

            // Summary Panel
            CreateSummaryPanel();

            // Filter Panel
            CreateFilterPanel();

            // Sales Grid
            CreateSalesGrid();
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

            var summaryCardsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                WrapContents = false,
                AutoSize = false
            };

            // Total Sales Card
            var totalSalesCard = CreateSummaryCard("Total Sales", "$0.00", Color.FromArgb(34, 197, 94));
            totalSalesLabel = totalSalesCard.Controls.OfType<Label>().FirstOrDefault(l => l.Name == "ValueLabel");

            // Transaction Count Card
            var transactionCard = CreateSummaryCard("Transactions", "0", Color.FromArgb(59, 130, 246));
            transactionCountLabel = transactionCard.Controls.OfType<Label>().FirstOrDefault(l => l.Name == "ValueLabel");

            // Average Sale Card
            var averageCard = CreateSummaryCard("Average Sale", "$0.00", Color.FromArgb(168, 85, 247));
            averageSaleLabel = averageCard.Controls.OfType<Label>().FirstOrDefault(l => l.Name == "ValueLabel");

            summaryCardsPanel.Controls.Add(totalSalesCard);
            summaryCardsPanel.Controls.Add(transactionCard);
            summaryCardsPanel.Controls.Add(averageCard);

            summaryPanel.Controls.Add(summaryCardsPanel);
            this.Controls.Add(summaryPanel);
        }

        private Panel CreateSummaryCard(string title, string value, Color bgColor)
        {
            var card = new Panel
            {
                Size = new Size(250, 90),
                BackColor = bgColor,
                Margin = new Padding(10),
                Padding = new Padding(20, 15, 20, 15)
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
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(0, 10),
                Size = new Size(230, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(0, 35),
                Size = new Size(230, 35),
                TextAlign = ContentAlignment.MiddleLeft,
                Name = "ValueLabel"
            };

            card.Controls.Add(titleLabel);
            card.Controls.Add(valueLabel);

            return card;
        }

        private void CreateFilterPanel()
        {
            filterPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.White,
                Padding = new Padding(20, 15, 20, 15)
            };

            // Search Box
            var searchLabel = new Label
            {
                Text = "Search:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(0, 0),
                Size = new Size(60, 25)
            };

            searchTextBox = new TextBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(0, 25),
                Size = new Size(200, 25),
                PlaceholderText = "Customer, Receipt #..."
            };
            searchTextBox.TextChanged += SearchTextBox_TextChanged;

            // Date Range
            var fromLabel = new Label
            {
                Text = "From:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(220, 0),
                Size = new Size(50, 25)
            };

            fromDatePicker = new DateTimePicker
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(220, 25),
                Size = new Size(150, 25),
                Value = DateTime.Today.AddDays(-30)
            };
            fromDatePicker.ValueChanged += FilterChanged;

            var toLabel = new Label
            {
                Text = "To:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(390, 0),
                Size = new Size(30, 25)
            };

            toDatePicker = new DateTimePicker
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(390, 25),
                Size = new Size(150, 25),
                Value = DateTime.Today
            };
            toDatePicker.ValueChanged += FilterChanged;

            // Status Filter
            var statusLabel = new Label
            {
                Text = "Status:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(560, 0),
                Size = new Size(50, 25)
            };

            statusComboBox = new ComboBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(560, 25),
                Size = new Size(120, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            statusComboBox.Items.AddRange(new[] { "All", "Completed", "Pending", "Cancelled", "Refunded" });
            statusComboBox.SelectedIndex = 0;
            statusComboBox.SelectedIndexChanged += FilterChanged;

            // Refresh Button
            var refreshButton = new Button
            {
                Text = "Refresh",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(59, 130, 246),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(700, 25),
                Size = new Size(80, 25),
                Cursor = Cursors.Hand
            };
            refreshButton.FlatAppearance.BorderSize = 0;
            refreshButton.Click += RefreshButton_Click;

            filterPanel.Controls.Add(searchLabel);
            filterPanel.Controls.Add(searchTextBox);
            filterPanel.Controls.Add(fromLabel);
            filterPanel.Controls.Add(fromDatePicker);
            filterPanel.Controls.Add(toLabel);
            filterPanel.Controls.Add(toDatePicker);
            filterPanel.Controls.Add(statusLabel);
            filterPanel.Controls.Add(statusComboBox);
            filterPanel.Controls.Add(refreshButton);

            this.Controls.Add(filterPanel);
        }

        private void CreateSalesGrid()
        {
            salesGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 10F),
                Margin = new Padding(20)
            };

            // Configure columns
            salesGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ReceiptNumber",
                HeaderText = "Receipt #",
                Width = 120,
                DataPropertyName = "ReceiptNumber"
            });

            salesGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SaleDate",
                HeaderText = "Date",
                Width = 120,
                DataPropertyName = "SaleDate",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "MM/dd/yyyy HH:mm" }
            });

            salesGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CustomerName",
                HeaderText = "Customer",
                Width = 150,
                DataPropertyName = "CustomerName"
            });

            salesGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CashierName",
                HeaderText = "Cashier",
                Width = 120,
                DataPropertyName = "CashierName"
            });

            salesGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TotalAmount",
                HeaderText = "Total",
                Width = 100,
                DataPropertyName = "TotalAmount",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
            });

            salesGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PaymentMethod",
                HeaderText = "Payment",
                Width = 100,
                DataPropertyName = "PaymentMethod"
            });

            salesGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Status",
                HeaderText = "Status",
                Width = 100,
                DataPropertyName = "Status"
            });

            salesGridView.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "ViewDetails",
                HeaderText = "Details",
                Text = "View",
                UseColumnTextForButtonValue = true,
                Width = 80
            });

            salesGridView.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "PrintReceipt",
                HeaderText = "Print",
                Text = "Print",
                UseColumnTextForButtonValue = true,
                Width = 80
            });

            salesGridView.CellClick += SalesGridView_CellClick;

            var gridPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = Color.Transparent
            };
            gridPanel.Controls.Add(salesGridView);

            this.Controls.Add(gridPanel);
        }

        private async void LoadData()
        {
            try
            {
                _context = new ApplicationDbContext();
                await LoadSales();
                await UpdateSummary();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadSales()
        {
            if (_context == null) return;

            try
            {
                var query = _context.Sales
                    .Include(s => s.Customer)
                    .Include(s => s.Cashier)
                    .Where(s => s.SaleDate >= fromDatePicker.Value.Date && 
                               s.SaleDate <= toDatePicker.Value.Date.AddDays(1));

                if (!string.IsNullOrEmpty(searchTextBox.Text))
                {
                    var searchTerm = searchTextBox.Text.ToLower();
                    query = query.Where(s => s.ReceiptNumber.ToLower().Contains(searchTerm) ||
                                           (s.Customer != null && s.Customer.FullName.ToLower().Contains(searchTerm)));
                }

                if (statusComboBox.SelectedItem?.ToString() != "All")
                {
                    if (Enum.TryParse<SaleStatus>(statusComboBox.SelectedItem?.ToString(), out var status))
                    {
                        query = query.Where(s => s.Status == status);
                    }
                }

                var sales = await query
                    .OrderByDescending(s => s.SaleDate)
                    .Select(s => new
                    {
                        s.Id,
                        s.ReceiptNumber,
                        s.SaleDate,
                        CustomerName = s.Customer != null ? s.Customer.FullName : "Walk-in Customer",
                        CashierName = s.Cashier.FullName,
                        s.TotalAmount,
                        s.PaymentMethod,
                        Status = s.Status.ToString()
                    })
                    .ToListAsync();

                salesGridView.DataSource = sales;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading sales: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task UpdateSummary()
        {
            if (_context == null) return;

            try
            {
                var fromDate = fromDatePicker.Value.Date;
                var toDate = toDatePicker.Value.Date.AddDays(1);

                var salesQuery = _context.Sales
                    .Where(s => s.SaleDate >= fromDate && 
                               s.SaleDate <= toDate &&
                               s.Status == SaleStatus.Completed);

                var totalSales = await salesQuery.SumAsync(s => (decimal?)s.TotalAmount) ?? 0;
                var transactionCount = await salesQuery.CountAsync();
                var averageSale = transactionCount > 0 ? totalSales / transactionCount : 0;

                if (totalSalesLabel != null)
                    totalSalesLabel.Text = $"${totalSales:N2}";
                
                if (transactionCountLabel != null)
                    transactionCountLabel.Text = transactionCount.ToString("N0");
                
                if (averageSaleLabel != null)
                    averageSaleLabel.Text = $"${averageSale:N2}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating summary: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            await LoadSales();
        }

        private async void FilterChanged(object sender, EventArgs e)
        {
            await LoadSales();
            await UpdateSummary();
        }

        private async void RefreshButton_Click(object sender, EventArgs e)
        {
            await LoadSales();
            await UpdateSummary();
        }

        private void SalesGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = salesGridView.Rows[e.RowIndex];
            var saleId = (int)row.Cells["Id"].Value;

            if (e.ColumnIndex == salesGridView.Columns["ViewDetails"].Index)
            {
                ShowSaleDetails(saleId);
            }
            else if (e.ColumnIndex == salesGridView.Columns["PrintReceipt"].Index)
            {
                PrintReceipt(saleId);
            }
        }

        private void ShowSaleDetails(int saleId)
        {
            MessageBox.Show($"Showing details for Sale ID: {saleId}", "Sale Details", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void PrintReceipt(int saleId)
        {
            MessageBox.Show($"Printing receipt for Sale ID: {saleId}", "Print Receipt", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
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