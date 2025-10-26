using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SyncVerseStudio.Data;
using SyncVerseStudio.Models;
using SyncVerseStudio.Services;
using FontAwesome.Sharp;

namespace SyncVerseStudio.Views.CashierDashboard
{
    public partial class PaymentGatewayModal : Form
    {
        private readonly AuthenticationService _authService;
        private readonly ApplicationDbContext _context;
        private readonly System.Collections.Generic.List<POSCartItem> _cartItems;
        private readonly int? _customerId;
        private decimal _totalAmount;
        private decimal _paidAmount = 0;
        private TabControl paymentTabControl;
        private TextBox customValueTextBox;
        private TextBox paymentNoteTextBox;
        private Label payingLabel;
        private Label remainingLabel;
        private Button payButton;

        public PaymentGatewayModal(AuthenticationService authService, ApplicationDbContext context, 
            System.Collections.Generic.List<POSCartItem> cartItems, int? customerId)
        {
            _authService = authService;
            _context = context;
            _cartItems = cartItems;
            _customerId = customerId;
            _totalAmount = cartItems.Sum(i => i.Total);
            
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(600, 500);
            this.BackColor = Color.White;

            CreatePaymentInterface();

            this.ResumeLayout(false);
        }

        private void CreatePaymentInterface()
        {
            // Title
            var titleLabel = new Label
            {
                Text = "Client pay",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(30, 30),
                AutoSize = true
            };

            // Payment Method Tabs
            paymentTabControl = new TabControl
            {
                Location = new Point(30, 80),
                Size = new Size(540, 250),
                Font = new Font("Segoe UI", 11)
            };

            var cashTab = new TabPage("Cash");
            var cardTab = new TabPage("Card (POS)");
            var onlineTab = new TabPage("Online");

            paymentTabControl.TabPages.AddRange(new[] { cashTab, cardTab, onlineTab });

            // Custom Value Input
            var customValueLabel = new Label
            {
                Text = "Custom Value",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(30, 150),
                AutoSize = true
            };

            customValueTextBox = new TextBox
            {
                Font = new Font("Segoe UI", 12),
                Location = new Point(30, 175),
                Size = new Size(540, 30),
                PlaceholderText = "Enter amount..."
            };
            customValueTextBox.TextChanged += CustomValueTextBox_TextChanged;

            // Payment Note
            var noteLabel = new Label
            {
                Text = "Payment Note",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(30, 220),
                AutoSize = true
            };

            paymentNoteTextBox = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(30, 245),
                Size = new Size(540, 30),
                PlaceholderText = "Optional Note"
            };

            // Payment Summary
            var summaryPanel = new Panel
            {
                Location = new Point(30, 290),
                Size = new Size(540, 80),
                BackColor = Color.FromArgb(248, 250, 252)
            };

            payingLabel = new Label
            {
                Text = $"Paying: {_totalAmount:N0} KHR",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(20, 15),
                AutoSize = true
            };

            remainingLabel = new Label
            {
                Text = $"Remaining: {_totalAmount:N0} KHR",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(34, 197, 94),
                Location = new Point(20, 45),
                AutoSize = true
            };

            summaryPanel.Controls.AddRange(new Control[] { payingLabel, remainingLabel });

            // Pay Button
            payButton = new Button
            {
                Text = $"Pay {_totalAmount:N0} KHR",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(20, 184, 166),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(540, 55),
                Location = new Point(30, 390),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            payButton.FlatAppearance.BorderSize = 0;
            payButton.Click += PayButton_Click;

            this.Controls.AddRange(new Control[] { 
                titleLabel, paymentTabControl, customValueLabel, customValueTextBox,
                noteLabel, paymentNoteTextBox, summaryPanel, payButton 
            });
        }

        private void CustomValueTextBox_TextChanged(object sender, EventArgs e)
        {
            if (decimal.TryParse(customValueTextBox.Text, out decimal amount))
            {
                _paidAmount = amount;
                var remaining = _totalAmount - _paidAmount;
                
                payingLabel.Text = $"Paying: {_paidAmount:N0} KHR";
                remainingLabel.Text = $"Remaining: {remaining:N0} KHR";
                
                if (remaining <= 0)
                {
                    remainingLabel.ForeColor = Color.FromArgb(34, 197, 94);
                    payButton.Enabled = true;
                }
                else
                {
                    remainingLabel.ForeColor = Color.FromArgb(239, 68, 68);
                    payButton.Enabled = false;
                }
            }
            else
            {
                payButton.Enabled = false;
            }
        }

        private void PayButton_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedTab = paymentTabControl.SelectedTab.Text;
                PaymentMethodType paymentMethod = selectedTab switch
                {
                    "Cash" => PaymentMethodType.Cash,
                    "Card (POS)" => PaymentMethodType.Card,
                    "Online" => PaymentMethodType.Online,
                    _ => PaymentMethodType.Cash
                };

                // Create Invoice
                var invoice = new Invoice
                {
                    InvoiceNumber = GenerateInvoiceNumber(),
                    CustomerId = _customerId,
                    CustomerName = _customerId.HasValue ? null : "Walk-in Client",
                    CreatedByUserId = _authService.CurrentUser.Id,
                    SubTotal = _totalAmount,
                    TaxAmount = 0,
                    DiscountAmount = 0,
                    TotalAmount = _totalAmount,
                    PaidAmount = _paidAmount,
                    BalanceAmount = _totalAmount - _paidAmount,
                    Status = _paidAmount >= _totalAmount ? InvoiceStatus.Paid : InvoiceStatus.Active,
                    InvoiceDate = DateTime.Now,
                    Notes = paymentNoteTextBox.Text
                };

                // Add Invoice Items
                foreach (var item in _cartItems)
                {
                    invoice.InvoiceItems.Add(new InvoiceItem
                    {
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        TotalPrice = item.Total
                    });
                }

                _context.Invoices.Add(invoice);

                // Create Payment
                var payment = new Payment
                {
                    Invoice = invoice,
                    PaymentReference = GeneratePaymentReference(),
                    Amount = _paidAmount,
                    PaymentMethod = paymentMethod,
                    Status = PaymentStatus.Completed,
                    Notes = paymentNoteTextBox.Text,
                    ProcessedByUserId = _authService.CurrentUser.Id,
                    PaymentDate = DateTime.Now
                };

                _context.Payments.Add(payment);

                // Update Product Stock
                foreach (var item in _cartItems)
                {
                    var product = _context.Products.Find(item.ProductId);
                    if (product != null)
                    {
                        product.Quantity -= item.Quantity;
                        product.UpdatedAt = DateTime.Now;
                    }
                }

                _context.SaveChanges();

                // Show success modal
                var successModal = new TransactionSuccessModal(invoice.InvoiceNumber, _totalAmount);
                successModal.ShowDialog();

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Payment failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GenerateInvoiceNumber()
        {
            var year = DateTime.Now.Year;
            var count = _context.Invoices.Count(i => i.InvoiceDate.Year == year) + 1;
            return $"#{year}{count:D3}";
        }

        private string GeneratePaymentReference()
        {
            return $"PAY-{DateTime.Now:yyyyMMddHHmmss}";
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
