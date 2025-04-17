using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using NetworkSoftwareManager.Models;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;

namespace NetworkSoftwareManager.Services
{
    /// <summary>
    /// Service for scanning and managing software inventory on remote machines.
    /// </summary>
    public class SoftwareInventoryService
    {
        private readonly string _storagePath;
        private readonly SettingsService _settingsService;
        private CancellationTokenSource? _cancellationTokenSource;

        public event EventHandler<double>? InventoryProgress;

        public SoftwareInventoryService()
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
        /// Gets software inventory for a specific machine.
        /// </summary>
        public async Task<List<Software>> GetSoftwareInventoryAsync(Machine machine, Credentials credentials)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;
            
            try
            {
                // Check if machine is online
                if (!machine.IsOnline)
                {
                    machine.Status = "Offline";
                    return new List<Software>();
                }

                machine.Status = "Scanning software...";
                OnInventoryProgress(10);

                List<Software> softwareList = new List<Software>();
                bool usePowerShell = _settingsService.CurrentSettings.UsePowerShell;
                bool useWMI = _settingsService.CurrentSettings.UseWMI;

                // Try PowerShell method first if enabled
                if (usePowerShell)
                {
                    try
                    {
                        softwareList = await GetSoftwareInventoryViaPowerShellAsync(machine, credentials, cancellationToken);
                        if (softwareList.Count > 0)
                        {
                            machine.Status = "Software scan completed via PowerShell";
                            OnInventoryProgress(100);
                            return softwareList;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the error but continue with WMI method
                        machine.Status = $"PowerShell scan failed: {ex.Message}";
                    }
                }

                // Fallback to WMI method if PowerShell failed or is disabled
                if (useWMI && softwareList.Count == 0)
                {
                    try
                    {
                        softwareList = await GetSoftwareInventoryViaWMIAsync(machine, credentials, cancellationToken);
                        if (softwareList.Count > 0)
                        {
                            machine.Status = "Software scan completed via WMI";
                        }
                        else
                        {
                            machine.Status = "No software found";
                        }
                    }
                    catch (Exception ex)
                    {
                        machine.Status = $"WMI scan failed: {ex.Message}";
                    }
                }

                // Update software versions and update status
                await CheckForUpdatesAsync(softwareList);
                
                OnInventoryProgress(100);
                return softwareList;
            }
            catch (Exception ex)
            {
                machine.Status = $"Error scanning software: {ex.Message}";
                return new List<Software>();
            }
            finally
            {
                _cancellationTokenSource = null;
            }
        }

