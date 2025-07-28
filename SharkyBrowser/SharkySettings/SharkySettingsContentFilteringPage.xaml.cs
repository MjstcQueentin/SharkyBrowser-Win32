using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SharkyBrowser.SharkyWeb;

namespace SharkyBrowser.SharkySettings
{
    /// <summary>
    /// Content filtering settings page.
    /// </summary>
    public sealed partial class SharkySettingsContentFilteringPage : Page
    {
        public SharkySettingsContentFilteringPage()
        {
            InitializeComponent();

            BlockAdultWebsitesCheckbox.IsChecked = SharkyUserSettings.Instance.BlockAdultWebsites;
        }

        private void BlockAdultWebsitesCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            SharkyUserSettings.Instance.BlockAdultWebsites = true;
            SharkyWebFilter.Initialize();
        }

        private void BlockAdultWebsitesCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            SharkyUserSettings.Instance.BlockAdultWebsites = false;
            SharkyWebFilter.Initialize();
        }
    }
}
