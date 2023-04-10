using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Agilent.Licop;
using FluidicsSDK.Devices;
using LcmsNetSDK;
using LcmsNetSDK.Data;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Logging;
using LcmsNetSDK.Method;
using LcmsNetSDK.System;

namespace LcmsNetPlugins.Agilent.Pumps
{
    /// <summary>
    /// Interface to Agilent Pumps for running the solution of a gradient.
    /// </summary>
    [Serializable]
    [DeviceControl(typeof(AgilentPumpViewModel),
                                 "Agilent 1200 Nano Series",
                                 "Pumps")]
    public class AgilentPump : IDevice, IPump, IFluidicsPump, IDisposable
    {
        /// <summary>
        /// An 'instrument' object for the Agilent pump drivers
        /// </summary>
        private Instrument pumpModule = null;

        /// <summary>
        /// A 'module' object for the Agilent pump drivers
        /// </summary>
        private Module communicationModule;

        /// <summary>
        /// Error reporting channel.
        /// </summary>
        private Channel eventDataChannel;

        /// <summary>
        /// Channel for retrieving methods from the pumps.
        /// </summary>
        private Channel listDataChannel;

        /// <summary>
        /// A 'channel' object for the Agilent pump drivers
        /// </summary>
        private Channel commandDataChannel;

        /// <summary>
        /// Channel when monitoring the instrument.
        /// </summary>
        private Channel monitorDataChannel;

        /// <summary>
        /// The device's name.
        /// </summary>
        private string deviceName;

        /// <summary>
        /// The flow rate (used for save/load)
        /// </summary>
        private double currentFlowRate = -1.0;

        /// <summary>
        /// Filesystem watcher for real-time updating of pump methods.
        /// </summary>
        private static FileSystemWatcher methodFileWatcher;

        /// <summary>
        /// Dictionary that holds a method name, key, and the method time table, value.
        /// </summary>
        private static Dictionary<string, string> availableMethods;

        /// <summary>
        /// Status strings from the pumps.
        /// </summary>
        private static string[] notificationStrings;

        private static Dictionary<string, string> errorCodes;
        private static Dictionary<string, string> statusCodes;
        private static Dictionary<string, string> statusErrorCodes;

        private PumpState pumpState;
        private string pumpModel;
        private string pumpSerial;
        private string pumpFirmware;
        private readonly Timer statusReadTimer = null;
        private readonly object pumpCommLock = new object();
        private bool isMonitoringDisabled = false;
        private int statusReadErrorRepeatCount = 0;

        /// <summary>
        /// Status of the device.
        /// </summary>
        private DeviceStatus deviceStatus;

        private const string CONST_DEFAULTPORT = "COM1";
        private const int CONST_DEFAULTTIMEOUT = 6000; //milliseconds
        private const int CONST_WRITETIMEOUT = 10000; //milliseconds
        private const int CONST_READTIMEOUT = 10000; //milliseconds
        private const int CONST_MONITORING_MINUTES = 10;
        private const int CONST_MONITORING_SECONDS_ELAPSED = 10;

        /// <summary>
        /// Notification string for flow changes
        /// </summary>
        private const string CONST_FLOW_CHANGE = "Flow % below set point";

        /// <summary>
        /// Notification string for pressure values.
        /// </summary>
        private const string CONST_PRESSURE_VALUE = "Pressure Value";

        private const string CONST_ERROR_ABOVE_PRESSURE = "Pressure Above Limit";
        private const string CONST_ERROR_BELOW_PRESSURE = "Pressure Below Limit";
        private const string CONST_ERROR_FLOW_EXCEEDS = "Flow Exceeds limit while pressure control";
        private const string CONST_ERROR_FLOW_UNSTABLE = "Column flow is unstable";

        /// <summary>
        /// The error message was not set.
        /// </summary>
        private const string CONST_DEFAULT_ERROR = "None";

        private const string CONST_PUMP_ERROR = "Pump Error";
        private const string CONST_INITIALIZE_ERROR = "Failed to Initialize";
        private const string CONST_COMPOSITION_B_SET = "Failed to set composition B";
        private const string CONST_FLOW_SET = "Failed to set flow rate";
        private const string CONST_PRESSURE_SET = "Failed to set pressure";
        private const string CONST_VOLUME_SET = "Failed to set volume";

        /// <summary>
        /// Fired when monitoring data is received from the instrument.
        /// </summary>
        public event EventHandler<PumpDataEventArgs> MonitoringDataReceived;

        /// <summary>
        /// Indicates that a save is required in the Fluidics Designer
        /// </summary>
        public event EventHandler<DeviceStatusEventArgs> StatusUpdate;

        /// <summary>
        /// Fired when the Agilent Pump finds out what method names are available.
        /// </summary>
        public event DelegateDeviceHasData MethodNames;

        /// <summary>
        /// Fired when a property changes in the device.
        /// </summary>
        public event EventHandler DeviceSaveRequired;

        /// <summary>
        /// Fired when an error occurs in the device.
        /// </summary>
        public event EventHandler<DeviceErrorEventArgs> Error;

        /// <summary>
        /// Monitoring data - pump readbacks
        /// </summary>
        public List<PumpDataPoint> MonitoringData;

        /// <summary>
        /// Default constructor - for use within LCMSNet
        /// </summary>
        public AgilentPump() : this(false)
        {
        }

        /// <summary>
        /// Constructor - provides option to skip reading methods from file.
        /// </summary>
        public AgilentPump(bool skipMethodLoad)
        {
            CreateErrorCodes();
            CreateStatusCodes();
            CreateStatusErrorCodes();

            PortName = CONST_DEFAULTPORT;
            if (availableMethods == null)
            {
                availableMethods = new Dictionary<string, string>();
            }
            if (methodFileWatcher == null && !skipMethodLoad)
            {
                var path = PersistDataPaths.GetDirectoryLoadPathCheckFiles("PumpMethods", "*.txt");
                //ApplicationLogger.LogMessage(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "PATH: " + path);
                methodFileWatcher = new FileSystemWatcher(path, "*.txt");
                methodFileWatcher.Created += mwatcher_methods_Created;
                methodFileWatcher.Changed += mwatcher_methods_Changed;
                methodFileWatcher.EnableRaisingEvents = true;
            }
            deviceName = "pump";

            MonitoringData = new List<PumpDataPoint>();
            deviceStatus = DeviceStatus.NotInitialized;

            TotalMonitoringMinutesDataToKeep = CONST_MONITORING_MINUTES;
            TotalMonitoringSecondsElapsed = CONST_MONITORING_SECONDS_ELAPSED;

            if (notificationStrings == null)
            {
                notificationStrings = new[]{
                    CONST_FLOW_CHANGE,
                    CONST_PRESSURE_VALUE
                };
            }

            MobilePhases = new List<MobilePhase>
            {
                new MobilePhase("A", "This is a test"),
                new MobilePhase("B", "This is a test"),
                new MobilePhase("Aux", "This is a test")
            };

            PurgeA1 = new PumpPurgeData(PumpPurgeChannel.A1);
            PurgeA2 = new PumpPurgeData(PumpPurgeChannel.A2);
            PurgeB1 = new PumpPurgeData(PumpPurgeChannel.B1);
            PurgeB2 = new PumpPurgeData(PumpPurgeChannel.B2);

            statusReadTimer = new Timer(UpdateStatus, this, Timeout.Infinite, Timeout.Infinite);
        }

        public void Dispose()
        {
            statusReadTimer?.Dispose();
        }

        private int lastStatusInterval = 2;

