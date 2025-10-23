using System;
using System.Drawing;
using System.Windows.Forms;
using FontAwesome.Sharp;

namespace SyncVerseStudio.Views
{
    public partial class TaxSettingsForm : Form
    {
        public decimal TaxRate { get; private set; }
        private NumericUpDown _taxRateNumeric;

        public TaxSettingsForm(decimal currentTaxRate)
        {
            TaxRate = currentTaxRate;
            InitializeComponent();
            _taxRateNumeric.Value = currentTaxRate * 100; // Convert to percentage
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Name = "TaxSettingsForm";
            this.Text = "Tax Settings";
            this.Size = new Size(400, 250);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            // Header
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(59, 130, 246)
            };

            var iconBox = new IconPictureBox
            {
                IconChar = IconChar.Percent,
                IconColor = Color.White,
                IconSize = 24,
                Location = new Point(20, 18),
                Size = new Size(24, 24),
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(iconBox);

            var titleLabel = new Label
            {
                Text = "Tax Settings",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(55, 20),
                Size = new Size(200, 25),
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(titleLabel);

            this.Controls.Add(headerPanel);

            // Content
            var contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(30)
            };

            var instructionLabel = new Label
            {
                Text = "Set the tax rate for all sales transactions:",
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.FromArgb(60, 60, 60),
                Location = new Point(0, 20),
                Size = new Size(320, 25),
                BackColor = Color.Transparent
            };
            contentPanel.Controls.Add(instructionLabel);

            var taxLabel = new Label
            {
                Text = "Tax Rate (%):",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(0, 60),
                Size = new Size(100, 25),
                BackColor = Color.Transparent
            };
            contentPanel.Controls.Add(taxLabel);

            _taxRateNumeric = new NumericUpDown
            {
                Location = new Point(110, 60),
                Size = new Size(100, 30),
                Font = new Font("Segoe UI", 11F),
                DecimalPlaces = 2,
                Minimum = 0,
                Maximum = 50,
                Value = 10.00m
            };
            contentPanel.Controls.Add(_taxRateNumeric);

            var percentLabel = new Label
            {
                Text = "%",
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.FromArgb(60, 60, 60),
                Location = new Point(220, 63),
                Size = new Size(20, 25),
                BackColor = Color.Transparent
            };
            contentPanel.Controls.Add(percentLabel);

            var noteLabel = new Label
            {
                Text = "Note: This setting requires administrator privileges.\nChanges will apply to all future transactions.",
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                ForeColor = Color.FromArgb(120, 120, 120),
                Location = new Point(0, 100),
                Size = new Size(320, 40),
                BackColor = Color.Transparent
            };
            contentPanel.Controls.Add(noteLabel);

            // Buttons
            var buttonPanel = new Panel
            {
                Location = new Point(0, 150),
                Size = new Size(320, 40),
                BackColor = Color.Transparent
            };

            var saveButton = new Button
            {
                Text = "Save",
                Location = new Point(140, 0),
                Size = new Size(80, 35),
                BackColor = Color.FromArgb(34, 197, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            saveButton.FlatAppearance.BorderSize = 0;
            saveButton.Click += SaveButton_Click;
            buttonPanel.Controls.Add(saveButton);

            var cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(230, 0),
                Size = new Size(80, 35),
                BackColor = Color.FromArgb(156, 163, 175),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F)
            };
            cancelButton.FlatAppearance.BorderSize = 0;
            cancelButton.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            buttonPanel.Controls.Add(cancelButton);

            contentPanel.Controls.Add(buttonPanel);
            this.Controls.Add(contentPanel);

            this.ResumeLayout(false);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            TaxRate = _taxRateNumeric.Value / 100; // Convert percentage to decimal
            this.DialogResult = DialogResult.OK;
        }
    }
}