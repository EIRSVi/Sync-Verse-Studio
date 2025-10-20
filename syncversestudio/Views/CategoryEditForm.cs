using System.Drawing;
using SyncVerseStudio.Services;
using SyncVerseStudio.Data;
using SyncVerseStudio.Models;
using Microsoft.EntityFrameworkCore;
using FontAwesome.Sharp;

namespace SyncVerseStudio.Views
{
    public partial class CategoryEditForm : Form
    {
        private readonly AuthenticationService _authService;
        private readonly ApplicationDbContext _context;
        private readonly int? _categoryId;
        private Category? _category;

        private TextBox nameTextBox, descriptionTextBox;
        private CheckBox isActiveCheckBox;
        private IconButton saveButton, cancelButton;
        private Label titleLabel, nameLabel, descriptionLabel;
        private IconPictureBox titleIcon, nameIcon, descIcon, statusIconPic;
        private Panel mainPanel, buttonPanel;

        public CategoryEditForm(AuthenticationService authService, int? categoryId = null)
        {
            _authService = authService;
            _context = new ApplicationDbContext();
            _categoryId = categoryId;
            
            InitializeComponent();
            
            if (_categoryId.HasValue)
            {
                LoadCategory();
            }
        }

        private void InitializeComponent()
        {
            this.AutoScaleDimensions = new SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            this.ClientSize = new Size(480, 450);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CategoryEditForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = _categoryId.HasValue ? "Edit Category" : "Add Category";
            this.Font = new System.Drawing.Font("Segoe UI", 10F);

            CreateControls();
        }

        private void CreateControls()
        {
            // Main Panel
            mainPanel = new Panel
            {
                BackColor = System.Drawing.Color.White,
                Location = new Point(15, 15),
                Size = new Size(450, 380),
                Padding = new Padding(30)
            };

            int yPos = 20;
            int leftMargin = 30;
            int controlWidth = 390;
            int labelHeight = 30;
            int controlHeight = 35;
            int spacing = 25;

            // Title Section with Icon
            titleIcon = new IconPictureBox
            {
                IconChar = IconChar.Tags,
                IconColor = System.Drawing.Color.FromArgb(24, 119, 18),
                IconSize = 32,
                Location = new Point(leftMargin, yPos),
                Size = new Size(40, 40),
                BackColor = System.Drawing.Color.Transparent
            };
            mainPanel.Controls.Add(titleIcon);

            titleLabel = new Label
            {
                Text = _categoryId.HasValue ? "Edit Category" : "Create New Category",
                Font = new System.Drawing.Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(33, 33, 33),
                Location = new Point(leftMargin + 50, yPos + 5),
                Size = new Size(340, 35)
            };
            mainPanel.Controls.Add(titleLabel);
            yPos += 65;

            // Category Name Section with Icon
            nameIcon = new IconPictureBox
            {
                IconChar = IconChar.Tag,
                IconColor = System.Drawing.Color.FromArgb(64, 64, 64),
                IconSize = 16,
                Location = new Point(leftMargin, yPos + 5),
                Size = new Size(20, 20),
                BackColor = System.Drawing.Color.Transparent
            };
            mainPanel.Controls.Add(nameIcon);

            nameLabel = new Label
            {
                Text = "Category Name",
                Font = new System.Drawing.Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(64, 64, 64),
                Location = new Point(leftMargin + 25, yPos + 3),
                Size = new Size(150, 25)
            };
            mainPanel.Controls.Add(nameLabel);

            // Required indicator
            var requiredIcon = new IconPictureBox
            {
                IconChar = IconChar.Asterisk,
                IconColor = System.Drawing.Color.FromArgb(255, 0, 80),
                IconSize = 10,
                Location = new Point(leftMargin + 175, yPos + 5),
                Size = new Size(15, 15),
                BackColor = System.Drawing.Color.Transparent
            };
            mainPanel.Controls.Add(requiredIcon);

            yPos += labelHeight + 5;

            nameTextBox = new TextBox
            {
                Location = new Point(leftMargin, yPos),
                Size = new Size(controlWidth, controlHeight),
                Font = new System.Drawing.Font("Segoe UI", 11F),
                MaxLength = 100,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10)
            };
            mainPanel.Controls.Add(nameTextBox);
            yPos += controlHeight + spacing;

            // Description Section with Icon
            descIcon = new IconPictureBox
            {
                IconChar = IconChar.FileText,
                IconColor = System.Drawing.Color.FromArgb(64, 64, 64),
                IconSize = 16,
                Location = new Point(leftMargin, yPos + 5),
                Size = new Size(20, 20),
                BackColor = System.Drawing.Color.Transparent
            };
            mainPanel.Controls.Add(descIcon);

