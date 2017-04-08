using System;
using System.Collections.Generic;
using System.Threading;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Method;

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
            Name = "Blocker";
            Version = "Version 1.0";
            m_status = enumDeviceStatus.NotInitialized;
        }

        /// <summary>
        /// Fired when the status changes.
        /// </summary>
        public event EventHandler<classDeviceStatusEventArgs> StatusUpdate;

        /// <summary>
        /// Fired when an error occurs in the device.
        /// </summary>
        /// <remarks>This event is required by IDevice but this class does not use it</remarks>
        public event EventHandler<classDeviceErrorEventArgs> Error
        {
            add { }
            remove { }
        }

        /// <summary>
        /// Fired when a property changes in the device.
        /// </summary>
        /// <remarks>This event is required by IDevice but this class does not use it</remarks>
        public event EventHandler DeviceSaveRequired
        {
            add { }
            remove { }
        }

        #region LC-Method Registered Methods

        [classLCMethod("Block", enumMethodOperationTime.Parameter, "", -1, false)]
        public void Block(double timeToBlock)
        {
            var timer = new classTimerDevice {
                AbortEvent = AbortEvent
            };
            timer.WaitSeconds(timeToBlock);
        }

        #endregion

        /// <summary>
        /// Returns the name of this device.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        #region Members

        /// <summary>
        /// Status of the device currently.
        /// </summary>
        private enumDeviceStatus m_status;

        #endregion

        #region IDevice Members

        /// <summary>
        /// Gets or sets the abort event for scheduling.
        /// </summary>
        public ManualResetEvent AbortEvent { get; set; }

        /// <summary>
        /// Gets or sets whether the device is emulated or not.
        /// </summary>
        public bool Emulation { get; set; }

        /// <summary>
        /// Name of this device.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Version of this device.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the status of this device.
        /// </summary>
        public enumDeviceStatus Status
        {
            get { return m_status; }
            set
            {
                if (value != m_status)
                    StatusUpdate?.Invoke(this, new classDeviceStatusEventArgs(value, "None", this));
                m_status = value;
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

        public enumDeviceType DeviceType => enumDeviceType.BuiltIn;

        #endregion

        /*public Finch.Data.FinchComponentData GetData()
        {
            return null;
        }*/
    }
}