using System;
using System.Collections.Generic;
using System.ComponentModel;
using FluidicsSDK.Devices;
using LabJackTSeries;
using LcmsNetSDK;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Method;

namespace LcmsNetPlugins.LabJackTDevice.ContactClosure
{
    [Serializable]
    [DeviceControl(typeof(ContactClosureT7ViewModel),
                                 "Contact Closure T7",
                                 "Contact Closures")]
    public class ContactClosureT7 : IDevice, IContactClosure
    {
        private const string ConfigurationError = "Configuration error";
        private const string DeviceWriteError = "Voltage set error";
        private const string DeviceReadError = "Voltage read error";

        /// <summary>
        /// The LabJack used for signalling the pulse
        /// </summary>
        private readonly LabJackT7 labJackDevice;
        /// <summary>
        /// The length of the pulse to send on the port. Defaults to 1 second.
        /// </summary>
        private int pulseLength = 1;
        /// <summary>
        /// The voltage to apply on the port for the pulse duration. Defaults to 5V.
        /// </summary>
        private double pulseVoltage = 5;
        /// <summary>
        /// The 'normal'/default voltage for the LabJack port. Defaults to 0V.
        /// </summary>
        private double normalVoltage = CONST_ANALOGLOW;
        /// <summary>
        /// The port on the LabJack on which to apply the voltage.
        /// </summary>
        private LabJackT7Outputs labJackPort;
        /// <summary>
        /// The name, used in software for the symbol.
        /// </summary>
        private string deviceName;
        /// <summary>
        /// The current status of the LabJack.
        /// </summary>
        private DeviceStatus deviceStatus;

        private bool isInitialized = false;
        private const char CONST_ANALOGPREFIX = 'A';
        private const int CONST_ANALOGHIGH = 5;
        private const int CONST_DIGITALHIGH = 1;
        private const int CONST_ANALOGLOW = 0;
        private const int CONST_DIGITALLOW = 0;

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

        /// <summary>
        /// Default constructor--no LabJack assigned!
        /// </summary>
        public ContactClosureT7()
        {
            labJackDevice = new LabJackT7();
            labJackPort    = LabJackT7Outputs.FIO0;
            deviceName = "Contact Closure";
        }

        /// <summary>
        /// Constructor which assigns a LabJack
        /// </summary>
        /// <param name="lj">The LabJack</param>
        public ContactClosureT7(LabJackT7 lj)
        {
            labJackDevice = lj;
            labJackPort    = LabJackT7Outputs.FIO0;
            deviceName = "Contact Closure";
        }

        /// <summary>
        /// Constructor which assigns a port
        /// </summary>
        /// <param name="newPort">The port on the LabJack to use for the pulse</param>
        public ContactClosureT7(LabJackT7Outputs newPort)
        {
            labJackDevice = new LabJackT7();
            labJackPort    = newPort;
            deviceName = "Contact Closure";
        }

        /// <summary>
        /// Constructor which assigns a LabJack and a port
        /// </summary>
        /// <param name="lj">The LabJack</param>
        /// <param name="newPort">The port on the LabJack to use for the pulse</param>
        public ContactClosureT7(LabJackT7 lj, LabJackT7Outputs newPort)
        {
            labJackDevice = lj;
            labJackPort    = newPort;
            deviceName = "Contact Closure";
        }

        /// <summary>
        /// Gets or sets the abort event for scheduling.
        /// </summary>
        public System.Threading.ManualResetEvent AbortEvent { get; set; }

        /// <summary>
        /// Gets or sets the emulation state of the device.
        /// </summary>
        //[PersistenceDataAttribute("Emulated")]
        public bool Emulation { get; set; }

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
        /// The length of the pulse to send on the port. Defaults to 1 second.
        /// </summary>
        [DeviceSavedSetting("PulseLengthSeconds")]
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
        [DeviceSavedSetting("PulseVoltage")]
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
        /// The 'normal'/default voltage for the LabJack port. Defaults to 0V.
        /// </summary>
        [DeviceSavedSetting("NormalVoltage")]
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
        /// Gets or sets the port on the LabJack used for the pulse. Defaults to AO0.
        /// </summary>
        [DeviceSavedSetting("Port")]
        public LabJackT7Outputs Port
        {
            get => labJackPort;
            set
            {
                labJackPort = value;
                OnDeviceSaveRequired();
                SetPortNormalVoltage();
            }
        }

