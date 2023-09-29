using System;
using System.Collections.Generic;
using System.ComponentModel;
using FluidicsSDK.Base;
using FluidicsSDK.Devices.Valves;
using LcmsNetSDK;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Method;

namespace DemoPluginLibrary
{
    [DeviceControl(typeof(DemoValveAdvancedControlViewModel),
                                 "Demo Valve - Two Position",
                                 "Demo")]
    public class DemoValve : IDevice, IFourPortValve
    {
        public DemoValve()
        {
            Name = "Demo Valve";
            Position = 1;
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

        [LCMethodEvent("GetPosition", 1.0)]
        public int GetPosition()
        {
            return Position;
        }

        [LCMethodEvent("SetPosition", 1.0)]
        public void SetPosition(TwoPositionState position)
        {
            if ((int)position < 0 || (int)position > 1)
            {
                throw new Exception("The position is invalid.");
            }
            Position = (int)position;
            PositionChanged?.Invoke(this, new ValvePositionEventArgs<TwoPositionState>(position));
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

        public event EventHandler<ValvePositionEventArgs<TwoPositionState>> PositionChanged;

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

        public int Position { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
