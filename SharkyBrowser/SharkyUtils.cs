using System;

namespace SharkyBrowser
{
    public class SharkyUtils
    {
        public static long DateTimeToUnixTimestamp(DateTime dt)
        {
            return (long)dt.Subtract(DateTime.UnixEpoch).TotalSeconds;
        }

        public static DateTime UnixTimestampToDateTime(long unixTimestamp)
        {
            return DateTime.FromFileTime(unixTimestamp * 10000000 + 116444736000000000);
        }
    }
}
