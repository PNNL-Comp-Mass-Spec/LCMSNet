using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using LcmsNetSDK;
using LcmsNetSDK.Data;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Logging;
using LcmsNetSDK.Method;
using LcmsNetSDK.System;

namespace LcmsNetPlugins.XcaliburLC
{
    [Serializable]
    [DeviceControl(typeof(XcaliburLCViewModel),
        "Xcalibur LC",
        "Pumps")]
    public class XcaliburLCPump : IDevice, IHasDataProvider, IHasPerformanceData, IDisposable // TODO: maybe implement IPump?
    {
        public const string DefaultLcMethodPath = @"C:\Xcalibur\methods";

        /// <summary>
        /// The error message was not set.
        /// </summary>
        private const string CONST_DEFAULT_ERROR = "None";

        private const string CONST_PUMP_ERROR = "Pump Error";
        private const string CONST_INITIALIZE_ERROR = "Failed to Initialize";

        /// <summary>
        /// Default constructor - for use within LCMSNet
        /// </summary>
        public XcaliburLCPump()
        {
            CreateErrorCodes();
            CreateStatusCodes();

            if (availableMethods == null)
            {
                availableMethods = new Dictionary<string, string>();
            }

            if (methodFileWatcher == null)
            {
                var path = DefaultLcMethodPath;
                //ApplicationLogger.LogMessage(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "PATH: " + path);
                methodFileWatcher = new FileSystemWatcher(path, "*.meth");
                methodFileWatcher.Created += MethodWatcherFileCreated;
                methodFileWatcher.Changed += MethodWatcherFileChanged;
                methodFileWatcher.Deleted += MethodWatcherFileDeleted;
                methodFileWatcher.EnableRaisingEvents = true;
            }

            ReadMethodDirectory();

            deviceName = "xcalibur";

            deviceStatus = DeviceStatus.NotInitialized;

            if (notificationStrings == null)
            {
                notificationStrings = new string[0];
                //{
                //    "message1",
                //    "message2"
                //};
            }

            xcaliburCom = new XcaliburCOM(ProcessEvent);
        }

        public void Dispose()
        {
            xcaliburCom.Dispose();
        }

        private readonly XcaliburCOM xcaliburCom;
#pragma warning disable RCS1085 // Use auto-implemented property.
        internal XcaliburCOM XcaliburCom => xcaliburCom;
#pragma warning restore RCS1085 // Use auto-implemented property.

        /// <summary>
        /// The device's name.
        /// </summary>
        private string deviceName;

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

        private static readonly Regex methodNameExclusionRegex = new Regex("^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static Dictionary<string, string> errorCodes;
        private static Dictionary<string, string> statusCodes;

        private bool runCompleted = false;
        private bool readyToDownload = false;

        /// <summary>
        /// Status of the device.
        /// </summary>
        private DeviceStatus deviceStatus;

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

            return true;
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

            return true;
        }

        /// <summary>
        /// Runs the method provided by a string.
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="sample"></param>
        /// <param name="method">Method to run stored on the pumps.</param>
        [LCMethodEvent("Start Method", MethodOperationTimeoutType.Parameter, 1, "MethodNames", 2, HasPerformanceData = true, EventDescription = "Sends the sample name and method to Xcalibur to start a run")]
        [LCMethodEvent("Start Method NonDeterm", MethodOperationTimeoutType.Parameter, 1, "MethodNames", 2, HasPerformanceData = true, IgnoreLeftoverTime = true, EventDescription = "Sends the sample name and method to Xcalibur to start a run\nNon-deterministic, will not wait for the end of the timeout before starting the next step")]
        public bool StartMethod(double timeout, ISampleInfo sample, string method)
        {
            return StartMethod(method, sample.Name);
        }

        /// <summary>
        /// Runs the method provided by a string.
        /// </summary>
        /// <param name="method">Method to run stored on the pumps.</param>
        /// <param name="sampleName">Name to use for data file</param>
        public bool StartMethod(string method, string sampleName = "testLcmsNetRun")
        {
            // Make sure we have record of the method
            if (availableMethods.ContainsKey(method) == false) // TODO: Preferable to not throw an exception here.
                throw new Exception($"The method {method} does not exist.");

            // Then send commands to start the method
            var methodPath = availableMethods[method];

            var result = xcaliburCom.StartSample(sampleName, methodPath, out var errorMessage);

            if (!result)
            {
                ApplicationLogger.LogError(LogLevel.Error, $"Xcalibur method start failed!: {errorMessage}");
            }

            runCompleted = false;

            return result;
        }

