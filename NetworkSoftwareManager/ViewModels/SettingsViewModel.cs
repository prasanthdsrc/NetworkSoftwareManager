using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;
using NetworkSoftwareManager.Models;
using NetworkSoftwareManager.Services;
using NetworkSoftwareManager.Utils;

namespace NetworkSoftwareManager.ViewModels
{
    /// <summary>
    /// View model for the application settings.
    /// </summary>
    public class SettingsViewModel : BaseViewModel
    {
        private readonly SettingsService _settingsService;
        private readonly CredentialService _credentialService;
        
        private AppSettings _settings;
        private bool _isTestingCredentials;

        public SettingsViewModel()
        {
            _settingsService = new SettingsService();
            _credentialService = new CredentialService();
            
            // Load current settings
            _settings = _settingsService.CurrentSettings.Clone();
            
            // Initialize commands
            SaveSettingsCommand = new RelayCommand(SaveSettings);
            ResetSettingsCommand = new RelayCommand(ResetSettings);
            BrowseDirectoryCommand = new RelayCommand(BrowseDirectory);
            TestCredentialsCommand = new RelayCommand(async () => await TestCredentialsAsync(), () => !IsTestingCredentials);
            
            // Initial message
            StatusMessage = "Edit application settings.";
        }

        #region Properties

        public AppSettings Settings
        {
            get => _settings;
            set
            {
                if (_settings != value)
                {
                    _settings = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsTestingCredentials
        {
            get => _isTestingCredentials;
            set
            {
                if (_isTestingCredentials != value)
                {
                    _isTestingCredentials = value;
                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        #endregion

        #region Commands

        public ICommand SaveSettingsCommand { get; }
        public ICommand ResetSettingsCommand { get; }
        public ICommand BrowseDirectoryCommand { get; }
        public ICommand TestCredentialsCommand { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Saves the current settings.
        /// </summary>
        private void SaveSettings()
        {
            try
            {
                // Validate settings
                if (!ValidateSettings())
                {
                    return;
                }
                
                // If set to remember credentials, encrypt them
                if (Settings.RememberCredentials)
                {
                    Settings.GlobalCredentials = Settings.GlobalCredentials.GetSecureCredentials();
                }
                else
                {
                    // Clear password if not remembering
                    Settings.GlobalCredentials.Password = string.Empty;
                }
                
                // Ensure staging directory exists
                if (!string.IsNullOrEmpty(Settings.UpdateStagingDirectory) && 
                    !Directory.Exists(Settings.UpdateStagingDirectory))
                {
                    Directory.CreateDirectory(Settings.UpdateStagingDirectory);
                }
                
                // Save settings
                _settingsService.CurrentSettings = Settings;
                _settingsService.SaveSettings();
                
                StatusMessage = "Settings saved successfully.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error saving settings: {ex.Message}";
            }
        }

        /// <summary>
        /// Resets settings to default values.
        /// </summary>
        private void ResetSettings()
        {
            try
            {
                Settings = AppSettings.GetDefaultSettings();
                StatusMessage = "Settings reset to defaults.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error resetting settings: {ex.Message}";
            }
        }

        /// <summary>
        /// Opens a dialog to browse for a directory.
        /// </summary>
        private void BrowseDirectory()
        {
            try
            {
                // Using a standard OpenFileDialog with folder selection (OpenFolderDialog isn't available in .NET 6)
                var dialog = new Microsoft.Win32.OpenFileDialog
                {
                    Title = "Select Update Staging Directory",
                    CheckFileExists = false,
                    CheckPathExists = true,
                    ValidateNames = false,
                    FileName = "Folder Selection"
                };
                
                if (dialog.ShowDialog() == true)
                {
                    // Get directory path from the dialog
                    string path = System.IO.Path.GetDirectoryName(dialog.FileName) ?? string.Empty;
                    Settings.UpdateStagingDirectory = path;
                    StatusMessage = $"Selected directory: {path}";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error selecting directory: {ex.Message}";
            }
        }

        /// <summary>
        /// Tests the configured credentials.
        /// </summary>
        private async Task TestCredentialsAsync()
        {
            if (IsTestingCredentials) return;
            
            try
            {
                IsTestingCredentials = true;
                StatusMessage = "Testing credentials...";
                
                // Create a test credentials object
                var testCredentials = new Credentials
                {
                    Domain = Settings.GlobalCredentials.Domain,
                    Username = Settings.GlobalCredentials.Username,
                    Password = Settings.GlobalCredentials.Password
                };
                
                // Test credentials
                bool result = await _credentialService.TestCredentialsAsync(testCredentials);
                
                if (result)
                {
                    StatusMessage = "Credentials test successful.";
                }
                else
                {
                    StatusMessage = "Credentials test failed. Please check your username and password.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error testing credentials: {ex.Message}";
            }
            finally
            {
                IsTestingCredentials = false;
            }
        }

        /// <summary>
        /// Validates the settings.
        /// </summary>
        private bool ValidateSettings()
        {
            // Validate IP range format
            if (!string.IsNullOrEmpty(Settings.IPRangesToScan))
            {
                var ipRanges = NetworkHelper.ParseIPRanges(Settings.IPRangesToScan);
                if (ipRanges.Count == 0)
                {
                    StatusMessage = "Invalid IP range format. Please use format like 192.168.1.1-192.168.1.254";
                    return false;
                }
            }
            
            // Validate timeouts
            if (Settings.ScanTimeout <= TimeSpan.Zero)
            {
                StatusMessage = "Scan timeout must be greater than 0.";
                return false;
            }
            
            if (Settings.ConnectionTimeout <= TimeSpan.Zero)
            {
                StatusMessage = "Connection timeout must be greater than 0.";
                return false;
            }
            
            // Validate thread count
            if (Settings.ThreadCount <= 0 || Settings.ThreadCount > 100)
            {
                StatusMessage = "Thread count must be between 1 and 100.";
                return false;
            }
            
            // Staging directory
            if (string.IsNullOrEmpty(Settings.UpdateStagingDirectory))
            {
                StatusMessage = "Update staging directory cannot be empty.";
                return false;
            }
            
            return true;
        }

        #endregion
    }

    /// <summary>
    /// Extension method to clone settings.
    /// </summary>
    public static class AppSettingsExtensions
    {
        public static AppSettings Clone(this AppSettings settings)
        {
            return new AppSettings
            {
                GlobalCredentials = new Credentials
                {
                    Domain = settings.GlobalCredentials.Domain,
                    Username = settings.GlobalCredentials.Username,
                    Password = settings.GlobalCredentials.Password
                },
                UpdateStagingDirectory = settings.UpdateStagingDirectory,
                ScanOnStartup = settings.ScanOnStartup,
                ScanTimeout = settings.ScanTimeout,
                ConnectionTimeout = settings.ConnectionTimeout,
                IPRangesToScan = settings.IPRangesToScan,
                ExcludedIPs = new List<string>(settings.ExcludedIPs),
                SavedMachines = new List<string>(settings.SavedMachines),
                RememberCredentials = settings.RememberCredentials,
                ThreadCount = settings.ThreadCount,
                UseWMI = settings.UseWMI,
                UsePowerShell = settings.UsePowerShell,
                LogFileEnabled = settings.LogFileEnabled
            };
        }
    }
}
