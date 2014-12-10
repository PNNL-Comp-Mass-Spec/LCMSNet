﻿using System;
using System.Collections.Generic;
using System.IO.Ports;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Method;
using FluidicsSDK.Devices;
using System.Timers;
using FluidicsSDK.Base;

namespace DemoPluginLibrary
{
    //TODO: Add a custom user control for this guy....maybe?                                 
    [classDeviceControlAttribute(typeof(DemoValve2AdvancedControl),
                                    "Demo Valve - Multipostion",
                                    "Demo")]
    public class DemoValve2 : IDevice, INinePortValve
    {
        #region Methods
        public DemoValve2()
        {
            Name = "Demo Valve";
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

        [classLCMethodAttribute("GetPosition", 1.0, "", -1, false)]
        public int GetPosition()
        {
            return Position;
        }

        [classLCMethodAttribute("SetPosition", 1.0, "", -1, false)]
        public void SetPosition(EightPositionState position)
        {
            if ((int)position < 1 || (int)position > 8)
            {
                throw new Exception("The position is invalid.");
            }
            Position = (int)position;
            if (PosChanged != null)
            {
                PosChanged(this, new ValvePositionEventArgs<int>((int)position));
            }
            
        }

        #endregion

        #region Events
        public event EventHandler<classDeviceStatusEventArgs> StatusUpdate;

        public event EventHandler<classDeviceErrorEventArgs> Error;

        public event EventHandler DeviceSaveRequired;

        public event EventHandler<ValvePositionEventArgs<int>> PosChanged;
        #endregion

        #region Properties
        public enumDeviceType DeviceType
        {
            get { return enumDeviceType.Component; }
        }

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