        private void UpdateStatus(object state)
        {
            if (pumpModule != null)
            {
                GetPumpStatus();
                var agPumpState = GetPumpState();

                var statusInterval = 3; // default to 3 seconds; applies to startup and error conditions
                if (agPumpState == PumpState.On &&
                    PumpStatus.ErrorState == AgilentPumpStateError.NO_ERROR &&
                    PumpStatus.NotReadyState == AgilentPumpStateNotReady.READY &&
                    PumpStatus.TestState == AgilentPumpStateTest.NO_TEST)
                {
                    if (PumpStatus.AnalysisState == AgilentPumpStateAnalysis.NO_ANALYSIS &&
                        PumpStatus.GenericState == AgilentPumpStateGeneric.PRERUN)
                    {
                        statusInterval = 10;
                    }
                    else if (PumpStatus.AnalysisState == AgilentPumpStateAnalysis.ANALYSIS &&
                             (PumpStatus.GenericState == AgilentPumpStateGeneric.RUN ||
                              PumpStatus.GenericState == AgilentPumpStateGeneric.POSTRUN))
                    {
                        statusInterval = 60;
                    }
                }

                if (statusInterval != lastStatusInterval)
                {
                    statusReadTimer.Change(TimeSpan.FromSeconds(statusInterval), TimeSpan.FromSeconds(statusInterval));
                    lastStatusInterval = statusInterval;
                }
            }
        }

        void mwatcher_methods_Changed(object sender, FileSystemEventArgs e)
        {
            var methodLoaded = false;
            do
            {
                try
                {
                    AddMethod(Path.GetFileNameWithoutExtension(e.FullPath), File.ReadAllText(e.FullPath));
                    ApplicationLogger.LogMessage(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, e.FullPath + " changed.");
                    methodLoaded = true;
                }
                catch (IOException)
                {
                    //probably caught the file being opened for writing.
                }
            } while (!methodLoaded);
        }

        void mwatcher_methods_Created(object sender, FileSystemEventArgs e)
        {
            //AddMethod(Path.GetFileNameWithoutExtension(e.FullPath), File.ReadAllText(e.FullPath));
            //ApplicationLogger.LogMessage(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, e.FullPath + " created.");
        }

        [DeviceSavedSetting("TotalMonitoringMinutes")]
        public int TotalMonitoringMinutesDataToKeep { get; set; }

        [DeviceSavedSetting("TotalMonitoringSecondsElapsed")]
        public int TotalMonitoringSecondsElapsed { get; set; }

        /// <summary>
        /// Gets or sets the Emulation state.
        /// </summary>
        public bool Emulation { get; set; }

        /// <summary>
        /// Gets the device's status
        /// </summary>
        public DeviceStatus Status
        {
            get => deviceStatus;
            set
            {
                if (value != deviceStatus)
                    StatusUpdate?.Invoke(this, new DeviceStatusEventArgs(value, "Status", this));
                deviceStatus = value;
            }
        }

        /// <summary>
        /// Gets or sets whether the device is running
        /// </summary>
        public bool Running { get; set; }

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
        /// Gets or sets the device's version
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the port name to use to communicate with the pumps.
        /// </summary>
        [DeviceSavedSetting("PortName")]
        public string PortName { get; set; }

        /// <summary>
        /// Pump model. Read on initialize
        /// </summary>
        public string PumpModel
        {
            get => pumpModel;
            private set => this.RaiseAndSetIfChanged(ref pumpModel, value);
        }

        /// <summary>
        /// Pump Serial Number. Read on initialize
        /// </summary>
        public string PumpSerial
        {
            get => pumpSerial;
            private set => this.RaiseAndSetIfChanged(ref pumpSerial, value);
        }

        /// <summary>
        /// Pump Firmware revision. Read on initialize
        /// </summary>
        public string PumpFirmware
        {
            get => pumpFirmware;
            private set => this.RaiseAndSetIfChanged(ref pumpFirmware, value);
        }

        /// <summary>
        /// Gets or sets the error type of the last error reported.
        /// </summary>
        public DeviceErrorStatus ErrorType { get; set; }

        /// <summary>
        /// Gets the system device type.
        /// </summary>
        public DeviceType DeviceType => DeviceType.Component;

        /// <summary>
        /// Gets or sets the abort event for scheduling.
        /// </summary>
        public System.Threading.ManualResetEvent AbortEvent { get; set; }

        public AgilentPumpInfo PumpInfo { get; } = new AgilentPumpInfo();
        public AgilentPumpStatus PumpStatus{ get; } = new AgilentPumpStatus();

        public PumpPurgeData PurgeA1 { get; }
        public PumpPurgeData PurgeA2 { get; }
        public PumpPurgeData PurgeB1 { get; }
        public PumpPurgeData PurgeB2 { get; }

        public PumpState PumpState
        {
            get => pumpState;
            set => this.RaiseAndSetIfChanged(ref pumpState, value);
        }

        public bool IsMonitoringDiabled => isMonitoringDisabled;

        public bool LoadPurgeData()
        {
            return GetPurgeData(PurgeA1) && GetPurgeData(PurgeA2) && GetPurgeData(PurgeB1) && GetPurgeData(PurgeB2);
        }

        public AgilentPumpReplyErrorCodes StartPurge()
        {
            var reply = "";
            return SendCommand("PURG 1", out reply);
        }

        /// <summary>
        /// Purges the pump for the number of minutes provided.
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="channel"></param>
        /// <param name="flow"></param>
        /// <param name="numberOfMinutes"></param>
        /// <returns></returns>
        [LCMethodEvent("Purge Channel", MethodOperationTimeoutType.Parameter)]
        public AgilentPumpReplyErrorCodes PurgePump(double timeout, PumpPurgeChannel channel, double flow, double numberOfMinutes)
        {
            var command = string.Format("PG{0} {1}, {2}", channel, Convert.ToInt32(flow), numberOfMinutes);
            var reply = "";
            var worked = SendCommand(command, out reply);

            if (worked != AgilentPumpReplyErrorCodes.No_Error)
                return worked;

            var bitField = 0;
            switch (channel)
            {
                case PumpPurgeChannel.A1:
                    bitField = 1;
                    break;
                case PumpPurgeChannel.A2:
                    bitField = 2;
                    break;
                case PumpPurgeChannel.B1:
                    bitField = 4;
                    break;
                case PumpPurgeChannel.B2:
                    bitField = 8;
                    break;
            }
            command = string.Format("PRGE {0}, 1, 1", bitField);
            worked = SendCommand(command, out reply);
            if (worked != AgilentPumpReplyErrorCodes.No_Error)
                return worked;

            return SendCommand("PURG 1", out reply);
        }

        /// <summary>
        /// Aborts purging of the pumps for the given channel.
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        [LCMethodEvent("Abort Purge", MethodOperationTimeoutType.Parameter)]
        public AgilentPumpReplyErrorCodes AbortPurges(double timeout)
        {
            var reply = "";
            var command = "PURG 0";
            var worked = SendCommand(command, out reply);
            return worked;
        }

        private void CreateErrorCodes()
        {
            if (errorCodes == null)
            {
                var map = new Dictionary<string, string>
                {
                    {"ER 2037", "STRT while purging"},
                    {"ER 2038", "STRT while fast composition change"},
                    {"ER 2039", "No calibration table with specified index existing"},
                    {"ER 2040", "Not an allowed index for a user-defined table"},
                    {"ER 2041", "Specified command allowed in test mode only"},
                    {"ER 2042", "Calibration table cannot be deleted while in use"},
                    {"ER 2043", "FCC not possible, no partner module connected"},
                    {"ER 2044", "FCC active, configuration change not allowed"},
                    {"ER 2045", "FCC not allowed in this mode"},
                    {"ER 2046", "FCC not possible, no column flow"},
                    {"ER 2047", "FSAC base points not specified in descending order."},
                    {"ER 2048", "Attempt to specify more than 11 points in FSAC table."}
                };

                errorCodes = map;
            }
        }

