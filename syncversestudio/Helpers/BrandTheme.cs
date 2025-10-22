using System.Drawing;

namespace SyncVerseStudio.Helpers
{
    /// <summary>
    /// Centralized brand theme and color palette for SyncVerse Studio
    /// </summary>
    public static class BrandTheme
    {
        // Brand Colors
        public static readonly Color CoolWhite = Color.FromArgb(215, 232, 250);      // #D7E8FA
        public static readonly Color Charcoal = Color.FromArgb(118, 84, 44);         // #76542C
        public static readonly Color LimeGreen = Color.FromArgb(93, 251, 194);       // #5DF9C0
        public static readonly Color OceanBlue = Color.FromArgb(59, 30, 255);        // #3B1EFF

// https://raw.githubusercontent.com/EIRSVi/eirsvi/d8339235bb1765d461e284ab51bd1223d4345dce/assets/brand/mainlogo.svg 
// https://raw.githubusercontent.com/EIRSVi/eirsvi/refs/heads/docs/assets/brand/noBgBlack.png
// https://raw.githubusercontent.com/EIRSVi/eirsvi/refs/heads/docs/assets/brand/noBgColor.png
// https://raw.githubusercontent.com/EIRSVi/eirsvi/refs/heads/docs/assets/brand/noBgWhite.png
// https://raw.githubusercontent.com/EIRSVi/eirsvi/refs/heads/docs/assets/brand/with_padding.png

// # Clean and rebuild
// dotnet clean
// dotnet restore
// dotnet build
// dotnet run --project syncversestudio





        // UI Element Colors
        public static readonly Color Background = CoolWhite;
        public static readonly Color PrimaryText = Charcoal;
        public static readonly Color SecondaryText = Color.FromArgb(90, 70, 40);
        public static readonly Color AccentPrimary = OceanBlue;
        public static readonly Color AccentSecondary = LimeGreen;
        public static readonly Color CardBackground = Color.White;
        public static readonly Color BorderColor = Color.FromArgb(180, 200, 220);

        // Button Colors
        public static readonly Color ButtonPrimary = OceanBlue;
        public static readonly Color ButtonSecondary = LimeGreen;
        public static readonly Color ButtonDanger = Color.FromArgb(220, 60, 60);
        public static readonly Color ButtonWarning = Color.FromArgb(255, 165, 0);
        public static readonly Color ButtonText = Color.White;

        // Status Colors
        public static readonly Color Success = LimeGreen;
        public static readonly Color Error = Color.FromArgb(220, 60, 60);
        public static readonly Color Warning = Color.FromArgb(255, 165, 0);
        public static readonly Color Info = OceanBlue;

        // Sidebar/Navigation
        public static readonly Color SidebarBackground = Charcoal;
        public static readonly Color SidebarText = CoolWhite;
        public static readonly Color SidebarHover = Color.FromArgb(140, 100, 60);
        public static readonly Color SidebarActive = OceanBlue;

        // Header
        public static readonly Color HeaderBackground = OceanBlue;
        public static readonly Color HeaderText = Color.White;

        // Fonts
        public static readonly Font TitleFont = new Font("Segoe UI", 22, FontStyle.Bold);
        public static readonly Font SubtitleFont = new Font("Segoe UI", 16, FontStyle.Bold);
        public static readonly Font HeaderFont = new Font("Segoe UI", 14, FontStyle.Bold);
        public static readonly Font BodyFont = new Font("Segoe UI", 11, FontStyle.Regular);
        public static readonly Font SmallFont = new Font("Segoe UI", 9, FontStyle.Regular);
        public static readonly Font ButtonFont = new Font("Segoe UI", 12, FontStyle.Bold);

        // Brand Assets
        public const string LogoPath = "assets/brand/noBgColor.png"; // Blue circular logo
        public const string LogoPathWhite = "assets/brand/noBgWhite.png"; // White version
        public const string LogoPathBlack = "assets/brand/noBgBlack.png"; // Black version
        public const string BrandName = "SYNCVERSE STUDIO";
        public const string BrandTagline = "POS SYSTEM";

        /// <summary>
        /// Apply standard button styling
        /// </summary>
        public static void StyleButton(Button button, bool isPrimary = true)
        {
            button.BackColor = isPrimary ? ButtonPrimary : ButtonSecondary;
            button.ForeColor = ButtonText;
            button.FlatStyle = FlatStyle.Flat;
            button.Font = ButtonFont;
            button.Cursor = Cursors.Hand;
            button.FlatAppearance.BorderSize = 0;
            
            button.MouseEnter += (s, e) =>
            {
                button.BackColor = isPrimary ? 
                    Color.FromArgb(45, 25, 230) : 
                    Color.FromArgb(80, 230, 180);
            };
            
            button.MouseLeave += (s, e) =>
            {
                button.BackColor = isPrimary ? ButtonPrimary : ButtonSecondary;
            };
        }

        /// <summary>
        /// Apply standard panel styling
        /// </summary>
        public static void StylePanel(Panel panel, bool isCard = true)
        {
            panel.BackColor = isCard ? CardBackground : Background;
            if (isCard)
            {
                panel.Paint += (s, e) =>
                {
                    var rect = panel.ClientRectangle;
                    using (var pen = new Pen(BorderColor, 1))
                    {
                        e.Graphics.DrawRectangle(pen, 0, 0, rect.Width - 1, rect.Height - 1);
                    }
                };
            }
        }

        /// <summary>
        /// Apply standard DataGridView styling
        /// </summary>
        public static void StyleDataGridView(DataGridView grid)
        {
            grid.BackgroundColor = CardBackground;
            grid.BorderStyle = BorderStyle.None;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.ReadOnly = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.RowHeadersVisible = false;
            
            grid.DefaultCellStyle = new DataGridViewCellStyle
            {
                Font = BodyFont,
                Padding = new Padding(5),
                SelectionBackColor = Color.FromArgb(180, 210, 240),
                SelectionForeColor = PrimaryText,
                BackColor = CardBackground,
                ForeColor = PrimaryText
            };
            
            grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = CoolWhite,
                ForeColor = PrimaryText,
                Font = HeaderFont,
                Padding = new Padding(8),
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };
            
            grid.ColumnHeadersHeight = 40;
            grid.RowTemplate.Height = 45;
            grid.EnableHeadersVisualStyles = false;
        }

        /// <summary>
        /// Apply standard TextBox styling
        /// </summary>
        public static void StyleTextBox(TextBox textBox)
        {
            textBox.Font = BodyFont;
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.BackColor = CardBackground;
            textBox.ForeColor = PrimaryText;
        }

        /// <summary>
        /// Apply standard Label styling
        /// </summary>
        public static void StyleLabel(Label label, bool isHeader = false)
        {
            label.Font = isHeader ? HeaderFont : BodyFont;
            label.ForeColor = PrimaryText;
        }
    }
}
