using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using LcmsNetData.Logging;
using LcmsNetData.System;
using LcmsNetSDK.Data;

namespace LcmsNetSDK.Logging
{
    /// <summary>
    /// Logs errors and messages to a SQLite database
    /// </summary>
    public class DbLogger : LogWriterBase, IDisposable
    {
        public static DbLogger Instance { get; } = new DbLogger();

        /// <summary>
        /// Private default constructor to prevent external instanciation
        /// </summary>
        private DbLogger()
        {
        }

        private const string INSERT_CMD_BASE = "INSERT INTO T_LogData('Date','Type','Level','Sample',"
                                               + "'Column','Device','Message','Exception') VALUES(";

        #region "Properties"

        public string LogFolderPath
        {
            get
            {
                var logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "LCMSNet", "Log");
                return logDir;
            }
        }

        #endregion

        #region "Class variables"

        private readonly object dbWriteLock = new object();
        private bool logDbFileCreated;
        private string dbFileName;
        private string connStr;

        #endregion

        #region Connection Management

        private SQLiteConnection connection = null;
        private string lastConnectionString = "";
        private readonly TimeSpan connectionTimeoutTime = TimeSpan.FromSeconds(60);
        private Timer connectionTimeoutTimer = null;

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

        ~DbLogger()
        {
            Dispose();
        }

