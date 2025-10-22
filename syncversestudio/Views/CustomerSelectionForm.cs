using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using SyncVerseStudio.Data;
using SyncVerseStudio.Models;
using FontAwesome.Sharp;

namespace SyncVerseStudio.Views
{
    public class CustomerSelectionForm : Form
    {
        private readonly ApplicationDbContext _context;
        public Customer? SelectedCustomer { get; private set; }

        private TextBox searchBox = null!;
        private ListBox customerListBox = null!;

        public CustomerSelectionForm(ApplicationDbContext context)
        {
            _context = context;
            InitializeComponent();
            LoadCustomers();
        }

        private void InitializeComponent()
        {
            this.Text = "Select Customer";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            // Header
            var headerPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(600, 70),
                BackColor = Color.FromArgb(168, 85, 247)
            };

            var titleLabel = new Label
            {
                Text = "👥 Select Customer",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 20),
                AutoSize = true
            };

            headerPanel.Controls.Add(titleLabel);

            // Search
            var searchLabel = new Label
            {
                Text = "Search:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 90),
                AutoSize = true
            };

            searchBox = new TextBox
            {
                Location = new Point(20, 115),
                Size = new Size(560, 30),
                Font = new Font("Segoe UI", 12),
                PlaceholderText = "Search by name, phone, or email..."
            };
            searchBox.TextChanged += (s, e) => LoadCustomers(searchBox.Text);

            // Customer list
            customerListBox = new ListBox
            {
                Location = new Point(20, 160),
                Size = new Size(560, 250),
                Font = new Font("Segoe UI", 11),
                DrawMode = DrawMode.OwnerDrawFixed,
                ItemHeight = 50
            };
            customerListBox.DrawItem += CustomerListBox_DrawItem;
            customerListBox.DoubleClick += (s, e) => SelectCustomer();

            // Buttons
            var selectButton = new Button
            {
                Text = "✓ Select",
                Location = new Point(320, 425),
                Size = new Size(130, 40),
                BackColor = Color.FromArgb(34, 197, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            selectButton.FlatAppearance.BorderSize = 0;
            selectButton.Click += (s, e) => SelectCustomer();

            var cancelButton = new Button
            {
                Text = "✕ Cancel",
                Location = new Point(460, 425),
                Size = new Size(120, 40),
                BackColor = Color.FromArgb(100, 116, 139),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            cancelButton.FlatAppearance.BorderSize = 0;
            cancelButton.Click += (s, e) => this.Close();

            var newCustomerButton = new Button
            {
                Text = "+ New Customer",
                Location = new Point(20, 425),
                Size = new Size(150, 40),
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            newCustomerButton.FlatAppearance.BorderSize = 0;
            newCustomerButton.Click += (s, e) => CreateNewCustomer();

            this.Controls.AddRange(new Control[] {
                headerPanel, searchLabel, searchBox, customerListBox,
                selectButton, cancelButton, newCustomerButton
            });
        }

        private void LoadCustomers(string? searchTerm = null)
        {
            customerListBox.Items.Clear();

            var query = _context.Customers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c =>
                    (c.FirstName != null && c.FirstName.Contains(searchTerm)) ||
                    (c.LastName != null && c.LastName.Contains(searchTerm)) ||
                    (c.Phone != null && c.Phone.Contains(searchTerm)) ||
                    (c.Email != null && c.Email.Contains(searchTerm)));
            }

            var customers = query.OrderBy(c => c.FirstName).Take(50).ToList();

            foreach (var customer in customers)
            {
                customerListBox.Items.Add(customer);
            }
        }

        private void CustomerListBox_DrawItem(object? sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            var customer = (Customer)customerListBox.Items[e.Index];
            e.DrawBackground();

            // Name
            using (var font = new Font("Segoe UI", 11, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.FromArgb(30, 41, 59)))
            {
                e.Graphics.DrawString(customer.FullName, font, brush, e.Bounds.Left + 10, e.Bounds.Top + 5);
            }

            // Contact info
            using (var font = new Font("Segoe UI", 9))
            using (var brush = new SolidBrush(Color.FromArgb(100, 116, 139)))
            {
                var info = $"📞 {customer.Phone ?? "N/A"} | 📧 {customer.Email ?? "N/A"}";
                e.Graphics.DrawString(info, font, brush, e.Bounds.Left + 10, e.Bounds.Top + 28);
            }

            e.DrawFocusRectangle();
        }

        private void SelectCustomer()
        {
            if (customerListBox.SelectedItem is Customer customer)
            {
                SelectedCustomer = customer;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please select a customer!", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CreateNewCustomer()
        {
            MessageBox.Show("Quick customer creation coming soon!\n\nFor now, please use the Customer Management page.", 
                "New Customer", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
