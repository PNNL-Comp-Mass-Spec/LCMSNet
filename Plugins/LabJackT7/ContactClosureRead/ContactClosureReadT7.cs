using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using FluidicsSDK.Devices;
using LcmsNetSDK;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Method;
using LcmsNetSDK.System;

namespace LcmsNetPlugins.LabJackT7.ContactClosureRead
{
    [Serializable]
    [DeviceControl(typeof(ContactClosureReadT7ViewModel),
            "Contact Closure Read T7",
            "Contact Closure Readers")]
    public class ContactClosureReadT7 : IDevice, IContactClosure
    {
        [Flags]
        public enum ContactClosureState
        {
            Open = 0x1,
            Closed = 0x2,
        }

        private const string ConfigurationError = "Configuration error";
        private const string DeviceReadError = "Voltage read error";
        private const string DeviceReadStateError = "Read State Not Matched";

        /// <summary>
        /// The LabJack used for reading the ready signal
        /// </summary>
        private readonly LabJackT7 labJackDevice;
        /// <summary>
        /// The port on the LabJack on which to read the voltage.
        /// </summary>
        private LabJackT7Inputs labJackPort;
        /// <summary>
        /// The name, used in software for the symbol.
        /// </summary>
        private string deviceName;
        /// <summary>
        /// The current status of the LabJack.
        /// </summary>
        private DeviceStatus deviceStatus;
        /// <summary>
        /// Flag indicating if the device is in emulation mode.
        /// </summary>
        private bool inEmulationMode;
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
        public ContactClosureReadT7()
        {
            labJackDevice = new LabJackT7();
            labJackPort = LabJackT7Inputs.AIN0;
            deviceName = "Contact Closure Reader";
        }

        /// <summary>
        /// Constructor which assigns a LabJack
        /// </summary>
        /// <param name="lj">The LabJack</param>
        public ContactClosureReadT7(LabJackT7 lj)
        {
            labJackDevice = lj;
            labJackPort = LabJackT7Inputs.AIN0;
            deviceName = "Contact Closure Reader";
        }

        /// <summary>
        /// Constructor which assigns a port
        /// </summary>
        /// <param name="newPort">The port on the LabJack to use for reading the ready signal</param>
        public ContactClosureReadT7(LabJackT7Inputs newPort)
        {
            labJackDevice = new LabJackT7();
            labJackPort = newPort;
            deviceName = "Contact Closure Reader";
        }

        /// <summary>
        /// Constructor which assigns a LabJack and a port
        /// </summary>
        /// <param name="lj">The LabJack</param>
        /// <param name="newPort">The port on the LabJack to use for reading the ready signal</param>
        public ContactClosureReadT7(LabJackT7 lj, LabJackT7Inputs newPort)
        {
            labJackDevice = lj;
            labJackPort = newPort;
            deviceName = "Contact Closure Reader";
        }

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
        /// Gets or sets the port on the LabJack used for the pulse. Defaults to AO0.
        /// </summary>
        [DeviceSavedSetting("Port")]
        public LabJackT7Inputs Port
        {
            get => labJackPort;
            set
            {
                labJackPort = value;
                OnDeviceSaveRequired();
            }
        }

        [DeviceSavedSetting("LabJack ID")]
        public int LabJackID
        {
            get => labJackDevice.LocalID;
            set => labJackDevice.LocalID = value;
        }

