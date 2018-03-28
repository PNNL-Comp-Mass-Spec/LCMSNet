using System;

namespace LcmsNetSDK.Devices
{
    /// <summary>
    /// Event argument class when a device has an error.
    /// </summary>
    public class DeviceErrorEventArgs : EventArgs
    {
        /// <summary>
        /// String indicating that there is no notification required.
        /// </summary>
        public const string CONST_NO_NOTIFIER = "None";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="ex"></param>
        /// <param name="status"></param>
        /// <param name="device"></param>
        /// <param name="notifyStr"></param>
        /// <param name="loggingType"></param>
        public DeviceErrorEventArgs(string errorMessage,
            Exception ex,
            DeviceErrorStatus status,
            IDevice device,
            string notifyStr = "None",
            DeviceEventLoggingType loggingType = DeviceEventLoggingType.None)
        {
            Error = errorMessage;
            Exception = ex;
            ErrorStatus = status;
            Device = device;
            Notification = notifyStr;
            LoggingType = loggingType;
        }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// Gets or sets the Exception (if any involved).
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Error status of the device.
        /// </summary>
        public DeviceErrorStatus ErrorStatus { get; set; }

        /// <summary>
        /// Device
        /// </summary>
        public IDevice Device { get; private set; }

        /// <summary>
        /// Notification string
        /// </summary>
        public string Notification { get; set; }

        /// <summary>
        /// How the event should be logged
        /// </summary>
        public DeviceEventLoggingType LoggingType { get; set; }
    }
}