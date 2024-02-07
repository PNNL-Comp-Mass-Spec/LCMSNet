using System;

namespace ZaberStageControl
{
    /// <summary>
    /// Event Arguments for reporting warning messages
    /// </summary>
    public class ZaberStatusReportEventArgs : EventArgs
    {
        public ZaberStatusReportEventArgs(StatusReportType statusType, string message)
        {
            Message = message;
        }

        public StatusReportType StatusType { get; }
        public string Message { get; }
    }

    /// <summary>
    /// Event Arguments for reporting status and informational messages
    /// </summary>
    public class ZaberMessageEventArgs : EventArgs
    {
        public ZaberMessageEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }

    /// <summary>
    /// Event Arguments for reporting error messages and exceptions
    /// </summary>
    public class ZaberErrorEventArgs : EventArgs
    {
        public ZaberErrorEventArgs(string message, Exception ex = null)
        {
            Message = message;
            Exception = ex;
        }

        public string Message { get; }
        public Exception Exception { get; }
    }
}
