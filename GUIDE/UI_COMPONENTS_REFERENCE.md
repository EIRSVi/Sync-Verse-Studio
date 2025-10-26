# UI Components Reference Guide

## Color Palette

### Primary Colors
```
Teal (Primary Action)
- Hex: #14B8A6
- RGB: 20, 184, 166
- Usage: Pay buttons, primary actions, branding, active states

Blue (Secondary Action)
- Hex: #3B82F6
- RGB: 59, 130, 246
- Usage: Save buttons, Active invoice status, secondary actions

Green (Success)
- Hex: #22C55E
- RGB: 34, 197, 94
- Usage: Paid status, success messages, positive balance

Red (Danger)
- Hex: #EF4444
- RGB: 239, 68, 68
- Usage: Cancel buttons, Void status, delete actions
```

### Neutral Colors
```
Dark Slate (Primary Text)
- Hex: #0F172A
- RGB: 15, 23, 42
- Usage: Headings, primary text, important labels

Slate Gray (Secondary Text)
- Hex: #64748B
- RGB: 100, 116, 139
- Usage: Descriptions, secondary text, placeholders

Light Slate (Borders)
- Hex: #E2E8F0
- RGB: 226, 232, 240
- Usage: Borders, dividers, card outlines

Pale Gray (Background)
- Hex: #F8FAFC
- RGB: 248, 250, 252
- Usage: Page background, card backgrounds, panels

White
- Hex: #FFFFFF
- RGB: 255, 255, 255
- Usage: Cards, modals, primary surfaces
```

## Typography

### Font Family
```
Primary: Segoe UI
Fallback: Arial, sans-serif
```

### Font Sizes & Weights
```
Display Large: 24px Bold
Display Medium: 20px Bold
Display Small: 18px Bold

Heading 1: 16px Bold
Heading 2: 14px Bold
Heading 3: 12px Bold

Body Large: 12px Regular
Body Medium: 11px Regular
Body Small: 10px Regular
Body Tiny: 9px Regular
Body Micro: 8px Regular

Button Text: 14px Bold (Large), 12px Bold (Medium), 11px Bold (Small)
```

## Button Styles

### Primary Button (Teal)
```csharp
BackColor = Color.FromArgb(20, 184, 166)
ForeColor = Color.White
Font = new Font("Segoe UI", 14, FontStyle.Bold)
FlatStyle = FlatStyle.Flat
FlatAppearance.BorderSize = 0
Height = 50-60px
Cursor = Cursors.Hand
```

### Secondary Button (Blue)
```csharp
BackColor = Color.FromArgb(59, 130, 246)
ForeColor = Color.White
Font = new Font("Segoe UI", 12, FontStyle.Bold)
FlatStyle = FlatStyle.Flat
FlatAppearance.BorderSize = 0
Height = 40-50px
Cursor = Cursors.Hand
```

### Danger Button (Red)
```csharp
BackColor = Color.FromArgb(239, 68, 68)
ForeColor = Color.White
Font = new Font("Segoe UI", 12, FontStyle.Bold)
FlatStyle = FlatStyle.Flat
FlatAppearance.BorderSize = 0
Height = 40-50px
Cursor = Cursors.Hand
```

### Ghost Button (Outline)
```csharp
BackColor = Color.White
ForeColor = Color.FromArgb(100, 116, 139)
Font = new Font("Segoe UI", 11, FontStyle.Regular)
FlatStyle = FlatStyle.Flat
FlatAppearance.BorderSize = 1
FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240)
Height = 35-45px
Cursor = Cursors.Hand
```

## Card Components

### Metric Card
```csharp
Size = new Size(650, 100)
BackColor = Color.White
Padding = new Padding(20)

// Contains:
- Icon (40x40px, colored)
- Title (11px, gray)
- Value (20px Bold, dark)
```

### Product Card
```csharp
Size = new Size(120, 140)
BackColor = Color.White
Margin = new Padding(10)
Cursor = Cursors.Hand

// Contains:
- Image/Placeholder (120x80px)
- Name Label (9px Bold, truncated)
- Price Label (8px, gray)
```

### Cart Item Panel
```csharp
Size = new Size(390, 70)
BackColor = Color.FromArgb(248, 250, 252)
Margin = new Padding(0, 5, 0, 5)

// Contains:
- Product Name (10px Bold)
- Unit Price (9px, gray)
- Quantity Controls (30x30px buttons)
- Total Price (10px Bold)
```

## Icon Usage

### FontAwesome Sharp Icons
```
Dashboard: IconChar.ChartLine
Invoices: IconChar.FileInvoice
Payment Links: IconChar.Link
Online Store: IconChar.Store
POS: IconChar.CashRegister
Products: IconChar.Box
Clients: IconChar.UserFriends
Reports: IconChar.ChartBar

Actions:
- Add: IconChar.Plus
- Remove: IconChar.Times
- Save: IconChar.Save
- Search: IconChar.Search
- Print: IconChar.Print
- View: IconChar.Eye
- Email: IconChar.Envelope
- SMS: IconChar.CommentDots
- Success: IconChar.CheckCircle
- Barcode: IconChar.Barcode
- Money: IconChar.MoneyBillWave
```

### Icon Sizes
```
Large: 80px (Success modals)
Medium: 40px (Metric cards)
Standard: 24px (Headers, buttons)
Small: 20px (Inline icons)
Tiny: 18px (Menu items)
```

## Layout Patterns

