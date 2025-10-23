using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;

namespace SharkyBrowser.SharkyWeb
{
    internal class SharkyWebResource
    {
        public string ID;
        public string Name;
        public Uri Uri;
        public BitmapImage Icon;
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

        public static List<SharkyWebResource> ListFromSqliteReader(SqliteDataReader reader)
        {
            List<SharkyWebResource> resources = [];
            while (reader.Read())
            {
                SharkyWebResource r = new()
                {
                    ID = reader.IsDBNull(reader.GetOrdinal("id")) ? Guid.NewGuid().ToString() : reader.GetString(reader.GetOrdinal("id")),
                    Name = reader.IsDBNull(reader.GetOrdinal("name")) ? "Unnamed webpage" : reader.GetString(reader.GetOrdinal("name")),
                    Uri = new Uri(reader.IsDBNull(reader.GetOrdinal("uri")) ? "about:blank" : reader.GetString(reader.GetOrdinal("uri"))),
                    CreationTime = reader.IsDBNull(reader.GetOrdinal("creationTime")) ? SharkyUtils.DateTimeToUnixTimestamp(DateTime.Now) : reader.GetInt64(reader.GetOrdinal("creationTime")),
                    UpdateTime = reader.IsDBNull(reader.GetOrdinal("updateTime")) ? null : reader.GetInt64(reader.GetOrdinal("updateTime")),
                    DeletionTime = reader.IsDBNull(reader.GetOrdinal("deletionTime")) ? null : reader.GetInt64(reader.GetOrdinal("deletionTime"))
                };
                resources.Add(r);
            }
            return resources;
        }

        public SharkyWebResource()
        {
            ID = Guid.NewGuid().ToString();
            Name = "Unnamed webpage";
            Uri = new Uri("about:blank");
            CreationTime = SharkyUtils.DateTimeToUnixTimestamp(DateTime.Now);
        }
    }
}
