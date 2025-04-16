using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using NetworkSoftwareManager.Models;
using NetworkSoftwareManager.Services;
using NetworkSoftwareManager.Utils;

namespace NetworkSoftwareManager.ViewModels
{
    /// <summary>
    /// View model for the dashboard view.
    /// </summary>
    public class DashboardViewModel : BaseViewModel
    {
        private readonly NetworkScanService _networkScanService;
        private readonly SoftwareInventoryService _softwareInventoryService;
        private readonly SoftwareUpdateService _softwareUpdateService;
        private readonly SettingsService _settingsService;

        private int _totalMachines;
        private int _onlineMachines;
        private int _offlineMachines;
        private int _totalSoftware;
        private int _softwareWithUpdates;
        private int _pendingDeployments;
        private ObservableCollection<Machine> _recentlyScannedMachines = new();
        private ObservableCollection<Software> _topSoftwareByInstallCount = new();
        private ObservableCollection<SoftwareUpdate> _recentDeployments = new();

        public DashboardViewModel()
        {
            _networkScanService = new NetworkScanService();
            _softwareInventoryService = new SoftwareInventoryService();
            _softwareUpdateService = new SoftwareUpdateService();
            _settingsService = new SettingsService();

            // Initialize commands
            RefreshCommand = new RelayCommand(async () => await RefreshDashboardDataAsync());
            ScanNetworkCommand = new RelayCommand(async () => await ScanNetworkAsync());
            ViewMachineDetailsCommand = new RelayCommand<Machine>(OnViewMachineDetails);
            ViewSoftwareDetailsCommand = new RelayCommand<Software>(OnViewSoftwareDetails);
            ViewUpdateDetailsCommand = new RelayCommand<SoftwareUpdate>(OnViewUpdateDetails);
            
            // Load initial data
            Task.Run(async () => await LoadInitialDataAsync());
        }

        #region Properties

        public int TotalMachines
        {
            get => _totalMachines;
            set
            {
                if (_totalMachines != value)
                {
                    _totalMachines = value;
                    OnPropertyChanged();
                }
            }
        }

        public int OnlineMachines
        {
            get => _onlineMachines;
            set
            {
                if (_onlineMachines != value)
                {
                    _onlineMachines = value;
                    OnPropertyChanged();
                }
            }
        }

        public int OfflineMachines
        {
            get => _offlineMachines;
            set
            {
                if (_offlineMachines != value)
                {
                    _offlineMachines = value;
                    OnPropertyChanged();
                }
            }
        }

        public int TotalSoftware
        {
            get => _totalSoftware;
            set
            {
                if (_totalSoftware != value)
                {
                    _totalSoftware = value;
                    OnPropertyChanged();
                }
            }
        }

        public int SoftwareWithUpdates
        {
            get => _softwareWithUpdates;
            set
            {
                if (_softwareWithUpdates != value)
                {
                    _softwareWithUpdates = value;
                    OnPropertyChanged();
                }
            }
        }

        public int PendingDeployments
        {
            get => _pendingDeployments;
            set
            {
                if (_pendingDeployments != value)
                {
                    _pendingDeployments = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Machine> RecentlyScannedMachines
        {
            get => _recentlyScannedMachines;
            set
            {
                if (_recentlyScannedMachines != value)
                {
                    _recentlyScannedMachines = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Software> TopSoftwareByInstallCount
        {
            get => _topSoftwareByInstallCount;
            set
            {
                if (_topSoftwareByInstallCount != value)
                {
                    _topSoftwareByInstallCount = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<SoftwareUpdate> RecentDeployments
        {
            get => _recentDeployments;
            set
            {
                if (_recentDeployments != value)
                {
                    _recentDeployments = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Commands

        public ICommand RefreshCommand { get; }
        public ICommand ScanNetworkCommand { get; }
        public ICommand ViewMachineDetailsCommand { get; }
        public ICommand ViewSoftwareDetailsCommand { get; }
        public ICommand ViewUpdateDetailsCommand { get; }

        private void OnViewMachineDetails(Machine machine)
        {
            if (machine == null) return;
            // Navigate to machine details view or open details dialog
        }

        private void OnViewSoftwareDetails(Software software)
        {
            if (software == null) return;
            // Navigate to software details view or open details dialog
        }

        private void OnViewUpdateDetails(SoftwareUpdate update)
        {
            if (update == null) return;
            // Navigate to update details view or open details dialog
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads initial dashboard data.
        /// </summary>
        private async Task LoadInitialDataAsync()
        {
            await ExecuteWithBusyIndicator(async () =>
            {
                // Get summary data
                var machines = await _networkScanService.GetSavedMachinesAsync();
                var allSoftware = await _softwareInventoryService.GetAllSoftwareInventoryAsync();
                var updates = await _softwareUpdateService.GetRecentUpdatesAsync();

                // Update UI properties
                TotalMachines = machines.Count;
                OnlineMachines = machines.Count(m => m.IsOnline);
                OfflineMachines = TotalMachines - OnlineMachines;
                
                TotalSoftware = allSoftware.Count;
                SoftwareWithUpdates = allSoftware.Count(s => s.UpdateAvailable);
                
                PendingDeployments = updates.Count(u => u.Status == "Pending" || u.Status == "In Progress");
                
                // Update collections
                RecentlyScannedMachines = new ObservableCollection<Machine>(
                    machines.OrderByDescending(m => m.LastScanned).Take(5));
                
                TopSoftwareByInstallCount = new ObservableCollection<Software>(
                    allSoftware.OrderByDescending(s => s.InstallCount).Take(5));
                
                RecentDeployments = new ObservableCollection<SoftwareUpdate>(
                    updates.OrderByDescending(u => u.CreatedDate).Take(5));
            });
        }

        /// <summary>
        /// Refreshes dashboard data.
        /// </summary>
        private async Task RefreshDashboardDataAsync()
        {
            await LoadInitialDataAsync();
        }

        /// <summary>
        /// Initiates a network scan.
        /// </summary>
        private async Task ScanNetworkAsync()
        {
            await ExecuteWithBusyIndicator(async () =>
            {
                // Load settings
                var settings = _settingsService.CurrentSettings;
                
                // Parse IP ranges
                var ipRanges = NetworkHelper.ParseIPRanges(settings.IPRangesToScan);
                
                // Start scan
                var machines = await _networkScanService.ScanNetworkAsync(
                    ipRanges, 
                    settings.ExcludedIPs, 
                    settings.ScanTimeout,
                    settings.ThreadCount);
                
                // Update dashboard data
                await RefreshDashboardDataAsync();
                
                StatusMessage = $"Network scan completed. Found {machines.Count} machines.";
            }, "Scanning network...");
        }

        #endregion
    }
}
