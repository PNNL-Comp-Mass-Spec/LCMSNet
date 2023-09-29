using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO.Ports;
using System.Threading;
using System.Xml;
using LcmsNetSDK;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Logging;
using LcmsNetSDK.Method;
using LcmsNetSDK.System;

namespace LcmsNetPlugins.Teledyne.Pumps
{
    /// <summary>
    /// Interface to ISCO pumps
    /// </summary>
    [Serializable]

    [DeviceControl(typeof(IscoPumpViewModel), typeof(IscoPumpGlyph), "ISCO Pump", "Pumps")]
    public class IscoPump : IDevice, IDisposable
    {
        const int CONST_MAX_PUMPS = 3;
        const long CONST_DEFAULT_TIMER_INTERVAL = 10000; //10000ms = 10 seconds = 6 times/min

        // Device name
        string m_Name;

        // Device status
        DeviceStatus m_Status;

        // Emulation mode
        bool m_Emulation;

        // Address for pump controller
        int m_UnitAddr;

        // Flags for initialization
        bool m_Initialized;
        bool m_Initializing = true;

        // timer for autoupdating pump status
        Timer m_refresh;

        // Pump control mode (local/remote)
        IscoControlMode m_ControlMode = IscoControlMode.Local;

        // Pump operation mode (Constant pressure/Constant flow)
        IscoOperationMode m_OpMode = IscoOperationMode.ConstantPressure;

        // Number of pumps connected to this controller (3 max)
        int m_PumpCount;

        // Status data for each pump
        IscoPumpData[] m_PumpData;

        // Maximum ranges for pump parameters
        readonly IscoPumpRangeData[] m_PumpRanges;

        // Setpoint limits for the pumps
        readonly IscoPumpSetpointLimits[] m_SetpointLimits;

        // Serial port properties
        readonly IscoSerPortProps m_PortProps;

        // Serial port object
        SerialPort m_SerialPort;

        /// <summary>
        /// Pump models
        /// </summary>
        readonly ISCOModel[] models; // up to 3 per controller...

        public event DelegateIscoPumpRefreshCompleteHandler RefreshComplete;
        public event DelegateIscoPumpInitializationCompleteHandler InitializationComplete;
        public event DelegateIscoPumpInitializingHandler Initializing;
        public event DelegateIscoPumpControlModeSetHandler ControlModeSet;
        public event DelegateIscoPumpOpModeSetHandler OperationModeSet;
        public event DelegateIscoPumpDisconnected Disconnected;

        /// <summary>
        /// Emulation mode
        /// </summary>
        public bool Emulation
        {
            get => m_Emulation;
            set => m_Emulation = value;
        }

        /// <summary>
        /// Serial port name
        /// </summary>
        [DeviceSavedSetting("PortName")]
        public string PortName
        {
            get => m_SerialPort.PortName;
            set => m_SerialPort.PortName = value;
        }

        public bool IsOpen()
        {
            return m_SerialPort.IsOpen;
        }

        /// <summary>
        /// Serial port baud rate
        /// </summary>
        [DeviceSavedSetting("BaudRate")]
        public int BaudRate
        {
            get => m_SerialPort.BaudRate;
            set => m_SerialPort.BaudRate = value;
        }

        /// <summary>
        /// Serial port read timeout (msec)
        /// </summary>
        [DeviceSavedSetting("ReadTimeout")]
        public int ReadTimeout
        {
            get => m_SerialPort.ReadTimeout;
            set => m_SerialPort.ReadTimeout = value;
        }

        /// <summary>
        /// Serial port write timeout (msec)
        /// </summary>
        [DeviceSavedSetting("WriteTimeout")]
        public int WriteTimeout
        {
            get => m_SerialPort.WriteTimeout;
            set => m_SerialPort.WriteTimeout = value;
        }

        /// <summary>
        /// Class initialization state
        /// </summary>
        public bool Initialized => m_Initialized;

        /// <summary>
        /// Number of pumps connected to this controller (1 min, 3 max)
        /// </summary>
        [DeviceSavedSetting("PumpCount")]
        public int PumpCount
        {
            get => m_PumpCount;
            set => m_PumpCount = value;
        }

        /// <summary>
        /// Address for the pump controller
        /// </summary>
        [DeviceSavedSetting("UnitAddress")]
        public int UnitAddress
        {
            get => m_UnitAddr;
            set
            {
                m_UnitAddr = value;
                m_PortProps.UnitAddress = m_UnitAddr;
            }
        }

        /// <summary>
        /// Minimum pump controller address
        /// </summary>
        public int UnitAddressMin => m_PortProps.UnitAddressMin;

        /// <summary>
        /// Maximum pump controller address
        /// </summary>
        public int UnitAddressMax => m_PortProps.UnitAddressMax;

        /// <summary>
        /// Control mode (local/remote)
        /// </summary>
        public IscoControlMode ControlMode
        {
            get => m_ControlMode;
            set => SetControlMode(value);
        }

        /// <summary>
        /// Operation mode (Const flow/Const pressure)
        /// </summary>
        public IscoOperationMode OperationMode
        {
            get => m_OpMode;
            set => SetOperationMode(value);
        }

        /// <summary>
        /// Current data for all pumps
        /// </summary>
        public IscoPumpData[] PumpData => m_PumpData;

        public IscoPump()
        {
            m_Name = "Isco Pump";

            // Set the default number of pumps as the max
            m_PumpCount = CONST_MAX_PUMPS;

            models = new ISCOModel[CONST_MAX_PUMPS];

            //m_Name = classDeviceManager.Manager.CreateUniqueDeviceName(m_Name);
            // Initialize pump data array
            m_PumpData = new[] { new IscoPumpData(), new IscoPumpData(),
                                        new IscoPumpData() };

            // Initialize setpoint array
            m_SetpointLimits = new[] { new IscoPumpSetpointLimits(),
                                                new IscoPumpSetpointLimits(), new IscoPumpSetpointLimits() };

            // Initialize pump range array
            m_PumpRanges = new[] { new IscoPumpRangeData(),
                                                new IscoPumpRangeData(), new IscoPumpRangeData() };

            m_SerialPort = new SerialPort();

            // Configure the serial port with default settings
            m_PortProps = new IscoSerPortProps();
            ConfigSerialPort(m_PortProps);
        }

        ~IscoPump()
        {
            Dispose();
        }

