# SyncVerse Studio - Brand Theme Guide

## Overview
This guide explains the unified design system for consistent UI across all views.

## Color Palette

### Primary Colors
- **Background**: White (#FFFFFF) - Clean white for all pages
- **Primary Text**: Almost Black (#1E1E1E)
- **Secondary Text**: Gray (#646464)
- **Accent Green**: #22C55E - Success, Add buttons
- **Accent Blue**: #3B82F6 - Edit buttons, Info

### Button Colors
- **Add/Create/Save**: Green (#22C55E)
- **Edit/Update**: Blue (#3B82F6)
- **Delete/Remove**: Red (#EF4444)
- **Refresh/Cancel**: Gray (#6B7280)
- **Export/Print**: Purple (#A855F7)

### Table Colors
- **Header Background**: Very Light Gray (#F9FAFB)
- **Header Text**: Dark (#1E1E1E)
- **Row Even**: White
- **Row Odd**: Light Gray (#F9FAFB)
- **Row Hover**: Gray (#F3F4F6)
- **Row Selected**: Light Blue (#DBEAFE)
- **Border**: Light Gray (#E5E7EB)

## Usage Examples

### 1. Creating a Standard CRUD View

```csharp
public partial class YourManagementView : Form
{
    public YourManagementView()
    {
        InitializeComponent();
        SetupUI();
    }

    private void SetupUI()
    {
        // Main container
        this.BackColor = BrandTheme.Background;
        this.Padding = new Padding(0);

        // Page header
        var header = BrandTheme.CreatePageHeader(
            "Your Management", 
            "Manage your items here"
        );
        this.Controls.Add(header);

        // Button panel
        var buttonPanel = BrandTheme.CreateButtonPanel(
            ("Add Item", "add", AddButton_Click),
            ("Edit", "edit", EditButton_Click),
            ("Delete", "delete", DeleteButton_Click),
            ("Refresh", "refresh", RefreshButton_Click)
        );
        this.Controls.Add(buttonPanel);

        // Search panel
        var searchBox = new TextBox();
        var searchPanel = BrandTheme.CreateSearchPanel(searchBox);
        this.Controls.Add(searchPanel);

        // Content container with table
        var contentPanel = BrandTheme.CreateContentContainer();
        var dataGrid = new DataGridView { Dock = DockStyle.Fill };
        BrandTheme.StyleDataGridView(dataGrid);
        contentPanel.Controls.Add(dataGrid);
        this.Controls.Add(contentPanel);
    }
}
```

### 2. Styling Individual Buttons

```csharp
// Add button (Green)
var addBtn = new Button { Text = "Add Product" };
BrandTheme.StyleButton(addBtn, "add");

// Edit button (Blue)
var editBtn = new Button { Text = "Edit" };
BrandTheme.StyleButton(editBtn, "edit");

// Delete button (Red)
var deleteBtn = new Button { Text = "Delete" };
BrandTheme.StyleButton(deleteBtn, "delete");

// Refresh button (Gray)
var refreshBtn = new Button { Text = "Refresh" };
BrandTheme.StyleButton(refreshBtn, "refresh");
```

### 3. Styling DataGridView

```csharp
var dataGrid = new DataGridView();
BrandTheme.StyleDataGridView(dataGrid);

// The method automatically applies:
// - Clean white background
// - Light gray headers
// - Alternating row colors
// - Hover effects
// - Selection highlighting
// - Proper fonts and padding
```

### 4. Creating Custom Buttons with Brand Colors

```csharp
var button = new Button
{
    Text = "Custom Action",
    BackColor = BrandTheme.ButtonAdd,  // or ButtonEdit, ButtonDelete, etc.
    ForeColor = BrandTheme.ButtonText,
    FlatStyle = FlatStyle.Flat,
    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
    Size = new Size(120, 36)
};
button.FlatAppearance.BorderSize = 0;
```

## Layout Structure

All CRUD views should follow this structure:

```
┌─────────────────────────────────────────┐
│ Page Header (Title + Subtitle)         │ ← 80-100px height
├─────────────────────────────────────────┤
│ Button Panel (Add, Edit, Delete, etc)  │ ← 60px height
├─────────────────────────────────────────┤
│ Search Panel (Search box + filters)    │ ← 60px height
├─────────────────────────────────────────┤
│                                         │
│ Content Area (DataGridView)            │ ← Fill remaining
│                                         │
│                                         │
└─────────────────────────────────────────┘
```

## Consistent Spacing

- **Page Padding**: 40px left/right, 20px top, 40px bottom
- **Button Spacing**: 10px between buttons
- **Panel Heights**: 
  - Header: 80-100px
  - Button Panel: 60px
  - Search Panel: 60px
- **Button Size**: 120px width × 36px height
- **Table Row Height**: 50px
- **Table Header Height**: 45px

## Font Guidelines

- **Page Title**: Segoe UI, 24pt, Bold
- **Page Subtitle**: Segoe UI, 11pt, Regular
- **Buttons**: Segoe UI, 10pt, Bold
- **Table Headers**: Segoe UI, 10pt, Bold
- **Table Data**: Segoe UI, 10pt, Regular
- **Labels**: Segoe UI, 10pt, Regular

## Quick Migration Checklist

To update an existing view to use the new theme:

1. ✅ Replace page background with `BrandTheme.Background`
2. ✅ Use `BrandTheme.CreatePageHeader()` for title
3. ✅ Use `BrandTheme.CreateButtonPanel()` for action buttons
4. ✅ Use `BrandTheme.StyleDataGridView()` for tables
5. ✅ Use `BrandTheme.StyleButton()` for individual buttons
6. ✅ Remove any custom colors - use theme colors
7. ✅ Ensure consistent padding (40px sides)
8. ✅ Test hover and selection states

## Result

Following this theme will give you:
- ✨ Clean, professional white design
- 🎨 Consistent colors across all views
- 📊 Beautiful, readable tables
- 🔘 Clear, accessible buttons
- 📱 Responsive layouts
- 🎯 Better user experience
