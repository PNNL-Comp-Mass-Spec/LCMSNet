using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using FluidicsSDK.Devices;
using LcmsNetPlugins.LabJack;
using LcmsNetSDK;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Method;
using LcmsNetSDK.System;

namespace LcmsNetPlugins.PNNLDevices.ContactClosureRead
{
    [Serializable]
    [DeviceControl(typeof(ContactClosureReadU12ViewModel),
            "Contact Closure Read U12",
            "Contact Closure Readers")]
    public class ContactClosureReadU12 : IDevice, IContactClosure
    {
        [Flags]
        public enum ContactClosureState
        {
            Open = 0x1,
            Closed = 0x2,
        }

        #region Members
        /// <summary>
        /// The labjack used for reading the ready signal
        /// </summary>
        private readonly LabjackU12 m_labjack;
        /// <summary>
        /// The port on the labjack on which to read the voltage.
        /// </summary>
        private LabjackU12InputPorts m_port;
        /// <summary>
        /// The name, used in software for the symbol.
        /// </summary>
        private string m_name;
        /// <summary>
        /// The version.
        /// </summary>
        private string m_version;
        /// <summary>
        /// The current status of the Labjack.
        /// </summary>
        private DeviceStatus m_status;
        /// <summary>
        /// Flag indicating if the device is in emulation mode.
        /// </summary>
        private bool m_emulation;
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
        public ContactClosureReadU12()
        {
            m_labjack = new LabjackU12();
            m_port = LabjackU12InputPorts.AI1;
            m_name = "Contact Closure Reader";
        }

        /// <summary>
        /// Constructor which assigns a labjack
        /// </summary>
        /// <param name="lj">The labjack</param>
        public ContactClosureReadU12(LabjackU12 lj)
        {
            m_labjack = lj;
            m_port = LabjackU12InputPorts.AI1;
            m_name = "Contact Closure Reader";
        }

        /// <summary>
        /// Constructor which assigns a port
        /// </summary>
        /// <param name="newPort">The port on the labjack to use for reading the ready signal</param>
        public ContactClosureReadU12(LabjackU12InputPorts newPort)
        {
            m_labjack = new LabjackU12();
            m_port = newPort;
            m_name = "Contact Closure Reader";
        }

        /// <summary>
        /// Constructor which assigns a labjack and a port
        /// </summary>
        /// <param name="lj">The labjack</param>
        /// <param name="newPort">The port on the labjack to use for reading the ready signal</param>
        public ContactClosureReadU12(LabjackU12 lj, LabjackU12InputPorts newPort)
        {
            m_labjack = lj;
            m_port = newPort;
            m_name = "Contact Closure Reader";
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
            get { return m_emulation; }
            set { m_emulation = value; }
        }

