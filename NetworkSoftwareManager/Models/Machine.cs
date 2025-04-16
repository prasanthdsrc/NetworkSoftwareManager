using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NetworkSoftwareManager.Models
{
    /// <summary>
    /// Represents a networked machine with its software inventory.
    /// </summary>
    public class Machine : INotifyPropertyChanged
    {
        private string _ipAddress = string.Empty;
        private string _hostname = string.Empty;
        private string _operatingSystem = string.Empty;
        private bool _isOnline;
        private bool _isSelected;
        private DateTime _lastScanned;
        private string _domain = string.Empty;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _status = string.Empty;
        private List<Software> _installedSoftware = new();
        private int _pendingUpdates;

        public string IPAddress
        {
            get => _ipAddress;
            set
            {
                if (_ipAddress != value)
                {
                    _ipAddress = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Hostname
        {
            get => _hostname;
            set
            {
                if (_hostname != value)
                {
                    _hostname = value;
                    OnPropertyChanged();
                }
            }
        }

        public string OperatingSystem
        {
            get => _operatingSystem;
            set
            {
                if (_operatingSystem != value)
                {
                    _operatingSystem = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsOnline
        {
            get => _isOnline;
            set
            {
                if (_isOnline != value)
                {
                    _isOnline = value;
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

        public DateTime LastScanned
        {
            get => _lastScanned;
            set
            {
                if (_lastScanned != value)
                {
                    _lastScanned = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Domain
        {
            get => _domain;
            set
            {
                if (_domain != value)
                {
                    _domain = value;
                    OnPropertyChanged();
                }
            }
        }

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

        public string Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<Software> InstalledSoftware
        {
            get => _installedSoftware;
            set
            {
                if (_installedSoftware != value)
                {
                    _installedSoftware = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(SoftwareCount));
                    UpdatePendingUpdates();
                }
            }
        }

        public int SoftwareCount => InstalledSoftware.Count;

        public int PendingUpdates
        {
            get => _pendingUpdates;
            set
            {
                if (_pendingUpdates != value)
                {
                    _pendingUpdates = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Updates the count of pending updates based on software update status.
        /// </summary>
        public void UpdatePendingUpdates()
        {
            int count = 0;
            foreach (var software in InstalledSoftware)
            {
                if (software.UpdateAvailable)
                {
                    count++;
                }
            }
            PendingUpdates = count;
        }

        /// <summary>
        /// Determines if the credentials are local or inherited from global settings.
        /// </summary>
        public bool UseGlobalCredentials => string.IsNullOrEmpty(Username) && string.IsNullOrEmpty(Password);

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
