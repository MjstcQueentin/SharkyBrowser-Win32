using Microsoft.UI.Xaml.Controls;
using SharkyBrowser.SharkyUser;
using SharkyBrowser.SharkyWeb;
using System;

namespace SharkyBrowser
{
    public sealed partial class SharkyBookmarkUI : UserControl
    {
        public event EventHandler<SharkyBookmarkUIEventArgs> BookmarkSaved;
        public event EventHandler<SharkyBookmarkUIEventArgs> BookmarkPageRequested;

        private string mode = "add";
        private SharkyWebResource EditedResource = null;

        public SharkyBookmarkUI()
        {
            this.InitializeComponent();
        }

        public void OnFlyoutOpen(string pageName, string pageURI)
        {
            SharkyWebResource resource = SharkyUserDatabase.Instance.GetResourceByURI("bookmark", pageURI);
            if (resource != null)
            {
                mode = "edit";
                PageTitleTextBox.Text = resource.Name;
                PageURITextBox.Text = resource.Uri.ToString();
                EditedResource = resource;
            }
            else
            {
                mode = "add";
                PageTitleTextBox.Text = pageName;
                PageURITextBox.Text = pageURI;
            }
        }

        private void SaveButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (mode == "add")
            {
                SharkyUserDatabase.Instance.InsertResource("bookmark", new()
                {
                    Name = PageTitleTextBox.Text,
                    Uri = new(PageURITextBox.Text),
                });
            }
            else
            {
                EditedResource.Name = PageTitleTextBox.Text;
                EditedResource.Uri = new(PageURITextBox.Text);
                EditedResource.UpdateTime = (long)DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds;
                SharkyUserDatabase.Instance.UpdateResource("bookmark", EditedResource);
            }

            BookmarkSaved.Invoke(this, new());
        }

        private void OpenBookmarksButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            BookmarkPageRequested.Invoke(this, new());
        }
    }

    public class SharkyBookmarkUIEventArgs
    {
    }
}
