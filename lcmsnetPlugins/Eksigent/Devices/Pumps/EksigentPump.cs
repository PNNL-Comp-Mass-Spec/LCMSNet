using System;
using System.Collections.Generic;
using System.ComponentModel;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Method;
using LcmsNetDataClasses.Logging;
//using DirectControl;
using EksigentNanoLC;
using System.IO;
using LcmsNetDataClasses.Devices.Pumps;
using FluidicsSDK.Devices;
using LcmsNetSDK;
using LcmsNetSDK.Data;

namespace Eksigent.Devices.Pumps
{

    /// <summary>
    /// Software interface to the Eksigent pumps.
    /// </summary>
    [classDeviceControlAttribute(typeof(EksigentPumpControlViewModel),
                                 null,
                                 "Eksigent Pump",
                                 "Pumps")
    ]
    public class EksigentPump : IDevice, IDisposable, IPump, IFluidicsPump
    {
        #region Events
        public delegate void DelegateChannelNumbers(int totalChannels);
        public event EventHandler<classDeviceStatusEventArgs> StatusUpdate;
        public event EventHandler<classDeviceStatusEventArgs> PumpStatus;
        public event EventHandler<classDeviceErrorEventArgs> Error;
        public event EventHandler DeviceSaveRequired;
        public event DelegateChannelNumbers ChannelNumbers;
        /// <summary>
        /// Poor design of Ekisgent requires an object to initialize before the devices can be communicated with.
        /// </summary>
        public event EventHandler RequiresOCXInitialization;
        /// <summary>
        /// Fired when the Agilent Pump finds out what method names are available.
        /// </summary>
        public event DelegateDeviceHasData MethodNames;
        #endregion

        #region Members
        /// <summary>
        /// Interface to the eksigent interface.
        /// </summary>
        private EksigentInterface m_hardware;
        /// <summary>
        /// Determines the number of channels present in the system.
        /// </summary>
        private int m_numberOfChannels;
        /// <summary>
        /// Specific interface.
        /// </summary>
        private COM_Driver m_comDriver;
        /// <summary>
        /// Maps method names to the method path where the physical file is located.
        /// </summary>
        readonly Dictionary<string, string> m_methodPaths;
        /// <summary>
        /// List of notification strings.
        /// </summary>
        readonly string[] m_notifyStrings;
        /// <summary>
        /// Total number of status attempts to make.
        /// </summary>
        private const int CONST_STATUS_TRIES = 50;
        /// <summary>
        /// Sent when the device is initialized.
        /// </summary>
        private const string CONST_INITIALIZED = "Initialized";
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
        private const int CONST_MONITORING_MINUTES = 10;
        private const int CONST_MONITORING_SECONDS_ELAPSED = 10;
        private const string MethodFolder1 = @"C:\Program Files (x86)\Eksigent NanoLC\settings\method";
        private const string MethodFolder2 = @"C:\Program Files\Eksigent NanoLC\settings\method";
        #endregion


        /// <summary>
        /// Default constructor.
        /// </summary>
        public EksigentPump()
        {
            Name = "Eksigent";
            Version = "1.0";
            if (Directory.Exists(MethodFolder1))
            {
                MethodsFolder = MethodFolder1;
            }
            else
            {
                MethodsFolder = MethodFolder2;
            }
            Methods = new List<string>();
            m_methodPaths = new Dictionary<string, string>();
            m_numberOfChannels = -1;
            m_notifyStrings = new[] {
                "Initializing",
                "Ready to download",
                "Downloading Method",
                "Preparing for Run",
                "Ready for Run",
                "Waiting for Contact Closure",
                "Running",
                "Executing post-run",
                "Error in run condition",
                "Busy",
                "Not connected",
                "Standby",
                "Off",
                "Paused",
                "Lamp temperature stabilizing",
                "Pump Not Ready",
                "Another application is controlling hardware",
                "Peak Park Mode",
                "Waiting for column temperature stabilization",
                "Waiting for flow stabilization",
            };

            m_flowrates = new List<double>();
            m_percentB = new List<double>();
            m_pressures = new List<double>();
            m_times = new List<DateTime>();

            TotalMonitoringMinutesDataToKeep = CONST_MONITORING_MINUTES;
            TotalMonitoringSecondElapsed = CONST_MONITORING_SECONDS_ELAPSED;
        }

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
        /// Gets or sets the value to wait for the instrument to go into a valid state before starting a run.
        /// </summary>
        [classPersistence("PrepareTimeout")]
        public int PrepareTimeout
        {
            get;
            set;
        }
        /// <summary>
        /// Path to the methods folder.
        /// </summary>
        [classPersistence("MethodsFolder")]
        public string MethodsFolder
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the list of method names available.
        /// </summary>
        public List<string> Methods
        {
            get;
            set;
        }
        #endregion

