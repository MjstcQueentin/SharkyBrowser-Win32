
namespace SharkyBrowser
{
    class SharkyWindowManager
    {
        private static MainWindow LastFocusedWindow = null;

        public static void SetLastFocusedWindow(MainWindow window)
        {
            LastFocusedWindow = window;
        }

        public static MainWindow GetLastFocusedWindow()
        {
            return LastFocusedWindow;
        }
    }
}
