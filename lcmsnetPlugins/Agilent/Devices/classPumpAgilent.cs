//*********************************************************************************************************
// Written by John Ryan, Dave Clark, Brian LaMarche for the US Department of Energy 
// Pacific Northwest National Laboratory, Richland, WA
// Copyright 2009, Battelle Memorial Institute
// Created 08/17/2009
//
/* Last modified 08/17/2009
 *      BLL - 9-8-09 
 *          Added unique naming to the constructor so it will display on the fluidics designer
 *          
/*********************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Xml;

using Agilent.Licop;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Method;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Logging;
using LcmsNetDataClasses.Devices.Pumps;
using FluidicsSDK.Devices;
using LcmsNetSDK.Data;

namespace Agilent.Devices.Pumps
{
    /// <summary>
    /// Interface to Agilent Pumps for running the solution of a gradient.
    /// </summary>
	[Serializable]	
    [classDeviceControlAttribute(typeof(controlPumpAgilent), 
                                 "Agilent 1200 Nano Series",
                                 "Pumps")]
    public class classPumpAgilent : IDevice, IPump, IFluidicsPump
    {
        #region Members
        /// <summary>
        /// An 'instrument' object for the Agilent pump drivers
        /// </summary>
        private Instrument mobj_pumps;
        /// <summary>
        /// A 'module' object for the Agilent pump drivers
        /// </summary>
        private Module mobj_module;     
        /// <summary>
        /// Error reporting channel.
        /// </summary>
        private Channel mobj_evChannel;
        /// <summary>
        /// Channel for retrieving methods from the pumps.
        /// </summary>
        private Channel mobj_listChannel;
        /// <summary>
        /// A 'channel' object for the Agilent pump drivers
        /// </summary>
        private Channel mobj_inChannel;
        /// <summary>
        /// Channel when monitoring the instrument.
        /// </summary>
        private Channel mobj_monitorChannel;        
        /// <summary>
        /// The device's name.
        /// </summary>
        private string mstring_name;        
        /// <summary>
        /// The device's verion.
        /// </summary>
        private string mstring_version;
        /// <summary>
        /// Indicates if the device is currently running.
        /// </summary>
        private bool mbool_running;
        /// <summary>
        /// Indicates if the device is being emulated.
        /// </summary>
        private bool mbool_emulation;
        /// <summary>
        /// The flow rate (used for save/load)
        /// </summary>
        private double mdouble_flowrate = -1.0;
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
        #endregion 

        #region Constants 
        #region Constants
        /// <summary>
        /// Status of the device.
        /// </summary>
        private enumDeviceStatus menum_status;
        private const string CONST_DEFAULTPORT  = "COM1";
        private const int CONST_DEFAULTTIMEOUT  = 6000; //milliseconds
        private const int CONST_WRITETIMEOUT    = 10000; //milliseconds
        private const int CONST_READTIMEOUT     = 10000; //milliseconds
        private const int CONST_MONITORING_MINUTES         = 10;
        private const int CONST_MONITORING_SECONDS_ELAPSED = 10;
        #endregion

        #region Status Constants 
        /// <summary>
        /// Notification string for flow changes
        /// </summary>
        private const string CONST_FLOW_CHANGE      = "Flow % below set point";
        /// <summary>
        /// Notification string for pressure values.
        /// </summary>
        private const string CONST_PRESSURE_VALUE       = "Pressure Value";
        private const string CONST_ERROR_ABOVE_PRESSURE = "Pressure Above Limit";
        private const string CONST_ERROR_BELOW_PRESSURE = "Pressure Below Limit";
        private const string CONST_ERROR_FLOW_EXCEEDS   = "Flow Exceeds limit while pressure control";
        private const string CONST_ERROR_FLOW_UNSTABLE  = "Column flow is unstable";
        #endregion

        #region Error Constants
        /// <summary>
        /// The error message was not set.
        /// </summary>
        private const string CONST_DEFAULT_ERROR    = "None";
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
        /// Fired when a method is added.
        /// </summary>
        public event EventHandler<classPumpMethodEventArgs> MethodAdded;
        /// <summary>
        /// Fired when a method is added.
        /// </summary>
        public event EventHandler<classPumpMethodEventArgs> MethodUpdated;
        /// <summary>
        /// Fired when monitoring data is received from the instrument.
        /// </summary>        
        public event EventHandler<PumpDataEventArgs> MonitoringDataReceived;
        /// <summary>
        /// Indicates that a save is required in the Fluidics Designer
        /// </summary>
        public event EventHandler<classDeviceStatusEventArgs> StatusUpdate;
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
        public event EventHandler<classDeviceErrorEventArgs> Error;
        /// <summary>
        /// List of times monitoring data was received.
        /// </summar;y>
        public List<DateTime> mlist_times;
        /// <summary>
        /// List of pressures used throughout the run.
        /// </summary>
        public List<double> mlist_pressures;
        /// <summary>
        /// List of flowrates used throughout the run.
        /// </summary>
        public List<double> mlist_flowrates;
        /// <summary>
        /// List of %B compositions throughout the run.
        /// </summary>
        public List<double> mlist_percentB;
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public classPumpAgilent()
        {
            CreateErrorCodes();
            CreateStatusCodes();

            PortName        = CONST_DEFAULTPORT;
            if (mdict_methods == null)
            {
                mdict_methods = new Dictionary<string, string>();
            }
            mstring_name    = "pump";  

            mlist_flowrates = new List<double>();
            mlist_percentB  = new List<double>();
            mlist_pressures = new List<double>();
            mlist_times     = new List<DateTime>();
			menum_status	= enumDeviceStatus.NotInitialized;

            TotalMonitoringMinutesDataToKeep = CONST_MONITORING_MINUTES;
            TotalMonitoringSecondElapsed     = CONST_MONITORING_SECONDS_ELAPSED;

            if (m_notificationStrings == null)
            {
                m_notificationStrings = new string[]{
                    CONST_FLOW_CHANGE,
                    CONST_PRESSURE_VALUE                
                };
            }

            MobilePhases = new List<MobilePhase>();
            MobilePhases.Add(new MobilePhase("A",   "This is a test"));
            MobilePhases.Add(new MobilePhase("B",   "This is a test"));
            MobilePhases.Add(new MobilePhase("Aux", "This is a test"));
        }

        #endregion

        #region Properties
        [classPersistenceAttribute("TotalMonitoringMinutes")]
        public int TotalMonitoringMinutesDataToKeep
        {
            get;
            set;
        }
        [classPersistenceAttribute("TotalMonitoringSecondsElapsed")]
        public int TotalMonitoringSecondElapsed
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the Emulation state.
        /// </summary>
        public bool Emulation
        {
            get
            {
                return mbool_emulation;
            }
            set
            {
                mbool_emulation = value;
            }
        }
        /// <summary>
        /// Gets the device's status
        /// </summary>
        public LcmsNetDataClasses.Devices.enumDeviceStatus Status
        {
            get
            {
                return menum_status;
            }
            set
            {
                if (value != menum_status && StatusUpdate != null)
                    StatusUpdate(this, new classDeviceStatusEventArgs(value, "Status", this));
				menum_status = value;
            }
        }
        /// <summary>
        /// Gets or sets whether the device is running
        /// </summary>
        public bool Running
        {
            get
            {
                return mbool_running;
            }
            set
            {
                mbool_running = value;
            }
        }
        /// <summary>
        /// Gets or sets the device's name
        /// </summary>
        public string Name
        {
            get
            {
                return mstring_name;
            }
            set
            {
                mstring_name = value;
                OnDeviceSaveRequired();
            }
        }
        /// <summary>
        /// Gets or sets the device's version
        /// </summary>
        public string Version
        {
            get
            {
                return mstring_version;
            }
            set
            {
                mstring_version = value;
            }
        }
        /// <summary>
        /// Gets or sets the port name to use to communicate with the pumps.
        /// </summary>
        [classPersistenceAttribute("PortName")]
        public string PortName
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the error type of the last error reported.
        /// </summary>
        public enumDeviceErrorStatus ErrorType
        {
            get;
            set;
        }
        /// <summary>
        /// Gets the system device type.
        /// </summary>
        public enumDeviceType DeviceType
        {
            get
            {
                return enumDeviceType.Component;
            }
        }
        /// <summary>
        /// Gets or sets the abort event for scheduling.
        /// </summary>
        public System.Threading.ManualResetEvent AbortEvent
        {
            get;
            set;
        }        
        #endregion

        #region Pump Purging
        /// <summary>
        /// Purges the pump for the number of minutes provided.
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="flow"></param>
        /// <param name="numberOfMinutes"></param>
        /// <returns></returns>

        [classLCMethodAttribute("Purge Channel", enumMethodOperationTime.Parameter, "", -1, false)]
        public bool PurgePump(double timeout, enumPurgePumpChannel channel, double flow, double numberOfMinutes)
        {
            string command = string.Format("PG{0} {1}, {2}", channel, Convert.ToInt32(flow), numberOfMinutes);
            string reply = "";
            string error = "";
            bool worked = SendCommand(command, ref reply, error);

            if (!worked)
                return worked;

            int bitField = 0;
            switch (channel)
            {
                case enumPurgePumpChannel.A1:
                    bitField = 1;
                    break;
                case enumPurgePumpChannel.A2:
                    bitField = 2;
                    break;
                case enumPurgePumpChannel.B1:
                    bitField = 4;
                    break;
                case enumPurgePumpChannel.B2:
                    bitField = 8;
                    break;
            }
            command = string.Format("PRGE {0}, 1, 1", bitField);
            worked  = SendCommand(command, ref reply, error);
            if (!worked)
                return worked;

            return SendCommand("PURG 1", ref reply, error);
        }
        /// <summary>
        /// Aborts purging of the pumps for the given channel.
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>        
        [classLCMethodAttribute("Abort Purge", enumMethodOperationTime.Parameter, "", -1, false)]
        public bool AbortPurges(double timeout)
        { 
            string reply    = "";
            string error    = "";            
            string command  = string.Format("PRGE 0, 0, 0");
            bool worked     = SendCommand(command, ref reply, error);
            return worked;
        }
        #endregion

        #region Pump Status Code Converters
        private void CreateErrorCodes()
        {
            if (m_errorCodes == null)
            {
                Dictionary<string, string> map = new Dictionary<string, string>();


                map.Add("ER 2037", "STRT while purging");
                map.Add("ER 2038", "STRT while fast composition change");
                map.Add("ER 2039", "No calibration table with specified index existing");
                map.Add("ER 2040", "Not an allowed index for a user-defined table");
                map.Add("ER 2041", "Specified command allowed in test mode only");
                map.Add("ER 2042", "Calibration table cannot be deleted while in use");
                map.Add("ER 2043", "FCC not possible, no partner module connected");
                map.Add("ER 2044", "FCC active, configuration change not allowed");
                map.Add("ER 2045", "FCC not allowed in this mode");
                map.Add("ER 2046", "FCC not possible, no column flow");
                map.Add("ER 2047", "FSAC base points not specified in descending order.");
                map.Add("ER 2048", "Attempt to specify more than 11 points in FSAC table.");


                m_errorCodes = map;
            }
        }

        private void CreateStatusCodes()
        {
            if (m_statusCodes == null)
            {
                Dictionary<string, string> map = new Dictionary<string, string>();
                map.Add("EV 2227", "no start allowed while purging.");
                map.Add("EV 2228", "no start allowed while fast composition change.");
                map.Add("EV 2229", "Column flow in limit.");
                map.Add("EV 2230", "Column flow out of limit.");
                map.Add("EV 2231", "Fast composition change started.");
                map.Add("EV 2232", "Fast composition change complete.");
                map.Add("EV 2233", "Calibration table stored. Parameter: table reference, 1..10");
                map.Add("EV 2234", "Calibration table deleted. Parameter: table reference, 1..10");
                map.Add("EV 2235", "Fast Composition Change failed.");
                map.Add("EE 2066", "Column flow unstable.");
                map.Add("EE 2067", "No EMPV connected.");
                map.Add("EE 2068", "No flow sensor connected.");
                map.Add("EE 2069", "EMPV initialization failed.");
                map.Add("EE 2090", "Flow sensor not supported by pump.");
                map.Add("ES 2116", "Pump in normal mode.            ");
                map.Add("ES 2117", "Pump in micro mode.             ");
                map.Add("ES 2118", "Pump in test mode.              ");
                map.Add("ES 2119", "Purge valve on.                 ");
                map.Add("ES 2120", "Purge valve off.                ");
                map.Add("ES 2121", "Column flow in sensor range.    ");
                map.Add("ES 2122", "Column flow out of sensor range.");


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
        void mobj_pumps_ErrorOccurred(object sender, ErrorEventArgs e)
        {            
            string name =  e.Message;
            if (e.Message == null)
                return;

            string [] errorMessage = e.Message.Split(',');
            if (m_errorCodes.ContainsKey(errorMessage[0]))
            {
                string displayedError = m_errorCodes[errorMessage[0]];
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
            if (mdict_methods.ContainsKey(methodname) == true)
            {
                mdict_methods[methodname] = method;
                if (MethodUpdated != null)
                {
                    MethodUpdated(this, new classPumpMethodEventArgs(methodname));
                }
            }
            else
            {
                mdict_methods.Add(methodname, method);
                if (MethodAdded != null)
                {
                    MethodAdded(this, new classPumpMethodEventArgs(methodname));
                }

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
                string[] keys = new string[mdict_methods.Keys.Count];
                mdict_methods.Keys.CopyTo(keys, 0);

                List<object> data = new List<object>();
                data.AddRange(keys);

                MethodNames(this, (List<object>)data);
            }
        }
        /// <summary>
        /// Initializes the device.
        /// </summary>
        /// <returns>True on success</returns>
        public bool Initialize(ref string errorMessage)
        {
            if (mbool_emulation == true)
			{
                return true;
            }
            
            mobj_pumps = new Instrument(CONST_DEFAULTTIMEOUT, CONST_DEFAULTTIMEOUT);                         
            mobj_pumps.ErrorOccurred += new EventHandler<ErrorEventArgs>(mobj_pumps_ErrorOccurred);                 

            /// 
            /// Try initial connection
            /// 
            if (mobj_pumps.TryConnect(PortName, CONST_DEFAULTTIMEOUT) == false)
			{
				errorMessage = "Could not connect to the Agilent Pumps.";
                HandleError(errorMessage, CONST_INITIALIZE_ERROR);
                //Could not connect
                return false;
            }
                        
            mobj_module = mobj_pumps.CreateModule(mobj_pumps.GetAccessPointIdentifier());


            /// 
            /// Open a list channel to read time tables.
            /// 
            mobj_listChannel = mobj_module.CreateChannel("LI");            
            if (mobj_listChannel.TryOpen(ReadMode.Polling) == false)
			{
                errorMessage = "Could not open the communication channel for time table data.";
                HandleError(errorMessage, CONST_INITIALIZE_ERROR);
                return false;
            }
                
            /// 
            /// And an EV channel.Channel for errors or state events
            /// 
            mobj_evChannel               = mobj_module.CreateChannel("EV");
            mobj_evChannel.DataReceived += new EventHandler<DataEventArgs>(mobj_evChannel_DataReceived);
            if (mobj_evChannel.TryOpen(ReadMode.Events) == false)
			{
                //"Could not open EV channel."
                errorMessage = "Could not open the communication channel for error events";
                HandleError(errorMessage, CONST_INITIALIZE_ERROR);
                return false;
            }

            /// 
            /// Channel for inputs
            /// 
            mobj_inChannel = mobj_module.CreateChannel("IN");
            if (mobj_inChannel.TryOpen(ReadMode.Polling) == false)
			{
                //"Could not open IN channel."
                errorMessage = "Could not open the communication channel for input";
                HandleError(errorMessage, CONST_INITIALIZE_ERROR);
                return false;
            }

            /// 
            /// Monitoring for the pumps
            /// 
            mobj_monitorChannel                  = mobj_module.CreateChannel("MO");                
            mobj_monitorChannel.DataReceived += new EventHandler<DataEventArgs>(mobj_monitorChannel_DataReceived);                        
            if (mobj_monitorChannel.TryOpen(ReadMode.Events) == false)
			{
                errorMessage = "Could not open the communication channel for monitoring data";
                HandleError(errorMessage, CONST_INITIALIZE_ERROR);
                return false;
            }

            string reply = "";
            bool worked = SendCommand(
                string.Format("MONI:STRT {0},\"ACT:FLOW?; ACT:PRES?; ACT:COMP?\"", TotalMonitoringSecondElapsed),
                ref reply,
                errorMessage);

            if (worked == false)
			{
                errorMessage = "Could not put the pumps in monitoring mode.";
                HandleError(errorMessage, CONST_INITIALIZE_ERROR);
                return false;            
			}
            
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
                      new classDeviceErrorEventArgs(message,
                                                    ex,
                                                    enumDeviceErrorStatus.ErrorAffectsAllColumns,	
                                                    this,
                                                    type));
            }
        }
        private void UpdateNotificationStatus(string message, string type)
        {
            if (StatusUpdate != null)
            {
                classDeviceStatusEventArgs args = new classDeviceStatusEventArgs(Status, type, message, this);
                StatusUpdate(this, args);
            }
        }

        #region Pump Events
        /// <summary>
        /// Handles errors from the pump.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mobj_evChannel_DataReceived(object sender, DataEventArgs e)
        {            
            string data = e.AsciiData;

            if (data.Contains("EE 2014"))
            {
                if (Error != null)
                {
                    Error(this, new classDeviceErrorEventArgs(CONST_ERROR_ABOVE_PRESSURE,
                        null,
                        enumDeviceErrorStatus.ErrorAffectsAllColumns,
                        this,
                        CONST_ERROR_ABOVE_PRESSURE));                    
                }
            }
            else if (data.Contains("EE 2015"))
            {
                if (Error != null)
                {
                    Error(this, new classDeviceErrorEventArgs(CONST_ERROR_BELOW_PRESSURE,
                        null,
                        enumDeviceErrorStatus.ErrorAffectsAllColumns,
                        this,
                        CONST_ERROR_BELOW_PRESSURE));
                }
            }
            else if (data.Contains("EE 2064"))
            {
                if (Error != null)
                {
                    Error(this, new classDeviceErrorEventArgs(CONST_ERROR_FLOW_EXCEEDS,
                        null,
                        enumDeviceErrorStatus.ErrorAffectsAllColumns,
                        this,
                        CONST_ERROR_FLOW_EXCEEDS));
                }
            }
            else if (data.Contains("EE 2066"))
            {
                if (Error != null)
                {
                    Error(this, new classDeviceErrorEventArgs(CONST_ERROR_FLOW_UNSTABLE,
                        null,
                        enumDeviceErrorStatus.ErrorAffectsAllColumns,
                        this,
                        CONST_ERROR_FLOW_UNSTABLE));
                }
            }
            else
            {                                
                string[] errorMessage = data.Split(',');
                if (m_statusCodes.ContainsKey(errorMessage[0]))
                {
                    string displayedError = m_statusCodes[errorMessage[0]];
                    HandleError(displayedError, displayedError);
                }
                else
                {
                    HandleError("Pump " + Name + " status: " + data, CONST_PUMP_ERROR);
                }
            }
            LcmsNetDataClasses.Logging.classApplicationLogger.LogMessage(2, Name + " Agilent Pump Message " + data);
        }
        /// <summary>
        /// Handles m5onitoring data from the pumps.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mobj_monitorChannel_DataReceived(object sender, DataEventArgs e)
        {
            try
            {
                double pressure, flowrate, compositionB;
                if (e == null || e.AsciiData == null)
                    return;

                pressure = double.NaN;
                flowrate = double.NaN;
                compositionB = double.NaN;
                
                // Parse out the data                
                string[] dataArray = e.AsciiData.Split(new string[] { "ACT:FLOW" }, StringSplitOptions.RemoveEmptyEntries);
                if (dataArray.Length > 1)
                {
                    string data = dataArray[1].Replace(";ACT:", ",");
                    data = data.Replace("COMP", "");
                    data = data.Replace("PRES", "");

                    string[] values = data.Split(',');
                    flowrate = Convert.ToDouble(values[0]);
                    pressure = Convert.ToDouble(values[1]);
                    compositionB = Convert.ToDouble(values[3]);
                }
                ProcessMonitoringData(flowrate, pressure, compositionB);
            }
            catch(Exception)
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
            DateTime time = LcmsNetSDK.TimeKeeper.Instance.Now; // DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0));

            // notifications
            if (mdouble_flowrate != 0)
            {
                double percentFlowChange = (mdouble_flowrate - flowrate) / mdouble_flowrate;
                UpdateNotificationStatus(percentFlowChange.ToString(), CONST_FLOW_CHANGE);
            }
            UpdateNotificationStatus(pressure.ToString(), CONST_PRESSURE_VALUE);
            
            // Update log collections.
            mlist_times.Add(time);
            mlist_pressures.Add(pressure);
            mlist_flowrates.Add(flowrate);
            mlist_percentB.Add(compositionB);

            /// 
            /// Find old data to remove -- needs to be updated (or could be) using LINQ
            /// 
            int count = mlist_times.Count;
            int total = (TotalMonitoringMinutesDataToKeep * 60) / TotalMonitoringSecondElapsed;
            if (count >= total)
            {
                int i = 0;
                while (time.Subtract(mlist_times[i]).TotalMinutes > TotalMonitoringMinutesDataToKeep && i < mlist_times.Count)
                {
                    i++;
                }
                
                if (i > 0)
                {
                    i = Math.Min(i, mlist_times.Count - 1);
                    mlist_times.RemoveRange(0, i);
                    mlist_flowrates.RemoveRange(0, i);
                    mlist_pressures.RemoveRange(0, i);
                    mlist_percentB.RemoveRange(0, i);
                }
            }

            // Alert the user data is ready
            try
            {
                if (MonitoringDataReceived != null)
                {
                    MonitoringDataReceived(this,
                            new PumpDataEventArgs(  this, 
                                                    mlist_times, 
                                                    mlist_pressures, 
                                                    mlist_flowrates, 
                                                    mlist_percentB));
                }
            }
            catch
            {
            }            
        }
        #endregion

        /// <summary>
        /// Closes the connection to the device
        /// </summary>
        /// <returns>True on success</returns>
        public bool Shutdown()
        {			   
            if (mbool_emulation == true)
            {
                return true;
            }
           
            if (mobj_evChannel == null)
                return true;

            // close EV channel
            mobj_evChannel.Close();

            // close IN channel
            mobj_inChannel.Close();

            mobj_monitorChannel.DataReceived -= mobj_monitorChannel_DataReceived;
            
            // disconnect
            if (mobj_pumps == null)
                return true;

            mobj_pumps.Disconnect();

            return true;
        }
        /// <summary>
        /// Sends a command to the pump
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <param name="reply">The string to fill with the reply</param>
        /// <param name="errorstring">Any error results</param>
        /// <returns></returns>
        private bool SendCommand(string command, ref string reply, string errorstring)
        {
            return SendCommand(command, ref reply, errorstring, mobj_inChannel);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="reply"></param>
        /// <param name="errorstring"></param>
        /// <param name="readChannel"></param>
        /// <returns></returns>
        private bool SendCommand(string command, ref string reply, string errorstring, Channel readChannel)
        {
            if (mbool_emulation == true)
            {
                return true;
            }
            //Send the command over our serial port
            //TODO: Wrap this in exception checking
            //      (if there is an error, send out errorstring)
            if (mobj_inChannel.TryWrite(command, CONST_WRITETIMEOUT) == false)
            {
                //Couldn't send instruction
                return false;
            }            
            if (readChannel.TryRead(out reply, CONST_READTIMEOUT) == false)
            {
                //Couldn't read reply
                return false;
            }

            return true;
        }
        #endregion

        #region LC Methods - and method editor visible.
        /// <summary>
        /// Sets the pump mode.
        /// </summary>
        /// <param name="newMode">The new mode</param>
        [classLCMethodAttribute("Set Mode", 1, "", -1, false)]
        public void SetMode(enumPumpAgilentModes newMode)
        {
            if (mbool_emulation == true)
            {
                return;
            }

            string reply = "";
            SendCommand("MODE " + (int)newMode, ref reply, "Attempting to set mode to " + newMode.ToString());
        }
        /// <summary>
        /// Sets the flow rate.
        /// </summary>
        /// <param name="newFlowRate">The new flow rate.</param>
        /// 
        [classLCMethodAttribute("Set Flow Rate", 1, "", -1, false)]
        public void SetFlowRate(double newFlowRate)
        {
            if (mbool_emulation == true)
            {
                return;
            }
            mdouble_flowrate    = newFlowRate;
            string reply        = "";
            SendCommand("FLOW " + newFlowRate.ToString(), ref reply, "Attempting to set flow rate to " + newFlowRate.ToString());
        }
        /// <summary>
        /// Sets the mixer volume
        /// </summary>
        /// <param name="newVolumeuL">The new mixer volume, in uL</param>        
        [classLCMethodAttribute("Set Mixer Volume", 1, "", -1, false)]
        public void SetMixerVolume(double newVolumeuL)
        {
            if (mbool_emulation == true)
            {
                return;
            }
            
            string reply = "";
            if (newVolumeuL >= 0 && newVolumeuL <= 2000)
                SendCommand("MVOL " + newVolumeuL.ToString(), ref reply, "Attempting to set mixer volume to " + newVolumeuL.ToString());
        }
        /// <summary>
        /// Sets the percent B concentration.
        /// </summary>
        /// <param name="percent">Percent B concentration to have.</param>
        [classLCMethodAttribute("Set Percent B", 1, "", -1, false)]
        public void SetPercentB(double percent)
        {
            string reply = "";
            SendCommand("COMP " + Convert.ToInt32(percent).ToString() + ",-1,-1", ref reply, "Attempting to set percent of solvent B"); 
        }
        /// <summary>
        /// Runs the method provided by a string.
        /// </summary>
        /// <param name="method">Method to run stored on the pumps.</param>
        [classLCMethodAttribute("Start Method", enumMethodOperationTime.Parameter, "MethodNames", 1, true)]
        public void StartMethod(double timeout, string method)
        {
            DateTime start = LcmsNetSDK.TimeKeeper.Instance.Now; // DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0));
            
            StartMethod(method);

            /// 
            /// We see if we ran over or not...if so then return failure, otherwise let it continue.
            /// 
            TimeSpan span = LcmsNetSDK.TimeKeeper.Instance.Now.Subtract(start);

            LcmsNetDataClasses.Devices.classTimerDevice timer = new LcmsNetDataClasses.Devices.classTimerDevice();
            timer.WaitSeconds(span.TotalSeconds);
        }
        /// <summary>
        /// Runs the method provided by a string.
        /// </summary>
        /// <param name="method">Method to run stored on the pumps.</param>
        public void StartMethod(string method)
        {
            /// 
            /// Make sure we have record of the method 
            /// 
            if (mdict_methods.ContainsKey(method) == false)
                throw new Exception(string.Format("The method {0} does not exist.", method));

            /// 
            /// Then send commands to start the method
            /// 
            string methodData = mdict_methods[method];
            string reply = "";

            /// 
            /// The reason why we delete the time table, is to clear out any 
            /// existing methods that might have been downloaded to the pump.
            /// 
            /// For example, if a time table was loaded and has entry:
            ///     5 mins  - 30% B
            ///     75 mins - 40%  B
            /// And then we want to run this timetable:
            ///     0 mins  - 0%   B
            ///     15 mins - 50%  B
            ///     60 mins - 100% B
            ///     65 mins - 0%   B
            /// when the time table is done the pumps may resort back to the final
            /// entries and set the %B to 40 instead of the 0 we desire.
            /// 
            SendCommand("AT:DEL ", ref reply, "Clearing time table.");
            SendCommand(methodData, ref reply, "Loading method.");
            SendCommand("STRT", ref reply, "Attempting to start method.");
        }
        /// <summary>
        /// Stops the currently running method.
        /// </summary>
        [classLCMethodAttribute("Stop Method", 1, "", -1, false)]        
        public void StopMethod()
        {
            string reply = "";
            SendCommand("STOP", ref reply, "Attempting to stop a method.");
        }
        /// <summary>
        /// Turns the pumps on.
        /// </summary>
        [classLCMethodAttribute("Turn Pump On", 1, "", -1, false)]
        public void PumpOn()
        {
            string reply = "";
            SendCommand("PUMP 1", ref reply, "Attempting to turn pumps on.");
        }
        /// <summary>
        /// Turns the pumps off
        /// </summary>
        [classLCMethodAttribute("Turn Pump Off", 1, "", -1, false)]
        public void PumpOff()
        {
            string reply = "";
            SendCommand("PUMP 0", ref reply, "Attempting to turn pumps off.");
        }
        #endregion

        #region Pump Interface methods
        /// <summary>
        /// Gets the percent B.
        /// </summary>
        /// <returns></returns>
        public double GetPercentB()
        {
            if (mbool_emulation == true)
            {
                return 0.0;
            }
            string reply = "";
            SendCommand("ACT:COMP?", ref reply, "Attmempting to query composition of solvent B.");
            //expect: RA 0000 ACT:PRES 16.00

            if (reply.Contains("COMP"))
            {
                string [] replies = reply.Split(' ');
                if (replies.Length != 4)
                {
                    HandleError("Invalid pump response to composition request", CONST_COMPOSITION_B_SET);
                    return double.NaN;
                }
                string[] percents = replies[3].Split(','); 
                return Convert.ToDouble(percents[1]);
            }
            else
            {
                reply = "";
                SendCommand("ACT:COMP?", ref reply, "Attmempting to query composition of solvent B.");
                if (reply.Contains("COMP"))
                {
                    string[] replies = reply.Split(' ');
                    if (replies.Length != 4)
                    {
                        HandleError("Invalid pump response to composition request", CONST_COMPOSITION_B_SET);
                        return double.NaN;
                    }
                    string[] percents = replies[3].Split(',');
                    return Convert.ToDouble(percents[1]);
                }
                else
                {
                    HandleError("Invalid pump response to composition request", CONST_COMPOSITION_B_SET);
                    return double.NaN;
                }
            }
        }
        /// <summary>
        /// Gets the current loaded method in the pump module. 
        /// </summary>
        /// <returns>Method string kept on the pump.</returns>
        public string RetrieveMethod()
        {
            string methodString = "";
            if (mbool_emulation == true)
            {
                methodString = "test;\ntest12";
            }
            else
            {
                SendCommand("LIST \"TT\"", ref methodString, "Attempting to retrieve method", mobj_listChannel);
            }            
            return methodString;
        }
        /// <summary>
        /// Gets the flow rate. Note that this is the ideal flow rate not the actual flow rate.
        /// </summary>
        /// <returns>The flow rate</returns>
        public double GetFlowRate()
        {
            if (mbool_emulation == true)
            {
                return 0.0;
            }
            string reply = "";
            SendCommand("FLOW?", ref reply, "Attempting to query flow rate.");
            //We expect something like:
            //reply = "RA 0000 FLOW 2.000";
            int start = reply.IndexOf("FLOW");
            if (start == -1)
            {
                SendCommand("FLOW?", ref reply, "Attempting to re-query flow rate.");
                start = reply.IndexOf("FLOW");
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
        /// Gets the pressure
        /// </summary>
        /// <returns>The pressure</returns>
        public double GetPressure()
        {
            if (mbool_emulation == true)
            {
                return 0.0;
            }
            string reply = "";
            SendCommand("ACT:PRES?", ref reply, "Attmempting to query pressure.");
            //expect: RA 0000 ACT:PRES 16.00

            if (reply.Contains("ACT:PRES"))
            {
                string[] replies = reply.Split(' ');
                if (replies.Length != 4)
                {
                    HandleError("Invalid pump response to pressure request", CONST_PRESSURE_SET);
                    return double.NaN;
                }                
                return Convert.ToDouble(replies[3]);
            }
            else
            {
                reply = "";
                SendCommand("ACT:PRES?", ref reply, "Attmempting to query pressure.");
                //expect: RA 0000 ACT:PRES 16.00

                if (reply.Contains("ACT:PRES"))
                {
                    string[] replies = reply.Split(' ');
                    if (replies.Length != 4)
                    {
                        HandleError("Invalid pump response to pressure request", CONST_PRESSURE_SET);
                        return double.NaN;
                    }
                    return Convert.ToDouble(replies[3]);
                }
                else
                {
                    HandleError("Invalid pump response to pressure request", CONST_PRESSURE_SET);
                    return double.NaN;
                }
            }
        }
        /// <summary>
        /// Gets the current mixer volume.
        /// </summary>
        /// <returns>The current mixer volume</returns>
        public double GetMixerVolume()
        {
            if (mbool_emulation == true)
            {
                return 0.0;
            }
            string reply = "";
            SendCommand("MVOL?", ref reply, "Attempting to query mixer volume.");
            int start = reply.IndexOf("MVOL");
            if (start == -1)
            {
                reply = "";
                SendCommand("MVOL?", ref reply, "Attempting to query mixer volume.");
                start = reply.IndexOf("MVOL");
                if (start == -1)
                {
                    HandleError("Invalid pump response to volume request", CONST_VOLUME_SET);
                    return double.NaN;
                }
            }
            reply = reply.Substring(start + 5, reply.Length - (start + 5));
            return Convert.ToDouble(reply);
        }
        /// <summary>
        /// Gets the actual flow rate
        /// </summary>
        /// <returns>The actual measured current flow rate</returns>
        public double GetActualFlow()
        {
            if (mbool_emulation == true)
            {
                return 0.0;
            }
            string reply = "";
            SendCommand("ACT:FLOW?", ref reply, "Attempting to query actual flow.");
            int start = reply.IndexOf("ACT:FLOW");
            if (start == -1)
            {
                reply = "";
                SendCommand("ACT:FLOW?", ref reply, "Attempting to query actual flow.");
                start = reply.IndexOf("ACT:FLOW");
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
        public enumPumpAgilentModes GetMode()
        {
            if (mbool_emulation == true)
            {
                return enumPumpAgilentModes.Unknown;
            }
            string reply = "";
            SendCommand("MODE?", ref reply, "Attempting to query mode.");
            //reply = "RA 000 MODE 1
            int start = reply.IndexOf("MODE");
            if (start == -1)
            {
                //TODO: Error!
                return enumPumpAgilentModes.Unknown;
            }
            reply = reply.Substring(start + 5, 1);
            return (enumPumpAgilentModes)(Convert.ToInt32(reply));
        }
        #endregion

        #region Settings and Saving Methods
        /// <summary>
        /// Indicates that a save is required in the Fluidics Designer
        /// </summary>
        protected virtual void OnDeviceSaveRequired()
        {
            if (DeviceSaveRequired != null)
            {
                DeviceSaveRequired(this, null);
            }
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
        /// <param name="filepath"></param>
        /// <param name="methodName"></param>
        /// <param name="methodData"></param>
        private void WriteMethod(string directoryPath, string methodName)
        {            
            if (mdict_methods.ContainsKey(methodName) == true)
            {
                string methodData = mdict_methods[methodName];
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
            List<string> notifications = new List<string>() { "Status"
                                                            };

            notifications.AddRange(m_notificationStrings);

            foreach (string value in m_statusCodes.Values)
            {
                notifications.Add(value);
            }
            return notifications;
		  }
		public List<string> GetErrorNotificationList()
        {
            List<string> notifications = new List<string>() {   
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

            
            foreach (string value in m_errorCodes.Values)
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
            return this.Name;
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
            plotPressure.SetX<DateTime>(mlist_times);
            plotPressure.SetY<double>(mlist_pressures);
            component.Signals.Add(plotPressure);
            
            FinchDataTuple percentBPlot = new FinchDataTuple();
            percentBPlot.XUnits     = "DateTime";
            percentBPlot.YUnits     = "%B";
            percentBPlot.Name       = "Composition";
            percentBPlot.XDataType  = FinchDataType.DateTime;
            percentBPlot.YDataType  = FinchDataType.Double;
            percentBPlot.SetX<DateTime>(mlist_times);
            percentBPlot.SetY<double>(mlist_percentB);
            component.Signals.Add(percentBPlot);
                        
            FinchDataTuple flowratePlot = new FinchDataTuple();
            flowratePlot.XUnits     = "DateTime";
            flowratePlot.YUnits     = "ul/min";
            flowratePlot.Name       = "Flowrate";
            flowratePlot.XDataType  = FinchDataType.DateTime;
            flowratePlot.YDataType  = FinchDataType.Double;
            flowratePlot.SetX<DateTime>(mlist_times);
            flowratePlot.SetY<double>(mlist_flowrates);
            component.Signals.Add(flowratePlot);

            return component;
        }*/

        #endregion

        #region IPump Members


        public List<MobilePhase> MobilePhases
        {
            get;
            set;
        }

        #endregion
    }
}