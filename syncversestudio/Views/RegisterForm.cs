using System;
using System.Drawing;
using System.Windows.Forms;
using SyncVerseStudio.Helpers;
using SyncVerseStudio.Services;
using SyncVerseStudio.Models;
using SyncVerseStudio.Data;
using Microsoft.EntityFrameworkCore;

namespace SyncVerseStudio.Views
{
    public class RegisterForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtConfirmPassword;
        private TextBox txtEmail;
        private TextBox txtFirstName;
        private TextBox txtLastName;
        private ComboBox cmbRole;
        private Button btnRegister;
        private Button btnCancel;
        private Label lblStatus;
        private CheckBox chkShowPassword;
        private CheckBox chkNoPassword;
        private Panel headerPanel;
        private PictureBox logoPictureBox;
        private Button btnClose;
        private Point mouseOffset;

        public RegisterForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Register - SyncVerse Studio";
            this.Size = new Size(600, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.White;

            // Header Panel
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = BrandTheme.Primary
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
                Text = "X",
                Size = new Size(40, 40),
                Location = new Point(this.Width - 50, 5),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 50, 50);
            btnClose.Click += (s, e) => this.Close();
            headerPanel.Controls.Add(btnClose);

            // Logo
            logoPictureBox = new PictureBox
            {
                Size = new Size(250, 70),
                Location = new Point((this.Width - 250) / 2, 25),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };

            try
            {
                // Try multiple logo paths
                string[] logoPaths = { 
                    "assets\\brand\\logo.png", 
                    "logo.png",
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "brand", "logo.png"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logo.png")
                };

                bool logoLoaded = false;
                foreach (var logoPath in logoPaths)
                {
                    if (System.IO.File.Exists(logoPath))
                    {
                        logoPictureBox.Image = Image.FromFile(logoPath);
                        logoLoaded = true;
                        break;
                    }
                }

                if (!logoLoaded)
                {
                    var placeholderLabel = new Label
                    {
                        Text = "SyncVerse Studio",
                        Font = new Font("Segoe UI", 20, FontStyle.Bold),
                        ForeColor = Color.White,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Dock = DockStyle.Fill
                    };
                    logoPictureBox.Controls.Add(placeholderLabel);
                }
            }
            catch { }

            headerPanel.Controls.Add(logoPictureBox);
            this.Controls.Add(headerPanel);

