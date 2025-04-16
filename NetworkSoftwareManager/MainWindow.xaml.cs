using System.Windows;
using System.Windows.Controls;
using NetworkSoftwareManager.ViewModels;
using NetworkSoftwareManager.Views;

namespace NetworkSoftwareManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DashboardView _dashboardView;
        private readonly NetworkScanView _networkScanView;
        private readonly SoftwareInventoryView _softwareInventoryView;
        private readonly SoftwareUpdateView _softwareUpdateView;
        private readonly SettingsView _settingsView;

        public MainWindow()
        {
            InitializeComponent();
            
            // Initialize views
            _dashboardView = new DashboardView();
            _networkScanView = new NetworkScanView();
            _softwareInventoryView = new SoftwareInventoryView();
            _softwareUpdateView = new SoftwareUpdateView();
            _settingsView = new SettingsView();
            
            // Show the dashboard by default
            MainFrame.Navigate(_dashboardView);
            
            // Set up event handlers for view model
            var viewModel = (MainViewModel)DataContext;
            viewModel.NavigationRequested += ViewModel_NavigationRequested;
            viewModel.ShowNotification += ViewModel_ShowNotification;
        }

        private void ViewModel_NavigationRequested(object sender, string destination)
        {
            // Navigate based on the requested destination
            switch (destination.ToLower())
            {
                case "dashboard":
                    MenuItems.SelectedItem = DashboardItem;
                    break;
                case "networkscan":
                    MenuItems.SelectedItem = NetworkScanItem;
                    break;
                case "softwareinventory":
                    MenuItems.SelectedItem = SoftwareInventoryItem;
                    break;
                case "softwareupdate":
                    MenuItems.SelectedItem = SoftwareUpdateItem;
                    break;
                case "settings":
                    MenuItems.SelectedItem = SettingsItem;
                    break;
            }
        }

        private void ViewModel_ShowNotification(object sender, string message)
        {
            MainSnackbar.MessageQueue.Enqueue(message);
        }

        private void MenuItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainFrame == null) return;
            
            // Change the current view based on the selected menu item
            if (DashboardItem.IsSelected)
            {
                MainFrame.Navigate(_dashboardView);
            }
            else if (NetworkScanItem.IsSelected)
            {
                MainFrame.Navigate(_networkScanView);
            }
            else if (SoftwareInventoryItem.IsSelected)
            {
                MainFrame.Navigate(_softwareInventoryView);
            }
            else if (SoftwareUpdateItem.IsSelected)
            {
                MainFrame.Navigate(_softwareUpdateView);
            }
            else if (SettingsItem.IsSelected)
            {
                MainFrame.Navigate(_settingsView);
            }
        }
    }
}
