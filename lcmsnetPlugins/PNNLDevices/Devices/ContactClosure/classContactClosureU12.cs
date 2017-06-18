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
using LcmsNetDataClasses.Method;
using LcmsNetDataClasses.Devices;
using FluidicsSDK.Devices;
using LcmsNetSDK;

namespace LcmsNet.Devices.ContactClosure
{
    [Serializable]
    [classDeviceControlAttribute(typeof(controlContactClosureU12),
                                 "Contact Closure U12",
                                 "Contact Closures")
    ]
    public class classContactClosureU12 : IDevice, IContactClosure
    {
        #region Members
        /// <summary>
        /// The labjack used for signalling the pulse
        /// </summary>
        private readonly classLabjackU12 m_labjack;
        /// <summary>
        /// The port on the labjack on which to apply the voltage.
        /// </summary>
        private enumLabjackU12OutputPorts m_port;
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
        private enumDeviceStatus m_status;
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
        public event EventHandler<classDeviceStatusEventArgs> StatusUpdate;
        /// <summary>
        /// Fired when an error occurs in the device.
        /// </summary>
        public event EventHandler<classDeviceErrorEventArgs> Error;
        /// <summary>
        /// Fired when a property changes in the device.
        /// </summary>
        public event EventHandler DeviceSaveRequired;
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor--no labjack assigned!
        /// </summary>
        public classContactClosureU12()
        {
            m_labjack = new classLabjackU12();
            m_port    = enumLabjackU12OutputPorts.AO1;
            m_name = "Contact Closure";
        }

        /// <summary>
        /// Constructor which assigns a labjack
        /// </summary>
        /// <param name="lj">The labjack</param>
        public classContactClosureU12(classLabjackU12 lj)
        {
            m_labjack = lj;
            m_port    = enumLabjackU12OutputPorts.AO1;
            m_name = "Contact Closure";
        }

        /// <summary>
        /// Constructor which assigns a port
        /// </summary>
        /// <param name="newPort">The port on the labjack to use for the pulse</param>
        public classContactClosureU12(enumLabjackU12OutputPorts newPort)
        {
            m_labjack = new classLabjackU12();
            m_port    = newPort;
            m_name = "Contact Closure";
        }

