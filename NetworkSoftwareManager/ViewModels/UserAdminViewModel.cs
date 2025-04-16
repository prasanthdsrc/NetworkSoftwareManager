using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using NetworkSoftwareManager.Models;
using NetworkSoftwareManager.Services;
using NetworkSoftwareManager.Utils;

namespace NetworkSoftwareManager.ViewModels
{
    /// <summary>
    /// ViewModel for the user administration screen.
    /// </summary>
    public class UserAdminViewModel : BaseViewModel
    {
        private readonly UserService _userService;
        
        private ObservableCollection<User> _users = new();
        private User? _selectedUser;
        private User _newUser = new();
        private string _newPassword = string.Empty;
        private string _confirmPassword = string.Empty;
        private string _statusMessage = string.Empty;
        private bool _isLoading;
        private bool _resetPassword;
        private string _resetDbPassword = string.Empty;
        private bool _showResetDialog;
        
        /// <summary>
        /// Command for loading users.
        /// </summary>
        public ICommand LoadUsersCommand { get; private set; }
        
        /// <summary>
        /// Command for creating a new user.
        /// </summary>
        public ICommand CreateUserCommand { get; private set; }
        
        /// <summary>
        /// Command for updating a user.
        /// </summary>
        public ICommand UpdateUserCommand { get; private set; }
        
        /// <summary>
        /// Command for resetting the database.
        /// </summary>
        public ICommand ResetDatabaseCommand { get; private set; }
        
        /// <summary>
        /// Command for showing the reset dialog.
        /// </summary>
        public ICommand ShowResetDialogCommand { get; private set; }
        
        /// <summary>
        /// Command for canceling the reset dialog.
        /// </summary>
        public ICommand CancelResetCommand { get; private set; }
        
