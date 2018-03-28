using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using LcmsNetSDK;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Method;

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
            m_status = DeviceStatus.NotInitialized;
        }

        /// <summary>
        /// Fired when the status changes.
        /// </summary>
        public event EventHandler<DeviceStatusEventArgs> StatusUpdate;

        /// <summary>
        /// Fired when an error occurs in the device.
        /// </summary>
        /// <remarks>This event is required by IDevice but this class does not use it</remarks>
        public event EventHandler<DeviceErrorEventArgs> Error
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

        [LCMethodEvent("Block", MethodOperationTimeoutType.Parameter, "", -1, false)]
        public void Block(double timeToBlock)
        {
            var timer = new TimerDevice {
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
        private DeviceStatus m_status;

        private string name;

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
        public string Name
        {
            get { return name; }
            set { this.RaiseAndSetIfChanged(ref name, value); }
        }

        /// <summary>
        /// Version of this device.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the status of this device.
        /// </summary>
        public DeviceStatus Status
        {
            get { return m_status; }
            set
            {
                if (value != m_status)
                    StatusUpdate?.Invoke(this, new DeviceStatusEventArgs(value, "None", this));
                m_status = value;
            }
        }

        public bool Initialize(ref string errorMessage)
        {
            Status = DeviceStatus.Initialized;
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

        public DeviceErrorStatus ErrorType { get; set; }

        public DeviceType DeviceType => DeviceType.BuiltIn;

        #endregion

        /*public Finch.Data.FinchComponentData GetData()
        {
            return null;
        }*/
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}