        /// <summary>
        /// Stops the currently running method.
        /// </summary>
        [LCMethodEvent("Stop Method", 1000)]
        public void StopMethod()
        {
            xcaliburCom.StopQueue();
        }

        /// <summary>
        /// Causes the software to wait for a "READY" response from the PAL before proceeding.
        /// </summary>
        /// <param name="timeout">The timeout value, in seconds.</param>
        /// <returns>Integer error code.</returns>
        [LCMethodEvent("Wait Until Ready", MethodOperationTimeoutType.Parameter, EventDescription = "Wait until Xcalibur is reporting state \"Ready To Download\".\nDeterministic, next step will not be started until timeout is reached")]
        [LCMethodEvent("Wait Until Ready NonDeterm", MethodOperationTimeoutType.Parameter, IgnoreLeftoverTime = true, EventDescription = "Wait until the Xcalibur is reporting state \"Ready To Download\".\nNon-deterministic, will not wait for the end of the timeout before starting the next step")]
        public bool WaitUntilReady(double timeout)
        {
            var start = TimeKeeper.Instance.Now;
            var end = start;

            // Make sure we update this once, in case it has never been updated by the event firing
            var currentState = XcaliburCom.GetRunStatus();
            readyToDownload = currentState.Trim().Equals("Ready To Download", StringComparison.OrdinalIgnoreCase);

            var delayTime = 500;
            while (end.Subtract(start).TotalSeconds < timeout)
            {
                if (readyToDownload)
                {
                    return true;
                }

                System.Threading.Thread.Sleep(delayTime);
                end = TimeKeeper.Instance.Now;
            }

            // Timed out
            return false;
        }

        /// <summary>
        /// Wait until error, synchronization point, or ready
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        [LCMethodEvent("Wait for Run Complete", MethodOperationTimeoutType.Parameter, EventDescription = "Wait for the Xcalibur run to complete.\nDeterministic, next step will not be started until timeout is reached")]
        [LCMethodEvent("Wait for Run Complete NonDeterm", MethodOperationTimeoutType.Parameter, IgnoreLeftoverTime = true, EventDescription = "Wait for the Xcalibur run to complete.\nNon-deterministic, will not wait for the end of the timeout before starting the next step")]
        public bool WaitUntilStopPoint(double timeout)
        {
            var start = TimeKeeper.Instance.Now;
            var end = start;

            var delayTime = 500;
            while (end.Subtract(start).TotalSeconds < timeout)
            {
                if (runCompleted)
                {
                    return true;
                }

                System.Threading.Thread.Sleep(delayTime);
                end = TimeKeeper.Instance.Now;
            }

            // Timed out
            return false;
        }

        private void MethodWatcherFileChanged(object sender, FileSystemEventArgs e)
        {
            // Filter out method names with GUIDs (these are temporary sample-run-time methods)
            var name = Path.GetFileNameWithoutExtension(e.FullPath);
            if (methodNameExclusionRegex.IsMatch(name))
            {
                return;
            }

            var methodLoaded = false;
            do
            {
                try
                {
                    // TODO: use e.ChangeType to conditionalize behavior
                    AddMethod(name, e.FullPath);
                    ApplicationLogger.LogMessage(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, e.FullPath + $" {e.ChangeType.ToString().ToLowerInvariant()}.");
                    methodLoaded = true;
                }
                catch (IOException)
                {
                    //probably caught the file being opened for writing.
                }
            } while (!methodLoaded);
        }

        private void MethodWatcherFileDeleted(object sender, FileSystemEventArgs e)
        {
            var methodLoaded = false;
            do
            {
                try
                {
                    var name = Path.GetFileNameWithoutExtension(e.FullPath);
                    if (string.IsNullOrWhiteSpace(name) || methodNameExclusionRegex.IsMatch(name))
                    {
                        return;
                    }

                    RemoveMethod(name);
                    ApplicationLogger.LogMessage(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, e.FullPath + " removed.");
                    methodLoaded = true;
                }
                catch (IOException)
                {
                    //probably caught the file being opened for writing.
                }
            } while (!methodLoaded);
        }

