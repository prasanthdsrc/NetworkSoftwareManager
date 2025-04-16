using System;
using System.Threading.Tasks;
using System.Windows.Input;
using NetworkSoftwareManager.Models;
using NetworkSoftwareManager.Services;
using NetworkSoftwareManager.Utils;

namespace NetworkSoftwareManager.ViewModels
{
    /// <summary>
    /// ViewModel for the login screen.
    /// </summary>
    public class LoginViewModel : BaseViewModel
    {
        private readonly UserService _userService;
        private readonly SettingsService _settingsService;
        
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _statusMessage = string.Empty;
        private bool _isLoggingIn;
        private bool _rememberCredentials;

        /// <summary>
        /// Command for logging in.
        /// </summary>
        public ICommand LoginCommand { get; private set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username
        {
            get => _username;
            set
            {
                if (_username != value)
                {
                    _username = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the status message.
        /// </summary>
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether login is in progress.
        /// </summary>
        public bool IsLoggingIn
        {
            get => _isLoggingIn;
            set
            {
                if (_isLoggingIn != value)
                {
                    _isLoggingIn = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to remember credentials.
        /// </summary>
        public bool RememberCredentials
        {
            get => _rememberCredentials;
            set
            {
                if (_rememberCredentials != value)
                {
                    _rememberCredentials = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Event raised when login is successful.
        /// </summary>
        public event EventHandler<User>? LoginSuccessful;

        /// <summary>
        /// Initializes a new instance of the LoginViewModel class.
        /// </summary>
        public LoginViewModel(UserService userService, SettingsService settingsService)
        {
            _userService = userService;
            _settingsService = settingsService;
            
            LoginCommand = new RelayCommand(async () => await LoginAsync());
            
            // Load saved credentials if available
            LoadSavedCredentials();
        }

        /// <summary>
        /// Loads saved credentials from settings.
        /// </summary>
        private void LoadSavedCredentials()
        {
            if (_settingsService.CurrentSettings.RememberCredentials)
            {
                Username = _settingsService.CurrentSettings.GlobalCredentials.Username;
                Password = PasswordHelper.DecryptPassword(_settingsService.CurrentSettings.GlobalCredentials.Password);
                RememberCredentials = true;
            }
        }

        /// <summary>
        /// Saves credentials to settings.
        /// </summary>
        private void SaveCredentials()
        {
            if (RememberCredentials)
            {
                _settingsService.CurrentSettings.GlobalCredentials.Username = Username;
                _settingsService.CurrentSettings.GlobalCredentials.Password = PasswordHelper.EncryptPassword(Password);
                _settingsService.CurrentSettings.RememberCredentials = true;
            }
            else
            {
                _settingsService.CurrentSettings.GlobalCredentials.Username = string.Empty;
                _settingsService.CurrentSettings.GlobalCredentials.Password = string.Empty;
                _settingsService.CurrentSettings.RememberCredentials = false;
            }
            
            _settingsService.SaveSettings();
        }

        /// <summary>
        /// Handles the login process.
        /// </summary>
        private async Task LoginAsync()
        {
            if (IsLoggingIn) return;
            
            try
            {
                IsLoggingIn = true;
                StatusMessage = "Logging in...";
                
                // Validate input
                if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
                {
                    StatusMessage = "Please enter both username and password.";
                    return;
                }
                
                // Attempt to authenticate
                bool result = await _userService.AuthenticateAsync(Username, Password);
                
                if (result && _userService.CurrentUser != null)
                {
                    // Save credentials if requested
                    SaveCredentials();
                    
                    // Display login successful message
                    StatusMessage = "Login successful!";
                    
                    // Raise event
                    LoginSuccessful?.Invoke(this, _userService.CurrentUser);
                }
                else
                {
                    StatusMessage = "Invalid username or password.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Login error: {ex.Message}";
            }
            finally
            {
                IsLoggingIn = false;
            }
        }
    }
}