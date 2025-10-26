using System.Drawing;
using SyncVerseStudio.Services;
using SyncVerseStudio.Data;
using SyncVerseStudio.Models;
using SyncVerseStudio.Helpers;
using Microsoft.EntityFrameworkCore;

namespace SyncVerseStudio.Views
{
    public partial class UserEditForm : Form
    {
        private readonly AuthenticationService _authService;
        private readonly ApplicationDbContext _context;
        private readonly int? _userId;
        private User? _user;

        private TextBox usernameTextBox, firstNameTextBox, lastNameTextBox, emailTextBox, passwordTextBox, confirmPasswordTextBox;
        private ComboBox roleCombo;
        private CheckBox isActiveCheckBox;
        private Button saveButton, cancelButton;
        private Label titleLabel, passwordHintLabel;
        private Panel headerPanel;
        private Button btnClose;
        private Point mouseOffset;

        public UserEditForm(AuthenticationService authService, int? userId = null)
        {
            _authService = authService;
            _context = new ApplicationDbContext();
            _userId = userId;
            
            InitializeComponent();
            
            if (_userId.HasValue)
            {
                LoadUser();
            }
        }

        private void InitializeComponent()
        {
            this.AutoScaleDimensions = new SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new Size(500, 650);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Name = "UserEditForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = _userId.HasValue ? "Edit User" : "Add User";

            CreateHeaderPanel();
            CreateControls();
        }

        private void CreateHeaderPanel()
        {
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = BrandTheme.CoolWhite
            };

