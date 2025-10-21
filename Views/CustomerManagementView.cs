using SyncVerseStudio.Services;
using SyncVerseStudio.Models;
using SyncVerseStudio.Data;
using Microsoft.EntityFrameworkCore;

namespace SyncVerseStudio.Views
{
    public partial class CustomerManagementView : Form
    {
        private readonly AuthenticationService _authService;
        private ApplicationDbContext? _context;
        private DataGridView customersGridView;
        private Panel searchPanel;
        private Panel formPanel;
        private TextBox searchTextBox;
        private TextBox firstNameTextBox;
        private TextBox lastNameTextBox;
        private TextBox emailTextBox;
        private TextBox phoneTextBox;
        private TextBox addressTextBox;
        private Button saveButton;
        private Button cancelButton;
        private Button newCustomerButton;
        private int editingCustomerId = 0;
        private Label customerCountLabel;

        public CustomerManagementView(AuthenticationService authService)
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
            this.Name = "CustomerManagementView";
            this.Text = "Customer Management";

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
                Text = "Customer Management",
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(20, 20),
                Size = new Size(400, 40),
                TextAlign = ContentAlignment.MiddleLeft
            };

            customerCountLabel = new Label
            {
                Text = "Total Customers: 0",
                Font = new Font("Segoe UI", 12F, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(450, 25),
                Size = new Size(200, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(customerCountLabel);
            this.Controls.Add(headerPanel);

            // Search Panel
            CreateSearchPanel();

            // Form Panel (Right Side)
            CreateFormPanel();

            // Customer Grid (Left Side)
            CreateCustomerGrid();
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
                Text = "Search Customers:",
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
                PlaceholderText = "Search by name, email, or phone..."
            };
            searchTextBox.TextChanged += SearchTextBox_TextChanged;

            newCustomerButton = new Button
            {
                Text = "Add New Customer",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(34, 197, 94),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(320, 25),
                Size = new Size(150, 30),
                Cursor = Cursors.Hand
            };
            newCustomerButton.FlatAppearance.BorderSize = 0;
            newCustomerButton.Click += NewCustomerButton_Click;

            searchPanel.Controls.Add(searchLabel);
            searchPanel.Controls.Add(searchTextBox);
            searchPanel.Controls.Add(newCustomerButton);

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
                Text = "Customer Details",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(0, 0),
                Size = new Size(310, 30)
            };

            // First Name
            var firstNameLabel = new Label
            {
                Text = "First Name:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(0, 50),
                Size = new Size(310, 20)
            };

            firstNameTextBox = new TextBox
            {
                Font = new Font("Segoe UI", 11F),
                Location = new Point(0, 70),
                Size = new Size(310, 25)
            };

            // Last Name
            var lastNameLabel = new Label
            {
                Text = "Last Name:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(0, 110),
                Size = new Size(310, 20)
            };

            lastNameTextBox = new TextBox
            {
                Font = new Font("Segoe UI", 11F),
                Location = new Point(0, 130),
                Size = new Size(310, 25)
            };

            // Email
            var emailLabel = new Label
            {
                Text = "Email:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(0, 170),
                Size = new Size(310, 20)
            };

            emailTextBox = new TextBox
            {
                Font = new Font("Segoe UI", 11F),
                Location = new Point(0, 190),
                Size = new Size(310, 25)
            };

            // Phone
            var phoneLabel = new Label
            {
                Text = "Phone:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(0, 230),
                Size = new Size(310, 20)
            };

            phoneTextBox = new TextBox
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
                Size = new Size(310, 80),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            // Buttons
            saveButton = new Button
            {
                Text = "Save Customer",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(59, 130, 246),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(0, 410),
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
                Location = new Point(160, 410),
                Size = new Size(150, 35),
                Cursor = Cursors.Hand
            };
            cancelButton.FlatAppearance.BorderSize = 0;
            cancelButton.Click += CancelButton_Click;

            formPanel.Controls.Add(formTitle);
            formPanel.Controls.Add(firstNameLabel);
            formPanel.Controls.Add(firstNameTextBox);
            formPanel.Controls.Add(lastNameLabel);
            formPanel.Controls.Add(lastNameTextBox);
            formPanel.Controls.Add(emailLabel);
            formPanel.Controls.Add(emailTextBox);
            formPanel.Controls.Add(phoneLabel);
            formPanel.Controls.Add(phoneTextBox);
            formPanel.Controls.Add(addressLabel);
            formPanel.Controls.Add(addressTextBox);
            formPanel.Controls.Add(saveButton);
            formPanel.Controls.Add(cancelButton);

            this.Controls.Add(formPanel);
        }

