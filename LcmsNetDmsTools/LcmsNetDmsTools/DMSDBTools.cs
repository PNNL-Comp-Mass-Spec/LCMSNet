﻿//*********************************************************************************************************
// Written by Dave Clark for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 01/07/2009
//
// Updates
// - 01/26/2009 (DAC) - Added error handling for database errors. Removed excess "using" statements
// - 02/03/2009 (DAC) - Added methods for accessing additional data per Brian request
// - 02/04/2009 (DAC) - Changed DMS request retrieval to ruturn List<SampleData>
// - 02/19/2009 (DAC) - Incorporated renamed exceptions
// - 02/24/2009 (DAC) - Changed to cache downloaded data in SQLite database
// - 03/09/2009 (DAC) - Added download of DMS Comment field
// - 03/27/2009 (DAC) - Converted LINQ queries to ADO.Net due to terrible error handling by LINQ
// - 03/31/2009 (DAC) - Implemented file logging for exceptions
// - 04/02/2009 (DAC) - Implemented MRM file download tools
// - 04/09/2009 (DAC) - Added retrieval and caching of column lists
// - 05/12/2009 (DAC) - Added download of blocking and run order values
// - 08/11/2009 (DAC) - Added download of request batch number
// - 12/01/2009 (DAC) - Modified to accomodate changing vial from string to int
// - 02/18/2010 (DAC) - Modified to streamline generation of sample request query
// - 07/28/2010 (DAC) - Added conversion of alpha well/vial to integer
// - 12/08/2010 (DAC) - Modified DMS connection string to use name/password authentication
// - 04/30/2013 (FCT) - Modified get carts sql command string to include "No_Cart" in the results.
// - 05/22/2014 (MEM) - Replace Select * with selection of specific columns
// - 09/11/2014 (CJW) - Modified to be an MEF Extension for lcmsnet
// - 09/17/2014 (CJW) - Modified to use a stand-alone configuration file instead of the LcmsNet app configuration file.
// - 05/22/2015 (MEM) - Auto-create PrismDMS.config if missing
// - 07/30/2015 (MEM) - Add option to load dataset names
//*********************************************************************************************************
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using LcmsNetSQLiteTools;
using System.IO;
using System.Threading;
using LcmsNetData;
using LcmsNetData.Data;
using LcmsNetData.Logging;

namespace LcmsNetDmsTools
{
    /// <summary>
    /// Class for interacting with DMS database
    /// </summary>
    // Deprecated export: [Export(typeof(IDmsTools))]
    // Deprecated export: [ExportMetadata("Name", "PrismDMSTools")]
    // Deprecated export: [ExportMetadata("Version", "1.0")]
    public class DMSDBTools : IDisposable
    {
        #region "Class variables"
        string m_ErrMsg = "";

        private bool mConnectionStringLogged;

        /// <summary>
        /// Key to access the DMS version string in the configuration dictionary.
        /// </summary>
        private const string CONST_DMS_SERVER_KEY = "DMSServer";

        /// <summary>
        /// Key to access the DMS version string in the configuration dictionary.
        /// </summary>
        /// <remarks>This is the name of the database to connect to</remarks>
        private const string CONST_DMS_VERSION_KEY = "DMSVersion";

        /// <summary>
        /// Key to access the encoded DMS password string in the configuration dictionary.
        /// </summary>
        /// <remarks>This is the password of SQL Server user LCMSNetUser</remarks>
        private const string CONST_DMS_PASSWORD_KEY = "DMSPwd";

        private const string CONFIG_FILE = "PrismDMS.config";

        #endregion

        #region "Properties"
        public bool ForceValidation => true;

        public string ErrMsg
        {
            get => m_ErrMsg;
            set => m_ErrMsg = value;
        }

        public string DMSVersion => GetConfigSetting(CONST_DMS_VERSION_KEY, "UnknownVersion");

        /// <summary>
        /// Controls whether datasets are loaded when LoadCacheFromDMS() is called
        /// </summary>
        public bool LoadDatasets { get; set; }

        /// <summary>
        /// Controls whether experiments are loaded when LoadCacheFromDMS() is called
        /// </summary>
        public bool LoadExperiments { get; set; }

        /// <summary>
        /// Number of months back to search when reading dataset names
        /// </summary>
        /// <remarks>Default is 12 months; use 0 to load all data</remarks>
        public int RecentDatasetsMonthsToLoad { get; set; }

        /// <summary>
        /// Number of months back to search when reading experiment information
        /// </summary>
        /// <remarks>Default is 18 months; use 0 to load all data</remarks>
        public int RecentExperimentsMonthsToLoad { get; set; }

        /// <summary>
        /// Number of months back to load expired EMSL Proposals; defaults to '12', use '-1' to load all data
        /// </summary>
        public int EMSLProposalsRecentMonthsToLoad { get; set; }

        public bool UseConnectionPooling { get; set; }

        private readonly Dictionary<string,string> mConfiguration;

        #endregion

        #region "Events"

        public event ProgressEventHandler ProgressEvent;

        public void OnProgressUpdate(ProgressEventArgs e)
        {
            if (ProgressEvent == null)
                Console.WriteLine(e.CurrentTask + @": " + e.PercentComplete);
            else
                ProgressEvent.Invoke(this, e);

        }

        #endregion

        #region "Constructors"

        /// <summary>
        /// Constructor
        /// </summary>
        public DMSDBTools()
        {
            mConfiguration = new Dictionary<string, string>();
            RecentDatasetsMonthsToLoad = 12;
            RecentExperimentsMonthsToLoad = 18;
            EMSLProposalsRecentMonthsToLoad = 12;
            LoadConfiguration();
            // This should generally be true for SqlClient/SqlConnection, false means connection reuse (and potential multi-threading problems)
            UseConnectionPooling = true;
        }
        #endregion

        #region Instance

        private SqlConnection connection = null;
        private string lastConnectionString = "";
        private DateTime lastConnectionAttempt = DateTime.MinValue;
        private readonly TimeSpan minTimeBetweenConnectionAttempts = TimeSpan.FromSeconds(30);
        private readonly TimeSpan connectionTimeoutTime = TimeSpan.FromSeconds(60);
        private Timer connectionTimeoutTimer = null;
        private string failedConnectionAttemptMessage = "";

        private void ConnectionTimeoutActions(object sender)
        {
            CloseConnection();
        }

        /// <summary>
        /// Close the stored SqlConnection
        /// </summary>
        public void CloseConnection()
        {
            try
            {
                connection?.Close();
                connection?.Dispose();
                connectionTimeoutTimer?.Dispose();
                connection = null;
            }
            catch
            {
                // Swallow any exceptions that occurred...
            }
        }

        ~DMSDBTools()
        {
            Dispose();
        }

