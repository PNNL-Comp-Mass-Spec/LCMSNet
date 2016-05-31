using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsFormsApplication2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

        /// <summary>
        /// Class used for interacting with the VICI 2-position valve
        /// </summary>
        public class classValveVICI2Pos
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
            private enumValvePosition2Pos mobj_lastMeasuredPosition;
            /// <summary>
            /// The last sent position to the valve.
            /// </summary>
            private enumValvePosition2Pos mobj_lastSentPosition;
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
            private string mstr_name;

            #endregion
            
            #region Events

            //Position change
            public delegate void DelegatePositionChangeEventHandler(object sender, enumValvePosition2Pos p);
            public event DelegatePositionChangeEventHandler PosChanged;
            protected virtual void OnPosChanged(enumValvePosition2Pos position)
            {
                if (PosChanged != null)
                {
                    PosChanged(this, position);
                }
            }

            /*//Name change
            public delegate void DelegateDeviceNameChangeEventHandler(object sender, string newname);
            public event DelegateDeviceNameChangeEventHandler DeviceNameChanged;
            protected virtual void OnDeviceNameChanged(string newname)
            {
                if (DeviceNameChanged != null)
                {
                    DeviceNameChanged(this, newname);
                }
            }*/

            //Save required
            public delegate void DelegateDeviceSaveRequiredEventHandler(object sender);
            public event DelegateDeviceSaveRequiredEventHandler DeviceSaveRequired;
            protected virtual void OnDeviceSaveRequired()
            {
                if (DeviceSaveRequired != null)
                {
                    DeviceSaveRequired(this);
                }
            }

            #endregion

            #region Constructors
            /// <summary>
            /// Default constructor
            /// </summary>
            public classValveVICI2Pos()
            {
                //Set positions to unknown
                mobj_lastMeasuredPosition = enumValvePosition2Pos.Unknown;
                mobj_lastSentPosition = enumValvePosition2Pos.Unknown;
                //Set ID to a space (i.e. nonexistant)
                //NOTE: Spaces are ignored by the controller in sent commands
                mobj_valveID = ' ';
                mobj_versionInfo = "";
            }

            /// <summary>
            /// Constructor from a supplied serial port object.
            /// </summary>
            /// <param name="port">The serial port object to use.</param>
            public classValveVICI2Pos(System.IO.Ports.SerialPort port)
            {
                //Set positions to unknown
                mobj_lastMeasuredPosition = enumValvePosition2Pos.Unknown;
                mobj_lastSentPosition = enumValvePosition2Pos.Unknown;
                //Set ID to a space (i.e. nonexistant)
                //Note: spaces are ignored by the controller in sent commands
                mobj_valveID = ' ';
                mobj_versionInfo = "";

                mobj_serialPort = port;
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
            }
            #endregion

            #region Properties
            /// <summary>
            /// Gets or sets the device's name
            /// </summary>
            public string Name
            {
                get
                {
                    return mstr_name;
                }
                set
                {
                    //OnDeviceNameChanged(value);
                    OnDeviceSaveRequired();
                    mstr_name = value;
                }
            }
            /// <summary>
            /// Gets or sets the serial port.
            /// </summary>
            public System.IO.Ports.SerialPort Port
            {
                get
                {
                    return mobj_serialPort;
                }
                set
                {
                    mobj_serialPort = value;
                    mobj_serialPort.NewLine = "\r";
                    mobj_serialPort.ReadTimeout = 500;
                    mobj_serialPort.WriteTimeout = 500;
                    OnDeviceSaveRequired();
                }
            }
            /// <summary>
            /// Gets the last measured position of the valve.
            /// </summary>
            public enumValvePosition2Pos LastMeasuredPosition
            {
                get
                {
                    return mobj_lastMeasuredPosition;
                }
            }
            /// <summary>
            /// Gets the last position sent to the valve.
            /// </summary>
            public enumValvePosition2Pos LastSentPosition
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
            public string VersionInfo
            {
                get
                {
                    return mobj_versionInfo;
                }
            }
            #endregion

            #region Methods

            public enumValveErrors InitializeValve()
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

                mobj_serialPort.NewLine = "\r";
                mobj_serialPort.ReadTimeout = 500;
                mobj_serialPort.WriteTimeout = 500;

                try
                {
                    GetHardwareID();
                }
                catch (ValveExceptionUnauthorizedAccess)
                {
                    return enumValveErrors.UnauthorizedAccess;
                }
                catch (ValveExceptionReadTimeout)
                {
                    return enumValveErrors.TimeoutDuringRead;
                }
                catch (ValveExceptionWriteTimeout)
                {
                    return enumValveErrors.TimeoutDuringWrite;
                }

                try
                {
                    GetPosition();
                }
                catch (ValveExceptionUnauthorizedAccess)
                {
                    return enumValveErrors.UnauthorizedAccess;
                }
                catch (ValveExceptionReadTimeout)
                {
                    return enumValveErrors.TimeoutDuringRead;
                }
                catch (ValveExceptionWriteTimeout)
                {
                    return enumValveErrors.TimeoutDuringWrite;
                }

                try
                {
                    GetVersion();
                }
                catch (ValveExceptionUnauthorizedAccess)
                {
                    return enumValveErrors.UnauthorizedAccess;
                }
                catch (ValveExceptionReadTimeout)
                {
                    return enumValveErrors.TimeoutDuringRead;
                }
                catch (ValveExceptionWriteTimeout)
                {
                    return enumValveErrors.TimeoutDuringWrite;
                }

                return enumValveErrors.Success;
            }

            /// <summary>
            /// Sets the position of the valve (A or B).
            /// </summary>
            /// <param name="newPosition">The new position.</param>
            public enumValveErrors SetPosition(enumValvePosition2Pos newPosition)
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
                        return enumValveErrors.UnauthorizedAccess;
                    }
                }

                if (newPosition == enumValvePosition2Pos.A)
                {
                    try
                    {
                        mobj_lastSentPosition = enumValvePosition2Pos.A;
                        mobj_serialPort.WriteLine(mobj_valveID + "GOA");
                    }
                    catch (TimeoutException)
                    {
                        return enumValveErrors.TimeoutDuringWrite;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        return enumValveErrors.UnauthorizedAccess;
                    }

                    //Wait 145ms for valve to actually switch before proceeding
                    //NOTE: This can be shortened if there are more than 4 ports but still 
                    //      2 positions; see manual page 1 for switching times
                    //TODO: Make the 145ms a property or static variable or something
                    //      instead of being hardcoded.

                    System.Threading.Thread.Sleep(145);

                    //Doublecheck that the position change was correctly executed
                    try
                    {
                        GetPosition();
                    }
                    catch (ValveExceptionWriteTimeout)
                    {
                        return enumValveErrors.TimeoutDuringWrite;
                    }
                    catch (ValveExceptionUnauthorizedAccess)
                    {
                        return enumValveErrors.UnauthorizedAccess;
                    }

                    if (mobj_lastMeasuredPosition != mobj_lastSentPosition)
                    {
                        return enumValveErrors.ValvePositionMismatch;
                    }
                    else
                    {
                        OnPosChanged(mobj_lastMeasuredPosition);
                        return enumValveErrors.Success;
                    }
                }

                else if (newPosition == enumValvePosition2Pos.B)
                {
                    try
                    {
                        mobj_lastSentPosition = enumValvePosition2Pos.B;
                        mobj_serialPort.WriteLine(mobj_valveID + "GOB");
                    }
                    catch (TimeoutException)
                    {
                        return enumValveErrors.TimeoutDuringWrite;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        return enumValveErrors.UnauthorizedAccess;
                    }

                    //Wait 145ms for valve to actually switch before proceeding
                    //NOTE: This can be shortened if there are more than 4 ports but still 
                    //      2 positions; see manual page 1 for switching times
                    //TODO: Make the 145ms a property or static variable or something
                    //      instead of being hardcoded.

                    System.Threading.Thread.Sleep(145);

                    //Doublecheck that the position change was correctly executed
                    try
                    {
                        GetPosition();
                    }
                    catch (ValveExceptionWriteTimeout)
                    {
                        return enumValveErrors.TimeoutDuringWrite;
                    }
                    catch (ValveExceptionUnauthorizedAccess)
                    {
                        return enumValveErrors.UnauthorizedAccess;
                    }

                    if (mobj_lastMeasuredPosition != mobj_lastSentPosition)
                    {
                        return enumValveErrors.ValvePositionMismatch;
                    }
                    else
                    {
                        OnPosChanged(mobj_lastMeasuredPosition);
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
            public enumValvePosition2Pos GetPosition()
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
                    mobj_lastMeasuredPosition = enumValvePosition2Pos.A;
                    return enumValvePosition2Pos.A;
                }

                else if (tempPosition == "B")
                {
                    mobj_lastMeasuredPosition = enumValvePosition2Pos.B;
                    return enumValvePosition2Pos.B;
                }

                else
                {
                    mobj_lastMeasuredPosition = enumValvePosition2Pos.Unknown;
                    return enumValvePosition2Pos.Unknown;
                }
            }

            /// <summary>
            /// Gets the version (date) of the valve.
            /// </summary>
            /// <returns>A string containing the version.</returns>
            public string GetVersion()
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
                        //TODO: Make the 325ms a property or static variable or something
                        //      instead of being hardcoded.

                        System.Threading.Thread.Sleep(325);
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
                try
                {
                    mobj_serialPort.WriteLine(mobj_valveID + "ID*");
                    mobj_valveID = ' ';

                    //Wait 325ms for the command to go through
                    //TODO: Make the 325ms a property or static variable or something
                    //      instead of being hardcoded.

                    System.Threading.Thread.Sleep(325);
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

            #endregion

        }

        /*public class EventListener
        {
            private classValveVICI2Pos Valve;

            public EventListener(classValveVICI2Pos valve)
            {
                Valve = valve;
                Valve.PosChanged += new classValveVICI2Pos.DelegatePositionChangeEventHandler(Valve_PosChanged);
            }

            void Valve_PosChanged(object sender, enumValvePosition2Pos p)
            {
                
                System.Windows.Forms.MessageBox.Show("OH SNAP THE POSITION CHANGED TO " + p.ToString());
                //throw new NotImplementedException();
            }
        }*/

   
}

