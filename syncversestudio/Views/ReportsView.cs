using System.Drawing;
using SyncVerseStudio.Services;

namespace SyncVerseStudio.Views
{
    public partial class ReportsView : Form
    {
        private readonly AuthenticationService _authService;

        public ReportsView(AuthenticationService authService)
        {
            _authService = authService;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            this.Name = "ReportsView";
            this.Text = "Reports & Analytics";
            this.WindowState = FormWindowState.Normal;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.FromArgb(250, 250, 250);
            this.ClientSize = new Size(1000, 700);
            
            var titleLabel = new Label();
            titleLabel.Text = "?? Reports & Analytics";
            titleLabel.Font = new System.Drawing.Font("Segoe UI", 20F, FontStyle.Bold);
            titleLabel.ForeColor = System.Drawing.Color.FromArgb(33, 33, 33);
            titleLabel.SetBounds(20, 20, 400, 40);
            this.Controls.Add(titleLabel);

            var featuresLabel = new Label();
            featuresLabel.Text = "?? Comprehensive Reporting Suite - Coming Soon:\n\n" +
                                "?? Sales Analytics & Trends\n" +
                                "?? Revenue & Profit Reports\n" +
                                "?? Inventory Performance Analysis\n" +
                                "?? Customer Behavior Insights\n" +
                                "?? Staff Performance Metrics\n" +
                                "?? Business Intelligence Dashboard\n" +
                                "??? Daily, Weekly, Monthly Reports\n" +
                                "?? Top Products & Categories\n" +
                                "?? Loss Prevention Analytics\n" +
                                "?? Comparative Period Analysis\n" +
                                "?? Export to PDF, Excel, CSV";
            featuresLabel.Font = new System.Drawing.Font("Segoe UI", 12F);
            featuresLabel.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
            featuresLabel.SetBounds(20, 80, 600, 280);
            this.Controls.Add(featuresLabel);
            
            this.ResumeLayout(false);
        }
    }
}