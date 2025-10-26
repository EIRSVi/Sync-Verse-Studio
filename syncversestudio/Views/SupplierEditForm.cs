using SyncVerseStudio.Services;
using SyncVerseStudio.Data;
using SyncVerseStudio.Models;
using SyncVerseStudio.Helpers;
using Microsoft.EntityFrameworkCore;

namespace SyncVerseStudio.Views
{
    public partial class SupplierEditForm : Form
    {
 private readonly AuthenticationService _authService;
 private readonly ApplicationDbContext _context;
 private readonly int? _supplierId;
        private Supplier? _supplier;

        private TextBox nameTextBox, contactPersonTextBox, phoneTextBox, emailTextBox, addressTextBox;
        private CheckBox isActiveCheckBox;
        private Button saveButton, cancelButton;
  private Label titleLabel, productCountLabel;

        public SupplierEditForm(AuthenticationService authService, int? supplierId = null)
        {
            _authService = authService;
      _context = new ApplicationDbContext();
  _supplierId = supplierId;
            
      InitializeComponent();
     
            if (_supplierId.HasValue)
      {
    LoadSupplier();
        }
        }

        private void InitializeComponent()
        {
            this.AutoScaleDimensions = new SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.White;
            this.ClientSize = new Size(600, 550);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Name = "SupplierEditForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = _supplierId.HasValue ? "Edit Supplier" : "Add Supplier";

            CreateHeaderPanel();
            CreateControls();
        }

        private void CreateHeaderPanel()
        {
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = BrandTheme.CoolWhite
            };

