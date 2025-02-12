using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.Web.WebView2.Core;
using SharkyBrowser.SharkySettings;
using SharkyBrowser.SharkyUser;
using SharkyBrowser.SharkyWeb;
using System;
using System.Globalization;
using System.Reflection;
using System.Text.Encodings.Web;
using Windows.ApplicationModel.DataTransfer;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage.Pickers;

namespace SharkyBrowser
{
    public sealed partial class SharkyBrowsingUI : UserControl, IDisposable
    {
        public event EventHandler<CoreWebView2NewWindowRequestedEventArgs> NewTabRequested;
        public event EventHandler<EventArgs> ApplicationCloseRequested;

        private TabViewItem ParentTab
        {
            get => (TabViewItem)Tag;
        }

        private MainWindow ParentWindow
        {
            get => (MainWindow)ParentTab.Tag;
        }

        private bool IsWebViewInitialized = false;

        public SharkyBrowsingUI(string uri)
        {
            InitializeComponent();
            Loaded += async (s, e) =>
            {
                if(!IsWebViewInitialized)
                {
                    CoreWebView2EnvironmentOptions coreViewOptions = new()
                    {
                        AdditionalBrowserArguments = "--disable-features=msSmartScreenProtection",
                        Language = CultureInfo.InstalledUICulture.Name,
                        EnableTrackingPrevention = SharkyUserSettings.Instance.DNT
                    };

                    await TheWebView.EnsureCoreWebView2Async(await CoreWebView2Environment.CreateWithOptionsAsync("", "", coreViewOptions));

                            UrlBox.Text = uri;
                    TheWebView.Source = new Uri(uri);
                }
            };

            TheWebView.CoreWebView2Initialized += TheWebView_CoreWebView2Initialized;
            DownloadUI.DownloadRetryRequested += DownloadUI_DownloadRetryRequested;
        }

        private void DownloadUI_DownloadRetryRequested(object sender, SharkyFileDownloadUIEventArgs e)
        {
            TheWebView.CoreWebView2.Navigate(e.DownloadURI);
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            TheWebView.CoreWebView2.Navigate(SharkyUserSettings.Instance.Homepage);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            TheWebView.GoBack();
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            TheWebView.GoForward();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            TheWebView.Reload();
        }

        private void UrlBox_ProcessKeyboardAccelerators(UIElement sender, ProcessKeyboardAcceleratorEventArgs args)
        {
            if (args.Key == Windows.System.VirtualKey.Escape)
            {
                UrlBox.Text = TheWebView.Source.AbsoluteUri;
                args.Handled = true;
            }
        }

        private void UrlBox_GotFocus(object sender, RoutedEventArgs e)
        {
            UrlBox.QueryIcon = new SymbolIcon(Symbol.Forward);
        }

        private void UrlBox_LostFocus(object sender, RoutedEventArgs e)
        {
            UrlBox.QueryIcon = null;
        }

