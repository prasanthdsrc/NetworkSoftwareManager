using System;
using Newtonsoft.Json;

namespace NetworkSoftwareManager.Models
{
    public class Software
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public string InstalledVersion { get; set; } = string.Empty;
        public string LatestVersion { get; set; } = string.Empty;
        public DateTime InstallDate { get; set; }
        public string InstallLocation { get; set; } = string.Empty;
        public bool UseLatestVersion { get; set; } = true;
        public string TargetVersion { get; set; } = string.Empty;
        public bool UpdateAvailable { get; set; }
        public int InstallCount { get; set; }

        public string EffectiveTargetVersion => UseLatestVersion ? LatestVersion : TargetVersion;
    }
}