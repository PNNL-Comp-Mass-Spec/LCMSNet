using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LcmsNetSDK;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Method;
using FluidicsSDK.Devices;
using LcmsNetDataClasses;

namespace DemoPluginLibrary
{
    [Serializable]
    [classDeviceControlAttribute(typeof(DemoPALAdvancedControl),                                 
                                 "DemoPAL",
                                 "Demo")
    ]
    public class DemoPAL: IDevice, IFluidicsSampler
    {

        /// <summary>
        /// Fired when new method names are available.
        /// </summary>
        public event EventHandler<classAutoSampleEventArgs> MethodNames;
        /// <summary>
        /// Fired to the method editor handler with a List of method names 
        /// </summary>
        public event DelegateDeviceHasData Methods;

        public DemoPAL()
        {
            Name = "DemoPAL";
            Version = "infinity.";
            AbortEvent = new System.Threading.ManualResetEvent(false);
        }


         public bool Initialize(ref string errorMessage)
        {
            Status = enumDeviceStatus.Initialized;
            ErrorType = enumDeviceErrorStatus.NoError;
            return true;
        }

        public bool Shutdown()
        {
            return true;
        }

        public void RegisterDataProvider(string key, DelegateDeviceHasData remoteMethod)
        {
            switch (key.ToUpper())
            {
                case "METHODNAMES":
                    Methods += remoteMethod;
                    ListMethods();
                    break;
            }
        }

        public void UnRegisterDataProvider(string key, DelegateDeviceHasData remoteMethod)
        {
            switch (key.ToUpper())
            {
                case "METHODNAMES":
                    Methods -= remoteMethod;
                    break;
            }
        }

        private void ListMethods()
        {
            if (Methods != null)
            {
                List<object> data = new List<object>();
                data.Add("ExampleMethod1");
                data.Add("ExampleMethod2");
                data.Add("ExampleMethod3");
                Methods(this, data);
            }
        }

        public void WritePerformanceData(string directoryPath, string methodName, object[] parameters)
        {

        }

        [classLCMethodAttribute("Run Method", enumMethodOperationTime.Parameter, true, 1, "MethodNames", 2, false)]
        public void RunMethod(double timeout, classSampleData sample, string method)
        {
            // Interact with hardware here.
        }

        public List<string> GetStatusNotificationList()
        {
            return new List<string>();
        }

        public List<string> GetErrorNotificationList()
        {
            return new List<string>();
        }
        public event EventHandler<classDeviceStatusEventArgs> StatusUpdate;

        public event EventHandler<classDeviceErrorEventArgs> Error;

        public event EventHandler DeviceSaveRequired;

        public enumDeviceType DeviceType
        {
            get { return enumDeviceType.Component; }
        }

        public enumDeviceErrorStatus ErrorType
        {
            get;
            set;
        }

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

        public bool Emulation
        {
            get;
            set;
        }

    }
}
