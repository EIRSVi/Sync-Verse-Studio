using System.Drawing;
using SyncVerseStudio.Services;

namespace SyncVerseStudio.Views
{
    public partial class InventoryView : Form
    {
        private readonly AuthenticationService _authService;

        public InventoryView(AuthenticationService authService)
        {
            _authService = authService;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            this.Name = "InventoryView";
            this.Text = "Inventory Management";
            this.WindowState = FormWindowState.Normal;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.FromArgb(250, 250, 250);
            this.ClientSize = new Size(1000, 700);
            
            var titleLabel = new Label();
            titleLabel.Text = "?? Inventory Management";
            titleLabel.Font = new System.Drawing.Font("Segoe UI", 20F, FontStyle.Bold);
            titleLabel.ForeColor = System.Drawing.Color.FromArgb(33, 33, 33);
            titleLabel.SetBounds(20, 20, 400, 40);
            this.Controls.Add(titleLabel);

            var featuresLabel = new Label();
            featuresLabel.Text = "?? Advanced Inventory Features - Coming Soon:\n\n" +
                                "? Real-time Stock Tracking\n" +
                                "? Inventory Adjustments & Corrections\n" +
                                "? Stock Movement History\n" +
                                "? Low Stock Alerts & Notifications\n" +
                                "? Batch & Serial Number Tracking\n" +
                                "? Multi-location Inventory\n" +
                                "? Automated Reorder Points\n" +
                                "? Cycle Counting & Audits\n" +
                                "? Waste & Loss Tracking\n" +
                                "? Inventory Valuation Reports";
            featuresLabel.Font = new System.Drawing.Font("Segoe UI", 12F);
            featuresLabel.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
            featuresLabel.SetBounds(20, 80, 600, 250);
            this.Controls.Add(featuresLabel);
            
            this.ResumeLayout(false);
        }
    }
}
