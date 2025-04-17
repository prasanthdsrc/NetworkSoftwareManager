using System;
using System.IO;
using System.Threading.Tasks;
using NetworkSoftwareManager.Models;
using NetworkSoftwareManager.Services;
using Newtonsoft.Json;

namespace NetworkSoftwareManager
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Network Software Manager - TimeSpan Fix Demo");
            Console.WriteLine("===========================================");
            
            // Create data directory if it doesn't exist
            string dataPath = Path.Combine(Directory.GetCurrentDirectory(), "Data");
            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }
            
            try
            {
                // Initialize the settings service
                var settingsService = new SettingsService(dataPath);
                
                // Create and run the ConnectionOptions demo
                var connectionDemo = new ConnectionOptionsDemo(settingsService);
                
                // Print current settings (using TimeSpan properties)
                connectionDemo.ShowSettings();
                
                // Demonstrate before the fix (the problem)
                connectionDemo.DemonstrateBefore();
                
                // Demonstrate after the fix (the solution)
                connectionDemo.DemonstrateAfter();
                
                Console.WriteLine("\nTIMESPAN FIX SUMMARY");
                Console.WriteLine("==================");
                Console.WriteLine("The core issue was that System.Management.ConnectionOptions.Timeout property");
                Console.WriteLine("expects a TimeSpan object, but our code was trying to convert TimeSpan to int.");
                Console.WriteLine("\nWe fixed this by:");
                Console.WriteLine("1. Keeping AppSettings.ConnectionTimeout as TimeSpan (not as int milliseconds)");
                Console.WriteLine("2. Using ConnectionOptions.Timeout = settingsService.CurrentSettings.ConnectionTimeout");
                Console.WriteLine("3. Not converting the TimeSpan to milliseconds");
                
                Console.WriteLine("\nFILE CHANGES:");
                Console.WriteLine("1. NetworkSoftwareManager/Services/SoftwareInventoryService.cs");
                Console.WriteLine("2. NetworkSoftwareManager/Services/CredentialService.cs");
                Console.WriteLine("3. NetworkSoftwareManager/Services/SoftwareUpdateService.cs");
                
                Console.WriteLine("\nOLD CODE:");
                Console.WriteLine("connectionOptions.Timeout = (int)_settingsService.CurrentSettings.ConnectionTimeout.TotalMilliseconds;");
                
                Console.WriteLine("\nNEW CODE:");
                Console.WriteLine("connectionOptions.Timeout = _settingsService.CurrentSettings.ConnectionTimeout;");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Error: {ex.InnerException.Message}");
                }
            }
        }
    }
}