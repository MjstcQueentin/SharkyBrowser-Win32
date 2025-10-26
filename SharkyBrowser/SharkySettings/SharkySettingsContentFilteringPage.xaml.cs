using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SharkyBrowser.SharkyFilter;

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
            SharkyFilterController.Initialize();
        }

        private void BlockAdultWebsitesCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            SharkyUserSettings.Instance.BlockAdultWebsites = false;
            SharkyFilterController.Initialize();
        }

        private void BlockAnnoyingContentCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            SharkyUserSettings.Instance.BlockAnnoyances = true;
            SharkyFilterController.Initialize();
        }

        private void BlockAnnoyingContentCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            SharkyUserSettings.Instance.BlockAnnoyances = false;
            SharkyFilterController.Initialize();
        }

        private void BlockAdvertisementsCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            SharkyUserSettings.Instance.BlockAdvertisements = true;
            SharkyFilterController.Initialize();
        }

        private void BlockAdvertisementsCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            SharkyUserSettings.Instance.BlockAdvertisements = false;
            SharkyFilterController.Initialize();
        }
    }
}