        /// <summary>
        /// Gets software inventory using PowerShell remoting.
        /// </summary>
        private async Task<List<Software>> GetSoftwareInventoryViaPowerShellAsync(
            Machine machine, 
            Credentials credentials, 
            CancellationToken cancellationToken)
        {
            List<Software> result = new List<Software>();
            
            // Create a PowerShell runspace
            var connectionInfo = new WSManConnectionInfo(
                new Uri($"http://{machine.IPAddress}:5985/wsman"),
                "http://schemas.microsoft.com/powershell/Microsoft.PowerShell",
                credentials.GetFormattedCredentials());
            
            connectionInfo.AuthenticationMechanism = AuthenticationMechanism.Default;
            
            // Use connection timeout from settings
            // WSManConnectionInfo.OperationTimeout requires milliseconds as int
            connectionInfo.OperationTimeout = (int)_settingsService.CurrentSettings.ConnectionTimeout.TotalMilliseconds;
            
            using (var runspace = RunspaceFactory.CreateRunspace(connectionInfo))
            {
                try
                {
                    // Open the runspace
                    runspace.Open();
                    
                    // Create PowerShell pipeline
                    using (var powershell = PowerShell.Create())
                    {
                        powershell.Runspace = runspace;
                        
                        // Add PowerShell command to get installed software
                        powershell.AddScript(@"
                            # Get software from registry 32-bit
                            $software32 = Get-ItemProperty HKLM:\Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\* |
                                Where-Object { $_.DisplayName -ne $null } |
                                Select-Object DisplayName, DisplayVersion, Publisher, InstallDate, InstallLocation, UninstallString

                            # Get software from registry 64-bit
                            $software64 = Get-ItemProperty HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\* |
                                Where-Object { $_.DisplayName -ne $null } |
                                Select-Object DisplayName, DisplayVersion, Publisher, InstallDate, InstallLocation, UninstallString

                            # Combine results
                            $software = $software32 + $software64

                            # Return as JSON
                            $software | Select-Object DisplayName, DisplayVersion, Publisher, InstallDate, InstallLocation, UninstallString
                        ");
                        
                        // Execute the script
                        OnInventoryProgress(30);
                        var psResults = await Task.Run(() => powershell.Invoke(), cancellationToken);
                        OnInventoryProgress(70);
                        
                        // Convert results to Software objects
                        foreach (PSObject psObj in psResults)
                        {
                            var software = new Software
                            {
                                Name = SafeGetPropertyValue(psObj, "DisplayName"),
                                InstalledVersion = SafeGetPropertyValue(psObj, "DisplayVersion"),
                                Publisher = SafeGetPropertyValue(psObj, "Publisher"),
                                InstallLocation = SafeGetPropertyValue(psObj, "InstallLocation")
                            };
                            
                            // Parse install date if available
                            string installDateStr = SafeGetPropertyValue(psObj, "InstallDate");
                            if (!string.IsNullOrEmpty(installDateStr) && installDateStr.Length == 8)
                            {
                                try
                                {
                                    // Parse yyyyMMdd format
                                    int year = int.Parse(installDateStr.Substring(0, 4));
                                    int month = int.Parse(installDateStr.Substring(4, 2));
                                    int day = int.Parse(installDateStr.Substring(6, 2));
                                    software.InstallDate = new DateTime(year, month, day);
                                }
                                catch
                                {
                                    // Use default date if parsing fails
                                    software.InstallDate = DateTime.MinValue;
                                }
                            }
                            
                            result.Add(software);
                        }
                    }
                }
                finally
                {
                    // Close the runspace
                    runspace.Close();
                }
            }
            
            return result;
        }

        /// <summary>
        /// Gets software inventory using WMI.
        /// </summary>
        private async Task<List<Software>> GetSoftwareInventoryViaWMIAsync(
            Machine machine, 
            Credentials credentials, 
            CancellationToken cancellationToken)
        {
            List<Software> result = new List<Software>();
            
            try
            {
                // Create connection options
                var connectionOptions = new ConnectionOptions();
                
                // Use connection timeout from settings converted to milliseconds
                // ConnectionOptions.Timeout takes an integer milliseconds value
                // Note: TimeSpan needs to be converted to milliseconds for ConnectionOptions.Timeout
                connectionOptions.Timeout = (int)_settingsService.CurrentSettings.ConnectionTimeout.TotalMilliseconds;
                
                // Set credentials if available
                if (!string.IsNullOrEmpty(credentials.Username))
                {
                    connectionOptions.Username = credentials.GetFormattedCredentials();
                    connectionOptions.Password = credentials.Password;
                }
                
                // Connect to the remote machine
                ManagementScope scope = new ManagementScope(
                    $"\\\\{machine.IPAddress}\\root\\cimv2",
                    connectionOptions);
                
                scope.Connect();
                OnInventoryProgress(40);
                
                // Query Win32_Product for installed software
                ObjectQuery query = new ObjectQuery(
                    "SELECT Name, Version, Vendor, InstallDate FROM Win32_Product");
                
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
                
                // Execute the query
                var wmicResults = await Task.Run(() => searcher.Get(), cancellationToken);
                OnInventoryProgress(80);
                
                // Convert results to Software objects
                foreach (ManagementObject obj in wmicResults)
                {
                    var software = new Software
                    {
                        Name = obj["Name"]?.ToString() ?? "Unknown",
                        InstalledVersion = obj["Version"]?.ToString() ?? "",
                        Publisher = obj["Vendor"]?.ToString() ?? ""
                    };
                    
                    // Parse install date if available
                    if (obj["InstallDate"] != null)
                    {
                        string installDateStr = obj["InstallDate"].ToString();
                        if (!string.IsNullOrEmpty(installDateStr) && installDateStr.Length >= 8)
                        {
                            try
                            {
                                // WMI dates are in format: yyyyMMdd
                                int year = int.Parse(installDateStr.Substring(0, 4));
                                int month = int.Parse(installDateStr.Substring(4, 2));
                                int day = int.Parse(installDateStr.Substring(6, 2));
                                software.InstallDate = new DateTime(year, month, day);
                            }
                            catch
                            {
                                // Use default date if parsing fails
                                software.InstallDate = DateTime.MinValue;
                            }
                        }
                    }
                    
                    result.Add(software);
                }
                
                // Try additional registry locations if needed
                if (result.Count == 0)
                {
                    // Query for 32-bit software
                    result.AddRange(await GetSoftwareFromRegistryAsync(
                        machine, 
                        credentials, 
                        @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall", 
                        scope, 
                        cancellationToken));
                    
                    // Query for 64-bit software
                    result.AddRange(await GetSoftwareFromRegistryAsync(
                        machine, 
                        credentials, 
                        @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", 
                        scope, 
                        cancellationToken));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error scanning software via WMI: {ex.Message}", ex);
            }
            
            return result;
        }

        /// <summary>
        /// Gets software information from the Windows registry.
        /// </summary>
        private async Task<List<Software>> GetSoftwareFromRegistryAsync(
            Machine machine,
            Credentials credentials,
            string registryPath,
            ManagementScope scope,
            CancellationToken cancellationToken)
        {
            List<Software> result = new List<Software>();
            
            try
            {
                // Query StdRegProv for registry access
                var regPath = new ManagementPath("StdRegProv");
                var classInstance = new ManagementClass(scope, regPath, null);
                
                // Get the list of uninstall keys
                var inParams = classInstance.GetMethodParameters("EnumKey");
                inParams["hDefKey"] = 0x80000002; // HKLM
                inParams["sSubKeyName"] = registryPath;
                
                var outParams = await Task.Run(() => classInstance.InvokeMethod("EnumKey", inParams, null), cancellationToken);
                
                if (outParams["ReturnValue"].ToString() == "0" && outParams["sNames"] != null)
                {
                    string[] subKeys = (string[])outParams["sNames"];
                    
                    // Process each subkey to get software details
                    foreach (string subKey in subKeys)
                    {
                        var displayName = GetRegistryValue(classInstance, 0x80000002, $"{registryPath}\\{subKey}", "DisplayName");
                        
                        if (!string.IsNullOrEmpty(displayName))
                        {
                            var software = new Software
                            {
                                Name = displayName,
                                InstalledVersion = GetRegistryValue(classInstance, 0x80000002, $"{registryPath}\\{subKey}", "DisplayVersion"),
                                Publisher = GetRegistryValue(classInstance, 0x80000002, $"{registryPath}\\{subKey}", "Publisher"),
                                InstallLocation = GetRegistryValue(classInstance, 0x80000002, $"{registryPath}\\{subKey}", "InstallLocation")
                            };
                            
                            // Parse install date if available
                            string installDateStr = GetRegistryValue(classInstance, 0x80000002, $"{registryPath}\\{subKey}", "InstallDate");
                            if (!string.IsNullOrEmpty(installDateStr) && installDateStr.Length == 8)
                            {
                                try
                                {
                                    // Parse yyyyMMdd format
                                    int year = int.Parse(installDateStr.Substring(0, 4));
                                    int month = int.Parse(installDateStr.Substring(4, 2));
                                    int day = int.Parse(installDateStr.Substring(6, 2));
                                    software.InstallDate = new DateTime(year, month, day);
                                }
                                catch
                                {
                                    // Use default date if parsing fails
                                    software.InstallDate = DateTime.MinValue;
                                }
                            }
                            
                            result.Add(software);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving software from registry: {ex.Message}");
            }
            
            return result;
        }

        /// <summary>
        /// Gets the value of a registry key.
        /// </summary>
        private string GetRegistryValue(ManagementClass regProvider, uint hive, string key, string valueName)
        {
            try
            {
                var inParams = regProvider.GetMethodParameters("GetStringValue");
                inParams["hDefKey"] = hive;
                inParams["sSubKeyName"] = key;
                inParams["sValueName"] = valueName;
                
                var outParams = regProvider.InvokeMethod("GetStringValue", inParams, null);
                
                if (outParams["ReturnValue"].ToString() == "0" && outParams["sValue"] != null)
                {
                    return outParams["sValue"].ToString();
                }
            }
            catch
            {
                // Ignore errors reading registry
            }
            
            return string.Empty;
        }

        /// <summary>
        /// Safely gets a property value from a PowerShell object.
        /// </summary>
        private string SafeGetPropertyValue(PSObject obj, string propertyName)
        {
            if (obj == null) return string.Empty;
            
            var property = obj.Properties[propertyName];
            return property?.Value?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Checks for updates for a list of software.
        /// </summary>
        private async Task CheckForUpdatesAsync(List<Software> softwareList)
        {
            // Load software versions from database
            var softwareVersions = await LoadSoftwareVersionsAsync();
            
            foreach (var software in softwareList)
            {
                // Try to find this software in the version database
                var versionInfo = softwareVersions.FirstOrDefault(v => 
                    string.Equals(v.Name, software.Name, StringComparison.OrdinalIgnoreCase) && 
                    string.Equals(v.Publisher, software.Publisher, StringComparison.OrdinalIgnoreCase));
                
                if (versionInfo != null)
                {
                    // Set the latest version
                    software.LatestVersion = versionInfo.LatestVersion;
                    software.UseLatestVersion = versionInfo.UseLatestVersion;
                    software.TargetVersion = versionInfo.TargetVersion;
                    
                    // Update will recalculate update availability
                    software.UpdateAvailable = CompareVersions(software.InstalledVersion, software.EffectiveTargetVersion) < 0;
                }
                else
                {
                    // No version info found, initialize with defaults
                    software.LatestVersion = software.InstalledVersion;
                    software.UseLatestVersion = true;
                    software.UpdateAvailable = false;
                }
            }
        }

        /// <summary>
        /// Compares two version strings.
        /// </summary>
        /// <returns>
        /// -1 if v1 is less than v2
        /// 0 if v1 equals v2
        /// 1 if v1 is greater than v2
        /// </returns>
        private int CompareVersions(string v1, string v2)
        {
            if (string.IsNullOrEmpty(v1) && string.IsNullOrEmpty(v2))
                return 0;
            
            if (string.IsNullOrEmpty(v1))
                return -1;
            
            if (string.IsNullOrEmpty(v2))
                return 1;
            
            try
            {
                // Try parsing as Version objects
                if (Version.TryParse(v1, out Version? version1) && 
                    Version.TryParse(v2, out Version? version2))
                {
                    return version1.CompareTo(version2);
                }
                
                // Fallback to string comparison
                return string.Compare(v1, v2, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                // Default to string comparison
                return string.Compare(v1, v2, StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// Loads software version information from database.
        /// </summary>
        private async Task<List<Software>> LoadSoftwareVersionsAsync()
        {
            try
            {
                string filePath = Path.Combine(_storagePath, "software_versions.json");
                
                if (File.Exists(filePath))
                {
                    string json = await File.ReadAllTextAsync(filePath);
                    return JsonConvert.DeserializeObject<List<Software>>(json) ?? new List<Software>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading software versions: {ex.Message}");
            }
            
            return new List<Software>();
        }

        /// <summary>
        /// Gets all software inventory across all machines.
        /// </summary>
        public async Task<List<Software>> GetAllSoftwareInventoryAsync()
        {
            try
            {
                // Load machines
                var machines = await LoadMachinesAsync();
                
                // Combine all software from all machines
                var allSoftware = new List<Software>();
                
                foreach (var machine in machines)
                {
                    allSoftware.AddRange(machine.InstalledSoftware);
                }
                
                return allSoftware;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting all software inventory: {ex.Message}");
                return new List<Software>();
            }
        }

        /// <summary>
        /// Saves software version information to database.
        /// </summary>
        public void SaveSoftwareInventory(List<Machine> machines)
        {
            try
            {
                // Save machine data
                string machinesJson = JsonConvert.SerializeObject(machines, Formatting.Indented);
                File.WriteAllText(Path.Combine(_storagePath, "machines.json"), machinesJson);
                
                // Extract unique software and save version info
                var uniqueSoftware = machines
                    .SelectMany(m => m.InstalledSoftware)
                    .GroupBy(s => new { s.Name, s.Publisher })
                    .Select(g => new Software
                    {
                        Name = g.First().Name,
                        Publisher = g.First().Publisher,
                        InstalledVersion = g.First().InstalledVersion,
                        LatestVersion = g.First().LatestVersion,
                        UseLatestVersion = g.First().UseLatestVersion,
                        TargetVersion = g.First().TargetVersion,
                        InstallCount = g.Count()
                    })
                    .ToList();
                
                string versionsJson = JsonConvert.SerializeObject(uniqueSoftware, Formatting.Indented);
                File.WriteAllText(Path.Combine(_storagePath, "software_versions.json"), versionsJson);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving software inventory: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Loads machines from storage.
        /// </summary>
        private async Task<List<Machine>> LoadMachinesAsync()
        {
            try
            {
                string filePath = Path.Combine(_storagePath, "machines.json");
                
                if (File.Exists(filePath))
                {
                    string json = await File.ReadAllTextAsync(filePath);
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
        /// Cancels an ongoing inventory scan.
        /// </summary>
        public void CancelInventory()
        {
            _cancellationTokenSource?.Cancel();
        }

        /// <summary>
        /// Raises the InventoryProgress event.
        /// </summary>
        protected virtual void OnInventoryProgress(double progress)
        {
            InventoryProgress?.Invoke(this, progress);
        }
    }
}