        public void Dispose()
        {
            CloseConnection();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Get a SQLiteConnection, but lim
        /// </summary>
        /// <param name="connString"></param>
        /// <returns></returns>
        private SQLiteConnectionWrapper GetConnection(string connString)
        {
            if (connString == connStr)
            {
                // Reset out the close timer with every use
                connectionTimeoutTimer?.Dispose();
                connectionTimeoutTimer = new Timer(ConnectionTimeoutActions, this, connectionTimeoutTime, TimeSpan.FromMilliseconds(-1));

                if (!lastConnectionString.Equals(connString))
                {
                    CloseConnection();
                }

                if (connection == null)
                {
                    lastConnectionString = connString;
                    try
                    {
                        var cn = new SQLiteConnection(connString);
                        cn.Open();
                        connection = cn;
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Error connecting to logging database", e);
                    }
                }

                return new SQLiteConnectionWrapper(connection);
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

        #region "Methods"

        /// <summary>
        /// Logs an error
        /// </summary>
        /// <param name="errorLevel">Error level</param>
        /// <param name="args">Message arguments</param>
        public override void LogError(int errorLevel, ErrorLoggerArgs args)
        {
            if (errorLevel > ErrorLevel)
            {
                return;
            }

            var sqlCmdBlder = new StringBuilder(INSERT_CMD_BASE);
            //sqlCmdBlder.Append("'" + DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0)).ToString("MM/dd/yyyy HH:mm:ss.f") + "',");
            sqlCmdBlder.Append("'" + TimeKeeper.Instance.Now.ToString("MM/dd/yyyy HH:mm:ss.f") + "',");
            sqlCmdBlder.Append("'ERROR',");
            sqlCmdBlder.Append("'" + errorLevel + "',");

            // If sample is present, add it to the SQL string
            if (args.ErrorObject != null && args.ErrorObject is SampleData sample)
            {
                sqlCmdBlder.Append("'" + sample.DmsData.DatasetName + "',");
                sqlCmdBlder.Append("'" + sample.ColumnData.ID + "',");
                if (sample.ActualLCMethod != null)
                {
                    var eventIndx = sample.ActualLCMethod.CurrentEventNumber;
                    if (eventIndx < 0 || eventIndx >= sample.ActualLCMethod.Events.Count)
                    {
                        sqlCmdBlder.Append("'',");
                    }
                    else
                    {
                        sqlCmdBlder.Append("'" + sample.ActualLCMethod.Events[eventIndx].Device.Name + "',");
                    }
                }
            }
            else
            {
                sqlCmdBlder.Append("'','','',");
            }

            sqlCmdBlder.Append("'" + ReplaceQuotes(args.Message) + "',");

            // Unwrap any exception messages and add them
            var exMsg = "";
            if (args.Exception != null)
            {
                UnwrapExceptionMsgs(args.Exception, out exMsg);
            }
            sqlCmdBlder.Append("'" + ReplaceQuotes(exMsg) + "')");
            WriteLogMsgToDb(sqlCmdBlder.ToString());
        }

        /// <summary>
        /// Logs a message
        /// </summary>
        /// <param name="msgLevel">Message level</param>
        /// <param name="args">Message arguments</param>
        public override void LogMessage(int msgLevel, MessageLoggerArgs args)
        {
            if (msgLevel > MessageLevel)
            {
                return;
            }

            var sqlCmdBlder = new StringBuilder(INSERT_CMD_BASE);
            //sqlCmdBlder.Append("'" + DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0)).ToString("MM/dd/yyyy HH:mm:ss.f") + "',");
            sqlCmdBlder.Append("'" + TimeKeeper.Instance.Now.ToString("MM/dd/yyyy HH:mm:ss.f") + "',");
            sqlCmdBlder.Append("'MSG',");
            sqlCmdBlder.Append("'" + msgLevel + "',");

            // If sample is present, add it to the SQL string
            if (args.ErrorObject != null && args.ErrorObject is SampleData sample)
            {
                sqlCmdBlder.Append("'" + sample.DmsData.DatasetName + "',");
                sqlCmdBlder.Append("'" + sample.ColumnData.ID + "',"); // Add column here
                if (sample.ActualLCMethod != null && sample.ActualLCMethod.CurrentEventNumber >= 0)
                {
                    if (sample.ActualLCMethod.Events.Count > 0)
                        sqlCmdBlder.Append("'" +
                                           sample.ActualLCMethod.Events[sample.ActualLCMethod.CurrentEventNumber].Device
                                               .Name.Replace("'", "''") + "',");
                    else
                        sqlCmdBlder.Append("'No events',");
                }
                else
                {
                    sqlCmdBlder.Append("'" + "No Device " + "',"); // Add device here
                }
            }
            else
            {
                sqlCmdBlder.Append("'','','',");
            }

            sqlCmdBlder.Append("'" + args.Message.Replace("'", "''") + "',");

            // Create blank field for exception
            sqlCmdBlder.Append("'')");
            WriteLogMsgToDb(sqlCmdBlder.ToString());
        }

        /// <summary>
        /// Writes a SQL INSERT command to the log db file
        /// </summary>
        /// <param name="sqlCmd">SQL command string</param>
        private void WriteLogMsgToDb(string sqlCmd)
        {
            try
            {
                lock (dbWriteLock)
                {
                    // Verify logging db is ready
                    while (!logDbFileCreated)
                    {
                        // Database wasn't ready, so try to create it
                        if (!InitLogDatabase())
                        {
                            return;
                        }
                    }

                    // Insert the log entry into the data table
                    ExecuteSQLiteCommand(sqlCmd);
                }
            }
            catch (Exception ex)
            {
                UnwrapExceptionMsgs(ex, out var msg);
                MessageBox.Show("Exception logging error message: " + msg, "LOG ERROR", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Creates logging database file and initializes it
        /// </summary>
        private bool InitLogDatabase()
        {
            try
            {
                // Create the database file
                CreateDbFile();
                // Create the data table
                CreateLogTable();
                logDbFileCreated = true;
                return true;
            }
            catch (Exception ex)
            {
                logDbFileCreated = false;
                connStr = "";
                UnwrapExceptionMsgs(ex, out var msg);
                MessageBox.Show("Exception initializing log database: " + msg, "LOG ERROR", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }
        }

        /// <summary>
        /// Creates and initializes a SQLite DB file for logging, and sets up the connection string
        /// </summary>
        private void CreateDbFile()
        {
            dbFileName = Path.Combine(LogFolderPath, "LcmsNetDbLog.db3");
            var logFile = new FileInfo(dbFileName);

            // Create the file if it doesn't already exist
            if (!logFile.Exists)
            {
                try
                {
                    if (logFile.Directory == null)
                        throw new Exception("Db log file has a null directory object; unable to continue");

                    // Create the log folder if it does not yet exist
                    if (!logFile.Directory.Exists)
                        logFile.Directory.Create();
                }
                catch (Exception ex)
                {
                    connStr = "";
                    logDbFileCreated = false;
                    throw new DbLoggerException("Exception creating db log folder", ex);
                }

                try
                {
                    SQLiteConnection.CreateFile(dbFileName);
                }
                catch (Exception ex)
                {
                    connStr = "";
                    logDbFileCreated = false;
                    throw new DbLoggerException("Exception creating db log file", ex);
                }
            }
            connStr = "data source=" + dbFileName;
        }

        /// <summary>
        /// Creates the table for holding log entries
        /// </summary>
        private void CreateLogTable()
        {
            // Create the command string
            var sqlStr =
                "CREATE TABLE IF NOT EXISTS T_LogData('Date','Type','Level','Sample','Column','Device','Message','Exception')";
            try
            {
                ExecuteSQLiteCommand(sqlStr);
            }
            catch (Exception ex)
            {
                var msg = "Exception creating table in log database";
                throw new DbLoggerException(msg, ex);
            }
        }

        /// <summary>
        /// Executes specified SQLite command
        /// </summary>
        /// <param name="cmdStr">SQL statement to execute</param>
        private void ExecuteSQLiteCommand(string cmdStr)
        {
            using (var cn = GetConnection(connStr))
            using (var myCmd = cn.CreateCommand())
            {
                myCmd.CommandType = CommandType.Text;
                myCmd.CommandText = cmdStr;
                try
                {
                    myCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    var errMsg = "SQLite exception executing command " + cmdStr;
                    throw new DbLoggerException(errMsg, ex);
                }
            }
        }

        /// <summary>
        /// Determines if a particular table exists in the SQLite database
        /// </summary>
        /// <param name="tableName">Name of the table to search for</param>
        /// <returns>TRUE if table found; FALSE if not found or error</returns>
        private bool VerifyTableExists(string tableName)
        {
            var sqlString = "SELECT * FROM sqlite_master WHERE name ='" + tableName + "'";
            DataTable tableList;
            try
            {
                // Get a list of database tables matching the specified table name
                tableList = GetSQLiteDataTable(sqlString);
            }
            catch (Exception ex)
            {
                var errMsg = "SQLite exception verifying table " + tableName + " exists";
                throw new DbLoggerException(errMsg, ex);
            }

            // If exactly 1 row returned, then table exists
            if (tableList.Rows.Count == 1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Retrieves a data table from a SQLite database
        /// </summary>
        /// <param name="cmdStr">SQL command to execute</param>
        /// <returns>A DataTable containing data specfied by <paramref name="cmdStr"/></returns>
        private DataTable GetSQLiteDataTable(string cmdStr)
        {
            var returnTable = new DataTable();

            using (var cn = new SQLiteConnection(connStr))
            using (var da = new SQLiteDataAdapter())
            using (var cmd = new SQLiteCommand(cmdStr, cn))
            {
                cmd.CommandType = CommandType.Text;
                da.SelectCommand = cmd;
                try
                {
                    da.Fill(returnTable);
                }
                catch (Exception ex)
                {
                    var errMsg = "SQLite exception getting data table via query " + cmdStr;
                    throw new DbLoggerException(errMsg, ex);
                }
            }

            // Everything worked, so return the table
            return returnTable;
        }

        /// <summary>
        /// Peels nested exceptions to get to the innermost error message
        /// </summary>
        /// <param name="ex">Input exception</param>
        /// <param name="msg">Input/Output message</param>
        private void UnwrapExceptionMsgs(Exception ex, out string msg)
        {
            msg = ex.Message + " " + ex.StackTrace;
            if (ex.InnerException != null)
            {
                UnwrapExceptionMsgs(ex.InnerException, out var innerMsg);
                msg += "\nInner exception: " + innerMsg;
            }
        }

        /// <summary>
        /// Escapes single quotes in a string being stored in db
        /// </summary>
        /// <param name="inpStr">String to be tested</param>
        /// <returns>Escaped string</returns>
        private string ReplaceQuotes(string inpStr)
        {
            return inpStr.Replace("'", "''");
        }

        #endregion
    }
}
