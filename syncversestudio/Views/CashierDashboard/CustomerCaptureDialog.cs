using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using SyncVerseStudio.Data;
using SyncVerseStudio.Models;
using FontAwesome.Sharp;

namespace SyncVerseStudio.Views.CashierDashboard
{
    public class CustomerCaptureDialog : Form
    {
        private readonly ApplicationDbContext _context;
        private ComboBox cmbCustomer;
        private TextBox txtFirstName;
        private TextBox txtLastName;
        private TextBox txtPhone;
        private TextBox txtEmail;
        private RadioButton rbExisting;
        private RadioButton rbNew;
        private RadioButton rbWalkIn;
        private Panel newCustomerPanel;

        public int? SelectedCustomerId { get; private set; }
        public string CustomerName { get; private set; }

        public CustomerCaptureDialog()
        {
            _context = new ApplicationDbContext();
            InitializeComponent();
            LoadCustomers();
        }

        private void InitializeComponent()
        {
            this.Text = "Customer Information";
            this.Size = new Size(500, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            var titleLabel = new Label
            {
                Text = "Link Customer to Purchase",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(30, 20),
                Size = new Size(440, 30),
                BackColor = Color.Transparent
            };

            var subtitleLabel = new Label
            {
                Text = "Track purchases and build customer relationships",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(30, 55),
                Size = new Size(440, 20),
                BackColor = Color.Transparent
            };

            // Customer Type Selection
            int yPos = 100;

            rbWalkIn = new RadioButton
            {
                Text = "🚶 Walk-in Customer (No tracking)",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(30, yPos),
                Size = new Size(440, 30),
                Checked = true
            };
            rbWalkIn.CheckedChanged += CustomerType_Changed;

            yPos += 40;

            rbExisting = new RadioButton
            {
                Text = "👤 Existing Customer",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(30, yPos),
                Size = new Size(440, 30)
            };
            rbExisting.CheckedChanged += CustomerType_Changed;

            yPos += 40;

            cmbCustomer = new ComboBox
            {
                Location = new Point(30, yPos),
                Size = new Size(440, 30),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Enabled = false
            };

            yPos += 50;

            rbNew = new RadioButton
            {
                Text = "➕ New Customer",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(30, yPos),
                Size = new Size(440, 30)
            };
            rbNew.CheckedChanged += CustomerType_Changed;

            yPos += 40;

            // New Customer Panel
            newCustomerPanel = new Panel
            {
                Location = new Point(30, yPos),
                Size = new Size(440, 180),
                BackColor = Color.FromArgb(248, 250, 252),
                Visible = false
            };

            newCustomerPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, newCustomerPanel.Width - 1, newCustomerPanel.Height - 1), 8))
                using (var brush = new SolidBrush(Color.FromArgb(248, 250, 252)))
                {
                    e.Graphics.FillPath(brush, path);
                }
            };

            int formY = 15;

            var lblFirstName = new Label
            {
                Text = "First Name:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(15, formY),
                AutoSize = true
            };

            txtFirstName = new TextBox
            {
                Location = new Point(120, formY - 3),
                Size = new Size(300, 25),
                Font = new Font("Segoe UI", 10)
            };

            formY += 35;

            var lblLastName = new Label
            {
                Text = "Last Name:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(15, formY),
                AutoSize = true
            };

            txtLastName = new TextBox
            {
                Location = new Point(120, formY - 3),
                Size = new Size(300, 25),
                Font = new Font("Segoe UI", 10)
            };

            formY += 35;

            var lblPhone = new Label
            {
                Text = "Phone:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(15, formY),
                AutoSize = true
            };

            txtPhone = new TextBox
            {
                Location = new Point(120, formY - 3),
                Size = new Size(300, 25),
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "Optional"
            };

            formY += 35;

            var lblEmail = new Label
            {
                Text = "Email:",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(15, formY),
                AutoSize = true
            };

            txtEmail = new TextBox
            {
                Location = new Point(120, formY - 3),
                Size = new Size(300, 25),
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "Optional"
            };

            newCustomerPanel.Controls.AddRange(new Control[] { 
                lblFirstName, txtFirstName, 
                lblLastName, txtLastName, 
                lblPhone, txtPhone, 
                lblEmail, txtEmail 
            });

            // Buttons
            var btnOK = new Button
            {
                Text = "✓ Continue",
                Location = new Point(270, 470),
                Size = new Size(100, 40),
                BackColor = Color.FromArgb(34, 197, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand,
                DialogResult = DialogResult.OK
            };
            btnOK.FlatAppearance.BorderSize = 0;
            btnOK.Click += BtnOK_Click;

            var btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(380, 470),
                Size = new Size(90, 40),
                BackColor = Color.FromArgb(100, 116, 139),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10),
                Cursor = Cursors.Hand,
                DialogResult = DialogResult.Cancel
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            this.Controls.AddRange(new Control[] { 
                titleLabel, subtitleLabel, 
                rbWalkIn, rbExisting, cmbCustomer, 
                rbNew, newCustomerPanel, 
                btnOK, btnCancel 
            });

            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }

        private async void LoadCustomers()
        {
            try
            {
                var customers = await _context.Customers
                    .OrderBy(c => c.FirstName)
                    .ThenBy(c => c.LastName)
                    .ToListAsync();

                cmbCustomer.DisplayMember = "FullName";
                cmbCustomer.ValueMember = "Id";
                cmbCustomer.DataSource = customers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customers: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CustomerType_Changed(object sender, EventArgs e)
        {
            cmbCustomer.Enabled = rbExisting.Checked;
            newCustomerPanel.Visible = rbNew.Checked;
        }

        private async void BtnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (rbWalkIn.Checked)
                {
                    SelectedCustomerId = null;
                    CustomerName = "Walk-in Customer";
                }
                else if (rbExisting.Checked)
                {
                    if (cmbCustomer.SelectedItem is Customer customer)
                    {
                        SelectedCustomerId = customer.Id;
                        CustomerName = customer.FullName;
                    }
                }
                else if (rbNew.Checked)
                {
                    if (string.IsNullOrWhiteSpace(txtFirstName.Text) || string.IsNullOrWhiteSpace(txtLastName.Text))
                    {
                        MessageBox.Show("Please enter customer name.", "Validation Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        this.DialogResult = DialogResult.None;
                        return;
                    }

                    var newCustomer = new Customer
                    {
                        FirstName = txtFirstName.Text.Trim(),
                        LastName = txtLastName.Text.Trim(),
                        Phone = string.IsNullOrWhiteSpace(txtPhone.Text) ? null : 
                            SyncVerseStudio.Helpers.EncryptionHelper.Encrypt(txtPhone.Text.Trim()),
                        Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : 
                            SyncVerseStudio.Helpers.EncryptionHelper.Encrypt(txtEmail.Text.Trim()),
                        CreatedAt = DateTime.Now
                    };

                    _context.Customers.Add(newCustomer);
                    await _context.SaveChangesAsync();

                    SelectedCustomerId = newCustomer.Id;
                    CustomerName = newCustomer.FullName;

                    MessageBox.Show("New customer created successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
            }
        }

        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
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
