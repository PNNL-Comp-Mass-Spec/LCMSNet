using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Method;

namespace Newport.ESP300
{
    [Serializable]
    [classDeviceControlAttribute(typeof(controlNewportStage),
                                 typeof(FluidicsStage),
                                 "Newport Stage",
                                 "Stages")
    ]
    public class classNewportStage:IDisposable, IDevice
    {
        #region Members
        private SerialPort m_port;
        private int m_numAxes;
        private bool m_AtSlowSpeed;
        private double[] m_SpeedNormal;
        private bool[] m_motorStatus;
        private bool m_disposed;
        private Dictionary<string, classStagePosition> m_positions;
        private List<string> m_reportedErrors;
        private string m_name;
        private string m_position;

        #region Constants
        private const char CONST_CMD_TERMINATOR = '\r';
        private const int CONST_MAX_AXES = 3;
        #endregion
      
        #region ESP300SERIALPORTCONFIGURATION
        // ESP300 RS232C configuration is fixed, these constants define its configuration so we can interface with it and are as specified by page 3-4 of the ESP300 manual
        private const int CONST_BAUDRATE = 19200;
        private const int CONST_DATABITS = 8;
        private const StopBits CONST_STOPBITS = StopBits.One;
        private const Parity CONST_PARITY = Parity.None;
        private const Handshake CONST_HANDSHAKE = Handshake.RequestToSend;
        #endregion 

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a stage which tries to communicate on COM1
        /// </summary>
        public classNewportStage()
            : this(null, "NoPosition", 2)
        {
        }

        /// <summary>
        /// Creates stage on specified port with specified current position and number of axes.
        /// </summary>
        /// <param name="port">a System.IO.Ports SerialPort, defaults to COM1</param>
        /// <param name="numAxes">an integer representing the number of axes of movement available to this stage</param>
        public classNewportStage(SerialPort port, string currentPos, int numAxes)
        {
            m_disposed = false;        
            if (port == null)
            {
                Port = new SerialPort("COM1", CONST_BAUDRATE, CONST_PARITY, CONST_DATABITS, CONST_STOPBITS);
                Port.Handshake = CONST_HANDSHAKE;
            }
            else
            {
                Port = port;
            }            
            NumAxes = numAxes;
            m_SpeedNormal = new double[NumAxes];
            m_positions = new Dictionary<string, classStagePosition>();     
            Name = "Stage";
            m_position = currentPos;
            m_reportedErrors = new List<string>();
            m_motorStatus = new bool[CONST_MAX_AXES];
        }

        ~classNewportStage()
        {
            Dispose();
        }

        #endregion

        #region IDisposable Interface Methods

