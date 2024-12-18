﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using LcmsNetSDK;
using LcmsNetSDK.Devices;
using LcmsNetSDK.Method;

namespace LcmsNet.Devices
{
    /// <summary>
    /// A debug logging device for testing.
    /// </summary>
    //[classDeviceMonitoring(enumDeviceMonitoringType.Message, "")]
    public class LogDevice : IDevice, IHasPerformanceData
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public LogDevice()
        {
            m_name = "Logger";
            m_status = DeviceStatus.NotInitialized;
        }


        /// <summary>
        /// Returns the name of the device.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return m_name;
        }

        /// <summary>
        /// Name of the device.
        /// </summary>
        private string m_name;

        /// <summary>
        /// Status of the device currently.
        /// </summary>
        private DeviceStatus m_status;

        /// <summary>
        /// Fired when the status changes.
        /// </summary>
        //public event DelegateDeviceStatusUpdate StatusUpdate;
        public event EventHandler<DeviceStatusEventArgs> StatusUpdate;

        /// <summary>
        /// Fired when an error occurs in the device.
        /// </summary>
        public event EventHandler<DeviceErrorEventArgs> Error
        {
            add { }
            remove { }
        }

        /// <summary>
        /// Fired when a property changes in the device.
        /// </summary>
        public event EventHandler DeviceSaveRequired
        {
            add { }
            remove { }
        }

        /// <summary>
        /// Prints the message provided to the debug console.
        /// </summary>
        /// <param name="message">Message to write.</param>
        [LCMethodEvent("Write Message", 3)]
        public void DebugWrite(string message)
        {
            Debug.WriteLine("\t\t{0} -- {1}", m_name, message);
            Debug.Flush();
        }

        [LCMethodEvent("Update status!", 3)]
        public void UpdateStatus()
        {
        }

        /// <summary>
        /// Gets or sets the abort event for scheduling.
        /// </summary>
        public ManualResetEvent AbortEvent { get; set; }

        /// <summary>
        /// Gets or sets whether the device is emulated or not.
        /// </summary>
        public bool Emulation { get; set; }

        /// <summary>
        /// Gets or sets the name of the device.
        /// </summary>
        public string Name
        {
            get => m_name;
            set => this.RaiseAndSetIfChanged(ref m_name, value);
        }

        /// <summary>
        /// Initializes the device.
        /// </summary>
        /// <returns></returns>
        public bool Initialize(ref string errorMessage)
        {
            return true;
        }

        /// <summary>
        /// Shuts off the timer.
        /// </summary>
        /// <returns></returns>
        public bool Shutdown()
        {
            return true;
        }

        /// <summary>
        /// Status of the device
        /// </summary>
        public DeviceStatus Status
        {
            get => m_status;
            set
            {
                if (value != m_status)
                    StatusUpdate?.Invoke(this, new DeviceStatusEventArgs(value, "None", this));
                m_status = value;
            }
        }

        /// <summary>
        /// Writes dummy data to the file.
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        public void WritePerformanceData(string directoryPath, string name, object[] parameters)
        {
            var rand = new Random();
            var data = "test" + rand.Next(0, 100);
            using (TextWriter writer = File.CreateText(Path.Combine(directoryPath, "test.txt")))
            {
                writer.WriteLine(data);
            }
        }

        public string GetPerformanceData(string methodName, object[] parameters)
        {
            return "";
        }

        public List<string> GetStatusNotificationList()
        {
            return new List<string>();
        }

        public List<string> GetErrorNotificationList()
        {
            return new List<string>();
        }

        public DeviceErrorStatus ErrorType { get; set; }

        public DeviceType DeviceType => DeviceType.BuiltIn;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
