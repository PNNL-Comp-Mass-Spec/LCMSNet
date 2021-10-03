using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using LcmsNet.IO.DMS;
using LcmsNet.IO.DMS.Data;
using LcmsNetSDK.Logging;

namespace LcmsNet.IO.SQLite
{
    internal class SQLiteCacheIO : IDisposable
    {
        // ReSharper disable once CommentTypo
        // Ignore Spelling: pragma, sqlite, sql

        private SQLiteConnection storedConnection;
        private string lastConnectionString = "";

        private string cacheFullPath;

        private readonly PropertyToColumnMapping propToColumnMap = new PropertyToColumnMapping();

        private Func<string> defaultDirectoryPathGetMethod = () => ".";

        #region Properties

        /// <summary>
        /// Cache file name or path
        /// </summary>
        /// <remarks>Starts off as a filename, but is changed to a path by BuildConnectionString</remarks>
        public string CacheName { get; private set; }

        public string ConnString { get; set; } = "";
        public bool DatabaseImageBad { get; private set; }
        public bool DisableInMemoryCaching { get; set; }

        public bool AlwaysRead => !DatabaseImageBad && DisableInMemoryCaching;

        public string DefaultDirectoryPath => defaultDirectoryPathGetMethod();

        public void SetDefaultDirectoryPath(string path)
        {
            defaultDirectoryPathGetMethod = () => path;
        }

        public void SetDefaultDirectoryPath(Func<string> pathGetMethod)
        {
            defaultDirectoryPathGetMethod = pathGetMethod;
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
                var basePath = DefaultDirectoryPath;
                var fileName = Path.GetFileName(location);
                location = Path.Combine(basePath, fileName);
            }

            CacheName = location;

            BuildConnectionString(!File.Exists(location));
        }

        #endregion

        #region Initialize and Dispose

        public SQLiteCacheIO()
        {
            Initialize();
        }

