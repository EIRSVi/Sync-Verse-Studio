using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using SyncVerseStudio.Data;
using SyncVerseStudio.Models;
using SyncVerseStudio.Services;
using SyncVerseStudio.Helpers;
using FontAwesome.Sharp;
using System.Drawing.Printing;

namespace SyncVerseStudio.Views
{
    public partial class ModernPOSView : Form
    {
        private readonly AuthenticationService _authService;
        private readonly ApplicationDbContext _context;
        private List<CartItem> _cart = new List<CartItem>();
        
        // UI Components
        private FlowLayoutPanel productsPanel = null!;
        private DataGridView cartGrid = null!;
        private Label totalLabel = null!;
        private Label subtotalLabel = null!;
        private Label taxLabel = null!;
        private TextBox searchBox = null!;
        private ComboBox categoryFilter = null!;
        private TextBox cashAmountBox = null!;
        private Label changeLabel = null!;
        private RadioButton cashRadio = null!;
        private RadioButton cardRadio = null!;
        private RadioButton mobileRadio = null!;
        private Customer? selectedCustomer = null;
        private DataGridView recentSalesGrid = null!;

        public ModernPOSView(AuthenticationService authService)
        {
            _authService = authService;
            _context = new ApplicationDbContext();
            InitializeComponent();
            LoadProducts();
            LoadRecentSales();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form settings
            this.Text = "Point of Sale";
            this.Size = new Size(1600, 900);
            this.BackColor = BrandTheme.Background;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;

            // Main Layout - 3 Columns
            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                Padding = new Padding(0)
            };
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F)); // Products
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F)); // Cart
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F)); // Recent Sales

            // LEFT PANEL - Products
            var leftPanel = CreateProductsPanel();
            mainPanel.Controls.Add(leftPanel, 0, 0);

            // MIDDLE PANEL - Cart & Checkout
            var middlePanel = CreateCartPanel();
            mainPanel.Controls.Add(middlePanel, 1, 0);

            // RIGHT PANEL - Recent Sales
            var rightPanel = CreateRecentSalesPanel();
            mainPanel.Controls.Add(rightPanel, 2, 0);

            this.Controls.Add(mainPanel);
            this.ResumeLayout(false);
        }

        private Panel CreateProductsPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            // Header
            var headerPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(700, 70),
                BackColor = BrandTheme.LimeGreen
            };

            var titleLabel = new Label
            {
                Text = "Products",
                Font = BrandTheme.SubtitleFont,
                ForeColor = BrandTheme.DarkGray,
                Location = new Point(15, 20),
                AutoSize = true
            };

            headerPanel.Controls.Add(titleLabel);

            // Search Panel
            var searchPanel = new Panel
            {
                Location = new Point(0, 80),
                Size = new Size(700, 50),
                BackColor = Color.Transparent
            };

            searchBox = new TextBox
            {
                Location = new Point(0, 10),
                Size = new Size(450, 35),
                Font = BrandTheme.BodyFont,
                PlaceholderText = "Search products...",
                BackColor = BrandTheme.CoolWhite,
                ForeColor = BrandTheme.PrimaryText
            };
            searchBox.TextChanged += (s, e) => LoadProducts(searchBox.Text);

            categoryFilter = new ComboBox
            {
                Location = new Point(460, 10),
                Size = new Size(180, 35),
                Font = BrandTheme.BodyFont,
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = BrandTheme.CoolWhite,
                ForeColor = BrandTheme.PrimaryText
            };
            categoryFilter.Items.Add("All Categories");
            categoryFilter.SelectedIndex = 0;
            categoryFilter.SelectedIndexChanged += (s, e) => LoadProducts(searchBox.Text);

            searchPanel.Controls.AddRange(new Control[] { searchBox, categoryFilter });

            // Products Grid
            productsPanel = new FlowLayoutPanel
            {
                Location = new Point(0, 140),
                Size = new Size(700, 720),
                AutoScroll = true,
                BackColor = Color.Transparent
            };

            panel.Controls.AddRange(new Control[] { headerPanel, searchPanel, productsPanel });
            return panel;
        }

        private Panel CreateCartPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 250, 252),
                Padding = new Padding(10)
            };

            // Header
            var headerPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(470, 60),
                BackColor = BrandTheme.MediumBlue
            };

            var titleLabel = new Label
            {
                Text = "Shopping Cart",
                Font = BrandTheme.SubtitleFont,
                ForeColor = BrandTheme.HeaderText,
                Location = new Point(15, 18),
                AutoSize = true
            };

            headerPanel.Controls.Add(titleLabel);

            // Cart Grid
            cartGrid = new DataGridView
            {
                Location = new Point(10, 70),
                Size = new Size(450, 300),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                RowHeadersVisible = false,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 11),
                    Padding = new Padding(5),
                    SelectionBackColor = Color.FromArgb(219, 234, 254),
                    SelectionForeColor = Color.FromArgb(30, 41, 59)
                },
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(241, 245, 249),
                    ForeColor = Color.FromArgb(30, 41, 59),
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Padding = new Padding(8)
                },
                ColumnHeadersHeight = 35,
                RowTemplate = { Height = 40 }
            };

            cartGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Product", HeaderText = "Product", Width = 200 });
            cartGrid.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                Name = "Qty", 
                HeaderText = "Qty", 
                Width = 60,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            cartGrid.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                Name = "Price", 
                HeaderText = "Price", 
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            cartGrid.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                Name = "Total", 
                HeaderText = "Total", 
                Width = 90,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    ForeColor = Color.FromArgb(34, 197, 94)
                }
            });

            var deleteButton = new DataGridViewButtonColumn
            {
                Name = "Delete",
                HeaderText = "",
                Text = "✕",
                UseColumnTextForButtonValue = true,
                Width = 40
            };
            cartGrid.Columns.Add(deleteButton);
            cartGrid.CellContentClick += CartGrid_CellContentClick;
            cartGrid.CellEndEdit += CartGrid_CellEndEdit;

            // Summary Panel
            var summaryPanel = new Panel
            {
                Location = new Point(10, 380),
                Size = new Size(450, 120),
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            subtotalLabel = new Label
            {
                Text = "Subtotal: $0.00",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(15, 10),
                AutoSize = true
            };

            taxLabel = new Label
            {
                Text = "Tax (10%): $0.00",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(15, 40),
                AutoSize = true
            };

            totalLabel = new Label
            {
                Text = "$0.00",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.FromArgb(34, 197, 94),
                Location = new Point(15, 65),
                AutoSize = true
            };

            summaryPanel.Controls.AddRange(new Control[] { subtotalLabel, taxLabel, totalLabel });

            // Payment Panel
            var paymentPanel = new Panel
            {
                Location = new Point(10, 510),
                Size = new Size(450, 180),
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            var paymentLabel = new Label
            {
                Text = "Payment Method:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(15, 10),
                AutoSize = true
            };

            cashRadio = new RadioButton
            {
                Text = "Cash",
                Location = new Point(15, 40),
                Size = new Size(120, 30),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Checked = true
            };
            cashRadio.CheckedChanged += (s, e) => { if (cashRadio.Checked) cashAmountBox.Enabled = true; };

            cardRadio = new RadioButton
            {
                Text = "Card",
                Location = new Point(145, 40),
                Size = new Size(120, 30),
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            cardRadio.CheckedChanged += (s, e) => { if (cardRadio.Checked) { cashAmountBox.Enabled = false; UpdateChange(); } };

            mobileRadio = new RadioButton
            {
                Text = "Mobile",
                Location = new Point(275, 40),
                Size = new Size(120, 30),
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            mobileRadio.CheckedChanged += (s, e) => { if (mobileRadio.Checked) { cashAmountBox.Enabled = false; UpdateChange(); } };

            var cashLabel = new Label
            {
                Text = "Cash Amount:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(15, 80),
                AutoSize = true
            };

            cashAmountBox = new TextBox
            {
                Location = new Point(15, 105),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 14),
                Text = "0.00"
            };
            cashAmountBox.TextChanged += (s, e) => UpdateChange();

            changeLabel = new Label
            {
                Text = "Change: $0.00",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(249, 115, 22),
                Location = new Point(15, 145),
                AutoSize = true
            };

            paymentPanel.Controls.AddRange(new Control[] { 
                paymentLabel, cashRadio, cardRadio, mobileRadio, cashLabel, cashAmountBox, changeLabel 
            });

            // Action Buttons
            var checkoutButton = new Button
            {
                Text = "COMPLETE SALE",
                Location = new Point(10, 700),
                Size = new Size(450, 55),
                BackColor = BrandTheme.MediumBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            checkoutButton.FlatAppearance.BorderSize = 0;
            checkoutButton.Click += CheckoutButton_Click;

            var clearButton = new Button
            {
                Text = "CLEAR CART",
                Location = new Point(10, 765),
                Size = new Size(220, 45),
                BackColor = BrandTheme.DarkGray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            clearButton.FlatAppearance.BorderSize = 0;
            clearButton.Click += (s, e) => ClearCart();

            var holdButton = new Button
            {
                Text = "HOLD",
                Location = new Point(240, 765),
                Size = new Size(220, 45),
                BackColor = BrandTheme.LimeGreen,
                ForeColor = BrandTheme.DarkGray,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            holdButton.FlatAppearance.BorderSize = 0;

            panel.Controls.AddRange(new Control[] { 
                headerPanel, cartGrid, summaryPanel, paymentPanel, checkoutButton, clearButton, holdButton 
            });

            return panel;
        }

        private Panel CreateRecentSalesPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            // Header
            var headerPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(390, 60),
                BackColor = Color.FromArgb(59, 130, 246)
            };

            var titleIcon = new IconPictureBox
            {
                IconChar = IconChar.Receipt,
                IconColor = Color.White,
                IconSize = 24,
                Location = new Point(15, 18),
                Size = new Size(24, 24),
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(titleIcon);

            var titleLabel = new Label
            {
                Text = "Recent Sales",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(45, 18),
                AutoSize = true
            };

            headerPanel.Controls.Add(titleLabel);

            // Recent Sales Grid
            recentSalesGrid = new DataGridView
            {
                Location = new Point(10, 70),
                Size = new Size(370, 790),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 9),
                    Padding = new Padding(5),
                    SelectionBackColor = Color.FromArgb(219, 234, 254),
                    SelectionForeColor = Color.FromArgb(30, 41, 59),
                    WrapMode = DataGridViewTriState.True
                },
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(241, 245, 249),
                    ForeColor = Color.FromArgb(30, 41, 59),
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    Padding = new Padding(5)
                },
                ColumnHeadersHeight = 35,
                RowTemplate = { Height = 50 }
            };

            recentSalesGrid.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                Name = "Invoice", 
                HeaderText = "Invoice",
                DefaultCellStyle = new DataGridViewCellStyle { Font = new Font("Consolas", 8, FontStyle.Bold) }
            });
            recentSalesGrid.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                Name = "Time", 
                HeaderText = "Time"
            });
            recentSalesGrid.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                Name = "Amount", 
                HeaderText = "Amount",
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    ForeColor = Color.FromArgb(34, 197, 94)
                }
            });

            panel.Controls.AddRange(new Control[] { headerPanel, recentSalesGrid });
            return panel;
        }

        private void LoadProducts(string? searchTerm = null)
        {
            try
            {
                productsPanel.Controls.Clear();

                var query = _context.Products
                    .Include(p => p.Category)
                    .Where(p => p.IsActive && p.Quantity > 0)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(p => p.Name.Contains(searchTerm) || 
                                           (p.Barcode != null && p.Barcode.Contains(searchTerm)));
                }

                var products = query.OrderBy(p => p.Name).Take(50).ToList();

                // Load categories
                if (categoryFilter.Items.Count == 1)
                {
                    var categories = _context.Categories.Where(c => c.IsActive).ToList();
                    foreach (var cat in categories)
                    {
                        categoryFilter.Items.Add(new ComboBoxItem { Text = cat.Name, Value = cat.Id });
                    }
                }

                foreach (var product in products)
                {
                    var productCard = CreateProductCard(product);
                    productsPanel.Controls.Add(productCard);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Panel CreateProductCard(Product product)
        {
            var card = new Panel
            {
                Size = new Size(160, 140),
                BackColor = Color.White,
                Margin = new Padding(8),
                Cursor = Cursors.Hand,
                Tag = product,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Product name
            var nameLabel = new Label
            {
                Text = product.Name.Length > 20 ? product.Name.Substring(0, 20) + "..." : product.Name,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(8, 10),
                Size = new Size(144, 40),
                TextAlign = ContentAlignment.TopLeft
            };

            // Price
            var priceLabel = new Label
            {
                Text = $"${product.SellingPrice:N2}",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(34, 197, 94),
                Location = new Point(8, 55),
                AutoSize = true
            };

            // Stock
            var stockLabel = new Label
            {
                Text = $"Stock: {product.Quantity}",
                Font = new Font("Segoe UI", 8),
                ForeColor = product.IsLowStock ? Color.FromArgb(239, 68, 68) : Color.FromArgb(100, 116, 139),
                Location = new Point(8, 90),
                AutoSize = true
            };

            // Add button
            var addButton = new Button
            {
                Text = "+ ADD",
                Location = new Point(8, 110),
                Size = new Size(144, 25),
                BackColor = Color.FromArgb(34, 197, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            addButton.FlatAppearance.BorderSize = 0;
            addButton.Click += (s, e) => AddToCart(product);

            card.Controls.AddRange(new Control[] { nameLabel, priceLabel, stockLabel, addButton });

            // Hover effect
            card.MouseEnter += (s, e) => card.BackColor = Color.FromArgb(241, 245, 249);
            card.MouseLeave += (s, e) => card.BackColor = Color.White;
            card.Click += (s, e) => AddToCart(product);

            return card;
        }

        private void AddToCart(Product product)
        {
            var existingItem = _cart.FirstOrDefault(c => c.ProductId == product.Id);
            
            if (existingItem != null)
            {
                if (existingItem.Quantity < product.Quantity)
                {
                    existingItem.Quantity++;
                }
                else
                {
                    MessageBox.Show("Insufficient stock!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else
            {
                _cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.SellingPrice,
                    Quantity = 1,
                    MaxStock = product.Quantity
                });
            }

            UpdateCart();
            System.Media.SystemSounds.Beep.Play();
        }

        private void UpdateCart()
        {
            cartGrid.Rows.Clear();
            
            foreach (var item in _cart)
            {
                cartGrid.Rows.Add(item.ProductName, item.Quantity, $"${item.Price:N2}", $"${item.Total:N2}");
            }

            var subtotal = _cart.Sum(c => c.Total);
            var tax = subtotal * 0.10m;
            var total = subtotal + tax;

            subtotalLabel.Text = $"Subtotal: ${subtotal:N2}";
            taxLabel.Text = $"Tax (10%): ${tax:N2}";
            totalLabel.Text = $"${total:N2}";

            if (cashRadio.Checked)
            {
                cashAmountBox.Text = total.ToString("N2");
            }

            UpdateChange();
        }

        private void UpdateChange()
        {
            var total = _cart.Sum(c => c.Total) * 1.10m;
            
            if (cashRadio.Checked && decimal.TryParse(cashAmountBox.Text, out var paid))
            {
                var change = paid - total;
                changeLabel.Text = $"Change: ${change:N2}";
                changeLabel.ForeColor = change >= 0 ? Color.FromArgb(34, 197, 94) : Color.FromArgb(239, 68, 68);
            }
            else
            {
                changeLabel.Text = "Change: $0.00";
                changeLabel.ForeColor = Color.FromArgb(34, 197, 94);
            }
        }

        private void CartGrid_CellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == cartGrid.Columns["Delete"].Index)
            {
                _cart.RemoveAt(e.RowIndex);
                UpdateCart();
            }
        }

        private void CartGrid_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == cartGrid.Columns["Qty"].Index)
            {
                if (int.TryParse(cartGrid.Rows[e.RowIndex].Cells["Qty"].Value?.ToString(), out var newQty))
                {
                    if (newQty > 0 && newQty <= _cart[e.RowIndex].MaxStock)
                    {
                        _cart[e.RowIndex].Quantity = newQty;
                        UpdateCart();
                    }
                    else
                    {
                        MessageBox.Show("Invalid quantity!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        UpdateCart();
                    }
                }
            }
        }

        private void ClearCart()
        {
            if (_cart.Any())
            {
                var result = MessageBox.Show("Clear all items from cart?", "Confirm", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    _cart.Clear();
                    UpdateCart();
                }
            }
        }

        private void CheckoutButton_Click(object? sender, EventArgs e)
        {
            if (!_cart.Any())
            {
                MessageBox.Show("Cart is empty!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var total = _cart.Sum(c => c.Total) * 1.10m;
            decimal amountPaid = total;
            PaymentMethod paymentMethod = PaymentMethod.Cash;

            if (cashRadio.Checked)
            {
                if (!decimal.TryParse(cashAmountBox.Text, out amountPaid) || amountPaid < total)
                {
                    MessageBox.Show("Invalid or insufficient payment amount!", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                paymentMethod = PaymentMethod.Cash;
            }
            else if (cardRadio.Checked)
            {
                paymentMethod = PaymentMethod.Card;
            }
            else if (mobileRadio.Checked)
            {
                paymentMethod = PaymentMethod.Mobile;
            }

            ProcessSale(paymentMethod, amountPaid);
        }

        private void ProcessSale(PaymentMethod paymentMethod, decimal amountPaid)
        {
            try
            {
                var subtotal = _cart.Sum(c => c.Total);
                var tax = subtotal * 0.10m;
                var total = subtotal + tax;

                // Create sale
                var sale = new Sale
                {
                    InvoiceNumber = GenerateInvoiceNumber(),
                    CashierId = _authService.CurrentUser.Id,
                    CustomerId = selectedCustomer?.Id,
                    SaleDate = DateTime.Now,
                    TotalAmount = total,
                    TaxAmount = tax,
                    DiscountAmount = 0,
                    PaymentMethod = paymentMethod,
                    Status = SaleStatus.Completed
                };

                // Add sale items and update inventory
                foreach (var cartItem in _cart)
                {
                    // Add sale item
                    var saleItem = new SaleItem
                    {
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.Price,
                        TotalPrice = cartItem.Total
                    };
                    sale.SaleItems.Add(saleItem);

                    // Update product quantity
                    var product = _context.Products.Find(cartItem.ProductId);
                    if (product != null)
                    {
                        product.Quantity -= cartItem.Quantity;
                        product.UpdatedAt = DateTime.Now;
                    }
                }

                // Save sale first
                _context.Sales.Add(sale);
                _context.SaveChanges();

                // Create inventory movements after sale is saved
                foreach (var cartItem in _cart)
                {
                    _context.InventoryMovements.Add(new InventoryMovement
                    {
                        ProductId = cartItem.ProductId,
                        MovementType = MovementType.Sale,
                        Quantity = -cartItem.Quantity,
                        UserId = _authService.CurrentUser.Id,
                        Reference = sale.InvoiceNumber,
                        CreatedAt = DateTime.Now
                    });
                }
                _context.SaveChanges();

                // Print invoice
                PrintInvoice(sale, amountPaid);

                // Show success
                var change = amountPaid - total;
                MessageBox.Show($"SALE COMPLETED\n\n" +
                    $"Invoice: {sale.InvoiceNumber}\n" +
                    $"Total: ${total:N2}\n" +
                    $"Paid: ${amountPaid:N2}\n" +
                    $"Change: ${change:N2}", 
                    "Transaction Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Clear and refresh
                _cart.Clear();
                selectedCustomer = null;
                UpdateCart();
                LoadProducts();
                LoadRecentSales();
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? $"\n\nDetails: {ex.InnerException.Message}" : "";
                MessageBox.Show($"Error processing sale: {ex.Message}{innerMessage}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GenerateInvoiceNumber()
        {
            // Format: INV-YYMMDD-NNNN (max 17 chars, fits in 20 char limit)
            var dateStr = DateTime.Now.ToString("yyMMdd");
            var timeStr = DateTime.Now.ToString("HHmmss");
            return $"INV-{dateStr}-{timeStr}";
        }

        private void LoadRecentSales()
        {
            try
            {
                var cashierId = _authService.CurrentUser.Id;
                var recentSales = _context.Sales
                    .Where(s => s.CashierId == cashierId)
                    .OrderByDescending(s => s.SaleDate)
                    .Take(20)
                    .ToList();

                recentSalesGrid.Rows.Clear();
                foreach (var sale in recentSales)
                {
                    recentSalesGrid.Rows.Add(
                        sale.InvoiceNumber,
                        sale.SaleDate.ToString("HH:mm"),
                        $"${sale.TotalAmount:N2}"
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading recent sales: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintInvoice(Sale sale, decimal amountPaid)
        {
            try
            {
                var printDoc = new PrintDocument();
                printDoc.PrintPage += (sender, e) => PrintInvoicePage(sender, e, sale, amountPaid);
                
                // Show print preview
                var printPreview = new PrintPreviewDialog
                {
                    Document = printDoc,
                    Width = 800,
                    Height = 600
                };
                printPreview.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error printing invoice: {ex.Message}", "Print Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void PrintInvoicePage(object? sender, PrintPageEventArgs e, Sale sale, decimal amountPaid)
        {
            var graphics = e.Graphics!;
            var font = new Font("Courier New", 10);
            var boldFont = new Font("Courier New", 10, FontStyle.Bold);
            var titleFont = new Font("Courier New", 14, FontStyle.Bold);
            var brush = Brushes.Black;
            var y = 50;

            // Header
            graphics.DrawString("SYNCVERSE STUDIO", titleFont, brush, 200, y);
            y += 30;
            graphics.DrawString("Point of Sale System", font, brush, 220, y);
            y += 20;
            graphics.DrawString("================================", font, brush, 150, y);
            y += 30;

            // Invoice details
            graphics.DrawString($"Invoice: {sale.InvoiceNumber}", boldFont, brush, 50, y);
            y += 20;
            graphics.DrawString($"Date: {sale.SaleDate:yyyy-MM-dd HH:mm:ss}", font, brush, 50, y);
            y += 20;
            graphics.DrawString($"Cashier: {_authService.CurrentUser.FullName}", font, brush, 50, y);
            y += 20;
            graphics.DrawString($"Customer: {selectedCustomer?.FullName ?? "Walk-in"}", font, brush, 50, y);
            y += 20;
            graphics.DrawString("================================", font, brush, 150, y);
            y += 30;

            // Items header
            graphics.DrawString("Item", boldFont, brush, 50, y);
            graphics.DrawString("Qty", boldFont, brush, 300, y);
            graphics.DrawString("Price", boldFont, brush, 380, y);
            graphics.DrawString("Total", boldFont, brush, 480, y);
            y += 20;
            graphics.DrawString("--------------------------------", font, brush, 150, y);
            y += 20;

            // Items
            foreach (var item in sale.SaleItems)
            {
                var product = _context.Products.Find(item.ProductId);
                var name = product?.Name ?? "Unknown";
                if (name.Length > 25) name = name.Substring(0, 25);
                
                graphics.DrawString(name, font, brush, 50, y);
                graphics.DrawString(item.Quantity.ToString(), font, brush, 300, y);
                graphics.DrawString($"${item.UnitPrice:N2}", font, brush, 380, y);
                graphics.DrawString($"${item.TotalPrice:N2}", font, brush, 480, y);
                y += 20;
            }

            y += 10;
            graphics.DrawString("================================", font, brush, 150, y);
            y += 25;

            // Totals
            var subtotal = sale.TotalAmount - sale.TaxAmount;
            graphics.DrawString($"Subtotal:", font, brush, 350, y);
            graphics.DrawString($"${subtotal:N2}", boldFont, brush, 480, y);
            y += 20;
            graphics.DrawString($"Tax (10%):", font, brush, 350, y);
            graphics.DrawString($"${sale.TaxAmount:N2}", boldFont, brush, 480, y);
            y += 20;
            graphics.DrawString($"TOTAL:", titleFont, brush, 350, y);
            graphics.DrawString($"${sale.TotalAmount:N2}", titleFont, brush, 450, y);
            y += 30;

            // Payment
            graphics.DrawString($"Payment Method: {sale.PaymentMethod}", font, brush, 50, y);
            y += 20;
            graphics.DrawString($"Amount Paid: ${amountPaid:N2}", font, brush, 50, y);
            y += 20;
            var change = amountPaid - sale.TotalAmount;
            graphics.DrawString($"Change: ${change:N2}", boldFont, brush, 50, y);
            y += 40;

            // Footer
            graphics.DrawString("Thank you for your business!", font, brush, 180, y);
            y += 20;
            graphics.DrawString("Please come again!", font, brush, 210, y);
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

    // Helper classes
    public class ComboBoxItem
    {
        public string Text { get; set; } = string.Empty;
        public int Value { get; set; }
        public override string ToString() => Text;
    }
}
