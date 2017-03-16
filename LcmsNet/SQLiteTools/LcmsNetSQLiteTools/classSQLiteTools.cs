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
using LcmsNetDataClasses;
using LcmsNetDataClasses.Data;
using LcmsNetDataClasses.Logging;

namespace LcmsNetSQLiteTools
{
    public static class classSQLiteTools
    {
        #region Properties

        public static string ConnString { get; set; } = "";

        #endregion

        #region Class Variables

        //private static string m_errroString = "";

        private static List<string> m_cartNames;
        private static List<string> m_cartConfigNames;
        private static List<string> m_columnNames;
        private static List<string> m_separationNames;
        private static List<string> m_datasetTypeNames;

        private static List<string> m_datasetNames;

        private static List<classUserInfo> m_userInfo;
        private static List<classInstrumentInfo> m_instrumentInfo;
        private static List<classExperimentData> m_experimentsData;
        private static List<classLCColumn> m_lcColumns;

        private static List<classProposalUser> m_proposalUsers;
        private static Dictionary<string, List<classUserIDPIDCrossReferenceEntry>> m_pidIndexedReferenceList;

        #endregion

        #region Initialize

        /// <summary>
        /// Constructor
        /// </summary>
        static classSQLiteTools()
        {
            Initialize();
        }

        public static void Initialize()
        {
            Initialize("LCMSNet");
        }

        public static void Initialize(string appDataFolderName)
        {
            AppDataFolderName = appDataFolderName;
            CacheName = "LCMSCache.que";

            BuildConnectionString(false);
        }

        /// <summary>
        /// Cache file name or path
        /// </summary>
        /// <remarks>Starts off as a filename, but is changed to a path by BuildConnectionString</remarks>
        public static string CacheName
        {
            get { return classLCMSSettings.GetParameter(classLCMSSettings.PARAM_CACHEFILENAME); }
            private set { classLCMSSettings.SetParameter(classLCMSSettings.PARAM_CACHEFILENAME, value); }
        }

        public static string AppDataFolderName { get; set; }

        public static void BuildConnectionString(bool newCache)
        {
            try
            {
                var name = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_CACHEFILENAME);
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
                    classLCMSSettings.SetParameter(classLCMSSettings.PARAM_CACHEFILENAME, name);
                }