        private void CreateCustomerGrid()
        {
            customersGridView = new DataGridView
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
            customersGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Id",
                HeaderText = "ID",
                Width = 60,
                DataPropertyName = "Id",
                Visible = false
            });

            customersGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FullName",
                HeaderText = "Name",
                Width = 180,
                DataPropertyName = "FullName"
            });

            customersGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Email",
                HeaderText = "Email",
                Width = 200,
                DataPropertyName = "Email"
            });

            customersGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Phone",
                HeaderText = "Phone",
                Width = 130,
                DataPropertyName = "Phone"
            });

            customersGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TotalPurchases",
                HeaderText = "Total Purchases",
                Width = 120,
                DataPropertyName = "TotalPurchases",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
            });

            customersGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "LastPurchase",
                HeaderText = "Last Purchase",
                Width = 120,
                DataPropertyName = "LastPurchase",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "MM/dd/yyyy" }
            });

            customersGridView.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "Edit",
                HeaderText = "Edit",
                Text = "Edit",
                UseColumnTextForButtonValue = true,
                Width = 70
            });

            customersGridView.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "Delete",
                HeaderText = "Delete",
                Text = "Delete",
                UseColumnTextForButtonValue = true,
                Width = 70
            });

            customersGridView.CellClick += CustomersGridView_CellClick;

            var gridPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = Color.Transparent
            };
            gridPanel.Controls.Add(customersGridView);

            this.Controls.Add(gridPanel);
        }

        private async void LoadData()
        {
            try
            {
                _context = new ApplicationDbContext();
                await LoadCustomers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadCustomers()
        {
            if (_context == null) return;

            try
            {
                var query = _context.Customers.AsQueryable();

                if (!string.IsNullOrEmpty(searchTextBox.Text))
                {
                    var searchTerm = searchTextBox.Text.ToLower();
                    query = query.Where(c => c.FirstName.ToLower().Contains(searchTerm) ||
                                           c.LastName.ToLower().Contains(searchTerm) ||
                                           c.Email.ToLower().Contains(searchTerm) ||
                                           c.Phone.Contains(searchTerm));
                }

                var customers = await query
                    .Select(c => new
                    {
                        c.Id,
                        FullName = c.FirstName + " " + c.LastName,
                        c.Email,
                        c.Phone,
                        TotalPurchases = _context.Sales
                            .Where(s => s.CustomerId == c.Id && s.Status == SaleStatus.Completed)
                            .Sum(s => (decimal?)s.TotalAmount) ?? 0,
                        LastPurchase = _context.Sales
                            .Where(s => s.CustomerId == c.Id && s.Status == SaleStatus.Completed)
                            .Max(s => (DateTime?)s.SaleDate)
                    })
                    .OrderBy(c => c.FullName)
                    .ToListAsync();

                customersGridView.DataSource = customers;
                customerCountLabel.Text = $"Total Customers: {customers.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customers: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            await LoadCustomers();
        }

        private void NewCustomerButton_Click(object sender, EventArgs e)
        {
            ShowForm(true);
            ClearForm();
            editingCustomerId = 0;
        }

        private async void CustomersGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = customersGridView.Rows[e.RowIndex];
            var customerId = (int)row.Cells["Id"].Value;

            if (e.ColumnIndex == customersGridView.Columns["Edit"].Index)
            {
                await EditCustomer(customerId);
            }
            else if (e.ColumnIndex == customersGridView.Columns["Delete"].Index)
            {
                await DeleteCustomer(customerId);
            }
        }

        private async Task EditCustomer(int customerId)
        {
            if (_context == null) return;

            try
            {
                var customer = await _context.Customers.FindAsync(customerId);
                if (customer != null)
                {
                    editingCustomerId = customerId;
                    firstNameTextBox.Text = customer.FirstName;
                    lastNameTextBox.Text = customer.LastName;
                    emailTextBox.Text = customer.Email;
                    phoneTextBox.Text = customer.Phone;
                    addressTextBox.Text = customer.Address;
                    ShowForm(true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customer: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task DeleteCustomer(int customerId)
        {
            if (_context == null) return;

            var result = MessageBox.Show("Are you sure you want to delete this customer?", 
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    var customer = await _context.Customers.FindAsync(customerId);
                    if (customer != null)
                    {
                        _context.Customers.Remove(customer);
                        await _context.SaveChangesAsync();
                        await LoadCustomers();
                        MessageBox.Show("Customer deleted successfully!", "Success", 
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

        private async void SaveButton_Click(object sender, EventArgs e)
        {
            if (!ValidateForm()) return;

            if (_context == null) return;

            try
            {
                Customer customer;
                
                if (editingCustomerId > 0)
                {
                    customer = await _context.Customers.FindAsync(editingCustomerId);
                    if (customer == null)
                    {
                        MessageBox.Show("Customer not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    customer = new Customer();
                    customer.CreatedDate = DateTime.Now;
                    _context.Customers.Add(customer);
                }

                customer.FirstName = firstNameTextBox.Text.Trim();
                customer.LastName = lastNameTextBox.Text.Trim();
                customer.Email = emailTextBox.Text.Trim();
                customer.Phone = phoneTextBox.Text.Trim();
                customer.Address = addressTextBox.Text.Trim();
                customer.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                await LoadCustomers();
                ShowForm(false);
                ClearForm();

                MessageBox.Show($"Customer {(editingCustomerId > 0 ? "updated" : "created")} successfully!", 
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving customer: {ex.Message}", "Error", 
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
            if (string.IsNullOrWhiteSpace(firstNameTextBox.Text))
            {
                MessageBox.Show("First name is required!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                firstNameTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(lastNameTextBox.Text))
            {
                MessageBox.Show("Last name is required!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                lastNameTextBox.Focus();
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
            firstNameTextBox.Text = "";
            lastNameTextBox.Text = "";
            emailTextBox.Text = "";
            phoneTextBox.Text = "";
            addressTextBox.Text = "";
            editingCustomerId = 0;
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