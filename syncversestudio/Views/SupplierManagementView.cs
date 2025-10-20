using System.Drawing;
using SyncVerseStudio.Services;
using SyncVerseStudio.Data;
using SyncVerseStudio.Models;
using Microsoft.EntityFrameworkCore;

namespace SyncVerseStudio.Views
{
    public partial class SupplierManagementView : Form
    {
        private readonly AuthenticationService _authService;
        private readonly ApplicationDbContext _context;
        private DataGridView suppliersGrid;
        private Panel topPanel;
        private Button addButton, editButton, deleteButton, refreshButton;
        private TextBox searchBox;
        private ComboBox statusFilter;

        public SupplierManagementView(AuthenticationService authService)
        {
            _authService = authService;
            _context = new ApplicationDbContext();
            InitializeComponent();
            LoadSuppliers();
        }

        private void InitializeComponent()
        {
            this.AutoScaleDimensions = new SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(250, 250, 250);
            this.ClientSize = new Size(1000, 700);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Name = "SupplierManagementView";
            this.Text = "Supplier Management";

            CreateTopPanel();
            CreateSuppliersGrid();
        }

        private void CreateTopPanel()
        {
            topPanel = new Panel
            {
                BackColor = System.Drawing.Color.White,
                Dock = DockStyle.Top,
                Height = 80,
                Padding = new Padding(20, 10, 20, 10)
            };

            var titleLabel = new Label
            {
                Text = "?? Supplier Management",
                Font = new System.Drawing.Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(33, 33, 33),
                Location = new Point(20, 15),
                Size = new Size(300, 30)
            };

            searchBox = new TextBox
            {
                PlaceholderText = "?? Search suppliers...",
                Font = new System.Drawing.Font("Segoe UI", 10F),
                Location = new Point(20, 45),
                Size = new Size(200, 25)
            };
            searchBox.TextChanged += SearchBox_TextChanged;

            statusFilter = new ComboBox
            {
                Font = new System.Drawing.Font("Segoe UI", 10F),
                Location = new Point(240, 45),
                Size = new Size(120, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            statusFilter.Items.AddRange(new object[] { "All Status", "Active", "Inactive" });
            statusFilter.SelectedIndex = 0;
            statusFilter.SelectedIndexChanged += StatusFilter_SelectedIndexChanged;

            // Buttons
            int buttonX = 500;
            addButton = CreateButton("? Add Supplier", System.Drawing.Color.FromArgb(24, 119, 18), buttonX, 45);
            addButton.Click += AddButton_Click;
            buttonX += 130;

            editButton = CreateButton("?? Edit", System.Drawing.Color.FromArgb(37, 99, 102), buttonX, 45);
            editButton.Click += EditButton_Click;
            buttonX += 80;

            deleteButton = CreateButton("??? Delete", System.Drawing.Color.FromArgb(255, 0, 80), buttonX, 45);
            deleteButton.Click += DeleteButton_Click;
            buttonX += 90;

            refreshButton = CreateButton("?? Refresh", System.Drawing.Color.FromArgb(158, 158, 158), buttonX, 45);
            refreshButton.Click += RefreshButton_Click;

            topPanel.Controls.AddRange(new Control[] {
                titleLabel, searchBox, statusFilter,
                addButton, editButton, deleteButton, refreshButton
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
                Size = new Size(text.Contains("Add Supplier") ? 120 : text.Contains("Delete") ? 80 : text.Contains("Refresh") ? 90 : 70, 30),
                Cursor = Cursors.Hand
            };
        }

        private void CreateSuppliersGrid()
        {
            // Create a wrapper panel with padding
            var gridPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.FromArgb(250, 250, 250),
                Padding = new Padding(10, 200, 10, 10) // Left, Top, Right, Bottom - 200px padding from top
            };

            suppliersGrid = new DataGridView
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

            // Configure columns - NO ICONS, clean text only
            suppliersGrid.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID", Width = 50, Visible = false },
                new DataGridViewTextBoxColumn { Name = "Name", HeaderText = "Supplier Name", Width = 200 },
                new DataGridViewTextBoxColumn { Name = "ContactPerson", HeaderText = "Contact Person", Width = 150 },
                new DataGridViewTextBoxColumn { Name = "Phone", HeaderText = "Phone", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "Email", HeaderText = "Email", Width = 180 },
                new DataGridViewTextBoxColumn { Name = "Address", HeaderText = "Address", Width = 200 },
                new DataGridViewTextBoxColumn { Name = "ProductCount", HeaderText = "Products", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Status", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "CreatedAt", HeaderText = "Created", Width = 120 }
            });

            // Format date columns
            suppliersGrid.Columns["CreatedAt"].DefaultCellStyle.Format = "MM/dd/yyyy";

            gridPanel.Controls.Add(suppliersGrid);
            this.Controls.Add(gridPanel);
        }

        private async void LoadSuppliers()
        {
            try
            {
                var suppliers = await _context.Suppliers
                    .Include(s => s.Products)
                    .OrderBy(s => s.Name)
                    .ToListAsync();

                suppliersGrid.Rows.Clear();

                foreach (var supplier in suppliers)
                {
                    var status = supplier.IsActive ? "Active" : "Inactive";
                    var productCount = supplier.Products?.Count(p => p.IsActive) ?? 0;

                    var rowIndex = suppliersGrid.Rows.Add(
                        supplier.Id,
                        supplier.Name,
                        supplier.ContactPerson ?? "Not specified",
                        supplier.Phone ?? "Not specified",
                        supplier.Email ?? "Not specified",
                        supplier.Address ?? "Not specified",
                        productCount.ToString(), // Clean number, no icons
                        status,
                        supplier.CreatedAt
                    );

                    // Color code inactive suppliers
                    if (!supplier.IsActive)
                    {
                        suppliersGrid.Rows[rowIndex].DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(255, 235, 235);
                        suppliersGrid.Rows[rowIndex].DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(139, 0, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading suppliers: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            FilterSuppliers();
        }

        private void StatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterSuppliers();
        }

        private async void FilterSuppliers()
        {
            try
            {
                var searchTerm = searchBox.Text.ToLower();
                var selectedStatus = statusFilter.SelectedItem?.ToString();

                var query = _context.Suppliers.Include(s => s.Products).AsQueryable();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(s => 
                        s.Name.ToLower().Contains(searchTerm) ||
                        (s.ContactPerson != null && s.ContactPerson.ToLower().Contains(searchTerm)) ||
                        (s.Email != null && s.Email.ToLower().Contains(searchTerm)) ||
                        (s.Phone != null && s.Phone.ToLower().Contains(searchTerm)));
                }

                if (!string.IsNullOrEmpty(selectedStatus) && selectedStatus != "All Status")
                {
                    var isActive = selectedStatus == "Active";
                    query = query.Where(s => s.IsActive == isActive);
                }

                var suppliers = await query.OrderBy(s => s.Name).ToListAsync();

                suppliersGrid.Rows.Clear();

                foreach (var supplier in suppliers)
                {
                    var status = supplier.IsActive ? "Active" : "Inactive";
                    var productCount = supplier.Products?.Count(p => p.IsActive) ?? 0;

                    var rowIndex = suppliersGrid.Rows.Add(
                        supplier.Id,
                        supplier.Name,
                        supplier.ContactPerson ?? "Not specified",
                        supplier.Phone ?? "Not specified",
                        supplier.Email ?? "Not specified",
                        supplier.Address ?? "Not specified",
                        productCount.ToString(), // Clean number, no icons
                        status,
                        supplier.CreatedAt
                    );

                    if (!supplier.IsActive)
                    {
                        suppliersGrid.Rows[rowIndex].DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(255, 235, 235);
                        suppliersGrid.Rows[rowIndex].DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(139, 0, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error filtering suppliers: {ex.Message}");
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (!_authService.HasPermission("ADD_SUPPLIER"))
            {
                MessageBox.Show("You don't have permission to add suppliers.", "Access Denied", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show("?? Supplier Edit Form - Coming Soon!\n\nThis feature will allow you to add and edit supplier information including:\n\n• Company Details\n• Contact Information\n• Address & Location\n• Product Associations", 
                "Feature In Development", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            if (!_authService.HasPermission("EDIT_SUPPLIER"))
            {
                MessageBox.Show("You don't have permission to edit suppliers.", "Access Denied", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (suppliersGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a supplier to edit.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            MessageBox.Show("?? Edit Supplier Feature - Coming Soon!\n\nThis will allow you to modify all supplier details.", 
                "Feature In Development", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void DeleteButton_Click(object sender, EventArgs e)
        {
            if (!_authService.HasPermission("DELETE_SUPPLIER"))
            {
                MessageBox.Show("You don't have permission to delete suppliers.", "Access Denied", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (suppliersGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a supplier to delete.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var supplierId = (int)suppliersGrid.SelectedRows[0].Cells["Id"].Value;
            var supplierName = suppliersGrid.SelectedRows[0].Cells["Name"].Value.ToString();

            // Check if supplier has products
            var productCount = await _context.Products.CountAsync(p => p.SupplierId == supplierId && p.IsActive);
            if (productCount > 0)
            {
                MessageBox.Show($"Cannot delete supplier '{supplierName}' because it has {productCount} active products.\n\nPlease reassign or delete the products first.", 
                    "Cannot Delete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete the supplier '{supplierName}'?", 
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    var supplier = await _context.Suppliers.FindAsync(supplierId);
                    
                    if (supplier != null)
                    {
                        supplier.IsActive = false;
                        await _context.SaveChangesAsync();
                        
                        // Log the action
                        await _authService.LogAuditAsync("SUPPLIER_DELETED", "Suppliers", supplierId, 
                            $"Name: {supplierName}, Status: Active", 
                            $"Name: {supplierName}, Status: Inactive - Deleted by {_authService.CurrentUser?.Username}");
                        
                        LoadSuppliers();
                        
                        MessageBox.Show("Supplier deleted successfully.", "Success", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting supplier: {ex.Message}", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            LoadSuppliers();
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