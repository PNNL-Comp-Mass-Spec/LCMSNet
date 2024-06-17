using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using LcmsNetSDK.Logging;
using LcmsNetSDK.System;
using PRISM;
using PRISMDatabaseUtils;

namespace LcmsNet.IO.DMS
{
    internal class DMSDBConnection
    {
        public static string ApplicationName { get; set; } = "-LcmsNet- -version-";

        private bool mConnectionStringLogged;

        private const string CONFIG_FILE = "PrismDMS.json";
        private const string CENTRAL_CONFIG_FILE_PATH = @"\\proto-5\BionetSoftware\Buzzard\PrismDMS.json";

        public string ErrMsg { get; private set; } = "";

        public string SchemaPrefix => mConfiguration.DatabaseSchemaPrefix;

        public string DMSDatabase => mConfiguration.DatabaseName;

        private DMSConfig mConfiguration;

        private IDBTools dbTools = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public DMSDBConnection()
        {
            mConfiguration = new DMSConfig();
            LoadLocalConfiguration();
        }

        private string connectionString = "";
        private bool connectionStringUpdated = true;
        private DateTime lastConnectionAttempt = DateTime.MinValue;
        private readonly TimeSpan minTimeBetweenConnectionAttempts = TimeSpan.FromSeconds(30);
        private DateTime connectionStringLoadTime = DateTime.MinValue;
        private string cleanConnectionString = "";

        private void ConnectionErrorEvent(string message, Exception ex)
        {
            ErrMsg = message;

            if (ex != null)
            {
                ErrMsg += $"; {ex.Message}";
            }

            ApplicationLogger.LogError(0, ErrMsg);
        }

