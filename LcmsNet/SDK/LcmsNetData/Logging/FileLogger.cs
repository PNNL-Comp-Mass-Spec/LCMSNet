//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 03/30/2009
//
// Updates:
// - 04/16/2009: BLL
//         Made file logging class static so it can be used at program startup
//         Changed file log name to include hhmmss using a flag to indicate whether the file exists.
//
//*********************************************************************************************************

using System;
using System.IO;
using System.Text;
using System.Threading;
using LcmsNetData.System;

namespace LcmsNetData.Logging
{
    /// <summary>
    /// Logs errors and messages to a file
    /// </summary>
    public class FileLogger : LogWriterBase, IDisposable
    {
        public static FileLogger Instance { get; } = new FileLogger();

        /// <summary>
        /// Flag indicating whether a log file has been created for this program start.
        /// </summary>
        private bool logFileCreated;

        private readonly object fileWriteLock = new object();

        private StreamWriter logWriter = null;

        private readonly Timer logFileStreamTimeout;

        /// <summary>
        /// Constructor
        /// </summary>
        private FileLogger()
        {
            AppFolder = "LCMSNet";
            // Close out the log file once every hour, to help avoid issues with stale file handles
            logFileStreamTimeout = new Timer(CloseLogFile, this, TimeSpan.FromHours(1), TimeSpan.FromHours(1));
        }

        #region Properties

        #region Events

        /// <summary>
        /// Delegate method handler defining how an error event will be called.
        /// </summary>
        /// <param name="args"></param>
        public delegate void DelegateLogPathReporter(MessageLoggerArgs args);

        /// <summary>
        /// Fired when the log file path is defined
        /// </summary>
        public static event DelegateLogPathReporter LogFilePathDefined;

        #endregion

        /// <summary>
        /// Gets the file log path.
        /// </summary>
        public static string LogPath { get; private set; }

        /// <summary>
        /// Gets the file log path.
        /// </summary>
        public static string AppFolder { get; set; }

        #endregion

        #region "Methods"

        public override void LogError(int errorLevel, ErrorLoggerArgs args)
        {
            if (errorLevel > ErrorLevel)
            {
                return;
            }

            try
            {
                // Build the message string
                //var msgStr = new StringBuilder(DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0)).ToString("MM/dd/yyyy HH:mm:ss.fff"));
                var msgStr = new StringBuilder(TimeKeeper.Instance.Now.ToString("MM/dd/yyyy HH:mm:ss.fff"));
                msgStr.Append("\t");
                msgStr.Append("ERROR");
                msgStr.Append("\t");
                msgStr.Append(errorLevel.ToString());
                msgStr.Append("\t");

                var message = "Error message: " + args.Message + " ";
                msgStr.Append(message);
                msgStr.Append("\t");

                //
                // Make sure this is not just an error message with no relevant exception.
                //
                if (args.Exception != null)
                {
                    // Get all exception messages if exceptions are nested
                    GetExceptionMessage(args.Exception, out var exceptionMsg);
                    msgStr.Append("Exception message: " + exceptionMsg);
                }
                // Write the message to the log file
                WriteToLogFile(msgStr.ToString());
            }
                // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
                // if something dies...oh well..
            }
        }

        public override void LogMessage(int msgLevel, MessageLoggerArgs args)
        {
            if (msgLevel > MessageLevel)
            {
                return;
            }

            // Build the message string
            //var msgStr = new StringBuilder(DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0)).ToString("MM/dd/yyyy HH:mm:ss.fff"));
            var msgStr = new StringBuilder(TimeKeeper.Instance.Now.ToString("MM/dd/yyyy HH:mm:ss.fff"));
            msgStr.Append("\t");
            msgStr.Append("MSG");
            msgStr.Append("\t");
            msgStr.Append(msgLevel.ToString());
            msgStr.Append("\t");
            msgStr.Append("Message: " + args.Message);

            // Write the message to the log file
            WriteToLogFile(msgStr.ToString());
        }

        /// <summary>
        /// Unwraps nested exception messages
        /// </summary>
        /// <param name="ex">Input exception</param>
        /// <param name="msg">Message(s) contained in exception</param>
        private void GetExceptionMessage(Exception ex, out string msg)
        {
            msg = ex.Message + "\n" + ex.StackTrace;
            if (ex.InnerException != null)
            {
                GetExceptionMessage(ex.InnerException, out var innerMsg);
                // adding \t prior to \n, so that it is separated from the previous text in notepad.
                msg += "\t\nInner exception: " + innerMsg;
            }
        }

        private void ReportLogFilePath(string logFilePath)
        {
            LogFilePathDefined?.Invoke(new MessageLoggerArgs(0, logFilePath));
        }

        private bool SetupLogFile()
        {
            if (logWriter != null)
            {
                return true;
            }

            try
            {
                FileInfo logFile;

                //
                // We always create a new file every time we run the program.
                // Here we check to see that the file has been created before
                // because our file names will be Date_TimeOfDay which
                // will change.
                //
                if (logFileCreated == false || string.IsNullOrWhiteSpace(LogPath))
                {
                    var path = CreateLogFilePath();
                    LogPath = path;
                    logFile = new FileInfo(path);

                    ReportLogFilePath(LogPath);
                    logFileCreated = true;
                }
                else
                {
                    logFile = new FileInfo(LogPath);
                }

                if (logFile.Directory == null)
                    return false;

                //
                // Create the folder if it does not exist
                //
                if (!logFile.Directory.Exists)
                {
                    logFile.Directory.Create();
                }

                if (logFile.Directory.Exists)
                {
                    logWriter = new StreamWriter(new FileStream(logFile.FullName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite));

                    // Set AutoFlush to true to help ensure full log output.
                    logWriter.AutoFlush = true;
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception creating error log file", ex);
            }

            return false;
        }

        private void CloseLogFile(object sender)
        {
            lock (fileWriteLock)
            {
                logWriter?.Close();
                logWriter?.Dispose();
                logWriter = null;
            }
        }

        /// <summary>
        /// Writes a string to the log file
        /// </summary>
        /// <param name="msgStr">String to write</param>
        private void WriteToLogFile(string msgStr)
        {
            lock (fileWriteLock)
            {
                if (!SetupLogFile())
                {
                    return;
                }

                try
                {
                    logWriter.WriteLine(msgStr);
                }
                catch (IOException)
                {
                    // Ignore IO Exceptions
                }
                catch (Exception ex)
                {
                    throw new Exception("Exception writing error log file", ex);
                }
            }
        }

        /// <summary>
        /// Creates a log file name
        /// </summary>
        /// <returns>Name and path of error log file</returns>
        private string CreateLogFilePath()
        {
            var appPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            //string logFileName = "Log_" + DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0)).ToString("MMddyyyy_HHmmss") + ".txt";
            var logFileName = "Log_" + TimeKeeper.Instance.Now.ToString("yyyyMMdd_HHmmss") + ".txt";
            var path = Path.Combine(appPath, AppFolder);
            return Path.Combine(Path.Combine(path, "Log"), logFileName);
        }

        #endregion

        public void Dispose()
        {
            logFileStreamTimeout.Dispose();
            logWriter?.Close();
            logWriter?.Dispose();
        }
    }
}