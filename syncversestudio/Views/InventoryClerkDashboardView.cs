using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using SyncVerseStudio.Data;
using SyncVerseStudio.Helpers;
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

            // User info and logout section removed - cleaner header design

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

            // Increase the actions panel height to accommodate more buttons
            actionsPanel.Size = new Size(1140, 260);

            CreateActionButton(actionsPanel, IconChar.Plus, "Add New Product", "Create new product entry", Color.FromArgb(59, 130, 246), 20, 20, ProductManagement_Click);
            CreateActionButton(actionsPanel, IconChar.ArrowUp, "Receive Stock", "Process incoming stock", Color.FromArgb(34, 197, 94), 20, 100, ReceiveStock_Click);
            CreateActionButton(actionsPanel, IconChar.PenToSquare, "Stock Adjustment", "Adjust inventory levels", Color.FromArgb(249, 115, 22), 300, 20, StockAdjustment_Click);
            CreateActionButton(actionsPanel, IconChar.ArrowsLeftRight, "Stock Transfer", "Transfer between locations", Color.FromArgb(168, 85, 247), 300, 100, StockTransfer_Click);
            CreateActionButton(actionsPanel, IconChar.FileLines, "Generate Report", "Create inventory reports", Color.FromArgb(14, 165, 233), 580, 20, GenerateReport_Click);
            CreateActionButton(actionsPanel, IconChar.TruckFast, "Manage Suppliers", "View supplier information", Color.FromArgb(236, 72, 153), 580, 100, ManageSuppliers_Click);
            CreateActionButton(actionsPanel, IconChar.BoxOpen, "View Categories", "Manage product categories", Color.FromArgb(139, 92, 246), 860, 20, ViewCategories_Click);
            CreateActionButton(actionsPanel, IconChar.Barcode, "Scan Barcode", "Quick product lookup", Color.FromArgb(6, 182, 212), 860, 100, ScanBarcode_Click);
            
            // Product Image Management Buttons
            CreateActionButton(actionsPanel, IconChar.Images, "Add Product Images", "Add images to existing products", Color.FromArgb(155, 89, 182), 20, 180, AddProductImages_Click);
            CreateActionButton(actionsPanel, IconChar.Image, "Add Specific Image", "Add image to specific product", Color.FromArgb(142, 68, 173), 300, 180, AddSpecificImage_Click);
            
            // View Product Images Button
            CreateActionButton(actionsPanel, IconChar.Eye, "View Product Images", "View and manage product images", Color.FromArgb(52, 152, 219), 580, 180, ViewProductImages_Click);
            
            // Logout Button
            CreateActionButton(actionsPanel, IconChar.SignOutAlt, "Logout Options", "Switch user or exit application", Color.FromArgb(231, 76, 60), 860, 180, LogoutButton_Click);

            contentPanel.Controls.Add(actionsPanel);

            // Recent Activity & Alerts Section
            sectionY += 325;
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

            // Product ID Label (new)
            var idLabel = new Label
            {
                Text = $"ID:{product.Id}",
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(59, 130, 246),
                Location = new Point(20, 3),
                Size = new Size(40, 18),
                BackColor = Color.Transparent
            };
            itemPanel.Controls.Add(idLabel);

            var nameLabel = new Label
            {
                Text = product.Name,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(65, 3),
                Size = new Size(150, 18),
                BackColor = Color.Transparent
            };
            itemPanel.Controls.Add(nameLabel);

            var qtyLabel = new Label
            {
                Text = $"Qty: {product.Quantity}",
                Font = new Font("Segoe UI", 9F),
                ForeColor = statusColor,
                Location = new Point(220, 3),
                Size = new Size(60, 18),
                BackColor = Color.Transparent
            };
            itemPanel.Controls.Add(qtyLabel);

            var categoryLabel = new Label
            {
                Text = product.Category?.Name ?? "N/A",
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.FromArgb(120, 120, 120),
                Location = new Point(285, 4),
                Size = new Size(80, 16),
                BackColor = Color.Transparent
            };
            itemPanel.Controls.Add(categoryLabel);

            var timeLabel = new Label
            {
                Text = timeAgo,
                Font = new Font("Segoe UI", 8F),
                ForeColor = Color.FromArgb(150, 150, 150),
                Location = new Point(370, 5),
                Size = new Size(80, 14),
                TextAlign = ContentAlignment.MiddleRight,
                BackColor = Color.Transparent
            };
            itemPanel.Controls.Add(timeLabel);

            // Add click handler to copy product ID
            itemPanel.Cursor = Cursors.Hand;
            itemPanel.Click += (s, e) => 
            {
                Clipboard.SetText(product.Id.ToString());
                MessageBox.Show($"Product ID {product.Id} copied to clipboard!\n\nProduct: {product.Name}\nYou can now use this ID in the 'Add Specific Image' function.", 
                    "Product ID Copied", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

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

            // Product ID Label (new)
            var idLabel = new Label
            {
                Text = $"ID:{product.Id}",
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(59, 130, 246),
                Location = new Point(20, 3),
                Size = new Size(40, 18),
                BackColor = Color.Transparent
            };
            itemPanel.Controls.Add(idLabel);

            var nameLabel = new Label
            {
                Text = product.Name,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(65, 3),
                Size = new Size(130, 18),
                BackColor = Color.Transparent
            };
            itemPanel.Controls.Add(nameLabel);

            var qtyLabel = new Label
            {
                Text = $"{product.Quantity}/{product.MinQuantity}",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = urgencyColor,
                Location = new Point(200, 3),
                Size = new Size(50, 18),
                BackColor = Color.Transparent
            };
            itemPanel.Controls.Add(qtyLabel);

            var supplierLabel = new Label
            {
                Text = product.Supplier?.Name ?? "No Supplier",
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.FromArgb(120, 120, 120),
                Location = new Point(255, 4),
                Size = new Size(100, 16),
                BackColor = Color.Transparent
            };
            itemPanel.Controls.Add(supplierLabel);

            var actionLabel = new Label
            {
                Text = $"Order {reorderQty}",
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(59, 130, 246),
                Location = new Point(360, 5),
                Size = new Size(80, 14),
                TextAlign = ContentAlignment.MiddleRight,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            actionLabel.Click += (s, e) => 
            {
                MessageBox.Show($"Reorder {product.Name}\n\nProduct ID: {product.Id}\nSuggested Order Quantity: {reorderQty}\nSupplier: {product.Supplier?.Name ?? "Not Set"}\nCurrent Stock: {product.Quantity}\nMinimum Stock: {product.MinQuantity}", 
                    "Reorder Product", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            itemPanel.Controls.Add(actionLabel);

            // Add click handler to copy product ID
            itemPanel.Click += (s, e) => 
            {
                Clipboard.SetText(product.Id.ToString());
                MessageBox.Show($"Product ID {product.Id} copied to clipboard!\n\nProduct: {product.Name}\nStock: {product.Quantity}/{product.MinQuantity}\n\nYou can now use this ID in the 'Add Specific Image' function.", 
                    "Product ID Copied", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

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
            try
            {
                var productView = new ProductManagementView(_authService);
                ShowView(productView);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Product Management functionality - Coming Soon!\n\nThis will allow you to:\n• Add new products\n• Edit existing products\n• Manage product categories\n• Set pricing and stock levels\n• Upload product images", 
                    "Product Management", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
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
            try
            {
                var reportsView = new InventoryReportsView(_authService);
                ShowView(reportsView);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Inventory Reports functionality - Coming Soon!\n\nThis will allow you to:\n• Generate stock level reports\n• View sales analytics\n• Export inventory data\n• Create custom reports\n• Schedule automated reports", 
                    "Inventory Reports", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ManageSuppliers_Click(object sender, EventArgs e)
        {
            try
            {
                var supplierView = new SupplierManagementView(_authService);
                ShowView(supplierView);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Supplier Management functionality - Coming Soon!\n\nThis will allow you to:\n• Add new suppliers\n• Edit supplier information\n• Manage supplier contacts\n• Track supplier performance\n• View purchase history", 
                    "Supplier Management", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ViewCategories_Click(object sender, EventArgs e)
        {
            try
            {
                var categoryView = new CategoryManagementView(_authService);
                ShowView(categoryView);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Category Management functionality - Coming Soon!\n\nThis will allow you to:\n• Create product categories\n• Edit category details\n• Organize product hierarchy\n• Set category-specific settings\n• Manage category images", 
                    "Category Management", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ScanBarcode_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Barcode Scanner functionality - Coming Soon!\n\nThis will allow you to:\n• Scan product barcodes\n• Quick product lookup\n• Update stock levels\n• Print barcode labels", 
                "Barcode Scanner", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void AddProductImages_Click(object sender, EventArgs e)
        {
            try
            {
                // First, check how many products need images
                int productsWithoutImages;
                using (var context = new ApplicationDbContext())
                {
                    productsWithoutImages = await context.Products
                        .Where(p => !p.ProductImages.Any() && p.IsActive)
                        .CountAsync();
                }

                if (productsWithoutImages == 0)
                {
                    MessageBox.Show("All active products already have images assigned!", "No Action Needed", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var result = MessageBox.Show(
                    $"Found {productsWithoutImages} products without images.\n\n" +
                    "This will add images to all existing products that don't have images yet.\n\n" +
                    "The system will:\n" +
                    "• Find products without ProductImage entries\n" +
                    "• Generate image names based on product names\n" +
                    "• Create ProductImage records for each product\n" +
                    "• Use existing ImagePath if available\n\n" +
                    "This is safe and won't overwrite existing images.\n\n" +
                    "Continue?",
                    "Add Product Images",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        using (var context = new ApplicationDbContext())
                        {
                            await DatabaseSeeder.AddProductImagesToExistingProductsAsync(context);
                            
                            // Check how many images were actually added
                            var imagesAdded = await context.ProductImages
                                .Where(pi => pi.CreatedAt >= DateTime.Now.AddMinutes(-1))
                                .CountAsync();
                            
                            MessageBox.Show($"Product images added successfully!\n\n" +
                                $"• {imagesAdded} product images created\n" +
                                $"• {productsWithoutImages} products now have images\n\n" +
                                "You can now view these images in the product management section.", 
                                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            
                            // Refresh dashboard data to reflect changes
                            await LoadDashboardData();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error adding product images:\n\n{ex.Message}\n\nPlease check:\n" +
                            "• Database connection is working\n" +
                            "• Products exist in the database\n" +
                            "• You have write permissions", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking products: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void AddSpecificImage_Click(object sender, EventArgs e)
        {
            var productDialog = new ProductImageInputDialog();
            if (productDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (var context = new ApplicationDbContext())
                    {
                        // Get product details for confirmation
                        var product = await context.Products.FindAsync(productDialog.ProductId);
                        if (product == null)
                        {
                            MessageBox.Show($"Product with ID {productDialog.ProductId} not found!", "Product Not Found", 
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // Check if image already exists
                        var existingImage = await context.ProductImages
                            .FirstOrDefaultAsync(pi => pi.ProductId == productDialog.ProductId && pi.ImagePath == productDialog.ImagePath);

                        if (existingImage != null)
                        {
                            MessageBox.Show($"This image is already assigned to product '{product.Name}'!", "Image Already Exists", 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }

                        await DatabaseSeeder.AddSpecificImageToProductAsync(context, 
                            productDialog.ProductId, productDialog.ImagePath, productDialog.IsPrimary);
                        
                        // Get the count of images for this product
                        var imageCount = await context.ProductImages
                            .CountAsync(pi => pi.ProductId == productDialog.ProductId && pi.IsActive);
                        
                        MessageBox.Show($"Image added successfully!\n\n" +
                            $"Product: {product.Name}\n" +
                            $"Image Path: {productDialog.ImagePath}\n" +
                            $"Primary Image: {(productDialog.IsPrimary ? "Yes" : "No")}\n" +
                            $"Total Images for this product: {imageCount}\n\n" +
                            "You can view and manage product images in the Product Management section.", 
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        // Refresh dashboard data to reflect changes
                        await LoadDashboardData();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding image:\n\n{ex.Message}\n\nPlease check:\n" +
                        "• Product ID is valid\n" +
                        "• Image path is correct\n" +
                        "• Database connection is working", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void ViewProductImages_Click(object sender, EventArgs e)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var productImages = await context.ProductImages
                        .Include(pi => pi.Product)
                        .Where(pi => pi.IsActive)
                        .OrderBy(pi => pi.Product.Name)
                        .ThenBy(pi => pi.DisplayOrder)
                        .ToListAsync();

                    if (!productImages.Any())
                    {
                        MessageBox.Show("No product images found in the database.\n\n" +
                            "Use the 'Add Product Images' or 'Add Specific Image' buttons to add images to your products.", 
                            "No Images Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    var imagesByProduct = productImages.GroupBy(pi => pi.Product.Name).ToList();
                    var summary = string.Join("\n", imagesByProduct.Select(g => 
                        $"• {g.Key}: {g.Count()} image(s) - Primary: {(g.Any(i => i.IsPrimary) ? "Yes" : "No")}"));

                    MessageBox.Show($"Product Images Summary:\n\n" +
                        $"Total Products with Images: {imagesByProduct.Count}\n" +
                        $"Total Images: {productImages.Count}\n\n" +
                        "Products and their images:\n" +
                        summary + "\n\n" +
                        "Use the Product Management section to view and edit individual images.", 
                        "Product Images Overview", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading product images: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BackupDatabase_Click(object sender, EventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "This will create a backup of the current database.\n\n" +
                    "The backup will include:\n" +
                    "• Complete database file copy\n" +
                    "• JSON export of all data\n" +
                    "• Timestamped files for easy identification\n\n" +
                    "Continue with backup creation?",
                    "Create Database Backup",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    string backupPath = await CreateDatabaseBackup();
                    
                    var openResult = MessageBox.Show(
                        $"Database backup created successfully!\n\n" +
                        $"Backup saved to:\n{backupPath}\n\n" +
                        "Would you like to open the backup folder?",
                        "Backup Complete",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information);

                    if (openResult == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{backupPath}\"");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating backup: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async System.Threading.Tasks.Task<string> CreateDatabaseBackup()
        {
            return await System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    // Create backups directory if it doesn't exist
                    string backupDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");
                    Directory.CreateDirectory(backupDir);

                    // Generate backup filename with timestamp
                    string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    string backupFileName = $"SyncVerseStudio_Backup_{timestamp}.db";
                    string backupPath = Path.Combine(backupDir, backupFileName);

                    // Try multiple possible database paths
                    string[] possibleDbPaths = {
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "syncversestudio.db"),
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SyncVerseStudio.db"),
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "syncversestudio.db"),
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SyncVerseStudio", "syncversestudio.db")
                    };

                    string currentDbPath = null;
                    foreach (var path in possibleDbPaths)
                    {
                        if (File.Exists(path))
                        {
                            currentDbPath = path;
                            break;
                        }
                    }

                    if (currentDbPath == null)
                    {
                        // If no database file found, create a JSON export only
                        string jsonBackupPath = Path.Combine(backupDir, $"SyncVerseStudio_Export_{timestamp}.json");
                        ExportDatabaseToJson(jsonBackupPath);
                        return jsonBackupPath;
                    }

                    // Copy the database file
                    File.Copy(currentDbPath, backupPath, true);
                    
                    // Also create a JSON export for additional safety
                    string jsonBackupPath2 = Path.Combine(backupDir, $"SyncVerseStudio_Export_{timestamp}.json");
                    ExportDatabaseToJson(jsonBackupPath2);

                    return backupPath;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to create backup: {ex.Message}");
                }
            });
        }

        private void ExportDatabaseToJson(string jsonPath)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var exportData = new
                    {
                        ExportDate = DateTime.Now,
                        DatabaseVersion = "1.0",
                        Tables = new
                        {
                            Categories = context.Categories.ToList(),
                            Suppliers = context.Suppliers.ToList(),
                            Products = context.Products.Include(p => p.Category).Include(p => p.Supplier).ToList(),
                            ProductImages = context.ProductImages.ToList(),
                            Customers = context.Customers.ToList(),
                            Users = context.Users.Select(u => new { u.Id, u.Username, u.Role, u.CreatedAt }).ToList(),
                            Sales = context.Sales.Include(s => s.Customer).ToList(),
                            SaleItems = context.SaleItems.Include(si => si.Product).ToList()
                        }
                    };

                    string json = System.Text.Json.JsonSerializer.Serialize(exportData, new System.Text.Json.JsonSerializerOptions 
                    { 
                        WriteIndented = true,
                        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
                    });
                    
                    File.WriteAllText(jsonPath, json);
                }
            }
            catch (Exception ex)
            {
                // Silently fail for JSON export as it's supplementary
            }
        }

        private void LogoutButton_Click(object sender, EventArgs e)
        {
            ShowLogoutDialog();
        }

        private void ShowLogoutDialog()
        {
            var logoutDialog = new LogoutOptionsDialog();
            var result = logoutDialog.ShowDialog();
            
            if (result == DialogResult.OK)
            {
                switch (logoutDialog.SelectedAction)
                {
                    case LogoutAction.Logout:
                        ReturnToLogin();
                        break;
                    case LogoutAction.ExitApplication:
                        ExitApplication();
                        break;
                }
            }
        }

        private void ReturnToLogin()
        {
            try
            {
                // Clear current user session
                _authService.Logout();
                
                // Find the main login form
                var loginForm = Application.OpenForms.OfType<LoginForm>().FirstOrDefault();
                if (loginForm != null)
                {
                    loginForm.Show();
                    loginForm.BringToFront();
                }
                else
                {
                    // If login form not found, create new one
                    loginForm = new LoginForm();
                    loginForm.Show();
                }
                
                // Close the main dashboard
                var mainDashboard = this.ParentForm as MainDashboard;
                if (mainDashboard != null)
                {
                    mainDashboard.Hide();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during logout: {ex.Message}", "Logout Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                // Fallback - create new login form
                try
                {
                    var loginForm = new LoginForm();
                    loginForm.Show();
                    
                    var mainDashboard = this.ParentForm as MainDashboard;
                    if (mainDashboard != null)
                    {
                        mainDashboard.Close();
                    }
                }
                catch
                {
                    Application.Exit();
                }
            }
        }

        private void ExitApplication()
        {
            try
            {
                var result = MessageBox.Show(
                    "Are you sure you want to exit the application?\n\nAll unsaved changes will be lost.",
                    "Exit Application",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Clean up resources
                    _refreshTimer?.Stop();
                    _refreshTimer?.Dispose();
                    _context?.Dispose();
                    
                    // Exit the application
                    Application.Exit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during application exit: {ex.Message}", "Exit Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit(); // Force exit if there's an error
            }
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

    public class ProductImageInputDialog : Form
    {
        public int ProductId { get; private set; }
        public string ImagePath { get; private set; } = string.Empty;
        public bool IsPrimary { get; private set; }

        private TextBox txtProductId;
        private TextBox txtImagePath;
        private CheckBox chkIsPrimary;
        private Button btnOK;
        private Button btnCancel;
        private Button btnBrowse;

        public ProductImageInputDialog()
        {
            InitializeDialog();
        }

        private void InitializeDialog()
        {
            this.Text = "Add Specific Product Image";
            this.Size = new Size(500, 300);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            // Instructions
            Label lblInstructions = new Label
            {
                Text = "💡 Tip: Click on any product in the dashboard to copy its ID to clipboard",
                Location = new Point(20, 10),
                Size = new Size(450, 20),
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                ForeColor = Color.FromArgb(120, 120, 120)
            };

            // Product ID
            Label lblProductId = new Label
            {
                Text = "Product ID:",
                Location = new Point(20, 40),
                Size = new Size(80, 23),
                Font = new Font("Segoe UI", 10F)
            };

            txtProductId = new TextBox
            {
                Location = new Point(110, 40),
                Size = new Size(100, 23),
                Font = new Font("Segoe UI", 10F),
                PlaceholderText = "Enter Product ID"
            };

            // Product lookup button
            Button btnLookup = new Button
            {
                Text = "Lookup",
                Location = new Point(220, 40),
                Size = new Size(70, 25),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F)
            };
            btnLookup.Click += BtnLookup_Click;

            // Product info label
            Label lblProductInfo = new Label
            {
                Text = "Enter Product ID and click Lookup to verify",
                Location = new Point(110, 70),
                Size = new Size(350, 20),
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(120, 120, 120)
            };

            // Image Path
            Label lblImagePath = new Label
            {
                Text = "Image Path:",
                Location = new Point(20, 100),
                Size = new Size(80, 23),
                Font = new Font("Segoe UI", 10F)
            };

            txtImagePath = new TextBox
            {
                Location = new Point(110, 100),
                Size = new Size(280, 23),
                Font = new Font("Segoe UI", 10F),
                PlaceholderText = "e.g., assets/product/image.jpg"
            };

            btnBrowse = new Button
            {
                Text = "Browse",
                Location = new Point(400, 100),
                Size = new Size(70, 25),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F)
            };
            btnBrowse.Click += BtnBrowse_Click;

            // Is Primary
            chkIsPrimary = new CheckBox
            {
                Text = "Set as Primary Image",
                Location = new Point(110, 140),
                Size = new Size(200, 23),
                Font = new Font("Segoe UI", 10F)
            };

            // Buttons
            btnOK = new Button
            {
                Text = "Add Image",
                Location = new Point(280, 190),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F),
                DialogResult = DialogResult.OK
            };
            btnOK.Click += BtnOK_Click;

            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(380, 190),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F),
                DialogResult = DialogResult.Cancel
            };

            this.Controls.AddRange(new Control[] {
                lblInstructions,
                lblProductId, txtProductId, btnLookup,
                lblProductInfo,
                lblImagePath, txtImagePath, btnBrowse,
                chkIsPrimary,
                btnOK, btnCancel
            });

            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;

            // Store reference to product info label for updates
            this.Tag = lblProductInfo;
        }

        private void BtnLookup_Click(object sender, EventArgs e)
        {
            var lblProductInfo = this.Tag as Label;
            if (lblProductInfo == null) return;

            if (string.IsNullOrWhiteSpace(txtProductId.Text))
            {
                lblProductInfo.Text = "Please enter a Product ID";
                lblProductInfo.ForeColor = Color.FromArgb(239, 68, 68);
                return;
            }

            if (!int.TryParse(txtProductId.Text, out int productId))
            {
                lblProductInfo.Text = "Product ID must be a valid number";
                lblProductInfo.ForeColor = Color.FromArgb(239, 68, 68);
                return;
            }

            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var product = context.Products
                        .Include(p => p.Category)
                        .Include(p => p.Supplier)
                        .FirstOrDefault(p => p.Id == productId);

                    if (product == null)
                    {
                        lblProductInfo.Text = $"Product with ID {productId} not found";
                        lblProductInfo.ForeColor = Color.FromArgb(239, 68, 68);
                    }
                    else
                    {
                        lblProductInfo.Text = $"✓ {product.Name} - {product.Category?.Name ?? "No Category"} - Stock: {product.Quantity}";
                        lblProductInfo.ForeColor = Color.FromArgb(34, 197, 94);
                    }
                }
            }
            catch (Exception ex)
            {
                lblProductInfo.Text = $"Error looking up product: {ex.Message}";
                lblProductInfo.ForeColor = Color.FromArgb(239, 68, 68);
            }
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png, *.bmp)|*.jpg;*.jpeg;*.png;*.bmp|All files (*.*)|*.*";
                openFileDialog.Title = "Select Product Image";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Convert to relative path if it's in the application directory
                    string selectedPath = openFileDialog.FileName;
                    string appPath = AppDomain.CurrentDomain.BaseDirectory;
                    
                    if (selectedPath.StartsWith(appPath))
                    {
                        txtImagePath.Text = Path.GetRelativePath(appPath, selectedPath).Replace("\\", "/");
                    }
                    else
                    {
                        txtImagePath.Text = selectedPath;
                    }
                }
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtProductId.Text))
            {
                MessageBox.Show("Please enter a Product ID.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtProductId.Text, out int productId))
            {
                MessageBox.Show("Product ID must be a valid number.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtImagePath.Text))
            {
                MessageBox.Show("Please enter an image path.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ProductId = productId;
            ImagePath = txtImagePath.Text.Trim();
            IsPrimary = chkIsPrimary.Checked;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }

    public enum LogoutAction
    {
        Logout,
        ExitApplication
    }

    public class LogoutOptionsDialog : Form
    {
        public LogoutAction SelectedAction { get; private set; }

        public LogoutOptionsDialog()
        {
            InitializeDialog();
        }

        private void InitializeDialog()
        {
            this.Text = "Logout Options";
            this.Size = new Size(400, 250);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            // Title
            var titleLabel = new Label
            {
                Text = "What would you like to do?",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(20, 20),
                Size = new Size(350, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            // Logout button
            var btnLogout = new Button
            {
                Text = "Logout (Return to Login)",
                Location = new Point(50, 70),
                Size = new Size(300, 40),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnLogout.Click += (s, e) => {
                SelectedAction = LogoutAction.Logout;
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            // Exit button
            var btnExit = new Button
            {
                Text = "Exit Application",
                Location = new Point(50, 120),
                Size = new Size(300, 40),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnExit.Click += (s, e) => {
                SelectedAction = LogoutAction.ExitApplication;
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            // Cancel button
            var btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(150, 170),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F),
                DialogResult = DialogResult.Cancel,
                Cursor = Cursors.Hand
            };

            this.Controls.AddRange(new Control[] {
                titleLabel,
                btnLogout,
                btnExit,
                btnCancel
            });

            this.AcceptButton = btnLogout;
            this.CancelButton = btnCancel;
        }
    }
}
