using System.Drawing;
using SyncVerseStudio.Services;
using SyncVerseStudio.Models;
using SyncVerseStudio.Data;
using Microsoft.EntityFrameworkCore;

namespace SyncVerseStudio.Views
{
    public partial class PointOfSaleView : Form
    {
        private readonly AuthenticationService _authService;
        private ApplicationDbContext? _context;
        private Panel productSearchPanel;
        private Panel cartPanel;
        private Panel paymentPanel;
        private TextBox searchTextBox;
        private FlowLayoutPanel productsContainer;
        private DataGridView cartGridView;
        private Label totalLabel;
        private List<CartItem> cartItems = new List<CartItem>();
        private decimal totalAmount = 0;
        private TextBox orderTitleTextBox;

        public PointOfSaleView(AuthenticationService authService)
        {
            _authService = authService;
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.AutoScaleDimensions = new SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(248, 250, 252);
            this.ClientSize = new Size(1200, 800);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Name = "PointOfSaleView";
            this.Text = "Point of Sale";

            CreateLayout();
        }

        private void CreateLayout()
        {
            // Header
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.White,
                Padding = new Padding(20, 15, 20, 15)
            };

            var titleLabel = new Label
            {
                Text = "Point of Sale",
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(20, 20),
                Size = new Size(300, 40),
                TextAlign = ContentAlignment.MiddleLeft
            };
            headerPanel.Controls.Add(titleLabel);

            // Order Title input
            var orderTitleLabel = new Label
            {
                Text = "Order Title:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(350, 25),
                Size = new Size(80, 20)
            };
            headerPanel.Controls.Add(orderTitleLabel);

            orderTitleTextBox = new TextBox
            {
                Font = new Font("Segoe UI", 10F),
                Location = new Point(430, 20),
                Size = new Size(250, 30),
                PlaceholderText = "Enter order title (optional)"
            };
            headerPanel.Controls.Add(orderTitleTextBox);

            // Product Search Panel (Left Side)
            productSearchPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 600,
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            CreateProductSearchArea();

