using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Method;
using LcmsNetDataClasses.Logging;

namespace ASUTGen.Devices.Pumps
{
    [classDeviceControlAttribute(typeof(IDEXPumpControl),
                                 "IDEX Pump",
                                 "Pumps")
    ]
    public class IDEXPump :  IDevice
    {
        public event EventHandler<classDeviceStatusEventArgs> StatusUpdate;
        public event EventHandler<classDeviceErrorEventArgs> Error;
        public event EventHandler DeviceSaveRequired;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public IDEXPump()
        {
            Name = "IDEX Pumps";
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
        public enumDeviceStatus Status
        {
            get;
            set;
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
        public bool Initialize(ref string errorMessage)
        {
            return true;
        }
        public bool Shutdown()
        {
            return true;
        }
        public void RegiserDataProvider(string key, DelegateDeviceHasData remoteMethod)
        {
            
        }
        public void UnRegiserDataProvider(string key, DelegateDeviceHasData remoteMethod)
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
            return new List<string>() { "Inject Status", "Method Status" };
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
        [classLCMethodAttribute("Set Flow Rate", enumMethodOperationTime.Parameter, "", -1, false)]
        public bool SetFlowRate(double timeout, double flowRate)
        {
            
            return true;
        }
        /// <summary>
        /// Returns the name of the pump.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        #region IFinchComponent Members

        public Finch.Data.FinchComponentData GetData()
        {
            return null;
        }

        #endregion
    }
}
