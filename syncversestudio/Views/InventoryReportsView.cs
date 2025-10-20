using System.Drawing;
using SyncVerseStudio.Services;

namespace SyncVerseStudio.Views
{
    public partial class InventoryReportsView : Form
    {
        private readonly AuthenticationService _authService;

        public InventoryReportsView(AuthenticationService authService)
        {
            _authService = authService;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            this.Name = "InventoryReportsView";
            this.Text = "Inventory Reports";
            this.WindowState = FormWindowState.Normal;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.FromArgb(250, 250, 250);
            this.ClientSize = new Size(1000, 700);
            
            var titleLabel = new Label();
            titleLabel.Text = "?? Inventory Reports & Analytics";
            titleLabel.Font = new System.Drawing.Font("Segoe UI", 20F, FontStyle.Bold);
            titleLabel.ForeColor = System.Drawing.Color.FromArgb(33, 33, 33);
            titleLabel.SetBounds(20, 20, 500, 40);
            this.Controls.Add(titleLabel);

            var featuresLabel = new Label();
            featuresLabel.Text = "?? Advanced Inventory Reporting - Coming Soon:\n\n" +
                                "?? Stock Levels & Valuation Reports\n" +
                                "?? Low Stock & Reorder Alerts\n" +
                                "?? Stock Movement Analysis\n" +
                                "?? Inventory Turnover Metrics\n" +
                                "?? Cost Analysis & Margin Reports\n" +
                                "?? Slow Moving Stock Identification\n" +
                                "??? Category Performance Analysis\n" +
                                "?? Supplier Performance Reports\n" +
                                "?? ABC Analysis (Pareto)\n" +
                                "? Aging Reports & Dead Stock\n" +
                                "?? Exportable Reports (PDF/Excel)";
            featuresLabel.Font = new System.Drawing.Font("Segoe UI", 12F);
            featuresLabel.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
            featuresLabel.SetBounds(20, 80, 600, 280);
            this.Controls.Add(featuresLabel);
            
            this.ResumeLayout(false);
        }
    }
}