using System;

namespace NetworkSoftwareManager.Models
{
    /// <summary>
    /// Represents machine uptime data for a specific day.
    /// </summary>
    public class MachineUptime
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
        /// Gets or sets the date.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the total uptime minutes for the day.
        /// </summary>
        public int UptimeMinutes { get; set; }

        /// <summary>
        /// Gets or sets the last time the machine was checked.
        /// </summary>
        public DateTime LastChecked { get; set; }

        /// <summary>
        /// Gets or sets the count of successful checks.
        /// </summary>
        public int SuccessfulChecks { get; set; }

        /// <summary>
        /// Gets or sets the count of failed checks.
        /// </summary>
        public int FailedChecks { get; set; }

        /// <summary>
        /// Gets or sets whether all checks for the day are complete.
        /// </summary>
        public bool IsComplete { get; set; }
    }
}