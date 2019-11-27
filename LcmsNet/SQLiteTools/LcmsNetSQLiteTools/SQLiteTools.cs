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
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Media;
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

        private static readonly Dictionary<Type, Dictionary<string, PropertyColumnMapping>> PropertyColumnMappings = new Dictionary<Type, Dictionary<string, PropertyColumnMapping>>();
        private static readonly Dictionary<Type, Dictionary<string, string>> PropertyColumnNameMappings = new Dictionary<Type, Dictionary<string, string>>();

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

        #region Private Methods

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
        /// Read the data for a list from the cache, handling the in-memory cache appropriately
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="memoryCache"></param>
        /// <param name="tableType"></param>
        /// <param name="newObjectCreator"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        private static List<T> ReadMultiColumnDataListFromCache<T>(List<T> memoryCache, DatabaseTableTypes tableType, Func<T> newObjectCreator, bool force = false)
        {
            var returnData = memoryCache;
            if (memoryCache.Count == 0 || force || AlwaysRead)
            {
                var tableName = GetTableName(tableType);

                // Read the data from the cache
                returnData = ReadDataFromCache(tableName, newObjectCreator, ConnString);

                memoryCache.Clear();
                if (AlwaysRead)
                {
                    memoryCache.Capacity = 0;
                }
                else
                {
                    memoryCache.AddRange(returnData);
                }
            }

            // All finished, so return
            return returnData;
        }

        /// <summary>
        /// Create a list of objects with row data from a cache table
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="tableName">Name of table containing the properties</param>
        /// <param name="objectCreator">Method to create a new object of type <typeparamref name="T"/></param>
        /// <param name="connStr">Connection string</param>
        /// <returns>List of items read from the table</returns>
        private static List<T> ReadDataFromCache<T>(string tableName, Func<T> objectCreator, string connStr)
        {
            var returnData = new List<T>();

            // Verify table exists in database
            if (!VerifyTableExists(tableName, connStr))
            {
                // No table, so return an empty list
                return returnData;
            }

            var type = typeof(T);
            var typeMappings = GetPropertyColumnMapping(type);
            var nameMappings = PropertyColumnNameMappings[type];

            // Compatibility mappings
            // TODO: Remove these in the future
            if (nameMappings.TryGetValue("dms.emslusagetype", out var eusTypeVal) && !nameMappings.ContainsKey("dms.usagetype"))
            {
                nameMappings.Add("dms.usagetype", eusTypeVal);
            }

            if (nameMappings.TryGetValue("dms.emslproposalid", out var eusPid) && !nameMappings.ContainsKey("dms.proposalid"))
            {
                nameMappings.Add("dms.proposalid", eusPid);
            }

            // Get table containing cached data
            var sqlStr = "SELECT * FROM " + tableName;
            var cacheData = GetSQLiteDataTable(sqlStr, connStr);
            if (cacheData.Rows.Count < 1)
            {
                // No cached data found, so return an empty list
                return returnData;
            }

            returnData.Capacity = cacheData.Rows.Count;
            var columnMappings = new KeyValuePair<int, Action<object, object>>[cacheData.Columns.Count];
            var columnSetOrder = new int[cacheData.Columns.Count];

            // Create the column mappings, for fast access to set methods
            foreach (DataColumn column in cacheData.Columns)
            {
                var properName = nameMappings[column.ColumnName.ToLower()];
                var setMethod = typeMappings[properName].SetProperty;
                columnMappings[column.Ordinal] = new KeyValuePair<int, Action<object, object>>(column.Ordinal, setMethod);
                columnSetOrder[column.Ordinal] = column.Ordinal;

                if (properName.ToLower().Contains("runningstatus"))
                {
                    // Always process runningstatus last
                    columnSetOrder[column.Ordinal] = int.MaxValue;
                }
            }

            Array.Sort(columnSetOrder, columnMappings);

            // For each row (representing properties and values for one sample), create a string dictionary
            //          with the object's properties from the table columns, and add it to the return list
            foreach (DataRow currentRow in cacheData.Rows)
            {
                // Create a new object
                var data = objectCreator();

                // Populate the properties from the cache
                foreach (var column in columnMappings)
                {
                    var value = currentRow[column.Key];
                    var setMethod = column.Value;
                    setMethod(data, value);
                }

                // Add the object to the return list
                returnData.Add(data);
            }

            // Return the list
            return returnData;
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
        private static bool TableColumnNamesMatched(string tableName, string connStr, List<string> fieldNames)
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

            foreach (var name in fieldNames)
            {
                if (!namesList.Contains(name))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Store the contents of a list in the specified table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataList">list of data to be stored</param>
        /// <param name="memoryCache">in-memory cache of the list</param>
        /// <param name="clearFirst">if the existing data should be removed first (always); otherwise, if the row counts match, nothing is changed</param>
        /// <param name="tableType">table the data is to be stored in</param>
        private static void SaveMultiColumnListToCache<T>(List<T> dataList, List<T> memoryCache, bool clearFirst, DatabaseTableTypes tableType)
        {
            SaveMultiColumnListToCache(dataList, clearFirst, tableType);

            if (!AlwaysRead)
            {
                memoryCache.Clear();
                memoryCache.AddRange(dataList);
            }
        }

        /// <summary>
        /// Store the contents of a list in the specified table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataList">list of data to be stored</param>
        /// <param name="clearFirst">if the existing data should be removed first (always); otherwise, if the row counts match, nothing is changed</param>
        /// <param name="tableType">table the data is to be stored in</param>
        private static void SaveMultiColumnListToCache<T>(List<T> dataList, bool clearFirst, DatabaseTableTypes tableType)
        {
            var tableName = GetTableName(tableType);
            if (VerifyTableExists(tableName, ConnString, out _, out int rowCount, true) && !clearFirst && dataList.Count <= rowCount)
            {
                return;
            }

            // Clear the cache table
            ClearCacheTable(tableName, ConnString);

            //If no data in list, exit
            if (dataList.Count < 1)
            {
                return;
            }

            WriteDataToCache(dataList, tableName, ConnString);
        }

        /// <summary>
        /// Saves a list of objects to the cache database
        /// </summary>
        /// <param name="dataToCache">List of objects to save properties for</param>
        /// <param name="tableName">Name of the table to save data in</param>
        /// <param name="connStr">Connection string</param>
        private static void WriteDataToCache<T>(IReadOnlyCollection<T> dataToCache, string tableName, string connStr)
        {
            // If there is no data, then just exit
            if (dataToCache.Count < 1)
            {
                return;
            }

            var mappings = GetPropertyColumnMapping(typeof(T)).Values.ToList();

            // Verify table exists, and column names are correct; if not, create it; Otherwise, clear it
            var tableExists = VerifyTableExists(tableName, connStr);
            var fieldNames = mappings.Select(x => x.ColumnName).ToList();
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

            // Fill the data table
            using (var connection = GetConnection(ConnString))
            using (var command = connection.CreateCommand())
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    command.CommandText = $"INSERT INTO {tableName}({string.Join(",", mappings.Select(x => $"'{x.ColumnName}'"))}) VALUES ({string.Join(",", mappings.Select(x => $":{x.ColumnName.Replace(".", "")}"))})";

                    foreach (var item in dataToCache)
                    {
                        command.Parameters.Clear();
                        foreach (var map in mappings)
                        {
                            command.Parameters.Add(new SQLiteParameter($":{map.ColumnName.Replace(".", "")}", map.ReadProperty(item)?.ToString()));
                        }

                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    CheckExceptionMessageForDbState(ex);
                    const string errMsg = "SQLite exception adding data";
                    ApplicationLogger.LogError(0, errMsg, ex);
                    //throw new DatabaseDataException(errMsg, ex);
                }
            }
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
        /// Builds a CREATE TABLE command from the input string dictionary
        /// </summary>
        /// <param name="inpData">String list containing property names</param>
        /// <param name="tableName">Name of table to create</param>
        /// <returns>String consisting of a complete CREATE TABLE SQL statement</returns>
        private static string BuildCreatePropTableCmd(List<string> inpData, string tableName)
        {
            // Create column names for each key, which is same as property name in queue being saved
            return $"CREATE TABLE {tableName}({string.Join(",", inpData.Select(x => $"'{x}'"))})";
        }

        /// <summary>
        /// Converts a type of table to the corresponding cache db table name
        /// </summary>
        /// <param name="tableType">DatabaseTableTypes specifying table to get name for</param>
        /// <returns>Name of db table</returns>
        private static string GetTableName(DatabaseTableTypes tableType)
        {
            return "T_" + Enum.GetName(typeof(DatabaseTableTypes), tableType);
        }

        /// <summary>
        /// Generic method for saving a single column list to the cache db
        /// </summary>
        /// <param name="listData">List of data for storing in table</param>
        /// <param name="memoryCache">List used for in-memory cache of contents</param>
        /// <param name="tableType">enumTableNames specifying table name suffix</param>
        /// <param name="clearFirst">if true, the existing data will always be removed from the list; if false and <paramref name="listData"/>.Count is &lt;= to the number of existing rows, nothing is changed</param>
        private static void SaveSingleColumnListToCache(List<string> listData, List<string> memoryCache, DatabaseTableTypes tableType, bool clearFirst = true)
        {
            // Refresh the in-memory list with the new data
            if (!AlwaysRead)
            {
                memoryCache.Clear();
                memoryCache.AddRange(listData);
            }

            SaveSingleColumnListToCache(listData, tableType, clearFirst);
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

        /// <summary>
        /// Read a single-column list from the cache, handling management of an in-memory list
        /// </summary>
        /// <param name="memoryCache"></param>
        /// <param name="tabletype">DatabaseTableTypes specifying type of table to retrieve</param>
        /// <param name="force"></param>
        /// <returns>List containing cached data</returns>
        private static List<string> ReadSingleColumnListFromCache(List<string> memoryCache, DatabaseTableTypes tabletype, bool force = false)
        {
            var data = memoryCache;
            if (memoryCache.Count == 0 || force || AlwaysRead)
            {
                data = ReadSingleColumnListFromCache(tabletype);

                memoryCache.Clear();
                if (AlwaysRead)
                {
                    memoryCache.Capacity = 0;
                }
                else
                {
                    memoryCache.AddRange(data);
                }
            }
            return data;
        }

        /// <summary>
        /// Generic method for retrieving data from a single column table
        /// </summary>
        /// <param name="tableType">DatabaseTableTypes specifying type of table to retrieve</param>
        /// <returns>List containing cached data</returns>
        private static List<string> ReadSingleColumnListFromCache(DatabaseTableTypes tableType)
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

            returnList.Capacity = resultTable.Rows.Count;

            // Fill the return list
            foreach (DataRow currentRow in resultTable.Rows)
            {
                returnList.Add((string)currentRow[resultTable.Columns[0]]);
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

        #region Public Methods

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

            // Convert type of queue into a data table name
            var tableName = GetTableName(tableType);

            // All finished, so return
            return ReadDataFromCache(tableName, () => (T)(new T().GetNewNonDummy()), connectionString);
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
            return ReadSingleColumnListFromCache(cartNames, DatabaseTableTypes.CartList, false);
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

                // Read the data from the cache
                var configList = ReadDataFromCache(tableName, () => new CartConfigInfo(), ConnString);

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
            return ReadSingleColumnListFromCache(columnNames, DatabaseTableTypes.ColumnList, force);
        }

        /// <summary>
        /// Wrapper around generic retrieval method specifically for separation type lists
        /// </summary>
        /// <param name="force">Force reload of data from cache, rather than using the in-memory copy of it</param>
        /// <returns>List containing separation types</returns>
        public static List<string> GetSepTypeList(bool force)
        {
            return ReadSingleColumnListFromCache(separationNames, DatabaseTableTypes.SeparationTypeList, force);
        }

        /// <summary>
        /// Wrapper around generic retrieval method specifically for dataset name lists
        /// </summary>
        /// <returns>List containing separation types</returns>
        public static List<string> GetDatasetList()
        {
            return ReadSingleColumnListFromCache(datasetNames, DatabaseTableTypes.DatasetList, false);
        }

        /// <summary>
        /// Wrapper around generic retrieval method specifically for dataset type lists
        /// </summary>
        /// <param name="force">Force reload of data from cache, rather than using the in-memory copy of it</param>
        /// <returns>List containing dataset types</returns>
        public static List<string> GetDatasetTypeList(bool force)
        {
            return ReadSingleColumnListFromCache(datasetTypeNames, DatabaseTableTypes.DatasetTypeList, force);
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

                // Read the data from the cache
                var workPackages = ReadDataFromCache(tableName, () => new WorkPackageInfo(), ConnString);

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
            return ReadMultiColumnDataListFromCache(userInfo, DatabaseTableTypes.UserList, () => new UserInfo(), force);
        }

        /// <summary>
        /// Gets a list of instruments from the cache
        /// </summary>
        /// <param name="force">Force reload of data from cache, rather than using the in-memory copy of it</param>
        /// <returns>List of instruments</returns>
        public static List<InstrumentInfo> GetInstrumentList(bool force)
        {
            return ReadMultiColumnDataListFromCache(instrumentInfo, DatabaseTableTypes.InstrumentList, () => new InstrumentInfo(), force);
        }

        public static List<ExperimentData> GetExperimentList()
        {
            return ReadMultiColumnDataListFromCache(experimentsData, DatabaseTableTypes.ExperimentList, () => new ExperimentData(), false);
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

                // Read the data from the cache
                users = ReadDataFromCache(userTableName, () => new ProposalUser(), ConnString);
                var crossReferenceList = ReadDataFromCache(userTableName, () => new UserIDPIDCrossReferenceEntry(), ConnString);

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
            return ReadMultiColumnDataListFromCache(lcColumns, DatabaseTableTypes.LCColumnList, () => new LCColumnData(), false);
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
                cartConfigNames = ReadSingleColumnListFromCache(DatabaseTableTypes.CartConfigNameSelected);
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
        /// Retrieves the cached separation type
        /// </summary>
        /// <returns>Separation type</returns>
        public static string GetDefaultSeparationType()
        {
            List<string> sepType;
            try
            {
                sepType = ReadSingleColumnListFromCache(DatabaseTableTypes.SeparationTypeSelected);
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

            SaveMultiColumnListToCache(queueData, true, tableType);
        }

        /// <summary>
        /// Saves a list of users to cache
        /// </summary>
        /// <param name="userList">List containing user data</param>
        /// <param name="clearFirst">if true, the existing data will always be removed from the list; if false and <paramref name="userList"/>.Count is &lt;= to the number of existing rows, nothing is changed</param>
        public static void SaveUserListToCache(List<UserInfo> userList, bool clearFirst = true)
        {
            SaveMultiColumnListToCache(userList, userInfo, clearFirst, DatabaseTableTypes.UserList);
        }

        /// <summary>
        /// Save a list of experiments to cache
        /// </summary>
        /// <param name="expList"></param>
        /// <param name="clearFirst">if true, the existing data will always be removed from the list; if false and <paramref name="expList"/>.Count is &lt;= to the number of existing rows, nothing is changed</param>
        public static void SaveExperimentListToCache(List<ExperimentData> expList, bool clearFirst = true)
        {
            if (expList == null) return;

            SaveMultiColumnListToCache(expList, experimentsData, clearFirst, DatabaseTableTypes.ExperimentList);
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
            SaveMultiColumnListToCache(users, proposalUsers, clearFirst, DatabaseTableTypes.PUserList);
            SaveMultiColumnListToCache(crossReferenceList, clearFirst, DatabaseTableTypes.PReferenceList);

            if (!AlwaysRead)
            {
                UpdateProposalIdIndexReferenceList(pidIndexedReferenceList);
            }
        }

        public static void SaveEntireLCColumnListToCache(List<LCColumnData> lcColumnList)
        {
            SaveMultiColumnListToCache(lcColumnList, lcColumns, true, DatabaseTableTypes.LCColumnList);
        }

        /// <summary>
        /// Saves a list of instruments to cache
        /// </summary>
        /// <param name="instList">List of InstrumentInfo containing instrument data</param>
        /// <param name="clearFirst">if true, the existing data will always be removed from the list; if false and <paramref name="instList"/>.Count is &lt;= to the number of existing rows, nothing is changed</param>
        public static void SaveInstListToCache(List<InstrumentInfo> instList, bool clearFirst = true)
        {
            SaveMultiColumnListToCache(instList, instrumentInfo, clearFirst, DatabaseTableTypes.InstrumentList);
        }

        /// <summary>
        /// Saves a list of Cart_Configs (and associated Cart names) to cache
        /// </summary>
        /// <param name="cartConfigList">List containing cart config info.</param>
        /// <param name="clearFirst">if true, the existing data will always be removed from the list; if false and <paramref name="cartConfigList"/>.Count is &lt;= to the number of existing rows, nothing is changed</param>
        public static void SaveCartConfigListToCache(List<CartConfigInfo> cartConfigList, bool clearFirst = true)
        {
            SaveMultiColumnListToCache(cartConfigList, clearFirst, DatabaseTableTypes.CartConfigNameList);

            // Reload the in-memory copy of the cached data
            if (!AlwaysRead)
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
            SaveMultiColumnListToCache(workPackageList, clearFirst, DatabaseTableTypes.WorkPackages);

            // Reload the in-memory copy of the cached data
            if (!AlwaysRead)
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
            SaveSingleColumnListToCache(cartNameList, cartNames, DatabaseTableTypes.CartList, clearFirst);
        }

        /// <summary>
        /// Saves a list of column names to the SQLite cache
        /// </summary>
        /// <param name="columnList">Column names</param>
        /// <param name="clearFirst">if true, the existing data will always be removed from the list; if false and <paramref name="columnList"/>.Count is &lt;= to the number of existing rows, nothing is changed</param>
        public static void SaveColumnListToCache(List<string> columnList, bool clearFirst = true)
        {
            SaveSingleColumnListToCache(columnList, columnNames, DatabaseTableTypes.ColumnList, clearFirst);
        }

        /// <summary>
        /// Saves a list of Dataset names to the SQLite cache
        /// </summary>
        /// <param name="datasetNameList">Dataset names</param>
        /// <param name="clearFirst">if true, the existing data will always be removed from the list; if false and <paramref name="datasetNameList"/>.Count is &lt;= to the number of existing rows, nothing is changed</param>
        public static void SaveDatasetNameListToCache(List<string> datasetNameList, bool clearFirst = true)
        {
            SaveSingleColumnListToCache(datasetNameList, datasetNames, DatabaseTableTypes.DatasetList, clearFirst);
        }

        /// <summary>
        /// Saves a list of dataset type names to the SQLite cache
        /// </summary>
        /// <param name="datasetTypeList">Dataset type names</param>
        /// <param name="clearFirst">if true, the existing data will always be removed from the list; if false and <paramref name="datasetTypeList"/>.Count is &lt;= to the number of existing rows, nothing is changed</param>
        public static void SaveDatasetTypeListToCache(List<string> datasetTypeList, bool clearFirst = true)
        {
            SaveSingleColumnListToCache(datasetTypeList, datasetTypeNames, DatabaseTableTypes.DatasetTypeList, clearFirst);
        }

        /// <summary>
        /// Saves a list of separation types to the SQLite cache
        /// </summary>
        /// <param name="separationTypeList">Separation type names</param>
        /// <param name="clearFirst">if true, the existing data will always be removed from the list; if false and <paramref name="separationTypeList"/>.Count is &lt;= to the number of existing rows, nothing is changed</param>
        public static void SaveSeparationTypeListToCache(List<string> separationTypeList, bool clearFirst = true)
        {
            SaveSingleColumnListToCache(separationTypeList, separationNames, DatabaseTableTypes.SeparationTypeList, clearFirst);
        }

        /// <summary>
        /// Caches the cart configuration name that is currently selected for this cart
        /// </summary>
        /// <param name="cartConfigName">Cart configuration name</param>
        public static void SaveSelectedCartConfigName(string cartConfigName)
        {
            // Create a list for the Save call to use (it requires a list)
            SaveSingleColumnListToCache(new List<string> { cartConfigName }, DatabaseTableTypes.CartConfigNameSelected);
        }

        /// <summary>
        /// Caches the separation type that is currently selected for this cart
        /// </summary>
        /// <param name="separationType">Separation type</param>
        public static void SaveSelectedSeparationType(string separationType)
        {
            // Create a list for the Save call to use (it requires a list)
            SaveSingleColumnListToCache(new List<string> { separationType }, DatabaseTableTypes.SeparationTypeSelected);
        }

        #endregion

        #region Property to Column mappings

        private class PropertyColumnMapping
        {
            /// <summary>
            /// SQLite table column name
            /// </summary>
            public string ColumnName { get; }

            /// <summary>
            /// The type of the property, for conversion handling
            /// </summary>
            public Type PropertyType { get; }

            /// <summary>
            /// Method for reading the property
            /// </summary>
            public Func<object, object> ReadProperty { get; }

            /// <summary>
            /// Method for setting the property
            /// </summary>
            public Action<object, object> SetProperty { get; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="columnName">SQLite table column name</param>
            /// <param name="propertyType">Type of the property, for conversion handling</param>
            /// <param name="readProperty">Method for reading the property</param>
            /// <param name="setProperty">Method for setting the property</param>
            public PropertyColumnMapping(string columnName, Type propertyType,
                Func<object, object> readProperty, Action<object, object> setProperty)
            {
                ColumnName = columnName;
                PropertyType = propertyType;
                ReadProperty = readProperty;
                SetProperty = setProperty;
            }
        }

        private static Dictionary<string, PropertyColumnMapping> GetPropertyColumnMapping(Type type)
        {
            if (PropertyColumnMappings.TryGetValue(type, out var mappings))
            {
                return mappings;
            }

            mappings = new Dictionary<string, PropertyColumnMapping>();
            var nameMapping = new Dictionary<string, string>();

            foreach (var property in type.GetProperties())
            {
                var settings = (PersistenceSettingAttribute)Attribute.GetCustomAttribute(property, typeof(PersistenceSettingAttribute)) ?? new PersistenceSettingAttribute();
                if (settings.IgnoreProperty)
                {
                    continue;
                }

                // Test to make sure the property has both get and set accessors
                // TODO: Should not be necessary for non-primitive-type properties, as long as they are otherwise initialized appropriately.
                // TODO: Requirement: if an object, then "CanWrite" can be false as long as there is a default constructor for the object type.
                if (!(property.CanRead && property.CanWrite))
                {
                    throw new NotSupportedException(
                        "Operation requires get and set accessors for all persisted properties. " +
                        "Add the attribute '[PersistenceSetting(IgnoreProperty = true)]' to ignore the failing property. Property info: Class '" +
                        type.FullName + "', property '" + property.Name + "'.");
                }

                if (string.IsNullOrWhiteSpace(settings.ColumnName))
                {
                    settings.ColumnName = property.Name;
                }

                if (string.IsNullOrWhiteSpace(settings.ColumnNamePrefix))
                {
                    settings.ColumnNamePrefix = settings.ColumnName;
                }

                if (!string.IsNullOrWhiteSpace(settings.ColumnReadOverrideMethod))
                {
                    // Special read method: resolve the method, and generate the mappings
                    try
                    {
                        var methodInfo = type.GetMethod(settings.ColumnReadOverrideMethod, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        if (methodInfo == null)
                        {
                            throw new NotSupportedException($"Method named \"{settings.ColumnReadOverrideMethod}\" not found in type \"{type.FullName ?? type.Name}\"!");
                        }

                        if (methodInfo.GetParameters().Length > 0 && methodInfo.GetParameters().Any(x => !x.HasDefaultValue))
                        {
                            throw new NotSupportedException($"Method named \"{settings.ColumnReadOverrideMethod}\" in type \"{type.FullName ?? type.Name}\" has required arguments, and cannot be used for object persistence!");
                        }

                        var propMappings = GetPropertyColumnMapping(type, property, settings, x => methodInfo.Invoke(x, null));
                        foreach (var mapping in propMappings)
                        {
                            mappings.Add(mapping.ColumnName, mapping);
                            nameMapping.Add(mapping.ColumnName.ToLower(), mapping.ColumnName);
                        }
                    }
                    catch (AmbiguousMatchException)
                    {
                        throw new NotSupportedException($"Multiple matches found in class \"{type.FullName ?? type.Name}\" for method name \"{settings.ColumnReadOverrideMethod}\". Method name must be unique!");
                    }
                }
                else
                {
                    var propMappings = GetPropertyColumnMapping(type, property, settings);
                    foreach (var mapping in propMappings)
                    {
                        mappings.Add(mapping.ColumnName, mapping);
                        nameMapping.Add(mapping.ColumnName.ToLower(), mapping.ColumnName);
                    }
                }
            }

            PropertyColumnMappings.Add(type, mappings);
            PropertyColumnNameMappings.Add(type, nameMapping);
            return mappings;
        }

        private static IEnumerable<PropertyColumnMapping> GetPropertyColumnMapping(Type type, PropertyInfo property, PersistenceSettingAttribute settings, Func<object, object> readMethod = null)
        {
            var propType = property.PropertyType;
            if (propType.IsValueType || propType.IsEnum || propType == typeof(string))
            {
                // Built-in direct handling: read or assign, with some error checking
                yield return new PropertyColumnMapping(settings.ColumnName, propType, x =>
                {
                    if (x.GetType() != type)
                    {
                        // return the default value for the type
                        //return Activator.CreateInstance(propType);
                        throw new NotSupportedException($"Cannot access property \"{property.Name}\" on object of type \"{x.GetType().FullName ?? x.GetType().Name}\"");
                    }

                    if (readMethod != null)
                    {
                        return readMethod(x);
                    }

                    return property.GetValue(x);
                }, (cls, propValue) =>
                {
                    if (cls.GetType() != type)
                    {
                        // return the default value for the type
                        //return Activator.CreateInstance(propType);
                        throw new NotSupportedException($"Cannot access property \"{property.Name}\" on object of type \"{cls.GetType().FullName ?? cls.GetType().Name}\"");
                    }

                    if (propValue.GetType() == propType)
                    {
                        property.SetValue(cls, propValue);
                    }
                    else
                    {
                        try
                        {
                            var value = ConvertToType(propValue, propType);
                            property.SetValue(cls, value);
                        }
                        catch (Exception ex) when (ex is InvalidCastException || ex is FormatException || ex is OverflowException)
                        {
                            throw new NotSupportedException($"Could not convert value of type \"{propValue.GetType().Name}\" with value \"{propValue}\" to target type \"{propType.Name}\"; type \"{cls.GetType().FullName ?? cls.GetType().Name}\", column name \"{settings.ColumnName}\"");
                        }
                    }
                });
                yield break;
            }

            if (propType.IsClass && propType.FullName.ToLower().StartsWith("lcmsnet"))
            {
                // LCMSNet class: generate the mapping, with cascading through sub-objects
                var objectMappings = GetPropertyColumnMapping(propType);
                foreach (var mapping in objectMappings)
                {
                    yield return new PropertyColumnMapping($"{settings.ColumnNamePrefix}{mapping.Key}", mapping.Value.PropertyType, x =>
                    {
                        if (x.GetType() != type)
                        {
                            // return the default value for the type
                            //return Activator.CreateInstance(propType);
                            throw new NotSupportedException($"Cannot access property {property.Name} on object of type {x.GetType()}");
                        }

                        if (readMethod != null)
                        {
                            return mapping.Value.ReadProperty(readMethod(x));
                        }

                        return mapping.Value.ReadProperty(property.GetValue(x));
                    }, (cls, propValue) =>
                    {
                        if (cls.GetType() != type)
                        {
                            // return the default value for the type
                            //return Activator.CreateInstance(propType);
                            throw new NotSupportedException($"Cannot access property \"{property.Name}\" on object of type \"{cls.GetType().FullName ?? cls.GetType().Name}\"");
                        }

                        var subObject = property.GetValue(cls);
                        if (subObject == null)
                        {
                            try
                            {
                                subObject = Activator.CreateInstance(propType);
                                property.SetValue(cls, subObject);
                            }
                            catch (Exception e)
                            {
                                throw new NotSupportedException($"Could not set a value for property \"{property.Name}\" in class \"{cls.GetType().FullName ?? cls.GetType().Name}\", of type \"{propType}\": {e}");
                            }
                        }

                        var targetType = mapping.Value.PropertyType;
                        if (propValue.GetType() == targetType)
                        {
                            mapping.Value.SetProperty(subObject, propValue);
                        }
                        else
                        {
                            try
                            {
                                var value = ConvertToType(propValue, targetType);
                                mapping.Value.SetProperty(subObject, value);
                            }
                            catch (Exception ex) when (ex is InvalidCastException || ex is FormatException || ex is OverflowException)
                            {
                                throw new NotSupportedException($"Could not convert value of type \"{propValue.GetType().Name}\" with value \"{propValue}\" to target type \"{propType.Name}\"; type \"{cls.GetType().FullName ?? cls.GetType().Name}\", column name \"{settings.ColumnName}\"");
                            }
                        }
                    });
                }
            }
        }

        private static object ConvertToType(object value, Type targetType)
        {
            if (value == null || value is DBNull)
            {
                if (targetType.IsValueType || targetType.IsEnum || targetType.IsValueType)
                {
                    // return the default value for the type
                    return Activator.CreateInstance(targetType);
                }

                return null;
            }
            if (value.GetType() == targetType)
            {
                return value;
            }
            if (targetType == typeof(string))
            {
                return value;
            }

            if (targetType.IsEnum && !string.IsNullOrWhiteSpace(value.ToString()))
            {
                return Enum.Parse(targetType, value.ToString());
            }
            if (targetType.IsPrimitive)
            {
                return Convert.ChangeType(value, targetType);
            }
            if (targetType == typeof(DateTime))
            {
                var tempValue = (DateTime)Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
                return TimeZoneInfo.ConvertTimeToUtc(tempValue);
            }
            if (targetType == typeof(Color))
            {
                var convertFromString = TypeDescriptor.GetConverter(typeof(Color)).ConvertFromString(value.ToString());
                if (convertFromString != null)
                {
                    return (Color)convertFromString;
                }

                return new Color();
            }
            if (targetType.ToString().StartsWith("System.Nullable"))
            {
                var wrappedType = targetType.GenericTypeArguments[0];
                if (string.IsNullOrWhiteSpace(value.ToString()))
                {
                    return null;
                }

                // We're dealing with nullable types here, and the default
                // value for those is null, so we shouldn't have to set the
                // value to null if parsing doesn't work.
                try
                {
                    return ConvertToType(value, wrappedType);
                }
                catch (InvalidCastException) { }
                catch (FormatException) { }
                catch (OverflowException) { }

                return null;
            }

            throw new NotSupportedException("LcmsNetSQLiteTools.SQLiteTools.ConvertToType(), Invalid property type specified: " + targetType);
        }

        #endregion
    }
}