        private string m_name;

        #region IDevice Members
        /// <summary>
        /// Gets or sets the name of the device.
        /// </summary>
        public string Name
        {
            get { return m_name; }
            set
            {
                if (this.RaiseAndSetIfChangedRetBool(ref m_name, value))
                {
                    DeviceSaveRequired?.Invoke(this, null);
                }
            }
        }
        /// <summary>
        /// Gets or sets the version of this interface
        /// </summary>
        public string Version
        {
            get;
            set;
        }
        private enumDeviceStatus m_status;
        /// <summary>
        /// Gets or sets Current Status of device
        /// </summary>
        public enumDeviceStatus Status
        {
            get
            {
                return m_status;
            }
            set
            {
                m_status = value;
                StatusUpdate?.Invoke(this, new classDeviceStatusEventArgs(value, "Status", this));
            }
        }
        /// <summary>
        /// Gets or sets the event used to synchronize on wait handles from other control objects.
        /// </summary>
        public System.Threading.ManualResetEvent AbortEvent
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the error type status.
        /// </summary>
        public enumDeviceErrorStatus ErrorType
        {
            get;
            set;
        }
        /// <summary>
        /// Gets what type of device it is.
        /// </summary>
        public enumDeviceType DeviceType => enumDeviceType.Component;

        /// <summary>
        /// Gets or sets whether the device is in emulation mode or not.
        /// </summary>
        public bool Emulation
        {
            get;
            set;
        }
        public List<string> GetMethods()
        {
            GetMethodsFromFolder(MethodsFolder);
            return Methods;
        }
        /// <summary>
        /// Initializes the hardware interface.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool Initialize(ref string errorMessage)
        {
            if (Emulation)
            {
                return true;
            }
            var worked = GetMethodsFromFolder(MethodsFolder);
            if (!worked)
            {
                errorMessage = "Eksigent methods folder is empty or does not exist.";
                return false;
            }

            if (m_hardware == null)
            {
                try
                {
                    m_hardware = new EksigentInterfaceClass();
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    throw;
                }
                var initialized = m_hardware.Initialize();
                if (!initialized)
                {
                    errorMessage = "Could not create an interface to the Eksigent SDK.";
                    return false;
                }
                HandleStatus("Getting number of channels on Eksigent Pump.");
                m_numberOfChannels = m_hardware.GetNumChannels();
                ChannelNumbers?.Invoke(m_numberOfChannels);

                for (var i = 0; i < m_numberOfChannels; i++)
                {
                    HandleStatus("Getting Eksigent instrument status for channel " + i.ToString());
                    var status = -1;
                    var lastStatus = -1;
                    var counts = 0;
                    while (status != 1 && counts < CONST_STATUS_TRIES)
                    {
                        status = m_hardware.GetStatus(Convert.ToByte(i));
                        if (status != lastStatus)
                        {
                            ConvertAndLogStatus(status);
                        }
                        lastStatus = status;
                        System.Threading.Thread.Sleep(100);
                        counts++;
                    }

                    if (counts > CONST_STATUS_TRIES)
                    {
                        classApplicationLogger.LogMessage(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "Eksigent InstrStatus " + i + " EXCEEDED MAX TRIES");
                        Status = enumDeviceStatus.NotInitialized;
                        return false;
                    }
                }

                //classApplicationLogger.LogMessage(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "Eksigent COMDRIVER");
                try
                {
                    m_comDriver = m_hardware.get_COMInterface();
                    //classApplicationLogger.LogMessage(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "Eksigent HANDLERS");
                    //classApplicationLogger.LogMessage(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "flowprofilecomplete");
                    m_comDriver.FlowProfileComplete += driv_FlowProfileComplete;
                    //classApplicationLogger.LogMessage(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "devicemessage");
                    m_comDriver.DeviceMessage += driv_DeviceMessage;
                    //classApplicationLogger.LogMessage(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "davavail");
                    m_comDriver.DataAvailable += driv_DataAvailable;
                    //classApplicationLogger.LogMessage(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "Eksigent HANDLERS COMPLETE");
                }
                catch (Exception)
                {
                    // for some reason, assigning the event handlers above causes an invalid cast error to be thrown.
                    // it seems to have no effect on the functionality of the plugin, so we're ignoring it
                    // and not logging it so that it's not a distraction to users.
                    //classApplicationLogger.LogError(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, ex.Message);
                }

                if (RequiresOCXInitialization != null)
                {
                    try
                    {
                        classApplicationLogger.LogMessage(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "Eksigent OCXINIT");
                        RequiresOCXInitialization(this, null);
                    }
                    catch (Exception ex)
                    {
                        classApplicationLogger.LogMessage(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "Eksigent OCXINITFAILED");
                        HandleError("The registration failed for the Eksigent OCX.", ex);
                        errorMessage = "The registration failed for the Eksigent OCX.";
                        return false;
                    }
                }
                else
                {
                    classApplicationLogger.LogMessage(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "Eksigent STEPNOTPERFORMED");
                    errorMessage = "A step in the initialization was not performed that is required by the software.";
                    return false;
                }
            }
            else
            {
                classApplicationLogger.LogMessage(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "Eksigent SDKFAILURE");
                errorMessage = "Could not get a software hold on the Eksigent SDK objects required to control this device.";
                return false;
            }
            Status = enumDeviceStatus.Initialized;
            StatusUpdate?.Invoke(this, new classDeviceStatusEventArgs(Status, CONST_INITIALIZED, this));
            classApplicationLogger.LogMessage(classApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "Eksigent INITIALIZED!");
            return true;
        }