        private void CreateStatusCodes()
        {
            if (statusCodes == null)
            {
                var map = new Dictionary<string, string>
                {
                    {"EC 0001", "Module cold reset."},
                    {"EC 0002", "Module warm reset."},
                    {"EC 0003", "Module connected."},
                    {"EC 0010", "Module ready for communication."},
                    {"ES 0100", "Module Runtime state LEAK."},
                    {"ES 0101", "Module Runtime state SHUTDOWN."},
                    {"ES 0103", "Module Runtime state PRERUN."},
                    {"ES 0104", "Module Runtime state RUN."},
                    {"ES 0105", "Module Runtime state POSTRUN."},
                    {"ES 0102", "Module Error state ERROR."},
                    {"ES 0107", "Module Error state NOERROR."},
                    {"ES 0108", "Module Ready state NOTREADY."},
                    {"ES 0109", "Module Ready state READY."},
                    {"ES 0110", "Module Analysis state ANALYSIS."},
                    {"ES 0111", "Module Analysis state NO ANALYSIS."},
                    {"ES 0114", "Module Analysis state PENDING."},
                    {"ES 0112", "Module Test state TEST."},
                    {"ES 0113", "Module Test state NOTEST."},
                    {"ES 0128", "Module Start/Ready state START NOTREADY."},
                    {"ES 0129", "Module Start/Ready state START READY."},

                    {"EE 2066", "Column flow unstable."},
                    {"EE 2067", "No EMPV connected."},
                    {"EE 2068", "No flow sensor connected."},
                    {"EE 2069", "EMPV initialization failed."},
                    {"EE 2090", "Flow sensor not supported by pump."},
                    {"ES 2100", "Pump state PUMP OFF."},
                    {"ES 2101", "Pump state INIT PUMP."},
                    {"ES 2102", "Pump state PUMP STANDBY."},
                    {"ES 2104", "Pump state PUMP ON."},
                    {"ES 2110", "Pump composition state RAMP OFF."},
                    {"ES 2111", "Pump composition state RAMP ON."},
                    {"ES 2112", "Pump composition state RAMP HOLD."},
                    {"ES 2113", "Pump flow ramp state RAMP OFF."},
                    {"ES 2114", "Pump flow ramp state RAMP ON."},
                    {"ES 2115", "Pump flow ramp state RAMP HOLD."},
                    {"ES 2116", "Pump in normal mode."},
                    {"ES 2117", "Pump in micro mode."},
                    {"ES 2118", "Pump in test mode."},
                    {"ES 2119", "Purge valve on."},
                    {"ES 2120", "Purge valve off."},
                    {"ES 2121", "Column flow in sensor range."},
                    {"ES 2122", "Column flow out of sensor range."},
                    {"ES 2123", "Pump operating pressure control state ENABLED."},
                    {"ES 2124", "Pump operating pressure control state DISABLED."},
                    {"ES 2126", "Unknown startup event."},
                    {"EV 2227", "no start allowed while purging."},
                    {"EV 2228", "no start allowed while fast composition change."},
                    {"EV 2229", "Column flow in limit."},
                    {"EV 2230", "Column flow out of limit."},
                    {"EV 2231", "Fast composition change started."},
                    {"EV 2232", "Fast composition change complete."},
                    {"EV 2233", "Calibration table stored. Parameter: table reference, 1..10"},
                    {"EV 2234", "Calibration table deleted. Parameter: table reference, 1..10"},
                    {"EV 2235", "Fast Composition Change failed."},
                };

                statusCodes = map;
            }
        }

        private void CreateStatusErrorCodes()
        {
            if (statusErrorCodes == null)
            {
                var map = new Dictionary<string, string>
                {
                    {"EE 2014", CONST_ERROR_ABOVE_PRESSURE},
                    {"EE 2015", CONST_ERROR_BELOW_PRESSURE},
                    {"EE 2064", CONST_ERROR_FLOW_EXCEEDS},
                    {"EE 2066", CONST_ERROR_FLOW_UNSTABLE},
                    {"EE 2500", CONST_ERROR_ABOVE_PRESSURE},
                    {"EE 2501", CONST_ERROR_BELOW_PRESSURE},
                };

                statusErrorCodes = map;
            }
        }

        /// <summary>
        /// Handles events when a pump error occurs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PumpErrorOccurred(object sender, global::Agilent.Licop.ErrorEventArgs e)
        {
            if (e.Message == null)
                return;

            var errorMessage = e.Message.Split(',');
            if (errorCodes.ContainsKey(errorMessage[0]))
            {
                var displayedError = errorCodes[errorMessage[0]];
                HandleError(displayedError, displayedError);
            }
            else
            {
                HandleError("Pump " + Name + " failed " + e.Message, CONST_PUMP_ERROR);
            }
        }

        public void DisableMonitoring()
        {
            isMonitoringDisabled = true;

            if (monitorDataChannel != null && monitorDataChannel.IsOpen)
            {
                SendCommand("MONI:STOP", out _);
            }
        }

        public void EnableMonitoring()
        {
            isMonitoringDisabled = false;

            if (monitorDataChannel != null && monitorDataChannel.IsOpen)
            {
                SendCommand($"MONI:STRT {TotalMonitoringSecondsElapsed},\"ACT:FLOW?; ACT:PRES?; ACT:COMP?\"", out _);
            }
        }

        /// <summary>
        /// Clears all of the listed pump methods.
        /// </summary>
        public void ClearMethods()
        {
            availableMethods.Clear();
        }

        public void AddMethods(Dictionary<string, string> methods)
        {
            availableMethods = methods;
            ListMethods();
        }

        /// <summary>
        /// Adds a given method.
        /// </summary>
        /// <param name="methodname">Name of method to track.</param>
        /// <param name="method">Method data to store.</param>
        public void AddMethod(string methodname, string method)
        {
            if (availableMethods.ContainsKey(methodname))
            {
                availableMethods[methodname] = method;

                // Don't need to fire ListMethods() - the name didn't change.
            }
            else
            {
                availableMethods.Add(methodname, method);

                ListMethods();
            }
        }

        /// <summary>
        /// Lists the methods
        /// </summary>
        private void ListMethods()
        {
            if (MethodNames != null)
            {
                var keys = new string[availableMethods.Keys.Count];
                availableMethods.Keys.CopyTo(keys, 0);

                var data = new List<object>();
                data.AddRange(keys);

                MethodNames(this, data);
            }
        }

