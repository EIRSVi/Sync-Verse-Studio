using System.Drawing;
using SyncVerseStudio.Services;
using SyncVerseStudio.Data;
using SyncVerseStudio.Models;
using Microsoft.EntityFrameworkCore;
using FontAwesome.Sharp;

namespace SyncVerseStudio.Views
{
    public partial class AuditLogView : Form
    {
        private readonly AuthenticationService _authService;
        private readonly ApplicationDbContext _context;
        private DataGridView auditGrid;
        private Panel topPanel, statsPanel;
        private IconButton refreshButton, exportButton, clearButton, filterButton;
        private TextBox searchBox;
        private ComboBox actionFilter, userFilter;
        private DateTimePicker fromDatePicker, toDatePicker;
        private Label totalLogsLabel, todayLogsLabel, successLabel, failedLabel;

        public AuditLogView(AuthenticationService authService)
        {
            _authService = authService;
            _context = new ApplicationDbContext();
            InitializeComponent();
            LoadAuditLogs();
            LoadFilters();
            LoadStatistics();
        }

        private void InitializeComponent()
        {
            this.AutoScaleDimensions = new SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(245, 245, 250);
            this.ClientSize = new Size(1200, 800);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Name = "AuditLogView";
            this.Text = "Audit Log Management";
            this.Padding = new Padding(0);

            CreateTopPanel();
            CreateStatsPanel();
            CreateAuditGrid();
        }

        private void CreateTopPanel()
        {
            topPanel = new Panel
            {
                BackColor = Color.White,
                Dock = DockStyle.Top,
                Height = 120, // Reduced from 140
                Padding = new Padding(25, 12, 25, 12) // Reduced padding
            };

            // Title with icon
            var titleIconPic = new IconPictureBox
            {
                IconChar = IconChar.History,
                IconColor = Color.FromArgb(103, 58, 183),
                IconSize = 32, // Reduced from 36
                Location = new Point(25, 15),
                Size = new Size(38, 38),
                BackColor = Color.Transparent
            };
            topPanel.Controls.Add(titleIconPic);

            var titleLabel = new Label
            {
                Text = "System Audit Logs",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold), // Reduced from 20F
                ForeColor = Color.FromArgb(33, 33, 33),
                Location = new Point(70, 16),
                Size = new Size(320, 34)
            };
            topPanel.Controls.Add(titleLabel);

            var subtitleLabel = new Label
            {
                Text = "Monitor all system activities and user actions",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular), // Slightly smaller
                ForeColor = Color.FromArgb(117, 117, 117),
                Location = new Point(70, 48),
                Size = new Size(380, 20)
            };
            topPanel.Controls.Add(subtitleLabel);

            // Search bar with icon overlay
            var searchPanel = new Panel
            {
                Location = new Point(25, 78), // Adjusted Y position
                Size = new Size(300, 34), // Slightly smaller
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle
            };

            var searchIcon = new IconPictureBox
            {
                IconChar = IconChar.Search,
                IconColor = Color.FromArgb(117, 117, 117),
                IconSize = 16,
                Location = new Point(10, 8),
                Size = new Size(20, 20),
                BackColor = Color.Transparent
            };
            searchPanel.Controls.Add(searchIcon);

            searchBox = new TextBox
            {
                PlaceholderText = "Search logs...", // Shorter placeholder
                Font = new Font("Segoe UI", 10F),
                Location = new Point(38, 6),
                Size = new Size(255, 24),
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(248, 249, 250)
            };
            searchBox.TextChanged += SearchBox_TextChanged;
            searchPanel.Controls.Add(searchBox);
            topPanel.Controls.Add(searchPanel);

            // Date range filters
            var dateIcon = new IconPictureBox
            {
                IconChar = IconChar.CalendarAlt,
                IconColor = Color.FromArgb(103, 58, 183),
                IconSize = 18,
                Location = new Point(340, 83),
                Size = new Size(22, 22),
                BackColor = Color.Transparent
            };
            topPanel.Controls.Add(dateIcon);

            fromDatePicker = new DateTimePicker
            {
                Font = new Font("Segoe UI", 9.5F),
                Location = new Point(368, 80),
                Size = new Size(120, 26),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today.AddDays(-30)
            };
            fromDatePicker.ValueChanged += DateFilter_Changed;
            topPanel.Controls.Add(fromDatePicker);