        public void Dispose()
        {
            CloseConnection();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Get a SQLiteConnection, but control creation of new connections based on UseConnectionPooling
        /// </summary>
        /// <param name="connString"></param>
        /// <returns></returns>
        private SqlConnectionWrapper GetConnection(string connString)
        {
            // Reset out the close timer with every use
            connectionTimeoutTimer?.Dispose();
            connectionTimeoutTimer = new Timer(ConnectionTimeoutActions, this, connectionTimeoutTime, TimeSpan.FromMilliseconds(-1));

            var newServer = false;
            if (!lastConnectionString.Equals(connString))
            {
                CloseConnection();
                newServer = true;
            }

            if (connection == null && (DateTime.UtcNow > lastConnectionAttempt.Add(minTimeBetweenConnectionAttempts) || newServer))
            {
                lastConnectionString = connString;
                lastConnectionAttempt = DateTime.UtcNow;
                try
                {
                    var cn = new SqlConnection(connString);
                    cn.Open();
                    connection = cn;
                    failedConnectionAttemptMessage = "";
                }
                catch (Exception e)
                {
                    failedConnectionAttemptMessage = $"Error connecting to database; Please check network connections and try again. Exception message: {e.Message}";
                    ErrMsg = failedConnectionAttemptMessage;
                    ApplicationLogger.LogError(0, failedConnectionAttemptMessage);
                }
            }

            if (UseConnectionPooling && connection != null)
            {
                // MSSQL/SqlConnection connection pooling: handled transparently based on connection strings
                // https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-connection-pooling
                connection.Close();
                return new SqlConnectionWrapper(connString);
            }
            else
            {
                return new SqlConnectionWrapper(connection, failedConnectionAttemptMessage);
            }
        }

        /// <summary>
        /// A SqlConnection wrapper that only disposes in certain circumstances
        /// </summary>
        private class SqlConnectionWrapper : IDisposable
        {
            private readonly SqlConnection connection;
            private readonly bool closeConnectionOnDispose = true;
            public string FailedConnectionAttemptMessage { get; }

            /// <summary>
            /// Open a new connection, which will get closed on Dispose().
            /// </summary>
            /// <param name="connString"></param>
            public SqlConnectionWrapper(string connString)
            {
                try
                {
                    connection = new SqlConnection(connString);
                    connection.Open();
                }
                catch (Exception e)
                {
                    FailedConnectionAttemptMessage =
                        $"Error connecting to database; Please check network connections and try again. Exception message: {e.Message}";
                }

                closeConnectionOnDispose = true;
                IsValid = connection != null && connection.State == ConnectionState.Open;
            }

            /// <summary>
            /// Wrap an existing connection, which will stay open on Dispose().
            /// </summary>
            /// <param name="existingConnection"></param>
            /// <param name="failedConnectionAttemptMessage"></param>
            public SqlConnectionWrapper(SqlConnection existingConnection, string failedConnectionAttemptMessage = "")
            {
                connection = existingConnection;
                closeConnectionOnDispose = false;
                IsValid = connection != null && connection.State == ConnectionState.Open;
                FailedConnectionAttemptMessage = failedConnectionAttemptMessage;
            }

            public bool IsValid { get; }

            public SqlConnection GetConnection()
            {
                return connection;
            }

            public SqlCommand CreateCommand()
            {
                return connection.CreateCommand();
            }

            public SqlTransaction BeginTransaction()
            {
                return connection.BeginTransaction();
            }

            public void Dispose()
            {
                if (!closeConnectionOnDispose)
                {
                    return;
                }

                connection?.Close();
                connection?.Dispose();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Loads DMS configuration from file
        /// </summary>
        private void LoadConfiguration()
        {
            var readerSettings = new XmlReaderSettings
            {
                ValidationType = ValidationType.Schema,
                ValidationFlags = XmlSchemaValidationFlags.ProcessInlineSchema
            };
            readerSettings.ValidationEventHandler += settings_ValidationEventHandler;

            var folderPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (string.IsNullOrEmpty(folderPath))
            {
                throw new DirectoryNotFoundException("Directory for the executing assembly is empty; unable to load the configuration in DMSDBTools");
            }

            var configurationPath = Path.Combine(folderPath, CONFIG_FILE);
            if (!File.Exists(configurationPath))
            {
                CreateDefaultConfigFile(configurationPath);
            }

            var reader = XmlReader.Create(configurationPath, readerSettings);
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Equals(reader.GetAttribute("dmssetting"), "true", StringComparison.CurrentCultureIgnoreCase))
                        {
                            var settingName = reader.Name.Remove(0, 2);
                            // Add/update the configuration item
                            mConfiguration[settingName] = reader.ReadString();
                        }
                        break;
                }
            }
        }

        private void settings_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Error)
            {
                throw new InvalidOperationException(e.Message, e.Exception);
            }