                //workaround for SQLite library version 1.0.93 for network addresses
                if (name.Substring(0, 1) == "\\")
                {
                    name = "\\" + name;
                }
                ConnString = "data source=" + name;
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL,
                    "Could not load the sample queue cache.", ex);
            }
        }

        #endregion

        #region Methods

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
            //m_ConnectionString = "data source=" + location;
        }

        /// <summary>
        /// Retrieves a sample queue from cache database
        /// Connection string and database name are defined by defaults
        /// </summary>
        /// <param name="tableType">tableType enum specifying type of queue to retrieve</param>
        /// <returns>List containing queue data</returns>
        public static List<classSampleData> GetQueueFromCache(enumTableTypes tableType)
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
        public static List<classSampleData> GetQueueFromCache(enumTableTypes tableType, string connectionString)
        {
            var returnData = new List<classSampleData>();

            // Convert type of queue into a data table name
            var tableName = GetTableName(tableType);

            // Get a list of string dictionaries containing properties for each sample
            var allSampleProps = GetPropertiesFromCache(tableName, connectionString);

            // For each row (representing one sample), create a sample data object
            //      and load the property values
            foreach (var sampleProps in allSampleProps)
            {
                // Create a classSampleData object
                var sampleData = new classSampleData(false);

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
        private static IEnumerable<Dictionary<string, string>> GetPropertiesFromCache(string tableName, string connStr)
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
            int columnCount;
            return VerifyTableExists(tableName, connStr, out columnCount);
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
            columnCount = 0;

            var sqlString = "SELECT * FROM sqlite_master WHERE name ='" + tableName + "'";
            DataTable resultSet1;
            try
            {
                // Get a list of database tables matching the specified table name
                resultSet1 = GetSQLiteDataTable(sqlString, connStr);
            }
            catch (Exception ex)
            {
                var errMsg = "SQLite exception verifying table " + tableName + " exists";
                // throw new classDatabaseDataException(errMsg, ex);
                classApplicationLogger.LogError(0, errMsg, ex);
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
                var errMsg = "SQLite exception counting columns in table " + tableName;
                classApplicationLogger.LogError(0, errMsg, ex);
                columnCount = 0;
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
            var FieldNames = firstItem.GetPropertyValues();

            // Verify table exists; if not, create it; Otherwise, clear it
            if (!VerifyTableExists(tableName, connStr))
            {
                // Table doesn't exist, so create it
                var sqlCmd = BuildCreatePropTableCmd(FieldNames, tableName);
                try
                {
                    ExecuteSQLiteCommand(sqlCmd, connStr);
                }
                catch (Exception ex)
                {
                    var errMsg = "SQLite exception creating table " + tableName;
                    classApplicationLogger.LogError(0, errMsg, ex);
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

        /// <summary>
        /// Saves the contents of specified sample queue to the SQLite cache file
        /// Connection string and database name are defined by defaults
        /// </summary>
        /// <param name="QueueData">List of classSampleData containing the sample data to save</param>
        /// <param name="tableType">TableTypes enum specifying which queue is being saved</param>
        public static void SaveQueueToCache(List<classSampleData> QueueData, enumTableTypes tableType)
        {
            SaveQueueToCache(QueueData, tableType, ConnString);
        }

        /// <summary>
        /// Saves the contents of specified sample queue to an SQLite database file
        /// Overload requires database connection string be specified
        /// </summary>
        /// <param name="QueueData">List containing the sample data to save</param>
        /// <param name="tableType">TableTypes enum specifying which queue is being saved</param>
        /// <param name="connStr">Connection string for database file</param>
        public static void SaveQueueToCache(
            List<classSampleData> QueueData,
            enumTableTypes tableType,
            string connStr)
        {
            var DataInList = (QueueData.Count > 0);
            var tableName = GetTableName(tableType);

            // Clear the cache table
            ClearCacheTable(tableName, connStr);

            //If no data in list, just exit
            if (!DataInList)
            {
                return;
            }

            // Convert input data for caching and call cache routine
            var dataList = new List<ICacheInterface>();
            foreach (var currentSample in QueueData)
            {
                dataList.Add(currentSample);
            }
            SavePropertiesToCache(dataList, tableName, connStr, true);
        }

        /// <summary>
        /// Saves a list of users to cache
        /// </summary>
        /// <param name="UserList">List containing user data</param>
        public static void SaveUserListToCache(List<classUserInfo> UserList)
        {
            var dataInList = (UserList.Count > 0);
            var tableName = GetTableName(enumTableTypes.UserList);

            // Clear the cache table
            ClearCacheTable(tableName, ConnString);

            m_userInfo = new List<classUserInfo>(UserList);

            //If no data in list, exit
            if (!dataInList)
            {
                return;
            }

            // Convert input data for caching and call cache routine
            var dataList = new List<ICacheInterface>();
            foreach (var currentUser in UserList)
            {
                dataList.Add(currentUser);
            }
            SavePropertiesToCache(dataList, tableName, ConnString, false);
        }

        public static void SaveExperimentListToCache(List<classExperimentData> expList)
        {
            if (expList == null || expList.Count < 1)
                return;

            var listHasData = expList.Count != 0;
            var tableName = GetTableName(enumTableTypes.ExperimentList);

            // Clear the cache table
            ClearCacheTable(tableName, ConnString);

            m_experimentsData = new List<classExperimentData>(expList);
            // Exit if there's nothing to cache
            if (!listHasData)
                return;

            // Convert input data for caching and call cache routine

            try
            {
                using (var connection = new SQLiteConnection(ConnString))
                {
                    connection.Open();

                    if (!VerifyTableExists(tableName, ConnString))
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText =
                                "CREATE TABLE T_ExperimentList ('Created', 'Experiment', 'ID', 'Organism', 'Reason', 'Request', 'Researcher')";
                            command.ExecuteNonQuery();
                        }
                    }

                    using (var transaction = connection.BeginTransaction())
                    {
                        using (var command = connection.CreateCommand())
                        {
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
                        }
                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0,
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
        public static void SaveProposalUsers(List<classProposalUser> users,
            List<classUserIDPIDCrossReferenceEntry> crossReferenceList,
            Dictionary<string, List<classUserIDPIDCrossReferenceEntry>> pidIndexedReferenceList)
        {
            var userTableName = GetTableName(enumTableTypes.PUserList);
            var referenceTableName = GetTableName(enumTableTypes.PReferenceList);

            ClearCacheTable(userTableName, ConnString);
            ClearCacheTable(referenceTableName, ConnString);

            var userCacheList = new List<ICacheInterface>();
            var referenceCacheList = new List<ICacheInterface>();

            userCacheList.AddRange(users);
            referenceCacheList.AddRange(crossReferenceList);

            SavePropertiesToCache(userCacheList, userTableName, ConnString, false);

            SavePropertiesToCache(referenceCacheList, referenceTableName, ConnString, false);

            m_proposalUsers = users;
            m_pidIndexedReferenceList = pidIndexedReferenceList;
        }

        public static void SaveEntireLCColumnListToCache(List<classLCColumn> lcColumnList)
        {
            var listHasData = lcColumnList.Count != 0;
            var tableName = GetTableName(enumTableTypes.LCColumnList);

            // Clear the cache table
            ClearCacheTable(tableName, ConnString);

            m_lcColumns = new List<classLCColumn>(lcColumnList);
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
        /// <param name="InstList">List of classInstrumentInfo containing instrument data</param>
        public static void SaveInstListToCache(List<classInstrumentInfo> InstList)
        {
            var dataInList = (InstList.Count > 0);
            var tableName = GetTableName(enumTableTypes.InstrumentList);

            // Clear the cache table
            ClearCacheTable(tableName, ConnString, 6);

            m_instrumentInfo = new List<classInstrumentInfo>(InstList);
            //If no data in list, just exit
            if (!dataInList)
            {
                return;
            }

            // Convert input data for caching and call cache routine
            var dataList = new List<ICacheInterface>();
            foreach (var currentInst in InstList)
            {
                dataList.Add(currentInst);
            }
            SavePropertiesToCache(dataList, tableName, ConnString, false);
        }

        /// <summary>
        /// Executes specified SQLite command
        /// </summary>
        /// <param name="CmdStr">SQL statement to execute</param>
        /// <param name="connStr">Connection string for SQL database file</param>
        private static void ExecuteSQLiteCommand(string CmdStr, string connStr)
        {
            using (var Cn = new SQLiteConnection(connStr))
            {
                using (var myCmd = new SQLiteCommand(Cn))
                {
                    myCmd.CommandType = CommandType.Text;
                    myCmd.CommandText = CmdStr;
                    try
                    {
                        myCmd.Connection.Open();
                        var affectedRows = myCmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        var errMsg = "SQLite Exception executing command " + CmdStr;
                        classApplicationLogger.LogError(0, errMsg, ex);
                        throw new classDatabaseDataException(errMsg, ex);
                    }
                    finally
                    {
                        myCmd.Connection.Close();
                    }
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
            using (var connection = new SQLiteConnection(connStr))
            {
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandType = CommandType.Text;
                    try
                    {
                        command.Connection.Open();

                        using (var transaction = connection.BeginTransaction())
                        {
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
                    }
                    catch (Exception ex)
                    {
                        const string errMsg = "SQLite exception adding data";
                        classApplicationLogger.LogError(0, errMsg, ex);
                        throw new classDatabaseDataException(errMsg, ex);
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
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
            using (var connection = new SQLiteConnection(connStr))
            {
                using (var dataAdapter = new SQLiteDataAdapter())
                {
                    using (var command = new SQLiteCommand(cmdStr, connection))
                    {
                        command.CommandType = CommandType.Text;
                        dataAdapter.SelectCommand = command;

                        try
                        {
                            var rowCount = dataAdapter.Fill(returnTable);
                        }
                        catch (Exception ex)
                        {
                            var errMsg = "SQLite exception getting data table via query " + cmdStr + " : " + connStr;
                            classApplicationLogger.LogError(0, errMsg, ex);
                            throw new classDatabaseDataException(errMsg, ex);
                        }
                    }
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
        /// <param name="tableType">enumTableTypes specifying table to get name for</param>
        /// <returns>Name of db table</returns>
        private static string GetTableName(enumTableTypes tableType)
        {
            return "T_" + Enum.GetName(typeof (enumTableTypes), tableType);
        }

        /// <summary>
        /// Wrapper around generic retrieval method specifically for cart lists
        /// </summary>
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
            if (m_cartNames == null)
            {
                m_cartNames = GetSingleColumnListFromCache(enumTableTypes.CartList);
            }
            return m_cartNames;
        }

        /// <summary>
        /// Wrapper around generic retrieval method specifically for cart config name lists
        /// </summary>
        /// <returns>List containing cart config names</returns>
        public static List<string> GetCartConfigNameList(bool force)
        {
            if (m_cartConfigNames == null || force)
            {
                m_cartConfigNames = GetSingleColumnListFromCache(enumTableTypes.CartConfigNameList);
            }
            return m_cartConfigNames;
        }

        /// <summary>
        /// Wrapper around generic retrieval method specifically for LC column lists
        /// </summary>
        /// <returns>List containing cart names</returns>
        public static List<string> GetColumnList(bool force)
        {
            if (m_columnNames == null || force)
            {
                m_columnNames = GetSingleColumnListFromCache(enumTableTypes.ColumnList);
            }
            return m_columnNames;
        }

        /// <summary>
        /// Wrapper around generic retrieval method specifically for separation type lists
        /// </summary>
        /// <returns>List containing separation types</returns>
        public static List<string> GetSepTypeList(bool force)
        {
            if (m_separationNames == null)
            {
                m_separationNames = GetSingleColumnListFromCache(enumTableTypes.SeparationTypeList);
            }
            return m_separationNames;
        }

        /// <summary>
        /// Wrapper around generic retrieval method specifically for dataset name lists
        /// </summary>
        /// <returns>List containing separation types</returns>
        public static List<string> GetDatasetList()
        {
            if (m_datasetNames == null)
            {
                m_datasetNames = GetSingleColumnListFromCache(enumTableTypes.DatasetList);
            }
            return m_datasetNames;
        }

        /// <summary>
        /// Wrapper around generic retrieval method specifically for dataset type lists
        /// </summary>
        /// <returns>List containing dataset types</returns>
        public static List<string> GetDatasetTypeList(bool force)
        {
            if (m_datasetTypeNames == null)
            {
                m_datasetTypeNames = GetSingleColumnListFromCache(enumTableTypes.DatasetTypeList);
            }
            return m_datasetTypeNames;
        }

        /// <summary>
        /// Gets user list from cache
        /// </summary>
        /// <returns>List of user data</returns>
        public static List<classUserInfo> GetUserList(bool force)
        {
            if (m_userInfo == null || force)
            {
                var returnData = new List<classUserInfo>();

                // Get data table name
                var tableName = GetTableName(enumTableTypes.UserList);

                // Get a list of string dictionaries containing properties for each item
                var allUserProps = GetPropertiesFromCache(tableName, ConnString);

                // For each row (representing one user), create a user data object
                //      and load the property values
                foreach (var userProps in allUserProps)
                {
                    // Create a classUserInfo object
                    var userData = new classUserInfo();

                    // Load the user data object from the string dictionary
                    userData.LoadPropertyValues(userProps);

                    // Add the user data object to the return list
                    returnData.Add(userData);
                }
                m_userInfo = returnData;
            }
            // All finished, so return
            return m_userInfo;
        }

        /// <summary>
        /// Gets a list of instruments from the cache
        /// </summary>
        /// <returns>List of instruments</returns>
        public static List<classInstrumentInfo> GetInstrumentList(bool force)
        {
            if (m_instrumentInfo == null)
            {
                var returnData = new List<classInstrumentInfo>();

                // Convert type of list into a data table name
                var tableName = GetTableName(enumTableTypes.InstrumentList);

                // Get a list of string dictionaries containing properties for each instrument
                var allInstProps = GetPropertiesFromCache(tableName, ConnString);

                // For each row (representing one instrument), create an instrument data object
                //      and load the property values
                foreach (var instProps in allInstProps)
                {
                    // Create a classInstrumentInfo object
                    var instData = new classInstrumentInfo();

                    // Load the instrument data object from the string dictionary
                    instData.LoadPropertyValues(instProps);

                    // Add the instrument data object to the return list
                    returnData.Add(instData);
                }

                // All finished, so return
                m_instrumentInfo = returnData;
            }
            return m_instrumentInfo;
        }

        public static List<classExperimentData> GetExperimentList()
        {
            if (m_experimentsData == null)
            {
                var returnData = new List<classExperimentData>();

                var tableName = GetTableName(enumTableTypes.ExperimentList);

                var allExpProperties = GetPropertiesFromCache(tableName, ConnString);

                foreach (var props in allExpProperties)
                {
                    var expDatum = new classExperimentData();

                    expDatum.LoadPropertyValues(props);

                    returnData.Add(expDatum);
                }

                m_experimentsData = returnData;
            }

            return m_experimentsData;
        }

        public static void GetProposalUsers(
            out List<classProposalUser> users,
            out Dictionary<string, List<classUserIDPIDCrossReferenceEntry>> pidIndexedReferenceList)
        {
            if (m_proposalUsers != null && m_proposalUsers.Count > 0 && m_pidIndexedReferenceList != null &&
                m_pidIndexedReferenceList.Count > 0)
            {
                users = m_proposalUsers;
                pidIndexedReferenceList = m_pidIndexedReferenceList;
            }
            else
            {
                var crossReferenceList = new List<classUserIDPIDCrossReferenceEntry>();
                pidIndexedReferenceList = new Dictionary<string, List<classUserIDPIDCrossReferenceEntry>>();

                users = new List<classProposalUser>();
                var userTableName = GetTableName(enumTableTypes.PUserList);
                var referenceTableName = GetTableName(enumTableTypes.PReferenceList);

                var userExpProperties = GetPropertiesFromCache(userTableName, ConnString);

                var referenceExpProperties = GetPropertiesFromCache(referenceTableName, ConnString);

                foreach (var props in userExpProperties)
                {
                    var datum = new classProposalUser();
                    datum.LoadPropertyValues(props);
                    users.Add(datum);
                }

                foreach (var props in referenceExpProperties)
                {
                    var datum = new classUserIDPIDCrossReferenceEntry();
                    datum.LoadPropertyValues(props);
                    crossReferenceList.Add(datum);
                }

                foreach (var crossReference in crossReferenceList)
                {
                    if (!pidIndexedReferenceList.ContainsKey(crossReference.PID))
                    {
                        pidIndexedReferenceList.Add(
                            crossReference.PID,
                            new List<classUserIDPIDCrossReferenceEntry>());
                    }

                    pidIndexedReferenceList[crossReference.PID].Add(crossReference);
                }

                m_pidIndexedReferenceList = pidIndexedReferenceList;
                m_proposalUsers = users;
            }
        }

        public static List<classLCColumn> GetEntireLCColumnList()
        {
            if (m_experimentsData == null)
            {
                var returnData = new List<classLCColumn>();

                var tableName = GetTableName(enumTableTypes.LCColumnList);

                var allLCColumnProperties = GetPropertiesFromCache(tableName, ConnString);

                foreach (var props in allLCColumnProperties)
                {
                    var datum = new classLCColumn();
                    datum.LoadPropertyValues(props);
                    returnData.Add(datum);
                }

                m_lcColumns = returnData;
            }

            return m_lcColumns;
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

            SaveSingleColumnListToCache(cartConfigTypes, enumTableTypes.CartConfigNameSelected);
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
                cartConfigNames = GetSingleColumnListFromCache(enumTableTypes.CartConfigNameSelected);
            }
            catch
            {
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

            SaveSingleColumnListToCache(sepTypes, enumTableTypes.SeparationTypeSelected);
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
                sepType = GetSingleColumnListFromCache(enumTableTypes.SeparationTypeSelected);
            }
            catch (Exception ex)
            {
                var firstTimeLookupedSelectedSepType = classLCMSSettings.GetParameter(classLCMSSettings.PARAM_FIRSTTIME_LOOKUP_SELECTED_SEP_TYPE);

                var isFirstTime = true;
                if (!string.IsNullOrWhiteSpace(firstTimeLookupedSelectedSepType))
                {
                    isFirstTime = Convert.ToBoolean(firstTimeLookupedSelectedSepType);
                }

                if (!isFirstTime)
                {
                    const string errorMessage =
                        "Exception getting default separation type. (NOTE: This is normal if a new cache is being used)";
                    classApplicationLogger.LogError(0, errorMessage, ex);
                }
                else
                {
                    classLCMSSettings.SetParameter(classLCMSSettings.PARAM_FIRSTTIME_LOOKUP_SELECTED_SEP_TYPE, false.ToString());
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
        /// <param name="ListData">List of data for storing in table</param>
        /// <remarks>Used with T_CartList, T_SeparationTypeSelected, T_LCColumnList, T_DatasetTypeList, T_DatasetList, and T_CartConfigNameSelected</remarks>
        public static void SaveSingleColumnListToCache(List<string> ListData, enumTableTypes tableType)
        {
            const string GENERIC_COLUMN_NAME = "Column1";

            // Set up table name
            var tableName = GetTableName(tableType);

            // SQL statement for table clear command
            var sqlClearCmd = "DELETE FROM " + tableName;

            // Build SQL statement for creating table
            string[] colNames = {GENERIC_COLUMN_NAME};
            var sqlCreateCmd = BuildGenericCreateTableCmd(tableName, colNames, GENERIC_COLUMN_NAME);

            // If table exists, clear it. Otherwise create one
            if (VerifyTableExists(tableName, ConnString))
            {
                // Clear table
                try
                {
                    ExecuteSQLiteCommand(sqlClearCmd, ConnString);
                }
                catch (Exception ex)
                {
                    var errMsg = "SQLite exception clearing table via command " + sqlClearCmd;
                    // throw new classDatabaseDataException(errMsg, ex);
                    classApplicationLogger.LogError(0, errMsg, ex);
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
                    var errMsg = "SQLite exception creating table " + tableName;
                    // throw new classDatabaseDataException(errMsg, ex);
                    classApplicationLogger.LogError(0, errMsg, ex);
                    return;
                }
            }

            // Fill the data table
            const int MAX_ROWS_PER_TRANSACTION = 100000;

            var cmdList = new List<string>();
            foreach (var itemName in ListData)
            {
                var sqlInsertCmd = "INSERT INTO " + tableName + " values('" + itemName + "')";
                cmdList.Add(sqlInsertCmd);

                if (cmdList.Count >= MAX_ROWS_PER_TRANSACTION)
                {
                    StoreCmdListData(ConnString, tableName, cmdList);
                    cmdList.Clear();
                }
            }

            StoreCmdListData(ConnString, tableName, cmdList);
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
                var errMsg = "SQLite exception filling table " + tableName;
                // throw new classDatabaseDataException(errMsg, ex);
                classApplicationLogger.LogError(0, errMsg, ex);
            }
        }

        /// <summary>
        /// Generic method for retrieving data from a single column table
        /// </summary>
        /// <param name="tableType">enumTableTypes specifying type of table to retrieve</param>
        /// <returns>List containing cached data</returns>
        private static List<string> GetSingleColumnListFromCache(enumTableTypes tableType)
        {
            var returnList = new List<string>();

            // Set up table name
            var tableName = GetTableName(tableType);

            // Verify specified table exists
            if (!VerifyTableExists(tableName, ConnString))
            {
                var errMsg = "Data table " + tableName + " not found in cache";
                throw new classDatabaseDataException(errMsg, new Exception());
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
                var errMsg = "SQLite exception getting data table via query " + sqlQueryCmd;
                throw new classDatabaseDataException(errMsg, ex);
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
                    var errorMessage = "Exception clearing table " + tableName;
                    classApplicationLogger.LogError(0, errorMessage, ex);
                    throw new classDatabaseDataException("Exception clearing table " + tableName, ex);
                }
            }
        }

        #endregion
    }
}
