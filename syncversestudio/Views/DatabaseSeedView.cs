using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SyncVerseStudio.Data;
using SyncVerseStudio.Helpers;
using FontAwesome.Sharp;
using Microsoft.EntityFrameworkCore;

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
        private IconButton btnRefreshAll;
        private IconButton btnClearAll;
        private IconButton btnResetDatabase;
        private IconButton btnBackupDatabase;
        private RichTextBox txtLog;
        private Panel panelHeader;
        private Label lblTitle;

        public DatabaseSeedView(ApplicationDbContext context)
        {
            _context = context;
            InitializeComponent();
            SetupUI();
            
            // Log initial information
            LogMessage("Database Seeder initialized", Color.Blue);
            LogMessage($"Application directory: {AppDomain.CurrentDomain.BaseDirectory}", Color.Blue);
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
            // Set form size
            this.Size = new Size(700, 650);

            // Header Panel
            panelHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(41, 128, 185)
            };

            lblTitle = new Label
            {
                Text = "Database Management & Seeder",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 25)
            };

            panelHeader.Controls.Add(lblTitle);
            this.Controls.Add(panelHeader);

            // Buttons Panel - First Row (Seed Individual)
            Panel panelButtons1 = new Panel
            {
                Location = new Point(20, 100),
                Size = new Size(660, 80),
                BackColor = Color.White
            };

            btnSeedSuppliers = CreateButton("Seed Suppliers", IconChar.TruckField, 20, Color.FromArgb(52, 152, 219));
            btnSeedCategories = CreateButton("Seed Categories", IconChar.Tags, 175, Color.FromArgb(52, 152, 219));
            btnSeedCustomers = CreateButton("Seed Customers", IconChar.UserFriends, 330, Color.FromArgb(52, 152, 219));
            btnSeedProducts = CreateButton("Seed Products", IconChar.Box, 485, Color.FromArgb(52, 152, 219));

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

            // Buttons Panel - Second Row (Seed All)
            Panel panelButtons2 = new Panel
            {
                Location = new Point(20, 190),
                Size = new Size(660, 80),
                BackColor = Color.White
            };

            btnSeedAll = CreateButton("Seed All Data", IconChar.Database, 235, Color.FromArgb(46, 204, 113));
            btnSeedAll.Size = new Size(190, 50);
            btnSeedAll.Click += BtnSeedAll_Click;

            panelButtons2.Controls.Add(btnSeedAll);
            this.Controls.Add(panelButtons2);

            // Buttons Panel - Third Row (Refresh & Management)
            Panel panelButtons3 = new Panel
            {
                Location = new Point(20, 280),
                Size = new Size(660, 80),
                BackColor = Color.White
            };

            btnRefreshAll = CreateButton("Refresh All Data", IconChar.Sync, 10, Color.FromArgb(230, 126, 34));
            btnClearAll = CreateButton("Clear All Data", IconChar.TrashAlt, 175, Color.FromArgb(231, 76, 60));
            btnResetDatabase = CreateButton("Reset Database", IconChar.ExclamationTriangle, 340, Color.FromArgb(192, 57, 43));
            btnBackupDatabase = CreateButton("Backup Database", IconChar.Download, 505, Color.FromArgb(52, 152, 219));

            btnRefreshAll.Size = new Size(155, 50);
            btnClearAll.Size = new Size(155, 50);
            btnResetDatabase.Size = new Size(155, 50);
            btnBackupDatabase.Size = new Size(155, 50);

            btnRefreshAll.Click += BtnRefreshAll_Click;
            btnClearAll.Click += BtnClearAll_Click;
            btnResetDatabase.Click += BtnResetDatabase_Click;
            btnBackupDatabase.Click += BtnBackupDatabase_Click;

            panelButtons3.Controls.AddRange(new Control[] { btnRefreshAll, btnClearAll, btnResetDatabase, btnBackupDatabase });
            this.Controls.Add(panelButtons3);

            // Log TextBox
            txtLog = new RichTextBox
            {
                Location = new Point(20, 370),
                Size = new Size(660, 250),
                ReadOnly = true,
                BackColor = Color.White,
                Font = new Font("Consolas", 9)
            };

            this.Controls.Add(txtLog);
        }

        private IconButton CreateButton(string text, IconChar icon, int x, Color backgroundColor)
        {
            return new IconButton
            {
                Text = text,
                IconChar = icon,
                IconColor = Color.White,
                IconSize = 24,
                Size = new Size(190, 50),
                Location = new Point(x, 15),
                BackColor = backgroundColor,
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

        private async void BtnRefreshAll_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "This will refresh all data by clearing existing data and re-seeding with fresh data.\n\n" +
                "⚠️ WARNING: This will delete all existing:\n" +
                "• Products and their images\n" +
                "• Categories\n" +
                "• Suppliers\n" +
                "• Customers (encrypted data)\n" +
                "• Sales data will be preserved\n\n" +
                "A backup will be created automatically before proceeding.\n\n" +
                "Are you sure you want to continue?",
                "Refresh All Data - Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                await SeedDataWithBackup("Refresh All Data", async () => await DatabaseSeeder.RefreshAllDataAsync(_context));
            }
        }

        private async void BtnClearAll_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "This will clear ALL data from the database including:\n\n" +
                "⚠️ DANGER: This will permanently delete:\n" +
                "• All Products and their images\n" +
                "• All Categories\n" +
                "• All Suppliers\n" +
                "• All Customers\n" +
                "• All Sales data\n" +
                "• All Users (except current admin)\n\n" +
                "A backup will be created automatically before proceeding.\n\n" +
                "Are you absolutely sure?",
                "Clear All Data - DANGER",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Stop);

            if (result == DialogResult.Yes)
            {
                var confirmResult = MessageBox.Show(
                    "FINAL CONFIRMATION\n\n" +
                    "This will permanently delete ALL data!\n" +
                    "A backup will be created first.\n\n" +
                    "Type 'DELETE' to confirm this destructive action.",
                    "Final Confirmation Required",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Stop);

                if (confirmResult == DialogResult.OK)
                {
                    // Show input dialog for confirmation
                    string userInput = ShowInputDialog("Type 'DELETE' to confirm:", "Confirmation Required");

                    if (userInput == "DELETE")
                    {
                        await SeedDataWithBackup("Clear All Data", async () => await DatabaseSeeder.ClearAllDataAsync(_context));
                    }
                    else
                    {
                        LogMessage("Clear operation cancelled - incorrect confirmation text.", Color.Orange);
                    }
                }
            }
        }

        private async void BtnResetDatabase_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "This will completely reset the database to factory defaults:\n\n" +
                "⚠️ EXTREME DANGER: This will:\n" +
                "• Drop and recreate all database tables\n" +
                "• Delete ALL data permanently\n" +
                "• Reset database schema to initial state\n" +
                "• Create fresh admin user\n" +
                "• Seed with default sample data\n\n" +
                "A backup will be created automatically before proceeding.\n" +
                "This is equivalent to a fresh installation!\n\n" +
                "Are you absolutely sure?",
                "Reset Database - EXTREME DANGER",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Stop);

            if (result == DialogResult.Yes)
            {
                var confirmResult = MessageBox.Show(
                    "FINAL WARNING\n\n" +
                    "This will DESTROY the entire database!\n" +
                    "A backup will be created first.\n\n" +
                    "Continue with database reset?",
                    "Database Reset - Point of No Return",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Stop);

                if (confirmResult == DialogResult.Yes)
                {
                    // Show input dialog for confirmation
                    string userInput = ShowInputDialog("Type 'RESET DATABASE' to confirm:", "Final Confirmation Required");

                    if (userInput == "RESET DATABASE")
                    {
                        await SeedDataWithBackup("Reset Database", async () => await DatabaseSeeder.ResetDatabaseAsync(_context));
                    }
                    else
                    {
                        LogMessage("Database reset cancelled - incorrect confirmation text.", Color.Orange);
                    }
                }
            }
        }

        private async void BtnBackupDatabase_Click(object sender, EventArgs e)
        {
            try
            {
                DisableButtons();
                
                // First test database connection
                LogMessage("Testing database connection...", Color.Blue);
                await TestDatabaseConnection();
                
                LogMessage("Creating database backup...", Color.Blue);
                string backupPath = await CreateDatabaseBackup();
                
                LogMessage($"✓ Database backup created successfully!", Color.Green);
                LogMessage($"Backup saved to: {backupPath}", Color.Blue);
                
                var result = MessageBox.Show(
                    $"Database backup created successfully!\n\n" +
                    $"Backup saved to:\n{backupPath}\n\n" +
                    "Would you like to open the backup folder?",
                    "Backup Complete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{backupPath}\"");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"✗ Error creating backup: {ex.Message}", Color.Red);
                LogMessage($"Full backup error: {ex}", Color.Red);
                MessageBox.Show($"Error creating backup: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                EnableButtons();
            }
        }

        private async Task TestDatabaseConnection()
        {
            try
            {
                using (var testContext = new ApplicationDbContext())
                {
                    var canConnect = await testContext.Database.CanConnectAsync();
                    LogMessage($"Database connection test: {canConnect}", canConnect ? Color.Green : Color.Red);
                    
                    if (canConnect)
                    {
                        var productCount = await testContext.Products.CountAsync();
                        var categoryCount = await testContext.Categories.CountAsync();
                        var supplierCount = await testContext.Suppliers.CountAsync();
                        
                        LogMessage($"Database stats - Products: {productCount}, Categories: {categoryCount}, Suppliers: {supplierCount}", Color.Blue);
                    }
                    else
                    {
                        throw new Exception("Cannot connect to database");
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Database connection test failed: {ex.Message}", Color.Red);
                throw;
            }
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
                LogMessage($"Full error details: {ex}", Color.Red);
                
                var result = MessageBox.Show(
                    $"Error seeding {dataType}:\n{ex.Message}\n\n" +
                    "Would you like to see the full error details?",
                    "Seeding Error", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Error);
                
                if (result == DialogResult.Yes)
                {
                    MessageBox.Show(ex.ToString(), "Full Error Details", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            finally
            {
                EnableButtons();
            }
        }

        private async Task SeedDataWithBackup(string dataType, Func<Task> seedAction)
        {
            try
            {
                DisableButtons();
                
                // Test database connection first
                LogMessage("Testing database connection...", Color.Blue);
                await TestDatabaseConnection();
                
                LogMessage($"Creating backup before {dataType}...", Color.Blue);

                // Create backup first
                string backupPath = await CreateDatabaseBackup();
                LogMessage($"✓ Backup created: {backupPath}", Color.Green);

                LogMessage($"Starting to {dataType}...", Color.Blue);
                
                // Execute the action with better error handling
                try
                {
                    await seedAction();
                    LogMessage($"✓ Successfully completed {dataType}!", Color.Green);
                    MessageBox.Show($"{dataType} completed successfully!\n\nBackup saved to:\n{backupPath}", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception actionEx)
                {
                    LogMessage($"✗ Error during {dataType} operation: {actionEx.Message}", Color.Red);
                    LogMessage($"Full error details: {actionEx}", Color.Red);
                    
                    var result = MessageBox.Show(
                        $"Error during {dataType}:\n{actionEx.Message}\n\n" +
                        $"A backup was created before the operation:\n{backupPath}\n\n" +
                        "Would you like to see the full error details?",
                        "Operation Failed", 
                        MessageBoxButtons.YesNo, 
                        MessageBoxIcon.Error);
                    
                    if (result == DialogResult.Yes)
                    {
                        MessageBox.Show(actionEx.ToString(), "Full Error Details", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"✗ Error during backup creation: {ex.Message}", Color.Red);
                LogMessage($"Full backup error: {ex}", Color.Red);
                MessageBox.Show($"Error creating backup: {ex.Message}\n\nOperation cancelled for safety.", "Backup Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                EnableButtons();
            }
        }

        private async Task<string> CreateDatabaseBackup()
        {
            return await Task.Run(() =>
            {
                try
                {
                    // Create backups directory if it doesn't exist
                    string backupDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");
                    Directory.CreateDirectory(backupDir);
                    LogMessage($"Backup directory: {backupDir}", Color.Blue);

                    // Generate backup filename with timestamp
                    string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    string backupFileName = $"SyncVerseStudio_Backup_{timestamp}.db";
                    string backupPath = Path.Combine(backupDir, backupFileName);

                    // Try multiple possible database paths
                    string[] possibleDbPaths = {
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "syncversestudio.db"),
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SyncVerseStudio.db"),
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "syncversestudio.db"),
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SyncVerseStudio", "syncversestudio.db")
                    };

                    string currentDbPath = null;
                    foreach (var path in possibleDbPaths)
                    {
                        LogMessage($"Checking database path: {path}", Color.Blue);
                        if (File.Exists(path))
                        {
                            currentDbPath = path;
                            LogMessage($"✓ Found database at: {path}", Color.Green);
                            break;
                        }
                    }

                    if (currentDbPath == null)
                    {
                        // If no database file found, create a backup from the current context
                        LogMessage("No physical database file found, creating JSON export only...", Color.Orange);
                        string jsonBackupPath = Path.Combine(backupDir, $"SyncVerseStudio_Export_{timestamp}.json");
                        ExportDatabaseToJson(jsonBackupPath);
                        LogMessage($"✓ JSON export backup: SyncVerseStudio_Export_{timestamp}.json", Color.Green);
                        return jsonBackupPath;
                    }

                    // Copy the database file
                    File.Copy(currentDbPath, backupPath, true);
                    LogMessage($"✓ Database file copied to: {backupFileName}", Color.Green);
                    
                    // Also create a JSON export for additional safety
                    string jsonBackupPath2 = Path.Combine(backupDir, $"SyncVerseStudio_Export_{timestamp}.json");
                    ExportDatabaseToJson(jsonBackupPath2);
                    LogMessage($"✓ JSON export backup: SyncVerseStudio_Export_{timestamp}.json", Color.Green);

                    return backupPath;
                }
                catch (Exception ex)
                {
                    LogMessage($"Backup error details: {ex}", Color.Red);
                    throw new Exception($"Failed to create backup: {ex.Message}");
                }
            });
        }

        private void ExportDatabaseToJson(string jsonPath)
        {
            try
            {
                LogMessage("Starting JSON export...", Color.Blue);
                using (var context = new ApplicationDbContext())
                {
                    // Test database connection first
                    var canConnect = context.Database.CanConnect();
                    LogMessage($"Database connection test: {canConnect}", Color.Blue);
                    
                    if (!canConnect)
                    {
                        LogMessage("Cannot connect to database for JSON export", Color.Orange);
                        return;
                    }

                    var exportData = new
                    {
                        ExportDate = DateTime.Now,
                        DatabaseVersion = "1.0",
                        Tables = new
                        {
                            Categories = SafeGetData(() => context.Categories.ToList(), "Categories"),
                            Suppliers = SafeGetData(() => context.Suppliers.ToList(), "Suppliers"),
                            Products = SafeGetData(() => context.Products.Include(p => p.Category).Include(p => p.Supplier).ToList(), "Products"),
                            ProductImages = SafeGetData(() => context.ProductImages.ToList(), "ProductImages"),
                            Customers = SafeGetData(() => context.Customers.ToList(), "Customers"),
                            Users = SafeGetData(() => context.Users.Select(u => new { u.Id, u.Username, u.Role, u.CreatedAt }).ToList(), "Users"),
                            Sales = SafeGetData(() => context.Sales.Include(s => s.Customer).ToList(), "Sales"),
                            SaleItems = SafeGetData(() => context.SaleItems.Include(si => si.Product).ToList(), "SaleItems")
                        }
                    };

                    string json = System.Text.Json.JsonSerializer.Serialize(exportData, new System.Text.Json.JsonSerializerOptions 
                    { 
                        WriteIndented = true,
                        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
                    });
                    
                    File.WriteAllText(jsonPath, json);
                    LogMessage("JSON export completed successfully", Color.Green);
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Warning: JSON export failed: {ex.Message}", Color.Orange);
                LogMessage($"JSON export error details: {ex}", Color.Red);
            }
        }

        private T SafeGetData<T>(Func<T> getData, string tableName) where T : new()
        {
            try
            {
                LogMessage($"Exporting {tableName}...", Color.Blue);
                var result = getData();
                LogMessage($"✓ {tableName} exported", Color.Green);
                return result;
            }
            catch (Exception ex)
            {
                LogMessage($"Warning: Failed to export {tableName}: {ex.Message}", Color.Orange);
                return new T();
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
            btnRefreshAll.Enabled = false;
            btnClearAll.Enabled = false;
            btnResetDatabase.Enabled = false;
            btnBackupDatabase.Enabled = false;
        }

        private void EnableButtons()
        {
            btnSeedSuppliers.Enabled = true;
            btnSeedCategories.Enabled = true;
            btnSeedCustomers.Enabled = true;
            btnSeedProducts.Enabled = true;
            btnSeedAll.Enabled = true;
            btnRefreshAll.Enabled = true;
            btnClearAll.Enabled = true;
            btnResetDatabase.Enabled = true;
            btnBackupDatabase.Enabled = true;
        }

        private string ShowInputDialog(string prompt, string title)
        {
            Form inputForm = new Form()
            {
                Width = 400,
                Height = 200,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = title,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false
            };

            Label textLabel = new Label() 
            { 
                Left = 20, 
                Top = 20, 
                Width = 350, 
                Height = 40, 
                Text = prompt,
                Font = new Font("Segoe UI", 10F)
            };

            TextBox textBox = new TextBox() 
            { 
                Left = 20, 
                Top = 70, 
                Width = 350, 
                Height = 25,
                Font = new Font("Segoe UI", 10F)
            };

            Button confirmButton = new Button() 
            { 
                Text = "OK", 
                Left = 200, 
                Width = 80, 
                Top = 110, 
                Height = 30,
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK 
            };

            Button cancelButton = new Button() 
            { 
                Text = "Cancel", 
                Left = 290, 
                Width = 80, 
                Top = 110, 
                Height = 30,
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel 
            };

            confirmButton.Click += (sender, e) => { inputForm.Close(); };
            cancelButton.Click += (sender, e) => { inputForm.Close(); };

            inputForm.Controls.Add(textLabel);
            inputForm.Controls.Add(textBox);
            inputForm.Controls.Add(confirmButton);
            inputForm.Controls.Add(cancelButton);
            inputForm.AcceptButton = confirmButton;
            inputForm.CancelButton = cancelButton;

            return inputForm.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }


}
