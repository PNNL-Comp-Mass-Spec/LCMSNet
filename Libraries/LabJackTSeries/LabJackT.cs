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
