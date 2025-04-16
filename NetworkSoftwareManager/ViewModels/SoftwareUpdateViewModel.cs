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
    /// View model for the software update functionality.
    /// </summary>
    public class SoftwareUpdateViewModel : BaseViewModel
    {
        private readonly SoftwareUpdateService _softwareUpdateService;
        private readonly SoftwareInventoryService _softwareInventoryService;
        private readonly NetworkScanService _networkScanService;
        private readonly SettingsService _settingsService;
        
        private ObservableCollection<SoftwareUpdate> _pendingUpdates = new();
        private ObservableCollection<SoftwareUpdate> _completedUpdates = new();
        private ObservableCollection<Machine> _targetMachines = new();
        private ObservableCollection<Software> _softwareToUpdate = new();
        private SoftwareUpdate? _selectedUpdate;
        private Machine? _selectedMachine;
        private Software? _selectedSoftware;
        private string _updateName = string.Empty;
        private bool _useLatestVersion = true;
        private string _targetVersion = string.Empty;
        private string _installCommandTemplate = string.Empty;
        private string _uninstallCommandTemplate = string.Empty;
        private string _updateFilePath = string.Empty;
        private bool _forceReinstall;
        private bool _isUpdateRunning;
        private double _updateProgress;

        public SoftwareUpdateViewModel()
        {
            _softwareUpdateService = new SoftwareUpdateService();
            _softwareInventoryService = new SoftwareInventoryService();
            _networkScanService = new NetworkScanService();
            _settingsService = new SettingsService();

            // Initialize commands
            RefreshCommand = new RelayCommand(async () => await LoadUpdatesAsync());
            
            CreateUpdateTaskCommand = new RelayCommand(CreateUpdateTask, 
                () => TargetMachines.Count > 0 && SoftwareToUpdate.Count > 0 && !string.IsNullOrEmpty(UpdateName));
            
            DeployUpdateCommand = new RelayCommand<SoftwareUpdate>(async update => await DeployUpdateAsync(update), 
                update => update != null && update.Status == "Pending" && !IsUpdateRunning);
            
            CancelUpdateCommand = new RelayCommand(CancelUpdate, () => IsUpdateRunning);
            
            RemoveUpdateCommand = new RelayCommand<SoftwareUpdate>(RemoveUpdate, 
                update => update != null);
            
            SelectAllMachinesCommand = new RelayCommand(SelectAllMachines);
            
            DeselectAllMachinesCommand = new RelayCommand(DeselectAllMachines);
            
            SelectAllSoftwareCommand = new RelayCommand(SelectAllSoftware);
            
            DeselectAllSoftwareCommand = new RelayCommand(DeselectAllSoftware);
            
            BrowseUpdateFileCommand = new RelayCommand(BrowseUpdateFile);
            
            ViewUpdateDetailsCommand = new RelayCommand<SoftwareUpdate>(ViewUpdateDetails,
                update => update != null);
            
            // Load initial data
            Task.Run(async () => await LoadInitialDataAsync());
        }

        #region Properties

        public ObservableCollection<SoftwareUpdate> PendingUpdates
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

        public ObservableCollection<SoftwareUpdate> CompletedUpdates
        {
            get => _completedUpdates;
            set
            {
                if (_completedUpdates != value)
                {
                    _completedUpdates = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Machine> TargetMachines
        {
            get => _targetMachines;
            set
            {
                if (_targetMachines != value)
                {
                    _targetMachines = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Software> SoftwareToUpdate
        {
            get => _softwareToUpdate;
            set
            {
                if (_softwareToUpdate != value)
                {
                    _softwareToUpdate = value;
                    OnPropertyChanged();
                }
            }
        }

        public SoftwareUpdate? SelectedUpdate
        {
            get => _selectedUpdate;
            set
            {
                if (_selectedUpdate != value)
                {
                    _selectedUpdate = value;
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

        public Software? SelectedSoftware
        {
            get => _selectedSoftware;
            set
            {
                if (_selectedSoftware != value)
                {
                    _selectedSoftware = value;
                    OnPropertyChanged();
                    if (_selectedSoftware != null)
                    {
                        UpdateName = _selectedSoftware.Name;
                        UseLatestVersion = _selectedSoftware.UseLatestVersion;
                        TargetVersion = _selectedSoftware.LatestVersion;
                    }
                }
            }
        }

        public string UpdateName
        {
            get => _updateName;
            set
            {
                if (_updateName != value)
                {
                    _updateName = value;
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

        public string InstallCommandTemplate
        {
            get => _installCommandTemplate;
            set
            {
                if (_installCommandTemplate != value)
                {
                    _installCommandTemplate = value;
                    OnPropertyChanged();
                }
            }
        }

        public string UninstallCommandTemplate
        {
            get => _uninstallCommandTemplate;
            set
            {
                if (_uninstallCommandTemplate != value)
                {
                    _uninstallCommandTemplate = value;
                    OnPropertyChanged();
                }
            }
        }

        public string UpdateFilePath
        {
            get => _updateFilePath;
            set
            {
                if (_updateFilePath != value)
                {
                    _updateFilePath = value;
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

        public bool IsUpdateRunning
        {
            get => _isUpdateRunning;
            set
            {
                if (_isUpdateRunning != value)
                {
                    _isUpdateRunning = value;
                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public double UpdateProgress
        {
            get => _updateProgress;
            set
            {
                if (Math.Abs(_updateProgress - value) > 0.001)
                {
                    _updateProgress = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Commands

        public ICommand RefreshCommand { get; }
        public ICommand CreateUpdateTaskCommand { get; }
        public ICommand DeployUpdateCommand { get; }
        public ICommand CancelUpdateCommand { get; }
        public ICommand RemoveUpdateCommand { get; }
        public ICommand SelectAllMachinesCommand { get; }
        public ICommand DeselectAllMachinesCommand { get; }
        public ICommand SelectAllSoftwareCommand { get; }
        public ICommand DeselectAllSoftwareCommand { get; }
        public ICommand BrowseUpdateFileCommand { get; }
        public ICommand ViewUpdateDetailsCommand { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Loads initial data for the view.
        /// </summary>
        private async Task LoadInitialDataAsync()
        {
            await ExecuteWithBusyIndicator(async () =>
            {
                // Load updates
                await LoadUpdatesAsync();
                
                // Load machines
                var machines = await _networkScanService.GetSavedMachinesAsync();
                TargetMachines = new ObservableCollection<Machine>(
                    machines.Where(m => m.IsOnline).OrderBy(m => m.Hostname));
                
                // Load software
                var allSoftware = await _softwareInventoryService.GetAllSoftwareInventoryAsync();
                var uniqueSoftware = allSoftware
                    .GroupBy(s => new { s.Name, s.Publisher })
                    .Select(g => g.First())
                    .OrderBy(s => s.Name)
                    .ToList();
                
                SoftwareToUpdate = new ObservableCollection<Software>(uniqueSoftware);
                
                // Set default command templates
                InstallCommandTemplate = "msiexec /i \"{FILEPATH}\" /qn";
                UninstallCommandTemplate = "msiexec /x \"{PRODUCTCODE}\" /qn";
                
                StatusMessage = "Ready to create update tasks.";
            }, "Loading update data...");
        }

        /// <summary>
        /// Loads pending and completed updates.
        /// </summary>
        private async Task LoadUpdatesAsync()
        {
            var updates = await _softwareUpdateService.GetRecentUpdatesAsync();
            
            PendingUpdates = new ObservableCollection<SoftwareUpdate>(
                updates.Where(u => u.Status == "Pending" || u.Status == "In Progress")
                       .OrderBy(u => u.CreatedDate));
            
            CompletedUpdates = new ObservableCollection<SoftwareUpdate>(
                updates.Where(u => u.Status == "Completed")
                       .OrderByDescending(u => u.CompletedDate));
            
            StatusMessage = $"Loaded {PendingUpdates.Count} pending and {CompletedUpdates.Count} completed updates.";
        }

        /// <summary>
        /// Creates a new update task.
        /// </summary>
        private void CreateUpdateTask()
        {
            if (string.IsNullOrEmpty(UpdateName))
            {
                StatusMessage = "Please enter an update name.";
                return;
            }
            
            if (TargetMachines.Count(m => m.IsSelected) == 0)
            {
                StatusMessage = "Please select at least one target machine.";
                return;
            }
            
            if (SoftwareToUpdate.Count(s => s.IsSelected) == 0)
            {
                StatusMessage = "Please select at least one software to update.";
                return;
            }
            
            try
            {
                // Create update tasks for each selected software
                foreach (var software in SoftwareToUpdate.Where(s => s.IsSelected))
                {
                    var updateTask = new SoftwareUpdate
                    {
                        SoftwareName = software.Name,
                        Publisher = software.Publisher,
                        TargetVersion = UseLatestVersion ? software.LatestVersion : TargetVersion,
                        UseLatestVersion = UseLatestVersion,
                        TargetMachines = TargetMachines.Where(m => m.IsSelected).ToList(),
                        UpdateStagingPath = UpdateFilePath,
                        InstallCommand = InstallCommandTemplate,
                        UninstallCommand = UninstallCommandTemplate,
                        ForceReinstall = ForceReinstall,
                        Status = "Pending",
                        CreatedDate = DateTime.Now
                    };
                    
                    // Save the update task
                    _softwareUpdateService.SaveUpdateTask(updateTask);
                    
                    // Add to pending updates
                    PendingUpdates.Add(updateTask);
                }
                
                // Reset form
                UpdateName = string.Empty;
                TargetVersion = string.Empty;
                UpdateFilePath = string.Empty;
                ForceReinstall = false;
                
                // Deselect all
                foreach (var machine in TargetMachines)
                {
                    machine.IsSelected = false;
                }
                
                foreach (var software in SoftwareToUpdate)
                {
                    software.IsSelected = false;
                }
                
                StatusMessage = "Update task(s) created successfully.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error creating update task: {ex.Message}";
            }
        }

        /// <summary>
        /// Deploys a software update to target machines.
        /// </summary>
        private async Task DeployUpdateAsync(SoftwareUpdate? update)
        {
            if (update == null || IsUpdateRunning) return;
            
            try
            {
                IsUpdateRunning = true;
                UpdateProgress = 0;
                update.Status = "In Progress";
                StatusMessage = $"Deploying {update.SoftwareName} to {update.TargetMachines.Count} machines...";
                
                // Register for progress updates
                _softwareUpdateService.UpdateProgress += OnUpdateProgress;
                
                try
                {
                    // Start the deployment
                    await _softwareUpdateService.DeployUpdateAsync(update);
                    
                    // Update was completed (success or partial)
                    update.Status = "Completed";
                    update.CompletedDate = DateTime.Now;
                    
                    // Save the update status
                    _softwareUpdateService.SaveUpdateTask(update);
                    
                    // Move from pending to completed
                    PendingUpdates.Remove(update);
                    CompletedUpdates.Insert(0, update);
                    
                    StatusMessage = $"Update deployed: {update.SuccessCount} succeeded, {update.FailureCount} failed.";
                }
                finally
                {
                    // Ensure event handler is unregistered
                    _softwareUpdateService.UpdateProgress -= OnUpdateProgress;
                    IsUpdateRunning = false;
                    UpdateProgress = 100;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error deploying update: {ex.Message}";
                IsUpdateRunning = false;
                update.Status = "Failed";
            }
        }

        /// <summary>
        /// Cancels an ongoing update deployment.
        /// </summary>
        private void CancelUpdate()
        {
            _softwareUpdateService.CancelUpdate();
            StatusMessage = "Update deployment cancelled.";
        }

        /// <summary>
        /// Removes an update from the list.
        /// </summary>
        private void RemoveUpdate(SoftwareUpdate? update)
        {
            if (update == null) return;
            
            try
            {
                // Remove from appropriate list
                if (update.Status == "Pending" || update.Status == "In Progress")
                {
                    PendingUpdates.Remove(update);
                }
                else
                {
                    CompletedUpdates.Remove(update);
                }
                
                // Delete the update task
                _softwareUpdateService.DeleteUpdateTask(update.Id);
                
                StatusMessage = "Update task removed.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error removing update: {ex.Message}";
            }
        }

        /// <summary>
        /// Handles progress updates during update deployment.
        /// </summary>
        private void OnUpdateProgress(object? sender, double progress)
        {
            UpdateProgress = progress;
        }

        /// <summary>
        /// Selects all machines in the list.
        /// </summary>
        private void SelectAllMachines()
        {
            foreach (var machine in TargetMachines)
            {
                machine.IsSelected = true;
            }
        }

        /// <summary>
        /// Deselects all machines in the list.
        /// </summary>
        private void DeselectAllMachines()
        {
            foreach (var machine in TargetMachines)
            {
                machine.IsSelected = false;
            }
        }

        /// <summary>
        /// Selects all software in the list.
        /// </summary>
        private void SelectAllSoftware()
        {
            foreach (var software in SoftwareToUpdate)
            {
                software.IsSelected = true;
            }
        }

        /// <summary>
        /// Deselects all software in the list.
        /// </summary>
        private void DeselectAllSoftware()
        {
            foreach (var software in SoftwareToUpdate)
            {
                software.IsSelected = false;
            }
        }

        /// <summary>
        /// Opens a dialog to browse for update files.
        /// </summary>
        private void BrowseUpdateFile()
        {
            try
            {
                // Open file dialog
                var openFileDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "Installation Files|*.msi;*.exe|All Files|*.*",
                    Title = "Select Update File"
                };
                
                if (openFileDialog.ShowDialog() == true)
                {
                    UpdateFilePath = openFileDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error selecting file: {ex.Message}";
            }
        }

        /// <summary>
        /// Views details of an update.
        /// </summary>
        private void ViewUpdateDetails(SoftwareUpdate? update)
        {
            if (update == null) return;
            
            // In a real implementation, this would open a details dialog
            // For now, just select the update
            SelectedUpdate = update;
            
            StatusMessage = $"Viewing details for {update.SoftwareName} update.";
        }

        #endregion
    }
}
