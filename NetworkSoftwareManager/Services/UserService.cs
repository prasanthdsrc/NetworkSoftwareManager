using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using NetworkSoftwareManager.Models;
using Newtonsoft.Json;

namespace NetworkSoftwareManager.Services
{
    /// <summary>
    /// Service for managing users and authentication.
    /// </summary>
    public class UserService
    {
        private readonly string _storagePath;
        private readonly SettingsService _settingsService;
        private User? _currentUser;
        private const string DEFAULT_SUPERADMIN_PASSWORD = "NSM_Admin@2025";
        
        /// <summary>
        /// Gets the currently logged-in user.
        /// </summary>
        public User? CurrentUser => _currentUser;
        
        /// <summary>
        /// Event raised when a user logs in or out.
        /// </summary>
        public event EventHandler<User?>? AuthenticationChanged;

        /// <summary>
        /// Initializes a new instance of the UserService class.
        /// </summary>
        public UserService(SettingsService settingsService)
        {
            _settingsService = settingsService;
            _storagePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "NetworkSoftwareManager");
                
            // Ensure storage directory exists
            Directory.CreateDirectory(_storagePath);
            
            // Initialize with demo data if no users exist
            InitializeDataAsync().Wait();
        }
        
        /// <summary>
        /// Initializes the database with demo data if it doesn't exist.
        /// </summary>
        private async Task InitializeDataAsync()
        {
            string filePath = Path.Combine(_storagePath, "users.json");
            
            // Create default users if file doesn't exist
            if (!File.Exists(filePath))
            {
                var users = new List<User>
                {
                    // Create a superadmin user
                    new User
                    {
                        Id = 1,
                        Username = "superadmin",
                        PasswordHash = HashPassword(DEFAULT_SUPERADMIN_PASSWORD),
                        Email = "superadmin@example.com",
                        Role = UserRole.SuperAdmin,
                        FirstName = "Super",
                        LastName = "Admin",
                        CreatedDate = DateTime.Now,
                        IsActive = true
                    },
                    
                    // Create an admin user
                    new User
                    {
                        Id = 2,
                        Username = "admin",
                        PasswordHash = HashPassword("admin123"),
                        Email = "admin@example.com",
                        Role = UserRole.Admin,
                        FirstName = "Admin",
                        LastName = "User",
                        CreatedDate = DateTime.Now,
                        IsActive = true
                    },
                    
                    // Create a regular user
                    new User
                    {
                        Id = 3,
                        Username = "user",
                        PasswordHash = HashPassword("user123"),
                        Email = "user@example.com",
                        Role = UserRole.User,
                        FirstName = "Regular",
                        LastName = "User",
                        CreatedDate = DateTime.Now,
                        IsActive = true
                    }
                };
                
                // Save users to file
                await SaveUsersAsync(users);
            }
        }

