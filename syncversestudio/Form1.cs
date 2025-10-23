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
        private bool _isExiting = false;

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
            // cavi 123456 | icv 123456 | vi vi
            this.usernameTextBox.Text = "icv";
            this.passwordTextBox.Text = "123456";
        }

        private async void LoginForm_Load(object sender, EventArgs e)
        {
            await InitializeDatabase();
            
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
                    errorLabel.Text = "Login successful!";
                    errorLabel.ForeColor = System.Drawing.Color.FromArgb(120, 220, 120);
                    
                    // Immediately load dashboard for faster experience
                    this.Hide();
                    var mainForm = new MainDashboard(_authService, this);
                    
                    // When main form closes, show login form again or close application
                    mainForm.FormClosed += (s, args) => 
                    {
                        // Check if application is exiting
                        if (_isExiting)
                        {
                            Console.WriteLine("LoginForm: Application is exiting, closing login form");
                            this.Close();
                            return;
                        }
                        
                        Console.WriteLine("LoginForm: MainDashboard closed, returning to login");
                        
                        // Clear the form fields for security
                        this.usernameTextBox.Clear();
                        this.passwordTextBox.Clear();
                        this.errorLabel.Text = "";
                        
                        // Show login form again
                        this.Show();
                        this.WindowState = FormWindowState.Normal;
                        this.BringToFront();
                        this.Focus();
                        this.usernameTextBox.Focus();
                    };
                    
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

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // If user tries to close login form, ask for confirmation
            if (this.Visible)
            {
                var result = MessageBox.Show(
                    "Are you sure you want to exit SyncVerse Studio?",
                    "Exit Application",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }
            
            base.OnFormClosing(e);
        }

        public void SetExiting()
        {
            Console.WriteLine("LoginForm: SetExiting() called - application will exit");
            _isExiting = true;
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
