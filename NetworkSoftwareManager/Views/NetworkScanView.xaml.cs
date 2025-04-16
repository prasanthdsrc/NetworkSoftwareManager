using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System;
using System.Globalization;

namespace NetworkSoftwareManager.Views
{
    /// <summary>
    /// Interaction logic for NetworkScanView.xaml
    /// </summary>
    public partial class NetworkScanView : Page
    {
        public NetworkScanView()
        {
            InitializeComponent();
            
            // Set up value converters if not globally defined
            Resources.Add("BooleanToVisibilityConverter", new BooleanToVisibilityConverter());
            Resources.Add("InverseBooleanToVisibilityConverter", new InverseBooleanToVisibilityConverter());
            Resources.Add("BooleanToOnlineStatusConverter", new BooleanToOnlineStatusConverter());
            Resources.Add("InverseBooleanConverter", new InverseBooleanConverter());
        }

        /// <summary>
        /// Handle the password box loaded event to set its initial value.
        /// </summary>
        private void PasswordBox_Loaded(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox == null) return;
            
            var dataContext = passwordBox.DataContext as Models.Machine;
            if (dataContext == null) return;
            
            // Set the password box to the machine's password
            passwordBox.Password = dataContext.Password;
        }

        /// <summary>
        /// Handle the password changed event to update the machine's password.
        /// </summary>
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox == null) return;
            
            var dataContext = passwordBox.DataContext as Models.Machine;
            if (dataContext == null) return;
            
            // Update the machine's password
            dataContext.Password = passwordBox.Password;
        }
    }

    /// <summary>
    /// Converts boolean to its inverse.
    /// </summary>
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return false;
        }
    }
}
