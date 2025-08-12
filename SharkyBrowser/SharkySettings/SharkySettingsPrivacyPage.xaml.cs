using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SharkyBrowser.SharkyUser;
using SharkyBrowser.SharkyWeb;
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

            DNTCheckBox.IsChecked = SharkyUserSettings.Instance.SendDNTHeaders;
            BlockTrackingDomainsCheckbox.IsChecked = SharkyUserSettings.Instance.BlockTrackingRequests;
        }

        private void SharkySettingsPrivacyPage_Unloaded(object sender, RoutedEventArgs e)
        {
            SharkyUserSettings.Instance.WriteToFile();
            SharkyWebFilter.Initialize();
        }

        private void CookiePolicyRadioButtons_SelectionChanged(object sender, RoutedEventArgs e)
        {
            RadioButton checkedRadio = (RadioButton)sender;
            string choosenPolicy = (string)checkedRadio.Tag;

            SharkyUserSettings.Instance.GlobalCookiePolicy = choosenPolicy;
        }

        private void DNTCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SharkyUserSettings.Instance.SendDNTHeaders = true;
        }

        private void DNTCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SharkyUserSettings.Instance.SendDNTHeaders = false;
        }

        private void BlockTrackingDomainsCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            SharkyUserSettings.Instance.BlockTrackingRequests = true;
        }

        private void BlockTrackingDomainsCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            SharkyUserSettings.Instance.BlockTrackingRequests = false;
        }

        private void EraseBrowsingHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            SharkyUserDatabase.Instance.EmptyResources("history");
        }
    }
}