        public bool Shutdown()
        {
            return true;
        }

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
        public void UnRegisterDataProvider(string key, DelegateDeviceHasData remoteMethod)
        {

            switch (key.ToUpper())
            {
                case "METHODNAMES":
                    MethodNames -= remoteMethod;
                    break;
            }
        }
        public void WritePerformanceData(string directoryPath, string methodName, object[] parameters)
        {

        }

        public List<string> GetStatusNotificationList()
        {
            var notifications = new List<string>() { "Status" };
            notifications.AddRange(m_notifyStrings);
            notifications.Add(CONST_INITIALIZED);
            return notifications;
        }
        public List<string> GetErrorNotificationList()
        {
            return new List<string>() { "Error" };
        }
        #endregion

        #region Utility Methods
        /// <summary>
        /// Updates listeners with error information.
        /// </summary>
        /// <param name="error"></param>
        /// <param name="ex"></param>
        private void HandleError(string error, Exception ex)
        {
            if (Error != null)
            {
                var args = new classDeviceErrorEventArgs(error, ex, ErrorType, this, "Error");
                Error(this, args);
            }
            else
            {
                classApplicationLogger.LogError(0, error, ex);
            }
        }
        /// <summary>
        /// Updates listener with status information.
        /// </summary>
        /// <param name="status"></param>
        private void HandleStatus(string status)
        {
            if (StatusUpdate != null)
            {
                var args = new classDeviceStatusEventArgs(Status, "Status", status, this);

                StatusUpdate(this, args);
            }
            else
            {
                classApplicationLogger.LogMessage(0, status);
            }
        }
        /// <summary>
        /// Notifies listeners of the status but publishes the specific status notify string.
        /// </summary>
        /// <param name="statusNotifyString"></param>
        /// <param name="status"></param>
        private void HandleStatusType(string statusNotifyString, string status)
        {
            if (StatusUpdate != null)
            {
                var args = new classDeviceStatusEventArgs(Status, statusNotifyString, status, this);

                StatusUpdate(this, args);
            }
            else
            {
                classApplicationLogger.LogMessage(0, status);
            }
        }
        private void ConvertAndLogStatus(int code)
        {
            var message = ConvertStatusCode(code);
            HandleStatus(message);
        }
        /// <summary>
        /// Converts the status code into a message for the user.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private string ConvertStatusCode(int code)
        {
            string message;

            switch (code)
            {
                case 0:
                    message = "The instrument is initializing. This could be when it first starts up or while the Initialize routine is being handled.";
                    break;
                case 1:
                    message = "The instrument is ready to download a method. The Run Manager can call the PrepareForRun routine. If a new method is not loaded, the previous method is used when the run begins.  ";
                    break;
                case 2:
                    message = "The instrument is busy downloading the method. This state is rarely returned as the instrument loads the method immediately after requested";
                    break;
                case 3:
                    message = "The instrument is preparing for a run. Examples include executing the pre-run flush requirements of the chromatography method.  The instrument may also return this state if column temperature control is selected and still stabilizing. If these method attributes are not selected, this state will not occur.  If these method attributes are selected, the preflush state will begin immediately after the new method is loaded. While in this state the instrument will accept a StartRun command, but the instrument will not start until the pre-run flush requirements have been met (i.e. pre-run attributes of the method may not be bypassed)";
                    break;
                case 4:
                    message = "The instrument is ready for a run. The Run Manager can call the StartRun routine.  This state occurs after 'Preflush' is complete.";
                    break;
                case 5:
                    message = "The instrument is waiting for a contact closure to begin the run. This occurs when the Run Manager gives the StartRun command specifying that the instrument should wait for a contact closure. The Instrument Controller will move on to the 'Running' state as soon as possible after it receives the external trigger.";
                    break;
                case 6:
                    message = "The instrument is running.  The instrument will remain \"Running\" until it has completed its current method or is stopped by external intervention. ";
                    break;
                case 7:
                    message = "The instrument is executing the post-run requirements of the method. The run can be considered finished as far as the Run Manager is concerned. External data collection can be closed at this point. This state informs the run manager that the instrument is still busy, and gives an informative (post run) state to the operator. The run manger should treat this as \"not ready for the next run yet\".  Although not recommended, this state may be bypassed by loading the next method or by giving the StartRun command.";
                    break;
                case 8:
                    message = "The instrument is in an error condition. It will not be able to do a run.";
                    break;
                case 9:
                    message = "The instrument is busy. For example: Another application (supplied with the instrument) is using private interfaces into this instrument, to perform tuning or calibration. ";
                    break;
                case 10:
                    message = "No Eksigent device was found connected to the computer.  Possible reasons for setting this status are: 1.) The instrument wasn't powered up. 2.) The instrument wasn't connected to the proper COM port or was disconnected";
                    break;
                case 11:
                    message = "The instrument is in standby.";
                    break;
                case 12:
                    message = "The instrument is off.";
                    break;
                case 13:
                    message = "The instrument is paused.";
                    break;
                case 14:
                    message = "The lamp temperature is stabilizing.";
                    break;
                case 15:
                    message = "Equivalent to “Pump Not Ready.”  The Run Manager should assume that the device will become ready in time and should wait.";
                    break;
                case 16:
                    message = "The device is under direct control from another application or is executing a required flush operation  (i.e. as required to change mobile phase fluids)";
                    break;
                case 17:
                    message = "The device is in peak-park mode and is operating at reduced flowrates.";
                    break;
                case 18:
                    message = "The column temperature is stabilizing.";
                    break;
                case 19:
                    message = "Waiting for flow to stabilize";
                    break;
                default:
                    message = string.Format("The device status {0} is unknown and not documented by the Eksigent API.", code);
                    break;
            }

            PumpStatus?.Invoke(this, new classDeviceStatusEventArgs(Status, "None", message, this));
            if (code < m_notifyStrings.Length && code > -1)
            {
                HandleStatusType(m_notifyStrings[code], message);
            }
            return message;
        }
        /// <summary>
        /// Updates the internal list of method names.
        /// </summary>
        /// <param name="folder"></param>
        private bool GetMethodsFromFolder(string folder)
        {
            var worked = false;

            if (folder == null)
            {
                HandleError("Methods folder name is not set in the configuration file.", null);
                return false;
            }

            if (Directory.Exists(MethodsFolder))
            {
                var files = Directory.GetFiles(folder, "*.ini");
                Methods.Clear();
                m_methodPaths.Clear();

                if (files.Length > 0)
                {
                    foreach (var file in files)
                    {
                        var method = Path.GetFileNameWithoutExtension(file);
                        if (method != null)
                        {
                            m_methodPaths.Add(method, file);
                            Methods.Add(method);
                        }
                    }
                    worked = true;
                    ListMethods();
                }
                else
                {
                    HandleError(string.Format("No Eksigent methods were found in folder {0}", folder), null);
                }
            }
            else
            {
                HandleError(string.Format("The specified Eksigent methods folder does not exist {0}.", folder), null);
            }
            return worked;
        }
        private void ListMethods()
        {
            if (MethodNames != null)
            {
                var data = new List<object>();
                foreach (var name in Methods)
                {
                    data.Add(name);
                }
                MethodNames(this, data);
            }
        }
        #endregion

