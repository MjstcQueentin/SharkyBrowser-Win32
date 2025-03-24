using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace SharkyBrowser.SharkyUser
{
    public sealed partial class SharkyUserLibraryHistoryPage : Page
    {
        public SharkyUserLibraryHistoryPage()
        {
            InitializeComponent();

            HistoryItemsView.ItemsSource = SharkyUserDatabase.Instance.GetResources("history");
        }
    }
}
