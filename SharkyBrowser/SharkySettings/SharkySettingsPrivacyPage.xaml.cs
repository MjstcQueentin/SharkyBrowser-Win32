using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SharkyBrowser.SharkyUser;
using System;
using Windows.UI.Popups;

namespace SharkyBrowser.SharkySettings
{
    public sealed partial class SharkySettingsPrivacyPage : Page
    {
        public SharkySettingsPrivacyPage()
        {
            InitializeComponent();

            switch (SharkyUserSettings.Instance.GlobalCookiePolicy)
            {
                case "Accept":
                    AcceptAllCookiesRadioButton.IsChecked = true;
                    break;
                case "RejectThirdParty":
                    RejectThirdPartyCookiesRadioButton.IsChecked = true;
                    break;
                case "Reject":
                    RejectAllCookiesRadioButton.IsChecked = true;
                    break;
            }

            DNTCheckBox.IsChecked = SharkyUserSettings.Instance.DNT;
        }

        private void CookiePolicyRadioButtons_SelectionChanged(object sender, RoutedEventArgs e)
        {
            RadioButton checkedRadio = (RadioButton)sender;
            string choosenPolicy = (string)checkedRadio.Tag;

            SharkyUserSettings.Instance.GlobalCookiePolicy = choosenPolicy;
            SharkyUserSettings.Instance.WriteToFile();
        }

        private void DNTCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SharkyUserSettings.Instance.DNT = true;
            SharkyUserSettings.Instance.WriteToFile();
        }

        private void DNTCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SharkyUserSettings.Instance.DNT = false;
            SharkyUserSettings.Instance.WriteToFile();
        }

        private void EraseBrowsingHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            SharkyUserDatabase.Instance.EmptyResources("history");
        }
    }
}
