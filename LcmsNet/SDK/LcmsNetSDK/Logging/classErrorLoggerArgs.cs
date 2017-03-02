using System;

namespace LcmsNetDataClasses.Logging
{
    /// <summary>
    /// Class that encapsulates an error found by a component.
    /// </summary>
    public class classErrorLoggerArgs : classMessageLoggerArgs
    {
        /// <summary>
        /// Constructor that takes the error message.
        /// </summary>
        /// <param name="message">Error message</param>
        public classErrorLoggerArgs(string message) :
            base(message)
        {
            Exception = null;
        }

        /// <summary>
        /// Constructor that takes the error message.
        /// </summary>
        /// <param name="message">Error message</param>
        public classErrorLoggerArgs(string message, classSampleData sample) :
            base(message, sample)
        {
            Exception = null;
        }

        /// <summary>
        /// Constructor that takes an error message and an exception.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="ex">Exception found</param>
        public classErrorLoggerArgs(string message, Exception ex) :
            base(message)
        {
            Exception = ex;
        }

        /// <summary>
        /// Constructor that takes an error message and an exception.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="ex">Exception found</param>
        public classErrorLoggerArgs(string message, Exception ex, classSampleData sample) :
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