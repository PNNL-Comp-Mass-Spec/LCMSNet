using System;
using System.Collections.Generic;
using System.ComponentModel;
using LcmsNetSDK;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Logging;
using LcmsNetSDK.Method;

namespace FailureInjector.Drivers
{
    /*[classDeviceControlAttribute(typeof(NotificationDriverViewModel),
                                 typeof(controlNotificationDriverGlyph),
                                 "Failure Injector",
                                 "Testing")
    ]*/
    public class NotificationDriver :  IDevice
    {
        public event EventHandler<classDeviceStatusEventArgs> StatusUpdate;
        public event EventHandler<classDeviceErrorEventArgs> Error;
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
            get { return name; }
            set { this.RaiseAndSetIfChanged(ref name, value); }
        }

        public string Version
        {
            get;
            set;
        }
        private enumDeviceStatus m_status;
        public enumDeviceStatus Status
        {
            get
            {
                return m_status;
            }
            set
            {
                m_status = value;
                StatusUpdate?.Invoke(this, new classDeviceStatusEventArgs(m_status, "Status", this));
            }
        }
        public System.Threading.ManualResetEvent AbortEvent
        {
            get;
            set;
        }
        /// <summary>
        /// Gets the error type.
        /// </summary>
        public enumDeviceErrorStatus ErrorType
        {
            get;
            set;
        }
        /// <summary>
        /// Gets what type of device it is.
        /// </summary>
        public enumDeviceType DeviceType => enumDeviceType.Component;

        /// <summary>
        /// Gets or sets whether the device is in emulation mode or not.
        /// </summary>
        public bool Emulation
        {
            get;
            set;
        }
        /// <summary>
        /// Initializes the failure injector.
        /// </summary>
        /// <param name="errorMessage">Error message if found.</param>
        /// <returns>True if initialized, false if not.</returns>
        public bool Initialize(ref string errorMessage)
        {
            Status = enumDeviceStatus.Initialized;
            StatusUpdate?.Invoke(this, new classDeviceStatusEventArgs(this.Status, "Initialized", this));
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
        public classMonitoringComponent GetHealthData()
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
        [classLCMethod("Inject Failure", enumMethodOperationTime.Parameter, "", -1, false)]
        public bool MethodFailure(double timeout)
        {
            if (this.Error != null)
            {
                var args = new classDeviceErrorEventArgs(
                    "Method Failure",
                    null,
                    enumDeviceErrorStatus.ErrorAffectsAllColumns,
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
        [classLCMethod("Send Number", enumMethodOperationTime.Parameter, "", -1, false)]
        public bool SendNumber(double timeout, double number)
        {
            if (StatusUpdate != null)
            {
                var args = new classDeviceStatusEventArgs(
                    enumDeviceStatus.InUseByMethod,
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
                Status = enumDeviceStatus.Error;

                var args = new classDeviceErrorEventArgs(
                    "Inject Failure",
                    null,
                    enumDeviceErrorStatus.ErrorAffectsAllColumns,
                    this,
                    "Inject Failure");

                this.Error(this, args);
            }
            return false;
        }
        public bool InjectStatus(string status)
        {
            classApplicationLogger.LogMessage(1, status);
            return true;
        }

        /// <summary>
        /// Injects a failure into the system.
        /// </summary>
        /// <returns></returns>
        [classLCMethod("Inject Status", enumMethodOperationTime.Parameter, "", -1, false)]
        public bool MethodStatus(double timeout)
        {
            if (this.StatusUpdate != null)
            {
                var args = new classDeviceStatusEventArgs(
                    enumDeviceStatus.InUseByMethod,
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
                var args = new classDeviceStatusEventArgs(
                    enumDeviceStatus.InUseByMethod,
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
