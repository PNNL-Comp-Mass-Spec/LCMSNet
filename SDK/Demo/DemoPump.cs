using System;
using System.Collections.Generic;
using System.ComponentModel;
using FluidicsSDK.Devices;
using LcmsNetSDK;
using LcmsNetSDK.Data;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Method;

namespace DemoPluginLibrary
{
    //TODO: Add a custom user control for this guy....maybe?
    [DeviceControl(null,
                                 "Demo Pump",
                                 "Demo")]
    public class DemoPump : IDevice, IPump, IFluidicsPump, IHasDataProvider, IHasPerformanceData
    {
        double m_flowrate;
        double m_pressure;
        double m_percentB;
        private string name;

        public DemoPump()
        {
            Name = "DemoPump";
            Flowrate = 1;
            Pressure = 1;
            PercentB = 1;
            AbortEvent = new System.Threading.ManualResetEvent(false);
            MobilePhases = new List<MobilePhase>
            {
                new MobilePhase("DemoPhase1", "A test mobile phase"),
                new MobilePhase("DemoPhase2", "A test mobile phase")
            };

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
            switch (key.ToUpper())
            {
                case "METHODNAMES":
                    MethodNames += remoteMethod;
                    ListMethods();
                    break;
            }
        }

        public void UnRegisterDataProvider(string key, DelegateDeviceHasData remoteMethod)
        {
            switch (key.ToUpper())
            {
                case "METHODNAMES":
                    MethodNames -= remoteMethod;
                    break;
            }
        }

        private void ListMethods()
        {
            if (MethodNames != null)
            {
                var data = new List<object> {
                    "ExampleMethod1",
                    "ExampleMethod2",
                    "ExampleMethod3" };

                MethodNames(this, data);
            }
        }

        public void WritePerformanceData(string directoryPath, string methodName, object[] parameters)
        {

        }

        public string GetPerformanceData(string methodName, object[] parameters)
        {
            return "";
        }

        public List<string> GetStatusNotificationList()
        {
            return new List<string>();
        }

        public List<string> GetErrorNotificationList()
        {
            return new List<string>();
        }

        public event DelegateDeviceHasData MethodNames;

        [LCMethodEvent("Start Method", MethodOperationTimeoutType.Parameter, "MethodNames", 2, false)]
        public bool StartMethod(double timeout, double channel, string methodName)
        {
            return true;
        }

        [LCMethodEvent("Stop Method", 1.0)]
        public bool StopMethod()
        {
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

        public event EventHandler<PumpEventArgs<double>> FlowChanged;

        public event EventHandler<PumpEventArgs<double>> PressureChanged;

        public event EventHandler<PumpEventArgs<double>> PercentBChanged;

        /// <summary>
        /// Event fired when new pump data is available.
        /// </summary>
        public event EventHandler<PumpDataEventArgs> MonitoringDataReceived
        {
            add { }
            remove { }
        }

        public string Name
        {
            get => name;
            set => this.RaiseAndSetIfChanged(ref name, value);
        }

        public DeviceStatus Status { get; set; }

        public System.Threading.ManualResetEvent AbortEvent { get; set; }

        public DeviceErrorStatus ErrorType { get; set; }

        public DeviceType DeviceType => DeviceType.Component;

        public bool Emulation { get; set; }


        public double GetFlowRate()
        {
            return Flowrate;
        }

        public double GetPressure()
        {
            return Pressure;
        }

        public double GetPercentB()
        {
            return PercentB;
        }

        /// <summary>
        /// property representing the current flowrate of fluid through the pump
        /// </summary>
        public double Flowrate {
            get => m_flowrate;
            set
            {
                m_flowrate = value;
                FlowChanged?.Invoke(this, new PumpEventArgs<double>(Flowrate));
            }

        }


        public double GetActualFlow()
        {
            return 0;
        }

        public double GetMixerVolume()
        {
            return 0;
        }

        /// <summary>
        /// property representing the current Pressure exerted on/by the pump.
        /// </summary>
        public double Pressure
        {
            get => m_pressure;
            set
            {
                m_pressure = value;
                PressureChanged?.Invoke(this, new PumpEventArgs<double>(Pressure));
            }
        }

        /// <summary>
        /// Property representing the PercentB for the pump.
        /// </summary>
        public double PercentB
        {
            get => m_percentB;
            set
            {
                m_percentB = value;
                PercentBChanged?.Invoke(this, new PumpEventArgs<double>(PercentB));
            }
        }

        /// <summary>
        /// Gets or sets the list of mobile phases associated with the pump.
        /// </summary>
        public List<MobilePhase> MobilePhases { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
