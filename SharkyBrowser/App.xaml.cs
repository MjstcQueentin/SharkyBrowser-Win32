using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using SharkyBrowser.SharkyFilter;
using SharkyBrowser.SharkySettings;
using SharkyBrowser.SharkyWeb;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SharkyBrowser
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.m_windows = new List<MainWindow>();

            SharkyFilterController.Initialize();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            Directory.CreateDirectory(string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "\\Sharky\\"));
            SQLitePCL.Batteries.Init();
            OpenNewWindow();
        }

        private readonly List<MainWindow> m_windows;

        public void OpenNewWindow()
        {
            MainWindow m_window = new()
            {
                ParentApp = this
            };
            m_window.Closed += OnMainWindowClosed;
            m_window.Activate();

            m_windows.Add(m_window);
        }

        public void OnMainWindowClosed(object sender, WindowEventArgs e) {
            MainWindow m_window = (MainWindow)sender;
            if (e.Handled) return;
            m_windows.Remove(m_window);
            if (m_windows.Count == 0) CloseGracefully();
        }

        public async void CloseGracefully(XamlRoot xamlRoot = null)
        {
            if (xamlRoot != null && m_windows.Count > 1 && SharkyUserSettings.Instance.MultipleTabWarningOnClose)
            {
                ContentDialog confirmationDialog = new()
                {
                    Title = "Do you really wish to close Sharky?",
                    Content = "There are " + m_windows.Count + " windows open.",
                    PrimaryButtonText = "Yes",
                    CloseButtonText = "No",
                    XamlRoot = xamlRoot
                };

                ContentDialogResult result = await confirmationDialog.ShowAsync();

                if (result == ContentDialogResult.None)
                {
                    return;
                }
            }

            SharkyUserSettings.Instance.WriteToFile();

            for(int i = m_windows.Count -1; i >= 0; i--)
            {
                m_windows[i].ForceClose();
            }
            Exit();
        }
    }
}
