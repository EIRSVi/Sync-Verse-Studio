using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using SyncVerseStudio.Services;
using SyncVerseStudio.Data;
using SyncVerseStudio.Models;

namespace SyncVerseStudio.Views
{
    public partial class PointOfSaleView : Form
    {
        private readonly AuthenticationService _authService;
        private readonly ApplicationDbContext _context;
        private List<Product> _cart = new List<Product>();
        private decimal _total = 0;

        public PointOfSaleView(AuthenticationService authService)
        {
            _authService = authService;
            _context = new ApplicationDbContext(); // Assuming context is instantiated here; in real app, inject via DI
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            this.Name = "PointOfSaleView";
            this.Text = "Point of Sale";
            this.WindowState = FormWindowState.Normal;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.FromArgb(250, 250, 250);
            this.ClientSize = new Size(1200, 800);
            
            // Title Label
            var titleLabel = new Label();
            titleLabel.Text = "Point of Sale System";
            titleLabel.Font = new System.Drawing.Font("Segoe UI", 20F, FontStyle.Bold);
            titleLabel.ForeColor = System.Drawing.Color.FromArgb(33, 33, 33);
            titleLabel.SetBounds(20, 20, 400, 40);
            this.Controls.Add(titleLabel);

            // Product Search
            var searchLabel = new Label();
            searchLabel.Text = "Product Search:";
            searchLabel.Font = new System.Drawing.Font("Segoe UI", 12F);
            searchLabel.SetBounds(20, 80, 150, 30);
            this.Controls.Add(searchLabel);

            var searchTextBox = new TextBox();
            searchTextBox.Name = "searchTextBox";
            searchTextBox.SetBounds(180, 80, 300, 30);
            searchTextBox.TextChanged += SearchTextBox_TextChanged;
            this.Controls.Add(searchTextBox);

            // Products DataGridView
            var productsGrid = new DataGridView();
            productsGrid.Name = "productsGrid";
            productsGrid.SetBounds(20, 120, 600, 300);
            productsGrid.AllowUserToAddRows = false;
            productsGrid.ReadOnly = true;
            productsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            productsGrid.Columns.Add("Id", "ID");
            productsGrid.Columns.Add("Name", "Name");
            productsGrid.Columns.Add("Price", "Price");
            productsGrid.Columns.Add("Stock", "Stock");
            productsGrid.CellDoubleClick += ProductsGrid_CellDoubleClick;
            this.Controls.Add(productsGrid);

            // Add to Cart Button
            var addToCartButton = new Button();
            addToCartButton.Text = "Add to Cart";
            addToCartButton.SetBounds(650, 120, 150, 40);
            addToCartButton.Click += AddToCartButton_Click;
            this.Controls.Add(addToCartButton);

            // Cart DataGridView
            var cartGrid = new DataGridView();
            cartGrid.Name = "cartGrid";
            cartGrid.SetBounds(20, 450, 600, 200);
            cartGrid.AllowUserToAddRows = false;
            cartGrid.ReadOnly = true;
            cartGrid.Columns.Add("Id", "ID");
            cartGrid.Columns.Add("Name", "Name");
            cartGrid.Columns.Add("Price", "Price");
            this.Controls.Add(cartGrid);

            // Total Label
            var totalLabel = new Label();
            totalLabel.Name = "totalLabel";
            totalLabel.Text = "Total: $0.00";
            totalLabel.Font = new System.Drawing.Font("Segoe UI", 14F, FontStyle.Bold);
            totalLabel.SetBounds(650, 450, 200, 30);
            this.Controls.Add(totalLabel);

            // Checkout Button
            var checkoutButton = new Button();
            checkoutButton.Text = "Checkout";
            checkoutButton.SetBounds(650, 500, 150, 40);
            checkoutButton.Click += CheckoutButton_Click;
            this.Controls.Add(checkoutButton);

            // CRUD Buttons for Products
            var addProductButton = new Button();
            addProductButton.Text = "Add Product";
            addProductButton.SetBounds(20, 680, 120, 40);
            addProductButton.Click += AddProductButton_Click;
            this.Controls.Add(addProductButton);

            var editProductButton = new Button();
            editProductButton.Text = "Edit Product";
            editProductButton.SetBounds(160, 680, 120, 40);
            editProductButton.Click += EditProductButton_Click;
            this.Controls.Add(editProductButton);

            var deleteProductButton = new Button();
            deleteProductButton.Text = "Delete Product";
            deleteProductButton.SetBounds(300, 680, 120, 40);
            deleteProductButton.Click += DeleteProductButton_Click;
            this.Controls.Add(deleteProductButton);

            // Load initial data
            LoadProducts();

            this.ResumeLayout(false);
        }

        private void LoadProducts()
        {
            var products = _context.Products.ToList();
            var productsGrid = (DataGridView)this.Controls["productsGrid"];
            productsGrid.Rows.Clear();
            foreach (var product in products)
            {
                productsGrid.Rows.Add(product.Id, product.Name, product.SellingPrice, product.Quantity);
            }
        }

        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            var searchText = ((TextBox)sender).Text.ToLower();
            var products = _context.Products.Where(p => p.Name.ToLower().Contains(searchText)).ToList();
            var productsGrid = (DataGridView)this.Controls["productsGrid"];
            productsGrid.Rows.Clear();
            foreach (var product in products)
            {
                productsGrid.Rows.Add(product.Id, product.Name, product.SellingPrice, product.Quantity);
            }
        }

