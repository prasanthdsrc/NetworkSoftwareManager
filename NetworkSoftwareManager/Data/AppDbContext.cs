using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using NetworkSoftwareManager.Models;

namespace NetworkSoftwareManager.Data
{
    /// <summary>
    /// Database context for the NetworkSoftwareManager application.
    /// </summary>
    public class AppDbContext : DbContext
    {
        // Tables
        public DbSet<Machine> Machines { get; set; } = null!;
        public DbSet<Software> Software { get; set; } = null!;
        public DbSet<SoftwareUpdate> SoftwareUpdates { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;
        public DbSet<MachineUptime> MachineUptimes { get; set; } = null!;
        public DbSet<MachineStatus> MachineStatuses { get; set; } = null!;
        public DbSet<ExcludedMachine> ExcludedMachines { get; set; } = null!;
        public DbSet<UpdateHistory> UpdateHistory { get; set; } = null!;

        /// <summary>
        /// Gets the database connection string.
        /// </summary>
        public static string GetConnectionString()
        {
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "NetworkSoftwareManager",
                "Data");
            
            // Ensure the directory exists
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }
            
            return $"Data Source={Path.Combine(appDataPath, "nsm.db")}";
        }

        /// <summary>
        /// Configures the database context.
        /// </summary>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(GetConnectionString());
        }

        /// <summary>
        /// Configures the database model.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Machine entity
            modelBuilder.Entity<Machine>()
                .HasKey(m => m.Id);
            
            modelBuilder.Entity<Machine>()
                .HasIndex(m => m.IPAddress)
                .IsUnique();
            
            modelBuilder.Entity<Machine>()
                .HasMany(m => m.InstalledSoftware)
                .WithOne()
                .HasForeignKey(s => s.MachineId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Configure Software entity
            modelBuilder.Entity<Software>()
                .HasKey(s => s.Id);
            
            // Configure SoftwareUpdate entity
            modelBuilder.Entity<SoftwareUpdate>()
                .HasKey(u => u.Id);
            
            // Configure AuditLog entity
            modelBuilder.Entity<AuditLog>()
                .HasKey(a => a.Id);
            
            // Configure MachineUptime entity
            modelBuilder.Entity<MachineUptime>()
                .HasKey(u => u.Id);
            
            modelBuilder.Entity<MachineUptime>()
                .HasIndex(u => new { u.MachineId, u.Date })
                .IsUnique();
            
            // Configure MachineStatus entity
            modelBuilder.Entity<MachineStatus>()
                .HasKey(s => s.Id);
            
            // Configure ExcludedMachine entity
            modelBuilder.Entity<ExcludedMachine>()
                .HasKey(e => e.Id);
            
            modelBuilder.Entity<ExcludedMachine>()
                .HasIndex(e => e.IPAddress)
                .IsUnique();
            
            // Configure UpdateHistory entity
            modelBuilder.Entity<UpdateHistory>()
                .HasKey(h => h.Id);
        }
    }
}