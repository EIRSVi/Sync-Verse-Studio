using SyncVerseStudio.Services;
using SyncVerseStudio.Models;
using SyncVerseStudio.Data;
using SyncVerseStudio.Helpers;
using Microsoft.EntityFrameworkCore;

namespace SyncVerseStudio.Views
{
    public partial class CustomerManagementView : Form
    {
        private readonly AuthenticationService _authService;
        private readonly ApplicationDbContext _context;
        private DataGridView customersGrid;
        private Panel topPanel;
        private Button addButton, editButton, deleteButton, refreshButton;
        private TextBox searchBox;
        private Label customerCountLabel;

        public CustomerManagementView(AuthenticationService authService)
        {
            _authService = authService;
            _context = new ApplicationDbContext();
            InitializeComponent();
            LoadCustomers();
        }

        private void InitializeComponent()
        {
            this.AutoScaleDimensions = new SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(250, 250, 250);
            this.ClientSize = new Size(1000, 700);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Name = "CustomerManagementView";
            this.Text = "Customer Management";

            CreateTopPanel();
            CreateCustomersGrid();
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
                Text = "CUSTOMER MANAGEMENT",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                Location = new Point(20, 18),
                Size = new Size(400, 35)
            };

            customerCountLabel = new Label
            {
                Text = "Total Customers: 0",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(330, 20),
                Size = new Size(200, 25)
            };

            searchBox = new TextBox
            {
                PlaceholderText = "Search customers...",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(20, 45),
                Size = new Size(250, 25)
            };
            searchBox.TextChanged += SearchBox_TextChanged;

            // Buttons
            int buttonX = 600;
            addButton = CreateButton("ADD CUSTOMER", Color.FromArgb(48, 148, 255), buttonX, 45, 140);
            addButton.Click += AddButton_Click;
            buttonX += 150;

            editButton = CreateButton("EDIT", Color.FromArgb(48, 148, 255), buttonX, 45, 80);
            editButton.Click += EditButton_Click;
            buttonX += 90;

            deleteButton = CreateButton("DELETE", Color.FromArgb(255, 0, 80), buttonX, 45, 90);
            deleteButton.Click += DeleteButton_Click;
            buttonX += 100;

            refreshButton = CreateButton("REFRESH", Color.FromArgb(117, 117, 117), buttonX, 45, 100);
            refreshButton.Click += RefreshButton_Click;

            topPanel.Controls.AddRange(new Control[] {
                titleLabel, customerCountLabel, searchBox, 
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

        private void CreateCustomersGrid()
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

            customersGrid = new DataGridView
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
            customersGrid.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID", Width = 50, Visible = false },
                new DataGridViewTextBoxColumn { Name = "FirstName", HeaderText = "First Name", Width = 150 },
                new DataGridViewTextBoxColumn { Name = "LastName", HeaderText = "Last Name", Width = 150 },
                new DataGridViewTextBoxColumn { Name = "Email", HeaderText = "Email", Width = 200 },
                new DataGridViewTextBoxColumn { Name = "Phone", HeaderText = "Phone", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "Address", HeaderText = "Address", Width = 200 },
                new DataGridViewTextBoxColumn { Name = "LoyaltyPoints", HeaderText = "Points", Width = 80 },
                new DataGridViewTextBoxColumn { Name = "TotalPurchases", HeaderText = "Total Spent", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "LastPurchase", HeaderText = "Last Purchase", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "CreatedAt", HeaderText = "Member Since", Width = 120 }
            });

            // Format currency column
            customersGrid.Columns["TotalPurchases"].DefaultCellStyle.Format = "C2";
     
            // Format date columns
            customersGrid.Columns["LastPurchase"].DefaultCellStyle.Format = "MM/dd/yyyy";
            customersGrid.Columns["CreatedAt"].DefaultCellStyle.Format = "MM/dd/yyyy";

            gridPanel.Controls.Add(customersGrid);
            this.Controls.Add(gridPanel);
        }

