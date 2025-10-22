using System.Drawing;
using System.Drawing.Drawing2D;
using SyncVerseStudio.Services;
using SyncVerseStudio.Data;
using SyncVerseStudio.Models;
using SyncVerseStudio.Helpers;
using Microsoft.EntityFrameworkCore;
using FontAwesome.Sharp;

namespace SyncVerseStudio.Views
{
    public partial class AuditLogView : Form
    {
        private readonly AuthenticationService _authService;
        private readonly ApplicationDbContext _context;
        private DataGridView auditGrid;
        private Panel statsCardsPanel;
        private TextBox searchBox;
        private ComboBox actionFilter, userFilter;
        private DateTimePicker fromDatePicker, toDatePicker;
        
        // Blue theme colors
        private readonly Color BlueLight = Color.FromArgb(158, 236, 255);  // #9EECFF
        private readonly Color BlueMedium = Color.FromArgb(48, 148, 255);  // #3094FF
        private readonly Color BlueDark = Color.FromArgb(30, 120, 220);
        private readonly Color BlueVeryLight = Color.FromArgb(230, 248, 255);

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
            this.BackColor = Color.White;
            this.ClientSize = new Size(1400, 900);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Name = "AuditLogView";
            this.Text = "Audit Log Management";
            this.Padding = new Padding(40, 30, 40, 40);

            CreateHeader();
            CreateStatsCards();
            CreateFiltersPanel();
            CreateAuditGrid();
        }

        private void CreateHeader()
        {
            var headerPanel = new Panel
            {
                Location = new Point(40, 30),
                Size = new Size(1320, 80),
                BackColor = Color.Transparent
            };

            // Modern title with gradient underline
            var titleLabel = new Label
            {
                Text = "System Audit Logs",
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = BlueDark,
                Location = new Point(0, 0),
                Size = new Size(500, 45),
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(titleLabel);

            var subtitleLabel = new Label
            {
                Text = "Monitor all system activities and user actions in real-time",
                Font = new Font("Segoe UI", 11F, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(0, 50),
                Size = new Size(600, 25),
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(subtitleLabel);

            // Action buttons on the right
            var exportBtn = new Button
            {
                Text = "📊 Export",
                Location = new Point(1050, 15),
                Size = new Size(120, 40),
                BackColor = BlueMedium,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            exportBtn.FlatAppearance.BorderSize = 0;
            exportBtn.Click += ExportButton_Click;
            headerPanel.Controls.Add(exportBtn);

            var refreshBtn = new Button
            {
                Text = "🔄 Refresh",
                Location = new Point(1180, 15),
                Size = new Size(120, 40),
                BackColor = Color.FromArgb(107, 114, 128),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            refreshBtn.FlatAppearance.BorderSize = 0;
            refreshBtn.Click += RefreshButton_Click;
            headerPanel.Controls.Add(refreshBtn);

            this.Controls.Add(headerPanel);
        }

        private void CreateStatsCards()
        {
            statsCardsPanel = new FlowLayoutPanel
            {
                Location = new Point(40, 130),
                Size = new Size(1320, 120),
                BackColor = Color.Transparent,
                WrapContents = false,
                AutoScroll = false
            };

            // Card 1: Total Logs
            CreateStatCard("Total Logs", "0", IconChar.Database, Color.FromArgb(99, 102, 241), 0);
            
            // Card 2: Today's Activity
            CreateStatCard("Today's Activity", "0", IconChar.CalendarDay, BlueMedium, 1);
            
            // Card 3: Success Rate
            CreateStatCard("Success Rate", "0%", IconChar.CheckCircle, Color.FromArgb(34, 197, 94), 2);
            
            // Card 4: Failed Actions
            CreateStatCard("Failed Actions", "0", IconChar.ExclamationTriangle, Color.FromArgb(239, 68, 68), 3);

            this.Controls.Add(statsCardsPanel);
        }

        private void CreateStatCard(string title, string value, IconChar icon, Color accentColor, int index)
        {
            var card = new Panel
            {
                Size = new Size(310, 110),
                Margin = new Padding(0, 0, 20, 0),
                BackColor = Color.White,
                Tag = new { Title = title, Value = value }
            };

            // Card paint for rounded corners and shadow
            card.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var rect = card.ClientRectangle;
                
                // Shadow
                using (var shadowPath = GetRoundedRectPath(new Rectangle(2, 2, rect.Width - 2, rect.Height - 2), 12))
                using (var shadowBrush = new SolidBrush(Color.FromArgb(15, 0, 0, 0)))
                {
                    e.Graphics.FillPath(shadowBrush, shadowPath);
                }
                
                // Card background
                using (var path = GetRoundedRectPath(new Rectangle(0, 0, rect.Width - 1, rect.Height - 1), 12))
                using (var brush = new SolidBrush(Color.White))
                using (var pen = new Pen(Color.FromArgb(230, 230, 230), 1))
                {
                    e.Graphics.FillPath(brush, path);
                    e.Graphics.DrawPath(pen, path);
                }
                
                // Accent bar on left
                using (var accentBrush = new SolidBrush(accentColor))
                {
                    e.Graphics.FillRectangle(accentBrush, 0, 15, 4, 80);
                }
            };

            // Icon with circular background
            var iconBg = new Panel
            {
                Size = new Size(50, 50),
                Location = new Point(20, 30),
                BackColor = Color.Transparent
            };
            iconBg.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var brush = new SolidBrush(Color.FromArgb(30, accentColor)))
                {
                    e.Graphics.FillEllipse(brush, 0, 0, 50, 50);
                }
            };
            card.Controls.Add(iconBg);

            var iconPic = new IconPictureBox
            {
                IconChar = icon,
                IconColor = accentColor,
                IconSize = 26,
                Location = new Point(12, 12),
                Size = new Size(26, 26),
                BackColor = Color.Transparent
            };
            iconBg.Controls.Add(iconPic);

            // Title
            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(85, 30),
                Size = new Size(210, 20),
                BackColor = Color.Transparent
            };
            card.Controls.Add(titleLabel);

            // Value
            var valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(85, 48),
                Size = new Size(210, 40),
                BackColor = Color.Transparent,
                Name = title.Replace(" ", "") + "Label"
            };
            card.Controls.Add(valueLabel);

            statsCardsPanel.Controls.Add(card);
        }

