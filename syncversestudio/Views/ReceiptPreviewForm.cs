using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using SyncVerseStudio.Data;
using SyncVerseStudio.Models;
using FontAwesome.Sharp;

namespace SyncVerseStudio.Views
{
    public partial class ReceiptPreviewForm : Form
    {
        private readonly Sale _sale;
        private readonly ApplicationDbContext _context;
        private RichTextBox _receiptTextBox;
        private PrintDocument _printDocument;

        public ReceiptPreviewForm(Sale sale)
        {
            _sale = sale;
            _context = new ApplicationDbContext();
            InitializeComponent();
            LoadReceiptData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Name = "ReceiptPreviewForm";
            this.Text = "Receipt Preview";
            this.Size = new Size(500, 700);
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
                IconChar = IconChar.Receipt,
                IconColor = Color.White,
                IconSize = 24,
                Location = new Point(20, 18),
                Size = new Size(24, 24),
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(iconBox);

            var titleLabel = new Label
            {
                Text = "Receipt Preview",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(55, 20),
                Size = new Size(200, 25),
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(titleLabel);

            this.Controls.Add(headerPanel);

            // Receipt content
            _receiptTextBox = new RichTextBox
            {
                Location = new Point(20, 80),
                Size = new Size(440, 520),
                Font = new Font("Courier New", 9F),
                ReadOnly = true,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(_receiptTextBox);

            // Buttons
            var buttonPanel = new Panel
            {
                Location = new Point(20, 620),
                Size = new Size(440, 50),
                BackColor = Color.Transparent
            };

            var printButton = new Button
            {
                Text = "🖨️ Print Receipt",
                Location = new Point(0, 0),
                Size = new Size(140, 40),
                BackColor = Color.FromArgb(34, 197, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            printButton.FlatAppearance.BorderSize = 0;
            printButton.Click += PrintButton_Click;
            buttonPanel.Controls.Add(printButton);

            var emailButton = new Button
            {
                Text = "📧 Email",
                Location = new Point(150, 0),
                Size = new Size(100, 40),
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            emailButton.FlatAppearance.BorderSize = 0;
            emailButton.Click += EmailButton_Click;
            buttonPanel.Controls.Add(emailButton);

            var closeButton = new Button
            {
                Text = "Close",
                Location = new Point(360, 0),
                Size = new Size(80, 40),
                BackColor = Color.FromArgb(156, 163, 175),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F)
            };
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.Click += (s, e) => this.Close();
            buttonPanel.Controls.Add(closeButton);

            this.Controls.Add(buttonPanel);

            // Initialize print document
            _printDocument = new PrintDocument();
            _printDocument.PrintPage += PrintDocument_PrintPage;

            this.ResumeLayout(false);
        }

        private async void LoadReceiptData()
        {
            try
            {
                // Load sale items
                var saleItems = await _context.SaleItems
                    .Include(si => si.Product)
                    .Where(si => si.SaleId == _sale.Id)
                    .ToListAsync();

                // Load cashier info
                var cashier = await _context.Users.FindAsync(_sale.CashierId);

                // Generate receipt content
                var receipt = GenerateReceiptContent(saleItems, cashier);
                _receiptTextBox.Text = receipt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading receipt data: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GenerateReceiptContent(System.Collections.Generic.List<SaleItem> saleItems, User cashier)
        {
            var receipt = new System.Text.StringBuilder();

            // Header with logo
            receipt.AppendLine("========================================");
            receipt.AppendLine("           SYNCVERSE STUDIO            ");
            receipt.AppendLine("        Professional POS System       ");
            receipt.AppendLine("========================================");
            receipt.AppendLine();
            
            // Store info
            receipt.AppendLine("123 Business Street");
            receipt.AppendLine("Phnom Penh, Cambodia");
            receipt.AppendLine("Tel: +855 12 345 678");
            receipt.AppendLine("Email: info@syncverse.com");
            receipt.AppendLine();
            receipt.AppendLine("========================================");
            receipt.AppendLine();

            // Transaction details
            receipt.AppendLine($"Invoice: {_sale.InvoiceNumber}");
            receipt.AppendLine($"Date: {_sale.SaleDate:yyyy-MM-dd HH:mm:ss}");
            receipt.AppendLine($"Cashier: {cashier?.FirstName} {cashier?.LastName}");
            
            if (_sale.Customer != null)
            {
                receipt.AppendLine($"Customer: {_sale.Customer.FirstName} {_sale.Customer.LastName}");
            }
            
            receipt.AppendLine($"Payment: {_sale.PaymentMethod}");
            receipt.AppendLine();
            receipt.AppendLine("========================================");
            receipt.AppendLine("ITEMS");
            receipt.AppendLine("========================================");

            // Items
            foreach (var item in saleItems)
            {
                var itemName = item.Product?.Name ?? "Unknown Product";
                var qty = item.Quantity;
                var unitPrice = item.UnitPrice;
                var total = item.TotalPrice;

                receipt.AppendLine($"{itemName}");
                receipt.AppendLine($"  {qty} x ${unitPrice:N2} = ${total:N2}");
                receipt.AppendLine();
            }

            receipt.AppendLine("========================================");
            receipt.AppendLine("TOTALS");
            receipt.AppendLine("========================================");
            receipt.AppendLine($"Subtotal:        ${_sale.SubTotal:N2}");
            receipt.AppendLine($"Tax:             ${_sale.TaxAmount:N2}");
            receipt.AppendLine("----------------------------------------");
            receipt.AppendLine($"TOTAL:           ${_sale.TotalAmount:N2}");
            receipt.AppendLine("========================================");
            receipt.AppendLine();

            // Payment details
            if (_sale.PaymentMethod == PaymentMethod.Cash)
            {
                // Calculate change (would need to store cash amount in sale)
                receipt.AppendLine("PAYMENT DETAILS");
                receipt.AppendLine("----------------------------------------");
                receipt.AppendLine($"Cash Received:   ${_sale.TotalAmount:N2}");
                receipt.AppendLine($"Change:          $0.00");
                receipt.AppendLine();
            }

            // Footer
            receipt.AppendLine("========================================");
            receipt.AppendLine("       Thank you for your business!    ");
            receipt.AppendLine("     Please keep this receipt for      ");
            receipt.AppendLine("           your records.               ");
            receipt.AppendLine("========================================");
            receipt.AppendLine();
            receipt.AppendLine($"Printed: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

            return receipt.ToString();
        }

        private void PrintButton_Click(object sender, EventArgs e)
        {
            try
            {
                var printDialog = new PrintDialog
                {
                    Document = _printDocument
                };

                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    _printDocument.Print();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error printing receipt: {ex.Message}", "Print Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            var font = new Font("Courier New", 8);
            var brush = new SolidBrush(Color.Black);
            var x = e.MarginBounds.Left;
            var y = e.MarginBounds.Top;
            var lineHeight = font.GetHeight(e.Graphics);

            var lines = _receiptTextBox.Text.Split('\n');
            
            foreach (var line in lines)
            {
                e.Graphics.DrawString(line, font, brush, x, y);
                y += (int)lineHeight;
                
                if (y >= e.MarginBounds.Bottom)
                {
                    e.HasMorePages = true;
                    return;
                }
            }

            e.HasMorePages = false;
        }

        private void EmailButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Email functionality would be implemented here.\n\nThis would allow sending the receipt to customer's email address.",
                "Email Receipt", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context?.Dispose();
                _printDocument?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}