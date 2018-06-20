using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using FluidicsSDK.Base;
using LcmsNetSDK;
using LcmsNetSDK.Devices;

namespace LcmsNetPlugins.VICI.Valves
{
    public abstract class ValveVICIBase : IDevice
    {
        #region Members

        /// <summary>
        /// The valve's ID.
        /// </summary>
        private char m_valveID;

        /// <summary>
        /// The valve's name
        /// </summary>
        private string m_name;

        /// <summary>
        /// Holds the status of the device.
        /// </summary>
        private DeviceStatus m_status;

        protected static readonly int IDChangeDelayTimems = 325;  //milliseconds

        #endregion

        #region Events

        /// <summary>
        /// Fired when a property changes in the device.
        /// </summary>
        public event EventHandler DeviceSaveRequired;

        /// <summary>
        /// Fired when the status changes.
        /// </summary>
        public event EventHandler<DeviceStatusEventArgs> StatusUpdate;

        /// <summary>
        /// Fired when an error occurs in the device.
        /// </summary>
        public event EventHandler<DeviceErrorEventArgs> Error;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="readTimeout">serial port read timeout</param>
        /// <param name="writeTimeout">serial port write timeout</param>
        /// <param name="defaultName">default device name</param>
        protected ValveVICIBase(int readTimeout, int writeTimeout, string defaultName)
        {
            //     Baud Rate   9600
            //     Parity      None
            //     Stop Bits   One
            //     Data Bits   8
            //     Handshake   None
            Port = new SerialPort
            {
                PortName = "COM1",
                BaudRate = 9600,
                StopBits = StopBits.One,
                DataBits = 8,
                Handshake = Handshake.None,
                Parity = Parity.None,
                ReadTimeout = readTimeout,
                WriteTimeout = writeTimeout
            };

            //
            // Set ID to a space (i.e. nonexistent)
            // NOTE: Spaces are ignored by the controller in sent commands
            //
            m_valveID = ' ';
            Version = "";

            m_name = defaultName;
        }

