using System.Drawing;
using SyncVerseStudio.Services;
using SyncVerseStudio.Data;
using SyncVerseStudio.Models;
using Microsoft.EntityFrameworkCore;
using FontAwesome.Sharp;

namespace SyncVerseStudio.Views
{
    public partial class CategoryManagementView : Form
    {
        private readonly AuthenticationService _authService;
        private readonly ApplicationDbContext _context;
        private DataGridView categoriesGrid;
        private Panel topPanel, metricsPanel;
        private IconButton addButton, editButton, deleteButton, refreshButton, exportButton;
        private TextBox searchBox;
        private ComboBox statusFilter;
        private Label totalCategoriesLabel, activeCategoriesLabel, totalProductsLabel, avgProductsLabel;
        private IconPictureBox titleIcon, searchIcon, filterIcon;
        private Label titleLabel;

        // Skyward Serenity Color Palette
        private readonly Color PattenBlue = Color.FromArgb(226, 244, 255);      // #e2f4ff - Lightest
        private readonly Color Malibu = Color.FromArgb(118, 189, 255);          // #76bdff
        private readonly Color DodgerBlue1 = Color.FromArgb(69, 174, 255);      // #45aeff
        private readonly Color DodgerBlue2 = Color.FromArgb(33, 163, 255);      // #21a3ff - Primary
        private readonly Color DodgerBlue3 = Color.FromArgb(33, 136, 255);      // #2188ff - Darkest

        public CategoryManagementView(AuthenticationService authService)
        {
            _authService = authService;
            _context = new ApplicationDbContext();
            InitializeComponent();
            LoadCategories();
            LoadMetrics();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            this.Name = "CategoryManagementView";
            this.Text = "Category Management";
            this.WindowState = FormWindowState.Normal;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = PattenBlue; // Light blue background
            this.ClientSize = new Size(1200, 800);
            this.Padding = new Padding(0);
            
            CreateTopPanel();
            CreateMetricsPanel();
            CreateCategoriesGrid();
            
            this.ResumeLayout(false);
        }

        private void CreateTopPanel()
        {
            topPanel = new Panel
            {
                BackColor = Color.White,
                Dock = DockStyle.Top,
                Height = 80, // Increased from 70
                Padding = new Padding(20, 10, 20, 10)
            };

            // Title with Icon
            titleIcon = new IconPictureBox
            {
                IconChar = IconChar.Tags,
                IconColor = DodgerBlue2, // Primary blue
                IconSize = 26,
                Location = new Point(20, 12),
                Size = new Size(32, 32),
                BackColor = Color.Transparent
            };
            topPanel.Controls.Add(titleIcon);

            titleLabel = new Label
            {
                Text = "Category Management",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                Location = new Point(58, 13),
                Size = new Size(300, 28)
            };
            topPanel.Controls.Add(titleLabel);

            // Search with Icon
            var searchPanel = new Panel
            {
                BackColor = PattenBlue,
                Location = new Point(20, 50),
                Size = new Size(200, 28),
                BorderStyle = BorderStyle.FixedSingle
            };

            searchIcon = new IconPictureBox
            {
                IconChar = IconChar.Search,
                IconColor = Malibu,
                IconSize = 14,
                Location = new Point(8, 6),
                Size = new Size(18, 18),
                BackColor = Color.Transparent
            };
            searchPanel.Controls.Add(searchIcon);

            searchBox = new TextBox
            {
                PlaceholderText = "Search...",
                Font = new Font("Segoe UI", 9F),
                Location = new Point(30, 4),
                Size = new Size(165, 22),
                BorderStyle = BorderStyle.None,
                BackColor = PattenBlue
            };
            searchBox.TextChanged += SearchBox_TextChanged;
            searchPanel.Controls.Add(searchBox);
            topPanel.Controls.Add(searchPanel);

            // Status Filter
            statusFilter = new ComboBox
            {
                Font = new Font("Segoe UI", 9F),
                Location = new Point(235, 50),
                Size = new Size(110, 26),
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                BackColor = PattenBlue
            };
            statusFilter.Items.AddRange(new object[] { "All Status", "Active", "Inactive" });
            statusFilter.SelectedIndex = 0;
            statusFilter.SelectedIndexChanged += StatusFilter_SelectedIndexChanged;
            topPanel.Controls.Add(statusFilter);

            // Icon-only Action Buttons
            int buttonX = 900;
            
            addButton = CreateIconOnlyButton(IconChar.Plus, DodgerBlue2, "Add Category", buttonX, 47);
            addButton.Click += AddButton_Click;
            buttonX += 42;

            editButton = CreateIconOnlyButton(IconChar.Edit, Malibu, "Edit", buttonX, 47);
            editButton.Click += EditButton_Click;
            buttonX += 42;

            deleteButton = CreateIconOnlyButton(IconChar.Trash, Color.FromArgb(244, 67, 54), "Delete", buttonX, 47);
            deleteButton.Click += DeleteButton_Click;
            buttonX += 42;

            exportButton = CreateIconOnlyButton(IconChar.FileExport, DodgerBlue1, "Export", buttonX, 47);
            exportButton.Click += ExportButton_Click;
            buttonX += 42;

            refreshButton = CreateIconOnlyButton(IconChar.SyncAlt, DodgerBlue3, "Refresh", buttonX, 47);
            refreshButton.Click += RefreshButton_Click;

            topPanel.Controls.AddRange(new Control[] {
                addButton, editButton, deleteButton, exportButton, refreshButton
            });

            this.Controls.Add(topPanel);
        }

