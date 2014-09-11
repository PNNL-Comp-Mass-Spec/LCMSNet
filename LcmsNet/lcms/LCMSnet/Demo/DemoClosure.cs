using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LcmsNetDataClasses.Devices;
using FluidicsSDK.Devices;
using LcmsNetDataClasses.Method;

namespace DemoPluginLibrary
{
     [classDeviceControlAttribute(typeof(DemoClosureAdvanceControl),
                                 "Test Closure",
                                 "Test")]

    public class DemoClosure: IDevice, IFluidicsClosure
    {
        public DemoClosure()
        {
            Name = "Test Closure Name";
        }

        #region Methods
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

        /// <summary>
        /// Triggers a pulse of the specified voltage, lasting the specified duration.
        /// This is intended for use on the analog output ports--if it is a digital 
        /// port the specified voltage will be disregarded.
        /// </summary>
        /// <param name="pulseLengthMS">The length of the pulse in milliseconds</param>
        /// <param name="portName">The port to send the voltage on</param>
        /// <param name="voltage">The voltage to set</param>        
        [classLCMethodAttribute("Trigger With Voltage", enumMethodOperationTime.Parameter, "", -1, false)]
        public int Trigger(int pulseLengthSeconds, string portName, double voltage)
        {
            //interact with hardware here.
            return 0;
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

        public string GetClosureType()
        {
            return "Test Closure Type";
        }
    }
}
