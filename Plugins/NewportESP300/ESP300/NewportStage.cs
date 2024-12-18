﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO.Ports;
using System.Text;
using LcmsNetData;
using LcmsNetData.Logging;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Method;

namespace LcmsNetPlugins.Newport.ESP300
{
    [Serializable]
    [DeviceControl(typeof(NewportStageViewModel),
                                 typeof(FluidicsStage),
                                 "Newport Stage",
                                 "Stages")]
    public class NewportStage:IDisposable, IDevice, IHasDataProvider, INotifyPropertyChangedExt
    {
        #region Members
        private SerialPort m_port;
        private int m_numAxes;
        private bool m_AtSlowSpeed;
        private readonly double[] m_SpeedNormal;
        private readonly bool[] m_motorStatus;
        private bool m_disposed;
        private Dictionary<string, StagePosition> m_positions;
        private readonly List<string> m_reportedErrors;
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
        public NewportStage()
            : this(null, "NoPosition", 2)
        {
        }

        /// <summary>
        /// Creates stage on specified port with specified current position and number of axes.
        /// </summary>
        /// <param name="port">a System.IO.Ports SerialPort, defaults to COM1</param>
        /// <param name="currentPos"></param>
        /// <param name="numAxes">an integer representing the number of axes of movement available to this stage</param>
        public NewportStage(SerialPort port, string currentPos, int numAxes)
        {
            m_disposed = false;
            if (port == null)
            {
                Port = new SerialPort("COM1", CONST_BAUDRATE, CONST_PARITY, CONST_DATABITS, CONST_STOPBITS) {
                    Handshake = CONST_HANDSHAKE
                };
            }
            else
            {
                Port = port;
            }
            NumAxes = numAxes;
            m_SpeedNormal = new double[NumAxes];
            m_positions = new Dictionary<string, StagePosition>();
            Name = "Stage";
            m_position = currentPos;
            m_reportedErrors = new List<string>();
            m_motorStatus = new bool[CONST_MAX_AXES];
        }

