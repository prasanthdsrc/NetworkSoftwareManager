using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NetworkSoftwareManager.Models
{
    /// <summary>
    /// Application settings that can be persisted.
    /// </summary>
    public class AppSettings : INotifyPropertyChanged
    {
        private Credentials _globalCredentials = new();
        private string _updateStagingDirectory = string.Empty;
        private bool _scanOnStartup;
        private TimeSpan _scanTimeout = TimeSpan.FromMilliseconds(500);
        private TimeSpan _connectionTimeout = TimeSpan.FromMilliseconds(5000);
        private string _ipRangesToScan = string.Empty;
        private List<string> _excludedIPs = new();
        private List<string> _savedMachines = new();
        private bool _rememberCredentials;
        private int _threadCount = 10;
        private bool _useWMI = true;
        private bool _usePowerShell = true;
        private bool _logFileEnabled = true;

        public Credentials GlobalCredentials
        {
            get => _globalCredentials;
            set
            {
                if (_globalCredentials != value)
                {
                    _globalCredentials = value;
                    OnPropertyChanged();
                }
            }
        }

        public string UpdateStagingDirectory
        {
            get => _updateStagingDirectory;
            set
            {
                if (_updateStagingDirectory != value)
                {
                    _updateStagingDirectory = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool ScanOnStartup
        {
            get => _scanOnStartup;
            set
            {
                if (_scanOnStartup != value)
                {
                    _scanOnStartup = value;
                    OnPropertyChanged();
                }
            }
        }

        public TimeSpan ScanTimeout
        {
            get => _scanTimeout;
            set
            {
                if (_scanTimeout != value)
                {
                    _scanTimeout = value;
                    OnPropertyChanged();
                }
            }
        }

        public TimeSpan ConnectionTimeout
        {
            get => _connectionTimeout;
            set
            {
                if (_connectionTimeout != value)
                {
                    _connectionTimeout = value;
                    OnPropertyChanged();
                }
            }
        }

        public string IPRangesToScan
        {
            get => _ipRangesToScan;
            set
            {
                if (_ipRangesToScan != value)
                {
                    _ipRangesToScan = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<string> ExcludedIPs
        {
            get => _excludedIPs;
            set
            {
                if (_excludedIPs != value)
                {
                    _excludedIPs = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<string> SavedMachines
        {
            get => _savedMachines;
            set
            {
                if (_savedMachines != value)
                {
                    _savedMachines = value;
                    OnPropertyChanged();
                }
            }
        }

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

        public int ThreadCount
        {
            get => _threadCount;
            set
            {
                if (_threadCount != value)
                {
                    _threadCount = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool UseWMI
        {
            get => _useWMI;
            set
            {
                if (_useWMI != value)
                {
                    _useWMI = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool UsePowerShell
        {
            get => _usePowerShell;
            set
            {
                if (_usePowerShell != value)
                {
                    _usePowerShell = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool LogFileEnabled
        {
            get => _logFileEnabled;
            set
            {
                if (_logFileEnabled != value)
                {
                    _logFileEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Returns the default settings for the application.
        /// </summary>
        public static AppSettings GetDefaultSettings()
        {
            return new AppSettings
            {
                GlobalCredentials = new Credentials(),
                UpdateStagingDirectory = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "NetworkSoftwareManager",
                    "Updates"),
                ScanOnStartup = false,
                ScanTimeout = TimeSpan.FromMilliseconds(500),
                ConnectionTimeout = TimeSpan.FromMilliseconds(5000),
                IPRangesToScan = "192.168.1.1-192.168.1.254",
                ExcludedIPs = new List<string>(),
                SavedMachines = new List<string>(),
                RememberCredentials = false,
                ThreadCount = 10,
                UseWMI = true,
                UsePowerShell = true,
                LogFileEnabled = true
            };
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
