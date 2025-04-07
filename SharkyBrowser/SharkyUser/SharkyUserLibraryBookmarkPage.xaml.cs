using Microsoft.UI.Xaml.Controls;

namespace SharkyBrowser.SharkyUser
{
    public sealed partial class SharkyUserLibraryBookmarkPage : Page
    {
        public SharkyUserLibraryBookmarkPage()
        {
            InitializeComponent();

            BookmarkItemsView.ItemsSource = SharkyUserDatabase.Instance.GetResources("bookmark");
        }
    }
}
