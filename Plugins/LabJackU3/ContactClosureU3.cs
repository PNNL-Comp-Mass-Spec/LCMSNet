using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using FluidicsSDK.Devices;
using LcmsNetSDK;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Logging;
using LcmsNetSDK.Method;

namespace LcmsNetPlugins.LabJackU3
{
    [Serializable]
    [DeviceControl(typeof(ContactClosureU3ViewModel),
                                 "Contact Closure U3",
                                 "Contact Closures")]
    public class ContactClosureU3 : IDevice, IContactClosure
    {
         #region Members
        /// <summary>
        /// The labjack used for signalling the pulse
        /// </summary>
        private readonly LabjackU3 m_labjack;
        /// <summary>
        /// The port on the labjack on which to apply the voltage.
        /// </summary>
        private LabjackU3OutputPorts m_port;
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
        private const double CONST_ANALOGHIGH = 5.0;
        private const double CONST_DIGITALHIGH = 1.0;
        private const double CONST_LOW = 0;
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
#pragma warning disable CS0067
        public event EventHandler<DeviceErrorEventArgs> Error;
#pragma warning restore CS0067
        /// <summary>
        /// Fired when a property changes in the device.
        /// </summary>
        public event EventHandler DeviceSaveRequired;
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor--no labjack assigned!
        /// </summary>
        public ContactClosureU3()
        {
            m_labjack = new LabjackU3();
            m_port    = LabjackU3OutputPorts.DAC1Analog;
            m_name = "Contact Closure";
        }

        /// <summary>
        /// Constructor which assigns a labjack
        /// </summary>
        /// <param name="lj">The labjack</param>
        public ContactClosureU3(LabjackU3 lj)
        {
            m_labjack = lj;
            m_port    = LabjackU3OutputPorts.DAC1Analog;
            m_name = "Contact Closure";
        }

        /// <summary>
        /// Constructor which assigns a port
        /// </summary>
        /// <param name="newPort">The port on the labjack to use for the pulse</param>
        public ContactClosureU3(LabjackU3OutputPorts newPort)
        {
            m_labjack = new LabjackU3();
            m_port    = newPort;
            m_name = "Contact Closure";
        }

        /// <summary>
        /// Constructor which assigns a labjack and a port
        /// </summary>
        /// <param name="lj">The labjack</param>
        /// <param name="newPort">The port on the labjack to use for the pulse</param>
        public ContactClosureU3(LabjackU3 lj, LabjackU3OutputPorts newPort)
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
        //[PersistenceDataAttribute("Emulated")]
        public bool Emulation
        {
            get => m_emulation;
            set => m_emulation = value;
        }

        /// <summary>
        /// Gets or sets the current status of the device.
        /// </summary>
        public DeviceStatus Status
        {
            get => m_status;
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
            get => m_name;
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
            get => m_version;
            set
            {
                m_version = value;
                OnDeviceSaveRequired();
            }
        }

        /// <summary>
        /// Gets or sets the port on the labjack used for the pulse. Defaults to AO0.
        /// </summary>
        [DeviceSavedSetting("Port")]
        public LabjackU3OutputPorts Port
        {
            get => m_port;
            set
            {
                m_port = value;
                OnDeviceSaveRequired();
            }
        }

        [DeviceSavedSetting("Labjack ID")]
        public int LabJackID
        {
            get => m_labjack.LocalID;
            set => m_labjack.LocalID = value;
        }

        #endregion

        #region Methods

        //Initialize/Shutdown don't really apply
        //Maybe confirm that we can communicate to the labjack? I don't know.
        public bool Initialize(ref string errorMessage)
        {
            if(m_emulation)
            {
                return true;
            }

            //Get the version info
            try
            {
                m_labjack.Initialize();
            }
            catch(Exception ex)
            {
                Status       = DeviceStatus.Error;
                errorMessage = "Could not create a labjack object. Is one connected?";
                ApplicationLogger.LogError(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "Unable to create LabJack U3 object. Exception: " + ex.Message);
                return false;
            }
            Version = m_labjack.GetDriverVersion().ToString(CultureInfo.InvariantCulture);
            m_labjack.GetFirmwareVersion();

            //If we got anything, call it good
            if (m_labjack.FirmwareVersion.ToString(CultureInfo.InvariantCulture).Length > 0 &&
                m_labjack.DriverVersion.ToString(CultureInfo.InvariantCulture).Length > 0)
            {
                Status = DeviceStatus.Initialized;
                return true;
            }

            Status = DeviceStatus.Error;
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
        /// Triggers a pulse of the specified voltage, lasting the specified duration.
        /// This is intended for use on the analog output ports--if it is a digital
        /// port the specified voltage will be disregarded.
        /// </summary>
        /// <param name="pulseLengthSeconds">The length of the pulse in seconds</param>
        /// <param name="port"></param>
        /// <param name="voltage">The voltage to set</param>
        [LCMethodEvent("Trigger With Voltage Port", MethodOperationTimeoutType.Parameter)]
        public int Trigger(double pulseLengthSeconds, LabjackU3OutputPorts port, double voltage)
        {
            if (m_emulation)
            {
                return 0;
            }

            var error = 0;
            try
            {
                if (port.ToString().EndsWith("Analog"))
                {
                    m_labjack.Write(port, voltage);
                }
                else
                {
                    m_labjack.Write(port, CONST_DIGITALHIGH);
                }
            }
            catch (LabjackU3Exception)
            {
                throw;
            }

            var timer = new TimerDevice();
            timer.WaitSeconds(pulseLengthSeconds);

            try
            {
                m_labjack.Write(port, CONST_LOW);
            }
            catch (LabjackU3Exception)
            {
                throw;
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

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
