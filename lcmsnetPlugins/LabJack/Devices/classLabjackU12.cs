//*********************************************************************************************************
// Written by John Ryan, Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 08/17/2009
//
// Last modified 1/10/2014
//*********************************************************************************************************

using System;
using System.Text;

namespace LcmsNet.Devices.ContactClosure
{
    [Serializable]
    public class classLabjackU12
    {
        #region Members

        /// <summary>
        /// The Labjack's ID. Defaults to 0.
        /// </summary>
        private int m_localID;
        private float m_firmwareVersion;
        private float m_driverVersion;

        private const char CONST_ANALOGPREFIX_1 = 'A';
        private const char CONST_ANALOGPREFIX_2 = 'O';
        private const char CONST_ANALOGPREFIX_3 = 'I';
        private const char CONST_DIGITALPREFIX = 'D';
        private const char CONST_IOPREFIX_1 = 'I';
        private const char CONST_IOPREFIX_2 = 'O';
        private const int CONST_ERROR_INVALIDINPUT = 40;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public classLabjackU12()
        {
            m_localID = 0;
            m_firmwareVersion = 0;
            m_driverVersion = 0;
        }

        /// <summary>
        /// Constructor which specifies ID. Probably won't get used.
        /// </summary>
        /// <param name="labjackID">The labjack's local ID</param>
        public classLabjackU12(int labjackID)
        {
            m_localID = labjackID;
            m_firmwareVersion = 0;
            m_driverVersion = 0;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the labjack's local ID, which is probably 0.
        /// This doesn't change the hardware ID of the labjack itself, just the ID the software uses to communicate.
        /// </summary>
        public int LocalID
        {
            get
            {
                return m_localID;
            }
            set
            {
                m_localID = value;
            }
        }

        /// <summary>
        /// Gets the firmware version, as set by the getFirmwareVersion() function
        /// </summary>
        public float FirmwareVersion
        {
            get
            {
                return m_firmwareVersion;
            }
        }

        /// <summary>
        /// Gets the driver version, as set by the getDriverVersion() function
        /// </summary>
        public float DriverVersion
        {
            get
            {
                return m_driverVersion;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// General method for writing to a port
        /// </summary>
        /// <param name="channel">Enumerated port to write to</param>
        /// <param name="value">The value to write (0/1 for digital)</param>
        public void Write(enumLabjackU12OutputPorts channel, double value)
        {
            var tempPortName = Enum.GetName(typeof(enumLabjackU12OutputPorts), channel).ToString();

            //Determine which type of port we are writing to
            //AO = Analog
            //D = Digital
            //IO = Digital on top of labjack
            if (tempPortName[0] == CONST_ANALOGPREFIX_1 && tempPortName[1] == CONST_ANALOGPREFIX_2)
            {   
                WriteAnalog(tempPortName[2]-'0', (float)value);
                //              ^-Finds the distance from '0' char (converts int to char)
            }
            else if (tempPortName[0] == CONST_DIGITALPREFIX)
            {
                WriteDigital(tempPortName[1]-'0', (int)value);
            }
            else if (tempPortName[0] == CONST_IOPREFIX_1 && tempPortName[1] == CONST_IOPREFIX_2)
            {
                WriteIO(tempPortName[2] - '0', (int)value);
            }
        }

        /// <summary>
        /// General method for reading from a port
        /// </summary>
        /// <param name="channel">Enumerated port to read from</param>
        /// <returns>The measured value</returns>
        public float Read(enumLabjackU12InputPorts channel)
        {
            var tempPortName = Enum.GetName(typeof(enumLabjackU12InputPorts), channel).ToString();

            //Determine which type of port we are reading from
            //AI = Analog
            //D = Digital
            //IO = Digital on top of labjack
            if (tempPortName[0] == CONST_ANALOGPREFIX_1 && tempPortName[1] == CONST_ANALOGPREFIX_3)
            {
                return (ReadAnalog(tempPortName[2] - '0'));
            }
            else if (tempPortName[0] == CONST_DIGITALPREFIX)
            {
                return (ReadDigital(tempPortName[1] - '0'));
            }
            else if (tempPortName[0] == CONST_IOPREFIX_1 && tempPortName[1] == CONST_IOPREFIX_2)
            {
                return (ReadIO(tempPortName[2] - '0'));
            }
            else
            {
                return (-1);    //Is this a bad idea? Theoretically, we're never going to be here.
            }
        }

        /// <summary>
        /// Reads the current voltage on an analog input channel (AI0-AI7)
        /// </summary>
        /// <param name="channel">The channel number to read</param>
        /// <returns></returns>
        private float ReadAnalog(int channel)
        {            
            var overVoltage = 0;
            var voltage = 0.0f;

            var result = lj.LabJack.EAnalogIn(ref m_localID, 0, channel, 0, ref overVoltage, ref voltage);
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
        /// <param name="channel">The channel to write to</param>
        /// <param name="voltage">The voltage to write</param>
        /// <returns></returns>
        private int WriteAnalog(int channel, float voltage)
        {
            int result;

            if (channel == 0)
            {
                result = lj.LabJack.EAnalogOut(ref m_localID, 0, voltage, 0.0f);
            }
            else if (channel == 1)
            {
                result = lj.LabJack.EAnalogOut(ref m_localID, 0, 0.0f, voltage);
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
        /// Reads the current state of one of the digital channels
        /// </summary>
        /// <param name="channel">The channel to read from (</param>
        /// <returns>The state of the channel</returns>
        private int ReadDigital(int channel)
        {            
            var state = 0;

            var result = lj.LabJack.EDigitalIn(ref m_localID, 0, channel, 1, ref state);
            if (result != 0)
            {
                var error = GetErrorString(result);
                ThrowErrorMessage("Error reading digital input.  " + error, result);
            }
            return state;
        }

        /// <summary>
        /// Reads the current state of one of the IO channels (digital on top)
        /// </summary>
        /// <param name="channel">The channel to read from</param>
        /// <returns>The state of the channel</returns>
        private int ReadIO(int channel)
        {
            var state = 0;

            var result = lj.LabJack.EDigitalIn(ref m_localID, 0, channel, 0, ref state);
            if (result != 0)
            {
                var error = GetErrorString(result);
                ThrowErrorMessage("Error reading digital input.  " + error, result);
            }

            return state;
        }

        /// <summary>
        /// Writes a state to a digital channel
        /// </summary>
        /// <param name="channel">The channel to write to</param>
        /// <param name="state">The state (0/1)</param>
        /// <returns>The error message, if applicable</returns>
        private int WriteDigital(int channel, int state)
        {
            var result = lj.LabJack.EDigitalOut(ref m_localID, 0, channel, 1, state);
            if (result != 0)
            {
                var error = GetErrorString(result);
                ThrowErrorMessage("Error setting digital output.  " + error, result);
            }
            return result;
        }

        /// <summary>
        /// Writes a state to an IO channel (digital on top)
        /// </summary>
        /// <param name="channel">The channel to write to</param>
        /// <param name="state">The state (0/1)</param>
        /// <returns>The error message, if applicable</returns>
        private int WriteIO(int channel, int state)
        {
            var result = lj.LabJack.EDigitalOut(ref m_localID, 0, channel, 0, state);
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
            var tempVersion = lj.LabJack.GetDriverVersion();
            m_driverVersion = tempVersion;
            if (tempVersion == 0)
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
            lj.LabJack.GetErrorString(errorCode, tempBuilder);
            return (tempBuilder.ToString());
        }

        /// <summary>
        /// Gets the current Labjack firmware version
        /// </summary>
        /// <returns>The firmware version, as a float</returns>
        public float GetFirmwareVersion()
        {
            var tempVersion = lj.LabJack.GetFirmwareVersion(ref m_localID);
            m_firmwareVersion = tempVersion;
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
            lj.LabJack.GetErrorString(errorCode, errorString);
            throw new classLabjackU12Exception(msg + ":\r\n\r\n" + errorString);
        }

        #endregion
    }
}
