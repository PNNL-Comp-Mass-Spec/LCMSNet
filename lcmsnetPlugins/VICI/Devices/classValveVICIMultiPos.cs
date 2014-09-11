using System;
using System.Xml;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Collections.Generic;

using LcmsNetDataClasses;
using LcmsNetDataClasses.Method;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Logging;

namespace LcmsNet.Devices.Valves
{

    /// <summary>
    /// Class used for interacting with the VICI multipositon valve
    /// </summary>
	[Serializable]
    //[classDeviceMonitoring(enumDeviceMonitoringType.Message, "")]	
    [classDeviceControlAttribute(typeof(controlValveVICIMultiPos),
                                 typeof(controlValveVICIMultiPosGlyph),
                                 "Valve Multi-Position",
                                 "Valves")
    ]
    public class classValveVICIMultiPos : IDevice
    {
        #region Members
        /// <summary>
        /// Serial port used for communicating with the valve actuator.
        /// </summary>
        /// Settings for EHCA-CE (2-pos actuator):
        ///     Baud Rate   9600
        ///     Parity      None
        ///     Stop Bits   One
        ///     Data Bits   8
        ///     Handshake   None        
        private SerialPort mobj_serialPort;
        /// <summary>
        /// The last measured position of the valve.
        /// </summary>
        private int mobj_lastMeasuredPosition;
        /// <summary>
        /// The last sent position to the valve.
        /// </summary>
        private int mobj_lastSentPosition;
        /// <summary>
        /// The valve's ID.
        /// </summary>
        private char mobj_valveID;
        /// <summary>
        /// The valve's version information.
        /// </summary>
        private string mobj_versionInfo;
        /// <summary>
        /// The valve's name
        /// </summary>
        private string mstring_name;
        /// <summary>
        /// Decides if valve is in emulation mode.
        /// </summary>
        private bool mbool_emulation;
        /// <summary>
        /// Holds the status of the device.
        /// </summary>
        private enumDeviceStatus menum_status;

        private int mint_numberOfPositions;

        private static int mint_rotationDelayTimems = 1500;
        private static int mint_IDChangeDelayTimems = 325;

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
        public event DelegateDevicePositionChange PosChanged;
        protected virtual void OnPosChanged(int position)
        {
            if (PosChanged != null)
            {   
                PosChanged(this, position.ToString());
            }
        }

