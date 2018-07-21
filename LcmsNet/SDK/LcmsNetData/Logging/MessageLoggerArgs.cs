﻿using System;
using LcmsNetData.Data;

namespace LcmsNetData.Logging
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
            Sample = null;
        }

        /// <summary>
        /// Constructor that takes the error message.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="sample"></param>
        [Obsolete("Use constructor with logLevel and default parameters", true)]
        public MessageLoggerArgs(string message, SampleDataBasic sample)
        {
            Message = message;
            Sample = sample;
        }

        /// <summary>
        /// Constructor that takes the error message.
        /// </summary>
        /// <param name="logLevel">Error logging level</param>
        /// <param name="message">Error message</param>
        /// <param name="sample"></param>
        public MessageLoggerArgs(int logLevel, string message, SampleDataBasic sample = null)
        {
            LogLevel = logLevel;
            Message = message;
            Sample = sample;
        }

        public int LogLevel { get; }

        /// <summary>
        /// Gets the error message associated with the error.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the sample data associated with this method.
        /// </summary>
        public SampleDataBasic Sample { get; }
    }
}