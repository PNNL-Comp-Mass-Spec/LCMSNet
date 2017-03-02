using System;
using System.Collections.Generic;

namespace LcmsNetDataClasses.Devices
{
    /// <summary>
    /// Event argument class when a device has an error.
    /// </summary>
    public class classDeviceErrorEventArgs : System.EventArgs
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
        public classDeviceErrorEventArgs(string errorMessage,
            Exception ex,
            enumDeviceErrorStatus status,
            IDevice device,
            string notifyStr)
        {
            Error = errorMessage;
            Exception = ex;
            ErrorStatus = status;
            Device = device;
            Notification = notifyStr;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="ex"></param>
        /// <param name="status"></param>
        /// <param name="device"></param>
        /// <param name="notifyStr"></param>
        public classDeviceErrorEventArgs(string errorMessage,
            Exception ex,
            enumDeviceErrorStatus status,
            IDevice device)
        {
            Error = errorMessage;
            Exception = ex;
            ErrorStatus = status;
            Device = device;
            Notification = "None";
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
        public enumDeviceErrorStatus ErrorStatus { get; set; }

        /// <summary>
        /// Device
        /// </summary>
        public IDevice Device { get; private set; }

        /// <summary>
        /// Notification string
        /// </summary>
        public string Notification { get; set; }
    }
}