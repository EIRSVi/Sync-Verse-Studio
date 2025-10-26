using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using SyncVerseStudio.Helpers;
using System.IO;
using Newtonsoft.Json;

namespace SyncVerseStudio.Views
{
    public class DatabaseConnectionForm : Form
    {
        // Connection String Tab
        private TextBox txtConnectionString;

        // Properties Tab
        private TextBox txtServer;
        private TextBox txtDatabase;
        private ComboBox cmbAuthentication;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private CheckBox chkTrustServerCertificate;

        // Common controls
        private Button btnConnect;
        private Button btnTestConnection;
        private Button btnClose;
        private Label lblStatus;
        private Panel headerPanel;
        private PictureBox logoPictureBox;
        private TabControl tabControl;
        private Button btnCloseX;
        private Point mouseOffset;
        private const string ConfigFile = "dbconfig.json";

        public string ConnectionString { get; private set; }
        public bool IsConnected { get; private set; }

        public DatabaseConnectionForm()
        {
            InitializeComponents();
            LoadSavedConnection();
        }

        private void InitializeComponents()
        {
            this.Text = "Database Connection - SyncVerse Studio";
            this.Size = new Size(750, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = BrandTheme.CoolWhite; // #D7E8FA

            // Header Panel
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 200,
                BackColor = BrandTheme.CoolWhite // #D7E8FA
            };

            // Make header draggable
            headerPanel.MouseDown += (s, e) =>
            {
                mouseOffset = new Point(-e.X, -e.Y);
            };
            headerPanel.MouseMove += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    Point mousePos = Control.MousePosition;
                    mousePos.Offset(mouseOffset.X, mouseOffset.Y);
                    this.Location = mousePos;
                }
            };

            // Close Button
            btnCloseX = new Button
            {
                Text = "X",
                Size = new Size(40, 40),
                Location = new Point(this.Width - 50, 5),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCloseX.FlatAppearance.BorderSize = 0;
            btnCloseX.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 50, 50);
            btnCloseX.Click += (s, e) => this.Close();
            headerPanel.Controls.Add(btnCloseX);

            // Logo - Centered and larger
            logoPictureBox = new PictureBox
            {
                Size = new Size(400, 120),
                Location = new Point((this.Width - 400) / 2, 40),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };

            try
            {
                // Try multiple logo paths
                string[] logoPaths = { 
                    "assets\\brand\\logo.png", 
                    "logo.png",
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "brand", "logo.png"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logo.png")
                };

                bool logoLoaded = false;
                foreach (var logoPath in logoPaths)
                {
                    if (File.Exists(logoPath))
                    {
                        logoPictureBox.Image = Image.FromFile(logoPath);
                        logoLoaded = true;
                        break;
                    }
                }

                if (!logoLoaded)
                {
                    var placeholderLabel = new Label
                    {
                        Text = "SyncVerse Studio",
                        Font = new Font("Segoe UI", 20, FontStyle.Bold),
                        ForeColor = Color.White,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Dock = DockStyle.Fill
                    };
                    logoPictureBox.Controls.Add(placeholderLabel);
                }
            }
            catch { }

            headerPanel.Controls.Add(logoPictureBox);
            this.Controls.Add(headerPanel);

            // Tab Control - No title, minimalist design
            tabControl = new TabControl
            {
                Location = new Point(50, 210),
                Size = new Size(650, 330),
                Font = new Font("Segoe UI", 10)
            };

            // Connection String Tab
            var tabConnectionString = new TabPage("Connection String");
            CreateConnectionStringTab(tabConnectionString);
            tabControl.TabPages.Add(tabConnectionString);

            // Properties Tab
            var tabProperties = new TabPage("Properties");
            CreatePropertiesTab(tabProperties);
            tabControl.TabPages.Add(tabProperties);

            tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;
            this.Controls.Add(tabControl);

            // Status Label
            lblStatus = new Label
            {
                Location = new Point(50, 520),
                Size = new Size(650, 30),
                Font = new Font("Segoe UI", 9),
                ForeColor = BrandTheme.Info,
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lblStatus);