        private void CreateFiltersPanel()
        {
            var filtersPanel = new Panel
            {
                Location = new Point(40, 270),
                Size = new Size(1320, 70),
                BackColor = BlueVeryLight
            };

            filtersPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(filtersPanel.ClientRectangle, 12))
                using (var brush = new SolidBrush(BlueVeryLight))
                {
                    e.Graphics.FillPath(brush, path);
                }
            };

            // Search box
            searchBox = new TextBox
            {
                Location = new Point(20, 20),
                Size = new Size(280, 30),
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.FixedSingle,
                PlaceholderText = "🔍 Search logs..."
            };
            searchBox.TextChanged += SearchBox_TextChanged;
            filtersPanel.Controls.Add(searchBox);

            // Action filter
            var actionLabel = new Label
            {
                Text = "Action:",
                Location = new Point(320, 23),
                Size = new Size(60, 25),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = BlueDark
            };
            filtersPanel.Controls.Add(actionLabel);

            actionFilter = new ComboBox
            {
                Location = new Point(380, 20),
                Size = new Size(150, 30),
                Font = new Font("Segoe UI", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            actionFilter.SelectedIndexChanged += ActionFilter_SelectedIndexChanged;
            filtersPanel.Controls.Add(actionFilter);

            // User filter
            var userLabel = new Label
            {
                Text = "User:",
                Location = new Point(550, 23),
                Size = new Size(50, 25),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = BlueDark
            };
            filtersPanel.Controls.Add(userLabel);

            userFilter = new ComboBox
            {
                Location = new Point(600, 20),
                Size = new Size(150, 30),
                Font = new Font("Segoe UI", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            userFilter.SelectedIndexChanged += UserFilter_SelectedIndexChanged;
            filtersPanel.Controls.Add(userFilter);

            // Date range
            var fromLabel = new Label
            {
                Text = "From:",
                Location = new Point(770, 23),
                Size = new Size(50, 25),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = BlueDark
            };
            filtersPanel.Controls.Add(fromLabel);

            fromDatePicker = new DateTimePicker
            {
                Location = new Point(820, 20),
                Size = new Size(180, 30),
                Font = new Font("Segoe UI", 9.5F),
                Format = DateTimePickerFormat.Short
            };
            fromDatePicker.ValueChanged += DateFilter_Changed;
            filtersPanel.Controls.Add(fromDatePicker);

            var toLabel = new Label
            {
                Text = "To:",
                Location = new Point(1020, 23),
                Size = new Size(30, 25),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = BlueDark
            };
            filtersPanel.Controls.Add(toLabel);

            toDatePicker = new DateTimePicker
            {
                Location = new Point(1050, 20),
                Size = new Size(180, 30),
                Font = new Font("Segoe UI", 9.5F),
                Format = DateTimePickerFormat.Short
            };
            toDatePicker.ValueChanged += DateFilter_Changed;
            filtersPanel.Controls.Add(toDatePicker);

            // Clear filters button
            var clearBtn = new Button
            {
                Text = "✕ Clear",
                Location = new Point(1245, 18),
                Size = new Size(60, 34),
                BackColor = Color.FromArgb(239, 68, 68),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            clearBtn.FlatAppearance.BorderSize = 0;
            clearBtn.Click += ClearButton_Click;
            filtersPanel.Controls.Add(clearBtn);

            this.Controls.Add(filtersPanel);
        }

        private void CreateAuditGrid()
        {
            var gridContainer = new Panel
            {
                Location = new Point(40, 360),
                Size = new Size(1320, 500),
                BackColor = Color.White
            };

            gridContainer.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectPath(gridContainer.ClientRectangle, 12))
                using (var brush = new SolidBrush(Color.White))
                using (var pen = new Pen(Color.FromArgb(230, 230, 230), 1))
                {
                    e.Graphics.FillPath(brush, path);
                    e.Graphics.DrawPath(pen, path);
                }
            };

            auditGrid = new DataGridView
            {
                Location = new Point(15, 15),
                Size = new Size(1290, 470),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                MultiSelect = false,
                AutoGenerateColumns = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                EnableHeadersVisualStyles = false,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.FromArgb(240, 240, 240),
                Font = new Font("Segoe UI", 10F)
            };

            // Modern blue-themed styling
            auditGrid.DefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Segoe UI", 10F),
                Padding = new Padding(12, 10, 12, 10),
                SelectionBackColor = Color.FromArgb(200, 230, 255),
                SelectionForeColor = BlueDark,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(50, 50, 50),
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };

            auditGrid.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(248, 252, 255),
                SelectionBackColor = Color.FromArgb(200, 230, 255),
                SelectionForeColor = BlueDark
            };

            // Blue gradient header
            auditGrid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = BlueMedium,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                Padding = new Padding(12, 12, 12, 12),
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                SelectionBackColor = BlueMedium,
                SelectionForeColor = Color.White
            };

            auditGrid.ColumnHeadersHeight = 50;
            auditGrid.RowTemplate.Height = 55;

            // Hover effect
            auditGrid.CellMouseEnter += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    auditGrid.Rows[e.RowIndex].DefaultCellStyle.BackColor = BlueVeryLight;
                }
            };

            auditGrid.CellMouseLeave += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    auditGrid.Rows[e.RowIndex].DefaultCellStyle.BackColor = 
                        e.RowIndex % 2 == 0 ? Color.White : Color.FromArgb(248, 252, 255);
                }
            };

            // Setup columns
            auditGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Timestamp",
                HeaderText = "⏰ Timestamp",
                Width = 180,
                DataPropertyName = "Timestamp"
            });

            auditGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "User",
                HeaderText = "👤 User",
                Width = 150,
                DataPropertyName = "User"
            });

            auditGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Action",
                HeaderText = "⚡ Action",
                Width = 180,
                DataPropertyName = "Action"
            });

            auditGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Table",
                HeaderText = "📊 Table",
                Width = 120,
                DataPropertyName = "Table"
            });

            auditGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Details",
                HeaderText = "📝 Details",
                Width = 250,
                DataPropertyName = "Details"
            });

            auditGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "IPAddress",
                HeaderText = "🌐 IP Address",
                Width = 140,
                DataPropertyName = "IPAddress"
            });

            gridContainer.Controls.Add(auditGrid);
            this.Controls.Add(gridContainer);
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

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void ActionFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void UserFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void DateFilter_Changed(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            LoadAuditLogs();
            LoadStatistics();
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            using (var exportDialog = new Form())
            {
                exportDialog.Text = "Export Audit Logs";
                exportDialog.Size = new Size(400, 200);
                exportDialog.StartPosition = FormStartPosition.CenterParent;
                exportDialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                exportDialog.MaximizeBox = false;
                exportDialog.MinimizeBox = false;

                var label = new Label
                {
                    Text = "Select export format:",
                    Location = new Point(30, 30),
                    Size = new Size(340, 25),
                    Font = new Font("Segoe UI", 11F, FontStyle.Bold)
                };
                exportDialog.Controls.Add(label);

                var pdfBtn = new Button
                {
                    Text = "📄 Export as PDF",
                    Location = new Point(30, 70),
                    Size = new Size(150, 45),
                    BackColor = Color.FromArgb(220, 38, 38),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                pdfBtn.FlatAppearance.BorderSize = 0;
                pdfBtn.Click += (s, ev) =>
                {
                    exportDialog.DialogResult = DialogResult.OK;
                    exportDialog.Tag = "PDF";
                    exportDialog.Close();
                };
                exportDialog.Controls.Add(pdfBtn);

                var excelBtn = new Button
                {
                    Text = "📊 Export as Excel",
                    Location = new Point(200, 70),
                    Size = new Size(150, 45),
                    BackColor = Color.FromArgb(34, 197, 94),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                excelBtn.FlatAppearance.BorderSize = 0;
                excelBtn.Click += (s, ev) =>
                {
                    exportDialog.DialogResult = DialogResult.OK;
                    exportDialog.Tag = "EXCEL";
                    exportDialog.Close();
                };
                exportDialog.Controls.Add(excelBtn);

                if (exportDialog.ShowDialog() == DialogResult.OK)
                {
                    string format = exportDialog.Tag?.ToString() ?? "EXCEL";
                    if (format == "PDF")
                        ExportToPDF();
                    else
                        ExportToExcel();
                }
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            searchBox.Clear();
            actionFilter.SelectedIndex = 0;
            userFilter.SelectedIndex = 0;
            fromDatePicker.Value = DateTime.Now.AddDays(-30);
            toDatePicker.Value = DateTime.Now;
            ApplyFilters();
        }

        private void LoadFilters()
        {
            // Load action types
            actionFilter.Items.Add("All Actions");
            actionFilter.Items.Add("LOGIN_SUCCESS");
            actionFilter.Items.Add("LOGIN_FAILED");
            actionFilter.Items.Add("LOGOUT");
            actionFilter.Items.Add("CREATE");
            actionFilter.Items.Add("UPDATE");
            actionFilter.Items.Add("DELETE");
            actionFilter.SelectedIndex = 0;

            // Load users
            userFilter.Items.Add("All Users");
            var users = _context.Users.Select(u => u.Username).Distinct().ToList();
            foreach (var user in users)
            {
                userFilter.Items.Add(user);
            }
            userFilter.SelectedIndex = 0;

            // Set default date range
            fromDatePicker.Value = DateTime.Now.AddDays(-30);
            toDatePicker.Value = DateTime.Now;
        }

        private void LoadAuditLogs()
        {
            try
            {
                var logs = _context.AuditLogs
                    .Include(a => a.User)
                    .OrderByDescending(a => a.Timestamp)
                    .Take(1000)
                    .Select(a => new
                    {
                        Timestamp = a.Timestamp.ToString("MM/dd/yyyy HH:mm:ss"),
                        User = a.User != null ? a.User.Username : "System",
                        Action = a.Action,
                        Table = a.TableName,
                        Details = (a.OldValues ?? "") + " → " + (a.NewValues ?? ""),
                        IPAddress = a.IpAddress ?? "N/A"
                    })
                    .ToList();

                auditGrid.DataSource = logs;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading audit logs: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadStatistics()
        {
            try
            {
                var totalLogs = _context.AuditLogs.Count();
                var todayLogs = _context.AuditLogs.Count(a => a.Timestamp.Date == DateTime.Today);
                var successLogs = _context.AuditLogs.Count(a => a.Action.Contains("SUCCESS"));
                var failedLogs = _context.AuditLogs.Count(a => a.Action.Contains("FAILED"));
                
                var successRate = totalLogs > 0 ? (successLogs * 100.0 / totalLogs) : 0;

                // Update stat cards
                UpdateStatCard("TotalLogsLabel", totalLogs.ToString("N0"));
                UpdateStatCard("Today'sActivityLabel", todayLogs.ToString("N0"));
                UpdateStatCard("SuccessRateLabel", $"{successRate:F1}%");
                UpdateStatCard("FailedActionsLabel", failedLogs.ToString("N0"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading statistics: {ex.Message}");
            }
        }

        private void UpdateStatCard(string labelName, string value)
        {
            var label = statsCardsPanel.Controls.Find(labelName, true).FirstOrDefault() as Label;
            if (label != null)
            {
                label.Text = value;
            }
        }

        private void ApplyFilters()
        {
            try
            {
                var query = _context.AuditLogs.Include(a => a.User).AsQueryable();

                // Search filter
                if (!string.IsNullOrWhiteSpace(searchBox.Text))
                {
                    var searchTerm = searchBox.Text.ToLower();
                    query = query.Where(a =>
                        a.Action.ToLower().Contains(searchTerm) ||
                        a.TableName.ToLower().Contains(searchTerm) ||
                        (a.OldValues != null && a.OldValues.ToLower().Contains(searchTerm)) ||
                        (a.NewValues != null && a.NewValues.ToLower().Contains(searchTerm)) ||
                        (a.User != null && a.User.Username.ToLower().Contains(searchTerm)));
                }

                // Action filter
                if (actionFilter.SelectedIndex > 0)
                {
                    var selectedAction = actionFilter.SelectedItem.ToString();
                    query = query.Where(a => a.Action == selectedAction);
                }

                // User filter
                if (userFilter.SelectedIndex > 0)
                {
                    var selectedUser = userFilter.SelectedItem.ToString();
                    query = query.Where(a => a.User != null && a.User.Username == selectedUser);
                }

                // Date range filter
                query = query.Where(a => a.Timestamp >= fromDatePicker.Value.Date &&
                                        a.Timestamp <= toDatePicker.Value.Date.AddDays(1));

                var logs = query
                    .OrderByDescending(a => a.Timestamp)
                    .Take(1000)
                    .Select(a => new
                    {
                        Timestamp = a.Timestamp.ToString("MM/dd/yyyy HH:mm:ss"),
                        User = a.User != null ? a.User.Username : "System",
                        Action = a.Action,
                        Table = a.TableName,
                        Details = (a.OldValues ?? "") + " → " + (a.NewValues ?? ""),
                        IPAddress = a.IpAddress ?? "N/A"
                    })
                    .ToList();

                auditGrid.DataSource = logs;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying filters: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportToExcel()
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "CSV Files (*.csv)|*.csv",
                    FileName = $"AuditLog_{DateTime.Now:yyyyMMdd_HHmmss}.csv",
                    Title = "Export Audit Logs to Excel"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    var logs = _context.AuditLogs
                        .Include(a => a.User)
                        .OrderByDescending(a => a.Timestamp)
                        .ToList();

                    using (var writer = new System.IO.StreamWriter(saveDialog.FileName))
                    {
                        // Write header with full user information
                        writer.WriteLine("Timestamp,User,Role,Email,Action,Table,Details,IP Address,Old Values,New Values");

                        // Write data
                        foreach (var log in logs)
                        {
                            var timestamp = log.Timestamp.ToString("yyyy-MM-dd HH:mm:ss");
                            var username = log.User?.Username ?? "System";
                            var role = log.User != null ? log.User.Role.ToString() : "N/A";
                            var email = log.User?.Email ?? "N/A";
                            var action = log.Action;
                            var table = log.TableName ?? "N/A";
                            var details = EscapeCsv((log.OldValues ?? "") + " → " + (log.NewValues ?? ""));
                            var ipAddress = log.IpAddress ?? "N/A";
                            var oldValues = EscapeCsv(log.OldValues ?? "N/A");
                            var newValues = EscapeCsv(log.NewValues ?? "N/A");

                            writer.WriteLine($"{timestamp},{username},{role},{email},{action},{table},{details},{ipAddress},{oldValues},{newValues}");
                        }
                    }

                    MessageBox.Show($"Audit logs exported successfully to:\n{saveDialog.FileName}", 
                        "Export Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting to Excel: {ex.Message}", "Export Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportToPDF()
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "Text Files (*.txt)|*.txt",
                    FileName = $"AuditLog_{DateTime.Now:yyyyMMdd_HHmmss}.txt",
                    Title = "Export Audit Logs to PDF (Text Format)"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    var logs = _context.AuditLogs
                        .Include(a => a.User)
                        .OrderByDescending(a => a.Timestamp)
                        .ToList();

                    using (var writer = new System.IO.StreamWriter(saveDialog.FileName))
                    {
                        writer.WriteLine("═══════════════════════════════════════════════════════════════════════");
                        writer.WriteLine("                    SYSTEM AUDIT LOG REPORT");
                        writer.WriteLine($"                Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                        writer.WriteLine("═══════════════════════════════════════════════════════════════════════");
                        writer.WriteLine();
                        writer.WriteLine($"Total Records: {logs.Count}");
                        writer.WriteLine();

                        foreach (var log in logs)
                        {
                            writer.WriteLine("───────────────────────────────────────────────────────────────────────");
                            writer.WriteLine($"Timestamp:    {log.Timestamp:yyyy-MM-dd HH:mm:ss}");
                            writer.WriteLine($"User:         {log.User?.Username ?? "System"}");
                            writer.WriteLine($"Role:         {(log.User != null ? log.User.Role.ToString() : "N/A")}");
                            writer.WriteLine($"Email:        {log.User?.Email ?? "N/A"}");
                            writer.WriteLine($"Action:       {log.Action}");
                            writer.WriteLine($"Table:        {log.TableName ?? "N/A"}");
                            writer.WriteLine($"IP Address:   {log.IpAddress ?? "N/A"}");
                            
                            if (!string.IsNullOrEmpty(log.OldValues))
                            {
                                writer.WriteLine($"Old Values:   {log.OldValues}");
                            }
                            
                            if (!string.IsNullOrEmpty(log.NewValues))
                            {
                                writer.WriteLine($"New Values:   {log.NewValues}");
                            }
                            
                            writer.WriteLine();
                        }

                        writer.WriteLine("═══════════════════════════════════════════════════════════════════════");
                        writer.WriteLine("                         END OF REPORT");
                        writer.WriteLine("═══════════════════════════════════════════════════════════════════════");
                    }

                    MessageBox.Show($"Audit logs exported successfully to:\n{saveDialog.FileName}\n\nNote: For true PDF format, please use a PDF library.", 
                        "Export Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting to PDF: {ex.Message}", "Export Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            }

            return value;
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
