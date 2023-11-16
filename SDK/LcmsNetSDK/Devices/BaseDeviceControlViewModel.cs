using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace LcmsNetSDK.Devices
{
    public abstract class BaseDeviceControlViewModel : IDeviceControl, INotifyPropertyChangedExt
    {
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

        protected virtual void UpdateStatusDisplay(string message)
        {
            DeviceStatus = "Status: " + message;
        }

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
            get => Device?.Name ?? name;
            set => this.RaiseAndSetIfChanged(ref name, value);
        }

        /// <summary>
        /// Gets an instance of the default view for this view model
        /// </summary>
        /// <returns></returns>
        public abstract UserControl GetDefaultView();

        /// <summary>
        /// Used to disable conditional control without clearing selection states
        /// </summary>
        public virtual void OutOfView()
        { }

        /// <summary>
        /// Status of device, updated using UpdateStatusDisplay
        /// </summary>
        public string DeviceStatus
        {
            get => deviceStatus;
            private set => this.RaiseAndSetIfChanged(ref deviceStatus, value);
        }

        /// <summary>
        /// Used for conditional control (like key bindings) when the device control is visible
        /// </summary>
        public bool DeviceTabSelected
        {
            get => deviceTabSelected;
            set => this.RaiseAndSetIfChanged(ref deviceTabSelected, value);
        }

        private string name = "";
        private string deviceStatus = "";
        private bool deviceTabSelected = false;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
