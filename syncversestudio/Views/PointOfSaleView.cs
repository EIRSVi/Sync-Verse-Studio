using System.Drawing;
using SyncVerseStudio.Services;

namespace SyncVerseStudio.Views
{
    public partial class PointOfSaleView : Form
    {
        private readonly AuthenticationService _authService;

        public PointOfSaleView(AuthenticationService authService)
        {
            _authService = authService;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            this.Name = "PointOfSaleView";
            this.Text = "Point of Sale";
            this.WindowState = FormWindowState.Normal;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.FromArgb(250, 250, 250);
            this.ClientSize = new Size(1000, 700);
            
            var titleLabel = new Label();
            titleLabel.Text = "?? Point of Sale System";
            titleLabel.Font = new System.Drawing.Font("Segoe UI", 20F, FontStyle.Bold);
            titleLabel.ForeColor = System.Drawing.Color.FromArgb(33, 33, 33);
            titleLabel.SetBounds(20, 20, 400, 40);
            this.Controls.Add(titleLabel);

            var featuresLabel = new Label();
            featuresLabel.Text = "?? Advanced POS Features - Coming Soon:\n\n" +
                                "? Product Search & Barcode Scanning\n" +
                                "? Shopping Cart Management\n" +
                                "? Multiple Payment Methods\n" +
                                "? Discount & Promotion Handling\n" +
                                "? Receipt Generation & Printing\n" +
                                "? Customer Lookup & Selection\n" +
                                "? Real-time Inventory Updates\n" +
                                "? Split Payments & Refunds\n" +
                                "? Tax Calculations\n" +
                                "? Cash Drawer Integration";
            featuresLabel.Font = new System.Drawing.Font("Segoe UI", 12F);
            featuresLabel.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
            featuresLabel.SetBounds(20, 80, 600, 250);
            this.Controls.Add(featuresLabel);
            
            this.ResumeLayout(false);
        }
    }
}