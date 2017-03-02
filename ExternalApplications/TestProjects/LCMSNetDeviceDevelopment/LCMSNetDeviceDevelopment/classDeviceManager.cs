
/*********************************************************************************************************
 * Written by Dave Clark, Brian LaMarche, John Ryan for the US Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2009, Battelle Memorial Institute
 * Created 06/19/2009
 *
 *  Last modified 06/19/2009
 *      Created class and added static methods with static device manager object that registers itself with
 *      the static method property.
 *
/*********************************************************************************************************/
using System;
using System.Collections.Generic;

using LcmsNetDataClasses.Devices;

namespace LcmsNet.Devices
{    
    /// <summary>
    /// Device manager class for maintaining a list of all devices used by the application.
    /// </summary>
    public class classDeviceManager
    {
        #region Members
        /// <summary>
        /// A current list of devices the application is using.
        /// </summary>
        private List<IDevice> mlist_devices;
        /// <summary>
        /// Static Device Manager Reference.
        /// </summary>
        private static classDeviceManager mobj_deviceManager;
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor.  Sets the static device manager object reference to this.
        /// </summary>
        public classDeviceManager()
        {
            mlist_devices       = new List<IDevice>();
            Manager             = this;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the list of registered devices.
        /// </summary>
        public List<IDevice> Devices
        {
            get
            {
                return mlist_devices;
            }
        }
        #endregion

        #region Device Name and Usage Checking
        /// <summary>
        /// Searches the device manager for a device with the same name.
        /// </summary>
        /// <param name="deviceName">Name to search the device manager for.</param>
        /// <returns>True if device name is in use.  False if the device name is free</returns>
        public bool DeviceNameExists(string deviceName)
        {
            if (deviceName == null)
                return false;

            foreach (IDevice dev in mlist_devices)
            {
                if (dev.Name == deviceName)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Adds the device to the device manager if the name is not a duplicate and the same object reference does not exist.
        /// </summary>
        /// <returns>True if the device was added.  False if device was not added.</returns>
        public bool AddDevice(IDevice device)
        {
            /// 
            /// No null devices allowed.
            /// 
            if (device == null)
                return false;

            /// 
            /// No duplicate references allowed.
            /// 
            if (mlist_devices.Contains(device) == true)
                return false;

            /// 
            /// No duplicate names allowed.
            /// 
            if (DeviceNameExists(device.Name))
                return false;

            mlist_devices.Add(device);

            return true;
        }
        /// <summary>
        /// Removes the device from the device manager.
        /// </summary>
        /// <param name="device">Device to remove.</param>
        /// <returns>True if device was removed successfully.  False if the device could not be removed at that time.</returns>
        public bool RemoveDevice(IDevice device)
        {
            /// 
            /// Make sure we have the reference
            /// 
            if (mlist_devices.Contains(device) == false)
                return false;

            mlist_devices.Remove(device);

            return true;
        }
        #endregion

        #region Shutdown and Initialization
        /// <summary>
        /// Calls the shutdown method for each device.
        /// </summary>
        /// <returns>True if shutdown successful.  False if shutdown failed.</returns>
        public bool ShutdownDevices()
        {
            bool worked = true;
            foreach (IDevice device in mlist_devices)
            {
                worked = (worked && device.Shutdown());
            }
            return worked;
        }
        /// <summary>
        /// Initializes all the devices if they have not been initialized already.
        /// </summary>
        /// <returns>True if initialization was successful or not.</returns>
        public bool InitializeDevices()
        {
            bool worked = true;
            foreach (IDevice device in mlist_devices)
            {
                worked = (worked && device.Initialize());
            }
            return worked;
        }
        #endregion

        #region Static Property
        /// <summary>
        /// Gets or sets the static device manager reference.
        /// </summary>
        public static classDeviceManager Manager
        {
            get
            {
                return mobj_deviceManager;
            }
            set
            {
                mobj_deviceManager = value;
            }
        }
        #endregion
    }
}
