using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace SharkyBrowser.SharkyUser
{
    public sealed partial class SharkyUserLibraryView : Page
    {
        public SharkyUserLibraryView()
        {
            InitializeComponent();

            SetNavigation(HistoryNavigationViewItem);
        }

        private void LibraryNavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            SetNavigation(args.InvokedItem as NavigationViewItem, new()
            {
                TransitionInfoOverride = args.RecommendedNavigationTransitionInfo
            });
        }

        private void LibraryNavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            SetNavigation(args.SelectedItem as NavigationViewItem, new()
            {
                TransitionInfoOverride = args.RecommendedNavigationTransitionInfo
            });
        }

        private void SetNavigation(NavigationViewItem item, FrameNavigationOptions navOptions = null)
        {
            if (navOptions == null) navOptions = new();

            if (item == HistoryNavigationViewItem)
            {
                ContentFrame.NavigateToType(typeof(SharkyUserLibraryHistoryPage), null, navOptions);
                LibraryNavView.Header = "History";
            }
            else if (item == BookmarkNavigationViewItem)
            {
                ContentFrame.NavigateToType(typeof(SharkyUserLibraryBookmarkPage), null, navOptions);
                LibraryNavView.Header = "Bookmarks";
            }
        }
    }
}
