using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using SharkyBrowser.SharkySettings;
using SharkyBrowser.SharkyUser;

namespace SharkyBrowser.SharkyWeb
{
    public class SharkyWebView : WebView2
    {
        SharkyWebResource CurrentPage;

        public SharkyWebView()
        {
            CoreWebView2Initialized += SharkyWebView_CoreWebView2Initialized;
            NavigationStarting += SharkyWebView_NavigationStarting;
            NavigationCompleted += SharkyWebView_NavigationCompleted;
        }

        private void SharkyWebView_NavigationStarting(WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs args)
        {
            CurrentPage = new SharkyWebResource(args.Uri, args.Uri);

            // Annuler la navigation si l'URI correspond à un filtre de contenu
            if (SharkyWebFilter.TestUri(new System.Uri(args.Uri)))
            {
                args.Cancel = true;
                return;
            }
        }

        private void SharkyWebView_NavigationCompleted(WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs args)
        {
            SharkyUserDatabase.Instance.InsertResource("history", CurrentPage);
        }

        private void SharkyWebView_CoreWebView2Initialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            sender.CoreWebView2.AddWebResourceRequestedFilter("http://*", Microsoft.Web.WebView2.Core.CoreWebView2WebResourceContext.All);
            sender.CoreWebView2.AddWebResourceRequestedFilter("https://*", Microsoft.Web.WebView2.Core.CoreWebView2WebResourceContext.All);
            sender.CoreWebView2.WebResourceRequested += CoreWebView2_WebResourceRequested;
            sender.CoreWebView2.DocumentTitleChanged += CoreWebView2_DocumentTitleChanged;
            sender.CoreWebView2.FaviconChanged += CoreWebView2_FaviconChanged;
        }

        private void CoreWebView2_FaviconChanged(Microsoft.Web.WebView2.Core.CoreWebView2 sender, object args)
        {
            if (string.IsNullOrEmpty(sender.FaviconUri)) return;

            var logo = new BitmapImage
            {
                UriSource = new System.Uri(sender.FaviconUri)
            };

            CurrentPage.Icon = logo;
            SharkyUserDatabase.Instance.UpdateResource("history", CurrentPage);
        }

        private void CoreWebView2_DocumentTitleChanged(Microsoft.Web.WebView2.Core.CoreWebView2 sender, object args)
        {
            CurrentPage.Name = sender.DocumentTitle;
            SharkyUserDatabase.Instance.UpdateResource("history", CurrentPage);
        }

        private void CoreWebView2_WebResourceRequested(Microsoft.Web.WebView2.Core.CoreWebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2WebResourceRequestedEventArgs args)
        {
            // Si l'URI correspond à un filtre de contenu, annuler la requête en falsifiant la réponse
            if (SharkyWebFilter.TestUri(new System.Uri(args.Request.Uri)))
            {
                args.Response = CoreWebView2.Environment.CreateWebResourceResponse(null, 400, "Request canceled", null);
            }
            else
            {
                if (SharkyUserSettings.Instance.DNT)
                {
                    args.Request.Headers.SetHeader("DNT", "1");
                    args.Request.Headers.SetHeader("Sec-GPC", "1");
                }

                args.Request.Headers.SetHeader("Accept-Language", SharkyUserSettings.Instance.AcceptLanguage);
            }
        }
    }
}
