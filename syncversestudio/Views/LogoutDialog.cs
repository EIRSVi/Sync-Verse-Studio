using System;
using System.Drawing;
using System.Windows.Forms;
using SyncVerseStudio.Helpers;
using SyncVerseStudio.Models;

namespace SyncVerseStudio.Views
{
    public class LogoutDialog : Form
    {
        public enum LogoutAction
        {
            Cancel,
            SwitchUser,
            Logout,
            ExitApplication
        }

        public LogoutAction SelectedAction { get; private set; } = LogoutAction.Cancel;

        public LogoutDialog(User currentUser)
        {
            InitializeDialog(currentUser);
        }

        private void InitializeDialog(User currentUser)
        {
            this.Text = "";
            this.Size = new Size(550, 380);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.White;

            // Header Panel
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = BrandTheme.Primary
            };

            // Close Button
            var btnClose = new Button
            {
                Text = "X",
                Size = new Size(35, 35),
                Location = new Point(510, 5),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 50, 50);
            btnClose.Click += (s, e) => { SelectedAction = LogoutAction.Cancel; this.Close(); };
            headerPanel.Controls.Add(btnClose);

            // Title
            var titleLabel = new Label
            {
                Text = "Account Options",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 25),
                Size = new Size(500, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };
            headerPanel.Controls.Add(titleLabel);
            this.Controls.Add(headerPanel);

            // User Info
            var userInfoLabel = new Label
            {
                Text = $"Logged in as: {currentUser?.FirstName} {currentUser?.LastName} ({currentUser?.Role})",
                Font = new Font("Segoe UI", 10),
                ForeColor = BrandTheme.SecondaryText,
                Location = new Point(30, 100),
                Size = new Size(490, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(userInfoLabel);

            // Message
            var messageLabel = new Label
            {
                Text = "What would you like to do?",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = BrandTheme.PrimaryText,
                Location = new Point(30, 135),
                Size = new Size(490, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(messageLabel);

            // Switch User Button
            var switchUserButton = new Button
            {
                Text = "Switch User",
                Location = new Point(30, 180),
                Size = new Size(490, 50),
                BackColor = BrandTheme.Primary,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            switchUserButton.FlatAppearance.BorderSize = 0;
            switchUserButton.FlatAppearance.MouseOverBackColor = BrandTheme.PrimaryHover;
            switchUserButton.Click += (s, e) => { SelectedAction = LogoutAction.SwitchUser; this.Close(); };
            this.Controls.Add(switchUserButton);

            // Logout Button
            var logoutButton = new Button
            {
                Text = "Logout (Return to Login)",
                Location = new Point(30, 240),
                Size = new Size(235, 50),
                BackColor = Color.FromArgb(34, 197, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            logoutButton.FlatAppearance.BorderSize = 0;
            logoutButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(22, 163, 74);
            logoutButton.Click += (s, e) => { SelectedAction = LogoutAction.Logout; this.Close(); };
            this.Controls.Add(logoutButton);

            // Exit Application Button
            var exitButton = new Button
            {
                Text = "Exit Application",
                Location = new Point(285, 240),
                Size = new Size(235, 50),
                BackColor = Color.FromArgb(239, 68, 68),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            exitButton.FlatAppearance.BorderSize = 0;
            exitButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(220, 38, 38);
            exitButton.Click += (s, e) => { SelectedAction = LogoutAction.ExitApplication; this.Close(); };
            this.Controls.Add(exitButton);

            // Cancel Button
            var cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(30, 300),
                Size = new Size(490, 45),
                BackColor = Color.White,
                ForeColor = BrandTheme.SecondaryText,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10),
                Cursor = Cursors.Hand
            };
            cancelButton.FlatAppearance.BorderSize = 2;
            cancelButton.FlatAppearance.BorderColor = Color.FromArgb(220, 220, 220);
            cancelButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 245, 245);
            cancelButton.Click += (s, e) => { SelectedAction = LogoutAction.Cancel; this.Close(); };
            this.Controls.Add(cancelButton);
        }
    }
}
