using System;

namespace NetworkSoftwareManager.Models
{
    /// <summary>
    /// Represents a record of a software update.
    /// </summary>
    public class UpdateHistory
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the machine ID.
        /// </summary>
        public int MachineId { get; set; }

        /// <summary>
        /// Gets or sets the machine reference.
        /// </summary>
        public Machine? Machine { get; set; }

        /// <summary>
        /// Gets or sets the update ID.
        /// </summary>
        public string UpdateId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the update task ID.
        /// </summary>
        public string? SoftwareUpdateId { get; set; }

        /// <summary>
        /// Gets or sets the software name.
        /// </summary>
        public string SoftwareName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the software publisher.
        /// </summary>
        public string Publisher { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the previous version.
        /// </summary>
        public string PreviousVersion { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the new version.
        /// </summary>
        public string NewVersion { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets when the update was initiated.
        /// </summary>
        public DateTime InitiatedDate { get; set; }

        /// <summary>
        /// Gets or sets when the update was completed.
        /// </summary>
        public DateTime? CompletedDate { get; set; }

        /// <summary>
        /// Gets or sets who initiated the update.
        /// </summary>
        public string InitiatedBy { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the update status.
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets any error message.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets whether it was a manual update.
        /// </summary>
        public bool IsManualUpdate { get; set; }

        /// <summary>
        /// Gets or sets whether it was a forced reinstall.
        /// </summary>
        public bool WasForceReinstall { get; set; }
    }
}