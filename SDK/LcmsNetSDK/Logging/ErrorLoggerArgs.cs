using System;

namespace LcmsNetSDK.Logging
{
    /// <summary>
    /// Class that encapsulates an error found by a component.
    /// </summary>
    public class ErrorLoggerArgs : MessageLoggerArgs
    {
        /// <summary>
        /// Constructor that takes the error message.
        /// </summary>
        /// <param name="message">Error message</param>
        [Obsolete("Use constructor with logLevel and default parameters", true)]
        public ErrorLoggerArgs(string message) :
            base(message)
        {
            Exception = null;
        }

        /// <summary>
        /// Constructor that takes the error message.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="errorObject"></param>
        [Obsolete("Use constructor with logLevel and default parameters", true)]
        public ErrorLoggerArgs(string message, object errorObject) :
            base(message, errorObject)
        {
            Exception = null;
        }

        /// <summary>
        /// Constructor that takes an error message and an exception.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="ex">Exception found</param>
        [Obsolete("Use constructor with logLevel and default parameters", true)]
        public ErrorLoggerArgs(string message, Exception ex) :
            base(message)
        {
            Exception = ex;
        }

        /// <summary>
        /// Constructor that takes an error message and an exception.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="ex">Exception found</param>
        /// <param name="errorObject"></param>
        [Obsolete("Use constructor with logLevel and default parameters", true)]
        public ErrorLoggerArgs(string message, Exception ex, object errorObject) :
            base(message, errorObject)
        {
            Exception = ex;
        }

        /// <summary>
        /// Constructor that takes an error message and an exception.
        /// </summary>
        /// <param name="logLevel">Error logging level</param>
        /// <param name="message">Error message</param>
        /// <param name="ex">Exception found</param>
        /// <param name="errorObject"></param>
        public ErrorLoggerArgs(int logLevel, string message, Exception ex = null, object errorObject = null) : base(logLevel, message, errorObject)
        {
            Exception = ex;
        }

        /// <summary>
        /// Gets the exception associated with the error message if any.
        /// </summary>
        public Exception Exception { get; }
    }
}
