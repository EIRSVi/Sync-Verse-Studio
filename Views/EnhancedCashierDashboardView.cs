using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Microsoft.EntityFrameworkCore;
using SyncVerseStudio.Data;
using SyncVerseStudio.Models;
using SyncVerseStudio.Services;
using FontAwesome.Sharp;

namespace SyncVerseStudio.Views
{
    public partial class EnhancedCashierDashboardView : Form
    {
        private readonly AuthenticationService _authService;
        private readonly ApplicationDbContext _context;
        private System.Windows.Forms.Timer _refreshTimer;

        public EnhancedCashierDashboardView(AuthenticationService authService)
        {
            _authService = authService;
            _context = new ApplicationDbContext();
            InitializeComponent();
            LoadDashboardData();
            StartAutoRefresh();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(248, 250, 252);
            this.Size = new Size(1400, 900);

            CreateDashboardLayout();

            this.ResumeLayout(false);
        }

        private void CreateDashboardLayout()
        {
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 250, 252),
                Padding = new Padding(30)
            };

            // Header with SYNCVERSE branding
            var headerPanel = CreateHeaderPanel();
            headerPanel.Location = new Point(0, 0);
            headerPanel.Width = mainPanel.Width - 60;

            // Metrics Cards Row
            var metricsPanel = CreateMetricsPanel();
            metricsPanel.Location = new Point(0, 80);
            metricsPanel.Width = mainPanel.Width - 60;

            // Statistics Section
            var statsPanel = CreateStatisticsPanel();
            statsPanel.Location = new Point(0, 200);
            statsPanel.Size = new Size((mainPanel.Width - 80) * 2 / 3, 350);

            // Account Summary Panel (Right side)
            var summaryPanel = CreateAccountSummaryPanel();
            summaryPanel.Location = new Point(statsPanel.Width + 20, 200);
            summaryPanel.Size = new Size((mainPanel.Width - 80) / 3, 350);

