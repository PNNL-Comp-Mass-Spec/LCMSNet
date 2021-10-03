﻿using System;
using System.Collections.Generic;
using System.Linq;
using LcmsNet.IO.DMS.Data;
using LcmsNetSDK.Data;
using LcmsNetSDK.Logging;

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

        #region Class Variables

        private static readonly List<string> cartNames = new List<string>(0);
        private static readonly Dictionary<string, List<string>> cartConfigNames = new Dictionary<string, List<string>>(0);
        private static readonly List<string> columnNames = new List<string>(0);
        private static readonly List<string> separationNames = new List<string>(0);
        private static readonly List<string> datasetTypeNames = new List<string>(0);
        private static bool firstTimeLookupSelectedSepType = true;

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

        /// <summary>
        /// Wrapper around generic retrieval method specifically for cart lists
        /// </summary>
        /// <returns>List containing cart names</returns>
        public static IEnumerable<string> GetCartNameList()
        {
            return Cache.ReadSingleColumnListFromCache(DatabaseTableTypes.CartList, cartNames, false);
        }

        /// <summary>
        /// Wrapper around generic retrieval method specifically for cart config name lists
        /// </summary>
        /// <param name="force">Force reload of data from cache, rather than using the in-memory copy of it</param>
        /// <returns>Mapping of cart names to possible cart config names</returns>
        public static Dictionary<string, List<string>> GetCartConfigNameMap(bool force)
        {
            var cacheData = cartConfigNames;
            if (cartConfigNames.Count == 0 || force || Cache.AlwaysRead)
            {
                cacheData = new Dictionary<string, List<string>>();

                // Read the data from the cache
                var configList = Cache.ReadMultiColumnDataFromCache(DatabaseTableTypes.CartConfigNameList, () => new CartConfigInfo()).ToList();

                // Transform the data, and allow "unknown" cart configs for all carts
                foreach (var config in configList)
                {
                    if (!cacheData.TryGetValue(config.CartName, out var cartConfigList))
                    {
                        cartConfigList = new List<string>();
                        cacheData.Add(config.CartName, cartConfigList);
                    }
                    cartConfigList.Add(config.CartConfigName);
                }

                // Add the unknown configs
                var unknownConfigs = configList
                    .Where(x => x.CartName.StartsWith("unknown", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(x => x.CartConfigName).Select(x => x.CartConfigName).ToList();

                foreach (var cart in cacheData.Where(x => !x.Key.StartsWith("unknown", StringComparison.OrdinalIgnoreCase)))
                {
                    cart.Value.Sort();
                    cart.Value.AddRange(unknownConfigs);
                }

                // Add all carts without a config with the default unknown configs
                foreach (var cart in GetCartNameList().Where(x => !cacheData.ContainsKey(x)))
                {
                    cacheData.Add(cart, new List<string>(unknownConfigs));
                }

                if (Cache.AlwaysRead)
                {
                    cartConfigNames.Clear();
                }
                else
                {
                    foreach (var cart in cartConfigNames.Keys.ToArray())
                    {
                        if (!cacheData.ContainsKey(cart))
                        {
                            cartConfigNames.Remove(cart);
                        }
                    }

                    foreach (var item in cacheData)
                    {
                        if (cartConfigNames.TryGetValue(item.Key, out var configs))
                        {
                            configs.Clear();
                            configs.AddRange(item.Value);
                        }
                        else
                        {
                            cartConfigNames.Add(item.Key, item.Value.ToList());
                        }
                    }
                }
            }

            return cacheData;
        }

        /// <summary>
        /// Wrapper around generic retrieval method specifically for cart config name lists
        /// </summary>
        /// <param name="force">Force reload of data from cache, rather than using the in-memory copy of it</param>
        /// <returns>List containing cart config names</returns>
        public static IEnumerable<string> GetCartConfigNameList(bool force)
        {
            var configs = cartConfigNames;
            if (cartConfigNames.Count == 0 || force || Cache.AlwaysRead)
            {
                configs = GetCartConfigNameMap(force);
            }

            return configs.Values.SelectMany(x => x).Distinct().OrderBy(x => x);
        }

        /// <summary>
        /// Get the cart config name list for a specific cart
        /// </summary>
        /// <param name="cartName">Cart name</param>
        /// <param name="force">Force reload of data from cache, rather than using the in-memory copy of it</param>
        /// <returns>List containing cart config names</returns>
        public static IEnumerable<string> GetCartConfigNameList(string cartName, bool force)
        {
            var data = GetCartConfigNameMap(force);
            if (data.TryGetValue(cartName, out var configs))
            {
                return configs;
            }

            return data.First(x => x.Key.StartsWith("unknown", StringComparison.OrdinalIgnoreCase)).Value;
        }

        /// <summary>
        /// Wrapper around generic retrieval method specifically for LC column lists
        /// </summary>
        /// <param name="force">Force reload of data from cache, rather than using the in-memory copy of it</param>
        /// <returns>List containing cart names</returns>
        public static IEnumerable<string> GetColumnList(bool force)
        {
            return Cache.ReadSingleColumnListFromCache(DatabaseTableTypes.ColumnList, columnNames, force);
        }

        /// <summary>
        /// Wrapper around generic retrieval method specifically for separation type lists
        /// </summary>
        /// <param name="force">Force reload of data from cache, rather than using the in-memory copy of it</param>
        /// <returns>List containing separation types</returns>
        public static IEnumerable<string> GetSepTypeList(bool force)
        {
            return Cache.ReadSingleColumnListFromCache(DatabaseTableTypes.SeparationTypeList, separationNames, force);
        }

        /// <summary>
        /// Wrapper around generic retrieval method specifically for dataset type lists
        /// </summary>
        /// <param name="force">Force reload of data from cache, rather than using the in-memory copy of it</param>
        /// <returns>List containing dataset types</returns>
        public static IEnumerable<string> GetDatasetTypeList(bool force)
        {
            return Cache.ReadSingleColumnListFromCache(DatabaseTableTypes.DatasetTypeList, datasetTypeNames, force);
        }

        /// <summary>
        /// Retrieves the cached cart configuration name
        /// </summary>
        /// <returns>Cart configuration name</returns>
        public static string GetDefaultCartConfigName()
        {
            List<string> cartConfigNamesList;
            try
            {
                cartConfigNamesList = Cache.ReadSingleColumnListFromCacheCheckExceptions(DatabaseTableTypes.CartConfigNameSelected).ToList();
            }
            catch
            {
                // Table T_CartConfigNameSelected not found
                // This will happen if the default has not yet been saved
                return string.Empty;
            }

            if (cartConfigNamesList.Count < 1)
            {
                return string.Empty;
            }

            return cartConfigNamesList[0];
        }

        /// <summary>
        /// Retrieves the cached separation type
        /// </summary>
        /// <returns>Separation type</returns>
        public static string GetDefaultSeparationType()
        {
            List<string> sepType;
            try
            {
                sepType = Cache.ReadSingleColumnListFromCacheCheckExceptions(DatabaseTableTypes.SeparationTypeSelected).ToList();
            }
            catch (Exception ex)
            {
                if (!firstTimeLookupSelectedSepType)
                {
                    const string errorMessage =
                        "Exception getting default separation type. (NOTE: This is normal if a new cache is being used)";
                    ApplicationLogger.LogError(0, errorMessage, ex);
                }

                firstTimeLookupSelectedSepType = false;
                return string.Empty;
            }

            firstTimeLookupSelectedSepType = false;

            if (sepType.Count < 1)
            {
                return string.Empty;
            }

            return sepType[0];
        }

        #endregion

        #region Public Methods: Cache Writing

        public static void CheckOrCreateCache(SQLiteCacheDefaultData defaultData = null)
        {
            Cache.CheckOrCreateCache(defaultData);
        }

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

        /// <summary>
        /// Saves a list of Cart_Configs (and associated Cart names) to cache
        /// </summary>
        /// <param name="cartConfigList">List containing cart config info.</param>
        public static void SaveCartConfigListToCache(IEnumerable<CartConfigInfo> cartConfigList)
        {
            Cache.SaveMultiColumnListToCache(DatabaseTableTypes.CartConfigNameList, cartConfigList);

            // Reload the in-memory copy of the cached data
            if (!Cache.AlwaysRead)
            {
                GetCartConfigNameMap(true);
            }
        }

        /// <summary>
        /// Saves a list of cart names to the SQLite cache
        /// </summary>
        /// <param name="cartNameList">Cart names</param>
        public static void SaveCartListToCache(IEnumerable<string> cartNameList)
        {
            Cache.SaveSingleColumnListToCache(DatabaseTableTypes.CartList, cartNameList, cartNames);
        }

        /// <summary>
        /// Saves a list of column names to the SQLite cache
        /// </summary>
        /// <param name="columnList">Column names</param>
        public static void SaveColumnListToCache(IEnumerable<string> columnList)
        {
            Cache.SaveSingleColumnListToCache(DatabaseTableTypes.ColumnList, columnList, columnNames);
        }

        /// <summary>
        /// Saves a list of dataset type names to the SQLite cache
        /// </summary>
        /// <param name="datasetTypeList">Dataset type names</param>
        public static void SaveDatasetTypeListToCache(IEnumerable<string> datasetTypeList)
        {
            Cache.SaveSingleColumnListToCache(DatabaseTableTypes.DatasetTypeList, datasetTypeList, datasetTypeNames);
        }

        /// <summary>
        /// Saves a list of separation types to the SQLite cache
        /// </summary>
        /// <param name="separationTypeList">Separation type names</param>
        public static void SaveSeparationTypeListToCache(IEnumerable<string> separationTypeList)
        {
            Cache.SaveSingleColumnListToCache(DatabaseTableTypes.SeparationTypeList, separationTypeList, separationNames);
        }

        /// <summary>
        /// Caches the cart configuration name that is currently selected for this cart
        /// </summary>
        /// <param name="cartConfigName">Cart configuration name</param>
        public static void SaveSelectedCartConfigName(string cartConfigName)
        {
            // Create a list for the Save call to use (it requires a list)
            Cache.SaveSingleColumnListToCache(DatabaseTableTypes.CartConfigNameSelected, new List<string> { cartConfigName });
        }

        /// <summary>
        /// Caches the separation type that is currently selected for this cart
        /// </summary>
        /// <param name="separationType">Separation type</param>
        public static void SaveSelectedSeparationType(string separationType)
        {
            // Create a list for the Save call to use (it requires a list)
            Cache.SaveSingleColumnListToCache(DatabaseTableTypes.SeparationTypeSelected, new List<string> { separationType });
        }

        #endregion
    }
}
