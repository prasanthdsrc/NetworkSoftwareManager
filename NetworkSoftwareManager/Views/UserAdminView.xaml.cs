using System.Windows;
using System.Windows.Controls;
using NetworkSoftwareManager.ViewModels;

namespace NetworkSoftwareManager.Views
{
    /// <summary>
    /// Interaction logic for UserAdminView.xaml
    /// </summary>
    public partial class UserAdminView : UserControl
    {
        private readonly UserAdminViewModel _viewModel;

        public UserAdminView()
        {
            InitializeComponent();
            
            // Get the view model from the data context
            _viewModel = (UserAdminViewModel)DataContext;
            
            // Handle the password boxes
            this.Loaded += UserAdminView_Loaded;
        }

        private void UserAdminView_Loaded(object sender, RoutedEventArgs e)
        {
            // Set up password box events
            NewPasswordBox.PasswordChanged += NewPasswordBox_PasswordChanged;
            ConfirmPasswordBox.PasswordChanged += ConfirmPasswordBox_PasswordChanged;
            NewUserPasswordBox.PasswordChanged += NewUserPasswordBox_PasswordChanged;
            NewUserConfirmPasswordBox.PasswordChanged += NewUserConfirmPasswordBox_PasswordChanged;
            ResetDbPasswordBox.PasswordChanged += ResetDbPasswordBox_PasswordChanged;
        }

        private void NewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.NewPassword = NewPasswordBox.Password;
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.ConfirmPassword = ConfirmPasswordBox.Password;
        }

        private void NewUserPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.NewPassword = NewUserPasswordBox.Password;
        }

        private void NewUserConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.ConfirmPassword = NewUserConfirmPasswordBox.Password;
        }

        private void ResetDbPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.ResetDbPassword = ResetDbPasswordBox.Password;
        }
    }
}