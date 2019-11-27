//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 02/10/2009
//
// Updates
// - 02/12/2009 (DAC) - Added methods for retrieving cached queue
// - 02/19/2009 (DAC) - Incorporated renamed exceptions
// - 02/23/2009 (DAC) - Reworked queue saving to reduce future coding
// - 02/24/2009 (DAC) - Added storage and retrieval of DMS parameters
// - 03/03/2009 (DAC) - Modified constructor to fix form designer issue, added method overloads
//                      for queue ops to specify a database file other than the cache file
// - 03/10/2009 (DAC) - Added function to replace SQLite-incompatible characters
// - 04/01/2009 (DAC) - Added file logging for exceptions
// - 05/18/2010 (DAC) - Added error logging; Modified for queue import/export using SQLite
// - 04/17/2013 (FCT) - Added Proposal Users list with a a cross reference list of their UID to the PIDs of proposals they've worked.
//
//*********************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using LcmsNetData;
using LcmsNetData.Data;
using LcmsNetData.Logging;

namespace LcmsNetSQLiteTools
{
    public class SQLiteTools
    {
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
        private static readonly List<string> datasetNames = new List<string>(0);
        private static readonly Dictionary<string, WorkPackageInfo> workPackageMap = new Dictionary<string, WorkPackageInfo>(0);
        private static readonly List<UserInfo> userInfo = new List<UserInfo>(0);
        private static readonly List<InstrumentInfo> instrumentInfo = new List<InstrumentInfo>(0);
        private static readonly List<ExperimentData> experimentsData = new List<ExperimentData>(0);
        private static readonly List<LCColumnData> lcColumns = new List<LCColumnData>(0);
        private static readonly List<ProposalUser> proposalUsers = new List<ProposalUser>(0);
        private static readonly Dictionary<string, List<UserIDPIDCrossReferenceEntry>> proposalIdIndexedReferenceList = new Dictionary<string, List<UserIDPIDCrossReferenceEntry>>(0);

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
        /// Initialize the cache, with the provided app name and cache filename
        /// </summary>
        /// <param name="appDataFolderName"></param>
        /// <param name="cacheName"></param>
        public static void Initialize(string appDataFolderName = "LCMSNet", string cacheName = "LCMSCache.que")
        {
            Cache.Initialize(appDataFolderName, cacheName);
        }

        public static void BuildConnectionString(bool newCache)
        {
            Cache.BuildConnectionString(newCache);
        }

        #endregion

        #region Instance

        private static SQLiteCacheIO Cache { get; }

        private SQLiteTools()
        {
        }

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

        #region Private Methods

