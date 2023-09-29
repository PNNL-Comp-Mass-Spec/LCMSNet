using System;
using System.Collections.Generic;
using System.ComponentModel;
using FluidicsSDK.Devices;
using LcmsNetSDK;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Method;

namespace DemoPluginLibrary
{
     [DeviceControl(typeof(DemoClosureAdvancedControlViewModel),
                                 "Demo Closure",
                                 "Demo")]

    public class DemoClosure: IDevice, IFluidicsClosure
    {
        public DemoClosure()
        {
            Name = "Demo Closure";
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

        }

        public void UnRegisterDataProvider(string key, DelegateDeviceHasData remoteMethod)
        {

        }

        public void WritePerformanceData(string directoryPath, string methodName, object[] parameters)
        {

        }

        public List<string> GetStatusNotificationList()
        {
            return new List<string>();
        }

        public List<string> GetErrorNotificationList()
        {
            return new List<string>();
        }

        /// <summary>
        /// Triggers a pulse of the specified voltage, lasting the specified duration.
        /// This is intended for use on the analog output ports--if it is a digital
        /// port the specified voltage will be disregarded.
        /// </summary>
        /// <param name="pulseLengthSeconds">The length of the pulse in seconds</param>
        /// <param name="portName">The port to send the voltage on</param>
        /// <param name="voltage">The voltage to set</param>
        [LCMethodEvent("Trigger With Voltage", MethodOperationTimeoutType.Parameter)]
        public bool Trigger(int pulseLengthSeconds, string portName, double voltage)
        {
            //interact with hardware here.
            return true;
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

        public DeviceErrorStatus ErrorType { get; set; }

        private string name;
        public string Name
        {
            get => name;
            set => this.RaiseAndSetIfChanged(ref name, value);
        }

        public DeviceStatus Status { get; set; }

        public System.Threading.ManualResetEvent AbortEvent { get; set; }

        public bool Emulation { get; set; }

        public string GetClosureType()
        {
            return "DemoClosure";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
