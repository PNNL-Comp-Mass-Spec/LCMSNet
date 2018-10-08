//*********************************************************************************************************
// Written by John Ryan, Dave Clark, Brian LaMarche for the US Department of Energy
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 08/17/2009
//
// Updates:
// - 9/8/2009 (BLL) Added unique naming to the constructor so it will display on the fluidics designer
//
//*********************************************************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading;
using Agilent.Licop;
using FluidicsSDK.Devices;
using LcmsNetData;
using LcmsNetData.Logging;
using LcmsNetData.System;
using LcmsNetSDK.Data;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Method;

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
        #region Members

        /// <summary>
        /// An 'instrument' object for the Agilent pump drivers
        /// </summary>
        private Instrument m_pumps = null;

        /// <summary>
        /// A 'module' object for the Agilent pump drivers
        /// </summary>
        private Module m_module;

        /// <summary>
        /// Error reporting channel.
        /// </summary>
        private Channel m_evChannel;

        /// <summary>
        /// Channel for retrieving methods from the pumps.
        /// </summary>
        private Channel m_listChannel;

        /// <summary>
        /// A 'channel' object for the Agilent pump drivers
        /// </summary>
        private Channel m_inChannel;

        /// <summary>
        /// Channel when monitoring the instrument.
        /// </summary>
        private Channel m_monitorChannel;

        /// <summary>
        /// The device's name.
        /// </summary>
        private string m_name;

        /// <summary>
        /// The flow rate (used for save/load)
        /// </summary>
        private double m_flowrate = -1.0;

        /// <summary>
        /// Filesystem watcher for real-time updating of pump methods.
        /// </summary>
        private static FileSystemWatcher mwatcher_methods;

        /// <summary>
        /// Dictionary that holds a method name, key, and the method time table, value.
        /// </summary>
        private static Dictionary<string, string> mdict_methods;

        /// <summary>
        /// Status strings from the pumps.
        /// </summary>
        private static string[] m_notificationStrings;

        private static Dictionary<string, string> m_errorCodes;
        private static Dictionary<string, string> m_statusCodes;

        private PumpState pumpState;
        private string pumpModel;
        private string pumpSerial;
        private string pumpFirmware;
        private readonly Timer statusReadTimer = null;
        private readonly AgilentPumpStatus pumpStatusInternal = new AgilentPumpStatus();

        #endregion

        #region Constants

        #region COMConstants

        /// <summary>
        /// Status of the device.
        /// </summary>
        private DeviceStatus m_status;

        private const string CONST_DEFAULTPORT = "COM1";
        private const int CONST_DEFAULTTIMEOUT = 6000; //milliseconds
        private const int CONST_WRITETIMEOUT = 10000; //milliseconds
        private const int CONST_READTIMEOUT = 10000; //milliseconds
        private const int CONST_MONITORING_MINUTES = 10;
        private const int CONST_MONITORING_SECONDS_ELAPSED = 10;

        #endregion

        #region Status Constants

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

        #endregion

        #region Error Constants

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

        #endregion

        #endregion

        #region Events

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
        /// List of times monitoring data was received.
        /// </summary>
        public List<DateTime> m_times;

        /// <summary>
        /// List of pressures used throughout the run.
        /// </summary>
        public List<double> m_pressures;

        /// <summary>
        /// List of flowrates used throughout the run.
        /// </summary>
        public List<double> m_flowrates;

        /// <summary>
        /// List of %B compositions throughout the run.
        /// </summary>
        public List<double> m_percentB;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public AgilentPump()
        {
            CreateErrorCodes();
            CreateStatusCodes();

            PortName = CONST_DEFAULTPORT;
            if (mdict_methods == null)
            {
                mdict_methods = new Dictionary<string, string>();
            }
            if (mwatcher_methods == null)
            {
                var path = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                path = Path.Combine(Path.GetDirectoryName(path), "..\\pumpmethods");
                path = path.Substring(path.IndexOf(":", StringComparison.Ordinal) + 2); // gets rid of the file:/ tag.
                //ApplicationLogger.LogMessage(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "PATH: " + path);
                mwatcher_methods = new FileSystemWatcher(path, "*.txt");
                mwatcher_methods.Created += mwatcher_methods_Created;
                mwatcher_methods.Changed += mwatcher_methods_Changed;
                mwatcher_methods.EnableRaisingEvents = true;
            }
            m_name = "pump";

            m_flowrates = new List<double>();
            m_percentB = new List<double>();
            m_pressures = new List<double>();
            m_times = new List<DateTime>();
            m_status = DeviceStatus.NotInitialized;

            TotalMonitoringMinutesDataToKeep = CONST_MONITORING_MINUTES;
            TotalMonitoringSecondsElapsed = CONST_MONITORING_SECONDS_ELAPSED;

            if (m_notificationStrings == null)
            {
                m_notificationStrings = new[]{
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

        private void UpdateStatus(object state)
        {
            if (m_pumps != null)
            {
                GetPumpStatus(pumpStatusInternal);
                ReactiveUI.RxApp.MainThreadScheduler.Schedule(() => PumpStatus.UpdateValues(pumpStatusInternal));
                GetPumpState();
            }
        }

        /// <summary>
        /// Calling this constructor is only for the windows WPF designer.
        /// </summary>
        [Obsolete("For WPF Design time use only.", true)]
        public AgilentPump(bool emulated)
        {
            Emulation = true;

            PurgeA1 = new PumpPurgeData(PumpPurgeChannel.A1);
            PurgeA2 = new PumpPurgeData(PumpPurgeChannel.A2);
            PurgeB1 = new PumpPurgeData(PumpPurgeChannel.B1);
            PurgeB2 = new PumpPurgeData(PumpPurgeChannel.B2);
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

        #endregion

        #region Properties

        [PersistenceData("TotalMonitoringMinutes")]
        public int TotalMonitoringMinutesDataToKeep { get; set; }

        [PersistenceData("TotalMonitoringSecondsElapsed")]
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
            get => m_status;
            set
            {
                if (value != m_status)
                    StatusUpdate?.Invoke(this, new DeviceStatusEventArgs(value, "Status", this));
                m_status = value;
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
            get => m_name;
            set
            {
                if (this.RaiseAndSetIfChangedRetBool(ref m_name, value))
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
        [PersistenceData("PortName")]
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

        #endregion

        #region Pump Purging

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
        [LCMethodEvent("Purge Channel", MethodOperationTimeoutType.Parameter, "", -1, false)]
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
        [LCMethodEvent("Abort Purge", MethodOperationTimeoutType.Parameter, "", -1, false)]
        public AgilentPumpReplyErrorCodes AbortPurges(double timeout)
        {
            var reply = "";
            var command = "PURG 0";
            var worked = SendCommand(command, out reply);
            return worked;
        }

        #endregion

        #region Pump Status Code Converters

        private void CreateErrorCodes()
        {
            if (m_errorCodes == null)
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

                m_errorCodes = map;
            }
        }

        private void CreateStatusCodes()
        {
            if (m_statusCodes == null)
            {
                var map = new Dictionary<string, string>
                {
                    {"EV 2227", "no start allowed while purging."},
                    {"EV 2228", "no start allowed while fast composition change."},
                    {"EV 2229", "Column flow in limit."},
                    {"EV 2230", "Column flow out of limit."},
                    {"EV 2231", "Fast composition change started."},
                    {"EV 2232", "Fast composition change complete."},
                    {"EV 2233", "Calibration table stored. Parameter: table reference, 1..10"},
                    {"EV 2234", "Calibration table deleted. Parameter: table reference, 1..10"},
                    {"EV 2235", "Fast Composition Change failed."},
                    {"EE 2066", "Column flow unstable."},
                    {"EE 2067", "No EMPV connected."},
                    {"EE 2068", "No flow sensor connected."},
                    {"EE 2069", "EMPV initialization failed."},
                    {"EE 2090", "Flow sensor not supported by pump."},
                    {"ES 2116", "Pump in normal mode.            "},
                    {"ES 2117", "Pump in micro mode.             "},
                    {"ES 2118", "Pump in test mode.              "},
                    {"ES 2119", "Purge valve on.                 "},
                    {"ES 2120", "Purge valve off.                "},
                    {"ES 2121", "Column flow in sensor range.    "},
                    {"ES 2122", "Column flow out of sensor range."}
                };

                m_statusCodes = map;
            }
        }

        #endregion

        #region Pump Event Handlers

        /// <summary>
        /// Handles events when a pump error occurs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_pumps_ErrorOccurred(object sender, global::Agilent.Licop.ErrorEventArgs e)
        {
            if (e.Message == null)
                return;

            var errorMessage = e.Message.Split(',');
            if (m_errorCodes.ContainsKey(errorMessage[0]))
            {
                var displayedError = m_errorCodes[errorMessage[0]];
                HandleError(displayedError, displayedError);
            }
            else
            {
                HandleError("Pump " + Name + " failed " + e.Message, CONST_PUMP_ERROR);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Clears all of the listed pump methods.
        /// </summary>
        public void ClearMethods()
        {
            mdict_methods.Clear();
        }

        public void AddMethods(Dictionary<string, string> methods)
        {
            mdict_methods = methods;
            ListMethods();
        }

        /// <summary>
        /// Adds a given method.
        /// </summary>
        /// <param name="methodname">Name of method to track.</param>
        /// <param name="method">Method data to store.</param>
        public void AddMethod(string methodname, string method)
        {
            if (mdict_methods.ContainsKey(methodname))
            {
                mdict_methods[methodname] = method;

                // Don't need to fire ListMethods() - the name didn't change.
            }
            else
            {
                mdict_methods.Add(methodname, method);

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
                var keys = new string[mdict_methods.Keys.Count];
                mdict_methods.Keys.CopyTo(keys, 0);

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

            m_pumps = new Instrument(CONST_DEFAULTTIMEOUT, CONST_DEFAULTTIMEOUT);
            m_pumps.ErrorOccurred += m_pumps_ErrorOccurred;

            //
            // Try initial connection
            //
            if (m_pumps.TryConnect(PortName, CONST_DEFAULTTIMEOUT) == false)
            {
                errorMessage = "Could not connect to the Agilent Pumps.";
                HandleError(errorMessage, CONST_INITIALIZE_ERROR);
                //Could not connect
                return false;
            }

            m_module = m_pumps.CreateModule(m_pumps.GetAccessPointIdentifier());
            ReactiveUI.RxApp.MainThreadScheduler.Schedule(() =>
            {
                PumpModel = m_module.Type;
                PumpSerial = m_module.Serial;
            });

            //
            // Channel for inputs
            //
            m_inChannel = m_module.CreateChannel("IN");
            if (m_inChannel.TryOpen(ReadMode.Polling) == false)
            {
                //"Could not open IN channel."
                errorMessage = "Could not open the communication channel for input";
                HandleError(errorMessage, CONST_INITIALIZE_ERROR);
                return false;
            }

            var reply = "";
            // Get the firmware revision of the running system
            // "IDN? gets "<manufacturer>,<model>,<serialNumber>,<running firmware revision>"; "IDN" causes "Identify by frontend LED"
            var gotIdent = SendCommand("IDN?", out reply, "");
            var firmware = "";
            //ApplicationLogger.LogMessage(2, $"Pump {Name}: Got reply \"{reply}\"");
            if (gotIdent == AgilentPumpReplyErrorCodes.No_Error)
            {
                var split = reply.Replace("\"", "").Split(',');
                firmware = split[3];
            }

            ReactiveUI.RxApp.MainThreadScheduler.Schedule(() =>
            {
                PumpFirmware = firmware;
                GetPumpInformation();
                GetPumpStatus();
            });

            //
            // Open a list channel to read time tables.
            //
            m_listChannel = m_module.CreateChannel("LI");
            if (m_listChannel.TryOpen(ReadMode.Polling) == false)
            {
                errorMessage = "Could not open the communication channel for time table data.";
                HandleError(errorMessage, CONST_INITIALIZE_ERROR);
                return false;
            }

            //
            // And an EV channel.Channel for errors or state events
            //
            m_evChannel = m_module.CreateChannel("EV");
            m_evChannel.DataReceived += m_evChannel_DataReceived;
            if (m_evChannel.TryOpen(ReadMode.Events) == false)
            {
                //"Could not open EV channel."
                errorMessage = "Could not open the communication channel for error events";
                HandleError(errorMessage, CONST_INITIALIZE_ERROR);
                return false;
            }

            //
            // Monitoring for the pumps
            //
            m_monitorChannel = m_module.CreateChannel("MO");
            m_monitorChannel.DataReceived += m_monitorChannel_DataReceived;
            if (m_monitorChannel.TryOpen(ReadMode.Events) == false)
            {
                errorMessage = "Could not open the communication channel for monitoring data";
                HandleError(errorMessage, CONST_INITIALIZE_ERROR);
                return false;
            }

            var worked = SendCommand(
                string.Format("MONI:STRT {0},\"ACT:FLOW?; ACT:PRES?; ACT:COMP?\"", TotalMonitoringSecondsElapsed),
                out reply);

            if (worked != AgilentPumpReplyErrorCodes.No_Error)
            {
                errorMessage = "Could not put the pumps in monitoring mode.";
                HandleError(errorMessage, CONST_INITIALIZE_ERROR);
                return false;
            }

            GetPumpState();
            statusReadTimer.Change(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));

            return true;
        }

        /// <summary>
        /// Internal error handler that propogates the error message to listening objects.
        /// </summary>
        private void HandleError(string message, string type)
        {
            HandleError(message, type, null);
        }

        /// <summary>
        /// Internal error handler that propogates the error message to listening objects.
        /// </summary>
        private void HandleError(string message, string type, Exception ex)
        {
            if (Error != null)
            {
                if (type == "")
                {
                    type = CONST_DEFAULT_ERROR;
                }
                Error(this,
                      new DeviceErrorEventArgs(message,
                                                    ex,
                                                    DeviceErrorStatus.ErrorAffectsAllColumns,
                                                    this,
                                                    type));
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

        #region Pump Events

        /// <summary>
        /// Handles errors from the pump.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_evChannel_DataReceived(object sender, DataEventArgs e)
        {
            var data = e.AsciiData;

            if (data.Contains("EE 2014"))
            {
                Error?.Invoke(this, new DeviceErrorEventArgs(CONST_ERROR_ABOVE_PRESSURE, null, DeviceErrorStatus.ErrorAffectsAllColumns, this, CONST_ERROR_ABOVE_PRESSURE));
            }
            else if (data.Contains("EE 2015"))
            {
                Error?.Invoke(this, new DeviceErrorEventArgs(CONST_ERROR_BELOW_PRESSURE, null, DeviceErrorStatus.ErrorAffectsAllColumns, this, CONST_ERROR_BELOW_PRESSURE));
            }
            else if (data.Contains("EE 2064"))
            {
                Error?.Invoke(this, new DeviceErrorEventArgs(CONST_ERROR_FLOW_EXCEEDS, null, DeviceErrorStatus.ErrorAffectsAllColumns, this, CONST_ERROR_FLOW_EXCEEDS));
            }
            else if (data.Contains("EE 2066"))
            {
                Error?.Invoke(this, new DeviceErrorEventArgs(CONST_ERROR_FLOW_UNSTABLE, null, DeviceErrorStatus.ErrorAffectsAllColumns, this, CONST_ERROR_FLOW_UNSTABLE));
            }
            else
            {
                var errorMessage = data.Split(',');
                if (m_statusCodes.ContainsKey(errorMessage[0]))
                {
                    var displayedError = m_statusCodes[errorMessage[0]];
                    HandleError(displayedError, displayedError);
                }
                else
                {
                    HandleError("Pump " + Name + " status: " + data, CONST_PUMP_ERROR);
                }
            }
            //LcmsNetDataClasses.Logging.ApplicationLogger.LogMessage(2, Name + " Agilent Pump Message " + data);
        }

        /// <summary>
        /// Handles monitoring data from the pumps.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_monitorChannel_DataReceived(object sender, DataEventArgs e)
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
                //
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
            if (Math.Abs(m_flowrate) > float.Epsilon)
            {
                var percentFlowChange = (m_flowrate - flowrate) / m_flowrate;
                UpdateNotificationStatus(percentFlowChange.ToString("0.00"), CONST_FLOW_CHANGE);
            }
            UpdateNotificationStatus(pressure.ToString("0.000"), CONST_PRESSURE_VALUE);

            // Update log collections.
            m_times.Add(time);
            m_pressures.Add(pressure);
            m_flowrates.Add(flowrate);
            m_percentB.Add(compositionB);

            //
            // Find old data to remove -- needs to be updated (or could be) using LINQ
            //
            var count = m_times.Count;
            var total = (TotalMonitoringMinutesDataToKeep * 60) / TotalMonitoringSecondsElapsed;
            if (count >= total)
            {
                var i = 0;
                while (time.Subtract(m_times[i]).TotalMinutes > TotalMonitoringMinutesDataToKeep && i < m_times.Count)
                {
                    i++;
                }

                if (i > 0)
                {
                    i = Math.Min(i, m_times.Count - 1);
                    m_times.RemoveRange(0, i);
                    m_flowrates.RemoveRange(0, i);
                    m_pressures.RemoveRange(0, i);
                    m_percentB.RemoveRange(0, i);
                }
            }

            // Alert the user data is ready
            try
            {
                MonitoringDataReceived?.Invoke(this, new PumpDataEventArgs(this, m_times, m_pressures, m_flowrates, m_percentB));
            }
            catch
            {
                // ignored
            }
        }

        #endregion

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

            if (m_evChannel == null)
                return true;

            // close EV channel
            m_evChannel.Close();

            // close IN channel
            m_inChannel.Close();

            m_monitorChannel.DataReceived -= m_monitorChannel_DataReceived;

            // disconnect
            m_pumps?.Disconnect();

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
        ///
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
                readChannel = m_inChannel;
            }

            //Send the command over our serial port
            //TODO: Wrap this in exception checking
            //      (if there is an error, send out errorstring)
            if (m_inChannel.TryWrite(command, CONST_WRITETIMEOUT) == false)
            {
                //Couldn't send instruction
                ApplicationLogger.LogError(0, $"Agilent Pump \"{Name}\": Command got error response \"{AgilentPumpReplyErrorCodes.Instruction_Send_Failed}\"");
                return AgilentPumpReplyErrorCodes.Instruction_Send_Failed;
            }

            if (readChannel.TryRead(out reply, CONST_READTIMEOUT) == false)
            {
                //Couldn't read reply
                ApplicationLogger.LogError(0, $"Agilent Pump \"{Name}\": Command got error response \"{AgilentPumpReplyErrorCodes.Reply_Read_Failed}\"");
                return AgilentPumpReplyErrorCodes.Reply_Read_Failed;
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
                ApplicationLogger.LogError(0, $"Agilent Pump \"{Name}\": Command got error response \"{errorCode}\"");
            }

            return errorCode;
        }

        #endregion

        #region LC Methods - and method editor visible.

        /// <summary>
        /// Sets the pump mode.
        /// </summary>
        /// <param name="newMode">The new mode</param>
        [LCMethodEvent("Set Mode", 1, "", -1, false)]
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
        ///
        [LCMethodEvent("Set Flow Rate", 1, "", -1, false)]
        public void SetFlowRate(double newFlowRate)
        {
            if (Emulation)
            {
                return;
            }
            m_flowrate = newFlowRate;
            var reply = "";
            SendCommand("FLOW " + newFlowRate.ToString(CultureInfo.InvariantCulture),
                out reply, "Attempting to set flow rate to " + newFlowRate.ToString(CultureInfo.InvariantCulture));
        }

        ///// <summary>
        ///// Sets the mixer volume
        ///// </summary>
        ///// <param name="newVolumeuL">The new mixer volume, in uL</param>
        //[LCMethodEvent("Set Mixer Volume", 1, "", -1, false)]
        //public void SetMixerVolume(double newVolumeuL)
        //{
        //    if (Emulation)
        //    {
        //        return;
        //    }
        //
        //    var reply = "";
        //    if (newVolumeuL >= 0 && newVolumeuL <= 2000)
        //        SendCommand("MVOL " + newVolumeuL.ToString(CultureInfo.InvariantCulture),
        //            out reply, "Attempting to set mixer volume to " + newVolumeuL.ToString(CultureInfo.InvariantCulture));
        //}

        /// <summary>
        /// Sets the percent B concentration.
        /// </summary>
        /// <param name="percent">Percent B concentration to have.</param>
        [LCMethodEvent("Set Percent B", 1, "", -1, false)]
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

            //
            // We see if we ran over or not...if so then return failure, otherwise let it continue.
            //
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
            //
            // Make sure we have record of the method
            //
            if (mdict_methods.ContainsKey(method) == false)
                throw new Exception(string.Format("The method {0} does not exist.", method));

            //
            // Then send commands to start the method
            //
            var methodData = mdict_methods[method];
            var reply = "";

            //
            // The reason why we delete the time table, is to clear out any
            // existing methods that might have been downloaded to the pump.
            //
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
            //
            SendCommand("AT:DEL ", out reply, "Clearing time table.");
            SendCommand(methodData, out reply, "Loading method.");
            SendCommand("STRT", out reply, "Attempting to start method.");
        }

        /// <summary>
        /// Stops the currently running method.
        /// </summary>
        [LCMethodEvent("Stop Method", 1, "", -1, false)]
        public void StopMethod()
        {
            var reply = "";
            SendCommand("STOP", out reply, "Attempting to stop a method.");
        }

        /// <summary>
        /// Turns the pumps on.
        /// </summary>
        [LCMethodEvent("Turn Pump On", 1, "", -1, false)]
        public void PumpOn()
        {
            var reply = "";
            SendCommand("PUMP 1", out reply, "Attempting to turn pumps on.");
            GetPumpState();
        }

        /// <summary>
        /// Turns the pumps off
        /// </summary>
        [LCMethodEvent("Turn Pump Off", 1, "", -1, false)]
        public void PumpOff()
        {
            var reply = "";
            SendCommand("PUMP 0", out reply, "Attempting to turn pumps off.");
            GetPumpState();
        }

        /// <summary>
        /// Turns the pumps to standby
        /// </summary>
        [LCMethodEvent("Turn Pump Standby", 1, "", -1, false)]
        public void PumpStandby()
        {
            var reply = "";
            SendCommand("PUMP 2", out reply, "Attempting to turn pumps to standby.");
            GetPumpState();
        }

        #endregion

        #region Pump Interface methods

        public PumpState GetPumpState()
        {
            if (Emulation)
            {
                return PumpState.Unknown;
            }

            try
            {
                var reply = "";
                //ApplicationLogger.LogMessage(2, $"{Name}: Sending 'PUMP?'");
                var success = SendCommand($"ACT:PUMP?", out reply, $"Attempting to query pump state");
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

                var state = (PumpState) stateInt;
                ReactiveUI.RxApp.MainThreadScheduler.Schedule(() => PumpState = state);
                return state;
            }
            catch (Exception e)
            {
                ApplicationLogger.LogError(2, "Error getting pump state ", e);
            }

            ReactiveUI.RxApp.MainThreadScheduler.Schedule(() => PumpState = PumpState.Unknown);
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
            var reply = "";
            SendCommand("ACT:COMP?", out reply, "Attempting to query composition of solvent B.");
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
                SendCommand("LIST \"TT\"", out methodString, "Attempting to retrieve method", m_listChannel);
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
            var reply = "";
            SendCommand("FLOW?", out reply, "Attempting to query flow rate.");
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

            var reply = "";
            var purgeChannelText = purgeData.Channel.ToString();
            //ApplicationLogger.LogMessage(2, $"{Name}: Sending 'PG{purgeChannelText}?'");
            var success = SendCommand($"PG{purgeChannelText}?", out reply, $"Attempting to query purge settings for channel {purgeChannelText}");
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

            var reply = "";
            //ApplicationLogger.LogMessage(2, $"{Name}: Sending 'PRGE?'");
            var success = SendCommand($"PRGE?", out reply, $"Attempting to query channel purge states");
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
            var reply = "";
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
            var reply = "";
            SendCommand("ACT:FLOW?", out reply, "Attempting to query actual flow.");
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
            var reply = "";
            SendCommand("MODE?", out reply, "Attempting to query mode.");
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
                var reply = "";

                // Get the firmware revision of the running system
                // "IDN? gets "<manufacturer>,<model>,<serialNumber>,<running firmware revision>"; "IDN" causes "Identify by frontend LED"
                var gotIdent = SendCommand("IDN?", out reply);
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
            GetPumpStatus(PumpStatus);
        }

        private void GetPumpStatus(AgilentPumpStatus status)
        {
            try
            {
                var reply = "";
                string[] split;

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
                SendCommand("ACT:STAT?", out reply);

                var loc = reply.IndexOf("ACT:STAT", StringComparison.OrdinalIgnoreCase);
                split = reply.Substring(loc + 9).Split(new char[] { '"', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                status.GenericState = (AgilentPumpStateGeneric)int.Parse(split[0]);
                status.AnalysisState = (AgilentPumpStateAnalysis)int.Parse(split[1]);
                status.ErrorState = (AgilentPumpStateError)int.Parse(split[2]);
                status.NotReadyState = (AgilentPumpStateNotReady)int.Parse(split[3]);
                status.TestState = (AgilentPumpStateTest)int.Parse(split[4]);

                // ACT:BTMP? gets board temperature and leak status: ACT:BTMP? <boardtempC>,<leakSensorCurrent>,<leakState>
                SendCommand("ACT:BTMP?", out reply);
                split = reply.Substring(17).Split(',');
                status.BoardTemperatureC = split[0];
                status.LeakSensorCurrentMa = split[1];
                //PumpStatus.LeakState = split[2];
                switch (split[2])
                {
                    case "0":
                        status.LeakState = "No Leak";
                        break;
                    case "1":
                        status.LeakState = "Leak Detected";
                        break;
                    case "2":
                        status.LeakState = "NTC Board Sensor shorted";
                        break;
                    case "3":
                        status.LeakState = "NTC Board Sensor open";
                        break;
                    case "4":
                        status.LeakState = "PTC Leak Sensor shorted";
                        break;
                    case "5":
                        status.LeakState = "PTC Leak Sensor open";
                        break;
                }
                //ApplicationLogger.LogMessage(2, $"Pump {Name}: Got reply \"{reply}\"");

                // ACT:NRDY? gets the not ready status code, corresponding to bit flags
                SendCommand("ACT:NRDY?", out reply);
                status.NotReadyReasons = (AgilentPumpNotReadyStates) int.Parse(reply.Split(' ').Last());

                // ACT:SRDY? gets the start not ready status code, corresponding to bit flags
                SendCommand("ACT:SRDY?", out reply);
                status.StartNotReadyReasons = (AgilentPumpStartNotReadyStates)int.Parse(reply.Split(' ').Last());
            }
            catch (Exception e)
            {
                ApplicationLogger.LogError(0, "Error getting Agilent Pump Status", e);
            }
        }

        public void SetModuleDateTime()
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var reply = "";
            SendCommand($"TIME {timestamp}", out reply);
            //ApplicationLogger.LogMessage(2, $"Pump {Name}: Got reply \"{reply}\"");
            ReactiveUI.RxApp.MainThreadScheduler.Schedule(GetPumpInformation);
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
            ReactiveUI.RxApp.MainThreadScheduler.Schedule(GetPumpInformation);
        }

        #endregion

        #region Settings and Saving Methods

        /// <summary>
        /// Indicates that a save is required in the Fluidics Designer
        /// </summary>
        protected virtual void OnDeviceSaveRequired()
        {
            DeviceSaveRequired?.Invoke(this, null);
        }

        #endregion

        #region IDevice Data Provider Methods
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
        #endregion

        #region Performance and Error Notifications

        /// <summary>
        /// Writes the pump method time-table to the directory provided.
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="methodName"></param>
        private void WriteMethod(string directoryPath, string methodName)
        {
            if (mdict_methods.ContainsKey(methodName))
            {
                var methodData = mdict_methods[methodName];
            }
        }

        /// <summary>
        /// Writes the required data to the directory path provided.
        /// </summary>
        /// <param name="directoryPath">Path of directory to create files in.</param>
        /// <param name="name">Name of the method the user is requesting performance data about.</param>
        /// <param name="parameters">Parameters used to create the performance data.</param>
        public void WritePerformanceData(string directoryPath, string name, object[] parameters)
        {
            switch (name)
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

            notifications.AddRange(m_notificationStrings);

            foreach (var value in m_statusCodes.Values)
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
                                        CONST_PUMP_ERROR
            };

            foreach (var value in m_errorCodes.Values)
            {
                notifications.Add(value);
            }

            return notifications;
        }

        #endregion

        /// <summary>
        /// Returns the name of the device.
        /// </summary>
        /// <returns>String name of the device.</returns>
        public override string ToString()
        {
            return Name;
        }

        #region IFinchComponent Members
        /*
        public FinchComponentData GetData()
        {
            FinchComponentData component = new FinchComponentData();
            component.Status    = Status.ToString();
            component.Name      = Name;
            component.Type      = "Pump";
            component.LastUpdate = DateTime.Now;

            FinchScalarSignal port    = new FinchScalarSignal();
            port.Name   = "Port";
            port.Type   = FinchDataType.String;
            port.Units  = "";
            port.Value  = this.PortName.ToString();

            FinchDataTuple plotPressure = new FinchDataTuple();
            plotPressure.XUnits     = "DateTime";
            plotPressure.YUnits     = "bar";
            plotPressure.Name       = "Pressure";
            plotPressure.XDataType  = FinchDataType.DateTime;
            plotPressure.YDataType  = FinchDataType.Double;
            plotPressure.SetX<DateTime>(m_times);
            plotPressure.SetY<double>(m_pressures);
            component.Signals.Add(plotPressure);

            FinchDataTuple percentBPlot = new FinchDataTuple();
            percentBPlot.XUnits     = "DateTime";
            percentBPlot.YUnits     = "%B";
            percentBPlot.Name       = "Composition";
            percentBPlot.XDataType  = FinchDataType.DateTime;
            percentBPlot.YDataType  = FinchDataType.Double;
            percentBPlot.SetX<DateTime>(m_times);
            percentBPlot.SetY<double>(m_percentB);
            component.Signals.Add(percentBPlot);

            FinchDataTuple flowratePlot = new FinchDataTuple();
            flowratePlot.XUnits     = "DateTime";
            flowratePlot.YUnits     = "ul/min";
            flowratePlot.Name       = "Flowrate";
            flowratePlot.XDataType  = FinchDataType.DateTime;
            flowratePlot.YDataType  = FinchDataType.Double;
            flowratePlot.SetX<DateTime>(m_times);
            flowratePlot.SetY<double>(m_flowrates);
            component.Signals.Add(flowratePlot);

            return component;
        }*/

        #endregion

        #region IPump Members

        public List<MobilePhase> MobilePhases { get; set; }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
