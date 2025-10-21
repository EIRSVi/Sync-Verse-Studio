using System.Drawing;
using System.Windows.Forms;
using SyncVerseStudio.Services;
using SyncVerseStudio.Views;
using SyncVerseStudio.Helpers;

namespace SyncVerseStudio
{
    public partial class LoginForm : Form
    {
        private readonly AuthenticationService _authService;
        private bool _isInitializing = false;

        public LoginForm()
        {
            InitializeComponent();
            _authService = new AuthenticationService();

            // Set application icon using helper
            IconHelper.SetFormIcon(this);

            // Wire up events
            this.loginButton.Click += LoginButton_Click;
            this.passwordTextBox.KeyPress += PasswordTextBox_KeyPress;
            this.usernameTextBox.KeyPress += UsernameTextBox_KeyPress;
            this.Load += LoginForm_Load;

            // Set default test credentials for development
            this.usernameTextBox.Text = "vi";
            this.passwordTextBox.Text = "vi";
        }

        private async void LoginForm_Load(object sender, EventArgs e)
        {
            await InitializeDatabase();
            CenterLoginPanel(); // Center on load
            
            // Focus on username field
            usernameTextBox.Focus();
        }

        private async Task InitializeDatabase()
        {
            if (_isInitializing) return;

            try
            {
                _isInitializing = true;
                loginButton.Enabled = false;
                loginButton.Text = "Initializing...";
                errorLabel.Text = "Initializing database, please wait...";
                errorLabel.ForeColor = System.Drawing.Color.FromArgb(120, 220, 120);

                await DatabaseInitializer.InitializeAsync();

                errorLabel.Text = "Database ready. Please sign in.";
                errorLabel.ForeColor = System.Drawing.Color.FromArgb(120, 220, 120);
                
                // Clear the message after 2 seconds
                await Task.Delay(2000);
                errorLabel.Text = "";
            }
            catch (Exception ex)
            {
                errorLabel.Text = $"Initialization error: {ex.Message}";
                errorLabel.ForeColor = System.Drawing.Color.FromArgb(255, 120, 120);
            }
            finally
            {
                loginButton.Enabled = true;
                loginButton.Text = "Login";
                _isInitializing = false;
            }
        }

        private void UsernameTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                passwordTextBox.Focus();
                e.Handled = true;
            }
        }

        private void PasswordTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                LoginButton_Click(sender, e);
                e.Handled = true;
            }
        }

        private async void LoginButton_Click(object sender, EventArgs e)
        {
            if (_isInitializing) return;

            try
            {
                errorLabel.Text = "";
                loginButton.Enabled = false;
                loginButton.Text = "Signing in...";

                var username = usernameTextBox.Text.Trim();
                var password = passwordTextBox.Text;

                if (string.IsNullOrEmpty(username))
                {
                    errorLabel.Text = "Please enter your username.";
                    errorLabel.ForeColor = System.Drawing.Color.FromArgb(255, 120, 120);
                    usernameTextBox.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(password))
                {
                    errorLabel.Text = "Please enter your password.";
                    errorLabel.ForeColor = System.Drawing.Color.FromArgb(255, 120, 120);
                    passwordTextBox.Focus();
                    return;
                }

                var success = await _authService.LoginAsync(username, password);

                if (success)
                {
                    errorLabel.Text = "Login successful! Loading dashboard...";
                    errorLabel.ForeColor = System.Drawing.Color.FromArgb(120, 220, 120);
                    
                    // Brief delay to show success message
                    await Task.Delay(500);
                    
                    // Open main dashboard based on user role
                    this.Hide();
                    var mainForm = new MainDashboard(_authService);
                    mainForm.FormClosed += (s, args) => this.Close();
                    mainForm.Show();
                }
                else
                {
                    errorLabel.Text = "Invalid username or password.";
                    errorLabel.ForeColor = System.Drawing.Color.FromArgb(255, 120, 120);
                    passwordTextBox.Clear();
                    passwordTextBox.Focus();
                }
            }
            catch (Exception ex)
            {
                errorLabel.Text = "Login error. Please try again.";
                errorLabel.ForeColor = System.Drawing.Color.FromArgb(255, 120, 120);
                Console.WriteLine($"Login error: {ex.Message}");
            }
            finally
            {
                loginButton.Enabled = true;
                loginButton.Text = "Login";
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _authService?.Dispose();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
