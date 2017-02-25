using System;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using LcmsNet.Devices;
using LcmsNetDataClasses;
using LcmsNetDataClasses.Devices;
using LcmsNetDataClasses.Method;
using LcmsNetDataClasses.Logging;

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
            mstring_name = "Logger";
            menum_status = enumDeviceStatus.NotInitialized;
        }


        /// <summary>
        /// Returns the name of the device.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return mstring_name;
        }

        #region Members

        /// <summary>
        /// Name of the device.
        /// </summary>
        private string mstring_name;

        /// <summary>
        /// Status of the device currently.
        /// </summary>
        private enumDeviceStatus menum_status;

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
        [classLCMethodAttribute("Write Message", 3, false, "", -1, false)]
        public void DebugWrite(string message)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("\t\t{0} -- {1}", mstring_name, message));
            System.Diagnostics.Debug.Flush();
        }

        [classLCMethodAttribute("Update status!", 3, false, "", -1, false)]
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
            get { return mstring_name; }
            set { mstring_name = value; }
        }

        /// <summary>
        /// Version of the timer.
        /// </summary>
        public string Version
        {
            get { return ""; }
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
            get { return menum_status; }
            set
            {
                if (value != menum_status && StatusUpdate != null)
                    StatusUpdate(this, new classDeviceStatusEventArgs(value, "None", this));
                menum_status = value;
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
            Random rand = new Random();
            string data = "test" + rand.Next(0, 100).ToString();
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

        public enumDeviceType DeviceType
        {
            get { return enumDeviceType.BuiltIn; }
        }

        #endregion
    }
}