        private void UrlBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                sender.ItemsSource = SharkyUserDatabase.Instance.GetDistinctResources(sender.Text);
            }
        }

        private void UrlBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            SharkyWebResource selected = (SharkyWebResource)(args.SelectedItem);
            sender.Text = selected.Uri.AbsoluteUri;
        }

        private void UrlBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            try
            {
                if (args.ChosenSuggestion != null)
                {
                    SharkyWebResource selected = (SharkyWebResource)(args.ChosenSuggestion);
                    TheWebView.CoreWebView2.Navigate(selected.Uri.AbsoluteUri);
                }
                else
                {
                    Uri inputUri = new(args.QueryText);
                    TheWebView.CoreWebView2.Navigate(inputUri.AbsoluteUri);
                }
            }
            catch
            {
                string searchQuery = UrlEncoder.Default.Encode(args.QueryText);
                string searchUrl = SharkyUserSettings.Instance.SearchUrl.Replace("%s", searchQuery);
                Uri inputUri = new(searchUrl);
                TheWebView.CoreWebView2.Navigate(inputUri.AbsoluteUri);
            }
        }

        private void FlyoutButton_Click(object sender, RoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        private void NewTabMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            ParentWindow.AddTab();
        }

        private void NewWindowMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            ParentWindow.ParentApp.OpenNewWindow();
        }

        private async void OpenMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            // Create a file picker
            var openPicker = new FileOpenPicker();

            // Retrieve the window handle (HWND) of the current WinUI 3 window.
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(ParentWindow);

            // Initialize the file picker with the window handle (HWND).
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

            // Set options for your file picker
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add(".html");
            openPicker.FileTypeFilter.Add(".htm");
            openPicker.FileTypeFilter.Add(".pdf");

            // Open the picker for the user to pick a file
            var file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                ParentWindow.AddTab(file.Path);
            }
        }

        private void PrintMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            TheWebView.CoreWebView2.ShowPrintUI(CoreWebView2PrintDialogKind.Browser);
        }

        private void ShareMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManagerInterop.GetForWindow((IntPtr)ParentWindow.AppWindow.Id.Value).DataRequested += (sender, args) =>
            {
                args.Request.Data.Properties.Title = TheWebView.CoreWebView2.DocumentTitle;
                args.Request.Data.SetWebLink(TheWebView.Source);
            };

            DataTransferManagerInterop.ShowShareUIForWindow((IntPtr)ParentWindow.AppWindow.Id.Value);
        }

        private void FullscreenMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            if (ParentWindow.AppWindow.Presenter.Kind == Microsoft.UI.Windowing.AppWindowPresenterKind.FullScreen)
            {
                ParentWindow.AppWindow.SetPresenter(Microsoft.UI.Windowing.AppWindowPresenterKind.Default);
                FullscreenMenuFlyoutIcon.Glyph = "\uE740";
            }
            else
            {
                ParentWindow.AppWindow.SetPresenter(Microsoft.UI.Windowing.AppWindowPresenterKind.FullScreen);
                FullscreenMenuFlyoutIcon.Glyph = "\uE73F";
            }
        }

        private void SettingsMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            ParentWindow.AddTab("sharky:settings");
        }

        private void GetSupportMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            ParentWindow.AddTab("https://www.lesmajesticiels.org/support/kb/product/browser");
        }

        private async void AboutSharkyMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog aboutDialog = new()
            {
                Title = "Sharky",
                Content = string.Concat(
                    "Conçu pour être simple et efficace, votre meilleur compagnon au quotidien pour toutes vos tâches connectées.\n",
                    "Un projet Les Majesticiels\n",
                    "Version ", Assembly.GetExecutingAssembly().GetName().Version.ToString(), "\n",
                    "© 2024-2025 Quentin Pugeat"
                ),
                PrimaryButtonText = "Site internet",
                CloseButtonText = "OK",
                XamlRoot = ParentWindow.Content.XamlRoot
            };
            aboutDialog.PrimaryButtonClick += (sender, args) => ParentWindow.AddTab("https://www.lesmajesticiels.org/browser");

            await aboutDialog.ShowAsync();
        }

        private void CloseSharkyMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            ApplicationCloseRequested.Invoke(this, EventArgs.Empty);
        }

        private void TheWebView_CoreWebView2Initialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            sender.CoreWebView2.FaviconChanged += CoreWebView2_FaviconChanged;
            sender.CoreWebView2.DocumentTitleChanged += CoreWebView2_DocumentTitleChanged;
            sender.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
            sender.CoreWebView2.ServerCertificateErrorDetected += CoreWebView2_ServerCertificateErrorDetected;
            sender.CoreWebView2.DownloadStarting += CoreWebView2_DownloadStarting;

            // Chemin par défaut pour les téléchargements
            sender.CoreWebView2.Profile.DefaultDownloadFolderPath = SharkyUserSettings.Instance.DownloadLocation;

            sender.CoreWebView2.Profile.IsPasswordAutosaveEnabled = false;
            sender.CoreWebView2.Profile.PreferredColorScheme = CoreWebView2PreferredColorScheme.Auto;

            IsWebViewInitialized = true;
        }

        private void CoreWebView2_DownloadStarting(CoreWebView2 sender, CoreWebView2DownloadStartingEventArgs args)
        {
            DownloadUI.AddDownloadItem(args.DownloadOperation);
            FileDownloadButton.Visibility = Visibility.Visible;
            FlyoutBase.ShowAttachedFlyout(FileDownloadButton);
            args.Handled = true;
        }

        private void CoreWebView2_ServerCertificateErrorDetected(CoreWebView2 sender, CoreWebView2ServerCertificateErrorDetectedEventArgs args)
        {
            MainInfoBar.Severity = InfoBarSeverity.Warning;
            MainInfoBar.Title = "Certificate error";
            MainInfoBar.Message = args.ErrorStatus.ToString();
            MainInfoBar.IsOpen = true;

            SecurityButtonText.Text = args.ErrorStatus.ToString();
            SecurityButtonText.Icon = new FontIcon
            {
                Glyph = "\uE785"
            };
        }

        private void CoreWebView2_NewWindowRequested(CoreWebView2 sender, CoreWebView2NewWindowRequestedEventArgs args)
        {
            NewTabRequested.Invoke(this, args);
        }

        private void CoreWebView2_DocumentTitleChanged(CoreWebView2 sender, object args)
        {
            ParentTab.Header = sender.DocumentTitle;
            ParentWindow.Title = string.Concat(sender.DocumentTitle, " | Sharky");
        }

        private void CoreWebView2_FaviconChanged(CoreWebView2 sender, object args)
        {
            if (sender.FaviconUri.Length > 0)
            {
                ParentTab.IconSource = new BitmapIconSource()
                {
                    UriSource = new Uri(sender.FaviconUri),
                    ShowAsMonochrome = false
                };
            }
            else
            {
                ParentTab.IconSource = new SymbolIconSource() { Symbol = Symbol.Globe };
            }
        }

        private void SharkyWebView_NavigationStarting(WebView2 sender, CoreWebView2NavigationStartingEventArgs args)
        {
            if (args.Uri.ToString().Contains("sharky:"))
            {
                TriggerSpecialPage(args.Uri.ToString().Substring(args.Uri.ToString().IndexOf(":") + 1));
                return;
            }

            RemoveSpecialPage();
            UrlBox.Text = args.Uri.ToString();
            ParentTab.Header = args.Uri.ToString();
            RefreshButton.IsEnabled = false;
            TheProgressBar.Visibility = Visibility.Visible;
        }

        private void TheWebView_NavigationCompleted(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            BackButton.IsEnabled = sender.CanGoBack;
            ForwardButton.IsEnabled = sender.CanGoForward;
            RefreshButton.IsEnabled = true;
            TheProgressBar.Visibility = Visibility.Collapsed;

            SecurityButtonText.Text = "This connection is secure";
            SecurityButtonText.Icon = new FontIcon
            {
                Glyph = "\uE72E"
            };
        }

        private void TriggerSpecialPage(string pageName)
        {
            TheWebView.Visibility = Visibility.Collapsed;
            BookmarkButton.Visibility = Visibility.Collapsed;
            RefreshButton.Visibility = Visibility.Collapsed;
            SpecialPageContainer.Children.Clear();

            if (pageName == "settings")
            {
                SpecialPageContainer.Visibility = Visibility.Visible;
                SpecialPageContainer.Children.Add(new SharkySettingsView());
                ParentTab.Header = "Sharky Settings";
                ParentTab.IconSource = new SymbolIconSource() { Symbol = Symbol.Setting };
                ParentWindow.Title = string.Concat("Sharky Settings | Sharky");
                return;
            }

            MainInfoBar.Severity = InfoBarSeverity.Warning;
            MainInfoBar.Title = "Unknown internal URI";
            MainInfoBar.Message = string.Concat("sharky:", pageName, " does not exist.");
            MainInfoBar.IsOpen = true;
            TheWebView.Visibility = Visibility.Visible;
        }

        private void RemoveSpecialPage()
        {
            SpecialPageContainer.Visibility = Visibility.Collapsed;
            SpecialPageContainer.Children.Clear();
            TheWebView.Visibility = Visibility.Visible;
            BookmarkButton.Visibility = Visibility.Visible;
            RefreshButton.Visibility = Visibility.Visible;
        }

        public void Dispose()
        {
            TheWebView.NavigateToString("");
        }

        public void SaveForLater()
        {
            TheWebView.NavigateToString("");
        }

        public void Restore()
        {
            TheWebView.GoBack();
        }
    }
}
