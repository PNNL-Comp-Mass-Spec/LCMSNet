//*********************************************************************************************************
// Written by John Ryan, Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 08/17/2009
//
//*********************************************************************************************************

using System;
using System.Text;

namespace LcmsNetPlugins.LabJack
{
    [Serializable]
    public class LabjackU12
    {
        #region Members

        /// <summary>
        /// The Labjack's ID. Defaults to 0.
        /// </summary>
        private int localID;

        private const char CONST_ANALOGPREFIX_1 = 'A';
        private const char CONST_ANALOGPREFIX_2 = 'O';
        private const char CONST_ANALOGPREFIX_3 = 'I';
        private const char CONST_DIGITALPREFIX = 'D';
        private const char CONST_IOPREFIX_1 = 'I';
        private const char CONST_IOPREFIX_2 = 'O';
        private const string CONST_ANALOG_O_PREFIX = "AO";
        private const string CONST_ANALOG_I_PREFIX = "AI";
        private const string CONST_DIGITAL_PREFIX = "D";
        private const string CONST_IO_PREFIX = "IO";
        private const int CONST_ERROR_INVALIDINPUT = 40;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public LabjackU12()
        {
            localID = 0;
            FirmwareVersion = 0;
            DriverVersion = 0;
        }

        /// <summary>
        /// Constructor which specifies ID. Probably won't get used.
        /// </summary>
        /// <param name="labjackID">The labjack's local ID</param>
        public LabjackU12(int labjackID)
        {
            localID = labjackID;
            FirmwareVersion = 0;
            DriverVersion = 0;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the labjack's local ID, which is probably 0.
        /// This doesn't change the hardware ID of the labjack itself, just the ID the software uses to communicate.
        /// </summary>
        public int LocalID
        {
            get { return localID; }
            set { localID = value; }
        }

        /// <summary>
        /// Gets the firmware version, as set by the getFirmwareVersion() function
        /// </summary>
        public float FirmwareVersion { get; private set; }

        /// <summary>
        /// Gets the driver version, as set by the getDriverVersion() function
        /// </summary>
        public float DriverVersion { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// General method for writing to a port
        /// </summary>
        /// <param name="channel">Enumerated port to write to</param>
        /// <param name="value">The value to write (0/1 for digital)</param>
        public void Write(LabjackU12OutputPorts channel, double value)
        {
            var portName = Enum.GetName(typeof(LabjackU12OutputPorts), channel);
            if (portName == null)
                return;

            // Determine which type of port we are writing to
            // AO, D, or IO
            //if (portName[0] == CONST_ANALOGPREFIX_1 && portName[1] == CONST_ANALOGPREFIX_2)
            if (portName.StartsWith(CONST_ANALOG_O_PREFIX))
            {
                // AO = Analog
                WriteAnalog(channel, (float)value);
                //WriteAnalog(portName[2]-'0', (float)value);
                //              ^-Finds the distance from '0' char (converts int to char)
            }
            //else if (portName[0] == CONST_DIGITALPREFIX)
            else if (portName.StartsWith(CONST_DIGITAL_PREFIX))
            {
                // D = Digital
                WriteDigital(channel, (int)value);
                //WriteDigital(portName[1]-'0', (int)value);
            }
            //else if (portName[0] == CONST_IOPREFIX_1 && portName[1] == CONST_IOPREFIX_2)
            else if (portName.StartsWith(CONST_IO_PREFIX))
            {
                // IO = Digital on top of labjack
                WriteIO(channel, (int) value);
                //WriteIO(portName[2] - '0', (int)value);
            }
        }

        /// <summary>
        /// General method for reading from a port
        /// </summary>
        /// <param name="channel">Enumerated port to read from</param>
        /// <returns>The measured value, or -1 if a problem</returns>
        public float Read(LabjackU12InputPorts channel)
        {
            var portName = Enum.GetName(typeof(LabjackU12InputPorts), channel);
            if (portName == null)
            {
                return -1;
            }

            // Determine which type of port we are reading from
            // AI, D, or IO
            //if (portName[0] == CONST_ANALOGPREFIX_1 && portName[1] == CONST_ANALOGPREFIX_3)
            if (portName.StartsWith(CONST_ANALOG_I_PREFIX))
            {
                // AI = Analog
                return ReadAnalog(channel);
                //return (ReadAnalog(portName[2] - '0'));
            }

            //if (portName[0] == CONST_DIGITALPREFIX)
            if (portName.StartsWith(CONST_DIGITAL_PREFIX))
            {
                // D = Digital
                return ReadDigital(channel);
                //return (ReadDigital(portName[1] - '0'));
            }

            //if (portName[0] == CONST_IOPREFIX_1 && portName[1] == CONST_IOPREFIX_2)
            if (portName.StartsWith(CONST_IO_PREFIX))
            {
                // IO = Digital on top of labjack
                return ReadIO(channel);
                //return (ReadIO(portName[2] - '0'));
            }

            return -1;
        }

        /// <summary>
        /// Reads the current voltage on an analog input port (AI0-AI7)
        /// </summary>
        /// <param name="port">The port to read from (</param>
        /// <returns>channel voltage</returns>
        private float ReadAnalog(LabjackU12InputPorts port)
        {
            var channel = int.Parse(port.ToString().Replace(CONST_ANALOG_I_PREFIX, ""));
            return ReadAnalog(channel);
        }

        /// <summary>
        /// Reads the current voltage on an analog input channel (AI0-AI7)
        /// </summary>
        /// <param name="channel">The channel number to read</param>
        /// <returns>channel voltage</returns>
        private float ReadAnalog(int channel)
        {
            var overVoltage = 0;
            var voltage = 0.0f;

            var result = LabJackU12Wrapper.EAnalogIn(ref localID, 0, channel, 0, ref overVoltage, ref voltage);
            if (result != 0)
            {
                var error = GetErrorString(result);
                ThrowErrorMessage("Error reading analog input.  "  + error, result);
            }
            return voltage;
        }

        /// <summary>
        /// Writes a voltage to an analog output channel (AO0 or AO1)
        /// </summary>
        /// <param name="port">The port to write to</param>
        /// <param name="voltage">The voltage to write</param>
        /// <returns>The error message, if applicable</returns>
        private int WriteAnalog(LabjackU12OutputPorts port, float voltage)
        {
            var channel = int.Parse(port.ToString().Replace(CONST_ANALOG_O_PREFIX, ""));
            return WriteAnalog(channel, voltage);
        }

        /// <summary>
        /// Writes a voltage to an analog output channel (AO0 or AO1)
        /// </summary>
        /// <param name="channel">The channel to write to</param>
        /// <param name="voltage">The voltage to write</param>
        /// <returns></returns>
        private int WriteAnalog(int channel, float voltage)
        {
            int result;

            if (channel == 0)
            {
                result = LabJackU12Wrapper.EAnalogOut(ref localID, 0, voltage, 0.0f);
            }
            else if (channel == 1)
            {
                result = LabJackU12Wrapper.EAnalogOut(ref localID, 0, 0.0f, voltage);
            }
            else
            {
                result = CONST_ERROR_INVALIDINPUT; //Error code 40 - Invalid Input
            }

            if (result != 0)
            {
                var error = GetErrorString(result);
                ThrowErrorMessage("Error writing analog output.  " + error, result);
            }

            return result;
        }

        /// <summary>
        /// Reads the current state of one of the digital ports
        /// </summary>
        /// <param name="port">The port to read from (</param>
        /// <returns>The state of the channel</returns>
        private int ReadDigital(LabjackU12InputPorts port)
        {
            var channel = int.Parse(port.ToString().Replace(CONST_DIGITAL_PREFIX, ""));
            return ReadDigital(channel);
        }

        /// <summary>
        /// Reads the current state of one of the digital channels
        /// </summary>
        /// <param name="channel">The channel to read from (</param>
        /// <returns>The state of the channel</returns>
        private int ReadDigital(int channel)
        {
            var state = 0;

            var result = LabJackU12Wrapper.EDigitalIn(ref localID, 0, channel, 1, ref state);
            if (result != 0)
            {
                var error = GetErrorString(result);
                ThrowErrorMessage("Error reading digital input.  " + error, result);
            }
            return state;
        }

        /// <summary>
        /// Reads the current state of one of the IO ports
        /// </summary>
        /// <param name="port">The port to read from (</param>
        /// <returns>The state of the channel</returns>
        private int ReadIO(LabjackU12InputPorts port)
        {
            var channel = int.Parse(port.ToString().Replace(CONST_IO_PREFIX, ""));
            return ReadIO(channel);
        }

        /// <summary>
        /// Reads the current state of one of the IO channels (digital on top)
        /// </summary>
        /// <param name="channel">The channel to read from</param>
        /// <returns>The state of the channel</returns>
        private int ReadIO(int channel)
        {
            var state = 0;

            var result = LabJackU12Wrapper.EDigitalIn(ref localID, 0, channel, 0, ref state);
            if (result != 0)
            {
                var error = GetErrorString(result);
                ThrowErrorMessage("Error reading digital input.  " + error, result);
            }

            return state;
        }

        /// <summary>
        /// Writes a state to an digital port (digital on top)
        /// </summary>
        /// <param name="port">The port to write to</param>
        /// <param name="state">The state (0/1)</param>
        /// <returns>The error message, if applicable</returns>
        private int WriteDigital(LabjackU12OutputPorts port, int state)
        {
            var channel = int.Parse(port.ToString().Replace(CONST_DIGITAL_PREFIX, ""));
            return WriteDigital(channel, state);
        }

        /// <summary>
        /// Writes a state to a digital channel
        /// </summary>
        /// <param name="channel">The channel to write to</param>
        /// <param name="state">The state (0/1)</param>
        /// <returns>The error message, if applicable</returns>
        private int WriteDigital(int channel, int state)
        {
            var result = LabJackU12Wrapper.EDigitalOut(ref localID, 0, channel, 1, state);
            if (result != 0)
            {
                var error = GetErrorString(result);
                ThrowErrorMessage("Error setting digital output.  " + error, result);
            }
            return result;
        }

        /// <summary>
        /// Writes a state to an IO port (digital on top)
        /// </summary>
        /// <param name="port">The port to write to</param>
        /// <param name="state">The state (0/1)</param>
        /// <returns>The error message, if applicable</returns>
        private int WriteIO(LabjackU12OutputPorts port, int state)
        {
            var channel = int.Parse(port.ToString().Replace(CONST_IO_PREFIX, ""));
            return WriteIO(channel, state);
        }

        /// <summary>
        /// Writes a state to an IO channel (digital on top)
        /// </summary>
        /// <param name="channel">The channel to write to</param>
        /// <param name="state">The state (0/1)</param>
        /// <returns>The error message, if applicable</returns>
        private int WriteIO(int channel, int state)
        {
            var result = LabJackU12Wrapper.EDigitalOut(ref localID, 0, channel, 0, state);
            if (result != 0)
            {
                var error = GetErrorString(result);
                ThrowErrorMessage("Error setting digital output.  " + error, result);
            }
            return result;
        }

        /// <summary>
        /// Gets the current Labjack driver version
        /// </summary>
        /// <returns>The driver version, as a float</returns>
        public float GetDriverVersion()
        {
            var tempVersion = LabJackU12Wrapper.GetDriverVersion();
            DriverVersion = tempVersion;
            if (Math.Abs(tempVersion) < float.Epsilon)
            {
                ThrowErrorMessage("Unable to get driver version.", 12);
            }
            return (tempVersion);
        }

        /// <summary>
        /// Deciphers the Labjack error codes to a string describing the error
        /// </summary>
        /// <param name="errorCode">The integer error code</param>
        /// <returns>The error description, as a string</returns>
        public string GetErrorString(int errorCode)
        {
            var tempBuilder = new StringBuilder(100);
            LabJackU12Wrapper.GetErrorString(errorCode, tempBuilder);
            return (tempBuilder.ToString());
        }

        /// <summary>
        /// Gets the current Labjack firmware version
        /// </summary>
        /// <returns>The firmware version, as a float</returns>
        public float GetFirmwareVersion()
        {
            var tempVersion = LabJackU12Wrapper.GetFirmwareVersion(ref localID);
            FirmwareVersion = tempVersion;
            return (tempVersion);
        }

        /// <summary>
        /// Deals with errors in the above functions
        /// </summary>
        /// <param name="msg">A message describing where the error occured</param>
        /// <param name="errorCode">The integer errorcode</param>
        private void ThrowErrorMessage(string msg, int errorCode)
        {
            var errorString = new StringBuilder(50);
            LabJackU12Wrapper.GetErrorString(errorCode, errorString);
            throw new LabjackU12Exception(msg + ":\r\n\r\n" + errorString);
        }

        #endregion
    }
}