        /// <summary>
        /// Constructor from a supplied serial port object.
        /// </summary>
        /// <param name="port">The serial port object to use.</param>
        /// <param name="defaultName">default device name</param>
        protected ValveVICIBase(SerialPort port, string defaultName)
        {
            //Set positions to unknown

            //Set ID to a space (i.e. nonexistent)
            //Note: spaces are ignored by the controller in sent commands
            m_valveID = ' ';
            Version = "";

            Port = port;
            //m_name        = classDeviceManager.Manager.CreateUniqueDeviceName("valve");
            m_name = defaultName;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets whether the device is in emulation mode or not.
        /// </summary>
        //[PersistenceDataAttribute("Emulated")]
        public bool Emulation { get; set; }

        /// <summary>
        /// Gets or sets the status of the device
        /// </summary>
        public DeviceStatus Status
        {
            get
            {
                if (Emulation)
                {
                    return DeviceStatus.Initialized;
                }

                return m_status;
            }
            set
            {
                if (value != m_status)
                    StatusUpdate?.Invoke(this, new DeviceStatusEventArgs(value, "Status Changed", this));
                m_status = value;
            }
        }

        /// <summary>
        /// Gets or sets the device's name
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
        /// Gets the serial port.
        /// </summary>
        public SerialPort Port { get; }

        /// <summary>
        ///
        /// </summary>
        [PersistenceData("PortName")]
        public string PortName
        {
            get { return Port.PortName; }
            set
            {
                Port.PortName = value;
                OnDeviceSaveRequired();
            }
        }

        /// <summary>
        ///
        /// </summary>
        [PersistenceData("ReadTimeout")]
        public int ReadTimeout
        {
            get { return Port.ReadTimeout; }
            set
            {
                Port.ReadTimeout = value;
                OnDeviceSaveRequired();
            }
        }

        /// <summary>
        ///
        /// </summary>
        [PersistenceData("WriteTimeout")]
        public int WriteTimeout
        {
            get { return Port.WriteTimeout; }
            set
            {
                Port.WriteTimeout = value;
                OnDeviceSaveRequired();
            }
        }

        /// <summary>
        /// Gets and sets the valve's ID in the software. DOES NOT CHANGE THE VALVE'S HARDWARE ID.
        /// </summary>
        [PersistenceData("SoftwareID")]
        public char SoftwareID
        {
            get { return m_valveID; }
            set
            {
                m_valveID = value;
                OnDeviceSaveRequired();
            }
        }

        /// <summary>
        /// Gets the valve's version information.
        /// </summary>
        public string Version { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Indicates that a save in the Fluidics designer is required
        /// </summary>
        protected virtual void OnDeviceSaveRequired()
        {
            DeviceSaveRequired?.Invoke(this, null);
        }

        protected virtual void SendError(DeviceErrorEventArgs args)
        {
            Error?.Invoke(this, args);
        }

        /// <summary>
        /// Disconnects from the valve.
        /// </summary>
        /// <returns>True on success.</returns>
        public bool Shutdown()
        {
            if (Emulation)
            {
                return Emulation;
            }
            try
            {
                Port.Close();
            }
            catch (UnauthorizedAccessException)
            {
                if (Error != null)
                {
                }
                throw new ValveExceptionUnauthorizedAccess();
            }
            return true;
        }

        /// <summary>
        /// Initialize the valve in the software.
        /// </summary>
        /// <returns>True on success.</returns>
        public virtual bool Initialize(ref string errorMessage)
        {
            if (Emulation)
            {
                //Fill in fake ID, version
                m_valveID = '1';
                Version = "Device is in emulation";
                return true;
            }

            //If the serial port is not open, open it
            if (!Port.IsOpen)
            {
                try
                {
                    Port.Open();
                }
                catch (UnauthorizedAccessException ex)
                {
                    errorMessage = "Could not access the COM Port.  " + ex.Message;
                    //throw new ValveExceptionUnauthorizedAccess();
                    return false;
                }
            }
            Port.NewLine = "\r";

            try
            {
                GetHardwareID();
            }
            catch (ValveExceptionUnauthorizedAccess ex)
            {
                errorMessage = "Could not get the hardware ID. " + ex.Message;
                return false;
            }
            catch (ValveExceptionReadTimeout ex)
            {
                errorMessage = "Reading the hardware ID timed out. " + ex.Message;
                return false;
            }
            catch (ValveExceptionWriteTimeout ex)
            {
                errorMessage = "Sending a command to get the hardware ID timed out. " + ex.Message;
                return false;
            }

            try
            {
                GetVersion();
            }
            catch (ValveExceptionUnauthorizedAccess ex)
            {
                errorMessage = "Could not get the valve version. " + ex.Message;
                return false;
            }
            catch (ValveExceptionReadTimeout ex)
            {
                errorMessage = "Reading the valve version timed out. " + ex.Message;
                return false;
            }
            catch (ValveExceptionWriteTimeout ex)
            {
                errorMessage = "Sending a command to get the valve version timed out. " + ex.Message;
                return false;
            }

            try
            {
                var position = GetPosition();
                if (position == -1)
                {
                    errorMessage = "The valve position is unknown.  Make sure it is plugged in.";
                    SendError(new DeviceErrorEventArgs(errorMessage, null, DeviceErrorStatus.ErrorAffectsAllColumns, this, "Valve Position"));
                    return false;
                }
            }
            catch (ValveExceptionUnauthorizedAccess ex)
            {
                errorMessage = "Could not get the valve position. " + ex.Message;
                return false;
            }
            catch (ValveExceptionReadTimeout ex)
            {
                errorMessage = "Reading the valve position timed out. " + ex.Message;
                return false;
            }
            catch (ValveExceptionWriteTimeout ex)
            {
                errorMessage = "Sending a command to get the valve position timed out. " + ex.Message;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gets the version (date) of the valve.
        /// </summary>
        /// <returns>A string containing the version.</returns>
        public string GetVersion()
        {
            if (Emulation)
            {
                return "3.1337";
            }

            //If the serial port is not open, open it
            if (!Port.IsOpen)
            {
                try
                {
                    Port.Open();
                }
                catch (UnauthorizedAccessException)
                {
                    throw new ValveExceptionUnauthorizedAccess();
                }
            }

            try
            {
                Port.WriteLine(m_valveID + "VR");
            }
            catch (TimeoutException)
            {
                throw new ValveExceptionWriteTimeout();
            }
            catch (UnauthorizedAccessException)
            {
                throw new ValveExceptionUnauthorizedAccess();
            }

            string tempBuffer;
            //Version info is displayed on 2 lines
            try
            {
                tempBuffer = Port.ReadLine() + " " + Port.ReadLine();
                tempBuffer = tempBuffer.Replace("\r", " "); //Readability
            }
            catch (TimeoutException)
            {
                throw new ValveExceptionReadTimeout();
            }
            catch (UnauthorizedAccessException)
            {
                throw new ValveExceptionUnauthorizedAccess();
            }

            Version = tempBuffer;
            return tempBuffer;
        }
        /// <summary>
        /// Get the hardware ID of the connected valve.
        /// </summary>
        /// <returns></returns>
        public char GetHardwareID()
        {
            if (Emulation)
            {
                return '0';
            }

            //If the serial port is not open, open it
            if (!Port.IsOpen)
            {
                try
                {
                    Port.Open();
                }
                catch (UnauthorizedAccessException)
                {
                    throw new ValveExceptionUnauthorizedAccess();
                }
            }

            try
            {
                Port.WriteLine(m_valveID + "ID");
            }
            catch (TimeoutException)
            {
                throw new ValveExceptionWriteTimeout();
            }
            catch (UnauthorizedAccessException)
            {
                throw new ValveExceptionUnauthorizedAccess();
            }

            var tempID = ' ';  //Default to blank space
            string tempBuffer;

            try
            {
                tempBuffer = Port.ReadLine();
            }
            catch (TimeoutException)
            {
                throw new ValveExceptionReadTimeout();
            }
            catch (UnauthorizedAccessException)
            {
                throw new ValveExceptionUnauthorizedAccess();
            }

            //This should look something like
            //  ID = 0
            //If there is no ID present, it will read
            //  ID = not used
            if (tempBuffer.IndexOf("not used", StringComparison.Ordinal) == -1)   //Only do this if string doesn't contain "not used"
            {
                //Grab the actual position from the above string
                if (tempBuffer.Length > 1)  //Make sure we have content in the string
                {
                    //Find the first =
                    var tempCharIndex = tempBuffer.IndexOf("=", StringComparison.Ordinal);
                    if (tempCharIndex >= 0)  //Make sure we found a =
                    {
                        //Change the position to be the second character following the first =
                        tempID = tempBuffer.Substring(tempCharIndex + 2, 1).ToCharArray()[0];
                    }
                }
            }

            //Set the valveID (software ID) to the one we just found.
            m_valveID = tempID;
            return tempID;
        }

        /// <summary>
        /// Sets the hardware ID of the connected valve.
        /// </summary>
        /// <param name="newID">The new ID, as a character 0-9.</param>
        public ValveErrors SetHardwareID(char newID)
        {
            if (Emulation)
            {
                return ValveErrors.Success;
            }

            //Validate the new ID
            if (newID - '0' <= 9 && newID - '0' >= 0)
            {
                //If the serial port is not open, open it
                if (!Port.IsOpen)
                {
                    try
                    {
                        Port.Open();
                    }
                    catch (UnauthorizedAccessException)
                    {
                        throw new ValveExceptionUnauthorizedAccess();
                    }
                }

                try
                {
                    Port.WriteLine(m_valveID + "ID" + newID);
                    m_valveID = newID;

                    //Wait 325ms for the command to go through

                    System.Threading.Thread.Sleep(IDChangeDelayTimems);
                }
                catch (TimeoutException)
                {
                    return ValveErrors.TimeoutDuringWrite;
                }
                catch (UnauthorizedAccessException)
                {
                    return ValveErrors.UnauthorizedAccess;
                }
                OnDeviceSaveRequired();
                return ValveErrors.Success;
            }

            return ValveErrors.BadArgument;
        }

        /// <summary>
        /// Clears the hardware ID.
        /// </summary>
        public ValveErrors ClearHardwareID()
        {
            if (Emulation)
            {
                return ValveErrors.Success;
            }

            try
            {
                Port.WriteLine(m_valveID + "ID*");
                m_valveID = ' ';

                //Wait 325ms for the command to go through

                System.Threading.Thread.Sleep(IDChangeDelayTimems);
            }
            catch (TimeoutException)
            {
                return ValveErrors.TimeoutDuringWrite;
            }
            catch (UnauthorizedAccessException)
            {
                return ValveErrors.UnauthorizedAccess;
            }

            return ValveErrors.Success;
        }

        /// <summary>
        /// Gets the current position of the valve.
        /// </summary>
        /// <returns>The position as an int.</returns>
        public abstract int GetPosition();

        /// <summary>
        /// Returns the name of the device.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return m_name;
        }

        #endregion

        #region IDevice Members

        /// <summary>
        /// Gets or sets the abort event for scheduling.
        /// </summary>
        public System.Threading.ManualResetEvent AbortEvent { get; set; }

        public List<string> GetStatusNotificationList()
        {
            return new List<string>() { "Status Changed" };
        }

        public List<string> GetErrorNotificationList()
        {
            return new List<string>() { "Valve Position" };
        }

        public void RegisterDataProvider(string key, DelegateDeviceHasData remoteMethod)
        {
        }
        public void UnRegisterDataProvider(string key, DelegateDeviceHasData remoteMethod)
        {
        }

        public void WritePerformanceData(string directoryPath, string name, object[] parameters)
        {
        }

        public DeviceErrorStatus ErrorType
        {
            get;
            set;
        }

        public DeviceType DeviceType => DeviceType.Component;

        #endregion

        /*public abstract FinchComponentData GetData();*/

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
