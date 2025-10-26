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
    public partial class ReceiveStockView : Form
    {
        private readonly AuthenticationService _authService;
        private readonly ApplicationDbContext _context;
        private ComboBox cmbProduct;
        private NumericUpDown numQuantity;
        private ComboBox cmbSupplier;
        private TextBox txtReferenceNumber;
        private TextBox txtNotes;
        private Label lblCurrentStock;
        private Label lblNewStock;

        public ReceiveStockView(AuthenticationService authService)
        {
            _authService = authService;
            _context = new ApplicationDbContext();
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Name = "ReceiveStockView";
            this.Text = "Receive Stock";
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
                IconChar = IconChar.ArrowUp,
                IconColor = Color.FromArgb(34, 197, 94),
                IconSize = 32,
                Location = new Point(30, 24),
                Size = new Size(32, 32),
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(iconBox);

            var titleLabel = new Label
            {
                Text = "Receive Stock",
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
                Text = "Process incoming stock deliveries",
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
                Size = new Size(700, 600),
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

            // Quantity
            var lblQuantity = new Label
            {
                Text = "Quantity Received:",
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
            numQuantity.ValueChanged += NumQuantity_ValueChanged;
            formCard.Controls.Add(numQuantity);

            lblNewStock = new Label
            {
                Text = "New Stock: -",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(34, 197, 94),
                Location = new Point(250, yPos + 35),
                Size = new Size(300, 25),
                BackColor = Color.Transparent
            };
            formCard.Controls.Add(lblNewStock);

            yPos += 80;

            // Supplier
            var lblSupplier = new Label
            {
                Text = "Supplier:",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(30, yPos),
                Size = new Size(200, 25),
                BackColor = Color.Transparent
            };
            formCard.Controls.Add(lblSupplier);

            cmbSupplier = new ComboBox
            {
                Location = new Point(30, yPos + 30),
                Size = new Size(640, 30),
                Font = new Font("Segoe UI", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            formCard.Controls.Add(cmbSupplier);

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
                PlaceholderText = "PO Number, Invoice Number, etc."
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
                Size = new Size(640, 80),
                Font = new Font("Segoe UI", 10F),
                Multiline = true,
                PlaceholderText = "Additional notes about this delivery..."
            };
            formCard.Controls.Add(txtNotes);

            yPos += 130;

            // Buttons
            var btnSave = new Button
            {
                Text = "✓ Receive Stock",
                Location = new Point(30, yPos),
                Size = new Size(150, 40),
                BackColor = Color.FromArgb(34, 197, 94),
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
                Location = new Point(200, yPos),
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

                var suppliers = await _context.Suppliers
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.Name)
                    .ToListAsync();

                cmbSupplier.DisplayMember = "Name";
                cmbSupplier.ValueMember = "Id";
                cmbSupplier.DataSource = suppliers;
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
                UpdateNewStock();
            }
        }

        private void NumQuantity_ValueChanged(object sender, EventArgs e)
        {
            UpdateNewStock();
        }

        private void UpdateNewStock()
        {
            if (cmbProduct.SelectedItem is Models.Product product)
            {
                int newStock = product.Quantity + (int)numQuantity.Value;
                lblNewStock.Text = $"New Stock: {newStock} units";
            }
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            if (cmbProduct.SelectedItem == null)
            {
                MessageBox.Show("Please select a product.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var product = cmbProduct.SelectedItem as Models.Product;
                int quantityReceived = (int)numQuantity.Value;
                int oldQuantity = product.Quantity;
                int newQuantity = oldQuantity + quantityReceived;

                product.Quantity = newQuantity;
                product.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                MessageBox.Show(
                    $"Stock received successfully!\n\n" +
                    $"Product: {product.Name}\n" +
                    $"Quantity Received: {quantityReceived}\n" +
                    $"Previous Stock: {oldQuantity}\n" +
                    $"New Stock: {newQuantity}\n" +
                    $"Reference: {txtReferenceNumber.Text}",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                ClearForm();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error receiving stock: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearForm()
        {
            if (cmbProduct.Items.Count > 0)
                cmbProduct.SelectedIndex = 0;
            numQuantity.Value = 1;
            if (cmbSupplier.Items.Count > 0)
                cmbSupplier.SelectedIndex = 0;
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
