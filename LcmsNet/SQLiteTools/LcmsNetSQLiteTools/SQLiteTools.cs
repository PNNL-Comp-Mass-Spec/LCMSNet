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
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using LcmsNetData;
using LcmsNetData.Data;
using LcmsNetData.Logging;

namespace LcmsNetSQLiteTools
{
    public class SQLiteTools : IDisposable
    {
        #region Properties

        public static string ConnString { get; set; } = "";

        public static bool DatabaseImageBad { get; private set; }

        public static bool DisableInMemoryCaching { get; set; }

        private static bool AlwaysRead => !DatabaseImageBad && DisableInMemoryCaching;

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

        private static string cacheFullPath;

        #endregion

        #region Initialize

        /// <summary>
        /// Constructor
        /// </summary>
        static SQLiteTools()
        {
            Instance = new SQLiteTools();
            Initialize();
        }

        /// <summary>
        /// Initialize the cache, with the provided app name and cache filename
        /// </summary>
        /// <param name="appDataFolderName"></param>
        /// <param name="cacheName"></param>
        public static void Initialize(string appDataFolderName = "LCMSNet", string cacheName = "LCMSCache.que")
        {
            AppDataFolderName = appDataFolderName;
            CacheName = cacheName;

            BuildConnectionString(false);
        }

        /// <summary>
        /// Cache file name or path
        /// </summary>
        /// <remarks>Starts off as a filename, but is changed to a path by BuildConnectionString</remarks>
        public static string CacheName
        {
            get { return LCMSSettings.GetParameter(LCMSSettings.PARAM_CACHEFILENAME); }
            private set { LCMSSettings.SetParameter(LCMSSettings.PARAM_CACHEFILENAME, value); }
        }

        public static string AppDataFolderName { get; set; }

        public static void BuildConnectionString(bool newCache)
        {
            try
            {
                var name = LCMSSettings.GetParameter(LCMSSettings.PARAM_CACHEFILENAME);
                var exists = File.Exists(name);
                if (!exists && !newCache)
                {
                    var appPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    name = Path.Combine(appPath, AppDataFolderName);

                    if (!Directory.Exists(name))
                    {
                        Directory.CreateDirectory(name);
                    }
                    name = Path.Combine(name, CacheName);
                    LCMSSettings.SetParameter(LCMSSettings.PARAM_CACHEFILENAME, name);
                }

                cacheFullPath = name;

                //workaround for SQLite library version 1.0.93 for network addresses
                if (name.Substring(0, 1) == "\\")
                {
                    name = "\\" + name;
                }
                ConnString = "data source=" + name;
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL,
                    "Could not load the sample queue cache.", ex);
            }
        }

        #endregion

        #region Instance

        private static SQLiteTools Instance { get; }

        private SQLiteTools()
        {
        }

        private SQLiteConnection connection = null;
        private string lastConnectionString = "";

        private void Close()
        {
            try
            {
                connection?.Close();
                connection?.Dispose();
                connection = null;
            }
            catch
            {
                // Swallow any exceptions that occurred...
            }
        }

        ~SQLiteTools()
        {
            Dispose();
        }

        public void Dispose()
        {
            Close();
            GC.SuppressFinalize(this);
        }

        public static SQLiteTools GetInstance()
        {
            return Instance;
        }

        /// <summary>
        /// Get a SQLiteConnection, but limit how often we open a new connection
        /// </summary>
        /// <param name="connString"></param>
        /// <returns></returns>
        private static SQLiteConnectionWrapper GetConnection(string connString)
        {
            if (connString.Equals(ConnString))
            {
                if (!Instance.lastConnectionString.Equals(connString))
                {
                    Instance.Close();
                }

                if (Instance.connection == null)
                {
                    Instance.lastConnectionString = connString;
                    Instance.connection = new SQLiteConnection(connString).OpenAndReturn();
                }

                return new SQLiteConnectionWrapper(Instance.connection);
            }

            return new SQLiteConnectionWrapper(connString);
        }

        /// <summary>
        /// Close the stored SQLite connection
        /// </summary>
        public static void CloseConnection()
        {
            Instance.Close();
        }

        /// <summary>
        /// A SQLiteConnection wrapper that only disposes in certain circumstances
        /// </summary>
        private class SQLiteConnectionWrapper : IDisposable
        {
            private readonly SQLiteConnection connection;
            private readonly bool closeConnectionOnDispose = true;

            /// <summary>
            /// Open a new connection, which will get closed on Dispose().
            /// </summary>
            /// <param name="connString"></param>
            public SQLiteConnectionWrapper(string connString)
            {
                connection = new SQLiteConnection(connString).OpenAndReturn();
                closeConnectionOnDispose = true;
            }

            /// <summary>
            /// Wrap an existing connection, which will stay open on Dispose().
            /// </summary>
            /// <param name="existingConnection"></param>
            public SQLiteConnectionWrapper(SQLiteConnection existingConnection)
            {
                connection = existingConnection;
                closeConnectionOnDispose = false;
            }

            public SQLiteCommand CreateCommand()
            {
                return connection.CreateCommand();
            }

            public SQLiteTransaction BeginTransaction()
            {
                return connection.BeginTransaction();
            }

            ~SQLiteConnectionWrapper()
            {
                Dispose();
            }

