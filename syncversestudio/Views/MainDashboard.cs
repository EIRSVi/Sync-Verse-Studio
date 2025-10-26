using System.Drawing;
using System.Drawing.Drawing2D;
using SyncVerseStudio.Services;
using SyncVerseStudio.Models;
using SyncVerseStudio.Helpers;
using SyncVerseStudio.Views.CashierDashboard;
using FontAwesome.Sharp;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Linq;

namespace SyncVerseStudio.Views
{
    public partial class MainDashboard : Form
    {
        private readonly AuthenticationService _authService;
        private Panel sidebarPanel;
        private Panel topPanel;
        private Panel contentPanel;
        private Label userLabel;
        private Label roleLabel;
        private IconButton logoutButton;
        private Form? _currentChildForm;
        private PictureBox logoPictureBox;
        private List<Panel> menuButtons = new List<Panel>();
        private LoginForm? _loginForm;

        public MainDashboard(AuthenticationService authService, LoginForm? loginForm = null)
        {
            _authService = authService;
            _loginForm = loginForm;
            InitializeComponent();
            
            // Set application icon using helper
            // SyncVerseStudio.Helpers.IconHelper.SetFormIcon(this);
            
            LoadUserInterface();
        }

        private void InitializeComponent()
        {
            this.sidebarPanel = new Panel();
            this.topPanel = new Panel();
            this.contentPanel = new Panel();
            this.userLabel = new Label();
            this.roleLabel = new Label();
            this.logoutButton = new IconButton();

            this.SuspendLayout();

            // Main Form - SyncVerse Studio Brand Theme
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = BrandTheme.Background;
            this.ClientSize = new Size(1600, 900);
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.Name = "MainDashboard";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = BrandTheme.BrandName + " - Dashboard";
            this.MinimumSize = new Size(1366, 768);
            this.WindowState = FormWindowState.Maximized;
            this.Icon = SystemIcons.Application;

            // Sidebar with light blue background
            this.sidebarPanel.BackColor = Color.FromArgb(215, 232, 250); // #D7E8FA
            this.sidebarPanel.Dock = DockStyle.Left;
            this.sidebarPanel.Width = 280;
            this.sidebarPanel.Padding = new Padding(0);

            // No top panel for cleaner design
            this.topPanel.Visible = false;
            
            // Content Panel - Brand background
            this.contentPanel.BackColor = BrandTheme.Background;
            this.contentPanel.Dock = DockStyle.Fill;
            this.contentPanel.Padding = new Padding(0);

            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.sidebarPanel);

            this.ResumeLayout(false);
        }



        private void LoadUserInterface()
        {
            var user = _authService.CurrentUser;
            if (user == null) return;

            CreateModernSidebar(user);
            LoadDefaultView(user.Role);
        }

        private void CreateModernSidebar(User user)
        {
            sidebarPanel.Controls.Clear();
            menuButtons.Clear();
            
            int yPos = 0;

            // === BRAND HEADER ===
            CreateBrandHeader(ref yPos);

            // === NAVIGATION MENU ===
            CreateNavigationMenu(user.Role, ref yPos);

            // === FOOTER & LOGOUT ===
            CreateSidebarFooter();
        }

