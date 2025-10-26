using System;
using System.Drawing;
using System.Windows.Forms;
using SyncVerseStudio.Services;

namespace SyncVerseStudio.Views
{
    public partial class OnlineStoreView : Form
    {
        private readonly AuthenticationService _authService;

        public OnlineStoreView(AuthenticationService authService)
        {
            _authService = authService;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(248, 250, 252);
            this.Dock = DockStyle.Fill;

            var titleLabel = new Label
            {
                Text = "Online Store Integration",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(30, 30),
                AutoSize = true
            };

            var comingSoonLabel = new Label
            {
                Text = "Coming Soon - Online Store Sync Features",
                Font = new Font("Segoe UI", 14),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(30, 80),
                AutoSize = true
            };

            this.Controls.AddRange(new Control[] { titleLabel, comingSoonLabel });

            this.ResumeLayout(false);
        }
    }
}
