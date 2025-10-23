using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using SharkyBrowser.SharkyWeb;
using System.Collections.Generic;

namespace SharkyBrowser.SharkyUser
{
    public sealed partial class SharkyUserLibraryHistoryPage : Page
    {
        public SharkyUserLibraryHistoryPage()
        {
            InitializeComponent();
            FetchData();
        }

        private void FetchData()
        {
            List<SharkyWebResource> l = SharkyUserDatabase.Instance.GetResources("history");

            HistoryItemsView.ItemsSource = l;
            IsEmptyInfoBar.IsOpen = l.Count == 0;
        }

        private void ElementGrid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
        }

        private void OpenInNewTabFlyoutItem_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
        }

        private void DeleteFlyoutItem_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            string itemId = ((MenuFlyoutItem)sender).Tag.ToString();

            SharkyUserDatabase.Instance.DeleteResource("history", itemId);
            FetchData();
        }
    }
}