        /// <summary>
        /// Constructor which assigns a labjack and a port
        /// </summary>
        /// <param name="lj">The labjack</param>
        /// <param name="newPort">The port on the labjack to use for the pulse</param>
        public classContactClosureU12(classLabjackU12 lj, enumLabjackU12OutputPorts newPort)
        {
            m_labjack = lj;
            m_port    = newPort;
            m_name = "Contact Closure";
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
        //[classPersistenceAttribute("Emulated")]
        public bool Emulation
        {
            get
            {
                return m_emulation;
            }
            set
            {
                m_emulation = value;
            }
        }
        /// <summary>
        /// Gets or sets the current status of the device.
        /// </summary>
        public enumDeviceStatus Status
        {
            get
            {
                return m_status;
            }
            set
            {
                if (value != m_status)
                {
                    StatusUpdate?.Invoke(this, new classDeviceStatusEventArgs(value, "Status", this));
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
            get
            {
                return m_version;
            }
            set
            {
                m_version = value;
                OnDeviceSaveRequired();
            }
        }
        /// <summary>
        /// Gets or sets the port on the labjack used for the pulse. Defaults to AO0.
        /// </summary>
        [classPersistenceAttribute("Port")]
        public enumLabjackU12OutputPorts Port
        {
            get
            {
                return m_port;
            }
            set
            {
                m_port = value;
                OnDeviceSaveRequired();
            }
        }
        [classPersistenceAttribute("Labjack ID")]
        public int LabJackID
        {
            get
            {
                return m_labjack.LocalID;
            }
            set
            {
                m_labjack.LocalID = value;
            }
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

        /// <summary>
        /// Triggers a 5V pulse of the specified length.
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="pulseLengthSeconds">The length of the pulse in seconds</param>
        [classLCMethodAttribute("Trigger", enumMethodOperationTime.Parameter, "", -1, false)]
        public int Trigger(double timeout, double pulseLengthSeconds)
        {
            return Trigger(timeout, m_port, pulseLengthSeconds);
        }
        /// <summary>
        /// Triggers a 5V pulse of the specified length.
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="port"></param>
        /// <param name="pulseLengthSeconds">The length of the pulse in seconds</param>
        [classLCMethodAttribute("Trigger Port", enumMethodOperationTime.Parameter, "", -1, false)]
        public int Trigger(double timeout, enumLabjackU12OutputPorts port, double pulseLengthSeconds)
        {
            if (m_emulation)
            {
                return 0;
            }

            var tempPortName = Enum.GetName(typeof(enumLabjackU12OutputPorts), m_port).ToString();

            var error = 0;

            try
            {
                if (tempPortName[0] == CONST_ANALOGPREFIX)
                {
                    m_labjack.Write(port, CONST_ANALOGHIGH);
                }
                else
                {
                    m_labjack.Write(port, CONST_DIGITALHIGH);
                }
            }
            catch (classLabjackU12Exception ex)
            {
                Error?.Invoke(this, new classDeviceErrorEventArgs("Could not start the trigger.",
                                     ex,
                                     enumDeviceErrorStatus.ErrorAffectsAllColumns,
                                     this));
                throw new Exception("Could not trigger the contact closure on write.  " + ex.Message, ex);
            }

            var timer = new classTimerDevice();
            if (AbortEvent != null)
            {
                timer.AbortEvent = AbortEvent;
            }
            timer.WaitSeconds(pulseLengthSeconds);

            try
            {
                m_labjack.Write(port, CONST_ANALOGLOW);
            }
            catch (classLabjackU12Exception ex)
            {
                Error?.Invoke(this,
                    new classDeviceErrorEventArgs("Could not stop the trigger.",
                                             ex,
                                             enumDeviceErrorStatus.ErrorAffectsAllColumns,
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
        [classLCMethodAttribute("Trigger With Voltage", enumMethodOperationTime.Parameter, "", -1, false)]
        public int Trigger(int pulseLengthSeconds, double voltage)
        {
            return Trigger(pulseLengthSeconds, m_port, voltage);
        }
        /// <summary>
        /// Triggers a pulse of the specified voltage, lasting the specified duration.
        /// This is intended for use on the analog output ports--if it is a digital
        /// port the specified voltage will be disregarded.
        /// </summary>
        /// <param name="pulseLengthSeconds">The length of the pulse in seconds</param>
        /// <param name="voltage">The voltage to set</param>
        [classLCMethodAttribute("Trigger With Voltage Port", enumMethodOperationTime.Parameter, "", -1, false)]
        public int Trigger(int pulseLengthSeconds, enumLabjackU12OutputPorts port, double voltage)
        {
            if (m_emulation)
            {
                return 0;
            }

            var tempPortName = Enum.GetName(typeof(enumLabjackU12OutputPorts), m_port).ToString();
            var error = 0;
            try
            {
                if (tempPortName[0] == CONST_ANALOGPREFIX)
                {
                    m_labjack.Write(port, voltage);
                }
                else
                {
                    m_labjack.Write(port, CONST_DIGITALHIGH);
                }
            }
            catch (classLabjackU12Exception ex)
            {
                throw ex;
            }

            var timer = new LcmsNetDataClasses.Devices.classTimerDevice();
            timer.WaitSeconds(pulseLengthSeconds);

            try
            {
                m_labjack.Write(port, CONST_ANALOGLOW);
            }
            catch (classLabjackU12Exception ex)
            {
                throw ex;
            }

            return error;
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
        public enumDeviceErrorStatus ErrorType
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the device type.
        /// </summary>
        public enumDeviceType DeviceType => enumDeviceType.Component;

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
