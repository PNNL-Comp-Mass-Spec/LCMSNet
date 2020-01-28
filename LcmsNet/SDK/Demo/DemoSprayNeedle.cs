using System;
using System.Collections.Generic;
using System.ComponentModel;
using FluidicsPack;
using FluidicsSDK.Devices;
using FluidicsSDK.Base;
using FluidicsSDK.Devices.Valves;
using LcmsNetData;
using LcmsNetSDK.Devices;

namespace DemoPluginLibrary
{
    [DeviceControl(null,
                                 typeof(FluidicsSprayNeedle),
                                 "Demo Spray Needle",
                                 "Demo")]
    public class DemoSprayNeedle : IDevice
    {
        public DemoSprayNeedle()
        {
            Name = "Demo Spray Needle";
            Version = "infinity.";
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

        #region Events
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

#pragma warning disable 67
        public event EventHandler<ValvePositionEventArgs<TwoPositionState>> PositionChanged
        {
            add { }
            remove { }
        }
#pragma warning restore 67


        #endregion

        #region Properties
        public DeviceType DeviceType => DeviceType.Component;

        public DeviceErrorStatus ErrorType { get; set; }

        private string name;
        public string Name
        {
            get => name;
            set => this.RaiseAndSetIfChanged(ref name, value);
        }

        public string Version { get; set; }

        public DeviceStatus Status { get; set; }

        public System.Threading.ManualResetEvent AbortEvent { get; set; }

        public bool Emulation { get; set; }

        public int Position { get; set; }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
