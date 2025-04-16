using System;
using System.Collections.Generic;
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
    /// View model for the software inventory functionality.
    /// </summary>
    public class SoftwareInventoryViewModel : BaseViewModel
    {
        private readonly SoftwareInventoryService _softwareInventoryService;
        private readonly NetworkScanService _networkScanService;
        private readonly SettingsService _settingsService;
        
        private ObservableCollection<Machine> _machines = new();
        private ObservableCollection<Software> _softwareList = new();
        private Machine? _selectedMachine;
        private Software? _selectedSoftware;
        private string _searchText = string.Empty;
        private bool _showOnlyUpdatable;
        private bool _isInventoryRunning;
        private double _inventoryProgress;
        private int _softwareCount;
        private int _updatableSoftwareCount;

        public SoftwareInventoryViewModel()
        {
            _softwareInventoryService = new SoftwareInventoryService();
            _networkScanService = new NetworkScanService();
            _settingsService = new SettingsService();

            // Initialize commands
            ScanSoftwareCommand = new RelayCommand<Machine>(async machine => await ScanSoftwareForMachineAsync(machine), 
                machine => machine != null && !IsInventoryRunning);
            
            ScanAllSoftwareCommand = new RelayCommand(async () => await ScanSoftwareForAllMachinesAsync(), 
                () => Machines.Count > 0 && !IsInventoryRunning);
            
            CancelScanCommand = new RelayCommand(CancelScan, () => IsInventoryRunning);
            
            RefreshCommand = new RelayCommand(async () => await LoadInitialDataAsync());
            
            SearchCommand = new RelayCommand(FilterSoftwareList);
            
            SelectAllSoftwareCommand = new RelayCommand(SelectAllSoftware);
            
            DeselectAllSoftwareCommand = new RelayCommand(DeselectAllSoftware);
            
            SetLatestVersionCommand = new RelayCommand<Software>(
                software => SetLatestVersion(software),
                software => software != null);
                
            RefreshLatestVersionsCommand = new RelayCommand(
                async () => await RefreshLatestVersionsAsync(),
                () => SoftwareList.Count > 0);
            
            // Load initial data
            Task.Run(async () => await LoadInitialDataAsync());
        }

        #region Properties

        public ObservableCollection<Machine> Machines
        {
            get => _machines;
            set
            {
                if (_machines != value)
                {
                    _machines = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Software> SoftwareList
        {
            get => _softwareList;
            set
            {
                if (_softwareList != value)
                {
                    _softwareList = value;
                    OnPropertyChanged();
                    UpdateSoftwareCounts();
                }
            }
        }

        public Machine? SelectedMachine
        {
            get => _selectedMachine;
            set
            {
                if (_selectedMachine != value)
                {
                    _selectedMachine = value;
                    OnPropertyChanged();
                    LoadSoftwareForSelectedMachine();
                }
            }
        }

        public Software? SelectedSoftware
        {
            get => _selectedSoftware;
            set
            {
                if (_selectedSoftware != value)
                {
                    _selectedSoftware = value;
                    OnPropertyChanged();
                }
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool ShowOnlyUpdatable
        {
            get => _showOnlyUpdatable;
            set
            {
                if (_showOnlyUpdatable != value)
                {
                    _showOnlyUpdatable = value;
                    OnPropertyChanged();
                    FilterSoftwareList();
                }
            }
        }

        public bool IsInventoryRunning
        {
            get => _isInventoryRunning;
            set
            {
                if (_isInventoryRunning != value)
                {
                    _isInventoryRunning = value;
                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public double InventoryProgress
        {
            get => _inventoryProgress;
            set
            {
                if (Math.Abs(_inventoryProgress - value) > 0.001)
                {
                    _inventoryProgress = value;
                    OnPropertyChanged();
                }
            }
        }

        public int SoftwareCount
        {
            get => _softwareCount;
            set
            {
                if (_softwareCount != value)
                {
                    _softwareCount = value;
                    OnPropertyChanged();
                }
            }
        }

        public int UpdatableSoftwareCount
        {
            get => _updatableSoftwareCount;
            set
            {
                if (_updatableSoftwareCount != value)
                {
                    _updatableSoftwareCount = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Commands

        public ICommand ScanSoftwareCommand { get; }
        public ICommand ScanAllSoftwareCommand { get; }
        public ICommand CancelScanCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand SelectAllSoftwareCommand { get; }
        public ICommand DeselectAllSoftwareCommand { get; }
        public ICommand SetLatestVersionCommand { get; }
        public ICommand RefreshLatestVersionsCommand { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Loads initial data for the view.
        /// </summary>
        private async Task LoadInitialDataAsync()
        {
            await ExecuteWithBusyIndicator(async () =>
            {
                // Load machines
                var machines = await _networkScanService.GetSavedMachinesAsync();
                Machines = new ObservableCollection<Machine>(machines.OrderBy(m => m.Hostname));
                
                // Load all software inventory
                var allSoftware = await _softwareInventoryService.GetAllSoftwareInventoryAsync();
                
                // Group by software name and count installations
                var groupedSoftware = allSoftware
                    .GroupBy(s => new { s.Name, s.Publisher })
                    .Select(g => new Software
                    {
                        Name = g.Key.Name,
                        Publisher = g.Key.Publisher,
                        InstalledVersion = GetMostCommonVersion(g.Select(s => s.InstalledVersion)),
                        InstallCount = g.Count(),
                        UpdateAvailable = g.Any(s => s.UpdateAvailable),
                        LatestVersion = GetLatestVersion(g.Select(s => s.LatestVersion)),
                        UseLatestVersion = true
                    })
                    .OrderBy(s => s.Name)
                    .ToList();
                
                SoftwareList = new ObservableCollection<Software>(groupedSoftware);
                
                StatusMessage = "Software inventory loaded successfully.";
            }, "Loading software inventory...");
        }

        /// <summary>
        /// Gets the most common version from a list of versions.
        /// </summary>
        private string GetMostCommonVersion(IEnumerable<string> versions)
        {
            return versions
                .GroupBy(v => v)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault() ?? string.Empty;
        }

        /// <summary>
        /// Gets the latest version from a list of versions.
        /// </summary>
        private string GetLatestVersion(IEnumerable<string> versions)
        {
            var validVersions = versions.Where(v => !string.IsNullOrEmpty(v)).ToList();
            if (validVersions.Count == 0) return string.Empty;
            
            try
            {
                // Try parsing as Version objects
                List<Version> parsedVersions = new List<Version>();
                foreach (var version in validVersions)
                {
                    if (Version.TryParse(version, out Version? parsedVersion))
                    {
                        parsedVersions.Add(parsedVersion);
                    }
                }
                
                if (parsedVersions.Count > 0)
                {
                    var latest = parsedVersions.Max();
                    return latest.ToString();
                }
            }
            catch
            {
                // If parsing fails, use string comparison
            }
            
            // Default to string comparison
            return validVersions.OrderByDescending(v => v).First();
        }

        /// <summary>
        /// Updates the software count statistics.
        /// </summary>
        private void UpdateSoftwareCounts()
        {
            SoftwareCount = SoftwareList.Count;
            UpdatableSoftwareCount = SoftwareList.Count(s => s.UpdateAvailable);
        }

        /// <summary>
        /// Loads software inventory for the selected machine.
        /// </summary>
        private void LoadSoftwareForSelectedMachine()
        {
            if (SelectedMachine == null)
            {
                SoftwareList.Clear();
                return;
            }
            
            // Display software for the selected machine
            SoftwareList = new ObservableCollection<Software>(SelectedMachine.InstalledSoftware);
            
            StatusMessage = $"Loaded {SoftwareList.Count} software items for {SelectedMachine.Hostname}.";
        }

        /// <summary>
        /// Filters the software list based on search criteria.
        /// </summary>
        private void FilterSoftwareList()
        {
            if (SelectedMachine == null) return;
            
            var filteredList = SelectedMachine.InstalledSoftware;
            
            // Apply search filter
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var searchLower = SearchText.ToLower();
                filteredList = filteredList
                    .Where(s => 
                        s.Name.ToLower().Contains(searchLower) || 
                        s.Publisher.ToLower().Contains(searchLower) ||
                        s.InstalledVersion.ToLower().Contains(searchLower))
                    .ToList();
            }
            
            // Apply update filter
            if (ShowOnlyUpdatable)
            {
                filteredList = filteredList.Where(s => s.UpdateAvailable).ToList();
            }
            
            // Update the list
            SoftwareList = new ObservableCollection<Software>(filteredList);
            
            StatusMessage = $"Found {SoftwareList.Count} software items matching filter.";
        }

        /// <summary>
        /// Scans software for a specific machine.
        /// </summary>
        private async Task ScanSoftwareForMachineAsync(Machine? machine)
        {
            if (machine == null || IsInventoryRunning) return;
            
            try
            {
                IsInventoryRunning = true;
                InventoryProgress = 0;
                StatusMessage = $"Scanning software for {machine.Hostname}...";
                
                // Register for progress updates
                _softwareInventoryService.InventoryProgress += OnInventoryProgress;
                
                try
                {
                    // Get credentials
                    var credentials = GetCredentialsForMachine(machine);
                    
                    // Scan software
                    var softwareList = await _softwareInventoryService.GetSoftwareInventoryAsync(
                        machine,
                        credentials);
                    
                    // Update machine with software list
                    machine.InstalledSoftware = softwareList;
                    machine.LastScanned = DateTime.Now;
                    machine.UpdatePendingUpdates();
                    
                    // Save updated machine
                    await _networkScanService.SaveMachinesAsync(new List<Machine> { machine });
                    
                    // Update UI if this is the selected machine
                    if (SelectedMachine != null && SelectedMachine.IPAddress == machine.IPAddress)
                    {
                        LoadSoftwareForSelectedMachine();
                    }
                    
                    StatusMessage = $"Found {softwareList.Count} software items for {machine.Hostname}.";
                }
                finally
                {
                    // Ensure event handler is unregistered
                    _softwareInventoryService.InventoryProgress -= OnInventoryProgress;
                    IsInventoryRunning = false;
                    InventoryProgress = 100;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error scanning software: {ex.Message}";
                IsInventoryRunning = false;
            }
        }

        /// <summary>
        /// Scans software for all machines.
        /// </summary>
        private async Task ScanSoftwareForAllMachinesAsync()
        {
            if (IsInventoryRunning || Machines.Count == 0) return;
            
            try
            {
                IsInventoryRunning = true;
                InventoryProgress = 0;
                StatusMessage = "Scanning software for all machines...";
                
                // Register for progress updates
                _softwareInventoryService.InventoryProgress += OnInventoryProgress;
                
                try
                {
                    // Get online machines
                    var onlineMachines = Machines.Where(m => m.IsOnline).ToList();
                    if (onlineMachines.Count == 0)
                    {
                        StatusMessage = "No online machines found to scan.";
                        return;
                    }
                    
                    // Scan software for each machine
                    int machineCount = 0;
                    foreach (var machine in onlineMachines)
                    {
                        try
                        {
                            // Get credentials
                            var credentials = GetCredentialsForMachine(machine);
                            
                            // Scan software
                            var softwareList = await _softwareInventoryService.GetSoftwareInventoryAsync(
                                machine,
                                credentials);
                            
                            // Update machine with software list
                            machine.InstalledSoftware = softwareList;
                            machine.LastScanned = DateTime.Now;
                            machine.UpdatePendingUpdates();
                            
                            machineCount++;
                            InventoryProgress = (double)machineCount / onlineMachines.Count * 100;
                            StatusMessage = $"Scanned {machineCount} of {onlineMachines.Count} machines...";
                        }
                        catch (Exception ex)
                        {
                            // Log the error but continue with next machine
                            machine.Status = $"Error: {ex.Message}";
                        }
                    }
                    
                    // Save all updated machines
                    await _networkScanService.SaveMachinesAsync(onlineMachines);
                    
                    // Update UI if a machine is selected
                    if (SelectedMachine != null)
                    {
                        LoadSoftwareForSelectedMachine();
                    }
                    
                    StatusMessage = $"Completed scanning software for {machineCount} machines.";
                }
                finally
                {
                    // Ensure event handler is unregistered
                    _softwareInventoryService.InventoryProgress -= OnInventoryProgress;
                    IsInventoryRunning = false;
                    InventoryProgress = 100;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error scanning software: {ex.Message}";
                IsInventoryRunning = false;
            }
        }

        /// <summary>
        /// Cancels an ongoing software inventory scan.
        /// </summary>
        private void CancelScan()
        {
            _softwareInventoryService.CancelInventory();
            StatusMessage = "Software scan cancelled.";
        }

        /// <summary>
        /// Handles progress updates during software scanning.
        /// </summary>
        private void OnInventoryProgress(object? sender, double progress)
        {
            InventoryProgress = progress;
        }

        /// <summary>
        /// Gets credentials for a specific machine.
        /// </summary>
        private Credentials GetCredentialsForMachine(Machine machine)
        {
            // Use machine-specific credentials if available, otherwise use global credentials
            if (!string.IsNullOrEmpty(machine.Username) && !string.IsNullOrEmpty(machine.Password))
            {
                return new Credentials
                {
                    Domain = machine.Domain,
                    Username = machine.Username,
                    Password = machine.Password
                };
            }
            
            // Use global credentials
            return _settingsService.CurrentSettings.GlobalCredentials;
        }

        /// <summary>
        /// Selects all software in the list.
        /// </summary>
        private void SelectAllSoftware()
        {
            foreach (var software in SoftwareList)
            {
                software.IsSelected = true;
            }
        }

        /// <summary>
        /// Deselects all software in the list.
        /// </summary>
        private void DeselectAllSoftware()
        {
            foreach (var software in SoftwareList)
            {
                software.IsSelected = false;
            }
        }

        /// <summary>
        /// Sets the latest version for a software.
        /// </summary>
        private void SetLatestVersion(Software? software)
        {
            if (software == null) return;
            
            // Prompt user for latest version
            var latestVersion = software.LatestVersion;
            if (string.IsNullOrEmpty(latestVersion))
            {
                // Use a dialog to get the latest version
                // For now, we'll simulate with a default value
                latestVersion = "1.0.0";
            }
            
            // Update the software
            software.LatestVersion = latestVersion;
            software.UseLatestVersion = true;
            
            // Update all instances of this software
            foreach (var machine in Machines)
            {
                foreach (var installedSoftware in machine.InstalledSoftware)
                {
                    if (installedSoftware.Name == software.Name && 
                        installedSoftware.Publisher == software.Publisher)
                    {
                        installedSoftware.LatestVersion = latestVersion;
                        installedSoftware.UseLatestVersion = true;
                    }
                }
            }
            
            // Save changes
            _softwareInventoryService.SaveSoftwareInventory(Machines.ToList());
            
            StatusMessage = $"Set latest version of {software.Name} to {latestVersion}.";
        }

        /// <summary>
        /// Refreshes latest versions of all software.
        /// </summary>
        private async Task RefreshLatestVersionsAsync()
        {
            await ExecuteWithBusyIndicator(async () =>
            {
                // This would connect to a software repository or vendor API
                // For now, we'll simulate by using a fake update for some software
                
                // Get all unique software
                var uniqueSoftware = SoftwareList
                    .GroupBy(s => new { s.Name, s.Publisher })
                    .Select(g => g.First())
                    .ToList();
                
                int updateCount = 0;
                foreach (var software in uniqueSoftware)
                {
                    // Check if an update is available (simulated)
                    if (!string.IsNullOrEmpty(software.InstalledVersion))
                    {
                        try
                        {
                            var version = Version.Parse(software.InstalledVersion);
                            var newVersion = new Version(version.Major, version.Minor, version.Build + 1);
                            
                            software.LatestVersion = newVersion.ToString();
                            updateCount++;
                            
                            // Update all instances of this software
                            foreach (var machine in Machines)
                            {
                                foreach (var installedSoftware in machine.InstalledSoftware)
                                {
                                    if (installedSoftware.Name == software.Name && 
                                        installedSoftware.Publisher == software.Publisher)
                                    {
                                        installedSoftware.LatestVersion = newVersion.ToString();
                                    }
                                }
                            }
                        }
                        catch
                        {
                            // If version parsing fails, simulate a minor update
                            software.LatestVersion = software.InstalledVersion + ".1";
                            updateCount++;
                        }
                    }
                }
                
                // Save changes
                await Task.Run(() => _softwareInventoryService.SaveSoftwareInventory(Machines.ToList()));
                
                // Update UI
                UpdateSoftwareCounts();
                
                StatusMessage = $"Found {updateCount} software updates.";
            }, "Checking for software updates...");
        }

        #endregion
    }
}