            // Cart Panel (Right Side)
            cartPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 250, 252),
                Padding = new Padding(20)
            };

            CreateCartArea();

            this.Controls.Add(cartPanel);
            this.Controls.Add(productSearchPanel);
            this.Controls.Add(headerPanel);
        }

        private void CreateProductSearchArea()
        {
            var searchLabel = new Label
            {
                Text = "Search Products",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(0, 0),
                Size = new Size(200, 30)
            };
            productSearchPanel.Controls.Add(searchLabel);

            // Search Box
            searchTextBox = new TextBox
            {
                Font = new Font("Segoe UI", 12F),
                Location = new Point(0, 40),
                Size = new Size(560, 30),
                PlaceholderText = "Search by product name, barcode..."
            };
            searchTextBox.TextChanged += SearchTextBox_TextChanged;
            productSearchPanel.Controls.Add(searchTextBox);

            // Product Categories
            var categoriesPanel = new FlowLayoutPanel
            {
                Location = new Point(0, 80),
                Size = new Size(560, 50),
                BackColor = Color.Transparent,
                WrapContents = true
            };

            var allCategoryBtn = CreateCategoryButton("All", Color.FromArgb(59, 130, 246));
            categoriesPanel.Controls.Add(allCategoryBtn);

            productSearchPanel.Controls.Add(categoriesPanel);

            // Products Container
            productsContainer = new FlowLayoutPanel
            {
                Location = new Point(0, 140),
                Size = new Size(560, 600),
                BackColor = Color.Transparent,
                AutoScroll = true,
                WrapContents = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            productSearchPanel.Controls.Add(productsContainer);
        }

        private Button CreateCategoryButton(string text, Color bgColor)
        {
            var button = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = bgColor,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(80, 35),
                Margin = new Padding(5),
                Cursor = Cursors.Hand
            };
            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        private void CreateCartArea()
        {
            var cartLabel = new Label
            {
                Text = "Shopping Cart",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(0, 0),
                Size = new Size(200, 30)
            };
            cartPanel.Controls.Add(cartLabel);

            // Cart Grid
            cartGridView = new DataGridView
            {
                Location = new Point(0, 40),
                Size = new Size(540, 400),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 11F)
            };

            cartGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ProductName",
                HeaderText = "Product",
                Width = 200,
                DataPropertyName = "ProductName"
            });

            cartGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Quantity",
                HeaderText = "Qty",
                Width = 60,
                DataPropertyName = "Quantity"
            });

            cartGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Price",
                HeaderText = "Price",
                Width = 80,
                DataPropertyName = "Price",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
            });

            cartGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Total",
                HeaderText = "Total",
                Width = 100,
                DataPropertyName = "Total",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
            });

            cartGridView.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "Remove",
                HeaderText = "Remove",
                Text = "Remove",
                UseColumnTextForButtonValue = true,
                Width = 80
            });

            cartGridView.CellClick += CartGridView_CellClick;
            cartPanel.Controls.Add(cartGridView);

            // Total Panel
            var totalPanel = new Panel
            {
                Location = new Point(0, 460),
                Size = new Size(540, 100),
                BackColor = Color.FromArgb(15, 23, 42),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            totalLabel = new Label
            {
                Text = "Total: $0.00",
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 30),
                Size = new Size(300, 40),
                TextAlign = ContentAlignment.MiddleLeft
            };
            totalPanel.Controls.Add(totalLabel);

            cartPanel.Controls.Add(totalPanel);

            // Payment Buttons
            CreatePaymentButtons();
        }

        private void CreatePaymentButtons()
        {
            var paymentButtonsPanel = new FlowLayoutPanel
            {
                Location = new Point(0, 580),
                Size = new Size(540, 80),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.Transparent,
                WrapContents = false
            };

            var cashButton = new Button
            {
                Text = "Cash Payment",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(34, 197, 94),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(130, 50),
                Margin = new Padding(5),
                Cursor = Cursors.Hand
            };
            cashButton.FlatAppearance.BorderSize = 0;
            cashButton.Click += CashButton_Click;

            var cardButton = new Button
            {
                Text = "Card Payment",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(59, 130, 246),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(130, 50),
                Margin = new Padding(5),
                Cursor = Cursors.Hand
            };
            cardButton.FlatAppearance.BorderSize = 0;
            cardButton.Click += CardButton_Click;

            var clearButton = new Button
            {
                Text = "Clear Cart",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(239, 68, 68),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 50),
                Margin = new Padding(5),
                Cursor = Cursors.Hand
            };
            clearButton.FlatAppearance.BorderSize = 0;
            clearButton.Click += ClearButton_Click;

            var holdButton = new Button
            {
                Text = "Hold Sale",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(245, 158, 11),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 50),
                Margin = new Padding(5),
                Cursor = Cursors.Hand
            };
            holdButton.FlatAppearance.BorderSize = 0;
            holdButton.Click += HoldButton_Click;

            paymentButtonsPanel.Controls.Add(cashButton);
            paymentButtonsPanel.Controls.Add(cardButton);
            paymentButtonsPanel.Controls.Add(clearButton);
            paymentButtonsPanel.Controls.Add(holdButton);

            cartPanel.Controls.Add(paymentButtonsPanel);
        }

        private async void LoadData()
        {
            try
            {
                _context = new ApplicationDbContext();
                await LoadProducts();
                await LoadCategories();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadProducts(string searchTerm = "")
        {
            if (_context == null) return;

            try
            {
                var products = await _context.Products
                    .Where(p => p.IsActive && p.Quantity > 0)
                    .Where(p => string.IsNullOrEmpty(searchTerm) || 
                               p.Name.Contains(searchTerm) || 
                               p.Barcode.Contains(searchTerm))
                    .Take(20)
                    .ToListAsync();

                productsContainer.Controls.Clear();

                foreach (var product in products)
                {
                    CreateProductCard(product);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadCategories()
        {
            if (_context == null) return;

            try
            {
                var categories = await _context.Categories
                    .Where(c => c.IsActive)
                    .ToListAsync();

                // Update categories panel with actual categories
                // This would be implemented based on your specific category structure
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateProductCard(Product product)
        {
            var card = new Panel
            {
                Size = new Size(120, 140),
                BackColor = Color.White,
                Margin = new Padding(10),
                Cursor = Cursors.Hand,
                Tag = product
            };

            card.Paint += (s, e) =>
            {
                var rect = card.ClientRectangle;
                using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1))
                {
                    e.Graphics.DrawRectangle(pen, rect);
                }
            };

            var nameLabel = new Label
            {
                Text = product.Name.Length > 15 ? product.Name.Substring(0, 15) + "..." : product.Name,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(5, 5),
                Size = new Size(110, 40),
                TextAlign = ContentAlignment.TopCenter
            };

            var priceLabel = new Label
            {
                Text = $"${product.SellingPrice:F2}",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(34, 197, 94),
                Location = new Point(5, 50),
                Size = new Size(110, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var stockLabel = new Label
            {
                Text = $"Stock: {product.Quantity}",
                Font = new Font("Segoe UI", 8F),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(5, 75),
                Size = new Size(110, 15),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var addButton = new Button
            {
                Text = "Add to Cart",
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(59, 130, 246),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(10, 100),
                Size = new Size(100, 30),
                Cursor = Cursors.Hand
            };
            addButton.FlatAppearance.BorderSize = 0;
            addButton.Click += (s, e) => AddToCart(product);

            card.Controls.Add(nameLabel);
            card.Controls.Add(priceLabel);
            card.Controls.Add(stockLabel);
            card.Controls.Add(addButton);

            productsContainer.Controls.Add(card);
        }

        private void AddToCart(Product product)
        {
            var existingItem = cartItems.FirstOrDefault(x => x.ProductId == product.Id);
            
            if (existingItem != null)
            {
                existingItem.Quantity++;
                existingItem.Total = existingItem.Quantity * existingItem.Price;
            }
            else
            {
                cartItems.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.SellingPrice,
                    Quantity = 1,
                    Total = product.SellingPrice
                });
            }

            UpdateCartDisplay();
        }

        private void UpdateCartDisplay()
        {
            cartGridView.DataSource = null;
            cartGridView.DataSource = cartItems;
            
            totalAmount = cartItems.Sum(x => x.Total);
            totalLabel.Text = $"Total: ${totalAmount:F2}";
        }

        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            _ = LoadProducts(searchTextBox.Text);
        }

        private void CartGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == cartGridView.Columns["Remove"].Index)
            {
                var item = cartItems[e.RowIndex];
                cartItems.Remove(item);
                UpdateCartDisplay();
            }
        }

        private void CashButton_Click(object sender, EventArgs e)
        {
            if (cartItems.Count == 0)
            {
                MessageBox.Show("Cart is empty!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ProcessPayment("Cash");
        }

        private void CardButton_Click(object sender, EventArgs e)
        {
            if (cartItems.Count == 0)
            {
                MessageBox.Show("Cart is empty!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ProcessPayment("Card");
        }

        private void ProcessPayment(string paymentMethod)
        {
            try
            {
                var orderTitle = orderTitleTextBox?.Text?.Trim() ?? "";
                var displayTitle = string.IsNullOrEmpty(orderTitle) ? "(no title)" : orderTitle;

                // Here you would implement the actual payment processing
                MessageBox.Show($"Payment of ${totalAmount:F2} processed successfully via {paymentMethod}!\nOrder Title: {displayTitle}", 
                    "Payment Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                ClearCart();
                if (orderTitleTextBox != null) orderTitleTextBox.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Payment failed: {ex.Message}", "Payment Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            ClearCart();
        }

        private void HoldButton_Click(object sender, EventArgs e)
        {
            if (cartItems.Count == 0)
            {
                MessageBox.Show("Cart is empty!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var orderTitle = orderTitleTextBox?.Text?.Trim() ?? "";
            var displayTitle = string.IsNullOrEmpty(orderTitle) ? "(no title)" : orderTitle;

            MessageBox.Show($"Sale held successfully!\nOrder Title: {displayTitle}", "Hold Sale", MessageBoxButtons.OK, MessageBoxIcon.Information);
            // Note: In a full implementation, this would save the cart state to a held sales table
            ClearCart();
        }

        private void ClearCart()
        {
            cartItems.Clear();
            UpdateCartDisplay();
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

    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
    }
}