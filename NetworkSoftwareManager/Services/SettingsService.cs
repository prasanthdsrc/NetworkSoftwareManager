using System;
using System.IO;
using Newtonsoft.Json;
using NetworkSoftwareManager.Models;

namespace NetworkSoftwareManager.Services
{
    /// <summary>
    /// Service for managing application settings.
    /// </summary>
    public class SettingsService
    {
        private readonly string _settingsFilePath;
        private AppSettings _currentSettings;

        /// <summary>
        /// Gets or sets the current application settings.
        /// </summary>
        public AppSettings CurrentSettings
        {
            get => _currentSettings;
            set => _currentSettings = value;
        }

        public SettingsService()
        {
            // Set up settings file path
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "NetworkSoftwareManager");
            
            // Ensure the directory exists
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }
            
            _settingsFilePath = Path.Combine(appDataPath, "settings.json");
            
            // Initialize with default settings
            _currentSettings = AppSettings.GetDefaultSettings();
            
            // Load settings from file if available
            LoadSettings();
        }

        /// <summary>
        /// Loads settings from the settings file.
        /// </summary>
        public void LoadSettings()
        {
            try
            {
                if (File.Exists(_settingsFilePath))
                {
                    string json = File.ReadAllText(_settingsFilePath);
                    var loadedSettings = JsonConvert.DeserializeObject<AppSettings>(json);
                    
                    if (loadedSettings != null)
                    {
                        _currentSettings = loadedSettings;
                        
                        // Ensure default staging directory exists
                        if (string.IsNullOrEmpty(_currentSettings.UpdateStagingDirectory))
                        {
                            _currentSettings.UpdateStagingDirectory = AppSettings.GetDefaultSettings().UpdateStagingDirectory;
                        }
                        
                        // Ensure directory exists
                        EnsureDirectoryExists(_currentSettings.UpdateStagingDirectory);
                        
                        // Decrypt credentials if needed
                        if (_currentSettings.RememberCredentials)
                        {
                            try
                            {
                                _currentSettings.GlobalCredentials.Password = 
                                    Utils.PasswordHelper.DecryptPassword(_currentSettings.GlobalCredentials.Password);
                            }
                            catch
                            {
                                // If decryption fails, clear the password
                                _currentSettings.GlobalCredentials.Password = string.Empty;
                            }
                        }
                        else
                        {
                            // Clear the password if not remembering credentials
                            _currentSettings.GlobalCredentials.Password = string.Empty;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading settings: {ex.Message}");
                
                // Fallback to default settings
                _currentSettings = AppSettings.GetDefaultSettings();
                
                // Ensure directory exists
                EnsureDirectoryExists(_currentSettings.UpdateStagingDirectory);
            }
        }

        /// <summary>
        /// Saves settings to the settings file.
        /// </summary>
        public void SaveSettings()
        {
            try
            {
                // Create a copy of the settings to save
                var settingsToSave = CloneSettings(_currentSettings);
                
                // Encrypt credentials if remembering
                if (settingsToSave.RememberCredentials)
                {
                    try
                    {
                        settingsToSave.GlobalCredentials.Password = 
                            Utils.PasswordHelper.EncryptPassword(settingsToSave.GlobalCredentials.Password);
                    }
                    catch
                    {
                        // If encryption fails, don't save the password
                        settingsToSave.GlobalCredentials.Password = string.Empty;
                        settingsToSave.RememberCredentials = false;
                    }
                }
                else
                {
                    // Clear the password if not remembering credentials
                    settingsToSave.GlobalCredentials.Password = string.Empty;
                }
                
                // Ensure directory exists
                EnsureDirectoryExists(settingsToSave.UpdateStagingDirectory);
                
                // Serialize and save
                string json = JsonConvert.SerializeObject(settingsToSave, Formatting.Indented);
                File.WriteAllText(_settingsFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving settings: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Resets settings to default values.
        /// </summary>
        public void ResetSettings()
        {
            _currentSettings = AppSettings.GetDefaultSettings();
            SaveSettings();
        }

        /// <summary>
        /// Ensures a directory exists, creates it if it doesn't.
        /// </summary>
        private void EnsureDirectoryExists(string directoryPath)
        {
            if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        /// <summary>
        /// Creates a deep clone of the settings object.
        /// </summary>
        private AppSettings CloneSettings(AppSettings original)
        {
            var clone = new AppSettings
            {
                GlobalCredentials = new Credentials
                {
                    Domain = original.GlobalCredentials.Domain,
                    Username = original.GlobalCredentials.Username,
                    Password = original.GlobalCredentials.Password
                },
                UpdateStagingDirectory = original.UpdateStagingDirectory,
                ScanOnStartup = original.ScanOnStartup,
                ScanTimeout = original.ScanTimeout,
                ConnectionTimeout = original.ConnectionTimeout,
                IPRangesToScan = original.IPRangesToScan,
                RememberCredentials = original.RememberCredentials,
                ThreadCount = original.ThreadCount,
                UseWMI = original.UseWMI,
                UsePowerShell = original.UsePowerShell,
                LogFileEnabled = original.LogFileEnabled
            };
            
            // Clone collections
            foreach (var ip in original.ExcludedIPs)
            {
                clone.ExcludedIPs.Add(ip);
            }
            
            foreach (var machine in original.SavedMachines)
            {
                clone.SavedMachines.Add(machine);
            }
            
            return clone;
        }
    }
}