        /// <summary>
        /// Initializes the device.
        /// </summary>
        /// <returns>True on success</returns>
        public bool Initialize(ref string errorMessage)
        {
            if (Emulation)
            {
                return true;
            }

            pumpModule = new Instrument(CONST_DEFAULTTIMEOUT, CONST_DEFAULTTIMEOUT);
            pumpModule.ErrorOccurred += PumpErrorOccurred;

            // Try initial connection
            if (pumpModule.TryConnect(PortName, CONST_DEFAULTTIMEOUT) == false)
            {
                errorMessage = "Could not connect to the Agilent Pumps.";
                HandleError(errorMessage, CONST_INITIALIZE_ERROR);
                //Could not connect
                return false;
            }

            communicationModule = pumpModule.CreateModule(pumpModule.GetAccessPointIdentifier());
            PumpModel = communicationModule.Type;
            PumpSerial = communicationModule.Serial;

            // Channel for inputs
            commandDataChannel = communicationModule.CreateChannel("IN");
            if (commandDataChannel.TryOpen(ReadMode.Polling) == false)
            {
                //"Could not open IN channel."
                errorMessage = "Could not open the communication channel for input";
                HandleError(errorMessage, CONST_INITIALIZE_ERROR);
                return false;
            }

            // Get the firmware revision of the running system
            // "IDN? gets "<manufacturer>,<model>,<serialNumber>,<running firmware revision>"; "IDN" causes "Identify by frontend LED"
            var gotIdent = SendCommand("IDN?", out var reply, "");
            var firmware = "";
            //ApplicationLogger.LogMessage(2, $"Pump {Name}: Got reply \"{reply}\"");
            if (gotIdent == AgilentPumpReplyErrorCodes.No_Error)
            {
                var split = reply.Replace("\"", "").Split(',');
                firmware = split[3];
            }

            PumpFirmware = firmware;
            GetPumpInformation();
            GetPumpStatus();

            // Open a list channel to read time tables.
            listDataChannel = communicationModule.CreateChannel("LI");
            if (listDataChannel.TryOpen(ReadMode.Polling) == false)
            {
                errorMessage = "Could not open the communication channel for time table data.";
                HandleError(errorMessage, CONST_INITIALIZE_ERROR);
                return false;
            }

            // And an EV channel.Channel for errors or state events
            eventDataChannel = communicationModule.CreateChannel("EV");
            eventDataChannel.DataReceived += evChannel_DataReceived;
            if (eventDataChannel.TryOpen(ReadMode.Events) == false)
            {
                //"Could not open EV channel."
                errorMessage = "Could not open the communication channel for error events";
                HandleError(errorMessage, CONST_INITIALIZE_ERROR);
                return false;
            }

            // Monitoring for the pumps
            monitorDataChannel = communicationModule.CreateChannel("MO");
            monitorDataChannel.DataReceived += monitorChannel_DataReceived;
            if (monitorDataChannel.TryOpen(ReadMode.Events) == false)
            {
                errorMessage = "Could not open the communication channel for monitoring data";
                HandleError(errorMessage, CONST_INITIALIZE_ERROR);
                return false;
            }

            var worked = AgilentPumpReplyErrorCodes.No_Error;
            if (!isMonitoringDisabled)
            {
                worked = SendCommand(
                    $"MONI:STRT {TotalMonitoringSecondsElapsed},\"ACT:FLOW?; ACT:PRES?; ACT:COMP?\"",
                    out reply);
            }

            if (worked != AgilentPumpReplyErrorCodes.No_Error)
            {
                errorMessage = "Could not put the pumps in monitoring mode.";
                HandleError(errorMessage, CONST_INITIALIZE_ERROR);
                return false;
            }

            GetPumpState();
            lastStatusInterval = 3;
            statusReadTimer.Change(TimeSpan.FromSeconds(lastStatusInterval), TimeSpan.FromSeconds(lastStatusInterval));

            return true;
        }

        /// <summary>
        /// Internal error handler that propagates the error message to listening objects.
        /// </summary>
        private void HandleError(string message, string type, Exception ex = null)
        {
            if (Error != null)
            {
                if (type == "")
                {
                    type = CONST_DEFAULT_ERROR;
                }

                Error(this, new DeviceErrorEventArgs(message, ex, DeviceErrorStatus.ErrorAffectsAllColumns, this, type));
            }
        }

        private void UpdateNotificationStatus(string message, string type)
        {
            if (StatusUpdate != null)
            {
                var args = new DeviceStatusEventArgs(Status, type, this, message);
                StatusUpdate(this, args);
            }
        }

        /// <summary>
        /// Handles errors from the pump.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void evChannel_DataReceived(object sender, DataEventArgs e)
        {
            var data = e.AsciiData;
            var message = data.Split(',');
            var code = message[0];

            if (statusErrorCodes.TryGetValue(code, out var errorMessage))
            {
                Error?.Invoke(this, new DeviceErrorEventArgs(errorMessage, null, DeviceErrorStatus.ErrorAffectsAllColumns, this, errorMessage));
            }
            else if (statusCodes.TryGetValue(code, out var eventMessage))
            {
                HandleError(eventMessage, "Pump Event");
            }
            else
            {
                HandleError("Pump " + Name + " status: " + data, CONST_PUMP_ERROR);
            }

            //LcmsNetDataClasses.Logging.ApplicationLogger.LogMessage(2, Name + " Agilent Pump Message " + data);
        }

        /// <summary>
        /// Handles monitoring data from the pumps.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void monitorChannel_DataReceived(object sender, DataEventArgs e)
        {
            try
            {
                if (e?.AsciiData == null)
                    return;

                var pressure = double.NaN;
                var flowrate = double.NaN;
                var compositionB = double.NaN;
                //"MO 0000 ACT:FLOW 0.000;ACT:PRES 0.94;ACT:COMP 20.0,80.0,-1.0,-1.0;"

                // Parse out the data
                //var dataArray = e.AsciiData.Split(new[] { "ACT:FLOW" }, StringSplitOptions.RemoveEmptyEntries);
                //if (dataArray.Length > 1)
                //{
                //    var data = dataArray[1].Replace(";ACT:", ",");
                //    data = data.Replace("COMP", "");
                //    data = data.Replace("PRES", "");

                //    var values = data.Split(',');
                //    flowrate = Convert.ToDouble(values[0]);
                //    pressure = Convert.ToDouble(values[1]);
                //    compositionB = Convert.ToDouble(values[3]);
                //}
                var components = e.AsciiData.Split(';');
                if (components.Length > 1)
                {
                    flowrate = Convert.ToDouble(components[0].Split(' ').Last());
                    pressure = Convert.ToDouble(components[1].Split(' ').Last());
                    compositionB = Convert.ToDouble(components[2].Split(' ').Last().Split(',')[1]);
                }
                ProcessMonitoringData(flowrate, pressure, compositionB);
            }
            catch (Exception)
            {
                // Leave blank....
            }
        }

        public void PushData(double flowrate, double pressure, double compositionB)
        {
            ProcessMonitoringData(flowrate, pressure, compositionB);
        }

