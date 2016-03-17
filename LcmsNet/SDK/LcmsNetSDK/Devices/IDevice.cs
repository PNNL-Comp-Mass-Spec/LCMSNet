//*********************************************************************************************************
// Written by John Ryan, Dave Clark, Brian LaMarche for the US Department of Energy 
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 08/17/2009
//
// Last modified 08/17/2009
/*     BLL - 9-14-2009
 *          Added device save and load settings method to XML. 
 *     BLL - 1-18-2010
 *          Added the ManualResetEvent that the device can use to synchronize with scheduling objects.
 */
//*********************************************************************************************************

using System;
using System.Xml;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using LcmsNetDataClasses.Logging;
using LcmsNetSDK.Notifications;

namespace LcmsNetDataClasses.Devices
{
    public interface IDevice : INotifier
    {
        #region Events

        /// <summary>
        /// Fired when a property changes in the device.
        /// </summary>
        event EventHandler DeviceSaveRequired;

        #endregion

        #region Methods

        /// <summary>
        /// Calls an initialization sequence for the device to perform after construction.
        /// </summary>
        /// <returns>True if initialization successful.  False if initialization failed.</returns>
        bool Initialize(ref string errorMessage);

        /// <summary>
        /// Calls a shutdown sequence for the device to stop all acquiring/control.
        /// </summary>
        /// <returns>True if shutdown successful.  False if failure occured.</returns>
        bool Shutdown();

        /// <summary>
        /// Register controls that are being disposed for a given data provider.
        /// </summary>
        /// <param name="key">Data provider name</param>
        /// <param name="remoteMethod">Delegate method to call from the device to synch events to.</param>
        void RegisterDataProvider(string key, DelegateDeviceHasData remoteMethod);

        /// <summary>
        /// De-register controls that are being disposed for a given data provider.
        /// </summary>
        /// <param name="key">Data provider name</param>
        /// <param name="remoteMethod">Delegate method to call from the device to synch events to.</param>
        void UnRegisterDataProvider(string key, DelegateDeviceHasData remoteMethod);

        /// <summary>        
        /// Write the performance data and other required information associated with this device after a run.
        /// </summary>
        /// <param name="directoryPath">Path to write data to.</param>
        /// <param name="methodName">Name of method to gather performance data about.</param>
        /// <param name="parameters">Parameter data to use when writing output.</param>
        void WritePerformanceData(string directoryPath, string methodName, object[] parameters);

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the version name of the hardware if applicable.
        /// </summary>
        string Version { get; set; }

        /// <summary>
        /// Gets or sets the status of the device. 
        /// </summary>
        enumDeviceStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the abort event for scheduling.
        /// </summary>
        ManualResetEvent AbortEvent { get; set; }

        /// <summary>
        /// Gets or sets the error type for a given device.
        /// </summary>
        enumDeviceErrorStatus ErrorType { get; set; }

        /// <summary>
        /// Gets or sets the device type.
        /// </summary>
        enumDeviceType DeviceType { get; }

        /// <summary>
        /// Gets or sets whether the device is emulation mode or not.
        /// </summary>
        bool Emulation { get; set; }

        #endregion
    }
}