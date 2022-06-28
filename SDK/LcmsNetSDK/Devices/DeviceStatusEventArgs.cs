using System;
using LcmsNetSDK.System;

namespace LcmsNetSDK.Devices
{
    public class DeviceStatusEventArgs : EventArgs
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="status"></param>
        /// <param name="notification"></param>
        /// <param name="notifier"></param>
        /// <param name="message"></param>
        /// <param name="loggingType"></param>
        public DeviceStatusEventArgs(DeviceStatus status, string notification, INotifier notifier, string message = "", DeviceEventLoggingType loggingType = DeviceEventLoggingType.None)
        {
            Message = message;
            Status = status;
            Notification = notification;
            Notifier = notifier;
            LoggingType = loggingType;
        }

        [Obsolete("Use other constructor", true)]
        public DeviceStatusEventArgs(DeviceStatus status, string notification, string message, INotifier notifier)
            : this(status, notification, notifier, message)
        {
        }

        public INotifier Notifier { get; }

        /// <summary>
        /// Gets the device status.
        /// </summary>
        public DeviceStatus Status { get; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Notification string
        /// </summary>
        public string Notification { get; }

        /// <summary>
        /// How the event should be logged
        /// </summary>
        public DeviceEventLoggingType LoggingType { get; set; }
    }
}
