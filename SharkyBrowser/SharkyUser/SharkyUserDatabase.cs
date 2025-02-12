using Microsoft.Data.Sqlite;
using SharkyBrowser.SharkyWeb;
using System;
using System.Collections.Generic;

namespace SharkyBrowser.SharkyUser
{
    /// <summary>
    /// Singleton class to use to access user databases (history and bookmarks)
    /// </summary>
    internal class SharkyUserDatabase : IDisposable
    {
        private static SharkyUserDatabase CurrentInstance;
        public static SharkyUserDatabase Instance
        {
            get
            {
                if (CurrentInstance == null)
                {
                    CurrentInstance = new SharkyUserDatabase();
                }

                return CurrentInstance;
            }
        }

        private SqliteConnection database;

        public SharkyUserDatabase()
        {
            string dbPath = string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "\\Sharky\\userdatabase.db");
            database = new SqliteConnection($"Data Source={dbPath}");
            database.Open();

            var command = database.CreateCommand();
            command.CommandText = @"CREATE TABLE IF NOT EXISTS history(
                creationTime BIGINT NOT NULL PRIMARY KEY,
                name TEXT NOT NULL,
                uri TEXT NOT NULL,
                icon BLOB NULL,
                updateTime BIGINT NULL,
                deletionTime BIGINT NULL
            );";
            command.ExecuteNonQuery();

            command.CommandText = @"CREATE TABLE IF NOT EXISTS bookmark(
                creationTime BIGINT NOT NULL PRIMARY KEY,
                name TEXT NOT NULL,
                uri TEXT NOT NULL,
                icon BLOB NULL,
                updateTime BIGINT NULL,
                deletionTime BIGINT NULL
            );";
            command.ExecuteNonQuery();
        }

        public void Dispose()
        {
            database.Close();
        }

        /// <summary>
        /// Selects all non-deleted resources from a table
        /// </summary>
        /// <param name="table">Table name</param>
        /// <returns>A collection of resources</returns>
        public List<SharkyWebResource> GetResources(string table)
        {
            var command = database.CreateCommand();
            command.CommandText = $@"SELECT name, uri, creationTime, icon, updateTime FROM {table} WHERE deletionTime IS NULL";
            SqliteDataReader reader = command.ExecuteReader();
            List<SharkyWebResource> list = new List<SharkyWebResource>();

            while (reader.Read())
            {
                list.Add(new SharkyWebResource(
                    reader.GetString(0),
                    reader.GetString(1),
                    reader.IsDBNull(4) ? Double.NaN : reader.GetDouble(2),
                    null,
                    reader.IsDBNull(4) ? Double.NaN : reader.GetDouble(4)
                ));
            }

            return list;
        }

        /// <summary>
        /// Search for resources in history
        /// </summary>
        /// <param name="query">Query</param>
        /// <returns>A collection of matching resources</returns>
        public List<SharkyWebResource> GetDistinctResources(string query = null)
        {
            var command = database.CreateCommand();
            command.CommandText = $@"SELECT DISTINCT name, uri, icon FROM history WHERE deletionTime IS NULL";
            if (query is not null && query.Length > 0)
            {
                string escapedQuery = query.Replace("%", "").Replace("'", "");
                command.CommandText += $" AND uri LIKE '%{escapedQuery}%' OR name LIKE '%{escapedQuery}%'";
            }

            SqliteDataReader reader = command.ExecuteReader();
            List<SharkyWebResource> list = new List<SharkyWebResource>();

            while (reader.Read())
            {
                list.Add(new SharkyWebResource(
                    reader.GetString(0),
                    reader.GetString(1)
                ));
            }

            return list;
        }

        /// <summary>
        /// Insert a resource in a table
        /// </summary>
        /// <param name="table">Table name</param>
        /// <param name="resource">Resource to insert</param>
        public void InsertResource(string table, SharkyWebResource resource)
        {
            var command = database.CreateCommand();
            command.CommandText = @$"INSERT INTO {table}(creationTime, name, uri, icon, updateTime, deletionTime) 
                VALUES($creationTime, $name, $uri, $icon, $updateTime, $deletionTime);";
            command.Parameters.AddWithValue("$creationTime", resource.CreationTime);
            command.Parameters.AddWithValue("$name", resource.Name);
            command.Parameters.AddWithValue("$uri", resource.Uri.AbsoluteUri);
            command.Parameters.AddWithValue("$icon", resource.Icon is null ? DBNull.Value : resource.Icon.ToString());
            command.Parameters.AddWithValue("$updateTime", resource.UpdateTime is null ? DBNull.Value : resource.UpdateTime);
            command.Parameters.AddWithValue("$deletionTime", resource.DeletionTime is null ? DBNull.Value : resource.DeletionTime);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Update a resource in a table
        /// </summary>
        /// <param name="table">Table name</param>
        /// <param name="resource">Resource to update, identifiable by its CreationTime</param>
        public void UpdateResource(string table, SharkyWebResource resource)
        {
            var command = database.CreateCommand();
            command.CommandText = @$"UPDATE {table} SET name = $name, uri = $uri, icon = $icon, updateTime = $updateTime, deletionTime = $deletionTime WHERE creationTime = $creationTime";
            command.Parameters.AddWithValue("$creationTime", resource.CreationTime);
            command.Parameters.AddWithValue("$name", resource.Name);
            command.Parameters.AddWithValue("$uri", resource.Uri.AbsoluteUri);
            command.Parameters.AddWithValue("$icon", resource.Icon is null ? DBNull.Value : resource.Icon.ToString());
            command.Parameters.AddWithValue("$updateTime", resource.UpdateTime is null ? DBNull.Value : resource.UpdateTime);
            command.Parameters.AddWithValue("$deletionTime", resource.DeletionTime is null ? DBNull.Value : resource.DeletionTime);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Deletes all resources from a table
        /// </summary>
        /// <param name="table">Table name</param>
        public void EmptyResources(string table)
        {
            var command = database.CreateCommand();
            command.CommandText = @$"UPDATE {table} SET deletionTime = $deletionTime WHERE deletionTime IS NULL;";
            command.Parameters.AddWithValue("$deletionTime", DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds);
            command.ExecuteNonQuery();
        }
    }
}
