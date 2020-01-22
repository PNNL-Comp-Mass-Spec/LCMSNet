using System;
using System.Collections.Generic;
using System.ComponentModel;
using FluidicsSDK.Devices;
using LcmsNetData;
using LcmsNetSDK.Data;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Method;

namespace DemoPluginLibrary
{
    [Serializable]
    [DeviceControl(typeof(DemoPALAdvancedControlViewModel),
                                 "Demo PAL",
                                 "Demo")
    ]
    public class DemoPAL: IDevice, IFluidicsSampler
    {

        /// <summary>
        /// Fired when new method names are available.
        /// </summary>
#pragma warning disable 67
        public event EventHandler<AutoSampleEventArgs> MethodNames
        {
            add { }
            remove { }
        }
#pragma warning restore 67

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
            Status = DeviceStatus.Initialized;
            ErrorType = DeviceErrorStatus.NoError;
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
                var data = new List<object> {
                    "ExampleMethod1",
                    "ExampleMethod2",
                    "ExampleMethod3"
                };
                Methods(this, data);
            }
        }

        public void WritePerformanceData(string directoryPath, string methodName, object[] parameters)
        {

        }

        [LCMethodEvent("Run Method", MethodOperationTimeoutType.Parameter, true, 1, "MethodNames", 2, false)]
        public void RunMethod(double timeout, SampleData sample, string method)
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
        public event EventHandler<DeviceStatusEventArgs> StatusUpdate
        {
            add { }
            remove { }
        }

        public event EventHandler<DeviceErrorEventArgs> Error
        {
            add { }
            remove { }
        }

        public event EventHandler DeviceSaveRequired
        {
            add { }
            remove { }
        }

        public DeviceType DeviceType => DeviceType.Component;

        public DeviceErrorStatus ErrorType
        {
            get;
            set;
        }

        private string name;
        public string Name
        {
            get => name;
            set => this.RaiseAndSetIfChanged(ref name, value);
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

        public bool Emulation
        {
            get;
            set;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
