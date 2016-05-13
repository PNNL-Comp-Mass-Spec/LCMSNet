using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using lj;

namespace LcmsNet.Devices.ContactClosure
{
    public class classLabjackU12
    {
        #region Members

        /// <summary>
        /// The Labjack's ID. Defaults to 0.
        /// </summary>
        private int mint_localID;
        private float mfloat_firmwareVersion;
        private float mfloat_driverVersion;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public classLabjackU12()
        {
            mint_localID = 0;
            mfloat_firmwareVersion = 0;
            mfloat_driverVersion = 0;
        }

        /// <summary>
        /// Constructor which specifies ID. Probably won't get used.
        /// </summary>
        /// <param name="labjackID">The labjack's local ID</param>
        public classLabjackU12(int labjackID)
        {
            mint_localID = labjackID;
            mfloat_firmwareVersion = 0;
            mfloat_driverVersion = 0;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the labjack's local ID, which is probably 0. 
        /// This doesn't change the hardware ID of the labjack itself, just the ID the software uses to communicate.
        /// </summary>
        public int localID
        {
            get
            {
                return mint_localID;
            }
            set
            {
                mint_localID = value;
            }
        }

        /// <summary>
        /// Gets the firmware version, as set by the getFirmwareVersion() function
        /// </summary>
        public float firmwareVersion
        {
            get
            {
                return mfloat_firmwareVersion;
            }
        }

        /// <summary>
        /// Gets the driver version, as set by the getDriverVersion() function
        /// </summary>
        public float driverVersion
        {
            get
            {
                return mfloat_driverVersion;
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
            string tempPortName = Enum.GetName(typeof(enumLabjackU12OutputPorts), channel).ToString();

            //Determine which type of port we are writing to
            //AO = Analog
            //D = Digital
            //IO = Digital on top of labjack
            if (tempPortName[0] == 'A' && tempPortName[1] == 'O')
            {
                WriteAnalog(tempPortName[2]-'0', (float)value);
            }
            else if (tempPortName[0] == 'D')
            {
                WriteDigital(tempPortName[1]-'0', (int)value);
            }
            else if (tempPortName[0] == 'I' && tempPortName[1] == 'O')
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
            string tempPortName = Enum.GetName(typeof(enumLabjackU12InputPorts), channel).ToString();

            //Determine which type of port we are reading from
            //AI = Analog
            //D = Digital
            //IO = Digital on top of labjack
            if (tempPortName[0] == 'A' && tempPortName[1] == 'I')
            {
                return (ReadAnalog(tempPortName[2] - '0'));
            }
            else if (tempPortName[0] == 'D')
            {
                return (ReadDigital(tempPortName[1] - '0'));
            }
            else if (tempPortName[0] == 'I' && tempPortName[1] == 'O')
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
            int overVoltage = 0;
            float voltage = 0.0f;

            int result = LabJack.EAnalogIn(ref mint_localID, 0, channel, 0, ref overVoltage, ref voltage);
            if (result != 0)
                ThrowErrorMessage("Error reading analog input", result);
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
                result = LabJack.EAnalogOut(ref mint_localID, 0, voltage, -1);
            }
            else if (channel == 1)
            {
                result = LabJack.EAnalogOut(ref mint_localID, 0, -1, voltage);
            }
            else
            {
                result = 40; //Error code 40 - Invalid Input
            }
            if (result != 0)
                ThrowErrorMessage("Error reading analog input", result);

            return result;


            
        }

        /// <summary>
        /// Reads the current state of one of the digital channels
        /// </summary>
        /// <param name="channel">The channel to read from (</param>
        /// <returns>The state of the channel</returns>
        private int ReadDigital(int channel)
        {            
            int state = 0;

            int result = LabJack.EDigitalIn(ref mint_localID, 0, channel, 1, ref state);
            if (result != 0)
                ThrowErrorMessage("Error reading digital input", result);

            return state;
        }

        /// <summary>
        /// Reads the current state of one of the IO channels (digital on top)
        /// </summary>
        /// <param name="channel">The channel to read from</param>
        /// <returns>The state of the channel</returns>
        private int ReadIO(int channel)
        {
            int state = 0;

            int result = LabJack.EDigitalIn(ref mint_localID, 0, channel, 0, ref state);
            if (result != 0)
                ThrowErrorMessage("Error reading digital input", result);

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
            int result = LabJack.EDigitalOut(ref mint_localID, 0, channel, 1, state);
            if (result != 0)
                ThrowErrorMessage("Error setting digital output", result);
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
            int result = LabJack.EDigitalOut(ref mint_localID, 0, channel, 0, state);
            if (result != 0)
                ThrowErrorMessage("Error setting digital output", result);
            return result;
        }

        /// <summary>
        /// Gets the current Labjack driver version
        /// </summary>
        /// <returns>The driver version, as a float</returns>
        public float GetDriverVersion()
        {
            float tempVersion = LabJack.GetDriverVersion();
            mfloat_driverVersion = tempVersion;
            if (tempVersion == 0)
            {
                ThrowErrorMessage("Unable to get driver version", 12);
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
            StringBuilder tempBuilder = new StringBuilder(100);
            LabJack.GetErrorString(errorCode, tempBuilder);
            return (tempBuilder.ToString());
        }

        /// <summary>
        /// Gets the current Labjack firmware version
        /// </summary>
        /// <returns>The firmware version, as a float</returns>
        public float GetFirmwareVersion()
        {
            float tempVersion = LabJack.GetFirmwareVersion(ref mint_localID);
            mfloat_firmwareVersion = tempVersion;
            return (tempVersion);
        }

        /// <summary>
        /// Deals with errors in the above functions
        /// </summary>
        /// <param name="msg">A message describing where the error occured</param>
        /// <param name="errorCode">The integer errorcode</param>
        private void ThrowErrorMessage(string msg, int errorCode)
        {
            StringBuilder errorString = new StringBuilder(50);
            LabJack.GetErrorString(errorCode, errorString);
            throw new classLabjackU12Exception(msg + ":\r\n\r\n" + errorString);
        }

        #endregion

    }
}
