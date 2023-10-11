using System;
using System.Collections.Generic;
using System.ComponentModel;
using LcmsNetData;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Method;

namespace LcmsNetPlugins.IDEX.Valves
{
    /*[DeviceControlAttribute(typeof(IDEXValveControlViewModel),
                                 typeof(IDEXValveGlyph),
                                 "IDEX Valve",
                                 "Valves")
    ]*/
    public class IDEXValve :  IDevice, INotifyPropertyChangedExt
    {
#pragma warning disable CS0067
        public event EventHandler<DeviceStatusEventArgs> StatusUpdate;
        public event EventHandler<DeviceErrorEventArgs> Error;
        public event EventHandler DeviceSaveRequired;
#pragma warning restore CS0067

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
            get => name;
            set => this.RaiseAndSetIfChanged(ref name, value);
        }

        public string Version { get; set; }

        public DeviceStatus Status { get; set; }

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

        public bool Initialize(ref string errorMessage)
        {
            return true;
        }
        public bool Shutdown()
        {
            return true;
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
        [LCMethodEvent("Change Position", MethodOperationTimeoutType.Parameter)]
        public bool ChangePosition(double timeout, int position)
        {
            return true;
        }
        [LCMethodEvent("Home Valve", MethodOperationTimeoutType.Parameter)]
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