            ApplicationLogger.LogMessage(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "DmsTools Configuration warning: " + e.Message);
        }

        /// <summary>
        /// Lookup the value for the given setting
        /// </summary>
        /// <param name="configName">Setting name</param>
        /// <param name="valueIfMissing">Value to return if configName is not defined in mConfiguration</param>
        /// <returns></returns>
        private string GetConfigSetting(string configName, string valueIfMissing)
        {
            if (mConfiguration.TryGetValue(configName, out var configValue))
            {
                return configValue;
            }
            return valueIfMissing;
        }

        /// <summary>
        /// Gets DMS connection string from config file
        /// </summary>
        /// <returns></returns>
        private string GetConnectionString()
        {
            // Construct the connection string, for example:
            // Data Source=Gigasax;Initial Catalog=DMS5;User ID=LCMSNetUser;Password=ThePassword"

            var retStr = "Data Source=";

            // Get the DMS Server name
            var dmsServer = GetConfigSetting(CONST_DMS_SERVER_KEY, "Gigasax");
            if (dmsServer != null)
            {
                retStr += dmsServer;
            }
            else
            {
                retStr += "Gigasax";
            }

            // Get name of the DMS database to use
            var dmsVersion = GetConfigSetting(CONST_DMS_VERSION_KEY, "DMS5");
            if (dmsVersion != null)
            {
                retStr += ";Initial Catalog=" + dmsVersion + ";User ID=LCMSNetUser";
            }
            else
            {
                throw new DatabaseConnectionStringException(
                    "DMS version string not found in configuration file (this parameter is the " +
                    "name of the database to connect to).  Delete the " + CONFIG_FILE + " file and " +
                    "it will be automatically re-created with the default values.");
            }

            if (!mConnectionStringLogged)
            {
                ApplicationLogger.LogMessage(ApplicationLogger.CONST_STATUS_LEVEL_DETAILED,
                                                  "Database connection string: " + retStr + ";Password=....");
                mConnectionStringLogged = true;
            }

            // Get the password for user LCMSNetUser
            var dmsPassword = GetConfigSetting(CONST_DMS_PASSWORD_KEY, "Mprptq3v");
            if (dmsPassword != null)
            {
                retStr += ";Password=" + DecodePassword(dmsPassword);
            }
            else
            {
                throw new DatabaseConnectionStringException(
                    "DMS password string not found in configuration file (this is the password " +
                    "for the LCMSOperator username.  Delete the " + CONFIG_FILE + " file and " +
                    "it will be automatically re-created with the default values.");
            }

            return retStr;
        }

        /// <summary>
        /// Generic method to retrieve data from a single-column table in DMS
        /// </summary>
        /// <param name="cmdStr">SQL command to execute</param>
        /// <param name="connStr">Database connection string</param>
        /// <returns>List containing the table's contents</returns>
        private IEnumerable<string> GetSingleColumnTableFromDMS(string cmdStr, string connStr)
        {
            var cn = GetConnection(connStr);
            if (!cn.IsValid)
            {
                cn.Dispose();
                throw new Exception(cn.FailedConnectionAttemptMessage);
            }

            using (cn)
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = cmdStr;
                cmd.CommandType = CommandType.Text;

                SqlDataReader reader;

                // Get a table from the database
                try
                {
                    reader = cmd.ExecuteReader();
                }
                catch (Exception ex)
                {
                    ErrMsg = "Exception getting single column table via command: " + cmdStr;
                    //                  throw new DatabaseDataException(ErrMsg, ex);
                    ApplicationLogger.LogError(0, ErrMsg, ex);
                    throw new Exception(ErrMsg, ex);
                }

                using (reader)
                {
                    // Copy the table contents into the list
                    while (reader.Read())
                    {
                        yield return reader.GetString(0);
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves a data table from DMS
        /// </summary>
        /// <param name="cmdStr">SQL command to retrieve table</param>
        /// <param name="connStr">DMS connection string</param>
        /// <returns>DataTable containing requested data</returns>
        /// <remarks>This tends to use more memory than directly reading and parsing data.</remarks>
        private DataTable GetDataTable(string cmdStr, string connStr)
        {
            var returnTable = new DataTable();
            var cn = GetConnection(connStr);
            if (!cn.IsValid)
            {
                cn.Dispose();
                throw new Exception(cn.FailedConnectionAttemptMessage);
            }

            using (cn)
            using (var da = new SqlDataAdapter())
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = cmdStr;
                cmd.CommandType = CommandType.Text;
                da.SelectCommand = cmd;
                try
                {
                    da.Fill(returnTable);
                }
                catch (Exception ex)
                {
                    var errMsg = "SQL exception getting data table via query " + cmdStr;
                    ApplicationLogger.LogError(0, errMsg, ex);
                    throw new DatabaseDataException(errMsg, ex);
                }
            }

            // Return the output table
            return returnTable;
        }

        /// <summary>
        /// Executes a stored procedure
        /// </summary>
        /// <param name="spCmd">SQL command object containing SP parameters</param>
        /// <param name="connStr">Connection string</param>
        /// <returns>SP result code</returns>
        private int ExecuteSP(SqlCommand spCmd, string connStr)
        {
            var resultCode = -9999;
            try
            {
                var cn = GetConnection(connStr);
                if (!cn.IsValid)
                {
                    cn.Dispose();
                    throw new Exception(cn.FailedConnectionAttemptMessage);
                }

                using (cn)
                using (var da = new SqlDataAdapter())
                using (var ds = new DataSet())
                {
                    spCmd.Connection = cn.GetConnection();
                    da.SelectCommand = spCmd;
                    da.Fill(ds);
                    resultCode = (int)da.SelectCommand.Parameters["@Return"].Value;
                }
            }
            catch (Exception ex)
            {
                ErrMsg = "Exception executing stored procedure " + spCmd.CommandText;
                ApplicationLogger.LogError(0, ErrMsg, ex);
                throw new DatabaseStoredProcException(spCmd.CommandText, resultCode, ex.Message);
            }
            return resultCode;
        }

        private static void CreateDefaultConfigFile(string configurationPath)
        {
            // Create a new file with default config data
            using (var writer = new StreamWriter(new FileStream(configurationPath, FileMode.Create, FileAccess.Write, FileShare.Read)))
            {
                writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                writer.WriteLine("<catalog>");
                writer.WriteLine("  <!-- DMS Configuration Schema definition -->");
                writer.WriteLine("  <xs:schema elementFormDefault=\"qualified\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" targetNamespace=\"PrismDMS\"> ");
                writer.WriteLine("    <xs:element name=\"PrismDMSConfig\">");
                writer.WriteLine("      <xs:complexType><xs:sequence>");
                writer.WriteLine("          <xs:element name=\"DMSServer\" minOccurs=\"0\" maxOccurs=\"1\">");
                writer.WriteLine("             <xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\">");
                writer.WriteLine("                  <xs:attribute name=\"dmssetting\" use=\"optional\" type=\"xs:string\"/>");
                writer.WriteLine("             </xs:extension></xs:simpleContent></xs:complexType>");
                writer.WriteLine("          </xs:element>");
                writer.WriteLine("          <xs:element name=\"DMSVersion\">");
                writer.WriteLine("             <xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\">");
                writer.WriteLine("                  <xs:attribute name=\"dmssetting\" use=\"required\" type=\"xs:string\"/>                 ");
                writer.WriteLine("             </xs:extension></xs:simpleContent></xs:complexType>");
                writer.WriteLine("          </xs:element>");
                writer.WriteLine("          <xs:element name=\"DMSPwd\">");
                writer.WriteLine("             <xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\">");
                writer.WriteLine("                  <xs:attribute name=\"dmssetting\" use=\"required\" type=\"xs:string\"/>");
                writer.WriteLine("             </xs:extension></xs:simpleContent></xs:complexType>");
                writer.WriteLine("          </xs:element>                 ");
                writer.WriteLine("      </xs:sequence></xs:complexType>");
                writer.WriteLine("    </xs:element>");
                writer.WriteLine("  </xs:schema>");
                writer.WriteLine(" ");
                writer.WriteLine("  <!-- DMS configuration -->");
                writer.WriteLine("  <p:PrismDMSConfig xmlns:p=\"PrismDMS\">");
                writer.WriteLine("    <!-- Server hosting DMS (defaults to Gigasax if missing) -->");
                writer.WriteLine("    <p:DMSServer dmssetting=\"true\">Gigasax</p:DMSServer>");
                writer.WriteLine("    <!-- DMSVersion is the name of the database to connect to -->");
                writer.WriteLine("    <p:DMSVersion dmssetting=\"true\">DMS5</p:DMSVersion>");
                writer.WriteLine("    <!-- DMSPwd is the encoded DMS password for SQL server user LCMSNetUser -->");
                writer.WriteLine("    <p:DMSPwd dmssetting=\"true\">Mprptq3v</p:DMSPwd>");
                writer.WriteLine("  </p:PrismDMSConfig>");
                writer.WriteLine("</catalog>");
            }
        }

        /// <summary>
        /// Decrypts password received from ini file
        /// </summary>
        /// <param name="enPwd">Encoded password</param>
        /// <returns>Clear text password</returns>
        private static string DecodePassword(string enPwd)
        {
            // Decrypts password received from ini file
            // Password was created by alternately subtracting or adding 1 to the ASCII value of each character

            // Convert the password string to a character array
            var pwdChars = enPwd.ToCharArray();
            var pwdBytes = new byte[pwdChars.Length];
            var pwdCharsAdj = new char[pwdChars.Length];

            for (var i = 0; i < pwdChars.Length; i++)
            {
                pwdBytes[i] = (byte)pwdChars[i];
            }

            // Modify the byte array by shifting alternating bytes up or down and convert back to char, and add to output string
            var retStr = "";
            for (var byteCntr = 0; byteCntr < pwdBytes.Length; byteCntr++)
            {
                if ((byteCntr % 2) == 0)
                {
                    pwdBytes[byteCntr] += 1;
                }
                else
                {
                    pwdBytes[byteCntr] -= 1;
                }
                pwdCharsAdj[byteCntr] = (char)pwdBytes[byteCntr];
                retStr += pwdCharsAdj[byteCntr].ToString(CultureInfo.InvariantCulture);
            }
            return retStr;
        }

        private void ReportProgress(string currentTask, int currentStep, int stepCountTotal)
        {
            var percentComplete = currentStep / (double)stepCountTotal * 100;
            OnProgressUpdate(new ProgressEventArgs(currentTask, percentComplete));
        }

        #endregion

        #region Private Methods: Read from DMS and cache to SQLite

        /// <summary>
        /// Gets a list of Cart Config Names from DMS and stores it in cache
        /// </summary>
        private void GetCartConfigNamesFromDMS()
        {
            try
            {
                var cartConfigs = ReadCartConfigNamesFromDMS();

                // Store the list of cart config names in the cache db
                try
                {
                    SQLiteTools.SaveCartConfigListToCache(cartConfigs);
                }
                catch (Exception ex)
                {
                    const string errMsg = "Exception storing LC cart config names in cache";
                    ApplicationLogger.LogError(0, errMsg, ex);
                }
            }
            catch (Exception ex)
            {
                ErrMsg = "Exception getting cart config list";
                ApplicationLogger.LogError(0, ErrMsg, ex);
            }
        }

        /// <summary>
        /// Gets a list of Work Packages from DMS and stores it in cache
        /// </summary>
        private void GetWorkPackagesFromDMS()
        {
            try
            {
                var dataFromDms = ReadWorkPackagesFromDMS();

                // Store the list of cart config names in the cache db
                try
                {
                    SQLiteTools.SaveWorkPackageListToCache(dataFromDms);
                }
                catch (Exception ex)
                {
                    const string errMsg = "Exception storing work packages in cache";
                    ApplicationLogger.LogError(0, errMsg, ex);
                }
            }
            catch (Exception ex)
            {
                ErrMsg = "Exception getting work package list";
                ApplicationLogger.LogError(0, ErrMsg, ex);
            }
        }

        /// <summary>
        /// Gets a list of instrument carts from DMS and stores it in cache
        /// </summary>
        private void GetCartListFromDMS()
        {
            IEnumerable<string> tmpCartList;   // Temp list for holding return values
            var connStr = GetConnectionString();

            // Get a List containing all the carts
            const string sqlCmd = "SELECT DISTINCT [Cart_Name] FROM V_LC_Cart_Active_Export " +
                                  "ORDER BY [Cart_Name]";
            try
            {
                tmpCartList = GetSingleColumnTableFromDMS(sqlCmd, connStr);
            }
            catch (Exception ex)
            {
                ErrMsg = "Exception getting cart list";
                ApplicationLogger.LogError(0, ErrMsg, ex);
                return;
            }

            // Store the list of carts in the cache db
            try
            {
                SQLiteTools.SaveCartListToCache(tmpCartList);
            }
            catch (Exception ex)
            {
                const string errMsg = "Exception storing LC cart list in cache";
                ApplicationLogger.LogError(0, errMsg, ex);
            }
        }

        private void GetDatasetListFromDMS()
        {
            var connStr = GetConnectionString();

            var sqlCmd = "SELECT Dataset FROM V_LCMSNet_Dataset_Export";

            if (RecentDatasetsMonthsToLoad > 0)
            {
                var dateThreshold = DateTime.Now.AddMonths(-RecentDatasetsMonthsToLoad).ToString("yyyy-MM-dd");
                sqlCmd += " WHERE Created >= '" + dateThreshold + "'";
            }

            try
            {
                var datasetList = GetSingleColumnTableFromDMS(sqlCmd, connStr);

                // Store the data in the cache db
                try
                {
                    SQLiteTools.SaveDatasetNameListToCache(datasetList);
                }
                catch (Exception ex)
                {
                    const string errMsg = "Exception storing dataset list in cache";
                    ApplicationLogger.LogError(0, errMsg, ex);
                }
            }
            catch (Exception ex)
            {
                ErrMsg = "Exception getting dataset list";
                ApplicationLogger.LogError(0, ErrMsg, ex);
            }

        }

        /// <summary>
        /// Gets a list of active LC columns from DMS and stores in the cache
        /// </summary>
        private void GetColumnListFromDMS()
        {
            IEnumerable<string> tmpColList;    // Temp list for holding return values
            var connStr = GetConnectionString();

            // Get a list of active columns
            const string sqlCmd = "SELECT ColumnNumber FROM V_LCMSNet_Column_Export WHERE State <> 'Retired' ORDER BY ColumnNumber";
            try
            {
                tmpColList = GetSingleColumnTableFromDMS(sqlCmd, connStr);
            }
            catch (Exception ex)
            {
                ErrMsg = "Exception getting column list";
                //              throw new DatabaseDataException(ErrMsg, ex);
                ApplicationLogger.LogError(0, ErrMsg, ex);
                return;
            }

            // Store the list of carts in the cache db
            try
            {
                SQLiteTools.SaveColumnListToCache(tmpColList);
            }
            catch (Exception ex)
            {
                const string errMsg = "Exception storing column list in cache";
                ApplicationLogger.LogError(0, errMsg, ex);
            }
        }

        public void GetEntireColumnListListFromDMS()
        {
            try
            {
                var lcColumns = ReadLcColumnsFromDMS();

                try
                {
                    SQLiteTools.SaveEntireLCColumnListToCache(lcColumns);
                }
                catch (Exception ex)
                {
                    const string errMsg = "Exception storing LC Column data list in cache";
                    ApplicationLogger.LogError(0, errMsg, ex);
                }
            }
            catch (Exception ex)
            {
                ErrMsg = "Exception getting experiment list";
                ApplicationLogger.LogError(0, ErrMsg, ex);
            }
        }

        /// <summary>
        /// Gets a list of separation types from DMS and stores it in cache
        /// </summary>
        private void GetSepTypeListFromDMS()
        {
            IEnumerable<string> tmpRetVal; // Temp list for holding separation types
            var connStr = GetConnectionString();

            const string sqlCmd = "SELECT Distinct SS_Name FROM T_Secondary_Sep WHERE SS_active > 0 ORDER BY SS_Name";

            try
            {
                tmpRetVal = GetSingleColumnTableFromDMS(sqlCmd, connStr);
            }
            catch (Exception ex)
            {
                ErrMsg = "Exception getting separation type list";
                //                  throw new DatabaseDataException(ErrMsg, ex);
                ApplicationLogger.LogError(0, ErrMsg, ex);
                return;
            }

            // Store data in cache
            try
            {
                SQLiteTools.SaveSeparationTypeListToCache(tmpRetVal);
            }
            catch (Exception ex)
            {
                const string errMsg = "Exception storing separation type list in cache";
                ApplicationLogger.LogError(0, errMsg, ex);
            }
        }

        /// <summary>
        /// Gets a list of dataset types from DMS ans stores it in cache
        /// </summary>
        private void GetDatasetTypeListFromDMS()
        {
            IEnumerable<string> tmpRetVal; // Temp list for holding dataset types
            var connStr = GetConnectionString();

            // Get a list of the dataset types
            const string sqlCmd = "SELECT Distinct DST_Name FROM t_DatasetTypeName ORDER BY DST_Name";
            try
            {
                tmpRetVal = GetSingleColumnTableFromDMS(sqlCmd, connStr);
            }
            catch (Exception ex)
            {
                ErrMsg = "Exception getting dataset type list";
                //                  throw new DatabaseDataException(ErrMsg, ex);
                ApplicationLogger.LogError(0, ErrMsg, ex);
                return;
            }

            // Store data in cache
            try
            {
                SQLiteTools.SaveDatasetTypeListToCache(tmpRetVal);
            }
            catch (Exception ex)
            {
                const string errMsg = "Exception storing dataset type list in cache";
                ApplicationLogger.LogError(0, errMsg, ex);
            }
        }

        /// <summary>
        /// Gets a list of active users from DMS and stores it in cache
        /// </summary>
        private void GetUserListFromDMS()
        {
            try
            {
                var dmsUsers = ReadUsersFromDMS();

                // Store data in cache
                try
                {
                    SQLiteTools.SaveUserListToCache(dmsUsers);
                }
                catch (Exception ex)
                {
                    const string errMsg = "Exception storing user list in cache";
                    ApplicationLogger.LogError(0, errMsg, ex);
                }
            }
            catch (Exception ex)
            {
                ErrMsg = "Exception getting user list";
                //                  throw new DatabaseDataException(ErrMsg, ex);
                ApplicationLogger.LogError(0, ErrMsg, ex);
            }
        }

        private void GetExperimentListFromDMS()
        {
            try
            {
                var experiments = ReadExperimentsFromDMS();

                try
                {
                    SQLiteTools.SaveExperimentListToCache(experiments);
                }
                catch (Exception ex)
                {
                    const string errMsg = "Exception storing experiment list in cache";
                    ApplicationLogger.LogError(0, errMsg, ex);
                }
            }
            catch (Exception ex)
            {
                ErrMsg = "Exception getting experiment list";
                ApplicationLogger.LogError(0, ErrMsg, ex);
            }
        }

        /// <summary>
        /// Get EMSL User Proposal IDs and associated users. Uses <see cref="EMSLProposalsRecentMonthsToLoad"/> to control how much data is loaded.
        /// </summary>
        private void GetProposalUsers()
        {
            var users = new List<ProposalUser>();
            var referenceList = new List<UserIDPIDCrossReferenceEntry>();
            var referenceDictionary = new Dictionary<string, List<UserIDPIDCrossReferenceEntry>>();

            try
            {
                // Split the View back into the two tables it was built from.
                // Note: It would be faster if we had the component tables the View was created from.
                var userMap = new Dictionary<int, ProposalUser>();

                foreach (var pUser in ReadProposalUsersFromDMS())
                {
                    if (!pUser.UserId.HasValue || string.IsNullOrWhiteSpace(pUser.ProposalId) || string.IsNullOrWhiteSpace(pUser.UserName))
                        continue;

                    var user = new ProposalUser();
                    var crossReference = new UserIDPIDCrossReferenceEntry();

                    user.UserID = pUser.UserId.Value;
                    user.UserName = pUser.UserName;

                    crossReference.PID = pUser.ProposalId;
                    crossReference.UserID = pUser.UserId.Value;

                    if (!userMap.ContainsKey(user.UserID))
                    {
                        userMap.Add(user.UserID, user);
                        users.Add(user);
                    }

                    if (!referenceDictionary.ContainsKey(crossReference.PID))
                        referenceDictionary.Add(crossReference.PID, new List<UserIDPIDCrossReferenceEntry>());

                    if (referenceDictionary[crossReference.PID].Any(cr => cr.UserID == crossReference.UserID))
                    {
                        continue;
                    }

                    referenceDictionary[crossReference.PID].Add(crossReference);
                    referenceList.Add(crossReference);
                }
            }
            catch (Exception ex)
            {
                ErrMsg = "Exception getting EUS Proposal Users list";
                ApplicationLogger.LogError(0, ErrMsg, ex);
                return;
            }

            try
            {
                SQLiteTools.SaveProposalUsers(users, referenceList, referenceDictionary);
            }
            catch (Exception ex)
            {
                const string errMsg = "Exception storing Proposal Users list in cache";
                //                  throw new DatabaseDataException(ErrMsg, ex);
                ApplicationLogger.LogError(0, errMsg, ex);
            }
        }

        /// <summary>
        /// Gets a list of instruments from DMS
        /// </summary>
        private void GetInstrumentListFromDMS()
        {
            try
            {
                var instruments = ReadInstrumentFromDMS();

                // Store data in cache
                try
                {
                    SQLiteTools.SaveInstListToCache(instruments);
                }
                catch (Exception ex)
                {
                    const string errMsg = "Exception storing instrument list in cache";
                    ApplicationLogger.LogError(0, errMsg, ex);
                }
            }
            catch (Exception ex)
            {
                ErrMsg = "Exception getting instrument list";
                //                  throw new DatabaseDataException(ErrMsg, ex);
                ApplicationLogger.LogError(0, ErrMsg, ex);
            }
        }

        #endregion

        #region Private DMS database read-and-convert methods

        private IEnumerable<CartConfigInfo> ReadCartConfigNamesFromDMS()
        {
            var connStr = GetConnectionString();

            // Get a list containing all active cart configuration names
            //  SELECT Cart_Config_ID,
            //         Cart_Config_Name,
            //         Cart_Name,
            //         Description,
            //         Autosampler,
            //         Pumps,
            //         Dataset_Usage_Count,
            //         Dataset_Usage_Last_Year,
            //         Cart_Config_State
            //    FROM V_LC_Cart_Config_Export
            const string sqlCmd =
                "SELECT Cart_Config_Name, Cart_Name " +
                "FROM V_LC_Cart_Config_Export " +
                "WHERE Cart_Config_State = 'Active' " +
                "ORDER BY Cart_Name, Cart_Config_Name";

            var cn = GetConnection(connStr);
            if (!cn.IsValid)
            {
                cn.Dispose();
                throw new Exception(cn.FailedConnectionAttemptMessage);
            }

            using (cn)
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = sqlCmd;
                cmd.CommandType = CommandType.Text;

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return new CartConfigInfo(
                            reader["Cart_Config_Name"].CastDBValTo<string>(),
                            reader["Cart_Name"].CastDBValTo<string>());
                    }
                }
            }
        }

        private IEnumerable<LCColumnData> ReadLcColumnsFromDMS()
        {
            var connStr = GetConnectionString();

            // This view will return all columns, even retired ones
            const string sqlCmd = "SELECT [State], [ColumnNumber] FROM V_LCMSNet_Column_Export";

            var cn = GetConnection(connStr);
            if (!cn.IsValid)
            {
                cn.Dispose();
                throw new Exception(cn.FailedConnectionAttemptMessage);
            }

            using (cn)
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = sqlCmd;
                cmd.CommandType = CommandType.Text;

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return new LCColumnData
                        {
                            LCColumn = reader["Column Number"].CastDBValTo<string>(),
                            State = reader["State"].CastDBValTo<string>()
                        };
                    }
                }
            }
        }

        private IEnumerable<ExperimentData> ReadExperimentsFromDMS()
        {
            var connStr = GetConnectionString();

            var sqlCmd = "SELECT ID, Experiment, Created, Organism, Reason, Request, Researcher FROM V_LCMSNet_Experiment_Export";

            if (RecentExperimentsMonthsToLoad > 0)
            {
                var dateThreshold = DateTime.Now.AddMonths(-RecentExperimentsMonthsToLoad).ToString("yyyy-MM-dd");
                sqlCmd += " WHERE Last_Used >= '" + dateThreshold + "'";
            }

            var cn = GetConnection(connStr);
            if (!cn.IsValid)
            {
                cn.Dispose();
                throw new Exception(cn.FailedConnectionAttemptMessage);
            }

            using (cn)
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = sqlCmd;
                cmd.CommandType = CommandType.Text;

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return new ExperimentData
                        {
                            Created = reader["Created"].CastDBValTo<DateTime>(),
                            Experiment = reader["Experiment"].CastDBValTo<string>(),
                            ID = reader["ID"].CastDBValTo<int>(),
                            Organism = reader["Organism"].CastDBValTo<string>(),
                            Reason = reader["Reason"].CastDBValTo<string>(),
                            Request = reader["Request"].CastDBValTo<int>(),
                            Researcher = reader["Researcher"].CastDBValTo<string>()
                        };
                    }
                }
            }
        }

        private IEnumerable<InstrumentInfo> ReadInstrumentFromDMS()
        {
            var connStr = GetConnectionString();

            // Get a table containing the instrument data
            const string sqlCmd = "SELECT Instrument, NameAndUsage, CaptureMethod, " +
                                  "Status, HostName, SharePath " +
                                  "FROM V_Instrument_Info_LCMSNet " +
                                  "ORDER BY Instrument";

            var cn = GetConnection(connStr);
            if (!cn.IsValid)
            {
                cn.Dispose();
                throw new Exception(cn.FailedConnectionAttemptMessage);
            }

            using (cn)
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = sqlCmd;
                cmd.CommandType = CommandType.Text;

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return new InstrumentInfo
                        {
                            DMSName = reader["Instrument"].CastDBValTo<string>(),
                            CommonName = reader["NameAndUsage"].CastDBValTo<string>(),
                            MethodName = reader["CaptureMethod"].CastDBValTo<string>(),
                            Status = reader["Status"].CastDBValTo<string>(),
                            HostName = reader["HostName"].CastDBValTo<string>(),
                            SharePath = reader["SharePath"].CastDBValTo<string>()
                        };
                    }
                }
            }
        }

        private IEnumerable<KeyValuePair<int, int>> ReadMRMFileListFromDMS(int minID, int maxID)
        {
            var connStr = GetConnectionString();
            var sqlCmd = "SELECT ID, RDS_MRM_Attachment FROM T_Requested_Run WHERE (not RDS_MRM_Attachment is null) " +
                         "AND (ID BETWEEN " + minID + " AND " + maxID + ")";

            var cn = GetConnection(connStr);
            if (!cn.IsValid)
            {
                cn.Dispose();
                throw new Exception(cn.FailedConnectionAttemptMessage);
            }

            using (cn)
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = sqlCmd;
                cmd.CommandType = CommandType.Text;

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return new KeyValuePair<int, int>(
                            reader["ID"].CastDBValTo<int>(),
                            reader["RDS_MRM_Attachment"].CastDBValTo<int>()
                        );
                    }
                }
            }
        }

        private IEnumerable<MRMFileData> ReadMRMFilesFromDMS(string fileIndxList)
        {
            var connStr = GetConnectionString();
            var sqlCmd = "SELECT File_Name, Contents FROM T_Attachments WHERE ID IN (" + fileIndxList + ")";

            var cn = GetConnection(connStr);
            if (!cn.IsValid)
            {
                cn.Dispose();
                throw new Exception(cn.FailedConnectionAttemptMessage);
            }

            using (cn)
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = sqlCmd;
                cmd.CommandType = CommandType.Text;

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return new MRMFileData
                        {
                            FileName = reader["File_Name"].CastDBValTo<string>(),
                            FileContents = reader["Contents"].CastDBValTo<string>()
                        };
                    }
                }
            }
        }

        private struct DmsProposalUserEntry
        {
            public int? UserId;
            public string UserName;
            public string ProposalId;

            public DmsProposalUserEntry(int? userId, string userName, string proposalId)
            {
                UserId = userId;
                UserName = userName;
                ProposalId = proposalId;
            }
        }

        private IEnumerable<DmsProposalUserEntry> ReadProposalUsersFromDMS()
        {
            var connStr = GetConnectionString();

            const string sqlCmdStart = "SELECT [User ID], [User Name], [#Proposal] FROM V_EUS_Proposal_Users";
            var sqlCmd = sqlCmdStart;
            if (EMSLProposalsRecentMonthsToLoad > -1)
            {
                var oldestExpiration = DateTime.Now.AddMonths(-EMSLProposalsRecentMonthsToLoad);
                sqlCmd += $" WHERE Proposal_End_Date >= '{oldestExpiration:yyyy-MM-dd}' OR Proposal_End_Date IS NULL";
            }

            var cn = GetConnection(connStr);
            if (!cn.IsValid)
            {
                cn.Dispose();
                throw new Exception(cn.FailedConnectionAttemptMessage);
            }

            using (cn)
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = sqlCmd;
                cmd.CommandType = CommandType.Text;

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return new DmsProposalUserEntry
                        (
                            reader["User ID"].CastDBValTo<int?>(),
                            reader["User Name"].CastDBValTo<string>(),
                            reader["#Proposal"].CastDBValTo<string>()
                        );
                    }
                }
            }
        }

        private IEnumerable<T> ReadRequestedRunsFromDMS<T>(SampleQueryData queryData) where T : SampleDataBasic, new()
        {
            var connStr = GetConnectionString();

            // Retrieve run requests from V_Scheduled_Run_Export, filtering based on settings in queryData
            var sqlCmd = queryData.BuildSqlString();

            var cn = GetConnection(connStr);
            if (!cn.IsValid)
            {
                cn.Dispose();
                throw new Exception(cn.FailedConnectionAttemptMessage);
            }

            using (cn)
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = sqlCmd;
                cmd.CommandType = CommandType.Text;

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var tmpDMSData = new T
                        {
                            DmsData =
                            {
                                DatasetType = reader["Type"].CastDBValTo<string>(),
                                Experiment = reader["Experiment"].CastDBValTo<string>(),
                                EMSLProposalID = reader["Proposal ID"].CastDBValTo<string>(),
                                RequestID = reader["Request"].CastDBValTo<int>(),
                                RequestName = reader["Name"].CastDBValTo<string>(),
                                WorkPackage = reader["Work Package"].CastDBValTo<string>(),
                                EMSLUsageType = reader["Usage Type"].CastDBValTo<string>(),
                                UserList = reader["EUS Users"].CastDBValTo<string>(),
                                CartName = reader["Cart"].CastDBValTo<string>(),
                                Comment = reader["Comment"].CastDBValTo<string>(),
                                MRMFileID = reader["MRMFileID"].CastDBValTo<int>(),
                                Block = reader["Block"].CastDBValTo<int>(),
                                RunOrder = reader["RunOrder"].CastDBValTo<int>(),
                                Batch = reader["Batch"].CastDBValTo<int>(),
                                SelectedToRun = false,
                            }
                        };

                        var wellNumber = reader["Well Number"].CastDBValTo<string>();
                        tmpDMSData.PAL.Well = ConvertWellStringToInt(wellNumber);

                        tmpDMSData.PAL.WellPlate = reader["Wellplate Number"].CastDBValTo<string>();

                        if (string.IsNullOrWhiteSpace(tmpDMSData.PAL.WellPlate) || tmpDMSData.PAL.WellPlate == "na")
                            tmpDMSData.PAL.WellPlate = "";

                        yield return tmpDMSData;
                    }
                }
            }
        }

        /// <summary>
        /// Converts a letter/number or just number string representing a well/vial into an integer
        /// </summary>
        /// <param name="vialPosition">Input string</param>
        /// <returns>Integer position</returns>
        private int ConvertWellStringToInt(string vialPosition)
        {
            if (string.IsNullOrWhiteSpace(vialPosition) || vialPosition == "na")
            {
                return 0;
            }

            // First, we'll see if it's a simple number
            if (int.TryParse(vialPosition, out var parsed))
            {
                // vialPosition is simply an integer
                return parsed;
            }

            try
            {
                // vialPosition is in the form A1 or B10
                // Convert it using ConvertVialToInt
                return ConvertVialPosition.ConvertVialToInt(vialPosition);
            }
            catch
            {
                return 0;
            }
        }

        private IEnumerable<UserInfo> ReadUsersFromDMS()
        {
            var connStr = GetConnectionString();

            // Get a data table containing all the users
            const string sqlCmd = "SELECT Name, [Payroll Num] as Payroll FROM V_Active_Users ORDER BY Name";

            var cn = GetConnection(connStr);
            if (!cn.IsValid)
            {
                cn.Dispose();
                throw new Exception(cn.FailedConnectionAttemptMessage);
            }

            using (cn)
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = sqlCmd;
                cmd.CommandType = CommandType.Text;

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return new UserInfo
                        {
                            UserName = reader["Name"].CastDBValTo<string>(),
                            PayrollNum = reader["Payroll"].CastDBValTo<string>()
                        };
                    }
                }
            }
        }

        private IEnumerable<WorkPackageInfo> ReadWorkPackagesFromDMS()
        {
            var connStr = GetConnectionString();

            // Get a list containing all active work packages
            // SELECT TOP(1000) [Charge_Code]
            //     ,[State]
            //     ,[SubAccount]
            //     ,[WorkBreakdownStructure]
            //     ,[Title]
            //     ,[Usage_SamplePrep]
            //     ,[Usage_RequestedRun]
            //     ,[Owner_PRN]
            //     ,[Owner_Name]
            //     ,[Setup_Date]
            //     ,[SortKey]
            // FROM[DMS5].[dbo].[V_Charge_Code_Export]

            // Filters:
            // * Only get the last 6 years
            // * None from an 'unallowable' subaccount
            // * None that are inactive and never used
            // * None that have not been used, where the owner name is unknown (not in DMS)
            var sqlCmd =
                "SELECT Charge_Code, State, SubAccount, WorkBreakdownStructure, Title, Owner_PRN, Owner_Name " +
                "FROM V_Charge_Code_Export " +
                $"WHERE Setup_Date > '{DateTime.Now.AddYears(-6):yyyy-MM-dd}' AND SubAccount NOT LIKE '%UNALLOWABLE%' AND State <> 'Inactive, unused' AND (State LIKE '%, used%' OR Owner_Name IS NOT NULL)" +
                "ORDER BY SortKey";

            var cn = GetConnection(connStr);
            if (!cn.IsValid)
            {
                cn.Dispose();
                throw new Exception(cn.FailedConnectionAttemptMessage);
            }

            using (cn)
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = sqlCmd;
                cmd.CommandType = CommandType.Text;

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return new WorkPackageInfo(
                            reader["Charge_Code"].CastDBValTo<string>()?.Trim(),
                            reader["State"].CastDBValTo<string>()?.Trim(),
                            reader["SubAccount"].CastDBValTo<string>()?.Trim(),
                            reader["WorkBreakdownStructure"].CastDBValTo<string>()?.Trim(),
                            reader["Title"].CastDBValTo<string>()?.Trim(),
                            reader["Owner_PRN"].CastDBValTo<string>()?.Trim(),
                            reader["Owner_Name"].CastDBValTo<string>()?.Trim());
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Test if we can query each of the needed DMS tables/views.
        /// </summary>
        /// <returns></returns>
        public bool CheckDMSConnection()
        {
            try
            {
                var connStr = GetConnectionString();
                using (var conn = GetConnection(connStr))
                // Test getting 1 row from every table we query?...
                using (var cmd = conn.CreateCommand())
                {
                    var tableNames = new List<string>()
                    {
                        "V_LC_Cart_Config_Export", "V_Charge_Code_Export", "V_LC_Cart_Active_Export",
                        "V_LCMSNet_Dataset_Export", "V_LCMSNet_Column_Export", "T_Secondary_Sep", "t_DatasetTypeName",
                        "V_Active_Users", "V_LCMSNet_Experiment_Export", "V_EUS_Proposal_Users",
                        "V_Instrument_Info_LCMSNet", "V_Scheduled_Run_Export", "T_Attachments", "T_Requested_Run"
                    };

                    foreach (var tableName in tableNames)
                    {
                        cmd.CommandText = $"SELECT TOP(1) * FROM {tableName}";
                        cmd.ExecuteScalar(); // TODO: Test the returned value? (for what?)
                    }

                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(0, "Failed to test read a needed table!", ex);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Loads all DMS data into cache
        /// </summary>
        public void LoadCacheFromDMS()
        {
            LoadCacheFromDMS(LoadExperiments, LoadDatasets);
        }

        public void LoadCacheFromDMS(bool loadExperiments)
        {
            ReportProgress("Loading data from DMS (entering LoadCacheFromDMS(bool loadExperiments)", 0, 20);
            LoadCacheFromDMS(loadExperiments, LoadDatasets);
        }

        public void LoadCacheFromDMS(bool loadExperiments, bool loadDatasets)
        {
            const int STEP_COUNT_BASE = 11;
            const int EXPERIMENT_STEPS = 20;
            const int DATASET_STEPS = 50;

            var stepCountTotal = STEP_COUNT_BASE;

            if (loadExperiments)
                stepCountTotal += EXPERIMENT_STEPS;

            if (loadDatasets)
                stepCountTotal += DATASET_STEPS;

            ReportProgress("Loading data from DMS (determining Connection String)", 0, stepCountTotal);

            var sqLiteConnectionString = SQLiteTools.ConnString;
            var equalsIndex = sqLiteConnectionString.IndexOf('=');
            string cacheFilePath;

            if (equalsIndex > 0 && equalsIndex < sqLiteConnectionString.Length - 1)
                cacheFilePath = @"SQLite cache file path: " + sqLiteConnectionString.Substring(equalsIndex + 1);
            else
                cacheFilePath = @"SQLite cache file path: " + sqLiteConnectionString;

            var dmsConnectionString = GetConnectionString();

            // Remove the password from the connection string
            var passwordStartindex = dmsConnectionString.IndexOf(";Password", StringComparison.InvariantCultureIgnoreCase);
            if (passwordStartindex > 0)
                dmsConnectionString = dmsConnectionString.Substring(0, passwordStartindex);

            ReportProgress("Loading data from DMS (" + dmsConnectionString + ") and storing in " + cacheFilePath, 0, stepCountTotal);

            ReportProgress("Loading cart names", 1, stepCountTotal);
            GetCartListFromDMS();

            ReportProgress("Loading cart config names", 2, stepCountTotal);
            GetCartConfigNamesFromDMS();

            ReportProgress("Loading separation types", 3, stepCountTotal);
            GetSepTypeListFromDMS();

            ReportProgress("Loading dataset types", 4, stepCountTotal);
            GetDatasetTypeListFromDMS();

            ReportProgress("Loading instruments", 5, stepCountTotal);
            GetInstrumentListFromDMS();

            ReportProgress("Loading work packages", 6, stepCountTotal);
            GetWorkPackagesFromDMS();

            ReportProgress("Loading users", 7, stepCountTotal);
            GetUserListFromDMS();

            ReportProgress("Loading LC columns", 8, stepCountTotal);
            GetColumnListFromDMS();

            ReportProgress("Loading proposal users", 9, stepCountTotal);
            GetProposalUsers();

            var stepCountCompleted = STEP_COUNT_BASE;
            if (loadExperiments)
            {
                var currentTask = "Loading experiments";
                if (RecentExperimentsMonthsToLoad > 0)
                    currentTask += " created/used in the last " + RecentExperimentsMonthsToLoad + " months";

                ReportProgress(currentTask, stepCountCompleted, stepCountTotal);

                GetExperimentListFromDMS();
                stepCountCompleted = stepCountCompleted + EXPERIMENT_STEPS;
            }

            if (loadDatasets)
            {
                var currentTask = "Loading datasets";
                if (RecentDatasetsMonthsToLoad > 0)
                    currentTask += " from the last " + RecentDatasetsMonthsToLoad + " months";

                ReportProgress(currentTask, stepCountCompleted, stepCountTotal);

                GetDatasetListFromDMS();
                stepCountCompleted = stepCountCompleted + DATASET_STEPS;
            }

            ReportProgress("DMS data loading complete", stepCountTotal, stepCountTotal);
        }

        /// <summary>
        /// Gets a list of samples (essentially requested runs) from DMS
        /// </summary>
        /// <remarks>Retrieves data from view V_Scheduled_Run_Export</remarks>
        public IEnumerable<T> GetRequestedRunsFromDMS<T>(SampleQueryData queryData) where T : SampleDataBasic, new()
        {
            try
            {
                return ReadRequestedRunsFromDMS<T>(queryData);
            }
            catch (Exception ex)
            {
                ErrMsg = "Exception getting run request list";
                //                  throw new DatabaseDataException(ErrMsg, ex);
                ApplicationLogger.LogError(0, ErrMsg, ex);
                return Enumerable.Empty<T>();
            }
        }

        /// <summary>
        /// Adds data for block of MRM files to file data list
        /// </summary>
        /// <param name="fileIndxList">Comma-separated list of file indices needing data</param>
        /// <param name="fileData">ist of file names and contents; new data will be appended to this list</param>
        public void GetMRMFilesFromDMS(string fileIndxList, List<MRMFileData> fileData)
        {
            if (fileData == null)
            {
                throw new ArgumentNullException(nameof(fileData), "fileData must be initialized before calling GetMRMFilesFromDMS");
            }

            // Get the data from DMS
            try
            {
                fileData.AddRange(ReadMRMFilesFromDMS(fileIndxList));
            }
            catch (Exception ex)
            {
                m_ErrMsg = "Exception getting MRM file data from DMS";
                ApplicationLogger.LogError(0, m_ErrMsg, ex);
                throw new DatabaseDataException(m_ErrMsg, ex);
            }
        }

        /// <summary>
        /// Gets a list of MRM files to retrieve
        /// </summary>
        /// <param name="minID">Minimum request ID for MRM file search</param>
        /// <param name="maxID"></param>
        /// <returns></returns>
        public Dictionary<int, int> GetMRMFileListFromDMS(int minID, int maxID)
        {
            var retList = new Dictionary<int, int>();

            // Get the data from DMS
            try
            {
                // Pull the data from the table
                foreach (var file in ReadMRMFileListFromDMS(minID, maxID))
                {
                    retList.Add(file.Key, file.Value);
                }
            }
            catch (Exception ex)
            {
                m_ErrMsg = "Exception getting MRM file list from DMS";
                ApplicationLogger.LogError(0, m_ErrMsg, ex);
                throw new DatabaseDataException(m_ErrMsg, ex);
            }

            return retList;
        }

        /// <summary>
        /// Updates the cart assignment in DMS
        /// </summary>
        /// <param name="requestList">Comma-delimited string of request ID's (must be less than 8000 chars long)</param>
        /// <param name="cartName">Name of cart to assign (ignored for removing aasignment)</param>
        /// <param name="cartConfigName">Name of cart config name to assign</param>
        /// <param name="updateMode">TRUE for updating assignment; FALSE to clear assignment</param>
        /// <returns>TRUE for success; FALSE for error</returns>
        public bool UpdateDMSCartAssignment(string requestList, string cartName, string cartConfigName, bool updateMode)
        {
            var connStr = GetConnectionString();
            string mode;
            int resultCode;

            // Verify request list is < 8000 chars (stored procedure limitation)
            if (requestList.Length > 8000)
            {
                m_ErrMsg = "Too many requests selected for import.\r\nReduce the number of requests being imported.";
                return false;
            }

            // Convert mode to string value
            if (updateMode)
            {
                mode = "Add";
            }
            else
            {
                mode = "Remove";
            }

            // Set up parameters for stored procedure call
            var spCmd = new SqlCommand
            {
                CommandType = CommandType.StoredProcedure,
                CommandText = "AddRemoveRequestCartAssignment"
            };
            spCmd.Parameters.Add(new SqlParameter("@Return", SqlDbType.Int)).Direction = ParameterDirection.ReturnValue;

            spCmd.Parameters.Add(new SqlParameter("@RequestIDList", SqlDbType.VarChar, 8000)).Value = requestList;
            spCmd.Parameters.Add(new SqlParameter("@CartName", SqlDbType.VarChar, 128)).Value = cartName;
            spCmd.Parameters.Add(new SqlParameter("@CartConfigName", SqlDbType.VarChar, 128)).Value = cartConfigName;
            spCmd.Parameters.Add(new SqlParameter("@Mode", SqlDbType.VarChar, 32)).Value = mode;

            spCmd.Parameters.Add(new SqlParameter("@message", SqlDbType.VarChar, 512));
            spCmd.Parameters["@message"].Direction = ParameterDirection.InputOutput;
            spCmd.Parameters["@message"].Value = "";

            // Execute the SP
            try
            {
                resultCode = ExecuteSP(spCmd, connStr);
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(0, "Exception updating DMS cart information", ex);
                return false;
            }

            if (resultCode != 0)    // Error occurred
            {
                var returnMsg = spCmd.Parameters["@message"].ToString();
                throw new DatabaseStoredProcException("AddRemoveRequestCartAssignment", resultCode, returnMsg);
            }

            // Success!
            return true;
        }

        #endregion
    }
}
