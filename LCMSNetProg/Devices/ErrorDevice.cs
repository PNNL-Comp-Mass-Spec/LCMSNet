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
    /// Error testing class.
    /// </summary>
    //[classDeviceMonitoring(enumDeviceMonitoringType.Message, "")]
    public class ErrorDevice : IDevice
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ErrorDevice()
        {
            Name = "ErrorDevice";
            m_status = DeviceStatus.NotInitialized;
        }

        /// <summary>
        /// Fired when the status changes.
        /// </summary>
        public event EventHandler<DeviceStatusEventArgs> StatusUpdate;

        /// <summary>
        /// Fired when an error occurs in the device.
        /// </summary>
        public event EventHandler<DeviceErrorEventArgs> Error;

        /// <summary>
        /// Fired when a property changes in the device.
        /// </summary>
        /// <remarks>This event is required by IDevice but this class does not use it</remarks>
        public event EventHandler DeviceSaveRequired
        {
            add { }
            remove { }
        }

        public void WritePerformanceData(string directoryPath, string name, object[] parameters)
        {
        }

        /// <summary>
        /// Returns the name of this device.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Status of the device currently.
        /// </summary>
        private DeviceStatus m_status;

        private string name;

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
            get => name;
            set => this.RaiseAndSetIfChanged(ref name, value);
        }

        /// <summary>
        /// Gets or sets the status of this device.
        /// </summary>
        public DeviceStatus Status
        {
            get => m_status;
            set
            {
                if (value != m_status)
                    StatusUpdate?.Invoke(this, new DeviceStatusEventArgs(value, "None", this));
                m_status = value;
            }
        }

        public bool Initialize(ref string errorMessage)
        {
            return true;
        }

        public bool Shutdown()
        {
            return true;
        }

        public List<string> GetStatusNotificationList()
        {
            return new List<string>();
        }

        public List<string> GetErrorNotificationList()
        {
            var errors = new List<string> {
                "Error!"
            };
            return errors;
        }

        /// <summary>
        /// Throws an exception.
        /// </summary>
        [LCMethodEvent("Throw Exception", 1)]
        public bool ThrowException()
        {
            var dummyCode = false;
            if (!dummyCode)
                throw new Exception("This exception was thrown on purpose.");
            return false;
        }

        [LCMethodEvent("Return Error", 1)]
        public bool ReturnError()
        {
            return false;
        }

        [LCMethodEvent("Event Error", 1)]
        public void EventError()
        {
            Error?.Invoke(this,
                          new DeviceErrorEventArgs("Error!", null, DeviceErrorStatus.ErrorSampleOnly, this, "Error!"));
        }

        /// <summary>
        /// This method times out, waiting indefinitely.
        /// </summary>
        /// <returns>False</returns>
        [LCMethodEvent("Timeout", 1)]
        public bool Timeout()
        {
            Thread.Sleep(4000);
            return true;
        }

        /// <summary>
        /// This method times out, waiting indefinitely.
        /// </summary>
        /// <returns>False</returns>
        [LCMethodEvent("Wait Full Timeout", 1)]
        public bool WaitUntilTimeout()
        {
            Thread.Sleep(1000);
            return true;
        }

        public DeviceErrorStatus ErrorType { get; set; }

        public DeviceType DeviceType => DeviceType.BuiltIn;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
