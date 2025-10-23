using System;
using System.Drawing;
using System.Windows.Forms;
using FontAwesome.Sharp;

namespace SyncVerseStudio.Views
{
    public partial class QRScannerForm : Form
    {
        public string ScannedCode { get; private set; } = string.Empty;
        private TextBox _codeInput;

        public QRScannerForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Name = "QRScannerForm";
            this.Text = "QR Code Scanner";
            this.Size = new Size(400, 300);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            // Header
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(59, 130, 246)
            };

            var iconBox = new IconPictureBox
            {
                IconChar = IconChar.Qrcode,
                IconColor = Color.White,
                IconSize = 24,
                Location = new Point(20, 18),
                Size = new Size(24, 24),
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(iconBox);

            var titleLabel = new Label
            {
                Text = "Scan QR Code",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(55, 20),
                Size = new Size(200, 25),
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(titleLabel);

            this.Controls.Add(headerPanel);

            // Content
            var contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            var instructionLabel = new Label
            {
                Text = "Position the QR code in front of the camera or enter the code manually:",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(60, 60, 60),
                Location = new Point(0, 20),
                Size = new Size(340, 40),
                BackColor = Color.Transparent
            };
            contentPanel.Controls.Add(instructionLabel);

            // Manual input
            var manualLabel = new Label
            {
                Text = "Manual Entry:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(0, 80),
                Size = new Size(100, 25),
                BackColor = Color.Transparent
            };
            contentPanel.Controls.Add(manualLabel);

            _codeInput = new TextBox
            {
                Location = new Point(0, 110),
                Size = new Size(340, 30),
                Font = new Font("Segoe UI", 11F),
                PlaceholderText = "Enter barcode or QR code..."
            };
            contentPanel.Controls.Add(_codeInput);

            // Buttons
            var buttonPanel = new Panel
            {
                Location = new Point(0, 160),
                Size = new Size(340, 40),
                BackColor = Color.Transparent
            };

            var scanButton = new Button
            {
                Text = "📷 Use Camera",
                Location = new Point(0, 0),
                Size = new Size(110, 35),
                BackColor = Color.FromArgb(34, 197, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            scanButton.FlatAppearance.BorderSize = 0;
            scanButton.Click += ScanButton_Click;
            buttonPanel.Controls.Add(scanButton);

            var okButton = new Button
            {
                Text = "OK",
                Location = new Point(170, 0),
                Size = new Size(80, 35),
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            okButton.FlatAppearance.BorderSize = 0;
            okButton.Click += OkButton_Click;
            buttonPanel.Controls.Add(okButton);

            var cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(260, 0),
                Size = new Size(80, 35),
                BackColor = Color.FromArgb(156, 163, 175),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F)
            };
            cancelButton.FlatAppearance.BorderSize = 0;
            cancelButton.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            buttonPanel.Controls.Add(cancelButton);

            contentPanel.Controls.Add(buttonPanel);
            this.Controls.Add(contentPanel);

            this.ResumeLayout(false);
        }

        private void ScanButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Camera scanning functionality would be implemented here.\n\nFor now, please use manual entry.",
                "Camera Scanner", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_codeInput.Text))
            {
                MessageBox.Show("Please enter a code or scan using camera.", "Input Required", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ScannedCode = _codeInput.Text.Trim();
            this.DialogResult = DialogResult.OK;
        }
    }
}