using System;

namespace XcaliburControl
{
    /// <summary>
    /// Event argument class when an Xcalibur device or software has an error.
    /// </summary>
    public class XcaliburErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageHeader"></param>
        /// <param name="ex"></param>
        public XcaliburErrorEventArgs(string message, string messageHeader, Exception ex = null)
        {
            Message = message;
            MessageHeader = messageHeader;
            HasException = ex != null;
            Exception = ex;
        }

        /// <summary>
        /// Message text
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Message header/message group
        /// </summary>
        public string MessageHeader { get; }

        /// <summary>
        /// True if an exception object is included
        /// </summary>
        public bool HasException { get; }

        /// <summary>
        /// Gets or sets the Exception (if any involved).
        /// </summary>
        public Exception Exception { get; }
    }
}
