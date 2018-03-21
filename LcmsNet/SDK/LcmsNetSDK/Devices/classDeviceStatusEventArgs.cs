using System;
using LcmsNetSDK.Notifications;

namespace LcmsNetDataClasses.Devices
{
    public class classDeviceStatusEventArgs : EventArgs
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="status"></param>
        /// <param name="notification"></param>
        /// <param name="notifier"></param>
        /// <param name="message"></param>
        /// <param name="loggingType"></param>
        public classDeviceStatusEventArgs(enumDeviceStatus status, string notification, INotifier notifier, string message = "", DeviceEventLoggingType loggingType = DeviceEventLoggingType.None)
        {
            Message = message;
            Status = status;
            Notification = notification;
            Notifier = notifier;
            LoggingType = loggingType;
        }

        [Obsolete("Use other constructor", true)]
        public classDeviceStatusEventArgs(enumDeviceStatus status, string notification, string message, INotifier notifier)
            : this(status, notification, notifier, message)
        {
        }

        public INotifier Notifier { get; private set; }

        #region Properties

        /// <summary>
        /// Gets the device status.
        /// </summary>
        public enumDeviceStatus Status { get; private set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Notification string
        /// </summary>
        public string Notification { get; private set; }

        /// <summary>
        /// How the event should be logged
        /// </summary>
        public DeviceEventLoggingType LoggingType { get; set; }

        #endregion
    }
}