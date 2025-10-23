using Microsoft.Data.Sqlite;
using SharkyBrowser.SharkyWeb;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;

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
                CurrentInstance ??= new SharkyUserDatabase();

                return CurrentInstance;
            }
        }

        private readonly SqliteConnection database;

        public SharkyUserDatabase()
        {
            string dbPath = string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "\\Sharky\\userdatabase.db");
            database = new SqliteConnection($"Data Source={dbPath}");
            database.Open();
            var command = database.CreateCommand();

            command.CommandText = "PRAGMA user_version;";
            var reader = command.ExecuteReader();
            reader.Read();
            int databaseVersion = reader.GetInt32(0);
            reader.Close();

            if(databaseVersion < 2)
            {
                command.CommandText = "DROP TABLE IF EXISTS history; DROP TABLE IF EXISTS bookmark;";
                command.ExecuteNonQuery();

                command.CommandText = @"CREATE TABLE IF NOT EXISTS history(
                    id TEXT NOT NULL PRIMARY KEY,
                    name TEXT NULL,
                    uri TEXT NULL,
                    icon BLOB NULL,
                    creationTime BIGINT NOT NULL,
                    updateTime BIGINT NULL,
                    deletionTime BIGINT NULL
                );";
                command.ExecuteNonQuery();

                command.CommandText = @"CREATE TABLE IF NOT EXISTS bookmark(
                    id TEXT NOT NULL PRIMARY KEY,
                    name TEXT NULL,
                    uri TEXT NULL,
                    icon BLOB NULL,
                    creationTime BIGINT NOT NULL,
                    updateTime BIGINT NULL,
                    deletionTime BIGINT NULL
                );";
                command.ExecuteNonQuery();
            }

            command.CommandText = "PRAGMA user_version = 2;";
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
            command.CommandText = $@"SELECT * FROM {table} WHERE deletionTime IS NULL ORDER BY creationTime DESC";
            SqliteDataReader reader = command.ExecuteReader();
            return SharkyWebResource.ListFromSqliteReader(reader);
        }

        public SharkyWebResource GetResourceByURI(string table, string uri)
        {
            var command = database.CreateCommand();
            command.CommandText = $@"SELECT id, name, uri, icon, creationTime, updateTime FROM {table} WHERE uri = $uri AND deletionTime IS NULL";
            command.Parameters.AddWithValue("$uri", uri);
            SqliteDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                SharkyWebResource resource = new()
                {
                    ID = reader.GetString(reader.GetOrdinal("id")),
                    Name = reader.GetString(reader.GetOrdinal("name")),
                    Uri = new Uri(reader.GetString(reader.GetOrdinal("uri"))),
                    CreationTime = reader.GetInt64(reader.GetOrdinal("creationTime")),
                    UpdateTime = reader.IsDBNull(reader.GetOrdinal("updateTime")) ? null : reader.GetInt64(reader.GetOrdinal("updateTime"))
                };

                return resource;
            }
            else
            {
                return null;
            }
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
            List<SharkyWebResource> resources = [];
            while (reader.Read())
            {
                SharkyWebResource r = new()
                {
                    Name = reader.IsDBNull(reader.GetOrdinal("name")) ? "Unnamed webpage" : reader.GetString(reader.GetOrdinal("name")),
                    Uri = new Uri(reader.IsDBNull(reader.GetOrdinal("uri")) ? "about:blank" : reader.GetString(reader.GetOrdinal("uri")))
                };
                resources.Add(r);
            }

            return resources;
        }

        /// <summary>
        /// Insert a resource in a table
        /// </summary>
        /// <param name="table">Table name</param>
        /// <param name="resource">Resource to insert</param>
        public void InsertResource(string table, SharkyWebResource resource)
        {
            var command = database.CreateCommand();
            command.CommandText = @$"INSERT INTO {table}(id, name, uri, icon, creationTime, updateTime, deletionTime) 
                VALUES($id, $name, $uri, $icon, $creationTime, $updateTime, $deletionTime);";
            command.Parameters.AddWithValue("$id", resource.ID);
            command.Parameters.AddWithValue("$name", resource.Name);
            command.Parameters.AddWithValue("$uri", resource.Uri.AbsoluteUri);
            command.Parameters.AddWithValue("$icon", resource.Icon is null ? DBNull.Value : resource.Icon.ToString());
            command.Parameters.AddWithValue("$creationTime", resource.CreationTime);
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
            command.CommandText = @$"UPDATE {table} SET name = $name, uri = $uri, icon = $icon, creationTime = $creationTime, updateTime = $updateTime, deletionTime = $deletionTime WHERE id = $id";
            command.Parameters.AddWithValue("$id", resource.ID);
            command.Parameters.AddWithValue("$name", resource.Name);
            command.Parameters.AddWithValue("$uri", resource.Uri.AbsoluteUri);
            command.Parameters.AddWithValue("$icon", resource.Icon is null ? DBNull.Value : resource.Icon.ToString());
            command.Parameters.AddWithValue("$creationTime", resource.CreationTime);
            command.Parameters.AddWithValue("$updateTime", resource.UpdateTime is null ? DBNull.Value : resource.UpdateTime);
            command.Parameters.AddWithValue("$deletionTime", resource.DeletionTime is null ? DBNull.Value : resource.DeletionTime);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Delete a resource in a table
        /// </summary>
        /// <param name="table">Table name</param>
        /// <param name="id">ID of the resource to delete</param>
        public void DeleteResource(string table, string id)
        {
            var command = database.CreateCommand();
            command.CommandText = @$"UPDATE {table} SET name = NULL, uri = NULL, icon = NULL, deletionTime = $deletionTime WHERE id = $id";
            command.Parameters.AddWithValue("$deletionTime", SharkyUtils.DateTimeToUnixTimestamp(DateTime.Now));
            command.Parameters.AddWithValue("id", id);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Deletes all resources from a table
        /// </summary>
        /// <param name="table">Table name</param>
        public void EmptyResources(string table)
        {
            var command = database.CreateCommand();
            command.CommandText = @$"UPDATE {table} SET name = NULL, uri = NULL, icon = NULL, deletionTime = $deletionTime WHERE deletionTime IS NULL;";
            command.Parameters.AddWithValue("$deletionTime", DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Deletes the database structure.
        /// </summary>
        public void NukeLocalDatabase()
        {
            var command = database.CreateCommand();
            command.CommandText = "DROP TABLE history; DROP TABLE bookmark;";
            command.ExecuteNonQuery();
        }
    }
}
