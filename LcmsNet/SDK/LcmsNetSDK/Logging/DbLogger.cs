﻿//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche, Christopher Walters for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2010, Battelle Memorial Institute
// Created 02/03/2010
//
// Updates:
// - 10/13/2010 (DAC) - Corrected error reporting bug
// - 09/30/2014 (CJW) - Minor bugs fixed.
//*********************************************************************************************************

using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using LcmsNetSDK.System;

namespace LcmsNetSDK.Logging
{
    /// <summary>
    /// Logs errors and messages to a SQLite database
    /// </summary>
    public static class DbLogger
    {

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
        private static bool m_LogDbFileCreated;
        private static string m_DbFileName;
        private static string m_ConnStr;

        #endregion

        #region "Methods"

        /// <summary>
        /// Logs an error
        /// </summary>
        /// <param name="errorLevel">Error level</param>
        /// <param name="args">Message arguments</param>
        public static void LogError(int errorLevel, ErrorLoggerArgs args)
        {
            var sqlCmdBlder = new StringBuilder(INSERT_CMD_BASE);
            //sqlCmdBlder.Append("'" + DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0)).ToString("MM/dd/yyyy HH:mm:ss.f") + "',");
            sqlCmdBlder.Append("'" + TimeKeeper.Instance.Now.ToString("MM/dd/yyyy HH:mm:ss.f") + "',");
            sqlCmdBlder.Append("'ERROR',");
            sqlCmdBlder.Append("'" + errorLevel + "',");

            // If sample is present, add it to the SQL string
            if (args.Sample != null)
            {
                sqlCmdBlder.Append("'" + args.Sample.DmsData.DatasetName + "',");
                sqlCmdBlder.Append("'" + args.Sample.ColumnData.ID + "',");
                var eventIndx = args.Sample.ActualLCMethod.CurrentEventNumber;
                if (eventIndx < 0 || eventIndx >= args.Sample.ActualLCMethod.Events.Count)
                {
                    sqlCmdBlder.Append("'',");
                }
                else
                {
                    sqlCmdBlder.Append("'" + args.Sample.ActualLCMethod.Events[eventIndx].Device.Name + "',");
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
        public static void LogMessage(int msgLevel, MessageLoggerArgs args)
        {
            var sqlCmdBlder = new StringBuilder(INSERT_CMD_BASE);
            //sqlCmdBlder.Append("'" + DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0)).ToString("MM/dd/yyyy HH:mm:ss.f") + "',");
            sqlCmdBlder.Append("'" + TimeKeeper.Instance.Now.ToString("MM/dd/yyyy HH:mm:ss.f") + "',");
            sqlCmdBlder.Append("'MSG',");
            sqlCmdBlder.Append("'" + msgLevel + "',");

            // If sample is present, add it to the SQL string
            if (args.Sample != null)
            {
                sqlCmdBlder.Append("'" + args.Sample.DmsData.DatasetName + "',");
                sqlCmdBlder.Append("'" + args.Sample.ColumnData.ID + "',"); // Add column here
                if (args.Sample.ActualLCMethod.CurrentEventNumber >= 0)
                {
                    if (args.Sample.ActualLCMethod.Events.Count > 0)
                        sqlCmdBlder.Append("'" +
                                           args.Sample.ActualLCMethod.Events[args.Sample.ActualLCMethod.CurrentEventNumber].Device
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
                MessageBox.Show(msg, "LOG ERROR", MessageBoxButton.OK,
                    MessageBoxImage.Error);
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
                MessageBox.Show(msg, "LOG ERROR", MessageBoxButton.OK,
                    MessageBoxImage.Error);
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
                    throw new DbLoggerException("Exception creating db log folder", ex);
                }

                try
                {
                    SQLiteConnection.CreateFile(m_DbFileName);
                }
                catch (Exception ex)
                {
                    m_ConnStr = "";
                    m_LogDbFileCreated = false;
                    throw new DbLoggerException("Exception creating db log file", ex);
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
                throw new DbLoggerException(msg, ex);
            }
        }

        /// <summary>
        /// Executes specified SQLite command
        /// </summary>
        /// <param name="cmdStr">SQL statement to execute</param>
        /// <param name="connStr">Connection string for SQL database file</param>
        private static void ExecuteSQLiteCommand(string cmdStr, string connStr)
        {
            using (var cn = new SQLiteConnection(connStr))
            using (var myCmd = new SQLiteCommand(cn))
            {
                myCmd.CommandType = CommandType.Text;
                myCmd.CommandText = cmdStr;
                try
                {
                    myCmd.Connection.Open();
                    myCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    var errMsg = "SQLite exception executing command " + cmdStr;
                    throw new DbLoggerException(errMsg, ex);
                }
                finally
                {
                    myCmd.Connection.Close();
                }
            }
        }

        /// <summary>
        /// Determines if a particular table exists in the SQLite database
        /// </summary>
        /// <param name="tableName">Name of the table to search for</param>
        /// <param name="connStr">Connection string for database</param>
        /// <returns>TRUE if table found; FALSE if not found or error</returns>
        private static bool VerifyTableExists(string tableName, string connStr)
        {
            var sqlString = "SELECT * FROM sqlite_master WHERE name ='" + tableName + "'";
            DataTable tableList;
            try
            {
                // Get a list of database tables matching the specified table name
                tableList = GetSQLiteDataTable(sqlString, connStr);
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
        /// <param name="connStr">Connection string for SQLite database file</param>
        /// <returns>A DataTable containing data specfied by <paramref name="cmdStr"/></returns>
        private static DataTable GetSQLiteDataTable(string cmdStr, string connStr)
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
        private static void UnwrapExceptionMsgs(Exception ex, out string msg)
        {
            msg = ex.Message + " " + ex.StackTrace;
            if (ex.InnerException != null)
            {
                string innerMsg;
                UnwrapExceptionMsgs(ex.InnerException, out innerMsg);
                msg += "\nInner exception: " + innerMsg;
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
}