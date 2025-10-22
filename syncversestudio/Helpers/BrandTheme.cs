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










        // UI Element Colors - Clean White Theme
        public static readonly Color Background = Color.White;                       // Clean white background
        public static readonly Color PrimaryText = Color.FromArgb(30, 30, 30);      // Almost black text
        public static readonly Color SecondaryText = Color.FromArgb(100, 100, 100); // Gray text
        public static readonly Color AccentPrimary = Color.FromArgb(34, 197, 94);   // Green accent
        public static readonly Color AccentSecondary = Color.FromArgb(59, 130, 246);// Blue accent
        public static readonly Color CardBackground = Color.White;
        public static readonly Color BorderColor = Color.FromArgb(230, 230, 230);   // Light gray border

        // Button Colors - Consistent Brand Colors
        public static readonly Color ButtonAdd = Color.FromArgb(34, 197, 94);       // Green - Add/Create
        public static readonly Color ButtonEdit = Color.FromArgb(59, 130, 246);     // Blue - Edit
        public static readonly Color ButtonDelete = Color.FromArgb(239, 68, 68);    // Red - Delete
        public static readonly Color ButtonRefresh = Color.FromArgb(107, 114, 128); // Gray - Refresh/Cancel
        public static readonly Color ButtonExport = Color.FromArgb(168, 85, 247);   // Purple - Export
        public static readonly Color ButtonText = Color.White;

        // Status Colors
        public static readonly Color Success = Color.FromArgb(34, 197, 94);         // Green
        public static readonly Color Error = Color.FromArgb(239, 68, 68);           // Red
        public static readonly Color Warning = Color.FromArgb(249, 115, 22);        // Orange
        public static readonly Color Info = Color.FromArgb(59, 130, 246);           // Blue
        
        // Table Colors - New Blue Theme
        public static readonly Color TableHeaderBg = Color.FromArgb(158, 236, 255); // #9EECFF - Blue 1
        public static readonly Color TableHeaderText = Color.FromArgb(30, 30, 30);  // Dark text
        public static readonly Color TableRowEven = Color.White;
        public static readonly Color TableRowOdd = Color.FromArgb(240, 250, 255);   // Very light blue
        public static readonly Color TableRowHover = Color.FromArgb(158, 236, 255); // #9EECFF - Blue 1
        public static readonly Color TableRowSelected = Color.FromArgb(48, 148, 255); // #3094FF - Blue 2
        public static readonly Color TableBorder = Color.FromArgb(158, 236, 255);   // #9EECFF - Blue 1

        // Sidebar/Navigation
        public static readonly Color SidebarBackground = Color.FromArgb(215, 232, 250); // Light blue
        public static readonly Color SidebarText = Color.FromArgb(60, 60, 60);      // Dark text
        public static readonly Color SidebarHover = Color.FromArgb(255, 0, 80);     // Pink hover
        public static readonly Color SidebarActive = Color.FromArgb(255, 0, 80);    // Pink active

        // Page Header
        public static readonly Color PageHeaderBg = Color.White;
        public static readonly Color PageHeaderText = Color.FromArgb(30, 30, 30);
        public static readonly Color PageSubtitleText = Color.FromArgb(100, 100, 100);
        
        // Legacy compatibility
        public static readonly Color HeaderBackground = Color.White;
        public static readonly Color HeaderText = Color.FromArgb(30, 30, 30);

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
        /// Apply standard button styling with consistent brand colors
        /// </summary>
        public static void StyleButton(Button button, string buttonType = "add")
        {
            // Set color based on button type
            Color bgColor = buttonType.ToLower() switch
            {
                "add" or "create" or "save" => ButtonAdd,
                "edit" or "update" => ButtonEdit,
                "delete" or "remove" => ButtonDelete,
                "refresh" or "cancel" or "close" => ButtonRefresh,
                "export" or "print" => ButtonExport,
                _ => ButtonAdd
            };
            
            button.BackColor = bgColor;
            button.ForeColor = ButtonText;
            button.FlatStyle = FlatStyle.Flat;
            button.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            button.Cursor = Cursors.Hand;
            button.FlatAppearance.BorderSize = 0;
            button.Height = 36;
            button.Padding = new Padding(16, 0, 16, 0);
            
            // Hover effect - slightly darker
            Color hoverColor = Color.FromArgb(
                Math.Max(0, bgColor.R - 20),
                Math.Max(0, bgColor.G - 20),
                Math.Max(0, bgColor.B - 20)
            );
            
            button.MouseEnter += (s, e) => { button.BackColor = hoverColor; };
            button.MouseLeave += (s, e) => { button.BackColor = bgColor; };
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
        /// Apply standard DataGridView styling - Clean white theme
        /// </summary>
        public static void StyleDataGridView(DataGridView grid)
        {
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.FixedSingle;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.ReadOnly = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.RowHeadersVisible = false;
            grid.MultiSelect = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.EnableHeadersVisualStyles = false;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = TableBorder;
            
            // Default cell style - Bold text
            grid.DefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Padding = new Padding(12, 8, 12, 8),
                SelectionBackColor = TableRowSelected,
                SelectionForeColor = Color.White,
                BackColor = Color.White,
                ForeColor = PrimaryText,
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };
            
            // Alternating row style - Bold text
            grid.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Padding = new Padding(12, 8, 12, 8),
                BackColor = TableRowOdd,
                ForeColor = PrimaryText,
                SelectionBackColor = TableRowSelected,
                SelectionForeColor = Color.White
            };
            
            // Column header style
            grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = TableHeaderBg,
                ForeColor = Color.White, // White text on blue header
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Padding = new Padding(12, 10, 12, 10),
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                SelectionBackColor = TableHeaderBg,
                SelectionForeColor = Color.White
            };
            
            grid.ColumnHeadersHeight = 45;
            grid.RowTemplate.Height = 50;
            
            // Hover effect
            grid.CellMouseEnter += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = TableRowHover;
                }
            };
            
            grid.CellMouseLeave += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = 
                        e.RowIndex % 2 == 0 ? Color.White : TableRowOdd;
                }
            };
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

        /// <summary>
        /// Create a standard page header with title and subtitle
        /// </summary>
        public static Panel CreatePageHeader(string title, string subtitle = "")
        {
            var headerPanel = new Panel
            {
                BackColor = PageHeaderBg,
                Height = string.IsNullOrEmpty(subtitle) ? 80 : 100,
                Dock = DockStyle.Top,
                Padding = new Padding(40, 20, 40, 20)
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                ForeColor = PageHeaderText,
                AutoSize = true,
                Location = new Point(40, 20)
            };
            headerPanel.Controls.Add(titleLabel);

            if (!string.IsNullOrEmpty(subtitle))
            {
                var subtitleLabel = new Label
                {
                    Text = subtitle,
                    Font = new Font("Segoe UI", 11F, FontStyle.Regular),
                    ForeColor = PageSubtitleText,
                    AutoSize = true,
                    Location = new Point(40, 55)
                };
                headerPanel.Controls.Add(subtitleLabel);
            }

            return headerPanel;
        }

        /// <summary>
        /// Create a standard button panel for CRUD operations
        /// </summary>
        public static Panel CreateButtonPanel(params (string text, string type, EventHandler onClick)[] buttons)
        {
            var buttonPanel = new Panel
            {
                Height = 60,
                Dock = DockStyle.Top,
                BackColor = Color.White,
                Padding = new Padding(40, 12, 40, 12)
            };

            int xPos = 40;
            foreach (var (text, type, onClick) in buttons)
            {
                var button = new Button
                {
                    Text = text,
                    Location = new Point(xPos, 12),
                    Size = new Size(120, 36),
                    TabIndex = 0
                };
                StyleButton(button, type);
                button.Click += onClick;
                buttonPanel.Controls.Add(button);
                xPos += 130;
            }

            return buttonPanel;
        }

        /// <summary>
        /// Create a standard search panel
        /// </summary>
        public static Panel CreateSearchPanel(TextBox searchBox, params Control[] additionalControls)
        {
            var searchPanel = new Panel
            {
                Height = 60,
                Dock = DockStyle.Top,
                BackColor = Color.White,
                Padding = new Padding(40, 12, 40, 12)
            };

            searchBox.Location = new Point(40, 15);
            searchBox.Size = new Size(300, 30);
            searchBox.Font = new Font("Segoe UI", 10F);
            searchBox.PlaceholderText = "Search...";
            searchPanel.Controls.Add(searchBox);

            int xPos = 360;
            foreach (var control in additionalControls)
            {
                control.Location = new Point(xPos, 12);
                searchPanel.Controls.Add(control);
                xPos += control.Width + 10;
            }

            return searchPanel;
        }

        /// <summary>
        /// Create a standard content container for tables
        /// </summary>
        public static Panel CreateContentContainer()
        {
            return new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(40, 20, 40, 40)
            };
        }
    }
}
