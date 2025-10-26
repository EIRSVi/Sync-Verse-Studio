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

namespace SyncVerseStudio.Views
{
    public partial class CashierDashboardView : Form
    {
        private readonly AuthenticationService _authService;
        private readonly ApplicationDbContext _context;
        
        // Main panels
        private Panel sidebarPanel = null!;
        private Panel topBarPanel = null!;
        private Panel productsPanel = null!;
        private Panel cartPanel = null!;
        private Panel bottomBarPanel = null!;
        
        // Product display
        private FlowLayoutPanel productCardsPanel = null!;
        private FlowLayoutPanel categoryButtonsPanel = null!;
        
        // Cart
        private FlowLayoutPanel cartItemsPanel = null!;
        private List<CartItem> cartItems = new List<CartItem>();
        
        // Labels
        private Label userLabel = null!;
        private Label subtotalLabel = null!;
        private Label taxLabel = null!;
        private Label totalLabel = null!;
        private Label itemCountLabel = null!;
        private Label discountLabel = null!;
        
        // Buttons
        private Button payButton = null!;
        private TextBox searchBox = null!;
        
        private decimal taxRate = 0.05m; // 5% tax

        public CashierDashboardView(AuthenticationService authService)
        {
            _authService = authService;
            _context = new ApplicationDbContext();
            InitializeComponent();
            LoadCategories();
            LoadProducts();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form settings
            this.Text = "LITHOSPOS - Cashier";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(245, 245, 245);
            this.FormBorderStyle = FormBorderStyle.None;

            // Top Bar Panel
            topBarPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.White
            };
            CreateTopBar();

            // Sidebar Panel (Categories)
            sidebarPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 220,
                BackColor = Color.FromArgb(95, 73, 64),
                Padding = new Padding(0, 10, 0, 0)
            };
            CreateSidebar();

