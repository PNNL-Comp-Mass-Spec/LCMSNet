using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Method;

namespace ASUTGen.Devices.Detectors
{
    [DeviceControl(typeof(UVDetectorViewModel),
                                 "UV Detector",
                                 "Detectors")
    ]
    public class UVDetector :  IDevice
    {
        public event EventHandler<DeviceStatusEventArgs> StatusUpdate;
        public event EventHandler<DeviceErrorEventArgs> Error;
        public event EventHandler DeviceSaveRequired;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public UVDetector()
        {
            Name = "UV Detector";
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
        [LCMethodEventAttribute("Detect Stuff", MethodOperationTimeoutType.Parameter)]
        public bool DetectStuff(double timeout)
        {
            return true;
        }
        public override string ToString()
        {
            return Name;
        }

        #region IFinchComponent Members

        //public Finch.Data.FinchAggregateData GetData()
        //{
        //    throw new NotImplementedException();
        //}

        //Finch.Data.FinchComponentData Finch.Data.IFinchComponent.GetData()
        //{
        //    throw new NotImplementedException();
        //}

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
