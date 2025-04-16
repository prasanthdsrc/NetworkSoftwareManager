using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Management;
using Newtonsoft.Json;
using NetworkSoftwareManager.Models;
using NetworkSoftwareManager.Utils;
using System.Text;

namespace NetworkSoftwareManager.Services
{
    /// <summary>
    /// Service for scanning the network and managing machine information.
    /// </summary>
    public class NetworkScanService
    {
        private readonly string _storagePath;
        private readonly SettingsService _settingsService;
        private CancellationTokenSource? _cancellationTokenSource;

        public event EventHandler<double>? ScanProgress;

        public NetworkScanService()
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
        /// Scans the network for machines within the specified IP ranges.
        /// </summary>
        public async Task<List<Machine>> ScanNetworkAsync(
            List<IPRange> ipRanges, 
            List<string> excludedIPs, 
            TimeSpan timeout,
            int threadCount)
        {
            // Create a new cancellation token source
            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;
            
            // Create a list of all IP addresses to scan
            var allIPs = new List<string>();
            foreach (var range in ipRanges)
            {
                allIPs.AddRange(range.GetAllIPsInRange());
            }
            
            // Remove excluded IPs
            allIPs = allIPs.Except(excludedIPs).ToList();
            
            // Create a list for results
            var results = new List<Machine>();
            var machineCount = allIPs.Count;
            var processedCount = 0;
            
            try
            {
                // Create a semaphore to limit concurrent operations
                using (var semaphore = new SemaphoreSlim(threadCount))
                {
                    // Create a list to hold all the tasks
                    var tasks = new List<Task>();
                    
                    // Process each IP address
                    foreach (var ip in allIPs)
                    {
                        // Check for cancellation
                        if (cancellationToken.IsCancellationRequested)
                        {
                            break;
                        }
                        
                        // Wait for a slot in the semaphore
                        await semaphore.WaitAsync(cancellationToken);
                        
                        // Start a new task for this IP
                        var task = Task.Run(async () =>
                        {
                            try
                            {
                                var machine = await ScanSingleMachineAsync(ip, timeout, cancellationToken);
                                if (machine != null)
                                {
                                    lock (results)
                                    {
                                        results.Add(machine);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                // Log the error
                                Console.WriteLine($"Error scanning {ip}: {ex.Message}");
                            }
                            finally
                            {
                                // Increment processed count and report progress
                                Interlocked.Increment(ref processedCount);
                                double progress = (double)processedCount / machineCount * 100;
                                OnScanProgress(progress);
                                
                                // Release the semaphore
                                semaphore.Release();
                            }
                        }, cancellationToken);
                        
                        tasks.Add(task);
                    }
                    
                    // Wait for all tasks to complete
                    await Task.WhenAll(tasks);
                }
            }
            catch (OperationCanceledException)
            {
                // Scan was cancelled
            }
            finally
            {
                _cancellationTokenSource = null;
            }
            
            // Return the results
            return results;
        }

        /// <summary>
        /// Scans a single machine for basic information.
        /// </summary>
        private async Task<Machine?> ScanSingleMachineAsync(string ipAddress, TimeSpan timeout, CancellationToken cancellationToken)
        {
            // Create a ping object
            using (var ping = new Ping())
            {
                try
                {
                    // Ping the machine
                    var reply = await ping.SendPingAsync(ipAddress, (int)timeout.TotalMilliseconds);
                    
                    // If the ping was successful, create a machine object
                    if (reply.Status == IPStatus.Success)
                    {
                        var machine = new Machine
                        {
                            IPAddress = ipAddress,
                            IsOnline = true,
                            LastScanned = DateTime.Now
                        };
                        
                        // Try to get the hostname
                        try
                        {
                            var hostEntry = await Dns.GetHostEntryAsync(ipAddress);
                            machine.Hostname = hostEntry.HostName;
                        }
                        catch
                        {
                            // Couldn't resolve hostname
                            machine.Hostname = ipAddress;
                        }
                        
                        // Try to get more information via WMI if enabled
                        if (_settingsService.CurrentSettings.UseWMI)
                        {
                            try
                            {
                                // Get OS information
                                var connectionOptions = new ConnectionOptions
                                {
                                    Timeout = TimeSpan.FromMilliseconds(_settingsService.CurrentSettings.ConnectionTimeout)
                                };
                                
                                // Set credentials if available
                                if (!string.IsNullOrEmpty(_settingsService.CurrentSettings.GlobalCredentials.Username))
                                {
                                    connectionOptions.Username = _settingsService.CurrentSettings.GlobalCredentials.GetFormattedCredentials();
                                    connectionOptions.Password = _settingsService.CurrentSettings.GlobalCredentials.Password;
                                }
                                
                                // Connect to the machine
                                var scope = new ManagementScope($"\\\\{ipAddress}\\root\\cimv2", connectionOptions);
                                scope.Connect();
                                
                                // Query for OS information
                                var query = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
                                var searcher = new ManagementObjectSearcher(scope, query);
                                
                                foreach (ManagementObject os in searcher.Get())
                                {
                                    machine.OperatingSystem = os["Caption"].ToString() ?? "Unknown";
                                    break;
                                }
                            }
                            catch (Exception ex)
                            {
                                // WMI query failed
                                machine.Status = $"Error: {ex.Message}";
                            }
                        }
                        
                        return machine;
                    }
                }
                catch (Exception)
                {
                    // Ping failed
                }
            }
            
            return null;
        }

        /// <summary>
        /// Cancels an ongoing network scan.
        /// </summary>
        public void CancelScan()
        {
            _cancellationTokenSource?.Cancel();
        }

        /// <summary>
        /// Saves a list of machines to storage.
        /// </summary>
        public void SaveMachines(List<Machine> machines)
        {
            string filePath = Path.Combine(_storagePath, "machines.json");
            
            try
            {
                // Get existing machines
                var existingMachines = GetSavedMachines();
                
                // Merge the lists
                foreach (var machine in machines)
                {
                    // Try to find an existing machine with the same IP
                    var existingMachine = existingMachines.Find(m => m.IPAddress == machine.IPAddress);
                    if (existingMachine != null)
                    {
                        // Update the existing machine
                        existingMachine.Hostname = machine.Hostname;
                        existingMachine.OperatingSystem = machine.OperatingSystem;
                        existingMachine.IsOnline = machine.IsOnline;
                        existingMachine.LastScanned = machine.LastScanned;
                        existingMachine.Domain = machine.Domain;
                        existingMachine.Username = machine.Username;
                        existingMachine.Password = machine.Password;
                        existingMachine.Status = machine.Status;
                        
                        // Keep existing software list if the new machine doesn't have any
                        if (machine.InstalledSoftware.Count > 0)
                        {
                            existingMachine.InstalledSoftware = machine.InstalledSoftware;
                        }
                    }
                    else
                    {
                        // Add the new machine
                        existingMachines.Add(machine);
                    }
                }
                
                // Save the merged list
                string json = JsonConvert.SerializeObject(existingMachines, Formatting.Indented);
                File.WriteAllText(filePath, json);
                
                // Update the settings with the saved machine IPs
                _settingsService.CurrentSettings.SavedMachines = existingMachines.Select(m => m.IPAddress).ToList();
                _settingsService.SaveSettings();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving machines: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Saves a list of machines to storage asynchronously.
        /// </summary>
        public async Task SaveMachinesAsync(List<Machine> machines)
        {
            await Task.Run(() => SaveMachines(machines));
        }

        /// <summary>
        /// Gets a list of saved machines from storage.
        /// </summary>
        public List<Machine> GetSavedMachines()
        {
            string filePath = Path.Combine(_storagePath, "machines.json");
            
            try
            {
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    return JsonConvert.DeserializeObject<List<Machine>>(json) ?? new List<Machine>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading machines: {ex.Message}");
            }
            
            return new List<Machine>();
        }

        /// <summary>
        /// Gets a list of saved machines from storage asynchronously.
        /// </summary>
        public async Task<List<Machine>> GetSavedMachinesAsync()
        {
            return await Task.Run(() => GetSavedMachines());
        }

        /// <summary>
        /// Imports machines from a file.
        /// </summary>
        public List<Machine> ImportMachinesFromFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string extension = Path.GetExtension(filePath).ToLower();
                    if (extension == ".json")
                    {
                        // Import from JSON
                        string json = File.ReadAllText(filePath);
                        return JsonConvert.DeserializeObject<List<Machine>>(json) ?? new List<Machine>();
                    }
                    else if (extension == ".csv")
                    {
                        // Import from CSV
                        var machines = new List<Machine>();
                        var lines = File.ReadAllLines(filePath);
                        
                        // Skip header line
                        for (int i = 1; i < lines.Length; i++)
                        {
                            var line = lines[i];
                            var parts = line.Split(',');
                            
                            if (parts.Length >= 2)
                            {
                                var machine = new Machine
                                {
                                    IPAddress = parts[0].Trim(),
                                    Hostname = parts[1].Trim()
                                };
                                
                                if (parts.Length >= 3)
                                {
                                    machine.OperatingSystem = parts[2].Trim();
                                }
                                
                                machines.Add(machine);
                            }
                        }
                        
                        return machines;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error importing machines: {ex.Message}", ex);
            }
            
            return new List<Machine>();
        }

        /// <summary>
        /// Exports machines to a file.
        /// </summary>
        public void ExportMachinesToFile(List<Machine> machines, string filePath)
        {
            try
            {
                string extension = Path.GetExtension(filePath).ToLower();
                if (extension == ".json")
                {
                    // Export to JSON
                    string json = JsonConvert.SerializeObject(machines, Formatting.Indented);
                    File.WriteAllText(filePath, json);
                }
                else if (extension == ".csv")
                {
                    // Export to CSV
                    var lines = new List<string>
                    {
                        "IPAddress,Hostname,OperatingSystem,IsOnline,LastScanned"
                    };
                    
                    foreach (var machine in machines)
                    {
                        lines.Add($"{machine.IPAddress},{machine.Hostname},{machine.OperatingSystem},{machine.IsOnline},{machine.LastScanned}");
                    }
                    
                    File.WriteAllLines(filePath, lines);
                }
                else
                {
                    // Default to JSON
                    string json = JsonConvert.SerializeObject(machines, Formatting.Indented);
                    File.WriteAllText(filePath, json);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error exporting machines: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Raises the ScanProgress event.
        /// </summary>
        protected virtual void OnScanProgress(double progress)
        {
            ScanProgress?.Invoke(this, progress);
        }
    }
}
