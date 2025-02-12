using Microsoft.UI.Xaml.Controls;

namespace SharkyBrowser.SharkySettings
{
    public sealed partial class SharkySettingsLocalizationPage : Page
    {
        public SharkySettingsLocalizationPage()
        {
            this.InitializeComponent();

            AcceptLanguageTextBox.Text = SharkyUserSettings.Instance.AcceptLanguage;
        }
    }
}
