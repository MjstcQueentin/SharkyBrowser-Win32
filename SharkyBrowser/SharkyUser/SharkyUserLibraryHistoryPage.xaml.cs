using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using SharkyBrowser.SharkyWeb;
using System;
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
            Uri uri = (Uri)((FrameworkElement)sender).Tag;
            SharkyWindowManager.GetLastFocusedWindow()?.AddTab(uri.ToString());
        }

        private void OpenInNewTabFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = (Uri)((FrameworkElement)sender).Tag;
            SharkyWindowManager.GetLastFocusedWindow()?.AddTab(uri.ToString());
        }

        private void DeleteFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            string itemId = ((FrameworkElement)sender).Tag.ToString();

            SharkyUserDatabase.Instance.DeleteResource("history", itemId);
            FetchData();
        }
    }
}
