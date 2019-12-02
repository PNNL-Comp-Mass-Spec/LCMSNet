using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using LcmsNetData;
using LcmsNetData.Logging;

namespace LcmsNetSQLiteTools
{
    internal class SQLiteCacheIO : IDisposable
    {
        private SQLiteConnection storedConnection = null;
        private string lastConnectionString = "";

        private string cacheFullPath;

        private PropertyToColumnMapping propToColumnMap = new PropertyToColumnMapping();

        #region Properties

        /// <summary>
        /// Cache file name or path
        /// </summary>
        /// <remarks>Starts off as a filename, but is changed to a path by BuildConnectionString</remarks>
        public string CacheName
        {
            get { return LCMSSettings.GetParameter(LCMSSettings.PARAM_CACHEFILENAME); }
            private set { LCMSSettings.SetParameter(LCMSSettings.PARAM_CACHEFILENAME, value); }
        }

        public string AppDataFolderName { get; set; }
        public string ConnString { get; set; } = "";
        public bool DatabaseImageBad { get; private set; }
        public bool DisableInMemoryCaching { get; set; }

        public bool AlwaysRead => !DatabaseImageBad && DisableInMemoryCaching;

        #endregion

        #region Initialize and Dispose

        public SQLiteCacheIO()
        {
            Initialize();
        }

        /// <summary>
        /// Initialize the cache, with the provided app name and cache filename
        /// </summary>
        /// <param name="appDataFolderName"></param>
        /// <param name="cacheName"></param>
        public void Initialize(string appDataFolderName = "LCMSNet", string cacheName = "LCMSCache.que")
        {
            AppDataFolderName = appDataFolderName;
            CacheName = cacheName;

            BuildConnectionString(false);
        }

        ~SQLiteCacheIO()
        {
            Dispose();
        }

        public void Dispose()
        {
            Close();
            GC.SuppressFinalize(this);
        }

        public void BuildConnectionString(bool newCache)
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

        public void CloseConnection()
        {
            Close();
        }

        private void Close()
        {
            try
            {
                storedConnection?.Close();
                storedConnection?.Dispose();
                storedConnection = null;
            }
            catch
            {
                // Swallow any exceptions that occurred...
            }
        }

        #endregion

        #region Connection Management

