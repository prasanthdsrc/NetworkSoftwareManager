using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace NetworkSoftwareManager.Utils
{
    /// <summary>
    /// Helper class for encrypting and decrypting passwords.
    /// </summary>
    public static class PasswordHelper
    {
        // Hardcoded encryption key and IV for simplicity
        // In a production environment, these would be stored securely or derived from machine-specific information
        private static readonly byte[] EncryptionKey = new byte[32] { 
            0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0x01, 
            0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0x01,
            0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0x01,
            0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF, 0x01
        };
        
        private static readonly byte[] EncryptionIV = new byte[16] { 
            0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10,
            0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10
        };

        /// <summary>
        /// Encrypts a password string.
        /// </summary>
        public static string EncryptPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return string.Empty;
            }

            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = EncryptionKey;
                    aes.IV = EncryptionIV;

                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter sw = new StreamWriter(cs))
                            {
                                sw.Write(password);
                            }
                        }
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error encrypting password: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Decrypts an encrypted password string.
        /// </summary>
        public static string DecryptPassword(string encryptedPassword)
        {
            if (string.IsNullOrEmpty(encryptedPassword))
            {
                return string.Empty;
            }

            try
            {
                byte[] cipherText = Convert.FromBase64String(encryptedPassword);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = EncryptionKey;
                    aes.IV = EncryptionIV;

                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (MemoryStream ms = new MemoryStream(cipherText))
                    {
                        using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader sr = new StreamReader(cs))
                            {
                                return sr.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error decrypting password: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Converts a string to a SecureString.
        /// </summary>
        public static System.Security.SecureString ConvertToSecureString(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return new System.Security.SecureString();
            }

            var securePassword = new System.Security.SecureString();
            foreach (char c in password)
            {
                securePassword.AppendChar(c);
            }
            securePassword.MakeReadOnly();
            return securePassword;
        }

        /// <summary>
        /// Generates a random password.
        /// </summary>
        public static string GenerateRandomPassword(int length = 16)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_-+=";
            StringBuilder password = new StringBuilder();
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] randomBytes = new byte[length];
                rng.GetBytes(randomBytes);

                for (int i = 0; i < length; i++)
                {
                    password.Append(validChars[randomBytes[i] % validChars.Length]);
                }
            }
            return password.ToString();
        }
    }
}
