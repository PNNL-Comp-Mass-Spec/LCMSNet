using System;
using System.Collections.Generic;
using System.ComponentModel;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Method;
using LcmsNetSDK;

namespace ASUTGen.Devices.Valves
{
    /*[classDeviceControlAttribute(typeof(IDEXValveControl),
                                 typeof(IDEXValveGlyph),
                                 "IDEX Valve",
                                 "Valves")
    ]*/
    public class IDEXValve :  IDevice
    {
        public event EventHandler<classDeviceStatusEventArgs> StatusUpdate;
        public event EventHandler<classDeviceErrorEventArgs> Error;
        public event EventHandler DeviceSaveRequired;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public IDEXValve()
        {
            Name = "IDEX Valve";
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
        public enumDeviceType DeviceType => enumDeviceType.Component;

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
        [classLCMethodAttribute("Change Position", enumMethodOperationTime.Parameter, "", -1, false)]
        public bool ChangePosition(double timeout, int position)
        {

            return true;
        }
        [classLCMethodAttribute("Home Valve", enumMethodOperationTime.Parameter, "", -1, false)]
        public bool HomeValve(double timeout, int position)
        {

            return true;
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