        private void MethodWatcherFileCreated(object sender, FileSystemEventArgs e)
        {
            //AddMethod(Path.GetFileNameWithoutExtension(e.FullPath), File.ReadAllText(e.FullPath));
            //ApplicationLogger.LogMessage(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, e.FullPath + " created.");
        }

        /// <summary>
        /// Reads the pump method directory and alerts the pumps of new methods to run.
        /// </summary>
        public void ReadMethodDirectory()
        {
            try
            {
                var path = DefaultLcMethodPath;
                if (!Directory.Exists(path))
                {
                    throw new DirectoryNotFoundException("The directory " + path + " does not exist.");
                }

                var filenames = Directory.GetFiles(path, "*.meth");

                var methods = new Dictionary<string, string>();
                foreach (var filename in filenames)
                {
                    // Filter out method names with GUIDs (these are temporary sample-run-time methods)
                    var name = Path.GetFileNameWithoutExtension(filename);
                    if (methodNameExclusionRegex.IsMatch(name))
                    {
                        continue;
                    }

                    methods[name] = filename;
                }

                // Clear any existing pump methods
                if (methods.Count > 0)
                {
                    ClearMethods();
                    AddMethods(methods);
                }
            }
            catch (DirectoryNotFoundException ex)
            {
                ApplicationLogger.LogError(0, ex.Message, ex);
            }
        }

        /// <summary>
        /// Clears all of the listed pump methods.
        /// </summary>
        public void ClearMethods()
        {
            availableMethods.Clear();
        }

        /// <summary>
        /// Removes a given method.
        /// </summary>
        /// <param name="methodName">Name of method to remove</param>
        public void RemoveMethod(string methodName)
        {
            if (availableMethods.ContainsKey(methodName))
            {
                availableMethods.Remove(methodName);

                // Fire ListMethods since we removed one.
                ListMethods();
            }
        }

        public void AddMethods(Dictionary<string, string> methods)
        {
            availableMethods = methods;
            ListMethods();
        }

