using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NetworkSoftwareManager.Models
{
    /// <summary>
    /// Represents a software application installed on a machine.
    /// </summary>
    public class Software : INotifyPropertyChanged
    {
        private string _name = string.Empty;
        private string _publisher = string.Empty;
        private string _installedVersion = string.Empty;
        private string _latestVersion = string.Empty;
        private string _installLocation = string.Empty;
        private DateTime _installDate;
        private bool _updateAvailable;
        private bool _isSelected;
        private string _updateStatus = string.Empty;
        private int _installCount;
        private string _targetVersion = string.Empty;
        private bool _useLatestVersion = true;

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Publisher
        {
            get => _publisher;
            set
            {
                if (_publisher != value)
                {
                    _publisher = value;
                    OnPropertyChanged();
                }
            }
        }

        public string InstalledVersion
        {
            get => _installedVersion;
            set
            {
                if (_installedVersion != value)
                {
                    _installedVersion = value;
                    OnPropertyChanged();
                    CheckUpdateAvailable();
                }
            }
        }

        public string LatestVersion
        {
            get => _latestVersion;
            set
            {
                if (_latestVersion != value)
                {
                    _latestVersion = value;
                    OnPropertyChanged();
                    CheckUpdateAvailable();
                }
            }
        }

        public string InstallLocation
        {
            get => _installLocation;
            set
            {
                if (_installLocation != value)
                {
                    _installLocation = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime InstallDate
        {
            get => _installDate;
            set
            {
                if (_installDate != value)
                {
                    _installDate = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool UpdateAvailable
        {
            get => _updateAvailable;
            set
            {
                if (_updateAvailable != value)
                {
                    _updateAvailable = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        public string UpdateStatus
        {
            get => _updateStatus;
            set
            {
                if (_updateStatus != value)
                {
                    _updateStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        public int InstallCount
        {
            get => _installCount;
            set
            {
                if (_installCount != value)
                {
                    _installCount = value;
                    OnPropertyChanged();
                }
            }
        }

        public string TargetVersion
        {
            get => _targetVersion;
            set
            {
                if (_targetVersion != value)
                {
                    _targetVersion = value;
                    OnPropertyChanged();
                    CheckUpdateAvailable();
                }
            }
        }

        public bool UseLatestVersion
        {
            get => _useLatestVersion;
            set
            {
                if (_useLatestVersion != value)
                {
                    _useLatestVersion = value;
                    OnPropertyChanged();
                    CheckUpdateAvailable();
                }
            }
        }

        /// <summary>
        /// The effective target version based on settings (either latest or specified target).
        /// </summary>
        public string EffectiveTargetVersion => UseLatestVersion ? LatestVersion : TargetVersion;

        /// <summary>
        /// Checks if an update is available based on version comparison.
        /// </summary>
        private void CheckUpdateAvailable()
        {
            if (string.IsNullOrEmpty(InstalledVersion) || 
                string.IsNullOrEmpty(UseLatestVersion ? LatestVersion : TargetVersion))
            {
                UpdateAvailable = false;
                return;
            }

            try
            {
                // Try parsing as Version objects first
                if (Version.TryParse(InstalledVersion, out Version? installedVer) && 
                    Version.TryParse(UseLatestVersion ? LatestVersion : TargetVersion, out Version? targetVer))
                {
                    UpdateAvailable = targetVer > installedVer;
                    return;
                }

                // Fallback to string comparison if version parsing fails
                string targetVersionToCompare = UseLatestVersion ? LatestVersion : TargetVersion;
                UpdateAvailable = !string.Equals(InstalledVersion, targetVersionToCompare, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                // Default to string comparison if any exception occurs
                string targetVersionToCompare = UseLatestVersion ? LatestVersion : TargetVersion;
                UpdateAvailable = !string.Equals(InstalledVersion, targetVersionToCompare, StringComparison.OrdinalIgnoreCase);
            }
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
