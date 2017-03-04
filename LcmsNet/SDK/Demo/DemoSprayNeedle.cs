/*********************************************************************************************************
 * Written by Brian LaMarche and Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2013 Battle Memorial Institute
 * Created 12/31/2013
 *
 ********************************************************************************************************/
using System;
using System.Collections.Generic;
using LcmsNetDataClasses.Devices;
using FluidicsSDK.Devices;
using FluidicsSDK.Base;

namespace DemoPluginLibrary
{
    [classDeviceControlAttribute(null,
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
        public event EventHandler<classDeviceStatusEventArgs> StatusUpdate
        {
            add { }
            remove { }
        }

        public event EventHandler<classDeviceErrorEventArgs> Error
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
        public enumDeviceType DeviceType => enumDeviceType.Component;

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

        public int Position { get; set; }

        #endregion
    }
}
