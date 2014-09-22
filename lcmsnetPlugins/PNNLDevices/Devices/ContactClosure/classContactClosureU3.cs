using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LcmsNet.Devices;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Method;
using LcmsNetDataClasses.Devices;
using FluidicsSDK.Devices;

namespace LcmsNet.Devices.ContactClosure
{
    [Serializable]
    [classDeviceControlAttribute(typeof(controlContactClosureU3),
                                 "Contact Closure U3",
                                 "Contact Closures")
    ]
    public class classContactClosureU3 : IDevice, IContactClosure
    {
         #region Members
        /// <summary>
        /// The labjack used for signalling the pulse
        /// </summary>
        private classLabjackU3 mobj_labjack;
        /// <summary>
        /// The port on the labjack on which to apply the voltage.
        /// </summary>
        private enumLabjackU3OutputPorts mobj_port;
        /// <summary>
        /// The name, used in software for the symbol.
        /// </summary>
        private string mstring_name;
        /// <summary>
        /// The version. 
        /// </summary>
        private string mstring_version;
        /// <summary>
        /// The current status of the Labjack.
        /// </summary>
        private enumDeviceStatus menum_status;
        /// <summary>
        /// Flag indicating if the device is in emulation mode.
        /// </summary>
        private bool mbool_emulation;
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
        public classContactClosureU3()
        {
            mobj_labjack = new classLabjackU3();
            mobj_port    = enumLabjackU3OutputPorts.DAC1Analog;
            mstring_name = "Contact Closure";
        }

        /// <summary>
        /// Constructor which assigns a labjack
        /// </summary>
        /// <param name="lj">The labjack</param>
        public classContactClosureU3(classLabjackU3 lj)
        {
            mobj_labjack = lj;
            mobj_port    = enumLabjackU3OutputPorts.DAC1Analog;
            mstring_name = "Contact Closure";
        }

        /// <summary>
        /// Constructor which assigns a port
        /// </summary>
        /// <param name="newPort">The port on the labjack to use for the pulse</param>
        public classContactClosureU3(enumLabjackU3OutputPorts newPort)
        {
            mobj_labjack = new classLabjackU3();
            mobj_port    = newPort;
            mstring_name = "Contact Closure";
        }