        #region COM Driver Event Handlers
        void driv_FlowProfileComplete(ref byte colnum, ref bool FinishNormal)
        {
            HandleStatus(string.Format("The Eksigent - {1} - flow profile has completed for channel: {0}",
                                                colnum,
                                                Name));
        }
        void driv_DeviceMessage(ref byte ChannelNumber, ref byte MessageNumber, ref object MessageData)
        {
            var message = ConvertStatusCode(Convert.ToInt32(MessageNumber));
            HandleStatus(string.Format("Eksigent device - {0} - had a message for channel {1} - {2}",
                                            Name,
                                            ChannelNumber,
                                            message));
        }
        void driv_DataAvailable(ref byte colnum, ref Array values, ref float T, ref float Qa, ref float Qb, ref float Pa, ref float Pb, ref float PnodeA, ref float PnodeB, ref float PowA, ref float PowB)
        {
            var time = LcmsNetSDK.TimeKeeper.Instance.Now; // DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0));

            // notifications
            //if (m_flowrate != 0)
            //{
            //    double percentFlowChange = (m_flowrate - flowrate) / m_flowrate;
            //   UpdateNotificationStatus(percentFlowChange.ToString(), CONST_FLOW_CHANGE);
            //}
            //UpdateNotificationStatus(pressure.ToString(), CONST_PRESSURE_VALUE);

            // Update log collections.
            m_times.Add(time);
            //  m_pressures.Add(pressure);
            // m_flowrates.Add(flowrate);
            //  m_percentB.Add(compositionB);

            // Find old data to remove -- needs to be updated (or could be) using LINQ
            //
            var count = m_times.Count;
            var total = (TotalMonitoringMinutesDataToKeep * 60) / TotalMonitoringSecondElapsed;
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
                MonitoringDataReceived?.Invoke(this,
        new PumpDataEventArgs(this,
                                m_times,
                                m_pressures,
                                m_flowrates,
                                m_percentB));
            }
            catch
            {
                // ignored
            }
        }
        #endregion

