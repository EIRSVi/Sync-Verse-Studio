using System.Drawing;
using SyncVerseStudio.Services;
using SyncVerseStudio.Models;
using SyncVerseStudio.Data;
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
        private Label supplierCountLabel;

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
            this.BackColor = Color.FromArgb(250, 250, 250);
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
                BackColor = Color.White,
                Dock = DockStyle.Top,
                Height = 80,
                Padding = new Padding(20, 10, 20, 10)
            };

            var titleLabel = new Label
            {
                Text = "Supplier Management",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                Location = new Point(20, 15),
                Size = new Size(300, 30)
            };

            supplierCountLabel = new Label
            {
                Text = "Total Suppliers: 0",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(330, 20),
                Size = new Size(200, 25)
            };

            searchBox = new TextBox
            {
                PlaceholderText = "Search suppliers...",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(20, 45),
                Size = new Size(200, 25)
            };
            searchBox.TextChanged += SearchBox_TextChanged;

            statusFilter = new ComboBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(240, 45),
                Size = new Size(120, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            statusFilter.Items.AddRange(new[] { "All Status", "Active", "Inactive" });
            statusFilter.SelectedIndex = 0;
            statusFilter.SelectedIndexChanged += StatusFilter_SelectedIndexChanged;

            // Buttons
            int buttonX = 600;
            addButton = CreateButton("Add Supplier", Color.FromArgb(24, 119, 18), buttonX, 45);
            addButton.Click += AddButton_Click;
            buttonX += 120;

            editButton = CreateButton("Edit", Color.FromArgb(37, 99, 102), buttonX, 45);
            editButton.Click += EditButton_Click;
            buttonX += 80;

            deleteButton = CreateButton("Delete", Color.FromArgb(255, 0, 80), buttonX, 45);
            deleteButton.Click += DeleteButton_Click;
            buttonX += 80;

            refreshButton = CreateButton("Refresh", Color.FromArgb(158, 158, 158), buttonX, 45);
            refreshButton.Click += RefreshButton_Click;

            topPanel.Controls.AddRange(new Control[] {
                titleLabel, supplierCountLabel, searchBox, statusFilter,
                addButton, editButton, deleteButton, refreshButton
            });

            this.Controls.Add(topPanel);
        }

        private Button CreateButton(string text, Color backgroundColor, int x, int y)
        {
            return new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                BackColor = backgroundColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Location = new Point(x, y),
                Size = new Size(text == "Add Supplier" ? 110 : 70, 30),
                Cursor = Cursors.Hand
            };
        }

        private void CreateSuppliersGrid()
        {
            suppliersGrid = new DataGridView
            {
                Location = new Point(0, 200),
                Size = new Size(1000, 500),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 9F),
                RowHeadersVisible = false,
                GridColor = Color.FromArgb(230, 230, 230),
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                ColumnHeadersHeight = 40,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.White,
                    ForeColor = Color.FromArgb(33, 33, 33),
                    SelectionBackColor = Color.FromArgb(24, 119, 18),
                    SelectionForeColor = Color.White,
                    Padding = new Padding(5)
                },
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(245, 245, 245),
                    ForeColor = Color.FromArgb(33, 33, 33),
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(5),
                    WrapMode = DataGridViewTriState.True
                }
            };

            // Configure columns
            suppliersGrid.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID", Width = 50, Visible = false },
                new DataGridViewTextBoxColumn { Name = "Name", HeaderText = "Supplier Name", Width = 180 },
                new DataGridViewTextBoxColumn { Name = "ContactPerson", HeaderText = "Contact Person", Width = 150 },
                new DataGridViewTextBoxColumn { Name = "Phone", HeaderText = "Phone", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "Email", HeaderText = "Email", Width = 180 },
                new DataGridViewTextBoxColumn { Name = "Address", HeaderText = "Address", Width = 200 },
                new DataGridViewTextBoxColumn { Name = "ProductCount", HeaderText = "Products", Width = 80 },
                new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Status", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "CreatedAt", HeaderText = "Created", Width = 120 }
            });

            // Format date column
            suppliersGrid.Columns["CreatedAt"].DefaultCellStyle.Format = "MM/dd/yyyy";

            this.Controls.Add(suppliersGrid);
        }

        private async void LoadSuppliers()
        {
            try
            {
                var query = _context.Suppliers.AsQueryable();

                var searchTerm = searchBox?.Text?.ToLower() ?? "";
                var statusValue = statusFilter?.SelectedItem?.ToString() ?? "All Status";

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(s => 
(s.Name != null && s.Name.ToLower().Contains(searchTerm)) ||
   (s.ContactPerson != null && s.ContactPerson.ToLower().Contains(searchTerm)) ||
       (s.Email != null && s.Email.ToLower().Contains(searchTerm)) ||
   (s.Phone != null && s.Phone.Contains(searchTerm)));
                }

                if (statusValue == "Active")
 {
  query = query.Where(s => s.IsActive);
      }
       else if (statusValue == "Inactive")
  {
   query = query.Where(s => !s.IsActive);
     }

         var suppliers = await query
       .Select(s => new
       {
    s.Id,
      s.Name,
        s.ContactPerson,
     s.Phone,
       s.Email,
         s.Address,
     ProductCount = _context.Products.Count(p => p.SupplierId == s.Id),
            Status = s.IsActive ? "Active" : "Inactive",
   s.CreatedAt
   })
         .OrderBy(s => s.Name)
 .ToListAsync();

              suppliersGrid.Rows.Clear();

foreach (var supplier in suppliers)
      {
      var rowIndex = suppliersGrid.Rows.Add(
supplier.Id,
        supplier.Name ?? "",
          supplier.ContactPerson ?? "",
           supplier.Phone ?? "",
           supplier.Email ?? "",
 supplier.Address ?? "",
        supplier.ProductCount,
   supplier.Status,
   supplier.CreatedAt
     );

// Color code inactive suppliers
     if (supplier.Status == "Inactive")
      {
    suppliersGrid.Rows[rowIndex].DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 235);
       suppliersGrid.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(139, 0, 0);
         }
  }

       if (supplierCountLabel != null)
          {
 supplierCountLabel.Text = $"Total Suppliers: {suppliers.Count}";
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
   LoadSuppliers();
 }

        private void StatusFilter_SelectedIndexChanged(object sender, EventArgs e)
  {
LoadSuppliers();
     }

   private void AddButton_Click(object sender, EventArgs e)
        {
if (!_authService.HasPermission("ADD_SUPPLIER"))
            {
      MessageBox.Show("You don't have permission to add suppliers.", "Access Denied", 
      MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
        }

   var addForm = new SupplierEditForm(_authService);
  if (addForm.ShowDialog() == DialogResult.OK)
       {
LoadSuppliers();
        }
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

   var supplierId = (int)suppliersGrid.SelectedRows[0].Cells["Id"].Value;
  var editForm = new SupplierEditForm(_authService, supplierId);
if (editForm.ShowDialog() == DialogResult.OK)
      {
  LoadSuppliers();
   }
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

  var supplierName = suppliersGrid.SelectedRows[0].Cells["Name"].Value?.ToString();
       
var result = MessageBox.Show($"Are you sure you want to delete supplier '{supplierName}'?", 
   "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

  if (result == DialogResult.Yes)
   {
try
 {
var supplierId = (int)suppliersGrid.SelectedRows[0].Cells["Id"].Value;
       var supplier = await _context.Suppliers.FindAsync(supplierId);

if (supplier != null)
       {
       // Check if supplier has any products
         var hasProducts = await _context.Products.AnyAsync(p => p.SupplierId == supplierId);
          if (hasProducts)
       {
       var confirmResult = MessageBox.Show(
       "This supplier has associated products. Are you sure you want to delete?", 
    "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
  
    if (confirmResult != DialogResult.Yes)
       return;
         }

   _context.Suppliers.Remove(supplier);
  await _context.SaveChangesAsync();

                  await _authService.LogAuditAsync("DELETE_SUPPLIER", "Suppliers", supplierId, null, 
                    $"Deleted supplier: {supplierName}");

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