            // Cart Panel (Right side)
            cartPanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = 420,
                BackColor = Color.White,
                Padding = new Padding(20)
            };
            CreateCartPanel();

            // Bottom Bar Panel (Payment)
            bottomBarPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 80,
                BackColor = Color.FromArgb(95, 73, 64)
            };
            CreateBottomBar();

            // Products Panel (Center)
            productsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(245, 245, 245),
                Padding = new Padding(20)
            };
            CreateProductsPanel();

            // Add all to form
            this.Controls.AddRange(new Control[] { 
                productsPanel, cartPanel, sidebarPanel, bottomBarPanel, topBarPanel
            });

            this.ResumeLayout(false);
        }

        private void CreateTopBar()
        {
            // Logo
            var logoIcon = new IconPictureBox
            {
                IconChar = IconChar.Store,
                IconColor = Color.FromArgb(95, 73, 64),
                IconSize = 32,
                Location = new Point(20, 20),
                Size = new Size(32, 32)
            };

            var logoLabel = new Label
            {
                Text = "LITHOSPOS",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(95, 73, 64),
                Location = new Point(60, 20),
                AutoSize = true
            };

            // Search Box
            searchBox = new TextBox
            {
                Location = new Point(250, 20),
                Size = new Size(300, 30),
                Font = new Font("Segoe UI", 12),
                PlaceholderText = "Search products..."
            };
            searchBox.TextChanged += SearchBox_TextChanged;

            // User Info
            userLabel = new Label
            {
                Text = $"User: {_authService.CurrentUser.FullName}",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(95, 73, 64),
                AutoSize = true
            };
            userLabel.Location = new Point(topBarPanel.Width - userLabel.Width - 20, 25);

            topBarPanel.Controls.AddRange(new Control[] { logoIcon, logoLabel, searchBox, userLabel });
        }

        private void CreateSidebar()
        {
            categoryButtonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                AutoScroll = true,
                WrapContents = false,
                Padding = new Padding(0)
            };

            sidebarPanel.Controls.Add(categoryButtonsPanel);
        }

        private void CreateProductsPanel()
        {
            productCardsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(245, 245, 245)
            };

            productsPanel.Controls.Add(productCardsPanel);
        }

        private void CreateCartPanel()
        {
            var cartTitle = new Label
            {
                Text = "Current Order",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(20, 10),
                AutoSize = true
            };

            cartItemsPanel = new FlowLayoutPanel
            {
                Location = new Point(20, 50),
                Size = new Size(380, 500),
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Color.White
            };

            // Summary Panel
            var summaryPanel = new Panel
            {
                Location = new Point(20, 560),
                Size = new Size(380, 150),
                BackColor = Color.FromArgb(248, 250, 252)
            };

            itemCountLabel = new Label
            {
                Text = "Items: 0",
                Font = new Font("Segoe UI", 11),
                Location = new Point(10, 10),
                AutoSize = true
            };

            subtotalLabel = new Label
            {
                Text = "Subtotal: dh 0.00",
                Font = new Font("Segoe UI", 11),
                Location = new Point(10, 35),
                AutoSize = true
            };

            discountLabel = new Label
            {
                Text = "Discount: dh 0.0",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(34, 197, 94),
                Location = new Point(10, 60),
                AutoSize = true
            };

            taxLabel = new Label
            {
                Text = "VAT 5.0%: dh 0.00",
                Font = new Font("Segoe UI", 11),
                Location = new Point(10, 85),
                AutoSize = true
            };

            totalLabel = new Label
            {
                Text = "dh 0.00",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(95, 73, 64),
                Location = new Point(10, 110),
                AutoSize = true
            };

            summaryPanel.Controls.AddRange(new Control[] { 
                itemCountLabel, subtotalLabel, discountLabel, taxLabel, totalLabel 
            });

            cartPanel.Controls.AddRange(new Control[] { cartTitle, cartItemsPanel, summaryPanel });
        }

        private void CreateBottomBar()
        {
            var menuButton = new Button
            {
                Text = "☰",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                Size = new Size(80, 60),
                Location = new Point(20, 10),
                BackColor = Color.FromArgb(75, 53, 44),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            menuButton.FlatAppearance.BorderSize = 0;

            var ordersButton = new Button
            {
                Text = "ORDERS",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(150, 60),
                Location = new Point(320, 10),
                BackColor = Color.FromArgb(75, 53, 44),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            ordersButton.FlatAppearance.BorderSize = 0;

            var cashLabel = new Label
            {
                Text = "💰 CASH",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(640, 25)
            };

            payButton = new Button
            {
                Text = "Pay",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Size = new Size(200, 60),
                BackColor = Color.FromArgb(255, 152, 0),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            payButton.FlatAppearance.BorderSize = 0;
            payButton.Click += PayButton_Click;
            
            // Position pay button on the right
            payButton.Location = new Point(bottomBarPanel.Width - payButton.Width - 20, 10);

            bottomBarPanel.Controls.AddRange(new Control[] { 
                menuButton, ordersButton, cashLabel, payButton 
            });
        }

        private void LoadCategories()
        {
            try
            {
                var categories = _context.Categories
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.Name)
                    .ToList();

                categoryButtonsPanel.Controls.Clear();

                // Add "All" button
                var allButton = CreateCategoryButton("All", null);
                categoryButtonsPanel.Controls.Add(allButton);

                foreach (var category in categories)
                {
                    var button = CreateCategoryButton(category.Name, category.Id);
                    categoryButtonsPanel.Controls.Add(button);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Button CreateCategoryButton(string name, int? categoryId)
        {
            var button = new Button
            {
                Text = name,
                Size = new Size(220, 50),
                BackColor = Color.FromArgb(95, 73, 64),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0),
                Cursor = Cursors.Hand,
                Tag = categoryId
            };
            button.FlatAppearance.BorderSize = 0;
            button.Click += (s, e) => FilterByCategory(categoryId);
            
            button.MouseEnter += (s, e) => button.BackColor = Color.FromArgb(75, 53, 44);
            button.MouseLeave += (s, e) => button.BackColor = Color.FromArgb(95, 73, 64);

            return button;
        }

        private void FilterByCategory(int? categoryId)
        {
            LoadProducts(categoryId);
        }

        private void LoadProducts(int? categoryId = null, string searchText = "")
        {
            try
            {
                var query = _context.Products
                    .Include(p => p.Category)
                    .Where(p => p.Quantity > 0 && p.IsActive);

                if (categoryId.HasValue)
                {
                    query = query.Where(p => p.CategoryId == categoryId.Value);
                }

                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    query = query.Where(p => p.Name.Contains(searchText) || 
                                           (p.Barcode != null && p.Barcode.Contains(searchText)));
                }

                var products = query.OrderBy(p => p.Name).ToList();

                productCardsPanel.Controls.Clear();

                foreach (var product in products)
                {
                    var card = CreateProductCard(product);
                    productCardsPanel.Controls.Add(card);
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
                Size = new Size(180, 240),
                BackColor = Color.White,
                Margin = new Padding(10),
                Cursor = Cursors.Hand
            };

            // Product image placeholder
            var imageBox = new PictureBox
            {
                Size = new Size(180, 140),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(240, 240, 240),
                SizeMode = PictureBoxSizeMode.Zoom
            };

            // Try to load product image
            try
            {
                if (!string.IsNullOrEmpty(product.ImagePath) && File.Exists(product.ImagePath))
                {
                    imageBox.Image = Image.FromFile(product.ImagePath);
                }
                else
                {
                    var imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "product", $"{product.Id}.jpg");
                    if (File.Exists(imagePath))
                    {
                        imageBox.Image = Image.FromFile(imagePath);
                    }
                }
            }
            catch { }

            var nameLabel = new Label
            {
                Text = product.Name,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(10, 150),
                Size = new Size(160, 40),
                ForeColor = Color.FromArgb(30, 41, 59)
            };

            var priceLabel = new Label
            {
                Text = $"dh {product.SellingPrice:N2}",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(95, 73, 64),
                Location = new Point(10, 195),
                AutoSize = true
            };

            var stockLabel = new Label
            {
                Text = $"Stock: {product.Quantity}",
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(10, 220),
                AutoSize = true
            };

            card.Controls.AddRange(new Control[] { imageBox, nameLabel, priceLabel, stockLabel });
            
            card.Click += (s, e) => AddToCart(product);
            imageBox.Click += (s, e) => AddToCart(product);
            nameLabel.Click += (s, e) => AddToCart(product);
            priceLabel.Click += (s, e) => AddToCart(product);

            return card;
        }

        private void AddToCart(Product product)
        {
            var existingItem = cartItems.FirstOrDefault(i => i.ProductId == product.Id);
            
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
                cartItems.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    UnitPrice = product.SellingPrice,
                    Quantity = 1,
                    MaxStock = product.Quantity
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

        private Panel CreateCartItemPanel(CartItem item)
        {
            var panel = new Panel
            {
                Size = new Size(360, 80),
                BackColor = Color.FromArgb(248, 250, 252),
                Margin = new Padding(0, 5, 0, 5),
                Padding = new Padding(10)
            };

            var nameLabel = new Label
            {
                Text = item.ProductName,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(10, 10),
                Size = new Size(200, 20),
                ForeColor = Color.FromArgb(30, 41, 59)
            };

            var priceLabel = new Label
            {
                Text = $"dh {item.UnitPrice:N2}",
                Font = new Font("Segoe UI", 9),
                Location = new Point(10, 35),
                AutoSize = true,
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            var qtyLabel = new Label
            {
                Text = $"{item.Quantity} x",
                Font = new Font("Segoe UI", 10),
                Location = new Point(10, 55),
                AutoSize = true
            };

            var totalLabel = new Label
            {
                Text = $"dh {item.Total:N2}",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(95, 73, 64),
                Location = new Point(250, 30),
                AutoSize = true
            };

            var removeButton = new Button
            {
                Text = "×",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Size = new Size(30, 30),
                Location = new Point(320, 25),
                BackColor = Color.FromArgb(239, 68, 68),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            removeButton.FlatAppearance.BorderSize = 0;
            removeButton.Click += (s, e) => RemoveFromCart(item);

            panel.Controls.AddRange(new Control[] { 
                nameLabel, priceLabel, qtyLabel, totalLabel, removeButton 
            });

            return panel;
        }

        private void RemoveFromCart(CartItem item)
        {
            cartItems.Remove(item);
            UpdateCart();
        }

        private void UpdateTotals()
        {
            var subtotal = cartItems.Sum(i => i.Total);
            var discount = 0m;
            var tax = subtotal * taxRate;
            var total = subtotal - discount + tax;

            itemCountLabel.Text = $"Items: {cartItems.Sum(i => i.Quantity)}";
            subtotalLabel.Text = $"Subtotal: dh {subtotal:N2}";
            discountLabel.Text = $"Discount: dh {discount:N2}";
            taxLabel.Text = $"VAT {taxRate * 100:N1}%: dh {tax:N2}";
            totalLabel.Text = $"dh {total:N2}";
        }

        private void SearchBox_TextChanged(object? sender, EventArgs e)
        {
            LoadProducts(null, searchBox.Text);
        }

        private void PayButton_Click(object? sender, EventArgs e)
        {
            if (cartItems.Count == 0)
            {
                MessageBox.Show("Cart is empty!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var subtotal = cartItems.Sum(i => i.Total);
                var tax = subtotal * taxRate;
                var total = subtotal + tax;

                var sale = new Sale
                {
                    InvoiceNumber = GenerateInvoiceNumber(),
                    SaleDate = DateTime.Now,
                    CashierId = _authService.CurrentUser.Id,
                    TaxAmount = tax,
                    TotalAmount = total,
                    PaymentMethod = PaymentMethod.Cash,
                    Status = SaleStatus.Completed,
                    SaleItems = cartItems.Select(i => new SaleItem
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        TotalPrice = i.Total
                    }).ToList()
                };

                _context.Sales.Add(sale);

                // Update stock
                foreach (var item in cartItems)
                {
                    var product = _context.Products.Find(item.ProductId);
                    if (product != null)
                    {
                        product.Quantity -= item.Quantity;
                        product.UpdatedAt = DateTime.Now;
                    }
                }

                _context.SaveChanges();

                MessageBox.Show($"Sale completed!\nInvoice: {sale.InvoiceNumber}\nTotal: dh {total:N2}", 
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Clear cart
                cartItems.Clear();
                UpdateCart();
                LoadProducts();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing sale: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GenerateInvoiceNumber()
        {
            return $"INV-{DateTime.Now:yyyyMMdd}-{DateTime.Now.Ticks % 10000:D4}";
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
