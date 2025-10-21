using SyncVerseStudio.Services;
using SyncVerseStudio.Models;
using SyncVerseStudio.Data;
using Microsoft.EntityFrameworkCore;

namespace SyncVerseStudio.Views
{
    public partial class SupplierManagementView : Form
    {
        private readonly AuthenticationService _authService;
        private ApplicationDbContext? _context;
        private DataGridView suppliersGridView;
        private Panel searchPanel;
        private Panel formPanel;
        private TextBox searchTextBox;
        private TextBox nameTextBox;
        private TextBox contactPersonTextBox;
        private TextBox phoneTextBox;
        private TextBox emailTextBox;
        private TextBox addressTextBox;
        private ComboBox statusComboBox;
        private Button saveButton;
        private Button cancelButton;
        private Button addSupplierButton;
        private int editingSupplierId = 0;
        private Label supplierCountLabel;

        public SupplierManagementView(AuthenticationService authService)
        {
            _authService = authService;
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.AutoScaleDimensions = new SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(248, 250, 252);
            this.ClientSize = new Size(1200, 800);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Name = "SupplierManagementView";
            this.Text = "Supplier Management";

            CreateLayout();
        }

        private void CreateLayout()
        {
            // Header
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.White,
                Padding = new Padding(20, 15, 20, 15)
            };

            var titleLabel = new Label
            {
                Text = "Supplier Management",
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(20, 20),
                Size = new Size(400, 40),
                TextAlign = ContentAlignment.MiddleLeft
            };

            supplierCountLabel = new Label
            {
                Text = "Total Suppliers: 0",
                Font = new Font("Segoe UI", 12F, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(450, 25),
                Size = new Size(200, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(supplierCountLabel);
            this.Controls.Add(headerPanel);

            // Search Panel
            CreateSearchPanel();

            // Form Panel (Right Side)
            CreateFormPanel();

            // Supplier Grid (Left Side)
            CreateSupplierGrid();
        }

        private void CreateSearchPanel()
        {
            searchPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.White,
                Padding = new Padding(20, 10, 20, 10)
            };

            var searchLabel = new Label
            {
                Text = "Search Suppliers:",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(0, 5),
                Size = new Size(150, 25)
            };

            searchTextBox = new TextBox
            {
                Font = new Font("Segoe UI", 11F),
                Location = new Point(0, 25),
                Size = new Size(300, 25),
                PlaceholderText = "Search suppliers..."
            };
            searchTextBox.TextChanged += SearchTextBox_TextChanged;

            var statusLabel = new Label
            {
                Text = "Status:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(320, 5),
                Size = new Size(50, 25)
            };

            var statusFilter = new ComboBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(320, 25),
                Size = new Size(100, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            statusFilter.Items.AddRange(new[] { "All Status", "Active", "Inactive" });
            statusFilter.SelectedIndex = 0;

            addSupplierButton = new Button
            {
                Text = "Add Supplier",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(34, 197, 94),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(440, 25),
                Size = new Size(120, 30),
                Cursor = Cursors.Hand
            };
            addSupplierButton.FlatAppearance.BorderSize = 0;
            addSupplierButton.Click += AddSupplierButton_Click;

            var refreshButton = new Button
            {
                Text = "Refresh",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(100, 116, 139),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(580, 25),
                Size = new Size(80, 30),
                Cursor = Cursors.Hand
            };
            refreshButton.FlatAppearance.BorderSize = 0;
            refreshButton.Click += RefreshButton_Click;

            searchPanel.Controls.Add(searchLabel);
            searchPanel.Controls.Add(searchTextBox);
            searchPanel.Controls.Add(statusLabel);
            searchPanel.Controls.Add(statusFilter);
            searchPanel.Controls.Add(addSupplierButton);
            searchPanel.Controls.Add(refreshButton);

            this.Controls.Add(searchPanel);
        }

        private void CreateFormPanel()
        {
            formPanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = 350,
                BackColor = Color.White,
                Padding = new Padding(20),
                Visible = false
            };

            var formTitle = new Label
            {
                Text = "Supplier Details",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(0, 0),
                Size = new Size(310, 30)
            };

            // Supplier Name
            var nameLabel = new Label
            {
                Text = "Supplier Name:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(0, 50),
                Size = new Size(310, 20)
            };

            nameTextBox = new TextBox
            {
                Font = new Font("Segoe UI", 11F),
                Location = new Point(0, 70),
                Size = new Size(310, 25)
            };

            // Contact Person
            var contactPersonLabel = new Label
            {
                Text = "Contact Person:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(0, 110),
                Size = new Size(310, 20)
            };

            contactPersonTextBox = new TextBox
            {
                Font = new Font("Segoe UI", 11F),
                Location = new Point(0, 130),
                Size = new Size(310, 25)
            };

            // Phone
            var phoneLabel = new Label
            {
                Text = "Phone:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(0, 170),
                Size = new Size(310, 20)
            };

            phoneTextBox = new TextBox
            {
                Font = new Font("Segoe UI", 11F),
                Location = new Point(0, 190),
                Size = new Size(310, 25)
            };

            // Email
            var emailLabel = new Label
            {
                Text = "Email:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(0, 230),
                Size = new Size(310, 20)
            };

            emailTextBox = new TextBox
            {
                Font = new Font("Segoe UI", 11F),
                Location = new Point(0, 250),
                Size = new Size(310, 25)
            };

            // Address
            var addressLabel = new Label
            {
                Text = "Address:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(0, 290),
                Size = new Size(310, 20)
            };

            addressTextBox = new TextBox
            {
                Font = new Font("Segoe UI", 11F),
                Location = new Point(0, 310),
                Size = new Size(310, 60),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            // Status
            var statusLabel = new Label
            {
                Text = "Status:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(0, 385),
                Size = new Size(310, 20)
            };

            statusComboBox = new ComboBox
            {
                Font = new Font("Segoe UI", 11F),
                Location = new Point(0, 405),
                Size = new Size(310, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            statusComboBox.Items.AddRange(new[] { "Active", "Inactive" });
            statusComboBox.SelectedIndex = 0;

            // Buttons
            saveButton = new Button
            {
                Text = "Save Supplier",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(59, 130, 246),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(0, 450),
                Size = new Size(150, 35),
                Cursor = Cursors.Hand
            };
            saveButton.FlatAppearance.BorderSize = 0;
            saveButton.Click += SaveButton_Click;

            cancelButton = new Button
            {
                Text = "Cancel",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(156, 163, 175),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(160, 450),
                Size = new Size(150, 35),
                Cursor = Cursors.Hand
            };
            cancelButton.FlatAppearance.BorderSize = 0;
            cancelButton.Click += CancelButton_Click;

            formPanel.Controls.Add(formTitle);
            formPanel.Controls.Add(nameLabel);
            formPanel.Controls.Add(nameTextBox);
            formPanel.Controls.Add(contactPersonLabel);
            formPanel.Controls.Add(contactPersonTextBox);
            formPanel.Controls.Add(phoneLabel);
            formPanel.Controls.Add(phoneTextBox);
            formPanel.Controls.Add(emailLabel);
            formPanel.Controls.Add(emailTextBox);
            formPanel.Controls.Add(addressLabel);
            formPanel.Controls.Add(addressTextBox);
            formPanel.Controls.Add(statusLabel);
            formPanel.Controls.Add(statusComboBox);
            formPanel.Controls.Add(saveButton);
            formPanel.Controls.Add(cancelButton);

            this.Controls.Add(formPanel);
        }

        private void CreateSupplierGrid()
        {
            suppliersGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 10F)
            };

            // Configure columns
            suppliersGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Id",
                HeaderText = "ID",
                Width = 60,
                DataPropertyName = "Id",
                Visible = false
            });

            suppliersGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SupplierName",
                HeaderText = "Supplier Name",
                Width = 180,
                DataPropertyName = "SupplierName"
            });

            suppliersGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ContactPerson",
                HeaderText = "Contact Person",
                Width = 150,
                DataPropertyName = "ContactPerson"
            });

            suppliersGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Phone",
                HeaderText = "Phone",
                Width = 120,
                DataPropertyName = "Phone"
            });

            suppliersGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Email",
                HeaderText = "Email",
                Width = 180,
                DataPropertyName = "Email"
            });

            suppliersGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Address",
                HeaderText = "Address",
                Width = 200,
                DataPropertyName = "Address"
            });

            suppliersGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Products",
                HeaderText = "Products",
                Width = 80,
                DataPropertyName = "ProductCount"
            });

            suppliersGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Status",
                HeaderText = "Status",
                Width = 80,
                DataPropertyName = "Status"
            });

            suppliersGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Created",
                HeaderText = "Created",
                Width = 100,
                DataPropertyName = "CreatedDate",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "MM/dd/yyyy" }
            });

            suppliersGridView.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "Edit",
                HeaderText = "Edit",
                Text = "Edit",
                UseColumnTextForButtonValue = true,
                Width = 70
            });

            suppliersGridView.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "Delete",
                HeaderText = "Delete",
                Text = "Delete",
                UseColumnTextForButtonValue = true,
                Width = 70
            });

            suppliersGridView.CellClick += SuppliersGridView_CellClick;

            var gridPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = Color.Transparent
            };
            gridPanel.Controls.Add(suppliersGridView);

            this.Controls.Add(gridPanel);
        }

        private async void LoadData()
        {
            try
            {
                _context = new ApplicationDbContext();
                await LoadSuppliers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadSuppliers()
        {
            if (_context == null) return;

            try
            {
                var query = _context.Suppliers.AsQueryable();

                if (!string.IsNullOrEmpty(searchTextBox.Text))
                {
                    var searchTerm = searchTextBox.Text.ToLower();
                    query = query.Where(s => s.Name.ToLower().Contains(searchTerm) ||
                                           s.ContactPerson.ToLower().Contains(searchTerm) ||
                                           s.Email.ToLower().Contains(searchTerm) ||
                                           s.Phone.Contains(searchTerm));
                }

                var suppliers = await query
                    .Select(s => new
                    {
                        s.Id,
                        SupplierName = s.Name,
                        s.ContactPerson,
                        s.Phone,
                        s.Email,
                        s.Address,
                        ProductCount = _context.Products.Count(p => p.SupplierId == s.Id),
                        Status = s.IsActive ? "Active" : "Inactive",
                        s.CreatedDate
                    })
                    .OrderBy(s => s.SupplierName)
                    .ToListAsync();

                suppliersGridView.DataSource = suppliers;
                supplierCountLabel.Text = $"Total Suppliers: {suppliers.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading suppliers: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            await LoadSuppliers();
        }

        private void AddSupplierButton_Click(object sender, EventArgs e)
        {
            ShowForm(true);
            ClearForm();
            editingSupplierId = 0;
        }

        private async void RefreshButton_Click(object sender, EventArgs e)
        {
            await LoadSuppliers();
        }

        private async void SuppliersGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = suppliersGridView.Rows[e.RowIndex];
            var supplierId = (int)row.Cells["Id"].Value;

            if (e.ColumnIndex == suppliersGridView.Columns["Edit"].Index)
            {
                await EditSupplier(supplierId);
            }
            else if (e.ColumnIndex == suppliersGridView.Columns["Delete"].Index)
            {
                await DeleteSupplier(supplierId);
            }
        }

        private async Task EditSupplier(int supplierId)
        {
            if (_context == null) return;

            try
            {
                var supplier = await _context.Suppliers.FindAsync(supplierId);
                if (supplier != null)
                {
                    editingSupplierId = supplierId;
                    nameTextBox.Text = supplier.Name;
                    contactPersonTextBox.Text = supplier.ContactPerson;
                    phoneTextBox.Text = supplier.Phone;
                    emailTextBox.Text = supplier.Email;
                    addressTextBox.Text = supplier.Address;
                    statusComboBox.SelectedItem = supplier.IsActive ? "Active" : "Inactive";
                    ShowForm(true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading supplier: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task DeleteSupplier(int supplierId)
        {
            if (_context == null) return;

            var result = MessageBox.Show("Are you sure you want to delete this supplier?", 
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    var supplier = await _context.Suppliers.FindAsync(supplierId);
                    if (supplier != null)
                    {
                        _context.Suppliers.Remove(supplier);
                        await _context.SaveChangesAsync();
                        await LoadSuppliers();
                        MessageBox.Show("Supplier deleted successfully!", "Success", 
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

        private async void SaveButton_Click(object sender, EventArgs e)
        {
            if (!ValidateForm()) return;

            if (_context == null) return;

            try
            {
                Supplier supplier;
                
                if (editingSupplierId > 0)
                {
                    supplier = await _context.Suppliers.FindAsync(editingSupplierId);
                    if (supplier == null)
                    {
                        MessageBox.Show("Supplier not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    supplier = new Supplier();
                    supplier.CreatedDate = DateTime.Now;
                    _context.Suppliers.Add(supplier);
                }

                supplier.Name = nameTextBox.Text.Trim();
                supplier.ContactPerson = contactPersonTextBox.Text.Trim();
                supplier.Phone = phoneTextBox.Text.Trim();
                supplier.Email = emailTextBox.Text.Trim();
                supplier.Address = addressTextBox.Text.Trim();
                supplier.IsActive = statusComboBox.SelectedItem?.ToString() == "Active";
                supplier.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                await LoadSuppliers();
                ShowForm(false);
                ClearForm();

                MessageBox.Show($"Supplier {(editingSupplierId > 0 ? "updated" : "created")} successfully!", 
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving supplier: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            ShowForm(false);
            ClearForm();
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(nameTextBox.Text))
            {
                MessageBox.Show("Supplier name is required!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nameTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(contactPersonTextBox.Text))
            {
                MessageBox.Show("Contact person is required!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                contactPersonTextBox.Focus();
                return false;
            }

            if (!string.IsNullOrWhiteSpace(emailTextBox.Text))
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(emailTextBox.Text);
                    if (addr.Address != emailTextBox.Text)
                        throw new FormatException();
                }
                catch
                {
                    MessageBox.Show("Please enter a valid email address!", "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    emailTextBox.Focus();
                    return false;
                }
            }

            return true;
        }

        private void ShowForm(bool visible)
        {
            formPanel.Visible = visible;
        }

        private void ClearForm()
        {
            nameTextBox.Text = "";
            contactPersonTextBox.Text = "";
            phoneTextBox.Text = "";
            emailTextBox.Text = "";
            addressTextBox.Text = "";
            statusComboBox.SelectedIndex = 0;
            editingSupplierId = 0;
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