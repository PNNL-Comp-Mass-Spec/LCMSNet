using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LcmsNetDataClasses.Devices;

namespace LcmsNetSDK.Notifications
{
    /// <summary>
    /// Inteface used to notify the sysetm when something bad has happened (or status)
    /// </summary>
    public interface INotifier
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
        event EventHandler<classDeviceStatusEventArgs> StatusUpdate;

        /// <summary>
        /// Fired when an error occurs on a device.
        /// </summary>
        event EventHandler<classDeviceErrorEventArgs> Error;
    }
}