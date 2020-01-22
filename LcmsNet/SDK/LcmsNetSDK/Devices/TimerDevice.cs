﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using LcmsNetData;
using LcmsNetSDK.Method;

//using LcmsNet.Devices;

namespace LcmsNetSDK.Devices
{
    /// <summary>
    /// Class that handles low level timing.  Has one method to wait!
    /// </summary>
    public class TimerDevice : IDevice
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public TimerDevice()
        {
            m_name = "Timer";
            m_status = DeviceStatus.NotInitialized;
        }

        /// <summary>
        /// Fired when the status changes.
        /// </summary>
        //public event DelegateDeviceStatusUpdate StatusUpdate;
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

        /// <summary>
        /// Gets or sets the abort event for scheduling.
        /// </summary>
        public ManualResetEvent AbortEvent { get; set; }


        public void WritePerformanceData(string directoryPath, string name, object[] parameters)
        {
        }

        /// <summary>
        /// Returns the name of the device.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(m_name) ? "Undefined timer" : m_name;
        }

        #region Members

        /// <summary>
        /// Name of the device.
        /// </summary>
        private string m_name;

        /// <summary>
        /// Status of the device currently.
        /// </summary>
        private DeviceStatus m_status;

        #endregion

        #region LC-Method Registered Methods..

        /// <summary>
        /// Waits for N milliseconds.
        /// </summary>
        /// <param name="seconds">Total number of milliseconds to wait.</param>
        [LCMethodEvent("Wait N Seconds", MethodOperationTimeoutType.Parameter, "", -1, false)]
        // Note, the timeout is converted by the optimizer.
        public void WaitSeconds(double seconds)
        {
            if (AbortEvent != null)
            {
                WaitMilliseconds(Convert.ToInt32(seconds) * 1000, AbortEvent);
            }
            else
            {
                using (var ev = new ManualResetEvent(false))
                {
                    WaitMilliseconds(Convert.ToInt32(seconds) * 1000, ev);
                }
            }
        }

        /// <summary>
        /// Waits for N milliseconds blocking.
        /// </summary>
        /// <param name="milliSeconds">Total number of milliseconds to wait.</param>
        /// <param name="resetEvent">Event to wait on</param>
        public void WaitMilliseconds(int milliSeconds, ManualResetEvent resetEvent)
        {
            resetEvent.WaitOne(milliSeconds);
        }

        #endregion

        #region IDevice Members

        /// <summary>
        /// Gets or sets whether the device is emulated or not.
        /// </summary>
        public bool Emulation { get; set; }

        /// <summary>
        /// Gets or sets the name of the device.
        /// </summary>
        public string Name
        {
            get => m_name;
            set => this.RaiseAndSetIfChanged(ref m_name, value, nameof(Name));
        }

        /// <summary>
        /// Version of the timer.
        /// </summary>
        public string Version
        {
            get => string.Empty;
            set
            {
                // Pass
            }
        }

        /// <summary>
        /// Initializes the device.
        /// </summary>
        /// <returns></returns>
        public bool Initialize(ref string errorMessage)
        {
            //
            // No initialization required.
            //

            return true;
        }

        /// <summary>
        /// Shuts off the timer.
        /// </summary>
        /// <returns></returns>
        public bool Shutdown()
        {
            //TODO: Do we want to thread the timer?
            return true;
        }

        /// <summary>
        /// Status of the device
        /// </summary>
        public DeviceStatus Status
        {
            get => m_status;
            set
            {
                m_status = value;
                if (value != m_status)
                {
                    StatusUpdate?.Invoke(this, new DeviceStatusEventArgs(m_status, "", this));
                }
            }
        }

        #endregion

        #region IDevice Data Provider Methods

        public void RegisterDataProvider(string key, DelegateDeviceHasData remoteMethod)
        {
        }

        public void UnRegisterDataProvider(string key, DelegateDeviceHasData remoteMethod)
        {
        }

        #endregion

        #region IDevice Members

        public DeviceErrorStatus ErrorType { get; set; }

        public DeviceType DeviceType => DeviceType.Virtual;

        #endregion

        #region IDevice Members

        public List<string> GetStatusNotificationList()
        {
            return new List<string>();
        }

        public List<string> GetErrorNotificationList()
        {
            return new List<string>();
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}