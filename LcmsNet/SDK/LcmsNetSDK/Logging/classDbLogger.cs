//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche, Christopher Walters for the US Department of Energy 
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2010, Battelle Memorial Institute
// Created 02/03/2010
//
// Last modified 9/30/2014
//						10/13/2010 (DAC) - Corrected error reporting bug
//                      09/30/2014 (CJW) - Minor bugs fixed.    
//*********************************************************************************************************

using System;
using System.Text;
using System.IO;
using System.Data;
using System.Data.SQLite;
using System.Threading;

namespace LcmsNetDataClasses.Logging
{
    public static class classDbLogger
    {
        //*********************************************************************************************************
        // Logs errors and messages to a SQLite database
        //**********************************************************************************************************

        #region "Constants"

        private const string INSERT_CMD_BASE = "INSERT INTO T_LogData('Date','Type','Level','Sample',"
                                               + "'Column','Device','Message','Exception') VALUES(";

        #endregion

        #region "Properies"

        public static string LogFolderPath
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

        private static readonly object m_lock = "AstringToLockOn";
        private static readonly object m_writeLock = "AnotherStringToLockOn";
        private static bool m_LogDbFileCreated = false;
        private static string m_DbFileName;
        private static string m_ConnStr;

        #endregion

        #region "Methods"

