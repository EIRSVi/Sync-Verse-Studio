using SyncVerseStudio.Services;
using SyncVerseStudio.Models;

namespace SyncVerseStudio.Views
{
    public partial class MainDashboard : Form
    {
        private readonly AuthenticationService _authService;
        private Panel sidebarPanel;
        private Panel topPanel;
        private Panel contentPanel;
        private Label welcomeLabel;
        private Label roleLabel;
        private Button logoutButton;
        private Form? _currentChildForm;
        private Button? _activeMenuButton;

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
            this.welcomeLabel = new Label();
            this.roleLabel = new Label();
            this.logoutButton = new Button();

            this.SuspendLayout();

            // Main Form
            this.AutoScaleDimensions = new SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(245, 245, 245);
            this.ClientSize = new Size(1400, 800);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Name = "MainDashboard";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "SyncVerse Studio - Dashboard";
            this.WindowState = FormWindowState.Maximized;

            // Top Panel
            this.topPanel.BackColor = Color.White;
            this.topPanel.Dock = DockStyle.Top;
            this.topPanel.Height = 70;
            this.topPanel.Padding = new Padding(20, 10);
            
            // Welcome Label
            this.welcomeLabel.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            this.welcomeLabel.ForeColor = Color.FromArgb(33, 33, 33);
            this.welcomeLabel.Location = new Point(20, 15);
            this.welcomeLabel.Size = new Size(400, 30);
            this.welcomeLabel.Text = "Welcome, User";

            // Role Label
            this.roleLabel.Font = new Font("Segoe UI", 10F);
            this.roleLabel.ForeColor = Color.FromArgb(117, 117, 117);
            this.roleLabel.Location = new Point(20, 40);
            this.roleLabel.Size = new Size(200, 20);
            this.roleLabel.Text = "Role";

            // Logout Button - Text only
            this.logoutButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.logoutButton.BackColor = Color.FromArgb(220, 38, 127);
            this.logoutButton.FlatStyle = FlatStyle.Flat;
            this.logoutButton.FlatAppearance.BorderSize = 0;
            this.logoutButton.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.logoutButton.ForeColor = Color.White;
            this.logoutButton.Location = new Point(this.Width - 120, 20);
            this.logoutButton.Size = new Size(80, 30);
            this.logoutButton.Text = "Logout";
            this.logoutButton.UseVisualStyleBackColor = false;
            this.logoutButton.Click += LogoutButton_Click;

            this.topPanel.Controls.Add(this.welcomeLabel);
            this.topPanel.Controls.Add(this.roleLabel);
            this.topPanel.Controls.Add(this.logoutButton);

            // Sidebar Panel - solid color, no gradient
            this.sidebarPanel.BackColor = Color.FromArgb(51, 65, 85);
            this.sidebarPanel.Dock = DockStyle.Left;
            this.sidebarPanel.Width = 280;
            this.sidebarPanel.Padding = new Padding(0, 20);

            // Content Panel
            this.contentPanel.BackColor = Color.FromArgb(250, 250, 250);
            this.contentPanel.Dock = DockStyle.Fill;
            this.contentPanel.Padding = new Padding(20);

            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.sidebarPanel);
            this.Controls.Add(this.topPanel);

