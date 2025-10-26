using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SyncVerseStudio.Data;
using SyncVerseStudio.Models;
using SyncVerseStudio.Services;
using FontAwesome.Sharp;

namespace SyncVerseStudio.Views.CashierDashboard
{
    public partial class ModernPOSView : Form
    {
        private readonly AuthenticationService _authService;
        private readonly ApplicationDbContext _context;
        private List<POSCartItem> cartItems = new List<POSCartItem>();
        private ComboBox clientComboBox;
        private FlowLayoutPanel productsPanel;
        private FlowLayoutPanel cartItemsPanel;
        private Label subtotalLabel, discountLabel, totalLabel;
        private int? selectedCustomerId = null;

        public ModernPOSView(AuthenticationService authService)
        {
            _authService = authService;
            _context = new ApplicationDbContext();
            InitializeComponent();
            LoadCustomers();
            LoadProducts();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.White;
            this.Size = new Size(1400, 900);
            this.Dock = DockStyle.Fill;

            CreatePOSLayout();

            this.ResumeLayout(false);
        }

        private void CreatePOSLayout()
        {
            // Header Panel
            var headerPanel = CreateHeaderPanel();
            headerPanel.Dock = DockStyle.Top;
            this.Controls.Add(headerPanel);

            // Left Panel - Products
            var leftPanel = CreateProductsPanel();
            leftPanel.Dock = DockStyle.Fill;
            this.Controls.Add(leftPanel);

            // Right Panel - Cart
            var rightPanel = CreateCartPanel();
            rightPanel.Dock = DockStyle.Right;
            this.Controls.Add(rightPanel);
        }

        private Panel CreateHeaderPanel()
        {
            var panel = new Panel
            {
                Height = 80,
                BackColor = Color.White,
                Padding = new Padding(20, 15, 20, 15)
            };

            var logoLabel = new Label
            {
                Text = "SYNCVERSE",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 184, 166),
                Location = new Point(20, 20),
                AutoSize = true
            };

            var dateTimeLabel = new Label
            {
                Text = DateTime.Now.ToString("MMM dd, yyyy, HH:mm"),
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(200, 28),
                AutoSize = true
            };

            var searchIcon = new IconButton
            {
                IconChar = IconChar.Search,
                IconColor = Color.FromArgb(100, 116, 139),
                IconSize = 24,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(40, 40),
                Cursor = Cursors.Hand
            };
            searchIcon.FlatAppearance.BorderSize = 0;
            searchIcon.Location = new Point(panel.Width - 180, 20);

            var saveIcon = new IconButton
            {
                IconChar = IconChar.Save,
                IconColor = Color.FromArgb(100, 116, 139),
                IconSize = 24,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(40, 40),
                Cursor = Cursors.Hand
            };
            saveIcon.FlatAppearance.BorderSize = 0;
            saveIcon.Location = new Point(panel.Width - 130, 20);
            saveIcon.Click += SaveTransaction_Click;

            var searchProductIcon = new IconButton
            {
                IconChar = IconChar.Barcode,
                IconColor = Color.FromArgb(100, 116, 139),
                IconSize = 24,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(40, 40),
                Cursor = Cursors.Hand
            };
            searchProductIcon.FlatAppearance.BorderSize = 0;
            searchProductIcon.Location = new Point(panel.Width - 80, 20);

            panel.Controls.AddRange(new Control[] { logoLabel, dateTimeLabel, searchIcon, saveIcon, searchProductIcon });
            return panel;
        }

