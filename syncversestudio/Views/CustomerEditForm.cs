using SyncVerseStudio.Services;
using SyncVerseStudio.Data;
using SyncVerseStudio.Models;
using SyncVerseStudio.Helpers;
using Microsoft.EntityFrameworkCore;

namespace SyncVerseStudio.Views
{
    public partial class CustomerEditForm : Form
    {
        private readonly AuthenticationService _authService;
        private readonly ApplicationDbContext _context;
        private readonly int? _customerId;
    private Customer? _customer;

 private TextBox firstNameTextBox, lastNameTextBox, emailTextBox, phoneTextBox, addressTextBox;
        private NumericUpDown loyaltyPointsNumeric;
   private Button saveButton, cancelButton;
        private Label titleLabel, purchaseHistoryLabel;

      public CustomerEditForm(AuthenticationService authService, int? customerId = null)
    {
   _authService = authService;
          _context = new ApplicationDbContext();
            _customerId = customerId;
        
            InitializeComponent();
 
            if (_customerId.HasValue)
         {
    LoadCustomer();
            }
     }

        private void InitializeComponent()
  {
         this.AutoScaleDimensions = new SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
          this.BackColor = Color.White;
    this.ClientSize = new Size(600, 600);
    this.FormBorderStyle = FormBorderStyle.FixedDialog;
          this.MaximizeBox = false;
   this.MinimizeBox = false;
            this.Name = "CustomerEditForm";
  this.StartPosition = FormStartPosition.CenterParent;
            this.Text = _customerId.HasValue ? "Edit Customer" : "Add Customer";

      CreateControls();
        }

