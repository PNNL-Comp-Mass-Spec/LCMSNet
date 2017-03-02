using System;
using System.Threading;

//using LcmsNet.Devices;
using LcmsNetDataClasses.Method;

namespace LcmsNetDataClasses.Devices
{
    /// <summary>
    /// Class that handles low level timing.  Has one method to wait!
    /// </summary>
    public class classTimerDevice : IDevice
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public classTimerDevice()
        {
            m_name = "Timer";
            menum_status = enumDeviceStatus.NotInitialized;
        }

        /// <summary>
        /// Fired when the status changes.
        /// </summary>
        //public event DelegateDeviceStatusUpdate StatusUpdate;
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

        /// <summary>
        /// Gets or sets the abort event for scheduling.
        /// </summary>
        public System.Threading.ManualResetEvent AbortEvent { get; set; }


        public void WritePerformanceData(string directoryPath, string name, object[] parameters)
        {
        }


        /// <summary>
        /// Returns the name of the device.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return m_name;
        }

        #region Members

        /// <summary>
        /// Name of the device.
        /// </summary>
        private string m_name;

        /// <summary>
        /// Status of the device currently.
        /// </summary>
        private enumDeviceStatus menum_status;

        #endregion

        #region LC-Method Registered Methods..

        /// <summary>
        /// Waits for N milliseconds.
        /// </summary>
        /// <param name="seconds">Total number of milliseconds to wait.</param> 
        [classLCMethodAttribute("Wait N Seconds", enumMethodOperationTime.Parameter, "", -1, false)]
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
        /// <param name="seconds">Total number of milliseconds to wait.</param>
        public void WaitMilliseconds(int milliSeconds, ManualResetEvent resetEvent)
        {
            var wait = resetEvent.WaitOne(milliSeconds);
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
            get { return m_name; }
            set { m_name = value; }
        }

        /// <summary>
        /// Version of the timer.
        /// </summary>
        public string Version
        {
            get { return string.Empty; }
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
        public enumDeviceStatus Status
        {
            get { return menum_status; }
            set
            {
                menum_status = value;
                if (value != menum_status && StatusUpdate != null)
                {
                    StatusUpdate(this, new classDeviceStatusEventArgs(menum_status, "", this));
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

        public enumDeviceErrorStatus ErrorType { get; set; }

        public enumDeviceType DeviceType
        {
            get { return enumDeviceType.Virtual; }
        }

        #endregion

        #region IDevice Members

        public System.Collections.Generic.List<string> GetStatusNotificationList()
        {
            return new System.Collections.Generic.List<string>();
        }

        public System.Collections.Generic.List<string> GetErrorNotificationList()
        {
            return new System.Collections.Generic.List<string>();
        }

        #endregion
    }
}