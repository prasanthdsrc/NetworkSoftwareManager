using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NetworkSoftwareManager.Models;

namespace NetworkSoftwareManager.Services
{
    /// <summary>
    /// Service for deploying software updates to remote machines.
    /// </summary>
    public class SoftwareUpdateService
    {
        private readonly string _storagePath;
        private readonly SettingsService _settingsService;
        private CancellationTokenSource? _cancellationTokenSource;

        public event EventHandler<double>? UpdateProgress;

        public SoftwareUpdateService()
        {
            _settingsService = new SettingsService();
            
            // Set up storage path
            _storagePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "NetworkSoftwareManager",
                "Data");
            
            // Ensure the directory exists
            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
        }

        /// <summary>
        /// Gets a list of recent software updates.
        /// </summary>
        public async Task<List<SoftwareUpdate>> GetRecentUpdatesAsync()
        {
            try
            {
                string filePath = Path.Combine(_storagePath, "updates.json");
                
                if (File.Exists(filePath))
                {
                    string json = await File.ReadAllTextAsync(filePath);
                    return JsonConvert.DeserializeObject<List<SoftwareUpdate>>(json) ?? new List<SoftwareUpdate>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading updates: {ex.Message}");
            }
            
            return new List<SoftwareUpdate>();
        }

        /// <summary>
        /// Saves an update task to the database.
        /// </summary>
        public void SaveUpdateTask(SoftwareUpdate update)
        {
            try
            {
                // Load existing updates
                var updates = GetRecentUpdates();
                
                // Find and update or add the update task
                var existingUpdate = updates.FirstOrDefault(u => u.Id == update.Id);
                if (existingUpdate != null)
                {
                    // Update existing task
                    int index = updates.IndexOf(existingUpdate);
                    updates[index] = update;
                }
                else
                {
                    // Add new task
                    updates.Add(update);
                }
                
                // Save updates
                SaveUpdates(updates);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving update task: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Deletes an update task from the database.
        /// </summary>
        public void DeleteUpdateTask(string updateId)
        {
            try
            {
                // Load existing updates
                var updates = GetRecentUpdates();
                
                // Remove the update task
                updates.RemoveAll(u => u.Id == updateId);
                
                // Save updates
                SaveUpdates(updates);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting update task: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets a list of recent updates.
        /// </summary>
        private List<SoftwareUpdate> GetRecentUpdates()
        {
            try
            {
                string filePath = Path.Combine(_storagePath, "updates.json");
                
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    return JsonConvert.DeserializeObject<List<SoftwareUpdate>>(json) ?? new List<SoftwareUpdate>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading updates: {ex.Message}");
            }
            
            return new List<SoftwareUpdate>();
        }

        /// <summary>
        /// Saves updates to the database.
        /// </summary>
        private void SaveUpdates(List<SoftwareUpdate> updates)
        {
            string filePath = Path.Combine(_storagePath, "updates.json");
            string json = JsonConvert.SerializeObject(updates, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Deploys a software update to target machines.
        /// </summary>
        public async Task DeployUpdateAsync(SoftwareUpdate update)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;
            
            try
            {
                // Check if the update is already in progress
                if (update.Status == "In Progress")
                {
                    throw new InvalidOperationException("Update is already in progress");
                }
                
                // Update status
                update.Status = "In Progress";
                update.ProgressPercentage = 0;
                update.SuccessCount = 0;
                update.FailureCount = 0;
                SaveUpdateTask(update);
                
                OnUpdateProgress(0);
                
                // Get target machines that are online
                var onlineMachines = update.TargetMachines.Where(m => m.IsOnline).ToList();
                if (onlineMachines.Count == 0)
                {
                    update.Status = "Failed";
                    update.CompletedDate = DateTime.Now;
                    SaveUpdateTask(update);
                    throw new InvalidOperationException("No online machines found");
                }
                
                // Create a semaphore to limit concurrent operations
                int maxConcurrent = _settingsService.CurrentSettings.ThreadCount;
                using (var semaphore = new SemaphoreSlim(maxConcurrent))
                {
                    // Create a list to hold all the tasks
                    var tasks = new List<Task>();
                    int processedCount = 0;
                    
                    // Process each machine
                    foreach (var machine in onlineMachines)
                    {
                        // Check for cancellation
                        if (cancellationToken.IsCancellationRequested)
                        {
                            break;
                        }
                        
                        // Wait for a slot in the semaphore
                        await semaphore.WaitAsync(cancellationToken);
                        
                        // Start a new task for this machine
                        var task = Task.Run(async () =>
                        {
                            bool success = false;
                            
                            try
                            {
                                // Get credentials for this machine
                                var credentials = GetCredentialsForMachine(machine);
                                
                                // Deploy the update
                                success = await DeployUpdateToMachineAsync(machine, update, credentials, cancellationToken);
                                
                                // Update counts
                                if (success)
                                {
                                    Interlocked.Increment(ref update.SuccessCount);
                                }
                                else
                                {
                                    Interlocked.Increment(ref update.FailureCount);
                                }
                            }
                            catch (Exception ex)
                            {
                                // Log the error
                                machine.Status = $"Update failed: {ex.Message}";
                                Interlocked.Increment(ref update.FailureCount);
                            }
                            finally
                            {
                                // Increment processed count and report progress
                                Interlocked.Increment(ref processedCount);
                                double progress = (double)processedCount / onlineMachines.Count * 100;
                                update.ProgressPercentage = progress;
                                OnUpdateProgress(progress);
                                
                                // Save the update
                                SaveUpdateTask(update);
                                
                                // Release the semaphore
                                semaphore.Release();
                            }
                        }, cancellationToken);
                        
                        tasks.Add(task);
                    }
                    
                    // Wait for all tasks to complete
                    await Task.WhenAll(tasks);
                }
                
                // Update status
                update.Status = "Completed";
                update.CompletedDate = DateTime.Now;
                update.ProgressPercentage = 100;
                SaveUpdateTask(update);
                
                OnUpdateProgress(100);
            }
            catch (OperationCanceledException)
            {
                // Update was cancelled
                update.Status = "Cancelled";
                update.CompletedDate = DateTime.Now;
                SaveUpdateTask(update);
            }
            catch (Exception ex)
            {
                // Update failed
                update.Status = $"Failed: {ex.Message}";
                update.CompletedDate = DateTime.Now;
                SaveUpdateTask(update);
                
                throw;
            }
            finally
            {
                _cancellationTokenSource = null;
            }
        }

        /// <summary>
        /// Deploys a software update to a specific machine.
        /// </summary>
        private async Task<bool> DeployUpdateToMachineAsync(
            Machine machine, 
            SoftwareUpdate update, 
            Credentials credentials, 
            CancellationToken cancellationToken)
        {
            // Find the software on the machine
            var software = machine.InstalledSoftware.FirstOrDefault(s => 
                s.Name == update.SoftwareName && s.Publisher == update.Publisher);
            
            // Check if update is needed
            if (software == null)
            {
                // Software not found on this machine
                machine.Status = $"{update.SoftwareName} not found";
                return false;
            }
            
            // Check if already at target version and not forcing reinstall
            if (!update.ForceReinstall && software.InstalledVersion == update.TargetVersion)
            {
                machine.Status = $"{update.SoftwareName} already at version {update.TargetVersion}";
                return true;
            }
            
            // Create a PowerShell runspace
            var connectionInfo = new WSManConnectionInfo(
                new Uri($"http://{machine.IPAddress}:5985/wsman"),
                "http://schemas.microsoft.com/powershell/Microsoft.PowerShell",
                credentials.GetFormattedCredentials());
            
            connectionInfo.AuthenticationMechanism = AuthenticationMechanism.Default;
            connectionInfo.OperationTimeout = TimeSpan.FromMilliseconds(_settingsService.CurrentSettings.ConnectionTimeout);
            
            using (var runspace = RunspaceFactory.CreateRunspace(connectionInfo))
            {
                try
                {
                    // Open the runspace
                    await Task.Run(() => runspace.Open(), cancellationToken);
                    
                    // Create PowerShell pipeline
                    using (var powershell = PowerShell.Create())
                    {
                        powershell.Runspace = runspace;
                        
                        // Prepare staging directory on remote machine
                        string remoteStagingDir = @"C:\Windows\Temp\SoftwareUpdates";
                        powershell.AddScript($"if (-not (Test-Path '{remoteStagingDir}')) {{ New-Item -Path '{remoteStagingDir}' -ItemType Directory -Force }}");
                        await Task.Run(() => powershell.Invoke(), cancellationToken);
                        powershell.Commands.Clear();
                        
                        // Copy update file to remote machine if specified
                        if (!string.IsNullOrEmpty(update.UpdateStagingPath) && File.Exists(update.UpdateStagingPath))
                        {
                            string fileName = Path.GetFileName(update.UpdateStagingPath);
                            string remoteFilePath = Path.Combine(remoteStagingDir, fileName).Replace('\\', '/');
                            
                            // Read the file content
                            byte[] fileContent = await File.ReadAllBytesAsync(update.UpdateStagingPath, cancellationToken);
                            
                            // Convert to base64 for transfer
                            string base64Content = Convert.ToBase64String(fileContent);
                            
                            // Create script to decode and write the file
                            powershell.AddScript($@"
                                $fileContent = [Convert]::FromBase64String('{base64Content}')
                                [System.IO.File]::WriteAllBytes('{remoteFilePath}', $fileContent)
                            ");
                            
                            await Task.Run(() => powershell.Invoke(), cancellationToken);
                            powershell.Commands.Clear();
                            
                            // Find product code if needed for uninstall
                            if (update.ForceReinstall && !string.IsNullOrEmpty(update.UninstallCommand))
                            {
                                powershell.AddScript($@"
                                    # Get uninstall string from registry
                                    $uninstallKeys = @(
                                        'HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\*',
                                        'HKLM:\Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\*'
                                    )
                                    
                                    $app = Get-ItemProperty $uninstallKeys | 
                                        Where-Object {{ $_.DisplayName -eq '{update.SoftwareName}' }} | 
                                        Select-Object -First 1 -Property UninstallString, PSChildName
                                    
                                    $app.PSChildName
                                ");
                                
                                var result = await Task.Run(() => powershell.Invoke(), cancellationToken);
                                powershell.Commands.Clear();
                                
                                string productCode = string.Empty;
                                if (result.Count > 0)
                                {
                                    productCode = result[0].ToString();
                                }
                                
                                // Uninstall existing software
                                if (!string.IsNullOrEmpty(productCode))
                                {
                                    string uninstallCommand = update.UninstallCommand.Replace("{PRODUCTCODE}", productCode);
                                    
                                    powershell.AddScript($@"
                                        # Uninstall the software
                                        Start-Process -FilePath 'cmd.exe' -ArgumentList '/c {uninstallCommand}' -Wait -NoNewWindow
                                    ");
                                    
                                    await Task.Run(() => powershell.Invoke(), cancellationToken);
                                    powershell.Commands.Clear();
                                }
                            }
                            
                            // Install the software
                            string installCommand = update.InstallCommand.Replace("{FILEPATH}", remoteFilePath);
                            
                            powershell.AddScript($@"
                                # Install the software
                                Start-Process -FilePath 'cmd.exe' -ArgumentList '/c {installCommand}' -Wait -NoNewWindow
                                $lastExitCode
                            ");
                            
                            var installResult = await Task.Run(() => powershell.Invoke(), cancellationToken);
                            powershell.Commands.Clear();
                            
                            // Check exit code
                            int exitCode = 0;
                            if (installResult.Count > 0 && int.TryParse(installResult[0].ToString(), out int parsedCode))
                            {
                                exitCode = parsedCode;
                            }
                            
                            // Clean up
                            powershell.AddScript($@"
                                # Clean up
                                Remove-Item -Path '{remoteFilePath}' -Force
                            ");
                            
                            await Task.Run(() => powershell.Invoke(), cancellationToken);
                            
                            // Update status based on exit code
                            if (exitCode == 0 || exitCode == 3010) // 3010 means reboot required
                            {
                                // Update successful
                                software.InstalledVersion = update.TargetVersion;
                                software.UpdateAvailable = false;
                                machine.Status = $"Updated {update.SoftwareName} to {update.TargetVersion}";
                                machine.UpdatePendingUpdates();
                                
                                // Save machine data
                                await SaveMachineDataAsync(machine);
                                
                                return true;
                            }
                            else
                            {
                                // Update failed
                                machine.Status = $"Update failed with exit code {exitCode}";
                                return false;
                            }
                        }
                        else
                        {
                            // No update file specified
                            machine.Status = "No update file specified";
                            return false;
                        }
                    }
                }
                finally
                {
                    // Close the runspace
                    runspace.Close();
                }
            }
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
        /// Saves machine data asynchronously.
        /// </summary>
        private async Task SaveMachineDataAsync(Machine machine)
        {
            try
            {
                // Get all machines
                string filePath = Path.Combine(_storagePath, "machines.json");
                
                if (File.Exists(filePath))
                {
                    string json = await File.ReadAllTextAsync(filePath);
                    var machines = JsonConvert.DeserializeObject<List<Machine>>(json) ?? new List<Machine>();
                    
                    // Update the machine
                    var existingMachine = machines.FirstOrDefault(m => m.IPAddress == machine.IPAddress);
                    if (existingMachine != null)
                    {
                        // Update the existing machine
                        int index = machines.IndexOf(existingMachine);
                        machines[index] = machine;
                    }
                    else
                    {
                        // Add the new machine
                        machines.Add(machine);
                    }
                    
                    // Save the machines
                    string updatedJson = JsonConvert.SerializeObject(machines, Formatting.Indented);
                    await File.WriteAllTextAsync(filePath, updatedJson);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving machine data: {ex.Message}");
            }
        }

        /// <summary>
        /// Cancels an ongoing update deployment.
        /// </summary>
        public void CancelUpdate()
        {
            _cancellationTokenSource?.Cancel();
        }

        /// <summary>
        /// Raises the UpdateProgress event.
        /// </summary>
        protected virtual void OnUpdateProgress(double progress)
        {
            UpdateProgress?.Invoke(this, progress);
        }
    }
}
