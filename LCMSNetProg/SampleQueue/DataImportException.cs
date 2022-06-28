using System;

namespace LcmsNet.SampleQueue
{
    /// <summary>
    /// Custom exception for reporting problems during data import
    /// </summary>
    class DataImportException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message to accompany exception</param>
        /// <param name="ex">Exception to report</param>
        public DataImportException(string message, Exception ex) :
            base(message, ex)
        {
        }
    }
}
