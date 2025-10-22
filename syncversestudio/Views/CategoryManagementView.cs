using System.Drawing;
using SyncVerseStudio.Services;
using SyncVerseStudio.Data;
using SyncVerseStudio.Models;
using SyncVerseStudio.Helpers;
using Microsoft.EntityFrameworkCore;

namespace SyncVerseStudio.Views
{
    public partial class CategoryManagementView : Form
    {
        private readonly AuthenticationService _authService;
        private readonly ApplicationDbContext _context;
        private DataGridView categoriesGrid;
        private Panel topPanel;
        private Button addButton, editButton, deleteButton, refreshButton, exportButton;
        private TextBox searchBox;
        private ComboBox statusFilter;
        private Label titleLabel;



        public CategoryManagementView(AuthenticationService authService)
        {
            _authService = authService;
            _context = new ApplicationDbContext();
            InitializeComponent();
            LoadCategories();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            this.Name = "CategoryManagementView";
            this.Text = "Category Management";
            this.WindowState = FormWindowState.Normal;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(250, 250, 250);
            this.ClientSize = new Size(1000, 700);
            this.Padding = new Padding(0);
            
            CreateTopPanel();
            CreateCategoriesGrid();
            
            this.ResumeLayout(false);
        }

        private void CreateTopPanel()
        {
            topPanel = new Panel
            {
                BackColor = Color.White,
                Dock = DockStyle.Top,
                Height = 80,
                Padding = new Padding(20, 10, 20, 10)
            };

            titleLabel = new Label
            {
                Text = "CATEGORY MANAGEMENT",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                Location = new Point(20, 18),
                Size = new Size(450, 35)
            };
            topPanel.Controls.Add(titleLabel);

            searchBox = new TextBox
            {
                PlaceholderText = "Search categories...",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(20, 55),
                Size = new Size(220, 30)
            };
            searchBox.TextChanged += SearchBox_TextChanged;
            topPanel.Controls.Add(searchBox);

            statusFilter = new ComboBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(255, 55),
                Size = new Size(130, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            statusFilter.Items.AddRange(new object[] { "All Status", "Active", "Inactive" });
            statusFilter.SelectedIndex = 0;
            statusFilter.SelectedIndexChanged += StatusFilter_SelectedIndexChanged;
            topPanel.Controls.Add(statusFilter);

            // Buttons without icons
            int buttonX = 550;
            
            addButton = CreateButton("ADD CATEGORY", Color.FromArgb(48, 148, 255), buttonX, 55, 140);
            addButton.Click += AddButton_Click;
            buttonX += 150;

            editButton = CreateButton("EDIT", Color.FromArgb(48, 148, 255), buttonX, 55, 80);
            editButton.Click += EditButton_Click;
            buttonX += 90;

            deleteButton = CreateButton("DELETE", Color.FromArgb(255, 0, 80), buttonX, 55, 90);
            deleteButton.Click += DeleteButton_Click;
            buttonX += 100;

            exportButton = CreateButton("EXPORT", Color.FromArgb(48, 148, 255), buttonX, 55, 90);
            exportButton.Click += ExportButton_Click;
            buttonX += 100;

            refreshButton = CreateButton("REFRESH", Color.FromArgb(117, 117, 117), buttonX, 55, 100);
            refreshButton.Click += RefreshButton_Click;

            topPanel.Controls.AddRange(new Control[] {
                addButton, editButton, deleteButton, exportButton, refreshButton
            });

            this.Controls.Add(topPanel);
        }

        private Button CreateButton(string text, Color backgroundColor, int x, int y, int width)
        {
            return new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = backgroundColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(x, y),
                Size = new Size(width, 35),
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0 }
            };
        }

        private void CreateCategoriesGrid()
        {
            var gridPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(10, 10, 10, 10)
            };

            categoriesGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 10F),
                RowHeadersVisible = false,
                GridColor = BrandTheme.TableBorder,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.White,
                    ForeColor = BrandTheme.PrimaryText,
                    SelectionBackColor = BrandTheme.TableRowSelected,
                    SelectionForeColor = Color.White,
                    Padding = new Padding(12, 8, 12, 8)
                },
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = BrandTheme.TableRowOdd,
                    ForeColor = BrandTheme.PrimaryText,
                    SelectionBackColor = BrandTheme.TableRowSelected,
                    SelectionForeColor = Color.White,
                    Padding = new Padding(12, 8, 12, 8)
                },
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = BrandTheme.TableHeaderBg,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(12, 10, 12, 10)
                },
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 45,
                RowTemplate = { Height = 50 },
                EnableHeadersVisualStyles = false,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
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
                refreshButton.Enabled = false;

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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                refreshButton.Enabled = true;
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
                    deleteButton.Enabled = false;

                    var category = await _context.Categories.FindAsync(categoryId);
                    
                    if (category != null)
                    {
                        category.IsActive = false;
                        await _context.SaveChangesAsync();
                        
                        await _authService.LogAuditAsync("CATEGORY_DELETED", "Categories", categoryId, 
                            $"'{categoryName}', Active", $"'{categoryName}', Inactive");
                        
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
                    deleteButton.Enabled = true;
                }
            }
        }

        private async void ExportButton_Click(object sender, EventArgs e)
        {
            try
            {
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
