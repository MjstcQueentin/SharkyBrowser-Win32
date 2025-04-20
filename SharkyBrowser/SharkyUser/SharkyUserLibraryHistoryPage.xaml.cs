using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace SharkyBrowser.SharkyUser
{
    public sealed partial class SharkyUserLibraryHistoryPage : Page
    {
        public SharkyUserLibraryHistoryPage()
        {
            InitializeComponent();

            HistoryItemsView.ItemsSource = SharkyUserDatabase.Instance.GetResources("history");
        }

        private void ElementGrid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            SharkyBrowsingService.RequestNewTab(((Grid)sender).Tag.ToString());
        }
    }
}