            // Make header draggable
            Point mouseOffset = Point.Empty;
            headerPanel.MouseDown += (s, e) =>
            {
                mouseOffset = new Point(-e.X, -e.Y);
            };
            headerPanel.MouseMove += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    Point mousePos = Control.MousePosition;
                    mousePos.Offset(mouseOffset.X, mouseOffset.Y);
                    this.Location = mousePos;
                }
            };

            // Close Button
            var btnClose = new Button
            {
                Text = "✕",
                Size = new Size(40, 40),
                Location = new Point(this.ClientSize.Width - 45, 10),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = BrandTheme.SecondaryText,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 240, 240);
            btnClose.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            headerPanel.Controls.Add(btnClose);

            // Title in header
            var headerTitle = new Label
            {
                Text = _supplierId.HasValue ? "Edit Supplier" : "Add New Supplier",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = BrandTheme.PrimaryText,
                Location = new Point(20, 15),
                Size = new Size(400, 30),
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(headerTitle);

            this.Controls.Add(headerPanel);
        }

        private void CreateControls()
        {
            int yPos = 80; // Start below header
            int leftMargin = 30;
            int controlWidth = 540;
            int labelHeight = 25;
            int controlHeight = 30;
            int spacing = 15;

        // Supplier Name
        AddLabel("Supplier Name *", leftMargin, yPos);
     yPos += labelHeight + 5;
     nameTextBox = new TextBox
     {
       Location = new Point(leftMargin, yPos),
    Size = new Size(controlWidth, controlHeight),
        Font = new Font("Segoe UI", 10F)
      };
       this.Controls.Add(nameTextBox);
            yPos += controlHeight + spacing;

            // Contact Person
    AddLabel("Contact Person *", leftMargin, yPos);
      yPos += labelHeight + 5;
            contactPersonTextBox = new TextBox
            {
                Location = new Point(leftMargin, yPos),
                Size = new Size(controlWidth, controlHeight),
    Font = new Font("Segoe UI", 10F)
      };
 this.Controls.Add(contactPersonTextBox);
 yPos += controlHeight + spacing;

            // Phone
      AddLabel("Phone", leftMargin, yPos);
     yPos += labelHeight + 5;
            phoneTextBox = new TextBox
          {
       Location = new Point(leftMargin, yPos),
    Size = new Size(controlWidth, controlHeight),
    Font = new Font("Segoe UI", 10F)
            };
this.Controls.Add(phoneTextBox);
     yPos += controlHeight + spacing;

   // Email
  AddLabel("Email", leftMargin, yPos);
       yPos += labelHeight + 5;
            emailTextBox = new TextBox
            {
         Location = new Point(leftMargin, yPos),
Size = new Size(controlWidth, controlHeight),
     Font = new Font("Segoe UI", 10F)
            };
          this.Controls.Add(emailTextBox);
            yPos += controlHeight + spacing;

            // Address
            AddLabel("Address", leftMargin, yPos);
        yPos += labelHeight + 5;
        addressTextBox = new TextBox
{
   Location = new Point(leftMargin, yPos),
          Size = new Size(controlWidth, controlHeight * 2),
       Font = new Font("Segoe UI", 10F),
        Multiline = true,
       ScrollBars = ScrollBars.Vertical
    };
       this.Controls.Add(addressTextBox);
  yPos += (controlHeight * 2) + spacing;

   // Is Active
   isActiveCheckBox = new CheckBox
       {
          Text = "Active Supplier",
  Location = new Point(leftMargin, yPos),
   Size = new Size(200, 25),
   Font = new Font("Segoe UI", 10F),
    Checked = true
       };
          this.Controls.Add(isActiveCheckBox);
            yPos += 35;

  // Product Count (for edit mode)
    if (_supplierId.HasValue)
            {
    productCountLabel = new Label
 {
         Text = "Loading product count...",
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
      ForeColor = Color.FromArgb(100, 116, 139),
     Location = new Point(leftMargin, yPos),
           Size = new Size(controlWidth, 25)
     };
  this.Controls.Add(productCountLabel);
        yPos += 35;
   }
            else
            {
        yPos += 10;
   }

         // Buttons
 cancelButton = new Button
    {
          Text = "Cancel",
              Location = new Point(leftMargin + 390, yPos),
    Size = new Size(80, 35),
         BackColor = Color.FromArgb(158, 158, 158),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
         Font = new Font("Segoe UI", 10F, FontStyle.Bold),
      DialogResult = DialogResult.Cancel
        };
   cancelButton.FlatAppearance.BorderSize = 0;
            this.Controls.Add(cancelButton);

 saveButton = new Button
     {
      Text = "Save",
       Location = new Point(leftMargin + 480, yPos),
    Size = new Size(80, 35),
    BackColor = Color.FromArgb(24, 119, 18),
         ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold)
       };
      saveButton.FlatAppearance.BorderSize = 0;
          saveButton.Click += SaveButton_Click;
            this.Controls.Add(saveButton);

        this.CancelButton = cancelButton;
            this.AcceptButton = saveButton;
        }

        private void AddLabel(string text, int x, int y)
        {
       var label = new Label
         {
       Text = text,
        Font = new Font("Segoe UI", 10F, FontStyle.Regular),
    ForeColor = Color.FromArgb(64, 64, 64),
         Location = new Point(x, y),
     Size = new Size(540, 25)
            };
            this.Controls.Add(label);
        }

        private async void LoadSupplier()
        {
       try
            {
                _supplier = await _context.Suppliers
        .FirstOrDefaultAsync(s => s.Id == _supplierId);

        if (_supplier != null)
     {
    nameTextBox.Text = _supplier.Name ?? "";
 contactPersonTextBox.Text = _supplier.ContactPerson ?? "";
          phoneTextBox.Text = _supplier.Phone ?? "";
         emailTextBox.Text = _supplier.Email ?? "";
 addressTextBox.Text = _supplier.Address ?? "";
         isActiveCheckBox.Checked = _supplier.IsActive;

                    // Load product count
           await LoadProductCount();
      }
      }
   catch (Exception ex)
            {
       MessageBox.Show($"Error loading supplier: {ex.Message}", "Error", 
     MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

   private async Task LoadProductCount()
        {
     if (!_supplierId.HasValue || productCountLabel == null) return;

   try
         {
            var productCount = await _context.Products
        .CountAsync(p => p.SupplierId == _supplierId);

        productCountLabel.Text = $"Associated Products: {productCount}";
}
            catch (Exception ex)
    {
  productCountLabel.Text = "Unable to load product count";
    Console.WriteLine($"Error loading product count: {ex.Message}");
            }
     }

    private async void SaveButton_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
      return;

            try
       {
        saveButton.Enabled = false;
        saveButton.Text = "Saving...";

        if (_supplier == null)
                {
         // Create new supplier
              _supplier = new Supplier
   {
           Name = nameTextBox.Text.Trim(),
        ContactPerson = string.IsNullOrEmpty(contactPersonTextBox.Text.Trim()) ? null : contactPersonTextBox.Text.Trim(),
    Phone = string.IsNullOrEmpty(phoneTextBox.Text.Trim()) ? null : phoneTextBox.Text.Trim(),
    Email = string.IsNullOrEmpty(emailTextBox.Text.Trim()) ? null : emailTextBox.Text.Trim(),
            Address = string.IsNullOrEmpty(addressTextBox.Text.Trim()) ? null : addressTextBox.Text.Trim(),
           IsActive = isActiveCheckBox.Checked,
      CreatedAt = DateTime.Now
        };

  _context.Suppliers.Add(_supplier);
                 await _context.SaveChangesAsync();

         await _authService.LogAuditAsync("ADD_SUPPLIER", "Suppliers", _supplier.Id, null,
          $"Added new supplier: {_supplier.Name}");
           }
    else
    {
   // Update existing supplier
        var oldValues = $"Name: {_supplier.Name}, Contact: {_supplier.ContactPerson}, Active: {_supplier.IsActive}";

      _supplier.Name = nameTextBox.Text.Trim();
       _supplier.ContactPerson = string.IsNullOrEmpty(contactPersonTextBox.Text.Trim()) ? null : contactPersonTextBox.Text.Trim();
       _supplier.Phone = string.IsNullOrEmpty(phoneTextBox.Text.Trim()) ? null : phoneTextBox.Text.Trim();
     _supplier.Email = string.IsNullOrEmpty(emailTextBox.Text.Trim()) ? null : emailTextBox.Text.Trim();
   _supplier.Address = string.IsNullOrEmpty(addressTextBox.Text.Trim()) ? null : addressTextBox.Text.Trim();
         _supplier.IsActive = isActiveCheckBox.Checked;

         await _context.SaveChangesAsync();

                 var newValues = $"Name: {_supplier.Name}, Contact: {_supplier.ContactPerson}, Active: {_supplier.IsActive}";
             await _authService.LogAuditAsync("EDIT_SUPPLIER", "Suppliers", _supplier.Id, oldValues, newValues);
        }

       MessageBox.Show("Supplier saved successfully!", "Success", 
          MessageBoxButtons.OK, MessageBoxIcon.Information);

          this.DialogResult = DialogResult.OK;
         this.Close();
        }
catch (Exception ex)
    {
     MessageBox.Show($"Error saving supplier: {ex.Message}", "Error", 
        MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
      finally
         {
                saveButton.Enabled = true;
    saveButton.Text = "Save";
        }
        }

        private bool ValidateInput()
        {
 if (string.IsNullOrWhiteSpace(nameTextBox.Text))
            {
    MessageBox.Show("Supplier name is required.", "Validation Error", 
              MessageBoxButtons.OK, MessageBoxIcon.Warning);
          nameTextBox.Focus();
      return false;
}

 if (string.IsNullOrWhiteSpace(contactPersonTextBox.Text))
 {
                MessageBox.Show("Contact person is required.", "Validation Error", 
         MessageBoxButtons.OK, MessageBoxIcon.Warning);
             contactPersonTextBox.Focus();
 return false;
        }

          // Validate email format if provided
        if (!string.IsNullOrWhiteSpace(emailTextBox.Text))
      {
                try
       {
      var addr = new System.Net.Mail.MailAddress(emailTextBox.Text);
        if (addr.Address != emailTextBox.Text)
      throw new FormatException();
                }
           catch
          {
     MessageBox.Show("Please enter a valid email address.", "Validation Error", 
      MessageBoxButtons.OK, MessageBoxIcon.Warning);
  emailTextBox.Focus();
   return false;
            }
      }

 return true;
        }

  protected override void Dispose(bool disposing)
        {
        if (disposing)
            {
           _context?.Dispose();
            }
        base.Dispose(disposing);
        }
    }
}