        /// <summary>
        /// Authenticates a user with username and password.
        /// </summary>
        public async Task<bool> AuthenticateAsync(string username, string password)
        {
            try
            {
                // Load users from storage
                var users = await LoadUsersAsync();
                
                // Find user by username
                var user = users.FirstOrDefault(u => 
                    u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && 
                    u.IsActive);
                
                if (user != null && VerifyPassword(password, user.PasswordHash))
                {
                    // Update last login date
                    user.LastLoginDate = DateTime.Now;
                    await SaveUsersAsync(users);
                    
                    // Set current user
                    _currentUser = user;
                    AuthenticationChanged?.Invoke(this, user);
                    
                    return true;
                }
                
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Authentication error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Logs out the current user.
        /// </summary>
        public void Logout()
        {
            _currentUser = null;
            AuthenticationChanged?.Invoke(this, null);
        }

        /// <summary>
        /// Gets all users.
        /// </summary>
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await LoadUsersAsync();
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        public async Task<bool> CreateUserAsync(User user, string password)
        {
            try
            {
                // Only superadmin can create admin accounts
                if (_currentUser?.Role != UserRole.SuperAdmin && user.Role == UserRole.Admin)
                {
                    return false;
                }
                
                // Only superadmin can create other users if they're an admin
                if (_currentUser?.Role != UserRole.SuperAdmin && _currentUser?.Role != UserRole.Admin)
                {
                    return false;
                }
                
                // Prevent creating superadmin accounts
                if (user.Role == UserRole.SuperAdmin)
                {
                    return false;
                }
                
                // Load existing users
                var users = await LoadUsersAsync();
                
                // Check if username already exists
                if (users.Any(u => u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase)))
                {
                    return false;
                }
                
                // Set new user properties
                user.Id = users.Count > 0 ? users.Max(u => u.Id) + 1 : 1;
                user.PasswordHash = HashPassword(password);
                user.CreatedDate = DateTime.Now;
                user.IsActive = true;
                
                // Add user and save
                users.Add(user);
                await SaveUsersAsync(users);
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating user: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                // Check permission
                if (_currentUser == null)
                {
                    return false;
                }
                
                // Only superadmin can edit other superadmins
                if (user.Role == UserRole.SuperAdmin && _currentUser.Role != UserRole.SuperAdmin)
                {
                    return false;
                }
                
                // Load users
                var users = await LoadUsersAsync();
                
                // Find user by id
                var existingUser = users.FirstOrDefault(u => u.Id == user.Id);
                if (existingUser == null)
                {
                    return false;
                }
                
                // Prevent changing the role to superadmin
                if (existingUser.Role != UserRole.SuperAdmin && user.Role == UserRole.SuperAdmin)
                {
                    return false;
                }
                
                // Update user properties but preserve password hash
                string currentHash = existingUser.PasswordHash;
                existingUser.Username = user.Username;
                existingUser.Email = user.Email;
                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.IsActive = user.IsActive;
                
                // Only superadmin can change roles
                if (_currentUser.Role == UserRole.SuperAdmin)
                {
                    existingUser.Role = user.Role;
                }
                
                // Save changes
                await SaveUsersAsync(users);
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Changes a user's password.
        /// </summary>
        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            try
            {
                // Check permission
                if (_currentUser == null)
                {
                    return false;
                }
                
                // Load users
                var users = await LoadUsersAsync();
                
                // Find user by id
                var user = users.FirstOrDefault(u => u.Id == userId);
                if (user == null)
                {
                    return false;
                }
                
                // Verify that we're changing our own password or we're a superadmin
                if (userId != _currentUser.Id && _currentUser.Role != UserRole.SuperAdmin)
                {
                    return false;
                }
                
                // Verify current password if changing own password
                if (userId == _currentUser.Id && !VerifyPassword(currentPassword, user.PasswordHash))
                {
                    return false;
                }
                
                // Update password
                user.PasswordHash = HashPassword(newPassword);
                
                // Save changes
                await SaveUsersAsync(users);
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error changing password: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Resets the database.
        /// </summary>
        public async Task<bool> ResetDatabaseAsync(string confirmationPassword)
        {
            try
            {
                // Only superadmin can reset the database
                if (_currentUser?.Role != UserRole.SuperAdmin)
                {
                    return false;
                }
                
                // Verify password
                if (!VerifyPassword(confirmationPassword, _currentUser.PasswordHash))
                {
                    return false;
                }
                
                // Delete all data files except users
                DeleteDataFiles();
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error resetting database: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Deletes all data files except users.
        /// </summary>
        private void DeleteDataFiles()
        {
            string[] filesToDelete = {
                "machines.json",
                "software_versions.json",
                "updates.json"
            };
            
            foreach (var file in filesToDelete)
            {
                string path = Path.Combine(_storagePath, file);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        /// <summary>
        /// Loads users from storage.
        /// </summary>
        private async Task<List<User>> LoadUsersAsync()
        {
            try
            {
                string filePath = Path.Combine(_storagePath, "users.json");
                
                if (File.Exists(filePath))
                {
                    string json = await File.ReadAllTextAsync(filePath);
                    return JsonConvert.DeserializeObject<List<User>>(json) ?? new List<User>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading users: {ex.Message}");
            }
            
            return new List<User>();
        }

        /// <summary>
        /// Saves users to storage.
        /// </summary>
        private async Task SaveUsersAsync(List<User> users)
        {
            try
            {
                string filePath = Path.Combine(_storagePath, "users.json");
                string json = JsonConvert.SerializeObject(users, Formatting.Indented);
                await File.WriteAllTextAsync(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving users: {ex.Message}");
            }
        }

        /// <summary>
        /// Hashes a password using SHA256.
        /// </summary>
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        /// <summary>
        /// Verifies a password against a hash.
        /// </summary>
        private bool VerifyPassword(string password, string hash)
        {
            string passwordHash = HashPassword(password);
            return passwordHash == hash;
        }
    }
}