            public void Dispose()
            {
                if (!closeConnectionOnDispose)
                {
                    return;
                }

                connection?.Close();
                connection?.Dispose();
                GC.SuppressFinalize(this);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete a cache file that has issues so a good cache can be made it its place.
        /// It is the responsibility of the calling method to ensure no other database operations are occurring that could interfere.
        /// </summary>
        /// <param name="force">If true, deletes the cache regardless of the <see cref="DatabaseImageBad"/> value</param>
        public static void DeleteBadCache(bool force = false)
        {
            if (DatabaseImageBad || force)
            {
                // close down existing connections
                CloseConnection();

                try
                {
                    if (File.Exists(cacheFullPath))
                    {
                        File.Delete(cacheFullPath);
                    }

                    DatabaseImageBad = false;
                }
                catch (Exception ex)
                {
                    ApplicationLogger.LogError(0, "Could not delete the SQLite database!", ex);
                }
            }
        }

        private static void CheckExceptionMessageForDbState(Exception ex)
        {
            while (ex != null)
            {
                if (!string.IsNullOrWhiteSpace(ex.Message) &&
                    (ex.Message.IndexOf("malformed", StringComparison.OrdinalIgnoreCase) > -1 ||
                     ex.Message.IndexOf("corrupted", StringComparison.OrdinalIgnoreCase) > -1))
                {
                    DatabaseImageBad = true;
                    break;
                }

                ex = ex.InnerException;
            }
        }

        /// <summary>
        /// Sets the cache location to the path provided
        /// </summary>
        /// <param name="location">New path to location of queue</param>
        /// <remarks>If location is a filename (and not a path), then updates to use AppDataFolderName</remarks>
        public static void SetCacheLocation(string location)
        {
            if (!location.Contains(@"\"))
            {
                var fileName = Path.GetFileName(location);
                var appPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                location = Path.Combine(appPath, AppDataFolderName, fileName);
            }

            CacheName = location;

            BuildConnectionString(!File.Exists(location));
        }

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

            // Convert type of queue into a data table name
            var tableName = GetTableName(tableType);

            // Get a list of string dictionaries containing properties for each sample
            var allSampleProps = GetPropertiesFromCache(tableName, connectionString);

            var returnData = new List<T>(allSampleProps.Count);

            // For each row (representing one sample), create a sample data object
            //      and load the property values
            foreach (var sampleProps in allSampleProps)
            {
                // Create a SampleData object
                var sampleData = (T)(new T().GetNewNonDummy());

                // Load the sample data object from the string dictionary
                sampleData.LoadPropertyValues(sampleProps);

                // Add the sample data object to the return list
                returnData.Add(sampleData);
            }

            // All finished, so return
            return returnData;
        }

        /// <summary>
        /// Gets a list of string dictionary objects containing properties for each item in the cache
        /// </summary>
        /// <param name="tableName">Name of table containing the properties</param>
        /// <param name="connStr">Connection string</param>
        /// <returns>List with properties for each item in cache</returns>
        private static List<Dictionary<string, string>> GetPropertiesFromCache(string tableName, string connStr)
        {
            var returnData = new List<Dictionary<string, string>>();

            // Verify table exists in database
            if (!VerifyTableExists(tableName, connStr))
            {
                // No table, so return an empty list
                return returnData;
            }

            // Get table containing cached data
            var sqlStr = "SELECT * FROM " + tableName;
            var cacheData = GetSQLiteDataTable(sqlStr, connStr);
            if (cacheData.Rows.Count < 1)
            {
                // No cached data found, so return an empty list
                return returnData;
            }

            // For each row (representing properties and values for one sample), create a string dictionary
            //          with the object's properties from the table columns, and add it to the return list
            foreach (DataRow currentRow in cacheData.Rows)
            {
                // Create a string dictionary containing the properties and values for this sample
                var sampleProps = GetPropertyDictionaryForSample(currentRow, cacheData.Columns);

                // Add the string dictionary to the return list
                returnData.Add(sampleProps);
            }

            // Return the list
            return returnData;
        }

        /// <summary>
        /// Gets a string dictionary containing property names and values contained in
        /// a row of the cache data table
        /// </summary>
        /// <param name="RowOfValues">DataRow containing property values from table</param>
        /// <param name="TableColumns">Collection of data columns in table</param>
        /// <returns>Dictionary in property name, property value format</returns>
        private static Dictionary<string, string> GetPropertyDictionaryForSample(DataRow RowOfValues,
            DataColumnCollection TableColumns)
        {
            var returnDict = new Dictionary<string, string>();

            // Build the string dictionary
            foreach (DataColumn column in TableColumns)
            {
                var colName = column.ColumnName;
                var colData = (string) RowOfValues[TableColumns[colName]];
                returnDict.Add(colName, colData);
            }

            // Return the dictionary
            return returnDict;
        }

        /// <summary>
        /// Determines if a particular table exists in the SQLite database
        /// </summary>
        /// <param name="tableName">Name of the table to search for</param>
        /// <param name="connStr">Connection string for database</param>
        /// <returns>TRUE if table found; FALSE if not found or error</returns>
        private static bool VerifyTableExists(string tableName, string connStr)
        {
            return VerifyTableExists(tableName, connStr, out _);
        }

        /// <summary>
        /// Determines if a particular table exists in the SQLite database
        /// </summary>
        /// <param name="tableName">Name of the table to search for</param>
        /// <param name="connStr">Connection string for database</param>
        /// <param name="columnCount">Number of columns in the table</param>
        /// <returns>TRUE if table found; FALSE if not found or error</returns>
        private static bool VerifyTableExists(string tableName, string connStr, out int columnCount)
        {
            return VerifyTableExists(tableName, connStr, out columnCount, out _, false);
        }

        /// <summary>
        /// Determines if a particular table exists in the SQLite database
        /// </summary>
        /// <param name="tableName">Name of the table to search for</param>
        /// <param name="connStr">Connection string for database</param>
        /// <param name="columnCount">Number of columns in the table</param>
        /// <param name="rowCount">Number of rows in the table</param>
        /// <param name="getRowCount">If false, row count is skipped</param>
        /// <returns>TRUE if table found; FALSE if not found or error</returns>
        private static bool VerifyTableExists(string tableName, string connStr, out int columnCount, out int rowCount, bool getRowCount = true)
        {
            columnCount = 0;
            rowCount = 0;

            var sqlString = "SELECT * FROM sqlite_master WHERE name ='" + tableName + "'";
            DataTable resultSet1;
            try
            {
                // Get a list of database tables matching the specified table name
                resultSet1 = GetSQLiteDataTable(sqlString, connStr);
            }
            catch (Exception ex)
            {
                CheckExceptionMessageForDbState(ex);
                var errMsg = "SQLite exception verifying table " + tableName + " exists";
                // throw new DatabaseDataException(errMsg, ex);
                ApplicationLogger.LogError(0, errMsg, ex);
                return false;
            }

            if (resultSet1.Rows.Count < 1)
            {
                return false;
            }

            // Exactly 1 row returned; examine the number of columns
            // Count the number of columns
            var colCountSql = "pragma table_info(" + tableName + ")";

            try
            {
                // Use the pragma statement to get a table with one row per column
                var resultSet2 = GetSQLiteDataTable(colCountSql, connStr);

                columnCount = resultSet2.Rows.Count;
            }
            catch (Exception ex)
            {
                CheckExceptionMessageForDbState(ex);
                var errMsg = "SQLite exception counting columns in table " + tableName;
                ApplicationLogger.LogError(0, errMsg, ex);
                columnCount = 0;
            }

            if (!getRowCount)
            {
                return true;
            }

            // Count the number of rows
            var rowCountSql = $"SELECT COUNT(*) FROM {tableName}";

            try
            {
                // Use the pragma statement to get a table with one row per column
                var resultSet3 = GetSQLiteDataTable(rowCountSql, connStr);

                rowCount = Convert.ToInt32(resultSet3.Rows[0][0]);
            }
            catch (Exception ex)
            {
                CheckExceptionMessageForDbState(ex);
                var errMsg = "SQLite exception counting rows in table " + tableName;
                ApplicationLogger.LogError(0, errMsg, ex);
                rowCount = 0;
            }

            return true;
        }

        /// <summary>
        /// Check if the table has the correct column count and names
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="connStr"></param>
        /// <param name="fieldNames"></param>
        /// <returns></returns>
        private static bool TableColumnNamesMatched(string tableName, string connStr, Dictionary<string, string> fieldNames)
        {
            if (!VerifyTableExists(tableName, connStr, out var columnCount))
            {
                return false;
            }

            if (columnCount != fieldNames.Count)
            {
                return false;
            }

            var sqlString = "pragma table_info(" + tableName + ")";
            DataTable resultSet1;
            try
            {
                // Get a list of database tables matching the specified table name
                resultSet1 = GetSQLiteDataTable(sqlString, connStr);
            }
            catch (Exception ex)
            {
                CheckExceptionMessageForDbState(ex);
                var errMsg = "SQLite exception verifying table " + tableName + " exists";
                // throw new DatabaseDataException(errMsg, ex);
                ApplicationLogger.LogError(0, errMsg, ex);
                return false;
            }

            if (resultSet1.Rows.Count < 1)
            {
                return false;
            }

            var namesList = new List<string>();
            foreach (DataRow currentRow in resultSet1.Rows)
            {
                foreach (DataColumn column in resultSet1.Columns)
                {
                    var colName = column.ColumnName;
                    if (colName.ToLower().Equals("name"))
                    {
                        // row that contains the column name
                        var colData = (string)currentRow[resultSet1.Columns[colName]];
                        namesList.Add(colData);
                    }
                }
            }

            foreach (var name in fieldNames.Keys)
            {
                if (!namesList.Contains(name))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Saves a list of properties for an object to the cache database
        /// </summary>
        /// <param name="dataToCache">List of ICacheInterface objects to save properites for</param>
        /// <param name="tableName">Name of the table to save data in</param>
        /// <param name="connStr">Connection string</param>
        /// <param name="insertsIncludeFieldNames"></param>
        private static void SavePropertiesToCache(
            IList<ICacheInterface> dataToCache, string tableName, string connStr, bool insertsIncludeFieldNames)
        {
            var dataExists = (dataToCache.Count > 0);

            // If there is no data, then just exit
            if (!dataExists)
            {
                return;
            }

            // Create a string dictionary holding the property names and values for object,
            //      using the first object in the input list
            var firstItem = dataToCache[0];
            var fieldNames = firstItem.GetPropertyValues();

            // Verify table exists, and column names are correct; if not, create it; Otherwise, clear it
            var tableExists = VerifyTableExists(tableName, connStr);
            var tableColumnsCorrect = !tableExists || TableColumnNamesMatched(tableName, connStr, fieldNames); // if the table doesn't exist, automatically set this to true
            if (!tableExists || !tableColumnsCorrect)
            {
                try
                {
                    if (!tableColumnsCorrect)
                    {
                        // table column names mismatched, drop the table so we can re-create it
                        var sqlStr = "DROP TABLE " + tableName;
                        ExecuteSQLiteCommand(sqlStr, connStr);
                    }

                    // Table doesn't exist, so create it
                    var sqlCmd = BuildCreatePropTableCmd(fieldNames, tableName);
                    ExecuteSQLiteCommand(sqlCmd, connStr);
                }
                catch (Exception ex)
                {
                    CheckExceptionMessageForDbState(ex);
                    var errMsg = "SQLite exception creating table " + tableName;
                    ApplicationLogger.LogError(0, errMsg, ex);
                    return;
                }
            }

            // Copy the field data to the data table
            var cmdList = new List<string>();
            foreach (var tempItem in dataToCache)
            {
                var itemProps = tempItem.GetPropertyValues();
                var sqlInsertCmd = BuildInsertPropValueCmd(itemProps, tableName, insertsIncludeFieldNames);
                cmdList.Add(sqlInsertCmd);
            }

            StoreCmdListData(connStr, tableName, cmdList);
        }

        private static void UpdateProposalIdIndexReferenceList(Dictionary<string, List<UserIDPIDCrossReferenceEntry>> pidIndexedReferenceList)
        {
            if (AlwaysRead)
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

            var dataInList = (queueData.Count > 0);
            var tableName = GetTableName(tableType);

            // Clear the cache table
            ClearCacheTable(tableName, connStr);

            //If no data in list, just exit
            if (!dataInList)
            {
                return;
            }

            // Convert input data for caching and call cache routine
            var dataList = new List<ICacheInterface>();
            foreach (var currentSample in queueData)
            {
                dataList.Add(currentSample);
            }
            SavePropertiesToCache(dataList, tableName, connStr, true);
        }

        /// <summary>
        /// Saves a list of users to cache
        /// </summary>
        /// <param name="userList">List containing user data</param>
        /// <param name="clearFirst">if true, the existing data will always be removed from the list; if false and <paramref name="userList"/>.Count is &lt;= to the number of existing rows, nothing is changed</param>
        public static void SaveUserListToCache(List<UserInfo> userList, bool clearFirst = true)
        {
            var dataInList = (userList.Count > 0);
            var tableName = GetTableName(DatabaseTableTypes.UserList);

            if (VerifyTableExists(tableName, ConnString, out _, out int rowCount, true) && !clearFirst && userList.Count <= rowCount)
            {
                return;
            }

            // Clear the cache table
            ClearCacheTable(tableName, ConnString);

            if (!AlwaysRead)
            {
                userInfo.Clear();
                userInfo.AddRange(userList);
            }

            //If no data in list, exit
            if (!dataInList)
            {
                return;
            }

            // Convert input data for caching and call cache routine
            var dataList = new List<ICacheInterface>();
            foreach (var currentUser in userList)
            {
                dataList.Add(currentUser);
            }
            SavePropertiesToCache(dataList, tableName, ConnString, true); // Force true, or suffer the random consequences...
        }

        /// <summary>
        /// Save a list of experiments to cache
        /// </summary>
        /// <param name="expList"></param>
        /// <param name="clearFirst">if true, the existing data will always be removed from the list; if false and <paramref name="expList"/>.Count is &lt;= to the number of existing rows, nothing is changed</param>
        public static void SaveExperimentListToCache(List<ExperimentData> expList, bool clearFirst = true)
        {
            if (expList == null || expList.Count < 1)
                return;

            var listHasData = expList.Count != 0;
            var tableName = GetTableName(DatabaseTableTypes.ExperimentList);

            if (VerifyTableExists(tableName, ConnString, out _, out int rowCount, true) && !clearFirst && expList.Count <= rowCount)
            {
                return;
            }

            // Clear the cache table
            ClearCacheTable(tableName, ConnString);

            if (!AlwaysRead)
            {
                experimentsData.Clear();
                experimentsData.AddRange(expList);
            }

            // Exit if there's nothing to cache
            if (!listHasData)
                return;

            // Convert input data for caching and call cache routine
            try
            {
                using (var connection = GetConnection(ConnString))
                using (var transaction = connection.BeginTransaction())
                using (var command = connection.CreateCommand())
                {
                    if (!VerifyTableExists(tableName, ConnString))
                    {
                        command.CommandText = "CREATE TABLE T_ExperimentList ('Created', 'Experiment', 'ID', 'Organism', 'Reason', 'Request', 'Researcher')";
                        command.ExecuteNonQuery();
                    }

                    foreach (var datum in expList)
                    {
                        var commandText =
                            string.Format(
                                "INSERT INTO T_ExperimentList ('ID', 'Organism', 'Researcher', 'Reason', 'Request', 'Experiment', 'Created') " +
                                "VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}')",
                                datum.ID,
                                datum.Organism,
                                datum.Researcher,
                                datum.Reason?.Replace("'", "") ?? "",
                                datum.Request,
                                datum.Experiment,
                                datum.Created ?? DateTime.MinValue);
                        command.CommandText = commandText;
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                CheckExceptionMessageForDbState(ex);
                ApplicationLogger.LogError(0,
                    string.Format("Could not insert all of the experiment data into the experiment table. {0}",
                        ex.Message));
            }
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
            var userTableName = GetTableName(DatabaseTableTypes.PUserList);
            var referenceTableName = GetTableName(DatabaseTableTypes.PReferenceList);

            if (VerifyTableExists(userTableName, ConnString, out _, out int rowCount, true) && !clearFirst && users.Count <= rowCount)
            {
                return;
            }

            ClearCacheTable(userTableName, ConnString);
            ClearCacheTable(referenceTableName, ConnString);

            var userCacheList = new List<ICacheInterface>();
            var referenceCacheList = new List<ICacheInterface>();

            userCacheList.AddRange(users);
            referenceCacheList.AddRange(crossReferenceList);

            SavePropertiesToCache(userCacheList, userTableName, ConnString, true); // Force true, or suffer the random consequences...

            SavePropertiesToCache(referenceCacheList, referenceTableName, ConnString, false); // Single column, column names don't matter...

            if (!AlwaysRead)
            {
                proposalUsers.Clear();
                proposalUsers.AddRange(users);

                UpdateProposalIdIndexReferenceList(pidIndexedReferenceList);
            }
        }

        public static void SaveEntireLCColumnListToCache(List<LCColumnData> lcColumnList)
        {
            var listHasData = lcColumnList.Count != 0;
            var tableName = GetTableName(DatabaseTableTypes.LCColumnList);

            // Clear the cache table
            ClearCacheTable(tableName, ConnString);

            if (!AlwaysRead)
            {
                lcColumns.Clear();
                lcColumns.AddRange(lcColumnList);
            }
            // Exit if there's nothing to cache
            if (!listHasData)
                return;

            // Convert input data for caching and call cache routine
            var dataList = new List<ICacheInterface>();
            foreach (var datum in lcColumnList)
                dataList.Add(datum);

            SavePropertiesToCache(dataList, tableName, ConnString, false);
        }

        /// <summary>
        /// Saves a list of instruments to cache
        /// </summary>
        /// <param name="instList">List of InstrumentInfo containing instrument data</param>
        /// <param name="clearFirst">if true, the existing data will always be removed from the list; if false and <paramref name="instList"/>.Count is &lt;= to the number of existing rows, nothing is changed</param>
        public static void SaveInstListToCache(List<InstrumentInfo> instList, bool clearFirst = true)
        {
            var dataInList = (instList.Count > 0);
            var tableName = GetTableName(DatabaseTableTypes.InstrumentList);

            if (VerifyTableExists(tableName, ConnString, out _, out int rowCount, true) && !clearFirst && instList.Count <= rowCount)
            {
                return;
            }

            // Clear the cache table
            ClearCacheTable(tableName, ConnString, 6);

            if (!AlwaysRead)
            {
                instrumentInfo.Clear();
                instrumentInfo.AddRange(instList);
            }
            //If no data in list, just exit
            if (!dataInList)
            {
                return;
            }

            // Convert input data for caching and call cache routine
            var dataList = new List<ICacheInterface>();
            foreach (var currentInst in instList)
            {
                dataList.Add(currentInst);
            }

            SavePropertiesToCache(dataList, tableName, ConnString, true); // Force true, or suffer the random consequences...
        }

        /// <summary>
        /// Saves a list of Cart_Configs (and associated Cart names) to cache
        /// </summary>
        /// <param name="cartConfigList">List containing cart config info.</param>
        /// <param name="clearFirst">if true, the existing data will always be removed from the list; if false and <paramref name="cartConfigList"/>.Count is &lt;= to the number of existing rows, nothing is changed</param>
        public static void SaveCartConfigListToCache(List<CartConfigInfo> cartConfigList, bool clearFirst = true)
        {
            var dataInList = (cartConfigList.Count > 0);
            var tableName = GetTableName(DatabaseTableTypes.CartConfigNameList);

            if (VerifyTableExists(tableName, ConnString, out _, out int rowCount, true) && !clearFirst && cartConfigList.Count <= rowCount)
            {
                return;
            }

            // Clear the cache table
            ClearCacheTable(tableName, ConnString);

            //If no data in list, exit
            if (!dataInList)
            {
                return;
            }

            // Convert input data for caching and call cache routine
            var dataList = new List<ICacheInterface>();
            foreach (var currentConfig in cartConfigList)
            {
                dataList.Add(currentConfig);
            }

            SavePropertiesToCache(dataList, tableName, ConnString, true); // Force true, or suffer the random consequences...

            // Reload the in-memory copy of the cached data
            GetCartConfigNameMap(true);
        }

        /// <summary>
        /// Saves a list of WorkPackageInfo objects to cache
        /// </summary>
        /// <param name="workPackageList">List containing work package info.</param>
        /// <param name="clearFirst">if true, the existing data will always be removed from the list; if false and <paramref name="workPackageList"/>.Count is &lt;= to the number of existing rows, nothing is changed</param>
        public static void SaveWorkPackageListToCache(List<WorkPackageInfo> workPackageList, bool clearFirst = true)
        {
            var dataInList = (workPackageList.Count > 0);
            var tableName = GetTableName(DatabaseTableTypes.WorkPackages);

            if (VerifyTableExists(tableName, ConnString, out _, out int rowCount, true) && !clearFirst && workPackageList.Count <= rowCount)
            {
                return;
            }

            // Clear the cache table
            ClearCacheTable(tableName, ConnString);

            //If no data in list, exit
            if (!dataInList)
            {
                return;
            }

            // Convert input data for caching and call cache routine
            var dataList = new List<ICacheInterface>();
            foreach (var currentConfig in workPackageList)
            {
                dataList.Add(currentConfig);
            }

            SavePropertiesToCache(dataList, tableName, ConnString, true); // Force true, or suffer the random consequences...

            // Reload the in-memory copy of the cached data
            GetWorkPackageMap(true);
        }

        /// <summary>
        /// Saves a list of cart names to the SQLite cache
        /// </summary>
        /// <param name="cartNameList">Cart names</param>
        /// <param name="clearFirst">if true, the existing data will always be removed from the list; if false and <paramref name="cartNameList"/>.Count is &lt;= to the number of existing rows, nothing is changed</param>
        public static void SaveCartListToCache(List<string> cartNameList, bool clearFirst = true)
        {
            // Refresh the in-memory list with the new data
            if (!AlwaysRead)
            {
                cartNames.Clear();
                cartNames.AddRange(cartNameList);
            }

            SaveSingleColumnListToCache(cartNameList, DatabaseTableTypes.CartList, clearFirst);
        }

        /// <summary>
        /// Saves a list of column names to the SQLite cache
        /// </summary>
        /// <param name="columnList">Column names</param>
        /// <param name="clearFirst">if true, the existing data will always be removed from the list; if false and <paramref name="columnList"/>.Count is &lt;= to the number of existing rows, nothing is changed</param>
        public static void SaveColumnListToCache(List<string> columnList, bool clearFirst = true)
        {
            // Refresh the in-memory list with the new data
            if (!AlwaysRead)
            {
                columnNames.Clear();
                columnNames.AddRange(columnList);
            }

            SaveSingleColumnListToCache(columnList, DatabaseTableTypes.ColumnList, clearFirst);
        }

        /// <summary>
        /// Saves a list of Dataset names to the SQLite cache
        /// </summary>
        /// <param name="datasetNameList">Dataset names</param>
        /// <param name="clearFirst">if true, the existing data will always be removed from the list; if false and <paramref name="datasetNameList"/>.Count is &lt;= to the number of existing rows, nothing is changed</param>
        public static void SaveDatasetNameListToCache(List<string> datasetNameList, bool clearFirst = true)
        {
            // Refresh the in-memory list with the new data
            if (!AlwaysRead)
            {
                datasetNames.Clear();
                datasetNames.AddRange(datasetNameList);
            }

            SaveSingleColumnListToCache(datasetNameList, DatabaseTableTypes.DatasetList, clearFirst);
        }

        /// <summary>
        /// Saves a list of dataset type names to the SQLite cache
        /// </summary>
        /// <param name="datasetTypeList">Dataset type names</param>
        /// <param name="clearFirst">if true, the existing data will always be removed from the list; if false and <paramref name="datasetTypeList"/>.Count is &lt;= to the number of existing rows, nothing is changed</param>
        public static void SaveDatasetTypeListToCache(List<string> datasetTypeList, bool clearFirst = true)
        {
            // Refresh the in-memory list with the new data
            if (!AlwaysRead)
            {
                datasetTypeNames.Clear();
                datasetTypeNames.AddRange(datasetTypeList);
            }

            SaveSingleColumnListToCache(datasetTypeList, DatabaseTableTypes.DatasetTypeList, clearFirst);
        }

        /// <summary>
        /// Saves a list of separation types to the SQLite cache
        /// </summary>
        /// <param name="separationTypeList">Separation type names</param>
        /// <param name="clearFirst">if true, the existing data will always be removed from the list; if false and <paramref name="separationTypeList"/>.Count is &lt;= to the number of existing rows, nothing is changed</param>
        public static void SaveSeparationTypeListToCache(List<string> separationTypeList, bool clearFirst = true)
        {
            // Refresh the in-memory list with the new data
            if (!AlwaysRead)
            {
                separationNames.Clear();
                separationNames.AddRange(separationTypeList);
            }

            SaveSingleColumnListToCache(separationTypeList, DatabaseTableTypes.SeparationTypeList, clearFirst);
        }

        /// <summary>
        /// Executes specified SQLite command
        /// </summary>
        /// <param name="cmdStr">SQL statement to execute</param>
        /// <param name="connStr">Connection string for SQL database file</param>
        private static void ExecuteSQLiteCommand(string cmdStr, string connStr)
        {
            using (var cn = GetConnection(connStr))
            using (var myCmd = cn.CreateCommand())
            {
                myCmd.CommandType = CommandType.Text;
                myCmd.CommandText = cmdStr;
                try
                {
                    var affectedRows = myCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    CheckExceptionMessageForDbState(ex);
                    var errMsg = "SQLite Exception executing command " + cmdStr;
                    ApplicationLogger.LogError(0, errMsg, ex);
                    throw new DatabaseDataException(errMsg, ex);
                }
            }
        }

        /// <summary>
        /// Executes a collection of SQL commands wrapped in a transaction to improve performance
        /// </summary>
        /// <param name="cmdList">List containing the commands to execute</param>
        /// <param name="connStr">Connection string</param>
        private static void ExecuteSQLiteCmdsWithTransaction(IEnumerable<string> cmdList, string connStr)
        {
            using (var connection = GetConnection(connStr))
            using (var command = connection.CreateCommand())
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    command.CommandType = CommandType.Text;
                    // Turn off journal, which speeds up transaction
                    command.CommandText = "PRAGMA journal_mode = OFF";
                    command.ExecuteNonQuery();

                    // Send each of the commands
                    foreach (var currCmd in cmdList)
                    {
                        command.CommandText = currCmd;
                        command.ExecuteNonQuery();
                    }

                    // End transaction
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    CheckExceptionMessageForDbState(ex);
                    const string errMsg = "SQLite exception adding data";
                    ApplicationLogger.LogError(0, errMsg, ex);
                    throw new DatabaseDataException(errMsg, ex);
                }
            }
        }

        /// <summary>
        /// Retrieves a data table from a SQLite database
        /// </summary>
        /// <param name="cmdStr">SQL command to execute</param>
        /// <param name="connStr">Connection string for SQLite database file</param>
        /// <returns>A DataTable containing data specfied by CmdStr</returns>
        private static DataTable GetSQLiteDataTable(string cmdStr, string connStr)
        {
            var returnTable = new DataTable();
            using (var connection = GetConnection(connStr))
            using (var dataAdapter = new SQLiteDataAdapter())
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = cmdStr;
                dataAdapter.SelectCommand = command;

                try
                {
                    var rowCount = dataAdapter.Fill(returnTable);
                }
                catch (Exception ex)
                {
                    CheckExceptionMessageForDbState(ex);
                    var errMsg = "SQLite exception getting data table via query " + cmdStr + " : " + connStr;
                    ApplicationLogger.LogError(0, errMsg, ex);
                    throw new DatabaseDataException(errMsg, ex);
                }
            }

            // Everything worked, so return the table
            return returnTable;
        }

        /// <summary>
        /// Replaces characters in a string that are incompatible with SQLite
        /// </summary>
        /// <param name="InpString">String to clean</param>
        /// <returns>String compatible with SQLite</returns>
        private static string ScrubField(string InpString)
        {
            // Check for empty string
            if (InpString == "")
            {
                return InpString;
            }

            // Escape single quotes
            return InpString.Replace("'", "''");
        }

        /// <summary>
        /// Builds a INSERT command from the input string dictionary
        /// </summary>
        /// <param name="inpData">String dictionary containing property names and values</param>
        /// <param name="tableName">Name of table to insert values into</param>
        /// <param name="insertsIncludeFieldNames">When true, use the key names in inpData as the field names</param>
        /// <returns>String consisting of a complete INSERT SQL statement</returns>
        /// <remarks>Set insertsIncludeFieldNames to true when the data in inpData does not match all of the columns in the target table</remarks>
        private static string BuildInsertPropValueCmd(Dictionary<string, string> inpData, string tableName, bool insertsIncludeFieldNames)
        {
            var sb = new StringBuilder();
            sb.Append("INSERT INTO ");
            sb.Append(tableName);

            if (insertsIncludeFieldNames)
            {
                // Field names in SQLite are delimited by double quotes
                var fieldNames = (from item in inpData.Keys select "\"" + item + "\"");
                sb.Append("(");
                sb.Append(string.Join(",", fieldNames));
                sb.Append(")");
            }

            sb.Append(" VALUES(");

            // Add the property values to the string
            // String values in SQLite are delimited by single quotes
            var valueData = (from item in inpData select "'" + ScrubField(item.Value) + "'");
            sb.Append(string.Join(",", valueData));

            // Terminate the string and return
            sb.Append(")");
            return sb.ToString();
        }

        /// <summary>
        /// Builds a CREATE TABLE command from the input string dictionary
        /// </summary>
        /// <param name="inpData">String dictionary containing property names and values</param>
        /// <param name="tableName">Name of table to create</param>
        /// <returns>String consisting of a complete CREATE TABLE SQL statement</returns>
        private static string BuildCreatePropTableCmd(Dictionary<string, string> inpData, string tableName)
        {
            var sb = new StringBuilder();
            sb.Append("CREATE TABLE ");
            sb.Append(tableName + "(");

            // Create column names for each key, which is same as property name in queue being saved
            var query = (from item in inpData.Keys select "'" + item + "'");
            sb.Append(string.Join(",", query));

            // Terminate the string and return
            sb.Append(")");
            return sb.ToString();
        }

        /// <summary>
        /// Converts a type of table to the corresponding cache db table name
        /// </summary>
        /// <param name="tableType">DatabaseTableTypes specifying table to get name for</param>
        /// <returns>Name of db table</returns>
        private static string GetTableName(DatabaseTableTypes tableType)
        {
            return "T_" + Enum.GetName(typeof (DatabaseTableTypes), tableType);
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
            var carts = cartNames;
            if (cartNames.Count == 0 || AlwaysRead)
            {
                carts = GetSingleColumnListFromCache(DatabaseTableTypes.CartList);
                cartNames.Clear();

                if (AlwaysRead)
                {
                    cartNames.Capacity = 0;
                }
                else
                {
                    cartNames.AddRange(carts);
                }
            }
            return carts;
        }

        /// <summary>
        /// Wrapper around generic retrieval method specifically for cart config name lists
        /// </summary>
        /// <param name="force">Force reload of data from cache, rather than using the in-memory copy of it</param>
        /// <returns>Mapping of cart names to possible cart config names</returns>
        public static Dictionary<string, List<string>> GetCartConfigNameMap(bool force)
        {
            var cacheData = cartConfigNames;
            if (cartConfigNames.Count == 0 || force || AlwaysRead)
            {
                cacheData = new Dictionary<string, List<string>>();

                // Get data table name
                var tableName = GetTableName(DatabaseTableTypes.CartConfigNameList);

                // Get a list of string dictionaries containing properties for each item
                var allConfigProps = GetPropertiesFromCache(tableName, ConnString);

                var configList = new List<CartConfigInfo>(allConfigProps.Count);

                // For each row (representing one config), create a dictionary and/or list entry
                foreach (var configProps in allConfigProps)
                {
                    // Create a CartConfigInfo object
                    var configInfo = new CartConfigInfo();

                    // Load the cart config data object from the string dictionary
                    configInfo.LoadPropertyValues(configProps);

                    // Add the cart config data object to the full list
                    configList.Add(configInfo);
                }

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

                if (AlwaysRead)
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
            if (cartConfigNames.Count == 0 || force || AlwaysRead)
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
            var data = columnNames;
            if (columnNames.Count == 0 || force || AlwaysRead)
            {
                data = GetSingleColumnListFromCache(DatabaseTableTypes.ColumnList);
                columnNames.Clear();

                if (AlwaysRead)
                {
                    columnNames.Capacity = 0;
                }
                else
                {
                    columnNames.AddRange(data);
                }
            }
            return data;
        }

        /// <summary>
        /// Wrapper around generic retrieval method specifically for separation type lists
        /// </summary>
        /// <param name="force">Force reload of data from cache, rather than using the in-memory copy of it</param>
        /// <returns>List containing separation types</returns>
        public static List<string> GetSepTypeList(bool force)
        {
            var data = separationNames;
            if (separationNames.Count == 0 || force || AlwaysRead)
            {
                data = GetSingleColumnListFromCache(DatabaseTableTypes.SeparationTypeList);
                separationNames.Clear();

                if (AlwaysRead)
                {
                    separationNames.Capacity = 0;
                }
                else
                {
                    separationNames.AddRange(data);
                }
            }
            return data;
        }

        /// <summary>
        /// Wrapper around generic retrieval method specifically for dataset name lists
        /// </summary>
        /// <returns>List containing separation types</returns>
        public static List<string> GetDatasetList()
        {
            var data = datasetNames;
            if (datasetNames.Count == 0 || AlwaysRead)
            {
                data = GetSingleColumnListFromCache(DatabaseTableTypes.DatasetList);
                datasetNames.Clear();

                if (AlwaysRead)
                {
                    datasetNames.Capacity = 0;
                }
                else
                {
                    datasetNames.AddRange(data);
                }
            }
            return data;
        }

        /// <summary>
        /// Wrapper around generic retrieval method specifically for dataset type lists
        /// </summary>
        /// <param name="force">Force reload of data from cache, rather than using the in-memory copy of it</param>
        /// <returns>List containing dataset types</returns>
        public static List<string> GetDatasetTypeList(bool force)
        {
            var data = datasetTypeNames;
            if (datasetTypeNames.Count == 0 || AlwaysRead)
            {
                data = GetSingleColumnListFromCache(DatabaseTableTypes.DatasetTypeList);
                datasetTypeNames.Clear();

                if (AlwaysRead)
                {
                    datasetTypeNames.Capacity = 0;
                }
                else
                {
                    datasetTypeNames.AddRange(data);
                }
            }
            return data;
        }

        /// <summary>
        /// Wrapper around generic retrieval method specifically for Work Package lists
        /// </summary>
        /// <param name="force">Force reload of data from cache, rather than using the in-memory copy of it</param>
        /// <returns>Mapping of Charge Codes to WorkPackageInfo objects</returns>
        public static Dictionary<string, WorkPackageInfo> GetWorkPackageMap(bool force)
        {
            var cacheData = workPackageMap;
            if (workPackageMap.Count == 0 || force || AlwaysRead)
            {
                // Get data table name
                var tableName = GetTableName(DatabaseTableTypes.WorkPackages);

                // Get a list of string dictionaries containing properties for each item
                var allWorkPackageProps = GetPropertiesFromCache(tableName, ConnString);

                cacheData = new Dictionary<string, WorkPackageInfo>(allWorkPackageProps.Count);

                // For each row (representing one work package), create a dictionary and/or list entry
                foreach (var wpProps in allWorkPackageProps)
                {
                    // Create a WorkPackageInfo object
                    var wpInfo = new WorkPackageInfo();

                    // Load the work package data object from the string dictionary
                    wpInfo.LoadPropertyValues(wpProps);

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
                if (!AlwaysRead)
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
            var returnData = userInfo;
            if (userInfo.Count == 0 || force || AlwaysRead)
            {
                // Get data table name
                var tableName = GetTableName(DatabaseTableTypes.UserList);

                // Get a list of string dictionaries containing properties for each item
                var allUserProps = GetPropertiesFromCache(tableName, ConnString);

                returnData = new List<UserInfo>(allUserProps.Count);

                // For each row (representing one user), create a user data object
                //      and load the property values
                foreach (var userProps in allUserProps)
                {
                    // Create a classUserInfo object
                    var userData = new UserInfo();

                    // Load the user data object from the string dictionary
                    userData.LoadPropertyValues(userProps);

                    // Add the user data object to the return list
                    returnData.Add(userData);
                }

                userInfo.Clear();
                if (AlwaysRead)
                {
                    userInfo.Capacity = 0;
                }
                else
                {
                    userInfo.AddRange(returnData);
                }
            }
            // All finished, so return
            return returnData;
        }

        /// <summary>
        /// Gets a list of instruments from the cache
        /// </summary>
        /// <param name="force">Force reload of data from cache, rather than using the in-memory copy of it</param>
        /// <returns>List of instruments</returns>
        public static List<InstrumentInfo> GetInstrumentList(bool force)
        {
            var returnData = instrumentInfo;
            if (instrumentInfo.Count == 0 || force || AlwaysRead)
            {
                // Convert type of list into a data table name
                var tableName = GetTableName(DatabaseTableTypes.InstrumentList);

                // Get a list of string dictionaries containing properties for each instrument
                var allInstProps = GetPropertiesFromCache(tableName, ConnString);

                returnData = new List<InstrumentInfo>(allInstProps.Count);

                // For each row (representing one instrument), create an instrument data object
                //      and load the property values
                foreach (var instProps in allInstProps)
                {
                    // Create a InstrumentInfo object
                    var instData = new InstrumentInfo();

                    // Load the instrument data object from the string dictionary
                    instData.LoadPropertyValues(instProps);

                    // Add the instrument data object to the return list
                    returnData.Add(instData);
                }

                // All finished, so return
                instrumentInfo.Clear();
                if (AlwaysRead)
                {
                    instrumentInfo.Capacity = 0;
                }
                else
                {
                    instrumentInfo.AddRange(returnData);
                }
            }
            return returnData;
        }

        public static List<ExperimentData> GetExperimentList()
        {
            var returnData = experimentsData;
            if (experimentsData.Count == 0 || AlwaysRead)
            {
                var tableName = GetTableName(DatabaseTableTypes.ExperimentList);

                var allExpProperties = GetPropertiesFromCache(tableName, ConnString);

                returnData = new List<ExperimentData>(allExpProperties.Count);

                foreach (var props in allExpProperties)
                {
                    var expDatum = new ExperimentData();

                    expDatum.LoadPropertyValues(props);

                    returnData.Add(expDatum);
                }

                experimentsData.Clear();
                if (AlwaysRead)
                {
                    experimentsData.Capacity = 0;
                }
                else
                {
                    experimentsData.AddRange(returnData);
                }
            }

            return returnData;
        }

        public static void GetProposalUsers(
            out List<ProposalUser> users,
            out Dictionary<string, List<UserIDPIDCrossReferenceEntry>> pidIndexedReferenceList)
        {
            if (proposalUsers.Count > 0 && proposalIdIndexedReferenceList.Count > 0 && !AlwaysRead)
            {
                users = proposalUsers;
                pidIndexedReferenceList = proposalIdIndexedReferenceList;
            }
            else
            {
                pidIndexedReferenceList = new Dictionary<string, List<UserIDPIDCrossReferenceEntry>>();

                var userTableName = GetTableName(DatabaseTableTypes.PUserList);
                var referenceTableName = GetTableName(DatabaseTableTypes.PReferenceList);

                var userExpProperties = GetPropertiesFromCache(userTableName, ConnString);

                var referenceExpProperties = GetPropertiesFromCache(referenceTableName, ConnString);

                users = new List<ProposalUser>(userExpProperties.Count);
                var crossReferenceList = new List<UserIDPIDCrossReferenceEntry>(referenceExpProperties.Count);

                foreach (var props in userExpProperties)
                {
                    var datum = new ProposalUser();
                    datum.LoadPropertyValues(props);
                    users.Add(datum);
                }

                foreach (var props in referenceExpProperties)
                {
                    var datum = new UserIDPIDCrossReferenceEntry();
                    datum.LoadPropertyValues(props);
                    crossReferenceList.Add(datum);
                }

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

                proposalUsers.Clear();
                if (AlwaysRead)
                {
                    proposalIdIndexedReferenceList.Clear();
                    proposalUsers.Capacity = 0;
                }
                else
                {
                    UpdateProposalIdIndexReferenceList(pidIndexedReferenceList);
                    proposalUsers.AddRange(users);
                }
            }
        }

        public static List<LCColumnData> GetEntireLCColumnList()
        {
            var returnData = lcColumns;
            if (lcColumns.Count == 0 || AlwaysRead)
            {
                var tableName = GetTableName(DatabaseTableTypes.LCColumnList);

                var allLCColumnProperties = GetPropertiesFromCache(tableName, ConnString);

                returnData = new List<LCColumnData>(allLCColumnProperties.Count);

                foreach (var props in allLCColumnProperties)
                {
                    var datum = new LCColumnData();
                    datum.LoadPropertyValues(props);
                    returnData.Add(datum);
                }

                lcColumns.Clear();
                if (AlwaysRead)
                {
                    lcColumns.Capacity = 0;
                }
                else
                {
                    lcColumns.AddRange(returnData);
                }
            }

            return returnData;
        }

        /// <summary>
        /// Caches the cart configuration name that is currently selected for this cart
        /// </summary>
        /// <param name="cartConfigName">Cart configuration name</param>
        public static void SaveSelectedCartConfigName(string cartConfigName)
        {
            // Create a list for the Save call to use (it requires a list)
            var cartConfigTypes = new List<string>
            {
                cartConfigName
            };

            SaveSingleColumnListToCache(cartConfigTypes, DatabaseTableTypes.CartConfigNameSelected);
        }

        /// <summary>
        /// Retrieves the cached cart configuration name
        /// </summary>
        /// <returns>Cart configuration name</returns>
        public static string GetDefaultCartConfigName()
        {
            List<string> cartConfigNames;
            try
            {
                cartConfigNames = GetSingleColumnListFromCache(DatabaseTableTypes.CartConfigNameSelected);
            }
            catch (Exception ex)
            {
                CheckExceptionMessageForDbState(ex);
                // Table T_CartConfigNameSelected not found
                // This will happen if the default has not yet been saved
                return string.Empty;
            }

            if (cartConfigNames.Count < 1)
            {
                return string.Empty;
            }

            return cartConfigNames[0];
        }

        /// <summary>
        /// Caches the separation type that is currently selected for this cart
        /// </summary>
        /// <param name="separationType">Separation type</param>
        public static void SaveSelectedSeparationType(string separationType)
        {
            // Create a list for the Save call to use (it requires a list)
            var sepTypes = new List<string>
            {
                separationType
            };

            SaveSingleColumnListToCache(sepTypes, DatabaseTableTypes.SeparationTypeSelected);
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
                sepType = GetSingleColumnListFromCache(DatabaseTableTypes.SeparationTypeSelected);
            }
            catch (Exception ex)
            {
                CheckExceptionMessageForDbState(ex);
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

        /// <summary>
        /// Generic method for saving a single column list to the cache db
        /// </summary>
        /// <param name="tableType">enumTableNames specifying table name suffix</param>
        /// <param name="listData">List of data for storing in table</param>
        /// <param name="clearFirst">if true, the existing data will always be removed from the list; if false and <paramref name="listData"/>.Count is &lt;= to the number of existing rows, nothing is changed</param>
        /// <remarks>Used with T_CartList, T_SeparationTypeSelected, T_LCColumnList, T_DatasetTypeList, T_DatasetList, and T_CartConfigNameSelected</remarks>
        private static void SaveSingleColumnListToCache(List<string> listData, DatabaseTableTypes tableType, bool clearFirst = true)
        {
            // Set up table name
            var tableName = GetTableName(tableType);

            // SQL statement for table clear command
            var sqlClearCmd = "DELETE FROM " + tableName;

            // Build SQL statement for creating table
            var columnName = tableName.Substring(2);
            if (columnName.EndsWith("List", StringComparison.OrdinalIgnoreCase) &&
                !columnName.ToLower().Contains("name"))
            {
                columnName = columnName.Substring(0, columnName.Length - 4) + "Name";
            }
            string[] colNames = { columnName };
            var sqlCreateCmd = BuildGenericCreateTableCmd(tableName, colNames, columnName);

            // If table exists, clear it. Otherwise create one
            if (VerifyTableExists(tableName, ConnString, out _, out int rowCount, true))
            {
                if (!clearFirst && rowCount > listData.Count)
                {
                    return;
                }

                // Clear table
                try
                {
                    ExecuteSQLiteCommand(sqlClearCmd, ConnString);
                }
                catch (Exception ex)
                {
                    CheckExceptionMessageForDbState(ex);
                    var errMsg = "SQLite exception clearing table via command " + sqlClearCmd;
                    // throw new DatabaseDataException(errMsg, ex);
                    ApplicationLogger.LogError(0, errMsg, ex);
                    return;
                }
            }
            else
            {
                // Create table
                try
                {
                    ExecuteSQLiteCommand(sqlCreateCmd, ConnString);
                }
                catch (Exception ex)
                {
                    CheckExceptionMessageForDbState(ex);
                    var errMsg = "SQLite exception creating table " + tableName;
                    // throw new DatabaseDataException(errMsg, ex);
                    ApplicationLogger.LogError(0, errMsg, ex);
                    return;
                }
            }

            if (listData.Count < 1)
            {
                return;
            }

            // Fill the data table
            using (var connection = GetConnection(ConnString))
            using (var command = connection.CreateCommand())
            using (var transaction = connection.BeginTransaction())
            {
                command.CommandText = $"INSERT INTO {tableName} VALUES(:Value)";
                foreach (var item in listData)
                {
                    var param = new SQLiteParameter(":Value", item);
                    command.Parameters.Clear();
                    command.Parameters.Add(param);
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }
        }

        private static void StoreCmdListData(string connectionString, string tableName, ICollection<string> cmdList)
        {
            if (cmdList.Count == 0)
                return;

            // Execute the command list to store data in database
            try
            {
                ExecuteSQLiteCmdsWithTransaction(cmdList, connectionString);
            }
            catch (Exception ex)
            {
                CheckExceptionMessageForDbState(ex);
                var errMsg = "SQLite exception filling table " + tableName;
                // throw new DatabaseDataException(errMsg, ex);
                ApplicationLogger.LogError(0, errMsg, ex);
            }
        }

        /// <summary>
        /// Generic method for retrieving data from a single column table
        /// </summary>
        /// <param name="tableType">DatabaseTableTypes specifying type of table to retrieve</param>
        /// <returns>List containing cached data</returns>
        private static List<string> GetSingleColumnListFromCache(DatabaseTableTypes tableType)
        {
            var returnList = new List<string>();

            // Set up table name
            var tableName = GetTableName(tableType);

            // Verify specified table exists
            if (!VerifyTableExists(tableName, ConnString))
            {
                var errMsg = "Data table " + tableName + " not found in cache";
                throw new DatabaseDataException(errMsg, new Exception());
            }

            // SQL statement for query command
            var sqlQueryCmd = "SELECT * FROM " + tableName;

            // Get a table from the cache db
            DataTable resultTable;
            try
            {
                resultTable = GetSQLiteDataTable(sqlQueryCmd, ConnString);
            }
            catch (Exception ex)
            {
                CheckExceptionMessageForDbState(ex);
                var errMsg = "SQLite exception getting data table via query " + sqlQueryCmd;
                throw new DatabaseDataException(errMsg, ex);
            }

            // Return empty list if no data in table
            if (resultTable.Rows.Count < 1)
            {
                return returnList;
            }

            // Fill the return list
            foreach (DataRow currentRow in resultTable.Rows)
            {
                returnList.Add((string) currentRow[resultTable.Columns[0]]);
            }

            // All finished, so return
            return returnList;
        }

        /// <summary>
        /// Generic method to build a CREATE TABLE command
        /// </summary>
        /// <param name="tableName">Name of table to create</param>
        /// <param name="ColNames">String array containing column names</param>
        /// <param name="primaryKeyColumn">Optional: name of the column to create as the primary key</param>
        /// <returns>Complete CREATE TABLE command</returns>
        private static string BuildGenericCreateTableCmd(string tableName, IEnumerable<string> ColNames,
            string primaryKeyColumn)
        {
            var sb = new StringBuilder();
            sb.Append("CREATE TABLE ");
            sb.Append(tableName + "(");

            // Create column names for each key, which is same as property name in queue being saved
            var query = (from item in ColNames select "'" + item + "'");
            sb.Append(string.Join(",", query));

            if (!string.IsNullOrWhiteSpace(primaryKeyColumn))
            {
                sb.Append(", PRIMARY KEY('" + primaryKeyColumn + "')");
            }

            // Terminate the string and return
            sb.Append(")");

            return sb.ToString();
        }

        /// <summary>
        /// Clears a cache table
        /// </summary>
        /// <param name="tableName">Name of table to clear</param>
        /// <param name="connStr">Connection string</param>
        /// <param name="columnCountExpected">Expected number of columns; 0 to not validate column count</param>
        /// <remarks>If the actual column count is less than columnCountExpected, then the table is deleted (dropped)</remarks>
        private static void ClearCacheTable(string tableName, string connStr, int columnCountExpected = 0)
        {
            // Clear the table, if it exists
            int columnCount;
            if (VerifyTableExists(tableName, connStr, out columnCount))
            {
                string sqlStr;
                if (columnCountExpected > 0 && columnCount < columnCountExpected)
                {
                    // Drop the table; it will get re-created later
                    sqlStr = "DROP TABLE " + tableName;
                }
                else
                {
                    // Clear the table (note that SQLite does not have command "Truncate Table")
                    sqlStr = "DELETE FROM " + tableName;
                }

                try
                {
                    ExecuteSQLiteCommand(sqlStr, connStr);
                }
                catch (Exception ex)
                {
                    CheckExceptionMessageForDbState(ex);
                    var errorMessage = "Exception clearing table " + tableName;
                    ApplicationLogger.LogError(0, errorMessage, ex);
                    throw new DatabaseDataException("Exception clearing table " + tableName, ex);
                }
            }
        }

        #endregion
    }
}