        /// <summary>
        /// Get a SQLiteConnection, but limit how often we open a new connection
        /// </summary>
        /// <param name="connString"></param>
        /// <returns></returns>
        private SQLiteConnectionWrapper GetConnection(string connString)
        {
            if (connString.Equals(ConnString))
            {
                if (!lastConnectionString.Equals(connString))
                {
                    Close();
                }

                if (storedConnection == null)
                {
                    lastConnectionString = connString;
                    storedConnection = new SQLiteConnection(connString).OpenAndReturn();
                }

                return new SQLiteConnectionWrapper(storedConnection);
            }

            return new SQLiteConnectionWrapper(connString);
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

        private void CheckExceptionMessageForDbState(Exception ex)
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
        /// Determines if a particular table exists in the SQLite database
        /// </summary>
        /// <param name="tableName">Name of the table to search for</param>
        /// <param name="connStr">Connection string for database</param>
        /// <returns>TRUE if table found; FALSE if not found or error</returns>
        private bool VerifyTableExists(string tableName, string connStr)
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
        private bool VerifyTableExists(string tableName, string connStr, out int columnCount)
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
        private bool VerifyTableExists(string tableName, string connStr, out int columnCount, out int rowCount, bool getRowCount = true)
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
        private bool TableColumnNamesMatched(string tableName, string connStr, List<string> fieldNames)
        {
            var namesList = GetTableColumnNames(tableName, connStr, out var columnCount);
            if (columnCount != fieldNames.Count || namesList.Count == 0)
            {
                return false;
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
        /// Get a list of column names for the specified table
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="connStr"></param>
        /// <param name="columnCount"></param>
        /// <returns></returns>
        private List<string> GetTableColumnNames(string tableName, string connStr, out int columnCount)
        {
            var colNames = new List<string>();
            if (!VerifyTableExists(tableName, connStr, out columnCount))
            {
                return colNames;
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
                return colNames;
            }

            if (resultSet1.Rows.Count < 1)
            {
                return colNames;
            }

            foreach (DataRow currentRow in resultSet1.Rows)
            {
                foreach (DataColumn column in resultSet1.Columns)
                {
                    var colName = column.ColumnName;
                    if (colName.ToLower().Equals("name"))
                    {
                        // row that contains the column name
                        var colData = (string)currentRow[resultSet1.Columns[colName]];
                        colNames.Add(colData);
                    }
                }
            }

            return colNames;
        }

        /// <summary>
        /// Saves a list of objects to the cache database
        /// </summary>
        /// <param name="dataToCache">List of objects to save properties for</param>
        /// <param name="tableName">Name of the table to save data in</param>
        /// <param name="connStr">Connection string</param>
        private void WriteDataToCache<T>(IReadOnlyCollection<T> dataToCache, string tableName, string connStr)
        {
            // If there is no data, then just exit
            if (dataToCache.Count < 1)
            {
                return;
            }

            var mappings = propToColumnMap.GetPropertyColumnMapping(typeof(T)).Values.ToList();

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

        /// <summary>
        /// Executes specified SQLite command
        /// </summary>
        /// <param name="cmdStr">SQL statement to execute</param>
        /// <param name="connStr">Connection string for SQL database file</param>
        private void ExecuteSQLiteCommand(string cmdStr, string connStr)
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
        private DataTable GetSQLiteDataTable(string cmdStr, string connStr)
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
        private string BuildCreatePropTableCmd(List<string> inpData, string tableName)
        {
            // Create column names for each key, which is same as property name in queue being saved
            return $"CREATE TABLE {tableName}({string.Join(",", inpData.Select(x => $"'{x}' TEXT"))})";
        }

        /// <summary>
        /// Generic method to build a CREATE TABLE command
        /// </summary>
        /// <param name="tableName">Name of table to create</param>
        /// <param name="colNames">String array containing column names</param>
        /// <param name="primaryKeyColumn">Optional: name of the column to create as the primary key</param>
        /// <param name="caseInsensitive">If true, 'COLLATE NOCASE' is added to column definitions</param>
        /// <returns>Complete CREATE TABLE command</returns>
        private string BuildGenericCreateTableCmd(string tableName, IEnumerable<string> colNames, string primaryKeyColumn, bool caseInsensitive = false)
        {
            var sb = new StringBuilder();
            sb.Append("CREATE TABLE ");
            sb.Append(tableName + "(");

            var nocaseAppend = "";
            if (caseInsensitive)
            {
                nocaseAppend = " COLLATE NOCASE";
            }

            // Create column names for each key, which is same as property name in queue being saved
            sb.Append(string.Join(",", colNames.Select(x => $"'{x}' TEXT{nocaseAppend}")));

            if (!string.IsNullOrWhiteSpace(primaryKeyColumn))
            {
                sb.Append(", PRIMARY KEY('" + primaryKeyColumn + "')");
            }

            // Terminate the string and return
            sb.Append(")");

            return sb.ToString();
        }

        /// <summary>
        /// Converts a type of table to the corresponding cache db table name
        /// </summary>
        /// <param name="tableType">DatabaseTableTypes specifying table to get name for</param>
        /// <returns>Name of db table</returns>
        private string GetTableName(DatabaseTableTypes tableType)
        {
            return "T_" + Enum.GetName(typeof(DatabaseTableTypes), tableType);
        }

        /// <summary>
        /// Get the name of the single column in a single-column table
        /// </summary>
        /// <param name="tableType"></param>
        /// <returns></returns>
        private string GetSingleColumnName(DatabaseTableTypes tableType)
        {
            var names = GetTableColumnNames(GetTableName(tableType), ConnString, out _);
            if (names.Count == 0)
            {
                var columnName = "" + Enum.GetName(typeof(DatabaseTableTypes), tableType);
                if (columnName.EndsWith("List", StringComparison.OrdinalIgnoreCase) &&
                    !columnName.ToLower().Contains("name"))
                {
                    columnName = columnName.Substring(0, columnName.Length - 4) + "Name";
                }

                return columnName;
            }

            return names[0];
        }

        /// <summary>
        /// Clears a cache table
        /// </summary>
        /// <param name="tableName">Name of table to clear</param>
        /// <param name="connStr">Connection string</param>
        /// <param name="columnCountExpected">Expected number of columns; 0 to not validate column count</param>
        /// <remarks>If the actual column count is less than columnCountExpected, then the table is deleted (dropped)</remarks>
        private void ClearCacheTable(string tableName, string connStr, int columnCountExpected = 0)
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
        public void DeleteBadCache(bool force = false)
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
        public void SetCacheLocation(string location)
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
        /// Read the data for a list from the cache, handling the in-memory cache appropriately
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="memoryCache"></param>
        /// <param name="tableType"></param>
        /// <param name="newObjectCreator"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public List<T> ReadMultiColumnDataFromCache<T>(DatabaseTableTypes tableType, Func<T> newObjectCreator, List<T> memoryCache, bool force = false)
        {
            var returnData = memoryCache;
            if (memoryCache.Count == 0 || force || AlwaysRead)
            {
                // Read the data from the cache
                returnData = ReadMultiColumnDataFromCache(tableType, newObjectCreator);

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
        /// <param name="tableType">Table containing the properties</param>
        /// <param name="objectCreator">Method to create a new object of type <typeparamref name="T"/></param>
        /// <returns>List of items read from the table</returns>
        public List<T> ReadMultiColumnDataFromCache<T>(DatabaseTableTypes tableType, Func<T> objectCreator)
        {
            return ReadMultiColumnDataFromCache(tableType, objectCreator, ConnString);
        }

        /// <summary>
        /// Create a list of objects with row data from a cache table
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="tableType">Table containing the properties</param>
        /// <param name="objectCreator">Method to create a new object of type <typeparamref name="T"/></param>
        /// <param name="connString">SQLite database connection string</param>
        /// <returns>List of items read from the table</returns>
        public List<T> ReadMultiColumnDataFromCache<T>(DatabaseTableTypes tableType, Func<T> objectCreator, string connString)
        {
            var returnData = new List<T>();
            var tableName = GetTableName(tableType);

            // Verify table exists in database
            if (!VerifyTableExists(tableName, connString))
            {
                // No table, so return an empty list
                return returnData;
            }

            var type = typeof(T);
            var typeMappings = propToColumnMap.GetPropertyColumnMapping(type);
            var nameMappings = propToColumnMap.GetPropertyColumnNameMapping(type);

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
            var cacheData = GetSQLiteDataTable(sqlStr, connString);
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
        /// Store the contents of a list in the specified table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataList">list of data to be stored</param>
        /// <param name="memoryCache">in-memory cache of the list</param>
        /// <param name="clearFirst">if the existing data should be removed first (always); otherwise, if the row counts match, nothing is changed</param>
        /// <param name="tableType">table the data is to be stored in</param>
        public void SaveMultiColumnListToCache<T>(DatabaseTableTypes tableType, List<T> dataList, List<T> memoryCache, bool clearFirst)
        {
            SaveMultiColumnListToCache(tableType, dataList, clearFirst);

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
        public void SaveMultiColumnListToCache<T>(DatabaseTableTypes tableType, List<T> dataList, bool clearFirst)
        {
            SaveMultiColumnListToCache(tableType, dataList, clearFirst, ConnString);
        }

        /// <summary>
        /// Store the contents of a list in the specified table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataList">list of data to be stored</param>
        /// <param name="clearFirst">if the existing data should be removed first (always); otherwise, if the row counts match, nothing is changed</param>
        /// <param name="tableType">table the data is to be stored in</param>
        /// <param name="connStr">Connection string; used for export/save as</param>
        public void SaveMultiColumnListToCache<T>(DatabaseTableTypes tableType, List<T> dataList, bool clearFirst, string connStr)
        {
            var tableName = GetTableName(tableType);
            if (VerifyTableExists(tableName, connStr, out _, out int rowCount, true) && !clearFirst && dataList.Count <= rowCount)
            {
                return;
            }

            // Clear the cache table
            ClearCacheTable(tableName, connStr);

            //If no data in list, exit
            if (dataList.Count < 1)
            {
                return;
            }

            WriteDataToCache(dataList, tableName, connStr);
        }

        /// <summary>
        /// Generic method for saving a single column list to the cache db
        /// </summary>
        /// <param name="listData">List of data for storing in table</param>
        /// <param name="memoryCache">List used for in-memory cache of contents</param>
        /// <param name="tableType">enumTableNames specifying table name suffix</param>
        /// <param name="clearFirst">if true, the existing data will always be removed from the list; if false and <paramref name="listData"/>.Count is &lt;= to the number of existing rows, nothing is changed</param>
        public void SaveSingleColumnListToCache(DatabaseTableTypes tableType, List<string> listData, List<string> memoryCache, bool clearFirst = true)
        {
            // Refresh the in-memory list with the new data
            if (!AlwaysRead)
            {
                memoryCache.Clear();
                memoryCache.AddRange(listData);
            }

            SaveSingleColumnListToCache(tableType, listData, clearFirst);
        }

        /// <summary>
        /// Generic method for saving a single column list to the cache db
        /// </summary>
        /// <param name="tableType">enumTableNames specifying table name suffix</param>
        /// <param name="listData">List of data for storing in table</param>
        /// <param name="clearFirst">if true, the existing data will always be removed from the list; if false and <paramref name="listData"/>.Count is &lt;= to the number of existing rows, nothing is changed</param>
        /// <remarks>Used with T_CartList, T_SeparationTypeSelected, T_LCColumnList, T_DatasetTypeList, T_DatasetList, and T_CartConfigNameSelected</remarks>
        public void SaveSingleColumnListToCache(DatabaseTableTypes tableType, List<string> listData, bool clearFirst = true)
        {
            // Set up table name
            var tableName = GetTableName(tableType);

            // SQL statement for table clear command
            var sqlClearCmd = "DELETE FROM " + tableName;

            // Build SQL statement for creating table
            var columnName = GetSingleColumnName(tableType);
            string[] colNames = { columnName };
            var sqlCreateCmd = BuildGenericCreateTableCmd(tableName, colNames, columnName, true);

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
        /// Checks if the provided dataset name exists in the cache, case-insensitive
        /// </summary>
        /// <param name="datasetName"></param>
        /// <returns>true if the dataset name exists</returns>
        public bool CheckDatasetExists(string datasetName)
        {
            if (string.IsNullOrWhiteSpace(datasetName))
            {
                return false;
            }

            var tableName = GetTableName(DatabaseTableTypes.DatasetList);
            var columnName = GetSingleColumnName(DatabaseTableTypes.DatasetList);
            using (var connection = GetConnection(ConnString))
            using (var command = connection.CreateCommand())
            using (var transaction = connection.BeginTransaction())
            {
                command.CommandText = $"SELECT COUNT(*) FROM {tableName} WHERE {columnName} LIKE :DatasetName";
                command.Parameters.Add(new SQLiteParameter(":DatasetName", datasetName));
                var result = command.ExecuteScalar();
                // We always expect a result, because COUNT(*) returns 0 for no-match
                return (long) result > 0;
            }
        }

        /// <summary>
        /// Read a single-column list from the cache, handling management of an in-memory list
        /// </summary>
        /// <param name="memoryCache"></param>
        /// <param name="tabletype">DatabaseTableTypes specifying type of table to retrieve</param>
        /// <param name="force"></param>
        /// <returns>List containing cached data</returns>
        public List<string> ReadSingleColumnListFromCache(DatabaseTableTypes tabletype, List<string> memoryCache, bool force = false)
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
        public List<string> ReadSingleColumnListFromCacheCheckExceptions(DatabaseTableTypes tableType)
        {
            try
            {
                return ReadSingleColumnListFromCache(tableType);
            }
            catch (Exception ex)
            {
                CheckExceptionMessageForDbState(ex);
                throw;
            }
        }

        /// <summary>
        /// Generic method for retrieving data from a single column table
        /// </summary>
        /// <param name="tableType">DatabaseTableTypes specifying type of table to retrieve</param>
        /// <returns>List containing cached data</returns>
        public List<string> ReadSingleColumnListFromCache(DatabaseTableTypes tableType)
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

        #endregion
    }
}