            // Latest Invoices Section
     eight = 60,
                BackColor = Color.Transparent
            };

            var brandLabel = new Label
            {
                Text = "SYNCVERSE",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 184, 166),
                Location = new Point(0, 0),
                AutoSize = true
            };

            var urlLabel = new Label
            {
                Text = "https://syncverse.com",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(0, 30),
                AutoSize = true
            };

            var dateLabel = new Label
            {
                Text = DateTime.Now.ToString("MMMM dd, yyyy"),
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.FromArgb(71, 85, 105),
                AutoSize = true
            };
            dateLabel.Location = new Point(panel.Width - dateLabel.Width - 20, 15);

            panel.Controls.AddRange(new Control[] { brandLabel, urlLabel, dateLabel });

            return panel;
        }

        private Panel CreateMetricsPanel()
        {
            var panel = new FlowLayoutPanel
            {
                Height = 100,
                BackColor = Color.Transparent,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false
            };

            var invoiceCountCard = CreateMetricCard("Invoices count", "0 Invoice", IconChar.FileInvoice, Color.FromArgb(59, 130, 246));
            var paidInvoicesCard = CreateMetricCard("Total paid invoices", "0 KHR", IconChar.MoneyBillWave, Color.FromArgb(34, 197, 94));

            panel.Controls.AddRange(new Control[] { invoiceCountCard, paidInvoicesCard });

            return panel;
        }

        private Panel CreateMetricCard(string title, string value, IconChar icon, Color iconColor)
        {
            var card = new Panel
            {
                Size = new Size(300, 100),
                BackColor = Color.White,
                Margin = new Padding(0, 0, 20, 0)
            };

            card.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
                }
            };

            var iconBox = new IconPictureBox
            {
                IconChar = icon,
                IconColor = iconColor,
                IconSize = 32,
                Location = new Point(20, 25),
                Size = new Size(32, 32)
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(70, 20),
                AutoSize = true
            };

            var valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(70, 45),
                AutoSize = true,
                Tag = title // Store title for updating
            };

            card.Controls.AddRange(new Control[] { iconBox, titleLabel, valueLabel });

            return card;
        }

        private Panel CreateStatisticsPanel()
        {
            var panel = new Panel
            {
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            panel.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
                }
            };

            var titleLabel = new Label
            {
                Text = "Statistics",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(20, 15),
                AutoSize = true
            };

            var dateRangeLabel = new Label
            {
                Text = "Last 7 days",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true
            };
            dateRangeLabel.Location = new Point(panel.Width - dateRangeLabel.Width - 40, 20);

            // Line Chart
            var lineChart = CreateLineChart();
            lineChart.Location = new Point(20, 60);
            lineChart.Size = new Size(panel.Width - 280, 260);

            // Donut Chart
            var donutChart = CreateDonutChart();
            donutChart.Location = new Point(panel.Width - 240, 60);
            donutChart.Size = new Size(220, 260);

            panel.Controls.AddRange(new Control[] { titleLabel, dateRangeLabel, lineChart, donutChart });

            return panel;
        }

        private Chart CreateLineChart()
        {
            var chart = new Chart
            {
                BackColor = Color.White,
                BorderlineColor = Color.Transparent
            };

            var chartArea = new ChartArea
            {
                BackColor = Color.White,
                BorderWidth = 0
            };
            chartArea.AxisX.MajorGrid.LineColor = Color.FromArgb(226, 232, 240);
            chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(226, 232, 240);
            chartArea.AxisX.LabelStyle.ForeColor = Color.FromArgb(100, 116, 139);
            chartArea.AxisY.LabelStyle.ForeColor = Color.FromArgb(100, 116, 139);
            chart.ChartAreas.Add(chartArea);

            // Active Series
            var activeSeries = new Series
            {
                Name = "Active",
                ChartType = SeriesChartType.Line,
                Color = Color.FromArgb(59, 130, 246),
                BorderWidth = 3,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 6
            };

            // Paid Series
            var paidSeries = new Series
            {
                Name = "Paid",
                ChartType = SeriesChartType.Line,
                Color = Color.FromArgb(34, 197, 94),
                BorderWidth = 3,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 6
            };

            // Void Series
            var voidSeries = new Series
            {
                Name = "Void",
                ChartType = SeriesChartType.Line,
                Color = Color.FromArgb(239, 68, 68),
                BorderWidth = 3,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 6
            };

            chart.Series.Add(activeSeries);
            chart.Series.Add(paidSeries);
            chart.Series.Add(voidSeries);

            var legend = new Legend
            {
                Docking = Docking.Bottom,
                Alignment = StringAlignment.Center,
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 9F)
            };
            chart.Legends.Add(legend);

            return chart;
        }

        private Chart CreateDonutChart()
        {
            var chart = new Chart
            {
                BackColor = Color.White,
                BorderlineColor = Color.Transparent
            };

            var chartArea = new ChartArea
            {
                BackColor = Color.White,
                BorderWidth = 0
            };
            chart.ChartAreas.Add(chartArea);

            var series = new Series
            {
                Name = "InvoiceStatus",
                ChartType = SeriesChartType.Doughnut,
                Font = new Font("Segoe UI", 9F)
            };

            series.Points.AddXY("Active", 0);
            series.Points.AddXY("Paid", 0);
            series.Points.AddXY("Void", 0);

            series.Points[0].Color = Color.FromArgb(59, 130, 246);
            series.Points[1].Color = Color.FromArgb(34, 197, 94);
            series.Points[2].Color = Color.FromArgb(239, 68, 68);

            series["PieLabelStyle"] = "Outside";
            series["DoughnutRadius"] = "60";

            chart.Series.Add(series);

            var legend = new Legend
            {
                Docking = Docking.Bottom,
                Alignment = StringAlignment.Center,
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 9F)
            };
            chart.Legends.Add(legend);

            return chart;
        }

        private Panel CreateAccountSummaryPanel()
        {
            var panel = new Panel
            {
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            panel.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
                }
            };

            var titleLabel = new Label
            {
                Text = "Account Summary",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(20, 15),
                AutoSize = true
            };

            int yPos = 60;
            var metrics = new[]
            {
                ("Total active invoices", "0 KHR", IconChar.FileInvoice),
                ("Repeated invoices", "0", IconChar.Redo),
                ("Payment links", "0", IconChar.Link),
                ("Store sales", "0", IconChar.Store),
                ("Products", "0", IconChar.Box)
            };

            foreach (var (label, value, icon) in metrics)
            {
                var metricPanel = CreateSummaryMetric(label, value, icon);
                metricPanel.Location = new Point(20, yPos);
                metricPanel.Width = panel.Width - 40;
                panel.Controls.Add(metricPanel);
                yPos += 50;
            }

            panel.Controls.Add(titleLabel);

            return panel;
        }

        private Panel CreateSummaryMetric(string label, string value, IconChar icon)
        {
            var panel = new Panel
            {
                Height = 40,
                BackColor = Color.Transparent
            };

            var iconBox = new IconPictureBox
            {
                IconChar = icon,
                IconColor = Color.FromArgb(100, 116, 139),
                IconSize = 20,
                Location = new Point(0, 10),
                Size = new Size(20, 20)
            };

            var labelText = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(30, 10),
                AutoSize = true
            };

            var valueText = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Tag = label // For updating
            };
            valueText.Location = new Point(panel.Width - valueText.Width - 10, 10);

            panel.Controls.AddRange(new Control[] { iconBox, labelText, valueText });

            return panel;
        }

        private Panel CreateLatestInvoicesPanel()
        {
            var panel = new Panel
            {
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            panel.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
                }
            };

            var titleLabel = new Label
            {
                Text = "Latest invoices",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(20, 15),
                AutoSize = true
            };

            var dataGrid = new DataGridView
            {
                Location = new Point(20, 60),
                Size = new Size(panel.Width - 40, panel.Height - 80),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 10F),
                ColumnHeadersHeight = 40,
                RowTemplate = { Height = 45 }
            };

            dataGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            dataGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(71, 85, 105);
            dataGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dataGrid.EnableHeadersVisualStyles = false;

            dataGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "InvoiceNumber",
                HeaderText = "Invoice Number",
                Width = 150,
                DataPropertyName = "InvoiceNumber"
            });

            dataGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ClientName",
                HeaderText = "Client Name",
                Width = 200,
                DataPropertyName = "CustomerName"
            });

            dataGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Status",
                HeaderText = "Status",
                Width = 120,
                DataPropertyName = "Status"
            });

            dataGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Amount",
                HeaderText = "Amount",
                Width = 150,
                DataPropertyName = "TotalAmount"
            });

            dataGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Date",
                HeaderText = "Date",
                Width = 180,
                DataPropertyName = "InvoiceDate"
            });

            dataGrid.CellFormatting += InvoiceGrid_CellFormatting;

            panel.Controls.AddRange(new Control[] { titleLabel, dataGrid });

            return panel;
        }

        private void InvoiceGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var grid = (DataGridView)sender;
            
            if (grid.Columns[e.ColumnIndex].Name == "Status" && e.Value != null)
            {
                var status = e.Value.ToString();
                switch (status)
                {
                    case "Active":
                        e.CellStyle.ForeColor = Color.FromArgb(59, 130, 246);
                        e.CellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                        break;
                    case "Paid":
                        e.CellStyle.ForeColor = Color.FromArgb(34, 197, 94);
                        e.CellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                        break;
                    case "Void":
                        e.CellStyle.ForeColor = Color.FromArgb(239, 68, 68);
                        e.CellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                        break;
                }
            }

            if (grid.Columns[e.ColumnIndex].Name == "Amount" && e.Value != null)
            {
                e.Value = $"{e.Value} KHR";
                e.FormattingApplied = true;
            }

            if (grid.Columns[e.ColumnIndex].Name == "Date" && e.Value != null)
            {
                if (e.Value is DateTime date)
                {
                    e.Value = date.ToString("MMMM dd, yyyy");
                    e.FormattingApplied = true;
                }
            }
        }

        private async void LoadDashboardData()
        {
            try
            {
                // Load invoice counts
                var activeInvoices = await _context.Invoices.CountAsync(i => i.Status == InvoiceStatus.Active);
                var paidInvoices = await _context.Invoices.Where(i => i.Status == InvoiceStatus.Paid).SumAsync(i => i.TotalAmount);
                
                // Update metric cards
                UpdateMetricCard("Invoices count", $"{activeInvoices} Invoice{(activeInvoices != 1 ? "s" : "")}");
                UpdateMetricCard("Total paid invoices", $"{paidInvoices:N0} KHR");

                // Load chart data
                await LoadChartData();

                // Load account summary
                await LoadAccountSummary();

                // Load latest invoices
                await LoadLatestInvoices();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading dashboard data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateMetricCard(string title, string value)
        {
            foreach (Control control in this.Controls)
            {
                if (control is Panel mainPanel)
                {
                    foreach (Control child in mainPanel.Controls)
                    {
                        if (child is FlowLayoutPanel metricsPanel)
                        {
                            foreach (Control card in metricsPanel.Controls)
                            {
                                if (card is Panel cardPanel)
                                {
                                    foreach (Control cardChild in cardPanel.Controls)
                                    {
                                        if (cardChild is Label label && label.Tag?.ToString() == title)
                                        {
                                            label.Text = value;
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private async Task LoadChartData()
        {
            var endDate = DateTime.Now.Date;
            var startDate = endDate.AddDays(-6);

            var invoiceData = await _context.Invoices
                .Where(i => i.InvoiceDate >= startDate && i.InvoiceDate <= endDate)
                .GroupBy(i => new { Date = i.InvoiceDate.Date, i.Status })
                .Select(g => new { g.Key.Date, g.Key.Status, Count = g.Count() })
                .ToListAsync();

            // Find charts and update
            foreach (Control control in this.Controls)
            {
                if (control is Panel mainPanel)
                {
                    foreach (Control child in mainPanel.Controls)
                    {
                        if (child is Panel statsPanel && statsPanel.Controls.Count > 2)
                        {
                            foreach (Control statsChild in statsPanel.Controls)
                            {
                                if (statsChild is Chart chart)
                                {
                                    if (chart.Series.Count == 3) // Line chart
                                    {
                                        UpdateLineChart(chart, invoiceData, startDate, endDate);
                                    }
                                    else if (chart.Series.Count == 1) // Donut chart
                                    {
                                        UpdateDonutChart(chart, invoiceData);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void UpdateLineChart(Chart chart, dynamic invoiceData, DateTime startDate, DateTime endDate)
        {
            chart.Series["Active"].Points.Clear();
            chart.Series["Paid"].Points.Clear();
            chart.Series["Void"].Points.Clear();

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                var dateStr = date.ToString("MMM dd");
                
                var activeCount = invoiceData.Where(d => d.Date == date && d.Status == InvoiceStatus.Active).Sum(d => d.Count);
                var paidCount = invoiceData.Where(d => d.Date == date && d.Status == InvoiceStatus.Paid).Sum(d => d.Count);
                var voidCount = invoiceData.Where(d => d.Date == date && d.Status == InvoiceStatus.Void).Sum(d => d.Count);

                chart.Series["Active"].Points.AddXY(dateStr, activeCount);
                chart.Series["Paid"].Points.AddXY(dateStr, paidCount);
                chart.Series["Void"].Points.AddXY(dateStr, voidCount);
            }
        }

        private void UpdateDonutChart(Chart chart, dynamic invoiceData)
        {
            var activeCount = invoiceData.Where(d => d.Status == InvoiceStatus.Active).Sum(d => d.Count);
            var paidCount = invoiceData.Where(d => d.Status == InvoiceStatus.Paid).Sum(d => d.Count);
            var voidCount = invoiceData.Where(d => d.Status == InvoiceStatus.Void).Sum(d => d.Count);

            chart.Series[0].Points[0].SetValueY(activeCount);
            chart.Series[0].Points[1].SetValueY(paidCount);
            chart.Series[0].Points[2].SetValueY(voidCount);
        }

        private async Task LoadAccountSummary()
        {
            var activeInvoicesTotal = await _context.Invoices
                .Where(i => i.Status == InvoiceStatus.Active)
                .SumAsync(i => i.TotalAmount);

            var productCount = await _context.Products.CountAsync(p => p.IsActive);

            // Update summary values (simplified for now)
            UpdateSummaryMetric("Total active invoices", $"{activeInvoicesTotal:N0} KHR");
            UpdateSummaryMetric("Products", productCount.ToString());
        }

        private void UpdateSummaryMetric(string label, string value)
        {
            foreach (Control control in this.Controls)
            {
                if (control is Panel mainPanel)
                {
                    foreach (Control child in mainPanel.Controls)
                    {
                        if (child is Panel summaryPanel && summaryPanel.Padding.All == 20)
                        {
                            foreach (Control summaryChild in summaryPanel.Controls)
                            {
                                if (summaryChild is Panel metricPanel)
                                {
                                    foreach (Control metricChild in metricPanel.Controls)
                                    {
                                        if (metricChild is Label valueLabel && valueLabel.Tag?.ToString() == label)
                                        {
                                            valueLabel.Text = value;
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private async Task LoadLatestInvoices()
        {
            var invoices = await _context.Invoices
                .Include(i => i.Customer)
                .OrderByDescending(i => i.InvoiceDate)
                .Take(5)
                .Select(i => new
                {
                    i.InvoiceNumber,
                    CustomerName = i.Customer != null ? i.Customer.FullName : i.CustomerName ?? "Walk-in Client",
                    Status = i.Status.ToString(),
                    i.TotalAmount,
                    i.InvoiceDate
                })
                .ToListAsync();

            // Find and update DataGridView
            foreach (Control control in this.Controls)
            {
                if (control is Panel mainPanel)
                {
                    foreach (Control child in mainPanel.Controls)
                    {
                        if (child is Panel invoicesPanel)
                        {
                            foreach (Control invoiceChild in invoicesPanel.Controls)
                            {
                                if (invoiceChild is DataGridView grid)
                                {
                                    grid.DataSource = invoices;
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void StartAutoRefresh()
        {
            _refreshTimer = new System.Windows.Forms.Timer
            {
                Interval = 5000 // 5 seconds
            };
            _refreshTimer.Tick += async (s, e) => await RefreshData();
            _refreshTimer.Start();
        }

        private async Task RefreshData()
        {
            try
            {
                await LoadDashboardData();
            }
            catch
            {
                // Silently fail on refresh errors
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _refreshTimer?.Stop();
                _refreshTimer?.Dispose();
                _context?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
