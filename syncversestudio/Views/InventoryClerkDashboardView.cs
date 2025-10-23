using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using SyncVerseStudio.Data;
using SyncVerseStudio.Services;
using FontAwesome.Sharp;

namespace SyncVerseStudio.Views
{
    public partial class InventoryClerkDashboardView : Form
    {
        private readonly AuthenticationService _authService;
        private readonly ApplicationDbContext _context;
        private System.Windows.Forms.Timer _refreshTimer;
        private Panel _activityPanel;
        private Panel _alertsPanel;

        public InventoryClerkDashboardView(AuthenticationService authService)
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
            _refreshTimer.Interval = 30000; // Refresh every 30 seconds
            _refreshTimer.Tick += async (s, e) => await RefreshLiveData();
            _refreshTimer.Start();
        }

        private async System.Threading.Tasks.Task RefreshLiveData()
        {
            await LoadDashboardData();
            await LoadRecentActivity();
            await LoadLowStockAlerts();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Name = "InventoryClerkDashboardView";
            this.Text = "Inventory Clerk Dashboard";
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
                Height = 100,
                Padding = new Padding(30, 20, 30, 20)
            };

            var iconBox = new IconPictureBox
            {
                IconChar = IconChar.BoxesStacked,
                IconColor = Color.FromArgb(34, 197, 94),
                IconSize = 36,
                Location = new Point(30, 22),
                Size = new Size(36, 36),
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(iconBox);

            var titleLabel = new Label
            {
                Text = "Inventory Clerk Dashboard",
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(75, 22),
                Size = new Size(500, 35),
                AutoSize = false,
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(titleLabel);

            var subtitleLabel = new Label
            {
                Text = "Stock Management • Product Control • Supplier Coordination",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(120, 120, 120),
                Location = new Point(75, 52),
                Size = new Size(500, 20),
                AutoSize = false,
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(subtitleLabel);

            this.Controls.Add(headerPanel);
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

            // Quick Stats Cards
            var statsFlow = new FlowLayoutPanel
            {
                Location = new Point(0, 0),
                Size = new Size(1140, 110),
                BackColor = Color.Transparent,
                WrapContents = true
            };

            CreateStatCard(statsFlow, IconChar.BoxesStacked, "Total Products", "0", Color.FromArgb(59, 130, 246), "products");
            CreateStatCard(statsFlow, IconChar.TriangleExclamation, "Low Stock Items", "0", Color.FromArgb(239, 68, 68), "lowstock");
            CreateStatCard(statsFlow, IconChar.TruckFast, "Pending Orders", "0", Color.FromArgb(249, 115, 22), "pending");
            CreateStatCard(statsFlow, IconChar.DollarSign, "Stock Value", "$0.00", Color.FromArgb(34, 197, 94), "value");
            CreateStatCard(statsFlow, IconChar.Users, "Active Suppliers", "0", Color.FromArgb(168, 85, 247), "suppliers");
            CreateStatCard(statsFlow, IconChar.ArrowsRotate, "Stock Movements", "0", Color.FromArgb(14, 165, 233), "movements");

            contentPanel.Controls.Add(statsFlow);

            // Quick Actions Section
            int sectionY = 130;
            var actionsLabel = new Label
            {
                Text = "Quick Actions",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(0, sectionY),
                Size = new Size(300, 35),
                BackColor = Color.Transparent
            };
            contentPanel.Controls.Add(actionsLabel);

            var actionsPanel = new Panel
            {
                Location = new Point(0, sectionY + 45),
                Size = new Size(1140, 180),
                BackColor = Color.White
            };

            actionsPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, actionsPanel.Width - 1, actionsPanel.Height - 1), 12))
                using (var brush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillPath(brush, path);
                }
            };

            CreateActionButton(actionsPanel, IconChar.Plus, "Add New Product", "Create new product entry", Color.FromArgb(59, 130, 246), 20, 20, ProductManagement_Click);
            CreateActionButton(actionsPanel, IconChar.ArrowUp, "Receive Stock", "Process incoming stock", Color.FromArgb(34, 197, 94), 20, 100, ReceiveStock_Click);
            CreateActionButton(actionsPanel, IconChar.PenToSquare, "Stock Adjustment", "Adjust inventory levels", Color.FromArgb(249, 115, 22), 300, 20, StockAdjustment_Click);
            CreateActionButton(actionsPanel, IconChar.ArrowsLeftRight, "Stock Transfer", "Transfer between locations", Color.FromArgb(168, 85, 247), 300, 100, StockTransfer_Click);
            CreateActionButton(actionsPanel, IconChar.FileLines, "Generate Report", "Create inventory reports", Color.FromArgb(14, 165, 233), 580, 20, GenerateReport_Click);
            CreateActionButton(actionsPanel, IconChar.TruckFast, "Manage Suppliers", "View supplier information", Color.FromArgb(236, 72, 153), 580, 100, ManageSuppliers_Click);
            CreateActionButton(actionsPanel, IconChar.BoxOpen, "View Categories", "Manage product categories", Color.FromArgb(139, 92, 246), 860, 20, ViewCategories_Click);
            CreateActionButton(actionsPanel, IconChar.Barcode, "Scan Barcode", "Quick product lookup", Color.FromArgb(6, 182, 212), 860, 100, ScanBarcode_Click);

            contentPanel.Controls.Add(actionsPanel);

            // Recent Activity & Alerts Section
            sectionY += 245;
            var activityLabel = new Label
            {
                Text = "Recent Activity & Alerts",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(0, sectionY),
                Size = new Size(300, 35),
                BackColor = Color.Transparent
            };
            contentPanel.Controls.Add(activityLabel);

            _activityPanel = new Panel
            {
                Location = new Point(0, sectionY + 45),
                Size = new Size(560, 300),
                BackColor = Color.White,
                AutoScroll = true
            };

            _activityPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, _activityPanel.Width - 1, _activityPanel.Height - 1), 12))
                using (var brush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillPath(brush, path);
                }
            };

            var activityTitle = new Label
            {
                Text = "Recent Stock Activity",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(20, 15),
                Size = new Size(300, 25),
                BackColor = Color.Transparent,
                Tag = "title"
            };
            _activityPanel.Controls.Add(activityTitle);

            var refreshIcon = new IconPictureBox
            {
                IconChar = IconChar.ArrowsRotate,
                IconColor = Color.FromArgb(120, 120, 120),
                IconSize = 16,
                Location = new Point(520, 18),
                Size = new Size(16, 16),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Tag = "refresh"
            };
            refreshIcon.Click += async (s, e) => await LoadRecentActivity();
            _activityPanel.Controls.Add(refreshIcon);

            contentPanel.Controls.Add(_activityPanel);

            // Low Stock Alerts Panel
            _alertsPanel = new Panel
            {
                Location = new Point(580, sectionY + 45),
                Size = new Size(560, 300),
                BackColor = Color.White,
                AutoScroll = true
            };

            _alertsPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, _alertsPanel.Width - 1, _alertsPanel.Height - 1), 12))
                using (var brush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillPath(brush, path);
                }
            };

            var alertsTitle = new Label
            {
                Text = "Low Stock Alerts",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(239, 68, 68),
                Location = new Point(20, 15),
                Size = new Size(300, 25),
                BackColor = Color.Transparent,
                Tag = "title"
            };
            _alertsPanel.Controls.Add(alertsTitle);

            var alertRefreshIcon = new IconPictureBox
            {
                IconChar = IconChar.ArrowsRotate,
                IconColor = Color.FromArgb(239, 68, 68),
                IconSize = 16,
                Location = new Point(520, 18),
                Size = new Size(16, 16),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Tag = "refresh"
            };
            alertRefreshIcon.Click += async (s, e) => await LoadLowStockAlerts();
            _alertsPanel.Controls.Add(alertRefreshIcon);

            contentPanel.Controls.Add(_alertsPanel);

            this.Controls.Add(contentPanel);
        }

        private void CreateStatCard(FlowLayoutPanel parent, IconChar icon, string title, string value, Color color, string tag)
        {
            var card = new Panel
            {
                Size = new Size(180, 90),
                Margin = new Padding(0, 0, 10, 10),
                BackColor = Color.White,
                Tag = tag,
                Cursor = Cursors.Hand
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
                Location = new Point(65, 20),
                Size = new Size(100, 24),
                BackColor = Color.Transparent,
                Tag = "value"
            };
            card.Controls.Add(valueLabel);

            card.MouseEnter += (s, e) => card.BackColor = Color.FromArgb(248, 250, 252);
            card.MouseLeave += (s, e) => card.BackColor = Color.White;

            parent.Controls.Add(card);
        }

        private void CreateActionButton(Panel parent, IconChar icon, string title, string description, Color color, int x, int y, EventHandler clickHandler)
        {
            var button = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(260, 60),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Tag = title
            };

            var iconBox = new IconPictureBox
            {
                IconChar = icon,
                IconColor = color,
                IconSize = 24,
                Location = new Point(10, 18),
                Size = new Size(24, 24),
                BackColor = Color.Transparent
            };
            button.Controls.Add(iconBox);

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(45, 10),
                Size = new Size(205, 22),
                BackColor = Color.Transparent
            };
            button.Controls.Add(titleLabel);

            var descLabel = new Label
            {
                Text = description,
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.FromArgb(120, 120, 120),
                Location = new Point(45, 32),
                Size = new Size(205, 18),
                BackColor = Color.Transparent
            };
            button.Controls.Add(descLabel);

            button.MouseEnter += (s, e) => button.BackColor = Color.FromArgb(248, 250, 252);
            button.MouseLeave += (s, e) => button.BackColor = Color.Transparent;
            button.Click += clickHandler;

            parent.Controls.Add(button);
        }

        private async System.Threading.Tasks.Task LoadDashboardData()
        {
            try
            {
                var totalProducts = await _context.Products.CountAsync(p => p.IsActive);
                var lowStockItems = await _context.Products.CountAsync(p => p.IsActive && p.Quantity <= p.MinQuantity);
                var criticalStock = await _context.Products.CountAsync(p => p.IsActive && p.Quantity == 0);
                var activeSuppliers = await _context.Suppliers.CountAsync(s => s.IsActive);
                
                var stockValue = await _context.Products
                    .Where(p => p.IsActive)
                    .SumAsync(p => (decimal?)(p.Quantity * p.CostPrice)) ?? 0;

                // Calculate today's movements (products updated today)
                var today = DateTime.Today;
                var todayMovements = await _context.Products
                    .CountAsync(p => p.IsActive && p.UpdatedAt.Date == today);

                // Calculate pending orders (low stock items that need reordering)
                var pendingOrders = await _context.Products
                    .CountAsync(p => p.IsActive && p.Quantity < p.MinQuantity);

                UpdateStatCard("products", totalProducts.ToString());
                UpdateStatCard("lowstock", lowStockItems.ToString());
                UpdateStatCard("pending", pendingOrders.ToString());
                UpdateStatCard("value", $"${stockValue:N2}");
                UpdateStatCard("suppliers", activeSuppliers.ToString());
                UpdateStatCard("movements", todayMovements.ToString());

                // Load live data sections
                await LoadRecentActivity();
                await LoadLowStockAlerts();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading dashboard data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async System.Threading.Tasks.Task LoadRecentActivity()
        {
            try
            {
                // Clear existing activity items (keep title and refresh icon)
                var itemsToRemove = _activityPanel.Controls.Cast<Control>()
                    .Where(c => c.Tag?.ToString() != "title" && c.Tag?.ToString() != "refresh")
                    .ToList();
                
                foreach (var item in itemsToRemove)
                {
                    _activityPanel.Controls.Remove(item);
                    item.Dispose();
                }

                // Get recently updated products (last 24 hours)
                var recentProducts = await _context.Products
                    .Include(p => p.Category)
                    .Where(p => p.IsActive && p.UpdatedAt >= DateTime.Now.AddHours(-24))
                    .OrderByDescending(p => p.UpdatedAt)
                    .Take(8)
                    .ToListAsync();

                int yPos = 50;
                
                if (recentProducts.Any())
                {
                    foreach (var product in recentProducts)
                    {
                        CreateActivityItem(_activityPanel, product, yPos);
                        yPos += 30;
                    }
                }
                else
                {
                    var noDataLabel = new Label
                    {
                        Text = "No recent stock activity in the last 24 hours",
                        Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                        ForeColor = Color.FromArgb(150, 150, 150),
                        Location = new Point(20, 50),
                        Size = new Size(500, 20),
                        BackColor = Color.Transparent
                    };
                    _activityPanel.Controls.Add(noDataLabel);
                }
            }
            catch (Exception ex)
            {
                var errorLabel = new Label
                {
                    Text = $"Error loading activity: {ex.Message}",
                    Font = new Font("Segoe UI", 9F),
                    ForeColor = Color.FromArgb(239, 68, 68),
                    Location = new Point(20, 50),
                    Size = new Size(500, 40),
                    BackColor = Color.Transparent
                };
                _activityPanel.Controls.Add(errorLabel);
            }
        }

        private void CreateActivityItem(Panel parent, Models.Product product, int yPos)
        {
            var timeAgo = GetTimeAgo(product.UpdatedAt);
            var stockStatus = product.Quantity <= product.MinQuantity ? "LOW" : "OK";
            var statusColor = product.Quantity <= product.MinQuantity ? Color.FromArgb(239, 68, 68) : Color.FromArgb(34, 197, 94);

            var itemPanel = new Panel
            {
                Location = new Point(20, yPos),
                Size = new Size(520, 25),
                BackColor = Color.Transparent
            };

            var icon = new IconPictureBox
            {
                IconChar = product.Quantity <= product.MinQuantity ? IconChar.TriangleExclamation : IconChar.Check,
                IconColor = statusColor,
                IconSize = 14,
                Location = new Point(0, 5),
                Size = new Size(14, 14),
                BackColor = Color.Transparent
            };
            itemPanel.Controls.Add(icon);

            var nameLabel = new Label
            {
                Text = product.Name,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(20, 3),
                Size = new Size(200, 18),
                BackColor = Color.Transparent
            };
            itemPanel.Controls.Add(nameLabel);

            var qtyLabel = new Label
            {
                Text = $"Qty: {product.Quantity}",
                Font = new Font("Segoe UI", 9F),
                ForeColor = statusColor,
                Location = new Point(230, 3),
                Size = new Size(80, 18),
                BackColor = Color.Transparent
            };
            itemPanel.Controls.Add(qtyLabel);

            var categoryLabel = new Label
            {
                Text = product.Category?.Name ?? "N/A",
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.FromArgb(120, 120, 120),
                Location = new Point(320, 4),
                Size = new Size(100, 16),
                BackColor = Color.Transparent
            };
            itemPanel.Controls.Add(categoryLabel);

            var timeLabel = new Label
            {
                Text = timeAgo,
                Font = new Font("Segoe UI", 8F),
                ForeColor = Color.FromArgb(150, 150, 150),
                Location = new Point(430, 5),
                Size = new Size(80, 14),
                TextAlign = ContentAlignment.MiddleRight,
                BackColor = Color.Transparent
            };
            itemPanel.Controls.Add(timeLabel);

            parent.Controls.Add(itemPanel);
        }

        private async System.Threading.Tasks.Task LoadLowStockAlerts()
        {
            try
            {
                // Clear existing alert items (keep title and refresh icon)
                var itemsToRemove = _alertsPanel.Controls.Cast<Control>()
                    .Where(c => c.Tag?.ToString() != "title" && c.Tag?.ToString() != "refresh")
                    .ToList();
                
                foreach (var item in itemsToRemove)
                {
                    _alertsPanel.Controls.Remove(item);
                    item.Dispose();
                }

                // Get low stock products
                var lowStockProducts = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Supplier)
                    .Where(p => p.IsActive && p.Quantity <= p.MinQuantity)
                    .OrderBy(p => p.Quantity)
                    .Take(8)
                    .ToListAsync();

                int yPos = 50;
                
                if (lowStockProducts.Any())
                {
                    foreach (var product in lowStockProducts)
                    {
                        CreateAlertItem(_alertsPanel, product, yPos);
                        yPos += 30;
                    }
                }
                else
                {
                    var noDataLabel = new Label
                    {
                        Text = "All products are adequately stocked!",
                        Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                        ForeColor = Color.FromArgb(34, 197, 94),
                        Location = new Point(20, 50),
                        Size = new Size(500, 20),
                        BackColor = Color.Transparent
                    };
                    _alertsPanel.Controls.Add(noDataLabel);
                }
            }
            catch (Exception ex)
            {
                var errorLabel = new Label
                {
                    Text = $"Error loading alerts: {ex.Message}",
                    Font = new Font("Segoe UI", 9F),
                    ForeColor = Color.FromArgb(239, 68, 68),
                    Location = new Point(20, 50),
                    Size = new Size(500, 40),
                    BackColor = Color.Transparent
                };
                _alertsPanel.Controls.Add(errorLabel);
            }
        }

        private void CreateAlertItem(Panel parent, Models.Product product, int yPos)
        {
            var urgency = product.Quantity == 0 ? "CRITICAL" : "LOW";
            var urgencyColor = product.Quantity == 0 ? Color.FromArgb(220, 38, 38) : Color.FromArgb(239, 68, 68);
            var reorderQty = product.MinQuantity * 2 - product.Quantity;

            var itemPanel = new Panel
            {
                Location = new Point(20, yPos),
                Size = new Size(520, 25),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };

            itemPanel.MouseEnter += (s, e) => itemPanel.BackColor = Color.FromArgb(254, 242, 242);
            itemPanel.MouseLeave += (s, e) => itemPanel.BackColor = Color.Transparent;

            var icon = new IconPictureBox
            {
                IconChar = product.Quantity == 0 ? IconChar.CircleExclamation : IconChar.TriangleExclamation,
                IconColor = urgencyColor,
                IconSize = 14,
                Location = new Point(0, 5),
                Size = new Size(14, 14),
                BackColor = Color.Transparent
            };
            itemPanel.Controls.Add(icon);

            var nameLabel = new Label
            {
                Text = product.Name,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(20, 3),
                Size = new Size(180, 18),
                BackColor = Color.Transparent
            };
            itemPanel.Controls.Add(nameLabel);

            var qtyLabel = new Label
            {
                Text = $"{product.Quantity}/{product.MinQuantity}",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = urgencyColor,
                Location = new Point(210, 3),
                Size = new Size(60, 18),
                BackColor = Color.Transparent
            };
            itemPanel.Controls.Add(qtyLabel);

            var supplierLabel = new Label
            {
                Text = product.Supplier?.Name ?? "No Supplier",
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.FromArgb(120, 120, 120),
                Location = new Point(280, 4),
                Size = new Size(120, 16),
                BackColor = Color.Transparent
            };
            itemPanel.Controls.Add(supplierLabel);

            var actionLabel = new Label
            {
                Text = $"Order {reorderQty}",
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(59, 130, 246),
                Location = new Point(410, 5),
                Size = new Size(100, 14),
                TextAlign = ContentAlignment.MiddleRight,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            actionLabel.Click += (s, e) => 
            {
                MessageBox.Show($"Reorder {product.Name}\n\nSuggested Order Quantity: {reorderQty}\nSupplier: {product.Supplier?.Name ?? "Not Set"}\nCurrent Stock: {product.Quantity}\nMinimum Stock: {product.MinQuantity}", 
                    "Reorder Product", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            itemPanel.Controls.Add(actionLabel);

            parent.Controls.Add(itemPanel);
        }

        private string GetTimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.Now - dateTime;
            
            if (timeSpan.TotalMinutes < 1)
                return "Just now";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes}m ago";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours}h ago";
            if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays}d ago";
            
            return dateTime.ToString("MMM dd");
        }

        private void UpdateStatCard(string tag, string value)
        {
            foreach (Control control in this.Controls)
            {
                if (control is Panel contentPanel && contentPanel.Dock == DockStyle.Fill)
                {
                    foreach (Control child in contentPanel.Controls)
                    {
                        if (child is FlowLayoutPanel flow)
                        {
                            foreach (Control card in flow.Controls)
                            {
                                if (card.Tag?.ToString() == tag)
                                {
                                    foreach (Control label in card.Controls)
                                    {
                                        if (label.Tag?.ToString() == "value")
                                        {
                                            label.Text = value;
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ProductManagement_Click(object sender, EventArgs e)
        {
            var productView = new ProductManagementView(_authService);
            ShowView(productView);
        }

        private void ReceiveStock_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Receive Stock functionality - Coming Soon!\n\nThis will allow you to:\n• Process incoming deliveries\n• Update stock quantities\n• Record supplier information\n• Generate receiving reports", 
                "Receive Stock", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void StockAdjustment_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Stock Adjustment functionality - Coming Soon!\n\nThis will allow you to:\n• Adjust inventory levels\n• Record reasons for adjustments\n• Track waste and loss\n• Maintain audit trail", 
                "Stock Adjustment", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void StockTransfer_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Stock Transfer functionality - Coming Soon!\n\nThis will allow you to:\n• Transfer stock between locations\n• Track transfer status\n• Generate transfer documents\n• Update inventory records", 
                "Stock Transfer", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void GenerateReport_Click(object sender, EventArgs e)
        {
            var reportsView = new InventoryReportsView(_authService);
            ShowView(reportsView);
        }

        private void ManageSuppliers_Click(object sender, EventArgs e)
        {
            var supplierView = new SupplierManagementView(_authService);
            ShowView(supplierView);
        }

        private void ViewCategories_Click(object sender, EventArgs e)
        {
            var categoryView = new CategoryManagementView(_authService);
            ShowView(categoryView);
        }

        private void ScanBarcode_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Barcode Scanner functionality - Coming Soon!\n\nThis will allow you to:\n• Scan product barcodes\n• Quick product lookup\n• Update stock levels\n• Print barcode labels", 
                "Barcode Scanner", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowView(Form view)
        {
            var mainDashboard = this.ParentForm as MainDashboard;
            if (mainDashboard != null)
            {
                mainDashboard.LoadView(view);
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
