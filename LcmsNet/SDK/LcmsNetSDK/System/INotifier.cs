using System;
using System.Collections.Generic;
using LcmsNetSDK.Devices;

namespace LcmsNetSDK.System
{
    /// <summary>
    /// Inteface used to notify the system when something bad has happened (or status)
    /// </summary>
    public interface INotifier : INotifyPropertyChangedExt
    {
        string Name { get; set; }

        /// <summary>
        /// Get a list of possible status notifications for this device
        /// </summary>
        /// <returns>List of notifications</returns>
        List<string> GetStatusNotificationList();

        /// <summary>
        /// Gets a list of possible error notifications for this device
        /// </summary>
        /// <returns>List of notifications</returns>
        List<string> GetErrorNotificationList();

        /// <summary>
        /// Fired when the status of a device has changed.
        /// </summary>
        event EventHandler<DeviceStatusEventArgs> StatusUpdate;

        /// <summary>
        /// Fired when an error occurs on a device.
        /// </summary>
        event EventHandler<DeviceErrorEventArgs> Error;
    }
}