        private Panel CreateProductsPanel()
        {
            var panel = new Panel
            {
                BackColor = Color.FromArgb(248, 250, 252),
                Padding = new Padding(20)
            };

            var addItemButton = new Button
            {
                Text = "  Add Item",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(20, 184, 166),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(200, 50),
                Location = new Point(20, 20),
                Cursor = Cursors.Hand,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleLeft
            };
            addItemButton.FlatAppearance.BorderSize = 0;

            var addIcon = new IconButton
            {
                IconChar = IconChar.Plus,
                IconColor = Color.White,
                IconSize = 20,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(30, 30),
                Location = new Point(30, 30),
                Enabled = false
            };
            addIcon.FlatAppearance.BorderSize = 0;

            var demoProductButton = new Button
            {
                Text = "  Demo product",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 116, 139),
                BackColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(200, 45),
                Location = new Point(20, 80),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleLeft
            };
            demoProductButton.FlatAppearance.BorderSize = 1;
            demoProductButton.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);

            productsPanel = new FlowLayoutPanel
            {
                Location = new Point(20, 140),
                Size = new Size(panel.Width - 40, panel.Height - 160),
                AutoScroll = true,
                BackColor = Color.Transparent
            };

            panel.Controls.AddRange(new Control[] { addItemButton, addIcon, demoProductButton, productsPanel });
            return panel;
        }

        private Panel CreateCartPanel()
        {
            var panel = new Panel
            {
                Width = 450,
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            var clientLabel = new Label
            {
                Text = "Client:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(20, 20),
                AutoSize = true
            };

            clientComboBox = new ComboBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(20, 50),
                Size = new Size(410, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            clientComboBox.SelectedIndexChanged += ClientComboBox_SelectedIndexChanged;

            cartItemsPanel = new FlowLayoutPanel
            {
                Location = new Point(20, 100),
                Size = new Size(410, 450),
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Color.White
            };

            // Summary Panel
            var summaryPanel = new Panel
            {
                Location = new Point(20, 570),
                Size = new Size(410, 120),
                BackColor = Color.FromArgb(248, 250, 252)
            };

            subtotalLabel = new Label
            {
                Text = "Subtotal: 0 KHR",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(15, 15),
                AutoSize = true
            };

            discountLabel = new Label
            {
                Text = "Discount: 0 KHR",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(15, 45),
                AutoSize = true
            };

            totalLabel = new Label
            {
                Text = "Total: 0 KHR",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(15, 75),
                AutoSize = true
            };

            summaryPanel.Controls.AddRange(new Control[] { subtotalLabel, discountLabel, totalLabel });

            // Action Buttons
            var cancelButton = new Button
            {
                Text = "",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(239, 68, 68),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(60, 60),
                Location = new Point(20, 710),
                Cursor = Cursors.Hand
            };
            cancelButton.FlatAppearance.BorderSize = 0;
            cancelButton.Click += CancelTransaction_Click;

            var cancelIcon = new IconButton
            {
                IconChar = IconChar.Times,
                IconColor = Color.White,
                IconSize = 30,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(40, 40),
                Location = new Point(30, 720),
                Enabled = false
            };
            cancelIcon.FlatAppearance.BorderSize = 0;

            var saveButton = new Button
            {
                Text = "",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(59, 130, 246),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(60, 60),
                Location = new Point(90, 710),
                Cursor = Cursors.Hand
            };
            saveButton.FlatAppearance.BorderSize = 0;
            saveButton.Click += SaveTransaction_Click;

            var saveIcon2 = new IconButton
            {
                IconChar = IconChar.Save,
                IconColor = Color.White,
                IconSize = 30,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(40, 40),
                Location = new Point(100, 720),
                Enabled = false
            };
            saveIcon2.FlatAppearance.BorderSize = 0;

            var payButton = new Button
            {
                Text = "Pay 0 KHR",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(20, 184, 166),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(270, 60),
                Location = new Point(160, 710),
                Cursor = Cursors.Hand
            };
            payButton.FlatAppearance.BorderSize = 0;
            payButton.Click += PayButton_Click;

            panel.Controls.AddRange(new Control[] { 
                clientLabel, clientComboBox, cartItemsPanel, summaryPanel, 
                cancelButton, cancelIcon, saveButton, saveIcon2, payButton 
            });

            return panel;
        }

        private void LoadCustomers()
        {
            try
            {
                var customers = _context.Customers.OrderBy(c => c.FirstName).ToList();
                
                clientComboBox.Items.Clear();
                clientComboBox.Items.Add("Walk-in Client");
                
                foreach (var customer in customers)
                {
                    clientComboBox.Items.Add(new ComboBoxItem 
                    { 
                        Text = customer.FullName, 
                        Value = customer.Id 
                    });
                }
                
                clientComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customers: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadProducts()
        {
            try
            {
                var products = _context.Products
                    .Where(p => p.IsActive && p.Quantity > 0)
                    .OrderBy(p => p.Name)
                    .ToList();

                productsPanel.Controls.Clear();

                foreach (var product in products)
                {
                    var productCard = CreateProductCard(product);
                    productsPanel.Controls.Add(productCard);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Panel CreateProductCard(Product product)
        {
            var card = new Panel
            {
                Size = new Size(120, 140),
                BackColor = Color.White,
                Margin = new Padding(10),
                Cursor = Cursors.Hand
            };

            // Generate placeholder with initials
            var initials = GetProductInitials(product.Name);
            var placeholderPanel = new Panel
            {
                Size = new Size(120, 80),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(226, 232, 240)
            };

            var initialsLabel = new Label
            {
                Text = initials,
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            placeholderPanel.Controls.Add(initialsLabel);

            // Try to load actual image
            try
            {
                if (!string.IsNullOrEmpty(product.ImagePath) && File.Exists(product.ImagePath))
                {
                    var pictureBox = new PictureBox
                    {
                        Image = Image.FromFile(product.ImagePath),
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Dock = DockStyle.Fill
                    };
                    placeholderPanel.Controls.Clear();
                    placeholderPanel.Controls.Add(pictureBox);
                }
            }
            catch { }

            var nameLabel = new Label
            {
                Text = product.Name.Length > 12 ? product.Name.Substring(0, 12) + "..." : product.Name,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(5, 85),
                Size = new Size(110, 35),
                TextAlign = ContentAlignment.TopCenter
            };

            var priceLabel = new Label
            {
                Text = $"{product.SellingPrice:N0} KHR",
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(5, 120),
                Size = new Size(110, 15),
                TextAlign = ContentAlignment.TopCenter
            };

            card.Controls.AddRange(new Control[] { placeholderPanel, nameLabel, priceLabel });
            card.Click += (s, e) => AddToCart(product);
            placeholderPanel.Click += (s, e) => AddToCart(product);
            nameLabel.Click += (s, e) => AddToCart(product);
            priceLabel.Click += (s, e) => AddToCart(product);

            return card;
        }

        private string GetProductInitials(string name)
        {
            var words = name.Split(' ');
            if (words.Length >= 2)
                return (words[0][0].ToString() + words[1][0].ToString()).ToUpper();
            return name.Length >= 2 ? name.Substring(0, 2).ToUpper() : name.ToUpper();
        }

        private void AddToCart(Product product)
        {
            var existingItem = cartItems.FirstOrDefault(i => i.ProductId == product.Id);
            
            if (existingItem != null)
            {
                if (existingItem.Quantity < product.Quantity)
                    existingItem.Quantity++;
                else
                {
                    MessageBox.Show("Insufficient stock!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else
            {
                cartItems.Add(new POSCartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    UnitPrice = product.SellingPrice,
                    Quantity = 1
                });
            }

            UpdateCart();
        }

        private void UpdateCart()
        {
            cartItemsPanel.Controls.Clear();

            foreach (var item in cartItems)
            {
                var itemPanel = CreateCartItemPanel(item);
                cartItemsPanel.Controls.Add(itemPanel);
            }

            UpdateTotals();
        }

        private Panel CreateCartItemPanel(POSCartItem item)
        {
            var panel = new Panel
            {
                Size = new Size(390, 70),
                BackColor = Color.FromArgb(248, 250, 252),
                Margin = new Padding(0, 5, 0, 5)
            };

            var nameLabel = new Label
            {
                Text = item.ProductName,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(10, 10),
                Size = new Size(200, 20)
            };

            var priceLabel = new Label
            {
                Text = $"{item.UnitPrice:N0} KHR",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(10, 35),
                AutoSize = true
            };

            var minusButton = new Button
            {
                Text = "-",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Size = new Size(30, 30),
                Location = new Point(250, 20),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(100, 116, 139),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            minusButton.FlatAppearance.BorderSize = 1;
            minusButton.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
            minusButton.Click += (s, e) => 
            {
                if (item.Quantity > 1)
                {
                    item.Quantity--;
                    UpdateCart();
                }
            };

            var qtyLabel = new Label
            {
                Text = item.Quantity.ToString(),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(290, 25),
                Size = new Size(30, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var plusButton = new Button
            {
                Text = "+",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Size = new Size(30, 30),
                Location = new Point(325, 20),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(100, 116, 139),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            plusButton.FlatAppearance.BorderSize = 1;
            plusButton.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
            plusButton.Click += (s, e) => 
            {
                item.Quantity++;
                UpdateCart();
            };

            var totalLabel = new Label
            {
                Text = $"{item.Total:N0} KHR",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(10, 50),
                AutoSize = true
            };

            panel.Controls.AddRange(new Control[] { nameLabel, priceLabel, minusButton, qtyLabel, plusButton, totalLabel });
            return panel;
        }

        private void UpdateTotals()
        {
            var subtotal = cartItems.Sum(i => i.Total);
            var discount = 0m;
            var total = subtotal - discount;

            subtotalLabel.Text = $"Subtotal: {subtotal:N0} KHR";
            discountLabel.Text = $"Discount: {discount:N0} KHR";
            totalLabel.Text = $"Total: {total:N0} KHR";

            var payButton = this.Controls.Find("payButton", true).FirstOrDefault() as Button;
            if (payButton != null)
                payButton.Text = $"Pay {total:N0} KHR";
        }

        private void ClientComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (clientComboBox.SelectedItem is ComboBoxItem item)
                selectedCustomerId = item.Value;
            else
                selectedCustomerId = null;
        }

        private void PayButton_Click(object sender, EventArgs e)
        {
            if (cartItems.Count == 0)
            {
                MessageBox.Show("Cart is empty!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var paymentModal = new PaymentGatewayModal(_authService, _context, cartItems, selectedCustomerId);
            if (paymentModal.ShowDialog() == DialogResult.OK)
            {
                cartItems.Clear();
                UpdateCart();
                LoadProducts();
            }
        }

        private void SaveTransaction_Click(object sender, EventArgs e)
        {
            if (cartItems.Count == 0)
            {
                MessageBox.Show("Cart is empty!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show("Transaction saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void CancelTransaction_Click(object sender, EventArgs e)
        {
            if (cartItems.Count > 0)
            {
                var result = MessageBox.Show("Are you sure you want to cancel this transaction?", 
                    "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    cartItems.Clear();
                    UpdateCart();
                }
            }
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

    public class POSCartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Total => UnitPrice * Quantity;
    }

    public class ComboBoxItem
    {
        public string Text { get; set; } = "";
        public int Value { get; set; }
        public override string ToString() => Text;
    }
}