    private void CreateControls()
     {
      int yPos = 20;
   int leftMargin = 30;
            int controlWidth = 540;
     int labelHeight = 25;
            int controlHeight = 30;
            int spacing = 15;

            // Title
      titleLabel = new Label
            {
                Text = _customerId.HasValue ? "Edit Customer" : "Add New Customer",
        Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
         Location = new Point(leftMargin, yPos),
      Size = new Size(400, 30)
        };
       this.Controls.Add(titleLabel);
   yPos += 50;

            // First Name
    AddLabel("First Name *", leftMargin, yPos);
 yPos += labelHeight + 5;
   firstNameTextBox = new TextBox
            {
                Location = new Point(leftMargin, yPos),
     Size = new Size(controlWidth, controlHeight),
 Font = new Font("Segoe UI", 10F)
            };
         this.Controls.Add(firstNameTextBox);
            yPos += controlHeight + spacing;

  // Last Name
            AddLabel("Last Name *", leftMargin, yPos);
      yPos += labelHeight + 5;
            lastNameTextBox = new TextBox
            {
                Location = new Point(leftMargin, yPos),
      Size = new Size(controlWidth, controlHeight),
     Font = new Font("Segoe UI", 10F)
  };
            this.Controls.Add(lastNameTextBox);
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

    // Loyalty Points
  AddLabel("Loyalty Points", leftMargin, yPos);
     yPos += labelHeight + 5;
loyaltyPointsNumeric = new NumericUpDown
            {
    Location = new Point(leftMargin, yPos),
    Size = new Size(150, controlHeight),
        Font = new Font("Segoe UI", 10F),
    Maximum = 999999,
        Minimum = 0,
              Value = 0
    };
      this.Controls.Add(loyaltyPointsNumeric);
  yPos += controlHeight + spacing;

  // Purchase History (for edit mode)
     if (_customerId.HasValue)
            {
                purchaseHistoryLabel = new Label
             {
           Text = "Loading purchase history...",
          Font = new Font("Segoe UI", 9F, FontStyle.Italic),
         ForeColor = Color.FromArgb(100, 116, 139),
  Location = new Point(leftMargin, yPos),
    Size = new Size(controlWidth, 40)
 };
                this.Controls.Add(purchaseHistoryLabel);
        yPos += 50;
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

        private async void LoadCustomer()
        {
 try
            {
         _customer = await _context.Customers
     .FirstOrDefaultAsync(c => c.Id == _customerId);

           if (_customer != null)
        {
                firstNameTextBox.Text = _customer.FirstName ?? "";
 lastNameTextBox.Text = _customer.LastName ?? "";
 emailTextBox.Text = _customer.DecryptedEmail ?? "";
        phoneTextBox.Text = _customer.DecryptedPhone ?? "";
        addressTextBox.Text = _customer.Address ?? "";
       loyaltyPointsNumeric.Value = _customer.LoyaltyPoints;

      // Load purchase history
           await LoadPurchaseHistory();
          }
         }
            catch (Exception ex)
            {
       MessageBox.Show($"Error loading customer: {ex.Message}", "Error", 
   MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
     }

     private async Task LoadPurchaseHistory()
        {
            if (!_customerId.HasValue || purchaseHistoryLabel == null) return;

            try
            {
         var totalPurchases = await _context.Sales
     .Where(s => s.CustomerId == _customerId && s.Status == SaleStatus.Completed)
          .SumAsync(s => (decimal?)s.TotalAmount) ?? 0;

           var purchaseCount = await _context.Sales
.Where(s => s.CustomerId == _customerId && s.Status == SaleStatus.Completed)
         .CountAsync();

      var lastPurchase = await _context.Sales
 .Where(s => s.CustomerId == _customerId && s.Status == SaleStatus.Completed)
             .MaxAsync(s => (DateTime?)s.SaleDate);

  purchaseHistoryLabel.Text = $"Total Purchases: ${totalPurchases:N2} | Orders: {purchaseCount} | " +
           $"Last Purchase: {(lastPurchase.HasValue ? lastPurchase.Value.ToString("MM/dd/yyyy") : "Never")}";
    }
         catch (Exception ex)
     {
    purchaseHistoryLabel.Text = "Unable to load purchase history";
   Console.WriteLine($"Error loading purchase history: {ex.Message}");
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

            if (_customer == null)
    {
// Create new customer
   _customer = new Customer
    {
        FirstName = firstNameTextBox.Text.Trim(),
        LastName = lastNameTextBox.Text.Trim(),
             Email = string.IsNullOrEmpty(emailTextBox.Text.Trim()) ? null : EncryptionHelper.Encrypt(emailTextBox.Text.Trim()),
  Phone = string.IsNullOrEmpty(phoneTextBox.Text.Trim()) ? null : EncryptionHelper.Encrypt(phoneTextBox.Text.Trim()),
    Address = string.IsNullOrEmpty(addressTextBox.Text.Trim()) ? null : addressTextBox.Text.Trim(),
          LoyaltyPoints = (int)loyaltyPointsNumeric.Value,
    CreatedAt = DateTime.Now
         };

      _context.Customers.Add(_customer);
       await _context.SaveChangesAsync();

         await _authService.LogAuditAsync("ADD_CUSTOMER", "Customers", _customer.Id, null,
       $"Added new customer: {_customer.FullName}");
       }
          else
           {
           // Update existing customer
        var oldValues = $"Name: {_customer.FullName}, Email: {_customer.MaskedEmail}, Phone: {_customer.MaskedPhone}";

       _customer.FirstName = firstNameTextBox.Text.Trim();
            _customer.LastName = lastNameTextBox.Text.Trim();
         _customer.Email = string.IsNullOrEmpty(emailTextBox.Text.Trim()) ? null : EncryptionHelper.Encrypt(emailTextBox.Text.Trim());
          _customer.Phone = string.IsNullOrEmpty(phoneTextBox.Text.Trim()) ? null : EncryptionHelper.Encrypt(phoneTextBox.Text.Trim());
         _customer.Address = string.IsNullOrEmpty(addressTextBox.Text.Trim()) ? null : addressTextBox.Text.Trim();
         _customer.LoyaltyPoints = (int)loyaltyPointsNumeric.Value;

       await _context.SaveChangesAsync();

    var newValues = $"Name: {_customer.FullName}, Email: {_customer.Email}, Phone: {_customer.Phone}";
     await _authService.LogAuditAsync("EDIT_CUSTOMER", "Customers", _customer.Id, oldValues, newValues);
     }

    MessageBox.Show("Customer saved successfully!", "Success", 
             MessageBoxButtons.OK, MessageBoxIcon.Information);

       this.DialogResult = DialogResult.OK;
        this.Close();
          }
       catch (Exception ex)
            {
     MessageBox.Show($"Error saving customer: {ex.Message}", "Error", 
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
            if (string.IsNullOrWhiteSpace(firstNameTextBox.Text))
  {
              MessageBox.Show("First name is required.", "Validation Error", 
   MessageBoxButtons.OK, MessageBoxIcon.Warning);
firstNameTextBox.Focus();
            return false;
            }

  if (string.IsNullOrWhiteSpace(lastNameTextBox.Text))
  {
       MessageBox.Show("Last name is required.", "Validation Error", 
         MessageBoxButtons.OK, MessageBoxIcon.Warning);
 lastNameTextBox.Focus();
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