        private void ProcessMonitoringData(double flowrate, double pressure, double compositionB)
        {
            var time = TimeKeeper.Instance.Now; // DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0));

            // notifications
            if (Math.Abs(currentFlowRate) > float.Epsilon)
            {
                var percentFlowChange = (currentFlowRate - flowrate) / currentFlowRate;
                UpdateNotificationStatus(percentFlowChange.ToString("0.00"), CONST_FLOW_CHANGE);
            }
            UpdateNotificationStatus(pressure.ToString("0.000"), CONST_PRESSURE_VALUE);

            // Update log collection.
            MonitoringData.Add(new PumpDataPoint(time, pressure, flowrate, compositionB));

            // Find old data to remove -- needs to be updated (or could be) using LINQ
            var count = MonitoringData.Count;
            var total = (TotalMonitoringMinutesDataToKeep * 60) / TotalMonitoringSecondsElapsed;
            if (count >= total)
            {
                var i = 0;
                while (time.Subtract(MonitoringData[i].Time).TotalMinutes > TotalMonitoringMinutesDataToKeep && i < MonitoringData.Count)
                {
                    i++;
                }

                if (i > 0)
                {
                    i = Math.Min(i, MonitoringData.Count - 1);
                    MonitoringData.RemoveRange(0, i);
                }
            }

            // Alert the user data is ready
            try
            {
                MonitoringDataReceived?.Invoke(this, new PumpDataEventArgs(this, MonitoringData));
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// Closes the connection to the device
        /// </summary>
        /// <returns>True on success</returns>
        public bool Shutdown()
        {
            if (Emulation)
            {
                return true;
            }

            if (eventDataChannel == null)
                return true;

            // close EV channel
            eventDataChannel.Close();

            // close IN channel
            commandDataChannel.Close();

            monitorDataChannel.DataReceived -= monitorChannel_DataReceived;

            // disconnect
            pumpModule?.Disconnect();

            return true;
        }

        /// <summary>
        /// Overload that reports extra details on error
        /// </summary>
        /// <param name="command"></param>
        /// <param name="reply"></param>
        /// <param name="errorMessage"></param>
        /// <param name="readChannel"></param>
        /// <returns></returns>
        private AgilentPumpReplyErrorCodes SendCommand(string command, out string reply, string errorMessage, Channel readChannel = null)
        {
            var result = SendCommand(command, out reply, readChannel);
            if (result != AgilentPumpReplyErrorCodes.No_Error)
            {
                ApplicationLogger.LogError(0, errorMessage);
            }

            return result;
        }

        /// <summary>
        /// Send a command to the Agilent pump
        /// </summary>
        /// <param name="command"></param>
        /// <param name="reply"></param>
        /// <param name="readChannel"></param>
        /// <returns></returns>
        private AgilentPumpReplyErrorCodes SendCommand(string command, out string reply, Channel readChannel = null)
        {
            reply = "";
            if (Emulation)
            {
                return AgilentPumpReplyErrorCodes.No_Error;
            }

            if (readChannel == null)
            {
                readChannel = commandDataChannel;
            }

            lock (pumpCommLock)
            {
                //Send the command over our serial port
                //TODO: Wrap this in exception checking
                //      (if there is an error, send out errorstring)
                if (commandDataChannel.TryWrite(command, CONST_WRITETIMEOUT) == false)
                {
                    //Couldn't send instruction
                    ApplicationLogger.LogError(0, $"Agilent Pump \"{Name}\": Command \"{command}\" got error response \"{AgilentPumpReplyErrorCodes.Instruction_Send_Failed}\"");
                    return AgilentPumpReplyErrorCodes.Instruction_Send_Failed;
                }

                if (readChannel.TryRead(out reply, CONST_READTIMEOUT) == false)
                {
                    //Couldn't read reply
                    ApplicationLogger.LogError(0, $"Agilent Pump \"{Name}\": Command \"{command}\" got error response \"{AgilentPumpReplyErrorCodes.Reply_Read_Failed}\"");
                    return AgilentPumpReplyErrorCodes.Reply_Read_Failed;
                }
            }

            // Reply information:
            // RA 0000 ....: Request accepted (no error)
            // RA xxxx ....: Request accepted (no error), xxxx is number of items received
            // RE xxxx ....: Request error (xxxx is the code)
            var errorCode = AgilentPumpReplyErrorCodes.No_Error;
            if (reply.StartsWith("RA", StringComparison.OrdinalIgnoreCase))
            {
                errorCode = AgilentPumpReplyErrorCodes.No_Error;
            }
            else if (reply.StartsWith("RE", StringComparison.OrdinalIgnoreCase))
            {
                var error = reply.Substring(3, 4);
                if (!Enum.TryParse(error, out errorCode))
                {
                    errorCode = AgilentPumpReplyErrorCodes.Unknown_Error;
                }
                ApplicationLogger.LogError(0, $"Agilent Pump \"{Name}\": Command \"{command}\" got error response \"{errorCode}\" (full text: {reply})");
            }

            return errorCode;
        }

        /// <summary>
        /// Sets the pump mode.
        /// </summary>
        /// <param name="newMode">The new mode</param>
        [LCMethodEvent("Set Mode", 1)]
        public void SetMode(AgilentPumpModes newMode)
        {
            if (Emulation)
            {
                return;
            }

            var reply = "";
            SendCommand("MODE " + (int)newMode, out reply, "Attempting to set mode to " + newMode.ToString());
        }

        /// <summary>
        /// Sets the flow rate.
        /// </summary>
        /// <param name="newFlowRate">The new flow rate.</param>
        [LCMethodEvent("Set Flow Rate", 1)]
        public void SetFlowRate(double newFlowRate)
        {
            if (Emulation)
            {
                return;
            }
            currentFlowRate = newFlowRate;
            var reply = "";
            SendCommand("FLOW " + newFlowRate.ToString(CultureInfo.InvariantCulture),
                out reply, "Attempting to set flow rate to " + newFlowRate.ToString(CultureInfo.InvariantCulture));
        }

        ///// <summary>
        ///// Sets the mixer volume
        ///// </summary>
        ///// <param name="newVolumeuL">The new mixer volume, in uL</param>
        //[LCMethodEvent("Set Mixer Volume", 1)]
        //public void SetMixerVolume(double newVolumeuL)
        //{
        //    if (Emulation)
        //    {
        //        return;
        //    }

        //    var reply = "";
        //    if (newVolumeuL >= 0 && newVolumeuL <= 2000)
        //        SendCommand("MVOL " + newVolumeuL.ToString(CultureInfo.InvariantCulture),
        //            out reply, "Attempting to set mixer volume to " + newVolumeuL.ToString(CultureInfo.InvariantCulture));
        //}

        /// <summary>
        /// Sets the percent B concentration.
        /// </summary>
        /// <param name="percent">Percent B concentration to have.</param>
        [LCMethodEvent("Set Percent B", 1)]
        public void SetPercentB(double percent)
        {
            var reply = "";
            SendCommand("COMP " + Convert.ToInt32(percent).ToString() + ",-1,-1", out reply, "Attempting to set percent of solvent B");
        }

        /// <summary>
        /// Runs the method provided by a string.
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="method">Method to run stored on the pumps.</param>
        [LCMethodEvent("Start Method", MethodOperationTimeoutType.Parameter, "MethodNames", 1, true)]
        public void StartMethod(double timeout, string method)
        {
            var start = TimeKeeper.Instance.Now; // DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0));

            StartMethod(method);

            // We see if we ran over or not...if so then return failure, otherwise let it continue.
            var span = TimeKeeper.Instance.Now.Subtract(start);

            var timer = new TimerDevice();
            timer.WaitSeconds(span.TotalSeconds);
        }

        /// <summary>
        /// Runs the method provided by a string.
        /// </summary>
        /// <param name="method">Method to run stored on the pumps.</param>
        public void StartMethod(string method)
        {
            // Make sure we have record of the method
            if (availableMethods.ContainsKey(method) == false)
                throw new Exception(string.Format("The method {0} does not exist.", method));

            // Then send commands to start the method
            var methodData = availableMethods[method];
            var reply = "";

            // The reason why we delete the time table, is to clear out any
            // existing methods that might have been downloaded to the pump.

            // For example, if a time table was loaded and has entry:
            //     5 mins  - 30% B
            //     75 mins - 40%  B
            // And then we want to run this timetable:
            //     0 mins  - 0%   B
            //     15 mins - 50%  B
            //     60 mins - 100% B
            //     65 mins - 0%   B
            // when the time table is done the pumps may resort back to the final
            // entries and set the %B to 40 instead of the 0 we desire.
            SendCommand("AT:DEL ", out reply, "Clearing time table.");
            SendCommand(methodData, out reply, "Loading method.");
            SendCommand("STRT", out reply, "Attempting to start method.");
        }

        /// <summary>
        /// Stops the currently running method.
        /// </summary>
        [LCMethodEvent("Stop Method", 1)]
        public void StopMethod()
        {
            var reply = "";
            SendCommand("STOP", out reply, "Attempting to stop a method.");
        }

        /// <summary>
        /// Turns the pumps on.
        /// </summary>
        [LCMethodEvent("Turn Pump On", 1)]
        public void PumpOn()
        {
            var reply = "";
            SendCommand("PUMP 1", out reply, "Attempting to turn pumps on.");
            GetPumpState();
        }

        /// <summary>
        /// Turns the pumps off
        /// </summary>
        [LCMethodEvent("Turn Pump Off", 1)]
        public void PumpOff()
        {
            var reply = "";
            SendCommand("PUMP 0", out reply, "Attempting to turn pumps off.");
            GetPumpState();
        }

        /// <summary>
        /// Turns the pumps to standby
        /// </summary>
        [LCMethodEvent("Turn Pump Standby", 1)]
        public void PumpStandby()
        {
            var reply = "";
            SendCommand("PUMP 2", out reply, "Attempting to turn pumps to standby.");
            GetPumpState();
        }

        public PumpState GetPumpState()
        {
            if (Emulation)
            {
                return PumpState.Unknown;
            }

            try
            {
                //ApplicationLogger.LogMessage(2, $"{Name}: Sending 'PUMP?'");
                var success = SendCommand($"ACT:PUMP?", out var reply, $"Attempting to query pump state");
                //ApplicationLogger.LogMessage(2, $"{Name}: Got '{reply}'");

                //We expect something like:
                //reply = "RA 0000 ACT:PUMP 1";
                var start = reply.IndexOf($"ACT:PUMP", StringComparison.InvariantCultureIgnoreCase);
                if (success != AgilentPumpReplyErrorCodes.No_Error || start == -1)
                {
                    return PumpState.Unknown;
                }

                var split = reply.Substring(start).Split(new char[] {',', ' '}, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length < 2)
                {
                    return PumpState.Unknown;
                }

                var stateInt = Convert.ToInt32(split[1]);
                if (stateInt < 0 || stateInt > 2)
                {
                    return PumpState.Unknown;
                }

                PumpState = (PumpState) stateInt;
                return PumpState;
            }
            catch (Exception e)
            {
                ApplicationLogger.LogError(2, "Error getting pump state ", e);
            }

            PumpState = PumpState.Unknown;
            return PumpState.Unknown;
        }

        /// <summary>
        /// Gets the percent B.
        /// </summary>
        /// <returns></returns>
        public double GetPercentB()
        {
            if (Emulation)
            {
                return 0.0;
            }

            SendCommand("ACT:COMP?", out var reply, "Attempting to query composition of solvent B.");
            //expect: RA 0000 ACT:COMP 16.00,84.00,-1.0,-1.0

            if (reply.Contains("COMP"))
            {
                var replies = reply.Split(' ');
                if (replies.Length != 4)
                {
                    HandleError("Invalid pump response to composition request", CONST_COMPOSITION_B_SET);
                    return double.NaN;
                }
                var percents = replies[3].Split(',');
                return Convert.ToDouble(percents[1]);
            }

            reply = "";
            SendCommand("ACT:COMP?", out reply, "Attempting to query composition of solvent B.");
            if (reply.Contains("COMP"))
            {
                var replies = reply.Split(' ');
                if (replies.Length != 4)
                {
                    HandleError("Invalid pump response to composition request", CONST_COMPOSITION_B_SET);
                    return double.NaN;
                }
                var percents = replies[3].Split(',');
                return Convert.ToDouble(percents[1]);
            }

            HandleError("Invalid pump response to composition request", CONST_COMPOSITION_B_SET);
            return double.NaN;
        }

        /// <summary>
        /// Gets the current loaded method in the pump module.
        /// </summary>
        /// <returns>Method string kept on the pump.</returns>
        public string RetrieveMethod()
        {
            var methodString = "";
            if (Emulation)
            {
                methodString = "test;\ntest12";
            }
            else
            {
                SendCommand("LIST \"TT\"", out methodString, "Attempting to retrieve method", listDataChannel);
            }
            return methodString;
        }

        /// <summary>
        /// Gets the flow rate. Note that this is the ideal flow rate not the actual flow rate.
        /// </summary>
        /// <returns>The flow rate</returns>
        public double GetFlowRate()
        {
            if (Emulation)
            {
                return 0.0;
            }

            SendCommand("FLOW?", out var reply, "Attempting to query flow rate.");
            //We expect something like:
            //reply = "RA 0000 FLOW 2.000";
            var start = reply.IndexOf("FLOW", StringComparison.InvariantCultureIgnoreCase);
            if (start == -1)
            {
                SendCommand("FLOW?", out reply, "Attempting to re-query flow rate.");
                start = reply.IndexOf("FLOW", StringComparison.InvariantCultureIgnoreCase);
                if (start == -1)
                {
                    HandleError("Invalid pump response to flow rate request", CONST_FLOW_SET);
                    return double.NaN;
                }
            }
            reply = reply.Substring(start + 5, 5);
            return Convert.ToDouble(reply);
        }

        /// <summary>
        /// Sets the purge parameters and state for the
        /// </summary>
        /// <param name="purgeData"></param>
        /// <returns></returns>
        public bool SetPurgeData(PumpPurgeData purgeData)
        {
            if (Emulation)
            {
                return false;
            }

            if (purgeData.FlowRate < 0 || purgeData.Duration < 0)
            {
                return false;
            }

            var reply = "";
            var purgeChannelText = purgeData.Channel.ToString();
            //ApplicationLogger.LogMessage(2, $"{Name}: Sending 'PG{purgeChannelText} {purgeData.FlowRate}, {purgeData.Duration}'");
            var success = SendCommand($"PG{purgeChannelText} {purgeData.FlowRate}, {purgeData.Duration}", out reply, $"Attempting to set purge settings for channel {purgeChannelText}");

            if (success != AgilentPumpReplyErrorCodes.No_Error)
            {
                return false;
            }

            var enabledChannels = GetPurgeState();
            if (purgeData.Enabled)
            {
                enabledChannels |= (int)purgeData.Channel;
            }
            else
            {
                enabledChannels &= ~((int) purgeData.Channel); // Reverse the bits, so that everything but this bit could possibly be true.
            }

            //ApplicationLogger.LogMessage(2, $"{Name}: Sending 'PRGE {enabledChannels}, 1, 1'");
            success = SendCommand($"PRGE {enabledChannels}, 1, 1", out reply, $"Attempting to set purge state for channel {purgeChannelText}");

            if (success != AgilentPumpReplyErrorCodes.No_Error)
            {
                return false;
            }

            return true;
        }

        public bool GetPurgeData(PumpPurgeData purgeData)
        {
            if (Emulation)
            {
                return false;
            }

            var purgeChannelText = purgeData.Channel.ToString();
            //ApplicationLogger.LogMessage(2, $"{Name}: Sending 'PG{purgeChannelText}?'");
            var success = SendCommand($"PG{purgeChannelText}?", out var reply, $"Attempting to query purge settings for channel {purgeChannelText}");
            //ApplicationLogger.LogMessage(2, $"{Name}: Got '{reply}'");

            //We expect something like:
            //reply = "RA 0000 PGA1 1000, 5";
            var start = reply.IndexOf($"PG{purgeChannelText}", StringComparison.InvariantCultureIgnoreCase);
            if (success != AgilentPumpReplyErrorCodes.No_Error || start == -1)
            {
                purgeData.FlowRate = -1;
                purgeData.Duration = -1;
                return false;
            }

            var split = reply.Substring(start).Split(new char[] {',', ' '}, StringSplitOptions.RemoveEmptyEntries);
            purgeData.FlowRate = Convert.ToDouble(split[1]);
            purgeData.Duration = Convert.ToDouble(split[2]);

            var purgeStates = GetPurgeState();
            //ApplicationLogger.LogMessage(2, $"{Name}: Got PurgeState: '{purgeStates}', setting {purgeData.Channel.ToString()} ({(int)(purgeData.Channel)}) enabled: '{(purgeStates & (int)purgeData.Channel) == (int)purgeData.Channel}'");
            purgeData.Enabled = (purgeStates & (int) purgeData.Channel) == (int)purgeData.Channel;

            return true;
        }

        private int GetPurgeState()
        {
            if (Emulation)
            {
                return 0;
            }

            //ApplicationLogger.LogMessage(2, $"{Name}: Sending 'PRGE?'");
            var success = SendCommand($"PRGE?", out var reply, $"Attempting to query channel purge states");
            //ApplicationLogger.LogMessage(2, $"{Name}: Got '{reply}'");

            //We expect something like:
            //reply = "RA 0000 PRGE 1, x, y";
            var start = reply.IndexOf($"PRGE", StringComparison.InvariantCultureIgnoreCase);
            if (success != AgilentPumpReplyErrorCodes.No_Error || start == -1)
            {
                return 0;
            }

            var split = reply.Substring(start).Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return Convert.ToInt32(split[1]);
        }

        /// <summary>
        /// Gets the pressure (in bar)
        /// </summary>
        /// <returns>The pressure</returns>
        public double GetPressure()
        {
            if (Emulation)
            {
                return 0.0;
            }

            SendCommand("ACT:PRES?", out var reply, "Attempting to query pressure.");
            //expect: RA 0000 ACT:PRES 16.00

            if (reply.Contains("ACT:PRES"))
            {
                var replies = reply.Split(' ');
                if (replies.Length != 4)
                {
                    HandleError("Invalid pump response to pressure request", CONST_PRESSURE_SET);
                    return double.NaN;
                }
                return Convert.ToDouble(replies[3]);
            }

            reply = "";
            SendCommand("ACT:PRES?", out reply, "Attempting to query pressure.");
            //expect: RA 0000 ACT:PRES 16.00

            if (reply.Contains("ACT:PRES"))
            {
                var replies = reply.Split(' ');
                if (replies.Length != 4)
                {
                    HandleError("Invalid pump response to pressure request", CONST_PRESSURE_SET);
                    return double.NaN;
                }
                return Convert.ToDouble(replies[3]);
            }

            HandleError("Invalid pump response to pressure request", CONST_PRESSURE_SET);
            return double.NaN;
        }

        ///// <summary>
        ///// Gets the current mixer volume.
        ///// </summary>
        ///// <returns>The current mixer volume</returns>
        //public double GetMixerVolume()
        //{
        //    if (Emulation)
        //    {
        //        return 0.0;
        //    }
        //    var reply = "";
        //    SendCommand("MVOL?", out reply, "Attempting to query mixer volume.");
        //    var start = reply.IndexOf("MVOL", StringComparison.InvariantCultureIgnoreCase);
        //    if (start == -1)
        //    {
        //        reply = "";
        //        SendCommand("MVOL?", out reply, "Attempting to query mixer volume.");
        //        start = reply.IndexOf("MVOL", StringComparison.InvariantCultureIgnoreCase);
        //        if (start == -1)
        //        {
        //            HandleError("Invalid pump response to volume request", CONST_VOLUME_SET);
        //            return double.NaN;
        //        }
        //    }
        //    reply = reply.Substring(start + 5, reply.Length - (start + 5));
        //    return Convert.ToDouble(reply);
        //}

        /// <summary>
        /// Gets the actual flow rate
        /// </summary>
        /// <returns>The actual measured current flow rate</returns>
        public double GetActualFlow()
        {
            if (Emulation)
            {
                return 0.0;
            }

            SendCommand("ACT:FLOW?", out var reply, "Attempting to query actual flow.");
            var start = reply.IndexOf("ACT:FLOW", StringComparison.InvariantCultureIgnoreCase);
            if (start == -1)
            {
                reply = "";
                SendCommand("ACT:FLOW?", out reply, "Attempting to query actual flow.");
                start = reply.IndexOf("ACT:FLOW", StringComparison.InvariantCultureIgnoreCase);
                if (start == -1)
                {
                    HandleError("Invalid pump response to flow request", CONST_FLOW_SET);
                    return double.NaN;
                }
            }
            reply = reply.Substring(start + 9, 5);
            return Convert.ToDouble(reply);
        }

        /// <summary>
        /// Gets the current pump mode
        /// </summary>
        /// <returns>The current pump mode</returns>
        public AgilentPumpModes GetMode()
        {
            if (Emulation)
            {
                return AgilentPumpModes.Unknown;
            }

            SendCommand("MODE?", out var reply, "Attempting to query mode.");
            //reply = "RA 000 MODE 1
            var start = reply.IndexOf("MODE", StringComparison.InvariantCultureIgnoreCase);
            if (start == -1)
            {
                //TODO: Error!
                return AgilentPumpModes.Unknown;
            }
            reply = reply.Substring(start + 5, 1);
            return (AgilentPumpModes)(Convert.ToInt32(reply));
        }

        public void Identify()
        {
            SendCommand("IDN", out var reply);
        }

        /// <summary>
        /// Load pump information into PumpInfo
        /// </summary>
        public void GetPumpInformation()
        {
            try
            {
                // Get the firmware revision of the running system
                // "IDN? gets "<manufacturer>,<model>,<serialNumber>,<running firmware revision>"; "IDN" causes "Identify by frontend LED"
                var gotIdent = SendCommand("IDN?", out var reply);
                //ApplicationLogger.LogMessage(2, $"Pump {Name}: Got reply \"{reply}\"");
                if (gotIdent == AgilentPumpReplyErrorCodes.No_Error)
                {
                    var data = reply.Substring(12);
                    var split = data.Replace("\"", "").Split(',');
                    PumpInfo.Manufacturer = split[0];
                    PumpInfo.Model = split[1];
                    PumpInfo.SerialNumber = split[2];
                }

                // Also can use "REV? 0|1|2" for main|resident|boot firmware revisions
                SendCommand("REV? 0", out reply);
                PumpInfo.MainFirmware = reply.Split(',')[1].Trim('"');
                //ApplicationLogger.LogMessage(2, $"Pump {Name}: Got reply \"{reply}\"");
                SendCommand("REV? 1", out reply);
                PumpInfo.ResidentFirmware = reply.Split(',')[1].Trim('"');
                //ApplicationLogger.LogMessage(2, $"Pump {Name}: Got reply \"{reply}\"");
                SendCommand("REV? 2", out reply);
                PumpInfo.BootFirmware = reply.Split(',')[1].Trim('"');
                //ApplicationLogger.LogMessage(2, $"Pump {Name}: Got reply \"{reply}\"");

                // Can also use "BLDN?" to get the firmware build number
                SendCommand("BLDN?", out reply);
                PumpInfo.FirmwareBuildNumber = reply.Split(' ').Last().Trim('"');
                //ApplicationLogger.LogMessage(2, $"Pump {Name}: Got reply \"{reply}\"");

                // "SER?" gets the serial number
                //SendCommand("SER?", out reply, errorMessage);
                //ApplicationLogger.LogMessage(2, $"Pump {Name}: Got reply \"{reply}\"");

                // "TYPE?" gets the instrument type/model number
                //SendCommand("TYPE?", out reply, errorMessage);
                //ApplicationLogger.LogMessage(2, $"Pump {Name}: Got reply \"{reply}\"");

                // "MFGD?" gets the instrument manufacture date as a unix timestamp
                SendCommand("MFGD?", out reply);
                PumpInfo.ManufactureDateUtc = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reply.Split(' ').Last())).DateTime;
                //ApplicationLogger.LogMessage(2, $"Pump {Name}: Got reply \"{reply}\"");

                // "OPT?" gets the option listing
                SendCommand("OPT?", out reply);
                PumpInfo.Options = reply.Split('"')[1];
                //ApplicationLogger.LogMessage(2, $"Pump {Name}: Got reply \"{reply}\"");

                // "DATE?" gets the date in yyyy,MM,dd,hh,mm,ss; can set with "DATE yyy,MM,dd,hh,mm,ss"
                //SendCommand("DATE?", out reply, errorMessage);
                //ApplicationLogger.LogMessage(2, $"Pump {Name}: Got reply \"{reply}\"");

                // "TIME?" gets the time as a unix timestamp; can set with "TIME secsSince1970"
                SendCommand("TIME?", out reply);
                PumpInfo.ModuleDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reply.Split(' ').Last())).DateTime.ToLocalTime();
                //ApplicationLogger.LogMessage(2, $"Pump {Name}: Got reply \"{reply}\"");

