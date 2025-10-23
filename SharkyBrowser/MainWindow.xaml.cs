using ABI.System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SharkyBrowser.SharkySettings;
using SharkyBrowser.SharkyWeb;
using System;
using System.Collections.Generic;

namespace SharkyBrowser
{
    public sealed partial class MainWindow : Window
    {
        public App ParentApp;
        private readonly List<SharkyBrowsingUI> ClosedTabs = [];
        private readonly SharkyUserSettings UserSettings = SharkyUserSettings.Instance;

        public MainWindow()
        {
            InitializeComponent();
            Closed += MainWindow_Closed;
            AppWindow.Title = "Sharky";

            ExtendsContentIntoTitleBar = true;
            SetTitleBar(CustomDragRegion);
            SizeChanged += MainWindow_SizeChanged;

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
                    Title = "Do you really wish to close this window?",
                    Content = "There are " + TabControl.TabItems.Count + " tabs open.",
                    PrimaryButtonText = "Yes",
                    CloseButtonText = "No",
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
            newBrowser.NewTabRequested += (sender, args) => {
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
            if(selectNewTab) TabControl.SelectedIndex = TabControl.TabItems.Count - 1;
        }

        private void RestoreTab()
        {
            if(ClosedTabs.Count > 0)
            {
                SharkyBrowsingUI lastBrowser = ClosedTabs[^1];
                TabViewItem newTab = new()
                {
                    IconSource = new SymbolIconSource() { Symbol = Symbol.Globe },
                    Header = "New tab",
                    Content = lastBrowser,
                    Tag = this
                };

                lastBrowser.Tag = newTab;
                TabControl.TabItems.Add(newTab);
                lastBrowser.Restore();
                ClosedTabs.RemoveAt(ClosedTabs.Count - 1);
                TabControl.SelectedIndex = TabControl.TabItems.Count - 1;
            }
        }

        private void TabView_AddTabButtonClick(TabView sender, object args)
        {
            AddTab();
        }

        private void TabControl_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        { 
            SharkyBrowsingUI theUi = (SharkyBrowsingUI)args.Tab.Content;
            theUi.SaveForLater();
            ClosedTabs.Add(theUi);

            sender.TabItems.Remove(args.Item);
            if (sender.TabItems.Count == 0) Close();
        }
    }
}
