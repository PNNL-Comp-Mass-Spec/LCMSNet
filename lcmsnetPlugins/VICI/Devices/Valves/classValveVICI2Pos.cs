//*********************************************************************************************************
// Written by John Ryan, Dave Clark, Brian LaMarche for the US Department of Energy 
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 08/17/2009
//
/* Last modified 08/17/2009
 *      BLL - 9-8-09
 *          Removed initialization from second constructor that takes a serial port.
 *          Added comments on properties missing them.  (e.g. describing Gets or sets properties)
 *          Additional cleanup in code making a new region for method editor enabled methods.
/*********************************************************************************************************/

using System;
using System.Collections.Generic;
using System.IO.Ports;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Method;
using FluidicsSDK.Devices;
using FluidicsSDK.Base;

namespace LcmsNet.Devices.Valves
{

    /// <summary>
    /// Class used for interacting with the VICI 2-position valve
    /// </summary>
	[Serializable]
    ////[classDeviceMonitoring(enumDeviceMonitoringType.Message, "")]	
    /*[classDeviceControlAttribute(typeof(controlValveVICI2Pos),
                                 typeof(controlValveVICI2PosGlyph),                                 
                                 "Valve 2-Position",
                                 "Valves")
    ]*/
    public class classValveVICI2Pos : IDevice
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
        private System.IO.Ports.SerialPort mobj_serialPort;
        /// <summary>
        /// The last measured position of the valve.
        /// </summary>
        private TwoPositionState mobj_lastMeasuredPosition;
        /// <summary>
        /// The last sent position to the valve.
        /// </summary>
        private TwoPositionState mobj_lastSentPosition;
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
        /// Holds the status of the device.
        /// </summary>
        private enumDeviceStatus menum_status;        
        /// <summary>
        /// Decides if valve is in emulation mode.
        /// </summary>
        private bool mbool_emulation;
        private static int mint_rotationDelayTimems = 145;  //milliseconds
        private static int mint_IDChangeDelayTimems = 325;  //milliseconds
        private const int CONST_READTIMEOUT = 500;          //milliseconds
        private const int CONST_WRITETIMEOUT = 500;         //milliseconds
        #endregion