        /// <summary>
        /// Gets or sets the list of users.
        /// </summary>
        public ObservableCollection<User> Users
        {
            get => _users;
            set
            {
                if (_users != value)
                {
                    _users = value;
                    OnPropertyChanged();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the selected user.
        /// </summary>
        public User? SelectedUser
        {
            get => _selectedUser;
            set
            {
                if (_selectedUser != value)
                {
                    _selectedUser = value;
                    OnPropertyChanged();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the new user.
        /// </summary>
        public User NewUser
        {
            get => _newUser;
            set
            {
                if (_newUser != value)
                {
                    _newUser = value;
                    OnPropertyChanged();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the new password.
        /// </summary>
        public string NewPassword
        {
            get => _newPassword;
            set
            {
                if (_newPassword != value)
                {
                    _newPassword = value;
                    OnPropertyChanged();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the confirm password.
        /// </summary>
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                if (_confirmPassword != value)
                {
                    _confirmPassword = value;
                    OnPropertyChanged();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the status message.
        /// </summary>
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    OnPropertyChanged();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets whether the view is loading data.
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets whether to reset the password.
        /// </summary>
        public bool ResetPassword
        {
            get => _resetPassword;
            set
            {
                if (_resetPassword != value)
                {
                    _resetPassword = value;
                    OnPropertyChanged();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the password for database reset.
        /// </summary>
        public string ResetDbPassword
        {
            get => _resetDbPassword;
            set
            {
                if (_resetDbPassword != value)
                {
                    _resetDbPassword = value;
                    OnPropertyChanged();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets whether to show the reset dialog.
        /// </summary>
        public bool ShowResetDialog
        {
            get => _showResetDialog;
            set
            {
                if (_showResetDialog != value)
                {
                    _showResetDialog = value;
                    OnPropertyChanged();
                }
            }
        }
        
        /// <summary>
        /// Gets whether the current user is a superadmin.
        /// </summary>
        public bool IsSuperAdmin => _userService.CurrentUser?.Role == UserRole.SuperAdmin;
        
        /// <summary>
        /// Initializes a new instance of the UserAdminViewModel class.
        /// </summary>
        public UserAdminViewModel(UserService userService)
        {
            _userService = userService;
            
            LoadUsersCommand = new RelayCommand(async () => await LoadUsersAsync());
            CreateUserCommand = new RelayCommand(async () => await CreateUserAsync());
            UpdateUserCommand = new RelayCommand(async () => await UpdateUserAsync());
            ResetDatabaseCommand = new RelayCommand(async () => await ResetDatabaseAsync());
            ShowResetDialogCommand = new RelayCommand(() => ShowResetDialog = true);
            CancelResetCommand = new RelayCommand(() => ShowResetDialog = false);
            
            // Load users on initialization
            _ = LoadUsersAsync();
        }
        
        /// <summary>
        /// Loads users from the service.
        /// </summary>
        private async Task LoadUsersAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Loading users...";
                
                var users = await _userService.GetAllUsersAsync();
                
                Users.Clear();
                foreach (var user in users.OrderBy(u => u.Username))
                {
                    Users.Add(user);
                }
                
                StatusMessage = $"Loaded {users.Count} users.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading users: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        /// <summary>
        /// Creates a new user.
        /// </summary>
        private async Task CreateUserAsync()
        {
            try
            {
                // Validate input
                if (string.IsNullOrEmpty(NewUser.Username) || 
                    string.IsNullOrEmpty(NewPassword) || 
                    string.IsNullOrEmpty(ConfirmPassword))
                {
                    StatusMessage = "Please fill in all required fields.";
                    return;
                }
                
                if (NewPassword != ConfirmPassword)
                {
                    StatusMessage = "Passwords do not match.";
                    return;
                }
                
                // Create user
                bool result = await _userService.CreateUserAsync(NewUser, NewPassword);
                
                if (result)
                {
                    StatusMessage = $"User '{NewUser.Username}' created successfully.";
                    
                    // Reset form
                    NewUser = new User();
                    NewPassword = string.Empty;
                    ConfirmPassword = string.Empty;
                    
                    // Refresh users
                    await LoadUsersAsync();
                }
                else
                {
                    StatusMessage = "Failed to create user. Check if username already exists or if you have sufficient permissions.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error creating user: {ex.Message}";
            }
        }
        
        /// <summary>
        /// Updates an existing user.
        /// </summary>
        private async Task UpdateUserAsync()
        {
            if (SelectedUser == null)
            {
                StatusMessage = "No user selected.";
                return;
            }
            
            try
            {
                // Update user
                bool result = await _userService.UpdateUserAsync(SelectedUser);
                
                if (result)
                {
                    StatusMessage = $"User '{SelectedUser.Username}' updated successfully.";
                    
                    // If we're also resetting the password
                    if (ResetPassword && !string.IsNullOrEmpty(NewPassword))
                    {
                        if (NewPassword != ConfirmPassword)
                        {
                            StatusMessage = "Passwords do not match.";
                            return;
                        }
                        
                        bool passwordResult = await _userService.ChangePasswordAsync(
                            SelectedUser.Id, 
                            string.Empty, // Current password not needed for admin reset
                            NewPassword);
                        
                        if (passwordResult)
                        {
                            StatusMessage = $"User '{SelectedUser.Username}' updated and password reset.";
                        }
                        else
                        {
                            StatusMessage = $"User updated but password reset failed.";
                        }
                        
                        // Reset form
                        NewPassword = string.Empty;
                        ConfirmPassword = string.Empty;
                        ResetPassword = false;
                    }
                    
                    // Refresh users
                    await LoadUsersAsync();
                }
                else
                {
                    StatusMessage = "Failed to update user. Check if you have sufficient permissions.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error updating user: {ex.Message}";
            }
        }
        
        /// <summary>
        /// Resets the database.
        /// </summary>
        private async Task ResetDatabaseAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(ResetDbPassword))
                {
                    StatusMessage = "Please enter your password to confirm database reset.";
                    return;
                }
                
                bool result = await _userService.ResetDatabaseAsync(ResetDbPassword);
                
                if (result)
                {
                    StatusMessage = "Database reset successful. All data has been cleared.";
                    ShowResetDialog = false;
                    ResetDbPassword = string.Empty;
                }
                else
                {
                    StatusMessage = "Database reset failed. Check your password or permissions.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error resetting database: {ex.Message}";
            }
        }
    }
}