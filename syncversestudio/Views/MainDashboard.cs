using System.Drawing;
using System.Drawing.Drawing2D;
using SyncVerseStudio.Services;
using SyncVerseStudio.Models;
using FontAwesome.Sharp;

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
        private IconPictureBox userIconPic, logoIconPic;
        private Panel userInfoPanel;
        private List<IconButton> menuButtons = new List<IconButton>();

        public MainDashboard(AuthenticationService authService)
        {
            _authService = authService;
            InitializeComponent();
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
            this.userInfoPanel = new Panel();

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
            this.Text = "SyncVerse Studio - Modern Dashboard";
            this.MinimumSize = new Size(1366, 768);
            this.WindowState = FormWindowState.Maximized;
            this.Icon = SystemIcons.Application;

            // Modern Sidebar with Material Design
            this.sidebarPanel.BackColor = Color.FromArgb(15, 23, 42); // Slate-900
            this.sidebarPanel.Dock = DockStyle.Left;
            this.sidebarPanel.Width = 300; // Wider for better content
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
                Color.FromArgb(15, 23, 42),    // slate-900
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

            // === USER PROFILE CARD ===
            CreateUserProfileCard(user, ref yPos);

            // === NAVIGATION MENU ===
            CreateNavigationMenu(user.Role, ref yPos);

            // === FOOTER & LOGOUT ===
            CreateSidebarFooter();
        }

        private void CreateBrandHeader(ref int yPos)
        {
            // Brand header with modern design
            var brandPanel = new Panel
            {
                BackColor = Color.FromArgb(30, 41, 59), // slate-800
                Location = new Point(0, yPos),
                Size = new Size(300, 90),
                Padding = new Padding(25, 20, 25, 20)
            };

            // Company logo
            logoIconPic = new IconPictureBox
            {
                IconChar = IconChar.Microchip,
                IconColor = Color.FromArgb(56, 189, 248), // sky-400
                IconSize = 40,
                Location = new Point(25, 20),
                Size = new Size(50, 50),
                BackColor = Color.Transparent
            };
            brandPanel.Controls.Add(logoIconPic);

            // Brand name
            var brandLabel = new Label
            {
                Text = "SyncVerse",
                Font = new Font("Segoe UI", 22F, FontStyle.Bold),
                ForeColor = Color.FromArgb(248, 250, 252), // slate-50
                Location = new Point(85, 22),
                Size = new Size(190, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };
            brandPanel.Controls.Add(brandLabel);

            // Tagline
            var taglineLabel = new Label
            {
                Text = "POS STUDIO",
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = Color.FromArgb(148, 163, 184), // slate-400
                Location = new Point(85, 50),
                Size = new Size(190, 15),
                TextAlign = ContentAlignment.MiddleLeft
            };
            brandPanel.Controls.Add(taglineLabel);

            sidebarPanel.Controls.Add(brandPanel);
            yPos += 90;

            // Gradient separator
            var separator = new Panel
            {
                BackColor = Color.FromArgb(56, 189, 248),
                Location = new Point(25, yPos),
                Size = new Size(250, 3),
                Paint = (s, e) =>
                {
                    using (var brush = new LinearGradientBrush(
                        new Rectangle(0, 0, 250, 3),
                        Color.FromArgb(56, 189, 248),
                        Color.FromArgb(147, 51, 234),
                        LinearGradientMode.Horizontal))
                    {
                        e.Graphics.FillRectangle(brush, 0, 0, 250, 3);
                    }
                }
            };
            sidebarPanel.Controls.Add(separator);
            yPos += 20;
        }

        private void CreateUserProfileCard(User user, ref int yPos)
        {
            // Modern user card with glass morphism effect
            userInfoPanel = new Panel
            {
                BackColor = Color.FromArgb(51, 65, 85), // slate-600 with transparency
                Location = new Point(20, yPos),
                Size = new Size(260, 100)
            };

            userInfoPanel.Paint += (s, e) =>
            {
                var rect = new Rectangle(0, 0, 260, 100);
                
                // Background with rounded corners effect
                using (var brush = new SolidBrush(Color.FromArgb(51, 65, 85)))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }
                
                // Subtle inner glow
                using (var pen = new Pen(Color.FromArgb(30, 100, 149, 237), 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, rect.Width - 1, rect.Height - 1);
                }
            };

            // User avatar with role-specific styling
            userIconPic = new IconPictureBox
            {
                IconChar = GetUserRoleIcon(user.Role),
                IconColor = GetModernRoleColor(user.Role),
                IconSize = 32,
                Location = new Point(20, 20),
                Size = new Size(40, 40),
                BackColor = Color.Transparent
            };
            userInfoPanel.Controls.Add(userIconPic);

            // User name
            userLabel = new Label
            {
                Text = user.FullName,
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.FromArgb(248, 250, 252), // slate-50
                Location = new Point(70, 22),
                Size = new Size(170, 22),
                AutoEllipsis = true
            };
            userInfoPanel.Controls.Add(userLabel);

            // Role with modern badge
            var roleContainer = new Panel
            {
                BackColor = GetModernRoleColor(user.Role),
                Location = new Point(70, 50),
                Size = new Size(100, 24)
            };

            roleContainer.Paint += (s, e) =>
            {
                using (var brush = new SolidBrush(GetModernRoleColor(user.Role)))
                {
                    e.Graphics.FillRectangle(brush, 0, 0, 100, 24);
                }
            };

            roleLabel = new Label
            {
                Text = user.Role.ToString().ToUpper(),
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(8, 4),
                Size = new Size(84, 16),
                TextAlign = ContentAlignment.MiddleCenter
            };
            roleContainer.Controls.Add(roleLabel);
            userInfoPanel.Controls.Add(roleContainer);

            // Online status indicator
            var statusDot = new Panel
            {
                BackColor = Color.FromArgb(34, 197, 94), // green-500
                Location = new Point(180, 55),
                Size = new Size(8, 8)
            };

            statusDot.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var brush = new SolidBrush(Color.FromArgb(34, 197, 94)))
                {
                    e.Graphics.FillEllipse(brush, 0, 0, 8, 8);
                }
            };

            userInfoPanel.Controls.Add(statusDot);

            var statusText = new Label
            {
                Text = "ONLINE",
                Font = new Font("Segoe UI", 7F, FontStyle.Bold),
                ForeColor = Color.FromArgb(34, 197, 94),
                Location = new Point(195, 52),
                Size = new Size(50, 14)
            };
            userInfoPanel.Controls.Add(statusText);

            sidebarPanel.Controls.Add(userInfoPanel);
            yPos += 120;
        }

        private void CreateNavigationMenu(UserRole role, ref int yPos)
        {
            // Navigation header
            var navHeader = new Label
            {
                Text = "NAVIGATION",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(148, 163, 184), // slate-400
                Location = new Point(30, yPos),
                Size = new Size(240, 25)
            };
            sidebarPanel.Controls.Add(navHeader);
            yPos += 40;

            // Create role-based menu
            switch (role)
            {
                case UserRole.Administrator:
                    CreateMenuSection("CORE", Color.FromArgb(59, 130, 246), ref yPos, new (string, IconChar, Action)[]
                    {
                        ("Dashboard", IconChar.ChartLine, () => SafeLoadChildForm(new DashboardView(_authService))),
                        ("Users", IconChar.Users, () => SafeLoadChildForm(new UserManagementView(_authService))),
                        ("Products", IconChar.Box, () => SafeLoadChildForm(new ProductManagementView(_authService)))
                    });

                    CreateMenuSection("BUSINESS", Color.FromArgb(34, 197, 94), ref yPos, new (string, IconChar, Action)[]
                    {
                        ("Point of Sale", IconChar.CashRegister, () => SafeLoadChildForm(new PointOfSaleView(_authService))),
                        ("Sales", IconChar.ChartLine, () => SafeLoadChildForm(new SalesView(_authService))),
                        ("Customers", IconChar.Users, () => SafeLoadChildForm(new CustomerManagementView(_authService)))
                    });

                    CreateMenuSection("INVENTORY", Color.FromArgb(168, 85, 247), ref yPos, new (string, IconChar, Action)[]
                    {
                        ("Categories", IconChar.Tags, () => SafeLoadChildForm(new CategoryManagementView(_authService))),
                        ("Suppliers", IconChar.Truck, () => SafeLoadChildForm(new SupplierManagementView(_authService))),
                        ("Reports", IconChar.ChartBar, () => SafeLoadChildForm(new InventoryReportsView(_authService)))
                    });

                    CreateMenuSection("SYSTEM", Color.FromArgb(245, 158, 11), ref yPos, new (string, IconChar, Action)[]
                    {
                        ("Analytics", IconChar.ChartPie, () => SafeLoadChildForm(new ReportsView(_authService))),
                        ("Audit Logs", IconChar.FileText, () => SafeLoadChildForm(new AuditLogView(_authService)))
                    });
                    break;

                case UserRole.Cashier:
                    CreateMenuSection("SALES", Color.FromArgb(34, 197, 94), ref yPos, new (string, IconChar, Action)[]
                    {
                        ("Dashboard", IconChar.ChartLine, () => SafeLoadChildForm(new DashboardView(_authService))),
                        ("Point of Sale", IconChar.CashRegister, () => SafeLoadChildForm(new PointOfSaleView(_authService))),
                        ("Sales History", IconChar.Receipt, () => SafeLoadChildForm(new SalesView(_authService))),
                        ("Customers", IconChar.Users, () => SafeLoadChildForm(new CustomerManagementView(_authService)))
                    });
                    break;

                case UserRole.InventoryClerk:
                    CreateMenuSection("INVENTORY", Color.FromArgb(59, 130, 246), ref yPos, new (string, IconChar, Action)[]
                    {
                        ("Dashboard", IconChar.ChartLine, () => SafeLoadChildForm(new DashboardView(_authService))),
                        ("Products", IconChar.Box, () => SafeLoadChildForm(new ProductManagementView(_authService))),
                        ("Categories", IconChar.Tags, () => SafeLoadChildForm(new CategoryManagementView(_authService))),
                        ("Suppliers", IconChar.Truck, () => SafeLoadChildForm(new SupplierManagementView(_authService))),
                        ("Stock Reports", IconChar.ChartBar, () => SafeLoadChildForm(new InventoryReportsView(_authService)))
                    });
                    break;
            }
        }

        private void CreateMenuSection(string title, Color accentColor, ref int yPos, (string name, IconChar icon, Action action)[] items)
        {
            // Section header
            var sectionHeader = new Panel
            {
                BackColor = Color.FromArgb(30, accentColor.R, accentColor.G, accentColor.B),
                Location = new Point(20, yPos),
                Size = new Size(260, 28)
            };

            var headerLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = accentColor,
                Location = new Point(15, 6),
                Size = new Size(230, 16),
                TextAlign = ContentAlignment.MiddleLeft
            };
            sectionHeader.Controls.Add(headerLabel);
            sidebarPanel.Controls.Add(sectionHeader);
            yPos += 28;

            // Menu items
            foreach (var (name, icon, action) in items)
            {
                CreateModernMenuItem(name, icon, accentColor, yPos, action);
                yPos += 48;
            }

            yPos += 15; // Section spacing
        }

        private void CreateModernMenuItem(string text, IconChar icon, Color accentColor, int yPos, Action clickAction)
        {
            var menuItem = new IconButton
            {
                Text = $"      {text}",
                Font = new Font("Segoe UI", 12F, FontStyle.Regular),
                ForeColor = Color.FromArgb(226, 232, 240), // slate-200
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(20, yPos),
                Size = new Size(260, 44),
                Cursor = Cursors.Hand,
                IconChar = icon,
                IconColor = Color.FromArgb(148, 163, 184), // slate-400
                IconSize = 20,
                Padding = new Padding(20, 0, 20, 0),
                TextImageRelation = TextImageRelation.ImageBeforeText
            };

            menuItem.FlatAppearance.BorderSize = 0;
            
            // Modern hover and click effects
            menuItem.MouseEnter += (s, e) =>
            {
                menuItem.BackColor = Color.FromArgb(40, accentColor.R, accentColor.G, accentColor.B);
                menuItem.ForeColor = Color.White;
                menuItem.IconColor = Color.White;
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
                    // Reset all buttons
                    foreach (var btn in menuButtons)
                    {
                        btn.BackColor = Color.Transparent;
                        btn.ForeColor = Color.FromArgb(226, 232, 240);
                        btn.IconColor = Color.FromArgb(148, 163, 184);
                        btn.Tag = null;
                    }

                    // Activate selected button
                    menuItem.BackColor = Color.FromArgb(80, accentColor.R, accentColor.G, accentColor.B);
                    menuItem.ForeColor = Color.White;
                    menuItem.IconColor = Color.White;
                    menuItem.Tag = "selected";

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
            // Modern logout button
            logoutButton = new IconButton
            {
                BackColor = Color.FromArgb(239, 68, 68), // red-500
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                IconChar = IconChar.ArrowRightFromBracket,
                IconColor = Color.White,
                IconSize = 18,
                Text = "      LOGOUT",
                TextAlign = ContentAlignment.MiddleLeft,
                Size = new Size(260, 50),
                Location = new Point(20, this.ClientSize.Height - 90),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                Cursor = Cursors.Hand,
                Padding = new Padding(20, 0, 20, 0),
                TextImageRelation = TextImageRelation.ImageBeforeText
            };

            logoutButton.FlatAppearance.BorderSize = 0;
            logoutButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(220, 38, 38); // red-600

            logoutButton.Click += LogoutButton_Click;

            // Update position on resize
            this.Resize += (s, e) =>
            {
                if (logoutButton != null)
                    logoutButton.Location = new Point(20, this.ClientSize.Height - 90);
            };

            sidebarPanel.Controls.Add(logoutButton);

            // Version info
            var versionLabel = new Label
            {
                Text = "v2.1.0 • SyncVerse Studio",
                Font = new Font("Segoe UI", 8F, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 116, 139), // slate-500
                Location = new Point(30, this.ClientSize.Height - 35),
                Size = new Size(240, 15),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                TextAlign = ContentAlignment.MiddleLeft
            };

            this.Resize += (s, e) =>
            {
                if (versionLabel != null)
                    versionLabel.Location = new Point(30, this.ClientSize.Height - 35);
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
                firstButton.BackColor = Color.FromArgb(80, GetModernRoleColor(role).R, GetModernRoleColor(role).G, GetModernRoleColor(role).B);
                firstButton.ForeColor = Color.White;
                firstButton.IconColor = Color.White;
                firstButton.Tag = "selected";
            }
            
            SafeLoadChildForm(new DashboardView(_authService));
        }

        private void SafeLoadChildForm(Form childForm)
        {
            try
            {
                LoadChildForm(childForm);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading form: {ex.Message}", "Form Loading Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                try { childForm?.Dispose(); } catch { }
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
                MessageBox.Show($"Error during logout: {ex.Message}", "Logout Error",
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
    }
}