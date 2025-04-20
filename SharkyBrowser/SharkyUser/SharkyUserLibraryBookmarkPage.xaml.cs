using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace SharkyBrowser.SharkyUser
{
    public sealed partial class SharkyUserLibraryBookmarkPage : Page
    {
        public SharkyUserLibraryBookmarkPage()
        {
            InitializeComponent();

            BookmarkItemsView.ItemsSource = SharkyUserDatabase.Instance.GetResources("bookmark");
        }

        private void ElementGrid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            SharkyBrowsingService.RequestNewTab(((Grid)sender).Tag.ToString());
        }
    }
}
