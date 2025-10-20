using SyncVerseStudio.Services;

namespace SyncVerseStudio.Views
{
    public partial class CustomerManagementView : Form
    {
        private readonly AuthenticationService _authService;

        public CustomerManagementView(AuthenticationService authService)
        {
            _authService = authService;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.AutoScaleDimensions = new SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(250, 250, 250);
            this.ClientSize = new Size(1000, 700);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Name = "CustomerManagementView";
            this.Text = "Customer Management";

            var titleLabel = new Label
            {
                Text = "Customer Management",
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                Location = new Point(20, 20),
                Size = new Size(400, 40),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(titleLabel);

            var comingSoonLabel = new Label
            {
                Text = "Customer Management functionality coming soon...",
                Font = new Font("Segoe UI", 12F),
                ForeColor = Color.FromArgb(117, 117, 117),
                Location = new Point(20, 80),
                Size = new Size(600, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(comingSoonLabel);
        }
    }
}