using System;
using System.Collections.Generic;
using LcmsNetSDK.Data;

// ReSharper disable UnusedMember.Global

namespace LcmsNet.IO.SQLite
{
    public static class SQLiteTools
    {
        // Ignore Spelling: configs

        #region Properties

        public static string ConnString => Cache.ConnString;

        public static bool DatabaseImageBad => Cache.DatabaseImageBad;

        public static bool DisableInMemoryCaching
        {
            get => Cache.DisableInMemoryCaching;
            set => Cache.DisableInMemoryCaching = value;
        }

        /// <summary>
        /// Cache file name or path
        /// </summary>
        /// <remarks>Starts off as a filename, but is changed to a path by BuildConnectionString</remarks>
        public static string CacheName => Cache.CacheName;

        #endregion

        #region Initialize

        /// <summary>
        /// Constructor
        /// </summary>
        static SQLiteTools()
        {
            Cache = new SQLiteCacheIO();
        }

        /// <summary>
        /// Initialize the cache, with the provided cache filename
        /// </summary>
        /// <param name="cacheName"></param>
        public static void Initialize(string cacheName = "LCMSCache.que")
        {
            Cache.Initialize(cacheName);
        }

        public static void BuildConnectionString(bool newCache)
        {
            Cache.BuildConnectionString(newCache);
        }

        public static void SetDefaultDirectoryPath(string path)
        {
            Cache.SetDefaultDirectoryPath(path);
        }

        public static void SetDefaultDirectoryPath(Func<string> pathGetMethod)
        {
            Cache.SetDefaultDirectoryPath(pathGetMethod);
        }

        #endregion

        #region Instance

        private static SQLiteCacheIO Cache { get; }

        public static IDisposable GetDisposable()
        {
            return Cache;
        }

        /// <summary>
        /// Close the stored SQLite connection
        /// </summary>
        public static void CloseConnection()
        {
            Cache.CloseConnection();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Delete a cache file that has issues so a good cache can be made it its place.
        /// It is the responsibility of the calling method to ensure no other database operations are occurring that could interfere.
        /// </summary>
        /// <param name="force">If true, deletes the cache regardless of the <see cref="DatabaseImageBad"/> value</param>
        public static void DeleteBadCache(bool force = false)
        {
            Cache.DeleteBadCache(force);
        }

        /// <summary>
        /// Sets the cache location to the path provided
        /// </summary>
        /// <param name="location">New path to location of queue</param>
        /// <remarks>If location is a filename (and not a path), then updates to use AppDataFolderName</remarks>
        public static void SetCacheLocation(string location)
        {
            Cache.SetCacheLocation(location);
        }

        #endregion

        #region Public Methods: Cache Reading

        /// <summary>
        /// Retrieves a sample queue from cache database
        /// Connection string and database name are defined by defaults
        /// </summary>
        /// <param name="tableType">tableType enum specifying type of queue to retrieve</param>
        /// <returns>List containing queue data</returns>
        public static IEnumerable<SampleData> GetQueueFromCache(DatabaseTableTypes tableType)
        {
            return GetQueueFromCache(tableType, ConnString);
        }

        /// <summary>
        /// Retrieves a sample queue from a SQLite database
        /// Overload requires connection string to be specified
        /// </summary>
        /// <param name="tableType">tableType enum specifying type of queue to retrieve</param>
        /// <param name="connectionString">Cache connection string</param>
        /// <returns>List containing queue data</returns>
        public static IEnumerable<SampleData> GetQueueFromCache(DatabaseTableTypes tableType, string connectionString)
        {
            // All finished, so return
            return Cache.ReadMultiColumnDataFromCache(tableType, () => new SampleData(false), connectionString);
        }

        #endregion

        #region Public Methods: Cache Writing

        /// <summary>
        /// Saves the contents of specified sample queue to the SQLite cache file
        /// Connection string and database name are defined by defaults
        /// </summary>
        /// <param name="queueData">List of SampleData containing the sample data to save</param>
        /// <param name="tableType">TableTypes enum specifying which queue is being saved</param>
        public static void SaveQueueToCache(IEnumerable<SampleData> queueData, DatabaseTableTypes tableType)
        {
            SaveQueueToCache(queueData, tableType, ConnString);
        }

        /// <summary>
        /// Saves the contents of specified sample queue to an SQLite database file
        /// Overload requires database connection string be specified
        /// </summary>
        /// <param name="queueData">List containing the sample data to save</param>
        /// <param name="tableType">TableTypes enum specifying which queue is being saved</param>
        /// <param name="connStr">Connection string for database file</param>
        public static void SaveQueueToCache(IEnumerable<SampleData> queueData, DatabaseTableTypes tableType, string connStr)
        {
            Cache.SaveMultiColumnListToCache(tableType, queueData, connStr);
        }

        #endregion
    }
}