        /// <summary>
        /// Initialize the cache, with the provided cache filename
        /// </summary>
        /// <param name="cacheName"></param>
        public void Initialize(string cacheName = "LCMSCache.que")
        {
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
                var name = CacheName;
                var exists = File.Exists(name);
                if (!exists && !newCache)
                {
                    var basePath = DefaultDirectoryPath;
                    if (!Directory.Exists(basePath))
                    {
                        Directory.CreateDirectory(basePath);
                    }

                    name = Path.Combine(basePath, CacheName);
                    CacheName = name;
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
            private readonly bool closeConnectionOnDispose;

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

            var sqlString = "SELECT COUNT(*) FROM sqlite_master WHERE name ='" + tableName + "'";
            try
            {
                // Get a list of database tables matching the specified table name
                var tableMatches = (long)ExecuteSQLiteCommandScalar(sqlString, connStr);
                if (tableMatches < 1)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                CheckExceptionMessageForDbState(ex);
                var errMsg = "SQLite exception verifying table " + tableName + " exists";
                // throw new DatabaseDataException(errMsg, ex);
                ApplicationLogger.LogError(0, errMsg, ex);
                return false;
            }

            // Exactly 1 row returned; examine the number of columns
            // Count the number of columns
            //var colCountSql = "pragma table_info(" + tableName + ")";
            // Requires SQLite 3.16.0 or newer:
            var colCountSql = "SELECT COUNT(*) FROM pragma_table_info('" + tableName + "')";

            try
            {
                // Use the pragma statement to get a table with one row per column
                //using (var resultSet2 = GetSQLiteDataTable(colCountSql, connStr))
                //{
                //    columnCount = resultSet2.Rows.Count;
                //}

                // Cast to long (because the object will be a boxed long) and then to int.
                columnCount = (int)(long)ExecuteSQLiteCommandScalar(colCountSql, connStr);
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
                // Cast to long (because the object will be a boxed long) and then to int.
                rowCount = (int)(long)ExecuteSQLiteCommandScalar(rowCountSql, connStr);
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
        /// Determines if a particular table exists in the SQLite database, and is in the expected format (column names, data types, etc.)
        /// </summary>
        /// <param name="tableName">Name of the table to search for</param>
        /// <param name="connStr">Connection string for database</param>
        /// <param name="createCommand">Table creation text, to compare to the create command of the existing table</param>
        /// <returns>TRUE if table found and create command matches (ignore case); FALSE if not found or error</returns>
        private bool VerifyTableFormat(string tableName, string connStr, string createCommand)
        {
            var sqlString = "SELECT sql FROM sqlite_master WHERE name ='" + tableName + "'";
            try
            {
                var createText = ExecuteSQLiteCommandScalar(sqlString, connStr)?.ToString();
                if (string.IsNullOrWhiteSpace(createText))
                {
                    return false;
                }

                return createText.Equals(createCommand, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                CheckExceptionMessageForDbState(ex);
                var errMsg = "SQLite exception verifying table format for table " + tableName;
                // throw new DatabaseDataException(errMsg, ex);
                ApplicationLogger.LogError(0, errMsg, ex);
                return false;
            }
        }

        /// <summary>
        /// Check if the table has the correct column count and names
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="connStr"></param>
        /// <param name="fieldNames"></param>
        /// <returns></returns>
        private bool TableColumnNamesMatched(string tableName, string connStr, IReadOnlyCollection<string> fieldNames)
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

            var sqlString = "SELECT name FROM pragma_table_info('" + tableName + "')";
            try
            {
                using (var connection = GetConnection(connStr))
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sqlString;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            colNames.Add(reader.GetString(0));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CheckExceptionMessageForDbState(ex);
                var errMsg = "SQLite exception verifying table " + tableName + " exists";
                // throw new DatabaseDataException(errMsg, ex);
                ApplicationLogger.LogError(0, errMsg, ex);
                return colNames;
            }

            return colNames;
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
        /// Executes specified SQLite command that returns a scalar value
        /// </summary>
        /// <param name="cmdStr">SQL command to execute</param>
        /// <param name="connStr">Connection string for SQLite database file</param>
        /// <returns>An object returned by the SQLite command - the value of the first row and column of the table; may be null.</returns>
        private object ExecuteSQLiteCommandScalar(string cmdStr, string connStr)
        {
            object value;
            using (var connection = GetConnection(connStr))
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = cmdStr;

                try
                {
                    value = command.ExecuteScalar();
                    if (value is DBNull)
                    {
                        value = null;
                    }
                }
                catch (Exception ex)
                {
                    CheckExceptionMessageForDbState(ex);
                    var errMsg = "SQLite exception getting scalar value via query " + cmdStr + " : " + connStr;
                    ApplicationLogger.LogError(0, errMsg, ex);
                    throw new DatabaseDataException(errMsg, ex);
                }
            }

            // Everything worked, so return the value
            return value;
        }

        /// <summary>
        /// Retrieves a data table from a SQLite database
        /// </summary>
        /// <param name="cmdStr">SQL command to execute</param>
        /// <param name="connStr">Connection string for SQLite database file</param>
        /// <returns>A DataTable containing data specified by the SQL command</returns>
        /// <remarks>Works well, but it also uses more memory and processing time vs. reading directly to the output object</remarks>
        [Obsolete("Unused")]
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
        private string BuildCreatePropTableCmd(IEnumerable<string> inpData, string tableName)
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
            sb.AppendFormat("CREATE TABLE {0} (", tableName);

            var textToAppend = "";
            if (caseInsensitive)
            {
                textToAppend = " COLLATE NOCASE";
            }

            // Create column names for each key, which is same as property name in queue being saved
            sb.Append(string.Join(",", colNames.Select(x => $"'{x}' TEXT{textToAppend}")));

            if (!string.IsNullOrWhiteSpace(primaryKeyColumn))
            {
                sb.AppendFormat(", PRIMARY KEY('{0}')", primaryKeyColumn);
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
        /// <param name="connString"></param>
        /// <returns></returns>
        private string GetSingleColumnName(DatabaseTableTypes tableType, string connString)
        {
            var names = GetTableColumnNames(GetTableName(tableType), connString, out _);
            if (names.Count == 0)
            {
                var columnName = "" + Enum.GetName(typeof(DatabaseTableTypes), tableType);
                if (columnName.EndsWith("List", StringComparison.OrdinalIgnoreCase) &&
                    columnName.IndexOf("name", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    columnName = columnName.Substring(0, columnName.Length - 4) + "Name";
                }

                return columnName;
            }

            return names[0];
        }

        /// <summary>
        /// Checks for existing multi-column table, creates it if it doesn't exist, and optionally drops existing data
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="connStr"></param>
        /// <param name="dataType"></param>
        /// <param name="dropOnMismatch">If true, the table exists, and the column names don't match property names, drops and re-creates the table</param>
        /// <param name="clearExisting">If true and the table exists, the existing data will be truncated</param>
        /// <returns>True if table exists and is readable</returns>
        private bool PrepareMultiColumnTable(string tableName, string connStr, Type dataType, bool dropOnMismatch = true, bool clearExisting = true)
        {
            var mappings = propToColumnMap.GetPropertyColumnMapping(dataType).Values.ToList();
            return PrepareMultiColumnTable(tableName, connStr, mappings, dropOnMismatch, clearExisting);
        }

        /// <summary>
        /// Checks for existing multi-column table, creates it if it doesn't exist, and optionally drops existing data
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="connStr"></param>
        /// <param name="mappings"></param>
        /// <param name="dropOnMismatch">If true, the table exists, and the column names don't match property names, drops and re-creates the table</param>
        /// <param name="clearExisting">If true and the table exists, the existing data will be truncated</param>
        /// <returns>True if table exists and is readable</returns>
        private bool PrepareMultiColumnTable(
            string tableName,
            string connStr,
            IEnumerable<PropertyToColumnMapping.PropertyColumnMapping> mappings,
            bool dropOnMismatch = true,
            bool clearExisting = true)
        {
            // Verify table exists, and column names are correct; if not, create it; Otherwise, clear it
            var tableExists = VerifyTableExists(tableName, connStr);
            var fieldNames = mappings.Select(x => x.ColumnName).ToList();
            var tableColumnsCorrect = !tableExists || TableColumnNamesMatched(tableName, connStr, fieldNames); // if the table doesn't exist, automatically set this to true
            if (tableExists && tableColumnsCorrect && clearExisting)
            {
                // Clear the table (note that SQLite does not have command "Truncate Table")
                var sqlStr = "DELETE FROM " + tableName;
                try
                {
                    ExecuteSQLiteCommand(sqlStr, connStr);
                }
                catch (Exception ex)
                {
                    CheckExceptionMessageForDbState(ex);
                    var errorMessage = "Exception clearing table " + tableName;
                    ApplicationLogger.LogError(0, errorMessage, ex);
                    return false;
                }
            }
            else if (!tableExists || (!tableColumnsCorrect && dropOnMismatch))
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
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks for existing single-column table, creates it if it doesn't exist, and optionally drops existing data
        /// </summary>
        /// <param name="tableType"></param>
        /// <param name="connStr"></param>
        /// <param name="dropOnMismatch">If true, the table exists, and the column name doesn't match desired name, drops and re-creates the table</param>
        /// <param name="clearExisting">If true and the table exists, the existing data will be truncated</param>
        /// <returns>True if table exists and is readable</returns>
        private bool PrepareSingleColumnTable(DatabaseTableTypes tableType, string connStr, bool dropOnMismatch = true, bool clearExisting = true)
        {
            var tableName = GetTableName(tableType);

            // Build SQL statement for creating table
            var columnName = GetSingleColumnName(tableType, connStr);
            string[] colNames = { columnName };
            var sqlCreateCmd = BuildGenericCreateTableCmd(tableName, colNames, columnName, true);

            // If table exists, clear it. Otherwise create one
            var tableFormatGood = VerifyTableFormat(tableName, connStr, sqlCreateCmd);
            var tableExists = VerifyTableExists(tableName, ConnString, out _, out _, false);
            if (tableExists && tableFormatGood)
            {
                if (!clearExisting)
                {
                    return true;
                }

                // SQL statement for table clear command
                var sqlClearCmd = "DELETE FROM " + tableName;

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
                    return false;
                }
            }
            else
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (tableExists && !tableFormatGood)
                {
                    if (!dropOnMismatch)
                    {
                        return true;
                    }

                    // Table column name wrong, or type/options incorrect; drop the table and re-create it.
                    sqlCreateCmd = $"DROP TABLE {tableName}; " + sqlCreateCmd;
                }

                // Create table
                try
                {
                    ExecuteSQLiteCommand(sqlCreateCmd, connStr);
                }
                catch (Exception ex)
                {
                    CheckExceptionMessageForDbState(ex);
                    var errMsg = "SQLite exception creating table " + tableName;
                    // throw new DatabaseDataException(errMsg, ex);
                    ApplicationLogger.LogError(0, errMsg, ex);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if a cache table exists, creating it if it doesn't (and writing the default data if there is no data present)
        /// </summary>
        /// <param name="tableType"></param>
        /// <param name="defaultData">Entries to add to the table if there are none present</param>
        /// <returns>table row count</returns>
        private void CheckSingleColumnCacheTable(DatabaseTableTypes tableType, IEnumerable<string> defaultData)
        {
            // Set up table name
            var tableName = GetTableName(tableType);

            // If table exists, clear it. Otherwise create one
            if (!VerifyTableExists(tableName, ConnString, out _, out var rowCount, true))
            {
                if (!PrepareSingleColumnTable(tableType, ConnString, false, false))
                {
                    rowCount = -1;
                }
            }

            if (rowCount < 1)
            {
                SaveSingleColumnListToCache(tableType, defaultData);
            }
        }

        private void CheckMultiColumnCacheTable<T>(DatabaseTableTypes tableType)
        {
            var tableName = GetTableName(tableType);
            if (!VerifyTableExists(tableName, ConnString, out _, out _, false))
            {
                PrepareMultiColumnTable(tableName, ConnString, typeof(T), false, false);
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
        /// Checks for an existing cache or creates a new one, and makes sure certain tables exist.
        /// </summary>
        /// <param name="defaultData"></param>
        public void CheckOrCreateCache(SQLiteCacheDefaultData defaultData = null)
        {
            var writeData = defaultData ?? new SQLiteCacheDefaultData();
            CheckSingleColumnCacheTable(DatabaseTableTypes.CartList, writeData.CartNames);
            CheckSingleColumnCacheTable(DatabaseTableTypes.SeparationTypeList, writeData.SeparationTypes);
            CheckSingleColumnCacheTable(DatabaseTableTypes.DatasetTypeList, writeData.DatasetTypes);
            CheckSingleColumnCacheTable(DatabaseTableTypes.ColumnList, writeData.ColumnNames);
        }

        /// <summary>
        /// Read the data for a list from the cache, handling the in-memory cache appropriately
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableType"></param>
        /// <param name="newObjectCreator"></param>
        /// <param name="memoryCache"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public IEnumerable<T> ReadMultiColumnDataFromCache<T>(DatabaseTableTypes tableType, Func<T> newObjectCreator, List<T> memoryCache, bool force = false)
        {
            var returnData = (IEnumerable<T>)memoryCache;
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
                    returnData = memoryCache;
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
        public IEnumerable<T> ReadMultiColumnDataFromCache<T>(DatabaseTableTypes tableType, Func<T> objectCreator)
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
        public IEnumerable<T> ReadMultiColumnDataFromCache<T>(DatabaseTableTypes tableType, Func<T> objectCreator, string connString)
        {
            var tableName = GetTableName(tableType);

            // Verify table exists in database
            if (!VerifyTableExists(tableName, connString))
            {
                // No table, so return an empty list
                yield break;
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

            if (nameMappings.TryGetValue("dms.emslproposaluser", out var eusUser) && !nameMappings.ContainsKey("dms.userlist"))
            {
                nameMappings.Add("dms.userlist", eusUser);
            }

            // Get table containing cached data
            var sqlStr = "SELECT * FROM " + tableName;

            using (var connection = GetConnection(connString))
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = sqlStr;
                SQLiteDataReader reader;

                try
                {
                    // Get a table from the cache db
                    reader = command.ExecuteReader();
                }
                catch (Exception ex)
                {
                    CheckExceptionMessageForDbState(ex);
                    var errMsg = "SQLite exception getting data table via query " + sqlStr + " : " + connString;
                    ApplicationLogger.LogError(0, errMsg, ex);
                    throw new DatabaseDataException(errMsg, ex);
                }

                if (!reader.HasRows)
                {
                    // No cached data found, so return an empty list
                    yield break;
                }

                KeyValuePair<int, Action<object, object>>[] columnMappings = null;
                var deDupDictionary = new Dictionary<string, string>();

                // Return the data; less likely to encounter exceptions here.
                // For each row (representing properties and values for one sample), create a string dictionary
                //          with the object's properties from the table columns, and add it to the return list
                using (reader)
                {
                    while (reader.Read())
                    {
                        if (columnMappings == null)
                        {
                            var columnCount = reader.VisibleFieldCount;
                            columnMappings = new KeyValuePair<int, Action<object, object>>[columnCount];
                            var columnSetOrder = new int[columnCount];

                            // Create the column mappings, for fast access to set methods
                            for (var i = 0; i < columnCount; i++)
                            {
                                var columnName = reader.GetName(i);
                                if (!nameMappings.TryGetValue(columnName.ToLower(), out var tempName))
                                {
                                    // The property that populated the column no longer exists. Report an error and continue.
                                    ApplicationLogger.LogError(LogLevel.Warning, $"Could not find the property to store data from column {columnName} in SQLite table {tableName}!");
                                    continue;
                                }
                                var properName = tempName;
                                var setProp = typeMappings[properName].SetProperty;
                                var setMethod = setProp;
                                if (typeMappings[properName].DoStringDeDuplication)
                                {
                                    setMethod = (obj, val) => setProp(obj, (val as string).LimitStringDuplication(deDupDictionary));
                                }
                                columnMappings[i] = new KeyValuePair<int, Action<object, object>>(i, setMethod);
                                columnSetOrder[i] = i;

                                if (properName.IndexOf("RunningStatus", StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    // Always process running status last
                                    columnSetOrder[i] = int.MaxValue;
                                }
                            }

                            Array.Sort(columnSetOrder, columnMappings);
                        }

                        // Create a new object
                        var data = objectCreator();

                        // Populate the properties from the cache
                        foreach (var column in columnMappings.Where(x => x.Value != null))
                        {
                            var value = reader[column.Key];
                            var setMethod = column.Value;
                            setMethod(data, value);
                        }

                        // return the data
                        yield return data;
                    }
                }
            }
        }

        /// <summary>
        /// Store the contents of a list in the specified table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableType">table the data is to be stored in</param>
        /// <param name="dataList">list of data to be stored</param>
        /// <param name="memoryCache">in-memory cache of the list</param>
        public void SaveMultiColumnListToCache<T>(DatabaseTableTypes tableType, IEnumerable<T> dataList, List<T> memoryCache)
        {
            if (!AlwaysRead)
            {
                memoryCache.Clear();
                memoryCache.AddRange(dataList);
                // Change the enumerable used to write to the cache
                dataList = memoryCache;
            }

            SaveMultiColumnListToCache(tableType, dataList);
        }

        /// <summary>
        /// Store the contents of a list in the specified table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableType">table the data is to be stored in</param>
        /// <param name="dataList">list of data to be stored</param>
        public void SaveMultiColumnListToCache<T>(DatabaseTableTypes tableType, IEnumerable<T> dataList)
        {
            SaveMultiColumnListToCache(tableType, dataList, ConnString);
        }

        /// <summary>
        /// Store the contents of a list in the specified table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableType">table the data is to be stored in</param>
        /// <param name="dataList">list of data to be stored</param>
        /// <param name="connStr">Connection string; used for export/save as</param>
        public void SaveMultiColumnListToCache<T>(DatabaseTableTypes tableType, IEnumerable<T> dataList, string connStr)
        {
            // Set up table name
            var tableName = GetTableName(tableType);
            var firstItem = true;

            // Fill the data table
            using (var connection = GetConnection(connStr))
            using (var command = connection.CreateCommand())
            using (var transaction = connection.BeginTransaction())
            {
                var mappings = propToColumnMap.GetPropertyColumnMapping(typeof(T)).Values.ToList();
                // TODO: GUTS!!!!

                try
                {
                    command.CommandText = $"INSERT INTO {tableName}({string.Join(",", mappings.Select(x => $"'{x.ColumnName}'"))}) VALUES ({string.Join(",", mappings.Select(x => $":{x.ColumnName.Replace(".", "")}"))})";

                    foreach (var item in dataList)
                    {
                        if (firstItem)
                        {
                            // Only clears the table if we have at least one item we are adding.
                            // Verify table exists, and column names are correct; if not, create it; Otherwise, clear it
                            if (!PrepareMultiColumnTable(tableName, connStr, mappings, true, true))
                            {
                                return;
                            }
                            firstItem = false;
                        }

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
        /// Generic method for saving a single column list to the cache db
        /// </summary>
        /// <param name="tableType">enumTableNames specifying table name suffix</param>
        /// <param name="listData">List of data for storing in table</param>
        /// <param name="memoryCache">List used for in-memory cache of contents</param>
        public void SaveSingleColumnListToCache(DatabaseTableTypes tableType, IEnumerable<string> listData, List<string> memoryCache)
        {
            // Refresh the in-memory list with the new data
            if (!AlwaysRead)
            {
                memoryCache.Clear();
                memoryCache.AddRange(listData);
                // Change the enumerable used to write to the cache
                listData = memoryCache;
            }

            SaveSingleColumnListToCache(tableType, listData);
        }

        /// <summary>
        /// Generic method for saving a single column list to the cache db
        /// </summary>
        /// <param name="tableType">enumTableNames specifying table name suffix</param>
        /// <param name="listData">List of data for storing in table</param>
        /// <remarks>Used with T_CartList, T_SeparationTypeSelected, T_LCColumnList, T_DatasetTypeList, T_DatasetList, and T_CartConfigNameSelected</remarks>
        public void SaveSingleColumnListToCache(DatabaseTableTypes tableType, IEnumerable<string> listData)
        {
            // Set up table name
            var tableName = GetTableName(tableType);
            var firstItem = true;

            // Fill the data table
            using (var connection = GetConnection(ConnString))
            using (var command = connection.CreateCommand())
            using (var transaction = connection.BeginTransaction())
            {
                command.CommandText = $"INSERT INTO {tableName} VALUES(:Value)";
                foreach (var item in listData)
                {
                    if (firstItem)
                    {
                        // Only clears the table if we have at least one item we are adding.
                        if (!PrepareSingleColumnTable(tableType, ConnString, true, true))
                        {
                            return;
                        }

                        firstItem = false;
                    }

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
        /// <param name="tableType">DatabaseTableTypes specifying type of table to retrieve</param>
        /// <param name="memoryCache"></param>
        /// <param name="force"></param>
        /// <returns>List containing cached data</returns>
        public IEnumerable<string> ReadSingleColumnListFromCache(DatabaseTableTypes tableType, List<string> memoryCache, bool force = false)
        {
            var data = (IEnumerable<string>)memoryCache;
            if (memoryCache.Count == 0 || force || AlwaysRead)
            {
                data = ReadSingleColumnListFromCache(tableType);

                memoryCache.Clear();
                if (AlwaysRead)
                {
                    memoryCache.Capacity = 0;
                }
                else
                {
                    memoryCache.AddRange(data);
                    data = memoryCache;
                }
            }
            return data;
        }

        /// <summary>
        /// Generic method for retrieving data from a single column table
        /// </summary>
        /// <param name="tableType">DatabaseTableTypes specifying type of table to retrieve</param>
        /// <returns>List containing cached data</returns>
        public IEnumerable<string> ReadSingleColumnListFromCacheCheckExceptions(DatabaseTableTypes tableType)
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
        public IEnumerable<string> ReadSingleColumnListFromCache(DatabaseTableTypes tableType)
        {
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

            using (var connection = GetConnection(ConnString))
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = sqlQueryCmd;
                SQLiteDataReader reader;

                try
                {
                    // Get a table from the cache db
                    reader = command.ExecuteReader();
                }
                catch (Exception ex)
                {
                    CheckExceptionMessageForDbState(ex);
                    var errMsg = "SQLite exception getting single column table via query " + sqlQueryCmd + " : " + ConnString;
                    ApplicationLogger.LogError(0, errMsg, ex);
                    throw new DatabaseDataException(errMsg, ex);
                }

                // Return the data; less likely to encounter exceptions here.
                using (reader)
                {
                    while (reader.Read())
                    {
                        // Only return the first column.
                        yield return reader.GetString(0);
                    }
                }
            }
        }

        #endregion
    }
}
