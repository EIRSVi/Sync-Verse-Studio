using System;
using System.Drawing;
using System.Windows.Forms;
using SyncVerseStudio.Helpers;
using SyncVerseStudio.Services;
using System.IO;
using FontAwesome.Sharp;

namespace SyncVerseStudio.Views
{
    public class LoginForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private IconButton btnLogin;
        private IconButton btnRegister;
        private IconButton btnChangeDatabaseConnection;
        private Label lblStatus;
        private Panel headerPanel;
        private PictureBox logoPictureBox;
        private CheckBox chkShowPassword;
        private Button btnClose;
        private Point mouseOffset;

        public LoginForm()
        {
            InitializeComponents();
            CheckDatabaseConnection();
        }

        private void InitializeComponents()
        {
            this.Text = "Login - SyncVerse Studio";
            this.Size = new Size(550, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = BrandTheme.CoolWhite; // #D7E8FA

            // Header Panel
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 250,
                BackColor = BrandTheme.CoolWhite // #D7E8FA
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
                Location = new Point(this.ClientSize.Width - 45, 5),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 50, 50);
            btnClose.Click += (s, e) => Application.Exit();
            headerPanel.Controls.Add(btnClose);

            // Logo - Centered and larger
            logoPictureBox = new PictureBox
            {
                Size = new Size(400, 120),
                Location = new Point((this.ClientSize.Width - 400) / 2, 60),
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
                    if (File.Exists(logoPath))
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
                        Font = new Font("Segoe UI", 22, FontStyle.Bold),
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

            // Username Label - No title/subtitle, minimalist design
            var lblUsername = new Label
            {
                Text = "Username",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(75, 270),
                Size = new Size(400, 22),
                ForeColor = BrandTheme.PrimaryText
            };
            this.Controls.Add(lblUsername);

            // Username TextBox
            txtUsername = new TextBox
            {
                Location = new Point(75, 295),
                Size = new Size(400, 35),
                Font = new Font("Segoe UI", 12),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
            this.Controls.Add(txtUsername);

            // Password Label
            var lblPassword = new Label
            {
                Text = "Password",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(75, 345),
                Size = new Size(400, 22),
                ForeColor = BrandTheme.PrimaryText
            };
            this.Controls.Add(lblPassword);

            // Password TextBox
            txtPassword = new TextBox
            {
                Location = new Point(75, 370),
                Size = new Size(400, 35),
                Font = new Font("Segoe UI", 12),
                UseSystemPasswordChar = true,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
            this.Controls.Add(txtPassword);

            // Show Password Checkbox
            chkShowPassword = new CheckBox
            {
                Text = "Show Password",
                Location = new Point(75, 415),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 9),
                ForeColor = BrandTheme.PrimaryText,
                BackColor = BrandTheme.CoolWhite
            };
            chkShowPassword.CheckedChanged += (s, e) => txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
            this.Controls.Add(chkShowPassword);

            // Status Label
            lblStatus = new Label
            {
                Location = new Point(75, 450),
                Size = new Size(400, 25),
                Font = new Font("Segoe UI", 9),
                ForeColor = BrandTheme.Error,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = BrandTheme.CoolWhite
            };
            this.Controls.Add(lblStatus);

            // Login Button with icon
            btnLogin = new IconButton
            {
                Text = "  Sign In",
                IconChar = IconChar.SignInAlt,
                IconColor = Color.White,
                IconSize = 20,
                Location = new Point(75, 485),
                Size = new Size(400, 50),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = BrandTheme.Primary,
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleCenter,
                TextImageRelation = TextImageRelation.ImageBeforeText
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.FlatAppearance.MouseOverBackColor = BrandTheme.PrimaryHover;
            btnLogin.Click += BtnLogin_Click;
            this.Controls.Add(btnLogin);

            // Register Button with icon
            btnRegister = new IconButton
            {
                Text = "  Create Account",
                IconChar = IconChar.UserPlus,
                IconColor = BrandTheme.Primary,
                IconSize = 18,
                Location = new Point(75, 545),
                Size = new Size(195, 40),
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = BrandTheme.Primary,
                Cursor = Cursors.Hand,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleCenter,
                TextImageRelation = TextImageRelation.ImageBeforeText
            };
            btnRegister.FlatAppearance.BorderSize = 2;
            btnRegister.FlatAppearance.BorderColor = BrandTheme.Primary;
            btnRegister.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 248, 255);
            btnRegister.Click += BtnRegister_Click;
            this.Controls.Add(btnRegister);

            // Change Database Connection Button with icon
            btnChangeDatabaseConnection = new IconButton
            {
                Text = "  Database",
                IconChar = IconChar.Database,
                IconColor = BrandTheme.Primary,
                IconSize = 18,
                Location = new Point(280, 545),
                Size = new Size(195, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = BrandTheme.Primary,
                Font = new Font("Segoe UI", 10),
                Cursor = Cursors.Hand,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleCenter,
                TextImageRelation = TextImageRelation.ImageBeforeText
            };
            btnChangeDatabaseConnection.FlatAppearance.BorderSize = 2;
            btnChangeDatabaseConnection.FlatAppearance.BorderColor = BrandTheme.Primary;
            btnChangeDatabaseConnection.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 248, 255);
            btnChangeDatabaseConnection.Click += BtnChangeDatabaseConnection_Click;
            this.Controls.Add(btnChangeDatabaseConnection);

            // Enter key support
            txtPassword.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    BtnLogin_Click(s, e);
                }
            };
        }

        private void CheckDatabaseConnection()
        {
            if (!File.Exists("dbconfig.json"))
            {
                ShowDatabaseConnectionForm();
            }
        }

        private void ShowDatabaseConnectionForm()
        {
            using (var dbForm = new DatabaseConnectionForm())
            {
                if (dbForm.ShowDialog() == DialogResult.OK)
                {
                    DatabaseConnectionManager.SetConnectionString(dbForm.ConnectionString);
                }
                else
                {
                    MessageBox.Show("Database connection is required to use the application.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
            }
        }

        private void BtnChangeDatabaseConnection_Click(object sender, EventArgs e)
        {
            ShowDatabaseConnectionForm();
        }

        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            var username = txtUsername.Text.Trim();
            var password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                lblStatus.Text = "Please enter username and password.";
                return;
            }

            try
            {
                btnLogin.Enabled = false;
                lblStatus.Text = "Logging in...";
                lblStatus.ForeColor = BrandTheme.Info;

                var authService = new AuthenticationService();
                var success = await authService.LoginAsync(username, password);

                if (success)
                {
                    this.Hide();
                    var dashboard = new MainDashboard(authService);
                    dashboard.FormClosed += (s, args) => this.Close();
                    dashboard.Show();
                }
                else
                {
                    lblStatus.Text = "Invalid username or password.";
                    lblStatus.ForeColor = BrandTheme.Error;
                    btnLogin.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
                lblStatus.ForeColor = BrandTheme.Error;
                btnLogin.Enabled = true;
            }
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            var registerForm = new RegisterForm();
            registerForm.ShowDialog();
        }
    }
}
