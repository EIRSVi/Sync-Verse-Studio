using System;
using System.Drawing;
using System.Windows.Forms;
using FontAwesome.Sharp;

namespace SyncVerseStudio.Views.CashierDashboard
{
    public partial class TransactionSuccessModal : Form
    {
        private readonly string _invoiceNumber;
        private readonly decimal _amount;

        public TransactionSuccessModal(string invoiceNumber, decimal amount)
        {
            _invoiceNumber = invoiceNumber;
            _amount = amount;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(500, 450);
            this.BackColor = Color.White;

            CreateSuccessInterface();

            this.ResumeLayout(false);
        }

        private void CreateSuccessInterface()
        {
            // Teal Header with Checkmark
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 150,
                BackColor = Color.FromArgb(20, 184, 166)
            };

            var checkIcon = new IconPictureBox
            {
                IconChar = IconChar.CheckCircle,
                IconColor = Color.White,
                IconSize = 80,
                Location = new Point(210, 35),
                Size = new Size(80, 80)
            };

            var closeButton = new Button
            {
                Text = "×",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(40, 40),
                Location = new Point(450, 10),
                Cursor = Cursors.Hand
            };
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.Click += (s, e) => this.Close();

            headerPanel.Controls.AddRange(new Control[] { checkIcon, closeButton });

            // Success Message
            var successLabel = new Label
            {
                Text = "Payment Successful!",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(0, 170),
                Size = new Size(500, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var invoiceLabel = new Label
            {
                Text = $"Invoice: {_invoiceNumber}",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(0, 210),
                Size = new Size(500, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var amountLabel = new Label
            {
                Text = $"Amount: {_amount:N0} KHR",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(0, 240),
                Size = new Size(500, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Receipt Options Grid (2x2)
            var optionsPanel = new Panel
            {
                Location = new Point(100, 280),
                Size = new Size(300, 120)
            };

            var printButton = CreateOptionButton("Print", IconChar.Print, 0, 0);
            printButton.Click += PrintButton_Click;

            var viewButton = CreateOptionButton("View", IconChar.Eye, 160, 0);
            viewButton.Click += ViewButton_Click;

            var emailButton = CreateOptionButton("Email", IconChar.Envelope, 0, 60);
            emailButton.Click += EmailButton_Click;

            var smsButton = CreateOptionButton("SMS", IconChar.CommentDots, 160, 60);
            smsButton.Click += SmsButton_Click;

            optionsPanel.Controls.AddRange(new Control[] { printButton, viewButton, emailButton, smsButton });

            // New Transaction Button
            var newTransactionButton = new Button
            {
                Text = "New Transaction",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(20, 184, 166),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(440, 50),
                Location = new Point(30, 420),
                Cursor = Cursors.Hand
            };
            newTransactionButton.FlatAppearance.BorderSize = 0;
            newTransactionButton.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { 
                headerPanel, successLabel, invoiceLabel, amountLabel, 
                optionsPanel, newTransactionButton 
            });
        }

        private Button CreateOptionButton(string text, IconChar icon, int x, int y)
        {
            var button = new Panel
            {
                Size = new Size(140, 50),
                Location = new Point(x, y),
                BackColor = Color.FromArgb(248, 250, 252),
                Cursor = Cursors.Hand
            };

            button.Paint += (s, e) =>
            {
                var rect = button.ClientRectangle;
                using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, rect.Width - 1, rect.Height - 1);
                }
            };

            var iconBox = new IconPictureBox
            {
                IconChar = icon,
                IconColor = Color.FromArgb(100, 116, 139),
                IconSize = 24,
                Location = new Point(58, 13),
                Size = new Size(24, 24)
            };

            var clickButton = new Button
            {
                Text = "",
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Tag = text
            };
            clickButton.FlatAppearance.BorderSize = 0;
            clickButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(226, 232, 240);

            button.Controls.AddRange(new Control[] { iconBox, clickButton });

            return clickButton;
        }

        private void PrintButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"Printing receipt for invoice {_invoiceNumber}...", "Print", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ViewButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"Opening receipt for invoice {_invoiceNumber}...", "View", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void EmailButton_Click(object sender, EventArgs e)
        {
            var email = Microsoft.VisualBasic.Interaction.InputBox("Enter email address:", "Email Receipt", "");
            if (!string.IsNullOrEmpty(email))
            {
                MessageBox.Show($"Receipt sent to {email}", "Email Sent", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void SmsButton_Click(object sender, EventArgs e)
        {
            var phone = Microsoft.VisualBasic.Interaction.InputBox("Enter phone number:", "SMS Receipt", "");
            if (!string.IsNullOrEmpty(phone))
            {
                MessageBox.Show($"Receipt sent to {phone}", "SMS Sent", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
