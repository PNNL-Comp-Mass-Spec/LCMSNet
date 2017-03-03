//*********************************************************************************************************
// Written by Christopher Walters for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 1/7/2014
//
// Last modified 5/19/2014
//*********************************************************************************************************
using System;
using LabJack.LabJackUD;

namespace LcmsNet.Devices.ContactClosure
{
    

    [Serializable]
    public class classLabjackU3
    {
        #region Members

        // The Labjack's ID. Defaults to 0.
        private int m_localID;
        // The U12 returned float, the U3 returns double
        private double mdouble_firmwareVersion;
        private double mdouble_driverVersion;
        //   U3 driver's ErrorToString requires a char[] of at least 256 characters.
        private const int CONST_ERROR_STRING_BUFFER_SIZE = 256;

        private U3 m_device;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public classLabjackU3():this(0)
        {            
        }

        /// <summary>
        /// Constructor which specifies ID.
        /// </summary>
        /// <param name="labjackID">The labjack's local ID</param>
        public classLabjackU3(int labjackID)
        {
            m_localID            = labjackID;
            mdouble_firmwareVersion = 0;
            mdouble_driverVersion    = 0;
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
        /// Gets the firmware version, as retrieved from the U3
        /// </summary>
        public double FirmwareVersion => mdouble_firmwareVersion;

        /// <summary>
        /// Gets the driver version, as set by the GetDriverVersion() driver function
        /// </summary>
        public double DriverVersion => mdouble_driverVersion;

        #endregion

        public void Initialize()
        {
         
            // Per example code
            // If an ID other than 0 is wanted for the U3 object being initialized
            // it must be changed before calling initialize, or initialze must be called again afterwards.
            m_device = new U3(LJUD.CONNECTION.USB, m_localID.ToString(), true);
            //Start by using the pin_configuration_reset IOType so that all
            //pin assignments are in the factory default condition.
            LJUD.ePut(m_device.ljhandle, LJUD.IO.PIN_CONFIGURATION_RESET, 0, 0, 0);
            
        }

        #region Methods
          
        /// <summary>
        /// General method for writing to a port
        /// </summary>
        /// <param name="channel">Enumerated port to write to</param>
        /// <param name="value">The value to write (0/1 for digital)</param>
        public void Write(enumLabjackU3OutputPorts channel, double value)
        {
            //Determine which type of port we are writing to
            var port = (int)channel;
            if (channel.ToString().EndsWith("Analog"))
            {
                port -= 20; // DAC ports are channel 0 and 1 for the function purposes, but we have 20-21 in enums representing DACs. so correct port number via substracting 20.
                WriteAnalog(port, value);
            }
            else if (channel.ToString().EndsWith("Digital"))
            {                
                WriteDigital(port, Convert.ToInt32(value));
            }         
        }

        /// <summary>
        /// General method for reading from a port
        /// </summary>
        /// <param name="channel">Enumerated port to read from</param>
        /// <returns>The measured value</returns>
        public double Read(enumLabjackU3InputPorts channel)
        {
            var tempPortName = Enum.GetName(typeof(enumLabjackU3InputPorts), channel).ToString();

            //Determine which type of port we are reading from
            var port = (int) channel;
            if (tempPortName.EndsWith("Analog"))
            {
                port -= 20; // analog port configuration are represented by number 20-35 in the enum, but they are the same
                // channel as 0-19 digital, simply configured for analog read. so we correct the port by subtracting 20.
                return (ReadAnalog(port));
            }
            else if (tempPortName.EndsWith("Digital"))
            {
                return (ReadDigital(port));
            }        
            else
            {
                return (-1);    //Is this a bad idea? Theoretically, we're never going to be here.
            }
        }
        private void ValidateDevice()
        {
            if (m_device == null)
            {
                throw new Exception("The labjack is not initialized properly.");
            }
        }
        /// <summary>
        /// Reads the current voltage on an analog input channel (AI0-AI7)
        /// </summary>
        /// <param name="channel">The channel number to read</param>
        /// <returns></returns>
        private double ReadAnalog(int channel)
        {
            var voltage = 0.0;
            var differentialchannel = 31; // not checking for differential so set to 31 as per 4.2.17 of the U3 user guide.
            var range = 0; // ignore on U3
            var resolution = 0; // non-zero for Quicksampling
            var settling = 0; // non-zero for LongSettling
            var binary = 0; // non-zero to return raw binary value of the voltage

            ValidateDevice();

            var result = U3.eAIN(m_device.ljhandle, channel, differentialchannel, ref voltage, range, resolution, settling, binary);
            if (result != LJUD.LJUDERROR.NOERROR)
            {

                var error = GetErrorString(result);
                ThrowErrorMessage("Error reading analog input.  " + error, result);
            }
            return voltage;
        }

        /// <summary>
        /// Writes a voltage to an analog output channel (DAC0 or DAC1)
        /// </summary>
        /// <param name="channel">The channel to write to</param>
        /// <param name="voltage">The voltage to write</param>
        /// <returns></returns>
        private LJUD.LJUDERROR WriteAnalog(int channel, double voltage)
        {
            ValidateDevice();
            LJUD.LJUDERROR result;
            if (channel == 0 || channel == 1)
            {
                var binary = 0; // non-zero to set voltage via binary
                var reserved1 = 0; // not used currently by driver, pass 0 as per 4.2.18 of U3 user guide, same for reserved 2
                var reserved2 = 0;
                result = U3.eDAC(m_device.ljhandle, channel, voltage, binary, reserved1, reserved2);
            }         
            else
            {
                result = LJUD.LJUDERROR.INVALID_PARAMETER; // closest thing I could find to the U12 invalidinput, CONST_ERROR_INVALIDINPUT; //Error code 40 - Invalid Input
            }
            if (result != LJUD.LJUDERROR.NOERROR)
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
            ValidateDevice();
            var state = 0;

            var result = U3.eDI(m_device.ljhandle, channel, ref state);
            if (result !=  LJUD.LJUDERROR.NOERROR)
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
        private LJUD.LJUDERROR WriteDigital(int channel, int state)
        {
            ValidateDevice();
            var result = LJUD.eDO(m_device.ljhandle, channel, state);
            if (result != LJUD.LJUDERROR.NOERROR)
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
        public double GetDriverVersion()
        {
            var tempVersion = LJUD.GetDriverVersion();
            mdouble_driverVersion = tempVersion;
            if (tempVersion == 0)
            {
                ThrowErrorMessage("Unable to get driver version.", LJUD.LJUDERROR.USB_DRIVER_NOT_FOUND);
            }
            return (tempVersion);
        }

        /// <summary>
        /// Deciphers the Labjack error codes to a string describing the error
        /// </summary>
        /// <param name="errorCode">The integer error code</param>
        /// <returns>The error description, as a string</returns>
        public string GetErrorString(LJUD.LJUDERROR errorCode)
        {            
            var errorString = new char[CONST_ERROR_STRING_BUFFER_SIZE];
            U3.ErrorToString(errorCode, errorString);
            var tmpStr = new string(errorString);
            return (tmpStr);
        }

        /// <summary>
        /// Gets the current Labjack firmware version
        /// </summary>
        /// <returns>The firmware version, as a float</returns>
        public double GetFirmwareVersion()
        {
            ValidateDevice();
            var tempVersion = m_device.firmwareversion;
            mdouble_firmwareVersion = tempVersion;
            return (tempVersion);
        }

        /// <summary>
        /// Deals with errors in the above functions
        /// </summary>
        /// <param name="msg">A message describing where the error occured</param>
        /// <param name="errorCode">The integer errorcode</param>
        private void ThrowErrorMessage(string msg, LJUD.LJUDERROR errorCode)
        {
            var errorString = GetErrorString(errorCode);
            throw new classLabjackU3Exception(msg + ":\r\n\r\n" + errorString);
        }

        #endregion

        #region ObsoleteMethods
        /***********************************************************************
        * The WriteIO method may be unnecessary for the U3
        * --Chris
        ***********************************************************************/
        /// <summary>
        /// Writes a state to an IO channel (digital on top)
        /// </summary>
        /// <param name="channel">The channel to write to</param>
        /// <param name="state">The state (0/1)</param>
        /// <returns>The error message, if applicable</returns>
        private LJUD.LJUDERROR WriteIO(int channel, int state)
        {
            ValidateDevice();
            var result = U3.eDO(m_device.ljhandle, channel, state);
            if (result != LJUD.LJUDERROR.NOERROR)
            {
                var error = GetErrorString(result);
                ThrowErrorMessage("Error setting digital output.  " + error, result);
            }
            return result;
        }

        /***********************************************************************
        * The ReadIO method may be unnecessary for the U3
        * --Chris
        ***********************************************************************/
        /// <summary>
        /// Reads the current state of one of the IO channels (digital on top)
        /// </summary>
        /// <param name="channel">The channel to read from</param>
        /// <returns>The state of the channel</returns>
        private int ReadIO(int channel)
        {
            ValidateDevice();
            var state = 0;

            var result = U3.eDI(m_device.ljhandle, channel, ref state);
            if (result != LJUD.LJUDERROR.NOERROR)
            {
                var error = GetErrorString(result);
                ThrowErrorMessage("Error reading digital input.  " + error, result);
            }

            return state;
        }
        #endregion
    }
}
