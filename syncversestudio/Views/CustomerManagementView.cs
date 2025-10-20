using System.Drawing;
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
            this.SuspendLayout();
            
            this.Name = "CustomerManagementView";
            this.Text = "Customer Management";
            this.WindowState = FormWindowState.Normal;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.FromArgb(250, 250, 250);
            this.ClientSize = new Size(1000, 700);
            
            var titleLabel = new Label();
            titleLabel.Text = "?? Customer Management";
            titleLabel.Font = new System.Drawing.Font("Segoe UI", 20F, FontStyle.Bold);
            titleLabel.ForeColor = System.Drawing.Color.FromArgb(33, 33, 33);
            titleLabel.SetBounds(20, 20, 400, 40);
            this.Controls.Add(titleLabel);

            var featuresLabel = new Label();
            featuresLabel.Text = "?? Coming Soon - Full Customer CRUD Operations:\n\n" +
                                "? Customer Database Management\n" +
                                "? Contact Information & Profiles\n" +
                                "? Purchase History Tracking\n" +
                                "? Loyalty Points System\n" +
                                "? Advanced Search & Filtering\n" +
                                "? Customer Analytics & Reports\n" +
                                "? Email & SMS Integration\n" +
                                "? Customer Segmentation";
            featuresLabel.Font = new System.Drawing.Font("Segoe UI", 12F);
            featuresLabel.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
            featuresLabel.SetBounds(20, 80, 600, 200);
            this.Controls.Add(featuresLabel);
            
            this.ResumeLayout(false);
        }
    }
}