        private IconButton CreateIconOnlyButton(IconChar icon, Color color, string tooltip, int x, int y)
        {
            var button = new IconButton
            {
                IconChar = icon,
                IconColor = Color.White,
                IconSize = 18,
                BackColor = color,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(x, y),
                Size = new Size(36, 36),
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0 }
            };

            var toolTip = new ToolTip();
            toolTip.SetToolTip(button, tooltip);

            button.MouseEnter += (s, e) =>
            {
                button.BackColor = ControlPaint.Light(color, 0.2f);
            };

            button.MouseLeave += (s, e) =>
            {
                button.BackColor = color;
            };

            return button;
        }

        private void CreateMetricsPanel()
        {
            metricsPanel = new Panel
            {
                BackColor = Color.White,
                Dock = DockStyle.Top,
                Height = 60,
                Padding = new Padding(20, 10, 20, 10)
            };

            CreateMetricCard("Total", "0", IconChar.Tags, DodgerBlue2, 20, 10, "totalCategoriesLabel");
            CreateMetricCard("Active", "0", IconChar.CheckCircle, Malibu, 280, 10, "activeCategoriesLabel");
            CreateMetricCard("Products", "0", IconChar.Box, DodgerBlue1, 540, 10, "totalProductsLabel");
            CreateMetricCard("Avg/Category", "0", IconChar.ChartBar, DodgerBlue3, 800, 10, "avgProductsLabel");

            this.Controls.Add(metricsPanel);
        }

        private void CreateMetricCard(string title, string value, IconChar icon, Color color, int x, int y, string labelName)
        {
            var cardPanel = new Panel
            {
                BackColor = PattenBlue,
                Location = new Point(x, y),
                Size = new Size(240, 40),
                BorderStyle = BorderStyle.None
            };

            var iconPic = new IconPictureBox
            {
                IconChar = icon,
                IconColor = color,
                IconSize = 20,
                Location = new Point(10, 10),
                Size = new Size(24, 24),
                BackColor = Color.Transparent
            };
            cardPanel.Controls.Add(iconPic);

            var titleLbl = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 8F, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(40, 6),
                Size = new Size(190, 14)
            };
            cardPanel.Controls.Add(titleLbl);

