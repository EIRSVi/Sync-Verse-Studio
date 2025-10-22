using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using FontAwesome.Sharp;
using SyncVerseStudio.Helpers;

namespace SyncVerseStudio
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;
        private Panel mainPanel;
        private Panel leftPanel;
        private Panel rightPanel;
        private PictureBox logoPictureBox;
        private Label brandLabel;
        private Label taglineLabel;
        private Label welcomeLabel;
        private Label usernameLabel;
        private TextBox usernameTextBox;
        private Label passwordLabel;
        private TextBox passwordTextBox;
        private Button loginButton;
        private Label errorLabel;
        private IconPictureBox userIcon;
        private IconPictureBox lockIcon;

        private void InitializeComponent()
        {
            this.mainPanel = new Panel();
            this.leftPanel = new Panel();
            this.rightPanel = new Panel();
            this.logoPictureBox = new PictureBox();
            this.brandLabel = new Label();
            this.taglineLabel = new Label();
            this.welcomeLabel = new Label();
            this.usernameLabel = new Label();
            this.usernameTextBox = new TextBox();
            this.passwordLabel = new Label();
            this.passwordTextBox = new TextBox();
            this.loginButton = new Button();
            this.errorLabel = new Label();
            this.userIcon = new IconPictureBox();
            this.lockIcon = new IconPictureBox();

            this.SuspendLayout();

            // 
            // MainForm (LoginForm) - Creative Modern Design
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Name = "LoginForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = BrandTheme.BrandName + " - Login";
            this.BackColor = BrandTheme.CoolWhite;
            this.WindowState = FormWindowState.Normal;
            this.DoubleBuffered = true;

            // 
            // mainPanel - Full screen container
            // 
            this.mainPanel.Dock = DockStyle.Fill;
            this.mainPanel.BackColor = BrandTheme.CoolWhite;

            // 
            // leftPanel - Centered login card
            // 
            this.leftPanel.Size = new Size(450, 600);
            this.leftPanel.Location = new Point(375, 50);
            this.leftPanel.BackColor = Color.White;
            this.leftPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                var rect = this.leftPanel.ClientRectangle;
                
                // Shadow
                using (var shadowPath = GetRoundedRectPath(new Rectangle(8, 8, rect.Width - 2, rect.Height - 2), 25))
                using (var shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
                {
                    e.Graphics.FillPath(shadowBrush, shadowPath);
                }
                
                // Card
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, rect.Width - 1, rect.Height - 1), 25))
                using (var brush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillPath(brush, path);
                }
            };

            // 
            // logoPictureBox - Centered logo
            // 
            this.logoPictureBox.Size = new System.Drawing.Size(150, 150);
            this.logoPictureBox.Location = new System.Drawing.Point(150, 40);
            this.logoPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            this.logoPictureBox.BackColor = Color.Transparent;

            // Load logo from URL
            Task.Run(async () =>
            {
                try
                {
                    using (var client = new System.Net.Http.HttpClient())
                    {
                        var imageBytes = await client.GetByteArrayAsync(BrandTheme.LogoUrlPng);
                        using (var ms = new System.IO.MemoryStream(imageBytes))
                        {
                            var image = Image.FromStream(ms);
                            this.Invoke(new Action(() =>
                            {
                                if (this.logoPictureBox != null && !this.logoPictureBox.IsDisposed)
                                {
                                    this.logoPictureBox.Image = image;
                                }
                            }));
                        }
                    }
                }
                catch
                {
                    this.Invoke(new Action(() =>
                    {
                        try
                        {
                            var logoPath = Path.Combine(Application.StartupPath, "..", "..", "..", BrandTheme.LogoPath);
                            if (File.Exists(logoPath))
                            {
                                this.logoPictureBox.Image = Image.FromFile(logoPath);
                            }
                        }
                        catch { }
                    }));
                }
            });

            // 
            // brandLabel - Brand Name
            // 
            this.brandLabel.Text = BrandTheme.BrandName;
            this.brandLabel.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            this.brandLabel.ForeColor = BrandTheme.DarkGray;
            this.brandLabel.Location = new Point(0, 200);
            this.brandLabel.Size = new Size(450, 40);
            this.brandLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.brandLabel.BackColor = Color.Transparent;

            // 
            // taglineLabel - Brand Tagline
            // 
            this.taglineLabel.Text = BrandTheme.BrandTagline;
            this.taglineLabel.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
            this.taglineLabel.ForeColor = BrandTheme.SecondaryText;
            this.taglineLabel.Location = new Point(0, 245);
            this.taglineLabel.Size = new Size(450, 25);
            this.taglineLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.taglineLabel.BackColor = Color.Transparent;

            // 
            // rightPanel - Not used in centered design
            // 
            this.rightPanel.Visible = false;

            // 
            // welcomeLabel
            // 
            this.welcomeLabel.Text = "Welcome";
            this.welcomeLabel.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            this.welcomeLabel.ForeColor = BrandTheme.DarkGray;
            this.welcomeLabel.Location = new Point(0, 290);
            this.welcomeLabel.Size = new Size(450, 30);
            this.welcomeLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.welcomeLabel.BackColor = Color.Transparent;

            // 
            // userIcon
            // 
            this.userIcon.IconChar = IconChar.User;
            this.userIcon.IconColor = BrandTheme.MediumBlue;
            this.userIcon.IconSize = 24;
            this.userIcon.Size = new Size(24, 24);
            this.userIcon.BackColor = Color.Transparent;

            // 
            // usernameLabel
            // 
            this.usernameLabel.Text = "Username";
            this.usernameLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.usernameLabel.ForeColor = BrandTheme.PrimaryText;
            this.usernameLabel.Location = new Point(50, 340);
            this.usernameLabel.Size = new Size(350, 20);
            this.usernameLabel.BackColor = Color.Transparent;

            // 
            // usernameTextBox
            // 
            this.usernameTextBox.Font = new Font("Segoe UI", 12F);
            this.usernameTextBox.Location = new Point(40, 8);
            this.usernameTextBox.Size = new Size(310, 35);
            this.usernameTextBox.BorderStyle = BorderStyle.None;
            this.usernameTextBox.BackColor = BrandTheme.CoolWhite;
            this.usernameTextBox.ForeColor = BrandTheme.PrimaryText;
            this.usernameTextBox.Text = "vi";

            // Custom border
            var usernamePanel = new Panel
            {
                Location = new Point(50, 365),
                Size = new Size(350, 50),
                BackColor = BrandTheme.CoolWhite
            };
            usernamePanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                var rect = usernamePanel.ClientRectangle;
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, rect.Width - 1, rect.Height - 1), 10))
                using (var pen = new Pen(Color.FromArgb(200, 210, 220), 2))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            };
            
            this.userIcon.Location = new Point(10, 13);
            this.userIcon.IconColor = BrandTheme.MediumBlue;
            usernamePanel.Controls.Add(this.userIcon);
            usernamePanel.Controls.Add(this.usernameTextBox);

            // 
            // lockIcon
            // 
            this.lockIcon.IconChar = IconChar.Lock;
            this.lockIcon.IconColor = BrandTheme.MediumBlue;
            this.lockIcon.IconSize = 24;
            this.lockIcon.Size = new Size(24, 24);
            this.lockIcon.BackColor = Color.Transparent;

            // 
            // passwordLabel
            // 
            this.passwordLabel.Text = "Password";
            this.passwordLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.passwordLabel.ForeColor = BrandTheme.PrimaryText;
            this.passwordLabel.Location = new Point(50, 430);
            this.passwordLabel.Size = new Size(350, 20);
            this.passwordLabel.BackColor = Color.Transparent;

            // 
            // passwordTextBox
            // 
            this.passwordTextBox.Font = new Font("Segoe UI", 12F);
            this.passwordTextBox.Location = new Point(40, 8);
            this.passwordTextBox.Size = new Size(310, 35);
            this.passwordTextBox.BorderStyle = BorderStyle.None;
            this.passwordTextBox.BackColor = BrandTheme.CoolWhite;
            this.passwordTextBox.ForeColor = BrandTheme.PrimaryText;
            this.passwordTextBox.UseSystemPasswordChar = true;
            this.passwordTextBox.Text = "vi";

            // Custom border
            var passwordPanel = new Panel
            {
                Location = new Point(50, 455),
                Size = new Size(350, 50),
                BackColor = BrandTheme.CoolWhite
            };
            passwordPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                var rect = passwordPanel.ClientRectangle;
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, rect.Width - 1, rect.Height - 1), 10))
                using (var pen = new Pen(Color.FromArgb(200, 210, 220), 2))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            };
            
            this.lockIcon.Location = new Point(10, 13);
            this.lockIcon.IconColor = BrandTheme.MediumBlue;
            passwordPanel.Controls.Add(this.lockIcon);
            passwordPanel.Controls.Add(this.passwordTextBox);

            // 
            // loginButton
            // 
            this.loginButton.Text = "SIGN IN";
            this.loginButton.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            this.loginButton.BackColor = BrandTheme.MediumBlue;
            this.loginButton.ForeColor = Color.White;
            this.loginButton.FlatStyle = FlatStyle.Flat;
            this.loginButton.FlatAppearance.BorderSize = 0;
            this.loginButton.Location = new Point(50, 525);
            this.loginButton.Size = new Size(350, 50);
            this.loginButton.Cursor = Cursors.Hand;
            this.loginButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(45, 25, 230);
            
            // Rounded button
            var buttonRegion = new System.Drawing.Drawing2D.GraphicsPath();
            buttonRegion.AddArc(0, 0, 10, 10, 180, 90);
            buttonRegion.AddArc(this.loginButton.Width - 10, 0, 10, 10, 270, 90);
            buttonRegion.AddArc(this.loginButton.Width - 10, this.loginButton.Height - 10, 10, 10, 0, 90);
            buttonRegion.AddArc(0, this.loginButton.Height - 10, 10, 10, 90, 90);
            buttonRegion.CloseFigure();

            // 
            // errorLabel
            // 
            this.errorLabel.Text = "";
            this.errorLabel.Font = new Font("Segoe UI", 9F);
            this.errorLabel.ForeColor = Color.FromArgb(220, 60, 60);
            this.errorLabel.Location = new Point(50, 580);
            this.errorLabel.Size = new Size(350, 20);
            this.errorLabel.TextAlign = ContentAlignment.TopCenter;
            this.errorLabel.BackColor = Color.Transparent;

            // Add controls to card panel
            this.leftPanel.Controls.Add(this.logoPictureBox);
            this.leftPanel.Controls.Add(this.brandLabel);
            this.leftPanel.Controls.Add(this.taglineLabel);
            this.leftPanel.Controls.Add(this.welcomeLabel);
            this.leftPanel.Controls.Add(this.usernameLabel);
            this.leftPanel.Controls.Add(usernamePanel);
            this.leftPanel.Controls.Add(this.passwordLabel);
            this.leftPanel.Controls.Add(passwordPanel);
            this.leftPanel.Controls.Add(this.loginButton);
            this.leftPanel.Controls.Add(this.errorLabel);

            this.mainPanel.Controls.Add(this.leftPanel);
            this.Controls.Add(this.mainPanel);

            // Add focus effects
            this.usernameTextBox.Enter += (s, e) =>
            {
                ((TextBox)s).BackColor = Color.White;
                usernamePanel.BackColor = Color.White;
                usernamePanel.Invalidate();
            };
            this.usernameTextBox.Leave += (s, e) =>
            {
                ((TextBox)s).BackColor = BrandTheme.CoolWhite;
                usernamePanel.BackColor = BrandTheme.CoolWhite;
                usernamePanel.Invalidate();
            };
            this.passwordTextBox.Enter += (s, e) =>
            {
                ((TextBox)s).BackColor = Color.White;
                passwordPanel.BackColor = Color.White;
                passwordPanel.Invalidate();
            };
            this.passwordTextBox.Leave += (s, e) =>
            {
                ((TextBox)s).BackColor = BrandTheme.CoolWhite;
                passwordPanel.BackColor = BrandTheme.CoolWhite;
                passwordPanel.Invalidate();
            };

            this.ResumeLayout(false);
        }

        private System.Drawing.Drawing2D.GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
