using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SharkyBrowser.SharkyWeb;

namespace SharkyBrowser.SharkySettings
{
    public sealed partial class SharkySettingsSafetyPage : Page
    {
        public SharkySettingsSafetyPage()
        {
            InitializeComponent();

            BlockMalwareCheckbox.IsChecked = SharkyUserSettings.Instance.BlockBadware;
        }

        private void SharkySettingsSafetyPage_Unloaded(object sender, RoutedEventArgs e)
        {
            SharkyWebFilter.Initialize();
        }

        private void BlockMalwareCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            SharkyUserSettings.Instance.BlockBadware = true;
        }

        private void BlockMalwareCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            SharkyUserSettings.Instance.BlockBadware = false;
        }
    }
}
