using System;
using Newtonsoft.Json;

namespace TimeSpanFixDemo
{
    public class AppSettings
    {
        // Changed from int to TimeSpan for better type safety
        public TimeSpan ConnectionTimeout { get; set; } = TimeSpan.FromSeconds(30);
        
        // Default scan interval - 24 hours
        public TimeSpan DefaultScanInterval { get; set; } = TimeSpan.FromHours(24);
        
        // Default uptime check interval - 4 hours
        public TimeSpan UptimeCheckInterval { get; set; } = TimeSpan.FromHours(4);
        
        // Default retry interval for unreachable machines - 4 hours
        public TimeSpan RetryInterval { get; set; } = TimeSpan.FromHours(4);
    }
}