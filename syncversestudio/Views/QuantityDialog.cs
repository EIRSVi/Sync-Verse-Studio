using System.Drawing;
using SyncVerseStudio.Helpers;

namespace SyncVerseStudio.Views
{
    public class QuantityDialog : Form
    {
        private TextBox quantityBox;
        private Button okButton, cancelButton;
        public int Quantity { get; private set; }
        private int maxStock;

        public QuantityDialog(int currentQuantity, int maxStock)
        {
            this.maxStock = maxStock;
            InitializeComponent();
            quantityBox.Text = currentQuantity.ToString();
            quantityBox.SelectAll();
        }

        private void InitializeComponent()
        {
            this.Text = "Update Quantity";
            this.Size = new Size(350, 180);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            var label = new Label
            {
                Text = "Enter new quantity:",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            this.Controls.Add(label);

            quantityBox = new TextBox
            {
                Font = new Font("Segoe UI", 14F),
                Location = new Point(20, 50),
                Size = new Size(290, 35),
                TextAlign = HorizontalAlignment.Center
            };
            quantityBox.KeyPress += QuantityBox_KeyPress;
            this.Controls.Add(quantityBox);

            okButton = new Button
            {
                Text = "OK",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = BrandTheme.ButtonAdd,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(20, 100),
                Size = new Size(140, 40),
                Cursor = Cursors.Hand
            };
            okButton.FlatAppearance.BorderSize = 0;
            okButton.Click += OkButton_Click;
            this.Controls.Add(okButton);

            cancelButton = new Button
            {
                Text = "CANCEL",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = BrandTheme.ButtonRefresh,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(170, 100),
                Size = new Size(140, 40),
                Cursor = Cursors.Hand
            };
            cancelButton.FlatAppearance.BorderSize = 0;
            cancelButton.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            this.Controls.Add(cancelButton);

            this.AcceptButton = okButton;
            this.CancelButton = cancelButton;
        }

        private void QuantityBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (int.TryParse(quantityBox.Text, out int qty) && qty > 0)
            {
                if (qty <= maxStock)
                {
                    Quantity = qty;
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    MessageBox.Show($"Maximum available stock: {maxStock}", "Stock Alert", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid quantity!", "Invalid Input", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