            // Title
            var titleLabel = new Label
            {
                Text = "Create New Account",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                Location = new Point(50, 135),
                Size = new Size(500, 30),
                ForeColor = BrandTheme.PrimaryText,
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(titleLabel);

            // Scrollable content panel
            var contentPanel = new Panel
            {
                Location = new Point(0, 180),
                Size = new Size(600, 570),
                AutoScroll = true,
                BackColor = Color.White
            };
            this.Controls.Add(contentPanel);

            int yPos = 10;

            // Username
            AddLabelToPanel(contentPanel, "Username *", yPos);
            txtUsername = AddTextBoxToPanel(contentPanel, yPos + 25);
            yPos += 65;

            // Email
            AddLabelToPanel(contentPanel, "Email *", yPos);
            txtEmail = AddTextBoxToPanel(contentPanel, yPos + 25);
            yPos += 65;

            // First Name and Last Name in same row
            var lblFirstName = new Label
            {
                Text = "First Name *",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(50, yPos),
                Size = new Size(240, 20),
                ForeColor = BrandTheme.PrimaryText
            };
            contentPanel.Controls.Add(lblFirstName);

            txtFirstName = new TextBox
            {
                Location = new Point(50, yPos + 25),
                Size = new Size(240, 35),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle
            };
            contentPanel.Controls.Add(txtFirstName);

            var lblLastName = new Label
            {
                Text = "Last Name *",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(310, yPos),
                Size = new Size(240, 20),
                ForeColor = BrandTheme.PrimaryText
            };
            contentPanel.Controls.Add(lblLastName);

            txtLastName = new TextBox
            {
                Location = new Point(310, yPos + 25),
                Size = new Size(240, 35),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle
            };
            contentPanel.Controls.Add(txtLastName);
            yPos += 75;

            // Role
            AddLabelToPanel(contentPanel, "Role *", yPos);
            cmbRole = new ComboBox
            {
                Location = new Point(50, yPos + 25),
                Size = new Size(500, 35),
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbRole.Items.AddRange(new object[] { "Administrator", "Cashier", "InventoryClerk" });
            cmbRole.SelectedIndex = 1; // Default to Cashier
            contentPanel.Controls.Add(cmbRole);
            yPos += 75;

            // Separator line
            var separator = new Panel
            {
                Location = new Point(50, yPos),
                Size = new Size(500, 2),
                BackColor = Color.FromArgb(230, 230, 230)
            };
            contentPanel.Controls.Add(separator);
            yPos += 15;

            // No Password Checkbox
            chkNoPassword = new CheckBox
            {
                Text = "No Password Required (Optional)",
                Location = new Point(50, yPos),
                Size = new Size(250, 25),
                Font = new Font("Segoe UI", 10),
                ForeColor = BrandTheme.SecondaryText
            };
            chkNoPassword.CheckedChanged += ChkNoPassword_CheckedChanged;
            contentPanel.Controls.Add(chkNoPassword);
            yPos += 40;

            // Password
            AddLabelToPanel(contentPanel, "Password", yPos);
            txtPassword = AddTextBoxToPanel(contentPanel, yPos + 25);
            txtPassword.UseSystemPasswordChar = true;
            yPos += 65;

            // Confirm Password
            AddLabelToPanel(contentPanel, "Confirm Password", yPos);
            txtConfirmPassword = AddTextBoxToPanel(contentPanel, yPos + 25);
            txtConfirmPassword.UseSystemPasswordChar = true;
            yPos += 65;

            // Show Password Checkbox
            chkShowPassword = new CheckBox
            {
                Text = "Show Password",
                Location = new Point(50, yPos),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 9),
                ForeColor = BrandTheme.SecondaryText
            };
            chkShowPassword.CheckedChanged += (s, e) =>
            {
                txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
                txtConfirmPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
            };
            contentPanel.Controls.Add(chkShowPassword);
            yPos += 40;

            // Status Label
            lblStatus = new Label
            {
                Location = new Point(50, yPos),
                Size = new Size(500, 25),
                Font = new Font("Segoe UI", 9),
                ForeColor = BrandTheme.Error,
                TextAlign = ContentAlignment.MiddleLeft
            };
            contentPanel.Controls.Add(lblStatus);
            yPos += 35;

            // Register Button
            btnRegister = new Button
            {
                Text = "Create Account",
                Location = new Point(50, yPos),
                Size = new Size(240, 45),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = BrandTheme.Primary,
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.FlatAppearance.MouseOverBackColor = BrandTheme.PrimaryHover;
            btnRegister.Click += BtnRegister_Click;
            contentPanel.Controls.Add(btnRegister);

            // Cancel Button
            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(310, yPos),
                Size = new Size(240, 45),
                Font = new Font("Segoe UI", 11),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = BrandTheme.SecondaryText,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 2;
            btnCancel.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 220);
            btnCancel.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 245, 245);
            btnCancel.Click += (s, e) => this.Close();
            contentPanel.Controls.Add(btnCancel);
        }

        private void ChkNoPassword_CheckedChanged(object sender, EventArgs e)
        {
            bool noPassword = chkNoPassword.Checked;
            txtPassword.Enabled = !noPassword;
            txtConfirmPassword.Enabled = !noPassword;
            chkShowPassword.Enabled = !noPassword;

            if (noPassword)
            {
                txtPassword.Text = "";
                txtConfirmPassword.Text = "";
            }
        }

        private Label AddLabelToPanel(Panel panel, string text, int yPos)
        {
            var label = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(50, yPos),
                Size = new Size(500, 20),
                ForeColor = BrandTheme.PrimaryText
            };
            panel.Controls.Add(label);
            return label;
        }

        private TextBox AddTextBoxToPanel(Panel panel, int yPos)
        {
            var textBox = new TextBox
            {
                Location = new Point(50, yPos),
                Size = new Size(500, 35),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle
            };
            panel.Controls.Add(textBox);
            return textBox;
        }

        private async void BtnRegister_Click(object sender, EventArgs e)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                lblStatus.Text = "Username is required.";
                return;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                lblStatus.Text = "Email is required.";
                return;
            }

            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                lblStatus.Text = "First name is required.";
                return;
            }

            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                lblStatus.Text = "Last name is required.";
                return;
            }

            if (!chkNoPassword.Checked)
            {
                if (string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    lblStatus.Text = "Password is required (or check 'No Password').";
                    return;
                }

                if (txtPassword.Text != txtConfirmPassword.Text)
                {
                    lblStatus.Text = "Passwords do not match.";
                    return;
                }
            }

            try
            {
                btnRegister.Enabled = false;
                lblStatus.Text = "Creating account...";
                lblStatus.ForeColor = BrandTheme.Info;

                using (var context = new ApplicationDbContext())
                {
                    // Ensure database is created
                    try
                    {
                        await context.Database.EnsureCreatedAsync();
                    }
                    catch (Exception dbEx)
                    {
                        lblStatus.Text = $"Database error: {dbEx.Message}";
                        lblStatus.ForeColor = BrandTheme.Error;
                        btnRegister.Enabled = true;
                        MessageBox.Show($"Database connection error. Please check your database settings.\n\nError: {dbEx.Message}", 
                            "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Check if username already exists
                    var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Username == txtUsername.Text.Trim());
                    if (existingUser != null)
                    {
                        lblStatus.Text = "Username already exists.";
                        lblStatus.ForeColor = BrandTheme.Error;
                        btnRegister.Enabled = true;
                        return;
                    }

                    // Create new user
                    var newUser = new User
                    {
                        Username = txtUsername.Text.Trim(),
                        Password = chkNoPassword.Checked ? "" : BCrypt.Net.BCrypt.HashPassword(txtPassword.Text),
                        Email = txtEmail.Text.Trim(),
                        FirstName = txtFirstName.Text.Trim(),
                        LastName = txtLastName.Text.Trim(),
                        Role = (UserRole)Enum.Parse(typeof(UserRole), cmbRole.SelectedItem.ToString()),
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    context.Users.Add(newUser);
                    await context.SaveChangesAsync();

                    MessageBox.Show("Account created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
                lblStatus.ForeColor = BrandTheme.Error;
                btnRegister.Enabled = true;
                MessageBox.Show($"Error creating account:\n{ex.Message}\n\nPlease ensure the database is properly configured.", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
