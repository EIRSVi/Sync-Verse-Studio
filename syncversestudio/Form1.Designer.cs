using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using FontAwesome.Sharp;

namespace SyncVerseStudio
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;
      private Panel mainPanel;
        private Panel loginPanel;
 private PictureBox logoPictureBox;
        private Label titleLabel;
        private Label subtitleLabel;
        private Label usernameLabel;
        private TextBox usernameTextBox;
 private Label passwordLabel;
        private TextBox passwordTextBox;
        private Button loginButton;
        private Label errorLabel;

        private void InitializeComponent()
        {
        this.mainPanel = new Panel();
          this.loginPanel = new Panel();
       this.logoPictureBox = new PictureBox();
      this.titleLabel = new Label();
       this.subtitleLabel = new Label();
   this.usernameLabel = new Label();
   this.usernameTextBox = new TextBox();
         this.passwordLabel = new Label();
      this.passwordTextBox = new TextBox();
          this.loginButton = new Button();
        this.errorLabel = new Label();

 this.SuspendLayout();

       // 
      // MainForm (LoginForm) - Modern dark theme optimized for 16:9
// 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(1366, 768); // Standard 16:9 resolution
    this.FormBorderStyle = FormBorderStyle.None;
      this.Name = "LoginForm";
 this.StartPosition = FormStartPosition.CenterScreen;
        this.Text = "SyncVerse Studio - Login";
            this.BackColor = System.Drawing.Color.FromArgb(15, 25, 35); // Very dark blue-gray
    this.WindowState = FormWindowState.Normal;
      this.DoubleBuffered = true;

         // 
            // mainPanel - Full screen container
  // 
     this.mainPanel.Dock = DockStyle.Fill;
   this.mainPanel.BackColor = System.Drawing.Color.Transparent;

       // 
   // loginPanel - Modern centered login card with logo space
   // 
         this.loginPanel.BackColor = System.Drawing.Color.FromArgb(25, 35, 45); // Slightly lighter than background
this.loginPanel.Size = new System.Drawing.Size(380, 480); // Increased height for logo
            this.loginPanel.BorderStyle = BorderStyle.FixedSingle;

        // 
    // logoPictureBox - Company logo
      // 
          this.logoPictureBox.Size = new System.Drawing.Size(80, 80);
  this.logoPictureBox.Location = new System.Drawing.Point(150, 20); // Centered horizontally
 this.logoPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
     this.logoPictureBox.BackColor = System.Drawing.Color.Transparent;

        // Load logo with fallback
try
            {
         var logoPath = Path.Combine(Application.StartupPath, "assets", "img", "logo.png");
             if (File.Exists(logoPath))
  {
 this.logoPictureBox.Image = Image.FromFile(logoPath);
      }
            else
      {
  // Hide the picture box if logo doesn't exist
  this.logoPictureBox.Visible = false;
            }
    }
      catch
       {
    // Hide the picture box if there's an error loading the logo
              this.logoPictureBox.Visible = false;
            }

            // 
   // titleLabel - "SyncVerse Studio" with modern styling
   // 
this.titleLabel.Text = "SyncVerse Studio";
   this.titleLabel.Font = new System.Drawing.Font("Segoe UI", 18F, FontStyle.Bold);
        this.titleLabel.ForeColor = System.Drawing.Color.FromArgb(220, 230, 240);
     this.titleLabel.Location = new System.Drawing.Point(0, 110);
  this.titleLabel.Size = new System.Drawing.Size(380, 40);
     this.titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.titleLabel.BackColor = System.Drawing.Color.Transparent;

     // 
            // subtitleLabel - Informative text
            // 
this.subtitleLabel.Text = "Please enter your credentials to proceed";
            this.subtitleLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F, FontStyle.Regular);
            this.subtitleLabel.ForeColor = System.Drawing.Color.FromArgb(140, 160, 180);
   this.subtitleLabel.Location = new System.Drawing.Point(20, 155);
   this.subtitleLabel.Size = new System.Drawing.Size(340, 25);
     this.subtitleLabel.TextAlign = ContentAlignment.MiddleCenter;
     this.subtitleLabel.BackColor = System.Drawing.Color.Transparent;

            // 
            // usernameLabel - Modern field label
 // 
  this.usernameLabel.Text = "username";
       this.usernameLabel.Font = new System.Drawing.Font("Segoe UI", 10F, FontStyle.Regular);
            this.usernameLabel.ForeColor = System.Drawing.Color.FromArgb(160, 180, 200);
            this.usernameLabel.Location = new System.Drawing.Point(40, 200);
   this.usernameLabel.Size = new System.Drawing.Size(300, 22);
      this.usernameLabel.BackColor = System.Drawing.Color.Transparent;

    // 
            // usernameTextBox - Modern input field
      // 
      this.usernameTextBox.Font = new System.Drawing.Font("Segoe UI", 11F);
         this.usernameTextBox.Location = new System.Drawing.Point(40, 225);
            this.usernameTextBox.Size = new System.Drawing.Size(300, 32);
            this.usernameTextBox.BorderStyle = BorderStyle.FixedSingle;
         this.usernameTextBox.BackColor = System.Drawing.Color.FromArgb(35, 45, 55);
    this.usernameTextBox.ForeColor = System.Drawing.Color.FromArgb(220, 230, 240);
   this.usernameTextBox.Text = "";

    // 
            // passwordLabel - Modern field label
        // 
 this.passwordLabel.Text = "password";
            this.passwordLabel.Font = new System.Drawing.Font("Segoe UI", 10F, FontStyle.Regular);
        this.passwordLabel.ForeColor = System.Drawing.Color.FromArgb(160, 180, 200);
       this.passwordLabel.Location = new System.Drawing.Point(40, 275);
  this.passwordLabel.Size = new System.Drawing.Size(300, 22);
  this.passwordLabel.BackColor = System.Drawing.Color.Transparent;

      // 
            // passwordTextBox - Modern password field
            // 
            this.passwordTextBox.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.passwordTextBox.Location = new System.Drawing.Point(40, 300);
            this.passwordTextBox.Size = new System.Drawing.Size(300, 32);
 this.passwordTextBox.BorderStyle = BorderStyle.FixedSingle;
            this.passwordTextBox.BackColor = System.Drawing.Color.FromArgb(35, 45, 55);
      this.passwordTextBox.ForeColor = System.Drawing.Color.FromArgb(220, 230, 240);
   this.passwordTextBox.UseSystemPasswordChar = true;
            this.passwordTextBox.Text = "";

            // 
  // loginButton - Modern styled button
            // 
            this.loginButton.Text = "Login";
 this.loginButton.Font = new System.Drawing.Font("Segoe UI", 11F, FontStyle.Bold);
    this.loginButton.BackColor = System.Drawing.Color.FromArgb(45, 65, 85);
        this.loginButton.ForeColor = System.Drawing.Color.FromArgb(220, 230, 240);
            this.loginButton.FlatStyle = FlatStyle.Flat;
   this.loginButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(65, 85, 105);
         this.loginButton.FlatAppearance.BorderSize = 1;
     this.loginButton.Location = new System.Drawing.Point(40, 360);
       this.loginButton.Size = new System.Drawing.Size(300, 42);
         this.loginButton.Cursor = Cursors.Hand;
  this.loginButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(55, 75, 95);

            // 
            // errorLabel - Status/error messages
            // 
            this.errorLabel.Text = "";
     this.errorLabel.Font = new System.Drawing.Font("Segoe UI", 9F, FontStyle.Regular);
       this.errorLabel.ForeColor = System.Drawing.Color.FromArgb(255, 120, 120);
            this.errorLabel.Location = new System.Drawing.Point(40, 420);
       this.errorLabel.Size = new System.Drawing.Size(300, 40);
            this.errorLabel.TextAlign = ContentAlignment.TopCenter;
        this.errorLabel.BackColor = System.Drawing.Color.Transparent;

       // Add all controls to login panel
            this.loginPanel.Controls.Add(this.logoPictureBox);
            this.loginPanel.Controls.Add(this.titleLabel);
    this.loginPanel.Controls.Add(this.subtitleLabel);
            this.loginPanel.Controls.Add(this.usernameLabel);
            this.loginPanel.Controls.Add(this.usernameTextBox);
    this.loginPanel.Controls.Add(this.passwordLabel);
       this.loginPanel.Controls.Add(this.passwordTextBox);
  this.loginPanel.Controls.Add(this.loginButton);
 this.loginPanel.Controls.Add(this.errorLabel);

 this.mainPanel.Controls.Add(this.loginPanel);
       this.Controls.Add(this.mainPanel);

            // Center panel on resize and load
    this.Load += (s, e) => CenterLoginPanel();
this.Resize += (s, e) => CenterLoginPanel();

   // Add modern styling effects
            this.usernameTextBox.Enter += (s, e) => ((TextBox)s).BackColor = System.Drawing.Color.FromArgb(40, 50, 60);
            this.usernameTextBox.Leave += (s, e) => ((TextBox)s).BackColor = System.Drawing.Color.FromArgb(35, 45, 55);
            this.passwordTextBox.Enter += (s, e) => ((TextBox)s).BackColor = System.Drawing.Color.FromArgb(40, 50, 60);
       this.passwordTextBox.Leave += (s, e) => ((TextBox)s).BackColor = System.Drawing.Color.FromArgb(35, 45, 55);

     this.ResumeLayout(false);
        }

        // Center the login panel for 16:9 screens
    private void CenterLoginPanel()
     {
  if (loginPanel != null)
       {
     loginPanel.Location = new Point(
       (this.ClientSize.Width - loginPanel.Width) / 2,
         (this.ClientSize.Height - loginPanel.Height) / 2
     );
     }
        }
    }
}
