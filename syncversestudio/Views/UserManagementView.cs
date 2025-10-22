using System.Drawing;
using SyncVerseStudio.Services;
using SyncVerseStudio.Data;
using SyncVerseStudio.Models;
using Microsoft.EntityFrameworkCore;
using FontAwesome.Sharp;

namespace SyncVerseStudio.Views
{
    public partial class UserManagementView : Form
    {
        private readonly AuthenticationService _authService;
        private readonly ApplicationDbContext _context;
        private DataGridView usersGrid;
        private Panel topPanel;
        private IconButton addButton, editButton, deleteButton, refreshButton;
        private TextBox searchBox;
        private ComboBox roleFilter, statusFilter;

        public UserManagementView(AuthenticationService authService)
        {
            _authService = authService;
            _context = new ApplicationDbContext();
            InitializeComponent();
            LoadUsers();
        }

        private void InitializeComponent()
        {
            this.AutoScaleDimensions = new SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(250, 250, 250);
            this.ClientSize = new Size(1200, 800); // Full screen size
            this.FormBorderStyle = FormBorderStyle.None;
            this.Name = "UserManagementView";
            this.Text = "User Management";
            this.Padding = new Padding(0); // No padding for full screen

            CreateTopPanel();
            CreateUsersGrid();
        }

        private void CreateTopPanel()
        {
            topPanel = new Panel
            {
                BackColor = Color.White,
                Dock = DockStyle.Top,
                Height = 90,
                Padding = new Padding(20, 10, 20, 10)
            };

            // Title with icon
            var titleIconPic = new IconPictureBox
            {
                IconChar = IconChar.Users,
                IconColor = Color.FromArgb(24, 119, 18),
                IconSize = 32,
                Location = new Point(20, 15),
                Size = new Size(40, 40),
                BackColor = Color.Transparent
            };
            topPanel.Controls.Add(titleIconPic);

            var titleLabel = new Label
            {
                Text = "User Management",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                Location = new Point(68, 18),
                Size = new Size(300, 35)
            };
            topPanel.Controls.Add(titleLabel);

            // Search box with icon
            var searchIconPic = new IconPictureBox
            {
                IconChar = IconChar.Search,
                IconColor = Color.FromArgb(117, 117, 117),
                IconSize = 16,
                Location = new Point(22, 58),
                Size = new Size(20, 20),
                BackColor = Color.Transparent
            };
            topPanel.Controls.Add(searchIconPic);

            searchBox = new TextBox
            {
                PlaceholderText = "     Search users...",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(20, 55),
                Size = new Size(220, 30)
            };
            searchBox.TextChanged += SearchBox_TextChanged;
            topPanel.Controls.Add(searchBox);

            // Role filter
            roleFilter = new ComboBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(255, 55),
                Size = new Size(160, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            roleFilter.Items.AddRange(new object[] { "All Roles", "Administrator", "Cashier", "InventoryClerk" });
            roleFilter.SelectedIndex = 0;
            roleFilter.SelectedIndexChanged += RoleFilter_SelectedIndexChanged;
            topPanel.Controls.Add(roleFilter);

            // Status filter
            statusFilter = new ComboBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(430, 55),
                Size = new Size(130, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            statusFilter.Items.AddRange(new object[] { "All Status", "Active", "Inactive" });
            statusFilter.SelectedIndex = 0;
            statusFilter.SelectedIndexChanged += StatusFilter_SelectedIndexChanged;
            topPanel.Controls.Add(statusFilter);

                // Buttons without FontAwesome icons
                int buttonX = 590;
            
                addButton = CreateIconButton("Add User", IconChar.None, Color.FromArgb(24, 119, 18), buttonX, 55, 120);
                addButton.Click += AddButton_Click;
                buttonX += 130;

                editButton = CreateIconButton("Edit", IconChar.None, Color.FromArgb(37, 99, 102), buttonX, 55, 90);
                editButton.Click += EditButton_Click;
                buttonX += 100;

                deleteButton = CreateIconButton("Delete", IconChar.None, Color.FromArgb(255, 0, 80), buttonX, 55, 100);
                deleteButton.Click += DeleteButton_Click;
                buttonX += 110;

                refreshButton = CreateIconButton("Refresh", IconChar.None, Color.FromArgb(117, 117, 117), buttonX, 55, 110);
                refreshButton.Click += RefreshButton_Click;

                topPanel.Controls.AddRange(new Control[] {
                    addButton, editButton, deleteButton, refreshButton
                });

                this.Controls.Add(topPanel);
            }

        private IconButton CreateIconButton(string text, IconChar icon, Color backgroundColor, int x, int y, int width)
        {
            return new IconButton
            {
                Text = text,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = backgroundColor,
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(x, y),
                Size = new Size(width, 35),
                Cursor = Cursors.Hand,
                IconChar = IconChar.None,
                TextAlign = ContentAlignment.MiddleCenter,
                Padding = new Padding(10, 0, 10, 0),
                FlatAppearance = { BorderSize = 0 }
            };
        }

        private void CreateUsersGrid()
        {
            // Create a wrapper panel with padding - exact same as CategoryManagementView
            var gridPanel = new Panel
            {
                Dock = DockStyle.Fill,
                //BackColor = Color.FromArgb(226, 244, 255), // PattenBlue - same as CategoryManagementView
                Padding = new Padding(10, 200, 10, 10) // Left, Top, Right, Bottom - same as CategoryManagementView
            };

            usersGrid = new DataGridView
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
                Font = new Font("Segoe UI", 9F), // Same font as CategoryManagementView
                RowHeadersVisible = false,
                GridColor = System.Drawing.Color.FromArgb(230, 230, 230), // Same as CategoryManagementView
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = System.Drawing.Color.White,
                    ForeColor = System.Drawing.Color.FromArgb(33, 33, 33),
                    SelectionBackColor = System.Drawing.Color.FromArgb(24, 119, 18), // Same green as CategoryManagementView
                    SelectionForeColor = System.Drawing.Color.White,
                    Padding = new Padding(5) // Same as CategoryManagementView
                },
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = System.Drawing.Color.FromArgb(245, 245, 245), // Same as CategoryManagementView
                    ForeColor = System.Drawing.Color.FromArgb(33, 33, 33), // Same as CategoryManagementView
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold), // Same as CategoryManagementView
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(5) // Same as CategoryManagementView
                },
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 35 // Same as CategoryManagementView
            };