            var dateArrow = new Label
            {
                Text = "?",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(117, 117, 117),
                Location = new Point(493, 80),
                Size = new Size(18, 26),
                TextAlign = ContentAlignment.MiddleCenter
            };
            topPanel.Controls.Add(dateArrow);

            toDatePicker = new DateTimePicker
            {
                Font = new Font("Segoe UI", 9.5F),
                Location = new Point(516, 80),
                Size = new Size(120, 26),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today
            };
            toDatePicker.ValueChanged += DateFilter_Changed;
            topPanel.Controls.Add(toDatePicker);

            // Action and User filters
            actionFilter = new ComboBox
            {
                Font = new Font("Segoe UI", 9.5F),
                Location = new Point(650, 80),
                Size = new Size(140, 26),
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat
            };
            actionFilter.SelectedIndexChanged += ActionFilter_SelectedIndexChanged;
            topPanel.Controls.Add(actionFilter);

            userFilter = new ComboBox
            {
                Font = new Font("Segoe UI", 9.5F),
                Location = new Point(805, 80),
                Size = new Size(140, 26),
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat
            };
            userFilter.SelectedIndexChanged += UserFilter_SelectedIndexChanged;
            topPanel.Controls.Add(userFilter);

            // Icon-only action buttons - smaller
            int buttonX = 965;
            
            refreshButton = CreateIconOnlyButton(IconChar.SyncAlt, Color.FromArgb(33, 150, 243), "Refresh", buttonX, 76);
            refreshButton.Click += RefreshButton_Click;
            buttonX += 46;

            exportButton = CreateIconOnlyButton(IconChar.FileExport, Color.FromArgb(76, 175, 80), "Export to CSV", buttonX, 76);
            exportButton.Click += ExportButton_Click;
            buttonX += 46;

            clearButton = CreateIconOnlyButton(IconChar.TrashAlt, Color.FromArgb(255, 152, 0), "Clear Old Logs", buttonX, 76);
            clearButton.Click += ClearButton_Click;

            topPanel.Controls.AddRange(new Control[] { refreshButton, exportButton, clearButton });