            // Buttons
            btnTestConnection = new Button
            {
                Text = "Test Connection",
                Location = new Point(50, 560),
                Size = new Size(150, 45),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnTestConnection.FlatAppearance.BorderSize = 0;
            btnTestConnection.FlatAppearance.MouseOverBackColor = Color.FromArgb(90, 98, 104);
            btnTestConnection.Click += BtnTestConnection_Click;
            this.Controls.Add(btnTestConnection);

            btnConnect = new Button
            {
                Text = "Connect",
                Location = new Point(220, 560),
                Size = new Size(150, 45),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = BrandTheme.Primary,
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnConnect.FlatAppearance.BorderSize = 0;
            btnConnect.FlatAppearance.MouseOverBackColor = BrandTheme.PrimaryHover;
            btnConnect.Click += BtnConnect_Click;
            this.Controls.Add(btnConnect);

            btnClose = new Button
            {
                Text = "Cancel",
                Location = new Point(390, 560),
                Size = new Size(150, 45),
                Font = new Font("Segoe UI", 10),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = BrandTheme.SecondaryText,
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 2;
            btnClose.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 220);
            btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 245, 245);
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);
        }

        private void CreateConnectionStringTab(TabPage tab)
        {
            tab.BackColor = Color.White;

            var lblInstructions = new Label
            {
                Text = "Enter your SQL Server connection string:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(600, 20),
                ForeColor = BrandTheme.PrimaryText
            };
            tab.Controls.Add(lblInstructions);

            txtConnectionString = new TextBox
            {
                Location = new Point(20, 50),
                Size = new Size(600, 150),
                Multiline = true,
                Font = new Font("Consolas", 9),
                ScrollBars = ScrollBars.Vertical,
                BorderStyle = BorderStyle.FixedSingle
            };
            tab.Controls.Add(txtConnectionString);

            var lblExample = new Label
            {
                Text = "Example:\nData Source=SERVER\\INSTANCE;Initial Catalog=POSDB;Integrated Security=True;Trust Server Certificate=True",
                Font = new Font("Segoe UI", 8),
                Location = new Point(20, 210),
                Size = new Size(600, 40),
                ForeColor = BrandTheme.SecondaryText
            };
            tab.Controls.Add(lblExample);
        }

        private void CreatePropertiesTab(TabPage tab)
        {
            tab.BackColor = Color.White;
            int yPos = 20;

            // Server
            AddLabel(tab, "Server Name / Instance *", yPos);
            txtServer = new TextBox
            {
                Location = new Point(20, yPos + 25),
                Size = new Size(580, 30),
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "e.g., localhost\\SQLEXPRESS or SERVER\\INSTANCE",
                BorderStyle = BorderStyle.FixedSingle
            };
            tab.Controls.Add(txtServer);
            yPos += 65;

            // Database
            AddLabel(tab, "Database Name *", yPos);
            txtDatabase = new TextBox
            {
                Location = new Point(20, yPos + 25),
                Size = new Size(580, 30),
                Font = new Font("Segoe UI", 10),
                PlaceholderText = "e.g., POSDB",
                BorderStyle = BorderStyle.FixedSingle
            };
            tab.Controls.Add(txtDatabase);
            yPos += 65;

            // Authentication
            AddLabel(tab, "Authentication *", yPos);
            cmbAuthentication = new ComboBox
            {
                Location = new Point(20, yPos + 25),
                Size = new Size(580, 30),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbAuthentication.Items.AddRange(new object[] { "Windows Authentication", "SQL Server Authentication" });
            cmbAuthentication.SelectedIndex = 0;
            cmbAuthentication.SelectedIndexChanged += CmbAuthentication_SelectedIndexChanged;
            tab.Controls.Add(cmbAuthentication);
            yPos += 65;

            // Username (initially hidden)
            var lblUsername = new Label
            {
                Text = "Username",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, yPos),
                Size = new Size(280, 20),
                ForeColor = BrandTheme.PrimaryText,
                Visible = false
            };
            tab.Controls.Add(lblUsername);

            txtUsername = new TextBox
            {
                Location = new Point(20, yPos + 25),
                Size = new Size(280, 30),
                Font = new Font("Segoe UI", 10),
                Visible = false,
                BorderStyle = BorderStyle.FixedSingle
            };
            tab.Controls.Add(txtUsername);

            // Password (initially hidden)
            var lblPassword = new Label
            {
                Text = "Password",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(320, yPos),
                Size = new Size(280, 20),
                ForeColor = BrandTheme.PrimaryText,
                Visible = false
            };
            tab.Controls.Add(lblPassword);

            txtPassword = new TextBox
            {
                Location = new Point(320, yPos + 25),
                Size = new Size(280, 30),
                Font = new Font("Segoe UI", 10),
                UseSystemPasswordChar = true,
                Visible = false,
                BorderStyle = BorderStyle.FixedSingle
            };
            tab.Controls.Add(txtPassword);
            yPos += 65;

            // Trust Server Certificate
            chkTrustServerCertificate = new CheckBox
            {
                Text = "Trust Server Certificate",
                Location = new Point(20, yPos),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10),
                Checked = true
            };
            tab.Controls.Add(chkTrustServerCertificate);
        }

        private void AddLabel(TabPage tab, string text, int yPos)
        {
            var label = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, yPos),
                Size = new Size(580, 20),
                ForeColor = BrandTheme.PrimaryText
            };
            tab.Controls.Add(label);
        }

        private void CmbAuthentication_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isSqlAuth = cmbAuthentication.SelectedIndex == 1;

            // Find and show/hide username and password controls
            foreach (Control control in tabControl.TabPages[1].Controls)
            {
                if (control == txtUsername || control == txtPassword)
                {
                    control.Visible = isSqlAuth;
                }
                if (control is Label lbl && (lbl.Text == "Username" || lbl.Text == "Password"))
                {
                    lbl.Visible = isSqlAuth;
                }
            }
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Sync between tabs if possible
            if (tabControl.SelectedIndex == 0 && !string.IsNullOrEmpty(txtServer?.Text))
            {
                // Properties to Connection String
                txtConnectionString.Text = BuildConnectionStringFromProperties();
            }
            else if (tabControl.SelectedIndex == 1 && !string.IsNullOrEmpty(txtConnectionString?.Text))
            {
                // Connection String to Properties (try to parse)
                TryParseConnectionString(txtConnectionString.Text);
            }
        }

        private string BuildConnectionStringFromProperties()
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = txtServer.Text,
                InitialCatalog = txtDatabase.Text,
                TrustServerCertificate = chkTrustServerCertificate.Checked
            };

            if (cmbAuthentication.SelectedIndex == 0)
            {
                builder.IntegratedSecurity = true;
            }
            else
            {
                builder.IntegratedSecurity = false;
                builder.UserID = txtUsername.Text;
                builder.Password = txtPassword.Text;
            }

            return builder.ConnectionString;
        }

        private void TryParseConnectionString(string connectionString)
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(connectionString);
                txtServer.Text = builder.DataSource;
                txtDatabase.Text = builder.InitialCatalog;
                chkTrustServerCertificate.Checked = builder.TrustServerCertificate;

                if (builder.IntegratedSecurity)
                {
                    cmbAuthentication.SelectedIndex = 0;
                }
                else
                {
                    cmbAuthentication.SelectedIndex = 1;
                    txtUsername.Text = builder.UserID;
                    txtPassword.Text = builder.Password;
                }
            }
            catch
            {
                // If parsing fails, just leave the properties as is
            }
        }

        private void LoadSavedConnection()
        {
            try
            {
                if (File.Exists(ConfigFile))
                {
                    var json = File.ReadAllText(ConfigFile);
                    var config = JsonConvert.DeserializeObject<DatabaseConfig>(json);
                    if (config != null && !string.IsNullOrEmpty(config.ConnectionString))
                    {
                        txtConnectionString.Text = config.ConnectionString;
                        TryParseConnectionString(config.ConnectionString);
                        lblStatus.Text = "Previous connection loaded. Click 'Connect' to use it.";
                        lblStatus.ForeColor = BrandTheme.Info;
                    }
                }
            }
            catch { }
        }

        private void SaveConnection(string connectionString)
        {
            try
            {
                var config = new DatabaseConfig { ConnectionString = connectionString };
                var json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(ConfigFile, json);
            }
            catch { }
        }

        private void BtnTestConnection_Click(object sender, EventArgs e)
        {
            string connString = GetCurrentConnectionString();

            if (string.IsNullOrEmpty(connString))
            {
                lblStatus.Text = "Please enter connection details.";
                lblStatus.ForeColor = BrandTheme.Error;
                return;
            }

            try
            {
                using (var connection = new SqlConnection(connString))
                {
                    connection.Open();
                    lblStatus.Text = "Connection successful!";
                    lblStatus.ForeColor = BrandTheme.Success;
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Connection failed: {ex.Message}";
                lblStatus.ForeColor = BrandTheme.Error;
            }
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            string connString = GetCurrentConnectionString();

            if (string.IsNullOrEmpty(connString))
            {
                MessageBox.Show("Please enter connection details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (var connection = new SqlConnection(connString))
                {
                    connection.Open();
                }

                ConnectionString = connString;
                IsConnected = true;
                SaveConnection(connString);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetCurrentConnectionString()
        {
            if (tabControl.SelectedIndex == 0)
            {
                return txtConnectionString.Text.Trim();
            }
            else
            {
                if (string.IsNullOrEmpty(txtServer.Text) || string.IsNullOrEmpty(txtDatabase.Text))
                {
                    return string.Empty;
                }
                return BuildConnectionStringFromProperties();
            }
        }

        private class DatabaseConfig
        {
            public string ConnectionString { get; set; }
        }
    }
}