            descriptionLabel = new Label
            {
                Text = "Description (Optional)",
                Font = new System.Drawing.Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(64, 64, 64),
                Location = new Point(leftMargin + 25, yPos + 3),
                Size = new Size(200, 25)
            };
            mainPanel.Controls.Add(descriptionLabel);

            yPos += labelHeight + 5;

            descriptionTextBox = new TextBox
            {
                Location = new Point(leftMargin, yPos),
                Size = new Size(controlWidth, controlHeight * 2),
                Font = new System.Drawing.Font("Segoe UI", 11F),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                MaxLength = 255,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10)
            };
            mainPanel.Controls.Add(descriptionTextBox);
            yPos += (controlHeight * 2) + spacing;

            // Active Status Section with Icon
            var statusPanel = new Panel
            {
                Location = new Point(leftMargin, yPos),
                Size = new Size(controlWidth, 35),
                BackColor = System.Drawing.Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle
            };

            statusIconPic = new IconPictureBox
            {
                IconChar = IconChar.ToggleOn,
                IconColor = System.Drawing.Color.FromArgb(24, 119, 18),
                IconSize = 24,
                Location = new Point(15, 5),
                Size = new Size(28, 28),
                BackColor = System.Drawing.Color.Transparent
            };
            statusPanel.Controls.Add(statusIconPic);

            isActiveCheckBox = new CheckBox
            {
                Text = "Category is Active",
                Font = new System.Drawing.Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(33, 33, 33),
                Location = new Point(50, 8),
                Size = new Size(200, 25),
                Checked = true,
                BackColor = System.Drawing.Color.Transparent,
                FlatStyle = FlatStyle.Flat
            };

            // Update icon based on checkbox state
            isActiveCheckBox.CheckedChanged += (s, e) =>
            {
                statusIconPic.IconChar = isActiveCheckBox.Checked ? IconChar.ToggleOn : IconChar.ToggleOff;
                statusIconPic.IconColor = isActiveCheckBox.Checked ? 
                    System.Drawing.Color.FromArgb(24, 119, 18) : 
                    System.Drawing.Color.FromArgb(158, 158, 158);
            };

            statusPanel.Controls.Add(isActiveCheckBox);
            mainPanel.Controls.Add(statusPanel);

            // Button Panel
            buttonPanel = new Panel
            {
                BackColor = System.Drawing.Color.FromArgb(248, 249, 250),
                Location = new Point(15, 400),
                Size = new Size(450, 60),
                Padding = new Padding(15)
            };

            // Cancel Button with Icon
            cancelButton = new IconButton
            {
                Text = "  Cancel",
                IconChar = IconChar.Times,
                IconColor = System.Drawing.Color.White,
                IconSize = 16,
                Location = new Point(250, 15),
                Size = new Size(90, 40),
                BackColor = System.Drawing.Color.FromArgb(158, 158, 158),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new System.Drawing.Font("Segoe UI", 10F, FontStyle.Bold),
                DialogResult = DialogResult.Cancel,
                TextAlign = ContentAlignment.MiddleRight,
                ImageAlign = ContentAlignment.MiddleLeft,
                Cursor = Cursors.Hand
            };
            cancelButton.FlatAppearance.BorderSize = 0;
            cancelButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(140, 140, 140);
            buttonPanel.Controls.Add(cancelButton);

            // Save Button with Icon
            saveButton = new IconButton
            {
                Text = "  Save",
                IconChar = IconChar.Save,
                IconColor = System.Drawing.Color.White,
                IconSize = 16,
                Location = new Point(350, 15),
                Size = new Size(90, 40),
                BackColor = System.Drawing.Color.FromArgb(24, 119, 18),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new System.Drawing.Font("Segoe UI", 10F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleRight,
                ImageAlign = ContentAlignment.MiddleLeft,
                Cursor = Cursors.Hand
            };
            saveButton.FlatAppearance.BorderSize = 0;
            saveButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(20, 100, 15);
            saveButton.Click += SaveButton_Click;
            buttonPanel.Controls.Add(saveButton);

            this.Controls.Add(mainPanel);
            this.Controls.Add(buttonPanel);
            this.CancelButton = cancelButton;
            this.AcceptButton = saveButton;

            // Add subtle hover effects for text boxes with icon feedback
            nameTextBox.Enter += (s, e) => 
            {
                nameTextBox.BackColor = System.Drawing.Color.FromArgb(240, 248, 255);
                nameIcon.IconColor = System.Drawing.Color.FromArgb(24, 119, 18);
            };
            nameTextBox.Leave += (s, e) => 
            {
                nameTextBox.BackColor = System.Drawing.Color.White;
                nameIcon.IconColor = System.Drawing.Color.FromArgb(64, 64, 64);
            };
            
            descriptionTextBox.Enter += (s, e) => 
            {
                descriptionTextBox.BackColor = System.Drawing.Color.FromArgb(240, 248, 255);
                descIcon.IconColor = System.Drawing.Color.FromArgb(24, 119, 18);
            };
            descriptionTextBox.Leave += (s, e) => 
            {
                descriptionTextBox.BackColor = System.Drawing.Color.White;
                descIcon.IconColor = System.Drawing.Color.FromArgb(64, 64, 64);
            };
        }