        /// <summary>
        /// IDisposable required method
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// IDisposable pattern method
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                m_disposed = true;
                if (disposing)
                {
                    if (m_port != null)
                    {
                        ClosePort();
                        m_port.Dispose();
                    }
                }
            }
        }

        #endregion
        
        #region Methods

        #region OperationMethods

        /// <summary>
        /// Set the x,y,z coodinates of a position
        /// </summary>
        /// <param name="positionNumber">the position whose coordinates to set</param>
        /// <param name="axis1Coord">the x coordinate</param>
        /// <param name="axis2Coord">the y coordinate </param>
        /// <param name="axis3Coord">the z coordinate, currently unused, pass zero</param>
        //[classLCMethodAttribute("Define Position", 1, true, "", -1, false)]
        public void SetPositionCoordinates(string positionName, float axis1Coord, float axis2Coord, float axis3Coord)
        {
            classStagePosition position = new classStagePosition();
            position.NumAxes = 2;
            position[0] = axis1Coord;
            position[1] = axis2Coord;
            position[2] = axis3Coord;
            m_positions[positionName] = position;
        }


        /// <summary>
        /// remove a position from the list of stored positions.
        /// </summary>
        /// <param name="positionName">name of the position to remove</param>
        public void RemovePosition(string positionName)
        {
            m_positions.Remove(positionName);
        }

        /// <summary>
        /// Go to a specified 2D/3D position defined by a stage position
        /// </summary>
        /// <param name="positionIndex">the position to go to</param>
        /// <param name="block">waits for the position to be reached if true, doesn't wait if false, deprecated</param>
        [classLCMethodAttribute("Go To Predefined Position", enumMethodOperationTime.Parameter, "POSNAMES", 1,false)]
        public void GoToPosition(double timeout, string positionName) // bool block = true)
        {
            if(Emulation)
            {
                return;
            }
            // newport stage indexes axes starting from 1, so we go from 1 to numAxes +1
            for(int i = 1; i < m_numAxes + 1; i++)
            {
                try
                {
                
                    MoveToPosition(i, Convert.ToSingle(m_positions[positionName][i - 1]));
                }
                catch (Exception ex)
                {
                    throw new ESP300Exception("Failure to move to position.", ex);
                }
            }
            //if (block)
            //{
            //    for (int i = 1; i < m_numAxes + 1; i++)
            //    {
            //        while (!MotionDone(i))
            //        {
            //            Thread.Sleep(10);
            //        }
            //    }
            //}
            m_position = positionName;
            if (PositionChanged != null)
            {
                PositionChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// Query position of the specified axis
        /// </summary>
        /// <param name="axis">integer representing the axis to query</param>
        /// <returns>double representing position of the axis</returns>
        //[classLCMethodAttribute("Query Axis Position", 1, true, "", -1, false)]
        public double QueryPosition(int axis)
        {           
            string response;
            StringBuilder cmd = new StringBuilder();
            cmd.Append(axis.ToString() + "TP" + CONST_CMD_TERMINATOR);
            
            if (!Emulation)
            {
                if (ValidateAxis(axis))
                {
                    response = WriteCommand(cmd.ToString(), true);
                    try
                    {
                        return Convert.ToDouble(response);
                    }
                    catch
                    {
                        ErrorType = enumDeviceErrorStatus.ErrorAffectsAllColumns;
                        if (Error != null)
                        {
                            LcmsNetDataClasses.Logging.classApplicationLogger.LogError(0, "Unexpected response when querying stage position");
                            Error(this, new classDeviceErrorEventArgs("Unexpected response", null, enumDeviceErrorStatus.ErrorAffectsAllColumns, this));
                        }
                        return 0.0;
                    }
                }
                return 0.0;
            }
            else
            {
                return 0.0;
            }
        }

        /// <summary>
        ///  Move specified axis to desired position
        /// </summary>
        /// <param name="axis">integer representing the axis to move</param>
        /// <param name="position">double indicating where to move the axis to</param>
        //[classLCMethodAttribute("Move Axis to Position", 1, true, "", -1, false)]
        public void MoveToPosition(int axis, float position)
        {
            if(Emulation)
            {
                return;
            }
            if (ValidateAxis(axis))
            {
                StringBuilder cmd = new StringBuilder();
                cmd.Append(axis.ToString() + "PA" + position.ToString() + CONST_CMD_TERMINATOR);
                WriteCommand(cmd.ToString());
            }
        }

        /// <summary>
        /// Turn motor of specified axis on
        /// </summary>
        /// <param name="axis">integer representing axis whose motor to turn on</param>
        public void MotorOn(int axis)
        {
            if(Emulation)
            {
                m_motorStatus[axis-1] = true;
                return;
            }
            if (ValidateAxis(axis))
            {
                StringBuilder cmd = new StringBuilder();
                cmd.Append(axis.ToString());
                cmd.Append("MO");
                cmd.Append(CONST_CMD_TERMINATOR);
                WriteCommand(cmd.ToString());
                m_motorStatus[axis - 1] = true;
                if (StatusUpdate != null)
                {
                    StatusUpdate(this, new classDeviceStatusEventArgs(Status, "Motor", axis.ToString() + " On", this));
                }
            }
        }

        /// <summary>
        /// Turn motor of specified axis off
        /// </summary>
        /// <param name="axis">integer representing axis whose motor to turn on</param>
        public void MotorOff(int axis)
        {
            if(Emulation)
            {
                m_motorStatus[axis-1] = false;
                return;
            }
            if (ValidateAxis(axis))
            {
                StringBuilder cmd = new StringBuilder();
                cmd.Append(axis.ToString());
                cmd.Append("MF");
                cmd.Append(CONST_CMD_TERMINATOR);
                WriteCommand(cmd.ToString());
                m_motorStatus[axis - 1] = false;
                if (StatusUpdate != null)
                {
                    StatusUpdate(this, new classDeviceStatusEventArgs(Status, "Motor", axis.ToString() + " Off", this));
                }
            }
        }


        public bool GetMotorStatus(int axis)
        {
            return m_motorStatus[axis-1];
        }

        /// <summary>
        /// Move specified axis in a specified direction, optionally at slow speed
        /// </summary>
        /// <param name="axis">integer specifying which axis to move</param>
        /// <param name="mvnegative">boolean specifying direction, true to move in the positive direction, false to move in the negative direction</param>
        /// <param name="slow"></param>
        //[classLCMethodAttribute("Move Axis in Direction", 1, true, "", -1, false)]
        public void MoveAxis(int axis, bool mvNegative, bool slow = false)
        {
            if(Emulation)
            {
                return;
            }
            if (ValidateAxis(axis))
            {
                StringBuilder cmd = new StringBuilder();
                cmd.Append(axis.ToString());
                cmd.Append("MV");
                cmd.Append(mvNegative ? "-" : "+");
                cmd.Append(CONST_CMD_TERMINATOR);
                if (slow)
                {
                    //prepending change of acceleration command to move command.
                    double divisor = 10.0;
                    cmd.Insert(0, ";");
                    cmd.Insert(0, string.Format("0.00000", m_SpeedNormal[axis] / divisor));
                    cmd.Insert(0, axis.ToString());
                    m_AtSlowSpeed = true;
                }
                WriteCommand(cmd.ToString());
            }
        }
         
        /// <summary>
        /// Determine if an axis is finished moving
        /// </summary>
        /// <param name="axis">integer representing axis to check</param>
        /// <returns>true if done moving, false if not</returns>
        //[classLCMethodAttribute("Query if Axis Motion is Finished", 1, true, "", -1, false)]
        public bool MotionDone(int axis)
        {
            if(Emulation)
            {
                return true;
            }
            if (ValidateAxis(axis))
            {
                StringBuilder cmd = new StringBuilder();
                cmd.Append(axis.ToString());
                cmd.Append("MD");
                cmd.Append(CONST_CMD_TERMINATOR);
                string response = WriteCommand(cmd.ToString(), true);
                response.Trim('\r');
                try
                {
                    if (!Emulation)
                    {
                        return Convert.ToBoolean(Convert.ToInt32(response));
                    }
                    else
                    {
                        return true;
                    }
                }
                catch
                {
                    ErrorType = enumDeviceErrorStatus.ErrorAffectsAllColumns;
                    if (Error != null)
                    {
                        LcmsNetDataClasses.Logging.classApplicationLogger.LogError(0, "Unexpected response when checking motion completion");
                        Error(this, new classDeviceErrorEventArgs("Unexpected response", null, enumDeviceErrorStatus.ErrorAffectsAllColumns, this));
                    }                    
                }
            }
            return false;
        }

        /// <summary>
        /// Stop axis from moving
        /// </summary>
        /// <param name="axis">integer representing axis to stop</param>
        //[classLCMethodAttribute("Stop Axis", 1, true, "", -1, false)]
        public void StopMotion(int axis)
        {
            if(Emulation)
            {
                return;
            }
            if (ValidateAxis(axis))
            {
                const int firstChar = 0;
                StringBuilder cmd = new StringBuilder();
                cmd.Append(axis.ToString());
                cmd.Append("ST");
                cmd.Append(CONST_CMD_TERMINATOR);
                // if we're moving the axis at slow speed, reset to normal speed.
                if (m_AtSlowSpeed)
                {
                    //prepending the change in acceleration command to the stop command
                    cmd.Insert(firstChar, ";");
                    cmd.Insert(firstChar, string.Format("0.00000", m_SpeedNormal[axis]));
                    cmd.Insert(firstChar, "VA");
                    cmd.Insert(firstChar, axis.ToString());
                }
                WriteCommand(cmd.ToString());
            }
        }

        private bool ValidateAxis(int axis)
        {
            if(0 < axis && axis <= NumAxes)
            {
                return true;
            }
            else
            {
                LcmsNetDataClasses.Logging.classApplicationLogger.LogError(LcmsNetDataClasses.Logging.classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "Attempt to modify invalid axis detected");
                return false;
            }
        }

        /// <summary>
        /// Find the home position of provided axis
        /// </summary>
        /// <param name="axis">integer determining which axis to find the home of</param>
        public void FindHome(int axis)
        {
            if(Emulation)
            {
                return;
            }
            if (ValidateAxis(axis))
            {
                StringBuilder cmd = new StringBuilder();
                cmd.Append(axis.ToString() + "OR4" + CONST_CMD_TERMINATOR);
                WriteCommand(cmd.ToString());
            }          
        }

        /// <summary>
        /// Read error message from the stage
        /// </summary>
        /// <param name="errcode">reference to an integer to store the errorcode</param>
        /// <param name="timestamp">reference to a long to store the timestamp</param>
        /// <param name="description">reference to a string to storee the description of the error</param>

        public void ReadErrorMessage(ref int errcode, ref long timestamp, ref string description)
        {
            if(Emulation)
            {
                errcode = 0;
                timestamp = 0L;
                description = string.Empty;
                return;
            }
            string response = string.Empty;
            string[] tokens;
            // we are not using WriteCommand here to avoid infinite recursion, since this is called within WriteCommand.
            try
            {
                Port.Write("TB?" + CONST_CMD_TERMINATOR);
            }
            catch (Exception)
            {
                ErrorType = enumDeviceErrorStatus.ErrorAffectsAllColumns;
                if (Error != null)
                {
                    Error(this, new classDeviceErrorEventArgs("Error writing data to stage on error check", null, enumDeviceErrorStatus.ErrorAffectsAllColumns, this));
                }
            }  
            try
            {
                response = Port.ReadLine();
            }
            catch (Exception)
            {
                ErrorType = enumDeviceErrorStatus.ErrorAffectsAllColumns;
                if (Error != null)
                {
                    Error(this, new classDeviceErrorEventArgs("Error reading data from stage on error check", null, enumDeviceErrorStatus.ErrorAffectsAllColumns, this));
                }
            }
            try
            {
                tokens = response.Split(',');
                errcode = Convert.ToInt32(tokens[0]);
                timestamp = Convert.ToInt64(tokens[1]);
                description = tokens[2];
            }
            catch (Exception)
            {
                ErrorType = enumDeviceErrorStatus.ErrorAffectsAllColumns;
                if (Error != null)
                {
                    Error(this, new classDeviceErrorEventArgs("Error converting response token on error check", null, enumDeviceErrorStatus.ErrorAffectsAllColumns, this));
                }
            }           
        }

        public string GetErrors()
        {
            if(Emulation)
            {
                return "No Errors Reported.";
            }
            StringBuilder errString = new StringBuilder();
            m_reportedErrors.ForEach(x=> errString.Append(x));
            if (errString.ToString().Equals(string.Empty))
            {
                return "No Errors Reported.";
            }
            return errString.ToString();
        }


        public void ClearErrors()
        {
            m_reportedErrors.Clear();
        }

        #endregion

        #region HelperMethods

        /// <summary>
        /// Write command to the serial port associated with this stage
        /// </summary>
        /// <param name="cmnd">string containing the command to write to the stage, should follow the stage command syntax</param>
        /// <param name="waitForResponse">boolean determining if the software waits for the stage to respond to the command</param>
        public string WriteCommand(string command, bool waitForResponse = false)
        {
            string response = string.Empty;
            if (!Emulation)
            {
                try
                {
                    Port.Write(command);
                }
                catch (Exception)
                {
                    ErrorType = enumDeviceErrorStatus.ErrorAffectsAllColumns;
                    if (Error != null)
                    {
                        Error(this, new classDeviceErrorEventArgs("Error writing to stage", null, enumDeviceErrorStatus.ErrorAffectsAllColumns, this));
                    }
                }
                if (waitForResponse)
                {
                    try
                    {
                        response = Port.ReadLine();
                    }
                    catch (Exception)
                    {
                        ErrorType = enumDeviceErrorStatus.ErrorAffectsAllColumns;
                        if (Error != null)
                        {
                            Error(this, new classDeviceErrorEventArgs("Error reading from stage", null, enumDeviceErrorStatus.ErrorAffectsAllColumns, this));
                        }
                    }
                }
                int errcode = -1;
                long timestamp = -1;
                string description = string.Empty;
                ReadErrorMessage(ref errcode, ref timestamp, ref description);
                if (errcode != 0)
                {
                    m_reportedErrors.Add(description + " happened at time: " + Convert.ToString(timestamp));
                    ErrorType = enumDeviceErrorStatus.ErrorAffectsAllColumns;
                    if (Error != null)
                    {
                        Error(this, new classDeviceErrorEventArgs(description, null, enumDeviceErrorStatus.ErrorAffectsAllColumns, this));
                    }
                }
                return response;
            }
            else
            {
                return "Emulated Write Complete";
            }
        }

        /// <summary>
        /// Configure serial port to match the Newport ESP300 communication configuration
        /// </summary>
        public void ConfigurePort()
        {
            Port.BaudRate = CONST_BAUDRATE;
            Port.DataBits = CONST_DATABITS;
            Port.Parity = CONST_PARITY;
            Port.StopBits = CONST_STOPBITS;
            Port.Handshake = CONST_HANDSHAKE;
            Port.ReadTimeout = 1000;
        }

        /// <summary>
        /// Attempt to open the serial port
        /// </summary>
        /// <param name="port"></param>
        //[classLCMethodAttribute("Open Port", 1, true, "", -1, false)]
        public void OpenPort()
        {
            if(Emulation)
            {
                return;
            }
            if (!Port.IsOpen)
            {
                // if this throws an error, it should just propigate up the call chain, we don't want to hide it, the port/device has issues.
                Port.Open();
                if (Port.IsOpen)
                {
                    // this writecommand is to clear out anything that may have been erroneously sent to the newport by another device, we don't care about errors reported from it
                    // since if something else sent a command, it likely did not exist anyway and will throw one.
                    WriteCommand("" + CONST_CMD_TERMINATOR, false);
                    Port.ReadLine();
                    ClearErrors();
                    for (int axis = 1; axis < NumAxes + 1; axis++)
                    {
                        Port.Write(axis.ToString() + "VA?" + CONST_CMD_TERMINATOR);
                        string resp = Port.ReadLine();
                        m_SpeedNormal[axis - 1] = Convert.ToDouble(resp);
                    }
                }
            }
        }

        /// <summary>
        /// Attempt to close the serial port.
        /// </summary>
        //[classLCMethodAttribute("Close Port", 1, true, "", -1, false)]
        public void ClosePort()
        {
            if(Emulation)
            {
                return;
            }
            if (Port.IsOpen)
            {
                Port.Close();
            }
        }


        /// <summary>
        /// Lists the position names 
        /// </summary>
        private void ListPositions()
        {
            if (PosNames != null)
            {
                string[] keys = new string[m_positions.Keys.Count];
                m_positions.Keys.CopyTo(keys, 0);

                List<object> data = new List<object>();
                data.AddRange(keys);
                PosNames(this, (List<object>)data);
            }
        }
        #endregion

        #endregion

        #region Properties


        public string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }

        public bool Emulation
        {
            get;
            set;
        }

        public enumDeviceStatus Status
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets the serial communications port associated with this ESP300.
        /// </summary>        
        public SerialPort Port
        {
            get
            {
                return m_port;
            }
            set
            {
                m_port = value;
            }
        }

      
        public string Version
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the status of the serial port
        /// </summary>
        public bool PortOpen
        {
            get
            {
                return Port.IsOpen;
            }
        }

        /// <summary>
        /// Gets or Sets the number of Axes available on this ESP300.
        /// </summary>
        [classPersistenceAttribute("NumAxes")]
        public int NumAxes
        {
            get
            {
                return m_numAxes;
            }
            set
            {
                m_numAxes = value;
            }
        }

        /// <summary>
        /// used to persist serial config to ini file and reload serial config from ini file.
        /// </summary>
        [classPersistenceAttribute("Port")]
        public string PortStr
        {
            get
            {
                char separator = ',';
                StringBuilder persistedPort = new StringBuilder();
                persistedPort.Append(Port.PortName);
                persistedPort.Append(separator);
                persistedPort.Append(Port.BaudRate);
                persistedPort.Append(separator);
                persistedPort.Append(Port.DataBits);
                persistedPort.Append(separator);
                persistedPort.Append(Port.Parity);
                persistedPort.Append(separator);
                persistedPort.Append(Port.StopBits.ToString());
                persistedPort.Append(separator);
                persistedPort.Append(Port.Handshake.ToString());
                return persistedPort.ToString();
            }
            set
            {
                ClosePort();
                string[] tokenizedValue = value.Split(',');
                Port.PortName = tokenizedValue[0];
                Port.BaudRate = Convert.ToInt32(tokenizedValue[1]);
                Port.DataBits = Convert.ToInt32(tokenizedValue[2]);
                string parity = tokenizedValue[3];
                if (parity == "None")
                {
                    Port.Parity = Parity.None;
                }
                else if (parity == "Odd")
                {
                    Port.Parity = Parity.Odd;
                }
                else if (parity == "Even")
                {
                    Port.Parity = Parity.Even;
                }
                else if (parity == "Mark")
                {
                    Port.Parity = Parity.Mark;
                }
                else if (parity == "Space")
                {
                    Port.Parity = Parity.Space;
                }
                else
                {
                    throw new ESP300Exception("Invalid Port Parity");
                }
                string stop = tokenizedValue[4];
                if (stop == "One")
                {
                    Port.StopBits = StopBits.One;
                }
                else if (stop == "Two")
                {
                    Port.StopBits = StopBits.Two;
                }
                else if (stop == "None")
                {
                    Port.StopBits = StopBits.None;
                }
                else if (stop == "OnePointFive")
                {
                    Port.StopBits = StopBits.OnePointFive;
                }
                else
                {
                    throw new ESP300Exception("Invalid Port Stopbits");
                }
                string handshake = tokenizedValue[5];
                if (handshake == "RequestToSend")
                {
                    Port.Handshake = Handshake.RequestToSend;
                }
                else if (handshake == "None")
                {
                    Port.Handshake = Handshake.None;
                }
                else if (handshake == "XOnXOff")
                {
                    Port.Handshake = Handshake.XOnXOff;
                }
                else if (handshake == "RequestToSendXonXOFF")
                {
                    Port.Handshake = Handshake.RequestToSendXOnXOff;
                }
                else
                {
                    throw new ESP300Exception("Invalid Port Handshake");
                }
            }
            
        }

        /// <summary>
        /// used to persist the positions to ini config file and reload positions from ini file.
        /// </summary>
        [classPersistenceAttribute("Positions")]
        public string PositionsStr
        {
            get
            {
                //create string of semicolon separated positions, each position contains 3 comma separated values for Axis1/2/3 position
                char positionInfoSeparator = ',';
                char positionSeparator = ';';
                StringBuilder semicolonSeparatedPositions = new StringBuilder();
                foreach (string key in m_positions.Keys)
                {
                    classStagePosition pos = m_positions[key];
                    semicolonSeparatedPositions.Append(key);
                    semicolonSeparatedPositions.Append(positionInfoSeparator);
                    string axis1Pos = pos[0].ToString();
                    string axis2Pos = pos[1].ToString();
                    string axis3Pos = pos[2].ToString();
                    semicolonSeparatedPositions.Append(axis1Pos);
                    semicolonSeparatedPositions.Append(positionInfoSeparator);
                    semicolonSeparatedPositions.Append(axis2Pos);
                    semicolonSeparatedPositions.Append(positionInfoSeparator);
                    semicolonSeparatedPositions.Append(axis3Pos);
                    semicolonSeparatedPositions.Append(positionSeparator);
                }
                semicolonSeparatedPositions.Append('\n');
                return semicolonSeparatedPositions.ToString();
            }
            set
            {
                char positionInfoSeparator = ',';
                char positionSeparator = ';';
                    string[] tokenizedPositions = value.Split(positionSeparator);
                    foreach (string position in tokenizedPositions)
                    {
                        if (position != string.Empty)
                        {
                            string[] positionInfo = position.Split(positionInfoSeparator);
                            classStagePosition persistedPosition = new classStagePosition();
                            string key = positionInfo[0];
                            persistedPosition[0] = Convert.ToSingle(positionInfo[1]);
                            persistedPosition[1] = Convert.ToSingle(positionInfo[2]);
                            persistedPosition[2] = Convert.ToSingle(positionInfo[3]);
                            m_positions[key] = persistedPosition;
                        }
                    }
                    if (PositionsLoaded != null)
                    {
                        PositionsLoaded(this, new EventArgs());
                    }
            }
        }

        /// <summary>
        /// Gets the positions of the axes of this ESP300
        /// </summary>
        public Dictionary<string, classStagePosition> Positions
        {
            get
            {
                return m_positions;
            }
            set
            {
                m_positions = value;
            }
        }
        /// <summary>
        /// Get or Sets the current position of the stage
        /// </summary>
        [classPersistenceAttribute("currentPosition")]
        public string CurrentPos
        {
            get
            {
                return m_position;
            }
            set
            {
                m_position = value;
                if (PositionChanged != null)
                {
                    PositionChanged(this, new EventArgs());
                }
            }
        }
        #endregion

        #region IDevice Members
        /// <summary>
        /// Gets or sets the abort event for scheduling.
        /// </summary>
        public System.Threading.ManualResetEvent AbortEvent { get; set; }
        public List<string> GetStatusNotificationList()
        {
            return new List<string>(){"Initialized", "Motor", "Shutdown"};
        }

        public List<string> GetErrorNotificationList()
        {
            return new List<string>(){"Error writing to stage", "Error reading from stage", "Unexpected response"};
        }

        public void RegisterDataProvider(string key, DelegateDeviceHasData remoteMethod)
        {
            switch (key.ToUpper())
            {
                case "POSNAMES":
                    PosNames += remoteMethod;
                    ListPositions();
                    break;
            }
        }
        public void UnRegisterDataProvider(string key, DelegateDeviceHasData remoteMethod)
        {
            switch (key.ToUpper())
            {
                case "POSNAMES":
                    PosNames -= remoteMethod;
                    break;
            }
        }

        public void WritePerformanceData(string directoryPath, string name, object[] parameters)
        {
        }

        public bool Shutdown()
        {
            MotorOff(1);
            MotorOff(2);
            MotorOff(3);
            ClosePort();
            Status = enumDeviceStatus.NotInitialized;
            if (StatusUpdate != null)
            {
                StatusUpdate(this, new classDeviceStatusEventArgs(Status, "Shutdown", this));
            }
            return true;
        }


        public bool Initialize(ref string astring)
        {
            OpenPort();
            for (int i = 1; i <= NumAxes;i++)
            {
                MotorOn(i);
            }
                Status = enumDeviceStatus.Initialized;
            if(StatusUpdate != null)
            {
                StatusUpdate(this, new classDeviceStatusEventArgs(Status, "Initialized", this));
            }
            return true;
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
        #endregion

        #region Events
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

        public event EventHandler PositionsLoaded;
        public event EventHandler PositionChanged;
        public DelegateDeviceHasData PosNames;
        #endregion
    }
}
