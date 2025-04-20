using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using SharkyBrowser.SharkyWeb;
using System.Collections.Generic;

namespace SharkyBrowser.SharkyUser
{
    public sealed partial class SharkyUserLibraryBookmarkPage : Page
    {
        public SharkyUserLibraryBookmarkPage()
        {
            InitializeComponent();
            FetchData();
        }

        private void FetchData()
        {
            List<SharkyWebResource> l = SharkyUserDatabase.Instance.GetResources("bookmark");

            BookmarkItemsView.ItemsSource = l;
            IsEmptyInfoBar.IsOpen = l.Count == 0;
        }

        private void ElementGrid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            SharkyBrowsingService.RequestNewTab(((Grid)sender).Tag.ToString());
        }

        private void OpenInNewTabFlyoutItem_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            SharkyBrowsingService.RequestNewTab(((MenuFlyoutItem)sender).Tag.ToString());
        }

        private void DeleteFlyoutItem_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            long itemId = long.Parse(((MenuFlyoutItem)sender).Tag.ToString());

            SharkyUserDatabase.Instance.DeleteResource("bookmark", itemId);
            FetchData();
        }
    }
}
