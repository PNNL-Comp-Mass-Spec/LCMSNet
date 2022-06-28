using System;

namespace LcmsNet.SampleQueue
{
    /// <summary>
    /// Custom exception for reporting problems during data export
    /// </summary>
    class DataExportException : Exception
    {
        public DataExportException(string message, Exception ex) :
            base(message, ex)
        {
        }
    }
}
