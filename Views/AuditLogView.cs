using SyncVerseStudio.Services;

namespace SyncVerseStudio.Views
{
    public partial class AuditLogView : Form
    {
        private readonly AuthenticationService _authService;

        public AuditLogView(AuthenticationService authService)
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
            this.Name = "AuditLogView";
            this.Text = "Audit Logs";

            var titleLabel = new Label
            {
                Text = "Audit Logs",
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                Location = new Point(20, 20),
                Size = new Size(400, 40),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(titleLabel);

            var comingSoonLabel = new Label
            {
                Text = "Audit Logs functionality coming soon...",
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