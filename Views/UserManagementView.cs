using SyncVerseStudio.Services;
using SyncVerseStudio.Models;
using SyncVerseStudio.Data;
using Microsoft.EntityFrameworkCore;

namespace SyncVerseStudio.Views
{
    public partial class UserManagementView : Form
    {
        private readonly AuthenticationService _authService;
        private ApplicationDbContext? _context;
        private DataGridView usersGridView;
        private Panel searchPanel;
        private Panel formPanel;
        private TextBox searchTextBox;
        private ComboBox roleFilterComboBox;
        private ComboBox statusFilterComboBox;
        private TextBox usernameTextBox;
        private TextBox firstNameTextBox;
        private TextBox lastNameTextBox;
        private TextBox emailTextBox;
        private TextBox passwordTextBox;
        private ComboBox roleComboBox;
        private ComboBox statusComboBox;
        private Button saveButton;
        private Button cancelButton;
        private Button addUserButton;
        private Button refreshButton;
        private int editingUserId = 0;
        private Label userCountLabel;

        public UserManagementView(AuthenticationService authService)
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
            this.Name = "UserManagementView";
            this.Text = "User Management";

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
                Text = "User Management",
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(20, 20),
                Size = new Size(400, 40),
                TextAlign = ContentAlignment.MiddleLeft
            };