            var valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                Location = new Point(40, 18),
                Size = new Size(190, 20),
                Name = labelName
            };
            cardPanel.Controls.Add(valueLabel);

            metricsPanel.Controls.Add(cardPanel);
        }

        private void CreateCategoriesGrid()
        {
            // Create a wrapper panel with padding
            var gridPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = PattenBlue,
                Padding = new Padding(10, 200, 10, 10) // Left, Top, Right, Bottom - 50px padding from top
            };

            categoriesGrid = new DataGridView
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
                Font = new Font("Segoe UI", 9F),
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
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(5)
                },
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 35
            };

            // Configure columns like Product Management
            categoriesGrid.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID", Width = 50, Visible = false },
                new DataGridViewTextBoxColumn { Name = "Name", HeaderText = "Category Name", Width = 200 },
                new DataGridViewTextBoxColumn { Name = "Description", HeaderText = "Description", Width = 300 },
                new DataGridViewTextBoxColumn { Name = "ProductCount", HeaderText = "Products", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Status", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "CreatedAt", HeaderText = "Created", Width = 120 }
            });

            // Format date column
            categoriesGrid.Columns["CreatedAt"].DefaultCellStyle.Format = "MM/dd/yyyy";

            gridPanel.Controls.Add(categoriesGrid);
            this.Controls.Add(gridPanel);
        }

        private async void LoadCategories()
        {
            try
            {
                refreshButton.IconChar = IconChar.Spinner;
                refreshButton.Enabled = false;
                titleIcon.IconChar = IconChar.HourglassHalf;

                var categories = await _context.Categories
                    .Include(c => c.Products)
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.Name)
                    .ToListAsync();

                categoriesGrid.Rows.Clear();

                foreach (var category in categories)
                {
                    var productCount = category.Products?.Count(p => p.IsActive) ?? 0;
                    var status = category.IsActive ? "Active" : "Inactive";

                    var rowIndex = categoriesGrid.Rows.Add(
                        category.Id,
                        category.Name,
                        category.Description ?? "No description",
                        productCount,
                        status,
                        category.CreatedAt
                    );

                    // Color code categories with no products (like low stock)
                    if (productCount == 0)
                    {
                        categoriesGrid.Rows[rowIndex].DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(255, 235, 235);
                        categoriesGrid.Rows[rowIndex].DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(139, 0, 0);
                    }
                }

                LoadMetrics();
            }
            catch (Exception ex)
            {
                titleIcon.IconChar = IconChar.ExclamationTriangle;
                titleIcon.IconColor = Color.FromArgb(244, 67, 54);
                MessageBox.Show($"Error loading categories: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                refreshButton.IconChar = IconChar.SyncAlt;
                refreshButton.Enabled = true;
                titleIcon.IconChar = IconChar.Tags;
                titleIcon.IconColor = DodgerBlue2;
            }
        }

        private Bitmap CreateStatusIconBitmap(bool isActive)
        {
            var bitmap = new Bitmap(20, 20);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                var color = isActive ? Malibu : Color.FromArgb(200, 200, 200);
                
                using (var brush = new SolidBrush(color))
                {
                    g.FillEllipse(brush, 3, 3, 14, 14);
                }
                
                using (var pen = new Pen(Color.White, 2))
                {
                    if (isActive)
                    {
                        g.DrawLines(pen, new Point[] { 
                            new Point(7, 10), 
                            new Point(9, 12), 
                            new Point(13, 8) 
                        });
                    }
                    else
                    {
                        g.DrawLine(pen, 8, 8, 12, 12);
                        g.DrawLine(pen, 12, 8, 8, 12);
                    }
                }
            }
            return bitmap;
        }

        private async void LoadMetrics()
        {
            try
            {
                var totalCategories = await _context.Categories.CountAsync();
                var activeCategories = await _context.Categories.CountAsync(c => c.IsActive);
                var totalProducts = await _context.Products.CountAsync(p => p.IsActive);
                var avgProducts = activeCategories > 0 ? (double)totalProducts / activeCategories : 0;

                UpdateMetricLabel("totalCategoriesLabel", totalCategories.ToString());
                UpdateMetricLabel("activeCategoriesLabel", activeCategories.ToString());
                UpdateMetricLabel("totalProductsLabel", totalProducts.ToString());
                UpdateMetricLabel("avgProductsLabel", Math.Round(avgProducts, 1).ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading metrics: {ex.Message}");
            }
        }

        private void UpdateMetricLabel(string labelName, string value)
        {
            var label = metricsPanel.Controls.Find(labelName, true).FirstOrDefault() as Label;
            if (label != null)
            {
                label.Text = value;
            }
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            FilterCategories();
        }

        private void StatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterCategories();
        }

        private async void FilterCategories()
        {
            try
            {
                searchIcon.IconColor = DodgerBlue2;

                var searchTerm = searchBox.Text.ToLower();
                var selectedStatus = statusFilter.SelectedItem?.ToString();

                var query = _context.Categories.Include(c => c.Products).Where(c => c.IsActive);

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(c => 
                        c.Name.ToLower().Contains(searchTerm) ||
                        (c.Description != null && c.Description.ToLower().Contains(searchTerm)));
                }

                if (!string.IsNullOrEmpty(selectedStatus) && selectedStatus != "All Status")
                {
                    var isActive = selectedStatus == "Active";
                    query = query.Where(c => c.IsActive == isActive);
                }

                var categories = await query.OrderBy(c => c.Name).ToListAsync();

                categoriesGrid.Rows.Clear();

                foreach (var category in categories)
                {
                    var productCount = category.Products?.Count(p => p.IsActive) ?? 0;
                    var status = category.IsActive ? "Active" : "Inactive";

                    var rowIndex = categoriesGrid.Rows.Add(
                        category.Id,
                        category.Name,
                        category.Description ?? "No description",
                        productCount,
                        status,
                        category.CreatedAt
                    );

                    if (productCount == 0)
                    {
                        categoriesGrid.Rows[rowIndex].DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(255, 235, 235);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error filtering categories: {ex.Message}");
            }
            finally
            {
                await Task.Delay(300);
                searchIcon.IconColor = Malibu;
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (!_authService.HasPermission("ADD_CATEGORY"))
            {
                MessageBox.Show("You don't have permission to add categories.", "Access Denied", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var addForm = new CategoryEditForm(_authService);
            if (addForm.ShowDialog() == DialogResult.OK)
            {
                LoadCategories();
            }
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            if (!_authService.HasPermission("EDIT_CATEGORY"))
            {
                MessageBox.Show("You don't have permission to edit categories.", "Access Denied", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (categoriesGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a category to edit.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var categoryId = (int)categoriesGrid.SelectedRows[0].Cells["Id"].Value;
            var editForm = new CategoryEditForm(_authService, categoryId);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadCategories();
            }
        }

        private async void DeleteButton_Click(object sender, EventArgs e)
        {
            if (!_authService.HasPermission("DELETE_CATEGORY"))
            {
                MessageBox.Show("You don't have permission to delete categories.", "Access Denied", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (categoriesGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a category to delete.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var categoryId = (int)categoriesGrid.SelectedRows[0].Cells["Id"].Value;
            var categoryName = categoriesGrid.SelectedRows[0].Cells["Name"].Value.ToString();

            var productCount = await _context.Products.CountAsync(p => p.CategoryId == categoryId && p.IsActive);
            if (productCount > 0)
            {
                MessageBox.Show($"Cannot delete '{categoryName}' - has {productCount} products.", 
                    "Cannot Delete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete the category '{categoryName}'?", 
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    deleteButton.IconChar = IconChar.Spinner;
                    deleteButton.Enabled = false;

                    var category = await _context.Categories.FindAsync(categoryId);
                    
                    if (category != null)
                    {
                        category.IsActive = false;
                        await _context.SaveChangesAsync();
                        
                        await _authService.LogAuditAsync("CATEGORY_DELETED", "Categories", categoryId, 
                            $"'{categoryName}', Active", $"'{categoryName}', Inactive");
                        
                        deleteButton.IconChar = IconChar.Check;
                        await Task.Delay(500);
                        
                        LoadCategories();
                        
                        MessageBox.Show("Category deleted successfully.", "Success", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting category: {ex.Message}", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    deleteButton.IconChar = IconChar.Trash;
                    deleteButton.Enabled = true;
                }
            }
        }

        private async void ExportButton_Click(object sender, EventArgs e)
        {
            try
            {
                exportButton.IconChar = IconChar.Spinner;
                exportButton.Enabled = false;

                var saveDialog = new SaveFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv",
                    DefaultExt = "csv",
                    FileName = $"Categories_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    var categories = await _context.Categories
                        .Include(c => c.Products)
                        .OrderBy(c => c.Name)
                        .ToListAsync();

                    using (var writer = new StreamWriter(saveDialog.FileName))
                    {
                        writer.WriteLine("Name,Description,Status,Products,Created");

                        foreach (var category in categories)
                        {
                            var productCount = category.Products?.Count(p => p.IsActive) ?? 0;
                            var status = category.IsActive ? "Active" : "Inactive";
                            var description = category.Description?.Replace("\"", "\"\"") ?? "";
                            
                            writer.WriteLine($"\"{category.Name}\",\"{description}\",\"{status}\",{productCount},\"{category.CreatedAt:yyyy-MM-dd}\"");
                        }
                    }

                    exportButton.IconChar = IconChar.Check;
                    await Task.Delay(500);

                    MessageBox.Show($"Exported to {saveDialog.FileName}", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Export Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                exportButton.IconChar = IconChar.FileExport;
                exportButton.Enabled = true;
            }
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            LoadCategories();
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