using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.ComponentModel;
using LcmsNetDataClasses.Devices;
using FluidicsSDK.Base;
using FluidicsSDK.Devices;
using LcmsNetSDK;

namespace LcmsNet.Devices.Valves
{

    /// <summary>
    /// Class used for interacting with the VICI multipositon valve
    /// </summary>
    //[Serializable]
    ////[classDeviceMonitoring(enumDeviceMonitoringType.Message, "")]
    //[classDeviceControlAttribute(typeof(controlValveVICIMultiPos),
    //                             typeof(controlValveVICIMultiPosGlyph),
    //                             "Valve Multi-Position",
    //                             "Valves")
    //]
    public class classValveVICIMultiPos : IDevice
    {
        #region Members
        /// <summary>
        /// Serial port used for communicating with the valve actuator.
        /// </summary>
        /// Settings for EMTCA-CE (Multi-pos actuator):
        ///     Baud Rate   9600
        ///     Parity      None
        ///     Stop Bits   One
        ///     Data Bits   8
        ///     Handshake   None
        private readonly SerialPort m_serialPort;
        /// <summary>
        /// The last measured position of the valve.
        /// </summary>
        private int m_lastMeasuredPosition;
        /// <summary>
        /// The last sent position to the valve.
        /// </summary>
        private int m_lastSentPosition;
        /// <summary>
        /// The valve's ID.
        /// </summary>
        private char m_valveID;
        /// <summary>
        /// The valve's version information.
        /// </summary>
        private string m_versionInfo;
        /// <summary>
        /// The valve's name
        /// </summary>
        private string m_name;
        /// <summary>
        /// Decides if valve is in emulation mode.
        /// </summary>
        private bool m_emulation;
        /// <summary>
        /// Holds the status of the device.
        /// </summary>
        private enumDeviceStatus m_status;

        private int m_numberOfPositions;

        /// <summary>
        /// How long to tell LCMSNet the SetPosition method can take. it is 6  seconds instead of 4 because we verify that
        /// the the position has change, and that 1.5 seconds+ so 4 + 1.5 rounded up = 6.
        /// </summary>
        public const int LC_EVENT_SET_POSITION_TIME_SECONDS = 6;

        //Model EMTCA-CE can take up to 3150(1161+(999*2)ms to rotate if only 4 positions are set
        //More positions reduces time it takes to rotate, but we can't know how many positions there are
        //Also, as LCEvents are timed in seconds, we round up to 4000ms to ensure that the
        //method isn't killed over 150ms + concurrency delays.
        private static readonly int m_rotationDelayTimems = 4000;
        private static readonly int m_IDChangeDelayTimems = 325;
        private const int CONST_DEAFULT_TIMEOUT = 1500;

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

        //Position change
        public event EventHandler<ValvePositionEventArgs<int>> PosChanged;
        protected virtual void OnPosChanged(int position)
        {
            PosChanged?.Invoke(this, new ValvePositionEventArgs<int>(position));
        }

        public virtual Type GetStateType()
        {
            return typeof(FifteenPositionState);
        }