        public void Dispose()
        {
            m_refresh?.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Converts a numeric (zero-based) pump index into an alpha pump designation
        /// </summary>
        /// <param name="pumpIndx">Index</param>
        /// <param name="pumpAIsBlank">If TRUE, returns "" when pump A is specified</param>
        /// <returns>Empty string, "A", "B", or "C", depending on input enum and pumpAIsBlank setting</returns>
        public string ConvertPumpIndxToString(int pumpIndx, bool pumpAIsBlank)
        {
            string retStr;

            if (pumpAIsBlank && (pumpIndx == 0))
            {
                // Pump A just uses an empty string for the pump designation
                retStr = "";
            }
            else
                retStr = ((char)(pumpIndx + 65)).ToString();   // Pump index is assumed to be zero-based

            return retStr;
        }

        /// <summary>
        /// Extracts the innermost exception from a nested exception
        /// </summary>
        /// <param name="ex">Input exception</param>
        /// <returns>Innermost exception</returns>
        public Exception GetBaseException(Exception ex)
        {
            var retExc = ex;
            if (ex.InnerException != null)
            {
                retExc = ex.InnerException;
                GetBaseException(ex.InnerException);
            }

            return retExc;
        }

        /// <summary>
        /// Gets an object containing data for specified pump
        /// </summary>
        /// <param name="pumpIndx">Index of pump (0-2)</param>
        /// <returns>Object containing pump data</returns>
        public IscoPumpData GetPumpData(int pumpIndx)
        {
            if (pumpIndx < m_PumpData.Length)
            {
                return m_PumpData[pumpIndx];
            }

            return null;
        }

        /// <summary>
        /// Gets an object containing ranges for specified pump
        /// </summary>
        /// <param name="pumpIndx">Index of pump (0-2)</param>
        /// <returns>Object containing range data</returns>
        public IscoPumpRangeData GetPumpRanges(int pumpIndx)
        {
            if (pumpIndx < m_PumpRanges.Length)
            {
                return m_PumpRanges[pumpIndx];
            }

            return null;
        }

        /// <summary>
        /// Gets an object containing setpoint limits for specified pump
        /// </summary>
        /// <param name="pumpIndx">Index of pump (0-2)</param>
        /// <returns>Object containing setpoint limit data</returns>
        public IscoPumpSetpointLimits GetSetpointLimits(int pumpIndx)
        {
            if (pumpIndx < m_SetpointLimits.Length)
            {
                return m_SetpointLimits[pumpIndx];
            }

            return null;
        }

        /// <summary>
        /// Closes serial port and resets selected parameters
        /// </summary>
        public bool Disconnect()
        {
#if DACTEST
                LogMessage("Beginning Disconnect"); // DAC testing
#endif

            if (m_Emulation)
            {
                // Emulation mode - just fake disconnecting
                //m_ControlMode = enumIscoControlMode.Local; // we don't want to do this because it stops the pumps
                m_Initialized = false;
                Disconnected?.Invoke();
                return true;
            }

#if DACTEST
                LogMessage("Disconnect: Testing initialization");   // DAC testing
#endif

            if (m_Initialized)
            {
#if DACTEST
                    LogMessage("Disconnect: Setting control mode to Local");    // DAC testing
#endif
                // Set the control mode to Local
                //SetControlMode(enumIscoControlMode.Local); // we don't want to do this because it stops the pumps.
                // instead we use a check in InitDevice to see if this mode needs to be changed to remote.

#if DACTEST
                    LogMessage("Disconnect: Closing serial port");  // DAC testing
#endif

                // Close the serial port
                try
                {
                    m_SerialPort.Close();
                }
                catch (Exception ex)
                {
                    var args = new DeviceErrorEventArgs("Exception closing serial port during Disconnect",
                                                                GetBaseException(ex), DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                                                IscoErrorNotifications.GetNotificationString(IscoProblemStatus.ComError.ToString()));
                    Error?.Invoke(this, args);
                    return false;
                }

#if DACTEST
                    LogMessage("Disconnect: Setting initialization state to FALSE");    // DAC testing
#endif

                m_Initialized = false;
            }

#if DACTEST
                LogMessage("Disconnect: Firing Disconnected event");    // DAC testing
#endif

            Disconnected?.Invoke();
            return true;
        }

        /// <summary>
        /// Sets pump control mode to local or remote
        /// </summary>
        /// <param name="newMode">New control mode</param>
        /// <returns>TRUE for success; FALSE otherwise</returns>
        public bool SetControlMode(IscoControlMode newMode)
        {
            if (m_Emulation)
            {
                m_ControlMode = newMode;
                ControlModeSet?.Invoke(newMode);
                return true;
            }

            // If already in same mode, no need to change it
            if (m_ControlMode == newMode)
                return true;

            // check initialization state
            if (!(m_Initialized || m_Initializing))
            {
                var args = new DeviceErrorEventArgs("Device not initialized", null,
                                    DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                    IscoErrorNotifications.GetNotificationString(IscoProblemStatus.DeviceNotInitialized.ToString()));
                Error?.Invoke(this, args);
                return false;
            }

            // Build the command
            var cmd = "";
            var notifyString = "";

            switch (newMode)
            {
                case IscoControlMode.External:
                    cmd = "EXTERNAL";
                    notifyString = IscoStatusNotifications.GetNotificationString(newMode.ToString());
                    break;
                case IscoControlMode.Local:
                    cmd = "LOCAL";
                    notifyString = IscoStatusNotifications.GetNotificationString(newMode.ToString());
                    break;
                case IscoControlMode.Remote:
                    cmd = "REMOTE";
                    notifyString = IscoStatusNotifications.GetNotificationString(newMode.ToString());
                    break;
            }

            // Send command to pump controller
            try
            {
                var resp = SendCommand(cmd, true);
                m_ControlMode = newMode;
                var args = new DeviceStatusEventArgs(m_Status, notifyString, this);
                StatusUpdate?.Invoke(this, args);
                ControlModeSet?.Invoke(newMode);
                return true;
            }
            catch (Exception ex)
            {
                var args = new DeviceErrorEventArgs("Exception setting control mode", GetBaseException(ex),
                                            DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                            IscoErrorNotifications.GetNotificationString(IscoProblemStatus.ComError.ToString()));
                Error?.Invoke(this, args);
                ControlModeSet?.Invoke(m_ControlMode);
                return false;
            }
        }

        [LCMethodEvent("Set Mode", MethodOperationTimeoutType.Parameter)]
        public bool SetOperationMode(double timeout, IscoOperationMode newMode)
        {
            return SetOperationMode(newMode);
        }

        /// <summary>
        /// Sets the operation mode for all pumps (Const flow/Const pressure)
        /// </summary>
        /// <param name="newMode">New operation mode</param>
        /// <returns>TRUE for success; FALSE otherwise</returns>
        public bool SetOperationMode(IscoOperationMode newMode)
        {
            if (m_Emulation)
            {
                m_OpMode = newMode;
                OperationModeSet?.Invoke(newMode);
                return true;
            }

            // If already in same mode, no need to change it
            if (m_OpMode == newMode)
                return true;

            var baseCmd = "";
            var notifyString = "";

            // Initialize the command
            switch (newMode)
            {
                case IscoOperationMode.ConstantFlow:
                    baseCmd = "CONST FLOW";
                    notifyString = IscoStatusNotifications.GetNotificationString(newMode.ToString());
                    break;
                case IscoOperationMode.ConstantPressure:
                    baseCmd = "CONST PRESS";
                    notifyString = IscoStatusNotifications.GetNotificationString(newMode.ToString());
                    break;
            }

            var success = true;
            for (var indx = 0; indx < m_PumpCount; indx++)
            {
                // Build the command for each pump
                var cmd = baseCmd + ConvertPumpIndxToString(indx, true);

                // Send command to pump controller
                try
                {
                    var resp = SendCommand(cmd, true);
                }
                catch (Exception ex)
                {
                    var errArgs = new DeviceErrorEventArgs("Exception setting operation mode",
                                                GetBaseException(ex), DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                                IscoErrorNotifications.GetNotificationString(IscoProblemStatus.ComError.ToString()));
                    Error?.Invoke(this, errArgs);
                    success = false;
                }
            }

            // All finished - save and report status
            if (success)
            {
                m_OpMode = newMode;
                var args = new DeviceStatusEventArgs(m_Status, notifyString, this);
                StatusUpdate?.Invoke(this, args);
                OperationModeSet?.Invoke(newMode);
                return true;
            }

            OperationModeSet?.Invoke(m_OpMode);
            return false;
        }

        /// <summary>
        /// Initializes the pump configuration
        /// </summary>
        /// <returns></returns>
        private bool InitDevice(ref string msg)
        {
            string notifyStr;

            Initializing?.Invoke();

            // If in emulation mode, just return
            if (m_Emulation)
                return true;

            // Close and reopen com port (to ensure any settings changes take effect)
            if (!Disconnect())
            {
                msg = "Initialization error during Disconnect";
                notifyStr = IscoErrorNotifications.GetNotificationString(IscoProblemStatus.InitializationError.ToString());
                var args = new DeviceErrorEventArgs(msg, null,
                                            DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                            notifyStr);
                Error?.Invoke(this, args);
                return false;
            }

            m_Initializing = true;

            // Load the com port parameters and open the port
            try
            {
                m_SerialPort.Open();
                if (!m_SerialPort.IsOpen)
                {
                    throw new Exception(string.Format("The serial port on the ISCO pump {0} could not be opened.", m_Name));
                }
            }
            catch (Exception ex)
            {
                msg = "Initialization error when opening serial port";
                notifyStr = IscoErrorNotifications.GetNotificationString(IscoProblemStatus.InitializationError.ToString());
                var args = new DeviceErrorEventArgs(msg, ex,
                                            DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                            notifyStr);
                Error?.Invoke(this, args);
                return false;
            }

            // Get the model(s) of pump(s) connected to the controller
            if (!GetModels())
            {
                msg = "Could not get pump models.";
                return false;
            }

            // Get the units data from the pump
            if (!ReadPumpUnits())
            {
                msg = "Could not read pump units.";
                return false;
            }

            // Refresh the pump data
            if (!Refresh())
            {
                msg = "Could not retrieve pump data.";
                return false;
            }

            // Get the setpoint and operating limits data
            for (var pumpIndx = 0; pumpIndx < m_PumpCount; pumpIndx++)
            {
                // Setpoint limits
                m_SetpointLimits[pumpIndx].MinFlowSp = ReadMinFlowSetpoint(pumpIndx);
                m_SetpointLimits[pumpIndx].MaxFlowSp = ReadMaxFlowSetpoint(pumpIndx);
                m_SetpointLimits[pumpIndx].MinPressSp = ReadMinPressSetpoint(pumpIndx);
                m_SetpointLimits[pumpIndx].MaxPressSp = ReadMaxPressSetpoint(pumpIndx);
                m_SetpointLimits[pumpIndx].MaxFlowLimit = ReadMaxFlowLimit(pumpIndx);

                // Range limits
                m_PumpRanges[pumpIndx] = ReadPumpRanges(pumpIndx);
            }

            // we've already got the control mode for the pumps in PumpData, and since we use them all in the same control mode
            // setting current mode here lets SetControlMode decide properly if it needs to change the mode or not.
            m_ControlMode = PumpData[0].ControlMode;

            // Set remote mode
            if (!SetControlMode(IscoControlMode.Remote))
            {
                msg = "Could not set the control mode.";
                return false;
            }

            m_Initializing = false;
            m_Initialized = true;

            // Tell world initializaion is complete
            InitializationComplete?.Invoke();

            // setup refresh timer so fluidics glyph will show updated status every CONST_DEFAULT_TIMER_INTERVAL ms
            m_refresh = new Timer(m_refresh_Elapsed);
            m_refresh.Change(0L, CONST_DEFAULT_TIMER_INTERVAL);
            return true;
        }

        private void m_refresh_Elapsed(object state)
        {
            Refresh();
        }

        private bool GetModels()
        {
            if (m_Emulation)
            {
                return true;
            }
            var cmd = "IDENTIFY";
            var result = SendCommand(cmd, true);
            //ApplicationLogger.LogError(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "ISCO MODELS STRING: " + result);
            var tokens = result.Split(',');
            //The models should be in tokens 1,3, and 5.
            var retval = true;
            var m = 1;
            for (var i = 0; i < m_PumpCount; i++)
            {
                var tokenOfInterest = tokens[m];
                m += 2;
                if (tokenOfInterest.Contains("100D"))
                {
                    models[i] = ISCOModel.ISCO100D;
                }
                else if (tokenOfInterest.Contains("65D"))
                {
                    models[i] = ISCOModel.ISCO65D;
                }
                else
                {
                    models[i] = ISCOModel.Unknown;
                    retval = false;
                }
            }
            return retval;
        }

        /// <summary>
        /// Refreshes the stored data for a pump
        /// </summary>
        /// <returns>TRUE for success; FALSE otherwise</returns>
        public bool Refresh()
        {
            //ApplicationLogger.LogMessage(ApplicationLogger.CONST_STATUS_LEVEL_DETAILED, "ISCO REFRESH STATUS DATA");
            if (m_Emulation)
                return true;

            if (!(m_Initialized || m_Initializing))
            {
                var args = new DeviceErrorEventArgs("Device not initialized", null,
                                    DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                    IscoErrorNotifications.GetNotificationString(IscoProblemStatus.DeviceNotInitialized.ToString()));
                Error?.Invoke(this, args);
                return false;
            }

            var cmd = "G&";
            string resp;
            try
            {
                resp = SendCommand(cmd, true);
            }
            catch (Exception ex)
            {
                var args = new DeviceErrorEventArgs("Exception refreshing status", GetBaseException(ex),
                                                DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                                IscoErrorNotifications.GetNotificationString(IscoProblemStatus.ComError.ToString()));
                Error?.Invoke(this, args);
                return false;
            }

            try
            {
                m_PumpData = ParseStatusMessage(resp);
            }
            catch (Exception ex)
            {
                var args = new DeviceErrorEventArgs("Exception parsing status message",
                                                GetBaseException(ex),
                                                DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                                IscoErrorNotifications.GetNotificationString(IscoProblemStatus.MessageParseError.ToString()));
                Error?.Invoke(this, args);
                return false;
            }

            // Get operation mode and setpoint for each pump
            for (var pumpIndx = 0; pumpIndx < m_PumpCount; pumpIndx++)
            {
                // Operation mode
                m_PumpData[pumpIndx].OperationMode = m_OpMode;

                // Setpoint
                m_PumpData[pumpIndx].SetPoint = ReadSetpoint(pumpIndx, m_PumpData[pumpIndx].OperationMode);

                // Refill rate
                m_PumpData[pumpIndx].RefillRate = ReadRefillRate(pumpIndx);
            }

            // Check for errors
            ReportPumpErrors();

            // Notify anybody who happens to be listening
            RefreshComplete?.Invoke();

            return true;
        }

        //public double GetPumpVolume(double timeout, int pumpId)
        public double GetPumpVolume(double timeout, ISCOPumpChannels pump)
        {
            Refresh();
            return m_PumpData[(int)pump].Volume;
        }

        /// <summary>
        /// Gets a flow or pressure setpoint
        /// </summary>
        /// <param name="pumpIndx">Index of pump</param>
        /// <param name="opMode">Operation mode (determines whether to get flow or pressure SP)</param>
        /// <returns>Current setpoint; -1000 if there's a problem</returns>
        public double ReadSetpoint(int pumpIndx, IscoOperationMode opMode)
        {
            if (m_Emulation)
                return 0.0;

            if (!(m_Initialized || m_Initializing))
            {
                var args = new DeviceErrorEventArgs("Device not initialized", null,
                                    DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                    IscoErrorNotifications.GetNotificationString(IscoProblemStatus.DeviceNotInitialized.ToString()));
                Error?.Invoke(this, args);
                return -1000;
            }

            var cmdStr = "";

            // Create the command string
            switch (opMode)
            {
                case IscoOperationMode.ConstantPressure:
                    cmdStr = "SETPRESS" + ConvertPumpIndxToString(pumpIndx, false);
                    break;
                case IscoOperationMode.ConstantFlow:
                    cmdStr = "SETFLOW" + ConvertPumpIndxToString(pumpIndx, false);
                    break;
            }

            // Send the command and wait for a response
            string resp;
            try
            {
                resp = SendCommand(cmdStr, true);
            }
            catch (Exception ex)
            {
                var args = new DeviceErrorEventArgs("Exception getting setpoint", GetBaseException(ex),
                                                DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                                IscoErrorNotifications.GetNotificationString(IscoProblemStatus.ComError.ToString()));
                Error?.Invoke(this, args);
                return -1000;
            }

            return ParseNumericValue(resp);
        }

        /// <summary>
        /// Gets the pump's minimum flow setpoint
        /// </summary>
        /// <param name="pumpIndx">Index of pump</param>
        /// <returns>Min flow setpoint; -1000 if there's a problem</returns>
        public double ReadMinFlowSetpoint(int pumpIndx)
        {
            if (m_Emulation)
                return 0.0;

            if (!(m_Initialized || m_Initializing))
            {
                var args = new DeviceErrorEventArgs("Device not initialized", null,
                                    DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                    IscoErrorNotifications.GetNotificationString(IscoProblemStatus.DeviceNotInitialized.ToString()));
                Error?.Invoke(this, args);
                return -1000;
            }

            var cmdStr = "MINFLOW" + ConvertPumpIndxToString(pumpIndx, false);

            // Send the command and wait for a response
            string resp;
            try
            {
                resp = SendCommand(cmdStr, true);
            }
            catch (Exception ex)
            {
                var args = new DeviceErrorEventArgs("Exception getting setpoint limit",
                                                GetBaseException(ex), DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                                IscoErrorNotifications.GetNotificationString(IscoProblemStatus.ComError.ToString()));
                Error?.Invoke(this, args);
                return -1000;
            }

            return ParseNumericValue(resp);
        }

        /// <summary>
        /// Gets the pump's maximum flow setpoint
        /// </summary>
        /// <param name="pumpIndx">Index of pump</param>
        /// <returns>Max flow setpoint; -1000 if there's a problem</returns>
        public double ReadMaxFlowSetpoint(int pumpIndx)
        {
            if (m_Emulation)
                return 0.0;

            if (!(m_Initialized || m_Initializing))
            {
                var args = new DeviceErrorEventArgs("Device not initialized", null,
                                    DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                    IscoErrorNotifications.GetNotificationString(IscoProblemStatus.DeviceNotInitialized.ToString()));
                Error?.Invoke(this, args);
                return -1000;
            }

            var cmdStr = "MAXFLOW" + ConvertPumpIndxToString(pumpIndx, false);

            // Send the command and wait for a response
            string resp;
            try
            {
                resp = SendCommand(cmdStr, true);
            }
            catch (Exception ex)
            {
                var args = new DeviceErrorEventArgs("Exception getting setpoint limit",
                                                GetBaseException(ex), DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                                IscoErrorNotifications.GetNotificationString(IscoProblemStatus.ComError.ToString()));
                Error?.Invoke(this, args);
                return -1000;
            }

            return ParseNumericValue(resp);
        }

        /// <summary>
        /// Gets the max flow limit for a pump
        /// </summary>
        /// <param name="pumpIndx">Index of pump</param>
        /// <returns>Max flow limit; -1000 if there's a problem</returns>
        public double ReadMaxFlowLimit(int pumpIndx)
        {
            if (m_Emulation)
                return 30.0;

            if (!(m_Initialized || m_Initializing))
            {
                var args = new DeviceErrorEventArgs("Device not initialized", null,
                                    DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                    IscoErrorNotifications.GetNotificationString(IscoProblemStatus.DeviceNotInitialized.ToString()));
                Error?.Invoke(this, args);
                return -1000;
            }

            var cmdStr = "MFLOW" + ConvertPumpIndxToString(pumpIndx, false);

            // Send the command and wait for a response
            string resp;
            try
            {
                resp = SendCommand(cmdStr, true);
            }
            catch (Exception ex)
            {
                var args = new DeviceErrorEventArgs("Exception getting setpoint limit",
                                                GetBaseException(ex), DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                                IscoErrorNotifications.GetNotificationString(IscoProblemStatus.ComError.ToString()));
                Error?.Invoke(this, args);
                return -1000;
            }

            return ParseNumericValue(resp);
        }

        /// <summary>
        /// Gets the pump's minimum pressure setpoint
        /// </summary>
        /// <param name="pumpIndx">Index of pump</param>
        /// <returns>Min pressure setpoint; -1000 if there's a problem</returns>
        public double ReadMinPressSetpoint(int pumpIndx)
        {
            if (m_Emulation)
                return 0.0;

            if (!(m_Initialized || m_Initializing))
            {
                var args = new DeviceErrorEventArgs("Device not initialized", null,
                                    DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                    IscoErrorNotifications.GetNotificationString(IscoProblemStatus.DeviceNotInitialized.ToString()));
                Error?.Invoke(this, args);
                return -1000;
            }

            var cmdStr = "MINPRESS" + ConvertPumpIndxToString(pumpIndx, false);

            // Send the command and wait for a response
            string resp;
            try
            {
                resp = SendCommand(cmdStr, true);
            }
            catch (Exception ex)
            {
                var args = new DeviceErrorEventArgs("Exception getting setpoint limit",
                                                GetBaseException(ex), DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                                IscoErrorNotifications.GetNotificationString(IscoProblemStatus.ComError.ToString()));
                Error?.Invoke(this, args);
                return -1000;
            }

            return ParseNumericValue(resp);
        }

        /// <summary>
        /// Gets the pump's maximum pressure setpoint
        /// </summary>
        /// <param name="pumpIndx">Index of pump</param>
        /// <returns>Max pressure setpoint; -1000 if there's a problem</returns>
        public double ReadMaxPressSetpoint(int pumpIndx)
        {
            if (m_Emulation)
                return 0.0;

            if (!(m_Initialized || m_Initializing))
            {
                var args = new DeviceErrorEventArgs("Device not initialized", null,
                                    DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                    IscoErrorNotifications.GetNotificationString(IscoProblemStatus.DeviceNotInitialized.ToString()));
                Error?.Invoke(this, args);
                return -1000;
            }

            var cmdStr = "MAXPRESS" + ConvertPumpIndxToString(pumpIndx, false);

            // Send the command and wait for a response
            string resp;
            try
            {
                resp = SendCommand(cmdStr, true);
            }
            catch (Exception ex)
            {
                var args = new DeviceErrorEventArgs("Exception getting setpoint limit",
                                                GetBaseException(ex), DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                                IscoErrorNotifications.GetNotificationString(IscoProblemStatus.ComError.ToString()));
                Error?.Invoke(this, args);
                return -1000;
            }

            return ParseNumericValue(resp);
        }

        /// <summary>
        /// Gets the maximum pump ranges
        /// </summary>
        /// <param name="pumpIndx">Index of pump</param>
        /// <returns>Object containing pump range data</returns>
        public IscoPumpRangeData ReadPumpRanges(int pumpIndx)
        {
            if (m_Emulation)
                return null;

            if (!(m_Initialized || m_Initializing))
            {
                var args = new DeviceErrorEventArgs("Device not initialized", null,
                                    DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                    IscoErrorNotifications.GetNotificationString(IscoProblemStatus.DeviceNotInitialized.ToString()));
                Error?.Invoke(this, args);
                return null;
            }

            var cmdStr = "RANGE" + ConvertPumpIndxToString(pumpIndx, false);

            // Send the command and wait for a response
            string resp;
            try
            {
                resp = SendCommand(cmdStr, true);
            }
            catch (Exception ex)
            {
                var args = new DeviceErrorEventArgs("Exception getting setpoint limit",
                                                GetBaseException(ex), DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                                IscoErrorNotifications.GetNotificationString(IscoProblemStatus.ComError.ToString()));
                Error?.Invoke(this, args);
                return null;
            }

            return ParseRangeMessage(resp);
        }

        [LCMethodEvent("Refill", MethodOperationTimeoutType.Parameter)]
        //public bool StartRefill(double timeout, int pumpIndx, double refillRate)
        public bool StartRefill(double timeout, ISCOPumpChannels pump, double refillRate)
        {
            return StartRefill((int)pump, refillRate);
        }

        /// <summary>
        /// Starts a refill for specified pump
        /// </summary>
        /// <param name="pumpIndx">Index for pump to refill</param>
        /// <param name="refillRate">Rate of refill</param>
        /// <returns>TRUE for success; FALSE otherwise</returns>
        public bool StartRefill(int pumpIndx, double refillRate)
        {
            if (m_Emulation)
                return true;

            if (!m_Initialized)
            {
                var errorArgs = new DeviceErrorEventArgs("Device not initialized", null,
                                    DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                    IscoErrorNotifications.GetNotificationString(IscoProblemStatus.DeviceNotInitialized.ToString()));
                Error?.Invoke(this, errorArgs);
                return false;
            }

            // Create the command string for the refill rate
            var cmdStrRate = "REFILL" + ConvertPumpIndxToString(pumpIndx, true) + "=" + refillRate.ToString(CultureInfo.InvariantCulture);

            // Create the command string for starting the refill
            var cmdStrStart = "REFILL" + ConvertPumpIndxToString(pumpIndx, true);

            // Send the rate command
            try
            {
                var resp = SendCommand(cmdStrRate, true);
            }
            catch (Exception ex)
            {
                var args = new DeviceErrorEventArgs("Exception setting refill rate",
                                                GetBaseException(ex), DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                                IscoErrorNotifications.GetNotificationString(IscoProblemStatus.ComError.ToString()));
                Error?.Invoke(this, args);
                return false;
            }

            // Send the start command
            try
            {
                SendCommand(cmdStrStart, false);
            }
            catch (Exception ex)
            {
                var args = new DeviceErrorEventArgs("Exception starting refill",
                                                GetBaseException(ex), DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                                IscoErrorNotifications.GetNotificationString(IscoProblemStatus.ComError.ToString()));
                Error?.Invoke(this, args);
                return false;
            }

            // Everything worked, so send message and return success
            var statusArgs = new DeviceStatusEventArgs(DeviceStatus.Initialized,
                        IscoStatusNotifications.GetNotificationString(IscoOperationStatus.Refilling.ToString() +
                        ConvertPumpIndxToString(pumpIndx, false)), this);
            StatusUpdate?.Invoke(this, statusArgs);
            return true;
        }

        /// <summary>
        /// Starts the pump.
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="pump"></param>
        /// <returns></returns>
        [LCMethodEvent("Start Pump", MethodOperationTimeoutType.Parameter)]
        public bool StartPump(double timeout, ISCOPumpChannels pump)
        {
            return StartPump(timeout, (int)pump);
        }

        /// <summary>
        /// Starts running the specified pump
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="pumpIndx">Pump to start</param>
        /// <returns>TRUE for success; FALSE otherwise</returns>
        public bool StartPump(double timeout, int pumpIndx)
        {
            if (m_Emulation)
                return true;

            if (!m_Initialized)
            {
                var errorArgs = new DeviceErrorEventArgs("Device not initialized", null,
                                    DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                    IscoErrorNotifications.GetNotificationString(IscoProblemStatus.DeviceNotInitialized.ToString()));
                Error?.Invoke(this, errorArgs);
                return false;
            }

            var cmdStr = "RUN" + ConvertPumpIndxToString(pumpIndx, true);

            // Send the command
            try
            {
                var resp = SendCommand(cmdStr, true);
            }
            catch (Exception ex)
            {
                var args = new DeviceErrorEventArgs("Exception starting pump",
                                                GetBaseException(ex), DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                                IscoErrorNotifications.GetNotificationString(IscoProblemStatus.ComError.ToString()));
                Error?.Invoke(this, args);
                return false;
            }

            // Everything worked, so send message and return success
            var statusArgs = new DeviceStatusEventArgs(DeviceStatus.Initialized,
                        IscoStatusNotifications.GetNotificationString(IscoOperationStatus.Running.ToString() +
                        ConvertPumpIndxToString(pumpIndx, false)), this);
            StatusUpdate?.Invoke(this, statusArgs);
            return true;
        }

        /// <summary>
        /// Stops specified pump
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="pump">Pump to stop</param>
        /// <returns>TRUE for success; FALSE otherwise</returns>
        [LCMethodEvent("Stop Pump", MethodOperationTimeoutType.Parameter)]
        public bool StopPump(double timeout, ISCOPumpChannels pump)
        {
            return StopPump(timeout, (int)pump);
        }

        /// <summary>
        /// Stops specified pump
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="pumpIndx">Index of pump to stop</param>
        /// <returns>TRUE for success; FALSE otherwise</returns>
        public bool StopPump(double timeout, int pumpIndx)
        {
            if (m_Emulation)
                return true;

            if (!m_Initialized)
            {
                var errorArgs = new DeviceErrorEventArgs("Device not initialized", null,
                                    DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                    IscoErrorNotifications.GetNotificationString(IscoProblemStatus.DeviceNotInitialized.ToString()));
                Error?.Invoke(this, errorArgs);
                return false;
            }

            var cmdStr = "STOP" + ConvertPumpIndxToString(pumpIndx, true);

            // Send the command
            try
            {
                var resp = SendCommand(cmdStr, true);
            }
            catch (Exception ex)
            {
                var args = new DeviceErrorEventArgs("Exception stopping pump",
                                                GetBaseException(ex), DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                                IscoErrorNotifications.GetNotificationString(IscoProblemStatus.ComError.ToString()));
                Error?.Invoke(this, args);
                return false;
            }

            // Everything worked, so send message and return success
            var statusArgs = new DeviceStatusEventArgs(DeviceStatus.Initialized,
                        IscoStatusNotifications.GetNotificationString(IscoOperationStatus.Stopped.ToString() +
                        ConvertPumpIndxToString(pumpIndx, false)), this);
            StatusUpdate?.Invoke(this, statusArgs);
            return true;
        }

        [LCMethodEvent("Set Flow Rate", MethodOperationTimeoutType.Parameter)]
        //public bool SetFlow(double timeout, int pumpIndx, double newFlow)
        public bool SetFlow(double timeout, ISCOPumpChannels pump, double newFlow)
        {
            return SetFlow((int)pump, newFlow);
        }

        /// <summary>
        /// Sets the flow rate for a pump in constant flow mode
        /// </summary>
        /// <param name="pumpIndx">Pump to set</param>
        /// <param name="newFlow">New flow rate</param>
        /// <returns>TRUE for success; FALSE otherwise</returns>
        public bool SetFlow(int pumpIndx, double newFlow)
        {
            if (m_Emulation)
                return true;

            if (!m_Initialized)
            {
                var errorArgs = new DeviceErrorEventArgs("Device not initialized", null,
                                    DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                    IscoErrorNotifications.GetNotificationString(IscoProblemStatus.DeviceNotInitialized.ToString()));
                Error?.Invoke(this, errorArgs);
                return false;
            }

            var cmdStr = "FLOW" + ConvertPumpIndxToString(pumpIndx, true) + "=" + newFlow.ToString(CultureInfo.InvariantCulture);

            // Send the command
            try
            {
                var resp = SendCommand(cmdStr, false);
            }
            catch (Exception ex)
            {
                var args = new DeviceErrorEventArgs("Exception setting flow",
                                                GetBaseException(ex), DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                                IscoErrorNotifications.GetNotificationString(IscoProblemStatus.ComError.ToString()));
                Error?.Invoke(this, args);
                return false;
            }

            // Everything worked, so send message and return success
            var statusArgs = new DeviceStatusEventArgs(DeviceStatus.Initialized,
                        IscoStatusNotifications.GetNotificationString(IscoOperationStatus.FlowSet.ToString() +
                        ConvertPumpIndxToString(pumpIndx, false)), this);
            StatusUpdate?.Invoke(this, statusArgs);
            return true;
        }

        /// <summary>
        /// Sets pressure for a pump in constant pressure mode
        /// </summary>
        /// <param name="timeout">Timeout (ignored)</param>
        /// <param name="pump">Pump to set</param>
        /// <param name="newPress">New pressure setpoint</param>
        /// <returns>TRUE for success; FALSE otherwise</returns>
        [LCMethodEvent("Set Pressure", MethodOperationTimeoutType.Parameter)]
        public bool SetPressure(double timeout, ISCOPumpChannels pump, double newPress)
        {
            return SetPressure((int)pump, newPress);
        }

        /// <summary>
        /// Sets pressure for a pump in constant pressure mode
        /// </summary>
        /// <param name="pumpIndx">Index of pump to set</param>
        /// <param name="newPress">New pressure setpoint</param>
        /// <returns>TRUE for success; FALSE otherwise</returns>
        public bool SetPressure(int pumpIndx, double newPress)
        {
            if (m_Emulation)
                return true;

            if (!m_Initialized)
            {
                var errorArgs = new DeviceErrorEventArgs("Device not initialized", null,
                                    DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                    IscoErrorNotifications.GetNotificationString(IscoProblemStatus.DeviceNotInitialized.ToString()));
                Error?.Invoke(this, errorArgs);
                return false;
            }

            var cmdStr = "PRESS" + ConvertPumpIndxToString(pumpIndx, true) + "=" + newPress.ToString(CultureInfo.InvariantCulture);

            // Send the command
            try
            {
                var resp = SendCommand(cmdStr, true);
            }
            catch (Exception ex)
            {
                var args = new DeviceErrorEventArgs("Exception setting pressure",
                                                GetBaseException(ex), DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                                IscoErrorNotifications.GetNotificationString(IscoProblemStatus.ComError.ToString()));
                Error?.Invoke(this, args);
                return false;
            }

            // Everything worked, so send message and return success
            var statusArgs = new DeviceStatusEventArgs(DeviceStatus.Initialized,
                        IscoStatusNotifications.GetNotificationString(IscoOperationStatus.PressSet.ToString() +
                        ConvertPumpIndxToString(pumpIndx, false)), this);
            StatusUpdate?.Invoke(this, statusArgs);
            return true;
        }

        /// <summary>
        /// Reports errors found from a pump status request
        /// </summary>
        private void ReportPumpErrors()
        {
            for (var indx = 0; indx < m_PumpCount; indx++)
            {
                var notifyMsg = "";
                var errorFound = false;
                switch (m_PumpData[indx].ProblemStatus)
                {
                    case IscoProblemStatus.None:
                        // No action necessary
                        break;
                    case IscoProblemStatus.OverPressure:
                        notifyMsg = IscoErrorNotifications.GetNotificationString(IscoProblemStatus.OverPressure.ToString() +
                            ConvertPumpIndxToString(indx, false));
                        errorFound = true;
                        break;
                    case IscoProblemStatus.UnderPressure:
                        notifyMsg = IscoErrorNotifications.GetNotificationString(IscoProblemStatus.UnderPressure.ToString() +
                            ConvertPumpIndxToString(indx, false));
                        errorFound = true;
                        break;
                    case IscoProblemStatus.CylinderBottom:
                        notifyMsg = IscoErrorNotifications.GetNotificationString(IscoProblemStatus.CylinderBottom.ToString() +
                            ConvertPumpIndxToString(indx, false));
                        errorFound = true;
                        break;
                    case IscoProblemStatus.CylinderEmpty:
                        notifyMsg = IscoErrorNotifications.GetNotificationString(IscoProblemStatus.CylinderEmpty.ToString() +
                            ConvertPumpIndxToString(indx, false));
                        errorFound = true;
                        break;
                    case IscoProblemStatus.MotorFailure:
                        notifyMsg = IscoErrorNotifications.GetNotificationString(IscoProblemStatus.MotorFailure.ToString() +
                            ConvertPumpIndxToString(indx, false));
                        errorFound = true;
                        break;
                }

                if (errorFound)
                {
                    // Pump had an error, so report it
                    var args = new DeviceErrorEventArgs("Pump error", null,
                            DeviceErrorStatus.ErrorAffectsAllColumns, this, notifyMsg);
                    Error?.Invoke(this, args);
                }
            }
        }

        /// <summary>
        /// Gets the pump units from the pump and stores them in static class classIscoConversions
        /// </summary>
        private bool ReadPumpUnits()
        {
            if (m_Emulation)
            {
                IscoConversions.FlowUnits = IscoFlowUnits.ul_min;
                IscoConversions.PressUnits = IscoPressureUnits.psi;
                return true;
            }

            if (!(m_Initialized || m_Initializing))
            {
                IscoConversions.FlowUnits = IscoFlowUnits.error;
                IscoConversions.PressUnits = IscoPressureUnits.error;

                var args = new DeviceErrorEventArgs("Device not initialized", null,
                                    DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                    IscoErrorNotifications.GetNotificationString(IscoProblemStatus.DeviceNotInitialized.ToString()));
                Error?.Invoke(this, args);
                return false;
            }

            var cmdStr = "UNITSA";

            // Send the command and wait for a response
            string resp;
            try
            {
                resp = SendCommand(cmdStr, true);
                //ApplicationLogger.LogError(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "ISCO READPUMPUNITS RESPONSE:" + resp);
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "ISCO READPUMPUNITS ERROR:" + ex.Message);
                IscoConversions.FlowUnits = IscoFlowUnits.error;
                IscoConversions.PressUnits = IscoPressureUnits.error;

                var args = new DeviceErrorEventArgs("Exception getting units",
                                                GetBaseException(ex), DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                                IscoErrorNotifications.GetNotificationString(IscoProblemStatus.ComError.ToString()));
                Error?.Invoke(this, args);
                return false;
            }

            if (ParseUnitsMessage(resp))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Reads the current refill rate setting
        /// </summary>
        /// <param name="pumpIndx">Index for pump</param>
        /// <returns>Refill rate; -1000 on error</returns>
        private double ReadRefillRate(int pumpIndx)
        {
            if (m_Emulation)
                return 5.0;

            if (!(m_Initialized || m_Initializing))
            {
                var args = new DeviceErrorEventArgs("Device not initialized", null,
                                    DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                    IscoErrorNotifications.GetNotificationString(IscoProblemStatus.DeviceNotInitialized.ToString()));
                Error?.Invoke(this, args);
                return -1000;
            }

            var cmdStr = "RLIMIT" + ConvertPumpIndxToString(pumpIndx, false);

            // Send the command and wait for a response
            string resp;
            try
            {
                resp = SendCommand(cmdStr, true);
            }
            catch (Exception ex)
            {
                var args = new DeviceErrorEventArgs("Exception getting refill rate",
                                                GetBaseException(ex), DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                                IscoErrorNotifications.GetNotificationString(IscoProblemStatus.ComError.ToString()));
                Error?.Invoke(this, args);
                return -1000;
            }

            return ParseNumericValue(resp);
        }

        /// <summary>
        /// Configures the serial port
        /// </summary>
        /// <param name="portProps">Port properties</param>
        private void ConfigSerialPort(IscoSerPortProps portProps)
        {
            if (m_SerialPort == null)
            {
                m_SerialPort = new SerialPort();
            }
            else if (m_SerialPort.IsOpen)
            {
                m_SerialPort.Close();
            }

            m_SerialPort.PortName = portProps.PortName;
            m_SerialPort.BaudRate = portProps.BaudRate;
            m_SerialPort.StopBits = portProps.StopBits;
            m_SerialPort.DataBits = portProps.DataBits;
            m_SerialPort.Handshake = portProps.HandShake;
            m_SerialPort.Parity = portProps.Parity;
            m_SerialPort.NewLine = portProps.NewLine;
            m_SerialPort.ReadTimeout = portProps.ReadTimeout;
            m_SerialPort.WriteTimeout = portProps.WriteTimeout;
            m_UnitAddr = portProps.UnitAddress;
        }

        /// <summary>
        /// Sends a command to a pump (overload using default parameters)
        /// ack = Received
        /// WaitResp = false
        /// </summary>
        /// <param name="cmdStr">Command string to send</param>
        /// <returns>Empty string or string containing response from pump</returns>
        private string SendCommand(string cmdStr)
        {
            try
            {
                return SendCommand(cmdStr, IscoMsgAckCodes.Recvd, false);
            }
            catch (IscoExceptionWriteTimeout)
            {
                throw new IscoExceptionWriteTimeout();
            }
            catch (IscoExceptionReadTimeout)
            {
                throw new IscoExceptionReadTimeout();
            }
            catch (IscoExceptionMessageError ex1)
            {
                throw new IscoExceptionMessageError(ex1.Message);
            }
            catch (IscoExceptionUnauthroizedAccess ex2)
            {
                throw new IscoExceptionUnauthroizedAccess(ex2.Message);
            }
        }

        /// <summary>
        /// Sends a command to a pump (Overload to allow waiting for a response from pump)
        /// Sets ack = Received
        /// </summary>
        /// <param name="cmdStr">Command string to send</param>
        /// <param name="waitResp">TRUE if a response is expected from pump; FALSE otherwise</param>
        /// <returns>Empty string or string containing response from pump</returns>
        private string SendCommand(string cmdStr, bool waitResp)
        {
            try
            {
                return SendCommand(cmdStr, IscoMsgAckCodes.Recvd, waitResp);
            }
            catch (IscoExceptionWriteTimeout)
            {
                throw new IscoExceptionWriteTimeout();
            }
            catch (IscoExceptionReadTimeout)
            {
                throw new IscoExceptionReadTimeout();
            }
            catch (IscoExceptionMessageError ex1)
            {
                throw new IscoExceptionMessageError(ex1.Message);
            }
            catch (IscoExceptionUnauthroizedAccess ex2)
            {
                throw new IscoExceptionUnauthroizedAccess(ex2.Message);
            }
        }

        /// <summary>
        /// Sends a command to a pump (Overload allowing specification of all params)
        /// </summary>
        /// <param name="cmdStr">Command string to send</param>
        /// <param name="ack">Message acknowledgement code</param>
        /// <param name="waitResp">TRUE if a response is expected from pump; FALSE otherwise</param>
        /// <returns>Empty string or string containing response from pump</returns>
        private string SendCommand(string cmdStr, IscoMsgAckCodes ack, bool waitResp)
        {
            string errMsg;

            // If the serial port isn't open, then open it
            if (!m_SerialPort.IsOpen)
            {
                try
                {
                    m_SerialPort.Open();
                }
                catch (UnauthorizedAccessException ex)
                {
                    throw new IscoExceptionUnauthroizedAccess("Could not access the COM port: " + ex.Message);
                }
            }

            var cmd = BuildCtrlMsgFrame(0, m_UnitAddr, cmdStr, ack);

            // Send the command
            try
            {
#if DACTEST
                    LogMessage("SEND: " + cmd); // DAC testing
#endif
                m_SerialPort.WriteLine("\r" + cmd); // DASNET messages are preceded by CR to ensure controller is listening
            }
            catch (TimeoutException)
            {
                throw new IscoExceptionWriteTimeout();
            }
            catch (UnauthorizedAccessException ex)
            {
                errMsg = "Could not access the COM port: " + ex.Message;
                throw new IscoExceptionUnauthroizedAccess(errMsg);
            }

            // If we're supposed to wait for a response, then read it in
            var respStr = "";
            var ackIn = IscoMsgAckCodes.PumpComError;
            if (waitResp)
            {
                string msgIn;
                try
                {
                    msgIn = m_SerialPort.ReadLine();
#if DACTEST
                        LogMessage("RECV: " + msgIn);   // DAC testing
#endif
                }
                catch (TimeoutException)
                {
#if DACTEST
                        LogMessage("Timeout Exception");
#endif
                    throw new IscoExceptionReadTimeout();
                }
                catch (UnauthorizedAccessException ex)
                {
                    errMsg = "Could not access the COM port: " + ex.Message;
#if DACTEST
                        LogMessage("COM port access exception");    // DAC testing
#endif
                    throw new IscoExceptionUnauthroizedAccess(errMsg);
                }

                // Process the received response
                bool result;

                try
                {
                    result = ParseMsgFrame(msgIn, ref ackIn, ref respStr);
                }
                catch (IscoExceptionMessageError ex)
                {
                    throw new IscoExceptionMessageError(ex.Message);
                }

                if (!result)
                {
                    throw new IscoExceptionMessageError("ISCO message error: Bad checksum");
                }

                if (ackIn == IscoMsgAckCodes.PumpBusy)
                {
                    throw new IscoExceptionMessageError("ISCO message error: Pump unit busy");
                }

                if (ackIn == IscoMsgAckCodes.PumpComError)
                    throw new IscoExceptionMessageError("ISCO message error: Pump unit error");
            }

            return respStr;
        }

#if DACTEST
            /// <summary>
            /// Logs an ISCO message to a file (testing only)
            /// </summary>
            /// <param name="msg"></param>
            public void LogMessage(string msg)
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(System.Windows.Forms.Application.ExecutablePath);
                string outFile = System.IO.Path.Combine(fi.DirectoryName, "Log.txt");

                string logTxt = DateTime.UtcNow.Subtract(new TimeSpan(8, 0 , 0)).ToString("MM/dd/yyyy HH:mm:ss.ff") + ", " + msg;

                using (System.IO.StreamWriter w = System.IO.File.AppendText(outFile))
                {
                    w.WriteLine(logTxt);
                    w.Flush();
                    w.Close();
                }
            }
#endif

