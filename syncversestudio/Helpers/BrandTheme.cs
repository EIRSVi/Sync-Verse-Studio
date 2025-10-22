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
        public static readonly Color DarkGray = Color.FromArgb(45, 45, 45);          // #2D2D2D - Dark color
        public static readonly Color LimeGreen = Color.FromArgb(95, 237, 131);       // #5FED83 - Green 3
        public static readonly Color LightGreen = Color.FromArgb(191, 255, 209);     // #BFFFD1 - Green 1
        public static readonly Color LightBlue = Color.FromArgb(158, 236, 255);      // #9EECFF - Blue 1
        public static readonly Color MediumBlue = Color.FromArgb(48, 148, 255);      // #3094FF - Blue 2
        public static readonly Color HoverColor = Color.FromArgb(246, 248, 250);     // #f6f8fa
        public static readonly Color PurpleHover = Color.FromArgb(80, 29, 175);      // #501DAF

// Logo URLs
// Main SVG Logo: https://raw.githubusercontent.com/EIRSVi/eirsvi/d8339235bb1765d461e284ab51bd1223d4345dce/assets/brand/mainlogo.svg
// PNG Variants:
// - Black: https://raw.githubusercontent.com/EIRSVi/eirsvi/refs/heads/docs/assets/brand/noBgBlack.png
// - Color: https://raw.githubusercontent.com/EIRSVi/eirsvi/refs/heads/docs/assets/brand/noBgColor.png
// - White: https://raw.githubusercontent.com/EIRSVi/eirsvi/refs/heads/docs/assets/brand/noBgWhite.png
// - Padded: https://raw.githubusercontent.com/EIRSVi/eirsvi/refs/heads/docs/assets/brand/with_padding.png
// https://raw.githubusercontent.com/EIRSVi/eirsvi/refs/heads/docs/assets/brand/logo.png

// Build Commands:
// dotnet clean
// dotnet restore
// dotnet build
// dotnet run --project syncversestudio

// Development Rules:
// - Never use emoji in UI
// - Never use gradient colors
// - Write documentation/guides only when explicitly requested

// Khmer translations for all menu items:
// ផ្ទាំងគ្រប់គ្រង (Dashboard)
// គណនី (Accounts)
// ផលិតផល (Products)
// អតិថិជន (Customers)
// ប្រភេទទំនិញ (Categories)
// អ្នកផ្គត់ផ្គង់ (Suppliers)
// វិភាគ (Analytics)
// កំណត់ហេតុសវនកម្ម (Audit Logs)
// ចំណុចលក់ (Point of Sale)
// ប្រវត្តិការលក់ (Sales History)
// របាយការណ៍ស្តុក (Stock Reports)
// ចាកចេញ (Logout)










        // UI Element Colors
        public static readonly Color Background = CoolWhite;
        public static readonly Color PrimaryText = DarkGray;
        public static readonly Color SecondaryText = Color.FromArgb(90, 90, 90);
        public static readonly Color AccentPrimary = DarkGray;
        public static readonly Color AccentSecondary = LimeGreen;
        public static readonly Color CardBackground = Color.White;
        public static readonly Color BorderColor = Color.FromArgb(180, 200, 220);

        // Button Colors
        public static readonly Color ButtonPrimary = LimeGreen;                      // #5FED83 - Green button
        public static readonly Color ButtonSecondary = MediumBlue;                   // #3094FF - Blue button
        public static readonly Color ButtonDanger = Color.FromArgb(220, 60, 60);
        public static readonly Color ButtonWarning = Color.FromArgb(255, 165, 0);
        public static readonly Color ButtonText = Color.White;

        // Status Colors
        public static readonly Color Success = LimeGreen;                            // #5FED83
        public static readonly Color Error = Color.FromArgb(220, 60, 60);
        public static readonly Color Warning = Color.FromArgb(255, 165, 0);
        public static readonly Color Info = MediumBlue;                              // #3094FF
        
        // Icon Colors
        public static readonly Color IconDark = DarkGray;                            // Dark icons
        public static readonly Color IconBlue = LightBlue;                           // #9EECFF - Table data icons

        // Sidebar/Navigation
        public static readonly Color SidebarBackground = DarkGray;
        public static readonly Color SidebarText = CoolWhite;
        public static readonly Color SidebarHover = HoverColor;
        public static readonly Color SidebarActive = PurpleHover;

        // Header
        public static readonly Color HeaderBackground = DarkGray;
        public static readonly Color HeaderText = Color.White;

        // Fonts
        public static readonly Font TitleFont = new Font("Segoe UI", 22, FontStyle.Bold);
        public static readonly Font SubtitleFont = new Font("Segoe UI", 16, FontStyle.Bold);
        public static readonly Font HeaderFont = new Font("Segoe UI", 14, FontStyle.Bold);
        public static readonly Font BodyFont = new Font("Segoe UI", 11, FontStyle.Regular);
        public static readonly Font SmallFont = new Font("Segoe UI", 9, FontStyle.Regular);
        public static readonly Font ButtonFont = new Font("Segoe UI", 12, FontStyle.Bold);

        // Brand Assets
        public const string LogoUrl = "https://raw.githubusercontent.com/EIRSVi/eirsvi/d8339235bb1765d461e284ab51bd1223d4345dce/assets/brand/mainlogo.svg";
        public const string LogoUrlPng = "https://raw.githubusercontent.com/EIRSVi/eirsvi/refs/heads/docs/assets/brand/noBgColor.png";
        public const string LogoPath = "assets/brand/noBgColor.png"; // Local fallback
        public const string LogoPathWhite = "assets/brand/noBgWhite.png";
        public const string LogoPathBlack = "assets/brand/noBgBlack.png";
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
