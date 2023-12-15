using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using LcmsNetSDK.Logging;
using LcmsNetSDK.System;

namespace XcaliburControl
{
    public class XcaliburController : INotifyPropertyChanged, IDisposable
    {
        public const string DefaultLcMethodPath = @"C:\Xcalibur\methods";

        /// <summary>
        /// Default constructor
        /// </summary>
        public XcaliburController()
        {
            //ApplicationLogger.LogMessage(ApplicationLogger.CONST_STATUS_LEVEL_CRITICAL, "PATH: " + DefaultLcMethodPath);
            methodFiles = new InstrumentMethodFiles(DefaultLcMethodPath);
            methodFiles.MethodsUpdated += (sender, args) => ListMethods();

            ReadMethodDirectory();

            xcaliburCom = new XcaliburCOM(ProcessEvent);
            xcaliburCom.PropertyChanged += XcaliburCom_PropertyChanged;

            methodFiles.PropertyChanged += MethodFiles_PropertyChanged;
        }

        private void MethodFiles_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(methodFiles.XcaliburMethodsDirectoryPath)))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(XcaliburMethodsDirectoryPath)));
                ReadMethodDirectory();
            }
        }

        private void XcaliburCom_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(xcaliburCom.DataFileDirectoryPath)))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(XcaliburDataFileDirectoryPath)));
            }
            else if (e.PropertyName.Equals(nameof(xcaliburCom.TemplateSldPath)))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TemplateSldFilePath)));
            }
        }

        static XcaliburController()
        {
            ErrorCodes = CreateErrorCodes();
            StatusCodes = CreateStatusCodes();
        }

        public void Dispose()
        {
            xcaliburCom.Dispose();
            methodFiles.Dispose();
        }

        private readonly XcaliburCOM xcaliburCom;

        /// <summary>
        /// Filesystem watcher for real-time updating of instrument methods.
        /// </summary>
        private readonly InstrumentMethodFiles methodFiles;

        protected static readonly IReadOnlyDictionary<string, string> ErrorCodes;
        protected static readonly IReadOnlyDictionary<string, string> StatusCodes;

        private bool runCompleted = false;
        private bool readyToDownload = false;
        private bool waitingForContactClosure = false;

        protected IReadOnlyDictionary<string, string> AvailableMethods => methodFiles.MethodFiles;

        /// <summary>
        /// Status events fired when events are received from Xcalibur acquisition
        /// </summary>
        public event EventHandler<XcaliburStatusEventArgs> StatusChange;

        /// <summary>
        /// Error events fired when events are received from Xcalibur acquisition
        /// </summary>
        public event EventHandler<XcaliburErrorEventArgs> ErrorReport;

        public event EventHandler<IEnumerable<string>> MethodNamesUpdated;

        public string XcaliburMethodsDirectoryPath
        {
            get => methodFiles.XcaliburMethodsDirectoryPath;
            set => methodFiles.XcaliburMethodsDirectoryPath = value;
        }

        public string XcaliburDataFileDirectoryPath
        {
            get => xcaliburCom.DataFileDirectoryPath;
            set => xcaliburCom.DataFileDirectoryPath = value;
        }

        public string TemplateSldFilePath
        {
            get => xcaliburCom.TemplateSldPath;
            set => xcaliburCom.TemplateSldPath = value;
        }

        /// <summary>
        /// Runs the method provided by a string.
        /// </summary>
        /// <param name="method">Method to run.</param>
        /// <param name="sampleName">Name to use for data file</param>
        public bool StartMethod(string method, string sampleName = "testLcmsNetRun")
        {
            // Make sure we have record of the method
            if (AvailableMethods.ContainsKey(method) == false) // TODO: Preferable to not throw an exception here.
                throw new Exception($"The method {method} does not exist.");

            // Then send commands to start the method
            var methodPath = AvailableMethods[method];

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
        public void StopQueue()
        {
            xcaliburCom.StopQueue();
        }

        /// <summary>
        /// Causes the software to wait for a "READY" response from the PAL before proceeding.
        /// </summary>
        /// <param name="timeout">The timeout value, in seconds.</param>
        /// <returns>Integer error code.</returns>
        public bool WaitUntilReadyToDownload(double timeout)
        {
            var start = TimeKeeper.Instance.Now;
            var end = start;

            // Make sure we update this once, in case it has never been updated by the event firing
            var currentState = xcaliburCom.GetRunStatus();
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
        public bool WaitUntilWaitingForContactClosure(double timeout)
        {
            var start = TimeKeeper.Instance.Now;
            var end = start;

            var delayTime = 500;
            while (end.Subtract(start).TotalSeconds < timeout)
            {
                if (waitingForContactClosure)
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
        public bool WaitUntilRunComplete(double timeout)
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

        /// <summary>
        /// Reads the Xcalibur method directory and alerts the watching objects of new methods to run.
        /// </summary>
        public void ReadMethodDirectory()
        {
            methodFiles.ReadMethodDirectory();
        }

        /// <summary>
        /// Lists the methods
        /// </summary>
        private void ListMethods()
        {
            MethodNamesUpdated?.Invoke(this, AvailableMethods.Keys.ToList());
        }

        public IEnumerable<string> GetMethodNames()
        {
            return AvailableMethods.Keys;
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
                        ErrorReport?.Invoke(this, new XcaliburErrorEventArgs(message, StatusCodes["overpressure"]));
                    }
                    else if (message.IndexOf("lower pressure limit exceeded", StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        ErrorReport?.Invoke(this, new XcaliburErrorEventArgs(message, StatusCodes["underpressure"]));
                    }
                    else if (message.IndexOf("Module is not connected", StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        ErrorReport?.Invoke(this, new XcaliburErrorEventArgs(message, StatusCodes["notConnected"]));
                    }
                    // TODO: are there any other InformationMessages we will need to act on?
                    //StatusChange?.Invoke(this, new XcaliburStatusEventArgs($"Info message: {message}", StatusCodes[eventName.ToString()]));
                    break;
                case EventClass.Warning:
                    if (message.IndexOf("reported an error while acquiring", StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        ErrorReport?.Invoke(this, new XcaliburErrorEventArgs(message, StatusCodes["acqerror"]));
                    }
                    //StatusChange?.Invoke(this, new XcaliburStatusEventArgs($"Warning message: {message}", eventName.ToString()));
                    // TODO: are there any other WarningMessages we will need to act on?
                    break;
                case EventClass.Error:
                    ErrorReport?.Invoke(this, new XcaliburErrorEventArgs(message, eventName.ToString()));
                    break;
                case EventClass.Status:
                    notification = StatusCodes.TryGetValue("Status " + eventName.ToString(), out var status) ? status : eventName.ToString();
                    StatusChange?.Invoke(this, new XcaliburStatusEventArgs($"Status message: {message}", notification));
                    break;
                case EventClass.StateChange:
                    notification = StatusCodes.TryGetValue("SC " + eventName.ToString(), out var state) ? state : eventName.ToString();
                    StatusChange?.Invoke(this, new XcaliburStatusEventArgs($"State changed: {message}", notification));
                    // TODO: Process the message or event name and handle some state changes differently
                    break;
                case EventClass.ManagerStateChange:
                    notification = StatusCodes.TryGetValue("MS " + message, out var mState) ? mState : StatusCodes["M Unknown"];
                    readyToDownload = message.Trim().Equals("Ready To Download", StringComparison.OrdinalIgnoreCase);
                    StatusChange?.Invoke(this, new XcaliburStatusEventArgs($"Manager state changed: {message}", notification));
                    // TODO: Process the message or event name and handle some state changes differently
                    break;
                case EventClass.DeviceStateChange:
                    var split = message.Split(':');
                    if (int.TryParse(split[0], out var deviceStatusCode))
                    {
                        if ((deviceStatusCode < 0 || deviceStatusCode > 16) && split.Length >= 3)
                        {
                            // Report the value on the log file for potential update?
                            ApplicationLogger.LogMessage(LogLevel.Warning, $"Xcalibur LC Device status code unknown: device '{split[1]}', code {deviceStatusCode}, reported meaning '{split[2]}'");
                        }
                        else if (processedDeviceStatusCodes.Contains(deviceStatusCode))
                        {
                            var statusString = XcaliburCOM.ConvertDeviceStatusCodeToString(deviceStatusCode);
                            notification = ErrorCodes.TryGetValue("DS " + deviceStatusCode, out var dState) ? dState : "";
                            if (string.IsNullOrWhiteSpace(notification))
                            {
                                ApplicationLogger.LogMessage(LogLevel.Warning, $"Xcalibur LC plugin code error: Configured to process device status code {deviceStatusCode}, but no corresponding entry in reported plugin status codes");
                            }
                            else
                            {
                                ErrorReport?.Invoke(this, new XcaliburErrorEventArgs($"Device state changed: {message}", notification));
                                //StatusChange?.Invoke(this, new XcaliburStatusEventArgs($"Device state changed: {message}", notification));
                                // TODO: Process the message or event name and handle some state changes differently
                            }
                        }

                        waitingForContactClosure = deviceStatusCode == 5;
                    }

                    break;
                default:
                    StatusChange?.Invoke(this, new XcaliburStatusEventArgs($"Unknown event class \"{eventClass.ToString()}\"; message: {message}", eventName.ToString()));
                    break;
            }
        }

        private readonly IReadOnlyList<int> processedDeviceStatusCodes = new int[] { 8, 10, 11, 12, 13, 15 };

        private static Dictionary<string, string> CreateErrorCodes()
        {
            return new Dictionary<string, string>
            {
                {"acqerror", "Xcalibur Acquisition Error"},
                {"overpressure", "Upper pressure limit exceeded"},
                {"underpressure", "Lower pressure limit exceeded"},
                {"notConnected", "Module is not connected"},
                {EventName.ErrorMessage.ToString(), "Xcalibur Error"},
                {EventName.ProgramError.ToString(), "Xcalibur Program Error"},
                {EventName.MethodCheckFail.ToString(), "Xcalibur Method Check Failed"},
                {"DS 8", "Device Status: Error"},
                {"DS 9", "Device Status: Busy"},
                {"DS 10", "Device Status: Not Connected"},
                {"DS 11", "Device Status: Standby"},
                {"DS 12", "Device Status: Off"},
                {"DS 13", "Device Status: Server Failed"},
                {"DS 15", "Device Status: Not Ready"},
            };
        }

        private static Dictionary<string, string> CreateStatusCodes()
        {
            return new Dictionary<string, string>
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
        }

        public virtual string GetMethodText(string methodName)
        {
            return "";
        }

        public IReadOnlyList<string> GetXcaliburDeviceNameList()
        {
            return xcaliburCom.GetDevices();
        }

        public string GetXcaliburDeviceNames()
        {
            var names = xcaliburCom.GetDevices();
            if (names.Count > 0)
            {
                return string.Join(", ", names);
            }

            return "No devices configured!";
        }

        public string GetXcaliburRunStatus()
        {
            try
            {
                return xcaliburCom.GetRunStatus();
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(LogLevel.Error, "Failed call to read Xcalibur status", ex);
                return "Error reading run status.";
            }
        }

        public string GetXcaliburDeviceStatus()
        {
            try
            {
                return xcaliburCom.GetDeviceStatus();
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(LogLevel.Error, "Failed call to read Xcalibur device status", ex);
                return ex.Message;
            }
        }

        public string GetXcaliburDeviceInfo()
        {
            try
            {
                return xcaliburCom.GetDeviceInfoString();
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(LogLevel.Error, "Failed call to read Xcalibur device info", ex);
                return ex.Message;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
