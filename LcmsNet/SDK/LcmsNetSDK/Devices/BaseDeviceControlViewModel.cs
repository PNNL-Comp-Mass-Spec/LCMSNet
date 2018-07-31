﻿using System;
using System.ComponentModel;
using System.Windows.Controls;
using LcmsNetData;

namespace LcmsNetSDK.Devices
{
    public abstract class BaseDeviceControlViewModel : IDeviceControl, INotifyPropertyChangedExt
    {
        #region Methods

        protected BaseDeviceControlViewModel()
        {
            this.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Equals(nameof(Device)))
                {
                    Name = Device?.Name;
                }
            };
        }

        /// <summary>
        /// Sets the base device and updates the name text field.
        /// </summary>
        /// <param name="device"></param>
        protected void SetBaseDevice(IDevice device)
        {
            Name = device?.Name;
        }

        #endregion

        protected virtual void UpdateStatusDisplay(string message)
        {
            DeviceStatus = "Status: " + message;
        }

        #region Events

        /// <summary>
        /// An event that indicates the name has changed.
        /// </summary>
        public event EventHandler<string> NameChanged;

        public virtual void OnNameChanged(string newname)
        {
            NameChanged?.Invoke(this, newname);
        }

        /// <summary>
        /// An event that indicates the control needs to be saved
        /// </summary>
        public event Action SaveRequired;

        public virtual void OnSaveRequired()
        {
            SaveRequired?.Invoke();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether the device is currently running or not.
        /// </summary>
        public virtual bool Running { get; set; }

        /// <summary>
        /// The associated device.
        /// </summary>
        public abstract IDevice Device { get; set; }

        /// <summary>
        /// Gets or sets the name of the control
        /// </summary>
        public string Name
        {
            get { return Device?.Name ?? name; }
            set { this.RaiseAndSetIfChanged(ref name, value, nameof(Name)); }
        }

        /// <summary>
        /// Gets an instance of the default view for this view model
        /// </summary>
        /// <returns></returns>
        public abstract UserControl GetDefaultView();

        /// <summary>
        /// Status of device, updated using UpdateStatusDisplay
        /// </summary>
        public string DeviceStatus
        {
            get { return deviceStatus; }
            private set { this.RaiseAndSetIfChanged(ref deviceStatus, value, nameof(DeviceStatus)); }
        }

        private string name = "";
        private string deviceStatus = "";

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
