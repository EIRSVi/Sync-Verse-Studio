using System.Drawing;
using SyncVerseStudio.Services;
using SyncVerseStudio.Models;
using SyncVerseStudio.Data;
using SyncVerseStudio.Helpers;
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
                Text = "SUPPLIER MANAGEMENT",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                Location = new Point(20, 18),
                Size = new Size(400, 35)
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
            addButton = CreateButton("ADD SUPPLIER", Color.FromArgb(48, 148, 255), buttonX, 45, 130);
            addButton.Click += AddButton_Click;
            buttonX += 140;

            editButton = CreateButton("EDIT", Color.FromArgb(48, 148, 255), buttonX, 45, 80);
            editButton.Click += EditButton_Click;
            buttonX += 90;

            deleteButton = CreateButton("DELETE", Color.FromArgb(255, 0, 80), buttonX, 45, 90);
            deleteButton.Click += DeleteButton_Click;
            buttonX += 100;

            refreshButton = CreateButton("REFRESH", Color.FromArgb(117, 117, 117), buttonX, 45, 100);
            refreshButton.Click += RefreshButton_Click;

            topPanel.Controls.AddRange(new Control[] {
                titleLabel, supplierCountLabel, searchBox, statusFilter,
                addButton, editButton, deleteButton, refreshButton
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
                FlatAppearance = { BorderSize = 0 },
                Location = new Point(x, y),
                Size = new Size(width, 35),
                Cursor = Cursors.Hand
            };
        }

        private void CreateSuppliersGrid()
        {
            var gridPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(10, 200, 10, 10)
            };
            
            // Add rounded border to panel
            gridPanel.Paint += (s, e) =>
            {
                var rect = new Rectangle(10, 200, gridPanel.Width - 20, gridPanel.Height - 210);
                using (var pen = new Pen(BrandTheme.TableBorder, 2))
                {
                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    var path = new System.Drawing.Drawing2D.GraphicsPath();
                    int radius = 10;
                    path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
                    path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
                    path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
                    path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
                    path.CloseFigure();
                    e.Graphics.DrawPath(pen, path);
                }
            };

            suppliersGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                RowHeadersVisible = false,
                GridColor = BrandTheme.TableBorder,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.White,
                    ForeColor = BrandTheme.PrimaryText,
                    SelectionBackColor = BrandTheme.TableRowSelected,
                    SelectionForeColor = Color.White,
                    Padding = new Padding(12, 8, 12, 8),
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold)
                },
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = BrandTheme.TableRowOdd,
                    ForeColor = BrandTheme.PrimaryText,
                    SelectionBackColor = BrandTheme.TableRowSelected,
                    SelectionForeColor = Color.White,
                    Padding = new Padding(12, 8, 12, 8),
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold)
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

            gridPanel.Controls.Add(suppliersGrid);
            this.Controls.Add(gridPanel);
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
