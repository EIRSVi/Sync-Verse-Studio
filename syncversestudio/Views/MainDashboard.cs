using System.Drawing;
using System.Drawing.Drawing2D;
using SyncVerseStudio.Services;
using SyncVerseStudio.Models;
using SyncVerseStudio.Helpers;
using FontAwesome.Sharp;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;

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
        private List<Button> menuButtons = new List<Button>();

        public MainDashboard(AuthenticationService authService)
        {
  _authService = authService;
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

            // Sidebar with Brand Theme
            this.sidebarPanel.BackColor = BrandTheme.SidebarBackground;
            this.sidebarPanel.Dock = DockStyle.Left;
            this.sidebarPanel.Width = 280;
            this.sidebarPanel.Padding = new Padding(0);
            
            // Custom paint for sidebar
            this.sidebarPanel.Paint += SidebarPanel_Paint;

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

        private void SidebarPanel_Paint(object sender, PaintEventArgs e)
        {
            var rect = sidebarPanel.ClientRectangle;
            
            // Brand gradient
            using (var brush = new LinearGradientBrush(rect, 
                BrandTheme.Charcoal,
                Color.FromArgb(100, 70, 35),
                LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(brush, rect);
            }

            // Right border with brand accent
            using (var pen = new Pen(BrandTheme.LimeGreen, 2))
            {
                e.Graphics.DrawLine(pen, rect.Width - 2, 0, rect.Width - 2, rect.Height);
            }
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
            // Brand header with brand colors
            var brandPanel = new Panel
            {
                BackColor = Color.FromArgb(100, 70, 35),
                Location = new Point(0, yPos),
   Size = new Size(280, 100),
                Padding = new Padding(20, 20, 20, 20)
            };

            // Create the logo PictureBox
            logoPictureBox = new PictureBox
  {
        Location = new Point(20, 20),
        Size = new Size(60, 60),
      BackColor = Color.Transparent,
        SizeMode = PictureBoxSizeMode.Zoom
};

    // Load brand logo
  Task.Run(async () =>
    {
        try
{
            var logoPath = Path.Combine(Application.StartupPath, "..", "..", "..", BrandTheme.LogoPath);
            if (File.Exists(logoPath))
            {
                var image = Image.FromFile(logoPath);
                var bitmap = new Bitmap(image);
        
                // Update UI on main thread
                this.Invoke(new Action(() =>
                {
                    if (logoPictureBox != null && !logoPictureBox.IsDisposed)
                    {
                        logoPictureBox.Image?.Dispose();
                        logoPictureBox.Image = bitmap;
                    }
                }));
            }
        }
   catch (Exception ex)
        {
            // On error, set a fallback icon on main thread
 this.Invoke(new Action(() =>
            {
   if (logoPictureBox != null && !logoPictureBox.IsDisposed)
      {
                    brandPanel.Controls.Remove(logoPictureBox);
     var fallbackIcon = new IconPictureBox
         {
            IconChar = IconChar.Store,
   IconColor = BrandTheme.LimeGreen,
         IconSize = 50,
      Location = new Point(20, 20),
     Size = new Size(60, 60),
         BackColor = Color.Transparent
      };
            brandPanel.Controls.Add(fallbackIcon);
          }
 }));
       
  Console.WriteLine($"Error loading logo: {ex.Message}");
        }
    });

    brandPanel.Controls.Add(logoPictureBox);

    // Brand name
    var brandLabel = new Label
    {
        Text = BrandTheme.BrandName,
        Font = new Font("Segoe UI", 16F, FontStyle.Bold),
     ForeColor = BrandTheme.CoolWhite,
        Location = new Point(90, 25),
        Size = new Size(180, 30),
 TextAlign = ContentAlignment.MiddleLeft
    };
  brandPanel.Controls.Add(brandLabel);

    // Tagline
    var taglineLabel = new Label
    {
  Text = BrandTheme.BrandTagline,
        Font = new Font("Segoe UI", 9F, FontStyle.Regular),
        ForeColor = BrandTheme.LimeGreen,
        Location = new Point(90, 55),
        Size = new Size(180, 20),
        TextAlign = ContentAlignment.MiddleLeft
    };
    brandPanel.Controls.Add(taglineLabel);

    sidebarPanel.Controls.Add(brandPanel);
    yPos += 100;

    // Brand separator
    var separator = new Panel
    {
        BackColor = BrandTheme.LimeGreen,
  Location = new Point(20, yPos),
    Size = new Size(240, 2)
    };
    
    sidebarPanel.Controls.Add(separator);
    yPos += 30;
}

        private void CreateNavigationMenu(UserRole role, ref int yPos)
        {
            // Navigation header
            var navHeader = new Label
       {
 Text = "NAVIGATION",
       Font = new Font("Segoe UI", 9F, FontStyle.Bold),
    ForeColor = BrandTheme.LimeGreen,
   Location = new Point(25, yPos),
        Size = new Size(230, 25)
   };
sidebarPanel.Controls.Add(navHeader);
     yPos += 40;

     // Create role-based menu with brand colors
 switch (role)
 {
 case UserRole.Administrator:
 CreateModernMenuItem("Dashboard", IconChar.Home, BrandTheme.OceanBlue, yPos, () => SafeLoadChildForm(() => new DashboardView(_authService)));
 yPos +=50;
 CreateModernMenuItem("Users", IconChar.Users, BrandTheme.OceanBlue, yPos, () => SafeLoadChildForm(() => new UserManagementView(_authService)));
 yPos +=50;
 CreateModernMenuItem("Products", IconChar.Box, BrandTheme.LimeGreen, yPos, () => SafeLoadChildForm(() => new ProductManagementView(_authService)));
 yPos +=50;
 CreateModernMenuItem("Customers", IconChar.UserFriends, BrandTheme.LimeGreen, yPos, () => SafeLoadChildForm(() => new CustomerManagementView(_authService)));
 yPos +=50;
 CreateModernMenuItem("Categories", IconChar.Tags, BrandTheme.OceanBlue, yPos, () => SafeLoadChildForm(() => new CategoryManagementView(_authService)));
 yPos +=50;
 CreateModernMenuItem("Suppliers", IconChar.Truck, BrandTheme.OceanBlue, yPos, () => SafeLoadChildForm(() => new SupplierManagementView(_authService)));
 yPos +=50;
 CreateModernMenuItem("Analytics", IconChar.ChartPie, BrandTheme.LimeGreen, yPos, () => SafeLoadChildForm(() => new AnalyticsView(_authService)));
 yPos +=50;
 CreateModernMenuItem("Audit Logs", IconChar.FileText, BrandTheme.LimeGreen, yPos, () => SafeLoadChildForm(() => new AuditLogView(_authService)));
 break;

 case UserRole.Cashier:
 CreateModernMenuItem("Dashboard", IconChar.Home, BrandTheme.LimeGreen, yPos, () => SafeLoadChildForm(() => new CashierDashboardView(_authService)));
 yPos += 50;
 CreateModernMenuItem("Point of Sale", IconChar.CashRegister, BrandTheme.LimeGreen, yPos, () => SafeLoadChildForm(() => new ModernPOSView(_authService)));
     yPos += 50;
 CreateModernMenuItem("Sales History", IconChar.Receipt, BrandTheme.LimeGreen, yPos, () => SafeLoadChildForm(() => new SalesView(_authService)));
         yPos += 50;
            CreateModernMenuItem("Customers", IconChar.UserFriends, BrandTheme.LimeGreen, yPos, () => SafeLoadChildForm(() => new CustomerManagementView(_authService)));
      break;

        case UserRole.InventoryClerk:
        CreateModernMenuItem("Dashboard", IconChar.Home, BrandTheme.OceanBlue, yPos, () => SafeLoadChildForm(() => new DashboardView(_authService)));
      yPos += 50;
    CreateModernMenuItem("Products", IconChar.Box, BrandTheme.OceanBlue, yPos, () => SafeLoadChildForm(() => new ProductManagementView(_authService)));
    yPos += 50;
                CreateModernMenuItem("Categories", IconChar.Tags, BrandTheme.OceanBlue, yPos, () => SafeLoadChildForm(() => new CategoryManagementView(_authService)));
         yPos += 50;
          CreateModernMenuItem("Suppliers", IconChar.Truck, BrandTheme.OceanBlue, yPos, () => SafeLoadChildForm(() => new SupplierManagementView(_authService)));
  yPos += 50;
 CreateModernMenuItem("Stock Reports", IconChar.FileAlt, BrandTheme.OceanBlue, yPos, () => SafeLoadChildForm(() => new InventoryReportsView(_authService)));
    break;
}
        }

   private void CreateModernMenuItem(string text, IconChar icon, Color accentColor, int yPos, Action clickAction)
        {
       var menuItem = new Button
            {
 Text = text,
              Font = new Font("Segoe UI", 12F, FontStyle.Bold),
          ForeColor = Color.White,
    BackColor = Color.Transparent,
          FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleCenter,
  Location = new Point(20, yPos),
     Size = new Size(240, 45),
   Cursor = Cursors.Hand,
                Padding = new Padding(0),
        Tag = new { AccentColor = accentColor, IsSelected = false }
            };

       menuItem.FlatAppearance.BorderSize = 0;
         
      // Brand hover and click effects
            menuItem.MouseEnter += (s, e) =>
            {
      var btn = (Button)s;
      dynamic tag = btn.Tag;
      if (!tag.IsSelected)
          {
    btn.BackColor = BrandTheme.SidebarHover;
             btn.ForeColor = Color.White;
     }
        };

            menuItem.MouseLeave += (s, e) =>
         {
      var btn = (Button)s;
      dynamic tag = btn.Tag;
                if (!tag.IsSelected)
   {
           btn.BackColor = Color.Transparent;
        btn.ForeColor = Color.White;
         }
};

     menuItem.Click += (s, e) =>
            {
      try
        {
   // Get reference to the clicked button
        var clickedButton = (Button)s;
       
     // Reset all buttons to default state
                    foreach (var btn in menuButtons)
         {
        btn.BackColor = Color.Transparent;
    btn.ForeColor = Color.White;
            dynamic btnTag = btn.Tag;
            btn.Tag = new { AccentColor = btnTag.AccentColor, IsSelected = false };
      }

              // Set active state for clicked button
        clickedButton.BackColor = BrandTheme.OceanBlue;
          clickedButton.ForeColor = Color.White;
  dynamic tag = clickedButton.Tag;
  clickedButton.Tag = new { AccentColor = tag.AccentColor, IsSelected = true };

     clickAction();
        }
              catch (Exception ex)
     {
MessageBox.Show($"Error: {ex.Message}", "Navigation Error", 
         MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
        };

   menuButtons.Add(menuItem);
 sidebarPanel.Controls.Add(menuItem);
   }

        private void CreateSidebarFooter()
        {
    // Logout button with brand styling
        logoutButton = new IconButton
    {
         BackColor = Color.Transparent,
 ForeColor = BrandTheme.SidebarText,
        FlatStyle = FlatStyle.Flat,
       Font = new Font("Segoe UI", 10F, FontStyle.Regular),
      IconChar = IconChar.ArrowRightFromBracket,
     IconColor = BrandTheme.CoolWhite,
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
            
          // Brand hover effect
    logoutButton.MouseEnter += (s, e) =>
 {
          logoutButton.BackColor = BrandTheme.SidebarHover;
     logoutButton.ForeColor = BrandTheme.Error;
                logoutButton.IconColor = BrandTheme.Error;
 };

         logoutButton.MouseLeave += (s, e) =>
            {
                logoutButton.BackColor = Color.Transparent;
      logoutButton.ForeColor = BrandTheme.SidebarText;
    logoutButton.IconColor = BrandTheme.CoolWhite;
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
       ForeColor = Color.FromArgb(90, 70, 40),
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
 firstButton.BackColor = BrandTheme.OceanBlue;
    firstButton.ForeColor = Color.White;
                dynamic tag = firstButton.Tag;
                firstButton.Tag = new { AccentColor = tag.AccentColor, IsSelected = true };
    }
            
            // Load appropriate dashboard based on role
            if (_authService.CurrentUser.Role == UserRole.Cashier)
            {
                SafeLoadChildForm(() => new CashierDashboardView(_authService));
            }
            else
            {
                SafeLoadChildForm(() => new DashboardView(_authService));
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
    var result = MessageBox.Show(
 "Are you sure you want to logout?",
          "Confirm Logout",
      MessageBoxButtons.YesNo,
  MessageBoxIcon.Question);

          if (result == DialogResult.Yes)
  {
  await _authService.LogoutAsync();
        this.Close();
      }
          }
            catch (Exception ex)
    {
         MessageBox.Show($"{ex.Message}", "Logout Error",
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