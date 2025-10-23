using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Xml.Linq;

namespace SharkyBrowser.SharkyUser
{
    public sealed partial class SharkyUserLibraryView : Page
    {
        public SharkyUserLibraryView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string pageParam = e.Parameter?.ToString();

            // Ensure the correct item is selected when navigating back to this page
            if (pageParam == "bookmarks")
            {
                LibraryNavView.SelectedItem = BookmarkNavigationViewItem;
            }
            else
            {
                LibraryNavView.SelectedItem = HistoryNavigationViewItem;
            }
        }

        private void LibraryNavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            FrameNavigationOptions navOptions = new()
            {
                TransitionInfoOverride = args.RecommendedNavigationTransitionInfo
            };

            string tag = args.SelectedItemContainer.Tag.ToString();

            if (tag == "SharkyUserHistoryPage")
            {
                ContentFrame.NavigateToType(typeof(SharkyUserLibraryHistoryPage), null, navOptions);
                LibraryNavView.Header = "History";
            }
            else if (tag == "SharkyUserBookmarkPage")
            {
                ContentFrame.NavigateToType(typeof(SharkyUserLibraryBookmarkPage), null, navOptions);
                LibraryNavView.Header = "Bookmarks";
            }
        }
    }
}
