/*********************************************************************************************************
 * Written by Brian LaMarche and Christopher Walters for U.S. Department of Energy
 * Pacific Northwest National Laboratory, Richland, WA
 * Copyright 2013 Battle Memorial Institute
 * Created 1/3/2013
 * 
 * Last Modified 1/3/2013 By Christopher Walters 
 ********************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluidicsSDK.Devices;
using LcmsNetDataClasses.Devices;

namespace FluidicsPack
{
     [classDeviceControlAttribute(null,
                                  typeof(FluidicsUnion),
                                 "Union",
                                 "Fluidics Components")]
    public class Union:IDevice
    {
        #region Methods
         public Union()
         {
             Name = "Union";
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

        #endregion

        #region Events
        public event EventHandler<classDeviceStatusEventArgs> StatusUpdate;

        public event EventHandler<classDeviceErrorEventArgs> Error;

        public event EventHandler DeviceSaveRequired;

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
        #endregion
    }     
}
