using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;
using NetworkSoftwareManager.Models;
using NetworkSoftwareManager.Services;
using NetworkSoftwareManager.Utils;

namespace NetworkSoftwareManager.ViewModels
{
    /// <summary>
    /// View model for the network scanning functionality.
    /// </summary>
    public class NetworkScanViewModel : BaseViewModel
    {
        private readonly NetworkScanService _networkScanService;
        private readonly SettingsService _settingsService;
        
        private ObservableCollection<Machine> _machines = new();
        private Machine? _selectedMachine;
        private string _ipRangeToScan = string.Empty;
        private string _excludedIPs = string.Empty;
        private TimeSpan _scanTimeout = TimeSpan.FromMilliseconds(500);
        private int _threadCount = 10;
        private bool _isNetworkScanRunning;
        private double _scanProgress;
        private int _machinesFound;
        private int _machinesOnline;
        private string _importFilePath = string.Empty;

        public NetworkScanViewModel()
        {
            _networkScanService = new NetworkScanService();
            _settingsService = new SettingsService();

            // Load settings
            var settings = _settingsService.CurrentSettings;
            IPRangeToScan = settings.IPRangesToScan;
            ExcludedIPs = string.Join(", ", settings.ExcludedIPs);
            ScanTimeout = settings.ScanTimeout;
            ThreadCount = settings.ThreadCount;

            // Initialize commands
            ScanNetworkCommand = new RelayCommand(async () => await ScanNetworkAsync(), () => !IsNetworkScanRunning);
            CancelScanCommand = new RelayCommand(CancelScan, () => IsNetworkScanRunning);
            SelectAllCommand = new RelayCommand(SelectAllMachines);
            DeselectAllCommand = new RelayCommand(DeselectAllMachines);
            ImportMachinesCommand = new RelayCommand(ImportMachines);
            ExportMachinesCommand = new RelayCommand(ExportMachines, () => Machines.Count > 0);
            SaveSelectedCommand = new RelayCommand(SaveSelectedMachines, () => Machines.Any(m => m.IsSelected));
            RemoveSelectedCommand = new RelayCommand(RemoveSelectedMachines, () => Machines.Any(m => m.IsSelected));
            
            // Load initial data
            Task.Run(async () => await LoadSavedMachinesAsync());
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

        public Machine? SelectedMachine
        {
            get => _selectedMachine;
            set
            {
                if (_selectedMachine != value)
                {
                    _selectedMachine = value;
                    OnPropertyChanged();
                }
            }
        }

        public string IPRangeToScan
        {
            get => _ipRangeToScan;
            set
            {
                if (_ipRangeToScan != value)
                {
                    _ipRangeToScan = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ExcludedIPs
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

        public bool IsNetworkScanRunning
        {
            get => _isNetworkScanRunning;
            set
            {
                if (_isNetworkScanRunning != value)
                {
                    _isNetworkScanRunning = value;
                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public double ScanProgress
        {
            get => _scanProgress;
            set
            {
                if (Math.Abs(_scanProgress - value) > 0.001)
                {
                    _scanProgress = value;
                    OnPropertyChanged();
                }
            }
        }

        public int MachinesFound
        {
            get => _machinesFound;
            set
            {
                if (_machinesFound != value)
                {
                    _machinesFound = value;
                    OnPropertyChanged();
                }
            }
        }

        public int MachinesOnline
        {
            get => _machinesOnline;
            set
            {
                if (_machinesOnline != value)
                {
                    _machinesOnline = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ImportFilePath
        {
            get => _importFilePath;
            set
            {
                if (_importFilePath != value)
                {
                    _importFilePath = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Commands

        public ICommand ScanNetworkCommand { get; }
        public ICommand CancelScanCommand { get; }
        public ICommand SelectAllCommand { get; }
        public ICommand DeselectAllCommand { get; }
        public ICommand ImportMachinesCommand { get; }
        public ICommand ExportMachinesCommand { get; }
        public ICommand SaveSelectedCommand { get; }
        public ICommand RemoveSelectedCommand { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Scans the network for machines based on configured IP ranges.
        /// </summary>
        private async Task ScanNetworkAsync()
        {
            if (IsNetworkScanRunning) return;

            try
            {
                // Save current settings
                SaveScanSettings();

                // Parse excluded IPs
                var excludedIPsList = ExcludedIPs.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(ip => ip.Trim())
                    .ToList();

                // Parse IP ranges
                var ipRanges = NetworkHelper.ParseIPRanges(IPRangeToScan);
                if (ipRanges.Count == 0)
                {
                    StatusMessage = "Invalid IP range format.";
                    return;
                }

                IsNetworkScanRunning = true;
                ScanProgress = 0;
                MachinesFound = 0;
                MachinesOnline = 0;
                StatusMessage = "Scanning network...";

                // Register for progress updates
                _networkScanService.ScanProgress += OnScanProgress;

                try
                {
                    // Start the scan
                    var results = await _networkScanService.ScanNetworkAsync(
                        ipRanges,
                        excludedIPsList,
                        ScanTimeout,
                        ThreadCount);

                    // Update the machines collection
                    UpdateMachinesCollection(results);

                    // Update statistics
                    MachinesFound = results.Count;
                    MachinesOnline = results.Count(m => m.IsOnline);

                    StatusMessage = $"Scan completed. Found {MachinesFound} machines, {MachinesOnline} online.";
                }
                finally
                {
                    // Ensure event handler is unregistered
                    _networkScanService.ScanProgress -= OnScanProgress;
                    IsNetworkScanRunning = false;
                    ScanProgress = 100;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error scanning network: {ex.Message}";
                IsNetworkScanRunning = false;
            }
        }

        /// <summary>
        /// Handles progress updates during network scanning.
        /// </summary>
        private void OnScanProgress(object? sender, double progress)
        {
            ScanProgress = progress;
        }

        /// <summary>
        /// Cancels an ongoing network scan.
        /// </summary>
        private void CancelScan()
        {
            _networkScanService.CancelScan();
            StatusMessage = "Scan cancelled.";
        }

        /// <summary>
        /// Selects all machines in the list.
        /// </summary>
        private void SelectAllMachines()
        {
            foreach (var machine in Machines)
            {
                machine.IsSelected = true;
            }
        }

        /// <summary>
        /// Deselects all machines in the list.
        /// </summary>
        private void DeselectAllMachines()
        {
            foreach (var machine in Machines)
            {
                machine.IsSelected = false;
            }
        }

        /// <summary>
        /// Imports machines from a CSV or JSON file.
        /// </summary>
        private void ImportMachines()
        {
            try
            {
                // Open file dialog
                var openFileDialog = new OpenFileDialog
                {
                    Filter = "Machine Files|*.csv;*.json|All Files|*.*",
                    Title = "Import Machines"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    ImportFilePath = openFileDialog.FileName;
                    
                    // Import machines
                    var importedMachines = _networkScanService.ImportMachinesFromFile(ImportFilePath);
                    if (importedMachines.Count > 0)
                    {
                        // Add imported machines to collection
                        foreach (var machine in importedMachines)
                        {
                            if (!Machines.Any(m => m.IPAddress == machine.IPAddress))
                            {
                                Machines.Add(machine);
                            }
                        }
                        
                        StatusMessage = $"Imported {importedMachines.Count} machines from {Path.GetFileName(ImportFilePath)}.";
                    }
                    else
                    {
                        StatusMessage = "No machines found in the import file.";
                    }
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error importing machines: {ex.Message}";
            }
        }

        /// <summary>
        /// Exports machines to a CSV or JSON file.
        /// </summary>
        private void ExportMachines()
        {
            try
            {
                // Open save file dialog
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "CSV File|*.csv|JSON File|*.json|All Files|*.*",
                    Title = "Export Machines",
                    DefaultExt = ".csv"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName;
                    
                    // Export machines
                    _networkScanService.ExportMachinesToFile(Machines.ToList(), filePath);
                    
                    StatusMessage = $"Exported {Machines.Count} machines to {Path.GetFileName(filePath)}.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error exporting machines: {ex.Message}";
            }
        }

        /// <summary>
        /// Saves selected machines to the persistent storage.
        /// </summary>
        private void SaveSelectedMachines()
        {
            try
            {
                var selectedMachines = Machines.Where(m => m.IsSelected).ToList();
                if (selectedMachines.Count > 0)
                {
                    _networkScanService.SaveMachines(selectedMachines);
                    StatusMessage = $"Saved {selectedMachines.Count} machines.";
                    
                    // Update settings
                    SaveScanSettings();
                }
                else
                {
                    StatusMessage = "No machines selected to save.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error saving machines: {ex.Message}";
            }
        }

        /// <summary>
        /// Removes selected machines from the list.
        /// </summary>
        private void RemoveSelectedMachines()
        {
            try
            {
                var selectedMachines = Machines.Where(m => m.IsSelected).ToList();
                if (selectedMachines.Count > 0)
                {
                    foreach (var machine in selectedMachines)
                    {
                        Machines.Remove(machine);
                    }
                    
                    StatusMessage = $"Removed {selectedMachines.Count} machines from the list.";
                }
                else
                {
                    StatusMessage = "No machines selected to remove.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error removing machines: {ex.Message}";
            }
        }

        /// <summary>
        /// Updates the machines collection with scan results.
        /// </summary>
        private void UpdateMachinesCollection(List<Machine> scanResults)
        {
            // Create a new collection with the scan results
            var newMachines = new ObservableCollection<Machine>();
            
            // First add existing machines that are not in scan results
            foreach (var existingMachine in Machines)
            {
                var resultMachine = scanResults.FirstOrDefault(m => m.IPAddress == existingMachine.IPAddress);
                if (resultMachine == null)
                {
                    // Machine was not in scan results, add the existing one
                    existingMachine.IsOnline = false;
                    newMachines.Add(existingMachine);
                }
            }
            
            // Then add all scan results
            foreach (var resultMachine in scanResults)
            {
                var existingMachine = Machines.FirstOrDefault(m => m.IPAddress == resultMachine.IPAddress);
                if (existingMachine != null)
                {
                    // Update existing machine properties
                    existingMachine.IsOnline = resultMachine.IsOnline;
                    existingMachine.Hostname = resultMachine.Hostname;
                    existingMachine.OperatingSystem = resultMachine.OperatingSystem;
                    existingMachine.LastScanned = DateTime.Now;
                    existingMachine.Status = resultMachine.Status;
                    
                    newMachines.Add(existingMachine);
                }
                else
                {
                    // New machine
                    resultMachine.LastScanned = DateTime.Now;
                    newMachines.Add(resultMachine);
                }
            }
            
            // Update the collection
            Machines = new ObservableCollection<Machine>(newMachines.OrderBy(m => m.IPAddress));
        }

        /// <summary>
        /// Loads saved machines from the persistent storage.
        /// </summary>
        private async Task LoadSavedMachinesAsync()
        {
            await ExecuteWithBusyIndicator(async () =>
            {
                var savedMachines = await _networkScanService.GetSavedMachinesAsync();
                Machines = new ObservableCollection<Machine>(savedMachines);
                
                MachinesFound = savedMachines.Count;
                MachinesOnline = savedMachines.Count(m => m.IsOnline);
                
                StatusMessage = $"Loaded {savedMachines.Count} saved machines.";
            }, "Loading saved machines...");
        }

        /// <summary>
        /// Saves the current scan settings.
        /// </summary>
        private void SaveScanSettings()
        {
            var settings = _settingsService.CurrentSettings;
            
            settings.IPRangesToScan = IPRangeToScan;
            settings.ExcludedIPs = ExcludedIPs.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(ip => ip.Trim())
                .ToList();
            settings.ScanTimeout = ScanTimeout;
            settings.ThreadCount = ThreadCount;
            
            _settingsService.SaveSettings();
        }

        #endregion
    }
}
