using System;
using System.Drawing;
using System.Windows.Forms;
using SyncVerseStudio.Models;
using FontAwesome.Sharp;

namespace SyncVerseStudio.Views
{
    public class PaymentDialog : Form
    {
        private decimal _totalAmount;
        public PaymentMethod PaymentMethod { get; private set; }
        public decimal AmountPaid { get; private set; }

        private TextBox amountTextBox = null!;
        private Label changeLabel = null!;
        private Button cashButton = null!;
        private Button cardButton = null!;
        private Button mobileButton = null!;

        public PaymentDialog(decimal totalAmount)
        {
            _totalAmount = totalAmount;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Payment";
            this.Size = new Size(500, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            // Header
            var headerPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(500, 80),
                BackColor = Color.FromArgb(34, 197, 94)
            };

            var titleLabel = new Label
            {
                Text = "💳 Payment",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 20),
                AutoSize = true
            };

            headerPanel.Controls.Add(titleLabel);

            // Total amount
            var totalPanel = new Panel
            {
                Location = new Point(20, 100),
                Size = new Size(460, 60),
                BackColor = Color.FromArgb(248, 250, 252)
            };

            var totalLabel = new Label
            {
                Text = "Total Amount:",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(20, 10),
                AutoSize = true
            };

            var amountLabel = new Label
            {
                Text = $"${_totalAmount:N2}",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(34, 197, 94),
                Location = new Point(300, 10),
                AutoSize = true
            };

            totalPanel.Controls.AddRange(new Control[] { totalLabel, amountLabel });

            // Payment method selection
            var methodLabel = new Label
            {
                Text = "Select Payment Method:",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(20, 180),
                AutoSize = true
            };

            cashButton = CreatePaymentButton("💵 Cash", Color.FromArgb(34, 197, 94), 20, 220);
            cashButton.Click += (s, e) => SelectPaymentMethod(PaymentMethod.Cash);

            cardButton = CreatePaymentButton("💳 Card", Color.FromArgb(59, 130, 246), 180, 220);
            cardButton.Click += (s, e) => SelectPaymentMethod(PaymentMethod.Card);

            mobileButton = CreatePaymentButton("📱 Mobile", Color.FromArgb(168, 85, 247), 340, 220);
            mobileButton.Click += (s, e) => SelectPaymentMethod(PaymentMethod.Mobile);

            // Amount paid
            var paidLabel = new Label
            {
                Text = "Amount Paid:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(20, 310),
                AutoSize = true
            };

            amountTextBox = new TextBox
            {
                Location = new Point(20, 340),
                Size = new Size(460, 35),
                Font = new Font("Segoe UI", 16),
                Text = _totalAmount.ToString("N2")
            };
            amountTextBox.TextChanged += AmountTextBox_TextChanged;

            // Quick amount buttons
            var quickPanel = new FlowLayoutPanel
            {
                Location = new Point(20, 385),
                Size = new Size(460, 40),
                FlowDirection = FlowDirection.LeftToRight
            };

            var amounts = new[] { 10, 20, 50, 100 };
            foreach (var amount in amounts)
            {
                var btn = new Button
                {
                    Text = $"+${amount}",
                    Size = new Size(105, 35),
                    BackColor = Color.FromArgb(241, 245, 249),
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                btn.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
                btn.Click += (s, e) =>
                {
                    if (decimal.TryParse(amountTextBox.Text, out var current))
                    {
                        amountTextBox.Text = (current + amount).ToString("N2");
                    }
                };
                quickPanel.Controls.Add(btn);
            }

            // Change
            changeLabel = new Label
            {
                Text = "Change: $0.00",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(249, 115, 22),
                Location = new Point(20, 440),
                AutoSize = true
            };

            // Confirm button
            var confirmButton = new Button
            {
                Text = "✓ Confirm Payment",
                Location = new Point(20, 480),
                Size = new Size(460, 45),
                BackColor = Color.FromArgb(34, 197, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            confirmButton.FlatAppearance.BorderSize = 0;
            confirmButton.Click += ConfirmButton_Click;

            this.Controls.AddRange(new Control[] {
                headerPanel, totalPanel, methodLabel, cashButton, cardButton, mobileButton,
                paidLabel, amountTextBox, quickPanel, changeLabel, confirmButton
            });

            // Default to cash
            SelectPaymentMethod(PaymentMethod.Cash);
        }

        private Button CreatePaymentButton(string text, Color color, int x, int y)
        {
            var button = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(140, 70),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        private void SelectPaymentMethod(PaymentMethod method)
        {
            PaymentMethod = method;

            // Reset all buttons
            cashButton.BackColor = Color.FromArgb(34, 197, 94);
            cardButton.BackColor = Color.FromArgb(59, 130, 246);
            mobileButton.BackColor = Color.FromArgb(168, 85, 247);

            // Highlight selected
            switch (method)
            {
                case PaymentMethod.Cash:
                    cashButton.BackColor = Color.FromArgb(22, 163, 74);
                    break;
                case PaymentMethod.Card:
                    cardButton.BackColor = Color.FromArgb(37, 99, 235);
                    amountTextBox.Text = _totalAmount.ToString("N2");
                    break;
                case PaymentMethod.Mobile:
                    mobileButton.BackColor = Color.FromArgb(126, 34, 206);
                    amountTextBox.Text = _totalAmount.ToString("N2");
                    break;
            }

            AmountTextBox_TextChanged(null, EventArgs.Empty);
        }

        private void AmountTextBox_TextChanged(object? sender, EventArgs e)
        {
            if (decimal.TryParse(amountTextBox.Text, out var paid))
            {
                var change = paid - _totalAmount;
                changeLabel.Text = $"Change: ${change:N2}";
                changeLabel.ForeColor = change >= 0 ? Color.FromArgb(34, 197, 94) : Color.FromArgb(239, 68, 68);
            }
        }

        private void ConfirmButton_Click(object? sender, EventArgs e)
        {
            if (!decimal.TryParse(amountTextBox.Text, out var paid))
            {
                MessageBox.Show("Please enter a valid amount!", "Invalid Amount", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (paid < _totalAmount)
            {
                MessageBox.Show("Amount paid is less than total!", "Insufficient Payment", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            AmountPaid = paid;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
