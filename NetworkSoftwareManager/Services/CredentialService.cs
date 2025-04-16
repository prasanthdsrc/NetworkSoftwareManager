using System;
using System.DirectoryServices;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using NetworkSoftwareManager.Models;
using NetworkSoftwareManager.Utils;

namespace NetworkSoftwareManager.Services
{
    /// <summary>
    /// Service for managing and validating credentials.
    /// </summary>
    public class CredentialService
    {
        private readonly SettingsService _settingsService;

        public CredentialService()
        {
            _settingsService = new SettingsService();
        }

        /// <summary>
        /// Tests if the provided credentials are valid.
        /// </summary>
        public async Task<bool> TestCredentialsAsync(Credentials credentials)
        {
            // Validate parameters
            if (credentials == null || string.IsNullOrEmpty(credentials.Username))
            {
                return false;
            }

            try
            {
                // Try to find a suitable online machine to test against
                string targetMachine = await FindOnlineMachineAsync();
                if (string.IsNullOrEmpty(targetMachine))
                {
                    // Try localhost as fallback
                    targetMachine = Environment.MachineName;
                }

                // First try Active Directory authentication
                if (!string.IsNullOrEmpty(credentials.Domain))
                {
                    bool adResult = await TestActiveDirectoryCredentialsAsync(credentials);
                    if (adResult)
                    {
                        return true;
                    }
                }

                // Try PowerShell remoting authentication
                return await TestPowerShellCredentialsAsync(credentials, targetMachine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error testing credentials: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Tests Active Directory credentials.
        /// </summary>
        private async Task<bool> TestActiveDirectoryCredentialsAsync(Credentials credentials)
        {
            return await Task.Run(() =>
            {
                try
                {
                    string domainPath = $"LDAP://{credentials.Domain}";
                    using (DirectoryEntry entry = new DirectoryEntry(
                        domainPath,
                        credentials.Username,
                        credentials.Password))
                    {
                        // Trigger authentication
                        object nativeObject = entry.NativeObject;
                        return true;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }

        /// <summary>
        /// Tests credentials using PowerShell remoting.
        /// </summary>
        private async Task<bool> TestPowerShellCredentialsAsync(Credentials credentials, string targetMachine)
        {
            return await Task.Run(() =>
            {
                try
                {
                    // Create credentials for PowerShell
                    var psCredential = new PSCredential(
                        credentials.GetFormattedCredentials(),
                        PasswordHelper.ConvertToSecureString(credentials.Password));

                    // Create a connection info for the remote machine
                    var connectionInfo = new WSManConnectionInfo(
                        new Uri($"http://{targetMachine}:5985/wsman"),
                        "http://schemas.microsoft.com/powershell/Microsoft.PowerShell",
                        psCredential);

                    connectionInfo.AuthenticationMechanism = AuthenticationMechanism.Default;
                    connectionInfo.OperationTimeout = TimeSpan.FromSeconds(10);

                    // Try to create a runspace with these credentials
                    using (var runspace = RunspaceFactory.CreateRunspace(connectionInfo))
                    {
                        runspace.Open();
                        using (var powershell = PowerShell.Create())
                        {
                            powershell.Runspace = runspace;
                            powershell.AddScript("$env:COMPUTERNAME");
                            var result = powershell.Invoke();
                            return result.Count > 0;
                        }
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }

        /// <summary>
        /// Finds an online machine to test credentials against.
        /// </summary>
        private async Task<string> FindOnlineMachineAsync()
        {
            try
            {
                // Try to use machines from settings if available
                foreach (var machineIP in _settingsService.CurrentSettings.SavedMachines)
                {
                    using (var ping = new Ping())
                    {
                        var reply = await ping.SendPingAsync(machineIP, 500);
                        if (reply.Status == IPStatus.Success)
                        {
                            return machineIP;
                        }
                    }
                }

                // Try to ping the default gateway
                string gatewayIP = await GetDefaultGatewayAsync();
                if (!string.IsNullOrEmpty(gatewayIP))
                {
                    using (var ping = new Ping())
                    {
                        var reply = await ping.SendPingAsync(gatewayIP, 500);
                        if (reply.Status == IPStatus.Success)
                        {
                            return gatewayIP;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error finding online machine: {ex.Message}");
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the IP address of the default gateway.
        /// </summary>
        private async Task<string> GetDefaultGatewayAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    // Get all network interfaces
                    NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
                    foreach (var ni in interfaces)
                    {
                        // Check if the interface is up and not a loopback
                        if (ni.OperationalStatus == OperationalStatus.Up &&
                            ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                        {
                            // Get IP properties
                            var ipProps = ni.GetIPProperties();
                            if (ipProps.GatewayAddresses.Count > 0)
                            {
                                // Return the first gateway address
                                return ipProps.GatewayAddresses[0].Address.ToString();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error getting default gateway: {ex.Message}");
                }

                return string.Empty;
            });
        }

        /// <summary>
        /// Secures the credentials by encrypting sensitive information.
        /// </summary>
        public Credentials SecureCredentials(Credentials credentials)
        {
            if (credentials == null)
            {
                return new Credentials();
            }

            return new Credentials
            {
                Domain = credentials.Domain,
                Username = credentials.Username,
                Password = PasswordHelper.EncryptPassword(credentials.Password)
            };
        }

        /// <summary>
        /// Decrypts secured credentials for use.
        /// </summary>
        public Credentials DecryptCredentials(Credentials securedCredentials)
        {
            if (securedCredentials == null)
            {
                return new Credentials();
            }

            return new Credentials
            {
                Domain = securedCredentials.Domain,
                Username = securedCredentials.Username,
                Password = PasswordHelper.DecryptPassword(securedCredentials.Password)
            };
        }
    }
}
