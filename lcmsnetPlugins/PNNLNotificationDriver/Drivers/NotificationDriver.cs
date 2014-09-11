using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Method;
using LcmsNetDataClasses.Logging;

namespace FailureInjector.Drivers
{
    /*[classDeviceControlAttribute(typeof(controlNotificationDriver),
                                 typeof(controlNotificationDriverGlyph),
                                 "Failure Injector",
                                 "Testing")
    ]*/
    public class NotificationDriver :  IDevice
    {
        public event EventHandler<classDeviceStatusEventArgs> StatusUpdate;
        public event EventHandler<classDeviceErrorEventArgs> Error;
        public event EventHandler DeviceSaveRequired;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public NotificationDriver()
        {
            Name = "Failure Injector";
        }

        #region IDevice Members
        public string Name
        {
            get;
            set;
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
                if (StatusUpdate != null)
                {
                    StatusUpdate(this, new classDeviceStatusEventArgs(m_status, "Status", this));
                }
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
        public enumDeviceType DeviceType
        {
            get { return enumDeviceType.Component; }
        }
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
            if (StatusUpdate != null)
            {
                StatusUpdate(this, new classDeviceStatusEventArgs(this.Status, "Initialized", this));
            }
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
        [classLCMethodAttribute("Inject Failure", enumMethodOperationTime.Parameter, "", -1, false)]
        public bool MethodFailure(double timeout)
        {
            if (this.Error != null)
            {
                classDeviceErrorEventArgs args = new classDeviceErrorEventArgs(
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
        [classLCMethodAttribute("Send Number", enumMethodOperationTime.Parameter, "", -1, false)]
        public bool SendNumber(double timeout, double number)
        {
            if (StatusUpdate != null)
            {
                classDeviceStatusEventArgs args = new classDeviceStatusEventArgs(
                    enumDeviceStatus.InUseByMethod,
                    "Number Change",
                    number.ToString(),
                    this);                

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

                classDeviceErrorEventArgs args = new classDeviceErrorEventArgs(
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
        [classLCMethodAttribute("Inject Status", enumMethodOperationTime.Parameter, "", -1, false)]
        public bool MethodStatus(double timeout)
        {
            if (this.StatusUpdate != null)
            {
                classDeviceStatusEventArgs args = new classDeviceStatusEventArgs(
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
                classDeviceStatusEventArgs args = new classDeviceStatusEventArgs(
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
    }
}