            this.Controls.Add(topPanel);
        }

        private IconButton CreateIconOnlyButton(IconChar icon, Color color, string tooltip, int x, int y)
        {
            var button = new IconButton
            {
                IconChar = icon,
                IconColor = Color.White,
                IconSize = 20, // Reduced from 22
                BackColor = color,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(x, y),
                Size = new Size(38, 38), // Reduced from 42x42
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0 },
                TextAlign = ContentAlignment.MiddleCenter,
                ImageAlign = ContentAlignment.MiddleCenter
            };

            // Tooltip
            var toolTip = new ToolTip();
            toolTip.SetToolTip(button, tooltip);

            // Hover effects
            button.MouseEnter += (s, e) =>
            {
                button.BackColor = ControlPaint.Light(color, 0.2f);
                button.FlatAppearance.BorderSize = 2;
                button.FlatAppearance.BorderColor = color;
            };

            button.MouseLeave += (s, e) =>
            {
                button.BackColor = color;
                button.FlatAppearance.BorderSize = 0;
            };

            return button;
        }

        private void CreateStatsPanel()
        {
            statsPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 85, // Reduced from 100
                BackColor = Color.FromArgb(245, 245, 250),
                Padding = new Padding(25, 10, 25, 10) // Reduced padding
            };

            // Total Logs Card
            var totalCard = CreateStatCard("Total Logs", "0", IconChar.Database, Color.FromArgb(103, 58, 183), 25);
            totalLogsLabel = totalCard.Controls.OfType<Label>().First(l => l.Name == "ValueLabel");
            statsPanel.Controls.Add(totalCard);

            // Today's Logs Card
            var todayCard = CreateStatCard("Today", "0", IconChar.CalendarDay, Color.FromArgb(33, 150, 243), 280);
            todayLogsLabel = todayCard.Controls.OfType<Label>().First(l => l.Name == "ValueLabel");
            statsPanel.Controls.Add(todayCard);

            // Success Count Card
            var successCard = CreateStatCard("Success", "0", IconChar.CheckCircle, Color.FromArgb(76, 175, 80), 535);
            successLabel = successCard.Controls.OfType<Label>().First(l => l.Name == "ValueLabel");
            statsPanel.Controls.Add(successCard);

            // Failed Count Card
            var failedCard = CreateStatCard("Failed", "0", IconChar.TimesCircle, Color.FromArgb(244, 67, 54), 790);
            failedLabel = failedCard.Controls.OfType<Label>().First(l => l.Name == "ValueLabel");
            statsPanel.Controls.Add(failedCard);

            this.Controls.Add(statsPanel);
        }

        private Panel CreateStatCard(string title, string value, IconChar icon, Color color, int x)
        {
            var card = new Panel
            {
                Location = new Point(x, 8), // Adjusted Y position
                Size = new Size(240, 65), // Reduced from 75
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Left accent bar
            var accent = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(4, 65), // Thinner accent bar
                BackColor = color
            };
            card.Controls.Add(accent);

            var iconPic = new IconPictureBox
            {
                IconChar = icon,
                IconColor = color,
                IconSize = 28, // Reduced from 32
                Location = new Point(15, 16),
                Size = new Size(32, 32),
                BackColor = Color.Transparent
            };
            card.Controls.Add(iconPic);

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Regular), // Smaller font
                ForeColor = Color.FromArgb(117, 117, 117),
                Location = new Point(58, 14),
                Size = new Size(170, 18)
            };
            card.Controls.Add(titleLabel);

            var valueLabel = new Label
            {
                Name = "ValueLabel",
                Text = value,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold), // Reduced from 20F
                ForeColor = Color.FromArgb(33, 33, 33),
                Location = new Point(58, 30),
                Size = new Size(170, 28)
            };
            card.Controls.Add(valueLabel);

            return card;
        }

        private void CreateAuditGrid()
        {
            // Create a wrapper panel with padding
            var gridPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(226, 244, 255), // Patten Blue background
                Padding = new Padding(10, 200, 10, 10) // Left, Top, Right, Bottom - 200px padding from top
            };

            auditGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 9F),
                RowHeadersVisible = false,
                GridColor = Color.FromArgb(226, 244, 255), // Patten Blue
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(226, 244, 255) // Patten Blue
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.White,
                    ForeColor = Color.FromArgb(33, 33, 33),
                    SelectionBackColor = Color.FromArgb(118, 189, 255), // Malibu
                    SelectionForeColor = Color.White,
                    Padding = new Padding(8, 4, 8, 4),
                    WrapMode = DataGridViewTriState.False,
                    Font = new Font("Segoe UI", 9F),
                    Alignment = DataGridViewContentAlignment.MiddleCenter // Center all text
                },
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(226, 244, 255), // Patten Blue
                    ForeColor = Color.FromArgb(60, 60, 60),
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleCenter, // Center headers
                    Padding = new Padding(8, 6, 8, 6)
                },
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 36,
                RowTemplate = { Height = 32 },
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None
            };

            // Columns WITHOUT Action column - removed as requested
            auditGrid.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn 
                { 
                    Name = "Id", 
                    HeaderText = "ID", 
                    Width = 50, 
                    Visible = false 
                },
                new DataGridViewTextBoxColumn 
                { 
                    Name = "Timestamp", 
                    HeaderText = "Timestamp",
                    FillWeight = 15,
                    DefaultCellStyle = new DataGridViewCellStyle 
                    { 
                        Alignment = DataGridViewContentAlignment.MiddleCenter
                    }
                },
                new DataGridViewTextBoxColumn 
                { 
                    Name = "User", 
                    HeaderText = "User", 
                    FillWeight = 18,
                    DefaultCellStyle = new DataGridViewCellStyle 
                    { 
                        Alignment = DataGridViewContentAlignment.MiddleCenter
                    }
                },
                new DataGridViewTextBoxColumn 
                { 
                    Name = "Action", 
                    HeaderText = "Action", 
                    FillWeight = 17,
                    DefaultCellStyle = new DataGridViewCellStyle 
                    { 
                        Alignment = DataGridViewContentAlignment.MiddleCenter
                    }
                },
                new DataGridViewTextBoxColumn 
                { 
                    Name = "Table", 
                    HeaderText = "Table", 
                    FillWeight = 12,
                    DefaultCellStyle = new DataGridViewCellStyle 
                    { 
                        Alignment = DataGridViewContentAlignment.MiddleCenter
                    }
                },
                new DataGridViewTextBoxColumn 
                { 
                    Name = "Details", 
                    HeaderText = "Details", 
                    FillWeight = 28,
                    DefaultCellStyle = new DataGridViewCellStyle 
                    { 
                        Alignment = DataGridViewContentAlignment.MiddleCenter
                    }
                },
                new DataGridViewTextBoxColumn 
                { 
                    Name = "IpAddress", 
                    HeaderText = "IP Address", 
                    FillWeight = 10,
                    DefaultCellStyle = new DataGridViewCellStyle 
                    { 
                        Alignment = DataGridViewContentAlignment.MiddleCenter
                    }
                }
                // REMOVED: Action button column as requested
            });

            // Format timestamp column to be more compact
            auditGrid.Columns["Timestamp"].DefaultCellStyle.Format = "MM/dd HH:mm";

            gridPanel.Controls.Add(auditGrid);
            this.Controls.Add(gridPanel);
        }

        private async void LoadAuditLogs()
        {
            try
            {
                var fromDate = fromDatePicker.Value.Date;
                var toDate = toDatePicker.Value.Date.AddDays(1).AddSeconds(-1);

                var logs = await _context.AuditLogs
                    .Include(a => a.User)
                    .Where(a => a.Timestamp >= fromDate && a.Timestamp <= toDate)
                    .OrderByDescending(a => a.Timestamp)
                    .Take(500)
                    .ToListAsync();

                auditGrid.Rows.Clear();

                foreach (var log in logs)
                {
                    var userName = log.User?.FullName ?? "System";
                    var displayDetails = log.NewValues ?? "No details";
                    
                    // Truncate long details for clean display
                    if (displayDetails.Length > 80)
                    {
                        displayDetails = displayDetails.Substring(0, 77) + "...";
                    }

                    var rowIndex = auditGrid.Rows.Add(
                        log.Id,
                        log.Timestamp,
                        userName,
                        log.Action,
                        log.TableName ?? "System",
                        displayDetails,
                        log.IpAddress ?? "N/A"
                        // REMOVED: Button column
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading audit logs: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void LoadStatistics()
        {
            try
            {
                var totalCount = await _context.AuditLogs.CountAsync();
                var todayCount = await _context.AuditLogs
                    .CountAsync(a => a.Timestamp.Date == DateTime.Today);
                var successCount = await _context.AuditLogs
                    .CountAsync(a => a.Action.Contains("SUCCESS"));
                var failedCount = await _context.AuditLogs
                    .CountAsync(a => a.Action.Contains("FAILED") || a.Action.Contains("ERROR"));

                // Display real data without leading zeros
                totalLogsLabel.Text = totalCount.ToString();
                todayLogsLabel.Text = todayCount.ToString();
                successLabel.Text = successCount.ToString();
                failedLabel.Text = failedCount.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading statistics: {ex.Message}");
            }
        }

        private async void LoadFilters()
        {
            try
            {
                var actions = await _context.AuditLogs
                    .Select(a => a.Action)
                    .Distinct()
                    .OrderBy(a => a)
                    .ToListAsync();

                actionFilter.Items.Clear();
                actionFilter.Items.Add("All Actions");
                actionFilter.Items.AddRange(actions.ToArray());
                actionFilter.SelectedIndex = 0;

                var users = await _context.Users
                    .Select(u => new { u.Id, u.FullName })
                    .OrderBy(u => u.FullName)
                    .ToListAsync();

                userFilter.Items.Clear();
                userFilter.Items.Add("All Users");
                foreach (var user in users)
                {
                    userFilter.Items.Add(user.FullName);
                }
                userFilter.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading filters: {ex.Message}");
            }
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            FilterLogs();
        }

        private void ActionFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterLogs();
        }

        private void UserFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterLogs();
        }

        private void DateFilter_Changed(object sender, EventArgs e)
        {
            LoadAuditLogs();
            LoadStatistics();
        }

        private async void FilterLogs()
        {
            try
            {
                var searchTerm = searchBox.Text.ToLower();
                var selectedAction = actionFilter.SelectedItem?.ToString();
                var selectedUser = userFilter.SelectedItem?.ToString();

                var fromDate = fromDatePicker.Value.Date;
                var toDate = toDatePicker.Value.Date.AddDays(1).AddSeconds(-1);

                var query = _context.AuditLogs
                    .Include(a => a.User)
                    .Where(a => a.Timestamp >= fromDate && a.Timestamp <= toDate);

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(a => 
                        a.Action.ToLower().Contains(searchTerm) ||
                        (a.NewValues != null && a.NewValues.ToLower().Contains(searchTerm)) ||
                        (a.TableName != null && a.TableName.ToLower().Contains(searchTerm)) ||
                        (a.User != null && (a.User.FirstName + " " + a.User.LastName).ToLower().Contains(searchTerm)));
                }

                if (!string.IsNullOrEmpty(selectedAction) && selectedAction != "All Actions")
                {
                    query = query.Where(a => a.Action == selectedAction);
                }

                if (!string.IsNullOrEmpty(selectedUser) && selectedUser != "All Users")
                {
                    query = query.Where(a => a.User != null && 
                        (a.User.FirstName + " " + a.User.LastName) == selectedUser);
                }

                var logs = await query.OrderByDescending(a => a.Timestamp).Take(500).ToListAsync();

                auditGrid.Rows.Clear();

                foreach (var log in logs)
                {
                    var userName = log.User?.FullName ?? "System";
                    var displayDetails = log.NewValues ?? "No details";
                    
                    if (displayDetails.Length > 80)
                    {
                        displayDetails = displayDetails.Substring(0, 77) + "...";
                    }

                    auditGrid.Rows.Add(
                        log.Id,
                        log.Timestamp,
                        userName,
                        log.Action,
                        log.TableName ?? "System",
                        displayDetails,
                        log.IpAddress ?? "N/A"
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error filtering logs: {ex.Message}");
            }
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            LoadAuditLogs();
            LoadFilters();
            LoadStatistics();
        }

        private async void ExportButton_Click(object sender, EventArgs e)
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv",
                    DefaultExt = "csv",
                    FileName = $"AuditLog_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    exportButton.Enabled = false;

                    var logs = await _context.AuditLogs
                        .Include(a => a.User)
                        .Where(a => a.Timestamp >= fromDatePicker.Value.Date && 
                                   a.Timestamp <= toDatePicker.Value.Date.AddDays(1))
                        .OrderByDescending(a => a.Timestamp)
                        .ToListAsync();

                    using (var writer = new StreamWriter(saveDialog.FileName))
                    {
                        writer.WriteLine("Timestamp,User,Action,Module,Details,IP");

                        foreach (var log in logs)
                        {
                            var userName = log.User?.FullName ?? "System";
                            var details = log.NewValues?.Replace("\"", "\"\"") ?? "";
                            
                            writer.WriteLine($"\"{log.Timestamp:yyyy-MM-dd HH:mm:ss}\",\"{userName}\",\"{log.Action}\",\"{log.TableName ?? ""}\",\"{details}\",\"{log.IpAddress ?? ""}\"");
                        }
                    }

                    MessageBox.Show($"Exported successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Export Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                exportButton.Enabled = true;
            }
        }

        private async void ClearButton_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Delete logs older than 90 days?", 
                "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    clearButton.Enabled = false;

                    var cutoffDate = DateTime.Now.AddDays(-90);
                    var oldLogs = await _context.AuditLogs
                        .Where(a => a.Timestamp < cutoffDate)
                        .ToListAsync();

                    if (oldLogs.Any())
                    {
                        _context.AuditLogs.RemoveRange(oldLogs);
                        await _context.SaveChangesAsync();

                        MessageBox.Show($"Deleted {oldLogs.Count:N0} entries.", "Success", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LoadAuditLogs();
                        LoadStatistics();
                    }
                    else
                    {
                        MessageBox.Show("No old logs found.", "Info", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    clearButton.Enabled = true;
                }
            }
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