        #region Custom HW/Software Interface
        /// <summary>
        /// Injects a failure into the system.
        /// </summary>
        /// <returns></returns>
        //[classLCMethodAttribute("Set Flow Rate", enumMethodOperationTime.Parameter, "", -1, false)]
        public bool SetFlowRate(double timeout, double flowRate)
        {
            return true;
        }
        /// <summary>
        /// Starts a gradient method on the specified channel.
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="channel"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        [classLCMethodAttribute("Start Method", enumMethodOperationTime.Parameter, "MethodNames", 2, false)]
        public bool StartMethod(double timeout, double channel, string methodName)
        {
            if (Emulation)
            {
                return true;
            }
            var intChannel = Convert.ToInt32(channel);

            var containsMethod = m_methodPaths.ContainsKey(methodName);
            if (!containsMethod)
            {
                HandleError("The specified method does not exist or was not mapped to a file correctly.",
                                        new FileNotFoundException("Could not find the file."));
                return false;
            }
            var method = m_methodPaths[methodName];

            double injVol = 0;
            var runFlag = 0;
            var startMode = 0;
            var vialName = "A1";
            var worked = m_hardware.PrepareForRun(Convert.ToByte(intChannel),
                                                                method,
                                                                0,
                                                                ref startMode,
                                                                vialName,
                                                                ref injVol,
                                                                ref runFlag);
            var status = m_hardware.GetStatus(Convert.ToByte(intChannel));

            if (!worked)
            {
                HandleStatus("Could not start the requested method to be run: " + methodName);
                ConvertAndLogStatus(status);
                return false;
            }

            if (AbortEvent == null)
            {
                AbortEvent = new System.Threading.ManualResetEvent(false);
            }

            var handles = new System.Threading.WaitHandle[] { AbortEvent };
            worked = false;
            while (!worked)
            {
                worked = m_hardware.StartRun(Convert.ToByte(intChannel));
                System.Threading.WaitHandle.WaitAny(handles, 100);
                if (!worked)
                {
                    status = m_hardware.GetStatus(Convert.ToByte(intChannel));
                    ConvertAndLogStatus(status);
                }
            }

            return true;
        }
        /// <summary>
        /// Stops the run on the specified channel.
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        [classLCMethodAttribute("Stop Method", enumMethodOperationTime.Parameter, "", -1, false)]
        public bool StopMethod(double timeout, double channel)
        {
            if (Emulation)
            {
                return true;
            }
            var intChannel = Convert.ToInt32(channel);
            var worked = m_hardware.StopRun(Convert.ToByte(intChannel));
            if (!worked)
            {
                ConvertAndLogStatus(m_hardware.GetStatus(Convert.ToByte(intChannel)));
            }
            return worked;
        }
        #endregion

