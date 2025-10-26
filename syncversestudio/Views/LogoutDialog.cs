using System;
using System.Drawing;
using System.Windows.Forms;
using SyncVerseStudio.Helpers;
using SyncVerseStudio.Models;
using FontAwesome.Sharp;

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
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = BrandTheme.CoolWhite; // #D7E8FA

            // Close Button
            var btnClose = new Button
            {
                Text = "X",
                Size = new Size(40, 40),
                Location = new Point(this.Width - 45, 5),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = BrandTheme.Primary,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 50, 50);
            btnClose.Click += (s, e) => { SelectedAction = LogoutAction.Cancel; this.Close(); };
            this.Controls.Add(btnClose);

            // Logo centered at top
            var logoPictureBox = new PictureBox
            {
                Size = new Size(300, 100),
                Location = new Point((this.Width - 300) / 2, 50),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };

            try
            {
                string[] logoPaths = { 
                    "assets\\brand\\logo.png", 
                    "logo.png",
                    System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "brand", "logo.png")
                };

                bool logoLoaded = false;
                foreach (var logoPath in logoPaths)
                {
                    if (System.IO.File.Exists(logoPath))
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
                        Font = new Font("Segoe UI", 18, FontStyle.Bold),
                        ForeColor = BrandTheme.Primary,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Dock = DockStyle.Fill
                    };
                    logoPictureBox.Controls.Add(placeholderLabel);
                }
            }
            catch { }

            this.Controls.Add(logoPictureBox);

            // User Info
            var userInfoLabel = new Label
            {
                Text = $"{currentUser?.FirstName} {currentUser?.LastName}\n{currentUser?.Role}",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = BrandTheme.PrimaryText,
                Location = new Point(50, 170),
                Size = new Size(400, 50),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = BrandTheme.CoolWhite
            };
            this.Controls.Add(userInfoLabel);

            // Switch User Button with icon - Only button as per requirements
            var switchUserButton = new IconButton
            {
                Text = "  Switch User",
                IconChar = IconChar.UsersCog,
                IconColor = Color.White,
                IconSize = 24,
                Location = new Point(75, 240),
                Size = new Size(350, 60),
                BackColor = BrandTheme.Primary,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Cursor = Cursors.Hand,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleCenter,
                TextImageRelation = TextImageRelation.ImageBeforeText
            };
            switchUserButton.FlatAppearance.BorderSize = 0;
            switchUserButton.FlatAppearance.MouseOverBackColor = BrandTheme.PrimaryHover;
            switchUserButton.Click += (s, e) => { SelectedAction = LogoutAction.SwitchUser; this.Close(); };
            this.Controls.Add(switchUserButton);

            // Note: Removed Logout, Exit Application, and Cancel buttons as per requirements
        }
    }
}
