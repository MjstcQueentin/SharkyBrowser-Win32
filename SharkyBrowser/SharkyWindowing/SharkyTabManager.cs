using System.Collections.Generic;

namespace SharkyBrowser.SharkyWindowing
{
    /// <summary>
    /// This class intends to enable Sharky to manage closed tabs.
    /// The user will be able to reopen closed tabs, and the browser will keep a history of recently closed tabs for easy access.
    /// </summary>
    internal class SharkyTabManager
    {
        private static SharkyTabManager CurrentInstance;
        public static SharkyTabManager Instance
        {
            get
            {
                CurrentInstance ??= new SharkyTabManager();

                return CurrentInstance;
            }
        }

        private readonly List<string> ClosedTabs;

        public bool IsEmpty
        {
            get
            {
                return ClosedTabs.Count == 0;
            }
        }

        public SharkyTabManager()
        {
            ClosedTabs = [];
        }

        public void AddClosedTab(string url)
        {
            ClosedTabs.Add(url);
            // Limit the number of stored closed tabs to 10 for memory efficiency
            if (ClosedTabs.Count > 10)
            {
                ClosedTabs.RemoveAt(0);
            }
        }

        /// <summary>
        /// Get the latest closed tab URL and remove it from the history.
        /// </summary>
        /// <returns>The URL of the most recently closed tab.</returns>
        public string PopClosedTab()
        {
            string url = string.Empty;

            if (ClosedTabs.Count > 0)
            {
                url = ClosedTabs[ClosedTabs.Count - 1];
                ClosedTabs.RemoveAt(ClosedTabs.Count - 1);
            }

            return url;
        }
    }
}
