using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security;

namespace NetworkSoftwareManager.Models
{
    /// <summary>
    /// Represents credentials for remote machine access.
    /// </summary>
    public class Credentials : INotifyPropertyChanged
    {
        private string _domain = string.Empty;
        private string _username = string.Empty;
        private string _password = string.Empty;

        public string Domain
        {
            get => _domain;
            set
            {
                if (_domain != value)
                {
                    _domain = value;
                    OnPropertyChanged();
                }
            }
        }

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

        public string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Creates a copy of the credentials object.
        /// </summary>
        public Credentials Clone()
        {
            return new Credentials
            {
                Domain = Domain,
                Username = Username,
                Password = Password
            };
        }

        /// <summary>
        /// Returns a secured version of the credentials with an encrypted password.
        /// </summary>
        /// <returns>Credentials with encrypted password</returns>
        public Credentials GetSecureCredentials()
        {
            return new Credentials
            {
                Domain = Domain,
                Username = Username,
                Password = Utils.PasswordHelper.EncryptPassword(Password)
            };
        }

        /// <summary>
        /// Returns the decrypted credentials with plaintext password.
        /// </summary>
        /// <returns>Credentials with decrypted password</returns>
        public Credentials GetDecryptedCredentials()
        {
            return new Credentials
            {
                Domain = Domain,
                Username = Username,
                Password = Utils.PasswordHelper.DecryptPassword(Password)
            };
        }

        /// <summary>
        /// Formats the credentials for network authentication.
        /// </summary>
        /// <returns>Formatted credential string</returns>
        public string GetFormattedCredentials()
        {
            if (string.IsNullOrEmpty(Domain))
            {
                return Username;
            }
            return $"{Domain}\\{Username}";
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
