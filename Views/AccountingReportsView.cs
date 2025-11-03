using System;
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
    public partial class AccountingReportsView : Form
    {
        private readonly AuthenticationService _authService;
        private readonly ApplicationDbContext _context;
        private TabControl _tabControl;
        private DateTimePicker _startDatePicker;
        private DateTimePicker _endDatePicker;

        public AccountingReportsView(AuthenticationService authService)
        {
            _authService = authService;
            _context = new ApplicationDbContext();
            InitializeComponent();
            LoadAccountingData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(248, 250, 252);
            this.Size = new Size(1400, 900);

            CreateHeaderPanel();
            CreateContentPanel();

            this.ResumeLayout(false);
        }

        private void CreateHeaderPanel()
        {
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = Color.White,
                Padding = new Padding(30, 20)
            };

            var iconBox = new IconPictureBox
            {
                IconChar = IconChar.ChartLine,
                IconColor = Color.FromArgb(59, 130, 246),
                IconSize = 36,
                Location = new Point(30, 22),
                Size = new Size(36, 36)
            };

            var titleLabel = new Label
            {
                Text = "Accounting Reports",
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(75, 22),
                Size = new Size(400, 35),
                AutoSize = false
            };

            var subtitleLabel = new Label
            {
                Text = "General Ledger • Books of Entry • Financial Statements",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(120, 120, 120),
                Location = new Point(75, 58),
                Size = new Size(500, 20),
                AutoSize = false
            };

            // Date Range Filters
            var dateLabel = new Label
            {
                Text = "Date Range:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(this.Width - 550, 25),
                Size = new Size(90, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _startDatePicker = new DateTimePicker
            {
                Location = new Point(this.Width - 450, 25),
                Size = new Size(150, 25),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now.AddMonths(-1)
            };
            _startDatePicker.ValueChanged += (s, e) => LoadAccountingData();

            var toLabel = new Label
            {
                Text = "to",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(120, 120, 120),
                Location = new Point(this.Width - 290, 25),
                Size = new Size(30, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };

            _endDatePicker = new DateTimePicker
            {
                Location = new Point(this.Width - 250, 25),
                Size = new Size(150, 25),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now
            };
            _endDatePicker.ValueChanged += (s, e) => LoadAccountingData();

            var refreshButton = new Button
            {
                Text = "Refresh",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(59, 130, 246),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(this.Width - 80, 25),
                Size = new Size(70, 30),
                Cursor = Cursors.Hand
            };
            refreshButton.FlatAppearance.BorderSize = 0;
            refreshButton.Click += (s, e) => LoadAccountingData();

            headerPanel.Controls.AddRange(new Control[] { 
                iconBox, titleLabel, subtitleLabel, 
                dateLabel, _startDatePicker, toLabel, _endDatePicker, refreshButton 
            });

            this.Controls.Add(headerPanel);
        }

        private void CreateContentPanel()
        {
            var contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 250, 252),
                Padding = new Padding(30, 20),
                AutoScroll = true
            };

            _tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F),
                Padding = new Point(10, 10)
            };

            // Create tabs
            _tabControl.TabPages.Add(CreateGeneralLedgerTab());
            _tabControl.TabPages.Add(CreateSalesDayBookTab());
            _tabControl.TabPages.Add(CreatePurchasesDayBookTab());
            _tabControl.TabPages.Add(CreateCashBookTab());
            _tabControl.TabPages.Add(CreateGeneralJournalTab());
            _tabControl.TabPages.Add(CreateFinancialStatementsTab());

            contentPanel.Controls.Add(_tabControl);
            this.Controls.Add(contentPanel);
        }

        private TabPage CreateGeneralLedgerTab()
        {
            var tab = new TabPage("General Ledger")
            {
                BackColor = Color.FromArgb(248, 250, 252),
                Padding = new Padding(20)
            };

            var grid = CreateDataGridView();
            grid.Dock = DockStyle.Fill;
            grid.Tag = "GeneralLedger";

            grid.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "EntryNumber", HeaderText = "Entry #", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "EntryDate", HeaderText = "Date", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "AccountName", HeaderText = "Account", Width = 200 },
                new DataGridViewTextBoxColumn { Name = "AccountType", HeaderText = "Type", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "DebitAmount", HeaderText = "Debit", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "CreditAmount", HeaderText = "Credit", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "Description", HeaderText = "Description", Width = 250 },
                new DataGridViewTextBoxColumn { Name = "BookOfEntry", HeaderText = "Book", Width = 150 }
            });

            tab.Controls.Add(grid);
            return tab;
        }

        private TabPage CreateSalesDayBookTab()
        {
            var tab = new TabPage("Sales Day Book")
            {
                BackColor = Color.FromArgb(248, 250, 252),
                Padding = new Padding(20)
            };

            var grid = CreateDataGridView();
            grid.Dock = DockStyle.Fill;
            grid.Tag = "SalesDayBook";

            grid.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "InvoiceNumber", HeaderText = "Invoice #", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "SaleDate", HeaderText = "Date", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "CustomerName", HeaderText = "Customer", Width = 200 },
                new DataGridViewTextBoxColumn { Name = "SubTotal", HeaderText = "Sub Total", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "TaxAmount", HeaderText = "Tax", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "TotalAmount", HeaderText = "Total", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "PaymentMethod", HeaderText = "Payment", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Status", Width = 100 }
            });

            tab.Controls.Add(grid);
            return tab;
        }

        private TabPage CreatePurchasesDayBookTab()
        {
            var tab = new TabPage("Purchases Day Book")
            {
                BackColor = Color.FromArgb(248, 250, 252),
                Padding = new Padding(20)
            };

            var grid = CreateDataGridView();
            grid.Dock = DockStyle.Fill;
            grid.Tag = "PurchasesDayBook";

            grid.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "PurchaseNumber", HeaderText = "Purchase #", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "PurchaseDate", HeaderText = "Date", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "SupplierName", HeaderText = "Supplier", Width = 200 },
                new DataGridViewTextBoxColumn { Name = "TotalAmount", HeaderText = "Total Amount", Width = 150 },
                new DataGridViewTextBoxColumn { Name = "PaidAmount", HeaderText = "Paid", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "Balance", HeaderText = "Balance", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Status", Width = 100 }
            });

            tab.Controls.Add(grid);
            return tab;
        }

        private TabPage CreateCashBookTab()
        {
            var tab = new TabPage("Cash Book")
            {
                BackColor = Color.FromArgb(248, 250, 252),
                Padding = new Padding(20)
            };

            var grid = CreateDataGridView();
            grid.Dock = DockStyle.Fill;
            grid.Tag = "CashBook";

            grid.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "PaymentReference", HeaderText = "Reference", Width = 150 },
                new DataGridViewTextBoxColumn { Name = "PaymentDate", HeaderText = "Date", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "Description", HeaderText = "Description", Width = 250 },
                new DataGridViewTextBoxColumn { Name = "CashIn", HeaderText = "Cash In", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "CashOut", HeaderText = "Cash Out", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "Balance", HeaderText = "Balance", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "PaymentMethod", HeaderText = "Method", Width = 120 }
            });

            tab.Controls.Add(grid);
            return tab;
        }

        private TabPage CreateGeneralJournalTab()
        {
            var tab = new TabPage("General Journal")
            {
                BackColor = Color.FromArgb(248, 250, 252),
                Padding = new Padding(20)
            };

            var grid = CreateDataGridView();
            grid.Dock = DockStyle.Fill;
            grid.Tag = "GeneralJournal";

            grid.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "EntryNumber", HeaderText = "Entry #", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "EntryDate", HeaderText = "Date", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "AccountName", HeaderText = "Account", Width = 200 },
                new DataGridViewTextBoxColumn { Name = "DebitAmount", HeaderText = "Debit", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "CreditAmount", HeaderText = "Credit", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "Description", HeaderText = "Description", Width = 300 }
            });

            tab.Controls.Add(grid);
            return tab;
        }

        private TabPage CreateFinancialStatementsTab()
        {
            var tab = new TabPage("Financial Statements")
            {
                BackColor = Color.FromArgb(248, 250, 252),
                Padding = new Padding(20)
            };

            var splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = 700
            };

            // Left side - Balance Sheet
            var balanceSheetPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            var bsTitle = new Label
            {
                Text = "Balance Sheet",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(20, 20),
                Size = new Size(300, 35)
            };

            var balanceSheetGrid = CreateDataGridView();
            balanceSheetGrid.Location = new Point(20, 70);
            balanceSheetGrid.Size = new Size(640, 700);
            balanceSheetGrid.Tag = "BalanceSheet";

            balanceSheetGrid.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "Category", HeaderText = "Category", Width = 200 },
                new DataGridViewTextBoxColumn { Name = "AccountName", HeaderText = "Account", Width = 250 },
                new DataGridViewTextBoxColumn { Name = "Amount", HeaderText = "Amount", Width = 150 }
            });

            balanceSheetPanel.Controls.AddRange(new Control[] { bsTitle, balanceSheetGrid });
            splitContainer.Panel1.Controls.Add(balanceSheetPanel);

            // Right side - Income Statement
            var incomeStatementPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            var isTitle = new Label
            {
                Text = "Income Statement",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(20, 20),
                Size = new Size(300, 35)
            };

            var incomeStatementGrid = CreateDataGridView();
            incomeStatementGrid.Location = new Point(20, 70);
            incomeStatementGrid.Size = new Size(620, 700);
            incomeStatementGrid.Tag = "IncomeStatement";

            incomeStatementGrid.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "Category", HeaderText = "Category", Width = 200 },
                new DataGridViewTextBoxColumn { Name = "AccountName", HeaderText = "Account", Width = 250 },
                new DataGridViewTextBoxColumn { Name = "Amount", HeaderText = "Amount", Width = 130 }
            });

            incomeStatementPanel.Controls.AddRange(new Control[] { isTitle, incomeStatementGrid });
            splitContainer.Panel2.Controls.Add(incomeStatementPanel);

            tab.Controls.Add(splitContainer);
            return tab;
        }

        private DataGridView CreateDataGridView()
        {
            var grid = new DataGridView
            {
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 10F),
                ColumnHeadersHeight = 45,
                RowTemplate = { Height = 40 },
                AlternatingRowsDefaultCellStyle = { BackColor = Color.FromArgb(248, 250, 252) }
            };

            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(59, 130, 246);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(10);
            grid.EnableHeadersVisualStyles = false;

            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            grid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 30, 30);

            return grid;
        }

        private async void LoadAccountingData()
        {
            try
            {
                var startDate = _startDatePicker.Value.Date;
                var endDate = _endDatePicker.Value.Date.AddDays(1).AddSeconds(-1);

                await LoadGeneralLedger(startDate, endDate);
                await LoadSalesDayBook(startDate, endDate);
                await LoadPurchasesDayBook(startDate, endDate);
                await LoadCashBook(startDate, endDate);
                await LoadGeneralJournal(startDate, endDate);
                await LoadFinancialStatements(startDate, endDate);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading accounting data: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async System.Threading.Tasks.Task LoadGeneralLedger(DateTime startDate, DateTime endDate)
        {
            var entries = await _context.GeneralLedgerEntries
                .Include(e => e.CreatedByUser)
                .Where(e => e.EntryDate >= startDate && e.EntryDate <= endDate)
                .OrderByDescending(e => e.EntryDate)
                .Select(e => new
                {
                    e.EntryNumber,
                    EntryDate = e.EntryDate.ToString("yyyy-MM-dd HH:mm"),
                    e.AccountName,
                    AccountType = e.AccountType.ToString(),
                    DebitAmount = e.DebitAmount > 0 ? $"${e.DebitAmount:N2}" : "",
                    CreditAmount = e.CreditAmount > 0 ? $"${e.CreditAmount:N2}" : "",
                    e.Description,
                    BookOfEntry = e.BookOfEntry.ToString()
                })
                .ToListAsync();

            var grid = FindGridByTag("GeneralLedger");
            if (grid != null)
            {
                grid.DataSource = entries;
            }
        }

        private async System.Threading.Tasks.Task LoadSalesDayBook(DateTime startDate, DateTime endDate)
        {
            var sales = await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.SaleItems)
                .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
                .OrderByDescending(s => s.SaleDate)
                .Select(s => new
                {
                    s.InvoiceNumber,
                    SaleDate = s.SaleDate.ToString("yyyy-MM-dd HH:mm"),
                    CustomerName = s.Customer != null ? s.Customer.FullName : "Walk-in Customer",
                    SubTotal = $"${s.SubTotal:N2}",
                    TaxAmount = $"${s.TaxAmount:N2}",
                    TotalAmount = $"${s.TotalAmount:N2}",
                    PaymentMethod = s.PaymentMethod.ToString(),
                    Status = s.Status.ToString()
                })
                .ToListAsync();

            var grid = FindGridByTag("SalesDayBook");
            if (grid != null)
            {
                grid.DataSource = sales;
            }
        }

        private async System.Threading.Tasks.Task LoadPurchasesDayBook(DateTime startDate, DateTime endDate)
        {
            var purchases = await _context.Purchases
                .Include(p => p.Supplier)
                .Where(p => p.PurchaseDate >= startDate && p.PurchaseDate <= endDate)
                .OrderByDescending(p => p.PurchaseDate)
                .Select(p => new
                {
                    p.PurchaseNumber,
                    PurchaseDate = p.PurchaseDate.ToString("yyyy-MM-dd HH:mm"),
                    SupplierName = p.Supplier.Name,
                    TotalAmount = $"${p.TotalAmount:N2}",
                    PaidAmount = $"${p.PaidAmount:N2}",
                    Balance = $"${(p.TotalAmount - p.PaidAmount):N2}",
                    Status = p.Status.ToString()
                })
                .ToListAsync();

            var grid = FindGridByTag("PurchasesDayBook");
            if (grid != null)
            {
                grid.DataSource = purchases;
            }
        }

        private async System.Threading.Tasks.Task LoadCashBook(DateTime startDate, DateTime endDate)
        {
            var payments = await _context.Payments
                .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate && 
                           p.Status == PaymentStatus.Completed)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            decimal runningBalance = 0;
            var cashBookEntries = payments.Select(p =>
            {
                var isCashIn = p.InvoiceId.HasValue || p.SaleId.HasValue;
                var cashIn = isCashIn ? p.Amount : 0;
                var cashOut = !isCashIn ? p.Amount : 0;
                runningBalance += cashIn - cashOut;

                return new
                {
                    p.PaymentReference,
                    PaymentDate = p.PaymentDate.ToString("yyyy-MM-dd HH:mm"),
                    Description = p.Notes ?? (isCashIn ? "Sales Receipt" : "Purchase Payment"),
                    CashIn = cashIn > 0 ? $"${cashIn:N2}" : "",
                    CashOut = cashOut > 0 ? $"${cashOut:N2}" : "",
                    Balance = $"${runningBalance:N2}",
                    PaymentMethod = p.PaymentMethod.ToString()
                };
            }).ToList();

            var grid = FindGridByTag("CashBook");
            if (grid != null)
            {
                grid.DataSource = cashBookEntries;
            }
        }

        private async System.Threading.Tasks.Task LoadGeneralJournal(DateTime startDate, DateTime endDate)
        {
            var entries = await _context.GeneralLedgerEntries
                .Where(e => e.EntryDate >= startDate && e.EntryDate <= endDate && 
                           e.BookOfEntry == BookOfEntry.GeneralJournal)
                .OrderByDescending(e => e.EntryDate)
                .Select(e => new
                {
                    e.EntryNumber,
                    EntryDate = e.EntryDate.ToString("yyyy-MM-dd HH:mm"),
                    e.AccountName,
                    DebitAmount = e.DebitAmount > 0 ? $"${e.DebitAmount:N2}" : "",
                    CreditAmount = e.CreditAmount > 0 ? $"${e.CreditAmount:N2}" : "",
                    e.Description
                })
                .ToListAsync();

            var grid = FindGridByTag("GeneralJournal");
            if (grid != null)
            {
                grid.DataSource = entries;
            }
        }

        private async System.Threading.Tasks.Task LoadFinancialStatements(DateTime startDate, DateTime endDate)
        {
            // Calculate Balance Sheet
            var assets = await CalculateAssets();
            var liabilities = await CalculateLiabilities();
            var equity = await CalculateEquity(startDate, endDate);

            var balanceSheetData = new List<dynamic>();
            
            // Assets
            balanceSheetData.Add(new { Category = "ASSETS", AccountName = "", Amount = "" });
            balanceSheetData.Add(new { Category = "Current Assets", AccountName = "Cash", Amount = $"${assets.Cash:N2}" });
            balanceSheetData.Add(new { Category = "Current Assets", AccountName = "Inventory", Amount = $"${assets.Inventory:N2}" });
            balanceSheetData.Add(new { Category = "Current Assets", AccountName = "Accounts Receivable", Amount = $"${assets.AccountsReceivable:N2}" });
            balanceSheetData.Add(new { Category = "", AccountName = "Total Assets", Amount = $"${assets.Total:N2}" });
            balanceSheetData.Add(new { Category = "", AccountName = "", Amount = "" });
            
            // Liabilities
            balanceSheetData.Add(new { Category = "LIABILITIES", AccountName = "", Amount = "" });
            balanceSheetData.Add(new { Category = "Current Liabilities", AccountName = "Accounts Payable", Amount = $"${liabilities.AccountsPayable:N2}" });
            balanceSheetData.Add(new { Category = "", AccountName = "Total Liabilities", Amount = $"${liabilities.Total:N2}" });
            balanceSheetData.Add(new { Category = "", AccountName = "", Amount = "" });
            
            // Equity
            balanceSheetData.Add(new { Category = "EQUITY", AccountName = "", Amount = "" });
            balanceSheetData.Add(new { Category = "Owner's Equity", AccountName = "Retained Earnings", Amount = $"${equity.RetainedEarnings:N2}" });
            balanceSheetData.Add(new { Category = "", AccountName = "Total Equity", Amount = $"${equity.Total:N2}" });

            var balanceSheetGrid = FindGridByTag("BalanceSheet");
            if (balanceSheetGrid != null)
            {
                balanceSheetGrid.DataSource = balanceSheetData;
            }

            // Calculate Income Statement
            var revenue = await CalculateRevenue(startDate, endDate);
            var expenses = await CalculateExpenses(startDate, endDate);
            var netIncome = revenue.Total - expenses.Total;

            var incomeStatementData = new List<dynamic>();
            
            // Revenue
            incomeStatementData.Add(new { Category = "REVENUE", AccountName = "", Amount = "" });
            incomeStatementData.Add(new { Category = "Sales Revenue", AccountName = "Product Sales", Amount = $"${revenue.ProductSales:N2}" });
            incomeStatementData.Add(new { Category = "", AccountName = "Total Revenue", Amount = $"${revenue.Total:N2}" });
            incomeStatementData.Add(new { Category = "", AccountName = "", Amount = "" });
            
            // Expenses
            incomeStatementData.Add(new { Category = "EXPENSES", AccountName = "", Amount = "" });
            incomeStatementData.Add(new { Category = "Cost of Goods Sold", AccountName = "COGS", Amount = $"${expenses.COGS:N2}" });
            incomeStatementData.Add(new { Category = "Operating Expenses", AccountName = "General Expenses", Amount = $"${expenses.Operating:N2}" });
            incomeStatementData.Add(new { Category = "", AccountName = "Total Expenses", Amount = $"${expenses.Total:N2}" });
            incomeStatementData.Add(new { Category = "", AccountName = "", Amount = "" });
            
            // Net Income
            incomeStatementData.Add(new { Category = "NET INCOME", AccountName = "", Amount = $"${netIncome:N2}" });

            var incomeStatementGrid = FindGridByTag("IncomeStatement");
            if (incomeStatementGrid != null)
            {
                incomeStatementGrid.DataSource = incomeStatementData;
            }
        }

        private async System.Threading.Tasks.Task<dynamic> CalculateAssets()
        {
            var cash = await _context.Payments
                .Where(p => p.Status == PaymentStatus.Completed)
                .SumAsync(p => (decimal?)(p.InvoiceId.HasValue || p.SaleId.HasValue ? p.Amount : -p.Amount)) ?? 0;

            var inventory = await _context.Products
                .Where(p => p.IsActive)
                .SumAsync(p => (decimal?)(p.Quantity * p.CostPrice)) ?? 0;

            var accountsReceivable = await _context.Invoices
                .Where(i => i.Status == InvoiceStatus.Active)
                .SumAsync(i => (decimal?)i.BalanceAmount) ?? 0;

            return new
            {
                Cash = cash,
                Inventory = inventory,
                AccountsReceivable = accountsReceivable,
                Total = cash + inventory + accountsReceivable
            };
        }

        private async System.Threading.Tasks.Task<dynamic> CalculateLiabilities()
        {
            var accountsPayable = await _context.Purchases
                .Where(p => p.Status == PurchaseStatus.Completed)
                .SumAsync(p => (decimal?)(p.TotalAmount - p.PaidAmount)) ?? 0;

            return new
            {
                AccountsPayable = accountsPayable,
                Total = accountsPayable
            };
        }

        private async System.Threading.Tasks.Task<dynamic> CalculateEquity(DateTime startDate, DateTime endDate)
        {
            var revenue = await _context.Sales
                .Where(s => s.Status == SaleStatus.Completed)
                .SumAsync(s => (decimal?)s.TotalAmount) ?? 0;

            var expenses = await _context.Purchases
                .Where(p => p.Status == PurchaseStatus.Completed)
                .SumAsync(p => (decimal?)p.TotalAmount) ?? 0;

            var retainedEarnings = revenue - expenses;

            return new
            {
                RetainedEarnings = retainedEarnings,
                Total = retainedEarnings
            };
        }

        private async System.Threading.Tasks.Task<dynamic> CalculateRevenue(DateTime startDate, DateTime endDate)
        {
            var productSales = await _context.Sales
                .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate && 
                           s.Status == SaleStatus.Completed)
                .SumAsync(s => (decimal?)s.TotalAmount) ?? 0;

            return new
            {
                ProductSales = productSales,
                Total = productSales
            };
        }

        private async System.Threading.Tasks.Task<dynamic> CalculateExpenses(DateTime startDate, DateTime endDate)
        {
            var cogs = await _context.SaleItems
                .Include(si => si.Sale)
                .Include(si => si.Product)
                .Where(si => si.Sale.SaleDate >= startDate && si.Sale.SaleDate <= endDate && 
                            si.Sale.Status == SaleStatus.Completed)
                .SumAsync(si => (decimal?)(si.Quantity * si.Product.CostPrice)) ?? 0;

            var operating = await _context.Purchases
                .Where(p => p.PurchaseDate >= startDate && p.PurchaseDate <= endDate && 
                           p.Status == PurchaseStatus.Completed)
                .SumAsync(p => (decimal?)p.TotalAmount) ?? 0;

            return new
            {
                COGS = cogs,
                Operating = operating,
                Total = cogs + operating
            };
        }

        private DataGridView? FindGridByTag(string tag)
        {
            foreach (TabPage tabPage in _tabControl.TabPages)
            {
                foreach (Control control in tabPage.Controls)
                {
                    if (control is DataGridView grid && grid.Tag?.ToString() == tag)
                        return grid;
                    
                    if (control is SplitContainer split)
                    {
                        foreach (Control panelControl in split.Panel1.Controls)
                        {
                            if (panelControl is Panel panel)
                            {
                                foreach (Control innerControl in panel.Controls)
                                {
                                    if (innerControl is DataGridView innerGrid && innerGrid.Tag?.ToString() == tag)
                                        return innerGrid;
                                }
                            }
                        }
                        foreach (Control panelControl in split.Panel2.Controls)
                        {
                            if (panelControl is Panel panel)
                            {
                                foreach (Control innerControl in panel.Controls)
                                {
                                    if (innerControl is DataGridView innerGrid && innerGrid.Tag?.ToString() == tag)
                                        return innerGrid;
                                }
                            }
                        }
                    }
                }
            }
            return null;
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
