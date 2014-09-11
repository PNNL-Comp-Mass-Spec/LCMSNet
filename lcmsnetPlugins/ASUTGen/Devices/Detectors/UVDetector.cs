using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Method;
using LcmsNetDataClasses.Logging;

namespace ASUTGen.Devices.Detectors
{
    [classDeviceControlAttribute(typeof(UVDetectorControl),
                                 "UV Detector",
                                 "Detectors")
    ]
    public class UVDetector :  IDevice
    {
        public event EventHandler<classDeviceStatusEventArgs> StatusUpdate;
        public event EventHandler<classDeviceErrorEventArgs> Error;
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
        [classLCMethodAttribute("Detect Stuff", enumMethodOperationTime.Parameter, "", -1, false)]
        public bool DetectStuff(double timeout)
        {
            
            return true;
        }
        public override string ToString()
        {
            return Name;
        }

        #region IFinchComponent Members

        public Finch.Data.FinchAggregateData GetData()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IFinchComponent Members

        Finch.Data.FinchComponentData Finch.Data.IFinchComponent.GetData()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDevice Members

        string IDevice.Name
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        string IDevice.Version
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        enumDeviceStatus IDevice.Status
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        System.Threading.ManualResetEvent IDevice.AbortEvent
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        bool IDevice.Initialize(ref string errorMessage)
        {
            throw new NotImplementedException();
        }

        bool IDevice.Shutdown()
        {
            throw new NotImplementedException();
        }

        void IDevice.RegiserDataProvider(string key, DelegateDeviceHasData remoteMethod)
        {
            throw new NotImplementedException();
        }

        void IDevice.UnRegiserDataProvider(string key, DelegateDeviceHasData remoteMethod)
        {
            throw new NotImplementedException();
        }

        void IDevice.WritePerformanceData(string directoryPath, string methodName, object[] parameters)
        {
            throw new NotImplementedException();
        }


        event EventHandler<classDeviceStatusEventArgs> IDevice.StatusUpdate
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        event EventHandler<classDeviceErrorEventArgs> IDevice.Error
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        event EventHandler IDevice.DeviceSaveRequired
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        enumDeviceErrorStatus IDevice.ErrorType
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        enumDeviceType IDevice.DeviceType
        {
            get { throw new NotImplementedException(); }
        }

        bool IDevice.Emulation
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