### Dashboard Layout
```
Header (60-80px height)
├── Logo/Branding (left)
├── URL/Info (center)
└── Date/Time (right)

Content Area
├── Metrics Row (100px height)
│   ├── Metric Card 1 (50% width)
│   └── Metric Card 2 (50% width)
├── Statistics Section (340px height)
│   ├── Line Chart (60% width)
│   └── Donut Chart (40% width)
└── Bottom Row (280px height)
    ├── Latest Invoices (65% width)
    └── Account Summary (35% width)
```

### POS Layout
```
Header (80px height)
├── Logo
├── DateTime
└── Action Icons

Main Area (Split)
├── Products Panel (Left, Fill)
│   ├── Add Item Button
│   ├── Demo Product Button
│   └── Product Grid (FlowLayoutPanel)
└── Cart Panel (Right, 450px width)
    ├── Client Selector
    ├── Cart Items (FlowLayoutPanel)
    ├── Summary Panel
    └── Action Buttons
```

### Modal Layout
```
Payment Modal (600x500px)
├── Title (30px top margin)
├── Payment Tabs (80px from top)
├── Custom Value Input
├── Payment Note
├── Summary Panel (80px height)
└── Pay Button (55px height)

Success Modal (500x450px)
├── Teal Header (150px height)
│   ├── Checkmark Icon (centered)
│   └── Close Button (top-right)
├── Success Message
├── Invoice Details
├── Receipt Options (2x2 grid)
└── New Transaction Button
```

## Spacing System

### Margins
```
Extra Large: 30px (Page margins)
Large: 20px (Section spacing)
Medium: 15px (Component spacing)
Small: 10px (Element spacing)
Tiny: 5px (Tight spacing)
```

### Padding
```
Large: 20px (Panels, cards)
Medium: 15px (Buttons, inputs)
Small: 10px (Labels, small elements)
```

## Input Components

### TextBox
```csharp
Font = new Font("Segoe UI", 11-12)
Height = 30-35px
BackColor = Color.White
BorderStyle = BorderStyle.FixedSingle
```

### ComboBox
```csharp
Font = new Font("Segoe UI", 11)
Height = 30px
DropDownStyle = ComboBoxStyle.DropDownList
```

### DataGridView
```csharp
BackgroundColor = Color.White
BorderStyle = BorderStyle.None
Font = new Font("Segoe UI", 10)
RowHeadersVisible = false
SelectionMode = DataGridViewSelectionMode.FullRowSelect
```

## Chart Components

### Line Chart
```csharp
BackColor = Color.White
ChartArea.BackColor = Color.White
Series.BorderWidth = 3
Legend.Docking = Docking.Top

Colors:
- Active: #3B82F6 (Blue)
- Paid: #22C55E (Green)
- Void: #EF4444 (Red)
```

### Donut Chart
```csharp
ChartType = SeriesChartType.Doughnut
BackColor = Color.White
Legend.Docking = Docking.Bottom

Colors:
- Active: #3B82F6 (Blue)
- Paid: #22C55E (Green)
- Void: #EF4444 (Red)
```

## Status Indicators

### Invoice Status Colors
```
Active: Blue (#3B82F6)
Paid: Green (#22C55E)
Void: Red (#EF4444)
Overdue: Orange (#F59E0B)
```

### Payment Status Colors
```
Pending: Yellow (#EAB308)
Completed: Green (#22C55E)
Failed: Red (#EF4444)
Refunded: Purple (#A855F7)
Cancelled: Gray (#6B7280)
```

## Responsive Behavior

### Breakpoints
```
Large Desktop: > 1400px
Desktop: 1200-1400px
Tablet: 768-1200px
Mobile: < 768px (not optimized yet)
```

### Anchor Styles
```
Top-Left: Static positioning
Top-Right: Anchor to right edge
Bottom: Anchor to bottom
Fill: Dock.Fill for main content
```

## Animation & Transitions

### Hover Effects
```csharp
button.MouseEnter += (s, e) => 
{
    button.BackColor = DarkenColor(button.BackColor, 10);
};

button.MouseLeave += (s, e) => 
{
    button.BackColor = originalColor;
};
```

### Focus States
```
Input Focus: Border color changes to teal
Button Focus: Slight shadow effect
```

## Accessibility

### Contrast Ratios
```
Text on White: 4.5:1 minimum
Text on Teal: 4.5:1 minimum
Icons: 3:1 minimum
```

### Font Sizes
```
Minimum: 9px (micro text only)
Recommended: 11px+ for body text
Headings: 14px+ for readability
```

### Touch Targets
```
Minimum: 40x40px
Recommended: 50x50px for primary actions
Large: 60x60px for critical actions
```

## Best Practices

### Do's
✅ Use consistent spacing (10px, 20px, 30px)
✅ Maintain color consistency across views
✅ Use FontAwesome icons for visual clarity
✅ Apply hover states to interactive elements
✅ Use proper font weights for hierarchy
✅ Keep button text concise and clear
✅ Use placeholders for empty states

### Don'ts
❌ Mix different color schemes
❌ Use custom colors outside the palette
❌ Create buttons smaller than 40x40px
❌ Use more than 3 font sizes per view
❌ Overcrowd interfaces with too many elements
❌ Use low contrast color combinations
❌ Forget to add cursor: hand to clickable elements

---

**Reference Version:** 1.0
**Last Updated:** October 26, 2025
**Applies To:** Cashier Dashboard System