        private async void LoadCustomers()
        {
            try
            {
                var customers = await _context.Customers
                    .Select(c => new
                    {
                        c.Id,
                        c.FirstName,
                        c.LastName,
                        c.Email,
                        c.Phone,
                        c.Address,
                        c.LoyaltyPoints,
                        TotalPurchases = _context.Sales
                            .Where(s => s.CustomerId == c.Id && s.Status == SaleStatus.Completed)
                            .Sum(s => (decimal?)s.TotalAmount) ?? 0,
                        LastPurchase = _context.Sales
                            .Where(s => s.CustomerId == c.Id && s.Status == SaleStatus.Completed)
                            .Max(s => (DateTime?)s.SaleDate),
                        c.CreatedAt
                    })
                    .OrderBy(c => c.FirstName)
                    .ThenBy(c => c.LastName)
                    .ToListAsync();

                customersGrid.Rows.Clear();

                foreach (var customer in customers)
                {
                    var rowIndex = customersGrid.Rows.Add(
                        customer.Id,
                        customer.FirstName ?? "",
                        customer.LastName ?? "",
                        customer.Email ?? "",
                        customer.Phone ?? "",
                        customer.Address ?? "",
                        customer.LoyaltyPoints,
                        customer.TotalPurchases,
                        customer.LastPurchase,
                        customer.CreatedAt
                    );

                    // Color code VIP customers (spent more than $1000)
                    if (customer.TotalPurchases >= 1000)
                    {
                        customersGrid.Rows[rowIndex].DefaultCellStyle.BackColor = Color.FromArgb(240, 253, 244);
                        customersGrid.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(22, 101, 52);
                    }
                }

                customerCountLabel.Text = $"Total Customers: {customers.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customers: {ex.Message}", "Error", 
        MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            FilterCustomers();
        }

        private async void FilterCustomers()
        {
            try
            {
                var searchTerm = searchBox.Text.ToLower();

                var query = _context.Customers.AsQueryable();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(c => 
                        (c.FirstName != null && c.FirstName.ToLower().Contains(searchTerm)) ||
                        (c.LastName != null && c.LastName.ToLower().Contains(searchTerm)) ||
                        (c.Email != null && c.Email.ToLower().Contains(searchTerm)) ||
                        (c.Phone != null && c.Phone.Contains(searchTerm)));
                }

                var customers = await query
                    .Select(c => new
                    {
                        c.Id,
                        c.FirstName,
                        c.LastName,
                        c.Email,
                        c.Phone,
                        c.Address,
                        c.LoyaltyPoints,
                        TotalPurchases = _context.Sales
                            .Where(s => s.CustomerId == c.Id && s.Status == SaleStatus.Completed)
                            .Sum(s => (decimal?)s.TotalAmount) ?? 0,
                        LastPurchase = _context.Sales
                            .Where(s => s.CustomerId == c.Id && s.Status == SaleStatus.Completed)
                            .Max(s => (DateTime?)s.SaleDate),
                        c.CreatedAt
                    })
                    .OrderBy(c => c.FirstName)
                    .ThenBy(c => c.LastName)
                    .ToListAsync();

                customersGrid.Rows.Clear();

                foreach (var customer in customers)
                {
                    var rowIndex = customersGrid.Rows.Add(
                        customer.Id,
                        customer.FirstName ?? "",
                        customer.LastName ?? "",
                        customer.Email ?? "",
                        customer.Phone ?? "",
                        customer.Address ?? "",
                        customer.LoyaltyPoints,
                        customer.TotalPurchases,
                        customer.LastPurchase,
                        customer.CreatedAt
                    );

                    if (customer.TotalPurchases >= 1000)
                    {
                        customersGrid.Rows[rowIndex].DefaultCellStyle.BackColor = Color.FromArgb(240, 253, 244);
                        customersGrid.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(22, 101, 52);
                    }
                }

                customerCountLabel.Text = $"Total Customers: {customers.Count}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error filtering customers: {ex.Message}");
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (!_authService.HasPermission("ADD_CUSTOMER"))
            {
                MessageBox.Show("You don't have permission to add customers.", "Access Denied", 
         MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var addForm = new CustomerEditForm(_authService);
            if (addForm.ShowDialog() == DialogResult.OK)
            {
                LoadCustomers();
            }
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            if (!_authService.HasPermission("EDIT_CUSTOMER"))
            {
                MessageBox.Show("You don't have permission to edit customers.", "Access Denied", 
      MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (customersGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a customer to edit.", "No Selection", 
          MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var customerId = (int)customersGrid.SelectedRows[0].Cells["Id"].Value;
            var editForm = new CustomerEditForm(_authService, customerId);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadCustomers();
            }
        }

        private async void DeleteButton_Click(object sender, EventArgs e)
        {
            if (!_authService.HasPermission("DELETE_CUSTOMER"))
            {
                MessageBox.Show("You don't have permission to delete customers.", "Access Denied", 
      MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (customersGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a customer to delete.", "No Selection", 
         MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var firstName = customersGrid.SelectedRows[0].Cells["FirstName"].Value?.ToString();
            var lastName = customersGrid.SelectedRows[0].Cells["LastName"].Value?.ToString();
            var customerName = $"{firstName} {lastName}".Trim();
  
            var result = MessageBox.Show($"Are you sure you want to delete customer '{customerName}'?", 
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    var customerId = (int)customersGrid.SelectedRows[0].Cells["Id"].Value;
                    var customer = await _context.Customers.FindAsync(customerId);

                    if (customer != null)
                    {
                        // Check if customer has any sales
                        var hasSales = await _context.Sales.AnyAsync(s => s.CustomerId == customerId);
                        if (hasSales)
                        {
                            var confirmResult = MessageBox.Show(
                                "This customer has purchase history. Are you sure you want to delete?", 
                                "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        
                            if (confirmResult != DialogResult.Yes)
                                return;
                        }

                        _context.Customers.Remove(customer);
                        await _context.SaveChangesAsync();

                        await _authService.LogAuditAsync("DELETE_CUSTOMER", "Customers", customerId, null, 
  $"Deleted customer: {customerName}");

                        LoadCustomers();

                        MessageBox.Show("Customer deleted successfully.", "Success", 
             MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting customer: {ex.Message}", "Error", 
      MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            LoadCustomers();
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
