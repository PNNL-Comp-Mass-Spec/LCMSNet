using System;
using System.Collections.Generic;
using System.ComponentModel;
using LcmsNetSDK;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Logging;
using LcmsNetSDK.Method;

namespace LcmsNetPlugins.FailureInjector.Drivers
{
    /*[DeviceControlAttribute(typeof(NotificationDriverViewModel),
                                 typeof(controlNotificationDriverGlyph),
                                 "Failure Injector",
                                 "Testing")
    ]*/
    public class NotificationDriver :  IDevice
    {
        public event EventHandler<DeviceStatusEventArgs> StatusUpdate;
        public event EventHandler<DeviceErrorEventArgs> Error;
#pragma warning disable CS0067
        public event EventHandler DeviceSaveRequired;
#pragma warning restore CS0067

        /// <summary>
        /// Default constructor.
        /// </summary>
        public NotificationDriver()
        {
            Name = "Failure Injector";
        }

        #region IDevice Members

        private string name;
        public string Name
        {
            get => name;
            set => this.RaiseAndSetIfChanged(ref name, value);
        }

        public string Version { get; set; }
        private DeviceStatus m_status;
        public DeviceStatus Status
        {
            get => m_status;
            set
            {
                m_status = value;
                StatusUpdate?.Invoke(this, new DeviceStatusEventArgs(m_status, "Status", this));
            }
        }

        public System.Threading.ManualResetEvent AbortEvent { get; set; }

        /// <summary>
        /// Gets the error type.
        /// </summary>
        public DeviceErrorStatus ErrorType { get; set; }

        /// <summary>
        /// Gets what type of device it is.
        /// </summary>
        public DeviceType DeviceType => DeviceType.Component;

        /// <summary>
        /// Gets or sets whether the device is in emulation mode or not.
        /// </summary>
        public bool Emulation { get; set; }

        /// <summary>
        /// Initializes the failure injector.
        /// </summary>
        /// <param name="errorMessage">Error message if found.</param>
        /// <returns>True if initialized, false if not.</returns>
        public bool Initialize(ref string errorMessage)
        {
            Status = DeviceStatus.Initialized;
            StatusUpdate?.Invoke(this, new DeviceStatusEventArgs(this.Status, "Initialized", this));
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
        public void WritePerformanceData(string directoryPath, string methodName, object[] parameters)
        {
        }
        public MonitoringComponent GetHealthData()
        {
            return null;
        }
        public List<string> GetStatusNotificationList()
        {
            return new List<string>() { "Inject Status", "Method Status", "Initialized", "Status", "Number Change"};
        }
        public List<string> GetErrorNotificationList()
        {
            return new List<string>() { "Inject Failure", "Method Failure" };
        }
        #endregion

        /// <summary>
        /// Injects a failure into the system.
        /// </summary>
        /// <returns></returns>
        [LCMethodEvent("Inject Failure", MethodOperationTimeoutType.Parameter, "", -1, false)]
        public bool MethodFailure(double timeout)
        {
            if (this.Error != null)
            {
                var args = new DeviceErrorEventArgs(
                    "Method Failure",
                    null,
                    DeviceErrorStatus.ErrorAffectsAllColumns,
                    this,
                    "Method Failure");

                this.Error(this, args);
            }
            return false;
        }

        /// <summary>
        /// Injects a failure into the system.
        /// </summary>
        /// <returns></returns>
        [LCMethodEvent("Send Number", MethodOperationTimeoutType.Parameter, "", -1, false)]
        public bool SendNumber(double timeout, double number)
        {
            if (StatusUpdate != null)
            {
                var args = new DeviceStatusEventArgs(
                    DeviceStatus.InUseByMethod,
                    "Number Change",
                    this,
                    number.ToString());

                this.StatusUpdate(this, args);
            }
            return true;
        }
        /// <summary>
        /// Injects a failure into the system.
        /// </summary>
        /// <returns></returns>
        public bool InjectFailure()
        {
            if (this.Error != null)
            {
                Status = DeviceStatus.Error;

                var args = new DeviceErrorEventArgs(
                    "Inject Failure",
                    null,
                    DeviceErrorStatus.ErrorAffectsAllColumns,
                    this,
                    "Inject Failure");

                this.Error(this, args);
            }
            return false;
        }
        public bool InjectStatus(string status)
        {
            ApplicationLogger.LogMessage(1, status);
            return true;
        }

        /// <summary>
        /// Injects a failure into the system.
        /// </summary>
        /// <returns></returns>
        [LCMethodEvent("Inject Status", MethodOperationTimeoutType.Parameter, "", -1, false)]
        public bool MethodStatus(double timeout)
        {
            if (this.StatusUpdate != null)
            {
                var args = new DeviceStatusEventArgs(
                    DeviceStatus.InUseByMethod,
                    "Method Status",
                    this);

                this.StatusUpdate(this, args);
            }
            return true;
        }
        /// <summary>
        /// Injects a failure into the system.
        /// </summary>
        /// <returns></returns>
        public bool InjectStatus()
        {
            if (this.StatusUpdate != null)
            {
                var args = new DeviceStatusEventArgs(
                    DeviceStatus.InUseByMethod,
                    "Inject Status",
                    this);

                this.StatusUpdate(this, args);
            }
            return false;
        }

        public override string ToString()
        {
            return Name;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