        private async void LoadCategory()
        {
            try
            {
                // Show loading state with spinner icon
                saveButton.Text = "  Loading...";
                saveButton.IconChar = IconChar.Spinner;
                saveButton.Enabled = false;
                titleIcon.IconChar = IconChar.HourglassHalf;

                _category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == _categoryId);

                if (_category != null)
                {
                    nameTextBox.Text = _category.Name;
                    descriptionTextBox.Text = _category.Description ?? "";
                    isActiveCheckBox.Checked = _category.IsActive;

                    // Update title for edit mode
                    titleLabel.Text = $"Edit Category: {_category.Name}";
                    titleIcon.IconChar = IconChar.Edit;
                }
            }
            catch (Exception ex)
            {
                // Show error with icon
                titleIcon.IconChar = IconChar.ExclamationTriangle;
                titleIcon.IconColor = System.Drawing.Color.FromArgb(255, 0, 80);
                
                MessageBox.Show($"Error loading category: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Reset button state
                saveButton.Text = "  Save";
                saveButton.IconChar = IconChar.Save;
                saveButton.Enabled = true;
                
                if (_category != null && titleIcon.IconChar != IconChar.Edit)
                {
                    titleIcon.IconChar = IconChar.Tags;
                    titleIcon.IconColor = System.Drawing.Color.FromArgb(24, 119, 18);
                }
            }
        }

        private async void SaveButton_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                // Show saving state with spinner animation
                saveButton.Enabled = false;
                saveButton.Text = "  Saving...";
                saveButton.IconChar = IconChar.Spinner;
                saveButton.BackColor = System.Drawing.Color.FromArgb(37, 99, 102);
                titleIcon.IconChar = IconChar.CloudUploadAlt;

