using System;
using LabJack;

namespace LabJackTSeries
{
    /// <summary>
    /// LabJack T Series base class
    /// </summary>
    [Serializable]
    public abstract class LabJackT : IDisposable
    {
        /// Example code at https://github.com/labjack/DotNet_LJM_Examples
        // https://support.labjack.com/docs/is-ljm-thread-safe
        // https://support.labjack.com/docs/sharing-a-particular-device-among-multiple-process
        // In summary, the device can only be opened by one device at a time, but calls are synchronized by device handle, so those calls are thread-safe
        // https://support.labjack.com/docs/can-i-write-an-ljm-program-without-a-device-presen - demo mode
        // https://support.labjack.com/docs/what-ljm-files-are-installed-on-my-machine - installed files

        private bool initialized = false;
        private LabJackDeviceReference labJackDeviceRef;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="deviceType">LabJack device type. Must match one of the values from the constants LJM.CONSTANTS.dtXXXX.</param>
        protected LabJackT(int deviceType)
        {
            DeviceType = deviceType;
            LabJackIdentifier = "";
            HardwareVersion = 0;
            FirmwareVersion = 0;
            DriverVersion = "";
        }

        /// <summary>
        /// Constructor which specifies ID. Probably won't get used.
        /// </summary>
        /// <param name="deviceType">LabJack device type. Must match one of the values from the constants LJM.CONSTANTS.dtXXXX.</param>
        /// <param name="identifier">The LabJack's local ID</param>
        protected LabJackT(int deviceType, string identifier)
        {
            LabJackIdentifier = identifier;
            HardwareVersion = 0;
            FirmwareVersion = 0;
            DriverVersion = "";
        }

        /// <summary>
        /// LabJack device type. Must match one of the values from the constants LJM.CONSTANTS.dtXXXX.
        /// </summary>
        protected int DeviceType { get; } = LJM.CONSTANTS.dtTSERIES;

        /// <summary>
        /// Gets or sets the LabJack's local ID, which is probably 0.
        /// This doesn't change the hardware ID of the LabJack itself, just the ID the software uses to communicate.
        /// </summary>
        public string LabJackIdentifier { get; set; }

        /// <summary>
        /// Gets the hardware version, as set by the GetHardwareVersion() function
        /// </summary>
        public double HardwareVersion { get; private set; }

        /// <summary>
        /// Gets the firmware version, as set by the GetFirmwareVersion() function
        /// </summary>
        public double FirmwareVersion { get; private set; }

        /// <summary>
        /// Gets the driver version, as set by the GetDriverVersion() function
        /// </summary>
        public string DriverVersion { get; private set; }

        /// <summary>
        /// Opens a handle to the LabJack device, or gets an already-open handle
        /// </summary>
        public bool Initialize()
        {
            labJackDeviceRef = LabJackDeviceReference.GetHandle(DeviceType, LabJackIdentifier);

            if (labJackDeviceRef != null)
            {
                // TODO: Throw some exception?
                initialized = true;
            }

            return initialized;
        }

        /// <summary>
        /// Closes the handle to the LabJack device, if not in use by other instances.
        /// </summary>
        /// <remarks>
        /// Differs from <see cref="Dispose"/> by the implied meaning of the method.
        /// 'Dispose' means 'don't use this object again.
        /// 'Close' implies the object could be re-opened.</remarks>
        public void Close()
        {
            if (initialized)
            {
                labJackDeviceRef.DisposeReference();
                labJackDeviceRef = null;
                initialized = false;
            }
        }