        /// <summary>
        /// Get a SQLiteConnection, but control creation of new connections based on UseConnectionPooling
        /// </summary>
        /// <returns></returns>
        private IDBTools GetConnection()
        {
            var updated = RefreshConfiguration();

            if (dbTools != null && !updated && !connectionStringUpdated)
            {
                try
                {
                    if (dbTools.TestDatabaseConnection(1))
                    {
                        ErrMsg = "";
                        return dbTools;
                    }
                }
                catch (Exception ex)
                {
                    ApplicationLogger.LogError(LogLevel.Warning, "Error connecting to DMS.", ex);
                    // TODO: throw a DatabaseConnectionStringException ? - but only if it is actually the connection string, not a 'no connection' issue
                }

                // Assuming temporary failure of the connection
                return null;
            }

            if (dbTools == null && (DateTime.UtcNow <= lastConnectionAttempt.Add(minTimeBetweenConnectionAttempts) && !updated))
            {
                // Prevent multiple checks for a database connection in close succession
                return null;
            }

            var db = DbToolsFactory.GetDBTools(mConfiguration.DatabaseSoftware, connectionString);
            db.ErrorEvent += ConnectionErrorEvent;
            lastConnectionAttempt = DateTime.UtcNow;

            try
            {
                if (db.TestDatabaseConnection(1))
                {
                    dbTools = db;
                    connectionStringUpdated = false;
                    ErrMsg = "";
                }
                else
                {
                    dbTools = null;
                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(LogLevel.Warning, "Error connecting to DMS.", ex);
                dbTools = null;
                // TODO: throw a DatabaseConnectionStringException ? - but only if it is actually the connection string, not a 'no connection' issue
            }

            return dbTools;
        }

        /// <summary>
        /// Checks for updates to the connection configuration if it hasn't been done recently
        /// </summary>
        /// <returns>True if the connection configuration was updated and is different from the previous configuration</returns>
        public bool RefreshConnectionConfiguration()
        {
            return RefreshConfiguration();
        }

        /// <summary>
        /// Gets DMS connection string from config file, excluding the password
        /// </summary>
        /// <returns></returns>
        public string GetCleanConnectionString()
        {
            RefreshConfiguration();
            return cleanConnectionString;
        }

        /// <summary>
        /// Gets DMS connection string from config file
        /// </summary>
        /// <returns>True if the configuration was updated</returns>
        private bool RefreshConfiguration()
        {
            if (connectionStringLoadTime > DateTime.UtcNow.AddMinutes(-10))
            {
                return false;
            }

            var lastConfig = mConfiguration;
            var loaded = LoadCentralConfiguration();
            if (!loaded)
            {
                LoadLocalConfiguration();
            }

            mConfiguration.ValidateConfig();

            if (!string.IsNullOrWhiteSpace(connectionString) && mConfiguration.Equals(lastConfig))
            {
                connectionStringLoadTime = DateTime.UtcNow;
                return false;
            }

            var lastConnectionString = cleanConnectionString;

            cleanConnectionString = DbToolsFactory.GetConnectionString(mConfiguration.DatabaseSoftware,
                mConfiguration.DatabaseServer, mConfiguration.DatabaseName, mConfiguration.Username,
                "", ApplicationName, false);

            connectionString = DbToolsFactory.GetConnectionString(mConfiguration.DatabaseSoftware,
                mConfiguration.DatabaseServer, mConfiguration.DatabaseName, mConfiguration.Username,
                AppUtils.DecodeShiftCipher(mConfiguration.EncodedPassword), ApplicationName);

            connectionStringLoadTime = DateTime.UtcNow;
            connectionStringUpdated = true;

            if (!mConnectionStringLogged || !lastConnectionString.StartsWith(cleanConnectionString))
            {
                ApplicationLogger.LogMessage(ApplicationLogger.CONST_STATUS_LEVEL_DETAILED,
                    "Database connection string: " + cleanConnectionString + ";Password=....");
                mConnectionStringLogged = true;
            }

            return true;
        }

        /// <summary>
        /// Loads DMS configuration from a centralized file
        /// </summary>
        /// <returns>True if able to read/load the central configuration</returns>
        private bool LoadCentralConfiguration()
        {
            var remoteConfigLoaded = false;

            try
            {
                if (File.Exists(CENTRAL_CONFIG_FILE_PATH))
                {
                    // Centralized config file exists; read it
                    var config = DMSConfig.FromJson(CENTRAL_CONFIG_FILE_PATH);
                    var good = config.ValidateConfig();

                    // Centralized config file contains all the important information; cache it and use it, if it is not a match for the current cached config
                    if (good && !config.Equals(mConfiguration))
                    {
                        ApplicationLogger.LogMessage(LogLevel.Info, "Loading updated DMS database configuration from centralized config file...");
                        mConfiguration = config;
                        remoteConfigLoaded = true;
                        var configPath = PersistDataPaths.GetFileSavePath(CONFIG_FILE);
                        config.ToJson(configPath);
                    }

                    remoteConfigLoaded = good;
                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(LogLevel.Info, "Exception attempting to load centralized database configuration file", ex);
            }

            return remoteConfigLoaded;
        }

        /// <summary>
        /// Loads DMS configuration from file
        /// </summary>
        private void LoadLocalConfiguration()
        {
            var configurationPath = PersistDataPaths.GetFileLoadPath(CONFIG_FILE);
            if (!File.Exists(configurationPath))
            {
                mConfiguration = CreateDefaultConfigFile(configurationPath);
            }
            else
            {
                try
                {
                    mConfiguration = DMSConfig.FromJson(configurationPath);
                }
                catch (Exception ex)
                {
                    ApplicationLogger.LogError(LogLevel.Info, "Exception attempting to load local database configuration file", ex);
                }
            }

            mConfiguration.ValidateConfig();
        }

        private static DMSConfig CreateDefaultConfigFile(string configurationPath)
        {
            var config = new DMSConfig();
            config.LoadDefaults();
            config.ToJson(configurationPath);
            return config;
        }

        /// <summary>
        /// Generic method to retrieve data from a single-column table
        /// </summary>
        /// <param name="cmdStr">SQL command to execute</param>
        /// <returns>List containing the table's contents</returns>
        public List<string> GetSingleColumnTableList(string cmdStr)
        {
            var db = GetConnection();
            if (db == null)
            {
                throw new Exception(ErrMsg);
            }

            try
            {
                return db.GetQueryResultsEnumerable(cmdStr, reader => reader.GetString(0)).ToList();
            }
            catch (DbException ex)
            {
                // Error connecting to or retrieving data from the database
                var errMsg = "SQL exception getting data via query " + cmdStr;
                ApplicationLogger.LogError(0, errMsg, ex);
                throw new DatabaseDataException(errMsg, ex);
            }
        }

        /// <summary>
        /// Generic method to retrieve data from a single-column table
        /// </summary>
        /// <param name="cmdStr">SQL command to execute</param>
        /// <returns>List containing the table's contents</returns>
        public IEnumerable<string> GetSingleColumnTable(string cmdStr)
        {
            var db = GetConnection();
            if (db == null)
            {
                throw new Exception(ErrMsg);
            }

            return db.GetQueryResultsEnumerable(cmdStr, reader => reader.GetString(0));
        }

        /// <summary>
        /// Retrieves a data table from DMS
        /// </summary>
        /// <param name="cmdStr">SQL command to retrieve table</param>
        /// <returns>DataTable containing requested data</returns>
        /// <remarks>This tends to use more memory than directly reading and parsing data.</remarks>
        [Obsolete("Unused")]
        // ReSharper disable once UnusedMember.Local
        private DataTable GetDataTable(string cmdStr)
        {
            var returnTable = new DataTable();
            var db = GetConnection();
            if (db == null)
            {
                throw new Exception(ErrMsg);
            }

            try
            {
                db.GetQueryResultsDataTable(cmdStr, out returnTable);
            }
            catch (Exception ex)
            {
                var errMsg = "SQL exception getting data table via query " + cmdStr;
                ApplicationLogger.LogError(0, errMsg, ex);
                throw new DatabaseDataException(errMsg, ex);
            }

            // Return the output table
            return returnTable;
        }

        // TODO: do I need to wrap usages of this in a foreach loop and yield return?
        public IEnumerable<T> ExecuteReader<T>(string sqlCmd, Func<IDataReader, T> rowParseObjectCreator)
        {
            var db = GetConnection();
            if (db == null)
            {
                throw new Exception(ErrMsg);
            }

            return db.GetQueryResultsEnumerable(sqlCmd, rowParseObjectCreator);
        }

        /// <summary>
        /// Test if we can query each of the needed DMS tables/views.
        /// </summary>
        /// <param name="tableNamesAndCheckColumns">Dictionary where the key is a table/view name, and the value a sortable column name</param>
        /// <returns></returns>
        public bool CheckDMSConnection(IReadOnlyDictionary<string, string> tableNamesAndCheckColumns)
        {
            try
            {
                var db = GetConnection();
                if (db == null)
                {
                    throw new Exception(ErrMsg);
                }

                // Test getting 1 row from every table we query?...
                // Keys in the dictionary are view names, values are the column to use when ranking rows using Row_number()
                foreach (var item in tableNamesAndCheckColumns)
                {
                    var cmdText = $"SELECT RowNum FROM (SELECT Row_number() Over (ORDER BY {item.Value}) AS RowNum FROM {item.Key}) RankQ WHERE RowNum = 1;";
                    var result = db.GetQueryScalar(cmdText, out _); // TODO: Test the returned value? (for what?)

                    if (!result)
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(0, "Failed to test read a needed table!", ex);
                return false;

                // TODO: throw a DatabaseConnectionStringException ?
            }

            return true;
        }
    }
}
