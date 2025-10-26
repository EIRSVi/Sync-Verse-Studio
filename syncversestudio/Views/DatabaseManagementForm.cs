using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using Microsoft.Data.SqlClient;
using SyncVerseStudio.Helpers;
using SyncVerseStudio.Services;
using SyncVerseStudio.Data;
using Microsoft.EntityFrameworkCore;

namespace SyncVerseStudio.Views
{
    public class DatabaseManagementForm : Form
    {
        private Button btnBackupDatabase;
        private Button btnRestoreDatabase;
        private Button btnWipeDatabase;
        private Button btnSeedCategories;
        private Button btnSeedSuppliers;
        private Button btnSeedProducts;
        private Button btnClose;
        private Label lblStatus;
        private TextBox txtBackupPath;
        private Button btnBrowseBackup;
        private Button btnBrowseRestore;
        private TextBox txtRestorePath;

        public DatabaseManagementForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Database Management - SyncVerse Studio";
            this.Size = new Size(700, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            // Title
            var titleLabel = new Label
            {
                Text = "Database Management",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                Location = new Point(50, 20),
                Size = new Size(600, 35),
                ForeColor = BrandTheme.PrimaryText
            };
            this.Controls.Add(titleLabel);

            int yPos = 80;

            // Backup Section
            var lblBackup = new Label
            {
                Text = "Backup Database",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(50, yPos),
                Size = new Size(600, 25),
                ForeColor = BrandTheme.PrimaryText
            };
            this.Controls.Add(lblBackup);
            yPos += 35;

            txtBackupPath = new TextBox
            {
                Location = new Point(50, yPos),
                Size = new Size(450, 30),
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "Select backup location..."
            };
            this.Controls.Add(txtBackupPath);

            btnBrowseBackup = new Button
            {
                Text = "Browse",
                Location = new Point(510, yPos),
                Size = new Size(100, 30)
            };
            BrandTheme.StyleButton(btnBrowseBackup, "edit");
            btnBrowseBackup.Click += BtnBrowseBackup_Click;
            this.Controls.Add(btnBrowseBackup);
            yPos += 45;

            btnBackupDatabase = new Button
            {
                Text = "Backup Database",
                Location = new Point(50, yPos),
                Size = new Size(200, 40)
            };
            BrandTheme.StyleButton(btnBackupDatabase, "add");
            btnBackupDatabase.Click += BtnBackupDatabase_Click;
            this.Controls.Add(btnBackupDatabase);
            yPos += 60;

            // Restore Section
            var lblRestore = new Label
            {
                Text = "Restore Database",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(50, yPos),
                Size = new Size(600, 25),
                ForeColor = BrandTheme.PrimaryText
            };
            this.Controls.Add(lblRestore);
            yPos += 35;

            txtRestorePath = new TextBox
            {
                Location = new Point(50, yPos),
                Size = new Size(450, 30),
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "Select backup file to restore..."
            };
            this.Controls.Add(txtRestorePath);

            btnBrowseRestore = new Button
            {
                Text = "Browse",
                Location = new Point(510, yPos),
                Size = new Size(100, 30)
            };
            BrandTheme.StyleButton(btnBrowseRestore, "edit");
            btnBrowseRestore.Click += BtnBrowseRestore_Click;
            this.Controls.Add(btnBrowseRestore);
            yPos += 45;

            btnRestoreDatabase = new Button
            {
                Text = "Restore Database",
                Location = new Point(50, yPos),
                Size = new Size(200, 40)
            };
            BrandTheme.StyleButton(btnRestoreDatabase, "edit");
            btnRestoreDatabase.Click += BtnRestoreDatabase_Click;
            this.Controls.Add(btnRestoreDatabase);
            yPos += 60;

            // Wipe Database Section
            var lblWipe = new Label
            {
                Text = "Wipe Database (Delete All Data)",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(50, yPos),
                Size = new Size(600, 25),
                ForeColor = BrandTheme.Error
            };
            this.Controls.Add(lblWipe);
            yPos += 35;

            btnWipeDatabase = new Button
            {
                Text = "⚠ Wipe All Data",
                Location = new Point(50, yPos),
                Size = new Size(200, 40)
            };
            BrandTheme.StyleButton(btnWipeDatabase, "delete");
            btnWipeDatabase.Click += BtnWipeDatabase_Click;
            this.Controls.Add(btnWipeDatabase);
            yPos += 60;

            // Seeder Section
            var lblSeeder = new Label
            {
                Text = "Data Seeders (Sample Data)",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(50, yPos),
                Size = new Size(600, 25),
                ForeColor = BrandTheme.PrimaryText
            };
            this.Controls.Add(lblSeeder);
            yPos += 35;

            btnSeedCategories = new Button
            {
                Text = "Seed Categories",
                Location = new Point(50, yPos),
                Size = new Size(150, 35)
            };
            BrandTheme.StyleButton(btnSeedCategories, "add");
            btnSeedCategories.Click += BtnSeedCategories_Click;
            this.Controls.Add(btnSeedCategories);

            btnSeedSuppliers = new Button
            {
                Text = "Seed Suppliers",
                Location = new Point(220, yPos),
                Size = new Size(150, 35)
            };
            BrandTheme.StyleButton(btnSeedSuppliers, "add");
            btnSeedSuppliers.Click += BtnSeedSuppliers_Click;
            this.Controls.Add(btnSeedSuppliers);

            btnSeedProducts = new Button
            {
                Text = "Seed Products",
                Location = new Point(390, yPos),
                Size = new Size(150, 35)
            };
            BrandTheme.StyleButton(btnSeedProducts, "add");
            btnSeedProducts.Click += BtnSeedProducts_Click;
            this.Controls.Add(btnSeedProducts);
            yPos += 55;

            // Status Label
            lblStatus = new Label
            {
                Location = new Point(50, yPos),
                Size = new Size(600, 30),
                Font = BrandTheme.BodyFont,
                ForeColor = BrandTheme.Info,
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lblStatus);
            yPos += 40;

            // Close Button
            btnClose = new Button
            {
                Text = "Close",
                Location = new Point(250, yPos),
                Size = new Size(200, 40)
            };
            BrandTheme.StyleButton(btnClose, "cancel");
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);
        }

