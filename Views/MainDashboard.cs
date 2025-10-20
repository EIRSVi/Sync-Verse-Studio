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

            // Logout Button
            this.logoutButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.logoutButton.BackColor = Color.FromArgb(255, 0, 80);
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

            // Sidebar Panel
            this.sidebarPanel.BackColor = Color.FromArgb(33, 33, 33);
            this.sidebarPanel.Dock = DockStyle.Left;
            this.sidebarPanel.Width = 250;
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
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, yPos),
                Size = new Size(200, 40),
                TextAlign = ContentAlignment.MiddleLeft
            };
            sidebarPanel.Controls.Add(titleLabel);
            yPos += 60;

            // Menu items based on role
            switch (role)
            {
                case UserRole.Administrator:
                    AddMenuItem("Dashboard", FontAwesome.Sharp.IconChar.ChartLine, yPos, () => LoadChildForm(new DashboardView(_authService)));
                    yPos += 50;
                    AddMenuItem("Users", FontAwesome.Sharp.IconChar.Users, yPos, () => LoadChildForm(new UserManagementView(_authService)));
                    yPos += 50;
                    AddMenuItem("Products", FontAwesome.Sharp.IconChar.Box, yPos, () => LoadChildForm(new ProductManagementView(_authService)));
                    yPos += 50;
                    AddMenuItem("Categories", FontAwesome.Sharp.IconChar.Tags, yPos, () => LoadChildForm(new CategoryManagementView(_authService)));
                    yPos += 50;
                    AddMenuItem("Suppliers", FontAwesome.Sharp.IconChar.Truck, yPos, () => LoadChildForm(new SupplierManagementView(_authService)));
                    yPos += 50;
                    AddMenuItem("Sales", FontAwesome.Sharp.IconChar.CashRegister, yPos, () => LoadChildForm(new SalesView(_authService)));
                    yPos += 50;
                    AddMenuItem("Reports", FontAwesome.Sharp.IconChar.ChartBar, yPos, () => LoadChildForm(new ReportsView(_authService)));
                    yPos += 50;
                    AddMenuItem("Audit Logs", FontAwesome.Sharp.IconChar.History, yPos, () => LoadChildForm(new AuditLogView(_authService)));
                    break;

                case UserRole.Cashier:
                    AddMenuItem("Dashboard", FontAwesome.Sharp.IconChar.ChartLine, yPos, () => LoadChildForm(new DashboardView(_authService)));
                    yPos += 50;
                    AddMenuItem("Point of Sale", FontAwesome.Sharp.IconChar.CashRegister, yPos, () => LoadChildForm(new PointOfSaleView(_authService)));
                    yPos += 50;
                    AddMenuItem("Customers", FontAwesome.Sharp.IconChar.UserFriends, yPos, () => LoadChildForm(new CustomerManagementView(_authService)));
                    yPos += 50;
                    AddMenuItem("Sales History", FontAwesome.Sharp.IconChar.Receipt, yPos, () => LoadChildForm(new SalesView(_authService)));
                    break;

                case UserRole.InventoryClerk:
                    AddMenuItem("Dashboard", FontAwesome.Sharp.IconChar.ChartLine, yPos, () => LoadChildForm(new DashboardView(_authService)));
                    yPos += 50;
                    AddMenuItem("Products", FontAwesome.Sharp.IconChar.Box, yPos, () => LoadChildForm(new ProductManagementView(_authService)));
                    yPos += 50;
                    AddMenuItem("Categories", FontAwesome.Sharp.IconChar.Tags, yPos, () => LoadChildForm(new CategoryManagementView(_authService)));
                    yPos += 50;
                    AddMenuItem("Suppliers", FontAwesome.Sharp.IconChar.Truck, yPos, () => LoadChildForm(new SupplierManagementView(_authService)));
                    yPos += 50;
                    AddMenuItem("Inventory", FontAwesome.Sharp.IconChar.Warehouse, yPos, () => LoadChildForm(new InventoryView(_authService)));
                    yPos += 50;
                    AddMenuItem("Stock Reports", FontAwesome.Sharp.IconChar.ChartArea, yPos, () => LoadChildForm(new InventoryReportsView(_authService)));
                    break;
            }
        }

        private void AddMenuItem(string text, FontAwesome.Sharp.IconChar icon, int yPos, Action clickAction)
        {
            var menuButton = new Button
            {
                Text = $"  {text}",
                Font = new Font("Segoe UI", 11F, FontStyle.Regular),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(0, yPos),
                Size = new Size(250, 45),
                Cursor = Cursors.Hand
            };

            menuButton.FlatAppearance.BorderSize = 0;
            menuButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(24, 119, 18);

            // Create icon
            var iconLabel = new FontAwesome.Sharp.IconButton
            {
                IconChar = icon,
                IconColor = Color.White,
                IconSize = 20,
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(15, yPos + 12),
                Size = new Size(20, 20),
                Enabled = false
            };

            iconLabel.FlatAppearance.BorderSize = 0;

            menuButton.Click += (s, e) => clickAction();

            sidebarPanel.Controls.Add(menuButton);
            sidebarPanel.Controls.Add(iconLabel);
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