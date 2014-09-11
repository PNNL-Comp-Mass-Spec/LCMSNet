//*********************************************************************************************************
// Written by Dave Clark, Brian LaMarche for the US Department of Energy 
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 03/30/2009
//
// Last modified 03/30/2009
//     04/16/2009: BLL
//         Made file logging class static so it can be used at program startup
//         Changed file log name to include hhmmss using a flag to indicate whether the file exists.
//     
//*********************************************************************************************************
using System;
using System.IO;
using System.Text;

namespace LcmsNetDataClasses.Logging
{
    /// <summary>
    /// Logs errors and messages to a file
    /// </summary>
    public static class classFileLogging
    {

        /// <summary>
        /// Flag indicating whether a log file has been created for this program start.
        /// </summary>
        private static bool mbool_logFileCreated = false;
        private static string m_LogFilePath;

        #region Properties

        #region Events

        /// <summary>
        /// Delegate method handler defining how an error event will be called.
        /// </summary>
        /// <param name="args"></param>
        public delegate void DelegateLogPathReporter(classMessageLoggerArgs args);
        
        /// <summary>
        /// Fired when the log file path is defined
        /// </summary>
        public static event DelegateLogPathReporter LogFilePathDefined;

        #endregion

        /// <summary>
        /// Gets the file log path.
        /// </summary>
        public static string LogPath
        {
            get
            {
                return m_LogFilePath;
            }
        }

        /// <summary>
        /// Gets the file log path.
        /// </summary>
        public static string AppFolder
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        static classFileLogging()
        {
            AppFolder = "LCMSNet";
        }

        #region "Methods"
        public static void LogError(int errorLevel, classErrorLoggerArgs args)
        {
            try
            {
                string ExMsg;

                // Build the message string
                //var msgStr = new StringBuilder(DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0)).ToString("MM/dd/yyyy HH:mm:ss.fff"));
                var msgStr = new StringBuilder(LcmsNetSDK.TimeKeeper.Instance.Now.ToString("MM/dd/yyyy HH:mm:ss.fff"));
                msgStr.Append("\t");
                msgStr.Append("ERROR");
                msgStr.Append("\t");
                msgStr.Append(errorLevel.ToString());
                msgStr.Append("\t");
                string message = "Error message: " + args.Message + " ";
                if (args.Exception != null)
                {
                    message += args.Exception.Message + " " + args.Exception.StackTrace;
                }
                msgStr.Append(message);
                msgStr.Append("\t");

                // 
                // Make sure this is not just an error message with no relevant exception.
                // 
                if (args.Exception != null)
                {
                    // Get all exception messages if exceptions are nested
                    GetExceptionMessage(args.Exception, out ExMsg);
                    msgStr.Append("Exception message: " + ExMsg);
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

        public static void LogMessage(int msgLevel, classMessageLoggerArgs args)
        {
            // Build the message string
            //var msgStr = new StringBuilder(DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0)).ToString("MM/dd/yyyy HH:mm:ss.fff"));
            var msgStr = new StringBuilder(LcmsNetSDK.TimeKeeper.Instance.Now.ToString("MM/dd/yyyy HH:mm:ss.fff"));
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
        private static void GetExceptionMessage(Exception ex, out string msg)
        {
            msg = ex.Message;
            if (ex.InnerException != null)
            {
                string innerMsg;
                GetExceptionMessage(ex.InnerException, out innerMsg);
                msg += " Inner exception: " + innerMsg;
            }
        }


        private static void ReportLogFilePath(string logFilePath)
        {
            if (LogFilePathDefined != null)
                LogFilePathDefined(new classMessageLoggerArgs(logFilePath));
        }

        private static readonly object m_lockObject = new object();

        /// <summary>
        /// Writes a string to the log file
        /// </summary>
        /// <param name="msgStr">String to write</param>
        private static void WriteToLogFile(string msgStr)
        {
            lock (m_lockObject)
            {
                //System.Diagnostics.Debug.WriteLine("\tWriteToLogFile: Thread " + System.Threading.Thread.CurrentThread.Name + ": " + msgStr);
                try
                {
                    string path;
                    // 
                    // We always create a new file every time we run the program. 
                    // Here we check to see that the file has been created before
                    // because our file names will be Date_TimeOfDay which 
                    // will change.
                    // 
                    if (mbool_logFileCreated == false)
                    {
                        path = CreateLogFilePath();
                        m_LogFilePath = path;
                        ReportLogFilePath(m_LogFilePath);
                        mbool_logFileCreated = true;
                    }
                    else
                    {
                        path = m_LogFilePath;
                    }

                    // 
                    // Make sure the directory exists
                    // 
                    if (System.IO.Directory.Exists(Path.GetDirectoryName(path)) == true)
                    {
                        StreamWriter logWriter = new StreamWriter(path, true);
                        logWriter.WriteLine(msgStr);
                        logWriter.Flush();
                        logWriter.Close();
                    }
                }
                catch (IOException)
                {

                }
                catch (Exception Ex)
                {
                    throw new Exception("Exception writing error log file", Ex);
                }
            }
        }

        /// <summary>
        /// Creates a log file name
        /// </summary>
        /// <returns>Name and path of error log file</returns>
        private static string CreateLogFilePath()
        {
            var appPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            //string logFileName = "Log_" + DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0)).ToString("MMddyyyy_HHmmss") + ".txt";
            string logFileName = "Log_" +LcmsNetSDK.TimeKeeper.Instance.Now.ToString("MMddyyyy_HHmmss") + ".txt";
            var path = Path.Combine(appPath, AppFolder);
            return Path.Combine(Path.Combine(path, "Log"), logFileName);
        }
        #endregion

    }
}