            // Make header draggable
            headerPanel.MouseDown += (s, e) =>
            {
                mouseOffset = new Point(-e.X, -e.Y);
            };
            headerPanel.MouseMove += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    Point mousePos = Control.MousePosition;
                    mousePos.Offset(mouseOffset.X, mouseOffset.Y);
                    this.Location = mousePos;
                }
            };

            // Close Button
            btnClose = new Button
            {
                Text = "✕",
                Size = new Size(40, 40),
                Location = new Point(this.ClientSize.Width - 45, 10),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = BrandTheme.SecondaryText,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 240, 240);
            btnClose.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            headerPanel.Controls.Add(btnClose);

            // Title in header
            var headerTitle = new Label
            {
                Text = _userId.HasValue ? "Edit User Account" : "Create New User Account",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = BrandTheme.PrimaryText,
                Location = new Point(20, 15),
                Size = new Size(400, 30),
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(headerTitle);

            this.Controls.Add(headerPanel);
        }

        private void CreateControls()
        {
            int yPos = 80; // Start below header
            int leftMargin = 30;
            int controlWidth = 300;
            int labelHeight = 25;
            int controlHeight = 30;
            int spacing = 15;

            // Username
            AddLabel("Username *", leftMargin, yPos);
            yPos += labelHeight + 5;
            usernameTextBox = new TextBox
            {
                Location = new Point(leftMargin, yPos),
                Size = new Size(controlWidth, controlHeight),
                Font = new System.Drawing.Font("Segoe UI", 10F)
            };
            this.Controls.Add(usernameTextBox);
            yPos += controlHeight + spacing;

            // First Name and Last Name row
            AddLabel("First Name *", leftMargin, yPos);
            AddLabel("Last Name *", leftMargin + 200, yPos);
            yPos += labelHeight + 5;

            firstNameTextBox = new TextBox
            {
                Location = new Point(leftMargin, yPos),
                Size = new Size(150, controlHeight),
                Font = new System.Drawing.Font("Segoe UI", 10F)
            };
            this.Controls.Add(firstNameTextBox);

            lastNameTextBox = new TextBox
            {
                Location = new Point(leftMargin + 200, yPos),
                Size = new Size(150, controlHeight),
                Font = new System.Drawing.Font("Segoe UI", 10F)
            };
            this.Controls.Add(lastNameTextBox);
            yPos += controlHeight + spacing;

            // Email
            AddLabel("Email Address *", leftMargin, yPos);
            yPos += labelHeight + 5;
            emailTextBox = new TextBox
            {
                Location = new Point(leftMargin, yPos),
                Size = new Size(controlWidth, controlHeight),
                Font = new System.Drawing.Font("Segoe UI", 10F)
            };
            this.Controls.Add(emailTextBox);
            yPos += controlHeight + spacing;

            // Role
            AddLabel(" User Role *", leftMargin, yPos);
            yPos += labelHeight + 5;
            roleCombo = new ComboBox
            {
                Location = new Point(leftMargin, yPos),
                Size = new Size(controlWidth, controlHeight),
                Font = new System.Drawing.Font("Segoe UI", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            roleCombo.Items.AddRange(new object[] { 
                "Administrator", 
                "Cashier", 
                "Inventory Clerk" 
            });
            this.Controls.Add(roleCombo);
            yPos += controlHeight + spacing;

            // Password section
            if (!_userId.HasValue) // Only show password fields for new users
            {
                AddLabel("Password *", leftMargin, yPos);
                yPos += labelHeight + 5;
                passwordTextBox = new TextBox
                {
                    Location = new Point(leftMargin, yPos),
                    Size = new Size(controlWidth, controlHeight),
                    Font = new System.Drawing.Font("Segoe UI", 10F),
                    UseSystemPasswordChar = true
                };
                this.Controls.Add(passwordTextBox);
                yPos += controlHeight + 10;

                AddLabel("Confirm Password *", leftMargin, yPos);
                yPos += labelHeight + 5;
                confirmPasswordTextBox = new TextBox
                {
                    Location = new Point(leftMargin, yPos),
                    Size = new Size(controlWidth, controlHeight),
                    Font = new System.Drawing.Font("Segoe UI", 10F),
                    UseSystemPasswordChar = true
                };
                this.Controls.Add(confirmPasswordTextBox);
                yPos += controlHeight + 10;

                passwordHintLabel = new Label
                {
                    Text = "Password must be at least 6 characters long",
                    Font = new System.Drawing.Font("Segoe UI", 8F, FontStyle.Italic),
                    ForeColor = System.Drawing.Color.FromArgb(117, 117, 117),
                    Location = new Point(leftMargin, yPos),
                    Size = new Size(controlWidth, 20)
                };
                this.Controls.Add(passwordHintLabel);
                yPos += 30;
            }

            // Active status
            isActiveCheckBox = new CheckBox
            {
                Text = "? Account is Active",
                Font = new System.Drawing.Font("Segoe UI", 10F),
                ForeColor = System.Drawing.Color.FromArgb(33, 33, 33),
                Location = new Point(leftMargin, yPos),
                Size = new Size(200, 25),
                Checked = true
            };
            this.Controls.Add(isActiveCheckBox);
            yPos += 40;

            // Buttons
            cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(leftMargin + 150, yPos),
                Size = new Size(80, 35),
                BackColor = System.Drawing.Color.FromArgb(158, 158, 158),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new System.Drawing.Font("Segoe UI", 10F, FontStyle.Bold),
                DialogResult = DialogResult.Cancel
            };
            cancelButton.FlatAppearance.BorderSize = 0;
            this.Controls.Add(cancelButton);

            saveButton = new Button
            {
                Text = "Save",
                Location = new Point(leftMargin + 240, yPos),
                Size = new Size(80, 35),
                BackColor = System.Drawing.Color.FromArgb(24, 119, 18),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new System.Drawing.Font("Segoe UI", 10F, FontStyle.Bold)
            };
            saveButton.FlatAppearance.BorderSize = 0;
            saveButton.Click += SaveButton_Click;
            this.Controls.Add(saveButton);

            this.CancelButton = cancelButton;
            this.AcceptButton = saveButton;
        }

        private void AddLabel(string text, int x, int y)
        {
            var label = new Label
            {
                Text = text,
                Font = new System.Drawing.Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = System.Drawing.Color.FromArgb(64, 64, 64),
                Location = new Point(x, y),
                Size = new Size(180, 25)
            };
            this.Controls.Add(label);
        }

        private async void LoadUser()
        {
            try
            {
                _user = await _context.Users.FirstOrDefaultAsync(u => u.Id == _userId);

                if (_user != null)
                {
                    usernameTextBox.Text = _user.Username;
                    firstNameTextBox.Text = _user.FirstName;
                    lastNameTextBox.Text = _user.LastName;
                    emailTextBox.Text = _user.Email;
                    isActiveCheckBox.Checked = _user.IsActive;

                    roleCombo.SelectedIndex = _user.Role switch
                    {
                        UserRole.Administrator => 0,
                        UserRole.Cashier => 1,
                        UserRole.InventoryClerk => 2,
                        _ => 0
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading user: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void SaveButton_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                saveButton.Enabled = false;
                saveButton.Text = "Saving...";

                var role = roleCombo.SelectedIndex switch
                {
                    0 => UserRole.Administrator,
                    1 => UserRole.Cashier,
                    2 => UserRole.InventoryClerk,
                    _ => UserRole.Cashier
                };

                if (_user == null)
                {
                    // Create new user
                    _user = new User
                    {
                        Username = usernameTextBox.Text.Trim(),
                        FirstName = firstNameTextBox.Text.Trim(),
                        LastName = lastNameTextBox.Text.Trim(),
                        Email = emailTextBox.Text.Trim(),
                        Role = role,
                        Password = _authService.HashPassword(passwordTextBox.Text),
                        IsActive = isActiveCheckBox.Checked,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    _context.Users.Add(_user);
                    await _context.SaveChangesAsync();

                    // Log the creation
                    await _authService.LogAuditAsync("USER_CREATED", "Users", _user.Id, null,
                        $"New user '{_user.Username}' created with role {role} by {_authService.CurrentUser?.Username}");

                    MessageBox.Show("User created successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Update existing user
                    var oldValues = $"Username: {_user.Username}, Role: {_user.Role}, Status: {(_user.IsActive ? "Active" : "Inactive")}";
                    
                    _user.Username = usernameTextBox.Text.Trim();
                    _user.FirstName = firstNameTextBox.Text.Trim();
                    _user.LastName = lastNameTextBox.Text.Trim();
                    _user.Email = emailTextBox.Text.Trim();
                    _user.Role = role;
                    _user.IsActive = isActiveCheckBox.Checked;
                    _user.UpdatedAt = DateTime.Now;

                    await _context.SaveChangesAsync();

                    var newValues = $"Username: {_user.Username}, Role: {_user.Role}, Status: {(_user.IsActive ? "Active" : "Inactive")}";

                    // Log the update
                    await _authService.LogAuditAsync("USER_UPDATED", "Users", _user.Id, oldValues, 
                        $"{newValues} - Updated by {_authService.CurrentUser?.Username}");

                    MessageBox.Show("User updated successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving user: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                saveButton.Enabled = true;
                saveButton.Text = " Save";
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(usernameTextBox.Text))
            {
                MessageBox.Show("Username is required.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                usernameTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(firstNameTextBox.Text))
            {
                MessageBox.Show("First name is required.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                firstNameTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(lastNameTextBox.Text))
            {
                MessageBox.Show("Last name is required.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                lastNameTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(emailTextBox.Text) || !emailTextBox.Text.Contains("@"))
            {
                MessageBox.Show("Valid email address is required.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                emailTextBox.Focus();
                return false;
            }

            if (roleCombo.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a user role.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                roleCombo.Focus();
                return false;
            }

            // Password validation for new users
            if (!_userId.HasValue)
            {
                if (string.IsNullOrEmpty(passwordTextBox.Text) || passwordTextBox.Text.Length < 6)
                {
                    MessageBox.Show("Password must be at least 6 characters long.", "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    passwordTextBox.Focus();
                    return false;
                }

                if (passwordTextBox.Text != confirmPasswordTextBox.Text)
                {
                    MessageBox.Show("Passwords do not match.", "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    confirmPasswordTextBox.Focus();
                    return false;
                }
            }

            return true;
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
