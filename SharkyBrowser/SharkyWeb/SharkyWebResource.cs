using Microsoft.UI.Xaml.Media.Imaging;
using System;

namespace SharkyBrowser.SharkyWeb
{
    internal class SharkyWebResource
    {
        public string Name;
        public Uri Uri;
#nullable enable
        public BitmapImage? Icon;
#nullable disable

        public double CreationTime;
        public double? UpdateTime;
        public double? DeletionTime;

        public SharkyWebResource()
        {
            Name = "Unnamed webpage";
            Uri = new Uri("about:blank");
            CreationTime = DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds;
        }

        public SharkyWebResource(string name)
        {
            Name = name;
            Uri = new Uri("about:blank");
            CreationTime = DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds;
        }

        public SharkyWebResource(string name, string url)
        {
            Name = name;
            Uri = new Uri(url);
            CreationTime = DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds;
        }

        public SharkyWebResource(string name, string url, double creationTime)
        {
            Name = name;
            Uri = new Uri(url);
            CreationTime = creationTime;
        }

        public SharkyWebResource(string name, string url, double creationTime, BitmapImage icon)
        {
            Name = name;
            Uri = new Uri(url);
            CreationTime = creationTime;
            Icon = icon;
        }

        public SharkyWebResource(string name, string url, double creationTime, BitmapImage icon, double updateTime)
        {
            Name = name;
            Uri = new Uri(url);
            CreationTime = creationTime;
            Icon = icon;
            UpdateTime = updateTime;
        }
    }
}