        #region Method Creation and Searching
        /// <summary>
        /// Gets the path of the file from the method name.
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        private string GetPathFromMethod(string methodName)
        {
            string path = null;

            var containsMethod = m_methodPaths.ContainsKey(methodName);
            if (containsMethod)
            {
                path = m_methodPaths[methodName];
            }
            return path;
        }
        #endregion

        #region Coupled UI calls to the Eksigent interface
        /// <summary>
        /// Displays the method menu.
        /// </summary>
        public void ShowMethodMenu(int channelNumber, string methodName)
        {

            var path = GetPathFromMethod(methodName);
            if (path == null)
            {
                HandleError("The method you requested does not exist.", null);
                return;
            }

            m_hardware.ShowMethodMenu(Convert.ToByte(channelNumber),
                                        ref path);
        }
        public void ShowDirectControl(int channelNumber, int owner)
        {
            m_hardware.ShowDirectControlMenu(Convert.ToByte(channelNumber),
                                            0,
                                            owner);
        }
        public void ShowDirectControl(int channelNumber)
        {
            m_hardware.ShowDirectControlMenu(Convert.ToByte(channelNumber), 0);
        }
        public void ShowAdvancedSettings(int owner)
        {
            m_hardware.ShowAdvancedSettings(0, owner);
        }
        public void ShowAdvancedSettings()
        {
            m_hardware.ShowAdvancedSettings(0);
        }
        public void ShowAlertsMenu()
        {
            m_hardware.ShowAlertsMenu();
        }
        public void ShowDiagnosticsMenu(int channelNumber, int owner)
        {
            m_hardware.ShowDiagnosticsMenu(Convert.ToByte(channelNumber),
                                            0,
                                            owner);
        }
        public void ShowDiagnosticsMenu(int channelNumber)
        {
            m_hardware.ShowDiagnosticsMenu(Convert.ToByte(channelNumber), 0);
        }
        public void ShowInstrumentConfigMenu()
        {
            m_hardware.ShowInstrumentConfigMenu();
        }
        public void ShowMainWindow(int owner)
        {
            m_hardware.ShowMainWindow(0, owner);
        }
        public void ShowMainWindow()
        {
            m_hardware.ShowMainWindow(0);
        }
        public void ShowMobilePhaseMenu(int channelNumber, int owner)
        {
            m_hardware.ShowMobilePhaseMenu(Convert.ToByte(channelNumber),
                                            0,
                                            owner);
        }
        public void ShowMobilePhaseMenu(int channelNumber)
        {
            m_hardware.ShowMobilePhaseMenu(Convert.ToByte(channelNumber), 0);
        }
        #endregion

        /// <summary>
        /// Returns the name of the pump.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        #region IDisposable Members
        public void Dispose()
        {
        }
        #endregion

        #region IPump Members
        /// <summary>
        /// Fired when monitoring data is received.
        /// </summary>
        public event EventHandler<PumpDataEventArgs> MonitoringDataReceived;
        private List<MobilePhase> m_mobilePhases;
        #endregion




        public double GetPressure()
        {
            return double.NaN;
        }

        public double GetFlowRate()
        {
            return double.NaN;
        }

        public double GetPercentB()
        {
            return double.NaN;
        }

        public double GetActualFlow()
        {
            return double.NaN;
        }

        public double GetMixerVolume()
        {
            return double.NaN;
        }


        public List<MobilePhase> MobilePhases
        {
            get { return m_mobilePhases ?? (m_mobilePhases = new List<MobilePhase>()); }
            set
            {
                // DO NOTHING
                m_mobilePhases = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
