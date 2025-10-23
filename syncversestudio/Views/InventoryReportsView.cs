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
    public partial class InventoryReportsView : Form
    {
        private readonly AuthenticationService _authService;
        private readonly ApplicationDbContext _context;
        private Panel _reportContentPanel;
        private string _currentReport = "";

        public InventoryReportsView(AuthenticationService authService)
        {
            _authService = authService;
            _context = new ApplicationDbContext();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Name = "InventoryReportsView";
            this.Text = "Stock Reports";
            this.WindowState = FormWindowState.Normal;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.ClientSize = new Size(1200, 800);
            this.Padding = new Padding(0);

            CreateHeaderPanel();
            CreateMainLayout();

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
                Text = "Stock Reports & Analytics",
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
                Text = "Comprehensive inventory analysis and reporting tools",
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

        private void CreateMainLayout()
        {
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(245, 247, 250),
                Padding = new Padding(30, 20, 30, 20)
            };

            // Left sidebar with report categories
            var sidebarPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(280, 680),
                BackColor = Color.White
            };

            sidebarPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, sidebarPanel.Width - 1, sidebarPanel.Height - 1), 12))
                using (var brush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillPath(brush, path);
                }
            };

            var sidebarTitle = new Label
            {
                Text = "Report Categories",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(20, 15),
                Size = new Size(240, 25),
                BackColor = Color.Transparent
            };
            sidebarPanel.Controls.Add(sidebarTitle);

            int yPos = 55;
            CreateReportMenuItem(sidebarPanel, IconChar.BoxesStacked, "Stock Levels", "Current inventory status", Color.FromArgb(59, 130, 246), yPos, "stock_levels");
            yPos += 70;
            CreateReportMenuItem(sidebarPanel, IconChar.TriangleExclamation, "Low Stock Alert", "Items needing reorder", Color.FromArgb(239, 68, 68), yPos, "low_stock");
            yPos += 70;
            CreateReportMenuItem(sidebarPanel, IconChar.ArrowsRotate, "Stock Movement", "Inventory transactions", Color.FromArgb(34, 197, 94), yPos, "stock_movement");
            yPos += 70;
            CreateReportMenuItem(sidebarPanel, IconChar.ChartPie, "Category Analysis", "Performance by category", Color.FromArgb(168, 85, 247), yPos, "category");
            yPos += 70;
            CreateReportMenuItem(sidebarPanel, IconChar.TruckFast, "Supplier Report", "Supplier performance", Color.FromArgb(236, 72, 153), yPos, "supplier");
            yPos += 70;
            CreateReportMenuItem(sidebarPanel, IconChar.DollarSign, "Valuation Report", "Stock value analysis", Color.FromArgb(34, 197, 94), yPos, "valuation");
            yPos += 70;
            CreateReportMenuItem(sidebarPanel, IconChar.Clock, "Aging Report", "Stock age analysis", Color.FromArgb(249, 115, 22), yPos, "aging");
            yPos += 70;
            CreateReportMenuItem(sidebarPanel, IconChar.ChartBar, "ABC Analysis", "Pareto analysis", Color.FromArgb(14, 165, 233), yPos, "abc");

            mainPanel.Controls.Add(sidebarPanel);

            // Right content area
            _reportContentPanel = new Panel
            {
                Location = new Point(300, 0),
                Size = new Size(840, 680),
                BackColor = Color.White,
                AutoScroll = true
            };

            _reportContentPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, _reportContentPanel.Width - 1, _reportContentPanel.Height - 1), 12))
                using (var brush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillPath(brush, path);
                }
            };

            ShowWelcomeScreen();

            mainPanel.Controls.Add(_reportContentPanel);
            this.Controls.Add(mainPanel);
        }

        private void CreateReportMenuItem(Panel parent, IconChar icon, string title, string description, Color color, int yPos, string reportType)
        {
            var menuItem = new Panel
            {
                Location = new Point(10, yPos),
                Size = new Size(260, 60),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Tag = reportType
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
            menuItem.Controls.Add(iconBox);

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(45, 12),
                Size = new Size(205, 20),
                BackColor = Color.Transparent
            };
            menuItem.Controls.Add(titleLabel);

            var descLabel = new Label
            {
                Text = description,
                Font = new Font("Segoe UI", 8F),
                ForeColor = Color.FromArgb(120, 120, 120),
                Location = new Point(45, 32),
                Size = new Size(205, 16),
                BackColor = Color.Transparent
            };
            menuItem.Controls.Add(descLabel);

            menuItem.MouseEnter += (s, e) => menuItem.BackColor = Color.FromArgb(248, 250, 252);
            menuItem.MouseLeave += (s, e) => menuItem.BackColor = _currentReport == reportType ? Color.FromArgb(239, 246, 255) : Color.Transparent;
            menuItem.Click += (s, e) => LoadReport(reportType, menuItem);

            parent.Controls.Add(menuItem);
        }

        private void ShowWelcomeScreen()
        {
            _reportContentPanel.Controls.Clear();

            var welcomeIcon = new IconPictureBox
            {
                IconChar = IconChar.ChartLine,
                IconColor = Color.FromArgb(59, 130, 246),
                IconSize = 64,
                Location = new Point(388, 200),
                Size = new Size(64, 64),
                BackColor = Color.Transparent
            };
            _reportContentPanel.Controls.Add(welcomeIcon);

            var welcomeLabel = new Label
            {
                Text = "Select a Report",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(300, 280),
                Size = new Size(240, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            _reportContentPanel.Controls.Add(welcomeLabel);

            var instructionLabel = new Label
            {
                Text = "Choose a report category from the left menu to view detailed analytics",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(120, 120, 120),
                Location = new Point(250, 320),
                Size = new Size(340, 40),
                TextAlign = ContentAlignment.TopCenter,
                BackColor = Color.Transparent
            };
            _reportContentPanel.Controls.Add(instructionLabel);
        }

        private async void LoadReport(string reportType, Panel menuItem)
        {
            _currentReport = reportType;
            
            // Update menu item backgrounds
            foreach (Control ctrl in menuItem.Parent.Controls)
            {
                if (ctrl is Panel panel && ctrl != menuItem)
                {
                    panel.BackColor = Color.Transparent;
                }
            }
            menuItem.BackColor = Color.FromArgb(239, 246, 255);

            _reportContentPanel.Controls.Clear();

            // Show loading indicator
            var loadingLabel = new Label
            {
                Text = "Loading report...",
                Font = new Font("Segoe UI", 12F),
                ForeColor = Color.FromArgb(120, 120, 120),
                Location = new Point(350, 300),
                Size = new Size(200, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            _reportContentPanel.Controls.Add(loadingLabel);
            _reportContentPanel.Refresh();

            try
            {
                switch (reportType)
                {
                    case "stock_levels":
                        await LoadStockLevelsReport();
                        break;
                    case "low_stock":
                        await LoadLowStockReport();
                        break;
                    case "stock_movement":
                        await LoadStockMovementReport();
                        break;
                    case "category":
                        await LoadCategoryAnalysisReport();
                        break;
                    case "supplier":
                        await LoadSupplierReport();
                        break;
                    case "valuation":
                        await LoadValuationReport();
                        break;
                    case "aging":
                        await LoadAgingReport();
                        break;
                    case "abc":
                        await LoadABCAnalysisReport();
                        break;
                }
            }
            catch (Exception ex)
            {
                _reportContentPanel.Controls.Clear();
                var errorLabel = new Label
                {
                    Text = $"Error loading report: {ex.Message}",
                    Font = new Font("Segoe UI", 11F),
                    ForeColor = Color.FromArgb(239, 68, 68),
                    Location = new Point(50, 50),
                    Size = new Size(740, 100),
                    BackColor = Color.Transparent
                };
                _reportContentPanel.Controls.Add(errorLabel);
            }
        }

        private async System.Threading.Tasks.Task LoadStockLevelsReport()
        {
            _reportContentPanel.Controls.Clear();

            CreateReportHeader("Stock Levels Report", "Current inventory status across all products", IconChar.BoxesStacked, Color.FromArgb(59, 130, 246));

            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();

            // Summary cards
            var summaryPanel = new FlowLayoutPanel
            {
                Location = new Point(20, 100),
                Size = new Size(800, 80),
                BackColor = Color.Transparent,
                WrapContents = true
            };

            var totalProducts = products.Count;
            var totalQuantity = products.Sum(p => p.Quantity);
            var totalValue = products.Sum(p => p.Quantity * p.CostPrice);
            var lowStockCount = products.Count(p => p.Quantity <= p.MinQuantity);

            CreateSummaryCard(summaryPanel, "Total Products", totalProducts.ToString(), Color.FromArgb(59, 130, 246));
            CreateSummaryCard(summaryPanel, "Total Units", totalQuantity.ToString(), Color.FromArgb(34, 197, 94));
            CreateSummaryCard(summaryPanel, "Stock Value", $"${totalValue:N2}", Color.FromArgb(168, 85, 247));
            CreateSummaryCard(summaryPanel, "Low Stock", lowStockCount.ToString(), Color.FromArgb(239, 68, 68));

            _reportContentPanel.Controls.Add(summaryPanel);

            // Data grid
            var dataGrid = CreateDataGrid(20, 200, 800, 450);
            dataGrid.Columns.Add("Product", "Product Name");
            dataGrid.Columns.Add("Category", "Category");
            dataGrid.Columns.Add("Quantity", "Quantity");
            dataGrid.Columns.Add("MinQty", "Min Qty");
            dataGrid.Columns.Add("Status", "Status");
            dataGrid.Columns.Add("Value", "Value");

            foreach (var product in products)
            {
                var status = product.Quantity == 0 ? "Out of Stock" : 
                            product.Quantity <= product.MinQuantity ? "Low Stock" : "In Stock";
                var statusColor = product.Quantity == 0 ? Color.FromArgb(220, 38, 38) :
                                 product.Quantity <= product.MinQuantity ? Color.FromArgb(239, 68, 68) : Color.FromArgb(34, 197, 94);

                var row = dataGrid.Rows.Add(
                    product.Name,
                    product.Category?.Name ?? "N/A",
                    product.Quantity,
                    product.MinQuantity,
                    status,
                    $"${product.Quantity * product.CostPrice:N2}"
                );
                dataGrid.Rows[row].Cells["Status"].Style.ForeColor = statusColor;
                dataGrid.Rows[row].Cells["Status"].Style.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            }

            _reportContentPanel.Controls.Add(dataGrid);
        }

        private async System.Threading.Tasks.Task LoadLowStockReport()
        {
            _reportContentPanel.Controls.Clear();

            CreateReportHeader("Low Stock Alert Report", "Products requiring immediate attention", IconChar.TriangleExclamation, Color.FromArgb(239, 68, 68));

            var lowStockProducts = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Where(p => p.IsActive && p.Quantity <= p.MinQuantity)
                .OrderBy(p => p.Quantity)
                .ToListAsync();

            // Summary
            var summaryPanel = new FlowLayoutPanel
            {
                Location = new Point(20, 100),
                Size = new Size(800, 80),
                BackColor = Color.Transparent
            };

            var criticalCount = lowStockProducts.Count(p => p.Quantity == 0);
            var lowCount = lowStockProducts.Count(p => p.Quantity > 0 && p.Quantity <= p.MinQuantity);
            var reorderValue = lowStockProducts.Sum(p => (p.MinQuantity * 2 - p.Quantity) * p.CostPrice);

            CreateSummaryCard(summaryPanel, "Critical (Out)", criticalCount.ToString(), Color.FromArgb(220, 38, 38));
            CreateSummaryCard(summaryPanel, "Low Stock", lowCount.ToString(), Color.FromArgb(239, 68, 68));
            CreateSummaryCard(summaryPanel, "Total Items", lowStockProducts.Count.ToString(), Color.FromArgb(249, 115, 22));
            CreateSummaryCard(summaryPanel, "Reorder Value", $"${reorderValue:N2}", Color.FromArgb(59, 130, 246));

            _reportContentPanel.Controls.Add(summaryPanel);

            // Data grid
            var dataGrid = CreateDataGrid(20, 200, 800, 450);
            dataGrid.Columns.Add("Product", "Product Name");
            dataGrid.Columns.Add("Current", "Current");
            dataGrid.Columns.Add("Minimum", "Minimum");
            dataGrid.Columns.Add("Reorder", "Reorder Qty");
            dataGrid.Columns.Add("Supplier", "Supplier");
            dataGrid.Columns.Add("Cost", "Unit Cost");
            dataGrid.Columns.Add("Total", "Reorder Cost");

            foreach (var product in lowStockProducts)
            {
                var reorderQty = product.MinQuantity * 2 - product.Quantity;
                var reorderCost = reorderQty * product.CostPrice;

                var row = dataGrid.Rows.Add(
                    product.Name,
                    product.Quantity,
                    product.MinQuantity,
                    reorderQty,
                    product.Supplier?.Name ?? "Not Set",
                    $"${product.CostPrice:N2}",
                    $"${reorderCost:N2}"
                );

                if (product.Quantity == 0)
                {
                    dataGrid.Rows[row].DefaultCellStyle.BackColor = Color.FromArgb(254, 242, 242);
                    dataGrid.Rows[row].DefaultCellStyle.ForeColor = Color.FromArgb(220, 38, 38);
                }
            }

            _reportContentPanel.Controls.Add(dataGrid);
        }

        private async System.Threading.Tasks.Task LoadStockMovementReport()
        {
            _reportContentPanel.Controls.Clear();

            CreateReportHeader("Stock Movement Report", "Recent inventory transactions and updates", IconChar.ArrowsRotate, Color.FromArgb(34, 197, 94));

            var recentProducts = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive && p.UpdatedAt >= DateTime.Now.AddDays(-30))
                .OrderByDescending(p => p.UpdatedAt)
                .ToListAsync();

            // Summary
            var summaryPanel = new FlowLayoutPanel
            {
                Location = new Point(20, 100),
                Size = new Size(800, 80),
                BackColor = Color.Transparent
            };

            var todayCount = recentProducts.Count(p => p.UpdatedAt.Date == DateTime.Today);
            var weekCount = recentProducts.Count(p => p.UpdatedAt >= DateTime.Now.AddDays(-7));
            var monthCount = recentProducts.Count;

            CreateSummaryCard(summaryPanel, "Today", todayCount.ToString(), Color.FromArgb(34, 197, 94));
            CreateSummaryCard(summaryPanel, "This Week", weekCount.ToString(), Color.FromArgb(59, 130, 246));
            CreateSummaryCard(summaryPanel, "This Month", monthCount.ToString(), Color.FromArgb(168, 85, 247));
            CreateSummaryCard(summaryPanel, "Active Products", recentProducts.Count.ToString(), Color.FromArgb(249, 115, 22));

            _reportContentPanel.Controls.Add(summaryPanel);

            // Data grid
            var dataGrid = CreateDataGrid(20, 200, 800, 450);
            dataGrid.Columns.Add("Product", "Product Name");
            dataGrid.Columns.Add("Category", "Category");
            dataGrid.Columns.Add("Quantity", "Current Qty");
            dataGrid.Columns.Add("LastUpdated", "Last Updated");
            dataGrid.Columns.Add("Status", "Status");

            foreach (var product in recentProducts)
            {
                var timeAgo = GetTimeAgo(product.UpdatedAt);
                var status = product.Quantity <= product.MinQuantity ? "Low Stock" : "Normal";
                var statusColor = product.Quantity <= product.MinQuantity ? Color.FromArgb(239, 68, 68) : Color.FromArgb(34, 197, 94);

                var row = dataGrid.Rows.Add(
                    product.Name,
                    product.Category?.Name ?? "N/A",
                    product.Quantity,
                    timeAgo,
                    status
                );
                dataGrid.Rows[row].Cells["Status"].Style.ForeColor = statusColor;
            }

            _reportContentPanel.Controls.Add(dataGrid);
        }

        private async System.Threading.Tasks.Task LoadCategoryAnalysisReport()
        {
            _reportContentPanel.Controls.Clear();

            CreateReportHeader("Category Performance Analysis", "Inventory breakdown by product category", IconChar.ChartPie, Color.FromArgb(168, 85, 247));

            var categories = await _context.Categories
                .Include(c => c.Products)
                .Where(c => c.IsActive)
                .ToListAsync();

            var totalProducts = await _context.Products.CountAsync(p => p.IsActive);
            var totalValue = await _context.Products.Where(p => p.IsActive).SumAsync(p => p.Quantity * p.CostPrice);

            // Summary
            var summaryPanel = new FlowLayoutPanel
            {
                Location = new Point(20, 100),
                Size = new Size(800, 80),
                BackColor = Color.Transparent
            };

            CreateSummaryCard(summaryPanel, "Total Categories", categories.Count.ToString(), Color.FromArgb(168, 85, 247));
            CreateSummaryCard(summaryPanel, "Total Products", totalProducts.ToString(), Color.FromArgb(59, 130, 246));
            CreateSummaryCard(summaryPanel, "Total Value", $"${totalValue:N2}", Color.FromArgb(34, 197, 94));
            CreateSummaryCard(summaryPanel, "Avg per Category", $"{(totalProducts / Math.Max(categories.Count, 1)):N0}", Color.FromArgb(249, 115, 22));

            _reportContentPanel.Controls.Add(summaryPanel);

            // Data grid
            var dataGrid = CreateDataGrid(20, 200, 800, 450);
            dataGrid.Columns.Add("Category", "Category Name");
            dataGrid.Columns.Add("Products", "Products");
            dataGrid.Columns.Add("TotalQty", "Total Qty");
            dataGrid.Columns.Add("Value", "Stock Value");
            dataGrid.Columns.Add("Percentage", "% of Total");
            dataGrid.Columns.Add("LowStock", "Low Stock Items");

            foreach (var category in categories.OrderByDescending(c => c.Products.Count))
            {
                var productCount = category.Products.Count(p => p.IsActive);
                var totalQty = category.Products.Where(p => p.IsActive).Sum(p => p.Quantity);
                var categoryValue = category.Products.Where(p => p.IsActive).Sum(p => p.Quantity * p.CostPrice);
                var percentage = totalValue > 0 ? (categoryValue / totalValue * 100) : 0;
                var lowStockCount = category.Products.Count(p => p.IsActive && p.Quantity <= p.MinQuantity);

                dataGrid.Rows.Add(
                    category.Name,
                    productCount,
                    totalQty,
                    $"${categoryValue:N2}",
                    $"{percentage:N1}%",
                    lowStockCount
                );
            }

            _reportContentPanel.Controls.Add(dataGrid);
        }

        private async System.Threading.Tasks.Task LoadSupplierReport()
        {
            _reportContentPanel.Controls.Clear();

            CreateReportHeader("Supplier Performance Report", "Analysis of supplier relationships and inventory", IconChar.TruckFast, Color.FromArgb(236, 72, 153));

            var suppliers = await _context.Suppliers
                .Include(s => s.Products)
                .Where(s => s.IsActive)
                .ToListAsync();

            // Summary
            var summaryPanel = new FlowLayoutPanel
            {
                Location = new Point(20, 100),
                Size = new Size(800, 80),
                BackColor = Color.Transparent
            };

            var totalSuppliers = suppliers.Count;
            var activeSuppliers = suppliers.Count(s => s.Products.Any(p => p.IsActive));
            var totalProducts = suppliers.Sum(s => s.Products.Count(p => p.IsActive));

            CreateSummaryCard(summaryPanel, "Total Suppliers", totalSuppliers.ToString(), Color.FromArgb(236, 72, 153));
            CreateSummaryCard(summaryPanel, "Active Suppliers", activeSuppliers.ToString(), Color.FromArgb(34, 197, 94));
            CreateSummaryCard(summaryPanel, "Total Products", totalProducts.ToString(), Color.FromArgb(59, 130, 246));
            CreateSummaryCard(summaryPanel, "Avg per Supplier", $"{(totalProducts / Math.Max(totalSuppliers, 1)):N0}", Color.FromArgb(168, 85, 247));

            _reportContentPanel.Controls.Add(summaryPanel);

            // Data grid
            var dataGrid = CreateDataGrid(20, 200, 800, 450);
            dataGrid.Columns.Add("Supplier", "Supplier Name");
            dataGrid.Columns.Add("Contact", "Contact");
            dataGrid.Columns.Add("Products", "Products");
            dataGrid.Columns.Add("LowStock", "Low Stock");
            dataGrid.Columns.Add("Value", "Stock Value");

            foreach (var supplier in suppliers.OrderByDescending(s => s.Products.Count))
            {
                var productCount = supplier.Products.Count(p => p.IsActive);
                var lowStockCount = supplier.Products.Count(p => p.IsActive && p.Quantity <= p.MinQuantity);
                var supplierValue = supplier.Products.Where(p => p.IsActive).Sum(p => p.Quantity * p.CostPrice);

                dataGrid.Rows.Add(
                    supplier.Name,
                    supplier.ContactPerson ?? "N/A",
                    productCount,
                    lowStockCount,
                    $"${supplierValue:N2}"
                );
            }

            _reportContentPanel.Controls.Add(dataGrid);
        }

        private async System.Threading.Tasks.Task LoadValuationReport()
        {
            _reportContentPanel.Controls.Clear();

            CreateReportHeader("Stock Valuation Report", "Financial analysis of inventory value", IconChar.DollarSign, Color.FromArgb(34, 197, 94));

            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive)
                .ToListAsync();

            var totalCostValue = products.Sum(p => p.Quantity * p.CostPrice);
            var totalRetailValue = products.Sum(p => p.Quantity * p.SellingPrice);
            var potentialProfit = totalRetailValue - totalCostValue;
            var avgMargin = totalCostValue > 0 ? ((totalRetailValue - totalCostValue) / totalCostValue * 100) : 0;

            // Summary
            var summaryPanel = new FlowLayoutPanel
            {
                Location = new Point(20, 100),
                Size = new Size(800, 80),
                BackColor = Color.Transparent
            };

            CreateSummaryCard(summaryPanel, "Cost Value", $"${totalCostValue:N2}", Color.FromArgb(239, 68, 68));
            CreateSummaryCard(summaryPanel, "Retail Value", $"${totalRetailValue:N2}", Color.FromArgb(34, 197, 94));
            CreateSummaryCard(summaryPanel, "Potential Profit", $"${potentialProfit:N2}", Color.FromArgb(59, 130, 246));
            CreateSummaryCard(summaryPanel, "Avg Margin", $"{avgMargin:N1}%", Color.FromArgb(168, 85, 247));

            _reportContentPanel.Controls.Add(summaryPanel);

            // Data grid
            var dataGrid = CreateDataGrid(20, 200, 800, 450);
            dataGrid.Columns.Add("Product", "Product Name");
            dataGrid.Columns.Add("Quantity", "Quantity");
            dataGrid.Columns.Add("CostPrice", "Cost Price");
            dataGrid.Columns.Add("SellingPrice", "Selling Price");
            dataGrid.Columns.Add("CostValue", "Cost Value");
            dataGrid.Columns.Add("RetailValue", "Retail Value");
            dataGrid.Columns.Add("Margin", "Margin %");

            foreach (var product in products.OrderByDescending(p => p.Quantity * p.CostPrice))
            {
                var costValue = product.Quantity * product.CostPrice;
                var retailValue = product.Quantity * product.SellingPrice;
                var margin = product.CostPrice > 0 ? ((product.SellingPrice - product.CostPrice) / product.CostPrice * 100) : 0;

                dataGrid.Rows.Add(
                    product.Name,
                    product.Quantity,
                    $"${product.CostPrice:N2}",
                    $"${product.SellingPrice:N2}",
                    $"${costValue:N2}",
                    $"${retailValue:N2}",
                    $"{margin:N1}%"
                );
            }

            _reportContentPanel.Controls.Add(dataGrid);
        }

        private async System.Threading.Tasks.Task LoadAgingReport()
        {
            _reportContentPanel.Controls.Clear();

            CreateReportHeader("Stock Aging Report", "Analysis of inventory age and movement", IconChar.Clock, Color.FromArgb(249, 115, 22));

            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive)
                .OrderBy(p => p.UpdatedAt)
                .ToListAsync();

            var now = DateTime.Now;
            var veryOld = products.Count(p => (now - p.UpdatedAt).TotalDays > 180);
            var old = products.Count(p => (now - p.UpdatedAt).TotalDays > 90 && (now - p.UpdatedAt).TotalDays <= 180);
            var moderate = products.Count(p => (now - p.UpdatedAt).TotalDays > 30 && (now - p.UpdatedAt).TotalDays <= 90);
            var recent = products.Count(p => (now - p.UpdatedAt).TotalDays <= 30);

            // Summary
            var summaryPanel = new FlowLayoutPanel
            {
                Location = new Point(20, 100),
                Size = new Size(800, 80),
                BackColor = Color.Transparent
            };

            CreateSummaryCard(summaryPanel, "Recent (< 30d)", recent.ToString(), Color.FromArgb(34, 197, 94));
            CreateSummaryCard(summaryPanel, "Moderate (30-90d)", moderate.ToString(), Color.FromArgb(59, 130, 246));
            CreateSummaryCard(summaryPanel, "Old (90-180d)", old.ToString(), Color.FromArgb(249, 115, 22));
            CreateSummaryCard(summaryPanel, "Very Old (> 180d)", veryOld.ToString(), Color.FromArgb(239, 68, 68));

            _reportContentPanel.Controls.Add(summaryPanel);

            // Data grid
            var dataGrid = CreateDataGrid(20, 200, 800, 450);
            dataGrid.Columns.Add("Product", "Product Name");
            dataGrid.Columns.Add("Category", "Category");
            dataGrid.Columns.Add("Quantity", "Quantity");
            dataGrid.Columns.Add("LastUpdated", "Last Updated");
            dataGrid.Columns.Add("Age", "Age (Days)");
            dataGrid.Columns.Add("Status", "Status");

            foreach (var product in products)
            {
                var age = (now - product.UpdatedAt).TotalDays;
                var status = age > 180 ? "Very Old" : age > 90 ? "Old" : age > 30 ? "Moderate" : "Recent";
                var statusColor = age > 180 ? Color.FromArgb(239, 68, 68) : 
                                 age > 90 ? Color.FromArgb(249, 115, 22) : 
                                 age > 30 ? Color.FromArgb(59, 130, 246) : Color.FromArgb(34, 197, 94);

                var row = dataGrid.Rows.Add(
                    product.Name,
                    product.Category?.Name ?? "N/A",
                    product.Quantity,
                    product.UpdatedAt.ToString("MMM dd, yyyy"),
                    $"{(int)age}",
                    status
                );
                dataGrid.Rows[row].Cells["Status"].Style.ForeColor = statusColor;
                dataGrid.Rows[row].Cells["Status"].Style.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            }

            _reportContentPanel.Controls.Add(dataGrid);
        }

        private async System.Threading.Tasks.Task LoadABCAnalysisReport()
        {
            _reportContentPanel.Controls.Clear();

            CreateReportHeader("ABC Analysis (Pareto)", "Product classification by value contribution", IconChar.ChartBar, Color.FromArgb(14, 165, 233));

            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive)
                .ToListAsync();

            var productsWithValue = products
                .Select(p => new
                {
                    Product = p,
                    Value = p.Quantity * p.CostPrice
                })
                .OrderByDescending(p => p.Value)
                .ToList();

            var totalValue = productsWithValue.Sum(p => p.Value);
            var cumulativeValue = 0m;
            var classifiedProducts = productsWithValue.Select(p =>
            {
                cumulativeValue += p.Value;
                var cumulativePercentage = totalValue > 0 ? (cumulativeValue / totalValue * 100) : 0;
                var classification = cumulativePercentage <= 80 ? "A" : cumulativePercentage <= 95 ? "B" : "C";
                return new { p.Product, p.Value, CumulativePercentage = cumulativePercentage, Classification = classification };
            }).ToList();

            var classA = classifiedProducts.Count(p => p.Classification == "A");
            var classB = classifiedProducts.Count(p => p.Classification == "B");
            var classC = classifiedProducts.Count(p => p.Classification == "C");

            // Summary
            var summaryPanel = new FlowLayoutPanel
            {
                Location = new Point(20, 100),
                Size = new Size(800, 80),
                BackColor = Color.Transparent
            };

            CreateSummaryCard(summaryPanel, "Class A (High)", classA.ToString(), Color.FromArgb(34, 197, 94));
            CreateSummaryCard(summaryPanel, "Class B (Medium)", classB.ToString(), Color.FromArgb(59, 130, 246));
            CreateSummaryCard(summaryPanel, "Class C (Low)", classC.ToString(), Color.FromArgb(249, 115, 22));
            CreateSummaryCard(summaryPanel, "Total Value", $"${totalValue:N2}", Color.FromArgb(168, 85, 247));

            _reportContentPanel.Controls.Add(summaryPanel);

            // Data grid
            var dataGrid = CreateDataGrid(20, 200, 800, 450);
            dataGrid.Columns.Add("Product", "Product Name");
            dataGrid.Columns.Add("Category", "Category");
            dataGrid.Columns.Add("Quantity", "Quantity");
            dataGrid.Columns.Add("Value", "Stock Value");
            dataGrid.Columns.Add("Cumulative", "Cumulative %");
            dataGrid.Columns.Add("Class", "Class");

            foreach (var item in classifiedProducts)
            {
                var classColor = item.Classification == "A" ? Color.FromArgb(34, 197, 94) :
                                item.Classification == "B" ? Color.FromArgb(59, 130, 246) : Color.FromArgb(249, 115, 22);

                var row = dataGrid.Rows.Add(
                    item.Product.Name,
                    item.Product.Category?.Name ?? "N/A",
                    item.Product.Quantity,
                    $"${item.Value:N2}",
                    $"{item.CumulativePercentage:N1}%",
                    item.Classification
                );
                dataGrid.Rows[row].Cells["Class"].Style.ForeColor = classColor;
                dataGrid.Rows[row].Cells["Class"].Style.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            }

            _reportContentPanel.Controls.Add(dataGrid);
        }

        // Helper methods
        private void CreateReportHeader(string title, string subtitle, IconChar icon, Color color)
        {
            var headerPanel = new Panel
            {
                Location = new Point(20, 20),
                Size = new Size(800, 60),
                BackColor = Color.Transparent
            };

            var iconBox = new IconPictureBox
            {
                IconChar = icon,
                IconColor = color,
                IconSize = 32,
                Location = new Point(0, 10),
                Size = new Size(32, 32),
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(iconBox);

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(45, 8),
                Size = new Size(600, 28),
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(titleLabel);

            var subtitleLabel = new Label
            {
                Text = subtitle,
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(120, 120, 120),
                Location = new Point(45, 36),
                Size = new Size(600, 18),
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(subtitleLabel);

            var exportBtn = new Button
            {
                Text = "Export",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.White,
                BackColor = color,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(680, 15),
                Size = new Size(100, 32),
                Cursor = Cursors.Hand
            };
            exportBtn.FlatAppearance.BorderSize = 0;
            exportBtn.Click += (s, e) => MessageBox.Show("Export functionality coming soon!", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
            headerPanel.Controls.Add(exportBtn);

            _reportContentPanel.Controls.Add(headerPanel);
        }

        private void CreateSummaryCard(FlowLayoutPanel parent, string title, string value, Color color)
        {
            var card = new Panel
            {
                Size = new Size(190, 70),
                Margin = new Padding(0, 0, 10, 0),
                BackColor = Color.FromArgb(248, 250, 252)
            };

            card.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, card.Width - 1, card.Height - 1), 8))
                using (var brush = new SolidBrush(Color.FromArgb(248, 250, 252)))
                {
                    e.Graphics.FillPath(brush, path);
                }
                using (var pen = new Pen(color, 3))
                {
                    e.Graphics.DrawLine(pen, 0, 0, 0, card.Height);
                }
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.FromArgb(110, 110, 110),
                Location = new Point(15, 12),
                Size = new Size(160, 16),
                BackColor = Color.Transparent
            };
            card.Controls.Add(titleLabel);

            var valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = color,
                Location = new Point(15, 32),
                Size = new Size(160, 28),
                BackColor = Color.Transparent
            };
            card.Controls.Add(valueLabel);

            parent.Controls.Add(card);
        }

        private DataGridView CreateDataGrid(int x, int y, int width, int height)
        {
            var dataGrid = new DataGridView
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 40,
                EnableHeadersVisualStyles = false,
                GridColor = Color.FromArgb(230, 230, 230),
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            dataGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            dataGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(60, 60, 60);
            dataGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dataGrid.ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 0, 0, 0);
            dataGrid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(248, 250, 252);

            dataGrid.DefaultCellStyle.BackColor = Color.White;
            dataGrid.DefaultCellStyle.ForeColor = Color.FromArgb(60, 60, 60);
            dataGrid.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            dataGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(239, 246, 255);
            dataGrid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 30, 30);
            dataGrid.DefaultCellStyle.Padding = new Padding(10, 5, 10, 5);

            dataGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);

            dataGrid.RowTemplate.Height = 35;

            return dataGrid;
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

            return dateTime.ToString("MMM dd, yyyy");
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