        /// <summary>
        /// Constructor which assigns a labjack and a port
        /// </summary>
        /// <param name="lj">The labjack</param>
        /// <param name="newPort">The port on the labjack to use for the pulse</param>
        public classContactClosureU3(classLabjackU3 lj, enumLabjackU3OutputPorts newPort)
        {
            mobj_labjack = lj;
            mobj_port    = newPort;
            mstring_name = "Contact Closure";
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
                return mbool_emulation;
            }
            set
            {                
                mbool_emulation = value;
            }
        }
        /// <summary>
        /// Gets or sets the current status of the device.
        /// </summary>
        public enumDeviceStatus Status
        {
            get
            {
                return menum_status;
            }
            set
			{
                if (value != menum_status && StatusUpdate != null)
                {
                    StatusUpdate(this, new classDeviceStatusEventArgs(value, "Status", this));
                }
				menum_status = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the device in the fluidics designer.
        /// </summary>       
        public string Name
        {
            get
            {
                return mstring_name;
            }
            set
            {
                mstring_name = value;
                OnDeviceSaveRequired();
            }
        }

        /// <summary>
        /// Gets or sets the version of the Labjack/dll
        /// </summary>        
        public string Version
        {
            get
            {
                return mstring_version;
            }
            set
            {
                mstring_version = value;
                OnDeviceSaveRequired();
            }
        }
        /// <summary>
        /// Gets or sets the port on the labjack used for the pulse. Defaults to AO0.
        /// </summary>
        [classPersistenceAttribute("Port")]
        public enumLabjackU3OutputPorts Port
        {
            get
            {
                return mobj_port;
            }
            set
            {
                mobj_port = value;
                OnDeviceSaveRequired();
            }
        }
        [classPersistenceAttribute("Labjack ID")]
        public int LabJackID
        {
            get
            {
                return mobj_labjack.LocalID;
            }
            set
            {
                mobj_labjack.LocalID = value;
            }
        }
        #endregion

        #region Methods

        //Initialize/Shutdown don't really apply
        //Maybe confirm that we can communicate to the labjack? I don't know.
        public bool Initialize(ref string errorMessage)
        {
            if(mbool_emulation)
            {
                return true;
            }

            //Get the version info
            try
            {
                mobj_labjack.Initialize();
            }
            catch(Exception ex)
            {
                Status       = enumDeviceStatus.Error;
                errorMessage = "Could not create a labjack object. Is one connected?";
                LcmsNetDataClasses.Logging.classApplicationLogger.LogError(LcmsNetDataClasses.Logging.classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "Unable to create LabJack U3 object. Exception: " + ex.Message);
                return false;
            }
            Version = mobj_labjack.GetDriverVersion().ToString();
            mobj_labjack.GetFirmwareVersion();

            //If we got anything, call it good
            if (mobj_labjack.FirmwareVersion.ToString().Length > 0 && mobj_labjack.DriverVersion.ToString().Length > 0)
            {
                Status = enumDeviceStatus.Initialized;
                return true;
            }
            else
            {
                Status = enumDeviceStatus.Error;                
                errorMessage = "Could not get the firmware version or driver version information.";
                return false;
            }
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
            if (DeviceSaveRequired != null)
            {
                DeviceSaveRequired(this, null);
            }
        }

        /// <summary>
        /// Triggers a 5V pulse of the specified length.
        /// </summary>
        /// <param name="pulseLengthMS">The length of the pulse in milliseconds</param>
        [classLCMethodAttribute("Trigger", enumMethodOperationTime.Parameter, "", -1, false)]
        public int Trigger(double timeout, double pulseLengthSeconds)
        {
            return Trigger(timeout, mobj_port, pulseLengthSeconds);            
        }
        /// <summary>
        /// Triggers a 5V pulse of the specified length.
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="port"></param>
        /// <param name="pulseLengthMS">The length of the pulse in milliseconds</param>
        [classLCMethodAttribute("Trigger Port", enumMethodOperationTime.Parameter, "", -1, false)]
        public int Trigger(double timeout, enumLabjackU3OutputPorts port, double pulseLengthSeconds)
        {
            if (mbool_emulation == true)
            {
                return 0;
            }

            string tempPortName = Enum.GetName(typeof(enumLabjackU3OutputPorts), mobj_port).ToString();

            int error = 0;

            try
            {
                if (tempPortName[0] == CONST_ANALOGPREFIX)
                {
                    mobj_labjack.Write(port, CONST_ANALOGHIGH);
                }
                else
                {
                    mobj_labjack.Write(port, CONST_DIGITALHIGH);
                }
            }
            catch (classLabjackU3Exception ex)
            {
                if (Error != null)
                {
                    Error(this, new classDeviceErrorEventArgs("Could not start the trigger.",
                                                         ex,
                                                         enumDeviceErrorStatus.ErrorAffectsAllColumns,
                                                         this));
                }
                throw new Exception("Could not trigger the contact closure on write.  " + ex.Message, ex);
            }

            LcmsNetDataClasses.Devices.classTimerDevice timer = new LcmsNetDataClasses.Devices.classTimerDevice();
            if (AbortEvent != null)
            {
                timer.AbortEvent = AbortEvent;
            }
            timer.WaitSeconds(pulseLengthSeconds);

            try
            {
                mobj_labjack.Write(port, CONST_ANALOGLOW);
            }
            catch (classLabjackU3Exception ex)
            {
                if (Error != null)
                {
                    Error(this, 
                                        new classDeviceErrorEventArgs("Could not stop the trigger.",
                                                                 ex,
                                                                 enumDeviceErrorStatus.ErrorAffectsAllColumns,
                                                                 this));

                }
                error = 1;
                throw ex;
            }
            return error;
        }

        /// <summary>
        /// Triggers a pulse of the specified voltage, lasting the specified duration.
        /// This is intended for use on the analog output ports--if it is a digital 
        /// port the specified voltage will be disregarded.
        /// </summary>
        /// <param name="pulseLengthMS">The length of the pulse in milliseconds</param>
        /// <param name="voltage">The voltage to set</param>        
        [classLCMethodAttribute("Trigger With Voltage", enumMethodOperationTime.Parameter, "", -1, false)]
        public int Trigger(int pulseLengthSeconds, double voltage)
        {
            return Trigger(pulseLengthSeconds, mobj_port, voltage);
        }
        /// <summary>
        /// Triggers a pulse of the specified voltage, lasting the specified duration.
        /// This is intended for use on the analog output ports--if it is a digital 
        /// port the specified voltage will be disregarded.
        /// </summary>
        /// <param name="pulseLengthMS">The length of the pulse in milliseconds</param>
        /// <param name="voltage">The voltage to set</param>        
        [classLCMethodAttribute("Trigger With Voltage Port", enumMethodOperationTime.Parameter, "", -1, false)]
        public int Trigger(int pulseLengthSeconds, enumLabjackU3OutputPorts port, double voltage)
        {
            if (mbool_emulation == true)
            {
                return 0;
            }

            string tempPortName = Enum.GetName(typeof(enumLabjackU3OutputPorts), mobj_port).ToString();
            int error = 0;
            try
            {
                if (tempPortName.EndsWith("Analog"))
                {
                    mobj_labjack.Write(port, voltage);
                }
                else
                {
                    mobj_labjack.Write(port, CONST_DIGITALHIGH);
                }
            }
            catch (classLabjackU3Exception ex)
            {                
                throw ex;
            }

            LcmsNetDataClasses.Devices.classTimerDevice timer = new LcmsNetDataClasses.Devices.classTimerDevice();
            timer.WaitSeconds(pulseLengthSeconds);

            try
            {
                mobj_labjack.Write(port, CONST_ANALOGLOW);
            }
            catch (classLabjackU3Exception ex)
            {
                throw ex;
            }

            return error;
        }
        public override string ToString()
        {
            return mstring_name;
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
        public enumDeviceType DeviceType
        {
            get
            {
                return enumDeviceType.Component;
            }
        }
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
        }*/                
    }
}
