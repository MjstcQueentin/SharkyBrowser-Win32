using System;

namespace SharkyBrowser
{
    public class SharkyBrowsingService
    {
        public static event EventHandler<SharkyBrowsingServiceEventArgs> NewTabRequested;

        public static void RequestNewTab(string uri)
        {
            NewTabRequested.Invoke(typeof(SharkyBrowsingService), new()
            {
                Uri = uri
            });
        }
    }

    public class SharkyBrowsingServiceEventArgs
    {
        public string Uri
        {
            get;
            set;
        }
    }
}