        private void ReleaseUnmanagedResources()
        {
            if (initialized)
            {
                labJackDeviceRef.DisposeReference();
                initialized = false;
            }
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~LabJackT()
        {
            ReleaseUnmanagedResources();
        }

        /// <summary>
        /// General method for reading from a port
        /// </summary>
        /// <param name="channel">Enumerated port to read from</param>
        /// <returns>The measured value, or -1 if a problem</returns>
        public double Read(LabJackTGeneralRegisters channel)
        {
            var portName = Enum.GetName(typeof(LabJackTGeneralRegisters), channel);
            if (portName == null)
            {
                return -1;
            }

            return ReadRegisterByName((LabJackTGeneralRegisters)(int)channel);
        }

        /// <summary>
        /// Reads the current state/voltage of one of the inputs
        /// </summary>
        /// <param name="register">The register to read from</param>
        /// <returns>The state (digital, 0/1) or voltage (analog) of the channel</returns>
        protected double ReadRegisterByName(LabJackTGeneralRegisters register)
        {
            var registerName = Enum.GetName(typeof(LabJackTGeneralRegisters), register);
            if (registerName == null)
            {
                return -1;
            }

            return ReadRegisterByName(registerName);
        }

        /// <summary>
        /// Reads the current state/voltage of one of the inputs
        /// </summary>
        /// <param name="registerName">register to read from</param>
        /// <returns>The state (digital, 0/1) or voltage (analog) of the channel</returns>
        protected double ReadRegisterByName(string registerName)
        {
            var value = 0.0;

            var result = LJM.eReadName(labJackDeviceRef.Handle, registerName, ref value);

            if (result != LJM.LJMERROR.NOERROR)
            {
                var error = GetErrorString((int)result);
                ThrowErrorMessage("Error reading input.  " + error, (int)result);
                return -1;
            }

            return value;
        }

        /// <summary>
        /// Reads the current state/voltage of one of the inputs
        /// </summary>
        /// <param name="register">The register to read from</param>
        /// <returns>The state (digital, 0/1) or voltage (analog) of the channel</returns>
        protected double ReadRegisterByAddress(int register)
        {
            var value = 0.0;

            var result = LJM.eReadAddress(labJackDeviceRef.Handle, register, register < 2000 ? LJM.CONSTANTS.FLOAT32 : LJM.CONSTANTS.UINT16, ref value);

            if (result != LJM.LJMERROR.NOERROR)
            {
                var error = GetErrorString((int)result);
                ThrowErrorMessage("Error reading input.  " + error, (int)result);
            }

            return value;
        }

        /// <summary>
        /// Set the state/voltage of one of the outputs
        /// </summary>
        /// <param name="registerName">The register to write to</param>
        /// <param name="value">Digital state (0/1) or analog voltage</param>
        /// <returns>The error message, if applicable</returns>
        protected int WriteRegisterByName(string registerName, double value)
        {
            var result = LJM.eWriteName(labJackDeviceRef.Handle, registerName, value);

            if (result != LJM.LJMERROR.NOERROR)
            {
                var error = GetErrorString((int)result);
                ThrowErrorMessage("Error setting output.  " + error, (int)result);
            }

            return (int)result;
        }

        /// <summary>
        /// General method for writing to a port
        /// </summary>
        /// <param name="name">Enumerated port to write to</param>
        /// <param name="value">The value to write (0/1 for digital)</param>
        public void Write(LabJackTGeneralRegisters name, double value)
        {
            var portName = Enum.GetName(typeof(LabJackTGeneralRegisters), name);
            if (portName == null)
                return;

            WriteRegisterByName((LabJackTGeneralRegisters)(int)name, value);
        }

        /// <summary>
        /// Set the state/voltage of one of the outputs
        /// </summary>
        /// <param name="register">The register to write to</param>
        /// <param name="value">Digital state (0/1) or analog voltage</param>
        /// <returns>The error message, if applicable</returns>
        protected int WriteRegisterByName(LabJackTGeneralRegisters register, double value)
        {
            var registerName = Enum.GetName(typeof(LabJackTGeneralRegisters), register);
            if (registerName == null)
            {
                return -1;
            }

            return WriteRegisterByName(registerName, value);
        }

        /// <summary>
        /// Set the state/voltage of one of the outputs
        /// </summary>
        /// <param name="register">The register to write to</param>
        /// <param name="value">Digital state (0/1) or analog voltage</param>
        /// <returns>The error message, if applicable</returns>
        protected int WriteRegisterByAddress(int register, double value)
        {
            var result = LJM.eWriteAddress(labJackDeviceRef.Handle, register, register < 2000 ? LJM.CONSTANTS.FLOAT32 : LJM.CONSTANTS.UINT16, value);

            if (result != LJM.LJMERROR.NOERROR)
            {
                var error = GetErrorString((int)result);
                ThrowErrorMessage("Error setting output.  " + error, (int)result);
            }

            return (int)result;
        }

        protected int ConfigureAINForThermocouple(int ainNumber, string cjcRegisterName = "TEMPERATURE_DEVICE_K", char thermocoupleType = 'K', bool differential = true)
        {
            if (!Enum.TryParse(char.ToUpper(thermocoupleType).ToString(), out ThermocoupleEFIndex thermocoupleEFIndex))
            {
                throw new Exception($"Unsupported thermocouple type: {thermocoupleType}");
            }

            var result = LJM.LJMERROR.NOERROR;
            var registerName = $"AIN{ainNumber}";
            // TODO: Set resolution for analog input(s) to max for device: https://support.labjack.com/docs/14-0-analog-inputs-t-series-datasheet
            if (differential)
            {
                var ainNegativeChannelRegisterName = $"{registerName}_NEGATIVE_CHANNEL";
                var negativeChannelNumber = ainNumber + 1;
                if (ainNumber >= 16)
                {
                    negativeChannelNumber = ainNumber + 8;
                }

                result = LJM.eWriteName(labJackDeviceRef.Handle, ainNegativeChannelRegisterName, negativeChannelNumber);
            }

            if (result != LJM.LJMERROR.NOERROR)
            {
                var error = GetErrorString((int)result);
                ThrowErrorMessage("Error configuring input.  " + error, (int)result);
            }

            var cjcAddress = 0;
            var cjcValueType = 0;
            result = LJM.NameToAddress(cjcRegisterName, ref cjcAddress, ref cjcValueType);

            if (result != LJM.LJMERROR.NOERROR)
            {
                var error = GetErrorString((int)result);
                ThrowErrorMessage("Error configuring input.  " + error, (int)result);
            }

            var result1 = LJM.eWriteName(labJackDeviceRef.Handle, $"{registerName}_EF_INDEX", (int)thermocoupleEFIndex); // feature index for thermocouple type
            var result2 = LJM.eWriteName(labJackDeviceRef.Handle, $"{registerName}_EF_CONFIG_B", cjcAddress); // modbus address for CJC
            var result3 = LJM.eWriteName(labJackDeviceRef.Handle, $"{registerName}_EF_CONFIG_D", 1.0); // Slope for CJC reading, default; for LM34 use 55.56
            var result4 = LJM.eWriteName(labJackDeviceRef.Handle, $"{registerName}_EF_CONFIG_E", 0.0); // Offset for CJC reading, default; for LM34 use 255.37


            if (result1 != LJM.LJMERROR.NOERROR)
            {
                var error = GetErrorString((int)result1);
                ThrowErrorMessage("Error configuring input.  " + error, (int)result1);
            }

            if (result2 != LJM.LJMERROR.NOERROR)
            {
                var error = GetErrorString((int)result2);
                ThrowErrorMessage("Error configuring input.  " + error, (int)result2);
            }

            if (result3 != LJM.LJMERROR.NOERROR)
            {
                var error = GetErrorString((int)result3);
                ThrowErrorMessage("Error configuring input.  " + error, (int)result3);
            }

            if (result4 != LJM.LJMERROR.NOERROR)
            {
                var error = GetErrorString((int)result4);
                ThrowErrorMessage("Error configuring input.  " + error, (int)result4);
            }

            return (int)result1;
        }

        private enum ThermocoupleEFIndex
        {
            E = 20,
            J = 21,
            K = 22,
            R = 23,
            T = 24,
            S = 25,
            N = 27,
            B = 28,
            C = 30,
        }

        /// <summary>
        /// Set the duty cycle (% on time) of a digital output configured for PWM
        /// </summary>
        /// <param name="dioIndex">Digital output index</param>
        /// <param name="dutyPercent">Duty cycle/"% on time" of the output, from 0 to 100</param>
        /// <returns></returns>
        public int SetDIOPWMDutyCycle(uint dioIndex, double dutyPercent)
        {
            if (dutyPercent < 0)
            {
                dutyPercent = 0;
            }

            if (dutyPercent > 100)
            {
                dutyPercent = 100;
            }

            var clockSource = $"DIO{dioIndex}_EF_CLOCK_SOURCE";
            var config = $"DIO{dioIndex}_EF_CONFIG_A";

            var clockIndex = 0d;
            var read1 = LJM.eReadName(labJackDeviceRef.Handle, clockSource, ref clockIndex);

            if (read1 != LJM.LJMERROR.NOERROR)
            {
                var error = GetErrorString((int)read1);
                ThrowErrorMessage("Error reading clock index.  " + error, (int)read1);
                return -1;
            }

            var clockRoll = $"DIO_EF_CLOCK{(int)clockIndex}_ROLL_VALUE";
            var clockRollValue = 0d;

            var read2 = LJM.eReadName(labJackDeviceRef.Handle, clockRoll, ref clockRollValue);

            if (read2 != LJM.LJMERROR.NOERROR)
            {
                var error = GetErrorString((int)read2);
                ThrowErrorMessage("Error reading clock roll value.  " + error, (int)read2);
                return -1;
            }

            var configValue = dutyPercent / 100.0 * clockRollValue;

            var result = LJM.eWriteName(labJackDeviceRef.Handle, config, (int)configValue); // Set the duty cycle

            if (result != LJM.LJMERROR.NOERROR)
            {
                var error = GetErrorString((int)result);
                ThrowErrorMessage("Error configuring digital output.  " + error, (int)result);
            }

            return (int)result;
        }

        /// <summary>
        /// Configure a digital output for PWM use (note: See LabJack data sheet for model to determine if output supports PWM)
        /// </summary>
        /// <param name="dioIndex">Digital output index</param>
        /// <param name="clockIndex">Clock index of the clock to use</param>
        /// <returns></returns>
        public int ConfigureDIOForPWMOut(uint dioIndex, uint clockIndex = 0)
        {
            if (clockIndex > 2)
            {
                return (int)LJM.LJMERROR.INVALID_NAME;
            }

            var enable = $"DIO{dioIndex}_EF_ENABLE";
            var index = $"DIO{dioIndex}_EF_INDEX";
            var clockSource = $"DIO{dioIndex}_EF_CLOCK_SOURCE";
            var config = $"DIO{dioIndex}_EF_CONFIG_A";
            var result1 = LJM.eWriteName(labJackDeviceRef.Handle, enable, 0); // Disable the DIO extended feature
            var result2 = LJM.eWriteName(labJackDeviceRef.Handle, index, 0); // Set the feature index to '0' (PWM)
            var result3 = LJM.eWriteName(labJackDeviceRef.Handle, clockSource, clockIndex); // Set the index of the clock to use
            var result4 = LJM.eWriteName(labJackDeviceRef.Handle, config, 0); // Set the high->low transition point to 0 (PWM 'off' for the output
            var result5 = LJM.eWriteName(labJackDeviceRef.Handle, enable, 1); // Enable the DIO extended feature

            if (result1 != LJM.LJMERROR.NOERROR)
            {
                var error = GetErrorString((int)result1);
                ThrowErrorMessage("Error configuring digital output.  " + error, (int)result1);
            }

            if (result2 != LJM.LJMERROR.NOERROR)
            {
                var error = GetErrorString((int)result2);
                ThrowErrorMessage("Error configuring digital output.  " + error, (int)result2);
            }

            if (result3 != LJM.LJMERROR.NOERROR)
            {
                var error = GetErrorString((int)result3);
                ThrowErrorMessage("Error configuring digital output.  " + error, (int)result3);
            }

            if (result4 != LJM.LJMERROR.NOERROR)
            {
                var error = GetErrorString((int)result4);
                ThrowErrorMessage("Error configuring digital output.  " + error, (int)result4);
            }

            if (result5 != LJM.LJMERROR.NOERROR)
            {
                var error = GetErrorString((int)result5);
                ThrowErrorMessage("Error configuring digital output.  " + error, (int)result5);
            }

            return (int)result1;
        }

        /// <summary>
        /// Configure a clock
        /// </summary>
        /// <param name="clockIndex">Clock index; 0 is 32-bit, 1 and 2 are 16-bit</param>
        /// <param name="divisor">Frequency divider: divide frequency by this value</param>
        /// <param name="rollValue">value at which the clock should reset; '0' for max value (with divisor, controls the output frequency</param>
        /// <param name="resultFrequencyHz">Computed result frequency; T4/T7 ref clock is 80 MHz, T8 is 100MHz, this is (ref clock) / (divisor * rollValue)</param>
        /// <returns></returns>
        public int ConfigureClockSource(uint clockIndex, LabJackTClockDivisor divisor, uint rollValue, out double resultFrequencyHz)
        {
            resultFrequencyHz = 0;
            if (clockIndex > 2)
            {
                return (int)LJM.LJMERROR.INVALID_NAME;
            }

            if (clockIndex > 0 && rollValue > 65535)
            {
                return (int)LJM.LJMERROR.INVALID_VALUE;
            }

            var refClock = 8e7; // 80 MHz
            var devType = 0;
            var connType = 0;
            var ipAddr = 0;
            var serialNum = 0;
            var port = 0;
            var maxBytesPerMB = 0;
            LJM.GetHandleInfo(labJackDeviceRef.Handle, ref devType, ref connType, ref serialNum, ref ipAddr, ref port, ref maxBytesPerMB);

            if (devType == LJM.CONSTANTS.dtT8)
            {
                refClock = 1e8; // 100 MHz
            }

            var enable = $"DIO_EF_CLOCK{clockIndex}_ENABLE";
            var div = $"DIO_EF_CLOCK{clockIndex}_DIVISOR";
            var roll = $"DIO_EF_CLOCK{clockIndex}_ROLL_VALUE";

            resultFrequencyHz = refClock / ((int)divisor * rollValue);

            var result1 = LJM.eWriteName(labJackDeviceRef.Handle, enable, 0); // Disable the clock
            var result2 = LJM.eWriteName(labJackDeviceRef.Handle, div, (int)divisor); // Set the divisor
            var result3 = LJM.eWriteName(labJackDeviceRef.Handle, roll, rollValue); // Set the roll (reset) value
            var result4 = LJM.eWriteName(labJackDeviceRef.Handle, enable, 1); // Enable the clock

            if (result1 != LJM.LJMERROR.NOERROR)
            {
                var error = GetErrorString((int)result1);
                ThrowErrorMessage("Error configuring clock.  " + error, (int)result1);
            }

            if (result2 != LJM.LJMERROR.NOERROR)
            {
                var error = GetErrorString((int)result2);
                ThrowErrorMessage("Error configuring clock.  " + error, (int)result2);
            }

            if (result3 != LJM.LJMERROR.NOERROR)
            {
                var error = GetErrorString((int)result3);
                ThrowErrorMessage("Error configuring clock.  " + error, (int)result3);
            }

            if (result4 != LJM.LJMERROR.NOERROR)
            {
                var error = GetErrorString((int)result4);
                ThrowErrorMessage("Error configuring clock.  " + error, (int)result4);
            }

            return (int)result1;
        }

        /// <summary>
        /// Gets the current LabJack driver version
        /// </summary>
        /// <returns>The driver version, as a float</returns>
        public string GetDriverVersion()
        {
            DriverVersion = LJM.LABJACK_LJM_VERSION;
            return DriverVersion;
        }

        /// <summary>
        /// Deciphers the LabJack error codes to a string describing the error
        /// </summary>
        /// <param name="errorCode">The integer error code</param>
        /// <returns>The error description, as a string</returns>
        public string GetErrorString(int errorCode)
        {
            var text = "";

            LJM.ErrorToString(errorCode, ref text);

            return text;
        }

        /// <summary>
        /// Gets the current LabJack hardware version
        /// </summary>
        /// <returns>The hardware version, as a double</returns>
        public double GetHardwareVersion()
        {
            if (!initialized)
            {
                return 0;
            }

            HardwareVersion = ReadRegisterByName(LabJackTGeneralRegisters.HARDWARE_VERSION);
            return HardwareVersion;
        }

        /// <summary>
        /// Gets the current LabJack firmware version
        /// </summary>
        /// <returns>The firmware version, as a double</returns>
        public double GetFirmwareVersion()
        {
            if (!initialized)
            {
                return 0;
            }

            FirmwareVersion = ReadRegisterByName(LabJackTGeneralRegisters.FIRMWARE_VERSION);
            return FirmwareVersion;
        }

        /// <summary>
        /// Deals with errors in the above functions
        /// </summary>
        /// <param name="msg">A message describing where the error occurred</param>
        /// <param name="errorCode">The integer errorcode</param>
        private void ThrowErrorMessage(string msg, int errorCode)
        {
            var text = "";

            LJM.ErrorToString(errorCode, ref text);

            throw new LabJackTException(msg + ":\r\n\r\n" + text);
        }
    }
}
