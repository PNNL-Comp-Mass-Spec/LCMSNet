using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Method;
using LcmsNetSDK;

namespace LcmsNet.Devices
{
    /// <summary>
    /// A debug logging device for testing.
    /// </summary>
    //[classDeviceMonitoring(enumDeviceMonitoringType.Message, "")]
    public class classLogDevice : IDevice
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public classLogDevice()
        {
            m_name = "Logger";
            m_status = enumDeviceStatus.NotInitialized;
        }


        /// <summary>
        /// Returns the name of the device.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return m_name;
        }

        #region Members

        /// <summary>
        /// Name of the device.
        /// </summary>
        private string m_name;

        /// <summary>
        /// Status of the device currently.
        /// </summary>
        private enumDeviceStatus m_status;

        #endregion

        #region Events

        /// <summary>
        /// Fired when the status changes.
        /// </summary>
        //public event DelegateDeviceStatusUpdate StatusUpdate;
        public event EventHandler<classDeviceStatusEventArgs> StatusUpdate;

        /// <summary>
        /// Fired when an error occurs in the device.
        /// </summary>
        public event EventHandler<classDeviceErrorEventArgs> Error
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

        #endregion

        #region LC-Method Registered Methods

        /// <summary>
        /// Prints the message provided to the debug console.
        /// </summary>
        /// <param name="message">Message to write.</param>
        [classLCMethod("Write Message", 3, false, "", -1, false)]
        public void DebugWrite(string message)
        {
            Debug.WriteLine("\t\t{0} -- {1}", m_name, message);
            Debug.Flush();
        }

        [classLCMethod("Update status!", 3, false, "", -1, false)]
        public void UpdateStatus()
        {
        }

        #endregion

        /*public Finch.Data.FinchComponentData GetData()
        {
            return null;
        }*/

        #region IDevice Data Provider Methods

        public void RegisterDataProvider(string key, DelegateDeviceHasData remoteMethod)
        {
        }

        public void UnRegisterDataProvider(string key, DelegateDeviceHasData remoteMethod)
        {
        }

        #endregion

        #region IDevice Members

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
            get { return m_name; }
            set { this.RaiseAndSetIfChanged(ref m_name, value); }
        }

        /// <summary>
        /// Version of the timer.
        /// </summary>
        public string Version
        {
            get { return string.Empty; }
            set
            {
                // Pass
            }
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
        public enumDeviceStatus Status
        {
            get { return m_status; }
            set
            {
                if (value != m_status)
                    StatusUpdate?.Invoke(this, new classDeviceStatusEventArgs(value, "None", this));
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

        public List<string> GetStatusNotificationList()
        {
            return new List<string>();
        }

        public List<string> GetErrorNotificationList()
        {
            return new List<string>();
        }

        #endregion

        #region IDevice Members

        public enumDeviceErrorStatus ErrorType { get; set; }

        public enumDeviceType DeviceType => enumDeviceType.BuiltIn;

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}