        private static void UpdateProposalIdIndexReferenceList(Dictionary<string, List<UserIDPIDCrossReferenceEntry>> pidIndexedReferenceList)
        {
            if (Cache.AlwaysRead)
            {
                return;
            }

            foreach (var key in proposalIdIndexedReferenceList.Keys.ToArray())
            {
                if (!pidIndexedReferenceList.ContainsKey(key))
                {
                    proposalIdIndexedReferenceList.Remove(key);
                }
            }

            foreach (var item in proposalIdIndexedReferenceList)
            {
                if (proposalIdIndexedReferenceList.TryGetValue(item.Key, out var crossReferenceList))
                {
                    crossReferenceList.Clear();
                    crossReferenceList.AddRange(item.Value);
                }
                else
                {
                    proposalIdIndexedReferenceList.Add(item.Key, item.Value.ToList());
                }
            }
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
        public static List<T> GetQueueFromCache<T>(DatabaseTableTypes tableType) where T : SampleDataBasic, new()
        {
            return GetQueueFromCache<T>(tableType, ConnString);
        }

        /// <summary>
        /// Retrieves a sample queue from a SQLite database
        /// Overload requires connection string to be specified
        /// </summary>
        /// <param name="tableType">tableType enum specifying type of queue to retrieve</param>
        /// <param name="connectionString">Cache connection string</param>
        /// <returns>List containing queue data</returns>
        public static List<T> GetQueueFromCache<T>(DatabaseTableTypes tableType, string connectionString) where T : SampleDataBasic, new()
        {
            if (typeof(T) == typeof(SampleDataBasic))
            {
                ApplicationLogger.LogError(0, "Cannot populate list of SampleDataBasic objects from database!");
                return new List<T>();
            }

            // All finished, so return
            return Cache.ReadMultiColumnDataFromCache(tableType, () => (T)(new T().GetNewNonDummy()), connectionString);
        }

        /// <summary>
        /// Wrapper around generic retrieval method specifically for cart lists
        /// </summary>
        /// <param name="force">Force reload of data from cache, rather than using the in-memory copy of it</param>
        /// <returns>List containing cart names</returns>
        [Obsolete("Use GetCartNameList that does not have parameter force (since force is not used)")]
        public static List<string> GetCartNameList(bool force)
        {
            return GetCartNameList();
        }

        /// <summary>
        /// Wrapper around generic retrieval method specifically for cart lists
        /// </summary>
        /// <returns>List containing cart names</returns>
        public static List<string> GetCartNameList()
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
                var configList = Cache.ReadMultiColumnDataFromCache(DatabaseTableTypes.CartConfigNameList, () => new CartConfigInfo());

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

                // Add the unknown configs last.
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
        public static List<string> GetCartConfigNameList(bool force)
        {
            var configs = cartConfigNames;
            if (cartConfigNames.Count == 0 || force || Cache.AlwaysRead)
            {
                configs = GetCartConfigNameMap(force);
            }

            return configs.Values.SelectMany(x => x).Distinct().OrderBy(x => x).ToList();
        }

        /// <summary>
        /// Get the cart config name list for a specific cart
        /// </summary>
        /// <param name="cartName">Cart name</param>
        /// <param name="force">Force reload of data from cache, rather than using the in-memory copy of it</param>
        /// <returns>List containing cart config names</returns>
        public static List<string> GetCartConfigNameList(string cartName, bool force)
        {
            var data = GetCartConfigNameMap(force);
            if (data.TryGetValue(cartName, out var configs))
            {
                return configs.ToList();
            }

            return data.First(x => x.Key.StartsWith("unknown", StringComparison.OrdinalIgnoreCase)).Value.ToList();
        }

        /// <summary>
        /// Wrapper around generic retrieval method specifically for LC column lists
        /// </summary>
        /// <param name="force">Force reload of data from cache, rather than using the in-memory copy of it</param>
        /// <returns>List containing cart names</returns>
        public static List<string> GetColumnList(bool force)
        {
            return Cache.ReadSingleColumnListFromCache(DatabaseTableTypes.ColumnList, columnNames, force);
        }

        /// <summary>
        /// Wrapper around generic retrieval method specifically for separation type lists
        /// </summary>
        /// <param name="force">Force reload of data from cache, rather than using the in-memory copy of it</param>
        /// <returns>List containing separation types</returns>
        public static List<string> GetSepTypeList(bool force)
        {
            return Cache.ReadSingleColumnListFromCache(DatabaseTableTypes.SeparationTypeList, separationNames, force);
        }

        /// <summary>
        /// Wrapper around generic retrieval method specifically for dataset name lists
        /// </summary>
        /// <returns>List containing separation types</returns>
        public static List<string> GetDatasetList()
        {
            return Cache.ReadSingleColumnListFromCache(DatabaseTableTypes.DatasetList, datasetNames, false);
        }

        /// <summary>
        /// Wrapper around generic retrieval method specifically for dataset type lists
        /// </summary>
        /// <param name="force">Force reload of data from cache, rather than using the in-memory copy of it</param>
        /// <returns>List containing dataset types</returns>
        public static List<string> GetDatasetTypeList(bool force)
        {
            return Cache.ReadSingleColumnListFromCache(DatabaseTableTypes.DatasetTypeList, datasetTypeNames, force);
        }

        /// <summary>
        /// Wrapper around generic retrieval method specifically for Work Package lists
        /// </summary>
        /// <param name="force">Force reload of data from cache, rather than using the in-memory copy of it</param>
        /// <returns>Mapping of Charge Codes to WorkPackageInfo objects</returns>
        public static Dictionary<string, WorkPackageInfo> GetWorkPackageMap(bool force)
        {
            var cacheData = workPackageMap;
            if (workPackageMap.Count == 0 || force || Cache.AlwaysRead)
            {
                // Read the data from the cache
                var workPackages = Cache.ReadMultiColumnDataFromCache(DatabaseTableTypes.WorkPackages, () => new WorkPackageInfo());

                cacheData = new Dictionary<string, WorkPackageInfo>(workPackages.Count);

                // For each row (representing one work package), create a dictionary and/or list entry
                foreach (var wpInfo in workPackages)
                {
                    // Add the work package data object to the full list
                    if (!cacheData.ContainsKey(wpInfo.ChargeCode))
                    {
                        cacheData.Add(wpInfo.ChargeCode, wpInfo);
                    }
                    else
                    {
                        // Collision: probably due to certain types of joint appointments
                        // if username exists in DMS, try to keep the one with the current owner name.
                        var existing = cacheData[wpInfo.ChargeCode];
                        if (existing.OwnerName == null || existing.OwnerName.IndexOf("(old", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            cacheData[wpInfo.ChargeCode] = wpInfo;
                        }
                    }
                }

                workPackageMap.Clear();
                if (!Cache.AlwaysRead)
                {
                    foreach (var item in cacheData)
                    {
                        workPackageMap.Add(item.Key, item.Value);
                    }
                }
            }

            return cacheData;
        }

        /// <summary>
        /// Gets user list from cache
        /// </summary>
        /// <param name="force">Force reload of data from cache, rather than using the in-memory copy of it</param>
        /// <returns>List of user data</returns>
        public static List<UserInfo> GetUserList(bool force)
        {
            return Cache.ReadMultiColumnDataFromCache(DatabaseTableTypes.UserList, () => new UserInfo(), userInfo, force);
        }

        /// <summary>
        /// Gets a list of instruments from the cache
        /// </summary>
        /// <param name="force">Force reload of data from cache, rather than using the in-memory copy of it</param>
        /// <returns>List of instruments</returns>
        public static List<InstrumentInfo> GetInstrumentList(bool force)
        {
            return Cache.ReadMultiColumnDataFromCache(DatabaseTableTypes.InstrumentList, () => new InstrumentInfo(), instrumentInfo, force);
        }

        public static List<ExperimentData> GetExperimentList()
        {
            return Cache.ReadMultiColumnDataFromCache(DatabaseTableTypes.ExperimentList, () => new ExperimentData(), experimentsData, false);
        }

        public static void GetProposalUsers(
            out List<ProposalUser> users,
            out Dictionary<string, List<UserIDPIDCrossReferenceEntry>> pidIndexedReferenceList)
        {
            if (proposalUsers.Count > 0 && proposalIdIndexedReferenceList.Count > 0 && !Cache.AlwaysRead)
            {
                users = proposalUsers;
                pidIndexedReferenceList = proposalIdIndexedReferenceList;
            }
            else
            {
                pidIndexedReferenceList = new Dictionary<string, List<UserIDPIDCrossReferenceEntry>>();

                // Read the data from the cache
                users = Cache.ReadMultiColumnDataFromCache(DatabaseTableTypes.PUserList, () => new ProposalUser(), proposalUsers);
                var crossReferenceList = Cache.ReadMultiColumnDataFromCache(DatabaseTableTypes.PReferenceList, () => new UserIDPIDCrossReferenceEntry());

                foreach (var crossReference in crossReferenceList)
                {
                    if (!pidIndexedReferenceList.ContainsKey(crossReference.PID))
                    {
                        pidIndexedReferenceList.Add(
                            crossReference.PID,
                            new List<UserIDPIDCrossReferenceEntry>());
                    }

                    pidIndexedReferenceList[crossReference.PID].Add(crossReference);
                }

                if (Cache.AlwaysRead)
                {
                    proposalIdIndexedReferenceList.Clear();
                }
                else
                {
                    UpdateProposalIdIndexReferenceList(pidIndexedReferenceList);
                }
            }
        }

        public static List<LCColumnData> GetEntireLCColumnList()
        {
            return Cache.ReadMultiColumnDataFromCache(DatabaseTableTypes.LCColumnList, () => new LCColumnData(), lcColumns, false);
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
                cartConfigNamesList = Cache.ReadSingleColumnListFromCacheCheckExceptions(DatabaseTableTypes.CartConfigNameSelected);
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
                sepType = Cache.ReadSingleColumnListFromCacheCheckExceptions(DatabaseTableTypes.SeparationTypeSelected);
            }
            catch (Exception ex)
            {
                var firstTimeLookupedSelectedSepType = LCMSSettings.GetParameter(LCMSSettings.PARAM_FIRSTTIME_LOOKUP_SELECTED_SEP_TYPE);

                var isFirstTime = true;
                if (!string.IsNullOrWhiteSpace(firstTimeLookupedSelectedSepType))
                {
                    isFirstTime = Convert.ToBoolean(firstTimeLookupedSelectedSepType);
                }

                if (!isFirstTime)
                {
                    const string errorMessage =
                        "Exception getting default separation type. (NOTE: This is normal if a new cache is being used)";
                    ApplicationLogger.LogError(0, errorMessage, ex);
                }
                else
                {
                    LCMSSettings.SetParameter(LCMSSettings.PARAM_FIRSTTIME_LOOKUP_SELECTED_SEP_TYPE, false.ToString());
                }
                return string.Empty;
            }

            if (sepType.Count < 1)
            {
                return string.Empty;
            }

            return sepType[0];
        }

        #endregion

        #region Public Methods: Cache Writing

        /// <summary>
        /// Saves the contents of specified sample queue to the SQLite cache file
        /// Connection string and database name are defined by defaults
        /// </summary>
        /// <param name="queueData">List of SampleData containing the sample data to save</param>
        /// <param name="tableType">TableTypes enum specifying which queue is being saved</param>
        public static void SaveQueueToCache<T>(List<T> queueData, DatabaseTableTypes tableType) where T : SampleDataBasic, new()
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
        public static void SaveQueueToCache<T>(List<T> queueData, DatabaseTableTypes tableType, string connStr) where T : SampleDataBasic, new()
        {
            if (typeof(T) == typeof(SampleDataBasic))
            {
                ApplicationLogger.LogError(0, "Cannot write list of SampleDataBasic objects to database!");
                return;
            }

            Cache.SaveMultiColumnListToCache(tableType, queueData, true, connStr);
        }

        /// <summary>
        /// Saves a list of users to cache
        /// </summary>
        /// <param name="userList">List containing user data</param>
        /// <param name="clearFirst">if true, the existing data will always be removed from the list; if false and <paramref name="userList"/>.Count is &lt;= to the number of existing rows, nothing is changed</param>
        public static void SaveUserListToCache(List<UserInfo> userList, bool clearFirst = true)
        {
            Cache.SaveMultiColumnListToCache(DatabaseTableTypes.UserList, userList, userInfo, clearFirst);
        }

        /// <summary>
        /// Save a list of experiments to cache
        /// </summary>
        /// <param name="expList"></param>
        /// <param name="clearFirst">if true, the existing data will always be removed from the list; if false and <paramref name="expList"/>.Count is &lt;= to the number of existing rows, nothing is changed</param>
        public static void SaveExperimentListToCache(List<ExperimentData> expList, bool clearFirst = true)
        {
            if (expList == null) return;

            Cache.SaveMultiColumnListToCache(DatabaseTableTypes.ExperimentList, expList, experimentsData, clearFirst);
        }

        /// <summary>
        /// Saves the Proposal Users list and a Proposal ID to Proposal User ID cross-reference
        /// list to the cache.
        /// </summary>
        /// <param name="users">A list of the Proposal Users to cache.</param>
        /// <param name="crossReferenceList">A list of cross references to cache.</param>
        /// <param name="pidIndexedReferenceList">
        /// A dictionary of cross reference lists that have been grouped by Proposal ID.
        /// </param>
        /// <param name="clearFirst">if true, the existing data will always be removed from the list; if false and <paramref name="users"/>.Count is &lt;= to the number of existing rows, nothing is changed</param>
        public static void SaveProposalUsers(List<ProposalUser> users,
            List<UserIDPIDCrossReferenceEntry> crossReferenceList,
            Dictionary<string, List<UserIDPIDCrossReferenceEntry>> pidIndexedReferenceList, bool clearFirst = true)
        {
            Cache.SaveMultiColumnListToCache(DatabaseTableTypes.PUserList, users, proposalUsers, clearFirst);
            Cache.SaveMultiColumnListToCache(DatabaseTableTypes.PReferenceList, crossReferenceList, clearFirst);

            if (!Cache.AlwaysRead)
            {
                UpdateProposalIdIndexReferenceList(pidIndexedReferenceList);
            }
        }

        public static void SaveEntireLCColumnListToCache(List<LCColumnData> lcColumnList)
        {
            Cache.SaveMultiColumnListToCache(DatabaseTableTypes.LCColumnList, lcColumnList, lcColumns, true);
        }

        /// <summary>
        /// Saves a list of instruments to cache
        /// </summary>
        /// <param name="instList">List of InstrumentInfo containing instrument data</param>
        /// <param name="clearFirst">if true, the existing data will always be removed from the list; if false and <paramref name="instList"/>.Count is &lt;= to the number of existing rows, nothing is changed</param>
        public static void SaveInstListToCache(List<InstrumentInfo> instList, bool clearFirst = true)
        {
            Cache.SaveMultiColumnListToCache(DatabaseTableTypes.InstrumentList, instList, instrumentInfo, clearFirst);
        }

        /// <summary>
        /// Saves a list of Cart_Configs (and associated Cart names) to cache
        /// </summary>
        /// <param name="cartConfigList">List containing cart config info.</param>
        /// <param name="clearFirst">if true, the existing data will always be removed from the list; if false and <paramref name="cartConfigList"/>.Count is &lt;= to the number of existing rows, nothing is changed</param>
        public static void SaveCartConfigListToCache(List<CartConfigInfo> cartConfigList, bool clearFirst = true)
        {
            Cache.SaveMultiColumnListToCache(DatabaseTableTypes.CartConfigNameList, cartConfigList, clearFirst);

            // Reload the in-memory copy of the cached data
            if (!Cache.AlwaysRead)
            {
                GetCartConfigNameMap(true);
            }
        }

        /// <summary>
        /// Saves a list of WorkPackageInfo objects to cache
        /// </summary>
        /// <param name="workPackageList">List containing work package info.</param>
        /// <param name="clearFirst">if true, the existing data will always be removed from the list; if false and <paramref name="workPackageList"/>.Count is &lt;= to the number of existing rows, nothing is changed</param>
        public static void SaveWorkPackageListToCache(List<WorkPackageInfo> workPackageList, bool clearFirst = true)
        {
            Cache.SaveMultiColumnListToCache(DatabaseTableTypes.WorkPackages, workPackageList, clearFirst);

            // Reload the in-memory copy of the cached data
            if (!Cache.AlwaysRead)
            {
                GetWorkPackageMap(true);
            }
        }

        /// <summary>
        /// Saves a list of cart names to the SQLite cache
        /// </summary>
        /// <param name="cartNameList">Cart names</param>
        /// <param name="clearFirst">if true, the existing data will always be removed from the list; if false and <paramref name="cartNameList"/>.Count is &lt;= to the number of existing rows, nothing is changed</param>
        public static void SaveCartListToCache(List<string> cartNameList, bool clearFirst = true)
        {
            Cache.SaveSingleColumnListToCache(DatabaseTableTypes.CartList, cartNameList, cartNames, clearFirst);
        }

        /// <summary>
        /// Saves a list of column names to the SQLite cache
        /// </summary>
        /// <param name="columnList">Column names</param>
        /// <param name="clearFirst">if true, the existing data will always be removed from the list; if false and <paramref name="columnList"/>.Count is &lt;= to the number of existing rows, nothing is changed</param>
        public static void SaveColumnListToCache(List<string> columnList, bool clearFirst = true)
        {
            Cache.SaveSingleColumnListToCache(DatabaseTableTypes.ColumnList, columnList, columnNames, clearFirst);
        }

        /// <summary>
        /// Saves a list of Dataset names to the SQLite cache
        /// </summary>
        /// <param name="datasetNameList">Dataset names</param>
        /// <param name="clearFirst">if true, the existing data will always be removed from the list; if false and <paramref name="datasetNameList"/>.Count is &lt;= to the number of existing rows, nothing is changed</param>
        public static void SaveDatasetNameListToCache(List<string> datasetNameList, bool clearFirst = true)
        {
            Cache.SaveSingleColumnListToCache(DatabaseTableTypes.DatasetList, datasetNameList, datasetNames, clearFirst);
        }

        /// <summary>
        /// Saves a list of dataset type names to the SQLite cache
        /// </summary>
        /// <param name="datasetTypeList">Dataset type names</param>
        /// <param name="clearFirst">if true, the existing data will always be removed from the list; if false and <paramref name="datasetTypeList"/>.Count is &lt;= to the number of existing rows, nothing is changed</param>
        public static void SaveDatasetTypeListToCache(List<string> datasetTypeList, bool clearFirst = true)
        {
            Cache.SaveSingleColumnListToCache(DatabaseTableTypes.DatasetTypeList, datasetTypeList, datasetTypeNames, clearFirst);
        }

        /// <summary>
        /// Saves a list of separation types to the SQLite cache
        /// </summary>
        /// <param name="separationTypeList">Separation type names</param>
        /// <param name="clearFirst">if true, the existing data will always be removed from the list; if false and <paramref name="separationTypeList"/>.Count is &lt;= to the number of existing rows, nothing is changed</param>
        public static void SaveSeparationTypeListToCache(List<string> separationTypeList, bool clearFirst = true)
        {
            Cache.SaveSingleColumnListToCache(DatabaseTableTypes.SeparationTypeList, separationTypeList, separationNames, clearFirst);
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
