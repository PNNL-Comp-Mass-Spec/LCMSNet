using System;
using System.Collections.Generic;

namespace LcmsNetDataClasses.Devices
{
    public class classDeviceErrorArgs: System.EventArgs
    {
        public classDeviceErrorArgs(IDevice device, string error, enumDeviceErrorStatus status, Exception ex)
        {
            Device    = device;
            Error     = error;
            Exception = ex;
            ErrorStatus = status;
        }
        /// <summary>
        /// Gets or sets the device that had an error.
        /// </summary>
        public IDevice Device { get; set; }
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
    }
}