        private void ProductsGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var productsGrid = (DataGridView)sender;
                var productId = (int)productsGrid.Rows[e.RowIndex].Cells[0].Value;
                var product = _context.Products.Find(productId);
                if (product != null)
                {
                    _cart.Add(product);
                    UpdateCart();
                }
            }
        }

        private void AddToCartButton_Click(object sender, EventArgs e)
        {
            var productsGrid = (DataGridView)this.Controls["productsGrid"];
            if (productsGrid.SelectedRows.Count > 0)
            {
                var productId = (int)productsGrid.SelectedRows[0].Cells[0].Value;
                var product = _context.Products.Find(productId);
                if (product != null)
                {
                    _cart.Add(product);
                    UpdateCart();
                }
            }
        }

        private void UpdateCart()
        {
            var cartGrid = (DataGridView)this.Controls["cartGrid"];
            cartGrid.Rows.Clear();
            _total = 0;
            foreach (var product in _cart)
            {
                cartGrid.Rows.Add(product.Id, product.Name, product.SellingPrice);
                _total += product.SellingPrice;
            }
            var totalLabel = (Label)this.Controls["totalLabel"];
            totalLabel.Text = $"Total: ${_total:F2}";
        }

        private void CheckoutButton_Click(object sender, EventArgs e)
        {
            if (_cart.Count == 0)
            {
                MessageBox.Show("Cart is empty!");
                return;
            }

            var sale = new Sale
            {
                CashierId = _authService.CurrentUser.Id,
                SaleDate = DateTime.Now,
                TotalAmount = _total,
                SaleItems = _cart.Select(p => new SaleItem { ProductId = p.Id, Quantity = 1, UnitPrice = p.SellingPrice }).ToList()
            };

            _context.Sales.Add(sale);
            _context.SaveChanges();

            // Update inventory
            foreach (var item in sale.SaleItems)
            {
                var product = _context.Products.Find(item.ProductId);
                if (product != null)
                {
                    product.Quantity -= item.Quantity;
                    _context.InventoryMovements.Add(new InventoryMovement
                    {
                        ProductId = product.Id,
                        MovementType = MovementType.Sale,
                        Quantity = -item.Quantity,
                        CreatedAt = DateTime.Now,
                        UserId = _authService.CurrentUser.Id
                    });
                }
            }
            _context.SaveChanges();

            MessageBox.Show("Checkout successful!");
            _cart.Clear();
            UpdateCart();
            LoadProducts();
        }

        private void AddProductButton_Click(object sender, EventArgs e)
        {
            var addForm = new ProductForm(_context);
            if (addForm.ShowDialog() == DialogResult.OK)
            {
                LoadProducts();
            }
        }

        private void EditProductButton_Click(object sender, EventArgs e)
        {
            var productsGrid = (DataGridView)this.Controls["productsGrid"];
            if (productsGrid.SelectedRows.Count > 0)
            {
                var productId = (int)productsGrid.SelectedRows[0].Cells[0].Value;
                var product = _context.Products.Find(productId);
                if (product != null)
                {
                    var editForm = new ProductForm(_context, product);
                    if (editForm.ShowDialog() == DialogResult.OK)
                    {
                        LoadProducts();
                    }
                }
            }
        }

        private void DeleteProductButton_Click(object sender, EventArgs e)
        {
            var productsGrid = (DataGridView)this.Controls["productsGrid"];
            if (productsGrid.SelectedRows.Count > 0)
            {
                var productId = (int)productsGrid.SelectedRows[0].Cells[0].Value;
                var product = _context.Products.Find(productId);
                if (product != null)
                {
                    _context.Products.Remove(product);
                    _context.SaveChanges();
                    LoadProducts();
                }
            }
        }
    }

    // Simple Product Form for CRUD
    public class ProductForm : Form
    {
        private readonly ApplicationDbContext _context;
        private readonly Product? _product;
        private TextBox nameTextBox, priceTextBox, stockTextBox;
        private Button saveButton, cancelButton;

        public ProductForm(ApplicationDbContext context, Product? product = null)
        {
            _context = context;
            _product = product;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = _product == null ? "Add Product" : "Edit Product";
            this.ClientSize = new Size(300, 200);

            var nameLabel = new Label { Text = "Name:", Location = new Point(10, 10) };
            nameTextBox = new TextBox { Location = new Point(100, 10), Width = 150 };
            if (_product != null) nameTextBox.Text = _product.Name;

            var priceLabel = new Label { Text = "Price:", Location = new Point(10, 50) };
            priceTextBox = new TextBox { Location = new Point(100, 50), Width = 150 };
            if (_product != null) priceTextBox.Text = _product.SellingPrice.ToString();

            var stockLabel = new Label { Text = "Stock:", Location = new Point(10, 90) };
            stockTextBox = new TextBox { Location = new Point(100, 90), Width = 150 };
            if (_product != null) stockTextBox.Text = _product.Quantity.ToString();

            saveButton = new Button { Text = "Save", Location = new Point(50, 130) };
            saveButton.Click += SaveButton_Click;

            cancelButton = new Button { Text = "Cancel", Location = new Point(150, 130) };
            cancelButton.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.AddRange(new Control[] { nameLabel, nameTextBox, priceLabel, priceTextBox, stockLabel, stockTextBox, saveButton, cancelButton });
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (decimal.TryParse(priceTextBox.Text, out var price) && int.TryParse(stockTextBox.Text, out var stock))
            {
                if (_product == null)
                {
                    var newProduct = new Product { Name = nameTextBox.Text, SellingPrice = price, Quantity = stock };
                    _context.Products.Add(newProduct);
                }
                else
                {
                    _product.Name = nameTextBox.Text;
                    _product.SellingPrice = price;
                    _product.Quantity = stock;
                }
                _context.SaveChanges();
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Invalid input!");
            }
        }
    }
}