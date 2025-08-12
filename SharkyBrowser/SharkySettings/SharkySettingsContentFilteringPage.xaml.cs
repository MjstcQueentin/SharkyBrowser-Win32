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
            BlockAnnoyingContentCheckbox.IsChecked = SharkyUserSettings.Instance.BlockAnnoyances;
            BlockAdvertisementsCheckbox.IsChecked = SharkyUserSettings.Instance.BlockAdvertisements;
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

        private void BlockAnnoyingContentCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            SharkyUserSettings.Instance.BlockAnnoyances = true;
            SharkyWebFilter.Initialize();
        }

        private void BlockAnnoyingContentCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            SharkyUserSettings.Instance.BlockAnnoyances = false;
            SharkyWebFilter.Initialize();
        }

        private void BlockAdvertisementsCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            SharkyUserSettings.Instance.BlockAdvertisements = true;
            SharkyWebFilter.Initialize();
        }

        private void BlockAdvertisementsCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            SharkyUserSettings.Instance.BlockAdvertisements = false;
            SharkyWebFilter.Initialize();
        }
    }
}
