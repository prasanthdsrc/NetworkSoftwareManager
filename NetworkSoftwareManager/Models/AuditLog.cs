using System;

namespace NetworkSoftwareManager.Models
{
    /// <summary>
    /// Represents an audit log entry.
    /// </summary>
    public class AuditLog
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the action.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the username of the user who performed the action.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the action type (e.g., "Install", "Uninstall", "Update").
        /// </summary>
        public string ActionType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the target machine IP or hostname.
        /// </summary>
        public string TargetMachine { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the target software name.
        /// </summary>
        public string TargetSoftware { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the details of the action.
        /// </summary>
        public string Details { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the previous version (if applicable).
        /// </summary>
        public string PreviousVersion { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the new version (if applicable).
        /// </summary>
        public string NewVersion { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the status of the action (e.g., "Success", "Failed").
        /// </summary>
        public string Status { get; set; } = string.Empty;
    }
}