            userCountLabel = new Label
            {
                Text = "Total Users: 0",
                Font = new Font("Segoe UI", 12F, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(450, 25),
                Size = new Size(200, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(userCountLabel);
            this.Controls.Add(headerPanel);

            // Search Panel
            CreateSearchPanel();

            // Form Panel (Right Side)
            CreateFormPanel();

            // User Grid (Left Side)
            CreateUserGrid();
        }

        private void CreateSearchPanel()
        {
            searchPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.White,
                Padding = new Padding(20, 10, 20, 10)
            };

            var searchLabel = new Label
            {
                Text = "Search Users:",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(0, 5),
                Size = new Size(150, 25)
            };

            searchTextBox = new TextBox
            {
                Font = new Font("Segoe UI", 11F),
                Location = new Point(0, 25),
                Size = new Size(200, 25),
                PlaceholderText = "Search users..."
            };
            searchTextBox.TextChanged += SearchTextBox_TextChanged;

            var roleLabel = new Label
            {
                Text = "Role:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(220, 5),
                Size = new Size(40, 25)
            };

            roleFilterComboBox = new ComboBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(220, 25),
                Size = new Size(120, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            roleFilterComboBox.Items.AddRange(new[] { "All Roles", "Administrator", "Cashier", "InventoryClerk" });
            roleFilterComboBox.SelectedIndex = 0;
            roleFilterComboBox.SelectedIndexChanged += FilterChanged;

            var statusLabel = new Label
            {
                Text = "Status:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(360, 5),
                Size = new Size(50, 25)
            };

            statusFilterComboBox = new ComboBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(360, 25),
                Size = new Size(100, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            statusFilterComboBox.Items.AddRange(new[] { "All Status", "Active", "Inactive" });
            statusFilterComboBox.SelectedIndex = 0;
            statusFilterComboBox.SelectedIndexChanged += FilterChanged;

            // Text-only buttons with centered text, gray background and black text
            addUserButton = new Button
            {
                Text = "Add",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.Black,
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(480, 25),
                Size = new Size(60, 30),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            addUserButton.FlatAppearance.BorderSize = 0;
            addUserButton.Click += AddUserButton_Click;

            var editButton = new Button
            {
                Text = "Edit",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.Black,
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(550, 25),
                Size = new Size(60, 30),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            editButton.FlatAppearance.BorderSize = 0;
            editButton.Click += EditButton_Click;

            var deleteButton = new Button
            {
                Text = "Delete",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.Black,
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(620, 25),
                Size = new Size(60, 30),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            deleteButton.FlatAppearance.BorderSize = 0;
            deleteButton.Click += DeleteButton_Click;

            refreshButton = new Button
            {
                Text = "Refresh",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.Black,
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(690, 25),
                Size = new Size(70, 30),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            refreshButton.FlatAppearance.BorderSize = 0;
            refreshButton.Click += RefreshButton_Click;

            searchPanel.Controls.Add(searchLabel);
            searchPanel.Controls.Add(searchTextBox);
            searchPanel.Controls.Add(roleLabel);
            searchPanel.Controls.Add(roleFilterComboBox);
            searchPanel.Controls.Add(statusLabel);
            searchPanel.Controls.Add(statusFilterComboBox);
            searchPanel.Controls.Add(addUserButton);
            searchPanel.Controls.Add(editButton);
            searchPanel.Controls.Add(deleteButton);
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
                Text = "User Details",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(0, 0),
                Size = new Size(310, 30)
            };

            // Username
            var usernameLabel = new Label
            {
                Text = "Username:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(0, 50),
                Size = new Size(310, 20)
            };

            usernameTextBox = new TextBox
            {
                Font = new Font("Segoe UI", 11F),
                Location = new Point(0, 70),
                Size = new Size(310, 25)
            };

            // First Name
            var firstNameLabel = new Label
            {
                Text = "First Name:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(0, 110),
                Size = new Size(310, 20)
            };

            firstNameTextBox = new TextBox
            {
                Font = new Font("Segoe UI", 11F),
                Location = new Point(0, 130),
                Size = new Size(310, 25)
            };

            // Last Name
            var lastNameLabel = new Label
            {
                Text = "Last Name:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(0, 170),
                Size = new Size(310, 20)
            };

            lastNameTextBox = new TextBox
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

            // Password
            var passwordLabel = new Label
            {
                Text = "Password:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(0, 290),
                Size = new Size(310, 20)
            };

            passwordTextBox = new TextBox
            {
                Font = new Font("Segoe UI", 11F),
                Location = new Point(0, 310),
                Size = new Size(310, 25),
                UseSystemPasswordChar = true
            };

            // Role
            var roleLabel = new Label
            {
                Text = "Role:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(0, 350),
                Size = new Size(310, 20)
            };

            roleComboBox = new ComboBox
            {
                Font = new Font("Segoe UI", 11F),
                Location = new Point(0, 370),
                Size = new Size(310, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            roleComboBox.Items.AddRange(new[] { "Administrator", "Cashier", "InventoryClerk" });
            roleComboBox.SelectedIndex = 0;

            // Status
            var statusLabel = new Label
            {
                Text = "Status:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(0, 410),
                Size = new Size(310, 20)
            };

            statusComboBox = new ComboBox
            {
                Font = new Font("Segoe UI", 11F),
                Location = new Point(0, 430),
                Size = new Size(310, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            statusComboBox.Items.AddRange(new[] { "Active", "Inactive" });
            statusComboBox.SelectedIndex = 0;

            // Buttons with centered text
            saveButton = new Button
            {
                Text = "Save User",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(59, 130, 246),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(0, 480),
                Size = new Size(150, 35),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
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
                Location = new Point(160, 480),
                Size = new Size(150, 35),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            cancelButton.FlatAppearance.BorderSize = 0;
            cancelButton.Click += CancelButton_Click;

            formPanel.Controls.Add(formTitle);
            formPanel.Controls.Add(usernameLabel);
            formPanel.Controls.Add(usernameTextBox);
            formPanel.Controls.Add(firstNameLabel);
            formPanel.Controls.Add(firstNameTextBox);
            formPanel.Controls.Add(lastNameLabel);
            formPanel.Controls.Add(lastNameTextBox);
            formPanel.Controls.Add(emailLabel);
            formPanel.Controls.Add(emailTextBox);
            formPanel.Controls.Add(passwordLabel);
            formPanel.Controls.Add(passwordTextBox);
            formPanel.Controls.Add(roleLabel);
            formPanel.Controls.Add(roleComboBox);
            formPanel.Controls.Add(statusLabel);
            formPanel.Controls.Add(statusComboBox);
            formPanel.Controls.Add(saveButton);
            formPanel.Controls.Add(cancelButton);

            this.Controls.Add(formPanel);
        }

        private void CreateUserGrid()
        {
            usersGridView = new DataGridView
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
            usersGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Id",
                HeaderText = "ID",
                Width = 60,
                DataPropertyName = "Id",
                Visible = false
            });

            usersGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Username",
                HeaderText = "Username",
                Width = 120,
                DataPropertyName = "Username"
            });

            usersGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FullName",
                HeaderText = "Full Name",
                Width = 180,
                DataPropertyName = "FullName"
            });

            usersGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Email",
                HeaderText = "Email Address",
                Width = 200,
                DataPropertyName = "Email"
            });

            usersGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Role",
                HeaderText = "Role",
                Width = 120,
                DataPropertyName = "Role"
            });

            usersGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Status",
                HeaderText = "Status",
                Width = 80,
                DataPropertyName = "Status"
            });

            usersGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Created",
                HeaderText = "Created",
                Width = 100,
                DataPropertyName = "CreatedAt",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "MM/dd/yyyy" }
            });

            var gridPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = Color.Transparent
            };
            gridPanel.Controls.Add(usersGridView);

            this.Controls.Add(gridPanel);
        }

        private async void LoadData()
        {
            try
            {
                _context = new ApplicationDbContext();
                await LoadUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadUsers()
        {
            if (_context == null) return;

            try
            {
                var query = _context.Users.AsQueryable();

                // Apply search filter
                if (!string.IsNullOrEmpty(searchTextBox.Text))
                {
                    var searchTerm = searchTextBox.Text.ToLower();
                    query = query.Where(u => u.Username.ToLower().Contains(searchTerm) ||
                                           u.FirstName.ToLower().Contains(searchTerm) ||
                                           u.LastName.ToLower().Contains(searchTerm) ||
                                           u.Email.ToLower().Contains(searchTerm));
                }

                // Apply role filter
                if (roleFilterComboBox.SelectedItem?.ToString() != "All Roles")
                {
                    if (Enum.TryParse<UserRole>(roleFilterComboBox.SelectedItem?.ToString(), out var role))
                    {
                        query = query.Where(u => u.Role == role);
                    }
                }

                // Apply status filter
                if (statusFilterComboBox.SelectedItem?.ToString() == "Active")
                {
                    query = query.Where(u => u.IsActive);
                }
                else if (statusFilterComboBox.SelectedItem?.ToString() == "Inactive")
                {
                    query = query.Where(u => !u.IsActive);
                }

                var users = await query
                    .Select(u => new
                    {
                        u.Id,
                        u.Username,
                        FullName = u.FirstName + " " + u.LastName,
                        u.Email,
                        Role = u.Role.ToString(),
                        Status = u.IsActive ? "Active" : "Inactive",
                        u.CreatedAt
                    })
                    .OrderBy(u => u.Username)
                    .ToListAsync();

                usersGridView.DataSource = users;
                userCountLabel.Text = $"Total Users: {users.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            await LoadUsers();
        }

        private async void FilterChanged(object sender, EventArgs e)
        {
            await LoadUsers();
        }

        private void AddUserButton_Click(object sender, EventArgs e)
        {
            ShowForm(true);
            ClearForm();
            editingUserId = 0;
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            if (usersGridView.SelectedRows.Count > 0)
            {
                var selectedRow = usersGridView.SelectedRows[0];
                var userId = (int)selectedRow.Cells["Id"].Value;
                _ = EditUser(userId);
            }
            else
            {
                MessageBox.Show("Please select a user to edit.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (usersGridView.SelectedRows.Count > 0)
            {
                var selectedRow = usersGridView.SelectedRows[0];
                var userId = (int)selectedRow.Cells["Id"].Value;
                _ = DeleteUser(userId);
            }
            else
            {
                MessageBox.Show("Please select a user to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void RefreshButton_Click(object sender, EventArgs e)
        {
            await LoadUsers();
        }

        private async Task EditUser(int userId)
        {
            if (_context == null) return;

            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    editingUserId = userId;
                    usernameTextBox.Text = user.Username;
                    firstNameTextBox.Text = user.FirstName;
                    lastNameTextBox.Text = user.LastName;
                    emailTextBox.Text = user.Email;
                    passwordTextBox.Text = ""; // Don't show existing password
                    roleComboBox.SelectedItem = user.Role.ToString();
                    statusComboBox.SelectedItem = user.IsActive ? "Active" : "Inactive";
                    ShowForm(true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading user: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task DeleteUser(int userId)
        {
            if (_context == null) return;

            var result = MessageBox.Show("Are you sure you want to delete this user?", 
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    var user = await _context.Users.FindAsync(userId);
                    if (user != null)
                    {
                        _context.Users.Remove(user);
                        await _context.SaveChangesAsync();
                        await LoadUsers();
                        MessageBox.Show("User deleted successfully!", "Success", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting user: {ex.Message}", "Error", 
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
                User user;
                
                if (editingUserId > 0)
                {
                    user = await _context.Users.FindAsync(editingUserId);
                    if (user == null)
                    {
                        MessageBox.Show("User not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    user = new User();
                    user.CreatedAt = DateTime.Now;
                    _context.Users.Add(user);
                }

                user.Username = usernameTextBox.Text.Trim();
                user.FirstName = firstNameTextBox.Text.Trim();
                user.LastName = lastNameTextBox.Text.Trim();
                user.Email = emailTextBox.Text.Trim();
                
                // Only update password if it's provided
                if (!string.IsNullOrEmpty(passwordTextBox.Text))
                {
                    user.Password = _authService.HashPassword(passwordTextBox.Text);
                }
                
                user.Role = Enum.Parse<UserRole>(roleComboBox.SelectedItem?.ToString() ?? "Cashier");
                user.IsActive = statusComboBox.SelectedItem?.ToString() == "Active";
                user.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                await LoadUsers();
                ShowForm(false);
                ClearForm();

                MessageBox.Show($"User {(editingUserId > 0 ? "updated" : "created")} successfully!", 
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving user: {ex.Message}", "Error", 
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
            if (string.IsNullOrWhiteSpace(usernameTextBox.Text))
            {
                MessageBox.Show("Username is required!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                usernameTextBox.Focus();
                return false;
            }

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

            if (string.IsNullOrWhiteSpace(emailTextBox.Text))
            {
                MessageBox.Show("Email is required!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                emailTextBox.Focus();
                return false;
            }

            if (editingUserId == 0 && string.IsNullOrWhiteSpace(passwordTextBox.Text))
            {
                MessageBox.Show("Password is required for new users!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                passwordTextBox.Focus();
                return false;
            }

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

            return true;
        }

        private void ShowForm(bool visible)
        {
            formPanel.Visible = visible;
        }

        private void ClearForm()
        {
            usernameTextBox.Text = "";
            firstNameTextBox.Text = "";
            lastNameTextBox.Text = "";
            emailTextBox.Text = "";
            passwordTextBox.Text = "";
            roleComboBox.SelectedIndex = 0;
            statusComboBox.SelectedIndex = 0;
            editingUserId = 0;
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