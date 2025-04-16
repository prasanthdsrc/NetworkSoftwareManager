using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NetworkSoftwareManager.Models
{
    /// <summary>
    /// Represents a user of the application.
    /// </summary>
    public class User : INotifyPropertyChanged
    {
        private int _id;
        private string _username = string.Empty;
        private string _passwordHash = string.Empty;
        private string _email = string.Empty;
        private UserRole _role = UserRole.User;
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private DateTime _createdDate = DateTime.Now;
        private DateTime _lastLoginDate = DateTime.MinValue;
        private bool _isActive = true;

        /// <summary>
        /// Gets or sets the unique identifier (primary key).
        /// </summary>
        public int Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username
        {
            get => _username;
            set
            {
                if (_username != value)
                {
                    _username = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the password hash.
        /// </summary>
        public string PasswordHash
        {
            get => _passwordHash;
            set
            {
                if (_passwordHash != value)
                {
                    _passwordHash = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        public string Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the user role.
        /// </summary>
        public UserRole Role
        {
            get => _role;
            set
            {
                if (_role != value)
                {
                    _role = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the user's first name.
        /// </summary>
        public string FirstName
        {
            get => _firstName;
            set
            {
                if (_firstName != value)
                {
                    _firstName = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the user's last name.
        /// </summary>
        public string LastName
        {
            get => _lastName;
            set
            {
                if (_lastName != value)
                {
                    _lastName = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        public DateTime CreatedDate
        {
            get => _createdDate;
            set
            {
                if (_createdDate != value)
                {
                    _createdDate = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the last login date.
        /// </summary>
        public DateTime LastLoginDate
        {
            get => _lastLoginDate;
            set
            {
                if (_lastLoginDate != value)
                {
                    _lastLoginDate = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the user is active.
        /// </summary>
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the full name of the user.
        /// </summary>
        public string FullName => $"{FirstName} {LastName}".Trim();

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    /// <summary>
    /// Defines the possible user roles.
    /// </summary>
    public enum UserRole
    {
        /// <summary>
        /// View-only access.
        /// </summary>
        User = 0,

        /// <summary>
        /// Can manage machines and install software.
        /// </summary>
        Admin = 1,

        /// <summary>
        /// Full access, including user management and database reset.
        /// </summary>
        SuperAdmin = 2
    }
}