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

        public long CreationTime;
        public long? UpdateTime;
        public long? DeletionTime;

        public DateTime CreationDateTime
        {
            get
            {
                return SharkyUtils.UnixTimestampToDateTime(CreationTime);
            }
        }

        public SharkyWebResource()
        {
            Name = "Unnamed webpage";
            Uri = new Uri("about:blank");
            CreationTime = SharkyUtils.DateTimeToUnixTimestamp(DateTime.Now);
        }

        public SharkyWebResource(string name)
        {
            Name = name;
            Uri = new Uri("about:blank");
            CreationTime = SharkyUtils.DateTimeToUnixTimestamp(DateTime.Now);
        }

        public SharkyWebResource(string name, string url)
        {
            Name = name;
            Uri = new Uri(url);
            CreationTime = SharkyUtils.DateTimeToUnixTimestamp(DateTime.Now);
        }

        public SharkyWebResource(string name, string url, long creationTime)
        {
            Name = name;
            Uri = new Uri(url);
            CreationTime = creationTime;
        }

        public SharkyWebResource(string name, string url, long? creationTime, BitmapImage icon)
        {
            Name = name;
            Uri = new Uri(url);
            CreationTime = creationTime ?? SharkyUtils.DateTimeToUnixTimestamp(DateTime.Now);
            Icon = icon;
        }

        public SharkyWebResource(string name, string url, long? creationTime, BitmapImage icon, long? updateTime)
        {
            Name = name;
            Uri = new Uri(url);
            CreationTime = creationTime ?? SharkyUtils.DateTimeToUnixTimestamp(DateTime.Now);
            Icon = icon;
            UpdateTime = updateTime;
        }
    }
}
