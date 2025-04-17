using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace NetworkSoftwareManager.Models
{
    public class Machine
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string IPAddress { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime LastScan { get; set; }
        public string OSVersion { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
        public TimeSpan ScanInterval { get; set; }
        public DateTime LastUptimeCheck { get; set; }
        public double UptimePercent { get; set; }
        public DateTime LastReboot { get; set; }
        public bool IsExcluded { get; set; }
        public string CredentialId { get; set; } = string.Empty;
        public int PendingUpdates { get; set; }
        public List<Software> InstalledSoftware { get; set; } = new List<Software>();
    }
}