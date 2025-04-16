using System;

namespace NetworkSoftwareManager.Models
{
    /// <summary>
    /// Represents a machine excluded from scanning or updates.
    /// </summary>
    public class ExcludedMachine
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the IP address of the excluded machine.
        /// </summary>
        public string IPAddress { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the hostname (if known).
        /// </summary>
        public string? Hostname { get; set; }

        /// <summary>
        /// Gets or sets the reason for exclusion.
        /// </summary>
        public string Reason { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets when the machine was excluded.
        /// </summary>
        public DateTime ExcludedDate { get; set; }

        /// <summary>
        /// Gets or sets who excluded the machine.
        /// </summary>
        public string ExcludedBy { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether the machine is excluded from scanning.
        /// </summary>
        public bool ExcludeFromScanning { get; set; }

        /// <summary>
        /// Gets or sets whether the machine is excluded from updates.
        /// </summary>
        public bool ExcludeFromUpdates { get; set; }
    }
}