        /// <summary>
        /// Gets or sets the current status of the device.
        /// </summary>
        public DeviceStatus Status
        {
            get { return m_status; }
            set
            {
                if (value != m_status)
                {
                    StatusUpdate?.Invoke(this, new DeviceStatusEventArgs(value, "Status", this));
                }

                m_status = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the device in the fluidics designer.
        /// </summary>
        public string Name
        {
            get { return m_name; }
            set
            {
                if (this.RaiseAndSetIfChangedRetBool(ref m_name, value))
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
            get { return m_version; }
            set
            {
                m_version = value;
                OnDeviceSaveRequired();
            }
        }

        /// <summary>
        /// Gets or sets the port on the labjack used for the pulse. Defaults to AO0.
        /// </summary>
        [PersistenceData("Port")]
        public LabjackU12InputPorts Port
        {
            get { return m_port; }
            set
            {
                m_port = value;
                OnDeviceSaveRequired();
            }
        }

        [PersistenceData("Labjack ID")]
        public int LabJackID
        {
            get { return m_labjack.LocalID; }
            set { m_labjack.LocalID = value; }
        }

        #endregion

        #region Methods

        //Initialize/Shutdown don't really apply
        //Maybe confirm that we can communicate to the labjack? I don't know.
        public bool Initialize(ref string errorMessage)
        {
            //Get the version info
            m_labjack.GetDriverVersion();
            m_labjack.GetFirmwareVersion();

            //If we got anything, call it good
            if (m_labjack.FirmwareVersion.ToString(CultureInfo.InvariantCulture).Length > 0 &&
                m_labjack.DriverVersion.ToString(CultureInfo.InvariantCulture).Length > 0)
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

        public float ReadVoltage()
        {
            return ReadVoltage(m_port);
        }

        public float ReadVoltage(LabjackU12InputPorts port)
        {
            var state = 0f;

            try
            {
                state = m_labjack.Read(port);
            }
            catch (LabjackU12Exception)
            {
            }

            return state;
        }

        /// <summary>
        /// Read a port's status, returning a bool for success if the state was matched
        /// </summary>
        /// <param name="timeout">Timeout, for when to consider the device has errored</param>
        /// <param name="target">The desired state of the contact closure</param>
        /// <returns>True if the state of the contact closure matched the target state</returns>
        [LCMethodEvent("Read", MethodOperationTimeoutType.Parameter, "", -1, false)]
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
        [LCMethodEvent("Read Digital", MethodOperationTimeoutType.Parameter, "", -1, false)]
        public bool ReadStatusDigital(double timeout, ContactClosureState target = ContactClosureState.Closed | ContactClosureState.Open)
        {
            return ReadStatusDigital(timeout, m_port, target);
        }

        /// <summary>
        /// Read a port's status, returning a bool for success if the state was matched
        /// </summary>
        /// <param name="timeout">Timeout, for when to consider the device has errored</param>
        /// <param name="port">The LabJack port</param>
        /// <param name="target">The desired state of the contact closure</param>
        /// <returns>True if the state of the contact closure matched the target state</returns>
        [LCMethodEvent("Read Port Digital", MethodOperationTimeoutType.Parameter, "", -1, false)]
        public bool ReadStatusDigital(double timeout, LabjackU12InputPorts port, ContactClosureState target = ContactClosureState.Closed | ContactClosureState.Open)
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
        [LCMethodEvent("Read Analog", MethodOperationTimeoutType.Parameter, "", -1, false)]
        public bool ReadStatusAnalog(double timeout, double voltage, ContactClosureState target = ContactClosureState.Closed | ContactClosureState.Open)
        {
            return ReadStatusAnalog(timeout, m_port, voltage, target);
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
        [LCMethodEvent("Read Port Analog", MethodOperationTimeoutType.Parameter, "", -1, false)]
        public bool ReadStatusAnalog(double timeout, LabjackU12InputPorts port, double voltage, ContactClosureState target = ContactClosureState.Closed | ContactClosureState.Open)
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
            if ((int) Port <= (int) LabjackU12InputPorts.AI7)
            {
                return ReadStateAnalog(timeout, m_port, 2.5, target);
            }
            else
            {
                return ReadStateDigital(timeout, m_port, target);
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
            return ReadStateDigital(timeout, m_port, target);
        }

        /// <summary>
        /// Read a port's status
        /// </summary>
        /// <param name="timeout">Timeout, for when to consider the device has errored</param>
        /// <param name="port">The LabJack port</param>
        /// <param name="target">The desired state of the contact closure</param>
        /// <returns>the state of the contact closure</returns>
        public ContactClosureState ReadStateDigital(double timeout, LabjackU12InputPorts port, ContactClosureState target = ContactClosureState.Closed | ContactClosureState.Open)
        {
            if (m_emulation)
            {
                return 0;
            }

            ContactClosureState closureState;

            try
            {
                var startTime = TimeKeeper.Instance.Now;
                var endTime = startTime.Add(TimeSpan.FromSeconds(timeout));
                var state = m_labjack.Read(port);
                closureState = state.Equals(0) ? ContactClosureState.Open : ContactClosureState.Closed;
                while (TimeKeeper.Instance.Now < endTime && !target.HasFlag(closureState))
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(200));
                    state = m_labjack.Read(port);
                    closureState = state.Equals(0) ? ContactClosureState.Open : ContactClosureState.Closed;
                }
            }
            catch (LabjackU12Exception ex)
            {
                Error?.Invoke(this, new DeviceErrorEventArgs("Could not read the port.",
                              ex, DeviceErrorStatus.ErrorAffectsAllColumns, this, "Read Failure"));
                throw new Exception("Could not read the contact closure state.  " + ex.Message, ex);
            }

            if (!target.HasFlag(closureState))
            {
                Error?.Invoke(this, new DeviceErrorEventArgs("Contact closure was not in the required state of \"" + target + "\"",
                    null, DeviceErrorStatus.ErrorAffectsAllColumns, this, "Read State Not Matched", DeviceEventLoggingType.Error));
            }

            return closureState;
        }

        /// <summary>
        /// Read a port's status
        /// This is intended for use on the analog output ports--if it is a digital
        /// port the specified voltage will be disregarded.
        /// </summary>
        /// <param name="timeout">Timeout, for when to consider the device has errored</param>
        /// <param name="voltage">The midpoint voltage - readVoltage >= voltage will be "closed" state</param>
        /// <param name="target">The desired state of the contact closure</param>
        /// <returns>the state of the contact closure</returns>
        public ContactClosureState ReadStateAnalog(double timeout, double voltage, ContactClosureState target = ContactClosureState.Closed | ContactClosureState.Open)
        {
            return ReadStateAnalog(timeout, m_port, voltage, target);
        }
        /// <summary>
        /// Read a port's status
        /// This is intended for use on the analog output ports--if it is a digital
        /// port the specified voltage will be disregarded.
        /// </summary>
        /// <param name="timeout">Timeout, for when to consider the device has errored</param>
        /// <param name="port">The port to read</param>
        /// <param name="voltage">The midpoint voltage - readVoltage >= voltage will be "closed" state</param>
        /// <param name="target">The desired state of the contact closure</param>
        /// <returns>the state of the contact closure</returns>
        public ContactClosureState ReadStateAnalog(double timeout, LabjackU12InputPorts port, double voltage, ContactClosureState target = ContactClosureState.Closed | ContactClosureState.Open)
        {
            if (m_emulation)
            {
                return 0;
            }

            ContactClosureState closureState;

            try
            {
                var startTime = TimeKeeper.Instance.Now;
                var endTime = startTime.Add(TimeSpan.FromSeconds(timeout));
                var outVoltage = m_labjack.Read(port);
                closureState = outVoltage < voltage ? ContactClosureState.Open : ContactClosureState.Closed;
                while (TimeKeeper.Instance.Now < endTime && !target.HasFlag(closureState))
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(200));
                    outVoltage = m_labjack.Read(port);
                    closureState = outVoltage < voltage ? ContactClosureState.Open : ContactClosureState.Closed;
                }
            }
            catch (LabjackU12Exception ex)
            {
                Error?.Invoke(this, new DeviceErrorEventArgs("Could not read the port.",
                              ex, DeviceErrorStatus.ErrorAffectsAllColumns, this, "Read Failure"));
                throw new Exception("Could not read the contact closure state.  " + ex.Message, ex);
            }

            if (!target.HasFlag(closureState))
            {
                Error?.Invoke(this, new DeviceErrorEventArgs("Contact closure was not in the required state of \"" + target + "\"",
                    null, DeviceErrorStatus.ErrorAffectsAllColumns, this, "Read State Not Matched", DeviceEventLoggingType.Error));
            }

            return closureState;
        }

        public override string ToString()
        {
            return m_name;
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
        public DeviceErrorStatus ErrorType
        {
            get;
            set;
        }
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
            return new List<string>() { "Read State Not Matched", "Read Failure" };
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
