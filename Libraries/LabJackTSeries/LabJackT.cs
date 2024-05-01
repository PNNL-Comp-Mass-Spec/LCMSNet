using System;
using System.Collections.Generic;
using LabJack;

namespace LabJackTSeries
{
    [Serializable]
    // https://support.labjack.com/docs/is-ljm-thread-safe
    // https://support.labjack.com/docs/sharing-a-particular-device-among-multiple-process
    // In summary, the device can only be opened by one device at a time, but calls are synchronized by device handle, so those calls are thread-safe
    // https://support.labjack.com/docs/can-i-write-an-ljm-program-without-a-device-presen - demo mode
    // https://support.labjack.com/docs/what-ljm-files-are-installed-on-my-machine - installed files
    public class LabJackT : IDisposable
    {
        /// <summary>
        /// The LabJack's ID. Defaults to 0.
        /// </summary>
        /// <remarks>
        /// Example code at https://github.com/labjack/DotNet_LJM_Examples
        /// </remarks>
        private int localID;

        private static readonly Dictionary<int, object> lockObjects = new Dictionary<int, object>(1);
        private static readonly object dictionaryLock = new object();
        private bool initialized = false;
        private int labJackDeviceHandle;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public LabJackT()
        {
            localID = 0;
            HardwareVersion = 0;
            FirmwareVersion = 0;
            DriverVersion = "";

            lock (dictionaryLock)
            {
                if (!lockObjects.ContainsKey(localID))
                {
                    lockObjects.Add(localID, new object());
                }
            }
        }

        /// <summary>
        /// Constructor which specifies ID. Probably won't get used.
        /// </summary>
        /// <param name="labJackID">The LabJack's local ID</param>
        public LabJackT(int labJackID)
        {
            localID = labJackID;
            HardwareVersion = 0;
            FirmwareVersion = 0;
            DriverVersion = "";

            lock (dictionaryLock)
            {
                if (!lockObjects.ContainsKey(localID))
                {
                    lockObjects.Add(localID, new object());
                }
            }
        }

        /// <summary>
        /// Gets or sets the LabJack's local ID, which is probably 0.
        /// This doesn't change the hardware ID of the LabJack itself, just the ID the software uses to communicate.
        /// </summary>
        public int LocalID
        {
            get => localID;
            set => localID = value;
        }

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

        public void Initialize()
        {
            // Per example code
            // If an ID other than 0 is wanted for the U3 object being initialized
            // it must be changed before calling initialize, or initialze must be called again afterwards.
            var err = LJM.Open(LJM.CONSTANTS.dtT7, LJM.CONSTANTS.ctUSB, "ANY", ref labJackDeviceHandle);

            if (err == LJM.LJMERROR.NOERROR)
            {
                initialized = true;
            }
        }

