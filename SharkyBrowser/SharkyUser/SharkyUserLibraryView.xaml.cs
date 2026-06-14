using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.ApplicationModel.Resources;
using Microsoft.UI.Xaml.Navigation;

namespace SharkyBrowser.SharkyUser
{
    public sealed partial class SharkyUserLibraryView : Page
    {
        private readonly ResourceLoader ResourceLoader = new(ResourceLoader.GetDefaultResourceFilePath(), "UserLibraryResources");
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
                LibraryNavView.Header = ResourceLoader.GetString("HistoryNavigationViewItem/Content");
            }
            else if (tag == "SharkyUserBookmarkPage")
            {
                ContentFrame.NavigateToType(typeof(SharkyUserLibraryBookmarkPage), null, navOptions);
                LibraryNavView.Header = ResourceLoader.GetString("BookmarkNavigationViewItem/Content");
            }
        }
    }
}
