﻿using System;
using System.Threading;
using LcmsNetSDK.System;

namespace LcmsNetSDK.Devices
{
    public interface IDevice : INotifier
    {
        /// <summary>
        /// Fired when a property changes in the device.
        /// </summary>
        event EventHandler DeviceSaveRequired;

        /// <summary>
        /// Calls an initialization sequence for the device to perform after construction.
        /// </summary>
        /// <returns>True if initialization successful.  False if initialization failed.</returns>
        bool Initialize(ref string errorMessage);

        /// <summary>
        /// Calls a shutdown sequence for the device to stop all acquiring/control.
        /// </summary>
        /// <returns>True if shutdown successful.  False if failure occurred.</returns>
        bool Shutdown();

        /// <summary>
        /// Write the performance data and other required information associated with this device after a run.
        /// </summary>
        /// <param name="directoryPath">Path to write data to.</param>
        /// <param name="methodName">Name of method to gather performance data about.</param>
        /// <param name="parameters">Parameter data to use when writing output.</param>
        void WritePerformanceData(string directoryPath, string methodName, object[] parameters);

        /// <summary>
        /// Gets or sets the status of the device.
        /// </summary>
        DeviceStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the abort event for scheduling.
        /// </summary>
        ManualResetEvent AbortEvent { get; set; }

        /// <summary>
        /// Gets the error type for a given device.
        /// </summary>
        DeviceErrorStatus ErrorType { get; }

        /// <summary>
        /// Gets the device type.
        /// </summary>
        DeviceType DeviceType { get; }

        /// <summary>
        /// Gets or sets whether the device is emulation mode or not.
        /// </summary>
        bool Emulation { get; set; }
    }
}
