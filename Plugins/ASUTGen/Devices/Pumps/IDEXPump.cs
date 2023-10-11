using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Method;
using LcmsNetSDK.Logging;

namespace ASUTGen.Devices.Pumps
{
    [DeviceControl(typeof(IDEXPumpViewModel),
                                 "IDEX Pump",
                                 "Pumps")
    ]
    public class IDEXPump :  IDevice
    {
        public event EventHandler<DeviceStatusEventArgs> StatusUpdate;
        public event EventHandler<DeviceErrorEventArgs> Error;
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
        public DeviceStatus Status
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
        public DeviceErrorStatus ErrorType
        {
            get;
            set;
        }
        /// <summary>
        /// Gets what type of device it is.
        /// </summary>
        public DeviceType DeviceType
        {
            get { return DeviceType.Component; }
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
        //public MonitoringComponent GetHealthData()
        //{
        //    return null;
        //}
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
        [LCMethodEventAttribute("Set Flow Rate", MethodOperationTimeoutType.Parameter)]
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

        //public Finch.Data.FinchComponentData GetData()
        //{
        //    return null;
        //}

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
