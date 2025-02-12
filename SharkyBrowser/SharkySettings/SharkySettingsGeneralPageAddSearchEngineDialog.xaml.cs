using Microsoft.UI.Xaml.Controls;
using SharkyBrowser.SharkyWeb;

namespace SharkyBrowser.SharkySettings
{
    public sealed partial class SharkySettingsGeneralPageAddSearchEngineDialog : Page
    {
        public SharkySettingsGeneralPageAddSearchEngineDialog()
        {
            this.InitializeComponent();
        }

        public SharkyWebSearchEngine NewSearchEngine
        {
            get
            {
                return new SharkyWebSearchEngine
                {
                    Name = SearchEngineNameTextBox.Text,
                    UrlPattern = SearchEngineUrlTextBox.Text
                };
            }
        }
    }
}