        private void CreateBrandHeader(ref int yPos)
        {
            // Add larger centered logo at the top - adjusted to left
            logoPictureBox = new PictureBox
            {
                Location = new Point(40, yPos + 30), // Moved left: adjust X value (40 = more left, 65 = center, 90 = more right)
                Size = new Size(150, 95), // Logo size (width, height)
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.Zoom,
                Anchor = AnchorStyles.Top
            };

            // Load brand logo from URL
            Task.Run(async () =>
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        // Load color logo without background
                        try
                        {
                            var pngUrl = "https://raw.githubusercontent.com/EIRSVi/eirsvi/refs/heads/docs/assets/brand/noBgColor.png";
                            var imageBytes = await client.GetByteArrayAsync(pngUrl);
                            using (var ms = new MemoryStream(imageBytes))
                            {
                                var image = Image.FromStream(ms);
                                this.Invoke(new Action(() =>
                                {
                                    if (logoPictureBox != null && !logoPictureBox.IsDisposed)
                                    {
                                        logoPictureBox.Image?.Dispose();
                                        logoPictureBox.Image = image;
                                    }
                                }));
                            }
                        }
                        catch
                        {
                            // Fallback to local file
                            var logoPath = Path.Combine(Application.StartupPath, "..", "..", "..", BrandTheme.LogoPath);
                            if (File.Exists(logoPath))
                            {
                                this.Invoke(new Action(() =>
                                {
                                    if (logoPictureBox != null && !logoPictureBox.IsDisposed)
                                    {
                                        logoPictureBox.Image = Image.FromFile(logoPath);
                                    }
                                }));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // On error, set a fallback icon
                    this.Invoke(new Action(() =>
                    {
                        if (logoPictureBox != null && !logoPictureBox.IsDisposed)
                        {
                            var fallbackIcon = new IconPictureBox
                            {
                                IconChar = IconChar.Store,
                                IconColor = BrandTheme.LimeGreen,
                                IconSize = 60,
                                Location = new Point(110, 40),
                                Size = new Size(60, 60),
                                BackColor = Color.Transparent
                            };
                            sidebarPanel.Controls.Add(fallbackIcon);
                        }
                    }));
       
                    Console.WriteLine($"Error loading logo: {ex.Message}");
                }
            });

            sidebarPanel.Controls.Add(logoPictureBox);
            yPos += 115; // Space for logo

            // Add brand name below logo - centered and capitalized
            var brandNameLabel = new Label
            {
                // Text = "SYNCVERSE STUDIO",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60), // Dark text
                BackColor = Color.Transparent,
                Location = new Point(0, yPos),
                Size = new Size(280, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                Anchor = AnchorStyles.Top
            };
            sidebarPanel.Controls.Add(brandNameLabel);
            yPos += 45; // Space for brand name
}

        private void CreateNavigationMenu(UserRole role, ref int yPos)
        {
            // Navigation header
            var navHeader = new Label
       {
 Text = "NAVIGATION",
       Font = new Font("Segoe UI", 9F, FontStyle.Bold),
    ForeColor = Color.FromArgb(100, 100, 100), // Dark gray for light background
   Location = new Point(25, yPos),
        Size = new Size(230, 25)
   };
sidebarPanel.Controls.Add(navHeader);
     yPos += 40;

     // Create role-based menu with brand colors
 switch (role)
 {
 case UserRole.Administrator:
 CreateModernMenuItem("Dashboard", IconChar.Home, BrandTheme.DarkGray, yPos, () => SafeLoadChildForm(() => new DashboardView(_authService)));
 yPos +=50;
 CreateModernMenuItem("Accounts", IconChar.Users, BrandTheme.DarkGray, yPos, () => SafeLoadChildForm(() => new UserManagementView(_authService)));
 yPos +=50;
 CreateModernMenuItem("Products", IconChar.Box, BrandTheme.LimeGreen, yPos, () => SafeLoadChildForm(() => new ProductManagementView(_authService)));
 yPos +=50;
 CreateModernMenuItem("Customers", IconChar.UserFriends, BrandTheme.LimeGreen, yPos, () => SafeLoadChildForm(() => new CustomerManagementView(_authService)));
 yPos +=50;
 CreateModernMenuItem("Categories", IconChar.Tags, BrandTheme.DarkGray, yPos, () => SafeLoadChildForm(() => new CategoryManagementView(_authService)));
 yPos +=50;
 CreateModernMenuItem("Suppliers", IconChar.Truck, BrandTheme.DarkGray, yPos, () => SafeLoadChildForm(() => new SupplierManagementView(_authService)));
 yPos +=50;
 CreateModernMenuItem("Database Management", IconChar.Server, BrandTheme.DarkGray, yPos, () => SafeLoadChildForm(() => new DatabaseManagementForm()));
 yPos +=50;
 CreateModernMenuItem("Analytics", IconChar.ChartPie, BrandTheme.LimeGreen, yPos, () => SafeLoadChildForm(() => new AnalyticsView(_authService)));
 yPos +=50;
 CreateModernMenuItem("Audit Logs", IconChar.FileText, BrandTheme.LimeGreen, yPos, () => SafeLoadChildForm(() => new AuditLogView(_authService)));
 break;

 case UserRole.Cashier:
 CreateModernMenuItem("Dashboard", IconChar.Home, BrandTheme.LimeGreen, yPos, () => SafeLoadChildForm(() => new RealTimeCashierDashboard(_authService)));
 yPos += 50;
 CreateModernMenuItem("Cashier (POS)", IconChar.CashRegister, BrandTheme.LimeGreen, yPos, () => SafeLoadChildForm(() => new EnhancedPOSSystem(_authService)));
     yPos += 50;
            CreateModernMenuItem("Customers", IconChar.UserFriends, BrandTheme.LimeGreen, yPos, () => SafeLoadChildForm(() => new CustomerManagementView(_authService)));
      break;

        case UserRole.InventoryClerk:
        CreateModernMenuItem("Dashboard", IconChar.Home, BrandTheme.LimeGreen, yPos, () => SafeLoadChildForm(() => new InventoryClerkDashboardView(_authService)));
      yPos += 50;
    CreateModernMenuItem("Products", IconChar.Box, BrandTheme.LimeGreen, yPos, () => SafeLoadChildForm(() => new ProductManagementView(_authService)));
    yPos += 50;
                CreateModernMenuItem("Categories", IconChar.Tags, BrandTheme.LimeGreen, yPos, () => SafeLoadChildForm(() => new CategoryManagementView(_authService)));
         yPos += 50;
          CreateModernMenuItem("Suppliers", IconChar.Truck, BrandTheme.LimeGreen, yPos, () => SafeLoadChildForm(() => new SupplierManagementView(_authService)));
  yPos += 50;
 CreateModernMenuItem("Stock Reports", IconChar.FileAlt, BrandTheme.LimeGreen, yPos, () => SafeLoadChildForm(() => new InventoryReportsView(_authService)));
    break;
}
        }

   private void CreateModernMenuItem(string text, IconChar icon, Color accentColor, int yPos, Action clickAction)
        {
       var menuItem = new Panel
            {
  Location = new Point(0, yPos),
     Size = new Size(280, 45),
   Cursor = Cursors.Hand,
        BackColor = Color.Transparent,
        Tag = new { AccentColor = accentColor, IsSelected = false }
            };

       // Active border indicator (left side)
       var borderIndicator = new Panel
       {
           BackColor = Color.Transparent,
           Location = new Point(0, 0),
           Size = new Size(4, 45),
           Name = "BorderIndicator"
       };
       menuItem.Controls.Add(borderIndicator);

       // Icon
       var iconPic = new IconPictureBox
       {
           IconChar = icon,
           IconColor = Color.FromArgb(80, 80, 80), // Dark gray for light background
           IconSize = 24,
           Location = new Point(25, 10),
           Size = new Size(24, 24),
           BackColor = Color.Transparent,
           Name = "MenuIcon"
       };
       menuItem.Controls.Add(iconPic);

       // Text label with font support for both English and Khmer
       var textLabel = new Label
       {
           Text = text,
           Font = new Font("Segoe UI", 11F, FontStyle.Regular),
           ForeColor = Color.FromArgb(60, 60, 60), // Dark text for light background
           BackColor = Color.Transparent,
           Location = new Point(60, 0),
           Size = new Size(220, 45),
           TextAlign = ContentAlignment.MiddleLeft,
           Cursor = Cursors.Hand,
           Name = "MenuText"
       };
       menuItem.Controls.Add(textLabel);
         
      // Hover effects - with background color
            menuItem.MouseEnter += (s, e) =>
            {
      var panel = (Panel)s;
      dynamic tag = panel.Tag;
      if (!tag.IsSelected)
          {
                panel.BackColor = Color.FromArgb(200, 220, 240); // Light hover background
                var icon = panel.Controls.Find("MenuIcon", false).FirstOrDefault() as IconPictureBox;
                var label = panel.Controls.Find("MenuText", false).FirstOrDefault() as Label;
                if (icon != null) icon.IconColor = Color.FromArgb(255, 0, 80); // #ff0050
                if (label != null) label.ForeColor = Color.FromArgb(255, 0, 80);
     }
        };

            menuItem.MouseLeave += (s, e) =>
         {
      var panel = (Panel)s;
      dynamic tag = panel.Tag;
                if (!tag.IsSelected)
   {
                panel.BackColor = Color.Transparent;
                var icon = panel.Controls.Find("MenuIcon", false).FirstOrDefault() as IconPictureBox;
                var label = panel.Controls.Find("MenuText", false).FirstOrDefault() as Label;
                if (icon != null) icon.IconColor = Color.FromArgb(80, 80, 80); // Back to dark gray
                if (label != null) label.ForeColor = Color.FromArgb(60, 60, 60);
         }
};

     // Shared click handler for panel and child controls
        EventHandler menuClickHandler = (s, e) =>
            {
      try
        {
   // Find the parent panel (could be clicked from panel, icon, or label)
   Panel clickedPanel = s as Panel;
   if (clickedPanel == null)
   {
       // If clicked from child control, find parent panel
       Control ctrl = s as Control;
       while (ctrl != null && !(ctrl is Panel && menuButtons.Contains(ctrl as Panel)))
       {
           ctrl = ctrl.Parent;
       }
       clickedPanel = ctrl as Panel;
   }
   
   if (clickedPanel == null) return;
       
     // Reset all menu items
                    foreach (var btn in menuButtons)
         {
        btn.BackColor = Color.Transparent;
            dynamic btnTag = btn.Tag;
            btn.Tag = new { AccentColor = btnTag.AccentColor, IsSelected = false };
            
            var border = btn.Controls.Find("BorderIndicator", false).FirstOrDefault();
            if (border != null) border.BackColor = Color.Transparent;
            
            var icon = btn.Controls.Find("MenuIcon", false).FirstOrDefault() as IconPictureBox;
            var label = btn.Controls.Find("MenuText", false).FirstOrDefault() as Label;
            if (icon != null) icon.IconColor = Color.FromArgb(80, 80, 80); // Dark gray
            if (label != null) label.ForeColor = Color.FromArgb(60, 60, 60);
      }

              // Set active state - with background color
        clickedPanel.BackColor = Color.FromArgb(180, 210, 240); // Active background color
  dynamic tag = clickedPanel.Tag;
  clickedPanel.Tag = new { AccentColor = tag.AccentColor, IsSelected = true };
  
  // Show active border and color
  var activeBorder = clickedPanel.Controls.Find("BorderIndicator", false).FirstOrDefault();
  if (activeBorder != null) activeBorder.BackColor = Color.FromArgb(255, 0, 80); // #ff0050
  
  var activeIcon = clickedPanel.Controls.Find("MenuIcon", false).FirstOrDefault() as IconPictureBox;
  var activeLabel = clickedPanel.Controls.Find("MenuText", false).FirstOrDefault() as Label;
  if (activeIcon != null) activeIcon.IconColor = Color.FromArgb(255, 0, 80);
  if (activeLabel != null) activeLabel.ForeColor = Color.FromArgb(255, 0, 80);

     clickAction();
        }
              catch (Exception ex)
     {
MessageBox.Show($"Error: {ex.Message}", "Navigation Error", 
         MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
        };

   // Attach click handler to panel and child controls
   menuItem.Click += menuClickHandler;
   textLabel.Click += menuClickHandler;
   iconPic.Click += menuClickHandler;

   menuButtons.Add(menuItem);
 sidebarPanel.Controls.Add(menuItem);
   }

        private void CreateSidebarFooter()
        {
    // Logout button with brand styling
        logoutButton = new IconButton
    {
         BackColor = Color.Transparent,
 ForeColor = Color.FromArgb(60, 60, 60), // Dark text for light background
        FlatStyle = FlatStyle.Flat,
       Font = new Font("Segoe UI", 10F, FontStyle.Regular),
      IconChar = IconChar.ArrowRightFromBracket,
     IconColor = Color.FromArgb(80, 80, 80), // Dark icon for light background
      IconSize = 18,
       Text = "  Logout",
                TextAlign = ContentAlignment.MiddleLeft,
      Size = new Size(260, 40),
    Location = new Point(10, this.ClientSize.Height - 70),
           Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
          Cursor = Cursors.Hand,
           Padding = new Padding(15, 0, 15, 0),
     TextImageRelation = TextImageRelation.ImageBeforeText
            };

            logoutButton.FlatAppearance.BorderSize = 0;
            
          // Hover effect
    logoutButton.MouseEnter += (s, e) =>
 {
          logoutButton.BackColor = Color.FromArgb(200, 220, 240); // Slightly darker blue on hover
     logoutButton.ForeColor = BrandTheme.Error;
                logoutButton.IconColor = BrandTheme.Error;
 };

         logoutButton.MouseLeave += (s, e) =>
            {
                logoutButton.BackColor = Color.Transparent;
      logoutButton.ForeColor = Color.FromArgb(60, 60, 60);
    logoutButton.IconColor = Color.FromArgb(80, 80, 80);
            };

       logoutButton.Click += LogoutButton_Click;

  // Update position on resize
            this.Resize += (s, e) =>
  {
                if (logoutButton != null)
           logoutButton.Location = new Point(10, this.ClientSize.Height - 70);
  };

            sidebarPanel.Controls.Add(logoutButton);

     // Version info
   var versionLabel = new Label
        {
        Text = "v2.1.0",
         Font = new Font("Segoe UI", 8F, FontStyle.Regular),
       ForeColor = Color.FromArgb(120, 120, 120), // Gray text for light background
          Location = new Point(10, this.ClientSize.Height - 25),
   Size = new Size(260, 15),
Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
       TextAlign = ContentAlignment.MiddleCenter
 };

    this.Resize += (s, e) =>
    {
            if (versionLabel != null)
           versionLabel.Location = new Point(10, this.ClientSize.Height - 25);
     };

       sidebarPanel.Controls.Add(versionLabel);
     }

        private IconChar GetUserRoleIcon(UserRole role)
        {
            return role switch
      {
        UserRole.Administrator => IconChar.Crown,
  UserRole.Cashier => IconChar.CashRegister,
    UserRole.InventoryClerk => IconChar.Box,
    _ => IconChar.User
            };
        }

   private Color GetModernRoleColor(UserRole role)
        {
  return role switch
          {
         UserRole.Administrator => Color.FromArgb(239, 68, 68),   // red-500
           UserRole.Cashier => Color.FromArgb(34, 197, 94),        // green-500
         UserRole.InventoryClerk => Color.FromArgb(59, 130, 246), // blue-500
      _ => Color.FromArgb(107, 114, 128)  // gray-500
            };
        }

    private void LoadDefaultView(UserRole role)
      {
            var firstButton = menuButtons.FirstOrDefault();
    if (firstButton != null)
            {
 firstButton.BackColor = Color.FromArgb(180, 210, 240); // Active background
                dynamic tag = firstButton.Tag;
                firstButton.Tag = new { AccentColor = tag.AccentColor, IsSelected = true };
                
                // Set active color #ff0050
                var border = firstButton.Controls.Find("BorderIndicator", false).FirstOrDefault();
                if (border != null) border.BackColor = Color.FromArgb(255, 0, 80);
                
                var icon = firstButton.Controls.Find("MenuIcon", false).FirstOrDefault() as IconPictureBox;
                var label = firstButton.Controls.Find("MenuText", false).FirstOrDefault() as Label;
                if (icon != null) icon.IconColor = Color.FromArgb(255, 0, 80);
                if (label != null) label.ForeColor = Color.FromArgb(255, 0, 80);
    }
            
            // Load appropriate dashboard based on role
            switch (_authService.CurrentUser.Role)
            {
                case UserRole.Cashier:
                    SafeLoadChildForm(() => new RealTimeCashierDashboard(_authService));
                    break;
                case UserRole.InventoryClerk:
                    SafeLoadChildForm(() => new InventoryClerkDashboardView(_authService));
                    break;
                default:
                    SafeLoadChildForm(() => new DashboardView(_authService));
                    break;
            }
 }

        private void SafeLoadChildForm(Func<Form> formFactory)
        {
         try
       {
      // Check if authentication service and current user are valid
                if (_authService == null)
     {
       MessageBox.Show("Authentication service is not available.", "System Error",
     MessageBoxButtons.OK, MessageBoxIcon.Error);
         return;
     }

       if (_authService.CurrentUser == null)
     {
   MessageBox.Show("No user is currently logged in. Please login again.", "Authentication Error",
          MessageBoxButtons.OK, MessageBoxIcon.Warning);
  return;
              }

                // Try to create the form
     Form childForm;
    try
                {
       childForm = formFactory();
       }
 catch (Exception ex)
          {
MessageBox.Show($"Error creating form: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}", "Form Creation Error",
          MessageBoxButtons.OK, MessageBoxIcon.Error);
    return;
       }

                // Try to load the form
    try
      {
       LoadChildForm(childForm);
    }
         catch (Exception ex)
           {
            MessageBox.Show($"Error loading form: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}", "Form Loading Error",
   MessageBoxButtons.OK, MessageBoxIcon.Error);
           try { childForm?.Dispose(); } catch { }
   }
        }
      catch (Exception ex)
          {
          MessageBox.Show($"Unexpected error: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}", "System Error",
    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
     }

     public void LoadView(Form childForm)
        {
            LoadChildForm(childForm);
        }

     private void LoadChildForm(Form childForm)
        {
         if (_currentChildForm != null)
       {
  try
          {
          _currentChildForm.Hide();
      _currentChildForm.Close();
        _currentChildForm.Dispose();
                }
        catch { }
             finally { _currentChildForm = null; }
         }

            _currentChildForm = childForm;
            childForm.TopLevel = false;
      childForm.FormBorderStyle = FormBorderStyle.None;
    childForm.Dock = DockStyle.Fill;

   contentPanel.Controls.Clear();
          contentPanel.Controls.Add(childForm);
        childForm.Show();
    }

        private async void LogoutButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Show modern logout dialog
                using (var dialog = new LogoutDialog(_authService.CurrentUser))
                {
                    dialog.ShowDialog(this);

                    switch (dialog.SelectedAction)
                    {
                        case LogoutDialog.LogoutAction.SwitchUser:
                            // Switch User - logout and show login form
                            Console.WriteLine("User chose to switch user");
                            await _authService.LogoutAsync();
                            this.Hide();

                            var loginForm = new LoginForm();
                            loginForm.FormClosed += (s, ev) =>
                            {
                                if (loginForm.DialogResult == DialogResult.OK)
                                {
                                    // User logged in successfully, close this dashboard
                                    this.Close();
                                }
                                else
                                {
                                    // User cancelled login, exit application
                                    Application.Exit();
                                }
                            };
                            loginForm.ShowDialog();
                            break;

                        case LogoutDialog.LogoutAction.Cancel:
                            // Do nothing
                            Console.WriteLine("User cancelled");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Logout Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
 {
       try
{
        _currentChildForm?.Close();
       _currentChildForm?.Dispose();
   }
      catch { }
  base.OnFormClosed(e);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // If user tries to close the main dashboard directly (X button), ask what they want to do
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true; // Cancel the close initially
                LogoutButton_Click(this, EventArgs.Empty); // Show the logout options dialog
                return;
            }
            
            try { _currentChildForm?.Hide(); } catch { }
            base.OnFormClosing(e);
        }

      protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
         logoPictureBox?.Image?.Dispose();
      logoPictureBox?.Dispose();
            }
  base.Dispose(disposing);
        }
    }
}
