namespace LcmsNetData.Logging
{
    public abstract class LogWriterBase : IHandlesLogging
    {
        protected LogWriterBase()
        {
            ErrorLevel = 5;
            MessageLevel = 5;
        }

        /// <summary>
        /// Error message importance level (0 is most important, 5 is least important)
        /// </summary>
        public int ErrorLevel { get; set; }

        /// <summary>
        /// Status message importance level (0 is most important, 5 is least important)
        /// </summary>
        /// <remarks>
        /// When MessageLevel is 0, only critical errors are logged
        /// When MessageLevel is 5, all messages are logged
        /// </remarks>
        public int MessageLevel { get; set; }

        /// <summary>
        /// Error message importance level
        /// </summary>
        public LogLevel ErrorLogLevel
        {
            get => ApplicationLogger.ConvertIntToLogLevel(ErrorLevel);
            set => ErrorLevel = (int)value;
        }

        /// <summary>
        /// Status message importance level
        /// </summary>
        public LogLevel MessageLogLevel
        {
            get => ApplicationLogger.ConvertIntToLogLevel(MessageLevel);
            set => MessageLevel = (int)value;
        }

        /// <summary>
        /// Logs an error
        /// </summary>
        /// <param name="errorLevel">Error level</param>
        /// <param name="args">Message arguments</param>
        public abstract void LogError(int errorLevel, ErrorLoggerArgs args);

        /// <summary>
        /// Logs a message
        /// </summary>
        /// <param name="msgLevel">Message level</param>
        /// <param name="args">Message arguments</param>
        public abstract void LogMessage(int msgLevel, MessageLoggerArgs args);
    }
}
