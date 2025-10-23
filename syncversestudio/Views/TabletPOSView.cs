using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using SyncVerseStudio.Data;
using SyncVerseStudio.Models;
using SyncVerseStudio.Services;
using SyncVerseStudio.Helpers;
using FontAwesome.Sharp;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SyncVerseStudio.Views
{
    /// <summary>
    /// Modern tablet-style POS interface
    /// </summary>
    public partial class TabletPOSView : Form
    {
        private readonly AuthenticationService _authService;
        private readonly ApplicationDbContext _context;
        private decimal _taxRate = 0.10m;

        // UI Components
        private Panel _sidebarPanel, _topBarPanel, _productsPanel, _cartPanel, _bottomBarPanel;
        private FlowLayoutPanel _productCardsPanel;
        private FlowLayoutPanel _cartItemsPanel;
        private TextBox _searchBox;
        private Label _userLabel, _subtotalLabel, _taxLabel, _totalLabel, _itemCountLabel;
        private Button _payButton;
        private List<CartItem> _cartItems = new List<CartItem>();

        public TabletPOSView(AuthenticationService authService)
        {
            _authService = authService;
            _context = new ApplicationDbContext();
            
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Text = "LITHOSPOS - Point of Sale";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(245, 245, 245);
            this.MinimumSize = new Size(1024, 768);
            
            CreateUI();
            this.ResumeLayout(false);
        }

        private void CreateUI()
        {
            CreateTopBar();
            CreateSidebar();
            CreateProductsPanel();
            CreateCartPanel();
            CreateBottomBar();
        }

        #region Top Bar
        private void CreateTopBar()
        {
            _topBarPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.White
            };

            // Logo
            var logoLabel = new Label
            {
                Text = "≡ ⚡ LITHOSPOS",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 80, 80),
                Location = new Point(20, 18),
                AutoSize = true
            };
            _topBarPanel.Controls.Add(logoLabel);

            // Search icon
            var searchIcon = new IconPictureBox
            {
                IconChar = IconChar.Search,
                IconColor = Color.FromArgb(100, 100, 100),
                IconSize = 24,
                Location = new Point(200, 18),
                Size = new Size(24, 24),
                Cursor = Cursors.Hand
            };
            _topBarPanel.Controls.Add(searchIcon);

            // User icon and label
            var userIcon = new IconPictureBox
            {
                IconChar = IconChar.UserCircle,
                IconColor = Color.FromArgb(100, 100, 100),
                IconSize = 24,
                Location = new Point(this.ClientSize.Width - 150, 18),
                Size = new Size(24, 24),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            _topBarPanel.Controls.Add(userIcon);

            _userLabel = new Label
            {
                Text = "User",
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.FromArgb(80, 80, 80),
                Location = new Point(this.ClientSize.Width - 120, 20),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            _topBarPanel.Controls.Add(_userLabel);

            this.Controls.Add(_topBarPanel);
        }
        #endregion

        #region Sidebar
        private void CreateSidebar()
        {
            _sidebarPanel = new Panel
            {
                Location = new Point(0, 60),
                Width = 220,
                Height = this.ClientSize.Height - 60 - 70,
                BackColor = Color.FromArgb(101, 84, 72),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left
            };

            int yPos = 10;
            var categories = new[] {
                "Biscuits", "Beverages", "Stationary", "Edible Oils",
                "Fruits", "Vegitables", "Masalas", "Rice Products",
                "Home Needs", "Chocolate", "SERVICES"
            };

            foreach (var category in categories)
            {
                var btn = CreateCategoryButton(category, yPos);
                _sidebarPanel.Controls.Add(btn);
                yPos += 45;
            }

            this.Controls.Add(_sidebarPanel);
        }

        private Button CreateCategoryButton(string text, int yPos)
        {
            var btn = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 11F, FontStyle.Regular),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(101, 84, 72),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(0, yPos),
                Size = new Size(220, 40),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.BorderColor = Color.FromArgb(120, 100, 88);

            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(120, 100, 88);
            btn.MouseLeave += (s, e) => btn.BackColor = Color.FromArgb(101, 84, 72);
            btn.Click += (s, e) => FilterByCategory(text);

            return btn;
        }
        #endregion

        #region Products Panel
        private void CreateProductsPanel()
        {
            _productsPanel = new Panel
            {
                Location = new Point(220, 60),
                Size = new Size(this.ClientSize.Width - 220 - 400, this.ClientSize.Height - 60 - 70),
                BackColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            _productCardsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                AutoScroll = true,
                Padding = new Padding(15)
            };

            _productsPanel.Controls.Add(_productCardsPanel);
            this.Controls.Add(_productsPanel);
        }
        #endregion

        #region Cart Panel
        private void CreateCartPanel()
        {
            _cartPanel = new Panel
            {
                Location = new Point(this.ClientSize.Width - 400, 60),
                Size = new Size(400, this.ClientSize.Height - 60 - 70),
                BackColor = Color.FromArgb(250, 250, 250),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right
            };

            // Cart items container
            _cartItemsPanel = new FlowLayoutPanel
            {
                Location = new Point(0, 0),
                Size = new Size(400, _cartPanel.Height - 200),
                BackColor = Color.White,
                AutoScroll = true,
                Padding = new Padding(10),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            _cartPanel.Controls.Add(_cartItemsPanel);

            // Totals panel
            var totalsPanel = new Panel
            {
                Location = new Point(0, _cartPanel.Height - 200),
                Size = new Size(400, 200),
                BackColor = Color.White,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            _subtotalLabel = new Label
            {
                Text = "Subtotal: dh 0.00",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(20, 20),
                Size = new Size(360, 25)
            };
            totalsPanel.Controls.Add(_subtotalLabel);

            _taxLabel = new Label
            {
                Text = "Tax (10%): dh 0.00",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(20, 50),
                Size = new Size(360, 25)
            };
            totalsPanel.Controls.Add(_taxLabel);

            _totalLabel = new Label
            {
                Text = "dh 0.00",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 140, 0),
                Location = new Point(20, 80),
                Size = new Size(360, 35),
                TextAlign = ContentAlignment.MiddleRight
            };
            totalsPanel.Controls.Add(_totalLabel);

            _itemCountLabel = new Label
            {
                Text = "0 Items",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.Gray,
                Location = new Point(20, 120),
                Size = new Size(100, 20)
            };
            totalsPanel.Controls.Add(_itemCountLabel);

            _payButton = new Button
            {
                Text = "Pay",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(255, 140, 0),
                Location = new Point(20, 150),
                Size = new Size(360, 45),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _payButton.FlatAppearance.BorderSize = 0;
            _payButton.Click += PayButton_Click;
            totalsPanel.Controls.Add(_payButton);

            _cartPanel.Controls.Add(totalsPanel);
            this.Controls.Add(_cartPanel);
        }
        #endregion

        #region Bottom Bar
        private void CreateBottomBar()
        {
            _bottomBarPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 70,
                BackColor = Color.FromArgb(101, 84, 72)
            };

            var menuBtn = new Button
            {
                Text = "≡",
                Font = new Font("Segoe UI", 20F),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(80, 65, 55),
                Location = new Point(20, 15),
                Size = new Size(180, 40),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            menuBtn.FlatAppearance.BorderSize = 0;
            _bottomBarPanel.Controls.Add(menuBtn);

            var ordersBtn = new Button
            {
                Text = "ORDERS",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(80, 65, 55),
                Location = new Point(220, 15),
                Size = new Size(200, 40),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            ordersBtn.FlatAppearance.BorderSize = 0;
            _bottomBarPanel.Controls.Add(ordersBtn);

            this.Controls.Add(_bottomBarPanel);
        }
        #endregion

        #region Data Loading
        private async void LoadData()
        {
            await LoadProducts();
            UpdateUserLabel();
        }

        private void UpdateUserLabel()
        {
            var user = _authService.CurrentUser;
            if (user != null)
            {
                _userLabel.Text = $"{user.FirstName} {user.LastName}";
            }
        }

        private async Task LoadProducts()
        {
            try
            {
                _productCardsPanel.Controls.Clear();

                var products = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.ProductImages)
                    .Where(p => p.IsActive && p.Quantity > 0)
                    .OrderBy(p => p.Name)
                    .ToListAsync();

                foreach (var product in products)
                {
                    CreateProductCard(product);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FilterByCategory(string category)
        {
            // Implement category filtering
            MessageBox.Show($"Filter by: {category}", "Category Filter");
        }
        #endregion

        #region Product Card
        private void CreateProductCard(Product product)
        {
            var card = new Panel
            {
                Size = new Size(140, 200),
                Margin = new Padding(8),
                BackColor = Color.White,
                Cursor = Cursors.Hand,
                Tag = product,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Product image
            var imageBox = new PictureBox
            {
                Location = new Point(10, 10),
                Size = new Size(120, 100),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White
            };

            try
            {
                var primaryImage = ProductImageHelper.GetPrimaryImage(product);
                if (primaryImage != null)
                {
                    imageBox.Image = ProductImageHelper.ResizeImage(primaryImage, 120, 100);
                }
            }
            catch { }

            card.Controls.Add(imageBox);

            // Product name
            var nameLabel = new Label
            {
                Text = product.Name.Length > 18 ? product.Name.Substring(0, 15) + "..." : product.Name,
                Font = new Font("Segoe UI", 8F),
                ForeColor = Color.FromArgb(60, 60, 60),
                Location = new Point(5, 115),
                Size = new Size(130, 30),
                TextAlign = ContentAlignment.TopCenter
            };
            card.Controls.Add(nameLabel);

            // Price label
            var priceLabel = new Label
            {
                Text = $"dh {product.SellingPrice:F2}",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 50, 50),
                Location = new Point(5, 150),
                Size = new Size(130, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };
            card.Controls.Add(priceLabel);

            // Stock label
            var stockLabel = new Label
            {
                Text = $"Stock: {product.Quantity}",
                Font = new Font("Segoe UI", 7F),
                ForeColor = Color.Gray,
                Location = new Point(5, 175),
                Size = new Size(130, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };
            card.Controls.Add(stockLabel);

            // Click to add
            card.Click += (s, e) => AddToCart(product);
            imageBox.Click += (s, e) => AddToCart(product);
            nameLabel.Click += (s, e) => AddToCart(product);
            priceLabel.Click += (s, e) => AddToCart(product);

            // Hover effect
            card.MouseEnter += (s, e) => card.BackColor = Color.FromArgb(245, 245, 245);
            card.MouseLeave += (s, e) => card.BackColor = Color.White;

            _productCardsPanel.Controls.Add(card);
        }
        #endregion

        #region Cart Operations
        private void AddToCart(Product product)
        {
            var existingItem = _cartItems.FirstOrDefault(i => i.ProductId == product.Id);
            
            if (existingItem != null)
            {
                if (existingItem.Quantity < product.Quantity)
                {
                    existingItem.Quantity++;
                }
                else
                {
                    MessageBox.Show("Insufficient stock!", "Stock Limit", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else
            {
                _cartItems.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    UnitPrice = product.SellingPrice,
                    Quantity = 1
                });
            }

            UpdateCartDisplay();
        }

        private void UpdateCartDisplay()
        {
            _cartItemsPanel.Controls.Clear();

            foreach (var item in _cartItems)
            {
                var itemPanel = new Panel
                {
                    Size = new Size(380, 80),
                    BackColor = Color.White,
                    Margin = new Padding(5),
                    BorderStyle = BorderStyle.FixedSingle
                };

                var nameLabel = new Label
                {
                    Text = item.ProductName,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    Location = new Point(10, 10),
                    Size = new Size(250, 25)
                };
                itemPanel.Controls.Add(nameLabel);

                var qtyLabel = new Label
                {
                    Text = $"{item.Quantity} x dh {item.UnitPrice:F2}",
                    Font = new Font("Segoe UI", 9F),
                    ForeColor = Color.Gray,
                    Location = new Point(10, 35),
                    Size = new Size(200, 20)
                };
                itemPanel.Controls.Add(qtyLabel);

                var totalLabel = new Label
                {
                    Text = $"dh {item.Total:F2}",
                    Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(220, 50, 50),
                    Location = new Point(270, 25),
                    Size = new Size(100, 30),
                    TextAlign = ContentAlignment.MiddleRight
                };
                itemPanel.Controls.Add(totalLabel);

                // Remove button
                var removeBtn = new Button
                {
                    Text = "×",
                    Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                    ForeColor = Color.Red,
                    BackColor = Color.White,
                    Location = new Point(340, 5),
                    Size = new Size(30, 30),
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    Tag = item
                };
                removeBtn.FlatAppearance.BorderSize = 0;
                removeBtn.Click += (s, e) =>
                {
                    _cartItems.Remove(item);
                    UpdateCartDisplay();
                };
                itemPanel.Controls.Add(removeBtn);

                _cartItemsPanel.Controls.Add(itemPanel);
            }

            UpdateTotals();
        }

        private void UpdateTotals()
        {
            decimal subtotal = _cartItems.Sum(i => i.Total);
            decimal tax = subtotal * _taxRate;
            decimal total = subtotal + tax;

            _subtotalLabel.Text = $"Subtotal: dh {subtotal:F2}";
            _taxLabel.Text = $"Tax (10%): dh {tax:F2}";
            _totalLabel.Text = $"dh {total:F2}";
            _itemCountLabel.Text = $"{_cartItems.Sum(i => i.Quantity)} Items";
        }

        private async void PayButton_Click(object sender, EventArgs e)
        {
            if (!_cartItems.Any())
            {
                MessageBox.Show("Cart is empty!", "Cannot Process", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                await ProcessSale();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing sale: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task ProcessSale()
        {
            var currentUser = _authService.CurrentUser;
            decimal subtotal = _cartItems.Sum(i => i.Total);
            decimal tax = subtotal * _taxRate;
            decimal total = subtotal + tax;

            var sale = new Sale
            {
                InvoiceNumber = $"INV-{DateTime.Now:yyyyMMddHHmmss}",
                CashierId = currentUser.Id,
                TaxAmount = tax,
                TotalAmount = total,
                PaymentMethod = PaymentMethod.Cash,
                Status = SaleStatus.Completed,
                SaleDate = DateTime.Now
            };

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();

            foreach (var item in _cartItems)
            {
                var saleItem = new SaleItem
                {
                    SaleId = sale.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.Total
                };
                _context.SaleItems.Add(saleItem);

                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.Quantity -= item.Quantity;
                    product.UpdatedAt = DateTime.Now;
                }
            }

            await _context.SaveChangesAsync();

            MessageBox.Show($"Sale completed!\nTotal: dh {total:F2}\nInvoice: {sale.InvoiceNumber}", 
                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            _cartItems.Clear();
            UpdateCartDisplay();
            await LoadProducts();
        }
        #endregion

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
