using System;
using System.Windows;
using System.Windows.Controls;
using NetworkSoftwareManager.Models;
using NetworkSoftwareManager.ViewModels;

namespace NetworkSoftwareManager.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        private readonly LoginViewModel _viewModel;

        /// <summary>
        /// Event raised when login is successful.
        /// </summary>
        public event EventHandler<User>? LoginSuccessful;

        public LoginView()
        {
            InitializeComponent();
            
            // Get the view model from the data context
            _viewModel = (LoginViewModel)DataContext;
            
            // Wire up events
            if (_viewModel != null)
            {
                _viewModel.LoginSuccessful += ViewModel_LoginSuccessful;
            }
            
            // Handle the password box
            this.Loaded += LoginView_Loaded;
        }

        private void LoginView_Loaded(object sender, RoutedEventArgs e)
        {
            // Set initial password from view model
            if (!string.IsNullOrEmpty(_viewModel.Password))
            {
                PasswordBox.Password = _viewModel.Password;
            }
            
            // Handle password changes
            PasswordBox.PasswordChanged += PasswordBox_PasswordChanged;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Update the view model when password changes
            if (_viewModel != null)
            {
                _viewModel.Password = PasswordBox.Password;
            }
        }

        private void ViewModel_LoginSuccessful(object? sender, User user)
        {
            // Propagate the event
            LoginSuccessful?.Invoke(this, user);
        }
    }
}