using System.Drawing;
using System.Drawing.Drawing2D;
using SyncVerseStudio.Services;
using SyncVerseStudio.Models;
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
        private List<IconButton> menuButtons = new List<IconButton>();

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

            // Main Form - Modern design
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(248, 250, 252);
            this.ClientSize = new Size(1600, 900);
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.Name = "MainDashboard";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "SYNCVERSE Studio - Dashboard";
            this.MinimumSize = new Size(1366, 768);
            this.WindowState = FormWindowState.Maximized;
            this.Icon = SystemIcons.Application;

            // Modern Sidebar with Material Design - increased width for better visibility
            this.sidebarPanel.BackColor = Color.FromArgb(15, 23, 42); // Slate-900
            this.sidebarPanel.Dock = DockStyle.Left;
            this.sidebarPanel.Width = 320; // Increased from 300 for better text visibility
            this.sidebarPanel.Padding = new Padding(0);
            
            // Custom paint for modern sidebar
            this.sidebarPanel.Paint += SidebarPanel_Paint;

            // No top panel for cleaner design
            this.topPanel.Visible = false;
            
            // Content Panel - Modern light background
            this.contentPanel.BackColor = Color.FromArgb(248, 250, 252);
            this.contentPanel.Dock = DockStyle.Fill;
            this.contentPanel.Padding = new Padding(0);

            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.sidebarPanel);

            this.ResumeLayout(false);
        }

        private void SidebarPanel_Paint(object sender, PaintEventArgs e)
        {
            var rect = sidebarPanel.ClientRectangle;
            
            // Modern dark gradient
            using (var brush = new LinearGradientBrush(rect, 
                Color.FromArgb(15, 23, 42),  // slate-900
                Color.FromArgb(30, 41, 59),    // slate-800
                LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(brush, rect);
            }

            // Subtle right border with glow
            using (var pen = new Pen(Color.FromArgb(51, 65, 85), 2))  // slate-600
            {
                e.Graphics.DrawLine(pen, rect.Width - 2, 0, rect.Width - 2, rect.Height);
            }
            
            // Inner glow effect
            using (var pen = new Pen(Color.FromArgb(30, 100, 149, 237), 1))
            {
              e.Graphics.DrawLine(pen, rect.Width - 1, 0, rect.Width - 1, rect.Height);
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
            // Brand header with modern design - increased height for better visibility
            var brandPanel = new Panel
            {
                BackColor = Color.FromArgb(30, 41, 59), // slate-800
                Location = new Point(0, yPos),
   Size = new Size(320, 110), // Increased height for better logo/text visibility
                Padding = new Padding(25, 25, 25, 25) // Increased padding
            };

            // Create the logo PictureBox - larger size
            logoPictureBox = new PictureBox
  {
        Location = new Point(25, 25),
        Size = new Size(60, 60), // Increased from 50x50
      BackColor = Color.Transparent,
        SizeMode = PictureBoxSizeMode.Zoom
};

    // Load GitHub logo asynchronously without blocking UI
  Task.Run(async () =>
    {
        try
{
        using (var client = new HttpClient())
       {
       client.Timeout = TimeSpan.FromSeconds(10);
            var imageBytes = await client.GetByteArrayAsync("https://github.com/eirsvi.png");
   using (var ms = new MemoryStream(imageBytes))
 {
            var image = Image.FromStream(ms);
      var bitmap = new Bitmap(image);
        
           // Update UI on main thread
         this.Invoke(new Action(() =>
         {
 if (logoPictureBox != null && !logoPictureBox.IsDisposed)
            {
       logoPictureBox.Image?.Dispose(); // Dispose old image if any
     logoPictureBox.Image = bitmap;
      }
           }));
         }
            }
        }
   catch (Exception ex)
        {
            // On error, set a fallback icon on main thread
 this.Invoke(new Action(() =>
            {
   if (logoPictureBox != null && !logoPictureBox.IsDisposed)
      {
                    // Remove the PictureBox and add a fallback icon
         brandPanel.Controls.Remove(logoPictureBox);
     var fallbackIcon = new IconPictureBox
         {
            IconChar = IconChar.User,
   IconColor = Color.FromArgb(56, 189, 248),
         IconSize = 50, // Increased size
      Location = new Point(25, 25),
     Size = new Size(60, 60), // Increased size
         BackColor = Color.Transparent
      };
            brandPanel.Controls.Add(fallbackIcon);
          }
 }));
       
  Console.WriteLine($"Error loading GitHub logo: {ex.Message}");
        }
    });

    brandPanel.Controls.Add(logoPictureBox);

    // Brand name - larger and more prominent
    var brandLabel = new Label
    {
        Text = "SyncVerse Studio",
        Font = new Font("Segoe UI", 20F, FontStyle.Bold), // Increased font size
     ForeColor = Color.FromArgb(248, 250, 252), // slate-50
        Location = new Point(95, 25), // Adjusted position
        Size = new Size(200, 35), // Increased size
 TextAlign = ContentAlignment.MiddleLeft
    };
  brandPanel.Controls.Add(brandLabel);

    // Tagline - more visible
    var taglineLabel = new Label
    {
  Text = "POS SYSTEM",
        Font = new Font("Segoe UI", 11F, FontStyle.Regular), // Increased font size
        ForeColor = Color.FromArgb(148, 163, 184), // slate-400
        Location = new Point(95, 60), // Adjusted position
        Size = new Size(200, 20), // Increased size
        TextAlign = ContentAlignment.MiddleLeft
    };
    brandPanel.Controls.Add(taglineLabel);

    sidebarPanel.Controls.Add(brandPanel);
    yPos += 110; // Adjusted for new height

    // Gradient separator
    var separator = new Panel
    {
        BackColor = Color.FromArgb(56, 189, 248),
  Location = new Point(25, yPos),
    Size = new Size(270, 3) // Adjusted width for new sidebar width
    };
    
    separator.Paint += (s, e) =>
    {
        using (var brush = new LinearGradientBrush(
            new Rectangle(0, 0, 270, 3),
      Color.FromArgb(56, 189, 248),
            Color.FromArgb(147, 51, 234),
      LinearGradientMode.Horizontal))
      {
            e.Graphics.FillRectangle(brush, 0, 0, 270, 3);
        }
 };
    
    sidebarPanel.Controls.Add(separator);
    yPos += 40; // Space after separator
}

        private void CreateNavigationMenu(UserRole role, ref int yPos)
        {
            // Navigation header
            var navHeader = new Label
       {
 Text = "NAVIGATION",
       Font = new Font("Segoe UI", 10F, FontStyle.Bold),
    ForeColor = Color.FromArgb(148,163,184), // slate-400
   Location = new Point(30, yPos),
        Size = new Size(260, 25)
   };
sidebarPanel.Controls.Add(navHeader);
     yPos += 45;

     // Create role-based menu with better spacing
 switch (role)
 {
 case UserRole.Administrator:
 // Core Operations (Point of Sale, Sales and Reports removed for Administrator)
 CreateModernMenuItem("Dashboard", IconChar.Home, Color.FromArgb(59,130,246), yPos, () => SafeLoadChildForm(() => new DashboardView(_authService)));
 yPos +=55;
 CreateModernMenuItem("Users", IconChar.Users, Color.FromArgb(59,130,246), yPos, () => SafeLoadChildForm(() => new UserManagementView(_authService)));
 yPos +=55;
 CreateModernMenuItem("Products", IconChar.Box, Color.FromArgb(34,197,94), yPos, () => SafeLoadChildForm(() => new ProductManagementView(_authService)));
 yPos +=55;
 CreateModernMenuItem("Customers", IconChar.UserFriends, Color.FromArgb(34,197,94), yPos, () => SafeLoadChildForm(() => new CustomerManagementView(_authService)));
 yPos +=55;
 CreateModernMenuItem("Categories", IconChar.Tags, Color.FromArgb(168,85,247), yPos, () => SafeLoadChildForm(() => new CategoryManagementView(_authService)));
 yPos +=55;
 CreateModernMenuItem("Suppliers", IconChar.Truck, Color.FromArgb(168,85,247), yPos, () => SafeLoadChildForm(() => new SupplierManagementView(_authService)));
 yPos +=55;
 CreateModernMenuItem("Analytics", IconChar.ChartPie, Color.FromArgb(245,158,11), yPos, () => SafeLoadChildForm(() => new AnalyticsView(_authService)));
 yPos +=55;
 CreateModernMenuItem("Audit Logs", IconChar.FileText, Color.FromArgb(245,158,11), yPos, () => SafeLoadChildForm(() => new AuditLogView(_authService)));
 break;

 case UserRole.Cashier:
 CreateModernMenuItem("Dashboard", IconChar.Home, Color.FromArgb(34,197,94), yPos, () => SafeLoadChildForm(() => new CashierDashboardView(_authService)));
 yPos += 55;
 CreateModernMenuItem("Point of Sale", IconChar.CashRegister, Color.FromArgb(34,197,94), yPos, () => SafeLoadChildForm(() => new PointOfSaleView(_authService)));
     yPos += 55;
 CreateModernMenuItem("Sales History", IconChar.Receipt, Color.FromArgb(34,197,94), yPos, () => SafeLoadChildForm(() => new SalesView(_authService)));
         yPos += 55;
            CreateModernMenuItem("Customers", IconChar.UserFriends, Color.FromArgb(34, 197, 94), yPos, () => SafeLoadChildForm(() => new CustomerManagementView(_authService)));
      break;

        case UserRole.InventoryClerk:
        CreateModernMenuItem("Dashboard", IconChar.Home, Color.FromArgb(59,130,246), yPos, () => SafeLoadChildForm(() => new DashboardView(_authService)));
      yPos += 55;
    CreateModernMenuItem("Products", IconChar.Box, Color.FromArgb(59,130,246), yPos, () => SafeLoadChildForm(() => new ProductManagementView(_authService)));
    yPos += 55;
                CreateModernMenuItem("Categories", IconChar.Tags, Color.FromArgb(59,130,246), yPos, () => SafeLoadChildForm(() => new CategoryManagementView(_authService)));
         yPos += 55;
          CreateModernMenuItem("Suppliers", IconChar.Truck, Color.FromArgb(59,130,246), yPos, () => SafeLoadChildForm(() => new SupplierManagementView(_authService)));
  yPos += 55;
 CreateModernMenuItem("Stock Reports", IconChar.FileAlt, Color.FromArgb(59,130,246), yPos, () => SafeLoadChildForm(() => new InventoryReportsView(_authService)));
    break;
}
        }

   private void CreateModernMenuItem(string text, IconChar icon, Color accentColor, int yPos, Action clickAction)
        {
       var menuItem = new IconButton
            {
 Text = $"   {text}", // Add spacing for better alignment
              Font = new Font("Segoe UI", 12F, FontStyle.Regular),
          ForeColor = Color.FromArgb(226, 232, 240), // slate-200
    BackColor = Color.Transparent,
          FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
  Location = new Point(15, yPos),
     Size = new Size(290, 50),
   Cursor = Cursors.Hand,
   IconChar = icon,
         IconColor = Color.FromArgb(148, 163, 184), // slate-400
    IconSize = 24,
                Padding = new Padding(15, 0, 15, 0),
           TextImageRelation = TextImageRelation.ImageBeforeText,
             ImageAlign = ContentAlignment.MiddleLeft,
        Tag = accentColor // Store accent color for later use
            };

       menuItem.FlatAppearance.BorderSize = 0;
         
      // Modern hover and click effects
            menuItem.MouseEnter += (s, e) =>
            {
      if (menuItem.Tag?.ToString() != "selected")
          {
    menuItem.BackColor = Color.FromArgb(30, 41, 59); // slate-800
             menuItem.ForeColor = Color.White;
      menuItem.IconColor = accentColor;
     }
        };

            menuItem.MouseLeave += (s, e) =>
         {
                if (menuItem.Tag?.ToString() != "selected")
   {
           menuItem.BackColor = Color.Transparent;
        menuItem.ForeColor = Color.FromArgb(226, 232, 240);
           menuItem.IconColor = Color.FromArgb(148, 163, 184);
         }
};

     menuItem.Click += (s, e) =>
            {
      try
        {
   // Get reference to the clicked button
        var clickedButton = (IconButton)s;
       
     // Reset all buttons to default state
                    foreach (var btn in menuButtons)
         {
        btn.BackColor = Color.Transparent;
    btn.ForeColor = Color.FromArgb(226, 232, 240);
        btn.IconColor = Color.FromArgb(148, 163, 184);
            btn.Tag = btn.Tag is Color ? btn.Tag : null; // Keep color, remove selected flag
      }

              // Set active state for clicked button
        clickedButton.BackColor = accentColor;
          clickedButton.ForeColor = Color.White;
          clickedButton.IconColor = Color.White;
  clickedButton.Tag = "selected"; // Mark as selected

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
    // Compact logout button - icon only with minimal styling
        logoutButton = new IconButton
    {
         BackColor = Color.Transparent,
 ForeColor = Color.FromArgb(148, 163, 184), // slate-400
        FlatStyle = FlatStyle.Flat,
       Font = new Font("Segoe UI", 10F, FontStyle.Regular), // Slightly larger
      IconChar = IconChar.ArrowRightFromBracket,
     IconColor = Color.FromArgb(148, 163, 184), // slate-400
      IconSize = 20, // Slightly larger
       Text = "   Logout",
                TextAlign = ContentAlignment.MiddleLeft,
      Size = new Size(280, 40), // Adjusted for new sidebar width
    Location = new Point(20, this.ClientSize.Height - 70),
           Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
          Cursor = Cursors.Hand,
           Padding = new Padding(20, 0, 20, 0),
     TextImageRelation = TextImageRelation.ImageBeforeText
            };

            logoutButton.FlatAppearance.BorderSize = 0;
            
          // Subtle hover effect
    logoutButton.MouseEnter += (s, e) =>
 {
          logoutButton.BackColor = Color.FromArgb(51, 65, 85); // slate-600
     logoutButton.ForeColor = Color.FromArgb(248, 113, 113); // red-400
                logoutButton.IconColor = Color.FromArgb(248, 113, 113); // red-400
 };

         logoutButton.MouseLeave += (s, e) =>
            {
                logoutButton.BackColor = Color.Transparent;
      logoutButton.ForeColor = Color.FromArgb(148, 163, 184); // slate-400
    logoutButton.IconColor = Color.FromArgb(148, 163, 184); // slate-400
            };

       logoutButton.Click += LogoutButton_Click;

  // Update position on resize
            this.Resize += (s, e) =>
  {
                if (logoutButton != null)
           logoutButton.Location = new Point(20, this.ClientSize.Height - 70);
  };

            sidebarPanel.Controls.Add(logoutButton);

     // Version info - compact and subtle
   var versionLabel = new Label
        {
        Text = "v2.1.0",
         Font = new Font("Segoe UI", 8F, FontStyle.Regular), // Slightly larger
       ForeColor = Color.FromArgb(71, 85, 105), // slate-600
          Location = new Point(20, this.ClientSize.Height - 25),
   Size = new Size(280, 15), // Adjusted for new sidebar width
Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
       TextAlign = ContentAlignment.MiddleCenter
 };

    this.Resize += (s, e) =>
    {
            if (versionLabel != null)
           versionLabel.Location = new Point(20, this.ClientSize.Height - 25);
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
                var accentColor = GetModernRoleColor(role);
 firstButton.BackColor = accentColor;
    firstButton.ForeColor = Color.White;
   firstButton.IconColor = Color.White;
                firstButton.Tag = "selected";
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