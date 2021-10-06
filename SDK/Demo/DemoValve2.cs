using System;
using System.Collections.Generic;
using System.ComponentModel;
using FluidicsSDK.Devices.Valves;
using LcmsNetSDK;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Method;

namespace DemoPluginLibrary
{
    public enum EightPositionState
    {
        [Description("1")] P1 = 1,
        [Description("2")] P2 = 2,
        [Description("3")] P3 = 3,
        [Description("4")] P4 = 4,
        [Description("5")] P5 = 5,
        [Description("6")] P6 = 6,
        [Description("7")] P7 = 7,
        [Description("8")] P8 = 8,
        [Description("Unknown")] Unknown = -1
    };

    //TODO: Add a custom user control for this guy....maybe?
    [DeviceControl(typeof(DemoValve2AdvancedControlViewModel),
                                    "Demo Valve - Multiposition",
                                    "Demo")]
    public class DemoValve2 : IDevice, IMultiPositionValve
    {
        #region Methods
        public DemoValve2()
        {
            Name = "Demo Valve";
            Version = "infinity.";
            Position = 1;
            NumberOfPositions = 8;
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

        [LCMethodEvent("GetPosition", 1.0, "", -1, false)]
        public int GetPosition()
        {
            return Position;
        }

        public void SetPosition(int s)
        {
            if (1 <= s && s <= 8)
            {
                SetPosition((EightPositionState) s);
            }
        }

        [LCMethodEvent("SetPosition", 1.0, "", -1, false)]
        public void SetPosition(EightPositionState position)
        {
            if ((int)position < 1 || (int)position > 8)
            {
                throw new Exception("The position is invalid.");
            }
            Position = (int)position;
            PosChanged?.Invoke(this, new ValvePositionEventArgs<int>((int)position));
        }

        #endregion

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

        public event EventHandler<ValvePositionEventArgs<int>> PosChanged;

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

        public int NumberOfPositions { get; }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