                if (_category == null)
                {
                    // Create new category
                    _category = new Category
                    {
                        Name = nameTextBox.Text.Trim(),
                        Description = string.IsNullOrWhiteSpace(descriptionTextBox.Text) ? null : descriptionTextBox.Text.Trim(),
                        IsActive = isActiveCheckBox.Checked,
                        CreatedAt = DateTime.Now
                    };

                    _context.Categories.Add(_category);
                    await _context.SaveChangesAsync();

                    // Log the creation
                    await _authService.LogAuditAsync("CATEGORY_CREATED", "Categories", _category.Id, null,
                        $"Category '{_category.Name}' created - Status: {(isActiveCheckBox.Checked ? "Active" : "Inactive")} - Created by {_authService.CurrentUser?.Username}");

                    // Success state with check icon
                    saveButton.IconChar = IconChar.Check;
                    saveButton.BackColor = System.Drawing.Color.FromArgb(24, 119, 18);
                    saveButton.Text = "  Success!";
                    titleIcon.IconChar = IconChar.CheckCircle;
                    titleIcon.IconColor = System.Drawing.Color.FromArgb(24, 119, 18);
                    
                    await Task.Delay(500); // Brief success display

                    MessageBox.Show($"Category '{_category.Name}' created successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Update existing category
                    var oldValues = $"Name: '{_category.Name}', Description: '{_category.Description}', Status: {(_category.IsActive ? "Active" : "Inactive")}";

                    _category.Name = nameTextBox.Text.Trim();
                    _category.Description = string.IsNullOrWhiteSpace(descriptionTextBox.Text) ? null : descriptionTextBox.Text.Trim();
                    _category.IsActive = isActiveCheckBox.Checked;

                    // Mark entity as modified to ensure EF tracks the changes
                    _context.Entry(_category).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    var newValues = $"Name: '{_category.Name}', Description: '{_category.Description}', Status: {(_category.IsActive ? "Active" : "Inactive")}";

                    // Log the update
                    await _authService.LogAuditAsync("CATEGORY_UPDATED", "Categories", _category.Id, oldValues, 
                        $"{newValues} - Updated by {_authService.CurrentUser?.Username}");

                    // Success state with check icon
                    saveButton.IconChar = IconChar.Check;
                    saveButton.BackColor = System.Drawing.Color.FromArgb(24, 119, 18);
                    saveButton.Text = "  Updated!";
                    titleIcon.IconChar = IconChar.CheckCircle;
                    titleIcon.IconColor = System.Drawing.Color.FromArgb(24, 119, 18);
                    
                    await Task.Delay(500); // Brief success display

                    MessageBox.Show($"Category '{_category.Name}' updated successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                // Error state with warning icon
                saveButton.IconChar = IconChar.ExclamationTriangle;
                saveButton.BackColor = System.Drawing.Color.FromArgb(255, 0, 80);
                saveButton.Text = "  Error";
                titleIcon.IconChar = IconChar.ExclamationCircle;
                titleIcon.IconColor = System.Drawing.Color.FromArgb(255, 0, 80);
                
                await Task.Delay(1000);
                
                MessageBox.Show($"Error saving category: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Reset button state
                saveButton.Enabled = true;
                saveButton.Text = "  Save";
                saveButton.IconChar = IconChar.Save;
                saveButton.BackColor = System.Drawing.Color.FromArgb(24, 119, 18);
            }
        }

        private bool ValidateInput()
        {
            // Reset visual validation states
            nameTextBox.BackColor = System.Drawing.Color.White;
            descriptionTextBox.BackColor = System.Drawing.Color.White;
            nameIcon.IconColor = System.Drawing.Color.FromArgb(64, 64, 64);
            descIcon.IconColor = System.Drawing.Color.FromArgb(64, 64, 64);

            if (string.IsNullOrWhiteSpace(nameTextBox.Text))
            {
                nameTextBox.BackColor = System.Drawing.Color.FromArgb(255, 235, 235);
                nameIcon.IconChar = IconChar.ExclamationCircle;
                nameIcon.IconColor = System.Drawing.Color.FromArgb(255, 0, 80);
                nameTextBox.Focus();
                
                MessageBox.Show("Category name is required.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
                // Reset icon after validation
                Task.Run(async () => 
                {
                    await Task.Delay(2000);
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new Action(() => 
                        {
                            nameIcon.IconChar = IconChar.Tag;
                            nameIcon.IconColor = System.Drawing.Color.FromArgb(64, 64, 64);
                        }));
                    }
                });
                
                return false;
            }

            if (nameTextBox.Text.Trim().Length < 2)
            {
                nameTextBox.BackColor = System.Drawing.Color.FromArgb(255, 235, 235);
                nameIcon.IconChar = IconChar.ExclamationCircle;
                nameIcon.IconColor = System.Drawing.Color.FromArgb(255, 0, 80);
                nameTextBox.Focus();
                
                MessageBox.Show("Category name must be at least 2 characters long.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
                // Reset icon after validation
                Task.Run(async () => 
                {
                    await Task.Delay(2000);
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new Action(() => 
                        {
                            nameIcon.IconChar = IconChar.Tag;
                            nameIcon.IconColor = System.Drawing.Color.FromArgb(64, 64, 64);
                        }));
                    }
                });
                
                return false;
            }

            // Check for duplicate category names (excluding current category)
            var existingCategory = _context.Categories.FirstOrDefault(c => 
                c.Name.ToLower() == nameTextBox.Text.Trim().ToLower() && 
                c.Id != (_categoryId ?? 0) && 
                c.IsActive);

            if (existingCategory != null)
            {
                nameTextBox.BackColor = System.Drawing.Color.FromArgb(255, 235, 235);
                nameIcon.IconChar = IconChar.Clone;
                nameIcon.IconColor = System.Drawing.Color.FromArgb(255, 152, 0);
                nameTextBox.Focus();
                
                MessageBox.Show($"A category with the name '{nameTextBox.Text.Trim()}' already exists.", "Duplicate Name", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
                // Reset icon after validation
                Task.Run(async () => 
                {
                    await Task.Delay(2000);
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new Action(() => 
                        {
                            nameIcon.IconChar = IconChar.Tag;
                            nameIcon.IconColor = System.Drawing.Color.FromArgb(64, 64, 64);
                        }));
                    }
                });
                
                return false;
            }

            // Show success icon on valid input
            nameIcon.IconChar = IconChar.CheckCircle;
            nameIcon.IconColor = System.Drawing.Color.FromArgb(24, 119, 18);
            
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