        /// <summary>
        /// Adds a given method.
        /// </summary>
        /// <param name="methodName">Name of method to track.</param>
        /// <param name="methodPath">Method data to store.</param>
        public void AddMethod(string methodName, string methodPath)
        {
            if (availableMethods.ContainsKey(methodName))
            {
                availableMethods[methodName] = methodPath;

                // Don't need to fire ListMethods() - the name didn't change.
            }
            else
            {
                availableMethods.Add(methodName, methodPath);

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

        public IEnumerable<string> GetMethodNames()
        {
            return availableMethods.Keys;
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

        private void ProcessEvent(EventClass eventClass, EventName eventName, string message)
        {
            if (eventClass == EventClass.Status && eventName == EventName.RunEnded)
            {
                runCompleted = true;
            }

            var notification = "";
            switch (eventClass)
            {
                case EventClass.Info:
                    if (message.IndexOf("upper pressure limit exceeded", StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        Error?.Invoke(this, new DeviceErrorEventArgs(message, null, DeviceErrorStatus.ErrorAffectsAllColumns, this, statusCodes[ "overpressure"]));
                    }
                    else if (message.IndexOf("lower pressure limit exceeded", StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        Error?.Invoke(this, new DeviceErrorEventArgs(message, null, DeviceErrorStatus.ErrorAffectsAllColumns, this, statusCodes["underpressure"]));
                    }
                    else if (message.IndexOf("Module is not connected", StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        Error?.Invoke(this, new DeviceErrorEventArgs(message, null, DeviceErrorStatus.ErrorAffectsAllColumns, this, statusCodes["notConnected"]));
                    }
                    // TODO: are there any other InformationMessages we will need to act on?
                    //StatusUpdate?.Invoke(this, new DeviceStatusEventArgs(Status, statusCodes[eventName.ToString()], this, $"Info message: {message}"));
                    break;
                case EventClass.Warning:
                    if (message.IndexOf("reported an error while acquiring", StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        Error?.Invoke(this, new DeviceErrorEventArgs(message, null, DeviceErrorStatus.ErrorAffectsAllColumns, this, statusCodes["acqerror"]));
                    }
                    //StatusUpdate?.Invoke(this, new DeviceStatusEventArgs(Status, eventName.ToString(), this, $"Warning message: {message}"));
                    // TODO: are there any other WarningMessages we will need to act on?
                    break;
                case EventClass.Error:
                    Error?.Invoke(this, new DeviceErrorEventArgs(message, null, DeviceErrorStatus.ErrorAffectsAllColumns, this, eventName.ToString()));
                    break;
                case EventClass.Status:
                    notification = statusCodes.TryGetValue("Status " + eventName.ToString(), out var status) ? status : eventName.ToString();
                    StatusUpdate?.Invoke(this, new DeviceStatusEventArgs(Status, notification, this, $"Status message: {message}"));
                    break;
                case EventClass.StateChange:
                    notification = statusCodes.TryGetValue("SC " + eventName.ToString(), out var state) ? state : eventName.ToString();
                    StatusUpdate?.Invoke(this, new DeviceStatusEventArgs(Status, notification, this, $"State changed: {message}"));
                    // TODO: Process the message or event name and handle some state changes differently
                    break;
                case EventClass.ManagerStateChange:
                    notification = statusCodes.TryGetValue("MS " + message, out var mState) ? mState : statusCodes["M Unknown"];
                    readyToDownload = message.Trim().Equals("Ready To Download", StringComparison.OrdinalIgnoreCase);
                    StatusUpdate?.Invoke(this, new DeviceStatusEventArgs(Status, notification, this, $"Manager state changed: {message}"));
                    // TODO: Process the message or event name and handle some state changes differently
                    break;
                default:
                    StatusUpdate?.Invoke(this, new DeviceStatusEventArgs(Status, eventName.ToString(), this, $"Unknown event class \"{eventClass.ToString()}\"; message: {message}"));
                    break;
            }
        }

        private void CreateErrorCodes()
        {
            if (errorCodes == null)
            {
                var map = new Dictionary<string, string>
                {
                    {"acqerror", "Xcalibur Acquisition Error"},
                    {"overpressure", "Upper pressure limit exceeded"},
                    {"underpressure", "Lower pressure limit exceeded"},
                    {"notConnected", "Module is not connected"},
                    {EventName.ErrorMessage.ToString(), "Xcalibur Error"},
                    {EventName.ProgramError.ToString(), "Xcalibur Program Error"},
                    {EventName.MethodCheckFail.ToString(), "Xcalibur Method Check Failed"},
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
                    //{EventName.InformationalMessage.ToString(), "Xcalibur Informational Message"},
                    {"MS Waiting For Devices", "Run Manager: Waiting For Devices"},
                    {"MS Ready To Download", "Run Manager: Ready To Download"},
                    {"MS Downloading", "Run Manager: Downloading"},
                    {"MS Devices Are Getting Ready", "Run Manager: Devices Are Getting Ready"},
                    {"MS Devices Are Ready", "Run Manager: Devices Are Ready"},
                    {"MS Control Device Started", "Run Manager: Control Device Started"},
                    {"MS Acquiring", "Run Manager: Acquiring"},
                    {"MS Devices Stopping", "Run Manager: Devices Stopping"},
                    {"MS Unknown", "Unknown Run Manager State"},
                    {"Status " + EventName.MethodCheckOK, "Method Check OK"},
                    {"Status " + EventName.RunEnded, "Acquisition Run Completed"},
                    {"SC " + EventName.Resume, "Queue Resumed"},
                    {"SC " + EventName.Pause, "Queue Paused"},
                };

                statusCodes = map;
            }
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

        public string GetMethodText(string methodName)
        {
            if (!availableMethods.TryGetValue(methodName, out var methodPath))
            {
                return "";
            }

            return XcaliburMethodReader.GetMethodText(methodPath);
        }

        /// <summary>
        /// Writes the required data to the directory path provided.
        /// </summary>
        /// <param name="directoryPath">Path of directory to create files in.</param>
        /// <param name="methodName">Name of the method the user is requesting performance data about.</param>
        /// <param name="parameters">Parameters used to create the performance data.</param>
        public void WritePerformanceData(string directoryPath, string methodName, object[] parameters)
        {
            var data = GetPerformanceData(methodName, parameters);
        }

        public string GetPerformanceData(string methodName, object[] parameters)
        {
            switch (methodName)
            {
                case "Start Method":
                    if (parameters != null && parameters.Length > 2 && availableMethods.TryGetValue((string)parameters[2], out var methodPath))
                    {
                        return XcaliburMethodReader.GetMethodText(methodPath);
                    }

                    break;
            }

            return "";
        }

        public List<string> GetStatusNotificationList()
        {
            var notifications = new List<string>() { "Status" };

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
                                        CONST_INITIALIZE_ERROR,
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

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
