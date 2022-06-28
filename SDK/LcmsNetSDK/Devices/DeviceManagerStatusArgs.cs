using System;

namespace LcmsNetSDK.Devices
{
    /// <summary>
    /// Arguments for manager status arguments.
    /// </summary>
    public class DeviceManagerStatusArgs : EventArgs
    {
        public DeviceManagerStatusArgs(string message)
        {
            Message = message;
        }

        /// <summary>
        /// Gets the status message.
        /// </summary>
        public string Message { get; }
    }
}
