using System;
using LcmsNetSDK.Devices;

namespace LcmsNet.Method
{
    /// <summary>
    /// Class that holds object device name and type definitions to validate
    /// that the device exists when a method is loaded.
    /// </summary>
    public class classLCMethodDeviceArgs
    {
        /// <summary>
        /// Gets or sets the name of the device to find.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the device type to search for.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Gets or sets the device if it exists.
        /// </summary>
        public IDevice Device { get; set; }
    }
}