        #region Events
        /// <summary>
        /// Indicates that the valve position has changed
        /// </summary>
        public event EventHandler<ValvePositionEventArgs<TwoPositionState>> PositionChanged;
        /// <summary>
        /// Fired when a property changes in the device.
        /// </summary>
        public event EventHandler DeviceSaveRequired;
        /// <summary>
        /// Fired when the status changes.
        /// </summary>
        public event EventHandler<classDeviceStatusEventArgs> StatusUpdate;
        /// <summary>
        /// Fired when an error occurs in the device.
        /// </summary>
        public event EventHandler<classDeviceErrorEventArgs> Error;  
        #endregion        

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public classValveVICI2Pos()
        {

            ///     Baud Rate   9600
            ///     Parity      None
            ///     Stop Bits   One
            ///     Data Bits   8
            ///     Handshake   None       
            mobj_serialPort             = new System.IO.Ports.SerialPort();
            mobj_serialPort.PortName    = "COM1";
            mobj_serialPort.BaudRate    = 9600;
            mobj_serialPort.StopBits    = StopBits.One;
            mobj_serialPort.DataBits    = 8;
            mobj_serialPort.Handshake   = Handshake.None;
            mobj_serialPort.Parity      = Parity.None;
            mobj_serialPort.ReadTimeout = CONST_READTIMEOUT;
            mobj_serialPort.WriteTimeout = CONST_WRITETIMEOUT;
            mobj_lastMeasuredPosition   = TwoPositionState.Unknown;
            mobj_lastSentPosition       = TwoPositionState.Unknown;

            /// 
            /// Set ID to a space (i.e. nonexistant)
            /// NOTE: Spaces are ignored by the controller in sent commands
            /// 
            mobj_valveID                = ' ';
            mobj_versionInfo            = "";

            mstring_name = "valve"; 
        }
        /// <summary>
        /// Constructor from a supplied serial port object.
        /// </summary>
        /// <param name="port">The serial port object to use.</param>
        public classValveVICI2Pos(SerialPort port)
        {
            //Set positions to unknown
            mobj_lastMeasuredPosition   = TwoPositionState.Unknown;
            mobj_lastSentPosition       = TwoPositionState.Unknown;

            //Set ID to a space (i.e. nonexistant)
            //Note: spaces are ignored by the controller in sent commands
            mobj_valveID        = ' ';
            mobj_versionInfo    = "";

            mobj_serialPort     = port;
            //mstring_name        = classDeviceManager.Manager.CreateUniqueDeviceName("valve");
            mstring_name = "valve";
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets whether the device is in emulation mode or not.
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
        /// Gets or sets the status of the device
        /// </summary>
        public LcmsNetDataClasses.Devices.enumDeviceStatus Status
        {
            get
            {
                if (Emulation)
                {
                    return enumDeviceStatus.Initialized;
                }
                {
                    return menum_status;
                }
            }
            set
			{
				if (value != menum_status && StatusUpdate != null)
                    StatusUpdate(this, new classDeviceStatusEventArgs(value, "StatusChange", this));
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
                OnDeviceSaveRequired();
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
                OnDeviceSaveRequired();
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
                OnDeviceSaveRequired();
            }
        }
        /// <summary>
        /// Gets the last measured position of the valve.
        /// </summary>
        public TwoPositionState LastMeasuredPosition
        {
            get
            {
                return mobj_lastMeasuredPosition;
            }
        }
        /// <summary>
        /// Gets the last position sent to the valve.
        /// </summary>
        public TwoPositionState LastSentPosition
        {
            get
            {
                return mobj_lastSentPosition;
            }
        }
        /// <summary>
        /// Gets and sets the valve's ID in the software. DOES NOT CHANGE THE VALVE'S HARDWARE ID.
        /// </summary>
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
        /// Indicates that a save in the Fluidics designer is required
        /// </summary>
        protected virtual void OnDeviceSaveRequired()
        {
            if (DeviceSaveRequired != null)
            {
                DeviceSaveRequired(this, null);
            }
        }
        /// <summary>
        /// Indicates that the device's position has changed.
        /// </summary>
        /// <param name="position">The new position</param>
        protected virtual void OnPositionChanged(TwoPositionState position)
        {
            if (PositionChanged != null)
            {
                PositionChanged(this, new ValvePositionEventArgs<TwoPositionState>(position));
            }
        }
        /// <summary>
        /// Disconnects from the valve.
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
        public bool Initialize(ref string errorMessage)
        {
            if (mbool_emulation == true)
            {
                //Fill in fake ID, version, position
                mobj_valveID                = '1';
                mobj_versionInfo            = "Device is in emulation";
                mobj_lastMeasuredPosition   = TwoPositionState.PositionA;
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
                    errorMessage = "Could not access the COM Port.  " + ex.Message;
                    //throw new ValveExceptionUnauthorizedAccess();
                    return false;
                }
            }
            //mobj_serialPort.ReadTimeout = CONST_READTIMEOUT;
            //mobj_serialPort.WriteTimeout = CONST_WRITETIMEOUT;
            mobj_serialPort.NewLine         = "\r";         

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
                GetPosition();
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
        /// Gets the current position of the valve.
        /// </summary>
        /// <returns>The position as an enumValvePosition2Pos.</returns>
        public int GetPosition()
        {
            if (mbool_emulation == true)
            {
                return (int)mobj_lastSentPosition;
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
                mobj_serialPort.WriteLine(mobj_valveID + "CP");
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


            //Make a string containing the position           
            string tempPosition = "Unknown";        //Default to unknown

            //Grab the actual position from the above string
            if (tempBuffer.Length > 1)  //Make sure we have content in the string
            {
                int tempCharIndex = tempBuffer.IndexOf("Position is \"");   //Find the "
                if (tempCharIndex >= 0)  //Make sure we found it
                {
                    //Change the position to be the character following the "
                    tempPosition = tempBuffer.Substring(tempCharIndex + 13, 1);
                }
            }

            if (tempPosition == "A")
            {
                mobj_lastMeasuredPosition = TwoPositionState.PositionA;
                return (int)TwoPositionState.PositionA;
            }

            else if (tempPosition == "B")
            {
                mobj_lastMeasuredPosition = TwoPositionState.PositionB;
                return (int)TwoPositionState.PositionB;
            }

            else
            {
                mobj_lastMeasuredPosition = TwoPositionState.Unknown;
                return (int)TwoPositionState.Unknown;
            }
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
        /// Returns the name of the device.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return mstring_name;
        }
        #endregion

        #region Method Editor Enabled Methods
//        /// <summary>
//        /// Sets the position of the valve (A or B).
//        /// </summary>
//        /// <param name="newPosition">The new position.</param>
//        [classLCMethodAttribute("Set Position", 1, true, "", -1, false)]
//        public FluidicsSDK.Base.enumValveErrors SetPosition(TwoPositionState newPosition)
//        {

//#if DEBUG
//            System.Diagnostics.Debug.WriteLine("\tSetting Position" + newPosition.ToString());
//#endif

//            if (mbool_emulation == true)
//            {
//                mobj_lastSentPosition = mobj_lastMeasuredPosition = newPosition;
//                OnPositionChanged(mobj_lastMeasuredPosition);
//                return enumValveErrors.Success;
//            }
//            // short circuit..if we're already in that position, why bother trying to move to it?
//            if (newPosition == mobj_lastMeasuredPosition)
//            {
//                return enumValveErrors.Success;
//            }
//            //If the serial port is not open, open it
//            if (!mobj_serialPort.IsOpen)
//            {
//                try
//                {
//                    mobj_serialPort.Open();
//                }
//                catch (UnauthorizedAccessException)
//                {
//                    return enumValveErrors.UnauthorizedAccess;
//                }
//            }
           
//            string cmd = null;           
//            if (newPosition == TwoPositionState.PositionA)
//            {
//                mobj_lastSentPosition = TwoPositionState.PositionA;
//                cmd = mobj_valveID.ToString() + "GOA";
//            }

//            else if (newPosition == TwoPositionState.PositionB)
//            {
//                mobj_lastSentPosition = TwoPositionState.PositionB;
//                cmd = mobj_valveID.ToString() + "GOB";
//            }
//            else
//            {
//                return enumValveErrors.BadArgument;
//            }

//            try
//            {               
//                mobj_serialPort.WriteLine(cmd);
//            }
//            catch (TimeoutException)
//            {
//                return enumValveErrors.TimeoutDuringWrite;
//            }
//            catch (UnauthorizedAccessException)
//            {
//                return enumValveErrors.UnauthorizedAccess;
//            }

//            //Wait 145ms for valve to actually switch before proceeding
//            //NOTE: This can be shortened if there are more than 4 ports but still 
//            //      2 positions; see manual page 1 for switching times

//            System.Threading.Thread.Sleep(mint_rotationDelayTimems);

//            //Doublecheck that the position change was correctly executed
//            try
//            {
//                GetPosition();
//            }
//            catch (ValveExceptionWriteTimeout)
//            {
//                return enumValveErrors.TimeoutDuringWrite;
//            }
//            catch (ValveExceptionUnauthorizedAccess)
//            {
//                return enumValveErrors.UnauthorizedAccess;
//            }

//            if (mobj_lastMeasuredPosition != mobj_lastSentPosition)
//            {
//                return enumValveErrors.ValvePositionMismatch;
//            }
//            else
//            {
//                OnPositionChanged(mobj_lastMeasuredPosition);
//                return enumValveErrors.Success;
//            }
//        }

        /// <summary>
        /// Sets the position of the valve (A or B).
        /// </summary>
        /// <param name="newPosition">The new position.</param>
        [classLCMethodAttribute("Set Position", 1, true, "", -1, false)]
        public void SetPosition(TwoPositionState newPosition)
        {

#if DEBUG
            System.Diagnostics.Debug.WriteLine("\tSetting Position" + newPosition.ToString());
#endif

            if (mbool_emulation == true)
            {
                mobj_lastSentPosition = mobj_lastMeasuredPosition = newPosition;
                OnPositionChanged(mobj_lastMeasuredPosition);
                return;
            }
            // short circuit..if we're already in that position, why bother trying to move to it?
            if (newPosition == mobj_lastMeasuredPosition)
            {
                return;
            }
            //If the serial port is not open, open it
            if (!mobj_serialPort.IsOpen)
            {
                mobj_serialPort.Open();             
            }

            string cmd = null;
            if (newPosition == TwoPositionState.PositionA)
            {
                mobj_lastSentPosition = TwoPositionState.PositionA;
                cmd = mobj_valveID.ToString() + "GOA";
            }

            else if (newPosition == TwoPositionState.PositionB)
            {
                mobj_lastSentPosition = TwoPositionState.PositionB;
                cmd = mobj_valveID.ToString() + "GOB";
            }
            else
            {
                throw new ArgumentOutOfRangeException("Invalid Position to set" + newPosition.ToCustomString());
            }
            
            mobj_serialPort.WriteLine(cmd);       

            //Wait 145ms for valve to actually switch before proceeding
            //NOTE: This can be shortened if there are more than 4 ports but still 
            //      2 positions; see manual page 1 for switching times

            System.Threading.Thread.Sleep(mint_rotationDelayTimems);

            //Doublecheck that the position change was correctly executed
            GetPosition();
            OnPositionChanged(mobj_lastMeasuredPosition);                        
        }
        #endregion

        #region IDevice Members
        /// <summary>
        /// Gets or sets the abort event for scheduling.
        /// </summary>
        public System.Threading.ManualResetEvent AbortEvent { get; set; }
		public List<string> GetStatusNotificationList()
        {
            return new List<string>() {"StatusChange"};
		}

		  public List<string> GetErrorNotificationList()
          {
              return new List<string>();
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

          public enumDeviceErrorStatus ErrorType
          {
              get;
              set;
          }

          public enumDeviceType DeviceType
          {
              get
              {
                  return enumDeviceType.Component;
              }
          }

          /*public FinchComponentData GetData()
          {
              FinchComponentData component = new FinchComponentData();
              component.Status = Status.ToString();
              component.Name = Name;
              component.Type = "Valve";
              component.LastUpdate = DateTime.Now;

              FinchScalarSignal measurementSentPosition = new FinchScalarSignal();
              measurementSentPosition.Name = "Set Position";
              measurementSentPosition.Type = FinchDataType.Integer;
              measurementSentPosition.Units = "";
              measurementSentPosition.Value = this.mobj_lastSentPosition.ToString();
              component.Signals.Add(measurementSentPosition);

              FinchScalarSignal measurementMeasuredPosition = new FinchScalarSignal();
              measurementMeasuredPosition.Name = "Measured Position";
              measurementMeasuredPosition.Type = FinchDataType.Integer;
              measurementMeasuredPosition.Units = "";
              measurementMeasuredPosition.Value = this.mobj_lastMeasuredPosition.ToString();
              component.Signals.Add(measurementMeasuredPosition);

              FinchScalarSignal port = new FinchScalarSignal();
              port.Name = "Port";
              port.Type = FinchDataType.String;
              port.Units = "";
              port.Value = this.PortName.ToString();
              component.Signals.Add(port);

              return component;
          }*/
        #endregion

        #region IDevice Data Provider Methods
   
        #endregion

       
    
    }
}
