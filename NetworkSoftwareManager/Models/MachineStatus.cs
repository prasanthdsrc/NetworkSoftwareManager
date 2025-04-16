using System;

namespace NetworkSoftwareManager.Models
{
    /// <summary>
    /// Represents a point-in-time status check for a machine.
    /// </summary>
    public class MachineStatus
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
        /// Gets or sets the timestamp of the status check.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets whether the machine was online.
        /// </summary>
        public bool IsOnline { get; set; }

        /// <summary>
        /// Gets or sets the ping response time in milliseconds (if online).
        /// </summary>
        public int ResponseTime { get; set; }

        /// <summary>
        /// Gets or sets any error message (if offline).
        /// </summary>
        public string? ErrorMessage { get; set; }
    }
}