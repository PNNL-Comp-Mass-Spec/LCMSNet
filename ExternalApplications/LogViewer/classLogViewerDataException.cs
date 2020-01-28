using System;

namespace LogViewer
{
    /// <summary>
    /// Log viewer custom exception class
    /// </summary>
    class classLogViewerDataException : Exception
    {
        #region "Constructors"
            public classLogViewerDataException(string message, Exception ex) :
                base(message,ex)
            {
            }
        #endregion
    }
}
