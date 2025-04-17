using System;
using System.IO;
using System.Threading.Tasks;
using NetworkSoftwareManager.Models;
using Newtonsoft.Json;

namespace NetworkSoftwareManager.Services
{
    public class SettingsService
    {
        private string _settingsPath;
        private AppSettings _currentSettings;

        public AppSettings CurrentSettings => _currentSettings;

        public SettingsService(string dataPath)
        {
            _settingsPath = Path.Combine(dataPath, "settings.json");
            _currentSettings = new AppSettings();
            LoadSettings();
        }

        private void LoadSettings()
        {
            try
            {
                if (File.Exists(_settingsPath))
                {
                    string json = File.ReadAllText(_settingsPath);
                    var settings = JsonConvert.DeserializeObject<AppSettings>(json);
                    
                    if (settings != null)
                    {
                        _currentSettings = settings;
                    }
                }
                else
                {
                    // Create default settings
                    _currentSettings = new AppSettings
                    {
                        ConnectionTimeout = TimeSpan.FromSeconds(30),
                        DefaultScanInterval = TimeSpan.FromHours(24),
                        UptimeCheckInterval = TimeSpan.FromHours(4),
                        RetryInterval = TimeSpan.FromHours(4),
                        SmtpServer = "smtp.example.com",
                        SmtpPort = 587,
                        SmtpUsername = "notifications@example.com",
                        SmtpPassword = "",
                        EmailFrom = "notifications@example.com",
                        EmailTo = "admin@example.com",
                        SendDailyReport = true,
                        DailyReportTime = new TimeSpan(8, 0, 0) // 8 AM
                    };
                    
                    SaveSettings();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading settings: {ex.Message}");
                
                // Create default settings on error
                _currentSettings = new AppSettings();
            }
        }

        public void SaveSettings()
        {
            try
            {
                string json = JsonConvert.SerializeObject(_currentSettings, Formatting.Indented);
                File.WriteAllText(_settingsPath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving settings: {ex.Message}");
            }
        }
        
        public void UpdateSettings(AppSettings newSettings)
        {
            _currentSettings = newSettings;
            SaveSettings();
        }
    }
}