            this.ResumeLayout(false);
        }

        private void LoadUserInterface()
        {
            var user = _authService.CurrentUser;
            if (user == null) return;

            welcomeLabel.Text = $"Welcome, {user.FullName}";
            roleLabel.Text = user.Role.ToString();

            CreateSidebarMenu(user.Role);
            LoadDefaultView(user.Role);
        }

        private void CreateSidebarMenu(UserRole role)
        {
            sidebarPanel.Controls.Clear();
            int yPos = 20;

            // Logo/Title
            var titleLabel = new Label
            {
                Text = "SyncVerse",
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, yPos),
                Size = new Size(240, 40),
                TextAlign = ContentAlignment.MiddleLeft
            };
            sidebarPanel.Controls.Add(titleLabel);
            
            var subtitleLabel = new Label
            {
                Text = "POS SYSTEM",
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(20, yPos + 35),
                Size = new Size(240, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };
            sidebarPanel.Controls.Add(subtitleLabel);
            yPos += 80;

            // Navigation section
            var navLabel = new Label
            {
                Text = "NAVIGATION",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(20, yPos),
                Size = new Size(240, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };
            sidebarPanel.Controls.Add(navLabel);
            yPos += 30;

            // Menu items based on role
            switch (role)
            {
                case UserRole.Administrator:
                    AddMenuItem("Dashboard", FontAwesome.Sharp.IconChar.ChartLine, yPos, () => LoadChildForm(new DashboardView(_authService)), true);
                    yPos += 50;
                    AddMenuItem("Users", FontAwesome.Sharp.IconChar.Users, yPos, () => LoadChildForm(new UserManagementView(_authService)));
                    yPos += 50;
                    AddMenuItem("Products", FontAwesome.Sharp.IconChar.Box, yPos, () => LoadChildForm(new ProductManagementView(_authService)));
                    yPos += 50;
                    AddMenuItem("Point of Sale", FontAwesome.Sharp.IconChar.CashRegister, yPos, () => LoadChildForm(new PointOfSaleView(_authService)));
                    yPos += 50;
                    AddMenuItem("Sales Management", FontAwesome.Sharp.IconChar.ChartArea, yPos, () => LoadChildForm(new SalesManagementView(_authService)));
                    yPos += 50;
                    AddMenuItem("Customer Management", FontAwesome.Sharp.IconChar.UserFriends, yPos, () => LoadChildForm(new CustomerManagementView(_authService)));
                    yPos += 50;
                    AddMenuItem("Categories", FontAwesome.Sharp.IconChar.Tags, yPos, () => LoadChildForm(new CategoryManagementView(_authService)));
                    yPos += 50;
                    AddMenuItem("Suppliers", FontAwesome.Sharp.IconChar.Truck, yPos, () => LoadChildForm(new SupplierManagementView(_authService)));
                    yPos += 50;
                    AddMenuItem("Reports", FontAwesome.Sharp.IconChar.ChartBar, yPos, () => LoadChildForm(new ReportsView(_authService)));
                    yPos += 50;
                    AddMenuItem("Analytics", FontAwesome.Sharp.IconChar.ChartPie, yPos, () => LoadChildForm(new AnalyticsView(_authService)));
                    yPos += 50;
                    AddMenuItem("Audit Logs", FontAwesome.Sharp.IconChar.History, yPos, () => LoadChildForm(new AuditLogView(_authService)));
                    break;

                case UserRole.Cashier:
                    AddMenuItem("Dashboard", FontAwesome.Sharp.IconChar.ChartLine, yPos, () => LoadChildForm(new DashboardView(_authService)), true);
                    yPos += 50;
                    AddMenuItem("Point of Sale", FontAwesome.Sharp.IconChar.CashRegister, yPos, () => LoadChildForm(new PointOfSaleView(_authService)));
                    yPos += 50;
                    AddMenuItem("Sales Management", FontAwesome.Sharp.IconChar.ChartArea, yPos, () => LoadChildForm(new SalesManagementView(_authService)));
                    yPos += 50;
                    AddMenuItem("Customer Management", FontAwesome.Sharp.IconChar.UserFriends, yPos, () => LoadChildForm(new CustomerManagementView(_authService)));
                    yPos += 50;
                    AddMenuItem("Reports", FontAwesome.Sharp.IconChar.ChartBar, yPos, () => LoadChildForm(new ReportsView(_authService)));
                    break;

                case UserRole.InventoryClerk:
                    AddMenuItem("Dashboard", FontAwesome.Sharp.IconChar.ChartLine, yPos, () => LoadChildForm(new DashboardView(_authService)), true);
                    yPos += 50;
                    AddMenuItem("Products", FontAwesome.Sharp.IconChar.Box, yPos, () => LoadChildForm(new ProductManagementView(_authService)));
                    yPos += 50;
                    AddMenuItem("Categories", FontAwesome.Sharp.IconChar.Tags, yPos, () => LoadChildForm(new CategoryManagementView(_authService)));
                    yPos += 50;
                    AddMenuItem("Suppliers", FontAwesome.Sharp.IconChar.Truck, yPos, () => LoadChildForm(new SupplierManagementView(_authService)));
                    yPos += 50;
                    AddMenuItem("Inventory", FontAwesome.Sharp.IconChar.Warehouse, yPos, () => LoadChildForm(new InventoryView(_authService)));
                    yPos += 50;
                    AddMenuItem("Reports", FontAwesome.Sharp.IconChar.ChartBar, yPos, () => LoadChildForm(new ReportsView(_authService)));
                    break;
            }
        }

        private void AddMenuItem(string text, FontAwesome.Sharp.IconChar icon, int yPos, Action clickAction, bool isActive = false)
        {
            // Create the main menu button container
            var menuButton = new Button
            {
                Text = "", // No text on button itself
                Font = new Font("Segoe UI", 11F, isActive ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = isActive ? Color.White : Color.FromArgb(203, 213, 225),
                BackColor = isActive ? Color.FromArgb(59, 130, 246) : Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(10, yPos),
                Size = new Size(260, 45),
                Cursor = Cursors.Hand,
                Tag = text // Store the text for later reference
            };

            menuButton.FlatAppearance.BorderSize = 0;
            menuButton.FlatAppearance.MouseOverBackColor = isActive ? Color.FromArgb(59, 130, 246) : Color.FromArgb(71, 85, 105);

            // Create icon
            var iconButton = new FontAwesome.Sharp.IconButton
            {
                IconChar = icon,
                IconColor = isActive ? Color.White : Color.FromArgb(203, 213, 225),
                IconSize = 20,
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(25, yPos + 12),
                Size = new Size(24, 24),
                Enabled = false,
                TabStop = false
            };
            iconButton.FlatAppearance.BorderSize = 0;

            // Create text label
            var textLabel = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 11F, isActive ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = isActive ? Color.White : Color.FromArgb(203, 213, 225),
                BackColor = Color.Transparent,
                Location = new Point(55, yPos + 12),
                Size = new Size(200, 21),
                TextAlign = ContentAlignment.MiddleLeft,
                UseMnemonic = false
            };

            // Add mouse events to all components for consistent behavior
            menuButton.Click += (s, e) => 
            {
                SetActiveMenuItem(menuButton, iconButton, textLabel, text);
                clickAction();
            };

            menuButton.MouseEnter += (s, e) =>
            {
                if (!isActive)
                {
                    menuButton.BackColor = Color.FromArgb(71, 85, 105);
                    iconButton.IconColor = Color.White;
                    textLabel.ForeColor = Color.White;
                }
            };

            menuButton.MouseLeave += (s, e) =>
            {
                if (menuButton != _activeMenuButton)
                {
                    menuButton.BackColor = Color.Transparent;
                    iconButton.IconColor = Color.FromArgb(203, 213, 225);
                    textLabel.ForeColor = Color.FromArgb(203, 213, 225);
                }
            };

            // Store references for active state management
            menuButton.Tag = new { Text = text, Icon = iconButton, Label = textLabel };

            if (isActive)
            {
                _activeMenuButton = menuButton;
            }

            sidebarPanel.Controls.Add(menuButton);
            sidebarPanel.Controls.Add(iconButton);
            sidebarPanel.Controls.Add(textLabel);
        }

        private void SetActiveMenuItem(Button selectedButton, FontAwesome.Sharp.IconButton iconButton, Label textLabel, string text)
        {
            // Reset previous active button
            if (_activeMenuButton != null)
            {
                var prevData = _activeMenuButton.Tag as dynamic;
                if (prevData != null)
                {
                    _activeMenuButton.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
                    _activeMenuButton.ForeColor = Color.FromArgb(203, 213, 225);
                    _activeMenuButton.BackColor = Color.Transparent;
                    
                    prevData.Icon.IconColor = Color.FromArgb(203, 213, 225);
                    prevData.Label.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
                    prevData.Label.ForeColor = Color.FromArgb(203, 213, 225);
                }
            }

            // Set new active button
            selectedButton.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            selectedButton.ForeColor = Color.White;
            selectedButton.BackColor = Color.FromArgb(59, 130, 246);
            
            iconButton.IconColor = Color.White;
            textLabel.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            textLabel.ForeColor = Color.White;
            
            _activeMenuButton = selectedButton;
        }

        private void LoadDefaultView(UserRole role)
        {
            LoadChildForm(new DashboardView(_authService));
        }

        private void LoadChildForm(Form childForm)
        {
            _currentChildForm?.Close();
            _currentChildForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            contentPanel.Controls.Clear();
            contentPanel.Controls.Add(childForm);
            childForm.Show();
        }

        private void LogoutButton_Click(object sender, EventArgs e)
        {
            _authService.Logout();
            this.Close();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _currentChildForm?.Close();
            base.OnFormClosed(e);
        }
    }
}