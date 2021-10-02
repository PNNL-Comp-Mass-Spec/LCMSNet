namespace LcmsNetSDK.Logging
{
    public interface IHandlesLogging
    {
        /// <summary>
        /// Error message importance level (0 is most important, 5 is least important)
        /// </summary>
        int ErrorLevel { get; set; }

        /// <summary>
        /// Status message importance level (0 is most important, 5 is least important)
        /// </summary>
        /// <remarks>
        /// When MessageLevel is 0, only critical errors are logged
        /// When MessageLevel is 5, all messages are logged
        /// </remarks>
        int MessageLevel { get; set; }

        /// <summary>
        /// Error message importance level
        /// </summary>
        LogLevel ErrorLogLevel { get; set; }

        /// <summary>
        /// Status message importance level
        /// </summary>
        LogLevel MessageLogLevel { get; set; }

        /// <summary>
        /// Logs an error
        /// </summary>
        /// <param name="errorLevel">Error level</param>
        /// <param name="args">Message arguments</param>
        void LogError(int errorLevel, ErrorLoggerArgs args);

        /// <summary>
        /// Logs a message
        /// </summary>
        /// <param name="msgLevel">Message level</param>
        /// <param name="args">Message arguments</param>
        void LogMessage(int msgLevel, MessageLoggerArgs args);
    }
}