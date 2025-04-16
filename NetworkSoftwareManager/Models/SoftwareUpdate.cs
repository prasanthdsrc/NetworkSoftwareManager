using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NetworkSoftwareManager.Models
{
    /// <summary>
    /// Represents a software update task for deploying to multiple machines.
    /// </summary>
    public class SoftwareUpdate : INotifyPropertyChanged
    {
        private int _id;
        private string _taskId = Guid.NewGuid().ToString();
        private string _softwareName = string.Empty;
        private string _targetVersion = string.Empty;
        private bool _useLatestVersion;
        private DateTime _createdDate = DateTime.Now;
        private DateTime? _completedDate;
        private string _status = "Pending";
        private double _progressPercentage;
        private List<Machine> _targetMachines = new();
        private int _successCount;
        private int _failureCount;
        private string _updateStagingPath = string.Empty;
        private string _installCommand = string.Empty;
        private string _uninstallCommand = string.Empty;
        private bool _forceReinstall;
        private string _publisher = string.Empty;
        private string _initiatedBy = string.Empty;
        private bool _requiresManualApproval = true;
        private DateTime? _approvedDate;
        private string _approvedBy = string.Empty;

        public int Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public string TaskId
        {
            get => _taskId;
            set
            {
                if (_taskId != value)
                {
                    _taskId = value;
                    OnPropertyChanged();
                }
            }
        }

        public string SoftwareName
        {
            get => _softwareName;
            set
            {
                if (_softwareName != value)
                {
                    _softwareName = value;
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
                }
            }
        }

        public DateTime CreatedDate
        {
            get => _createdDate;
            set
            {
                if (_createdDate != value)
                {
                    _createdDate = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime? CompletedDate
        {
            get => _completedDate;
            set
            {
                if (_completedDate != value)
                {
                    _completedDate = value;
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

        public double ProgressPercentage
        {
            get => _progressPercentage;
            set
            {
                if (Math.Abs(_progressPercentage - value) > 0.001)
                {
                    _progressPercentage = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<Machine> TargetMachines
        {
            get => _targetMachines;
            set
            {
                if (_targetMachines != value)
                {
                    _targetMachines = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TargetMachineCount));
                }
            }
        }

        public int TargetMachineCount => TargetMachines.Count;

        public int SuccessCount
        {
            get => _successCount;
            set
            {
                if (_successCount != value)
                {
                    _successCount = value;
                    OnPropertyChanged();
                    CalculateProgress();
                }
            }
        }

        public int FailureCount
        {
            get => _failureCount;
            set
            {
                if (_failureCount != value)
                {
                    _failureCount = value;
                    OnPropertyChanged();
                    CalculateProgress();
                }
            }
        }

        public string UpdateStagingPath
        {
            get => _updateStagingPath;
            set
            {
                if (_updateStagingPath != value)
                {
                    _updateStagingPath = value;
                    OnPropertyChanged();
                }
            }
        }

        public string InstallCommand
        {
            get => _installCommand;
            set
            {
                if (_installCommand != value)
                {
                    _installCommand = value;
                    OnPropertyChanged();
                }
            }
        }

        public string UninstallCommand
        {
            get => _uninstallCommand;
            set
            {
                if (_uninstallCommand != value)
                {
                    _uninstallCommand = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool ForceReinstall
        {
            get => _forceReinstall;
            set
            {
                if (_forceReinstall != value)
                {
                    _forceReinstall = value;
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

        /// <summary>
        /// Calculates the current progress percentage based on success and failure counts.
        /// </summary>
        private void CalculateProgress()
        {
            if (TargetMachines.Count == 0)
            {
                ProgressPercentage = 0;
                return;
            }

            int completedCount = SuccessCount + FailureCount;
            ProgressPercentage = (double)completedCount / TargetMachines.Count * 100;
            
            // Update status if all machines have been processed
            if (completedCount == TargetMachines.Count)
            {
                Status = "Completed";
                CompletedDate = DateTime.Now;
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
