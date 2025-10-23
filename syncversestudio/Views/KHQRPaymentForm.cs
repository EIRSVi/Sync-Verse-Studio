using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using FontAwesome.Sharp;
using QRCoder;

namespace SyncVerseStudio.Views
{
    public partial class KHQRPaymentForm : Form
    {
        private readonly decimal _amount;
        private System.Windows.Forms.Timer _paymentTimer;
        private int _countdown = 30; // 30 seconds timeout
        private Label _countdownLabel;
        private Button _confirmButton;

        public event EventHandler PaymentCompleted;

        public KHQRPaymentForm(decimal amount)
        {
            _amount = amount;
            InitializeComponent();
            GenerateQRCode();
            StartPaymentTimer();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Name = "KHQRPaymentForm";
            this.Text = "KHQR Mobile Payment";
            this.Size = new Size(500, 600);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.ShowInTaskbar = false;

            // Create blur overlay effect
            this.Opacity = 0.95;

            // Main panel
            var mainPanel = new Panel
            {
                Location = new Point(50, 50),
                Size = new Size(400, 500),
                BackColor = Color.White
            };

            mainPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, mainPanel.Width - 1, mainPanel.Height - 1), 15))
                using (var brush = new SolidBrush(Color.White))
                {
                    e.Graphics.FillPath(brush, path);
                }
            };

            // Header
            var headerPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(400, 80),
                BackColor = Color.FromArgb(59, 130, 246)
            };

            headerPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, headerPanel.Width - 1, 80), 15))
                using (var brush = new SolidBrush(Color.FromArgb(59, 130, 246)))
                {
                    e.Graphics.FillPath(brush, path);
                }
                // Draw rectangle to cover bottom rounded corners
                using (var brush = new SolidBrush(Color.FromArgb(59, 130, 246)))
                {
                    e.Graphics.FillRectangle(brush, 0, 40, headerPanel.Width, 40);
                }
            };

            var iconBox = new IconPictureBox
            {
                IconChar = IconChar.Mobile,
                IconColor = Color.White,
                IconSize = 32,
                Location = new Point(30, 24),
                Size = new Size(32, 32),
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(iconBox);

            var titleLabel = new Label
            {
                Text = "KHQR Mobile Payment",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(75, 25),
                Size = new Size(250, 30),
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(titleLabel);

            mainPanel.Controls.Add(headerPanel);

            // Amount display
            var amountLabel = new Label
            {
                Text = $"Amount: ${_amount:N2}",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(30, 100),
                Size = new Size(340, 35),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            mainPanel.Controls.Add(amountLabel);

            // QR Code container
            var qrPanel = new Panel
            {
                Location = new Point(75, 150),
                Size = new Size(250, 250),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            mainPanel.Controls.Add(qrPanel);

            // Instructions
            var instructionLabel = new Label
            {
                Text = "Scan this QR code with your mobile banking app\nto complete the payment",
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.FromArgb(60, 60, 60),
                Location = new Point(30, 420),
                Size = new Size(340, 40),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            mainPanel.Controls.Add(instructionLabel);

            // Countdown timer
            _countdownLabel = new Label
            {
                Text = $"Time remaining: {_countdown}s",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(249, 115, 22),
                Location = new Point(30, 470),
                Size = new Size(200, 20),
                BackColor = Color.Transparent
            };
            mainPanel.Controls.Add(_countdownLabel);

            // Buttons
            _confirmButton = new Button
            {
                Text = "✓ Payment Received",
                Location = new Point(30, 500),
                Size = new Size(150, 40),
                BackColor = Color.FromArgb(34, 197, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Enabled = false
            };
            _confirmButton.FlatAppearance.BorderSize = 0;
            _confirmButton.Click += ConfirmButton_Click;
            mainPanel.Controls.Add(_confirmButton);

            var cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(220, 500),
                Size = new Size(100, 40),
                BackColor = Color.FromArgb(239, 68, 68),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            cancelButton.FlatAppearance.BorderSize = 0;
            cancelButton.Click += (s, e) => this.Close();
            mainPanel.Controls.Add(cancelButton);

            this.Controls.Add(mainPanel);

            // Enable confirm button after 3 seconds (simulate payment processing time)
            var enableTimer = new System.Windows.Forms.Timer { Interval = 3000 };
            enableTimer.Tick += (s, e) =>
            {
                _confirmButton.Enabled = true;
                _confirmButton.BackColor = Color.FromArgb(34, 197, 94);
                enableTimer.Stop();
            };
            enableTimer.Start();

            this.ResumeLayout(false);
        }

        private void GenerateQRCode()
        {
            try
            {
                // Generate KHQR payment string (simplified)
                var paymentData = $"KHQR:AMOUNT:{_amount:N2}:MERCHANT:SyncVerse_POS:TIME:{DateTime.Now:yyyyMMddHHmmss}";

                var qrGenerator = new QRCodeGenerator();
                var qrCodeData = qrGenerator.CreateQrCode(paymentData, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new QRCode(qrCodeData);
                var qrCodeImage = qrCode.GetGraphic(10);

                var qrPictureBox = new PictureBox
                {
                    Image = qrCodeImage,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Dock = DockStyle.Fill,
                    BackColor = Color.White
                };

                var qrPanel = this.Controls[0].Controls.Cast<Control>().Where(c => c is Panel && ((Panel)c).BorderStyle == BorderStyle.FixedSingle).FirstOrDefault() as Panel;
                qrPanel?.Controls.Add(qrPictureBox);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating QR code: {ex.Message}", "QR Code Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StartPaymentTimer()
        {
            _paymentTimer = new System.Windows.Forms.Timer { Interval = 1000 }; // 1 second
            _paymentTimer.Tick += PaymentTimer_Tick;
            _paymentTimer.Start();
        }

        private void PaymentTimer_Tick(object sender, EventArgs e)
        {
            _countdown--;
            _countdownLabel.Text = $"Time remaining: {_countdown}s";

            if (_countdown <= 10)
            {
                _countdownLabel.ForeColor = Color.FromArgb(239, 68, 68);
            }

            if (_countdown <= 0)
            {
                _paymentTimer.Stop();
                MessageBox.Show("Payment timeout. Please try again.", "Payment Timeout", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
            }
        }

        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            _paymentTimer?.Stop();
            PaymentCompleted?.Invoke(this, EventArgs.Empty);
            this.Close();
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
                _paymentTimer?.Stop();
                _paymentTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}