        private void ReleaseUnmanagedResources()
        {
            if (initialized)
            {
                LJM.Close(labJackDeviceHandle);
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

        private object GetLockObject()
        {
            if (lockObjects.TryGetValue(localID, out var lockObj))
            {
                return lockObj;
            }

            lock (dictionaryLock)
            {
                if (!lockObjects.ContainsKey(localID))
                {
                    lockObjects.Add(localID, new object());
                }
            }

            return lockObjects[localID];
        }

        /// <summary>
        /// General method for reading from a port
        /// </summary>
        /// <param name="channel">Enumerated port to read from</param>
        /// <returns>The measured value, or -1 if a problem</returns>
        public double Read(LabJackT7Inputs channel)
        {
            var portName = Enum.GetName(typeof(LabJackT7Inputs), channel);
            if (portName == null)
            {
                return -1;
            }

            return ReadRegisterByName((LabJackT7IONames) (int) channel);
        }

        /// <summary>
        /// Reads the current state/voltage of one of the inputs
        /// </summary>
        /// <param name="register">The register to read from</param>
        /// <returns>The state (digital, 0/1) or voltage (analog) of the channel</returns>
        private double ReadRegisterByName(LabJackT7IONames register)
        {
            var registerName = Enum.GetName(typeof(LabJackT7IONames), register);
            if (registerName == null)
            {
                return -1;
            }

            var value = 0.0;
            LJM.LJMERROR result;

            lock (GetLockObject())
            {
                result = LJM.eReadName(labJackDeviceHandle, registerName, ref value);
            }

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
        private double ReadRegisterByAddress(int register)
        {
            var value = 0.0;
            LJM.LJMERROR result;

            lock (GetLockObject())
            {
                var dblValue = 0.0;
                result = LJM.eReadAddress(labJackDeviceHandle, register, register < 2000 ? LJM.CONSTANTS.FLOAT32 : LJM.CONSTANTS.UINT16, ref dblValue);
            }

            if (result != LJM.LJMERROR.NOERROR)
            {
                var error = GetErrorString((int)result);
                ThrowErrorMessage("Error reading input.  " + error, (int)result);
            }

            return value;
        }

        /// <summary>
        /// General method for writing to a port
        /// </summary>
        /// <param name="name">Enumerated port to write to</param>
        /// <param name="value">The value to write (0/1 for digital)</param>
        public void Write(LabJackT7Outputs name, double value)
        {
            var portName = Enum.GetName(typeof(LabJackT7Outputs), name);
            if (portName == null)
                return;

            WriteRegisterByName((LabJackT7IONames) (int) name, value);
        }

        /// <summary>
        /// Set the state/voltage of one of the outputs
        /// </summary>
        /// <param name="register">The register to write to</param>
        /// <param name="value">Digital state (0/1) or analog voltage</param>
        /// <returns>The error message, if applicable</returns>
        private int WriteRegisterByName(LabJackT7IONames register, double value)
        {
            var registerName = Enum.GetName(typeof(LabJackT7IONames), register);
            if (registerName == null)
            {
                return -1;
            }

            LJM.LJMERROR result;
            lock (GetLockObject())
            {
                result = LJM.eWriteName(labJackDeviceHandle, registerName, value);
            }

            if (result != LJM.LJMERROR.NOERROR)
            {
                var error = GetErrorString((int)result);
                ThrowErrorMessage("Error setting output.  " + error, (int)result);
            }

            return (int)result;
        }

        /// <summary>
        /// Set the state/voltage of one of the outputs
        /// </summary>
        /// <param name="register">The register to write to</param>
        /// <param name="value">Digital state (0/1) or analog voltage</param>
        /// <returns>The error message, if applicable</returns>
        private int WriteRegisterByAddress(int register, double value)
        {
            LJM.LJMERROR result;
            lock (GetLockObject())
            {
                result = LJM.eWriteAddress(labJackDeviceHandle, register, register < 2000 ? LJM.CONSTANTS.FLOAT32 : LJM.CONSTANTS.UINT16, value);
            }

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
            return LJM.LABJACK_LJM_VERSION;
        }

        /// <summary>
        /// Deciphers the LabJack error codes to a string describing the error
        /// </summary>
        /// <param name="errorCode">The integer error code</param>
        /// <returns>The error description, as a string</returns>
        public string GetErrorString(int errorCode)
        {
            var text = "";

            lock (GetLockObject())
            {
                LJM.ErrorToString(errorCode, ref text);
            }

            return text;
        }

        /// <summary>
        /// Gets the current LabJack hardware version
        /// </summary>
        /// <returns>The hardware version, as a double</returns>
        public double GetHardwareVersion()
        {
            HardwareVersion = ReadRegisterByName(LabJackT7IONames.HardwareVersion);
            return HardwareVersion;
        }

        /// <summary>
        /// Gets the current LabJack firmware version
        /// </summary>
        /// <returns>The firmware version, as a double</returns>
        public double GetFirmwareVersion()
        {
            FirmwareVersion = ReadRegisterByName(LabJackT7IONames.FirmwareVersion);
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

            lock (GetLockObject())
            {
                LJM.ErrorToString(errorCode, ref text);
            }

            throw new LabJackTException(msg + ":\r\n\r\n" + text);
        }
    }
}