        protected virtual void OnDeviceSaveRequired()
        {
            if (DeviceSaveRequired != null)
            {
                DeviceSaveRequired(this, null);
            }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public classValveVICIMultiPos()
        {

            ///     Baud Rate   9600
            ///     Parity      None
            ///     Stop Bits   One
            ///     Data Bits   8
            ///     Handshake   None       
            mobj_serialPort           = new System.IO.Ports.SerialPort();
            mobj_serialPort.PortName  = "COM1";
            mobj_serialPort.BaudRate  = 9600;
            mobj_serialPort.ReadTimeout = 1000;
            mobj_serialPort.StopBits  = StopBits.One;
            mobj_serialPort.DataBits  = 8;
            mobj_serialPort.Handshake = Handshake.None;
            mobj_serialPort.Parity    = Parity.None;

            //Set positions to unknown
            mobj_lastMeasuredPosition   = -1;
            mobj_lastSentPosition       = -1;

            //Set ID to a space (i.e. nonexistant)
            //NOTE: Spaces are ignored by the controller in sent commands
            mobj_valveID                = ' ';
            mobj_versionInfo            = "";
            mint_numberOfPositions      = 9;

            mstring_name                = "MPValve"; 
        }
        /// <summary>
        /// Constructor from a supplied serial port object.
        /// </summary>
        /// <param name="port">The serial port object to use.</param>
        public classValveVICIMultiPos(SerialPort port)
        {
            //Set positions to unknown
            mobj_lastMeasuredPosition   = -1;
            mobj_lastSentPosition       = -1;

            //Set ID to a space (i.e. nonexistant)
            //Note: spaces are ignored by the controller in sent commands
            mobj_valveID            = ' ';
            mobj_versionInfo        = "";
            mint_numberOfPositions  = 9;
            mobj_serialPort         = port;
            mstring_name            = "MPValve"; 
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
                return mbool_emulation;
            }
            set
            {
                mbool_emulation = value;
            }
        }
        /// <summary>
        /// Gets or sets the status of the device.
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
                    StatusUpdate(this, new classDeviceStatusEventArgs(value, "Status Changed", this));
				menum_status = value;
            }
        }
        /// <summary>
        /// Gets or sets the device's name
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
        /// Gets or sets the serial port.
        /// </summary>
        public SerialPort Port
        {
            get
            {
                return mobj_serialPort;
            }         
        }
        /// <summary>
        /// 
        /// </summary>
        [classPersistenceAttribute("PortName")]
        public string PortName
        {
            get
            {
                return mobj_serialPort.PortName;
            }
            set
            {
                mobj_serialPort.PortName = value;
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
                return mobj_serialPort.ReadTimeout;
            }
            set
            {
                mobj_serialPort.ReadTimeout = value;
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
                return mobj_serialPort.WriteTimeout;
            }
            set
            {
                mobj_serialPort.WriteTimeout = value;
            }
        }
        /// <summary>
        /// Gets the last measured position of the valve.
        /// </summary>
        public int LastMeasuredPosition
        {
            get
            {
                return mobj_lastMeasuredPosition;
            }
        }
        /// <summary>
        /// Gets the last position sent to the valve.
        /// </summary>
        public int LastSentPosition
        {
            get
            {
                return mobj_lastSentPosition;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [classPersistenceAttribute("NumberOfPositions")]
        public int NumberOfPositions
        {
            get
            {
                return mint_numberOfPositions;
            }
            set
            {
                mint_numberOfPositions = value;
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
                return mobj_valveID;
            }
            set
            {
                mobj_valveID = value;
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
                return mobj_versionInfo;
            }
            set
            {
                mobj_versionInfo = value;
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
            if (mbool_emulation == true)
            {
                return mbool_emulation;
            }
            try
            {
                mobj_serialPort.Close();
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
            if (mbool_emulation == true)
			{
				//Fill in fake ID, version, position
                mobj_valveID = '1';
                mobj_versionInfo = "Fake Version Information";
                mobj_lastMeasuredPosition = 0;
                return true;
            }

            //If the serial port is not open, open it
            if (!mobj_serialPort.IsOpen)
            {
                try
                {
                    mobj_serialPort.Open();
                }
                catch (UnauthorizedAccessException ex)
				{
					errorMessage = "Could not access the COM port specifieid. " + ex.Message;
                    return false;
                }
            }

            mobj_serialPort.NewLine = "\r";
            mobj_serialPort.ReadTimeout = 1500;
            mobj_serialPort.WriteTimeout = 1500;

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
                if (mobj_lastMeasuredPosition == -1)
                {
                    errorMessage = "The valve position is unknown.  Make sure it is plugged in.";
                    if (Error != null)
                    {
                        Error(this, new classDeviceErrorEventArgs(errorMessage, null, enumDeviceErrorStatus.ErrorAffectsAllColumns, this, "Valve Position"));
                    }
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
        /// <param name="newPosition">The new position.</param>
        [classLCMethodAttribute("Set Position", 5, true, "", -1, false)]
        public enumValveErrors SetPosition(enumValvePositionMultiPos position)
        {
            int newPosition = Convert.ToInt32(position);
            if (mbool_emulation == true)
            {
                mobj_lastSentPosition = mobj_lastMeasuredPosition = newPosition;
                OnPosChanged(mobj_lastMeasuredPosition);
                return enumValveErrors.Success;
            }

            //If the serial port is not open, open it
            if (!mobj_serialPort.IsOpen)
            {
                try
                {
                    mobj_serialPort.Open();
                }
                catch (UnauthorizedAccessException)
                {
                    return enumValveErrors.UnauthorizedAccess;
                }
            }

            if (newPosition >= 0 && newPosition <= mint_numberOfPositions)
            {

                try
                {
                    mobj_lastSentPosition = newPosition;
                    mobj_serialPort.WriteLine(mobj_valveID + "GO" + newPosition);
                }

                catch (TimeoutException)
                {
                    classApplicationLogger.LogError(0, "Could not set position.  Write timeout.");
                    return enumValveErrors.TimeoutDuringWrite;
                }
                catch (UnauthorizedAccessException)
                {
                    classApplicationLogger.LogError(0, "Could not set position.  Could not access serial port.");
                    return enumValveErrors.UnauthorizedAccess;
                }

                //Wait 145ms for valve to actually switch before proceeding
                //NOTE: This can be shortened if there are more than 4 ports but still 
                //      2 positions; see manual page 1 for switching times
                
                //TODO: BLL test this instead using the abort event.
                if (AbortEvent == null)
                    AbortEvent = new System.Threading.ManualResetEvent(false);

                bool waited = System.Threading.WaitHandle.WaitAll(new System.Threading.WaitHandle[] { AbortEvent }, mint_rotationDelayTimems);
                if (waited == true)
                    return enumValveErrors.BadArgument;
                
                //System.Threading.Thread.Sleep(mint_rotationDelayTimems);

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
                    classApplicationLogger.LogError(0, "Could not set position.  The write operation timed out to device.");
                    return enumValveErrors.TimeoutDuringWrite;
                }
                catch (ValveExceptionUnauthorizedAccess)
                {
                    classApplicationLogger.LogError(0, "Could not set position. Could not access port.");
                    return enumValveErrors.UnauthorizedAccess;
                }

                if (mobj_lastMeasuredPosition != mobj_lastSentPosition)
                {
                    classApplicationLogger.LogError(0, "Could not set position.  Valve did not move to intended position."); 
                    return enumValveErrors.ValvePositionMismatch;
                }
                else
                {
                    OnPosChanged(mobj_lastMeasuredPosition);
                    classApplicationLogger.LogMessage(0, "Changed position of multi-valve."); 
                    return enumValveErrors.Success;
                }
            }
            else
            {
                return enumValveErrors.BadArgument;
            }           
        }
        /// <summary>
        /// Gets the current position of the valve.
        /// </summary>
        /// <returns>The position as an enumValvePosition2Pos.</returns>
        public int GetPosition()
        {
            if (mbool_emulation == true)
            {
                return mobj_lastSentPosition;
            }

            //If the serial port is not open, open it
            if (!mobj_serialPort.IsOpen)
            {
                try
                {
                    mobj_serialPort.Open();
                }
                catch (UnauthorizedAccessException)
                {
                    throw new ValveExceptionUnauthorizedAccess();
                }
            }

            try
            {
                mobj_serialPort.DiscardInBuffer();
                mobj_serialPort.WriteLine(mobj_valveID + "CP");
                System.Threading.Thread.Sleep(50);
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
            string tempBuffer = "";
            try
            {
                tempBuffer    = mobj_serialPort.ReadExisting();
                bool contains = tempBuffer.Contains("Position is =");

                
                string [] data = tempBuffer.Split(new string [] {"\r"}, StringSplitOptions.RemoveEmptyEntries);
                tempBuffer = "";
                for (int i = data.Length - 1; i >= 0; i--)
                {
                    string x = data[i];
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
            
            /// 
            /// Grab the actual position from the above string
            /// 
            if (tempBuffer.Length > 1)  
            {
                string[] positions = tempBuffer.Split('=');
                string   tempPosition = positions[positions.Length - 1];

                int position = -1;
                if (int.TryParse(tempPosition, out position) == true)
                {              
                    if (position >= 0 && position <= NumberOfPositions)
                    {
                        mobj_lastMeasuredPosition = position;
                        return position;
                    }
                }                                 
            }
            mobj_lastMeasuredPosition = -1;
            return -1;
        }
        /// <summary>
        /// Gets the version (date) of the valve.
        /// </summary>
        /// <returns>A string containing the version.</returns>
        public string GetVersion()
        {
            if (mbool_emulation == true)
            {
                return "3.1337";
            }

            //If the serial port is not open, open it
            if (!mobj_serialPort.IsOpen)
            {
                try
                {
                    mobj_serialPort.Open();
                }
                catch (UnauthorizedAccessException)
                {
                    throw new ValveExceptionUnauthorizedAccess();
                }
            }

            try
            {
                mobj_serialPort.WriteLine(mobj_valveID + "VR");
            }
            catch (TimeoutException)
            {
                throw new ValveExceptionWriteTimeout();
            }
            catch (UnauthorizedAccessException)
            {
                throw new ValveExceptionUnauthorizedAccess();
            }

            string tempBuffer = "";
            //Version info is displayed on 2 lines
            try
            {
                tempBuffer = mobj_serialPort.ReadLine() + " " + mobj_serialPort.ReadLine();
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

            mobj_versionInfo = tempBuffer;
            return tempBuffer;
        }
        /// <summary>
        /// Get the hardware ID of the connected valve.
        /// </summary>
        /// <returns></returns>
        public char GetHardwareID()
        {
            if (mbool_emulation == true)
            {
                return '0';
            }

            //If the serial port is not open, open it
            if (!mobj_serialPort.IsOpen)
            {
                try
                {
                    mobj_serialPort.Open();
                }
                catch (UnauthorizedAccessException)
                {
                    throw new ValveExceptionUnauthorizedAccess();
                }
            }

            try
            {
                mobj_serialPort.WriteLine(mobj_valveID + "ID");
            }
            catch (TimeoutException)
            {
                throw new ValveExceptionWriteTimeout();
            }
            catch (UnauthorizedAccessException)
            {
                throw new ValveExceptionUnauthorizedAccess();
            }

            char tempID = ' ';  //Default to blank space
            string tempBuffer = "";

            try
            {
                tempBuffer = mobj_serialPort.ReadLine();
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
            if (tempBuffer.IndexOf("not used") == -1)   //Only do this if string doesn't contain "not used"
            {
                //Grab the actual position from the above string
                if (tempBuffer.Length > 1)  //Make sure we have content in the string
                {
                    int tempCharIndex = tempBuffer.IndexOf("=");   //Find the first =
                    if (tempCharIndex >= 0)  //Make sure we found a =
                    {
                        //Change the position to be the second character following the first =
                        tempID = tempBuffer.Substring(tempCharIndex + 2, 1).ToCharArray()[0];
                    }
                }
            }

            //Set the valveID (software ID) to the one we just found.
            mobj_valveID = tempID;
            return tempID;
        }
        /// <summary>
        /// Sets the hardware ID of the connected valve.
        /// </summary>
        /// <param name="newID">The new ID, as a character 0-9.</param>
        public enumValveErrors SetHardwareID(char newID)
        {
            if (mbool_emulation == true)
            {
                return enumValveErrors.Success;
            }

            //Validate the new ID
            if (newID - '0' <= 9 && newID - '0' >= 0)
            {
                //If the serial port is not open, open it
                if (!mobj_serialPort.IsOpen)
                {
                    try
                    {
                        mobj_serialPort.Open();
                    }
                    catch (UnauthorizedAccessException)
                    {
                        throw new ValveExceptionUnauthorizedAccess();
                    }
                }

                try
                {
                    mobj_serialPort.WriteLine(mobj_valveID + "ID" + newID);
                    mobj_valveID = newID;

                    //Wait 325ms for the command to go through

                    System.Threading.Thread.Sleep(mint_IDChangeDelayTimems);
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

            else
            {
                return enumValveErrors.BadArgument;
            }
        }
        /// <summary>
        /// Clears the hardware ID.
        /// </summary>
        public enumValveErrors ClearHardwareID()
        {
            if (mbool_emulation == true)
            {
                return enumValveErrors.Success;
            }

            try
            {
                mobj_serialPort.WriteLine(mobj_valveID + "ID*");
                mobj_valveID = ' ';

                //Wait 325ms for the command to go through

                System.Threading.Thread.Sleep(mint_IDChangeDelayTimems);
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
            if (mbool_emulation == true)
            {
                mint_numberOfPositions = numPositions;
                return enumValveErrors.Success;
            }

            try
            {
                mobj_serialPort.WriteLine(mobj_valveID + "NP" + numPositions);
                mint_numberOfPositions = numPositions;

                //Wait 325ms for the command to go through

                System.Threading.Thread.Sleep(mint_IDChangeDelayTimems);
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
            if (mbool_emulation == true)
            {
                return mint_numberOfPositions;
            }

            int tempNumPositions = -1;

            //If the serial port is not open, open it
            if (!mobj_serialPort.IsOpen)
            {
                try
                {
                    mobj_serialPort.Open();
                }
                catch (UnauthorizedAccessException)
                {
                    throw new ValveExceptionUnauthorizedAccess();
                }
            }

            try
            {
                mobj_serialPort.WriteLine(mobj_valveID + "NP");
            }
            catch (TimeoutException)
            {
                throw new ValveExceptionWriteTimeout();
            }
            catch (UnauthorizedAccessException)
            {
                throw new ValveExceptionUnauthorizedAccess();
            }

            string tempBuffer = "";

            try
            {
                tempBuffer = mobj_serialPort.ReadLine();
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
                int tempCharIndex = tempBuffer.IndexOf("=");   //Find the first =
                if (tempCharIndex >= 0)  //Make sure we found a =
                {
                    //Change the position to be the second character following the first =
                    //TODO: Do we ever have more than 9 positions? Do we need 2 digits?
                    tempNumPositions = Convert.ToInt32(tempBuffer.Substring(tempCharIndex + 2, 1).ToCharArray()[0]);
                }
            }

            mint_numberOfPositions = tempNumPositions;
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
        public enumDeviceType DeviceType
        {
            get
            {
                return enumDeviceType.Component;
            }
        }        
        public void RegiserDataProvider(string key, DelegateDeviceHasData remoteMethod)
        {
        }
        public void UnRegiserDataProvider(string key, DelegateDeviceHasData remoteMethod)
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
            return mstring_name;
        }
        /// <summary>
        /// Writes health information to the data file.
        /// </summary>
        /// <param name="writer"></param>
        public classMonitoringComponent GetHealthData()
        {            
            classMonitoringComponent component = new classMonitoringComponent();
            component.Status    = Status.ToString();
            component.Message   = "";
            component.Name      = Name;
            component.Type      = "Multi-Position Valve";
            component.Model     = "VICI Valco";

            classMonitoringMeasurementScalar measurementSentPosition = new classMonitoringMeasurementScalar();
            measurementSentPosition.Description = "The last position sent.";
            measurementSentPosition.Name        = "Send Timeout";
            measurementSentPosition.Type        = "int";
            measurementSentPosition.Units       = "";
            measurementSentPosition.Value       = this.mobj_lastSentPosition.ToString();
            component.MeasurementData.Add(measurementSentPosition);

            classMonitoringMeasurementScalar measurementMeasuredPosition = new classMonitoringMeasurementScalar();
            measurementMeasuredPosition.Description = "The last position sent.";
            measurementMeasuredPosition.Name        = "Send Timeout";
            measurementMeasuredPosition.Type        = "int";
            measurementMeasuredPosition.Units       = "";
            measurementMeasuredPosition.Value       = this.mobj_lastMeasuredPosition.ToString();
            component.MeasurementData.Add(measurementMeasuredPosition);
                        
            
            classMonitoringMeasurementScalar port = new classMonitoringMeasurementScalar();
            port.Description    = "The port used to communicate with.";
            port.Name           = "Port";
            port.Type           = "string";
            port.Units          = "";
            port.Value          = this.PortName.ToString();
            component.MeasurementData.Add(port);

            return component;
        }
    }
}
