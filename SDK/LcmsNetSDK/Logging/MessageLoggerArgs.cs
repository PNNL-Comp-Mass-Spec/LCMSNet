﻿using System;

namespace LcmsNetSDK.Logging
{
    /// <summary>
    /// Class that encapsulates a message generated by a component.
    /// </summary>
    public class MessageLoggerArgs : EventArgs
    {
        /// <summary>
        /// Constructor that takes the error message.
        /// </summary>
        /// <param name="message">Error message</param>
        [Obsolete("Use constructor with logLevel and default parameters", true)]
        public MessageLoggerArgs(string message)
        {
            Message = message;
            ErrorObject = null;
        }

        /// <summary>
        /// Constructor that takes the error message.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="errorObject"></param>
        [Obsolete("Use constructor with logLevel and default parameters", true)]
        public MessageLoggerArgs(string message, object errorObject)
        {
            Message = message;
            ErrorObject = errorObject;
        }

        /// <summary>
        /// Constructor that takes the error message.
        /// </summary>
        /// <param name="logLevel">Error logging level</param>
        /// <param name="message">Error message</param>
        /// <param name="errorObject"></param>
        public MessageLoggerArgs(int logLevel, string message, object errorObject = null)
        {
            LogLevel = logLevel;
            Message = message;
            ErrorObject = errorObject;
        }

        public int LogLevel { get; }

        /// <summary>
        /// Gets the error message associated with the error.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the sample data associated with this method.
        /// </summary>
        public object ErrorObject { get; }
    }
}
