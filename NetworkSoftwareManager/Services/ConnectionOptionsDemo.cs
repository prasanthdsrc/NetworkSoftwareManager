using System;
using NetworkSoftwareManager.Models;

namespace NetworkSoftwareManager.Services
{
    public class ConnectionOptions
    {
        // This is a simplified version of System.Management.ConnectionOptions
        // In the real class, this property is of type TimeSpan
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(60);
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    
    public class ConnectionOptionsDemo
    {
        private readonly SettingsService _settingsService;
        
        public ConnectionOptionsDemo(SettingsService settingsService)
        {
            _settingsService = settingsService;
        }
        
        public void DemonstrateBefore()
        {
            Console.WriteLine("\nBEFORE THE FIX:");
            Console.WriteLine("==============");
            
            try
            {
                var connectionOptions = new ConnectionOptions();
                
                // WRONG APPROACH: This was the incorrect code
                // ConnectionOptions.Timeout takes a TimeSpan value but we were trying to set it as int milliseconds
                int timeoutMilliseconds = (int)_settingsService.CurrentSettings.ConnectionTimeout.TotalMilliseconds;
                // This line would fail in the real implementation with error:
                // "Cannot implicitly convert type 'int' to 'System.TimeSpan'"
                Console.WriteLine($"Trying to set Timeout = {timeoutMilliseconds} milliseconds (this would fail in real code)");
                
                Console.WriteLine("The compiler would report: Cannot implicitly convert type 'int' to 'System.TimeSpan'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        
        public void DemonstrateAfter()
        {
            Console.WriteLine("\nAFTER THE FIX:");
            Console.WriteLine("=============");
            
            try
            {
                var connectionOptions = new ConnectionOptions();
                
                // CORRECT APPROACH: Directly use the TimeSpan property
                connectionOptions.Timeout = _settingsService.CurrentSettings.ConnectionTimeout;
                
                Console.WriteLine($"Successfully set Timeout = {connectionOptions.Timeout.TotalSeconds} seconds");
                Console.WriteLine($"TimeSpan directly used: {connectionOptions.Timeout}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        
        public void ShowSettings()
        {
            Console.WriteLine("\nCurrent Settings:");
            Console.WriteLine("===============");
            Console.WriteLine($"ConnectionTimeout: {_settingsService.CurrentSettings.ConnectionTimeout}");
            Console.WriteLine($"DefaultScanInterval: {_settingsService.CurrentSettings.DefaultScanInterval}");
            Console.WriteLine($"UptimeCheckInterval: {_settingsService.CurrentSettings.UptimeCheckInterval}");
            Console.WriteLine($"RetryInterval: {_settingsService.CurrentSettings.RetryInterval}");
        }
    }
}