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
        /// <summary>
        /// The valve's ID.
        /// </summary>
        private char valveID;

        /// <summary>
        /// The Serial Port Name
        /// </summary>
        private string portName = "COM1";

        private int readTimeout = 100;
        private int writeTimeout = 100;

        private ValveConnectionID connectionID;

        /// <summary>
        /// The valve's name
        /// </summary>
        private string deviceName;

        /// <summary>
        /// Holds the status of the device.
        /// </summary>
        private DeviceStatus status;

        private string versionBackingValue = "";

        protected static readonly int IDChangeDelayTimeMsec = 325;  //milliseconds
        protected const int MinTimeBetweenCommandsMs = 100; // milliseconds

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

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="readTimeout">serial port read timeout</param>
        /// <param name="writeTimeout">serial port write timeout</param>
        /// <param name="defaultName">default device name</param>
        protected ValveVICIBase(int readTimeout, int writeTimeout, string defaultName)
        {
            // Set ID to a space (i.e. nonexistent)
            // NOTE: Spaces are ignored by the controller in sent commands
            valveID = ' ';
            Version = "";

            this.readTimeout = readTimeout;
            this.writeTimeout = writeTimeout;

            UpdateConnection();

            deviceName = defaultName;
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
            valveID = ' ';
            Version = "";

            if (port.IsOpen)
            {
                port.Close();
            }

            portName = port.PortName;
            readTimeout = port.ReadTimeout;
            writeTimeout = port.WriteTimeout;

            UpdateConnection();

            //deviceName        = DeviceManager.Manager.CreateUniqueDeviceName("valve");
            deviceName = defaultName;
        }

        /// <summary>
        /// Gets or sets whether the device is in emulation mode or not.
        /// </summary>
        //[PersistenceDataAttribute("Emulated")]
        public bool Emulation { get; set; }

        /// <summary>
        /// Flag set by GetVersion, if a universal actuator is detected; used to conditionalize the use of some commands
        /// </summary>
        public bool IsUniversalActuator { get; private set; }

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

                return status;
            }
            set
            {
                if (value != status)
                    StatusUpdate?.Invoke(this, new DeviceStatusEventArgs(value, "Status Changed", this));
                status = value;
            }
        }

        /// <summary>
        /// Gets or sets the device's name
        /// </summary>
        public string Name
        {
            get => deviceName;
            set
            {
                if (this.RaiseAndSetIfChangedRetBool(ref deviceName, value))
                {
                    OnDeviceSaveRequired();
                }
            }
        }

        /// <summary>
        /// Serial connection (configured)
        /// </summary>
        internal IValveConnection Connection { get; private set; }

        /// <summary>
        /// Serial port name
        /// </summary>
        [DeviceSavedSetting("PortName")]
        public string PortName
        {
            get => portName;
            set
            {
                var oldValue = portName;
                portName = value;

                if (oldValue != value)
                {
                    UpdateConnection();
                }
                OnDeviceSaveRequired();
            }
        }

        /// <summary>
        /// Timeout to read status from valve
        /// </summary>
        [DeviceSavedSetting("ReadTimeout")]
        public int ReadTimeout
        {
            get => readTimeout;
            set
            {
                var oldValue = readTimeout;
                readTimeout = value;

                if (oldValue != value)
                {
                    Connection.ReadTimeout = Math.Max(Connection.ReadTimeout, readTimeout);
                }

                OnDeviceSaveRequired();
            }
        }

        /// <summary>
        /// Timeout to write command to valve
        /// </summary>
        [DeviceSavedSetting("WriteTimeout")]
        public int WriteTimeout
        {
            get => writeTimeout;
            set
            {
                var oldValue = writeTimeout;
                writeTimeout = value;

                if (oldValue != value)
                {
                    Connection.WriteTimeout = Math.Max(Connection.WriteTimeout, writeTimeout);
                }

                OnDeviceSaveRequired();
            }
        }

        /// <summary>
        /// Gets and sets the valve's ID in the software. DOES NOT CHANGE THE VALVE'S HARDWARE ID (Must call SetHardwareID to do that).
        /// </summary>
        [DeviceSavedSetting("SoftwareID")]
        public char SoftwareID
        {
            get => valveID;
            set
            {
                if (this.RaiseAndSetIfChangedRetBool(ref valveID, value))
                {
                    UpdateConnection();
                    OnDeviceSaveRequired();
                }
            }
        }

        /// <summary>
        /// Gets the valve's version information.
        /// </summary>
        public string Version
        {
            get => versionBackingValue;
            private set => this.RaiseAndSetIfChanged(ref versionBackingValue, value);
        }

        /// <summary>
        /// Display string for the Last Measured Position
        /// </summary>
        public abstract string LastMeasuredPositionDisplay { get; }

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

        private void UpdateConnection()
        {
            if (!string.IsNullOrWhiteSpace(connectionID.PortName))
            {
                ValveConnectionManager.Instance.ReleaseConnection(connectionID, !string.Equals(connectionID.PortName, PortName));
            }

            connectionID = new ValveConnectionID(PortName, SoftwareID);
            Connection = ValveConnectionManager.Instance.GetConnection(connectionID, ReadTimeout, WriteTimeout);
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
                ValveConnectionManager.Instance.ReleaseConnection(connectionID);
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
                SoftwareID = ' ';
                Version = "Device is in emulation";
                return true;
            }

            //If the serial port is not open, open it
            var result = Connection.Open(out errorMessage);
            if (result == ValveErrors.UnauthorizedAccess)
            {
                //throw new ValveExceptionUnauthorizedAccess();
                return false;
            }

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

            System.Threading.Thread.Sleep(MinTimeBetweenCommandsMs);

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

            System.Threading.Thread.Sleep(MinTimeBetweenCommandsMs);

            // Run mode-specific initialization that needs valve data
            if (!InitializeModeSpecific(out errorMessage))
            {
                return false;
            }

            System.Threading.Thread.Sleep(MinTimeBetweenCommandsMs);

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
        /// Valve initialization calls that are specific to the valve mode - for example, checking the valve control mode for universal actuators
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public abstract bool InitializeModeSpecific(out string errorMessage);

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

            Version = "";

            var result = ReadCommand("VR", out var version, 100);
            if (result != ValveErrors.Success)
            {
                switch (result)
                {
                    case ValveErrors.UnauthorizedAccess:
                        throw new ValveExceptionUnauthorizedAccess();
                    case ValveErrors.TimeoutDuringWrite:
                        throw new ValveExceptionWriteTimeout();
                    case ValveErrors.TimeoutDuringRead:
                        throw new ValveExceptionReadTimeout();
                }
            }

            // Universal Actuators have a "Main" firmware, and a "Serial" firmware (VR2)
            // Detect universal actuators by finding the text "UA_MAIN_" in the returned value (examples are UA_MAIN_CH and UA_MAIN_EQ
            if (version.IndexOf("UA_MAIN_", StringComparison.OrdinalIgnoreCase) > -1)
            {
                ReadCommand("VR2", out var version2, 100);
                if (result == ValveErrors.Success && !string.IsNullOrWhiteSpace(version2))
                {
                    IsUniversalActuator = true;
                    version += "\n" + version2;
                }
            }
            else
            {
                IsUniversalActuator = false;
            }

            Version = version;
            return version;
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

            // NOTE: sending the command '*ID' will get a response from any attached valve
            var result = ReadCommand("ID", out var tempBuffer, 100);
            if (result != ValveErrors.Success)
            {
                switch (result)
                {
                    case ValveErrors.UnauthorizedAccess:
                        throw new ValveExceptionUnauthorizedAccess();
                    case ValveErrors.TimeoutDuringWrite:
                        throw new ValveExceptionWriteTimeout();
                    case ValveErrors.TimeoutDuringRead:
                        throw new ValveExceptionReadTimeout();
                }
            }

            var tempID = ' ';  //Default to blank space

            //This should look something like
            //  ID = 0
            //If there is no ID present, it will read
            //  ID = not used
            // Universal actuator: response looks like "\0ID" (not set) or "5ID5" (set to 5)

            // Universal actuator: strip off null characters.
            if (!string.IsNullOrEmpty(tempBuffer))
            {
                tempBuffer = tempBuffer.Trim('\0', ' ');
            }
            else
            {
                tempBuffer = "";
            }

            if (tempBuffer.Length > 2 && tempBuffer.IndexOf("not used", StringComparison.Ordinal) == -1)   //Only do this if string doesn't contain "not used"
            {
                //Grab the actual position from the above string
                if (tempBuffer.Contains("=") && !tempBuffer.EndsWith("=")) //Make sure we have the expected content in the string
                {
                    //Find the first =
                    var tempCharIndex = tempBuffer.IndexOf("=", StringComparison.Ordinal);
                    if (tempCharIndex >= 0)  //Make sure we found a =
                    {
                        //Change the position to be the second character following the first =
                        tempID = tempBuffer.Substring(tempCharIndex + 2, 1).ToCharArray()[0];
                    }
                }
                else if (tempBuffer.Length == 4)
                {
                    // universal actuator
                    // Return the last character.
                    tempID = tempBuffer[3];
                }

                //Set the valveID (software ID) to the one we just found.
                SoftwareID = tempID;
            }

            return tempID;
        }

        /// <summary>
        /// Get the hardware ID of any valve connected to the serial port.
        /// </summary>
        /// <returns></returns>
        public string GetHardwareIDFromAny()
        {
            if (Emulation)
            {
                return "0";
            }

            // Sending the command '*ID' will get a response from any attached valve
            var result = ReadCommandBroadcast("ID", out var tempBuffer, 100);
            if (result != ValveErrors.Success)
            {
                switch (result)
                {
                    case ValveErrors.UnauthorizedAccess:
                        return "Unauthorized Access";
                    case ValveErrors.TimeoutDuringWrite:
                        return "Timeout during write";
                    case ValveErrors.TimeoutDuringRead:
                        return "Timeout during read";
                }
            }

            return tempBuffer.Replace('\r', ' ');
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
            if ('0' <= newID && newID <= '9' || (IsUniversalActuator && 'A' <= newID && newID <= 'Z'))
            {
                var result = SendCommand("ID" + newID);

                //Wait 325ms for the command to go through
                System.Threading.Thread.Sleep(IDChangeDelayTimeMsec);

                if (result == ValveErrors.Success)
                {
                    SoftwareID = newID;
                    OnDeviceSaveRequired();
                }

                return result;
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

            // Universal actuator can use '*ID*' (to clear ID for all connected valves); but nID* will clear it for a single valve
            var result = SendCommand("ID*");

            //Wait 325ms for the command to go through
            System.Threading.Thread.Sleep(IDChangeDelayTimeMsec);

            if (result == ValveErrors.Success)
            {
                SoftwareID = ' ';
                OnDeviceSaveRequired();
            }

            return result;
        }

        protected string GetHardwarePosition()
        {
            if (Emulation)
            {
                return "";
            }

            // TODO: Read issues exist with old 2-position valves!
            var error = ReadCommand("CP", out var hwPosition, 200);
            if (error != ValveErrors.Success)
            {
                switch (error)
                {
                    case ValveErrors.UnauthorizedAccess:
                        throw new ValveExceptionUnauthorizedAccess();
                    case ValveErrors.TimeoutDuringWrite:
                        throw new ValveExceptionWriteTimeout();
                    case ValveErrors.TimeoutDuringRead:
                        throw new ValveExceptionReadTimeout();
                }
            }

            //This should look like
            //  Position is "B" (2-position)
            //  Position is = 1 (MultiPosition)
            // Universal actuator:
            //  \0CP01 (no hardware ID) or 5CP01 (hardware ID 5) (multiposition)
            //  2-position unknown/not tested
            var result = "";
            var cpPos = hwPosition.IndexOf("CP", StringComparison.OrdinalIgnoreCase);
            if (cpPos >= 0 && cpPos < 2)
            {
                // Universal actuator
                result = hwPosition.Substring(cpPos + 2).Trim('\r', '\n');
            }
            else
            {
                var data = hwPosition.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                for (var i = data.Length - 1; i >= 0; i--)
                {
                    var x = data[i];
                    x = x.Replace(" ", "").ToLower();
                    var loc = x.IndexOf("positionis", StringComparison.OrdinalIgnoreCase);
                    if (loc >= 0)
                    {
                        // Grab the actual position from the string
                        result = x.Substring(loc + "positionis".Length).Trim('=', '"', '\'');
                        break;
                    }
                }
            }

            return result;
        }

        protected ValveErrors SetHardwarePosition(string position, int rotationDelayTimeMs = 200)
        {
            if (Emulation)
            {
                return ValveErrors.Success;
            }

            // 'GO' by itself, on universal actuator, will change position (2-position) or advance to the next position (multiposition)
            var sendError = SendCommand("GO" + position);
            if (sendError == ValveErrors.UnauthorizedAccess || sendError == ValveErrors.TimeoutDuringWrite)
            {
                return sendError;
            }

            //Wait rotationDelayTimeMs for valve to actually switch before proceeding
            //System.Threading.Thread.Sleep(rotationDelayTimeMs);
            //TODO: BLL test this instead using the abort event.
            if (AbortEvent == null)
                AbortEvent = new System.Threading.ManualResetEvent(false);

            var waited = System.Threading.WaitHandle.WaitAll(new System.Threading.WaitHandle[] { AbortEvent }, rotationDelayTimeMs);
            if (waited)
                return ValveErrors.BadArgument;

            GetPosition();

            return ValveErrors.Success;
        }

        /// <summary>
        /// Send a write-only command via the serial port
        /// </summary>
        /// <param name="command">The command to send, excluding valveId</param>
        /// <returns></returns>
        protected ValveErrors SendCommand(string command)
        {
            if (Emulation)
            {
                return ValveErrors.Success;
            }

            return Connection.SendCommand(command, SoftwareID);
        }

        /// <summary>
        /// Send a write-only command via the serial port to all valves attached to it
        /// </summary>
        /// <param name="command">The command to send, excluding valveId</param>
        /// <returns></returns>
        protected ValveErrors SendCommandBroadcast(string command)
        {
            if (Emulation)
            {
                return ValveErrors.Success;
            }

            return Connection.SendCommand(command, '*');
        }

        /// <summary>
        /// Send a read command via the serial port
        /// </summary>
        /// <param name="command">The read command to send, excluding valveId</param>
        /// <param name="returnData">The data returned by the command</param>
        /// <param name="readDelayMs">The delay between when the command is sent, and when returned data is read</param>
        /// <returns></returns>
        protected ValveErrors ReadCommand(string command, out string returnData, int readDelayMs = 100)
        {
            returnData = "";
            if (Emulation)
            {
                return ValveErrors.Success;
            }

            return Connection.ReadCommand(command, SoftwareID, out returnData, readDelayMs);
        }

        /// <summary>
        /// Send a read command via the serial port to all valves attached to it
        /// </summary>
        /// <param name="command">The read command to send, excluding valveId</param>
        /// <param name="returnData">The data returned by the command</param>
        /// <param name="readDelayMs">The delay between when the command is sent, and when returned data is read</param>
        /// <returns></returns>
        protected ValveErrors ReadCommandBroadcast(string command, out string returnData, int readDelayMs = 100)
        {
            returnData = "";
            if (Emulation)
            {
                return ValveErrors.Success;
            }

            return Connection.ReadCommand(command, '*', out returnData, readDelayMs);
        }

        /// <summary>
        /// Gets the current position of the valve.
        /// </summary>
        /// <returns>The position as an int.</returns>
        public abstract int GetPosition();

        /// <summary>
        /// Gets the current position of the valve.
        /// </summary>
        /// <returns>The position as a display string.</returns>
        public abstract string GetPositionDisplay();

        public bool CheckIsMultiPosition()
        {
            var mode = GetHardwareMode();

            return mode == 0 || mode == 3;
        }

        public bool CheckIsTwoPosition()
        {
            var mode = GetHardwareMode();

            return mode != 3;
        }

        /// <summary>
        /// Get hardware mode
        /// </summary>
        /// <returns>'0' for unknown/not supported, '1' = 2-position with stops, '2' = 2-position without stops, '3' = multiposition</returns>
        /// <exception cref="ValveExceptionUnauthorizedAccess"></exception>
        /// <exception cref="ValveExceptionWriteTimeout"></exception>
        /// <exception cref="ValveExceptionReadTimeout"></exception>
        public int GetHardwareMode()
        {
            if (Emulation)
            {
                return 0;
            }

            if (string.IsNullOrWhiteSpace(Version))
            {
                // Need to get the version to determine if it is a Universal Actuator valve
                GetVersion();
            }

            if (string.IsNullOrWhiteSpace(Version) || !IsUniversalActuator)
            {
                return 0;
            }

            var readError = ReadCommand("AM", out var modeString, 100);
            if (readError != ValveErrors.Success)
            {
                switch (readError)
                {
                    case ValveErrors.UnauthorizedAccess:
                        throw new ValveExceptionUnauthorizedAccess();
                    case ValveErrors.TimeoutDuringWrite:
                        throw new ValveExceptionWriteTimeout();
                    case ValveErrors.TimeoutDuringRead:
                        throw new ValveExceptionReadTimeout();
                }
            }

            if (string.IsNullOrWhiteSpace(modeString))
            {
                return 0;
            }

            // AM1 or AM2 are 2-position, AM3 is multiposition
            // The output is the same with legacy mode on the universal actuator.
            var pos = modeString.IndexOf("AM", StringComparison.OrdinalIgnoreCase);
            if (pos >= 0 && int.TryParse(modeString.Substring(pos + 2, 1), out var mode))
            {
                return mode;
            }

            return 0;
        }

        /// <summary>
        /// Returns the name of the device.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return deviceName;
        }

        /// <summary>
        /// Gets or sets the abort event for scheduling.
        /// </summary>
        public System.Threading.ManualResetEvent AbortEvent { get; set; }

        /// <summary>
        /// For reporting status changes within a method operation. Primary usage is by MultiPosition valves, with advanced methods.
        /// </summary>
        protected void SendStatusMessage(StatusReportType statusType, string message)
        {
            StatusUpdate?.Invoke(this, new DeviceStatusEventArgs(Status, statusType.ToString(), this, message));
        }

        protected enum StatusReportType
        {
            PositionChanged,
            CycleCount
        }

        public List<string> GetStatusNotificationList()
        {
            var statusOptions = new List<string>() {"Status Changed" };
            statusOptions.AddRange(Enum.GetNames(typeof(StatusReportType)));
            return statusOptions;
        }

        public List<string> GetErrorNotificationList()
        {
            return new List<string>() { "Valve Position" };
        }

        public DeviceErrorStatus ErrorType { get; set; }

        public DeviceType DeviceType => DeviceType.Component;

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
