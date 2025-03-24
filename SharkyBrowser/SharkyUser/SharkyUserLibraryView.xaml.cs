using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Xml.Linq;

namespace SharkyBrowser.SharkyUser
{
    public sealed partial class SharkyUserLibraryView : Page
    {
        public SharkyUserLibraryView()
        {
            InitializeComponent();

            FrameNavigationOptions navOptions = new();
            ContentFrame.NavigateToType(typeof(SharkyUserLibraryHistoryPage), null, navOptions);
            LibraryNavView.Header = "History";
        }

        private void LibraryNavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            FrameNavigationOptions navOptions = new();
            navOptions.TransitionInfoOverride = args.RecommendedNavigationTransitionInfo;

            switch (args.InvokedItem.GetType().Name)
            {
                case "SharkyUserLibraryHistoryPage":
                    ContentFrame.NavigateToType(typeof(SharkyUserLibraryHistoryPage), null, navOptions);
                    LibraryNavView.Header = "History";
                    break;
            }
        }
    }
}