            // Configure columns - simplified like CategoryManagementView
            usersGrid.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID", Width = 50, Visible = false },
                new DataGridViewTextBoxColumn { Name = "Username", HeaderText = "Username", Width = 150 },
                new DataGridViewTextBoxColumn { Name = "FullName", HeaderText = "Full Name", Width = 200 },
                new DataGridViewTextBoxColumn { Name = "Email", HeaderText = "Email Address", Width = 250 },
                new DataGridViewTextBoxColumn { Name = "Role", HeaderText = "Role", Width = 150 },
                new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Status", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "CreatedAt", HeaderText = "Created", Width = 120 }
            });

            // Format date column - same as CategoryManagementView
            usersGrid.Columns["CreatedAt"].DefaultCellStyle.Format = "MM/dd/yyyy";

            gridPanel.Controls.Add(usersGrid);
            this.Controls.Add(gridPanel);
        }

        private async void LoadUsers()
        {
            try
            {
                var users = await _context.Users
                    .OrderByDescending(u => u.IsActive)
                    .ThenBy(u => u.Username)
                    .ToListAsync();

                usersGrid.Rows.Clear();

                foreach (var user in users)
                {
                    var status = user.IsActive ? "Active" : "Inactive";
                    var role = user.Role switch
                    {
                        UserRole.Administrator => "Administrator",
                        UserRole.Cashier => "Cashier",
                        UserRole.InventoryClerk => "Inventory Clerk",
                        _ => user.Role.ToString()
                    };

                    var rowIndex = usersGrid.Rows.Add(
                        user.Id,
                        user.Username,
                        user.FullName,
                        user.Email,
                        role,
                        status,
                        user.CreatedAt
                    );

                    // Color code inactive users - same style as CategoryManagementView
                    if (!user.IsActive)
                    {
                        usersGrid.Rows[rowIndex].DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(255, 235, 235);
                        usersGrid.Rows[rowIndex].DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(139, 0, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            FilterUsers();
        }

        private void RoleFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterUsers();
        }

        private void StatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterUsers();
        }

        private async void FilterUsers()
        {
            try
            {
                var searchTerm = searchBox.Text.ToLower();
                var selectedRole = roleFilter.SelectedItem?.ToString();
                var selectedStatus = statusFilter.SelectedItem?.ToString();

                var query = _context.Users.AsQueryable();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(u => 
                        u.Username.ToLower().Contains(searchTerm) ||
                        u.FirstName.ToLower().Contains(searchTerm) ||
                        u.LastName.ToLower().Contains(searchTerm) ||
                        u.Email.ToLower().Contains(searchTerm));
                }

                if (!string.IsNullOrEmpty(selectedRole) && selectedRole != "All Roles")
                {
                    var role = selectedRole switch
                    {
                        "Administrator" => UserRole.Administrator,
                        "Cashier" => UserRole.Cashier,
                        "InventoryClerk" => UserRole.InventoryClerk,
                        _ => (UserRole?)null
                    };
                    if (role.HasValue)
                    {
                        query = query.Where(u => u.Role == role.Value);
                    }
                }

                if (!string.IsNullOrEmpty(selectedStatus) && selectedStatus != "All Status")
                {
                    var isActive = selectedStatus == "Active";
                    query = query.Where(u => u.IsActive == isActive);
                }

                var users = await query
                    .OrderByDescending(u => u.IsActive)
                    .ThenBy(u => u.Username)
                    .ToListAsync();

                usersGrid.Rows.Clear();

                foreach (var user in users)
                {
                    var status = user.IsActive ? "Active" : "Inactive";
                    var role = user.Role switch
                    {
                        UserRole.Administrator => "Administrator",
                        UserRole.Cashier => "Cashier",
                        UserRole.InventoryClerk => "Inventory Clerk",
                        _ => user.Role.ToString()
                    };

                    var rowIndex = usersGrid.Rows.Add(
                        user.Id,
                        user.Username,
                        user.FullName,
                        user.Email,
                        role,
                        status,
                        user.CreatedAt
                    );

                    // Color code inactive users - same style as CategoryManagementView
                    if (!user.IsActive)
                    {
                        usersGrid.Rows[rowIndex].DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(255, 235, 235);
                        usersGrid.Rows[rowIndex].DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(139, 0, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error filtering users: {ex.Message}");
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            var addForm = new UserEditForm(_authService);
            if (addForm.ShowDialog() == DialogResult.OK)
            {
                LoadUsers();
            }
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            if (usersGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a user to edit.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var userId = (int)usersGrid.SelectedRows[0].Cells["Id"].Value;
            var editForm = new UserEditForm(_authService, userId);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadUsers();
            }
        }

        private async void DeleteButton_Click(object sender, EventArgs e)
        {
            if (usersGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a user to delete.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var userId = (int)usersGrid.SelectedRows[0].Cells["Id"].Value;
            var username = usersGrid.SelectedRows[0].Cells["Username"].Value.ToString();

            // Prevent deleting current user
            if (userId == _authService.CurrentUser?.Id)
            {
                MessageBox.Show("You cannot delete your own account.", "Invalid Operation", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to deactivate user '{username}'?\n\nThis will set their status to inactive.", 
                "Confirm Deactivate", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    var user = await _context.Users.FindAsync(userId);
                    
                    if (user != null)
                    {
                        var oldStatus = user.IsActive ? "Active" : "Inactive";
                        user.IsActive = false;
                        user.UpdatedAt = DateTime.Now;
                        
                        await _context.SaveChangesAsync();
                        
                        // Log the action
                        await _authService.LogAuditAsync("USER_DEACTIVATED", "Users", userId, 
                            $"Status: {oldStatus}", $"Status: Inactive - User deactivated by {_authService.CurrentUser?.Username}");
                        
                        LoadUsers();
                        
                        MessageBox.Show("User deactivated successfully.", "Success", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deactivating user: {ex.Message}", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            LoadUsers();
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