        protected virtual void OnDeviceSaveRequired()
        {
            DeviceSaveRequired?.Invoke(this, null);
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public classValveVICIMultiPos(int numPositions)
        {
            //     Baud Rate   9600
            //     Parity      None
            //     Stop Bits   One
            //     Data Bits   8
            //     Handshake   None
            m_serialPort = new SerialPort
            {
                PortName = "COM1",
                BaudRate = 9600,
                ReadTimeout = CONST_DEAFULT_TIMEOUT,
                StopBits = StopBits.One,
                DataBits = 8,
                Handshake = Handshake.None,
                Parity = Parity.None,
                WriteTimeout = CONST_DEAFULT_TIMEOUT
            };

            //Set positions to unknown
            m_lastMeasuredPosition   = -1;
            m_lastSentPosition       = -1;

            //Set ID to a space (i.e. nonexistant)
            //NOTE: Spaces are ignored by the controller in sent commands
            m_valveID                = ' ';
            m_versionInfo            = "";
            m_numberOfPositions      = numPositions;

            SoftwareID                  = ' ';
            m_name                = "MPValve";
        }

        /// <summary>
        /// Constructor from a supplied serial port object.
        /// </summary>
        /// <param name="numPositions"></param>
        /// <param name="port">The serial port object to use.</param>
        public classValveVICIMultiPos(int numPositions, SerialPort port)
        {
            //Set positions to unknown
            m_lastMeasuredPosition   = -1;
            m_lastSentPosition       = -1;

            //Set ID to a space (i.e. nonexistant)
            //Note: spaces are ignored by the controller in sent commands
            m_valveID            = ' ';
            m_versionInfo        = "";
            m_numberOfPositions  = numPositions;
            m_serialPort         = port;
            m_name            = "MPValve";
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the emulation flag of the device.
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
        /// Gets or sets the status of the device.
        /// </summary>
        public enumDeviceStatus Status
        {
            get
            {
                if (Emulation)
                {
                    return enumDeviceStatus.Initialized;
                }

                return m_status;
            }
            set
            {
                if (value != m_status)
                    StatusUpdate?.Invoke(this, new classDeviceStatusEventArgs(value, "Status Changed", this));
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
        /// Gets or sets the serial port.
        /// </summary>
        public SerialPort Port => m_serialPort;

        /// <summary>
        ///
        /// </summary>
        [classPersistenceAttribute("PortName")]
        public string PortName
        {
            get
            {
                return m_serialPort.PortName;
            }
            set
            {
                m_serialPort.PortName = value;
            }
        }
        /// <summary>
        ///
        /// </summary>
        [classPersistenceAttribute("ReadTimeout")]
        public int ReadTimeout
        {
            get
            {
                return m_serialPort.ReadTimeout;
            }
            set
            {
                m_serialPort.ReadTimeout = value;
            }
        }
        /// <summary>
        ///
        /// </summary>
        [classPersistenceAttribute("WriteTimeout")]
        public int WriteTimeout
        {
            get
            {
                return m_serialPort.WriteTimeout;
            }
            set
            {
                m_serialPort.WriteTimeout = value;
            }
        }
        /// <summary>
        /// Gets the last measured position of the valve.
        /// </summary>
        public int LastMeasuredPosition => m_lastMeasuredPosition;

        /// <summary>
        /// Gets the last position sent to the valve.
        /// </summary>
        public int LastSentPosition => m_lastSentPosition;

        /// <summary>
        ///
        /// </summary>
        [classPersistenceAttribute("NumberOfPositions")]
        public int NumberOfPositions
        {
            get
            {
                return m_numberOfPositions;
            }
            set
            {
                m_numberOfPositions = value;
            }
        }
        /// <summary>
        /// Gets and sets the valve's ID in the software. DOES NOT CHANGE THE VALVE'S HARDWARE ID.
        /// </summary>
        [classPersistenceAttribute("SoftwareID")]
        public char SoftwareID
        {
            get
            {
                return m_valveID;
            }
            set
            {
                m_valveID = value;
                OnDeviceSaveRequired();
            }
        }
        /// <summary>
        /// Gets the valve's version information.
        /// </summary>
        public string Version
        {
            get
            {
                return m_versionInfo;
            }
            set
            {
                m_versionInfo = value;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Disconnect from the valve.
        /// </summary>
        /// <returns>True on success.</returns>
        public bool Shutdown()
        {
            if (m_emulation)
            {
                return m_emulation;
            }
            try
            {
                m_serialPort.Close();
            }
            catch (UnauthorizedAccessException)
            {
                throw new ValveExceptionUnauthorizedAccess();
            }
            return true;
        }
        /// <summary>
        /// Initialize the valve in the software.
        /// </summary>
        /// <returns>True on success.</returns>
        public bool Initialize(ref string errorMessage)
        {
            if (m_emulation)
            {
                //Fill in fake ID, version, position
                m_valveID = ' ';
                m_versionInfo = "Fake Version Information";
                m_lastMeasuredPosition = 0;
                return true;
            }

            //If the serial port is not open, open it
            if (!m_serialPort.IsOpen)
            {
                try
                {
                    m_serialPort.Open();
                }
                catch (UnauthorizedAccessException ex)
                {
                    errorMessage = "Could not access the COM port specifieid. " + ex.Message;
                    return false;
                }
            }
            m_serialPort.NewLine = "\r";

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

                if (Error != null)
                {

                }
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
                GetPosition();
                if (m_lastMeasuredPosition == -1)
                {
                    errorMessage = "The valve position is unknown.  Make sure it is plugged in.";
                    Error?.Invoke(this, new classDeviceErrorEventArgs(errorMessage, null, enumDeviceErrorStatus.ErrorAffectsAllColumns, this, "Valve Position"));
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
            return true;
        }
        /// <summary>
        /// Sets the position of the valve.
        /// </summary>
        /// <param name="position">The new position.</param>
        public enumValveErrors SetPosition(int position)
        {
            var newPosition = Convert.ToInt32(position);
            if (m_emulation)
            {
                m_lastSentPosition = m_lastMeasuredPosition = newPosition;
                OnPosChanged(newPosition);
                return enumValveErrors.Success;
            }
            if(position == m_lastMeasuredPosition)
            {
                return enumValveErrors.Success;
            }
            //If the serial port is not open, open it
            if (!m_serialPort.IsOpen)
            {
                try
                {
                    m_serialPort.Open();
                }
                catch (UnauthorizedAccessException)
                {
                    return enumValveErrors.UnauthorizedAccess;
                }
            }

            if (newPosition > 0 && newPosition <= m_numberOfPositions)
            {

                try
                {
                    m_lastSentPosition = newPosition;
                    m_serialPort.WriteLine(m_valveID + "GO" + newPosition);
                }

                catch (TimeoutException)
                {
                    //classApplicationLogger.LogError(0, "Could not set position.  Write timeout.");
                    return enumValveErrors.TimeoutDuringWrite;
                }
                catch (UnauthorizedAccessException)
                {
                    //classApplicationLogger.LogError(0, "Could not set position.  Could not access serial port.");
                    return enumValveErrors.UnauthorizedAccess;
                }

                //Wait m_rotationDelayTimems for valve to actually switch before proceeding
                //TODO: BLL test this instead using the abort event.
                if (AbortEvent == null)
                    AbortEvent = new System.Threading.ManualResetEvent(false);

                var waited = System.Threading.WaitHandle.WaitAll(new System.Threading.WaitHandle[] { AbortEvent }, m_rotationDelayTimems);
                if (waited)
                    return enumValveErrors.BadArgument;

                //System.Threading.Thread.Sleep(m_rotationDelayTimems);

                //Doublecheck that the position change was correctly executed
                try
                {
                    GetPosition();

                    if (StatusUpdate != null)
                    {

                    }
                }
                catch (ValveExceptionWriteTimeout)
                {
                    //classApplicationLogger.LogError(0, "Could not set position.  The write operation timed out to device.");
                    return enumValveErrors.TimeoutDuringWrite;
                }
                catch (ValveExceptionUnauthorizedAccess)
                {
                    //classApplicationLogger.LogError(0, "Could not set position. Could not access port.");
                    return enumValveErrors.UnauthorizedAccess;
                }

                if (m_lastMeasuredPosition != m_lastSentPosition)
                {
                    //classApplicationLogger.LogError(0, "Could not set position.  Valve did not move to intended position.");
                    return enumValveErrors.ValvePositionMismatch;
                }

                OnPosChanged(m_lastMeasuredPosition);
                //classApplicationLogger.LogMessage(0, Name + " changed position to: " + m_lastMeasuredPosition);
                return enumValveErrors.Success;
            }

            return enumValveErrors.BadArgument;
        }

        /// <summary>
        /// Gets the current position of the valve.
        /// </summary>
        /// <returns>The position as an enumValvePosition2Pos.</returns>
        public int GetPosition()
        {
            if (m_emulation)
            {
                return m_lastSentPosition;
            }

            //If the serial port is not open, open it
            if (!m_serialPort.IsOpen)
            {
                try
                {
                    m_serialPort.Open();
                }
                catch (UnauthorizedAccessException)
                {
                    throw new ValveExceptionUnauthorizedAccess();
                }
            }

            try
            {
                m_serialPort.DiscardInBuffer();
                m_serialPort.WriteLine(m_valveID + "CP");
                System.Threading.Thread.Sleep(200);
            }
            catch (TimeoutException)
            {
                throw new ValveExceptionWriteTimeout();
            }
            catch (UnauthorizedAccessException)
            {
                throw new ValveExceptionUnauthorizedAccess();
            }

            //Read in whatever is waiting in the buffer
            //This should look like
            //  Position is "B"
            string tempBuffer;
            try
            {
                tempBuffer    = m_serialPort.ReadExisting();
                var contains = tempBuffer.Contains("Position is =");


                var data = tempBuffer.Split(new[] {"\r"}, StringSplitOptions.RemoveEmptyEntries);
                tempBuffer = "";
                for (var i = data.Length - 1; i >= 0; i--)
                {
                    var x = data[i];
                    x = x.Replace(" ", "").ToLower();
                    if (x.Contains("positionis="))
                    {
                        tempBuffer = data[i];
                        break;
                    }
                }
            }
            catch (TimeoutException)
            {
                throw new ValveExceptionReadTimeout();
            }
            catch (UnauthorizedAccessException)
            {
                throw new ValveExceptionUnauthorizedAccess();
            }

            //
            // Grab the actual position from the above string
            //
            if (tempBuffer.Length > 1)
            {
                var positions = tempBuffer.Split('=');
                var   tempPosition = positions[positions.Length - 1];

                int position;
                if (int.TryParse(tempPosition, out position))
                {
                    if (position >= 0 && position <= NumberOfPositions)
                    {
                        m_lastMeasuredPosition = position;
                        return position;
                    }
                }
            }
            m_lastMeasuredPosition = -1;
            return -1;
        }
        /// <summary>
        /// Gets the version (date) of the valve.
        /// </summary>
        /// <returns>A string containing the version.</returns>
        public string GetVersion()
        {
            if (m_emulation)
            {
                return "3.1337";
            }

            //If the serial port is not open, open it
            if (!m_serialPort.IsOpen)
            {
                try
                {
                    m_serialPort.Open();
                }
                catch (UnauthorizedAccessException)
                {
                    throw new ValveExceptionUnauthorizedAccess();
                }
            }

            try
            {
                m_serialPort.WriteLine(m_valveID + "VR");
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
                tempBuffer = m_serialPort.ReadLine() + " " + m_serialPort.ReadLine();
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

            m_versionInfo = tempBuffer;
            return tempBuffer;
        }
        /// <summary>
        /// Get the hardware ID of the connected valve.
        /// </summary>
        /// <returns></returns>
        public char GetHardwareID()
        {
            if (m_emulation)
            {
                return '0';
            }

            //If the serial port is not open, open it
            if (!m_serialPort.IsOpen)
            {
                try
                {
                    m_serialPort.Open();
                }
                catch (UnauthorizedAccessException)
                {
                    throw new ValveExceptionUnauthorizedAccess();
                }
            }

            try
            {
                m_serialPort.WriteLine(m_valveID + "ID");
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
                tempBuffer = m_serialPort.ReadLine();
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
            if (!tempBuffer.Contains("not used"))   //Only do this if string doesn't contain "not used"
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
        public enumValveErrors SetHardwareID(char newID)
        {
            if (m_emulation)
            {
                return enumValveErrors.Success;
            }

            //Validate the new ID
            if (newID - '0' <= 9 && newID - '0' >= 0)
            {
                //If the serial port is not open, open it
                if (!m_serialPort.IsOpen)
                {
                    try
                    {
                        m_serialPort.Open();
                    }
                    catch (UnauthorizedAccessException)
                    {
                        throw new ValveExceptionUnauthorizedAccess();
                    }
                }

                try
                {
                    m_serialPort.WriteLine(m_valveID + "ID" + newID);
                    m_valveID = newID;

                    //Wait 325ms for the command to go through

                    System.Threading.Thread.Sleep(m_IDChangeDelayTimems);
                }
                catch (TimeoutException)
                {
                    return enumValveErrors.TimeoutDuringWrite;
                }
                catch (UnauthorizedAccessException)
                {
                    return enumValveErrors.UnauthorizedAccess;
                }
                OnDeviceSaveRequired();
                return enumValveErrors.Success;
            }

            return enumValveErrors.BadArgument;

        }

        /// <summary>
        /// Clears the hardware ID.
        /// </summary>
        public enumValveErrors ClearHardwareID()
        {
            if (m_emulation)
            {
                return enumValveErrors.Success;
            }

            try
            {
                m_serialPort.WriteLine(m_valveID + "ID*");
                m_valveID = ' ';

                //Wait 325ms for the command to go through

                System.Threading.Thread.Sleep(m_IDChangeDelayTimems);
            }
            catch (TimeoutException)
            {
                return enumValveErrors.TimeoutDuringWrite;
            }
            catch (UnauthorizedAccessException)
            {
                return enumValveErrors.UnauthorizedAccess;
            }

            return enumValveErrors.Success;
        }
        /// <summary>
        /// Sets the number of positions the device should use.
        /// </summary>
        /// <param name="numPositions"></param>
        /// <returns></returns>
        public enumValveErrors SetNumberOfPositions(int numPositions)
        {
            if (m_emulation)
            {
                m_numberOfPositions = numPositions;
                return enumValveErrors.Success;
            }

            try
            {
                m_serialPort.WriteLine(m_valveID + "NP" + numPositions);
                m_numberOfPositions = numPositions;

                //Wait 325ms for the command to go through

                System.Threading.Thread.Sleep(m_IDChangeDelayTimems);
            }
            catch (TimeoutException)
            {
                return enumValveErrors.TimeoutDuringWrite;
            }
            catch (UnauthorizedAccessException)
            {
                return enumValveErrors.UnauthorizedAccess;
            }

            return enumValveErrors.Success;
        }
        /// <summary>
        /// Gets the number of positions from the device.
        /// </summary>
        /// <returns></returns>
        public int GetNumberOfPositions()
        {
            if (m_emulation)
            {
                return m_numberOfPositions;
            }

            var tempNumPositions = -1;

            //If the serial port is not open, open it
            if (!m_serialPort.IsOpen)
            {
                try
                {
                    m_serialPort.Open();
                }
                catch (UnauthorizedAccessException)
                {
                    throw new ValveExceptionUnauthorizedAccess();
                }
            }

            try
            {
                m_serialPort.WriteLine(m_valveID + "NP");
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

            try
            {
                tempBuffer = m_serialPort.ReadLine();
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
            //  NP = 5
            //Grab the actual #positions from the above string
            if (tempBuffer.Length > 1)  //Make sure we have content in the string
            {
                //Find the first =
                var tempCharIndex = tempBuffer.IndexOf("=", StringComparison.Ordinal);
                if (tempCharIndex >= 0)  //Make sure we found a =
                {
                    //Change the position to be the second character following the first =
                    //TODO: Do we ever have more than 9 positions? Do we need 2 digits?
                    tempNumPositions = Convert.ToInt32(tempBuffer.Substring(tempCharIndex + 2, 1).ToCharArray()[0]);
                }
            }

            m_numberOfPositions = tempNumPositions;
            return tempNumPositions;
        }
        #endregion

        #region Method Editor Enabled Methods
        #endregion

        #region IDevice Members
        /// <summary>
        /// Gets or sets the abort event for scheduling.
        /// </summary>
        public System.Threading.ManualResetEvent AbortEvent { get; set; }
        public enumDeviceType DeviceType => enumDeviceType.Component;

        public void RegisterDataProvider(string key, DelegateDeviceHasData remoteMethod)
        {
        }
        public void UnRegisterDataProvider(string key, DelegateDeviceHasData remoteMethod)
        {

        }
        public void WritePerformanceData(string directoryPath, string name, object[] parameters)
        {

        }
        public enumDeviceErrorStatus ErrorType
        {
            get;
            set;
        }
        public List<string> GetStatusNotificationList()
        {
          return new List<string>() { "Status Changed" };
        }
        public List<string> GetErrorNotificationList()
        {
          return new List<string>() { "Valve Position" };
        }
        #endregion

        /// <summary>
        /// Returns the name of the device.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return m_name;
        }

        /*
        /// <summary>
        /// Writes health information to the data file.
        /// </summary>
        /// <param name="writer"></param>
        public FinchComponentData GetData()
        {
            FinchComponentData component = new FinchComponentData();
            component.Status    = Status.ToString();
            component.Name      = Name;
            component.Type      = "Multi-Position Valve";
            component.LastUpdate = DateTime.Now;

            FinchScalarSignal measurementSentPosition = new FinchScalarSignal();
            measurementSentPosition.Name        = "Set Position";
            measurementSentPosition.Type        = FinchDataType.Integer;
            measurementSentPosition.Units       = "";
            measurementSentPosition.Value       = this.m_lastSentPosition.ToString();
            component.Signals.Add(measurementSentPosition);

            FinchScalarSignal measurementMeasuredPosition = new FinchScalarSignal();
            measurementMeasuredPosition.Name        = "Measured Position";
            measurementMeasuredPosition.Type        = FinchDataType.Integer;
            measurementMeasuredPosition.Units       = "";
            measurementMeasuredPosition.Value       = this.m_lastMeasuredPosition.ToString();
            component.Signals.Add(measurementMeasuredPosition);

            FinchScalarSignal port = new FinchScalarSignal();
            port.Name           = "Port";
            port.Type           =  FinchDataType.String;
            port.Units          = "";
            port.Value          = this.PortName.ToString();
            component.Signals.Add(port);

            return component;
        }*/
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
