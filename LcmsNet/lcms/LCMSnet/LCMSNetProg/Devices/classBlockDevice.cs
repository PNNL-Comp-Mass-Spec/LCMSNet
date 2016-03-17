using System;
using System.Collections.Generic;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Method;
using LcmsNetDataClasses.Devices;
using LcmsNet.Method;
using LcmsNet.Devices;
using LcmsNet.SampleQueue.IO;

namespace LcmsNet.Devices
{
    /// <summary>
    ///
    /// </summary>
    public class classBlockDevice : IDevice
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public classBlockDevice()
        {
            mstring_name = "Blocker";
            mstring_version = "Version 1.0";
            menum_status = enumDeviceStatus.NotInitialized;
        }

        /// <summary>
        /// Fired when the status changes.
        /// </summary>
        public event EventHandler<classDeviceStatusEventArgs> StatusUpdate;

        /// <summary>
        /// Fired when an error occurs in the device.
        /// </summary>
        public event EventHandler<classDeviceErrorEventArgs> Error;

        /// <summary>
        /// Fired when a property changes in the device.
        /// </summary>
        public event EventHandler DeviceSaveRequired;

        #region LC-Method Registered Methods

        [classLCMethodAttribute("Block", enumMethodOperationTime.Parameter, "", -1, false)]
        public void Block(double timeToBlock)
        {
            classTimerDevice timer = new classTimerDevice();
            timer.AbortEvent = AbortEvent;
            timer.WaitSeconds(timeToBlock);
        }

        #endregion

        /// <summary>
        /// Returns the name of this device.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return mstring_name;
        }

        #region Members

        /// <summary>
        /// Name of this device.
        /// </summary>
        private string mstring_name;

        /// <summary>
        /// Version of this device.
        /// </summary>
        private string mstring_version;

        /// <summary>
        /// Status of the device currently.
        /// </summary>
        private enumDeviceStatus menum_status;

        #endregion

        #region IDevice Members

        /// <summary>
        /// Gets or sets the abort event for scheduling.
        /// </summary>
        public System.Threading.ManualResetEvent AbortEvent { get; set; }

        /// <summary>
        /// Gets or sets whether the device is emulated or not.
        /// </summary>
        public bool Emulation { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name
        {
            get { return mstring_name; }
            set { mstring_name = value; }
        }

        /// <summary>
        /// Gets or sets the version number.
        /// </summary>
        public string Version
        {
            get { return mstring_version; }
            set { mstring_version = value; }
        }

        /// <summary>
        /// Gets or sets the status of this device.
        /// </summary>
        public enumDeviceStatus Status
        {
            get { return menum_status; }
            set
            {
                if (value != menum_status && StatusUpdate != null)
                    StatusUpdate(this, new classDeviceStatusEventArgs(value, "None", this));
                menum_status = value;
            }
        }

        public bool Initialize(ref string errorMessage)
        {
            Status = enumDeviceStatus.Initialized;
            return true;
        }

        public bool Shutdown()
        {
            return true;
        }

        public void RegisterDataProvider(string key, DelegateDeviceHasData remoteMethod)
        {
        }

        public void UnRegisterDataProvider(string key, DelegateDeviceHasData remoteMethod)
        {
        }

        public void WritePerformanceData(string directoryPath, string name, object[] parameters)
        {
        }

        public List<string> GetStatusNotificationList()
        {
            return new List<string>();
        }

        public List<string> GetErrorNotificationList()
        {
            return new List<string>();
        }

        public enumDeviceErrorStatus ErrorType { get; set; }

        public enumDeviceType DeviceType
        {
            get { return enumDeviceType.BuiltIn; }
        }

        #endregion

        /*public Finch.Data.FinchComponentData GetData()
        {
            return null;
        }*/
    }
}