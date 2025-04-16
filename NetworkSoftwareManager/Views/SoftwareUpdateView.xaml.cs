using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace NetworkSoftwareManager.Views
{
    /// <summary>
    /// Interaction logic for SoftwareUpdateView.xaml
    /// </summary>
    public partial class SoftwareUpdateView : Page
    {
        public SoftwareUpdateView()
        {
            InitializeComponent();
            
            // Set up value converters if not globally defined
            if (!Resources.Contains("BooleanToVisibilityConverter"))
            {
                Resources.Add("BooleanToVisibilityConverter", new BooleanToVisibilityConverter());
            }
            
            if (!Resources.Contains("InverseBooleanToVisibilityConverter"))
            {
                Resources.Add("InverseBooleanToVisibilityConverter", new InverseBooleanToVisibilityConverter());
            }
            
            if (!Resources.Contains("InverseBooleanConverter"))
            {
                Resources.Add("InverseBooleanConverter", new InverseBooleanConverter());
            }
            
            Resources.Add("BooleanToLatestVersionConverter", new BooleanToLatestVersionConverter());
            Resources.Add("StringToPendingVisibilityConverter", new StringToPendingVisibilityConverter());
        }
    }

    /// <summary>
    /// Converts boolean to "Latest" or "Specific" text.
    /// </summary>
    public class BooleanToLatestVersionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? "Latest" : "Specific";
            }
            return "Specific";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is string stringValue && stringValue == "Latest";
        }
    }

    /// <summary>
    /// Converts string status to visibility (visible only if "Pending").
    /// </summary>
    public class StringToPendingVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                return stringValue == "Pending" ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }
    }
}
