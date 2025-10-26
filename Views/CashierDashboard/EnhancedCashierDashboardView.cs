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

namespace SyncVerseStudio.Views.CashierDashboard
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
            this.Dock = DockStyle.Fill;

            CreateDashboardLayout();

            this.ResumeLayout(false);
        }

        private void CreateDashboardLayout()
        {
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(30)
            };

            // Header with SYNCVERSE branding
            var headerPanel = CreateHeaderPanel();
            headerPanel.Location = new Point(30, 20);
            mainPanel.Controls.Add(headerPanel);

            // Metric Cards Row
            var metricsPanel = CreateMetricsPanel();
            metricsPanel.Location = new Point(30, 100);
            mainPanel.Controls.Add(metricsPanel);

            // Statistics Section
            var statsPanel = CreateStatisticsPanel();
            statsPanel.Location = new Point(30, 220);
            mainPanel.Controls.Add(statsPanel);

            // Latest Invoices and Account Summary
            var bottomPanel = CreateBottomPanel();
            bottomPanel.Location = new Point(30, 580);
            mainPanel.Controls.Add(bottomPanel);

            this.Controls.Add(mainPanel);
        }

        private Panel CreateHeaderPanel()
        {
            var panel = new Panel
            {
                Size = new Size(1340, 60),
                BackColor = Color.White
            };

            var syncLabel = new Label
            {
                Text = "SYNCVERSE",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 184, 166),
                Location = new Point(20, 15),
                AutoSize = true
            };

            var urlLabel = new Label
            {
                Text = "https://syncverse.studio/sync",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(180, 22),
                AutoSize = true
            };

            var dateLabel = new Label
            {
                Text = DateTime.Now.ToString("MMM dd, yyyy, HH:mm"),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true
            };
            dateLabel.Location = new Point(1340 - dateLabel.Width - 20, 22);

            panel.Controls.AddRange(new Control[] { syncLabel, urlLabel, dateLabel });
            return panel;
        }

        private Panel CreateMetricsPanel()
        {
            var panel = new Panel
            {
                Size = new Size(1340, 100),
                BackColor = Color.Transparent
            };

            var invoiceCountCard = CreateMetricCard("Invoices count", "3 Invoice", IconChar.FileInvoice, Color.FromArgb(59, 130, 246));
            invoiceCountCard.Location = new Point(0, 0);

            var paidInvoicesCard = CreateMetricCard("Total paid invoices", "401 KHR", IconChar.MoneyBillWave, Color.FromArgb(34, 197, 94));
            paidInvoicesCard.Location = new Point(680, 0);

            panel.Controls.AddRange(new Control[] { invoiceCountCard, paidInvoicesCard });
            return panel;
        }

        private Panel CreateMetricCard(string title, string value, IconChar icon, Color iconColor)
        {
            var card = new Panel
            {
                Size = new Size(650, 100),
                BackColor = Color.White
            };

            var iconBox = new IconPictureBox
            {
                IconChar = icon,
                IconColor = iconColor,
                IconSize = 40,
                Location = new Point(20, 30),
                Size = new Size(40, 40)
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(80, 25),
                AutoSize = true
            };

            var valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(80, 45),
                AutoSize = true
            };

            card.Controls.AddRange(new Control[] { iconBox, titleLabel, valueLabel });
            return card;
        }

        private Panel CreateStatisticsPanel()
        {
            var panel = new Panel
            {
                Size = new Size(1340, 340),
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            var titleLabel = new Label
            {
                Text = "Statistics",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(20, 15),
                AutoSize = true
            };

            var dateRangeCombo = new ComboBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(1180, 15),
                Size = new Size(140, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            dateRangeCombo.Items.AddRange(new[] { "Last 7 days", "Last 30 days", "Custom" });
            dateRangeCombo.SelectedIndex = 0;

            // Line Chart
            var lineChart = CreateLineChart();
            lineChart.Location = new Point(20, 60);
            lineChart.Size = new Size(800, 260);

            // Donut Chart
            var donutChart = CreateDonutChart();
            donutChart.Location = new Point(840, 60);
            donutChart.Size = new Size(480, 260);

            panel.Controls.AddRange(new Control[] { titleLabel, dateRangeCombo, lineChart, donutChart });
            return panel;
        }

        private Chart CreateLineChart()
        {
            var chart = new Chart
            {
                BackColor = Color.White
            };

            var chartArea = new ChartArea
            {
                BackColor = Color.White,
                AxisX = { MajorGrid = { LineColor = Color.FromArgb(226, 232, 240) } },
                AxisY = { MajorGrid = { LineColor = Color.FromArgb(226, 232, 240) } }
            };
            chart.ChartAreas.Add(chartArea);

            var activeSeries = new Series
            {
                Name = "Active",
                ChartType = SeriesChartType.Line,
                Color = Color.FromArgb(59, 130, 246),
                BorderWidth = 3
            };

            var paidSeries = new Series
            {
                Name = "Paid",
                ChartType = SeriesChartType.Line,
                Color = Color.FromArgb(34, 197, 94),
                BorderWidth = 3
            };

            var voidSeries = new Series
            {
                Name = "Void",
                ChartType = SeriesChartType.Line,
                Color = Color.FromArgb(239, 68, 68),
                BorderWidth = 3
            };

            // Sample data
            string[] dates = { "Oct 19", "Oct 20", "Oct 21", "Oct 22", "Oct 23", "Oct 24", "Oct 25", "Oct 26" };
            int[] activeData = { 1, 1, 2, 2, 2, 2, 2, 3 };
            int[] paidData = { 0, 0, 0, 1, 1, 1, 1, 2 };
            int[] voidData = { 0, 0, 0, 0, 0, 0, 0, 0 };

            for (int i = 0; i < dates.Length; i++)
            {
                activeSeries.Points.AddXY(dates[i], activeData[i]);
                paidSeries.Points.AddXY(dates[i], paidData[i]);
                voidSeries.Points.AddXY(dates[i], voidData[i]);
            }

            chart.Series.Add(activeSeries);
            chart.Series.Add(paidSeries);
            chart.Series.Add(voidSeries);

            var legend = new Legend
            {
                Docking = Docking.Top,
                Alignment = StringAlignment.Center
            };
            chart.Legends.Add(legend);

            return chart;
        }

        private Chart CreateDonutChart()
        {
            var chart = new Chart
            {
                BackColor = Color.White
            };

            var chartArea = new ChartArea
            {
                BackColor = Color.White
            };
            chart.ChartAreas.Add(chartArea);

            var series = new Series
            {
                ChartType = SeriesChartType.Doughnut
            };

            series.Points.AddXY("Active", 3);
            series.Points[0].Color = Color.FromArgb(59, 130, 246);

            series.Points.AddXY("Paid", 2);
            series.Points[1].Color = Color.FromArgb(34, 197, 94);

            series.Points.AddXY("Void", 0);
            series.Points[2].Color = Color.FromArgb(239, 68, 68);

            series["PieLabelStyle"] = "Outside";
            series.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            chart.Series.Add(series);

            var legend = new Legend
            {
                Docking = Docking.Bottom,
                Alignment = StringAlignment.Center
            };
            chart.Legends.Add(legend);

            return chart;
        }

        private Panel CreateBottomPanel()
        {
            var panel = new Panel
            {
                Size = new Size(1340, 280),
                BackColor = Color.Transparent
            };

            var invoicesPanel = CreateLatestInvoicesPanel();
            invoicesPanel.Location = new Point(0, 0);

            var summaryPanel = CreateAccountSummaryPanel();
            summaryPanel.Location = new Point(900, 0);

            panel.Controls.AddRange(new Control[] { invoicesPanel, summaryPanel });
            return panel;
        }

        private Panel CreateLatestInvoicesPanel()
        {
            var panel = new Panel
            {
                Size = new Size(880, 280),
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            var titleLabel = new Label
            {
                Text = "Latest invoices",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(20, 15),
                AutoSize = true
            };

            var gridView = new DataGridView
            {
                Location = new Point(20, 60),
                Size = new Size(840, 200),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Font = new Font("Segoe UI", 10)
            };

            gridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "InvoiceNumber",
                HeaderText = "Invoice Number",
                Width = 150
            });

            gridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ClientName",
                HeaderText = "Client Name",
                Width = 200
            });

            gridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Status",
                HeaderText = "Status",
                Width = 120
            });

            gridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Amount",
                HeaderText = "Amount",
                Width = 150
            });

            gridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Date",
                HeaderText = "Date",
                Width = 200
            });

            // Sample data
            gridView.Rows.Add("#2025003", "walk-in Client", "Active", "348 KHR", "October 26, 2025");
            gridView.Rows.Add("#2025002", "Fou Zhou", "Paid", "401 KHR", "October 26, 2025");
            gridView.Rows.Add("#2025001", "walk-in Client", "Paid", "5 KHR", "October 26, 2025");

            // Color code status
            foreach (DataGridViewRow row in gridView.Rows)
            {
                var status = row.Cells["Status"].Value?.ToString();
                if (status == "Active")
                    row.Cells["Status"].Style.ForeColor = Color.FromArgb(59, 130, 246);
                else if (status == "Paid")
                    row.Cells["Status"].Style.ForeColor = Color.FromArgb(34, 197, 94);
                else if (status == "Void")
                    row.Cells["Status"].Style.ForeColor = Color.FromArgb(239, 68, 68);
            }

            panel.Controls.AddRange(new Control[] { titleLabel, gridView });
            return panel;
        }

        private Panel CreateAccountSummaryPanel()
        {
            var panel = new Panel
            {
                Size = new Size(440, 280),
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            var titleLabel = new Label
            {
                Text = "Account Summary",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(20, 15),
                AutoSize = true
            };

            int yPos = 60;
            var metrics = new[]
            {
                ("Total active invoices", "353 KHR"),
                ("Repeated invoices", "0"),
                ("Payment links", "0"),
                ("Store sales", "0"),
                ("Products", "6")
            };

            foreach (var (label, value) in metrics)
            {
                var metricLabel = new Label
                {
                    Text = label,
                    Font = new Font("Segoe UI", 11),
                    ForeColor = Color.FromArgb(100, 116, 139),
                    Location = new Point(20, yPos),
                    AutoSize = true
                };

                var valueLabel = new Label
                {
                    Text = value,
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    ForeColor = Color.FromArgb(15, 23, 42),
                    Location = new Point(320, yPos),
                    AutoSize = true
                };

                panel.Controls.AddRange(new Control[] { metricLabel, valueLabel });
                yPos += 40;
            }

            panel.Controls.Add(titleLabel);
            return panel;
        }

        private void LoadDashboardData()
        {
            try
            {
                // Load real data from database
                var invoices = _context.Invoices
                    .Include(i => i.Customer)
                    .OrderByDescending(i => i.CreatedAt)
                    .Take(10)
                    .ToList();

                // Update UI with real data
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading dashboard data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StartAutoRefresh()
        {
            _refreshTimer = new System.Windows.Forms.Timer
            {
                Interval = 5000 // 5 seconds
            };
            _refreshTimer.Tick += (s, e) => LoadDashboardData();
            _refreshTimer.Start();
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
