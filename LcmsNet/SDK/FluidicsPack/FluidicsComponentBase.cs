using System;
using System.Collections.Generic;
using System.ComponentModel;
using LcmsNetSDK;
using LcmsNetSDK.Devices;

namespace FluidicsPack
{
    public abstract class FluidicsComponentBase : IDevice
    {
        private string name;
        public string Name
        {
            get => name;
            set => this.RaiseAndSetIfChanged(ref name, value);
        }

        public string Version { get; set; }

        public DeviceStatus Status { get; set; }

        public System.Threading.ManualResetEvent AbortEvent { get; set; }

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

        public List<string> GetStatusNotificationList()
        {
            return new List<string>();
        }

        public List<string> GetErrorNotificationList()
        {
            return new List<string>();
        }

#pragma warning disable 0067
        public event EventHandler<DeviceStatusEventArgs> StatusUpdate;

        public event EventHandler<DeviceErrorEventArgs> Error;

        public event EventHandler DeviceSaveRequired;
#pragma warning restore 0067

        public DeviceErrorStatus ErrorType { get; set; }

        public DeviceType DeviceType => DeviceType.Fluidics;

        public bool Emulation { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}