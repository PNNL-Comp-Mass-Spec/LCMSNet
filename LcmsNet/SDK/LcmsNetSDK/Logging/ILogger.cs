using System;

namespace LcmsNetDataClasses.Logging
{
    public interface ILogger
    {
        /// <summary>
        /// Logs an error to the listening error output streams
        /// </summary>
        /// <param name="errorLevel">Level of the error message so more verbose errors can be filtered (0 is most important, 5 is least important)</param>
        /// <param name="message">Error message</param>
        /// <param name="ex">Exception</param>
        /// <param name="sample">Data for a sample</param>
        void LogError(int errorLevel, string message, Exception ex, classSampleData sample);

        /// <summary>
        /// Logs an error to the listening error output streams
        /// </summary>
        /// <param name="errorLevel">Level of the error message so more verbose errors can be filtered (0 is most important, 5 is least important)</param>
        /// <param name="message">Error message</param>
        /// <param name="ex">Exception</param>
        void LogError(int errorLevel, string message, Exception ex);

        /// <summary>
        /// Logs an error to the listening error output streams
        /// </summary>
        /// <param name="errorLevel">Level of the error message so more verbose errors can be filtered (0 is most important, 5 is least important)</param>
        /// <param name="message">Error message</param>
        /// <param name="sample">Data for a sample</param>
        void LogError(int errorLevel, string message, classSampleData sample);

        /// <summary>
        /// Logs an error to the listening error output streams
        /// </summary>
        /// <param name="errorLevel">Level of the error message so more verbose errors can be filtered (0 is most important, 5 is least important)</param>
        /// <param name="message">Error message</param>
        void LogError(int errorLevel, string message);

        /// <summary>
        /// Logs a message to the listening message output streams.
        /// </summary>
        /// <param name="messageLevel">Level of the message so more verbose messages can be filtered (0 is most important, 5 is least important)</param>
        /// <param name="message">Message to log</param>
        void LogMessage(int messageLevel, string message);

        /// <summary>
        /// Logs a message to the listening message output streams.
        /// </summary>
        /// <param name="messageLevel">Level of the message so more verbose messages can be filtered (0 is most important, 5 is least important)</param>
        /// <param name="message">Message to log</param>
        /// <param name="sample">Sample data</param>
        void LogMessage(int messageLevel, string message, classSampleData sample);
    }
}