/*********************************************************************************************************
// Written by John Ryan, Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 08/17/2009
//
// Updates:
// - 9/08/2009 (BLL) Added method editing attributes for the method editor to use for an event.
//   - Method:  Trigger(int timeout)
//
//********************************************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using FluidicsSDK.Devices;
using LcmsNetSDK;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Method;

namespace LcmsNetPlugins.LabJackU12.ContactClosure
{
    [Serializable]
    [DeviceControl(typeof(ContactClosureU12ViewModel),
                                 "Contact Closure U12",
                                 "Contact Closures")]
    public class ContactClosureU12 : IDevice, IContactClosure
    {
        #region Members
        /// <summary>
        /// The labjack used for signalling the pulse
        /// </summary>
        private readonly LabjackU12 labjackDevice;
        /// <summary>
        /// The length of the pulse to send on the port. Defaults to 1 second.
        /// </summary>
        private int pulseLength = 1;
        /// <summary>
        /// The voltage to apply on the port for the pulse duration. Defaults to 5V.
        /// </summary>
        private double pulseVoltage = 5;
        /// <summary>
        /// The 'normal'/default voltage for the labjack port. Defaults to 0V.
        /// </summary>
        private double normalVoltage = CONST_ANALOGLOW;
        /// <summary>
        /// The port on the labjack on which to apply the voltage.
        /// </summary>
        private LabjackU12OutputPorts labjackPort;
        /// <summary>
        /// The name, used in software for the symbol.
        /// </summary>
        private string deviceName;
        /// <summary>
        /// The version.
        /// </summary>
        private string deviceVersion;
        /// <summary>
        /// The current status of the Labjack.
        /// </summary>
        private DeviceStatus deviceStatus;
        /// <summary>
        /// Flag indicating if the device is in emulation mode.
        /// </summary>
        private bool inEmulationMode;

        private bool isInitialized = false;
        private const char CONST_ANALOGPREFIX = 'A';
        private const int CONST_ANALOGHIGH = 5;
        private const int CONST_DIGITALHIGH = 1;
        private const int CONST_ANALOGLOW = 0;
        private const int CONST_DIGITALLOW = 0;
        #endregion

        #region Events
        /// <summary>
        /// Fired when the status changes.
        /// </summary>
        //public event DelegateDeviceStatusUpdate StatusUpdate;
        public event EventHandler<DeviceStatusEventArgs> StatusUpdate;
        /// <summary>
        /// Fired when an error occurs in the device.
        /// </summary>
        public event EventHandler<DeviceErrorEventArgs> Error;
        /// <summary>
        /// Fired when a property changes in the device.
        /// </summary>
        public event EventHandler DeviceSaveRequired;
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor--no labjack assigned!
        /// </summary>
        public ContactClosureU12()
        {
            labjackDevice = new LabjackU12();
            labjackPort    = LabjackU12OutputPorts.AO1;
            deviceName = "Contact Closure";
        }

        /// <summary>
        /// Constructor which assigns a labjack
        /// </summary>
        /// <param name="lj">The labjack</param>
        public ContactClosureU12(LabjackU12 lj)
        {
            labjackDevice = lj;
            labjackPort    = LabjackU12OutputPorts.AO1;
            deviceName = "Contact Closure";
        }

        /// <summary>
        /// Constructor which assigns a port
        /// </summary>
        /// <param name="newPort">The port on the labjack to use for the pulse</param>
        public ContactClosureU12(LabjackU12OutputPorts newPort)
        {
            labjackDevice = new LabjackU12();
            labjackPort    = newPort;
            deviceName = "Contact Closure";
        }

        /// <summary>
        /// Constructor which assigns a labjack and a port
        /// </summary>
        /// <param name="lj">The labjack</param>
        /// <param name="newPort">The port on the labjack to use for the pulse</param>
        public ContactClosureU12(LabjackU12 lj, LabjackU12OutputPorts newPort)
        {
            labjackDevice = lj;
            labjackPort    = newPort;
            deviceName = "Contact Closure";
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the abort event for scheduling.
        /// </summary>
        public System.Threading.ManualResetEvent AbortEvent { get; set; }

        /// <summary>
        /// Gets or sets the emulation state of the device.
        /// </summary>
        //[PersistenceDataAttribute("Emulated")]
        public bool Emulation
        {
            get => inEmulationMode;
            set => inEmulationMode = value;
        }

        /// <summary>
        /// Gets or sets the current status of the device.
        /// </summary>
        public DeviceStatus Status
        {
            get => deviceStatus;
            set
            {
                if (value != deviceStatus)
                {
                    StatusUpdate?.Invoke(this, new DeviceStatusEventArgs(value, "Status", this));
                }

                deviceStatus = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the device in the fluidics designer.
        /// </summary>
        public string Name
        {
            get => deviceName;
            set
            {
                if (this.RaiseAndSetIfChangedRetBool(ref deviceName, value))
                {
                    OnDeviceSaveRequired();
                }
            }
        }

        /// <summary>
        /// Gets or sets the version of the Labjack/dll
        /// </summary>
        public string Version
        {
            get => deviceVersion;
            set
            {
                deviceVersion = value;
                OnDeviceSaveRequired();
            }
        }

        /// <summary>
        /// The length of the pulse to send on the port. Defaults to 1 second.
        /// </summary>
        [PersistenceData("PulseLengthSeconds")]
        public int PulseLength
        {
            get => pulseLength;
            set
            {
                pulseLength = value;
                OnDeviceSaveRequired();
            }
        }

        /// <summary>
        /// The voltage to apply on the port for the pulse duration. Defaults to 5V.
        /// </summary>
        [PersistenceData("PulseVoltage")]
        public double PulseVoltage
        {
            get => pulseVoltage;
            set
            {
                pulseVoltage = value;
                OnDeviceSaveRequired();
            }
        }

        /// <summary>
        /// The 'normal'/default voltage for the labjack port. Defaults to 0V.
        /// </summary>
        [PersistenceData("NormalVoltage")]
        public double NormalVoltage
        {
            get => normalVoltage;
            set
            {
                normalVoltage = value;
                OnDeviceSaveRequired();
                SetPortNormalVoltage();
            }
        }

        /// <summary>
        /// Gets or sets the port on the labjack used for the pulse. Defaults to AO0.
        /// </summary>
        [PersistenceData("Port")]
        public LabjackU12OutputPorts Port
        {
            get => labjackPort;
            set
            {
                labjackPort = value;
                OnDeviceSaveRequired();
                SetPortNormalVoltage();
            }
        }

        [PersistenceData("Labjack ID")]
        public int LabJackID
        {
            get => labjackDevice.LocalID;
            set => labjackDevice.LocalID = value;
        }

        #endregion

        #region Methods

        //Initialize/Shutdown don't really apply
        //Maybe confirm that we can communicate to the labjack? I don't know.
        public bool Initialize(ref string errorMessage)
        {
            if (Emulation)
            {
                return true;
            }

            //Get the version info
            labjackDevice.GetDriverVersion();
            labjackDevice.GetFirmwareVersion();

            //If either FirmwareVersion or DriverVersion is zero, it means we have a communication failure.
            if (labjackDevice.FirmwareVersion > 0 &&
                labjackDevice.DriverVersion > 0)
            {
                isInitialized = true;
                SetPortNormalVoltage();
                return true;
            }

            errorMessage = "Could not get the firmware version or driver version information.";
            return false;
        }

        /// <summary>
        /// Disables access to the device
        /// </summary>
        /// <returns>True on success</returns>
        public bool Shutdown()
        {
            return true;
        }

        /// <summary>
        /// Indicates that the device has been modified enough to warrant saving.
        /// </summary>
        protected virtual void OnDeviceSaveRequired()
        {
            DeviceSaveRequired?.Invoke(this, null);
        }

        /// <summary>
        /// Convert voltage for digital values, if the port is digital
        /// </summary>
        /// <param name="port"></param>
        /// <param name="voltage"></param>
        /// <returns></returns>
        private static double GetVoltageToUse(LabjackU12OutputPorts port, double voltage)
        {
            return GetVoltageToUse(Enum.GetName(typeof(LabjackU12OutputPorts), port), voltage);
        }

        /// <summary>
        /// Convert voltage for digital values, if the port is digital
        /// </summary>
        /// <param name="port"></param>
        /// <param name="voltage"></param>
        /// <returns></returns>
        private static double GetVoltageToUse(string port, double voltage)
        {
            if (port.StartsWith("A"))
                return voltage;

            if (voltage < 2.51)
                return CONST_DIGITALLOW;

            return CONST_DIGITALHIGH;
        }

        /// <summary>
        /// Set the normal voltage value for the port
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void SetPortNormalVoltage()
        {
            if (!isInitialized || inEmulationMode)
            {
                return;
            }

            var normalVoltageValue = GetVoltageToUse(Port, NormalVoltage);

            try
            {
                labjackDevice.Write(Port, normalVoltageValue);
            }
            catch (LabjackU12Exception ex)
            {
                Error?.Invoke(this, new DeviceErrorEventArgs("Could not set the normal voltage.",
                    ex,
                    DeviceErrorStatus.ErrorAffectsAllColumns,
                    this));
                throw new Exception("Could not set the normal voltage on write.  " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Triggers a pulse, using the stored values.
        /// </summary>
        /// <param name="timeout"></param>
        [LCMethodEvent("Trigger Fixed", MethodOperationTimeoutType.Parameter, "", -1, false, EventDescription = "Send a trigger using the saved parameters for this hardware device. Can be used to send low-pulse triggers.")]
        public int Trigger(double timeout = 0)
        {
            return TriggerFlexible(PulseLength, Port, PulseVoltage);
        }

        /// <summary>
        /// Triggers a 5V pulse of the specified length.
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="pulseLengthSeconds">The length of the pulse in seconds</param>
        [LCMethodEvent("Trigger", MethodOperationTimeoutType.Parameter, "", -1, false, EventDescription = "Send a high-pulse trigger using the saved port parameter for this hardware device, for the provided time")]
        public int Trigger(double timeout, double pulseLengthSeconds)
        {
            return Trigger(timeout, labjackPort, pulseLengthSeconds);
        }

        /// <summary>
        /// Triggers a 5V pulse of the specified length.
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="port"></param>
        /// <param name="pulseLengthSeconds">The length of the pulse in seconds</param>
        [LCMethodEvent("Trigger Port", MethodOperationTimeoutType.Parameter, "", -1, false, EventDescription = "Send a high-pulse trigger to the provided port, for the provided time")]
        public int Trigger(double timeout, LabjackU12OutputPorts port, double pulseLengthSeconds)
        {
            if (inEmulationMode)
            {
                return 0;
            }

            var tempPortName = Enum.GetName(typeof(LabjackU12OutputPorts), port);

            var error = 0;

            try
            {
                if (tempPortName[0] == CONST_ANALOGPREFIX)
                {
                    labjackDevice.Write(port, CONST_ANALOGHIGH);
                }
                else
                {
                    labjackDevice.Write(port, CONST_DIGITALHIGH);
                }
            }
            catch (LabjackU12Exception ex)
            {
                Error?.Invoke(this, new DeviceErrorEventArgs("Could not start the trigger.",
                                     ex,
                                     DeviceErrorStatus.ErrorAffectsAllColumns,
                                     this));
                throw new Exception("Could not trigger the contact closure on write.  " + ex.Message, ex);
            }

            var timer = new TimerDevice();
            if (AbortEvent != null)
            {
                timer.AbortEvent = AbortEvent;
            }
            timer.WaitSeconds(pulseLengthSeconds);

            try
            {
                labjackDevice.Write(port, CONST_ANALOGLOW);
            }
            catch (LabjackU12Exception ex)
            {
                Error?.Invoke(this,
                    new DeviceErrorEventArgs("Could not stop the trigger.",
                                             ex,
                                             DeviceErrorStatus.ErrorAffectsAllColumns,
                                             this));
                error = 1;
                throw;
            }
            return error;
        }

        /// <summary>
        /// Triggers a pulse of the specified voltage, lasting the specified duration.
        /// This is intended for use on the analog output ports--if it is a digital
        /// port the specified voltage will be disregarded.
        /// </summary>
        /// <param name="pulseLengthSeconds">The length of the pulse in seconds</param>
        /// <param name="voltage">The voltage to set</param>
        [LCMethodEvent("Trigger With Voltage", MethodOperationTimeoutType.Parameter, "", -1, false, EventDescription = "Send a non-zero-pulse trigger using the saved port parameter for this hardware device, for the provided time\nFor analog ports, the set voltage is used, for digital ports digital 'high' is sent")]
        public int Trigger(int pulseLengthSeconds, double voltage)
        {
            return Trigger(pulseLengthSeconds, labjackPort, voltage);
        }

        /// <summary>
        /// Triggers a pulse of the specified voltage, lasting the specified duration.
        /// This is intended for use on the analog output ports--if it is a digital
        /// port the specified voltage will be disregarded.
        /// </summary>
        /// <param name="pulseLengthSeconds">The length of the pulse in seconds</param>
        /// <param name="port"></param>
        /// <param name="voltage">The voltage to set</param>
        [LCMethodEvent("Trigger With Voltage Port", MethodOperationTimeoutType.Parameter, "", -1, false, EventDescription = "Send a non-zero-pulse trigger to the provided port, for the provided time\nFor analog ports, the set voltage is used, for digital ports digital 'high' is sent")]
        public int Trigger(int pulseLengthSeconds, LabjackU12OutputPorts port, double voltage)
        {
            if (inEmulationMode)
            {
                return 0;
            }

            var tempPortName = Enum.GetName(typeof(LabjackU12OutputPorts), port);
            var error = 0;
            try
            {
                if (tempPortName[0] == CONST_ANALOGPREFIX)
                {
                    labjackDevice.Write(port, voltage);
                }
                else
                {
                    labjackDevice.Write(port, CONST_DIGITALHIGH);
                }
            }
            catch (LabjackU12Exception ex)
            {
                throw ex;
            }

            var timer = new TimerDevice();
            timer.WaitSeconds(pulseLengthSeconds);

            try
            {
                labjackDevice.Write(port, CONST_ANALOGLOW);
            }
            catch (LabjackU12Exception ex)
            {
                throw ex;
            }

            return error;
        }

        /// <summary>
        /// Triggers a pulse of the specified voltage, lasting the specified duration.
        /// This is intended for use particularly with ports where we need to set a 'normal' value that is not zero
        /// </summary>
        /// <param name="pulseLengthSeconds">The length of the pulse in seconds</param>
        /// <param name="port"></param>
        /// <param name="voltage">The voltage to set</param>
        private int TriggerFlexible(int pulseLengthSeconds, LabjackU12OutputPorts port, double voltage)
        {
            if (inEmulationMode)
            {
                return 0;
            }

            var tempPortName = Enum.GetName(typeof(LabjackU12OutputPorts), port);
            var error = 0;
            try
            {
                var useVoltage = GetVoltageToUse(tempPortName, voltage);
                labjackDevice.Write(port, useVoltage);
            }
            catch (LabjackU12Exception ex)
            {
                throw ex;
            }

            var timer = new TimerDevice();
            timer.WaitSeconds(pulseLengthSeconds);

            try
            {
                var useVoltage = GetVoltageToUse(tempPortName, NormalVoltage);
                labjackDevice.Write(port, useVoltage);
            }
            catch (LabjackU12Exception ex)
            {
                throw ex;
            }

            return error;
        }

        public override string ToString()
        {
            return deviceName;
        }
        #endregion

        #region IDevice Data Provider Methods
        public void RegisterDataProvider(string key, DelegateDeviceHasData remoteMethod)
        {
        }

        public void UnRegisterDataProvider(string key, DelegateDeviceHasData remoteMethod)
        {
        }
        #endregion

        /// <summary>
        /// Writes any performance data cached to directory path provided.
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        public void WritePerformanceData(string directoryPath, string name, object[] parameters)
        {
        }

        #region IDevice Members

        /// <summary>
        /// Gets or sets the error type of last error.
        /// </summary>
        public DeviceErrorStatus ErrorType { get; set; }

        /// <summary>
        /// Gets or sets the device type.
        /// </summary>
        public DeviceType DeviceType => DeviceType.Component;

        #endregion

        #region IDevice Members

        public List<string> GetStatusNotificationList()
        {
            return new List<string>() { "Status" };
        }

        public List<string> GetErrorNotificationList()
        {
            return new List<string>();
        }

        #endregion

        /*/// <summary>
        /// Returns a set of health information.
        /// </summary>
        /// <returns></returns>
        public FinchComponentData GetData()
        {
            FinchComponentData component = new FinchComponentData();
            component.Status    = Status.ToString();
            component.Name      = Name;
            component.Type = "Contact Closure";
            component.LastUpdate = DateTime.Now;

            FinchScalarSignal measurement = new FinchScalarSignal();
            measurement.Name        = "ID";
            measurement.Type        = FinchDataType.String;
            measurement.Units       = "";
            measurement.Value       = LabJackID.ToString();
            component.Signals.Add(measurement);

            FinchScalarSignal port = new FinchScalarSignal();
            port.Name           = "Port";
            port.Type           = FinchDataType.String;
            port.Units          = "";
            port.Value          = this.Port.ToString();
            component.Signals.Add(port);

            return component;
        } */
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