        ~NewportStage()
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
        /// <param name="positionName">the position whose coordinates to set</param>
        /// <param name="axis1Coord">the x coordinate</param>
        /// <param name="axis2Coord">the y coordinate </param>
        /// <param name="axis3Coord">the z coordinate, currently unused, pass zero</param>
        //[LCMethodEventAttribute("Define Position", 1, true, "", -1, false)]
        public void SetPositionCoordinates(string positionName, float axis1Coord, float axis2Coord, float axis3Coord)
        {
            var position = new StagePosition
            {
                NumAxes = 2,
                [0] = axis1Coord,
                [1] = axis2Coord,
                [2] = axis3Coord
            };
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
        /// <param name="timeout"></param>
        /// <param name="positionName">the position to go to</param>
        [LCMethodEvent("Go To Predefined Position", MethodOperationTimeoutType.Parameter, "POSNAMES", 1)]
        public void GoToPosition(double timeout, string positionName) // bool block = true)
        {
            if(Emulation)
            {
                return;
            }
            // newport stage indexes axes starting from 1, so we go from 1 to numAxes +1
            for(var i = 1; i < m_numAxes + 1; i++)
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
            PositionChanged?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Query position of the specified axis
        /// </summary>
        /// <param name="axis">integer representing the axis to query</param>
        /// <returns>double representing position of the axis</returns>
        //[LCMethodEventAttribute("Query Axis Position", 1, true, "", -1, false)]
        public double QueryPosition(int axis)
        {
            var cmd = new StringBuilder();
            cmd.Append(axis + "TP" + CONST_CMD_TERMINATOR);

            if (!Emulation)
            {
                if (ValidateAxis(axis))
                {
                    var response = WriteCommand(cmd.ToString(), true);
                    try
                    {
                        return Convert.ToDouble(response);
                    }
                    catch
                    {
                        ErrorType = DeviceErrorStatus.ErrorAffectsAllColumns;
                        if (Error != null)
                        {
                            ApplicationLogger.LogError(0, "Unexpected response when querying stage position");
                            Error(this, new DeviceErrorEventArgs("Unexpected response", null, DeviceErrorStatus.ErrorAffectsAllColumns, this));
                        }
                        return 0.0;
                    }
                }
                return 0.0;
            }

            return 0.0;
        }

        /// <summary>
        ///  Move specified axis to desired position
        /// </summary>
        /// <param name="axis">integer representing the axis to move</param>
        /// <param name="position">double indicating where to move the axis to</param>
        //[LCMethodEventAttribute("Move Axis to Position", 1, true, "", -1, false)]
        public void MoveToPosition(int axis, float position)
        {
            if(Emulation)
            {
                return;
            }
            if (ValidateAxis(axis))
            {
                var cmd = new StringBuilder();
                cmd.Append(axis + "PA" + position.ToString(CultureInfo.InvariantCulture) + CONST_CMD_TERMINATOR);
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
                var cmd = new StringBuilder();
                cmd.Append(axis.ToString());
                cmd.Append("MO");
                cmd.Append(CONST_CMD_TERMINATOR);
                WriteCommand(cmd.ToString());
                m_motorStatus[axis - 1] = true;
                StatusUpdate?.Invoke(this, new DeviceStatusEventArgs(Status, "Motor", this, axis.ToString() + " On"));
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
                var cmd = new StringBuilder();
                cmd.Append(axis.ToString());
                cmd.Append("MF");
                cmd.Append(CONST_CMD_TERMINATOR);
                WriteCommand(cmd.ToString());
                m_motorStatus[axis - 1] = false;
                StatusUpdate?.Invoke(this, new DeviceStatusEventArgs(Status, "Motor", this, axis.ToString() + " Off"));
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
        /// <param name="mvNegative">boolean specifying direction, true to move in the positive direction, false to move in the negative direction</param>
        /// <param name="slow"></param>
        //[LCMethodEventAttribute("Move Axis in Direction", 1, true, "", -1, false)]
        public void MoveAxis(int axis, bool mvNegative, bool slow = false)
        {
            if(Emulation)
            {
                return;
            }
            if (ValidateAxis(axis))
            {
                var cmd = new StringBuilder();
                cmd.Append(axis.ToString());
                cmd.Append("MV");
                cmd.Append(mvNegative ? "-" : "+");
                cmd.Append(CONST_CMD_TERMINATOR);
                if (slow)
                {
                    //prepending change of acceleration command to move command.
                    var divisor = 10.0;
                    cmd.Insert(0, ";");
                    cmd.Insert(0, (m_SpeedNormal[axis] / divisor).ToString("0.00000"));
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
        //[LCMethodEventAttribute("Query if Axis Motion is Finished", 1, true, "", -1, false)]
        public bool MotionDone(int axis)
        {
            if(Emulation)
            {
                return true;
            }
            if (ValidateAxis(axis))
            {
                var cmd = new StringBuilder();
                cmd.Append(axis.ToString());
                cmd.Append("MD");
                cmd.Append(CONST_CMD_TERMINATOR);
                var response = WriteCommand(cmd.ToString(), true);
                response = response.Trim('\r');
                try
                {
                    if (!Emulation)
                    {
                        return Convert.ToBoolean(Convert.ToInt32(response));
                    }

                    return true;
                }
                catch
                {
                    ErrorType = DeviceErrorStatus.ErrorAffectsAllColumns;
                    if (Error != null)
                    {
                        ApplicationLogger.LogError(0, "Unexpected response when checking motion completion");
                        Error(this, new DeviceErrorEventArgs("Unexpected response", null, DeviceErrorStatus.ErrorAffectsAllColumns, this));
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Stop axis from moving
        /// </summary>
        /// <param name="axis">integer representing axis to stop</param>
        //[LCMethodEventAttribute("Stop Axis", 1, true, "", -1, false)]
        public void StopMotion(int axis)
        {
            if(Emulation)
            {
                return;
            }
            if (ValidateAxis(axis))
            {
                const int firstChar = 0;
                var cmd = new StringBuilder();
                cmd.Append(axis.ToString());
                cmd.Append("ST");
                cmd.Append(CONST_CMD_TERMINATOR);
                // if we're moving the axis at slow speed, reset to normal speed.
                if (m_AtSlowSpeed)
                {
                    //prepending the change in acceleration command to the stop command
                    cmd.Insert(firstChar, ";");
                    cmd.Insert(firstChar, m_SpeedNormal[axis].ToString("0.00000"));
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

            ApplicationLogger.LogError(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL,
                "Attempt to modify invalid axis detected");
            return false;
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
                var cmd = new StringBuilder();
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
            var response = string.Empty;
            // we are not using WriteCommand here to avoid infinite recursion, since this is called within WriteCommand.
            try
            {
                Port.Write("TB?" + CONST_CMD_TERMINATOR);
            }
            catch (Exception)
            {
                ErrorType = DeviceErrorStatus.ErrorAffectsAllColumns;
                Error?.Invoke(this, new DeviceErrorEventArgs("Error writing data to stage on error check", null, DeviceErrorStatus.ErrorAffectsAllColumns, this));
            }
            try
            {
                response = Port.ReadLine();
            }
            catch (Exception)
            {
                ErrorType = DeviceErrorStatus.ErrorAffectsAllColumns;
                Error?.Invoke(this, new DeviceErrorEventArgs("Error reading data from stage on error check", null, DeviceErrorStatus.ErrorAffectsAllColumns, this));
            }
            try
            {
                var tokens = response.Split(',');
                errcode = Convert.ToInt32(tokens[0]);
                timestamp = Convert.ToInt64(tokens[1]);
                description = tokens[2];
            }
            catch (Exception)
            {
                ErrorType = DeviceErrorStatus.ErrorAffectsAllColumns;
                Error?.Invoke(this, new DeviceErrorEventArgs("Error converting response token on error check", null, DeviceErrorStatus.ErrorAffectsAllColumns, this));
            }
        }

        public string GetErrors()
        {
            if(Emulation)
            {
                return "No Errors Reported.";
            }
            var errString = new StringBuilder();
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
        /// <param name="command">string containing the command to write to the stage, should follow the stage command syntax</param>
        /// <param name="waitForResponse">boolean determining if the software waits for the stage to respond to the command</param>
        public string WriteCommand(string command, bool waitForResponse = false)
        {
            var response = string.Empty;
            if (!Emulation)
            {
                try
                {
                    Port.Write(command);
                }
                catch (Exception)
                {
                    ErrorType = DeviceErrorStatus.ErrorAffectsAllColumns;
                    Error?.Invoke(this, new DeviceErrorEventArgs("Error writing to stage", null, DeviceErrorStatus.ErrorAffectsAllColumns, this));
                }
                if (waitForResponse)
                {
                    try
                    {
                        response = Port.ReadLine();
                    }
                    catch (Exception)
                    {
                        ErrorType = DeviceErrorStatus.ErrorAffectsAllColumns;
                        Error?.Invoke(this, new DeviceErrorEventArgs("Error reading from stage", null, DeviceErrorStatus.ErrorAffectsAllColumns, this));
                    }
                }
                var errcode = -1;
                long timestamp = -1;
                var description = string.Empty;
                ReadErrorMessage(ref errcode, ref timestamp, ref description);
                if (errcode != 0)
                {
                    m_reportedErrors.Add(description + " happened at time: " + Convert.ToString(timestamp));
                    ErrorType = DeviceErrorStatus.ErrorAffectsAllColumns;
                    Error?.Invoke(this, new DeviceErrorEventArgs(description, null, DeviceErrorStatus.ErrorAffectsAllColumns, this));
                }
                return response;
            }

            return "Emulated Write Complete";
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
        [LCMethodEvent("Open Port", 1)]
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
                    WriteCommand("" + CONST_CMD_TERMINATOR);
                    Port.ReadLine();
                    ClearErrors();
                    for (var axis = 1; axis < NumAxes + 1; axis++)
                    {
                        Port.Write(axis.ToString() + "VA?" + CONST_CMD_TERMINATOR);
                        var resp = Port.ReadLine();
                        m_SpeedNormal[axis - 1] = Convert.ToDouble(resp);
                    }
                }
            }
        }

        /// <summary>
        /// Attempt to close the serial port.
        /// </summary>
        //[LCMethodEventAttribute("Close Port", 1, true, "", -1, false)]
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
                var keys = new string[m_positions.Keys.Count];
                m_positions.Keys.CopyTo(keys, 0);

                var data = new List<object>();
                data.AddRange(keys);
                PosNames(this, data);
            }
        }
        #endregion

        #endregion

        #region Properties

        public string Name
        {
            get => m_name;
            set => this.RaiseAndSetIfChanged(ref m_name, value);
        }

        public bool Emulation { get; set; }

        public DeviceStatus Status { get; set; }

        /// <summary>
        /// Gets or Sets the serial communications port associated with this ESP300.
        /// </summary>
        public SerialPort Port
        {
            get => m_port;
            set => this.RaiseAndSetIfChanged(ref m_port, value);
        }

        public string Version { get; set; }

        /// <summary>
        /// Gets the status of the serial port
        /// </summary>
        public bool PortOpen => Port.IsOpen;

        /// <summary>
        /// Gets or Sets the number of Axes available on this ESP300.
        /// </summary>
        [DeviceSavedSetting("NumAxes")]
        public int NumAxes
        {
            get => m_numAxes;
            set => m_numAxes = value;
        }

        /// <summary>
        /// used to persist serial config to ini file and reload serial config from ini file.
        /// </summary>
        [DeviceSavedSetting("Port")]
        public string PortStr
        {
            get
            {
                var separator = ',';
                var persistedPort = new StringBuilder();
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
                var tokenizedValue = value.Split(',');
                Port.PortName = tokenizedValue[0];
                Port.BaudRate = Convert.ToInt32(tokenizedValue[1]);
                Port.DataBits = Convert.ToInt32(tokenizedValue[2]);
                var parity = tokenizedValue[3];
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
                var stop = tokenizedValue[4];
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
                var handshake = tokenizedValue[5];
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
        [DeviceSavedSetting("Positions")]
        public string PositionsStr
        {
            get
            {
                //create string of semicolon separated positions, each position contains 3 comma separated values for Axis1/2/3 position
                var positionInfoSeparator = ',';
                var positionSeparator = ';';
                var semicolonSeparatedPositions = new StringBuilder();
                foreach (var key in m_positions.Keys)
                {
                    var pos = m_positions[key];
                    semicolonSeparatedPositions.Append(key);
                    semicolonSeparatedPositions.Append(positionInfoSeparator);
                    var axis1Pos = pos[0].ToString(CultureInfo.InvariantCulture);
                    var axis2Pos = pos[1].ToString(CultureInfo.InvariantCulture);
                    var axis3Pos = pos[2].ToString(CultureInfo.InvariantCulture);
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
                var positionInfoSeparator = ',';
                var positionSeparator = ';';
                    var tokenizedPositions = value.Split(positionSeparator);
                    foreach (var position in tokenizedPositions)
                    {
                        if (position != string.Empty)
                        {
                            var positionInfo = position.Split(positionInfoSeparator);
                            var persistedPosition = new StagePosition();
                            var key = positionInfo[0];
                            persistedPosition[0] = Convert.ToSingle(positionInfo[1]);
                            persistedPosition[1] = Convert.ToSingle(positionInfo[2]);
                            persistedPosition[2] = Convert.ToSingle(positionInfo[3]);
                            m_positions[key] = persistedPosition;
                        }
                    }
                PositionsLoaded?.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        /// Gets the positions of the axes of this ESP300
        /// </summary>
        public Dictionary<string, StagePosition> Positions
        {
            get => m_positions;
            set => m_positions = value;
        }

        /// <summary>
        /// Get or Sets the current position of the stage
        /// </summary>
        [DeviceSavedSetting("currentPosition")]
        public string CurrentPos
        {
            get => m_position;
            set
            {
                m_position = value;
                PositionChanged?.Invoke(this, new EventArgs());
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
                    if (PosNames != null)
                        PosNames -= remoteMethod;
                    break;
            }
        }

        public bool Shutdown()
        {
            MotorOff(1);
            MotorOff(2);
            MotorOff(3);
            ClosePort();
            Status = DeviceStatus.NotInitialized;
            StatusUpdate?.Invoke(this, new DeviceStatusEventArgs(Status, "Shutdown", this));
            return true;
        }

        public bool Initialize(ref string astring)
        {
            OpenPort();
            for (var i = 1; i <= NumAxes;i++)
            {
                MotorOn(i);
            }
                Status = DeviceStatus.Initialized;
            StatusUpdate?.Invoke(this, new DeviceStatusEventArgs(Status, "Initialized", this));
            return true;
        }

        public DeviceErrorStatus ErrorType { get; set; }

        public DeviceType DeviceType => DeviceType.Component;

        #endregion

        #region Events
        /// <summary>
        /// Fired when a property changes in the device.
        /// </summary>
#pragma warning disable CS0067
        public event EventHandler DeviceSaveRequired;
#pragma warning restore CS0067
        /// <summary>
        /// Fired when the status changes.
        /// </summary>
        public event EventHandler<DeviceStatusEventArgs> StatusUpdate;
        /// <summary>
        /// Fired when an error occurs in the device.
        /// </summary>
        public event EventHandler<DeviceErrorEventArgs> Error;

        public event EventHandler PositionsLoaded;
        public event EventHandler PositionChanged;
        public DelegateDeviceHasData PosNames;
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
