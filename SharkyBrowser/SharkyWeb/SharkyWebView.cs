using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Web.WebView2.Core;
using SharkyBrowser.SharkyFilter;
using SharkyBrowser.SharkyFilter.FilterCategories;
using SharkyBrowser.SharkySettings;
using SharkyBrowser.SharkyUser;
using System;

namespace SharkyBrowser.SharkyWeb
{
    public class SharkyWebView : WebView2
    {
        SharkyWebResource CurrentPage;

        public event EventHandler<SharkyFilteredResource> NavigationFiltered;
        public event EventHandler<SharkyFilteredResource> WebResourceFiltered;

        public SharkyWebView()
        {
            CoreWebView2Initialized += SharkyWebView_CoreWebView2Initialized;
            NavigationStarting += SharkyWebView_NavigationStarting;
        }

        private void SharkyWebView_NavigationStarting(WebView2 sender, CoreWebView2NavigationStartingEventArgs args)
        {
            // Annuler la navigation si l'URI correspond à un filtre de contenu
            SharkyFilteredResource filteredResource = SharkyFilterController.TestUri(new(args.Uri), SharkyFilteringContext.Navigating);
            if (filteredResource.HasMatched)
            {
                NavigationFiltered?.Invoke(this, filteredResource);
                args.Cancel = true;
                return;
            }

            if (args.NavigationKind == CoreWebView2NavigationKind.NewDocument)
            {
                CurrentPage = new()
                {
                    Name = args.Uri,
                    Uri = new(args.Uri)
                };

                SharkyUserDatabase.Instance.InsertResource("history", CurrentPage);
            }
            else if (args.NavigationKind == CoreWebView2NavigationKind.BackOrForward)
            {
                CurrentPage = null;
            }
        }

        private void SharkyWebView_CoreWebView2Initialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            sender.CoreWebView2.AddWebResourceRequestedFilter("http://*", CoreWebView2WebResourceContext.All);
            sender.CoreWebView2.AddWebResourceRequestedFilter("https://*", CoreWebView2WebResourceContext.All);
            sender.CoreWebView2.WebResourceRequested += CoreWebView2_WebResourceRequested;
            sender.CoreWebView2.DocumentTitleChanged += CoreWebView2_DocumentTitleChanged;
            sender.CoreWebView2.FaviconChanged += CoreWebView2_FaviconChanged;
        }

        private void CoreWebView2_FaviconChanged(CoreWebView2 sender, object args)
        {
            if (string.IsNullOrEmpty(sender.FaviconUri)) return;
            if (CurrentPage is null) return;

            var logo = new BitmapImage
            {
                UriSource = new(sender.FaviconUri)
            };

            CurrentPage.Icon = logo;
            SharkyUserDatabase.Instance.UpdateResource("history", CurrentPage);
        }

        private void CoreWebView2_DocumentTitleChanged(CoreWebView2 sender, object args)
        {
            if (CurrentPage is null) return;
            CurrentPage.Name = sender.DocumentTitle;
            SharkyUserDatabase.Instance.UpdateResource("history", CurrentPage);
        }

        private void CoreWebView2_WebResourceRequested(CoreWebView2 sender, CoreWebView2WebResourceRequestedEventArgs args)
        {
            // Si l'URI correspond à un filtre de contenu, annuler la requête en falsifiant la réponse
            if (args.ResourceContext != CoreWebView2WebResourceContext.Document)
            {
                SharkyFilteredResource filteredResource = SharkyFilterController.TestUri(new(args.Request.Uri), SharkyFilteringContext.FetchingResource);
                if (filteredResource.HasMatched)
                {
                    args.Response = CoreWebView2.Environment.CreateWebResourceResponse(null, 400, "Request canceled", null);
                    WebResourceFiltered?.Invoke(this, filteredResource);
                }
            }
            else
            {
                if (SharkyUserSettings.Instance.SendDNTHeaders)
                {
                    args.Request.Headers.SetHeader("DNT", "1");
                    args.Request.Headers.SetHeader("Sec-GPC", "1");
                }

                args.Request.Headers.SetHeader("Accept-Language", SharkyUserSettings.Instance.AcceptLanguage);
            }
        }
    }
}
