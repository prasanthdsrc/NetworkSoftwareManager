using System;
using System.Windows.Input;
using NetworkSoftwareManager.Services;
using NetworkSoftwareManager.Utils;

namespace NetworkSoftwareManager.ViewModels
{
    /// <summary>
    /// Main view model for the application.
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        private readonly SettingsService _settingsService;

        public MainViewModel()
        {
            _settingsService = new SettingsService();
            
            // Initialize commands
            RefreshCommand = new RelayCommand(OnRefresh, CanRefresh);
            SettingsCommand = new RelayCommand(OnSettings);
        }

        #region Commands

        public ICommand RefreshCommand { get; }
        public ICommand SettingsCommand { get; }

        private bool CanRefresh() => !IsBusy;

        private void OnRefresh()
        {
            // Notify to refresh current view
            StatusMessage = "Refreshing...";
            // This would be handled by the specific view's refresh mechanism
            StatusMessage = "Ready";
        }

        private void OnSettings()
        {
            // Navigate to settings view
            RaiseNavigationRequested("settings");
        }

        #endregion

        #region Events

        public event EventHandler<string> NavigationRequested;
        public event EventHandler<string> ShowNotification;

        /// <summary>
        /// Raises the NavigationRequested event.
        /// </summary>
        /// <param name="destination">The destination view to navigate to.</param>
        public void RaiseNavigationRequested(string destination)
        {
            NavigationRequested?.Invoke(this, destination);
        }

        /// <summary>
        /// Raises the ShowNotification event.
        /// </summary>
        /// <param name="message">The notification message to show.</param>
        public void RaiseShowNotification(string message)
        {
            ShowNotification?.Invoke(this, message);
        }

        #endregion
    }
}
