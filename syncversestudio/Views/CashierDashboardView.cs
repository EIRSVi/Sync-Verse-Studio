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
    public partial class CashierDashboardView : Form
    {
        private readonly AuthenticationService _authService;
        private readonly ApplicationDbContext _context;
        
        // Panels
        private Panel headerPanel = null!;
        private Panel metricsPanel = null!;
        private Panel quickActionsPanel = null!;
        private Panel recentSalesPanel = null!;
        private Panel todayStatsPanel = null!;
        
        // Metric labels
        private Label todaySalesLabel = null!;
        private Label todayTransactionsLabel = null!;
        private Label avgTransactionLabel = null!;
        private Label myTotalSalesLabel = null!;
        private Label customersServedLabel = null!;
        private Label cashPaymentsLabel = null!;
        
        // Recent sales grid
        private DataGridView recentSalesGrid = null!;
        
        // Timer for auto-refresh
        private System.Windows.Forms.Timer refreshTimer = null!;

        public CashierDashboardView(AuthenticationService authService)
        {
            _authService = authService;
            _context = new ApplicationDbContext();
            InitializeComponent();
            LoadDashboard();
            StartAutoRefresh();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form settings
            this.Text = "Cashier Dashboard";
            this.Size = new Size(1400, 900);
            this.BackColor = Color.FromArgb(248, 250, 252);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;

            // Header Panel
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = Color.FromArgb(34, 197, 94),
                Padding = new Padding(30, 20, 30, 20)
            };

            var welcomeIcon = new IconPictureBox
            {
                IconChar = IconChar.CashRegister,
                IconColor = Color.White,
                IconSize = 40,
                Location = new Point(30, 30),
                Size = new Size(40, 40)
            };

            var welcomeLabel = new Label
            {
                Text = $"Welcome back, {_authService.CurrentUser.FullName}!",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(80, 20),
                AutoSize = true
            };

            var roleLabel = new Label
            {
                Text = "Cashier Dashboard - Point of Sale Operations",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(220, 252, 231),
                Location = new Point(80, 55),
                AutoSize = true
            };

            var timeLabel = new Label
            {
                Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy - hh:mm tt"),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                Location = new Point(1050, 35),
                AutoSize = true
            };

            headerPanel.Controls.AddRange(new Control[] { welcomeIcon, welcomeLabel, roleLabel, timeLabel });

            // Metrics Panel (KPI Cards)
            metricsPanel = new Panel
            {
                Location = new Point(30, 120),
                Size = new Size(1340, 140),
                BackColor = Color.Transparent
            };

            CreateMetricCards();

            // Quick Actions Panel
            quickActionsPanel = new Panel
            {
                Location = new Point(30, 280),
                Size = new Size(440, 560),
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            CreateQuickActionsSection();

            // Today's Stats Panel
            todayStatsPanel = new Panel
            {
                Location = new Point(490, 280),
                Size = new Size(440, 280),
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            CreateTodayStatsSection();

            // Recent Sales Panel
            recentSalesPanel = new Panel
            {
                Location = new Point(950, 280),
                Size = new Size(420, 560),
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            CreateRecentSalesSection();

            // Performance Panel
            var performancePanel = new Panel
            {
                Location = new Point(490, 580),
                Size = new Size(440, 260),
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            CreatePerformanceSection(performancePanel);

            // Add all to form
            this.Controls.AddRange(new Control[] { 
                headerPanel, metricsPanel, quickActionsPanel, 
                todayStatsPanel, recentSalesPanel, performancePanel 
            });

            this.ResumeLayout(false);
        }

        private void CreateMetricCards()
        {
            int cardWidth = 210;
            int spacing = 15;

            // Today's Sales
            var todaySalesCard = CreateMetricCard("Today's Sales", "$0.00", IconChar.DollarSign, 
                Color.FromArgb(34, 197, 94), 0 * (cardWidth + spacing), out todaySalesLabel);
            
            // Today's Transactions
            var transactionsCard = CreateMetricCard("Transactions", "0", IconChar.Receipt, 
                Color.FromArgb(59, 130, 246), 1 * (cardWidth + spacing), out todayTransactionsLabel);
            
            // Average Transaction
            var avgCard = CreateMetricCard("Avg Transaction", "$0.00", IconChar.ChartLine, 
                Color.FromArgb(168, 85, 247), 2 * (cardWidth + spacing), out avgTransactionLabel);
            
            // My Total Sales
            var myTotalCard = CreateMetricCard("My Total Sales", "$0.00", IconChar.Award, 
                Color.FromArgb(249, 115, 22), 3 * (cardWidth + spacing), out myTotalSalesLabel);
            
            // Customers Served
            var customersCard = CreateMetricCard("Customers Served", "0", IconChar.Users, 
                Color.FromArgb(236, 72, 153), 4 * (cardWidth + spacing), out customersServedLabel);
            
            // Cash Payments
            var cashCard = CreateMetricCard("Cash Payments", "$0.00", IconChar.MoneyBill, 
                Color.FromArgb(14, 165, 233), 5 * (cardWidth + spacing), out cashPaymentsLabel);

            metricsPanel.Controls.AddRange(new Control[] { 
                todaySalesCard, transactionsCard, avgCard, myTotalCard, customersCard, cashCard 
            });
        }

        private Panel CreateMetricCard(string title, string value, IconChar icon, Color color, int xPos, out Label valueLabel)
        {
            var card = new Panel
            {
                Location = new Point(xPos, 0),
                Size = new Size(210, 140),
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            var iconBox = new IconPictureBox
            {
                IconChar = icon,
                IconColor = color,
                IconSize = 32,
                Location = new Point(15, 15),
                Size = new Size(32, 32)
            };

            var titleLbl = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(15, 55),
                AutoSize = true
            };

            valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(15, 75),
                AutoSize = true
            };

            var updateLabel = new Label
            {
                Text = "Live",
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.FromArgb(34, 197, 94),
                Location = new Point(15, 115),
                AutoSize = true
            };

            card.Controls.AddRange(new Control[] { iconBox, titleLbl, valueLabel, updateLabel });
            
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

        private void CreateQuickActionsSection()
        {
            var title = new Label
            {
                Text = "Quick Actions",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(0, 0),
                AutoSize = true
            };

            var subtitle = new Label
            {
                Text = "Common tasks and operations",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(0, 30),
                AutoSize = true
            };

            // Quick action buttons
            var yPos = 70;
            var buttonWidth = 400;
            var buttonHeight = 60;
            var spacing = 15;

            var newSaleBtn = CreateQuickActionButton("New Sale", "Process customer transaction", 
                IconChar.CashRegister, Color.FromArgb(34, 197, 94), yPos);
            newSaleBtn.Click += (s, e) => OpenPointOfSale();
            yPos += buttonHeight + spacing;

            var viewSalesBtn = CreateQuickActionButton("Sales History", "View my sales transactions", 
                IconChar.Receipt, Color.FromArgb(59, 130, 246), yPos);
            viewSalesBtn.Click += (s, e) => OpenSalesHistory();
            yPos += buttonHeight + spacing;

            var customerBtn = CreateQuickActionButton("Manage Customers", "Add or update customer info", 
                IconChar.UserFriends, Color.FromArgb(168, 85, 247), yPos);
            customerBtn.Click += (s, e) => OpenCustomerManagement();
            yPos += buttonHeight + spacing;

            var refundBtn = CreateQuickActionButton("Process Refund", "Handle returns and refunds", 
                IconChar.Undo, Color.FromArgb(239, 68, 68), yPos);
            refundBtn.Click += (s, e) => ProcessRefund();
            yPos += buttonHeight + spacing;

            var reportBtn = CreateQuickActionButton("My Reports", "View personal sales reports", 
                IconChar.ChartBar, Color.FromArgb(249, 115, 22), yPos);
            reportBtn.Click += (s, e) => ViewMyReports();
            yPos += buttonHeight + spacing;

            var helpBtn = CreateQuickActionButton("Help & Support", "Get assistance", 
                IconChar.QuestionCircle, Color.FromArgb(100, 116, 139), yPos);
            helpBtn.Click += (s, e) => ShowHelp();

            quickActionsPanel.Controls.AddRange(new Control[] { 
                title, subtitle, newSaleBtn, viewSalesBtn, customerBtn, refundBtn, reportBtn, helpBtn 
            });
        }

        private Button CreateQuickActionButton(string title, string description, IconChar icon, Color color, int yPos)
        {
            var button = new Button
            {
                Location = new Point(0, yPos),
                Size = new Size(400, 60),
                BackColor = Color.FromArgb(248, 250, 252),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(60, 0, 0, 0)
            };
            button.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);

            var iconBox = new IconPictureBox
            {
                IconChar = icon,
                IconColor = color,
                IconSize = 28,
                Location = new Point(15, 16),
                Size = new Size(28, 28),
                BackColor = Color.Transparent
            };
            button.Controls.Add(iconBox);

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(60, 10),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            button.Controls.Add(titleLabel);

            var descLabel = new Label
            {
                Text = description,
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(60, 32),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            button.Controls.Add(descLabel);

            button.MouseEnter += (s, e) => button.BackColor = Color.FromArgb(241, 245, 249);
            button.MouseLeave += (s, e) => button.BackColor = Color.FromArgb(248, 250, 252);

            return button;
        }

        private void CreateTodayStatsSection()
        {
            var title = new Label
            {
                Text = "Today's Performance",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(0, 0),
                AutoSize = true
            };

            var subtitle = new Label
            {
                Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy"),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(0, 30),
                AutoSize = true
            };

            // Stats items
            var yPos = 70;
            var stats = new[]
            {
                ("First Sale", "N/A", IconChar.Clock, Color.FromArgb(34, 197, 94)),
                ("Last Sale", "N/A", IconChar.Clock, Color.FromArgb(59, 130, 246)),
                ("Peak Hour", "N/A", IconChar.ChartLine, Color.FromArgb(168, 85, 247)),
                ("Payment Methods", "Cash: 0 | Card: 0", IconChar.CreditCard, Color.FromArgb(249, 115, 22))
            };

            foreach (var (label, value, icon, color) in stats)
            {
                var statItem = CreateStatItem(label, value, icon, color, yPos);
                todayStatsPanel.Controls.Add(statItem);
                yPos += 45;
            }

            todayStatsPanel.Controls.AddRange(new Control[] { title, subtitle });
        }

        private Panel CreateStatItem(string label, string value, IconChar icon, Color color, int yPos)
        {
            var panel = new Panel
            {
                Location = new Point(0, yPos),
                Size = new Size(400, 40),
                BackColor = Color.Transparent
            };

            var iconBox = new IconPictureBox
            {
                IconChar = icon,
                IconColor = color,
                IconSize = 20,
                Location = new Point(0, 10),
                Size = new Size(20, 20)
            };

            var labelText = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(30, 0),
                AutoSize = true
            };

            var valueText = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(30, 18),
                AutoSize = true
            };

            panel.Controls.AddRange(new Control[] { iconBox, labelText, valueText });
            return panel;
        }

        private void CreateRecentSalesSection()
        {
            var title = new Label
            {
                Text = "Recent Transactions",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(0, 0),
                AutoSize = true
            };

            var subtitle = new Label
            {
                Text = "Last 10 sales",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(0, 30),
                AutoSize = true
            };

            recentSalesGrid = new DataGridView
            {
                Location = new Point(0, 60),
                Size = new Size(380, 480),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(248, 250, 252),
                    ForeColor = Color.FromArgb(30, 41, 59),
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    Padding = new Padding(5)
                }
            };

            recentSalesGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Invoice", HeaderText = "Invoice", Width = 80 });
            recentSalesGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Time", HeaderText = "Time", Width = 70 });
            recentSalesGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Amount", HeaderText = "Amount", Width = 80 });
            recentSalesGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Status", Width = 80 });

            recentSalesPanel.Controls.AddRange(new Control[] { title, subtitle, recentSalesGrid });
        }

        private void CreatePerformanceSection(Panel panel)
        {
            var title = new Label
            {
                Text = "Performance Insights",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(0, 0),
                AutoSize = true
            };

            var subtitle = new Label
            {
                Text = "Your sales performance metrics",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(0, 30),
                AutoSize = true
            };

            // Performance metrics
            var yPos = 70;
            var metrics = new[]
            {
                ("Sales Target", "Goal: $1,000 | Achieved: $0", IconChar.Bullseye, Color.FromArgb(34, 197, 94)),
                ("Customer Satisfaction", "Rating: N/A", IconChar.Star, Color.FromArgb(249, 115, 22)),
                ("Transaction Speed", "Avg: N/A minutes", IconChar.Clock, Color.FromArgb(59, 130, 246)),
                ("Top Selling Item", "N/A", IconChar.Trophy, Color.FromArgb(168, 85, 247))
            };

            foreach (var (label, value, icon, color) in metrics)
            {
                var metricItem = CreateStatItem(label, value, icon, color, yPos);
                panel.Controls.Add(metricItem);
                yPos += 45;
            }

            panel.Controls.AddRange(new Control[] { title, subtitle });
        }

        private void LoadDashboard()
        {
            try
            {
                var today = DateTime.Today;
                var cashierId = _authService.CurrentUser.Id;

                // Get today's sales for this cashier
                var todaySales = _context.Sales
                    .Include(s => s.SaleItems)
                    .Include(s => s.Customer)
                    .Where(s => s.CashierId == cashierId && 
                               s.SaleDate >= today && 
                               s.Status == SaleStatus.Completed)
                    .ToList();

                // Calculate metrics
                var todayTotal = todaySales.Sum(s => s.TotalAmount);
                var todayCount = todaySales.Count;
                var avgTransaction = todayCount > 0 ? todayTotal / todayCount : 0;

                // Get all-time sales for this cashier
                var allTimeSales = _context.Sales
                    .Where(s => s.CashierId == cashierId && s.Status == SaleStatus.Completed)
                    .Sum(s => s.TotalAmount);

                // Get unique customers served today
                var customersServed = todaySales.Where(s => s.CustomerId.HasValue).Select(s => s.CustomerId).Distinct().Count();

                // Get cash payments today
                var cashPayments = todaySales.Where(s => s.PaymentMethod == PaymentMethod.Cash).Sum(s => s.TotalAmount);

                // Update UI
                todaySalesLabel.Text = $"${todayTotal:N2}";
                todayTransactionsLabel.Text = todayCount.ToString();
                avgTransactionLabel.Text = $"${avgTransaction:N2}";
                myTotalSalesLabel.Text = $"${allTimeSales:N2}";
                customersServedLabel.Text = customersServed.ToString();
                cashPaymentsLabel.Text = $"${cashPayments:N2}";

                // Load recent sales
                LoadRecentSales();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading dashboard: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadRecentSales()
        {
            try
            {
                var cashierId = _authService.CurrentUser.Id;
                var recentSales = _context.Sales
                    .Where(s => s.CashierId == cashierId)
                    .OrderByDescending(s => s.SaleDate)
                    .Take(10)
                    .ToList();

                recentSalesGrid.Rows.Clear();
                foreach (var sale in recentSales)
                {
                    recentSalesGrid.Rows.Add(
                        sale.InvoiceNumber,
                        sale.SaleDate.ToString("HH:mm"),
                        $"${sale.TotalAmount:N2}",
                        sale.Status.ToString()
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading recent sales: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StartAutoRefresh()
        {
            refreshTimer = new System.Windows.Forms.Timer
            {
                Interval = 30000 // Refresh every 30 seconds
            };
            refreshTimer.Tick += (s, e) => LoadDashboard();
            refreshTimer.Start();
        }

        // Quick action handlers
        private void OpenPointOfSale()
        {
            var posForm = new PointOfSaleView(_authService);
            posForm.ShowDialog();
            LoadDashboard(); // Refresh after closing POS
        }

        private void OpenSalesHistory()
        {
            var salesForm = new SalesView(_authService);
            salesForm.ShowDialog();
        }

        private void OpenCustomerManagement()
        {
            var customerForm = new CustomerManagementView(_authService);
            customerForm.ShowDialog();
        }

        private void ProcessRefund()
        {
            MessageBox.Show("Refund processing feature coming soon!\n\nThis will allow you to:\n- Search for transactions\n- Process refunds\n- Handle returns\n- Update inventory", 
                "Process Refund", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ViewMyReports()
        {
            MessageBox.Show($"Personal Sales Report for {_authService.CurrentUser.FullName}\n\nThis will show:\n- Your sales history\n- Performance metrics\n- Commission details\n- Daily reconciliation", 
                "My Reports", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowHelp()
        {
            MessageBox.Show("Cashier Help & Support\n\n" +
                "Quick Tips:\n" +
                "• Use 'New Sale' to start a transaction\n" +
                "• Scan barcodes or search products\n" +
                "• Apply customer loyalty points\n" +
                "• Process payments (Cash/Card)\n" +
                "• Print receipts for customers\n\n" +
                "For assistance, contact your supervisor.", 
                "Help & Support", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
