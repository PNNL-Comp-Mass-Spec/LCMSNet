using System;
using System.Threading.Tasks;
using LcmsNetSDK.Data;

namespace LcmsNetSDK.Logging
{
    /// <summary>
    /// Class that marshalls messages from different components to different logging and streaming capabilities.
    /// </summary>
    public class ApplicationLogger : ILogger
    {
        /// <summary>
        /// Delegate method handler defining how an error event will be called.
        /// </summary>
        /// <param name="errorLevel"></param>
        /// <param name="args"></param>
        public delegate void DelegateErrorHandler(int errorLevel, ErrorLoggerArgs args);

        /// <summary>
        /// Delegate method handler defining how a message event will be called.
        /// </summary>
        /// <param name="messageLevel"></param>
        /// <param name="args"></param>
        public delegate void DelegateMessageHandler(int messageLevel, MessageLoggerArgs args);

        /// <summary>
        /// Critical and should always be logged.
        /// </summary>
        public const int CONST_STATUS_LEVEL_CRITICAL = 0;

        /// <summary>
        /// More detailed error levels.
        /// </summary>
        public const int CONST_STATUS_LEVEL_DETAILED = 1;

        /// <summary>
        /// Less likely to need to be logged.
        /// </summary>
        public const int CONST_STATUS_LEVEL_USER = 2;

        /// <summary>
        /// Error message importance level (0 is most important, 5 is least important)
        /// </summary>
        public static int ErrorLevel { get; set; }

        /// <summary>
        /// Status message importance level (0 is most important, 5 is least important)
        /// </summary>
        /// <remarks>
        /// When MessageLevel is 0, only critical errors are logged
        /// When MessageLevel is 5, all messages are logged
        /// </remarks>
        public static int MessageLevel { get; set; }

        /// <summary>
        /// Found when the application finds a message.
        /// </summary>
        public static event DelegateMessageHandler Message;

        /// <summary>
        /// Fired when the application has an error.
        /// </summary>
        public static event DelegateErrorHandler Error;

        #region Error Methods

        /// <summary>
        /// Logs an error to the listening error output streams.
        /// </summary>
        /// <param name="errorLevel">Level of the error message so more verbose errors can be filtered (0 is most important, 5 is least important)</param>
        /// <param name="message">Error message</param>
        public static void LogError(int errorLevel, string message)
        {
            LogError(errorLevel, message, null, null);
        }

        /// <summary>
        /// Logs an error to the listening error output streams.
        /// </summary>
        /// <param name="errorLevel">Level of the error message so more verbose errors can be filtered (0 is most important, 5 is least important)</param>
        /// <param name="message">Error message</param>
        /// <param name="ex">Exception</param>
        public static void LogError(int errorLevel, string message, Exception ex)
        {
            LogError(errorLevel, message, ex, null);
        }

        /// <summary>
        /// Logs an error to the listening error output streams
        /// </summary>
        /// <param name="errorLevel">Level of the error message so more verbose errors can be filtered (0 is most important, 5 is least important)</param>
        /// <param name="message">Error message</param>
        /// <param name="ex">Exception</param>
        /// <param name="sample">Data for a sample</param>
        public static void LogError(int errorLevel, string message, Exception ex, SampleData sample)
        {
            ErrorLoggerArgs args;
            if (sample != null)
            {
                args = new ErrorLoggerArgs(message, sample);
            }
            else
            {
                args = new ErrorLoggerArgs(message);
            }

            if (ex != null)
            {
                args.Exception = ex;
            }

            if (errorLevel <= ErrorLevel && Error != null)
            {
                Task.Run(() => Error?.Invoke(errorLevel, args));
            }
        }

        #endregion

        #region Messages

        /// <summary>
        /// Logs a message to the listening message output streams.
        /// </summary>
        /// <param name="messageLevel">Level of the message so more verbose messages can be filtered (0 is most important, 5 is least important)</param>
        /// <param name="message">Message to log</param>
        public static void LogMessage(int messageLevel, string message)
        {
            LogMessage(messageLevel, message, null);
        }

        /// <summary>
        /// Logs a message to the listening message output streams.
        /// </summary>
        /// <param name="messageLevel">Level of the message so more verbose messages can be filtered (0 is most important, 5 is least important)</param>
        /// <param name="message">Message to log</param>
        /// <param name="sample">Sample data</param>
        public static void LogMessage(int messageLevel, string message, SampleData sample)
        {
            MessageLoggerArgs args;
            if (sample != null)
            {
                args = new MessageLoggerArgs(message, sample);
            }
            else
            {
                args = new MessageLoggerArgs(message);
            }

            if (messageLevel <= ErrorLevel && Message != null)
            {
                Task.Run(() => Message?.Invoke(messageLevel, args));
            }
        }

        #endregion

        #region ILogger Members

        void ILogger.LogError(int errorLevel, string message, Exception ex, SampleData sample)
        {
            LogError(errorLevel, message, ex, sample);
        }

        void ILogger.LogError(int errorLevel, string message, Exception ex)
        {
            LogError(errorLevel, message, ex);
        }

        void ILogger.LogError(int errorLevel, string message, SampleData sample)
        {
            LogError(errorLevel, message, null, sample);
        }

        void ILogger.LogError(int errorLevel, string message)
        {
            LogError(errorLevel, message);
        }

        void ILogger.LogMessage(int messageLevel, string message)
        {
            LogMessage(messageLevel, message);
        }

        void ILogger.LogMessage(int messageLevel, string message, SampleData sample)
        {
            LogMessage(messageLevel, message, sample);
        }

        #endregion
    }
}