        //Initialize/Shutdown don't really apply
        //Maybe confirm that we can communicate to the LabJack? I don't know.
        public bool Initialize(ref string errorMessage)
        {
            //Get the version info
            labJackDevice.GetDriverVersion();
            labJackDevice.GetFirmwareVersion();

            //If either FirmwareVersion or DriverVersion is zero, it means we have a communication failure.
            if (labJackDevice.HardwareVersion > 0 && labJackDevice.FirmwareVersion > 0 &&
                string.IsNullOrWhiteSpace(labJackDevice.DriverVersion))
            {
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

        public double ReadVoltage()
        {
            return ReadVoltage(labJackPort);
        }

        public double ReadVoltage(LabJackT7Inputs port)
        {
            var state = 0.0;

            try
            {
                state = labJackDevice.Read(port);
            }
            catch //(LabJackT7Exception ex)
            {
                // NOTE: Don't kill things if an exception occurs here, this is only used by the user interface right now.
                //Error?.Invoke(this, new DeviceErrorEventArgs("Could not read contact closure state.",
                //    ex,
                //    DeviceErrorStatus.ErrorAffectsAllColumns,
                //    this,
                //    DeviceReadError));
                //throw new Exception("Could not read contact closure state.  " + ex.Message, ex);
            }

            return state;
        }

        /// <summary>
        /// Read a port's status, returning a bool for success if the state was matched
        /// </summary>
        /// <param name="timeout">Timeout, for when to consider the device has errored</param>
        /// <param name="target">The desired state of the contact closure</param>
        /// <returns>True if the state of the contact closure matched the target state</returns>
        [LCMethodEvent("Read", MethodOperationTimeoutType.Parameter, IgnoreLeftoverTime = true, EventDescription = "Non-deterministic: Timeout is maximum time allowed, but next step will be started as soon as the desired state is read")]
        public bool ReadStatusAuto(double timeout, ContactClosureState target = ContactClosureState.Closed | ContactClosureState.Open)
        {
            var closureState = ReadStateAuto(timeout, target);
            return target.HasFlag(closureState);
        }

        /// <summary>
        /// Read a port's status, returning a bool for success if the state was matched
        /// </summary>
        /// <param name="timeout">Timeout, for when to consider the device has errored</param>
        /// <param name="target">The desired state of the contact closure</param>
        /// <returns>True if the state of the contact closure matched the target state</returns>
        [LCMethodEvent("Read Digital", MethodOperationTimeoutType.Parameter, IgnoreLeftoverTime = true, EventDescription = "Non-deterministic: Timeout is maximum time allowed, but next step will be started as soon as the desired state is read")]
        public bool ReadStatusDigital(double timeout, ContactClosureState target = ContactClosureState.Closed | ContactClosureState.Open)
        {
            return ReadStatusDigital(timeout, labJackPort, target);
        }

        /// <summary>
        /// Read a port's status, returning a bool for success if the state was matched
        /// </summary>
        /// <param name="timeout">Timeout, for when to consider the device has errored</param>
        /// <param name="port">The LabJack port</param>
        /// <param name="target">The desired state of the contact closure</param>
        /// <returns>True if the state of the contact closure matched the target state</returns>
        [LCMethodEvent("Read Port Digital", MethodOperationTimeoutType.Parameter, IgnoreLeftoverTime = true, EventDescription = "Non-deterministic: Timeout is maximum time allowed, but next step will be started as soon as the desired state is read")]
        public bool ReadStatusDigital(double timeout, LabJackT7Inputs port, ContactClosureState target = ContactClosureState.Closed | ContactClosureState.Open)
        {
            var closureState = ReadStateDigital(timeout, port, target);
            return target.HasFlag(closureState);
        }

        /// <summary>
        /// Reads a port's status, returning a bool for success if the state was matched
        /// This is intended for use on the analog output ports--if it is a digital
        /// port the specified voltage will be disregarded.
        /// </summary>
        /// <param name="timeout">Timeout, for when to consider the device has errored</param>
        /// <param name="voltage">The midpoint voltage - readVoltage >= voltage will be "closed" state</param>
        /// <param name="target">The desired state of the contact closure</param>
        /// <returns>True if the state of the contact closure matched the target state</returns>
        [LCMethodEvent("Read Analog", MethodOperationTimeoutType.Parameter, IgnoreLeftoverTime = true, EventDescription = "Non-deterministic: Timeout is maximum time allowed, but next step will be started as soon as the desired state is read")]
        public bool ReadStatusAnalog(double timeout, double voltage, ContactClosureState target = ContactClosureState.Closed | ContactClosureState.Open)
        {
            return ReadStatusAnalog(timeout, labJackPort, voltage, target);
        }

        /// <summary>
        /// Reads a port's status, returning a bool for success if the state was matched
        /// This is intended for use on the analog output ports--if it is a digital
        /// port the specified voltage will be disregarded.
        /// </summary>
        /// <param name="timeout">Timeout, for when to consider the device has errored</param>
        /// <param name="port">The port to read</param>
        /// <param name="voltage">The midpoint voltage - readVoltage >= voltage will be "closed" state</param>
        /// <param name="target">The desired state of the contact closure</param>
        /// <returns>True if the state of the contact closure matched the target state</returns>
        [LCMethodEvent("Read Port Analog", MethodOperationTimeoutType.Parameter, IgnoreLeftoverTime = true, EventDescription = "Non-deterministic: Timeout is maximum time allowed, but next step will be started as soon as the desired state is read")]
        public bool ReadStatusAnalog(double timeout, LabJackT7Inputs port, double voltage, ContactClosureState target = ContactClosureState.Closed | ContactClosureState.Open)
        {
            var closureState = ReadStateAnalog(timeout, port, voltage, target);
            return target.HasFlag(closureState);
        }

        /// <summary>
        /// Read a port's status
        /// </summary>
        /// <param name="timeout">Timeout, for when to consider the device has errored</param>
        /// <param name="target">The desired state of the contact closure</param>
        /// <returns>the state of the contact closure</returns>
        public ContactClosureState ReadStateAuto(double timeout, ContactClosureState target = ContactClosureState.Closed | ContactClosureState.Open)
        {
            if ((int) Port < (int) LabJackT7Inputs.FIO0)
            {
                return ReadStateAnalog(timeout, labJackPort, 2.5, target);
            }
            else
            {
                return ReadStateDigital(timeout, labJackPort, target);
            }
        }

        /// <summary>
        /// Read a port's status
        /// </summary>
        /// <param name="timeout">Timeout, for when to consider the device has errored</param>
        /// <param name="target">The desired state of the contact closure</param>
        /// <returns>the state of the contact closure</returns>
        public ContactClosureState ReadStateDigital(double timeout, ContactClosureState target = ContactClosureState.Closed | ContactClosureState.Open)
        {
            return ReadStateDigital(timeout, labJackPort, target);
        }

        /// <summary>
        /// Read a port's status
        /// </summary>
        /// <param name="timeout">Timeout, for when to consider the device has errored</param>
        /// <param name="port">The LabJack port</param>
        /// <param name="target">The desired state of the contact closure</param>
        /// <returns>the state of the contact closure</returns>
        public ContactClosureState ReadStateDigital(double timeout, LabJackT7Inputs port, ContactClosureState target = ContactClosureState.Closed | ContactClosureState.Open)
        {
            if (inEmulationMode)
            {
                return 0;
            }

            ContactClosureState closureState;

            try
            {
                var startTime = TimeKeeper.Instance.Now;
                var endTime = startTime.Add(TimeSpan.FromSeconds(timeout));
                var state = labJackDevice.Read(port);
                closureState = state.Equals(0) ? ContactClosureState.Open : ContactClosureState.Closed;
                while (TimeKeeper.Instance.Now < endTime && !target.HasFlag(closureState))
                {
                    // Maximum read speed: 50Hz (every 20 milliseconds).
                    // For faster read speeds, there are burst and streaming modes that would require processing array data (for analog in and IO inputs)
                    Thread.Sleep(TimeSpan.FromMilliseconds(50));
                    state = labJackDevice.Read(port);
                    closureState = state.Equals(0) ? ContactClosureState.Open : ContactClosureState.Closed;
                }
            }
            catch (LabJackT7Exception ex)
            {
                Error?.Invoke(this, new DeviceErrorEventArgs("Could not read the port.",
                              ex, DeviceErrorStatus.ErrorAffectsAllColumns, this, DeviceReadError));
                throw new Exception("Could not read the contact closure state.  " + ex.Message, ex);
            }

            if (!target.HasFlag(closureState))
            {
                Error?.Invoke(this, new DeviceErrorEventArgs("Contact closure was not in the required state of \"" + target + "\"",
                    null, DeviceErrorStatus.ErrorAffectsAllColumns, this, DeviceReadStateError, DeviceEventLoggingType.Error));
            }

            return closureState;
        }

        /// <summary>
        /// Read a port's status
        /// This is intended for use on the analog input ports--if it is a digital
        /// port the specified voltage will be disregarded.
        /// </summary>
        /// <param name="timeout">Timeout, for when to consider the device has errored</param>
        /// <param name="voltage">The midpoint voltage - readVoltage >= voltage will be "closed" state</param>
        /// <param name="target">The desired state of the contact closure</param>
        /// <returns>the state of the contact closure</returns>
        public ContactClosureState ReadStateAnalog(double timeout, double voltage, ContactClosureState target = ContactClosureState.Closed | ContactClosureState.Open)
        {
            return ReadStateAnalog(timeout, labJackPort, voltage, target);
        }

        /// <summary>
        /// Read a port's status
        /// This is intended for use on the analog input ports--if it is a digital
        /// port the specified voltage will be disregarded.
        /// </summary>
        /// <param name="timeout">Timeout, for when to consider the device has errored</param>
        /// <param name="port">The port to read</param>
        /// <param name="voltage">The midpoint voltage - readVoltage >= voltage will be "closed" state</param>
        /// <param name="target">The desired state of the contact closure</param>
        /// <returns>the state of the contact closure</returns>
        public ContactClosureState ReadStateAnalog(double timeout, LabJackT7Inputs port, double voltage, ContactClosureState target = ContactClosureState.Closed | ContactClosureState.Open)
        {
            if (inEmulationMode)
            {
                return 0;
            }

            ContactClosureState closureState;

            try
            {
                var startTime = TimeKeeper.Instance.Now;
                var endTime = startTime.Add(TimeSpan.FromSeconds(timeout));
                var outVoltage = labJackDevice.Read(port);
                closureState = outVoltage < voltage ? ContactClosureState.Open : ContactClosureState.Closed;
                while (TimeKeeper.Instance.Now < endTime && !target.HasFlag(closureState))
                {
                    // Maximum read speed: 50Hz (every 20 milliseconds).
                    // For faster read speeds, there are burst and streaming modes that would require processing array data (for analog in and IO inputs)
                    Thread.Sleep(TimeSpan.FromMilliseconds(50));
                    outVoltage = labJackDevice.Read(port);
                    closureState = outVoltage < voltage ? ContactClosureState.Open : ContactClosureState.Closed;
                }
            }
            catch (LabJackT7Exception ex)
            {
                Error?.Invoke(this, new DeviceErrorEventArgs("Could not read the port.",
                              ex, DeviceErrorStatus.ErrorAffectsAllColumns, this, DeviceReadError));
                throw new Exception("Could not read the contact closure state.  " + ex.Message, ex);
            }

            if (!target.HasFlag(closureState))
            {
                Error?.Invoke(this, new DeviceErrorEventArgs("Contact closure was not in the required state of \"" + target + "\"",
                    null, DeviceErrorStatus.ErrorAffectsAllColumns, this, DeviceReadStateError, DeviceEventLoggingType.Error));
            }

            return closureState;
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
            return new List<string>() { DeviceReadError, DeviceReadStateError };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
