using System;
using System.Linq;
using System.Threading.Tasks;
using LcmsNetData.Data;

namespace LcmsNetData.Logging
{
    /// <summary>
    /// Class that marshals messages from different components to different logging and streaming capabilities.
    /// </summary>
    public static class ApplicationLogger
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
        public const int CONST_STATUS_LEVEL_USER = 3;

        public static LogLevel ConvertIntToLogLevel(int level)
        {
            var levels = Enum.GetValues(typeof(LogLevel)).Cast<int>().OrderBy(x => x).ToList();

            // return the first logLevel that is greater than or equal to the specified level. This means that a
            foreach (var logLevel in levels)
            {
                if (logLevel >= level)
                {
                    return (LogLevel) logLevel;
                }
            }

            return (LogLevel) levels.Max();
        }

        /// <summary>
        /// Found when the application finds a message.
        /// </summary>
        public static event DelegateMessageHandler Message;

        /// <summary>
        /// Fired when the application has an error.
        /// </summary>
        public static event DelegateErrorHandler Error;

        /// <summary>
        /// Logs an error to the listening error output streams
        /// </summary>
        /// <param name="errorLevel">Level of the error message so more verbose errors can be filtered (0 is most important, 5 is least important)</param>
        /// <param name="message">Error message</param>
        /// <param name="ex">Exception</param>
        /// <param name="sample">Data for a sample</param>
        public static void LogError(int errorLevel, string message, Exception ex = null, SampleDataBasic sample = null)
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

            Task.Run(() => Error?.Invoke(errorLevel, args));
        }

        /// <summary>
        /// Logs an error to the listening error output streams
        /// </summary>
        /// <param name="errorLevel">Level of the error message so more verbose errors can be filtered</param>
        /// <param name="message">Error message</param>
        /// <param name="ex">Exception</param>
        /// <param name="sample">Data for a sample</param>
        public static void LogError(LogLevel errorLevel, string message, Exception ex = null, SampleDataBasic sample = null)
        {
            LogError((int)errorLevel, message, ex, sample);
        }

        /// <summary>
        /// Logs a message to the listening message output streams.
        /// </summary>
        /// <param name="messageLevel">Level of the message so more verbose messages can be filtered (0 is most important, 5 is least important)</param>
        /// <param name="message">Message to log</param>
        /// <param name="sample">Sample data</param>
        /// <remarks>
        /// When MessageLevel is 0, only critical errors are logged
        /// When MessageLevel is 5, all messages are logged
        /// </remarks>
        public static void LogMessage(int messageLevel, string message, SampleDataBasic sample = null)
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

            Task.Run(() => Message?.Invoke(messageLevel, args));
        }

        /// <summary>
        /// Logs a message to the listening message output streams.
        /// </summary>
        /// <param name="messageLevel">Level of the message so more verbose messages can be filtered</param>
        /// <param name="message">Message to log</param>
        /// <param name="sample">Sample data</param>
        /// <remarks>
        /// When MessageLevel is 0, only critical errors are logged
        /// When MessageLevel is 5, all messages are logged
        /// </remarks>
        public static void LogMessage(LogLevel messageLevel, string message, SampleDataBasic sample = null)
        {
            LogMessage((int)messageLevel, message, sample);
        }
    }
}