        private void BtnBrowseBackup_Click(object sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "Backup Files (*.bak)|*.bak|All Files (*.*)|*.*";
                dialog.DefaultExt = "bak";
                dialog.FileName = $"POSDB_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
                
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtBackupPath.Text = dialog.FileName;
                }
            }
        }

        private void BtnBrowseRestore_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Backup Files (*.bak)|*.bak|All Files (*.*)|*.*";
                
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtRestorePath.Text = dialog.FileName;
                }
            }
        }

        private async void BtnBackupDatabase_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBackupPath.Text))
            {
                MessageBox.Show("Please select a backup location.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                btnBackupDatabase.Enabled = false;
                lblStatus.Text = "Backing up database...";
                lblStatus.ForeColor = BrandTheme.Info;

                var connectionString = DatabaseConnectionManager.GetConnectionString();
                var builder = new SqlConnectionStringBuilder(connectionString);
                var databaseName = builder.InitialCatalog;

                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var backupQuery = $"BACKUP DATABASE [{databaseName}] TO DISK = '{txtBackupPath.Text}' WITH FORMAT, INIT";
                    using (var command = new SqlCommand(backupQuery, connection))
                    {
                        command.CommandTimeout = 300; // 5 minutes
                        await command.ExecuteNonQueryAsync();
                    }
                }

                lblStatus.Text = "✓ Database backed up successfully!";
                lblStatus.ForeColor = BrandTheme.Success;
                MessageBox.Show("Database backed up successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"✗ Backup failed: {ex.Message}";
                lblStatus.ForeColor = BrandTheme.Error;
                MessageBox.Show($"Backup failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnBackupDatabase.Enabled = true;
            }
        }

        private async void BtnRestoreDatabase_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRestorePath.Text))
            {
                MessageBox.Show("Please select a backup file to restore.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!File.Exists(txtRestorePath.Text))
            {
                MessageBox.Show("Backup file not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var result = MessageBox.Show(
                "This will restore the database from the backup file. All current data will be replaced. Continue?",
                "Confirm Restore",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result != DialogResult.Yes)
                return;

            try
            {
                btnRestoreDatabase.Enabled = false;
                lblStatus.Text = "Restoring database...";
                lblStatus.ForeColor = BrandTheme.Info;

                var connectionString = DatabaseConnectionManager.GetConnectionString();
                var builder = new SqlConnectionStringBuilder(connectionString);
                var databaseName = builder.InitialCatalog;

                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    
                    // Set database to single user mode
                    var singleUserQuery = $"ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE";
                    using (var command = new SqlCommand(singleUserQuery, connection))
                    {
                        await command.ExecuteNonQueryAsync();
                    }

                    // Restore database
                    var restoreQuery = $"RESTORE DATABASE [{databaseName}] FROM DISK = '{txtRestorePath.Text}' WITH REPLACE";
                    using (var command = new SqlCommand(restoreQuery, connection))
                    {
                        command.CommandTimeout = 300; // 5 minutes
                        await command.ExecuteNonQueryAsync();
                    }

                    // Set database back to multi user mode
                    var multiUserQuery = $"ALTER DATABASE [{databaseName}] SET MULTI_USER";
                    using (var command = new SqlCommand(multiUserQuery, connection))
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                }

                lblStatus.Text = "✓ Database restored successfully!";
                lblStatus.ForeColor = BrandTheme.Success;
                MessageBox.Show("Database restored successfully! Please restart the application.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"✗ Restore failed: {ex.Message}";
                lblStatus.ForeColor = BrandTheme.Error;
                MessageBox.Show($"Restore failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnRestoreDatabase.Enabled = true;
            }
        }

        private async void BtnWipeDatabase_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "⚠ WARNING: This will DELETE ALL DATA from the database!\n\nIt is STRONGLY RECOMMENDED to backup your database first.\n\nDo you want to proceed?",
                "Confirm Wipe Database",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result != DialogResult.Yes)
                return;

            // Second confirmation
            var result2 = MessageBox.Show(
                "Are you ABSOLUTELY SURE? This action CANNOT be undone!",
                "Final Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Error
            );

            if (result2 != DialogResult.Yes)
                return;

            try
            {
                btnWipeDatabase.Enabled = false;
                lblStatus.Text = "Wiping database...";
                lblStatus.ForeColor = BrandTheme.Warning;

                using (var context = new ApplicationDbContext())
                {
                    // Delete all data in order (respecting foreign keys)
                    await context.Database.ExecuteSqlRawAsync("DELETE FROM SaleItems");
                    await context.Database.ExecuteSqlRawAsync("DELETE FROM Sales");
                    await context.Database.ExecuteSqlRawAsync("DELETE FROM InventoryMovements");
                    await context.Database.ExecuteSqlRawAsync("DELETE FROM ProductImages");
                    await context.Database.ExecuteSqlRawAsync("DELETE FROM Products");
                    await context.Database.ExecuteSqlRawAsync("DELETE FROM Categories");
                    await context.Database.ExecuteSqlRawAsync("DELETE FROM Suppliers");
                    await context.Database.ExecuteSqlRawAsync("DELETE FROM Customers");
                    await context.Database.ExecuteSqlRawAsync("DELETE FROM AuditLogs");
                    await context.Database.ExecuteSqlRawAsync("DELETE FROM Users");

                    // Reset identity columns
                    await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('SaleItems', RESEED, 0)");
                    await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Sales', RESEED, 0)");
                    await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('InventoryMovements', RESEED, 0)");
                    await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('ProductImages', RESEED, 0)");
                    await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Products', RESEED, 0)");
                    await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Categories', RESEED, 0)");
                    await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Suppliers', RESEED, 0)");
                    await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Customers', RESEED, 0)");
                    await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('AuditLogs', RESEED, 0)");
                    await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Users', RESEED, 0)");
                }

                lblStatus.Text = "✓ Database wiped successfully!";
                lblStatus.ForeColor = BrandTheme.Success;
                MessageBox.Show("Database wiped successfully! All data has been deleted.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"✗ Wipe failed: {ex.Message}";
                lblStatus.ForeColor = BrandTheme.Error;
                MessageBox.Show($"Wipe failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnWipeDatabase.Enabled = true;
            }
        }

        private async void BtnSeedCategories_Click(object sender, EventArgs e)
        {
            try
            {
                btnSeedCategories.Enabled = false;
                lblStatus.Text = "Seeding categories...";
                lblStatus.ForeColor = BrandTheme.Info;

                await DatabaseInitializer.SeedCategoriesAsync();

                lblStatus.Text = "✓ Categories seeded successfully!";
                lblStatus.ForeColor = BrandTheme.Success;
                MessageBox.Show("Categories seeded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"✗ Seeding failed: {ex.Message}";
                lblStatus.ForeColor = BrandTheme.Error;
                MessageBox.Show($"Seeding failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSeedCategories.Enabled = true;
            }
        }

        private async void BtnSeedSuppliers_Click(object sender, EventArgs e)
        {
            try
            {
                btnSeedSuppliers.Enabled = false;
                lblStatus.Text = "Seeding suppliers...";
                lblStatus.ForeColor = BrandTheme.Info;

                await DatabaseInitializer.SeedSuppliersAsync();

                lblStatus.Text = "✓ Suppliers seeded successfully!";
                lblStatus.ForeColor = BrandTheme.Success;
                MessageBox.Show("Suppliers seeded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"✗ Seeding failed: {ex.Message}";
                lblStatus.ForeColor = BrandTheme.Error;
                MessageBox.Show($"Seeding failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSeedSuppliers.Enabled = true;
            }
        }

        private async void BtnSeedProducts_Click(object sender, EventArgs e)
        {
            try
            {
                btnSeedProducts.Enabled = false;
                lblStatus.Text = "Seeding products...";
                lblStatus.ForeColor = BrandTheme.Info;

                await DatabaseInitializer.SeedProductsAsync();

                lblStatus.Text = "✓ Products seeded successfully!";
                lblStatus.ForeColor = BrandTheme.Success;
                MessageBox.Show("Products seeded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"✗ Seeding failed: {ex.Message}";
                lblStatus.ForeColor = BrandTheme.Error;
                MessageBox.Show($"Seeding failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSeedProducts.Enabled = true;
            }
        }
    }
}
