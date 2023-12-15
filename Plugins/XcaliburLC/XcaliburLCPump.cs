using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FluidicsSDK.Devices;
using LcmsNetSDK;
using LcmsNetSDK.Data;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Logging;
using LcmsNetSDK.Method;
using LcmsNetSDK.System;
using XcaliburControl;
using DeviceStatus = LcmsNetSDK.Devices.DeviceStatus;

namespace LcmsNetPlugins.XcaliburLC
{
    [Serializable]
    [DeviceControl(typeof(XcaliburLCViewModel),
        "Xcalibur LC",
        "Pumps")]
    public class XcaliburLCPump : XcaliburController, IDevice, IFluidicsPump, IHasDataProvider, IHasPerformanceData, IDisposable // TODO: maybe implement IPump?
    {
        /// <summary>
        /// The error message was not set.
        /// </summary>
        private const string CONST_DEFAULT_ERROR = "None";

        private const string CONST_PUMP_ERROR = "Pump Error";
        private const string CONST_INITIALIZE_ERROR = "Failed to Initialize";

        /// <summary>
        /// Default constructor - for use within LCMSNet
        /// </summary>
        public XcaliburLCPump() : base()
        {
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

            StatusChange += OnStatusChanged;
            ErrorReport += OnErrorReport;
            MethodNamesUpdated += OnMethodNamesUpdated;
        }

        /// <summary>
        /// The device's name.
        /// </summary>
        private string deviceName;

        /// <summary>
        /// Status strings from the pumps.
        /// </summary>
        private static string[] notificationStrings;

        /// <summary>
        /// Status of the device.
        /// </summary>
        private DeviceStatus deviceStatus;

        /// <summary>
        /// Indicates that a save is required in the Fluidics Designer
        /// </summary>
        public event EventHandler<DeviceStatusEventArgs> StatusUpdate;

        /// <summary>
        /// Fired when the device finds out what method names are available.
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
        /// Stops the currently running method.
        /// </summary>
        [LCMethodEvent("Stop Method", 1000)]
        public void StopMethod()
        {
            StopQueue();
        }

        /// <summary>
        /// Causes the software to wait for a "READY" response from the PAL before proceeding.
        /// </summary>
        /// <param name="timeout">The timeout value, in seconds.</param>
        /// <returns>Integer error code.</returns>
        [LCMethodEvent("Wait Until Ready", MethodOperationTimeoutType.Parameter, EventDescription = "Wait until Xcalibur is reporting state \"Ready To Download\".\nDeterministic, next step will not be started until timeout is reached")]
        [LCMethodEvent("Wait Until Ready NonDeterm", MethodOperationTimeoutType.Parameter, IgnoreLeftoverTime = true, EventDescription = "Wait until the Xcalibur is reporting state \"Ready To Download\".\nNon-deterministic, will not wait for the end of the timeout before starting the next step")]
        public new bool WaitUntilReadyToDownload(double timeout)
        {
            return base.WaitUntilReadyToDownload(timeout);
        }

        /// <summary>
        /// Wait until error, synchronization point, or ready
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        [LCMethodEvent("Wait for 'Waiting for Contact Closure'", MethodOperationTimeoutType.Parameter, EventDescription = "Wait for the Xcalibur device to report 'Waiting for Contact Closure'.\nDeterministic, next step will not be started until timeout is reached")]
        [LCMethodEvent("Wait for 'Waiting for Contact Closure' NonDeterm", MethodOperationTimeoutType.Parameter, IgnoreLeftoverTime = true, EventDescription = "Wait for the Xcalibur device to report 'Waiting for Contact Closure'.\nNon-deterministic, will not wait for the end of the timeout before starting the next step")]
        public new bool WaitUntilWaitingForContactClosure(double timeout)
        {
            return base.WaitUntilWaitingForContactClosure(timeout);
        }

        /// <summary>
        /// Wait until error, synchronization point, or ready
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        [LCMethodEvent("Wait for Run Complete", MethodOperationTimeoutType.Parameter, EventDescription = "Wait for the Xcalibur run to complete.\nDeterministic, next step will not be started until timeout is reached")]
        [LCMethodEvent("Wait for Run Complete NonDeterm", MethodOperationTimeoutType.Parameter, IgnoreLeftoverTime = true, EventDescription = "Wait for the Xcalibur run to complete.\nNon-deterministic, will not wait for the end of the timeout before starting the next step")]
        public new bool WaitUntilRunComplete(double timeout)
        {
            return base.WaitUntilRunComplete(timeout);
        }

        /// <summary>
        /// Lists the methods
        /// </summary>
        private void ListMethods()
        {
            if (MethodNames != null)
            {
                var data = AvailableMethods.Keys.Cast<object>().ToList();

                MethodNames(this, data);
            }
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

        public override string GetMethodText(string methodName)
        {
            if (!AvailableMethods.TryGetValue(methodName, out var methodPath))
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
                    if (parameters != null && parameters.Length > 2 && AvailableMethods.TryGetValue((string)parameters[2], out var methodPath))
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

            foreach (var value in StatusCodes.Values)
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

            foreach (var value in ErrorCodes.Values)
            {
                notifications.Add(value);
            }

            return notifications;
        }

        private void OnStatusChanged(object sender, XcaliburStatusEventArgs args)
        {
            StatusUpdate?.Invoke(this, new DeviceStatusEventArgs(Status, args.MessageHeader, this, args.Message));
        }

        public void OnErrorReport(object sender, XcaliburErrorEventArgs args)
        {
            Error?.Invoke(this, new DeviceErrorEventArgs(args.Message, args.Exception, DeviceErrorStatus.ErrorAffectsAllColumns, this, args.MessageHeader));
        }

        private void OnMethodNamesUpdated(object sender, IEnumerable<string> methods)
        {
            MethodNames?.Invoke(this, methods.Cast<object>().ToList());
        }

        /// <summary>
        /// Returns the name of the device.
        /// </summary>
        /// <returns>String name of the device.</returns>
        public override string ToString()
        {
            return Name;
        }

        public double GetPressure()
        {
            return -1;
        }

        public double GetFlowRate()
        {
            return -1;
        }

        public double GetPercentB()
        {
            return -1;
        }

        public double GetActualFlow()
        {
            return -1;
        }
    }
}