        /// <summary>
        /// Sums the ASCII codes for all characters in a string
        /// </summary>
        /// <param name="inpStr">Input string</param>
        /// <returns>Sum of all ASCII codes for chars in string</returns>
        private int SumAsciiCodes(string inpStr)
        {
            var outSum = 0;

            // Convert the input string to an array of char
            var tmpCharArray = inpStr.ToCharArray();

            foreach (var tmpChar in tmpCharArray)
            {
                // Assuming all characters involved have ASCII values < 255, then ASCII value can be
                //  obtained via a simple cast from char to int
                outSum += (int)tmpChar;
            }

            return outSum;
        }

        /// <summary>
        /// Calculate the (weird) checksum per ISCO instructions
        /// </summary>
        /// <param name="inpStr">Input string</param>
        /// <returns>String version of hex checksum value</returns>
        private string ObtainChecksum(string inpStr)
        {
            // Too strange to explain - see ISCO docs
            var tmpInt = SumAsciiCodes(inpStr) % 256;
            if (tmpInt > 0)
                tmpInt = 256 - tmpInt;

            return tmpInt.ToString("X2");
        }

        /// <summary>
        /// Verifies the string that will be sent to ISCO pump is a multiple of 256
        /// </summary>
        /// <param name="inpStr">Message text</param>
        /// <param name="inpCheckSum">Checksum (hex) for message text</param>
        /// <returns>TRUE if multiple of 256; FALSE otherwise</returns>
        private bool VerifyChecksum(string inpStr, string inpCheckSum)
        {
            var tmpInt = SumAsciiCodes(inpStr) + int.Parse(inpCheckSum, NumberStyles.HexNumber);

            if ((tmpInt % 256) == 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Parses a message received from a pump
        /// </summary>
        /// <param name="inpMsg">String (frame) received in acknowledgement of a sent message</param>
        /// <param name="ack">Message acknowldgement code</param>
        /// <param name="msg">Message text</param>
        /// <returns>TRUE if valid checksum was received; FALSE otherwise</returns>
        private bool ParseMsgFrame(string inpMsg, ref IscoMsgAckCodes ack, ref string msg)
        {
            /* Message format is:
             *  Acknowledgement (1 char)
             *  Message source (1 char) or space if no message
             *  Length of message text (2 hex digits; 00 = 256 chars) if message present
             *  Message text (256 char max) if message present
             *  Checksum (2 digits)
             */

            try
            {
                var respStrLen = inpMsg.Length;

                // Extract the checksum portion of the response string
                var chkSumStr = inpMsg.Substring(respStrLen - 2, 2);
                var frameBodyStr = inpMsg.Substring(0, respStrLen - 2);

                // Test the checksum. If invalid, no point in going further
                if (!VerifyChecksum(frameBodyStr, chkSumStr))
                    return false;

                // Get the message acknowledgement
                var ackStr = frameBodyStr.Substring(0, 1);
                switch (ackStr)
                {
                    case "R":
                        ack = IscoMsgAckCodes.Recvd;
                        break;
                    case "E":
                        ack = IscoMsgAckCodes.PumpComError;
                        break;
                    case "B":
                        ack = IscoMsgAckCodes.PumpBusy;
                        break;
                    default:
                        // Shouldn't ever get here. If we did, something is wrong
                        return false;
                }

                var msgSource = frameBodyStr.Substring(1, 1);
                if (msgSource != " ")
                {
                    // If the message source isn't a space, then we need to extract the message
                    var msgLenStr = frameBodyStr.Substring(2, 2);
                    var msgLenInt = int.Parse(msgLenStr, NumberStyles.HexNumber);
                    msg = frameBodyStr.Substring(4, msgLenInt);
                    // Some messages end with a ",", so delete it
                    if (msg.EndsWith(","))
                        msg = msg.Remove(msg.Length - 1, 1);
                }
                else
                    msg = "";

                return true;
            }
            catch (Exception ex)
            {
                // There was a problem parsing the message
                var errMsg = "Exception parsing message from pump: " + ex.Message;
                throw new IscoExceptionMessageError(errMsg);
            }
        }

        /// <summary>
        /// Builds a message frame for sending to pump controller
        /// </summary>
        /// <param name="srce">Message source</param>
        /// <param name="dest">Pump controller will forward message to</param>
        /// <param name="msg">Message text</param>
        /// <param name="ack">Acknowledgement code</param>
        /// <returns>Complete message frame</returns>
        private string BuildCtrlMsgFrame(int srce, int dest, string msg, IscoMsgAckCodes ack)
        {
            /* Message structure:
             * Destination unit address (1 char)
             * Acknowledgement code, normally "R" for sending (1 char)
             * Message source unit address, normally "0" for sending from control computer
             *      If no message, this is a space, aka 0x20(1 char)
             * Length of message in hex. Max length is 256, with 256 indicated as "00" (2 char)
             * Message (256 char max)
             * Checksum (2 char)
            */

            var outFrame = dest.ToString();

            switch (ack)
            {
                case IscoMsgAckCodes.Recvd:
                    outFrame += "R";
                    break;
                case IscoMsgAckCodes.PumpBusy:
                    outFrame += "B";
                    break;
                case IscoMsgAckCodes.PumpComError:
                    outFrame += "E";
                    break;
                default:
                    // Should never get here
                    return "";
            }

            var msgLength = msg.Length;

            if (msgLength == 0)
            {
                outFrame += " ";
            }
            else
            {
                var msgLengthStr = (msgLength % 256).ToString("X2");
                outFrame += srce.ToString() + msgLengthStr + msg;
            }

            return outFrame + ObtainChecksum(outFrame);
        }

        /// <summary>
        /// Converts the status code read from the pump to an operation status enum
        /// </summary>
        /// <param name="inpStr">Input status code</param>
        /// <returns>Operation status enum</returns>
        private IscoOperationStatus ConvertStatusCodeToEnum(string inpStr)
        {
            IscoOperationStatus outVal;

            switch (inpStr)
            {
                case "S":
                    outVal = IscoOperationStatus.Stopped;
                    break;
                case "R":
                    outVal = IscoOperationStatus.Running;
                    break;
                case "F":
                    outVal = IscoOperationStatus.Refilling;
                    break;
                case "H":
                    outVal = IscoOperationStatus.Hold;
                    break;
                case "E":
                    outVal = IscoOperationStatus.Equilibrating;
                    break;
                default:
                    // Shouldn't ever get here
                    throw new IscoException("Invalid operation status code: " + inpStr);
            }

            return outVal;
        }

        /// <summary>
        /// Converts the control type code read from the pump to a control type enum
        /// </summary>
        /// <param name="inpStr">Input control type code</param>
        /// <returns>Control type enum</returns>
        private IscoControlMode ConvertControlModeCodeToEnum(string inpStr)
        {
            IscoControlMode outVal;

            switch (inpStr)
            {
                case "L":
                    outVal = IscoControlMode.Local;
                    break;
                case "R":
                    outVal = IscoControlMode.Remote;
                    break;
                case "E":
                    outVal = IscoControlMode.External;
                    break;
                default:
                    // Should never get here
                    throw new IscoException("Invalid control type code received: " + inpStr);
            }

            return outVal;
        }

        /// <summary>
        /// Converts the problem code read from the pump to a problem type enum
        /// </summary>
        /// <param name="inpStr"></param>
        /// <returns></returns>
        private IscoProblemStatus ConvertProblemCodeToEnum(string inpStr)
        {
            IscoProblemStatus outVal;

            switch (inpStr)
            {
                case "-":
                    outVal = IscoProblemStatus.None;
                    break;
                case "E":
                    outVal = IscoProblemStatus.CylinderEmpty;
                    break;
                case "B":
                    outVal = IscoProblemStatus.CylinderBottom;
                    break;
                case "O":
                    outVal = IscoProblemStatus.OverPressure;
                    break;
                case "U":
                    outVal = IscoProblemStatus.UnderPressure;
                    break;
                case "M":
                    outVal = IscoProblemStatus.MotorFailure;
                    break;
                default:
                    // Should never get here
                    throw new IscoException("Invalid problem code received: " + inpStr);
            }

            return outVal;
        }

        /// <summary>
        /// Parses reply from a status request
        /// </summary>
        /// <param name="inpMsg">Status request reply string</param>
        /// <returns>Array containing status for all 3 pumps</returns>
        private IscoPumpData[] ParseStatusMessage(string inpMsg)
        {
            // Set up return array
            IscoPumpData[] retArray = { new IscoPumpData(), new IscoPumpData(), new IscoPumpData() };

            // Wrap the whole mess in a try/catch just in case something goes wrong
            try
            {
                // Strip off unused prefix
                var signPos = inpMsg.IndexOf("=", StringComparison.Ordinal);
                var pumpData = inpMsg.Substring(signPos + 1, inpMsg.Length - signPos - 1);

                // Split the input message into fields (aka tokens, in original VB6 code)
                var tokens = pumpData.Split(',');

                /* ------------------------------------------------------------------------
                 * Token structure:
                 *      Array elements 0 - 2: Pump A, B, and C pressure
                 *      Array elements 3 - 7: Analog inputs 1-5
                 *      Array element 8: Digital inputs 1-8
                 *      Array element 9: Flow rate pump A
                 *      Array element 10: Volume pump A
                 *      Array element 11: Operation status pump A
                 *      Array element 12: Control status pump A
                 *      Array element 13: Problem status pump A
                 *      Array elements 14 - 18: Repeat of elements 9 - 13 for pump B
                 *      Array elements 19 - 23: Repeat of elements 9 - 13 for pump C
                 *      Array element 24: System flow rate
                 *      Array element 25: System pressure
                 *      Array element 26: Total volume
                */

                // Parse the field data and store
                for (var pump = 0; pump < m_PumpCount; pump++)
                {
                    // Sample time
                    retArray[pump].PointTime = TimeKeeper.Instance.Now;// DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0));

                    // Pressure
                    retArray[pump].Pressure = IscoConversions.ConvertPressFromString(tokens[pump], models[pump]);

                    // Flow
                    var tokenIndx = 9 + (pump * 5);
                    retArray[pump].Flow = IscoConversions.ConvertFlowFromString(tokens[tokenIndx]);

                    // Volume
                    retArray[pump].Volume = IscoConversions.ConvertVolumeFromString(tokens[tokenIndx + 1]);

                    // Operaton status
                    retArray[pump].OperationStatus = ConvertStatusCodeToEnum(tokens[tokenIndx + 2]);

                    // Control mode
                    retArray[pump].ControlMode = ConvertControlModeCodeToEnum(tokens[tokenIndx + 3]);

                    // Problem status
                    retArray[pump].ProblemStatus = ConvertProblemCodeToEnum(tokens[tokenIndx + 4]);
                }

                return retArray;
            }
            catch (Exception ex)
            {
                var msg = "Exception parsing status string";
                var notifyStr =
                    IscoErrorNotifications.GetNotificationString(IscoProblemStatus.MessageParseError.ToString());
                var args = new DeviceErrorEventArgs(msg, ex,
                                                            DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                                            notifyStr);
                Error?.Invoke(this, args);
                return null;
            }
        }

        /// <summary>
        /// Parses a mesaage string to get the flow, pressure, or volume value
        /// </summary>
        /// <param name="inpMsg">Input message</param>
        /// <returns>Numeric value in string, or -1000 if there's a problem</returns>
        private double ParseNumericValue(string inpMsg)
        {
            try
            {
                var signPos = inpMsg.IndexOf("=", StringComparison.Ordinal);
                var valueStr = inpMsg.Substring(signPos + 1, inpMsg.Length - signPos - 1);
                return double.Parse(valueStr);
            }
            catch (Exception ex)
            {
                var msg = "Exception parsing numeric field";
                var notifyStr =
                    IscoErrorNotifications.GetNotificationString(IscoProblemStatus.MessageParseError.ToString());
                var args = new DeviceErrorEventArgs(msg, ex,
                                                            DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                                            notifyStr);
                Error?.Invoke(this, args);
                return -1000;
            }
        }

        /// <summary>
        /// Parses the response to a RANGE command
        /// </summary>
        /// <param name="inpMsg">Input message</param>
        /// <returns>Object containing instrument range data</returns>
        private IscoPumpRangeData ParseRangeMessage(string inpMsg)
        {
            // Split the input message at the commas
            var rangeArray = inpMsg.Split(',');

            // Verify correct number of arguments
            if (rangeArray.Length != 4)
            {
                var msg = "Invalid field count in RANGE response string";
                var notifyStr =
                    IscoErrorNotifications.GetNotificationString(IscoProblemStatus.MessageParseError.ToString());
                var args = new DeviceErrorEventArgs(msg, null,
                                                            DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                                            notifyStr);
                Error?.Invoke(this, args);
                return null;
            }

            var retData = new IscoPumpRangeData();
            try
            {
                // Pressure field
                var signIndx = rangeArray[0].IndexOf("=", StringComparison.Ordinal);
                var parseStr = rangeArray[0].Substring(signIndx, rangeArray[0].Length - signIndx);
                retData.MaxPressure = ParseNumericValue(parseStr);

                // Flow field
                signIndx = rangeArray[1].IndexOf("=", StringComparison.Ordinal);
                parseStr = rangeArray[1].Substring(signIndx, rangeArray[1].Length - signIndx);
                retData.MaxFlowRate = ParseNumericValue(parseStr);

                // Refill rate field
                signIndx = rangeArray[2].IndexOf("=", StringComparison.Ordinal);
                parseStr = rangeArray[2].Substring(signIndx, rangeArray[2].Length - signIndx);
                retData.MaxRefillRate = ParseNumericValue(parseStr);

                // Volume field
                signIndx = rangeArray[3].IndexOf("=", StringComparison.Ordinal);
                parseStr = rangeArray[3].Substring(signIndx, rangeArray[3].Length - signIndx);
                retData.MaxVolume = ParseNumericValue(parseStr);

                return retData;
            }
            catch (Exception ex)
            {
                var msg = "Exception parsing RANGE string";
                var notifyStr =
                    IscoErrorNotifications.GetNotificationString(IscoProblemStatus.MessageParseError.ToString());
                var args = new DeviceErrorEventArgs(msg, ex,
                                                            DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                                            notifyStr);
                Error?.Invoke(this, args);
                return null;
            }
        }

        /// <summary>
        /// Parses the response to a "UNITS" command and sets unit conversion settings
        /// </summary>
        /// <param name="inpMsg">Message received from pump</param>
        private bool ParseUnitsMessage(string inpMsg)
        {
            // Split the message
            var unitsArray = inpMsg.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            // Verify correct number of arguments, for the 100D length should be 2,
            // for 65D the length should be 3, but we still only care about the first 2.
            if (unitsArray.Length != 2 && unitsArray.Length != 3)
            {
                ApplicationLogger.LogError(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "ParseUnits array length wrong: " + unitsArray.Length + " Array: " + unitsArray);
                IscoConversions.PressUnits = IscoPressureUnits.error;
                IscoConversions.FlowUnits = IscoFlowUnits.error;

                var msg = "Invalid field count in UNITSA response string";
                var notifyStr =
                    IscoErrorNotifications.GetNotificationString(IscoProblemStatus.MessageParseError.ToString());
                var args = new DeviceErrorEventArgs(msg, null,
                                                            DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                                            notifyStr);
                Error?.Invoke(this, args);
                return false;
            }

            try
            {
                // Pressure field
                var signIndx = unitsArray[0].IndexOf("=", StringComparison.Ordinal);
                var tmpStr = unitsArray[0].Substring(signIndx + 1, unitsArray[0].Length - signIndx - 1);
                IscoConversions.PressUnits = IscoConversions.ConvertPressStrToEnum(tmpStr);

                // Flow field
                signIndx = unitsArray[1].IndexOf("=", StringComparison.Ordinal);
                tmpStr = unitsArray[1].Substring(signIndx + 1, unitsArray[1].Length - signIndx - 1);
                IscoConversions.FlowUnits = IscoConversions.ConvertFlowStrToEnum(tmpStr);
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "ParseUnits exception:  " + ex.Message);
                var msg = "Exception parsing UNITSA string";
                var notifyStr =
                    IscoErrorNotifications.GetNotificationString(IscoProblemStatus.MessageParseError.ToString());
                var args = new DeviceErrorEventArgs(msg, ex,
                                                            DeviceErrorStatus.ErrorAffectsAllColumns, this,
                                                            notifyStr);
                Error?.Invoke(this, args);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Device name
        /// </summary>
        public string Name
        {
            get => m_Name;
            set
            {
                if (value != null && this.RaiseAndSetIfChangedRetBool(ref m_Name, value))
                {
                    DeviceSaveRequired?.Invoke(this, null);
                }
            }
        }

        /// <summary>
        /// Device status
        /// </summary>
        public DeviceStatus Status
        {
            get => m_Status;
            set => m_Status = value;
        }

        public ManualResetEvent AbortEvent { get; set; }

        /// <summary>
        /// Error type for device
        /// </summary>
        public DeviceErrorStatus ErrorType { get; set; }

        /// <summary>
        /// Device type for component
        /// </summary>
        public DeviceType DeviceType => DeviceType.Component;

        public bool Initialize(ref string errorMessage)
        {
            var notifyStr = IscoStatusNotifications.GetNotificationString(IscoOperationStatus.Initializing.ToString());
            var args = new DeviceStatusEventArgs(DeviceStatus.NotInitialized, notifyStr, this);
            StatusUpdate?.Invoke(this, args);

            if (!InitDevice(ref errorMessage))
            {
                return false;
            }

            notifyStr = IscoStatusNotifications.GetNotificationString(IscoOperationStatus.InitSucceeded.ToString());
            args = new DeviceStatusEventArgs(DeviceStatus.Initialized, notifyStr, this);
            StatusUpdate?.Invoke(this, args);
            return true;
        }

        public bool Shutdown()
        {
            Disconnect();
            return true;
        }

        public void SaveDeviceSettings(XmlElement deviceNode)
        {
            deviceNode.SetAttribute("Name", Name);
            deviceNode.SetAttribute("Emulation", Emulation.ToString());
            deviceNode.SetAttribute("PumpCount", PumpCount.ToString());
            deviceNode.SetAttribute("UnitAddress", UnitAddress.ToString());
            deviceNode.SetAttribute("PortName", PortName);
            deviceNode.SetAttribute("BaudRate", BaudRate.ToString());
            deviceNode.SetAttribute("ReadTimeout", ReadTimeout.ToString());
            deviceNode.SetAttribute("WriteTimeout", WriteTimeout.ToString());
        }

        public void LoadDeviceSettings(XmlElement deviceNode)
        {
            var doc = new XmlDocument();
            var root = doc.CreateElement("Root");
            doc.AppendChild(root);

            var importedNode = doc.ImportNode(deviceNode, true);
            if (doc.DocumentElement == null)
                return;

            doc.DocumentElement.AppendChild(importedNode);

            var xPath = "//@Name";
            var tmpAttribute = (XmlAttribute)doc.SelectSingleNode(xPath);
            if (tmpAttribute != null)
            {
                Name = tmpAttribute.Value;
            }

            xPath = "//@Emulation";
            tmpAttribute = (XmlAttribute)doc.SelectSingleNode(xPath);
            if (tmpAttribute != null)
            {
                Emulation = bool.Parse(tmpAttribute.Value);
            }

            xPath = "//@PumpCount";
            tmpAttribute = (XmlAttribute)doc.SelectSingleNode(xPath);
            if (tmpAttribute != null)
            {
                PumpCount = int.Parse(tmpAttribute.Value);
            }

            xPath = "//@UnitAddress";
            tmpAttribute = (XmlAttribute)doc.SelectSingleNode(xPath);
            if (tmpAttribute != null)
            {
                UnitAddress = int.Parse(tmpAttribute.Value);
            }

            xPath = "//@PortName";
            tmpAttribute = (XmlAttribute)doc.SelectSingleNode(xPath);
            if (tmpAttribute != null)
            {
                PortName = tmpAttribute.Value;
            }

            xPath = "//@BaudRate";
            tmpAttribute = (XmlAttribute)doc.SelectSingleNode(xPath);
            if (tmpAttribute != null)
            {
                BaudRate = int.Parse(tmpAttribute.Value);
            }

            xPath = "//@ReadTimeout";
            tmpAttribute = (XmlAttribute)doc.SelectSingleNode(xPath);
            if (tmpAttribute != null)
            {
                ReadTimeout = int.Parse(tmpAttribute.Value);
            }

            xPath = "//@WriteTimeout";
            tmpAttribute = (XmlAttribute)doc.SelectSingleNode(xPath);
            if (tmpAttribute != null)
            {
                WriteTimeout = int.Parse(tmpAttribute.Value);
            }
        }

        public void RegisterDataProvider(string key, DelegateDeviceHasData remoteMethod)
        {
        }

        public void UnRegisterDataProvider(string key, DelegateDeviceHasData remoteMethod)
        {
        }

        public void WritePerformanceData(string directoryPath, string methodName, object[] parameters)
        {
        }

        public List<string> GetStatusNotificationList()
        {
            return IscoStatusNotifications.GetNotificationListStrings();
        }

        public List<string> GetErrorNotificationList()
        {
            return IscoErrorNotifications.GetNotificationListStrings();
        }

        public event EventHandler<DeviceStatusEventArgs> StatusUpdate;

        public event EventHandler<DeviceErrorEventArgs> Error;

        public event EventHandler DeviceSaveRequired;

        public override string ToString()
        {
            return Name;
        }

        public double GetPressure()
        {
            return 0.0;
        }

        public double GetFlowRate()
        {
            return 0.0;
        }

        public double GetPercentB()
        {
            return 0.0;
        }

        public double GetActualFlow()
        {
            return 0.0;
        }

        public double GetMixerVolume()
        {
            return 0.0;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
