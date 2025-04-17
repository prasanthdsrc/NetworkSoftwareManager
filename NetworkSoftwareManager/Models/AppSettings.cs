using System;
using Newtonsoft.Json;

namespace NetworkSoftwareManager.Models
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
        
        // Email settings
        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; } = 25;
        public string SmtpUsername { get; set; } = string.Empty;
        public string SmtpPassword { get; set; } = string.Empty;
        public string EmailFrom { get; set; } = string.Empty;
        public string EmailTo { get; set; } = string.Empty;
        public bool SendDailyReport { get; set; } = false;
        public TimeSpan DailyReportTime { get; set; } = new TimeSpan(8, 0, 0);  // 8 AM
    }
}