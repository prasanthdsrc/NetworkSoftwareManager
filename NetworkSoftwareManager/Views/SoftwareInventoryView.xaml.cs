using System.Windows.Controls;

namespace NetworkSoftwareManager.Views
{
    /// <summary>
    /// Interaction logic for SoftwareInventoryView.xaml
    /// </summary>
    public partial class SoftwareInventoryView : Page
    {
        public SoftwareInventoryView()
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
        }
    }
}
