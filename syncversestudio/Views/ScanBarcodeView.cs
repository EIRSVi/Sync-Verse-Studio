using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using SyncVerseStudio.Data;
using SyncVerseStudio.Services;
using FontAwesome.Sharp;

namespace SyncVerseStudio.Views
{
    public partial class ScanBarcodeView : Form
    {
        private readonly AuthenticationService _authService;
        private readonly ApplicationDbContext _context;
        private TextBox txtBarcode;
        private Panel resultPanel;
        private Label lblProductName;
        private Label lblProductDetails;
        private Label lblStockInfo;
        private Label lblPriceInfo;
        private Button btnQuickUpdate;

        public ScanBarcodeView(AuthenticationService authService)
        {
            _authService = authService;
            _context = new ApplicationDbContext();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Name = "ScanBarcodeView";
            this.Text = "Scan Barcode";
            this.WindowState = FormWindowState.Normal;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.ClientSize = new Size(1200, 800);
            this.Padding = new Padding(0);

            CreateHeaderPanel();
            CreateContentPanel();

            this.ResumeLayout(false);
        }

        private void CreateHeaderPanel()
        {
            var headerPanel = new Panel
            {
                BackColor = Color.White,
                Dock = DockStyle.Top,
                Height = 80,
                Padding = new Padding(30, 20, 30, 20)
            };

            var iconBox = new IconPictureBox
            {
                IconChar = IconChar.Barcode,
                IconColor = Color.FromArgb(6, 182, 212),
                IconSize = 32,
                Location = new Point(30, 24),
                Size = new Size(32, 32),
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(iconBox);

            var titleLabel = new Label
            {
                Text = "Scan Barcode",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(70, 20),
                Size = new Size(400, 30),
                AutoSize = false,
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(titleLabel);

            var subtitleLabel = new Label
            {
                Text = "Quick product lookup and stock management",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(120, 120, 120),
                Location = new Point(70, 48),
                Size = new Size(400, 20),
                AutoSize = false,
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(subtitleLabel);

            var btnBack = new Button
            {
                Text = "← Back",
                Location = new Point(1080, 25),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(100, 116, 139),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F),
                Cursor = Cursors.Hand
            };
            btnBack.FlatAppearance.BorderSize = 0;
            btnBack.Click += (s, e) => this.Close();
            headerPanel.Controls.Add(btnBack);

            this.Controls.Add(headerPanel);
        }

        private void CreateContentPanel()
        {
            var contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(245, 247, 250),
                Padding = new Padding(30, 20, 30, 20),
                AutoScroll = true
            };

            // Scanner Card
            var scannerCard = new Panel
            {
                Location = new Point(30, 20),
                Size = new Size(700, 180),
                BackColor = Color.White
            };

            scannerCard.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, scannerCard.Width - 1, scannerCard.Height - 1), 12))
                using (var brush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillPath(brush, path);
                }
            };

            var lblInstruction = new Label
            {
                Text = "Enter Barcode or Product ID:",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(30, 30),
                Size = new Size(300, 25),
                BackColor = Color.Transparent
            };
            scannerCard.Controls.Add(lblInstruction);

            txtBarcode = new TextBox
            {
                Location = new Point(30, 70),
                Size = new Size(500, 40),
                Font = new Font("Segoe UI", 16F),
                PlaceholderText = "Scan or type barcode..."
            };
            txtBarcode.KeyPress += TxtBarcode_KeyPress;
            scannerCard.Controls.Add(txtBarcode);

            var btnSearch = new Button
            {
                Text = "Search",
                Location = new Point(550, 70),
                Size = new Size(120, 40),
                BackColor = Color.FromArgb(6, 182, 212),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.Click += BtnSearch_Click;
            scannerCard.Controls.Add(btnSearch);

            var lblTip = new Label
            {
                Text = "💡 Tip: Press Enter after scanning or typing",
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                ForeColor = Color.FromArgb(120, 120, 120),
                Location = new Point(30, 125),
                Size = new Size(400, 20),
                BackColor = Color.Transparent
            };
            scannerCard.Controls.Add(lblTip);

            contentPanel.Controls.Add(scannerCard);

            // Result Panel
            resultPanel = new Panel
            {
                Location = new Point(30, 220),
                Size = new Size(700, 450),
                BackColor = Color.White,
                Visible = false
            };

            resultPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, resultPanel.Width - 1, resultPanel.Height - 1), 12))
                using (var brush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillPath(brush, path);
                }
            };

            var lblResultTitle = new Label
            {
                Text = "Product Information",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(30, 30),
                Size = new Size(300, 30),
                BackColor = Color.Transparent
            };
            resultPanel.Controls.Add(lblResultTitle);

            lblProductName = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(6, 182, 212),
                Location = new Point(30, 80),
                Size = new Size(640, 35),
                BackColor = Color.Transparent
            };
            resultPanel.Controls.Add(lblProductName);

            lblProductDetails = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.FromArgb(60, 60, 60),
                Location = new Point(30, 125),
                Size = new Size(640, 60),
                BackColor = Color.Transparent
            };
            resultPanel.Controls.Add(lblProductDetails);

            lblStockInfo = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(34, 197, 94),
                Location = new Point(30, 200),
                Size = new Size(640, 30),
                BackColor = Color.Transparent
            };
            resultPanel.Controls.Add(lblStockInfo);

            lblPriceInfo = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.FromArgb(60, 60, 60),
                Location = new Point(30, 240),
                Size = new Size(640, 50),
                BackColor = Color.Transparent
            };
            resultPanel.Controls.Add(lblPriceInfo);

            // Quick Actions
            btnQuickUpdate = new Button
            {
                Text = "Quick Stock Update",
                Location = new Point(30, 320),
                Size = new Size(180, 40),
                BackColor = Color.FromArgb(34, 197, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnQuickUpdate.FlatAppearance.BorderSize = 0;
            btnQuickUpdate.Click += BtnQuickUpdate_Click;
            resultPanel.Controls.Add(btnQuickUpdate);

            var btnViewDetails = new Button
            {
                Text = "View Full Details",
                Location = new Point(230, 320),
                Size = new Size(160, 40),
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11F),
                Cursor = Cursors.Hand
            };
            btnViewDetails.FlatAppearance.BorderSize = 0;
            btnViewDetails.Click += (s, e) => MessageBox.Show("This would open the full product details view.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            resultPanel.Controls.Add(btnViewDetails);

            var btnScanAnother = new Button
            {
                Text = "Scan Another",
                Location = new Point(410, 320),
                Size = new Size(140, 40),
                BackColor = Color.FromArgb(100, 116, 139),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11F),
                Cursor = Cursors.Hand
            };
            btnScanAnother.FlatAppearance.BorderSize = 0;
            btnScanAnother.Click += (s, e) => { resultPanel.Visible = false; txtBarcode.Clear(); txtBarcode.Focus(); };
            resultPanel.Controls.Add(btnScanAnother);

            contentPanel.Controls.Add(resultPanel);
            this.Controls.Add(contentPanel);

            txtBarcode.Focus();
        }

        private void TxtBarcode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                BtnSearch_Click(sender, e);
            }
        }

        private async void BtnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtBarcode.Text.Trim();
            
            if (string.IsNullOrEmpty(searchTerm))
            {
                MessageBox.Show("Please enter a barcode or product ID.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Models.Product product = null;

                // Try to find by ID first
                if (int.TryParse(searchTerm, out int productId))
                {
                    product = await _context.Products
                        .Include(p => p.Category)
                        .Include(p => p.Supplier)
                        .FirstOrDefaultAsync(p => p.Id == productId && p.IsActive);
                }

                // If not found by ID, try by barcode (assuming Barcode field exists)
                if (product == null)
                {
                    product = await _context.Products
                        .Include(p => p.Category)
                        .Include(p => p.Supplier)
                        .FirstOrDefaultAsync(p => p.Barcode == searchTerm && p.IsActive);
                }

                // If still not found, try by name
                if (product == null)
                {
                    product = await _context.Products
                        .Include(p => p.Category)
                        .Include(p => p.Supplier)
                        .FirstOrDefaultAsync(p => p.Name.Contains(searchTerm) && p.IsActive);
                }

                if (product == null)
                {
                    MessageBox.Show($"No product found with barcode or ID: {searchTerm}", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                DisplayProductInfo(product);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching product: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayProductInfo(Models.Product product)
        {
            lblProductName.Text = product.Name;
            lblProductDetails.Text = $"ID: {product.Id} | Category: {product.Category?.Name ?? "N/A"} | Supplier: {product.Supplier?.Name ?? "N/A"}";
            
            string stockStatus = product.Quantity <= product.MinQuantity ? "⚠ LOW STOCK" : "✓ In Stock";
            Color stockColor = product.Quantity <= product.MinQuantity ? Color.FromArgb(239, 68, 68) : Color.FromArgb(34, 197, 94);
            lblStockInfo.Text = $"{stockStatus} - Current: {product.Quantity} units | Min: {product.MinQuantity} units";
            lblStockInfo.ForeColor = stockColor;

            lblPriceInfo.Text = $"Cost Price: ${product.CostPrice:N2} | Selling Price: ${product.SellingPrice:N2}\n" +
                               $"Profit Margin: ${(product.SellingPrice - product.CostPrice):N2} per unit";

            btnQuickUpdate.Tag = product;
            resultPanel.Visible = true;
        }

        private async void BtnQuickUpdate_Click(object sender, EventArgs e)
        {
            if (btnQuickUpdate.Tag is Models.Product product)
            {
                var dialog = new QuickStockUpdateDialog(product);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var dbProduct = await _context.Products.FindAsync(product.Id);
                        if (dbProduct != null)
                        {
                            dbProduct.Quantity = dialog.NewQuantity;
                            dbProduct.UpdatedAt = DateTime.Now;
                            await _context.SaveChangesAsync();

                            MessageBox.Show($"Stock updated successfully!\n\nProduct: {dbProduct.Name}\nNew Quantity: {dbProduct.Quantity}", 
                                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Refresh display
                            DisplayProductInfo(dbProduct);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating stock: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context?.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    public class QuickStockUpdateDialog : Form
    {
        public int NewQuantity { get; private set; }
        private NumericUpDown numQuantity;

        public QuickStockUpdateDialog(Models.Product product)
        {
            InitializeDialog(product);
        }

        private void InitializeDialog(Models.Product product)
        {
            this.Text = "Quick Stock Update";
            this.Size = new Size(400, 220);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            var lblProduct = new Label
            {
                Text = $"Product: {product.Name}",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(20, 20),
                Size = new Size(350, 25),
                BackColor = Color.Transparent
            };

            var lblCurrent = new Label
            {
                Text = $"Current Stock: {product.Quantity} units",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(120, 120, 120),
                Location = new Point(20, 50),
                Size = new Size(350, 25),
                BackColor = Color.Transparent
            };

            var lblNew = new Label
            {
                Text = "New Quantity:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(20, 85),
                Size = new Size(120, 25),
                BackColor = Color.Transparent
            };

            numQuantity = new NumericUpDown
            {
                Location = new Point(140, 85),
                Size = new Size(120, 30),
                Font = new Font("Segoe UI", 11F),
                Minimum = 0,
                Maximum = 100000,
                Value = product.Quantity
            };

            var btnOK = new Button
            {
                Text = "Update",
                Location = new Point(180, 135),
                Size = new Size(90, 35),
                BackColor = Color.FromArgb(34, 197, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                DialogResult = DialogResult.OK
            };
            btnOK.FlatAppearance.BorderSize = 0;
            btnOK.Click += (s, e) => { NewQuantity = (int)numQuantity.Value; };

            var btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(280, 135),
                Size = new Size(90, 35),
                BackColor = Color.FromArgb(100, 116, 139),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F),
                DialogResult = DialogResult.Cancel
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            this.Controls.AddRange(new Control[] { lblProduct, lblCurrent, lblNew, numQuantity, btnOK, btnCancel });
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }
    }
}
