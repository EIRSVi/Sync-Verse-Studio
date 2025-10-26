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
    public partial class StockTransferView : Form
    {
        private readonly AuthenticationService _authService;
        private readonly ApplicationDbContext _context;
        private ComboBox cmbProduct;
        private ComboBox cmbFromLocation;
        private ComboBox cmbToLocation;
        private NumericUpDown numQuantity;
        private TextBox txtReferenceNumber;
        private TextBox txtNotes;
        private Label lblCurrentStock;
        private Label lblTransferInfo;

        public StockTransferView(AuthenticationService authService)
        {
            _authService = authService;
            _context = new ApplicationDbContext();
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Name = "StockTransferView";
            this.Text = "Stock Transfer";
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
                IconChar = IconChar.ArrowsLeftRight,
                IconColor = Color.FromArgb(168, 85, 247),
                IconSize = 32,
                Location = new Point(30, 24),
                Size = new Size(32, 32),
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(iconBox);

            var titleLabel = new Label
            {
                Text = "Stock Transfer",
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
                Text = "Transfer stock between locations",
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

            var formCard = new Panel
            {
                Location = new Point(30, 20),
                Size = new Size(700, 650),
                BackColor = Color.White
            };

            formCard.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, formCard.Width - 1, formCard.Height - 1), 12))
                using (var brush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillPath(brush, path);
                }
            };

            int yPos = 30;

            // Product Selection
            var lblProduct = new Label
            {
                Text = "Select Product:",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(30, yPos),
                Size = new Size(200, 25),
                BackColor = Color.Transparent
            };
            formCard.Controls.Add(lblProduct);

            cmbProduct = new ComboBox
            {
                Location = new Point(30, yPos + 30),
                Size = new Size(640, 30),
                Font = new Font("Segoe UI", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbProduct.SelectedIndexChanged += CmbProduct_SelectedIndexChanged;
            formCard.Controls.Add(cmbProduct);

            yPos += 80;

            // Current Stock Info
            lblCurrentStock = new Label
            {
                Text = "Current Stock: -",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(120, 120, 120),
                Location = new Point(30, yPos),
                Size = new Size(300, 25),
                BackColor = Color.Transparent
            };
            formCard.Controls.Add(lblCurrentStock);

            yPos += 40;

            // From Location
            var lblFromLocation = new Label
            {
                Text = "From Location:",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(30, yPos),
                Size = new Size(200, 25),
                BackColor = Color.Transparent
            };
            formCard.Controls.Add(lblFromLocation);

            cmbFromLocation = new ComboBox
            {
                Location = new Point(30, yPos + 30),
                Size = new Size(300, 30),
                Font = new Font("Segoe UI", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbFromLocation.Items.AddRange(new string[] { 
                "Main Warehouse", 
                "Store Front", 
                "Storage Room A", 
                "Storage Room B",
                "Distribution Center"
            });
            cmbFromLocation.SelectedIndex = 0;
            formCard.Controls.Add(cmbFromLocation);

            yPos += 80;

            // To Location
            var lblToLocation = new Label
            {
                Text = "To Location:",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(30, yPos),
                Size = new Size(200, 25),
                BackColor = Color.Transparent
            };
            formCard.Controls.Add(lblToLocation);

            cmbToLocation = new ComboBox
            {
                Location = new Point(30, yPos + 30),
                Size = new Size(300, 30),
                Font = new Font("Segoe UI", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbToLocation.Items.AddRange(new string[] { 
                "Main Warehouse", 
                "Store Front", 
                "Storage Room A", 
                "Storage Room B",
                "Distribution Center"
            });
            cmbToLocation.SelectedIndex = 1;
            formCard.Controls.Add(cmbToLocation);

            yPos += 80;

            // Quantity
            var lblQuantity = new Label
            {
                Text = "Transfer Quantity:",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(30, yPos),
                Size = new Size(200, 25),
                BackColor = Color.Transparent
            };
            formCard.Controls.Add(lblQuantity);

            numQuantity = new NumericUpDown
            {
                Location = new Point(30, yPos + 30),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 10F),
                Minimum = 1,
                Maximum = 100000,
                Value = 1
            };
            numQuantity.ValueChanged += (s, e) => UpdateTransferInfo();
            formCard.Controls.Add(numQuantity);

            lblTransferInfo = new Label
            {
                Text = "Ready to transfer",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(120, 120, 120),
                Location = new Point(250, yPos + 35),
                Size = new Size(400, 25),
                BackColor = Color.Transparent
            };
            formCard.Controls.Add(lblTransferInfo);

            yPos += 80;

            // Reference Number
            var lblReference = new Label
            {
                Text = "Reference Number (Optional):",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(30, yPos),
                Size = new Size(300, 25),
                BackColor = Color.Transparent
            };
            formCard.Controls.Add(lblReference);

            txtReferenceNumber = new TextBox
            {
                Location = new Point(30, yPos + 30),
                Size = new Size(640, 30),
                Font = new Font("Segoe UI", 10F),
                PlaceholderText = "Transfer ID, Request Number, etc."
            };
            formCard.Controls.Add(txtReferenceNumber);

            yPos += 80;

            // Notes
            var lblNotes = new Label
            {
                Text = "Notes (Optional):",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(30, yPos),
                Size = new Size(200, 25),
                BackColor = Color.Transparent
            };
            formCard.Controls.Add(lblNotes);

            txtNotes = new TextBox
            {
                Location = new Point(30, yPos + 30),
                Size = new Size(640, 60),
                Font = new Font("Segoe UI", 10F),
                Multiline = true,
                PlaceholderText = "Additional notes about this transfer..."
            };
            formCard.Controls.Add(txtNotes);

            yPos += 110;

            // Buttons
            var btnSave = new Button
            {
                Text = "✓ Process Transfer",
                Location = new Point(30, yPos),
                Size = new Size(160, 40),
                BackColor = Color.FromArgb(168, 85, 247),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;
            formCard.Controls.Add(btnSave);

            var btnClear = new Button
            {
                Text = "Clear Form",
                Location = new Point(210, yPos),
                Size = new Size(120, 40),
                BackColor = Color.FromArgb(100, 116, 139),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F),
                Cursor = Cursors.Hand
            };
            btnClear.FlatAppearance.BorderSize = 0;
            btnClear.Click += (s, e) => ClearForm();
            formCard.Controls.Add(btnClear);

            contentPanel.Controls.Add(formCard);
            this.Controls.Add(contentPanel);
        }

        private async void LoadData()
        {
            try
            {
                var products = await _context.Products
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.Name)
                    .ToListAsync();

                cmbProduct.DisplayMember = "Name";
                cmbProduct.ValueMember = "Id";
                cmbProduct.DataSource = products;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CmbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbProduct.SelectedItem is Models.Product product)
            {
                lblCurrentStock.Text = $"Current Stock: {product.Quantity} units";
                UpdateTransferInfo();
            }
        }

        private void UpdateTransferInfo()
        {
            if (cmbProduct.SelectedItem is Models.Product product)
            {
                int transferQty = (int)numQuantity.Value;
                if (transferQty > product.Quantity)
                {
                    lblTransferInfo.Text = "⚠ Insufficient stock for transfer";
                    lblTransferInfo.ForeColor = Color.FromArgb(239, 68, 68);
                }
                else
                {
                    lblTransferInfo.Text = $"Transferring {transferQty} units";
                    lblTransferInfo.ForeColor = Color.FromArgb(34, 197, 94);
                }
            }
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            if (cmbProduct.SelectedItem == null)
            {
                MessageBox.Show("Please select a product.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbFromLocation.Text == cmbToLocation.Text)
            {
                MessageBox.Show("From and To locations must be different.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var product = cmbProduct.SelectedItem as Models.Product;
                int transferQty = (int)numQuantity.Value;

                if (transferQty > product.Quantity)
                {
                    MessageBox.Show("Insufficient stock for this transfer.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // In a real system, you'd track location-specific inventory
                // For now, we'll just log the transfer
                product.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();

                MessageBox.Show(
                    $"Stock transfer processed successfully!\n\n" +
                    $"Product: {product.Name}\n" +
                    $"Quantity: {transferQty} units\n" +
                    $"From: {cmbFromLocation.Text}\n" +
                    $"To: {cmbToLocation.Text}\n" +
                    $"Reference: {txtReferenceNumber.Text}\n\n" +
                    "Note: In a full system, location-specific inventory would be updated.",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                ClearForm();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing transfer: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearForm()
        {
            if (cmbProduct.Items.Count > 0)
                cmbProduct.SelectedIndex = 0;
            cmbFromLocation.SelectedIndex = 0;
            cmbToLocation.SelectedIndex = 1;
            numQuantity.Value = 1;
            txtReferenceNumber.Clear();
            txtNotes.Clear();
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
}
