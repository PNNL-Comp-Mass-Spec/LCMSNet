using System;

namespace XcaliburControl
{
    /// <summary>
    /// Event argument class when an Xcalibur device or software has a status update.
    /// </summary>
    public class XcaliburStatusEventArgs : EventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageHeader"></param>
        public XcaliburStatusEventArgs(string message, string messageHeader)
        {
            Message = message;
            MessageHeader = messageHeader;
        }

        /// <summary>
        /// Message text
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Message header/message group
        /// </summary>
        public string MessageHeader { get; }
    }
}
