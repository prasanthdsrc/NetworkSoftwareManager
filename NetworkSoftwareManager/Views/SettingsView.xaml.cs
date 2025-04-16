using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace NetworkSoftwareManager.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : Page
    {
        public SettingsView()
        {
            InitializeComponent();
            
            // Set up value converters if not globally defined
            if (!Resources.Contains("InverseBooleanConverter"))
            {
                Resources.Add("InverseBooleanConverter", new InverseBooleanConverter());
            }
        }

        /// <summary>
        /// Handle the global password box loaded event to set its initial value.
        /// </summary>
        private void GlobalPasswordBox_Loaded(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ViewModels.SettingsViewModel;
            if (viewModel == null) return;
            
            // Set the password box to the stored password
            GlobalPasswordBox.Password = viewModel.Settings.GlobalCredentials.Password;
        }

        /// <summary>
        /// Handle the global password changed event to update the password in the view model.
        /// </summary>
        private void GlobalPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ViewModels.SettingsViewModel;
            if (viewModel == null) return;
            
            // Update the password in the view model
            viewModel.Settings.GlobalCredentials.Password = GlobalPasswordBox.Password;
        }
    }
}
