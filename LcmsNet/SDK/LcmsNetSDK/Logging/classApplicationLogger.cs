using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LcmsNetDataClasses.Logging
{
    /// <summary>
    /// Class that marshalls messages from different components to different logging and streaming capabilities.
    /// </summary>
    public class classApplicationLogger: ILogger
    {
        /// <summary>
        /// Critical and should always be loged.
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
        /// Delegate method handler defining how a message event will be called.
        /// </summary>
        /// <param name="messageLevel"></param>
        /// <param name="args"></param>
        public delegate void DelegateMessageHandler(int messageLevel, classMessageLoggerArgs args);
        
        /// <summary>
        /// Delegate method handler defining how an error event will be called.
        /// </summary>
        /// <param name="errorLevel"></param>
        /// <param name="args"></param>
        public delegate void DelegateErrorHandler(int errorLevel, classErrorLoggerArgs args);
        
        /// <summary>
        /// Found when the application finds a message.
        /// </summary>
        public static event DelegateMessageHandler Message;
        
        /// <summary>
        /// Fired when the application has an error.
        /// </summary>
        public static event DelegateErrorHandler Error;

        private static int mint_errorLevel = 0;
        private static int mint_messageLevel = 0;

        /// <summary>
        /// Gets or sets the error level to log.
        /// </summary>
        public static int ErrorLevel
        {
            get
            {
                return mint_errorLevel;
            }
            set
            {
                mint_errorLevel = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the message level to log.  
        /// </summary>
        public static int MessageLevel
        {
            get
            {
                return mint_messageLevel;
            }
            set
            {
                mint_messageLevel = value;
            }
        }

        #region Error Methods
        /// <summary>
        /// Logs an error to the listening error output streams.
        /// </summary>
        /// <param name="errorLevel">Level of the error message so more verbose errors can be filtered.</param>
        /// <param name="message">Error message to report.</param>
        /// <param name="ex">An associated exception</param>
        public static void LogError(int errorLevel, string message, Exception ex, classSampleData sample)
        {
            if (errorLevel <= mint_errorLevel)
                if (Error != null)
                    Error(errorLevel, new classErrorLoggerArgs(message, ex, sample));
        }
        /// <summary>
        /// Logs an error to the listening error output streams.
        /// </summary>
        /// <param name="errorLevel">Level of the error message so more verbose errors can be filtered.</param>
        /// <param name="message">Error message to report.</param>
        /// <param name="ex">An associated exception</param>
        public static void LogError(int errorLevel, string message, Exception ex)
        {
            if (errorLevel <= mint_errorLevel)
                if (Error != null)
                    Error(errorLevel, new classErrorLoggerArgs(message, ex));
        }
        /// <summary>
        /// Logs an error to the listening error output streams.
        /// </summary>
        /// <param name="errorLevel">Level of the error message so more verbose errors can be filtered.</param>
        /// <param name="message">Error message to report.</param>
        public static void LogError(int errorLevel, string message, classSampleData sample)
        {
            if (errorLevel <= mint_errorLevel)
                if (Error != null)
                    Error(errorLevel, new classErrorLoggerArgs(message, sample));
        }
        /// <summary>
        /// Logs an error to the listening error output streams.
        /// </summary>
        /// <param name="errorLevel">Level of the error message so more verbose errors can be filtered.</param>
        /// <param name="message">Error message to report.</param>
        public static void LogError(int errorLevel, string message)
        {
            if (errorLevel <= mint_errorLevel)
                if (Error != null)
                    Error(errorLevel, new classErrorLoggerArgs(message));
        }
        #endregion

        #region Messages 
        /// <summary>
        /// Logs a message to the listening message output streams.
        /// </summary>
        /// <param name="messageLevel"></param>
        /// <param name="message"></param>
        public static void LogMessage(int messageLevel, string message)
        {
            if (messageLevel <= mint_errorLevel)
                if (Message != null)
                    Message(messageLevel, new classMessageLoggerArgs(message));            
        }

        /// <summary>
        /// Logs a message to the listening message output streams.
        /// </summary>
        /// <param name="messageLevel"></param>
        /// <param name="message"></param>
        /// <param name="sample"></param>
        public static void LogMessage(int messageLevel, string message, classSampleData sample)
        {
            if (messageLevel <= mint_errorLevel)
                if (Message != null)
                    Message(messageLevel, new classMessageLoggerArgs(message, sample));
        }
        #endregion

        #region ILogger Members

        void ILogger.LogError(int errorLevel, string message, Exception ex, classSampleData sample)
        {
            classApplicationLogger.LogError(errorLevel, message, ex, sample);
        }

        void ILogger.LogError(int errorLevel, string message, Exception ex)
        {
            classApplicationLogger.LogError(errorLevel, message, ex);
        }

        void ILogger.LogError(int errorLevel, string message, classSampleData sample)
        {
            classApplicationLogger.LogError(errorLevel, message, sample);
        }

        void ILogger.LogError(int errorLevel, string message)
        {
            classApplicationLogger.LogError(errorLevel, message);
        }

        void ILogger.LogMessage(int messageLevel, string message)
        {
            classApplicationLogger.LogMessage(messageLevel, message);
        }

        void ILogger.LogMessage(int messageLevel, string message, classSampleData sample)
        {
            classApplicationLogger.LogMessage(messageLevel, message, sample);
        }

        #endregion
    }   
}
