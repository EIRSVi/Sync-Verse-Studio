using System;
using System.Drawing;
using System.Windows.Forms;
using SyncVerseStudio.Data;
using SyncVerseStudio.Helpers;
using FontAwesome.Sharp;

namespace SyncVerseStudio.Views
{
    public partial class DatabaseSeedView : Form
    {
        private readonly ApplicationDbContext _context;
        private IconButton btnSeedSuppliers;
        private IconButton btnSeedCategories;
        private IconButton btnSeedCustomers;
        private IconButton btnSeedProducts;
        private IconButton btnSeedAll;
        private RichTextBox txtLog;
        private Panel panelHeader;
        private Label lblTitle;

        public DatabaseSeedView(ApplicationDbContext context)
        {
            _context = context;
            InitializeComponent();
            SetupUI();
        }

        private void InitializeComponent()
        {
            this.Text = "Database Seeder";
            this.Size = new Size(700, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 244, 248);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
        }

        private void SetupUI()
        {
            // Header Panel
            panelHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(41, 128, 185)
            };

            lblTitle = new Label
            {
                Text = "Database Seeder",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 25)
            };

            panelHeader.Controls.Add(lblTitle);
            this.Controls.Add(panelHeader);

            // Buttons Panel - First Row
            Panel panelButtons1 = new Panel
            {
                Location = new Point(20, 100),
                Size = new Size(660, 80),
                BackColor = Color.White
            };

            btnSeedSuppliers = CreateButton("Seed Suppliers", IconChar.TruckField, 20);
            btnSeedCategories = CreateButton("Seed Categories", IconChar.Tags, 175);
            btnSeedCustomers = CreateButton("Seed Customers", IconChar.UserFriends, 330);
            btnSeedProducts = CreateButton("Seed Products", IconChar.Box, 485);

            btnSeedSuppliers.Click += BtnSeedSuppliers_Click;
            btnSeedCategories.Click += BtnSeedCategories_Click;
            btnSeedCustomers.Click += BtnSeedCustomers_Click;
            btnSeedProducts.Click += BtnSeedProducts_Click;

            btnSeedSuppliers.Size = new Size(145, 50);
            btnSeedCategories.Size = new Size(145, 50);
            btnSeedCustomers.Size = new Size(145, 50);
            btnSeedProducts.Size = new Size(145, 50);

            panelButtons1.Controls.AddRange(new Control[] { btnSeedSuppliers, btnSeedCategories, btnSeedCustomers, btnSeedProducts });
            this.Controls.Add(panelButtons1);

            // Buttons Panel - Second Row
            Panel panelButtons2 = new Panel
            {
                Location = new Point(20, 190),
                Size = new Size(660, 80),
                BackColor = Color.White
            };

            btnSeedAll = CreateButton("Seed All Data", IconChar.Database, 235);
            btnSeedAll.Size = new Size(190, 50);
            btnSeedAll.Click += BtnSeedAll_Click;

            panelButtons2.Controls.Add(btnSeedAll);
            this.Controls.Add(panelButtons2);

            // Log TextBox
            txtLog = new RichTextBox
            {
                Location = new Point(20, 280),
                Size = new Size(660, 170),
                ReadOnly = true,
                BackColor = Color.White,
                Font = new Font("Consolas", 9)
            };

            this.Controls.Add(txtLog);
        }

        private IconButton CreateButton(string text, IconChar icon, int x)
        {
            return new IconButton
            {
                Text = text,
                IconChar = icon,
                IconColor = Color.White,
                IconSize = 24,
                Size = new Size(190, 50),
                Location = new Point(x, 15),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleRight,
                ImageAlign = ContentAlignment.MiddleLeft,
                Cursor = Cursors.Hand
            };
        }

        private async void BtnSeedSuppliers_Click(object sender, EventArgs e)
        {
            await SeedData("Suppliers", async () => await DatabaseSeeder.SeedSuppliersAsync(_context));
        }

        private async void BtnSeedCategories_Click(object sender, EventArgs e)
        {
            await SeedData("Categories", async () => await DatabaseSeeder.SeedCategoriesAsync(_context));
        }

        private async void BtnSeedCustomers_Click(object sender, EventArgs e)
        {
            await SeedData("Customers (Encrypted)", async () => await DatabaseSeeder.SeedCustomersAsync(_context));
        }

        private async void BtnSeedProducts_Click(object sender, EventArgs e)
        {
            await SeedData("Products with Images", async () => await DatabaseSeeder.SeedProductsAsync(_context));
        }

        private async void BtnSeedAll_Click(object sender, EventArgs e)
        {
            await SeedData("All Data", async () => await DatabaseSeeder.SeedAllAsync(_context));
        }

        private async Task SeedData(string dataType, Func<Task> seedAction)
        {
            try
            {
                DisableButtons();
                LogMessage($"Starting to seed {dataType}...", Color.Blue);

                await seedAction();

                LogMessage($"✓ Successfully seeded {dataType}!", Color.Green);
                MessageBox.Show($"{dataType} seeded successfully!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                LogMessage($"✗ Error seeding {dataType}: {ex.Message}", Color.Red);
                MessageBox.Show($"Error seeding {dataType}: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                EnableButtons();
            }
        }

        private void LogMessage(string message, Color color)
        {
            txtLog.SelectionStart = txtLog.TextLength;
            txtLog.SelectionLength = 0;
            txtLog.SelectionColor = color;
            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\n");
            txtLog.SelectionColor = txtLog.ForeColor;
            txtLog.ScrollToCaret();
        }

        private void DisableButtons()
        {
            btnSeedSuppliers.Enabled = false;
            btnSeedCategories.Enabled = false;
            btnSeedCustomers.Enabled = false;
            btnSeedProducts.Enabled = false;
            btnSeedAll.Enabled = false;
        }

        private void EnableButtons()
        {
            btnSeedSuppliers.Enabled = true;
            btnSeedCategories.Enabled = true;
            btnSeedCustomers.Enabled = true;
            btnSeedProducts.Enabled = true;
            btnSeedAll.Enabled = true;
        }
    }
}