        /// <summary>
        /// Logs an error
        /// </summary>
        /// <param name="errorLevel">Error level</param>
        /// <param name="args">Message arguments</param>
        public static void LogError(int errorLevel, classErrorLoggerArgs args)
        {
            var sqlCmdBlder = new StringBuilder(INSERT_CMD_BASE);
            //sqlCmdBlder.Append("'" + DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0)).ToString("MM/dd/yyyy HH:mm:ss.f") + "',");
            sqlCmdBlder.Append("'" + LcmsNetSDK.TimeKeeper.Instance.Now.ToString("MM/dd/yyyy HH:mm:ss.f") + "',");
            sqlCmdBlder.Append("'ERROR',");
            sqlCmdBlder.Append("'" + errorLevel.ToString() + "',");

            // If sample is present, add it to the SQL string
            if (args.Sample != null)
            {
                sqlCmdBlder.Append("'" + args.Sample.DmsData.DatasetName + "',");
                sqlCmdBlder.Append("'" + args.Sample.ColumnData.ID.ToString() + "',");
                var eventIndx = args.Sample.LCMethod.CurrentEventNumber;
                if (eventIndx < 0 || eventIndx > args.Sample.LCMethod.Events.Count)
                {
                    sqlCmdBlder.Append("'',");
                }
                else
                {
                    sqlCmdBlder.Append("'" + args.Sample.LCMethod.Events[eventIndx].Device.Name + "',");
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
            lock (m_writeLock)
            {
                WriteLogMsgToDb(sqlCmdBlder.ToString(), m_ConnStr);
            }
        }

        /// <summary>
        /// Logs a message
        /// </summary>
        /// <param name="msgLevel">Message level</param>
        /// <param name="args">Message arguments</param>
        public static void LogMessage(int msgLevel, classMessageLoggerArgs args)
        {
            var sqlCmdBlder = new StringBuilder(INSERT_CMD_BASE);
            //sqlCmdBlder.Append("'" + DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0)).ToString("MM/dd/yyyy HH:mm:ss.f") + "',");
            sqlCmdBlder.Append("'" + LcmsNetSDK.TimeKeeper.Instance.Now.ToString("MM/dd/yyyy HH:mm:ss.f") + "',");
            sqlCmdBlder.Append("'MSG',");
            sqlCmdBlder.Append("'" + msgLevel.ToString() + "',");

            // If sample is present, add it to the SQL string
            if (args.Sample != null)
            {
                sqlCmdBlder.Append("'" + args.Sample.DmsData.DatasetName + "',");
                sqlCmdBlder.Append("'" + args.Sample.ColumnData.ID.ToString() + "',"); // Add column here
                if (args.Sample.LCMethod.CurrentEventNumber >= 0)
                {
                    if (args.Sample.LCMethod.Events.Count > 0)
                        sqlCmdBlder.Append("'" +
                                           args.Sample.LCMethod.Events[args.Sample.LCMethod.CurrentEventNumber].Device
                                               .Name + "',");
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

            sqlCmdBlder.Append("'" + args.Message + "',");

            // Create blank field for exception
            sqlCmdBlder.Append("'')");
            lock (m_writeLock)
            {
                WriteLogMsgToDb(sqlCmdBlder.ToString(), m_ConnStr);
            }
        }

        /// <summary>
        /// Writes a SQL INSERT command to the log db file
        /// </summary>
        /// <param name="sqlCmd">SQL command string</param>
        /// <param name="connStr">Connection string</param>
        private static void WriteLogMsgToDb(string sqlCmd, string connStr)
        {
            try
            {
                // Verify logging db is ready
                while (!m_LogDbFileCreated)
                {
                    var check = false;
                    try
                    {
                        check = Monitor.TryEnter(m_lock);
                        if (check && !m_LogDbFileCreated)
                        {
                            // Database wasn't ready, so try to create it
                            if (!InitLogDatabase())
                            {
                                return;
                            }
                        }
                    }
                    finally
                    {
                        if (check)
                        {
                            Monitor.Exit(m_lock);
                        }
                    }
                }
                // Insert the log entry into the data table
                ExecuteSQLiteCommand(sqlCmd, m_ConnStr);
            }
            catch (Exception ex)
            {
                var msg = "Exception logging error message: ";
                UnwrapExceptionMsgs(ex, out msg);
                System.Windows.Forms.MessageBox.Show(msg, "LOG ERROR", System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Creates logging database file and initializes it
        /// </summary>
        private static bool InitLogDatabase()
        {
            try
            {
                // Create the database file
                CreateDbFile();
                // Create the data table
                CreateLogTable(m_ConnStr);
                m_LogDbFileCreated = true;
                return true;
            }
            catch (Exception ex)
            {
                m_LogDbFileCreated = false;
                m_ConnStr = "";
                var msg = "Exception initializing log database: ";
                UnwrapExceptionMsgs(ex, out msg);
                System.Windows.Forms.MessageBox.Show(msg, "LOG ERROR", System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Creates and initializes a SQLite DB file for logging, and sets up the connection string
        /// </summary>
        private static void CreateDbFile()
        {
            m_DbFileName = Path.Combine(LogFolderPath, "LcmsNetDbLog.db3");
            var logFile = new FileInfo(m_DbFileName);

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
                    m_ConnStr = "";
                    m_LogDbFileCreated = false;
                    throw new classDbLoggerException("Exception creating db log folder", ex);
                }

                try
                {
                    SQLiteConnection.CreateFile(m_DbFileName);
                }
                catch (Exception ex)
                {
                    m_ConnStr = "";
                    m_LogDbFileCreated = false;
                    throw new classDbLoggerException("Exception creating db log file", ex);
                }
            }
            m_ConnStr = "data source=" + m_DbFileName;
        }

        /// <summary>
        /// Creates the table for holding log entries
        /// </summary>
        /// <param name="connStr">DB connection string</param>
        private static void CreateLogTable(string connStr)
        {
            // Create the command string
            var sqlStr =
                "CREATE TABLE IF NOT EXISTS T_LogData('Date','Type','Level','Sample','Column','Device','Message','Exception')";
            try
            {
                ExecuteSQLiteCommand(sqlStr, connStr);
            }
            catch (Exception ex)
            {
                var msg = "Exception creating table in log database";
                throw new classDbLoggerException(msg, ex);
            }
        }

        /// <summary>
        /// Executes specified SQLite command
        /// </summary>
        /// <param name="CmdStr">SQL statement to execute</param>
        /// <param name="ConnStr">Connection string for SQL database file</param>
        private static void ExecuteSQLiteCommand(string CmdStr, string connStr)
        {
            int AffectedRows;
            using (var Cn = new SQLiteConnection(connStr))
            {
                using (var myCmd = new SQLiteCommand(Cn))
                {
                    myCmd.CommandType = CommandType.Text;
                    myCmd.CommandText = CmdStr;
                    try
                    {
                        myCmd.Connection.Open();
                        AffectedRows = myCmd.ExecuteNonQuery();
                        return;
                    }
                    catch (Exception Ex)
                    {
                        var ErrMsg = "SQLite exception executing command " + CmdStr;
                        throw new classDbLoggerException(ErrMsg, Ex);
                    }
                    finally
                    {
                        myCmd.Connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Determines if a particular table exists in the SQLite database
        /// </summary>
        /// <param name="TableName">Name of the table to search for</param>
        /// <param name="ConnStr">Connection string for database</param>
        /// <returns>TRUE if table found; FALSE if not found or error</returns>
        private static bool VerifyTableExists(string TableName, string connStr)
        {
            var sqlString = "SELECT * FROM sqlite_master WHERE name ='" + TableName + "'";
            var tableList = new DataTable();
            try
            {
                // Get a list of database tables matching the specified table name
                tableList = GetSQLiteDataTable(sqlString, connStr);
            }
            catch (Exception Ex)
            {
                var ErrMsg = "SQLite exception verifying table " + TableName + " exists";
                throw new classDbLoggerException(ErrMsg, Ex);
            }

            // If exactly 1 row returned, then table exists
            if (tableList.Rows.Count == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Retrieves a data table from a SQLite database
        /// </summary>
        /// <param name="CmdStr">SQL command to execute</param>
        /// <param name="ConnStr">Connection string for SQLite database file</param>
        /// <returns>A DataTable containing data specfied by CmdStr</returns>
        private static DataTable GetSQLiteDataTable(string CmdStr, string connStr)
        {
            var returnTable = new DataTable();
            var FilledRows = 0;
            using (var Cn = new SQLiteConnection(connStr))
            {
                using (var Da = new SQLiteDataAdapter())
                {
                    using (var Cmd = new SQLiteCommand(CmdStr, Cn))
                    {
                        Cmd.CommandType = CommandType.Text;
                        Da.SelectCommand = Cmd;
                        try
                        {
                            FilledRows = Da.Fill(returnTable);
                        }
                        catch (Exception Ex)
                        {
                            var ErrMsg = "SQLite exception getting data table via query " + CmdStr;
                            throw new classDbLoggerException(ErrMsg, Ex);
                        }
                    }
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
        private static void UnwrapExceptionMsgs(Exception ex, out string msg)
        {
            msg = ex.Message;
            if (!(ex.InnerException == null))
            {
                string innerMsg;
                UnwrapExceptionMsgs(ex.InnerException, out innerMsg);
                msg += " Inner exception: " + innerMsg;
            }
        }

        /// <summary>
        /// Escapes single quotes in a string being stored in db
        /// </summary>
        /// <param name="inpStr">String to be tested</param>
        /// <returns>Escaped string</returns>
        private static string ReplaceQuotes(string inpStr)
        {
            return inpStr.Replace("'", "''");
        }

        #endregion
    }
} // End namespace