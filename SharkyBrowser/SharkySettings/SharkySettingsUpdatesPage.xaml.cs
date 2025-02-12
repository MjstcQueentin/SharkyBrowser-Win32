using Microsoft.UI.Xaml.Controls;
using System.Reflection;

namespace SharkyBrowser.SharkySettings
{
    public sealed partial class SharkySettingsUpdatesPage : Page
    {

        public SharkySettingsUpdatesPage()
        {
            InitializeComponent();
            VersionTextBlock.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
