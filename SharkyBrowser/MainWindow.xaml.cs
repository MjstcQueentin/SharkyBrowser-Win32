using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.ApplicationModel.Resources;
using SharkyBrowser.SharkySettings;
using SharkyBrowser.SharkyWindowing;
using System;

namespace SharkyBrowser
{
    public sealed partial class MainWindow : Window
    {
        public App ParentApp;
        private readonly SharkyTabManager TabManager = new();
        private readonly SharkyUserSettings UserSettings = SharkyUserSettings.Instance;
        private readonly ResourceLoader resourceLoader = new(ResourceLoader.GetDefaultResourceFilePath(), "MainWindowResources");

        public MainWindow()
        {
            InitializeComponent();
            Closed += MainWindow_Closed;
            AppWindow.Title = "Sharky";

            ExtendsContentIntoTitleBar = true;
            SetTitleBar(CustomDragRegion);
            SizeChanged += MainWindow_SizeChanged;
            TabControl.GettingFocus += (sender, args) =>
            {
                SharkyWindowManager.SetLastFocusedWindow(this);
            };

            AddTab();
        }

        private void MainWindow_SizeChanged(object sender, WindowSizeChangedEventArgs args)
        {
            if (TabControl.FlowDirection == FlowDirection.LeftToRight)
            {
                CustomDragRegion.MinWidth = AppWindow.TitleBar.RightInset;
                ShellTitlebarInset.MinWidth = AppWindow.TitleBar.LeftInset;
            }
            else
            {
                CustomDragRegion.MinWidth = AppWindow.TitleBar.LeftInset;
                ShellTitlebarInset.MinWidth = AppWindow.TitleBar.RightInset;
            }

            if (CustomDragRegion.MinWidth < 180) CustomDragRegion.MinWidth = 180;
            CustomDragRegion.Height = ShellTitlebarInset.Height = AppWindow.TitleBar.Height;
        }

        public void ForceClose()
        {
            TabControl.TabItems.Clear();
            Close();
        }

        private async void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            if (TabControl.TabItems.Count > 1 && UserSettings.MultipleTabWarningOnClose)
            {
                args.Handled = true;
                ContentDialog confirmationDialog = new()
                {
                    Title = resourceLoader.GetString("WindowCloseConfirmDialogTitle"),
                    Content = string.Format(resourceLoader.GetString("WindowCloseConfirmDialogContent"), TabControl.TabItems.Count),
                    PrimaryButtonText = resourceLoader.GetString("Yes"),
                    CloseButtonText = resourceLoader.GetString("No"),
                    XamlRoot = Content.XamlRoot
                };

                ContentDialogResult result = await confirmationDialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    ForceClose();
                }
            }
        }

        public void AddTab(string uri = null, bool selectNewTab = true)
        {
            uri ??= UserSettings.Homepage;
            SharkyBrowsingUI newBrowser = new(uri);
            newBrowser.NewTabRequested += (sender, args) =>
            {
                AddTab(args.Uri, args.IsUserInitiated);
                args.Handled = true;
            };
            newBrowser.ApplicationCloseRequested += (sender, args) => ParentApp.CloseGracefully(Content.XamlRoot);

            TabViewItem newTab = new()
            {
                IconSource = new SymbolIconSource() { Symbol = Symbol.Globe },
                Header = uri,
                Content = newBrowser,
                Tag = this
            };

            newBrowser.Tag = newTab;
            TabControl.TabItems.Add(newTab);
            if (selectNewTab) TabControl.SelectedIndex = TabControl.TabItems.Count - 1;
        }

        private void TabView_AddTabButtonClick(TabView sender, object args)
        {
            AddTab();
        }

        private void TabControl_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            SharkyBrowsingUI theUi = (SharkyBrowsingUI)args.Tab.Content;
            TabManager.AddClosedTab(theUi.CurrentURI);
            PopLastClosedTabMenuFlyoutItem.IsEnabled = true;

            sender.TabItems.Remove(args.Item);
            if (sender.TabItems.Count == 0) Close();
        }

        private void PopLastClosedTabMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            string url = TabManager.PopClosedTab();

            if (!string.IsNullOrEmpty(url))
            {
                AddTab(url);

                if (TabManager.IsEmpty)
                {
                    PopLastClosedTabMenuFlyoutItem.IsEnabled = false;
                }
            }
        }
    }
}
