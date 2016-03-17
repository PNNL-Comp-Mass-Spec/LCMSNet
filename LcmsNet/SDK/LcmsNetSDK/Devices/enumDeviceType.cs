using System;

namespace LcmsNetDataClasses.Devices
{
    /// <summary>
    /// Enumeration of the type of device.
    /// </summary>
    public enum enumDeviceType
    {
        /// <summary>
        /// Component is one created by the user.
        /// </summary>
        Component,

        /// <summary>
        /// A system type is one that is built-into the application.
        /// </summary>
        BuiltIn,

        /// <summary>
        /// Virtual devices can be instantiated multiple times.
        /// </summary>
        Virtual,

        /// <summary>
        /// Virtual device that has no effect on the system but is not virtual
        /// </summary>
        Fluidics
    }
}