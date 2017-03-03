using System;
using System.Collections.Generic;
using LcmsNetDataClasses.Devices;

namespace FluidicsPack
{
    public abstract class FluidicsComponentBase : IDevice
    {


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

        public event EventHandler<classDeviceStatusEventArgs> StatusUpdate;

        public event EventHandler<classDeviceErrorEventArgs> Error;

        public event EventHandler DeviceSaveRequired;

        public enumDeviceErrorStatus ErrorType
        {
            get;
            set;
        }

        public enumDeviceType DeviceType => enumDeviceType.Fluidics;

        public bool Emulation
        {
            get;
            set;
        }
        #endregion

    }
}