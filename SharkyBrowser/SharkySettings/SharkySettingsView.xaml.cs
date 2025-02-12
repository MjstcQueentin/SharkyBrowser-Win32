using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;

namespace SharkyBrowser.SharkySettings
{
    public sealed partial class SharkySettingsView : Page
    {
        public SharkySettingsView()
        {
            this.InitializeComponent();
        }

        private void NavigationView_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            SettingsNavigationView.SelectedItem = SettingsNavigationView.MenuItems[0];
            NavigationView_Navigate(typeof(SharkySettingsGeneralPage), new EntranceNavigationTransitionInfo());
        }

        private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItemContainer != null)
            {
                Type navPageType = Type.GetType(args.InvokedItemContainer.Tag.ToString());
                NavigationView_Navigate(navPageType, args.RecommendedNavigationTransitionInfo);
            }
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItemContainer != null)
            {
                Type navPageType = Type.GetType(args.SelectedItemContainer.Tag.ToString());
                NavigationView_Navigate(navPageType, args.RecommendedNavigationTransitionInfo);
            }
        }

        private void NavigationView_Navigate(Type navPageType,NavigationTransitionInfo transitionInfo)
        {
            // Get the page type before navigation so you can prevent duplicate
            // entries in the backstack.
            Type preNavPageType = SettingsViewFrame.CurrentSourcePageType;

            // Only navigate if the selected page isn't currently loaded.
            if (navPageType is not null && !Type.Equals(preNavPageType, navPageType))
            {
                SettingsViewFrame.Navigate(navPageType, null, transitionInfo);
            }
        }
    }
}