                // "NAME?" gets the symbolic module name; "NAME MyName" sets it
                SendCommand("NAME?", out reply);
                PumpInfo.ModuleName = reply.Split('"')[1];
                //ApplicationLogger.LogMessage(2, $"Pump {Name}: Got reply \"{reply}\"");
            }
            catch (Exception e)
            {
                ApplicationLogger.LogError(0, "Error getting Agilent Pump Information", e);
            }
        }

        public void GetPumpStatus()
        {
            try
            {
                //// STAT? and ACT:STAT? get module states, STAT? gets the states in ASCII format, ACT:STAT? in decimal format
                //SendCommand("STAT?", out reply, errorMessage);
                //split = reply.Substring(14).Split(new char[] {'"', ',', ' '}, StringSplitOptions.RemoveEmptyEntries);
                //PumpStatus.GenericState = split[0];
                //PumpStatus.AnalysisState = split[1];
                //PumpStatus.ErrorState = split[2];
                //PumpStatus.NotReadyState = split[3];
                //PumpStatus.TestState = split[4];
                ////ApplicationLogger.LogMessage(2, $"Pump {Name}: Got reply \"{reply}\"");

                // STAT? and ACT:STAT? get module states, STAT? gets the states in ASCII format, ACT:STAT? in decimal format
                SendCommand("ACT:STAT?", out var reply);

                var loc = reply.IndexOf("ACT:STAT", StringComparison.OrdinalIgnoreCase);
                var split = reply.Substring(loc + 9).Split(new char[] { '"', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                PumpStatus.GenericState = (AgilentPumpStateGeneric)int.Parse(split[0]);
                PumpStatus.AnalysisState = (AgilentPumpStateAnalysis)int.Parse(split[1]);
                PumpStatus.ErrorState = (AgilentPumpStateError)int.Parse(split[2]);
                PumpStatus.NotReadyState = (AgilentPumpStateNotReady)int.Parse(split[3]);
                PumpStatus.TestState = (AgilentPumpStateTest)int.Parse(split[4]);

                // ACT:BTMP? gets board temperature and leak status: ACT:BTMP? <boardtempC>,<leakSensorCurrent>,<leakState>
                SendCommand("ACT:BTMP?", out reply);
                split = reply.Substring(17).Split(',');
                PumpStatus.BoardTemperatureC = split[0];
                PumpStatus.LeakSensorCurrentMa = split[1];
                //PumpStatus.LeakState = split[2];
                switch (split[2])
                {
                    case "0":
                        PumpStatus.LeakState = "No Leak";
                        break;
                    case "1":
                        PumpStatus.LeakState = "Leak Detected";
                        break;
                    case "2":
                        PumpStatus.LeakState = "NTC Board Sensor shorted";
                        break;
                    case "3":
                        PumpStatus.LeakState = "NTC Board Sensor open";
                        break;
                    case "4":
                        PumpStatus.LeakState = "PTC Leak Sensor shorted";
                        break;
                    case "5":
                        PumpStatus.LeakState = "PTC Leak Sensor open";
                        break;
                }
                //ApplicationLogger.LogMessage(2, $"Pump {Name}: Got reply \"{reply}\"");

                // ACT:NRDY? gets the not ready status code, corresponding to bit flags
                SendCommand("ACT:NRDY?", out reply);
                PumpStatus.NotReadyReasons = (AgilentPumpNotReadyStates) int.Parse(reply.Split(' ').Last());

                // ACT:SRDY? gets the start not ready status code, corresponding to bit flags
                SendCommand("ACT:SRDY?", out reply);
                PumpStatus.StartNotReadyReasons = (AgilentPumpStartNotReadyStates)int.Parse(reply.Split(' ').Last());
                statusReadErrorRepeatCount = 0;
            }
            catch (Exception e)
            {
                if (statusReadErrorRepeatCount < 10)
                {
                    ApplicationLogger.LogError(0, "Error getting Agilent Pump Status", e);
                    statusReadErrorRepeatCount++;
                }
            }
        }

        public void SetModuleDateTime()
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var reply = "";
            SendCommand($"TIME {timestamp}", out reply);
            //ApplicationLogger.LogMessage(2, $"Pump {Name}: Got reply \"{reply}\"");
            GetPumpInformation();
        }

        public void SetModuleName(string newName)
        {
            if (newName == null)
            {
                newName = "";
            }

            if (newName.Length > 30)
            {
                newName = newName.Substring(0, 30);
            }

            var reply = "";
            SendCommand($"NAME \"{newName}\"", out reply);
            //ApplicationLogger.LogMessage(2, $"Pump {Name}: Got reply \"{reply}\"");
            GetPumpInformation();
        }

        /// <summary>
        /// Indicates that a save is required in the Fluidics Designer
        /// </summary>
        protected virtual void OnDeviceSaveRequired()
        {
            DeviceSaveRequired?.Invoke(this, null);
        }

        /// <summary>
        /// Registers the method with a data provider.
        /// </summary>
        /// <param name="key">Data provider name.</param>
        /// <param name="remoteMethod">Method to invoke when data provider has new data.</param>
        public void RegisterDataProvider(string key, DelegateDeviceHasData remoteMethod)
        {
            switch (key.ToUpper())
            {
                case "METHODNAMES":
                    MethodNames += remoteMethod;
                    ListMethods();
                    break;
            }
        }

        /// <summary>
        /// Unregisters the method from the data provider.
        /// </summary>
        /// <param name="key">Data provider name.</param>
        /// <param name="remoteMethod">Method to invoke when data provider has new data.</param>
        public void UnRegisterDataProvider(string key, DelegateDeviceHasData remoteMethod)
        {
            switch (key.ToUpper())
            {
                case "METHODNAMES":
                    MethodNames -= remoteMethod;
                    break;
            }
        }

        /// <summary>
        /// Writes the pump method time-table to the directory provided.
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="methodName"></param>
        private void WriteMethod(string directoryPath, string methodName)
        {
            if (availableMethods.ContainsKey(methodName))
            {
                var methodData = availableMethods[methodName];
            }
        }

        /// <summary>
        /// Writes the required data to the directory path provided.
        /// </summary>
        /// <param name="directoryPath">Path of directory to create files in.</param>
        /// <param name="methodName">Name of the method the user is requesting performance data about.</param>
        /// <param name="parameters">Parameters used to create the performance data.</param>
        public void WritePerformanceData(string directoryPath, string methodName, object[] parameters)
        {
            switch (methodName)
            {
                case "Start Method":
                    if (parameters != null && parameters.Length > 1)
                    {
                        WriteMethod(directoryPath, parameters[1].ToString());
                    }
                    break;
            }
        }

        public List<string> GetStatusNotificationList()
        {
            var notifications = new List<string>() { "Status"
                                                            };

            notifications.AddRange(notificationStrings);

            foreach (var value in statusCodes.Values)
            {
                notifications.Add(value);
            }
            return notifications;
        }

        public List<string> GetErrorNotificationList()
        {
            var notifications = new List<string>() {
                                        CONST_COMPOSITION_B_SET,
                                        CONST_INITIALIZE_ERROR,
                                        CONST_PRESSURE_SET,
                                        CONST_FLOW_SET,
                                        CONST_VOLUME_SET,
                                        CONST_ERROR_ABOVE_PRESSURE,
                                        CONST_ERROR_BELOW_PRESSURE,
                                        CONST_ERROR_FLOW_EXCEEDS,
                                        CONST_ERROR_FLOW_UNSTABLE,
                                        CONST_PUMP_ERROR,
                                        "Pump Event", // Seeing this, it doesn't make much sense, but I need to at least silence the "unpublished error" warnings
            };

            foreach (var value in errorCodes.Values)
            {
                notifications.Add(value);
            }

            return notifications;
        }

        /// <summary>
        /// Returns the name of the device.
        /// </summary>
        /// <returns>String name of the device.</returns>
        public override string ToString()
        {
            return Name;
        }

        public List<MobilePhase> MobilePhases { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