        [DeviceSavedSetting("LabJack ID")]
        public string LabJackID
        {
            get => labJackDevice.LabJackIdentifier;
            set => labJackDevice.LabJackIdentifier = value;
        }

        public bool Initialize(ref string errorMessage)
        {
            if (Emulation)
            {
                return true;
            }

            labJackDevice.Initialize();

            //Get the version info
            labJackDevice.GetDriverVersion();
            labJackDevice.GetHardwareVersion();
            labJackDevice.GetFirmwareVersion();

            //If either FirmwareVersion or DriverVersion is zero, it means we have a communication failure.
            if (labJackDevice.HardwareVersion > 0 && labJackDevice.FirmwareVersion > 0 &&
                !string.IsNullOrWhiteSpace(labJackDevice.DriverVersion))
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
            labJackDevice.Close();
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
        private static double GetVoltageToUse(LabJackT7Outputs port, double voltage)
        {
            return GetVoltageToUse(Enum.GetName(typeof(LabJackT7Outputs), port), voltage);
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
            if (!isInitialized || Emulation)
            {
                return;
            }

            var normalVoltageValue = GetVoltageToUse(Port, NormalVoltage);

            try
            {
                labJackDevice.Write(Port, normalVoltageValue);
            }
            catch (LabJackTException ex)
            {
                Error?.Invoke(this, new DeviceErrorEventArgs("Could not set the normal voltage.",
                    ex,
                    DeviceErrorStatus.ErrorAffectsAllColumns,
                    this,
                    ConfigurationError));
                throw new Exception("Could not set the normal voltage on write.  " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Triggers a pulse, using the stored values.
        /// </summary>
        /// <param name="timeout"></param>
        [LCMethodEvent("Trigger Fixed", MethodOperationTimeoutType.Parameter, EventDescription = "Send a trigger using the saved parameters for this hardware device. Can be used to send low-pulse triggers.")]
        public int Trigger(double timeout = 0)
        {
            return TriggerFlexible(PulseLength, Port, PulseVoltage);
        }

        /// <summary>
        /// Triggers a 5V pulse of the specified length.
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="pulseLengthSeconds">The length of the pulse in seconds</param>
        [LCMethodEvent("Trigger", MethodOperationTimeoutType.Parameter, EventDescription = "Send a high-pulse trigger using the saved port parameter for this hardware device, for the provided time")]
        public int Trigger(double timeout, double pulseLengthSeconds)
        {
            return Trigger(timeout, labJackPort, pulseLengthSeconds);
        }

        /// <summary>
        /// Triggers a 5V pulse of the specified length.
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="port"></param>
        /// <param name="pulseLengthSeconds">The length of the pulse in seconds</param>
        [LCMethodEvent("Trigger Port", MethodOperationTimeoutType.Parameter, EventDescription = "Send a high-pulse trigger to the provided port, for the provided time")]
        public int Trigger(double timeout, LabJackT7Outputs port, double pulseLengthSeconds)
        {
            if (Emulation)
            {
                return 0;
            }

            var tempPortName = Enum.GetName(typeof(LabJackT7Outputs), port);

            var error = 0;

            try
            {
                if (tempPortName[0] == CONST_ANALOGPREFIX)
                {
                    labJackDevice.Write(port, CONST_ANALOGHIGH);
                }
                else
                {
                    labJackDevice.Write(port, CONST_DIGITALHIGH);
                }
            }
            catch (LabJackTException ex)
            {
                Error?.Invoke(this, new DeviceErrorEventArgs("Could not start the trigger.",
                                     ex,
                                     DeviceErrorStatus.ErrorAffectsAllColumns,
                                     this,
                                     DeviceWriteError));
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
                labJackDevice.Write(port, CONST_ANALOGLOW);
            }
            catch (LabJackTException ex)
            {
                Error?.Invoke(this,
                    new DeviceErrorEventArgs("Could not stop the trigger.",
                                             ex,
                                             DeviceErrorStatus.ErrorAffectsAllColumns,
                                             this,
                                             DeviceWriteError));
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
        [LCMethodEvent("Trigger With Voltage", MethodOperationTimeoutType.Parameter, EventDescription = "Send a non-zero-pulse trigger using the saved port parameter for this hardware device, for the provided time\nFor analog ports, the set voltage is used, for digital ports digital 'high' is sent")]
        public int Trigger(int pulseLengthSeconds, double voltage)
        {
            return Trigger(pulseLengthSeconds, labJackPort, voltage);
        }

        /// <summary>
        /// Triggers a pulse of the specified voltage, lasting the specified duration.
        /// This is intended for use on the analog output ports--if it is a digital
        /// port the specified voltage will be disregarded.
        /// </summary>
        /// <param name="pulseLengthSeconds">The length of the pulse in seconds</param>
        /// <param name="port"></param>
        /// <param name="voltage">The voltage to set</param>
        [LCMethodEvent("Trigger With Voltage Port", MethodOperationTimeoutType.Parameter, EventDescription = "Send a non-zero-pulse trigger to the provided port, for the provided time\nFor analog ports, the set voltage is used, for digital ports digital 'high' is sent")]
        public int Trigger(int pulseLengthSeconds, LabJackT7Outputs port, double voltage)
        {
            if (Emulation)
            {
                return 0;
            }

            var tempPortName = Enum.GetName(typeof(LabJackT7Outputs), port);
            var error = 0;
            try
            {
                if (tempPortName[0] == CONST_ANALOGPREFIX)
                {
                    labJackDevice.Write(port, voltage);
                }
                else
                {
                    labJackDevice.Write(port, CONST_DIGITALHIGH);
                }
            }
            catch (LabJackTException ex)
            {
                Error?.Invoke(this, new DeviceErrorEventArgs("Could not start the trigger.",
                    ex,
                    DeviceErrorStatus.ErrorAffectsAllColumns,
                    this,
                    DeviceWriteError));
                throw new Exception("Could not trigger the contact closure on write.  " + ex.Message, ex);
            }

            var timer = new TimerDevice();
            timer.WaitSeconds(pulseLengthSeconds);

            try
            {
                labJackDevice.Write(port, CONST_ANALOGLOW);
            }
            catch (LabJackTException ex)
            {
                Error?.Invoke(this,
                    new DeviceErrorEventArgs("Could not stop the trigger.",
                        ex,
                        DeviceErrorStatus.ErrorAffectsAllColumns,
                        this,
                        DeviceWriteError));
                error = 1;
                throw;
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
        private int TriggerFlexible(int pulseLengthSeconds, LabJackT7Outputs port, double voltage)
        {
            if (Emulation)
            {
                return 0;
            }

            var tempPortName = Enum.GetName(typeof(LabJackT7Outputs), port);
            var error = 0;
            try
            {
                var useVoltage = GetVoltageToUse(tempPortName, voltage);
                labJackDevice.Write(port, useVoltage);
            }
            catch (LabJackTException ex)
            {
                Error?.Invoke(this, new DeviceErrorEventArgs("Could not start the trigger.",
                    ex,
                    DeviceErrorStatus.ErrorAffectsAllColumns,
                    this,
                    DeviceWriteError));
                throw new Exception("Could not trigger the contact closure on write.  " + ex.Message, ex);
            }

            var timer = new TimerDevice();
            timer.WaitSeconds(pulseLengthSeconds);

            try
            {
                var useVoltage = GetVoltageToUse(tempPortName, NormalVoltage);
                labJackDevice.Write(port, useVoltage);
            }
            catch (LabJackTException ex)
            {
                Error?.Invoke(this,
                    new DeviceErrorEventArgs("Could not stop the trigger.",
                        ex,
                        DeviceErrorStatus.ErrorAffectsAllColumns,
                        this,
                        DeviceWriteError));
                error = 1;
                throw;
            }

            return error;
        }

        public override string ToString()
        {
            return deviceName;
        }

        /// <summary>
        /// Gets or sets the error type of last error.
        /// </summary>
        public DeviceErrorStatus ErrorType { get; set; }

        /// <summary>
        /// Gets or sets the device type.
        /// </summary>
        public DeviceType DeviceType => DeviceType.Component;

        public List<string> GetStatusNotificationList()
        {
            return new List<string>() { "Status" };
        }

        public List<string> GetErrorNotificationList()
        {
            return new List<string>() { ConfigurationError, DeviceWriteError };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
