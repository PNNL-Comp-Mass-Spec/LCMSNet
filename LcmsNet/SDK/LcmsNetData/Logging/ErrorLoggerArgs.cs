﻿using System;
using LcmsNetData.Data;

namespace LcmsNetData.Logging
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
        public ErrorLoggerArgs(string message) :
            base(message)
        {
            Exception = null;
        }

        /// <summary>
        /// Constructor that takes the error message.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="sample"></param>
        public ErrorLoggerArgs(string message, SampleDataBasic sample) :
            base(message, sample)
        {
            Exception = null;
        }

        /// <summary>
        /// Constructor that takes an error message and an exception.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="ex">Exception found</param>
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
        /// <param name="sample"></param>
        public ErrorLoggerArgs(string message, Exception ex, SampleDataBasic sample) :
            base(message, sample)
        {
            Exception = ex;
        }

        /// <summary>
        /// Gets or sets the exception associated with the error message if any.
        /// </summary>
        public Exception Exception { get; set; }
    }
}